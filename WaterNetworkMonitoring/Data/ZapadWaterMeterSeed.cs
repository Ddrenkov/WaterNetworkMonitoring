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
                new WaterMeter { Id = 100001, MapNumber = 49, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100002, MapNumber = 50, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100003, MapNumber = 51, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100004, MapNumber = 194, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100005, MapNumber = 195, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100006, MapNumber = 196, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100007, MapNumber = 197, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100008, MapNumber = 198, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100009, MapNumber = 199, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100010, MapNumber = 200, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100011, MapNumber = 201, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100012, MapNumber = 202, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100013, MapNumber = 203, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100014, MapNumber = 204, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100015, MapNumber = 205, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100016, MapNumber = 206, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100017, MapNumber = 207, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100018, MapNumber = 208, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100019, MapNumber = 209, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100020, MapNumber = 210, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100021, MapNumber = 211, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100022, MapNumber = 212, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100023, MapNumber = 213, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100024, MapNumber = 214, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100025, MapNumber = 215, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100026, MapNumber = 216, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100027, MapNumber = 217, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100028, MapNumber = 218, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100029, MapNumber = 219, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100030, MapNumber = 220, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100031, MapNumber = 221, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100032, MapNumber = 222, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100033, MapNumber = 223, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100034, MapNumber = 224, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100035, MapNumber = 225, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100036, MapNumber = 226, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100037, MapNumber = 227, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100038, MapNumber = 228, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100039, MapNumber = 229, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100040, MapNumber = 230, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100041, MapNumber = 231, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100042, MapNumber = 232, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100043, MapNumber = 233, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100044, MapNumber = 234, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100045, MapNumber = 235, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100046, MapNumber = 236, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100047, MapNumber = 237, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100048, MapNumber = 238, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100049, MapNumber = 239, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100050, MapNumber = 240, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100051, MapNumber = 241, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100052, MapNumber = 500, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100053, MapNumber = 605, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100054, MapNumber = 606, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100055, MapNumber = 607, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100056, MapNumber = 608, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100057, MapNumber = 725, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100058, MapNumber = 1034, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100059, MapNumber = 2131, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100060, MapNumber = 2254, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100061, MapNumber = 2530, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100062, MapNumber = 2531, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100063, MapNumber = 2801, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100064, MapNumber = 3200, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100065, MapNumber = 3201, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100066, MapNumber = 3202, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100067, MapNumber = 3374, Reading = "00000000", TakenAt = takenAt },
                new WaterMeter { Id = 100068, MapNumber = 3375, Reading = "00000000", TakenAt = takenAt },
            };
        }
    }
}
