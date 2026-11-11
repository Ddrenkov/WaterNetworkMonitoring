namespace WaterNetworkMonitoring.Models
{
    public class LoggerReading
    {
        public long Id { get; set; }
        public int LoggerId { get; set; }
        public DateTimeOffset TakenAt { get; set; }
        // Average flow during the preceding 15 minutes, in cubic metres per hour.
        public decimal FlowM3PerHour { get; set; }
        public decimal PressureBar { get; set; }
        public bool IsSimulated { get; set; }
        public PressureLogger Logger { get; set; } = null!;
    }
}
