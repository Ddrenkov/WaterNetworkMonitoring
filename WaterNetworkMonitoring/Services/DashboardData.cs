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
        public List<Repair> Repairs { get; init; } = new();

        public Repair? LatestCleanWaterRepair(Month month)
        {
            var end = new DateTime(month.Year, month.Number, 1).AddMonths(1);
            return Repairs.Where(r => r.WaterSystem == WaterSystem.CleanWater && r.Date < end)
                .OrderByDescending(r => r.Date).ThenByDescending(r => r.Id).FirstOrDefault();
        }

        internal sealed record MeterScope(int LoggerId, string Name, int MeterCount)
        {
            public override string ToString() => $"{Name} · {MeterCount} {(MeterCount == 1 ? "meter" : "meters")}";
        }
        internal sealed record MeterComparison(MeterScope Scope, IReadOnlyList<BranchBalance> Balances, bool Estimated, string? Note);

        private List<WaterMeter> SuppliedMeters(PressureLogger logger)
        {
            var loggerIds = new HashSet<int> { logger.Id };
            var pending = new Queue<int>();
            pending.Enqueue(logger.Id);
            while (pending.TryDequeue(out int id))
                foreach (var child in Loggers.Where(l => l.ParentLoggerId == id))
                    if (loggerIds.Add(child.Id)) pending.Enqueue(child.Id);
            return Meters.Where(m => m.LoggerId.HasValue && loggerIds.Contains(m.LoggerId.Value)).ToList();
        }

        public List<MeterScope> MeterScopes => Loggers.OrderBy(l => l.Id)
            .Select(l => new MeterScope(l.Id, l.Name, SuppliedMeters(l).Count)).ToList();

        public MeterComparison CompareMeters(int loggerId, Month month)
        {
            var logger = Loggers.Single(l => l.Id == loggerId);
            var scope = MeterScopes.Single(s => s.LoggerId == loggerId);
            var meters = SuppliedMeters(logger);
            var start = new DateTime(month.Year, month.Number, 1);
            var end = start.AddMonths(1);
            try
            {
                var comparisonReadings = ComparisonReadings(logger);
                var baselineEnd = start;
                if (comparisonReadings.Count > 0)
                {
                    var firstReading = comparisonReadings.Where(r => r.LoggerId == logger.Id)
                        .Select(r => r.TakenAt).DefaultIfEmpty(start).Min();
                    baselineEnd = firstReading.AddDays(7);
                }
                var balances = LeakDetectionService.AnalyzeMeterBalance(logger, comparisonReadings, meters,
                    MeterReadings, baselineEnd, requireBaseline: false)
                    .Where(b => b.To > start && b.To <= end).ToList();
                bool estimated = HasEstimatedCounters(balances, meters);
                return new MeterComparison(scope, balances, estimated,
                    balances.Count == 0 ? "No complete matching logger and meter intervals." : null);
            }
            catch (ArgumentException)
            {
                return new MeterComparison(scope, Array.Empty<BranchBalance>(), false,
                    "Matching logger and meter readings are needed.");
            }
        }

        public List<Month> Months => LoggerReadings
            .Select(reading => IntervalDate(reading.TakenAt))
            .GroupBy(date => new { date.Year, date.Month })
            .Select(group => new Month(group.Key.Year, group.Key.Month, group.Count()))
            .OrderByDescending(month => month.Year).ThenByDescending(month => month.Number).ToList();

        public static async Task<DashboardData> LoadAsync(CancellationToken cancellationToken)
        {
            using var db = new AppDbContext();
            var connection = new SqlConnectionStringBuilder(db.Database.GetConnectionString())
                { ConnectTimeout = 5, ConnectRetryCount = 0 };
            db.Database.SetConnectionString(connection.ConnectionString);
            db.Database.SetCommandTimeout(10);
            var loggers = await db.Loggers.AsNoTracking().Where(l => l.ZoneName == "Zapad" && l.WaterSystem == WaterSystem.CleanWater)
                .ToListAsync(cancellationToken);
            var ids = loggers.Select(l => l.Id).ToArray();
            // Load three calendar months, rather than cutting a month at an arbitrary row count.
            var latest = await db.LoggerReadings.AsNoTracking().Where(r => ids.Contains(r.LoggerId))
                .OrderByDescending(r => r.TakenAt).Select(r => (DateTime?)r.TakenAt)
                .FirstOrDefaultAsync(cancellationToken);
            var readings = new List<LoggerReading>();
            if (latest.HasValue)
            {
                var date = IntervalDate(latest.Value);
                var from = new DateTime(date.Year, date.Month, 1).AddMonths(-2).AddDays(-1);
                readings = await db.LoggerReadings.AsNoTracking()
                    .Where(r => ids.Contains(r.LoggerId) && r.TakenAt > from && r.TakenAt <= latest.Value)
                    .OrderBy(r => r.TakenAt).ToListAsync(cancellationToken);
            }
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
            var repairs = await db.Repairs.AsNoTracking().Where(r => r.WaterSystem == WaterSystem.CleanWater)
                .OrderByDescending(r => r.Date).ToListAsync(cancellationToken);
            return new DashboardData { Loggers = loggers, LoggerReadings = readings,
                MeterReadings = meterReadings, Meters = meters, Repairs = repairs };
        }

        public DashboardReport Report(Month month)
        {
            var start = new DateTime(month.Year, month.Number, 1);
            var end = start.AddMonths(1);
            var readings = LoggerReadings.Where(r => r.TakenAt > start && r.TakenAt <= end).ToList();
            var inlet = Loggers.FirstOrDefault(l => l.ParentLoggerId == null);
            var inletReadings = readings.Where(r => r.LoggerId == inlet?.Id).ToList();
            var baselineEnd = LoggerReadings.Count == 0 ? start : LoggerReadings.Min(r => r.TakenAt).AddDays(7);
            var analyses = AnalyzeLoggers(baselineEnd, out int incomplete);
            var alert = FindLatestLeak(analyses, start, end);
            var selected = SelectLogger(alert.Logger, inlet);
            var allBalances = analyses.FirstOrDefault(a => a.Logger.Id == selected?.Id).Balances ?? new List<BranchBalance>();
            var monthBalances = allBalances.Where(b => b.To > start && b.To <= end).ToList();
            var latestLeak = alert.Balance;
            var firstLeak = latestLeak == null ? null : allBalances.FirstOrDefault(b => b.LeakSuspected && b.To <= latestLeak.To);
            var assigned = Meters.Where(m => m.LoggerId == selected?.Id).ToList();
            var singleMeter = assigned.Count == 1 ? assigned[0] : null;
            bool complete = CompleteMonth(month, inletReadings);
            string location = DescribeLeak(selected, latestLeak, assigned);
            return new DashboardReport
            {
                Period = month.ToString(),
                IsSimulated = readings.Any(reading => reading.IsSimulated),
                InletDays = Daily(inletReadings, reading => reading.FlowM3PerHour),
                InletPressure = Daily(inletReadings, reading => reading.PressureBar),
                BranchPressure = Daily(readings.Where(reading => reading.LoggerId == selected?.Id).ToList(), reading => reading.PressureBar),
                FlowSeries = CreateSeries(readings, reading => reading.FlowM3PerHour),
                PressureSeries = CreateSeries(readings, reading => reading.PressureBar),
                Balances = monthBalances,
                LatestLeak = latestLeak,
                FirstLeak = firstLeak,
                BranchName = selected?.Name ?? "Branch",
                BuildingNumber = singleMeter?.MapNumber,
                MeterId = singleMeter?.Id,
                LeakLocation = location,
                FlaggedLoggerId = latestLeak == null ? null : selected?.Id,
                LoggerSummary = "Loggers: " + string.Join(" · ", Loggers.OrderBy(logger => logger.Id)
                    .Select(logger => $"{LoggerMapPin.LabelFor(logger.Id)} ({logger.Id})")),
                AnalysisNote = latestLeak == null && (monthBalances.Count == 0 || incomplete > 0)
                    ? "Not enough matching readings to assess every branch." : null,
                SuppliedM3 = inletReadings.Sum(reading => reading.FlowM3PerHour * .25m),
                CompleteMonth = complete,
                LastReading = readings.Count == 0 ? null : readings.Max(reading => reading.TakenAt)
            };
        }

        private List<LoggerReading> ComparisonReadings(PressureLogger logger)
        {
            if (logger.ParentLoggerId != null) return LoggerReadings;

            // The inlet comparison needs a shared observation period for all its branches.
            DateTime from = DateTime.MinValue, to = DateTime.MaxValue;
            var monitored = Loggers.Where(item => item.Id == logger.Id || item.ParentLoggerId == logger.Id);
            foreach (var item in monitored)
            {
                var rows = LoggerReadings.Where(reading => reading.LoggerId == item.Id).ToList();
                if (rows.Count == 0) throw new ArgumentException("A branch has no observations.");
                var first = rows.Min(reading => reading.TakenAt);
                var last = rows.Max(reading => reading.TakenAt);
                if (first > from) from = first;
                if (last < to) to = last;
            }
            return LoggerReadings.Where(reading => reading.TakenAt >= from && reading.TakenAt <= to).ToList();
        }

        private bool HasEstimatedCounters(List<BranchBalance> balances, List<WaterMeter> meters)
        {
            var recorded = MeterReadings.GroupBy(reading => reading.WaterMeterId)
                .ToDictionary(group => group.Key, group => group.Select(reading => reading.TakenAt).ToHashSet());
            foreach (var balance in balances)
                foreach (var meter in meters)
                    if (!recorded.TryGetValue(meter.Id, out var times) ||
                        !times.Contains(balance.From) || !times.Contains(balance.To))
                        return true;
            return false;
        }

        private List<(PressureLogger Logger, List<BranchBalance> Balances)> AnalyzeLoggers(
            DateTime baselineEnd, out int incomplete)
        {
            var analyses = new List<(PressureLogger Logger, List<BranchBalance> Balances)>();
            incomplete = 0;
            foreach (var logger in Loggers)
            {
                try
                {
                    IReadOnlyList<BranchBalance> balances;
                    bool hasBranches = Loggers.Any(child => child.ParentLoggerId == logger.Id);
                    if (hasBranches)
                        balances = LeakDetectionService.AnalyzeInlet(logger, Loggers, LoggerReadings, baselineEnd);
                    else
                        balances = LeakDetectionService.AnalyzeSubzone(logger, LoggerReadings, Meters, MeterReadings, baselineEnd);
                    analyses.Add((logger, balances.ToList()));
                }
                catch (ArgumentException)
                {
                    incomplete++;
                }
            }
            return analyses;
        }

        private static (PressureLogger? Logger, BranchBalance? Balance) FindLatestLeak(
            List<(PressureLogger Logger, List<BranchBalance> Balances)> analyses, DateTime start, DateTime end)
        {
            PressureLogger? logger = null;
            BranchBalance? latest = null;
            foreach (var analysis in analyses)
                foreach (var balance in analysis.Balances)
                {
                    if (!balance.LeakSuspected || balance.To <= start || balance.To > end) continue;
                    if (latest == null || balance.To > latest.To ||
                        (balance.To == latest.To && balance.ExcessM3PerHour > latest.ExcessM3PerHour))
                    {
                        logger = analysis.Logger;
                        latest = balance;
                    }
                }
            return (logger, latest);
        }

        private PressureLogger? SelectLogger(PressureLogger? alertLogger, PressureLogger? inlet)
        {
            if (alertLogger != null) return alertLogger;
            var logger = Loggers.FirstOrDefault(item => item.SuppliedWaterMeterId.HasValue);
            if (logger != null) return logger;
            logger = Loggers.FirstOrDefault(item => item.ParentLoggerId.HasValue);
            if (logger != null) return logger;
            return inlet;
        }

        private string DescribeLeak(PressureLogger? logger, BranchBalance? leak, List<WaterMeter> meters)
        {
            if (logger == null || leak == null) return "";
            if (logger.ParentLoggerId == null)
            {
                int branches = Loggers.Count(child => child.ParentLoggerId == logger.Id);
                return $"Suspected leak: between logger {logger.Id} and its {branches} downstream loggers. Exact point unknown.";
            }
            if (meters.Count == 1)
                return $"Suspected leak: between logger {logger.Id} and meter {meters[0].Id}. Exact point unknown.";
            return $"Suspected leak: downstream of logger {logger.Id}, serving {meters.Count} buildings. Exact point unknown.";
        }

        private ChartSeries[] CreateSeries(List<LoggerReading> readings, Func<LoggerReading, decimal> value)
        {
            var series = new List<ChartSeries>();
            foreach (var logger in Loggers.OrderBy(item => item.Id))
            {
                var rows = readings.Where(reading => reading.LoggerId == logger.Id).ToList();
                if (rows.Count == 0) continue;
                series.Add(new ChartSeries(LoggerMapPin.LabelFor(logger.Id), LoggerMapPin.ColorFor(logger.Id),
                    Daily(rows, value), TimeSpan.FromHours(26)));
            }
            return series.ToArray();
        }

        private static bool CompleteMonth(Month month, List<LoggerReading> readings)
        {
            var days = readings.GroupBy(reading => IntervalDate(reading.TakenAt)).ToArray();
            if (days.Length != DateTime.DaysInMonth(month.Year, month.Number)) return false;
            foreach (var day in days)
                if (!CompleteDay(day.Key, day.ToList())) return false;
            return true;
        }

        private static List<ChartPoint> Daily(List<LoggerReading> rows, Func<LoggerReading, decimal> value) =>
            rows.GroupBy(r => IntervalDate(r.TakenAt)).OrderBy(g => g.Key)
                .Select(g => new ChartPoint(g.Key.ToDateTime(new TimeOnly(12, 0)), (double)g.Average(value))).ToList();

        private static bool CompleteDay(DateOnly day, List<LoggerReading> readings)
        {
            var from = day.ToDateTime(TimeOnly.MinValue);
            var to = day.AddDays(1).ToDateTime(TimeOnly.MinValue);
            var ordered = readings.OrderBy(r => r.TakenAt).ToArray();
            if (ordered.Length != (to - from).TotalMinutes / 15) return false;
            for (int i = 0; i < ordered.Length; i++)
                if (ordered[i].TakenAt != from.AddMinutes((i + 1) * 15)) return false;
            return true;
        }

        // Each logger reading describes the preceding 15 minutes.
        private static DateOnly IntervalDate(DateTime timestamp) => DateOnly.FromDateTime(timestamp.AddTicks(-1));
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
        public DateTime? LastReading { get; init; }
    }
}
