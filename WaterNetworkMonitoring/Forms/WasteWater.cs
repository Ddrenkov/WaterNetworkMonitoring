using GMap.NET;
using GMap.NET.WindowsForms;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using WaterNetworkMonitoring.Models;
using WaterNetworkMonitoring.Services;

namespace WaterNetworkMonitoring
{
    public partial class WasteWater : Form
    {
        private readonly WasteWaterData? previewData;
        private CancellationTokenSource? loadCancellation;
        private string loggerLoadMessage = "No saved wastewater loggers";
        private readonly GMapOverlay streetPipes = new("wastewater-pipes");
        private readonly GMapOverlay junctions = new("wastewater-junctions");
        private readonly GMapOverlay buildingPins = new("wastewater-buildings");
        private readonly GMapOverlay loggerPins = new("wastewater-loggers");
        private List<BuildingMapData.Building> buildings = new();
        private WasteWaterNetwork? network;
        private BlagoevgradNeighbourhoods.Zone? zapad;
        private bool initialized;
        private bool layingOut;

        public WasteWater()
        {
            Program.ConfigureTraceOutput();
            InitializeComponent();
            if (!IsDesigner()) maintenancePanel.Visible = false;
            Resize += (_, _) => LayoutMap();
            map.SizeChanged += (_, _) =>
            {
                if (IsHandleCreated && network != null)
                    BeginInvoke(new Action(() => { if (!IsDisposed) ResetView(); }));
            };
            LayoutMap();
        }
        internal WasteWater(WasteWaterData preview) : this() => previewData = preview;
        private bool IsDesigner() => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        private async void WasteWater_Load(object? sender, EventArgs e)
        {
            if (initialized || IsDesigner()) return;
            initialized = true;
            GMaps.Instance.Mode = previewData != null ? AccessMode.CacheOnly : AccessMode.ServerAndCache;
            map.MapProvider = OSMap.Instance;
            var zones = BlagoevgradNeighbourhoods.LoadZones();
            var zone = zones.Single(z => z.Name == "Zapad");
            zapad = zone;
            buildings = BuildingMapData.Load();
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
                var result = await Task.Run(() => WasteWaterNetwork.Load());
                if (IsDisposed || Disposing) return;
                network = result;
                DrawNetwork();
                ResetView();
                await LoadLoggersAsync();
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
            ShowStatus();
            map.Refresh();
        }
        private void ShowStatus()
        {
            if (network != null)
                statusLabel.Text = $"Zapad · {network.Nodes.Values.Count(n => n.BuildingNumber.HasValue)} building connections · {network.ComponentCount} pipe groups · {loggerLoadMessage}";
        }
        private void ShowMaintenance(PressureLogger[] loggers, List<WasteWaterMaintenance.Alert> alerts,
            List<WasteWaterReading> readings)
        {
            maintenanceGrid.Rows.Clear();
            maintenancePanel.Visible = alerts.Count > 0;
            maintenanceTitleLabel.Text = alerts.Count == 0 ? "" : "Possible blockage · cleaning required";
            maintenanceDetailsLabel.Text = string.Join(" · ", alerts.Select(a =>
                $"{loggers.First(l => l.Id == a.LoggerId).Name} ({a.LoggerId})"));
            flowChangeLabel.Text = alerts.Count == 0 ? "" : "Flow: " + string.Join(" · ", alerts.Select(a =>
                $"{a.FlowDropPercent:0}% below normal"));
            levelChangeLabel.Text = alerts.Count == 0 ? "" : "Water level: " + string.Join(" · ", alerts.Select(a =>
                $"+{a.ExtraLevelMetres * 100:0} cm above normal"));
            checkedAtLabel.Text = alerts.Count == 0 ? "" : $"Last check: {alerts.Max(a => a.TakenAt):dd MMM yyyy HH:mm:ss}";
            maintenanceGrid.Columns["Logger"].Visible = alerts.Count > 1;
            var ids = alerts.Select(a => a.LoggerId).ToHashSet();
            var recent = readings.Where(r => ids.Contains(r.LoggerId)).GroupBy(r => r.LoggerId)
                .SelectMany(g => g.OrderByDescending(r => r.TakenAt).Take(6))
                .OrderBy(r => r.TakenAt).ThenBy(r => r.LoggerId);
            foreach (var reading in recent)
            {
                var logger = loggers.First(l => l.Id == reading.LoggerId);
                int row = maintenanceGrid.Rows.Add(logger.Name, reading.TakenAt,
                    reading.FlowM3PerHour, reading.WaterLevelMetres, reading.IsRaining ? "Yes" : "No");
                maintenanceGrid.Rows[row].Tag = reading;
            }
            maintenanceGrid.ClearSelection();
            LayoutMap();
        }
        private async void refreshLoggersButton_Click(object? sender,EventArgs e) => await LoadLoggersAsync();
        private async Task LoadLoggersAsync()
        {
            loadCancellation?.Cancel();
            loadCancellation?.Dispose();
            loadCancellation = new CancellationTokenSource();
            var token = loadCancellation.Token;
            refreshLoggersButton.Enabled = false;
            loggerLoadMessage = "Loading wastewater loggers…";
            ShowStatus();
            try
            {
                var data = previewData ?? await WasteWaterData.LoadAsync(token);
                if (IsDisposed || Disposing || token.IsCancellationRequested) return;
                foreach (var pin in loggerPins.Markers) pin.Dispose();
                loggerPins.Markers.Clear();
                var loggers = data.Loggers.Where(l => l.WaterSystem == WaterSystem.Wastewater && l.ZoneName == "Zapad").ToArray();
                var ids = loggers.Select(l => l.Id).ToHashSet();
                var alerts = WasteWaterMaintenance.FindAlerts(data.Readings.Where(r => ids.Contains(r.LoggerId)));
                ShowMaintenance(loggers, alerts, data.Readings);
                foreach (var logger in loggers.Where(l => double.IsFinite(l.Latitude) && double.IsFinite(l.Longitude) && l.Latitude != 0 && l.Longitude != 0 &&
                    Math.Abs(l.Latitude) <= 90 && Math.Abs(l.Longitude) <= 180))
                {
                    loggerPins.Markers.Add(new WasteWaterLoggerPin(logger));
                }
                loggerLoadMessage = $"{loggerPins.Markers.Count} saved wastewater loggers";
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) when (ex is Microsoft.Data.SqlClient.SqlException or InvalidOperationException or TimeoutException)
            {
                if (IsDisposed || Disposing || token.IsCancellationRequested) return;
                foreach (var pin in loggerPins.Markers) pin.Dispose();
                loggerPins.Markers.Clear();
                ShowMaintenance(Array.Empty<PressureLogger>(), new List<WasteWaterMaintenance.Alert>(), new List<WasteWaterReading>());
                loggerLoadMessage = "Loggers unavailable. Run Update-Database, then Refresh.";
                System.Diagnostics.Trace.TraceWarning("Wastewater loggers: "+ex.Message);
            }
            finally
            {
                if (!IsDisposed && !Disposing && !token.IsCancellationRequested)
                {
                    refreshLoggersButton.Enabled = true;
                    ShowStatus();
                    map.Invalidate();
                }
            }
        }

