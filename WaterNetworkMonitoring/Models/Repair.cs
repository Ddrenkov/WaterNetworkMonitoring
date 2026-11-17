namespace WaterNetworkMonitoring.Models
{
    public enum WaterSystem
    {
        CleanWater,
        Wastewater
    }

    public class Repair
    {
        public int Id { get; set; }
        // Local Bulgarian date and time, shown without a time-zone suffix.
        public DateTime Date { get; set; }
        public string Place { get; set; } = string.Empty;
        public WaterSystem WaterSystem { get; set; }
    }
}
