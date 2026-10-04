using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterNetworkMonitoring.Models
{
    public class WastewaterLevelSensor : Sensor
    {
        public double CurrentLevel { get; set; }

        public double MaximumLevel { get; set; }
    }
}
