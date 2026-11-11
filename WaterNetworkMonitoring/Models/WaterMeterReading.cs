namespace WaterNetworkMonitoring.Models
{
    public class WaterMeterReading
    {
        public long Id { get; set; }
        public int WaterMeterId { get; set; }
        // Cumulative litres, displayed as exactly eight digits (not interval consumption).
        public string Reading { get; set; } = "00000000";
        public DateTimeOffset TakenAt { get; set; }
        public bool IsSimulated { get; set; }
        public WaterMeter WaterMeter { get; set; } = null!;
    }
}
