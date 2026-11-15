using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterNetworkMonitoring.Models
{
    public class PressureLogger
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ZoneName { get; set; } = string.Empty;
        public int? ParentLoggerId { get; set; }
        public int? SuppliedWaterMeterId { get; set; }
        public bool IsSimulated { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public ICollection<WaterMeter> WaterMeters { get; set; } = new List<WaterMeter>();
        public PressureLogger? ParentLogger { get; set; }
        public WaterMeter? SuppliedWaterMeter { get; set; }
    }
}
