using System.Globalization;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Data
{
    internal static class SepNovSeed
    {
        internal static readonly DateTime SeptemberStart = new DateTime(2026, 9, 1);
        internal static readonly DateTime OctoberStart = new DateTime(2026, 10, 1);
        internal static readonly DateTime NovemberStart = new DateTime(2026, 11, 1);
        internal static readonly DateTime DecemberStart = new DateTime(2026, 12, 1);
        internal sealed record SampleData(List<LoggerReading> LoggerReadings, List<WaterMeterReading> MeterReadings);

        internal static Repair[] GetRepairs() => new[]
        {
            new Repair { Id = 1, Date = new DateTime(2026, 10, 31, 18, 0, 0),
                Place = "Zapad - Skaptopara supply branch", WaterSystem = WaterSystem.CleanWater }
        };

        internal static SampleData Generate()
        {
            var profiles = ZapadOctober2026Seed.Profiles;
            var firstDay = new DateOnly(2026, 8, 31);
            var counters = profiles.Select(p => 500000 + p.MapNumber * 10000).ToArray();
            for (var day = firstDay; day < new DateOnly(2026, 9, 30); day = day.AddDays(1))
                for (int i = 0; i < profiles.Length; i++) counters[i] -= Usage(profiles[i], day).Sum();

            var meters = new List<WaterMeterReading>();
            var branchUsage = new Dictionary<DateTime, int>();
            for (var day = firstDay; day <= new DateOnly(2026, 12, 1); day = day.AddDays(1))
            {
                var start = day.ToDateTime(TimeOnly.MinValue);
                var usage = profiles.Select(p => Usage(p, day)).ToArray();
                for (int slot = 0; slot < usage[0].Length; slot++)
                {
                    var time = start.AddMinutes((slot + 1) * 15);
                    if (time > DecemberStart.AddHours(6)) break;
                    for (int i = 0; i < profiles.Length; i++)
                    {
                        var profile = profiles[i];
                        counters[i] += usage[i][slot];
                        if (profile.Id == 100058) branchUsage[time] = usage[i][slot];
                        int hour = profile.MapNumber % 2 == 0 && profile.Id != 100058 ? 2 : 6;
                        if ((time.Minute == 0 && (time.Hour == hour || time.Hour == hour + 12)) ||
                            time == ZapadOctober2026Seed.SnapshotAt)
                            meters.Add(new WaterMeterReading { WaterMeterId = profile.Id,
                                Reading = counters[i].ToString("D8", CultureInfo.InvariantCulture),
                                TakenAt = time, IsSimulated = true });
                    }
                }
            }
            if (meters.Any(r => r.Reading.Length != 8 || r.Reading[0] == '-'))
                throw new InvalidOperationException("Sample counters must fit eight digits.");

            var histories = meters.GroupBy(r => r.WaterMeterId)
                .ToDictionary(g => g.Key, g => g.OrderBy(r => r.TakenAt).ToArray());
            var loggers = new List<LoggerReading>();
            foreach (var range in new[] { (SeptemberStart, OctoberStart), (NovemberStart, DecemberStart) })
                for (var sampleTime = range.Item1.AddMinutes(15); sampleTime <= range.Item2; sampleTime = sampleTime.AddMinutes(15))
                {
                    var time = sampleTime;
                    decimal branch = branchUsage[time] * .004m;
                    decimal north = Flow(200003, histories, time), south = Flow(200004, histories, time);
                    decimal inlet = branch + north + south;
                    decimal jitter = ((int)(Noise(200001, DateOnly.FromDateTime(time.Date).DayNumber, time.Hour * 4 + time.Minute / 15) % 41) - 20) / 1000m;
                    loggers.Add(Row(200001, time, inlet, 4.5m - inlet * .0125m + jitter));
                    loggers.Add(Row(200002, time, branch, 4.1m - branchUsage[time] * .0004m + jitter));
                    loggers.Add(Row(200003, time, north, 4.3m - north * .005m));
                    loggers.Add(Row(200004, time, south, 4.3m - south * .005m));
                }
            // The example readings return to normal after the seeded repair record.
            var repair = GetRepairs().Single(r => r.WaterSystem == WaterSystem.CleanWater);
            var repairTime = repair.Date;
            foreach (var old in ZapadFourLoggerSeed.GenerateReadings().Where(r =>
                r.LoggerId is 200001 or 200002 && r.TakenAt > repairTime && r.TakenAt <= NovemberStart))
                loggers.Add(Row(old.LoggerId, old.TakenAt, old.FlowM3PerHour - .5m,
                    old.PressureBar + (old.LoggerId == 200001 ? .12m : .45m)));
            return new SampleData(loggers, meters.Where(r => r.TakenAt < OctoberStart || r.TakenAt >= NovemberStart).ToList());
        }

        private static LoggerReading Row(int id, DateTime time, decimal flow, decimal pressure) =>
            new() { LoggerId = id, TakenAt = time, FlowM3PerHour = flow,
                PressureBar = decimal.Round(pressure, 3), IsSimulated = true };

        private static decimal Flow(int loggerId, Dictionary<int, WaterMeterReading[]> histories, DateTime time) =>
            decimal.Round(histories.Where(h => ZapadFourLoggerSeed.LoggerForMeter(h.Key) == loggerId)
                .Sum(h => CounterAt(h.Value, time) - CounterAt(h.Value, time.AddMinutes(-15))) * .004m, 3);

        private static decimal CounterAt(WaterMeterReading[] rows, DateTime time)
        {
            var left = rows.Last(r => r.TakenAt <= time);
            decimal value = decimal.Parse(left.Reading, CultureInfo.InvariantCulture);
            if (left.TakenAt == time) return value;
            var right = rows.First(r => r.TakenAt > time);
            return value + (decimal.Parse(right.Reading, CultureInfo.InvariantCulture) - value) *
                (decimal)((time - left.TakenAt).TotalSeconds / (right.TakenAt - left.TakenAt).TotalSeconds);
        }

        private static int[] Usage(ZapadOctober2026Seed.MeterProfile profile, DateOnly day)
        {
            var start = day.ToDateTime(TimeOnly.MinValue);
            var end = day.AddDays(1).ToDateTime(TimeOnly.MinValue);
            int slots = (int)(end - start).TotalMinutes / 15;
            var weights = Enumerable.Range(0, slots).Select(slot =>
                HourWeight(profile.Kind, start.AddMinutes(slot * 15).Hour) *
                (.90m + Noise(profile.Id, day.DayNumber, slot) % 21 / 100m)).ToArray();
            decimal daily = DailyLitres(profile) * (.90m + Noise(profile.Id, day.DayNumber, 101) % 21 / 100m);
            decimal total = weights.Sum(), running = 0;
            int assigned = 0;
            var result = new int[slots];
            for (int i = 0; i < slots; i++)
            {
                running += weights[i];
                int target = (int)decimal.Round(daily * running / total, 0, MidpointRounding.AwayFromZero);
                result[i] = target - assigned;
                assigned = target;
            }
            return result;
        }

        private static decimal DailyLitres(ZapadOctober2026Seed.MeterProfile p) => p.Kind switch
        {
            "roof" => 0, "service" => 100, "sports_hall" => 4000, "clinic" => 2400, "market" => 900,
            _ => p.AssumedOccupants * 149
        };

        private static decimal HourWeight(string kind, int hour)
        {
            if (kind == "sports_hall") return hour is >= 8 and < 22 ? 1m : .04m;
            if (kind is "clinic" or "market" or "service") return hour is >= 7 and < 20 ? 1m : .08m;
            return hour switch { < 6 => .18m, < 10 => 1.8m, < 17 => .9m, < 22 => 1.6m, _ => .55m };
        }

        private static uint Noise(int id, int day, int slot)
        {
            unchecked
            {
                uint value = (uint)(id * 73856093) ^ (uint)(day * 19349663) ^ (uint)(slot * 83492791);
                value ^= value >> 16; value *= 0x7feb352d; value ^= value >> 15;
                return value;
            }
        }
    }
}
