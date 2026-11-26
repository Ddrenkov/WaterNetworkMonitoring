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

        private const int InletLoggerId = 200001;
        private const int BranchLoggerId = 200002;
        private const int NorthLoggerId = 200003;
        private const int SouthLoggerId = 200004;
        private const int SkaptoparaMeterId = 100058;

        // ID, normal pressure, pressure reduction per m3/h, small pressure variation.
        private sealed record LoggerSettings(int Id, decimal Pressure, decimal PressurePerFlow, bool VaryPressure);
        private static readonly LoggerSettings[] LoggerProfiles =
        {
            new(InletLoggerId,  4.5m, 0.0125m, true),
            new(BranchLoggerId, 4.1m, 0.1m,    true),
            new(NorthLoggerId,  4.3m, 0.005m,  false),
            new(SouthLoggerId,  4.3m, 0.005m,  false)
        };

        internal sealed record SampleData(List<LoggerReading> LoggerReadings, List<WaterMeterReading> MeterReadings);

        internal static Repair[] GetRepairs()
        {
            return new[]
            {
                new Repair
                {
                    Id = 1,
                    Date = new DateTime(2026, 10, 31, 18, 0, 0),
                    Place = "Zapad - Skaptopara supply branch",
                    WaterSystem = WaterSystem.CleanWater
                }
            };
        }

        internal static SampleData Generate()
        {
            // Created continuous meter counters and group their observations by meter.
            var allMeterReadings = CreateMeterReadings(out var skaptoparaUsage);
            var meterHistories = GroupMeterReadings(allMeterReadings);

            // Created healthy logger readings for September and November.
            var loggerReadings = new List<LoggerReading>();
            AddMonthReadings(loggerReadings, SeptemberStart, skaptoparaUsage, meterHistories);
            AddMonthReadings(loggerReadings, NovemberStart, skaptoparaUsage, meterHistories);

            // Restored the October readings after the recorded repair.
            AddReadingsAfterRepair(loggerReadings);

            // October meter readings are provided by its own seed.
            var meterReadings = new List<WaterMeterReading>();
            foreach (var reading in allMeterReadings)
            {
                if (reading.TakenAt < OctoberStart || reading.TakenAt >= NovemberStart)
                    meterReadings.Add(reading);
            }
            return new SampleData(loggerReadings, meterReadings);
        }

        private static List<WaterMeterReading> CreateMeterReadings(out Dictionary<DateTime, int> skaptoparaUsage)
        {
            var readings = new List<WaterMeterReading>();
            skaptoparaUsage = new Dictionary<DateTime, int>();
            var firstDay = new DateOnly(2026, 8, 31);
            var lastDay = new DateOnly(2026, 12, 1);

            foreach (var profile in ZapadOctober2026Seed.Profiles)
            {
                int counter = GetStartingCounter(profile, firstDay);
                int firstHour = 6;
                if (profile.MapNumber % 2 == 0 && profile.Id != SkaptoparaMeterId)
                    firstHour = 2;

                for (var day = firstDay; day <= lastDay; day = day.AddDays(1))
                {
                    var dayStart = day.ToDateTime(TimeOnly.MinValue);
                    var usage = ZapadOctober2026Seed.CreateDailyUsage(profile, day);

                    for (int slot = 0; slot < usage.Length; slot++)
                    {
                        var time = dayStart.AddMinutes((slot + 1) * 15);
                        if (time > DecemberStart.AddHours(6)) break;

                        counter += usage[slot];
                        if (profile.Id == SkaptoparaMeterId)
                            skaptoparaUsage[time] = usage[slot];

                        bool scheduled = time.Minute == 0 &&
                            (time.Hour == firstHour || time.Hour == firstHour + 12);
                        bool snapshot = time == ZapadOctober2026Seed.SnapshotAt;
                        if (!scheduled && !snapshot) continue;

                        string reading = counter.ToString("D8", CultureInfo.InvariantCulture);
                        if (reading.Length != 8 || reading[0] == '-')
                            throw new InvalidOperationException("Sample counters must fit eight digits.");

                        readings.Add(new WaterMeterReading
                        {
                            WaterMeterId = profile.Id,
                            Reading = reading,
                            TakenAt = time,
                            IsSimulated = true
                        });
                    }
                }
            }
            return readings.OrderBy(r => r.TakenAt).ThenBy(r => r.WaterMeterId).ToList();
        }

        private static int GetStartingCounter(ZapadOctober2026Seed.MeterProfile profile, DateOnly firstDay)
        {
            // Work back from October's starting counter so the months join correctly.
            int counter = 500000 + profile.MapNumber * 10000;
            for (var day = firstDay; day < new DateOnly(2026, 9, 30); day = day.AddDays(1))
            {
                foreach (int litres in ZapadOctober2026Seed.CreateDailyUsage(profile, day))
                    counter -= litres;
            }
            return counter;
        }

        private static Dictionary<int, List<WaterMeterReading>> GroupMeterReadings(List<WaterMeterReading> readings)
        {
            var histories = new Dictionary<int, List<WaterMeterReading>>();
            foreach (var reading in readings)
            {
                if (!histories.ContainsKey(reading.WaterMeterId))
                    histories[reading.WaterMeterId] = new List<WaterMeterReading>();

                histories[reading.WaterMeterId].Add(reading);
            }
            return histories;
        }

        private static void AddMonthReadings(List<LoggerReading> readings, DateTime monthStart,
            Dictionary<DateTime, int> skaptoparaUsage, Dictionary<int, List<WaterMeterReading>> histories)
        {
            var monthEnd = monthStart.AddMonths(1);
            for (var time = monthStart.AddMinutes(15); time <= monthEnd; time = time.AddMinutes(15))
            {
                decimal branchFlow = skaptoparaUsage[time] * 0.004m;
                decimal northFlow = Flow(NorthLoggerId, histories, time);
                decimal southFlow = Flow(SouthLoggerId, histories, time);
                decimal inletFlow = branchFlow + northFlow + southFlow;

                decimal[] flows = { inletFlow, branchFlow, northFlow, southFlow };
                decimal variation = ZapadOctober2026Seed.PressureVariation(time);
                for (int i = 0; i < LoggerProfiles.Length; i++)
                {
                    var logger = LoggerProfiles[i];
                    decimal pressure = logger.Pressure - flows[i] * logger.PressurePerFlow;
                    if (logger.VaryPressure) pressure += variation;

                    readings.Add(new LoggerReading
                    {
                        LoggerId = logger.Id,
                        TakenAt = time,
                        FlowM3PerHour = flows[i],
                        PressureBar = decimal.Round(pressure, 3),
                        IsSimulated = true
                    });
                }
            }
        }

        private static void AddReadingsAfterRepair(List<LoggerReading> readings)
        {
            var repairTime = GetRepairs().Single(r => r.WaterSystem == WaterSystem.CleanWater).Date;
            foreach (var old in ZapadFourLoggerSeed.GenerateReadings())
            {
                if (old.LoggerId != InletLoggerId && old.LoggerId != BranchLoggerId) continue;
                if (old.TakenAt <= repairTime || old.TakenAt > NovemberStart) continue;

                decimal restoredPressure = 0.45m;
                if (old.LoggerId == InletLoggerId) restoredPressure = 0.12m;

                readings.Add(new LoggerReading
                {
                    LoggerId = old.LoggerId,
                    TakenAt = old.TakenAt,
                    FlowM3PerHour = old.FlowM3PerHour - 0.5m,
                    PressureBar = decimal.Round(old.PressureBar + restoredPressure, 3),
                    IsSimulated = true
                });
            }
        }

        private static decimal Flow(int loggerId, Dictionary<int, List<WaterMeterReading>> histories, DateTime time)
        {
            decimal litres = 0;
            foreach (var history in histories)
            {
                if (ZapadFourLoggerSeed.LoggerForMeter(history.Key) != loggerId) continue;

                decimal before = ZapadFourLoggerSeed.CounterAt(history.Value, time.AddMinutes(-15));
                decimal after = ZapadFourLoggerSeed.CounterAt(history.Value, time);
                litres += after - before;
            }
            return decimal.Round(litres * 0.004m, 3);
        }

    }
}
