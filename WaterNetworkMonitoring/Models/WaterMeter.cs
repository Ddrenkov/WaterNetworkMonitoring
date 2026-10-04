using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterNetworkMonitoring.Models
{
    public class WaterMeter : Sensor
    {
        public double CurrentFlow { get; set; }
        public double TotalConsumption { get; set; }
    }
}
