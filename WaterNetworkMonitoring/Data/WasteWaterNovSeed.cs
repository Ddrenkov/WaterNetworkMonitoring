using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Data
{
    internal static class WasteWaterNovSeed
    {
        internal static readonly DateTimeOffset Start = Local(new DateTime(2026, 11, 1));
        internal static readonly DateTimeOffset End = Local(new DateTime(2026, 12, 1));

        internal sealed record RainPeriod(DateTimeOffset Start, DateTimeOffset End);
        // Example rain dates, not historical weather observations.
        internal static readonly RainPeriod[] RainPeriods =
        {
            new(Local(new DateTime(2026, 11, 3, 6, 0, 0)), Local(new DateTime(2026, 11, 3, 9, 0, 0))),
            new(Local(new DateTime(2026, 11, 9, 16, 0, 0)), Local(new DateTime(2026, 11, 9, 19, 0, 0))),
            new(Local(new DateTime(2026, 11, 15, 8, 0, 0)), Local(new DateTime(2026, 11, 15, 10, 0, 0))),
            new(Local(new DateTime(2026, 11, 21, 18, 0, 0)), Local(new DateTime(2026, 11, 21, 21, 0, 0))),
            new(Local(new DateTime(2026, 11, 27, 6, 0, 0)), Local(new DateTime(2026, 11, 27, 9, 0, 0)))
        };

        public static List<WasteWaterReading> GenerateReadings()
        {
            var result = new List<WasteWaterReading>();
            var loggers = WasteWaterSepSeed.GetLoggers();
            decimal[] averageFlow = { 3.6m, 6.2m, 4.5m };
            for (var time = Start.AddHours(2); time <= End; time = time.AddHours(2))
            {
                var from = time.AddHours(-2);
                double rainMinutes = RainPeriods.Sum(r => Math.Max(0,
                    ((r.End < time ? r.End : time) - (r.Start > from ? r.Start : from)).TotalMinutes));
                decimal wetFraction = (decimal)(rainMinutes / 120);
                for (int i = 0; i < loggers.Length; i++)
                {
                    decimal flowTotal = 0, levelTotal = 0;
                    for (var sample = from; sample < time; sample = sample.AddMinutes(15))
                    {
                        int slot = sample.Hour * 4 + sample.Minute / 15;
                        decimal hourFactor = sample.Hour switch
                        {
                            < 6 => .35m, < 10 => 1.45m, < 17 => .9m, < 22 => 1.3m, _ => .6m
                        };
                        decimal daily = .93m + (loggers[i].Id + sample.Day * 17) % 15 / 100m;
                        decimal variation = .96m + (loggers[i].Id * 3 + slot * 7) % 9 / 100m;
                        decimal flow = averageFlow[i] * hourFactor * daily * variation;
                        decimal level = .10m + i * .02m + flow * .022m + ((slot + i * 3) % 7 - 3) * .001m;

                        // Continue October's obstruction: W2 loses 35% to 65% of normal flow.
                        // Upstream backwater grows from 25 cm to 50 cm during November.
                        decimal progress = (decimal)((sample - Start).TotalHours / (End - Start).TotalHours);
                        bool blocked = loggers[i].Id == 300002;
                        flowTotal += flow * (blocked ? .65m - .30m * progress : 1m);
                        levelTotal += level + (blocked ? .25m + .25m * progress : 0m);
                    }
                    result.Add(new WasteWaterReading
                    {
                        LoggerId = loggers[i].Id, TakenAt = time,
                        FlowM3PerHour = decimal.Round(flowTotal / 8 * (1 + .55m * wetFraction), 3),
                        WaterLevelMetres = decimal.Round(levelTotal / 8 + .18m * wetFraction, 3),
                        IsRaining = rainMinutes > 0, IsSimulated = true
                    });
                }
            }
            return result;
        }

        private static DateTimeOffset Local(DateTime time) => new(time, TimeSpan.FromHours(2));
    }
}
