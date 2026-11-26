using System.Globalization;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Data
{
    internal static class ZapadFourLoggerSeed
    {
        private const int InletLoggerId = 200001;
        private const int BranchLoggerId = 200002;
        private const int NorthLoggerId = 200003;
        private const int SouthLoggerId = 200004;
        private const int SkaptoparaMeterId = 100058;

        private static readonly HashSet<int> NorthMeters = new()
        {
            100001, 100002, 100003, 100004, 100005, 100006, 100007, 100008, 100009, 100010,
            100011, 100012, 100013, 100014, 100015, 100016, 100017, 100018, 100021, 100022,
            100023, 100042, 100043, 100045, 100046, 100047, 100048, 100049, 100050, 100051,
            100053, 100055, 100056, 100057, 100061, 100062, 100064, 100065, 100067, 100068
        };

        private sealed record BranchSettings(int LoggerId, decimal Pressure, decimal PressurePerFlow);
        private static readonly BranchSettings[] Branches =
        {
            new(NorthLoggerId, 4.3m, 0.005m),
            new(SouthLoggerId, 4.3m, 0.005m)
        };

        public static int LoggerForMeter(int meterId)
        {
            if (meterId == SkaptoparaMeterId) return BranchLoggerId;
            if (NorthMeters.Contains(meterId)) return NorthLoggerId;
            return SouthLoggerId;
        }

        public static PressureLogger[] GetLoggers()
        {
            return new[]
            {
                new PressureLogger
                {
                    Id = InletLoggerId, Name = "Zapad inlet", ZoneName = "Zapad",
                    Latitude = 42.0176, Longitude = 23.0795, IsSimulated = true
                },
                new PressureLogger
                {
                    Id = BranchLoggerId, Name = "Skaptopara supply branch", ZoneName = "Zapad",
                    ParentLoggerId = InletLoggerId, SuppliedWaterMeterId = SkaptoparaMeterId,
                    Latitude = 42.01701, Longitude = 23.08290, IsSimulated = true
                },
                new PressureLogger
                {
                    Id = NorthLoggerId, Name = "Zapad North", ZoneName = "Zapad",
                    ParentLoggerId = InletLoggerId,
                    Latitude = 42.0181, Longitude = 23.0826, IsSimulated = true
                },
                new PressureLogger
                {
                    Id = SouthLoggerId, Name = "Zapad South", ZoneName = "Zapad",
                    ParentLoggerId = InletLoggerId,
                    Latitude = 42.0161, Longitude = 23.0836, IsSimulated = true
                }
            };
        }

        public static List<LoggerReading> GenerateReadings()
        {
            // The October seed provides the original inlet and Skaptopara readings.
            var october = ZapadOctober2026Seed.Generate();
            var histories = GroupMeterReadings(october.MeterReadings);
            var skaptoparaReadings = new Dictionary<DateTime, LoggerReading>();
            foreach (var reading in october.LoggerReadings)
            {
                if (reading.LoggerId == BranchLoggerId)
                    skaptoparaReadings.Add(reading.TakenAt, reading);
            }

            var addedReadings = new List<LoggerReading>();
            foreach (var inlet in october.LoggerReadings)
            {
                if (inlet.LoggerId != InletLoggerId) continue;
                if (inlet.TakenAt <= ZapadOctober2026Seed.MonthStart ||
                    inlet.TakenAt > ZapadOctober2026Seed.MonthEnd) continue;

                decimal totalFlow = skaptoparaReadings[inlet.TakenAt].FlowM3PerHour;
                foreach (var branch in Branches)
                {
                    decimal flow = FlowForBranch(branch.LoggerId, histories, inlet.TakenAt);
                    decimal pressureVariation = (inlet.TakenAt.Minute / 15 - 1) * 0.003m;
                    decimal pressure = branch.Pressure - flow * branch.PressurePerFlow + pressureVariation;
                    addedReadings.Add(new LoggerReading
                    {
                        LoggerId = branch.LoggerId,
                        TakenAt = inlet.TakenAt,
                        FlowM3PerHour = flow,
                        PressureBar = decimal.Round(pressure, 3),
                        IsSimulated = true
                    });
                    totalFlow += flow;
                }
                // All three branch flows pass through the common inlet.
                inlet.FlowM3PerHour = totalFlow;
            }
            october.LoggerReadings.AddRange(addedReadings);
            return october.LoggerReadings;
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
            foreach (var history in histories.Values)
                history.Sort((first, second) => first.TakenAt.CompareTo(second.TakenAt));
            return histories;
        }

        private static decimal FlowForBranch(int loggerId,
            Dictionary<int, List<WaterMeterReading>> histories, DateTime time)
        {
            decimal litres = 0;
            foreach (var history in histories)
            {
                if (LoggerForMeter(history.Key) != loggerId) continue;
                decimal before = CounterAt(history.Value, time.AddMinutes(-15));
                decimal after = CounterAt(history.Value, time);
                litres += after - before;
            }
            // Convert litres in 15 minutes to cubic metres per hour.
            return decimal.Round(litres * 0.004m, 3);
        }

        internal static decimal CounterAt(IReadOnlyList<WaterMeterReading> readings, DateTime time)
        {
            WaterMeterReading? before = null;
            WaterMeterReading? after = null;
            foreach (var reading in readings)
            {
                if (reading.TakenAt <= time) before = reading;
                if (after == null && reading.TakenAt > time) after = reading;
            }
            if (before == null) throw new InvalidOperationException("An earlier meter reading is needed.");

            decimal firstCounter = decimal.Parse(before.Reading, CultureInfo.InvariantCulture);
            if (before.TakenAt == time) return firstCounter;
            if (after == null) throw new InvalidOperationException("A later meter reading is needed.");

            decimal lastCounter = decimal.Parse(after.Reading, CultureInfo.InvariantCulture);
            decimal fraction = (decimal)((time - before.TakenAt).TotalSeconds /
                (after.TakenAt - before.TakenAt).TotalSeconds);
            return firstCounter + (lastCounter - firstCounter) * fraction;
        }
    }
}
