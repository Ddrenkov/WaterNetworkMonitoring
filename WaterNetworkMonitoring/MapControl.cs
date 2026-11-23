using System.ComponentModel;
using GMap.NET.WindowsForms;

namespace WaterNetworkMonitoring
{
    [ToolboxItem(false)]
    public sealed class MapControl : GMapControl
    {
        // The designer can create a map before the containing form's constructor runs.
        // Configure tracing before GMapControl's static constructor logs SQLite details.
        static MapControl() => Program.ConfigureTraceOutput();

        public MapControl() => Program.ConfigureTraceOutput();
    }
}
