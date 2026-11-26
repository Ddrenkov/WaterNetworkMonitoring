using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Data
{
    // Fixed September example data for the wastewater migrations.
    internal static class WasteWaterSepSeed
    {
        public static readonly DateTime Start = new(2026, 9, 1, 0, 0, 0);
        public static readonly DateTime End = new(2026, 10, 1, 0, 0, 0);

        public static PressureLogger[] GetLoggers() => new[]
        {
            new PressureLogger { Id = 300001, Name = "W1", ZoneName = "Zapad", WaterSystem = WaterSystem.Wastewater,
                Latitude = 42.0160494629729, Longitude = 23.083341027286686, IsSimulated = true },
            new PressureLogger { Id = 300002, Name = "W2", ZoneName = "Zapad", WaterSystem = WaterSystem.Wastewater,
                Latitude = 42.01735678485204, Longitude = 23.083793084477268, IsSimulated = true },
            new PressureLogger { Id = 300003, Name = "W3", ZoneName = "Zapad", WaterSystem = WaterSystem.Wastewater,
                Latitude = 42.017898909010135, Longitude = 23.084494568877314, IsSimulated = true }
        };

        internal sealed record RainPeriod(DateTime Start, DateTime End);
        // Invented rain events for the project, not historical weather observations.
        internal static readonly RainPeriod[] RainPeriods =
        {
            new(Start.AddDays(5).AddHours(6), Start.AddDays(5).AddHours(9)),
            new(Start.AddDays(15).AddHours(16), Start.AddDays(15).AddHours(19)),
            new(Start.AddDays(24), Start.AddDays(24).AddHours(2))
        };

        public static List<WasteWaterReading> GenerateTwoHourReadings()
        {
            var original = GenerateReadings().ToLookup(r => r.LoggerId);
            var result = new List<WasteWaterReading>();
            for (var time = Start.AddHours(2); time <= End; time = time.AddHours(2))
            {
                var from = time.AddHours(-2);
                double rainMinutes = RainPeriods.Sum(r => Math.Max(0,
                    ((r.End < time ? r.End : time) - (r.Start > from ? r.Start : from)).TotalMinutes));
                decimal wetFraction = (decimal)(rainMinutes / 120);
                foreach (var logger in GetLoggers())
                {
                    var samples = original[logger.Id].Where(r => r.TakenAt > from && r.TakenAt <= time).ToArray();
                    result.Add(new WasteWaterReading
                    {
                        LoggerId = logger.Id, TakenAt = time,
                        FlowM3PerHour = decimal.Round(samples.Average(r => r.FlowM3PerHour) * (1 + .55m * wetFraction), 3),
                        WaterLevelMetres = decimal.Round(samples.Average(r => r.WaterLevelMetres) + .18m * wetFraction, 3),
                        IsRaining = rainMinutes > 0, IsSimulated = true
                    });
                }
            }
            return result;
        }

        // Retained unchanged so the first migration can still reproduce its original data.
        public static List<WasteWaterReading> GenerateReadings()
        {
            var result = new List<WasteWaterReading>();
            decimal[] averageFlow = { 3.6m, 6.2m, 4.5m };
            var loggers = GetLoggers();
            for (var time = Start.AddMinutes(15); time <= End; time = time.AddMinutes(15))
            {
                var interval = time.AddMinutes(-15);
                int slot = interval.Hour * 4 + interval.Minute / 15;
                decimal hourFactor = interval.Hour switch
                {
                    < 6 => .35m, < 10 => 1.45m, < 17 => .9m, < 22 => 1.3m, _ => .6m
                };
                for (int i = 0; i < loggers.Length; i++)
                {
                    var logger = loggers[i];
                    decimal daily = .93m + (logger.Id + interval.Day * 17) % 15 / 100m;
                    decimal variation = .96m + (logger.Id * 3 + slot * 7) % 9 / 100m;
                    decimal flow = decimal.Round(averageFlow[i] * hourFactor * daily * variation, 3);
                    decimal level = decimal.Round(.10m + i * .02m + flow * .022m + ((slot + i * 3) % 7 - 3) * .001m, 3);
                    result.Add(new WasteWaterReading { LoggerId = logger.Id, TakenAt = time,
                        FlowM3PerHour = flow, WaterLevelMetres = level, IsSimulated = true });
                }
            }
            return result;
        }
    }
}
