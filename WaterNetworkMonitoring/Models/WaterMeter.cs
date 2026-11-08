using System;

namespace WaterNetworkMonitoring.Models
{
    public class WaterMeter
    {
        public int Id { get; set; }
        public int MapNumber { get; set; }
        public string Reading { get; set; } = "00000000";
        public DateTimeOffset TakenAt { get; set; }
    }
}
