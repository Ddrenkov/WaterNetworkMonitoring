using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Data
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
                new WaterMeter { Id = 100052, MapNumber = 52, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100053, MapNumber = 53, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100054, MapNumber = 54, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100055, MapNumber = 55, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100056, MapNumber = 56, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100057, MapNumber = 57, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100058, MapNumber = 58, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100059, MapNumber = 59, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100060, MapNumber = 60, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100061, MapNumber = 61, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100062, MapNumber = 62, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100063, MapNumber = 63, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100064, MapNumber = 64, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100065, MapNumber = 65, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100066, MapNumber = 66, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100067, MapNumber = 67, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100068, MapNumber = 68, Reading = "00000000", TakenAt = takenAt },
            };
        }
    }
}
