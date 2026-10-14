using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaterNetworkMonitoring
{
    public partial class DashboardControl : UserControl
    {
        private readonly Panel mapPanel;
        public DashboardControl()
        {
            InitializeComponent();

            GMap.NET.GMaps.Instance.Mode =
                GMap.NET.AccessMode.ServerAndCache;

            mapPanel = new Panel { BorderStyle = BorderStyle.FixedSingle };

            var map = new GMap.NET.WindowsForms.GMapControl
            {
                Dock = DockStyle.Fill,
                MapProvider = ProjectOpenStreetMapProvider.Instance,
                Position = new GMap.NET.PointLatLng(42.0209, 23.0943),
                MinZoom = 2,
                MaxZoom = 19,
                Zoom = 14,
                CanDragMap = true,
                DragButton = MouseButtons.Left,
                ShowCenter = false
            };

            var attribution = new Label
            {
                Text = "© OpenStreetMap contributors",
                Dock = DockStyle.Bottom,
                Height = 24,
                TextAlign = ContentAlignment.MiddleRight
            };

            mapPanel.Controls.Add(map);
            mapPanel.Controls.Add(attribution);
            Controls.Add(mapPanel);
            Resize += (_, _) => LayoutMap();
            LayoutMap();
        }

        private void LayoutMap()
        {
            mapPanel.Size = new Size(
                Math.Min(1000, Math.Max(1, ClientSize.Width - 48)),
                Math.Min(500, Math.Max(1, ClientSize.Height - 72)));
            mapPanel.Location = new Point((ClientSize.Width - mapPanel.Width) / 2, 24);
        }
    }
}
