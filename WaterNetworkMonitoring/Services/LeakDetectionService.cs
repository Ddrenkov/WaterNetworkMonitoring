using System.Globalization;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Services
{
    public sealed record BranchBalance(DateTime From, DateTime To,
        decimal SuppliedM3, decimal ConsumedM3, decimal ExcessM3PerHour,
        decimal Cusum, bool LeakSuspected, bool PressureDrop)
    {
        public decimal DetectionThreshold { get; init; }
    }

    public static class LeakDetectionService
    {
        private const int IntervalMinutes = 15;
        private const decimal IntervalHours = IntervalMinutes / 60m;
        private const int MinimumBaselineCount = 10;

        public static IReadOnlyList<BranchBalance> AnalyzeBranch(PressureLogger branch,
            IEnumerable<LoggerReading> loggerReadings, IEnumerable<WaterMeterReading> meterReadings,
            DateTime baselineEnd)
        {
            if (branch.SuppliedWaterMeterId is not int id)
                throw new ArgumentException("Choose a logger with an assigned meter.");
            var meter = new WaterMeter { Id = id, LoggerId = branch.Id };
            return AnalyzeMeterBalance(branch, loggerReadings, new[] { meter }, meterReadings, baselineEnd);
        }

        public static IReadOnlyList<BranchBalance> AnalyzeSubzone(PressureLogger branch,
            IEnumerable<LoggerReading> loggerReadings, IEnumerable<WaterMeter> meters,
            IEnumerable<WaterMeterReading> meterReadings, DateTime baselineEnd)
        {
            var assignedMeters = meters.Where(meter => meter.LoggerId == branch.Id);
            return AnalyzeMeterBalance(branch, loggerReadings, assignedMeters, meterReadings, baselineEnd);
        }

        public static IReadOnlyList<BranchBalance> AnalyzeMeterBalance(PressureLogger supply,
            IEnumerable<LoggerReading> loggerReadings, IEnumerable<WaterMeter> meters,
            IEnumerable<WaterMeterReading> meterReadings, DateTime baselineEnd, bool requireBaseline = true)
        {
            var histories = PrepareMeterHistories(meters, meterReadings);
            var rows = PrepareLoggerReadings(supply.Id, loggerReadings);
            if (rows.Length == 0) return Array.Empty<BranchBalance>();

            var boundaries = histories.OrderBy(history => history.Key).First().Value
                .Select(reading => reading.TakenAt)
                .Where(time => time >= rows[0].TakenAt.AddMinutes(-IntervalMinutes) && time <= rows[^1].TakenAt)
                .ToArray();
            var balances = new List<(BranchBalance Balance, decimal Pressure)>();
            for (int i = 1; i < boundaries.Length; i++)
            {
                DateTime from = boundaries[i - 1], to = boundaries[i];
                var supplied = CalculateSupply(rows, from, to);
                if (supplied == null) continue;
                decimal? consumed = CalculateConsumption(histories.Values, from, to);
                if (!consumed.HasValue) continue;

                decimal hours = (decimal)(to - from).TotalHours;
                decimal difference = (supplied.Value.Volume - consumed.Value) / hours;
                var balance = new BranchBalance(from, to, supplied.Value.Volume, consumed.Value, difference, 0, false, false);
                balances.Add((balance, supplied.Value.Pressure));
            }
            return ApplyCusum(balances, baselineEnd, requireBaseline);
        }

        public static IReadOnlyList<BranchBalance> AnalyzeInlet(PressureLogger inlet,
            IEnumerable<PressureLogger> loggers, IEnumerable<LoggerReading> readings, DateTime baselineEnd)
        {
            var children = loggers.Where(logger => logger.ParentLoggerId == inlet.Id).Select(logger => logger.Id).ToHashSet();
            if (children.Count == 0) throw new ArgumentException("No downstream loggers are configured.");
            var rows = readings.Where(reading => reading.LoggerId == inlet.Id || children.Contains(reading.LoggerId)).ToArray();
            ValidateMeasurements(rows);
            var lookup = rows.ToDictionary(reading => (reading.LoggerId, reading.TakenAt));
            var balances = new List<(BranchBalance Balance, decimal Pressure)>();
            foreach (var row in rows.Where(reading => reading.LoggerId == inlet.Id).OrderBy(reading => reading.TakenAt))
            {
                if (children.Any(id => !lookup.ContainsKey((id, row.TakenAt)))) continue;
                decimal downstream = children.Sum(id => lookup[(id, row.TakenAt)].FlowM3PerHour);
                var balance = new BranchBalance(row.TakenAt.AddMinutes(-IntervalMinutes), row.TakenAt,
                    row.FlowM3PerHour * IntervalHours, downstream * IntervalHours,
                    row.FlowM3PerHour - downstream, 0, false, false);
                balances.Add((balance, row.PressureBar));
            }
            return ApplyCusum(balances, baselineEnd);
        }

        private static Dictionary<int, WaterMeterReading[]> PrepareMeterHistories(
            IEnumerable<WaterMeter> meters, IEnumerable<WaterMeterReading> readings)
        {
            var ids = meters.Select(meter => meter.Id).ToHashSet();
            if (ids.Count == 0) throw new ArgumentException("No meters are assigned to this logger.");
            var histories = readings.Where(reading => ids.Contains(reading.WaterMeterId))
                .GroupBy(reading => reading.WaterMeterId)
                .ToDictionary(group => group.Key, group => group.OrderBy(reading => reading.TakenAt).ToArray());
            if (histories.Count != ids.Count) throw new ArgumentException("Some meters have no readings.");
            foreach (var history in histories.Values)
                if (history.DistinctBy(reading => reading.TakenAt).Count() != history.Length)
                    throw new ArgumentException("Duplicate meter timestamps cannot be analyzed.");
            return histories;
        }

        private static LoggerReading[] PrepareLoggerReadings(int loggerId, IEnumerable<LoggerReading> readings)
        {
            var rows = readings.Where(reading => reading.LoggerId == loggerId).OrderBy(reading => reading.TakenAt).ToArray();
            if (rows.DistinctBy(reading => reading.TakenAt).Count() != rows.Length)
                throw new ArgumentException("Duplicate logger timestamps cannot be analyzed.");
            ValidateMeasurements(rows);
            return rows;
        }

        private static void ValidateMeasurements(IEnumerable<LoggerReading> readings)
        {
            if (readings.Any(reading => reading.FlowM3PerHour < 0 || reading.PressureBar < 0))
                throw new ArgumentException("Logger readings must be nonnegative.");
        }

        private static (decimal Volume, decimal Pressure)? CalculateSupply(LoggerReading[] rows, DateTime from, DateTime to)
        {
            decimal volume = 0, pressure = 0;
            DateTime covered = from;
            foreach (var row in rows.Where(reading => reading.TakenAt > from && reading.TakenAt.AddMinutes(-IntervalMinutes) < to))
            {
                DateTime start = row.TakenAt.AddMinutes(-IntervalMinutes);
                DateTime left = start > from ? start : from;
                DateTime right = row.TakenAt < to ? row.TakenAt : to;
                if (left != covered) break;
                decimal hours = (decimal)(right - left).TotalHours;
                volume += row.FlowM3PerHour * hours;
                pressure += row.PressureBar * hours;
                covered = right;
            }
            if (covered != to || to <= from) return null;
            return (volume, pressure / (decimal)(to - from).TotalHours);
        }

        private static decimal? CalculateConsumption(IEnumerable<WaterMeterReading[]> histories, DateTime from, DateTime to)
        {
            decimal litres = 0;
            foreach (var history in histories)
            {
                decimal? first = CounterAt(history, from), last = CounterAt(history, to);
                if (!first.HasValue || !last.HasValue) return null;
                if (last.Value < first.Value) throw new ArgumentException("A meter reset needs separate handling.");
                litres += last.Value - first.Value;
            }
            return litres / 1000m;
        }

        private static decimal? CounterAt(WaterMeterReading[] rows, DateTime time)
        {
            var left = rows.LastOrDefault(reading => reading.TakenAt <= time);
            if (left == null) return null;
            int first = Counter(left.Reading);
            if (left.TakenAt == time) return first;
            var right = rows.FirstOrDefault(reading => reading.TakenAt > time);
            if (right == null) return null;
            double typicalGap = TypicalGapSeconds(rows);
            if (typicalGap == 0 || (right.TakenAt - left.TakenAt).TotalSeconds > typicalGap * 1.5) return null;
            int last = Counter(right.Reading);
            if (last < first) throw new ArgumentException("A meter reset needs separate handling.");
            decimal fraction = (decimal)((time - left.TakenAt).TotalSeconds / (right.TakenAt - left.TakenAt).TotalSeconds);
            return first + (last - first) * fraction;
        }

        private static double TypicalGapSeconds(WaterMeterReading[] rows)
        {
            var gaps = new List<double>();
            for (int i = 1; i < rows.Length; i++)
                gaps.Add((rows[i].TakenAt - rows[i - 1].TakenAt).TotalSeconds);
            gaps.Sort();
            return gaps.Count == 0 ? 0 : gaps[gaps.Count / 2];
        }

        private static IReadOnlyList<BranchBalance> ApplyCusum(List<(BranchBalance Balance, decimal Pressure)> balances,
            DateTime baselineEnd, bool requireBaseline = true)
        {
            var baseline = balances.Where(item => item.Balance.To <= baselineEnd).ToArray();
            if (baseline.Length < MinimumBaselineCount) baseline = balances.Take(MinimumBaselineCount).ToArray();
            if (baseline.Length < MinimumBaselineCount)
            {
                if (requireBaseline) throw new ArgumentException("More normal history is needed for leak detection.");
                return balances.Select(item => item.Balance).ToList();
            }
            if (baseline[^1].Balance.To > baselineEnd) baselineEnd = baseline[^1].Balance.To;
            decimal mean = baseline.Average(item => item.Balance.ExcessM3PerHour);
            decimal sigma = StandardDeviation(baseline.Select(item => item.Balance.ExcessM3PerHour).ToArray(), mean);
            decimal allowance = Math.Max(0.05m, sigma * 0.5m);
            decimal threshold = Math.Max(0.20m, sigma * 5m);
            decimal baselinePressure = baseline.Average(item => item.Pressure);
            decimal cusum = 0;
            DateTime? previousEnd = null;
            var result = new List<BranchBalance>();
            foreach (var item in balances)
            {
                var balance = item.Balance;
                if (previousEnd != balance.From) cusum = 0;
                bool afterBaseline = balance.From >= baselineEnd;
                if (afterBaseline)
                    cusum = Math.Max(0, cusum + balance.ExcessM3PerHour - mean - allowance);
                result.Add(balance with
                {
                    Cusum = cusum,
                    DetectionThreshold = afterBaseline ? threshold : 0,
                    LeakSuspected = cusum > threshold && balance.ExcessM3PerHour > mean + allowance,
                    PressureDrop = item.Pressure < baselinePressure - 0.2m
                });
                previousEnd = balance.To;
            }
            return result;
        }

        private static decimal StandardDeviation(decimal[] values, decimal mean)
        {
            decimal squaredDifferences = 0;
            foreach (decimal value in values)
            {
                decimal difference = value - mean;
                squaredDifferences += difference * difference;
            }
            decimal variance = squaredDifferences / (values.Length - 1);
            return (decimal)Math.Sqrt((double)variance);
        }

        private static int Counter(string reading)
        {
            if (reading.Length != 8 || reading.Any(character => character is < '0' or > '9'))
                throw new ArgumentException("Meter counters must contain eight digits.");
            return int.Parse(reading, CultureInfo.InvariantCulture);
        }
    }
}
