// Frozen generators used only by migrations written before plain local timestamps.
using System.Globalization;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Migrations.Historical
{
    internal static class ZapadWaterMeterSeed
    {
        public static WaterMeter[] GetReadings()
        {
            // Zapad prototype readings, taken on 8 November 2026 at 10 PM (UTC+02:00).
            var takenAt = new DateTimeOffset(2026, 11, 8, 22, 0, 0, TimeSpan.FromHours(2));

            return new WaterMeter[]
            {
                new WaterMeter { Id = 100001, MapNumber = 1, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100002, MapNumber = 2, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100003, MapNumber = 3, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100004, MapNumber = 4, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100005, MapNumber = 5, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100006, MapNumber = 6, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100007, MapNumber = 7, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100008, MapNumber = 8, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100009, MapNumber = 9, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100010, MapNumber = 10, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100011, MapNumber = 11, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100012, MapNumber = 12, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100013, MapNumber = 13, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100014, MapNumber = 14, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100015, MapNumber = 15, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100016, MapNumber = 16, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100017, MapNumber = 17, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100018, MapNumber = 18, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100019, MapNumber = 19, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100020, MapNumber = 20, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100021, MapNumber = 21, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100022, MapNumber = 22, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100023, MapNumber = 23, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100024, MapNumber = 24, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100025, MapNumber = 25, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100026, MapNumber = 26, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100027, MapNumber = 27, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100028, MapNumber = 28, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100029, MapNumber = 29, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100030, MapNumber = 30, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100031, MapNumber = 31, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100032, MapNumber = 32, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100033, MapNumber = 33, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100034, MapNumber = 34, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100035, MapNumber = 35, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100036, MapNumber = 36, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100037, MapNumber = 37, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100038, MapNumber = 38, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100039, MapNumber = 39, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100040, MapNumber = 40, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100041, MapNumber = 41, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100042, MapNumber = 42, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100043, MapNumber = 43, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100044, MapNumber = 44, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100045, MapNumber = 45, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100046, MapNumber = 46, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100047, MapNumber = 47, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100048, MapNumber = 48, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100049, MapNumber = 49, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100050, MapNumber = 50, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100051, MapNumber = 51, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100053, MapNumber = 52, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100054, MapNumber = 53, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100055, MapNumber = 54, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100056, MapNumber = 55, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100057, MapNumber = 56, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100058, MapNumber = 57, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100059, MapNumber = 58, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100060, MapNumber = 59, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100061, MapNumber = 60, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100062, MapNumber = 61, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100063, MapNumber = 62, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100064, MapNumber = 63, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100065, MapNumber = 64, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100066, MapNumber = 65, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100067, MapNumber = 66, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100068, MapNumber = 67, Reading = "00000000", TakenAt = takenAt },
            };
        }
    }
}

namespace WaterNetworkMonitoring.Migrations.Historical
{
    // Frozen sample-data version used by AddZapadOctober2026Readings. Do not change after release:
    // a later scenario belongs in a new migration. No map JSON, current date, network or Random dependency.
    internal static class ZapadOctober2026Seed
    {
        internal const int InletLoggerId = 200001;
        internal const int BranchLoggerId = 200002;
        internal const int SkaptoparaMeterId = 100058;
        internal const int LitresPerPersonPerDay = 149;
        internal static readonly DateTimeOffset MonthStart = Local(2026, 10, 1);
        internal static readonly DateTimeOffset MonthEnd = Local(2026, 11, 1);
        internal static readonly DateTimeOffset LeakStart = Local(2026, 10, 30, 6);
        internal static readonly DateTimeOffset SnapshotAt = Local(2026, 11, 8, 22);

        internal sealed record MeterProfile(int Id, int MapNumber, long OsmId, string Kind, int AssumedOccupants);
        internal sealed record SampleData(PressureLogger[] Loggers, List<LoggerReading> LoggerReadings,
            List<WaterMeterReading> MeterReadings);

