using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Services
{
    internal static class WasteWaterMaintenance
    {
        internal sealed record Alert(int LoggerId, decimal FlowDropPercent,
            decimal ExtraLevelMetres, DateTime TakenAt);

        public static List<Alert> FindAlerts(IEnumerable<WasteWaterReading> readings)
        {
            var alerts = new List<Alert>();
            foreach (var group in readings.GroupBy(r => r.LoggerId))
            {
                var history = group.OrderBy(r => r.TakenAt).ToArray();
                // The first seven days are assumed to be a healthy reference period.
                var referenceEnd = history[0].TakenAt.AddDays(7);
                var reference = history.Where(r => r.TakenAt < referenceEnd && !r.IsRaining).ToArray();
                var latestTime = history[^1].TakenAt;
                var recent = history.Where(r => r.TakenAt >= referenceEnd && !r.IsRaining &&
                    r.TakenAt > latestTime.AddHours(-12)).TakeLast(3).ToArray();
                if (recent.Length < 3) continue;

                var changes = new List<(decimal Drop, decimal Rise)>();
                foreach (var reading in recent)
                {
                    // Compare the same local hours so normal night-time lows do not trigger an alert.
                    var normal = reference.Where(r => r.TakenAt.Hour == reading.TakenAt.Hour &&
                        r.TakenAt.Minute == reading.TakenAt.Minute).ToArray();
                    if (normal.Length < 3) break;
                    decimal flow = Median(normal.Select(r => r.FlowM3PerHour));
                    decimal level = Median(normal.Select(r => r.WaterLevelMetres));
                    if (flow <= 0 || reading.FlowM3PerHour > flow * .60m ||
                        reading.WaterLevelMetres < level + .15m) break;
                    changes.Add(((1 - reading.FlowM3PerHour / flow) * 100,
                        reading.WaterLevelMetres - level));
                }
                // Three recent dry readings must all show both symptoms.
                if (changes.Count == 3)
                    alerts.Add(new Alert(group.Key, changes.Average(c => c.Drop),
                        changes.Average(c => c.Rise), recent[^1].TakenAt));
            }
            return alerts;
        }

        private static decimal Median(IEnumerable<decimal> values)
        {
            var sorted = values.OrderBy(v => v).ToArray();
            int middle = sorted.Length / 2;
            return sorted.Length % 2 == 0 ? (sorted[middle - 1] + sorted[middle]) / 2 : sorted[middle];
        }
    }
}