        private void resetButton_Click(object? sender,EventArgs e) => ResetView();
        private void zoomInButton_Click(object? sender,EventArgs e) => map.Zoom=Math.Min(map.MaxZoom,map.Zoom+1);
        private void zoomOutButton_Click(object? sender,EventArgs e) => map.Zoom=Math.Max(map.MinZoom,map.Zoom-1);
        private void numbersCheckBox_CheckedChanged(object? sender,EventArgs e) => UpdateBuildings();
        private void attributionLabel_LinkClicked(object? sender,LinkLabelLinkClickedEventArgs e) =>
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://www.openstreetmap.org/copyright") { UseShellExecute=true });
        private void LayoutMap()
        {
            if (page == null || contentPanel == null || layingOut || IsDesigner()) return;
            layingOut = true;
            try
            {
                float scale = DeviceDpi / 96f;
                int gap = (int)(24 * scale), space = (int)(16 * scale);
                bool details = maintenancePanel.Visible;
                bool stacked = details && page.Width < 1100 * scale;
                int mapHeight = stacked
                    ? (int)(420 * scale)
                    : Math.Min((int)(760 * scale), Math.Max((int)(400 * scale), page.Height - gap * 2));
                int detailsHeight = (int)(370 * scale);
                int height = stacked ? mapHeight + space + detailsHeight : Math.Max(mapHeight, details ? detailsHeight : 0);
                int viewportWidth = page.Width - (height + gap * 2 > page.Height ? SystemInformation.VerticalScrollBarWidth : 0);
                int width = Math.Min((int)(1480 * scale), Math.Max(1, viewportWidth - gap * 2));
                int detailsWidth = (int)(360 * scale);
                page.AutoScrollMinSize = new Size(0, height + gap * 2);
                contentPanel.Bounds = new Rectangle((viewportWidth - width) / 2 + page.AutoScrollPosition.X,
                    gap + page.AutoScrollPosition.Y, width, height);
                mapPanel.Bounds = new Rectangle(0, 0, details && !stacked ? width - detailsWidth - space : width, mapHeight);
                maintenancePanel.Bounds = stacked
                    ? new Rectangle(0, mapHeight + space, width, detailsHeight)
                    : new Rectangle(width - detailsWidth, 0, detailsWidth, detailsHeight);
                toolbar.WrapContents = mapPanel.Width < 650 * scale;
                toolbar.Height = (int)((toolbar.WrapContents ? 76 : 42) * scale);
            }
            finally { layingOut = false; }
        }
    }
}