        // Assumed occupancy is for the demo; OSM footprints do not establish real residents or meters.
        internal static readonly MeterProfile[] Profiles =
        {
            new(100001, 1, 179455066, "roof", 0),
            new(100002, 2, 179455068, "yes", 10),
            new(100003, 3, 179455074, "apartments", 63),
            new(100004, 4, 392727286, "apartments", 76),
            new(100005, 5, 392727316, "apartments", 89),
            new(100006, 6, 392727380, "apartments", 102),
            new(100007, 7, 392727386, "apartments", 115),
            new(100008, 8, 392727393, "apartments", 31),
            new(100009, 9, 392727561, "apartments", 44),
            new(100010, 10, 392727660, "apartments", 57),
            new(100011, 11, 392727741, "apartments", 70),
            new(100012, 12, 392727760, "apartments", 83),
            new(100013, 13, 392727769, "apartments", 96),
            new(100014, 14, 392727803, "apartments", 109),
            new(100015, 15, 392727806, "apartments", 25),
            new(100016, 16, 392727835, "apartments", 38),
            new(100017, 17, 392727840, "apartments", 51),
            new(100018, 18, 392727850, "apartments", 64),
            new(100019, 19, 392727884, "apartments", 77),
            new(100020, 20, 392727895, "apartments", 90),
            new(100021, 21, 392727907, "apartments", 103),
            new(100022, 22, 392728197, "apartments", 116),
            new(100023, 23, 392728199, "apartments", 32),
            new(100024, 24, 392728392, "apartments", 45),
            new(100025, 25, 392728541, "apartments", 58),
            new(100026, 26, 392728567, "apartments", 71),
            new(100027, 27, 392728588, "apartments", 84),
            new(100028, 28, 392728592, "apartments", 97),
            new(100029, 29, 392728740, "apartments", 110),
            new(100030, 30, 392728789, "apartments", 26),
            new(100031, 31, 392728798, "apartments", 39),
            new(100032, 32, 392728837, "apartments", 52),
            new(100033, 33, 392729176, "apartments", 65),
            new(100034, 34, 392729468, "apartments", 78),
            new(100035, 35, 392729477, "apartments", 91),
            new(100036, 36, 392729514, "apartments", 104),
            new(100037, 37, 392729806, "apartments", 117),
            new(100038, 38, 392730475, "apartments", 33),
            new(100039, 39, 392730764, "apartments", 46),
            new(100040, 40, 392730992, "apartments", 59),
            new(100041, 41, 392730999, "apartments", 72),
            new(100042, 42, 392731036, "apartments", 85),
            new(100043, 43, 392731047, "apartments", 98),
            new(100044, 44, 392731227, "apartments", 111),
            new(100045, 45, 392731284, "apartments", 27),
            new(100046, 46, 392731342, "apartments", 40),
            new(100047, 47, 392731354, "apartments", 53),
            new(100048, 48, 392731367, "apartments", 66),
            new(100049, 49, 392731416, "apartments", 79),
            new(100050, 50, 392731422, "apartments", 92),
            new(100051, 51, 392731614, "apartments", 105),
            new(100053, 52, 655151349, "apartments", 118),
            new(100054, 53, 655151350, "apartments", 34),
            new(100055, 54, 655151351, "apartments", 47),
            new(100056, 55, 655151353, "apartments", 60),
            new(100057, 56, 787194353, "clinic", 0),
            new(100058, 57, 992665907, "sports_hall", 0),
            new(100059, 58, 1156645762, "yes", 7),
            new(100060, 59, 1158815837, "yes", 10),
            new(100061, 60, 1163889891, "yes", 4),
            new(100062, 61, 1163889892, "yes", 7),
            new(100063, 62, 1190394430, "market", 0),
            new(100064, 63, 1509244942, "yes", 4),
            new(100065, 64, 1509244943, "service", 0),
            new(100066, 65, 1509307146, "yes", 10),
            new(100067, 66, 1555328967, "yes", 4),
            new(100068, 67, 1555328968, "yes", 7),
        };

