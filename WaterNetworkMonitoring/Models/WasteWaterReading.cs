namespace WaterNetworkMonitoring.Models
{
    public class WasteWaterReading
    {
        public long Id { get; set; }
        public int LoggerId { get; set; }
        public DateTime TakenAt { get; set; }
        // Average flow over the preceding two hours.
        public decimal FlowM3PerHour { get; set; }
        public decimal WaterLevelMetres { get; set; }
        // True if rain occurred during any part of the preceding two-hour interval.
        public bool IsRaining { get; set; }
        public bool IsSimulated { get; set; }
        public PressureLogger Logger { get; set; } = null!;
    }
}
