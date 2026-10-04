using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterNetworkMonitoring.Models
{
    public class PressureLogger : Sensor
    {
        public double CurrentPressure { get; set; }

        public double MinPressure { get; set; }

        public double MaxPressure { get; set; }
    }
}
