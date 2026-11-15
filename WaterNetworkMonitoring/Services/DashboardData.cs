using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WaterNetworkMonitoring.Data;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Services
{
    internal sealed class DashboardData
    {
        internal sealed record Month(int Year, int Number, int Count)
        {
            public override string ToString() => new DateTime(Year, Number, 1).ToString("MMMM yyyy");
        }

        public List<PressureLogger> Loggers { get; init; } = new();
        public List<LoggerReading> LoggerReadings { get; init; } = new();
        public List<WaterMeterReading> MeterReadings { get; init; } = new();
        public List<WaterMeter> Meters { get; init; } = new();
        public List<Month> Months => LoggerReadings.GroupBy(r => new { r.TakenAt.Year, r.TakenAt.Month })
            .Select(g => new Month(g.Key.Year, g.Key.Month, g.Count()))
            .OrderByDescending(m => m.Year).ThenByDescending(m => m.Number).ToList();

        public static async Task<DashboardData> LoadAsync(CancellationToken cancellationToken)
        {
            using var db = new AppDbContext();
            var connection = new SqlConnectionStringBuilder(db.Database.GetConnectionString())
                { ConnectTimeout = 5, ConnectRetryCount = 0 };
            db.Database.SetConnectionString(connection.ConnectionString);
            db.Database.SetCommandTimeout(10);
            var loggers = await db.Loggers.AsNoTracking().Where(l => l.ZoneName == "Zapad")
                .ToListAsync(cancellationToken);
            var ids = loggers.Select(l => l.Id).ToArray();
            // Bound the dashboard query as history grows; the sample month fits comfortably.
            var readings = await db.LoggerReadings.AsNoTracking().Where(r => ids.Contains(r.LoggerId))
                .OrderByDescending(r => r.TakenAt).Take(20000).ToListAsync(cancellationToken);
            var meters = await db.WaterMeters.AsNoTracking().OrderBy(m => m.MapNumber).ToListAsync(cancellationToken);
            var meterIds = meters.Where(m => m.LoggerId.HasValue).Select(m => m.Id).ToArray();
            var meterReadings = new List<WaterMeterReading>();
            if (readings.Count > 0)
            {
                var from = readings.Min(r => r.TakenAt).AddDays(-1);
                var to = readings.Max(r => r.TakenAt).AddDays(1);
                meterReadings = await db.WaterMeterReadings.AsNoTracking()
                    .Where(r => meterIds.Contains(r.WaterMeterId) && r.TakenAt >= from && r.TakenAt <= to)
                    .OrderBy(r => r.TakenAt).ToListAsync(cancellationToken);
            }
            return new DashboardData { Loggers = loggers, LoggerReadings = readings,
                MeterReadings = meterReadings, Meters = meters };
        }

        public DashboardReport Report(Month month)
        {
            var start = Local(new DateTime(month.Year, month.Number, 1));
            var end = Local(new DateTime(month.Year, month.Number, 1).AddMonths(1));
            var readings = LoggerReadings.Where(r => r.TakenAt > start && r.TakenAt <= end).ToList();
            var inlet = Loggers.FirstOrDefault(l => l.ParentLoggerId == null);
            var inletReadings = readings.Where(r => r.LoggerId == inlet?.Id).ToList();
            var baselineEnd = LoggerReadings.Count == 0 ? start : LoggerReadings.Min(r => r.TakenAt).AddDays(7);
            var analyses = new List<(Models.PressureLogger Logger, List<BranchBalance> Balances)>();
            int incomplete = 0;
            foreach (var logger in Loggers)
            {
                try
                {
                    var balances = logger.ParentLoggerId == null
                        ? LeakDetectionService.AnalyzeInlet(logger, Loggers, LoggerReadings, baselineEnd)
                        : LeakDetectionService.AnalyzeSubzone(logger, LoggerReadings, Meters, MeterReadings, baselineEnd);
                    analyses.Add((logger, balances.ToList()));
                }
                catch (ArgumentException) { incomplete++; }
            }
            var alert = analyses.SelectMany(a => a.Balances.Where(b => b.To > start && b.To <= end && b.LeakSuspected)
                .Select(b => (a.Logger, Balance:b))).OrderByDescending(a => a.Balance.To)
                .ThenByDescending(a => a.Balance.ExcessM3PerHour).FirstOrDefault();
            var selected = alert.Logger ?? Loggers.FirstOrDefault(l => l.SuppliedWaterMeterId.HasValue)
                ?? Loggers.FirstOrDefault(l => l.ParentLoggerId.HasValue) ?? inlet;
            var allBalances = analyses.FirstOrDefault(a => a.Logger.Id == selected?.Id).Balances ?? new List<BranchBalance>();
            var monthBalances = allBalances.Where(b => b.To > start && b.To <= end).ToList();
            var latestLeak = alert.Logger == null ? null : alert.Balance;
            var firstLeak = latestLeak == null ? null : allBalances.FirstOrDefault(b => b.LeakSuspected && b.To <= latestLeak.To);
            var assigned = Meters.Where(m => m.LoggerId == selected?.Id).ToList();
            var singleMeter = assigned.Count == 1 ? assigned[0] : null;
            var inletDays = inletReadings.GroupBy(r => SofiaDate(r.TakenAt.AddTicks(-1))).ToArray();
            bool complete = inletDays.Length == DateTime.DaysInMonth(month.Year, month.Number) &&
                inletDays.All(g => CompleteDay(g.Key, g.ToList()));
            string location = "";
            if (latestLeak != null && selected != null)
                location = selected.ParentLoggerId == null
                    ? $"Suspected leak: between logger {selected.Id} and its three downstream loggers. Exact point unknown."
                    : singleMeter != null
                        ? $"Suspected leak: between logger {selected.Id} and meter {singleMeter.Id}. Exact point unknown."
                        : $"Suspected leak: downstream of logger {selected.Id}, serving {assigned.Count} buildings. Exact point unknown.";
            ChartSeries[] Series(Func<Models.LoggerReading,decimal> value) => Loggers.OrderBy(l => l.Id)
                .Where(l => readings.Any(r => r.LoggerId == l.Id))
                .Select(l => new ChartSeries(LoggerMapPin.LabelFor(l.Id), LoggerMapPin.ColorFor(l.Id),
                    Daily(readings.Where(r => r.LoggerId == l.Id).ToList(),value), TimeSpan.FromHours(26))).ToArray();
            return new DashboardReport
            {
                Period=month.ToString(), IsSimulated=readings.Any(r => r.IsSimulated),
                InletDays=Daily(inletReadings,r => r.FlowM3PerHour),
                InletPressure=Daily(inletReadings,r => r.PressureBar),
                BranchPressure=Daily(readings.Where(r => r.LoggerId == selected?.Id).ToList(),r => r.PressureBar),
                FlowSeries=Series(r => r.FlowM3PerHour), PressureSeries=Series(r => r.PressureBar),
                Balances=monthBalances, LatestLeak=latestLeak, FirstLeak=firstLeak,
                BranchName=selected?.Name ?? "Branch", BuildingNumber=singleMeter?.MapNumber, MeterId=singleMeter?.Id,
                LeakLocation=location, FlaggedLoggerId=latestLeak == null ? null : selected?.Id,
                LoggerSummary="Loggers: "+string.Join(" · ",Loggers.OrderBy(l => l.Id).Select(l => $"{LoggerMapPin.LabelFor(l.Id)} ({l.Id})")),
                AnalysisNote=latestLeak == null && (monthBalances.Count == 0 || incomplete > 0)
                    ? "Not enough matching readings to assess every branch." : null,
                SuppliedM3=inletReadings.Sum(r => r.FlowM3PerHour*.25m), CompleteMonth=complete,
                LastReading=readings.Count == 0 ? null : readings.Max(r => r.TakenAt)
            };
        }

        private static List<ChartPoint> Daily(List<LoggerReading> rows, Func<LoggerReading, decimal> value) =>
            rows.GroupBy(r => SofiaDate(r.TakenAt.AddTicks(-1))).OrderBy(g => g.Key)
                .Select(g => new ChartPoint(Local(g.Key.ToDateTime(new TimeOnly(12, 0))), (double)g.Average(value))).ToList();

        private static bool CompleteDay(DateOnly day, List<LoggerReading> readings)
        {
            var from = Local(day.ToDateTime(TimeOnly.MinValue));
            var to = Local(day.AddDays(1).ToDateTime(TimeOnly.MinValue));
            var ordered = readings.OrderBy(r => r.TakenAt).ToArray();
            return ordered.Length == (to - from).TotalMinutes / 15 &&
                ordered.Select((r, i) => r.TakenAt == from.AddMinutes((i + 1) * 15)).All(b => b);
        }

        private static readonly TimeZoneInfo Sofia = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
        private static DateOnly SofiaDate(DateTimeOffset timestamp) => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(timestamp, Sofia).DateTime);
        private static DateTimeOffset Local(DateTime local) => new(local, Sofia.GetUtcOffset(local));
    }

    internal sealed class DashboardReport
    {
        public string Period { get; init; } = "";
        public bool IsSimulated { get; init; }
        public List<ChartPoint> InletDays { get; init; } = new();
        public List<ChartPoint> InletPressure { get; init; } = new();
        public List<ChartPoint> BranchPressure { get; init; } = new();
        public ChartSeries[] FlowSeries { get; init; } = Array.Empty<ChartSeries>();
        public ChartSeries[] PressureSeries { get; init; } = Array.Empty<ChartSeries>();
        public int? FlaggedLoggerId { get; init; }
        public List<BranchBalance> Balances { get; init; } = new();
        public BranchBalance? LatestLeak { get; init; }
        public BranchBalance? FirstLeak { get; init; }
        public string BranchName { get; init; } = "";
        public string LoggerSummary { get; init; } = "";
        public string LeakLocation { get; init; } = "";
        public int? BuildingNumber { get; init; }
        public int? MeterId { get; init; }
        public string? AnalysisNote { get; init; }
        public decimal SuppliedM3 { get; init; }
        public bool CompleteMonth { get; init; }
        public DateTimeOffset? LastReading { get; init; }
    }
}
