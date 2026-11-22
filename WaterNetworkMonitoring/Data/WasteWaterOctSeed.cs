using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Data
{
    internal static class WasteWaterOctSeed
    {
        internal static readonly DateTimeOffset Start = Local(new DateTime(2026, 10, 1));
        internal static readonly DateTimeOffset End = Local(new DateTime(2026, 11, 1));
        internal static readonly DateTimeOffset BlockageStartsAt = Local(new DateTime(2026, 10, 12));

        internal sealed record RainPeriod(DateTimeOffset Start, DateTimeOffset End);
        // Example rain dates, not historical weather observations.
        internal static readonly RainPeriod[] RainPeriods =
        {
            new(Local(new DateTime(2026, 10, 4, 6, 0, 0)), Local(new DateTime(2026, 10, 4, 9, 0, 0))),
            new(Local(new DateTime(2026, 10, 11, 16, 0, 0)), Local(new DateTime(2026, 10, 11, 19, 0, 0))),
            new(Local(new DateTime(2026, 10, 20, 6, 0, 0)), Local(new DateTime(2026, 10, 20, 8, 0, 0))),
            new(Local(new DateTime(2026, 10, 28, 18, 0, 0)), Local(new DateTime(2026, 10, 28, 21, 0, 0)))
        };

        public static List<WasteWaterReading> GenerateReadings()
        {
            var result = new List<WasteWaterReading>();
            var loggers = WasteWaterSepSeed.GetLoggers();
            decimal[] averageFlow = { 3.6m, 6.2m, 4.5m };
            // Readings stay at 02:00, 04:00, ... local time, including the clock change.
            for (var clock = Start.DateTime.AddHours(2); clock <= End.DateTime; clock = clock.AddHours(2))
            {
                var time = Local(clock);
                var from = Local(clock.AddHours(-2));
                double rainMinutes = RainPeriods.Sum(r => Math.Max(0,
                    ((r.End < time ? r.End : time) - (r.Start > from ? r.Start : from)).TotalMinutes));
                decimal wetFraction = (decimal)(rainMinutes / (time - from).TotalMinutes);
                int samples = (int)(time - from).TotalMinutes / 15;
                for (int i = 0; i < loggers.Length; i++)
                {
                    decimal flowTotal = 0, levelTotal = 0;
                    // Actual elapsed time includes the repeated hour on 25 October.
                    for (var utc = from.ToUniversalTime(); utc < time; utc = utc.AddMinutes(15))
                    {
                        var local = BulgariaTime(utc);
                        int slot = local.Hour * 4 + local.Minute / 15;
                        decimal hourFactor = local.Hour switch
                        {
                            < 6 => .35m, < 10 => 1.45m, < 17 => .9m, < 22 => 1.3m, _ => .6m
                        };
                        decimal daily = .93m + (loggers[i].Id + local.Day * 17) % 15 / 100m;
                        decimal variation = .96m + (loggers[i].Id * 3 + slot * 7) % 9 / 100m;
                        decimal flow = averageFlow[i] * hourFactor * daily * variation;
                        decimal level = .10m + i * .02m + flow * .022m + ((slot + i * 3) % 7 - 3) * .001m;

                        // W2 is assumed just upstream of a growing obstruction.
                        // This example trend reduces flow by up to 35% and adds up to 25 cm of backwater.
                        decimal blockage = loggers[i].Id == 300002
                            ? Math.Clamp((decimal)((utc - BlockageStartsAt).TotalHours / (End - BlockageStartsAt).TotalHours), 0m, 1m)
                            : 0m;
                        flowTotal += flow * (1 - .35m * blockage);
                        levelTotal += level + .25m * blockage;
                    }
                    result.Add(new WasteWaterReading
                    {
                        LoggerId = loggers[i].Id, TakenAt = time,
                        FlowM3PerHour = decimal.Round(flowTotal / samples * (1 + .55m * wetFraction), 3),
                        WaterLevelMetres = decimal.Round(levelTotal / samples + .18m * wetFraction, 3),
                        IsRaining = rainMinutes > 0, IsSimulated = true
                    });
                }
            }
            return result;
        }

        private static DateTimeOffset Local(DateTime time) => new(time,
            TimeSpan.FromHours(time < new DateTime(2026, 10, 25, 4, 0, 0) ? 3 : 2));
        private static DateTimeOffset BulgariaTime(DateTimeOffset utc) => utc.ToOffset(TimeSpan.FromHours(
            utc < new DateTimeOffset(2026, 10, 25, 1, 0, 0, TimeSpan.Zero) ? 3 : 2));
    }
}
