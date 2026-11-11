using System.Globalization;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Services
{
    public sealed record BranchBalance(DateTimeOffset From, DateTimeOffset To,
        decimal SuppliedM3, decimal ConsumedM3, decimal ExcessM3PerHour,
        decimal Cusum, bool LeakSuspected, bool PressureDrop);

    public static class LeakDetectionService
    {
        // Uses measured volumes and the logger-to-meter relationship, never a planted leak date or label.
        // This is a branch balance detector; mixed schedules at the entire zone need separate alignment.
        public static IReadOnlyList<BranchBalance> AnalyzeBranch(PressureLogger branch,
            IEnumerable<LoggerReading> loggerReadings, IEnumerable<WaterMeterReading> meterReadings,
            DateTimeOffset baselineEnd)
        {
            if (branch.SuppliedWaterMeterId is not int meterId)
                throw new ArgumentException("Choose a logger supplying one metered branch.", nameof(branch));
            var logger = loggerReadings.Where(r => r.LoggerId == branch.Id).OrderBy(r => r.TakenAt).ToArray();
            var meter = meterReadings.Where(r => r.WaterMeterId == meterId).OrderBy(r => r.TakenAt).ToArray();
            if (logger.DistinctBy(r => r.TakenAt).Count() != logger.Length ||
                meter.DistinctBy(r => r.TakenAt).Count() != meter.Length)
                throw new ArgumentException("Duplicate timestamps cannot be analyzed.");

            var balances = new List<(BranchBalance Balance, decimal Pressure)>();
            for (int i = 1; i < meter.Length; i++)
            {
                var from = meter[i - 1].TakenAt;
                var to = meter[i].TakenAt;
                // Ignore the isolated November snapshot and any incomplete logger window.
                if (to - from > TimeSpan.FromHours(13) ||
                    (to - from).Ticks % TimeSpan.FromMinutes(15).Ticks != 0) continue;
                var window = logger.Where(r => r.TakenAt > from && r.TakenAt <= to).ToArray();
                int required = (int)((to - from).TotalMinutes / 15);
                if (required == 0 || window.Length != required) continue;
                bool complete = true;
                for (int j = 0; j < window.Length; j++)
                    if (window[j].TakenAt != from.AddMinutes(15 * (j + 1))) complete = false;
                if (!complete) continue;
                if (window.Any(r => r.FlowM3PerHour < 0 || r.PressureBar < 0))
                    throw new ArgumentException("Logger readings must be nonnegative.");

                int previous = Counter(meter[i - 1].Reading), current = Counter(meter[i].Reading);
                if (current < previous) throw new ArgumentException("A meter reset or rollover needs separate handling.");
                decimal supplied = window.Sum(r => r.FlowM3PerHour * 0.25m);
                decimal consumed = (current - previous) / 1000m;
                decimal excess = (supplied - consumed) / (decimal)(to - from).TotalHours;
                balances.Add((new BranchBalance(from, to, supplied, consumed, excess, 0, false, false),
                    window.Average(r => r.PressureBar)));
            }

            var baseline = balances.Where(b => b.Balance.To <= baselineEnd).ToArray();
            if (baseline.Length < 10)
                throw new ArgumentException("At least ten complete normal baseline windows are required.");
            decimal mean = baseline.Average(b => b.Balance.ExcessM3PerHour);
            decimal variance = baseline.Sum(b => (b.Balance.ExcessM3PerHour - mean) *
                (b.Balance.ExcessM3PerHour - mean)) / (baseline.Length - 1);
            decimal sigma = (decimal)Math.Sqrt((double)variance);
            decimal allowance = Math.Max(0.05m, sigma * 0.5m);
            decimal threshold = Math.Max(0.20m, sigma * 5m);
            decimal baselinePressure = baseline.Average(b => b.Pressure);
            decimal cusum = 0;
            DateTimeOffset? previousEnd = null;
            var result = new List<BranchBalance>();
            foreach (var (balance, pressure) in balances)
            {
                if (previousEnd != balance.From) cusum = 0; // Missing windows break persistence.
                if (balance.From >= baselineEnd)
                    cusum = Math.Max(0, cusum + balance.ExcessM3PerHour - mean - allowance);
                result.Add(balance with
                {
                    Cusum = cusum, LeakSuspected = cusum > threshold,
                    PressureDrop = pressure < baselinePressure - 0.2m
                });
                previousEnd = balance.To;
            }
            return result;
        }

        private static int Counter(string reading)
        {
            if (reading.Length != 8 || reading.Any(c => c is < '0' or > '9'))
                throw new ArgumentException("Meter counters must contain eight digits.");
            return int.Parse(reading, CultureInfo.InvariantCulture);
        }
    }
}
