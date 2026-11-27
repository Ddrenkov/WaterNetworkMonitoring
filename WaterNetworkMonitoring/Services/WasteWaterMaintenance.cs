using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Services
{
    internal static class WasteWaterMaintenance
    {
        private const int ReferenceDays = 7;
        private const int RecentHours = 12;
        private const int RequiredReadings = 3;
        private const int MinimumReferenceReadings = 3;
        private const decimal MaximumFlowRatio = 0.60m;
        private const decimal MinimumLevelRiseMetres = 0.15m;

        internal sealed record Alert(int LoggerId, decimal FlowDropPercent,
            decimal ExtraLevelMetres, DateTime TakenAt);

        public static List<Alert> FindAlerts(IEnumerable<WasteWaterReading> readings)
        {
            var alerts = new List<Alert>();
            foreach (var group in readings.GroupBy(reading => reading.LoggerId))
            {
                var history = group.OrderBy(reading => reading.TakenAt).ToArray();
                var alert = FindAlert(group.Key, history);
                if (alert != null) alerts.Add(alert);
            }
            return alerts;
        }

        private static Alert? FindAlert(int loggerId, WasteWaterReading[] history)
        {
            // The first seven days are assumed to be a healthy reference period.
            var referenceEnd = history[0].TakenAt.AddDays(ReferenceDays);
            var reference = history.Where(reading => reading.TakenAt < referenceEnd && !reading.IsRaining).ToArray();
            var recentStart = history[^1].TakenAt.AddHours(-RecentHours);
            var recent = history.Where(reading => reading.TakenAt >= referenceEnd && !reading.IsRaining &&
                reading.TakenAt > recentStart).TakeLast(RequiredReadings).ToArray();
            if (recent.Length < RequiredReadings) return null;

            var changes = new List<(decimal Drop, decimal Rise)>();
            foreach (var reading in recent)
            {
                var change = CompareWithReference(reading, reference);
                if (change == null) return null;
                changes.Add(change.Value);
            }
            return new Alert(loggerId, changes.Average(change => change.Drop),
                changes.Average(change => change.Rise), recent[^1].TakenAt);
        }

        private static (decimal Drop, decimal Rise)? CompareWithReference(
            WasteWaterReading reading, WasteWaterReading[] reference)
        {
            // Compare the same local hour and minute to account for the daily pattern.
            var normal = reference.Where(row => row.TakenAt.Hour == reading.TakenAt.Hour &&
                row.TakenAt.Minute == reading.TakenAt.Minute).ToArray();
            if (normal.Length < MinimumReferenceReadings) return null;

            decimal normalFlow = Median(normal.Select(row => row.FlowM3PerHour));
            decimal normalLevel = Median(normal.Select(row => row.WaterLevelMetres));
            if (normalFlow <= 0) return null;
            if (reading.FlowM3PerHour > normalFlow * MaximumFlowRatio) return null;
            if (reading.WaterLevelMetres < normalLevel + MinimumLevelRiseMetres) return null;

            decimal drop = (1 - reading.FlowM3PerHour / normalFlow) * 100;
            decimal rise = reading.WaterLevelMetres - normalLevel;
            return (drop, rise);
        }

        private static decimal Median(IEnumerable<decimal> values)
        {
            var sorted = values.OrderBy(value => value).ToArray();
            int middle = sorted.Length / 2;
            if (sorted.Length % 2 == 0)
                return (sorted[middle - 1] + sorted[middle]) / 2;
            return sorted[middle];
        }
    }
}
