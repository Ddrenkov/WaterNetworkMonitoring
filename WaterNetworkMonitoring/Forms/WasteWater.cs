using GMap.NET;
using GMap.NET.WindowsForms;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace WaterNetworkMonitoring
{
    public partial class WasteWater : Form
    {
        private readonly bool offlinePreview;
        private readonly GMapOverlay streetPipes = new("wastewater-pipes");
        private readonly GMapOverlay junctions = new("wastewater-junctions");
        private readonly GMapOverlay buildingPins = new("wastewater-buildings");
        private readonly GMapOverlay loggerPins = new("wastewater-loggers");
        private List<BuildingMapData.Building> buildings = new();
        private WasteWaterNetwork? network;
        private BlagoevgradNeighbourhoods.Zone? zapad;
        private bool initialized;

        public WasteWater()
        {
            Program.ConfigureTraceOutput();
            InitializeComponent();
            Resize += (_, _) => LayoutMap();
            map.SizeChanged += (_, _) =>
            {
                if (IsHandleCreated && network != null)
                    BeginInvoke(new Action(() => { if (!IsDisposed) ResetView(); }));
            };
            LayoutMap();
        }
        internal WasteWater(bool offline) : this() => offlinePreview = offline;
        private bool IsDesigner() => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        private async void WasteWater_Load(object? sender, EventArgs e)
        {
            if (initialized || IsDesigner()) return;
            initialized = true;
            GMaps.Instance.Mode = offlinePreview ? AccessMode.CacheOnly : AccessMode.ServerAndCache;
            map.MapProvider = OSMap.Instance;
            var zones = BlagoevgradNeighbourhoods.LoadZones();
            var zone = zones.Single(z => z.Name == "Zapad");
            zapad = zone;
            buildings = BuildingMapData.Load(zones);
            map.SetZoomToFitRect(RectLatLng.FromLTRB(zone.Points.Min(p=>p[1])-.0002,zone.Points.Max(p=>p[0])+.0002,
                zone.Points.Max(p=>p[1])+.0002,zone.Points.Min(p=>p[0])-.0002));
            statusLabel.Text = "Drawing the Zapad pipe graph…";
            map.Overlays.Add(streetPipes); map.Overlays.Add(junctions);
            map.Overlays.Add(buildingPins); map.Overlays.Add(loggerPins);
            map.OnMarkerClick += (marker, args) =>
            {
                if (args.Button != MouseButtons.Left) return;
                if (marker is WasteWaterLoggerPin)
                {
                    foreach (var pin in loggerPins.Markers)
                        pin.ToolTipMode = pin == marker ? MarkerTooltipMode.Always : MarkerTooltipMode.OnMouseOver;
                    statusLabel.Text = marker.ToolTipText.Replace("\n"," · ");
                }
                else if (marker.Tag is BuildingMapData.Building building)
                    statusLabel.Text = $"Building {building.Number} · connected to the street pipe";
                map.Invalidate();
            };
            map.OnRouteClick += (route,args) =>
            {
                if (args.Button == MouseButtons.Left && route.Tag is WasteWaterNetwork.Pipe pipe)
                    statusLabel.Text = pipe.BuildingNumber.HasValue
                        ? $"Building {pipe.BuildingNumber} connection · {pipe.LengthMetres:0} m"
                        : $"{pipe.Street} · proposed pipe · {pipe.LengthMetres:0} m";
            };
            map.OnMarkerEnter += _ => map.Cursor = Cursors.Hand;
            map.OnMarkerLeave += _ => map.Cursor = Cursors.Default;
            try
            {
                var result = await Task.Run(() => WasteWaterNetwork.Create(buildings,zone));
                if (IsDisposed || Disposing) return;
                network = result;
                DrawNetwork();
                ResetView();
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
            {
                if (IsDisposed || Disposing) return;
                statusLabel.Text = "The pipe graph could not be drawn.";
                System.Diagnostics.Trace.TraceError("Waste water map: "+ex.Message);
            }
        }

        private void DrawNetwork()
        {
            if (network == null) return;
            foreach (var pipe in network.Pipes)
            {
                var route = new GMapRoute(new List<PointLatLng> { network.Nodes[pipe.From].Position,network.Nodes[pipe.To].Position },
                    $"{pipe.From}-{pipe.To}") { Tag=pipe,IsHitTestVisible=true };
                route.Stroke = new Pen(pipe.BuildingNumber.HasValue ? Color.FromArgb(155,124,91) : Color.FromArgb(111,73,43),
                    pipe.BuildingNumber.HasValue ? 1.6f : 3.2f);
                if (pipe.BuildingNumber.HasValue) route.Stroke.DashStyle = DashStyle.Dash;
                streetPipes.Routes.Add(route);
            }
            var degree = network.Pipes.SelectMany(p => new[] { p.From,p.To }).GroupBy(id=>id).ToDictionary(g=>g.Key,g=>g.Count());
            foreach (var node in network.Nodes.Values.Where(n => n.BuildingNumber==null && degree[n.Id]>=3))
                junctions.Markers.Add(new WasteWaterJunctionPin(node.Position));
            foreach (var logger in network.Loggers) loggerPins.Markers.Add(new WasteWaterLoggerPin(logger));
            UpdateBuildings();
        }
        private void UpdateBuildings()
        {
            if (network == null) return;
            foreach (var pin in buildingPins.Markers) pin.Dispose();
            buildingPins.Markers.Clear();
            foreach (var building in buildings.Where(b => network.Nodes.ContainsKey(-b.Number)))
                buildingPins.Markers.Add(new NumberedBuildingPin(building,numbersCheckBox.Checked,false,compact:true));
            map.Invalidate();
        }
        private void ResetView()
        {
            if (network == null || zapad == null) return;
            var points = zapad.Points.Select(p => new PointLatLng(p[0],p[1])).ToArray();
            map.SetZoomToFitRect(RectLatLng.FromLTRB(points.Min(p=>p.Lng)-.0005,points.Max(p=>p.Lat)+.0005,
                points.Max(p=>p.Lng)+.0005,points.Min(p=>p.Lat)-.0005));
            foreach (var overlay in map.Overlays)
            {
                foreach (var marker in overlay.Markers) map.UpdateMarkerLocalPosition(marker);
                foreach (var route in overlay.Routes) map.UpdateRouteLocalPosition(route);
            }
            foreach (var pin in loggerPins.Markers) pin.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            statusLabel.Text = $"Zapad · {network.Nodes.Values.Count(n => n.BuildingNumber.HasValue)} building connections · {network.ComponentCount} pipe groups · 3 logger positions";
            map.Refresh();
        }
        private void resetButton_Click(object? sender,EventArgs e) => ResetView();
        private void zoomInButton_Click(object? sender,EventArgs e) => map.Zoom=Math.Min(map.MaxZoom,map.Zoom+1);
        private void zoomOutButton_Click(object? sender,EventArgs e) => map.Zoom=Math.Max(map.MinZoom,map.Zoom-1);
        private void numbersCheckBox_CheckedChanged(object? sender,EventArgs e) => UpdateBuildings();
        private void attributionLabel_LinkClicked(object? sender,LinkLabelLinkClickedEventArgs e) =>
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://www.openstreetmap.org/copyright") { UseShellExecute=true });
        private void LayoutMap()
        {
            if (mapPanel == null) return;
            float scale=DeviceDpi/96f;
            int gap=(int)(24*scale);
            int width=Math.Min((int)(1200*scale),Math.Max(1,ClientSize.Width-gap*2));
            int height=Math.Min((int)(760*scale),Math.Max(1,ClientSize.Height-gap*2));
            mapPanel.Bounds=new Rectangle((ClientSize.Width-width)/2,(ClientSize.Height-height)/2,width,height);
            toolbar.WrapContents=width<650*scale;
            toolbar.Height=(int)((width<650*scale ? 76 : 42)*scale);
        }
    }
}
