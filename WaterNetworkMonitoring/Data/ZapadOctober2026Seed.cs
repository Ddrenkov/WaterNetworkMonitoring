using System.Globalization;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Data
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