        internal static SampleData Generate()
        {
            var loggers = new[]
            {
                new PressureLogger { Id = InletLoggerId, Name = "Zapad inlet", ZoneName = "Zapad", IsSimulated = true },
                new PressureLogger { Id = BranchLoggerId, Name = "Skaptopara supply branch", ZoneName = "Zapad",
                    ParentLoggerId = InletLoggerId, SuppliedWaterMeterId = SkaptoparaMeterId, IsSimulated = true }
            };
            var loggerReadings = new List<LoggerReading>();
            var meterReadings = new List<WaterMeterReading>();
            var counters = Profiles.Select(p => 500000 + p.MapNumber * 10000).ToArray();
            var historyStart = Local(2026, 9, 30, 12);
            var historyEnd = Local(2026, 11, 1, 6);
            long loggerReadingId = 1, meterReadingId = 1;

            for (var day = new DateOnly(2026, 9, 30); day <= new DateOnly(2026, 11, 8); day = day.AddDays(1))
            {
                var dayStart = Local(day.Year, day.Month, day.Day).ToUniversalTime();
                var next = day.AddDays(1);
                var dayEnd = Local(next.Year, next.Month, next.Day).ToUniversalTime();
                int slots = (int)((dayEnd - dayStart).TotalMinutes / 15);
                var usage = new int[Profiles.Length, slots];

                // Allocate each day's volume across real quarter hours. The DST day has 100 slots.
                for (int i = 0; i < Profiles.Length; i++)
                {
                    var profile = Profiles[i];
                    var weights = new decimal[slots];
                    for (int slot = 0; slot < slots; slot++)
                    {
                        int hour = BulgariaTime(dayStart.AddMinutes(slot * 15)).Hour;
                        weights[slot] = HourWeight(profile.Kind, hour) *
                            (0.90m + Noise(profile.Id, day.DayNumber, slot) % 21 / 100m);
                    }
                    decimal dailyLitres = DailyLitres(profile) *
                        (0.90m + Noise(profile.Id, day.DayNumber, 101) % 21 / 100m);
                    decimal totalWeight = weights.Sum(), runningWeight = 0;
                    int assigned = 0;
                    for (int slot = 0; slot < slots; slot++)
                    {
                        runningWeight += weights[slot];
                        int target = (int)decimal.Round(dailyLitres * runningWeight / totalWeight, 0,
                            MidpointRounding.AwayFromZero);
                        usage[i, slot] = target - assigned;
                        assigned = target;
                    }
                }

                for (int slot = 0; slot < slots; slot++)
                {
                    var intervalStart = dayStart.AddMinutes(slot * 15);
                    var takenAt = BulgariaTime(intervalStart.AddMinutes(15));
                    if (takenAt > SnapshotAt) break;
                    int totalLitres = 0, branchLitres = 0;
                    for (int i = 0; i < Profiles.Length; i++)
                    {
                        var profile = Profiles[i];
                        int litres = usage[i, slot];
                        counters[i] += litres;
                        totalLitres += litres;
                        if (profile.Id == SkaptoparaMeterId) branchLitres = litres;

                        // Alternate fixed local schedules: 02/14 and 06/18. Skaptopara uses 06/18.
                        int firstHour = profile.MapNumber % 2 == 0 && profile.Id != SkaptoparaMeterId ? 2 : 6;
                        bool scheduled = takenAt.Minute == 0 &&
                            (takenAt.Hour == firstHour || takenAt.Hour == firstHour + 12);
                        bool inOctober = takenAt >= MonthStart && takenAt < MonthEnd;
                        bool boundary = (takenAt.Date == new DateTime(2026, 9, 30) && takenAt.Hour == firstHour + 12) ||
                            (takenAt.Date == new DateTime(2026, 11, 1) && takenAt.Hour == firstHour);
                        if ((scheduled && (inOctober || boundary)) || takenAt == SnapshotAt)
                        {
                            meterReadings.Add(new WaterMeterReading
                            {
                                Id = meterReadingId++, WaterMeterId = profile.Id,
                                Reading = counters[i].ToString("D8", CultureInfo.InvariantCulture),
                                TakenAt = takenAt, IsSimulated = true
                            });
                        }
                    }

                    if (takenAt > historyStart && takenAt <= historyEnd)
                    {
                        bool leaking = intervalStart >= LeakStart;
                        int leakedLitres = leaking ? 125 : 0; // 500 L/hour, upstream of the building meter.
                        decimal jitter = ((int)(Noise(200001, day.DayNumber, slot) % 41) - 20) / 1000m;
                        loggerReadings.Add(new LoggerReading
                        {
                            Id = loggerReadingId++, LoggerId = InletLoggerId, TakenAt = takenAt,
                            FlowM3PerHour = (totalLitres + leakedLitres) * 0.004m,
                            PressureBar = decimal.Round(4.5m - totalLitres * 0.00005m + jitter -
                                (leaking ? 0.12m : 0), 3), IsSimulated = true
                        });
                        loggerReadings.Add(new LoggerReading
                        {
                            Id = loggerReadingId++, LoggerId = BranchLoggerId, TakenAt = takenAt,
                            FlowM3PerHour = (branchLitres + leakedLitres) * 0.004m,
                            PressureBar = decimal.Round(4.1m - branchLitres * 0.0004m + jitter -
                                (leaking ? 0.45m : 0), 3), IsSimulated = true
                        });
                    }
                }
            }
            return new SampleData(loggers, loggerReadings, meterReadings);
        }

