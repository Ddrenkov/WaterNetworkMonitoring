using System.Globalization;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Data
{
    // Frozen four-logger prototype. Historical meter counters and reading schedules are preserved.
    internal static class ZapadFourLoggerSeed
    {
        private static readonly HashSet<int> NorthMeters = new()
        {
            100001,100002,100003,100004,100005,100006,100007,100008,100009,100010,
            100011,100012,100013,100014,100015,100016,100017,100018,100021,100022,
            100023,100042,100043,100045,100046,100047,100048,100049,100050,100051,
            100053,100055,100056,100057,100061,100062,100064,100065,100067,100068
        };

        public static int LoggerForMeter(int meterId) => meterId == 100058 ? 200002 :
            NorthMeters.Contains(meterId) ? 200003 : 200004;

        public static PressureLogger[] GetLoggers() => new[]
        {
            new PressureLogger { Id=200001, Name="Zapad inlet", ZoneName="Zapad", Latitude=42.0176, Longitude=23.0795, IsSimulated=true },
            new PressureLogger { Id=200002, Name="Skaptopara supply branch", ZoneName="Zapad", ParentLoggerId=200001,
                SuppliedWaterMeterId=100058, Latitude=42.01701, Longitude=23.08290, IsSimulated=true },
            new PressureLogger { Id=200003, Name="Zapad North", ZoneName="Zapad", ParentLoggerId=200001,
                Latitude=42.0181, Longitude=23.0826, IsSimulated=true },
            new PressureLogger { Id=200004, Name="Zapad South", ZoneName="Zapad", ParentLoggerId=200001,
                Latitude=42.0161, Longitude=23.0836, IsSimulated=true }
        };

        public static List<LoggerReading> GenerateReadings()
        {
            var original = ZapadOctober2026Seed.Generate();
            var result = original.LoggerReadings;
            var histories = original.MeterReadings.GroupBy(r => r.WaterMeterId)
                .ToDictionary(g => g.Key, g => g.OrderBy(r => r.TakenAt).ToArray());
            var skaptopara = result.Where(r => r.LoggerId == 200002).ToDictionary(r => r.TakenAt);
            var newRows = new List<LoggerReading>();
            foreach (var inlet in result.Where(r => r.LoggerId == 200001 &&
                r.TakenAt > ZapadOctober2026Seed.MonthStart && r.TakenAt <= ZapadOctober2026Seed.MonthEnd))
            {
                decimal childSum = skaptopara[inlet.TakenAt].FlowM3PerHour;
                foreach (int loggerId in new[] { 200003, 200004 })
                {
                    decimal litres = histories.Where(h => LoggerForMeter(h.Key) == loggerId).Sum(h =>
                        CounterAt(h.Value, inlet.TakenAt) - CounterAt(h.Value, inlet.TakenAt.AddMinutes(-15)));
                    decimal flow = decimal.Round(litres * .004m, 3);
                    childSum += flow;
                    newRows.Add(new LoggerReading
                    {
                        LoggerId=loggerId, TakenAt=inlet.TakenAt, FlowM3PerHour=flow,
                        PressureBar=decimal.Round(4.3m-flow*.005m+(inlet.TakenAt.Minute/15-1)*.003m,3), IsSimulated=true
                    });
                }
                // Every outlet is covered. The Skaptopara loss is downstream of L2, not on the common inlet.
                inlet.FlowM3PerHour = childSum;
            }
            result.AddRange(newRows);
            return result;
        }

        private static decimal CounterAt(WaterMeterReading[] rows, DateTimeOffset time)
        {
            var left = rows.Last(r => r.TakenAt <= time);
            if (left.TakenAt == time) return decimal.Parse(left.Reading, CultureInfo.InvariantCulture);
            var right = rows.First(r => r.TakenAt > time);
            decimal fraction = (decimal)((time-left.TakenAt).TotalSeconds/(right.TakenAt-left.TakenAt).TotalSeconds);
            decimal start = decimal.Parse(left.Reading, CultureInfo.InvariantCulture);
            // Linear interpolation is a demo estimate between the existing 12-hour meter observations.
            return start+(decimal.Parse(right.Reading, CultureInfo.InvariantCulture)-start)*fraction;
        }
    }
}
