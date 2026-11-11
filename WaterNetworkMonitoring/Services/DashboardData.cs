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
            var branchIds = loggers.Where(l => l.SuppliedWaterMeterId.HasValue)
                .Select(l => l.SuppliedWaterMeterId!.Value).ToArray();
            var meterReadings = new List<WaterMeterReading>();
            if (readings.Count > 0)
            {
                var from = readings.Min(r => r.TakenAt).AddDays(-1);
                var to = readings.Max(r => r.TakenAt).AddDays(1);
                meterReadings = await db.WaterMeterReadings.AsNoTracking()
                    .Where(r => branchIds.Contains(r.WaterMeterId) && r.TakenAt >= from && r.TakenAt <= to)
                    .OrderBy(r => r.TakenAt).ToListAsync(cancellationToken);
            }
            return new DashboardData { Loggers = loggers, LoggerReadings = readings,
                MeterReadings = meterReadings, Meters = meters };
        }

        public DashboardReport Report(Month month)
        {
            var start = Local(new DateTime(month.Year, month.Number, 1));
            var end = Local(new DateTime(month.Year, month.Number, 1).AddMonths(1));
            // A reading at midnight describes the quarter hour at the end of the previous day.
            var readings = LoggerReadings.Where(r => r.TakenAt > start && r.TakenAt <= end).ToList();
            var inlet = Loggers.FirstOrDefault(l => l.ParentLoggerId == null && l.SuppliedWaterMeterId == null);
            var branch = Loggers.FirstOrDefault(l => l.SuppliedWaterMeterId.HasValue);
            var inletReadings = readings.Where(r => r.LoggerId == inlet?.Id).ToList();
            var branchReadings = readings.Where(r => r.LoggerId == branch?.Id).ToList();
            var balances = new List<BranchBalance>();
            string? analysisNote = null;
            if (branch != null)
            {
                var branchMeters = MeterReadings.Where(r => r.WaterMeterId == branch.SuppliedWaterMeterId)
                    .OrderBy(r => r.TakenAt).ToList();
                if (branchMeters.Count > 0)
                {
                    // Prototype baseline: the first seven days must be known normal readings.
                    var baselineEnd = branchMeters[0].TakenAt.AddDays(7);
                    try
                    {
                        balances = LeakDetectionService.AnalyzeBranch(branch, LoggerReadings, branchMeters, baselineEnd).ToList();
                    }
                    catch (ArgumentException)
                    {
                        analysisNote = "Not enough valid, matching readings to assess a leak.";
                    }
                }
            }
            var monthBalances = balances.Where(b => b.To > start && b.To <= end).ToList();
            if (monthBalances.Count == 0) analysisNote ??= "No complete branch readings are available for this period.";
            var latestLeak = monthBalances.LastOrDefault(b => b.LeakSuspected);
            var firstLeak = latestLeak == null ? null : balances.FirstOrDefault(b => b.LeakSuspected && b.To <= latestLeak.To);
            int? building = Meters.FirstOrDefault(m => m.Id == branch?.SuppliedWaterMeterId)?.MapNumber;
            var inletDays = inletReadings.GroupBy(r => SofiaDate(r.TakenAt.AddTicks(-1))).ToArray();
            bool complete = inletDays.Length == DateTime.DaysInMonth(month.Year, month.Number) &&
                inletDays.All(g => CompleteDay(g.Key, g.ToList()));
            return new DashboardReport
            {
                Period = month.ToString(), IsSimulated = readings.Any(r => r.IsSimulated),
                InletDays = Daily(inletReadings, r => r.FlowM3PerHour),
                InletPressure = Daily(inletReadings, r => r.PressureBar),
                BranchPressure = Daily(branchReadings, r => r.PressureBar),
                Balances = monthBalances, LatestLeak = latestLeak, FirstLeak = firstLeak,
                BranchName = branch?.Name ?? "Branch", BuildingNumber = building,
                MeterId = branch?.SuppliedWaterMeterId, AnalysisNote = analysisNote,
                SuppliedM3 = inletReadings.Sum(r => r.FlowM3PerHour * 0.25m), CompleteMonth = complete,
                LastReading = readings.Count > 0 ? readings.Max(r => r.TakenAt) : null
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
        public List<BranchBalance> Balances { get; init; } = new();
        public BranchBalance? LatestLeak { get; init; }
        public BranchBalance? FirstLeak { get; init; }
        public string BranchName { get; init; } = "";
        public int? BuildingNumber { get; init; }
        public int? MeterId { get; init; }
        public string? AnalysisNote { get; init; }
        public decimal SuppliedM3 { get; init; }
        public bool CompleteMonth { get; init; }
        public DateTimeOffset? LastReading { get; init; }
    }
}