        private static decimal DailyLitres(MeterProfile profile) => profile.Kind switch
        {
            "roof" => 0, "service" => 100, "sports_hall" => 4000,
            "clinic" => 2400, "market" => 900,
            _ => profile.AssumedOccupants * LitresPerPersonPerDay
        };

        private static decimal HourWeight(string kind, int hour)
        {
            if (kind == "sports_hall") return hour is >= 8 and < 22 ? 1m : 0.04m;
            if (kind is "clinic" or "market" or "service") return hour is >= 7 and < 20 ? 1m : 0.08m;
            return hour switch { < 6 => 0.18m, < 10 => 1.8m, < 17 => 0.9m, < 22 => 1.6m, _ => 0.55m };
        }

        // A fixed integer mixer keeps the dataset repeatable across runs and .NET versions.
        private static uint Noise(int meterId, int dayNumber, int slot)
        {
            unchecked
            {
                uint value = (uint)(meterId * 73856093) ^ (uint)(dayNumber * 19349663) ^ (uint)(slot * 83492791);
                value ^= value >> 16;
                value *= 0x7feb352d;
                value ^= value >> 15;
                return value;
            }
        }

        // Frozen Europe/Sofia offsets for this dataset only: DST ends 25 October 2026 at 01:00 UTC.
        private static DateTimeOffset Local(int year, int month, int day, int hour = 0) =>
            new(year, month, day, hour, 0, 0, TimeSpan.FromHours(
                new DateTime(year, month, day, hour, 0, 0) < new DateTime(2026, 10, 25, 4, 0, 0) ? 3 : 2));

        private static DateTimeOffset BulgariaTime(DateTimeOffset utc) => utc.ToOffset(TimeSpan.FromHours(
            utc < new DateTimeOffset(2026, 10, 25, 1, 0, 0, TimeSpan.Zero) ? 3 : 2));
    }
}

namespace WaterNetworkMonitoring.Migrations.Historical
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

