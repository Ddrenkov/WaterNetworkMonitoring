using System.Globalization;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Data
{
    // Repeatable October example data, extended to four loggers by ZapadFourLoggerSeed.
    internal static class ZapadOctober2026Seed
    {
        internal const int InletLoggerId = 200001;
        internal const int BranchLoggerId = 200002;
        internal const int SkaptoparaMeterId = 100058;
        internal const int LitresPerPersonPerDay = 149;
        internal static readonly DateTime MonthStart = new DateTime(2026, 10, 1);
        internal static readonly DateTime MonthEnd = new DateTime(2026, 11, 1);
        internal static readonly DateTime LeakStart = new DateTime(2026, 10, 30, 6, 0, 0);
        internal static readonly DateTime SnapshotAt = new DateTime(2026, 11, 8, 22, 0, 0);

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

        private static readonly DateTime HistoryStart = new DateTime(2026, 9, 30, 12, 0, 0);
        private static readonly DateTime HistoryEnd = new DateTime(2026, 11, 1, 6, 0, 0);
        private const int MinutesPerReading = 15;
        private const int SlotsPerDay = 96;
        private const int LeakedLitresPerReading = 125;

        // ID, whole-zone measurement, normal pressure, pressure per litre, leak pressure drop.
        private sealed record LoggerSettings(int Id, bool WholeZone, decimal Pressure,
            decimal PressurePerLitre, decimal LeakPressureDrop);
        private static readonly LoggerSettings[] LoggerProfiles =
        {
            new(InletLoggerId,  true,  4.5m, 0.00005m, 0.12m),
            new(BranchLoggerId, false, 4.1m, 0.0004m,  0.45m)
        };

        private sealed record DailyVolume(string Kind, int Litres);
        private static readonly DailyVolume[] SpecialDailyVolumes =
        {
            new("roof", 0),
            new("service", 100),
            new("sports_hall", 4000),
            new("clinic", 2400),
            new("market", 900)
        };

        private sealed record ActivityHours(string Kind, int Opens, int Closes, decimal NightWeight);
        private static readonly ActivityHours[] Activities =
        {
            new("sports_hall", 8, 22, 0.04m),
            new("clinic",      7, 20, 0.08m),
            new("market",      7, 20, 0.08m),
            new("service",     7, 20, 0.08m)
        };

        private sealed record HourPeriod(int BeforeHour, decimal Weight);
        private static readonly HourPeriod[] ResidentialHours =
        {
            new(6,  0.18m),
            new(10, 1.8m),
            new(17, 0.9m),
            new(22, 1.6m),
            new(24, 0.55m)
        };

        private sealed record IntervalTotals(int TotalLitres, int BranchLitres);

        internal static SampleData Generate()
        {
            var result = new SampleData(CreateBaseLoggers(),
                new List<LoggerReading>(), new List<WaterMeterReading>());
            var counters = new int[Profiles.Length];
            for (int i = 0; i < Profiles.Length; i++)
                counters[i] = 500000 + Profiles[i].MapNumber * 10000;

            var firstDay = new DateOnly(2026, 9, 30);
            var lastDay = DateOnly.FromDateTime(SnapshotAt);
            for (var day = firstDay; day <= lastDay; day = day.AddDays(1))
            {
                var dayStart = day.ToDateTime(TimeOnly.MinValue);
                var dailyUsage = new int[Profiles.Length][];
                for (int i = 0; i < Profiles.Length; i++)
                    dailyUsage[i] = CreateDailyUsage(Profiles[i], day);

                for (int slot = 0; slot < SlotsPerDay; slot++)
                {
                    var time = dayStart.AddMinutes((slot + 1) * MinutesPerReading);
                    if (time > SnapshotAt) break;

                    var totals = AddMeterReadings(result.MeterReadings, counters, dailyUsage, slot, time);
                    if (time > HistoryStart && time <= HistoryEnd)
                        AddLoggerReadings(result.LoggerReadings, totals, day, slot, time);
                }
            }
            return result;
        }

        private static PressureLogger[] CreateBaseLoggers()
        {
            // L3 and L4 are added by ZapadFourLoggerSeed.
            return new[]
            {
                new PressureLogger
                {
                    Id = InletLoggerId, Name = "Zapad inlet",
                    ZoneName = "Zapad", IsSimulated = true
                },
                new PressureLogger
                {
                    Id = BranchLoggerId, Name = "Skaptopara supply branch", ZoneName = "Zapad",
                    ParentLoggerId = InletLoggerId, SuppliedWaterMeterId = SkaptoparaMeterId,
                    IsSimulated = true
                }
            };
        }

        private static IntervalTotals AddMeterReadings(List<WaterMeterReading> readings,
            int[] counters, int[][] dailyUsage, int slot, DateTime time)
        {
            int totalLitres = 0;
            int branchLitres = 0;
            for (int i = 0; i < Profiles.Length; i++)
            {
                var profile = Profiles[i];
                int litres = dailyUsage[i][slot];
                counters[i] += litres;
                totalLitres += litres;
                if (profile.Id == SkaptoparaMeterId) branchLitres = litres;

                if (ShouldSaveMeterReading(profile, time))
                {
                    readings.Add(new WaterMeterReading
                    {
                        Id = readings.Count + 1L,
                        WaterMeterId = profile.Id,
                        Reading = counters[i].ToString("D8", CultureInfo.InvariantCulture),
                        TakenAt = time,
                        IsSimulated = true
                    });
                }
            }
            return new IntervalTotals(totalLitres, branchLitres);
        }

        private static bool ShouldSaveMeterReading(MeterProfile profile, DateTime time)
        {
            if (time == SnapshotAt) return true;
            if (time.Minute != 0) return false;

            int morningHour = 6;
            if (profile.MapNumber % 2 == 0 && profile.Id != SkaptoparaMeterId)
                morningHour = 2;
            int eveningHour = morningHour + 12;
            if (time.Hour != morningHour && time.Hour != eveningHour) return false;

            if (time >= MonthStart && time < MonthEnd) return true;
            // The surrounding observations allow consumption at month boundaries to be estimated.
            if (time.Date == MonthStart.AddDays(-1) && time.Hour == eveningHour) return true;
            if (time.Date == MonthEnd && time.Hour == morningHour) return true;
            return false;
        }

        private static void AddLoggerReadings(List<LoggerReading> readings, IntervalTotals totals,
            DateOnly day, int slot, DateTime time)
        {
            bool leaking = time.AddMinutes(-MinutesPerReading) >= LeakStart;
            int leakedLitres = 0;
            if (leaking) leakedLitres = LeakedLitresPerReading;
            decimal variation = ((int)(Noise(InletLoggerId, day.DayNumber, slot) % 41) - 20) / 1000m;

            foreach (var logger in LoggerProfiles)
            {
                int usedLitres = totals.BranchLitres;
                if (logger.WholeZone) usedLitres = totals.TotalLitres;
                decimal pressure = logger.Pressure - usedLitres * logger.PressurePerLitre + variation;
                if (leaking) pressure -= logger.LeakPressureDrop;

                readings.Add(new LoggerReading
                {
                    Id = readings.Count + 1L,
                    LoggerId = logger.Id,
                    TakenAt = time,
                    FlowM3PerHour = (usedLitres + leakedLitres) * 0.004m,
                    PressureBar = decimal.Round(pressure, 3),
                    IsSimulated = true
                });
            }
        }

        internal static int[] CreateDailyUsage(MeterProfile profile, DateOnly day)
        {
            // Give each quarter-hour a relative weight, then divide the daily litres between them.
            var dayStart = day.ToDateTime(TimeOnly.MinValue);
            var weights = new decimal[SlotsPerDay];
            decimal totalWeight = 0;
            for (int slot = 0; slot < SlotsPerDay; slot++)
            {
                int hour = dayStart.AddMinutes(slot * MinutesPerReading).Hour;
                decimal variation = 0.90m + Noise(profile.Id, day.DayNumber, slot) % 21 / 100m;
                weights[slot] = HourWeight(profile.Kind, hour) * variation;
                totalWeight += weights[slot];
            }

            decimal dailyVariation = 0.90m + Noise(profile.Id, day.DayNumber, 101) % 21 / 100m;
            decimal dailyLitres = DailyLitres(profile) * dailyVariation;
            decimal runningWeight = 0;
            int assignedLitres = 0;
            var usage = new int[SlotsPerDay];
            for (int slot = 0; slot < SlotsPerDay; slot++)
            {
                runningWeight += weights[slot];
                int targetLitres = (int)decimal.Round(dailyLitres * runningWeight / totalWeight,
                    0, MidpointRounding.AwayFromZero);
                usage[slot] = targetLitres - assignedLitres;
                assignedLitres = targetLitres;
            }
            return usage;
        }

        internal static decimal PressureVariation(DateTime time)
        {
            int dayNumber = DateOnly.FromDateTime(time.Date).DayNumber;
            int slot = time.Hour * 4 + time.Minute / MinutesPerReading;
            return ((int)(Noise(InletLoggerId, dayNumber, slot) % 41) - 20) / 1000m;
        }

        private static decimal DailyLitres(MeterProfile profile)
        {
            foreach (var setting in SpecialDailyVolumes)
            {
                if (setting.Kind == profile.Kind) return setting.Litres;
            }
            return profile.AssumedOccupants * LitresPerPersonPerDay;
        }

        private static decimal HourWeight(string kind, int hour)
        {
            foreach (var activity in Activities)
            {
                if (activity.Kind != kind) continue;
                if (hour >= activity.Opens && hour < activity.Closes) return 1m;
                return activity.NightWeight;
            }
            foreach (var period in ResidentialHours)
            {
                if (hour < period.BeforeHour) return period.Weight;
            }
            return 0.55m;
        }

        // Keep the same repeatable variation so existing demo readings do not change.
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
    }
}