namespace WaterNetworkMonitoring.Migrations.Historical
{
    internal static class SepNovSeed
    {
        internal static readonly DateTimeOffset SeptemberStart = Local(new DateTime(2026, 9, 1));
        internal static readonly DateTimeOffset OctoberStart = Local(new DateTime(2026, 10, 1));
        internal static readonly DateTimeOffset NovemberStart = Local(new DateTime(2026, 11, 1));
        internal static readonly DateTimeOffset DecemberStart = Local(new DateTime(2026, 12, 1));
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
            var branchUsage = new Dictionary<DateTimeOffset, int>();
            for (var day = firstDay; day <= new DateOnly(2026, 12, 1); day = day.AddDays(1))
            {
                var start = Local(day.ToDateTime(TimeOnly.MinValue)).ToUniversalTime();
                var usage = profiles.Select(p => Usage(p, day)).ToArray();
                for (int slot = 0; slot < usage[0].Length; slot++)
                {
                    var time = BulgariaTime(start.AddMinutes((slot + 1) * 15));
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
                for (var utc = range.Item1.ToUniversalTime().AddMinutes(15); utc <= range.Item2; utc = utc.AddMinutes(15))
                {
                    var time = BulgariaTime(utc);
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
            var repairTime = Local(repair.Date);
            foreach (var old in ZapadFourLoggerSeed.GenerateReadings().Where(r =>
                r.LoggerId is 200001 or 200002 && r.TakenAt > repairTime && r.TakenAt <= NovemberStart))
                loggers.Add(Row(old.LoggerId, old.TakenAt, old.FlowM3PerHour - .5m,
                    old.PressureBar + (old.LoggerId == 200001 ? .12m : .45m)));
            return new SampleData(loggers, meters.Where(r => r.TakenAt < OctoberStart || r.TakenAt >= NovemberStart).ToList());
        }

        private static LoggerReading Row(int id, DateTimeOffset time, decimal flow, decimal pressure) =>
            new() { LoggerId = id, TakenAt = time, FlowM3PerHour = flow,
                PressureBar = decimal.Round(pressure, 3), IsSimulated = true };

        private static decimal Flow(int loggerId, Dictionary<int, WaterMeterReading[]> histories, DateTimeOffset time) =>
            decimal.Round(histories.Where(h => ZapadFourLoggerSeed.LoggerForMeter(h.Key) == loggerId)
                .Sum(h => CounterAt(h.Value, time) - CounterAt(h.Value, time.AddMinutes(-15))) * .004m, 3);

        private static decimal CounterAt(WaterMeterReading[] rows, DateTimeOffset time)
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
            var start = Local(day.ToDateTime(TimeOnly.MinValue)).ToUniversalTime();
            var end = Local(day.AddDays(1).ToDateTime(TimeOnly.MinValue)).ToUniversalTime();
            int slots = (int)(end - start).TotalMinutes / 15;
            var weights = Enumerable.Range(0, slots).Select(slot =>
                HourWeight(profile.Kind, BulgariaTime(start.AddMinutes(slot * 15)).Hour) *
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
        private static DateTimeOffset Local(DateTime time) => new(time,
            TimeSpan.FromHours(time < new DateTime(2026, 10, 25, 4, 0, 0) ? 3 : 2));
        private static DateTimeOffset BulgariaTime(DateTimeOffset utc) => utc.ToOffset(TimeSpan.FromHours(
            utc < new DateTimeOffset(2026, 10, 25, 1, 0, 0, TimeSpan.Zero) ? 3 : 2));
    }
}

namespace WaterNetworkMonitoring.Migrations.Historical
{
    // Fixed September example data for the wastewater migrations.
    internal static class WasteWaterSepSeed
    {
        public static readonly DateTimeOffset Start = new(2026, 9, 1, 0, 0, 0, TimeSpan.FromHours(3));
        public static readonly DateTimeOffset End = new(2026, 10, 1, 0, 0, 0, TimeSpan.FromHours(3));

        public static PressureLogger[] GetLoggers() => new[]
        {
            new PressureLogger { Id = 300001, Name = "W1", ZoneName = "Zapad", WaterSystem = WaterSystem.Wastewater,
                Latitude = 42.0160494629729, Longitude = 23.083341027286686, IsSimulated = true },
            new PressureLogger { Id = 300002, Name = "W2", ZoneName = "Zapad", WaterSystem = WaterSystem.Wastewater,
                Latitude = 42.01735678485204, Longitude = 23.083793084477268, IsSimulated = true },
            new PressureLogger { Id = 300003, Name = "W3", ZoneName = "Zapad", WaterSystem = WaterSystem.Wastewater,
                Latitude = 42.017898909010135, Longitude = 23.084494568877314, IsSimulated = true }
        };

        internal sealed record RainPeriod(DateTimeOffset Start, DateTimeOffset End);
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

namespace WaterNetworkMonitoring.Migrations.Historical
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

namespace WaterNetworkMonitoring.Migrations.Historical
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

namespace WaterNetworkMonitoring.Migrations.Historical
{
    internal sealed class WaterMeter
    {
        public int Id { get; set; }
        public int MapNumber { get; set; }
        public string Reading { get; set; } = "00000000";
        public DateTimeOffset TakenAt { get; set; }
    }
    internal sealed class LoggerReading
    {
        public long Id { get; set; }
        public int LoggerId { get; set; }
        public DateTimeOffset TakenAt { get; set; }
        public decimal FlowM3PerHour { get; set; }
        public decimal PressureBar { get; set; }
        public bool IsSimulated { get; set; }
    }
    internal sealed class WaterMeterReading
    {
        public long Id { get; set; }
        public int WaterMeterId { get; set; }
        public string Reading { get; set; } = "00000000";
        public DateTimeOffset TakenAt { get; set; }
        public bool IsSimulated { get; set; }
    }
    internal sealed class WasteWaterReading
    {
        public int LoggerId { get; set; }
        public DateTimeOffset TakenAt { get; set; }
        public decimal FlowM3PerHour { get; set; }
        public decimal WaterLevelMetres { get; set; }
        public bool IsRaining { get; set; }
        public bool IsSimulated { get; set; }
    }
}
