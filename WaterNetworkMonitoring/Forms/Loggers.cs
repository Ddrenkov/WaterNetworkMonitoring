using GMap.NET;
using GMap.NET.WindowsForms;
using System.ComponentModel;
using WaterNetworkMonitoring.Services;

namespace WaterNetworkMonitoring
{
    public partial class Loggers : Form
    {
        private GMapOverlay? neighbourhoods;
        private readonly DashboardData? previewData;
        private GMapOverlay? loggerPins;
        private BuildingPipeGraph? pipeGraph;
        private GMapOverlay? pipes;
        private DashboardData? data;
        private bool chartsShown;
        private CancellationTokenSource? loadCancellation;
        private bool updatingSelection;
        private string? selectedZoneName;

        public Loggers()
        {
            Program.ConfigureTraceOutput();
            InitializeComponent();
            if (!DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
                chartsPanel.Visible = false;
            map.Load += map_Load;
            Resize += (_, _) =>
            {
                LayoutMap();
                if (selectedZoneName != null && neighbourhoods != null)
                    FitZone((BlagoevgradNeighbourhoods.Zone)neighbourhoods.Polygons.Single(p => p.Name == selectedZoneName).Tag);
            };
            LayoutMap();
        }

        internal Loggers(DashboardData preview) : this() => previewData = preview;

        private void Loggers_Load(object? sender, EventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime || neighbourhoods != null) return;
            GMaps.Instance.Mode = previewData == null ? AccessMode.ServerAndCache : AccessMode.CacheOnly;
            map.MapProvider = OSMap.Instance;
            CenterOnBlagoevgrad();
            neighbourhoods = BlagoevgradNeighbourhoods.CreateOverlay(BlagoevgradNeighbourhoods.LoadAllZones());
            map.Overlays.Add(neighbourhoods);
            try
            {
                var zones = BlagoevgradNeighbourhoods.LoadZones();
                pipeGraph = BuildingPipeGraph.Create(BuildingMapData.Load(zones),zones.Single(z => z.Name == "Zapad"));
                pipes = pipeGraph.CreateOverlay("logger-pipes");
                map.Overlays.Add(pipes);
                map.RoutesEnabled = true;
            }
            catch (InvalidOperationException ex) { System.Diagnostics.Trace.TraceWarning(ex.Message); }
            loggerPins = LoggerMapPin.CreateOverlay();
            map.Overlays.Add(loggerPins);
            foreach (var polygon in neighbourhoods.Polygons) map.UpdatePolygonLocalPosition(polygon);
            foreach (var marker in neighbourhoods.Markers) map.UpdateMarkerLocalPosition(marker);
            zoneSelector.Items.Add("All regions");
            foreach (var polygon in neighbourhoods.Polygons) zoneSelector.Items.Add(polygon.Name);
            zoneSelector.SelectedIndex = 0;
            zoneSelector.Enabled = true;
            resetButton_Click(this, EventArgs.Empty);
            map.OnPolygonClick += (polygon, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    var point = map.FromLocalToLatLng(args.X, args.Y);
                    SelectZoneAt(point);
                }
            };
            map.OnMarkerClick += (marker, args) =>
            {
                if (args.Button != MouseButtons.Left) return;
                if (marker is LoggerMapPin logger)
                {
                    if (selectedZoneName != "Zapad")
                        SelectZone((BlagoevgradNeighbourhoods.Zone)neighbourhoods.Polygons.Single(p => p.Name == "Zapad").Tag);
                    LoggerMapPin.Select(loggerPins, logger);
                    map.Invalidate();
                }
                else if (marker.Tag is BlagoevgradNeighbourhoods.Zone zone) SelectZone(zone);
            };
            map.OnPolygonEnter += _ => map.Cursor = Cursors.Hand;
            map.OnPolygonLeave += _ => map.Cursor = Cursors.Default;
            map.OnMarkerEnter += _ => map.Cursor = Cursors.Hand;
            map.OnMarkerLeave += _ => map.Cursor = Cursors.Default;
            map.OnMapZoomChanged += UpdateLabelVisibility;
            map.OnPositionChanged += _ => UpdateLabelVisibility();
            UpdateLabelVisibility();
            map.Refresh();
            _ = LoadLoggerReadingsAsync();
        }

        private async void refreshReadingsButton_Click(object? sender, EventArgs e) => await LoadLoggerReadingsAsync();
        private void periodSelector_SelectedIndexChanged(object? sender, EventArgs e) => UpdateCharts();

        private async Task LoadLoggerReadingsAsync()
        {
            loadCancellation?.Cancel(); loadCancellation?.Dispose();
            loadCancellation=new CancellationTokenSource();
            var token=loadCancellation.Token;
            refreshReadingsButton.Enabled=false;
            try
            {
                var result=previewData ?? await DashboardData.LoadAsync(token);
                if (IsDisposed || Disposing || token.IsCancellationRequested) return;
                data=result;
                periodSelector.Items.Clear();
                foreach(var month in result.Months) periodSelector.Items.Add(month);
                if(result.Months.Count>0) periodSelector.SelectedItem=result.Months.OrderByDescending(m=>m.Count).First();
                else UpdateCharts();
            }
            catch(OperationCanceledException) { }
            catch(Exception ex) when(ex is Microsoft.Data.SqlClient.SqlException or InvalidOperationException or TimeoutException)
            {
                if(IsDisposed || Disposing || token.IsCancellationRequested) return;
                data=null; periodSelector.Items.Clear(); UpdateCharts();
                System.Diagnostics.Trace.TraceWarning("Logger map readings: "+ex.Message);
            }
            finally { if(!IsDisposed && !Disposing && !token.IsCancellationRequested) refreshReadingsButton.Enabled=true; }
        }

        private void UpdateCharts()
        {
            bool wasVisible=chartsShown;
            DashboardReport? report=null;
            if(selectedZoneName=="Zapad" && data!=null && periodSelector.SelectedItem is DashboardData.Month month)
                report=data.Report(month);
            flowChart.SetSeries(report?.FlowSeries ?? Array.Empty<ChartSeries>());
            pressureChart.SetSeries(report?.PressureSeries ?? Array.Empty<ChartSeries>());
            chartsShown=report?.FlowSeries.Length>0;
            chartsPanel.Visible=chartsShown;
            LoggerMapPin.UpdateReadings(loggerPins,data,report?.LastReading,report?.FlaggedLoggerId,pipeGraph);
            if (pipes != null && pipeGraph != null)
                pipeGraph.StyleOverlay(pipes,selectedZoneName is null or "Zapad",
                    report?.LatestLeak == null ? null : report.BuildingNumber);
            map.Invalidate(); LayoutMap();
            if(wasVisible!=chartsShown && selectedZoneName!=null && neighbourhoods!=null)
                FitZone((BlagoevgradNeighbourhoods.Zone)neighbourhoods.Polygons.Single(p=>p.Name==selectedZoneName).Tag);
        }

        private void resetButton_Click(object? sender, EventArgs e)
        {
            if(neighbourhoods==null) return;
            var zone=(BlagoevgradNeighbourhoods.Zone)neighbourhoods.Polygons.Single(p=>p.Name=="Zapad").Tag;
            SelectZone(zone); FitZone(zone);
        }

        private void ShowAllRegions()
        {
            if(neighbourhoods==null) return;
            selectedZoneName=null; LoggerMapPin.Select(loggerPins,null);
            foreach(var polygon in neighbourhoods.Polygons) BlagoevgradNeighbourhoods.StylePolygon(polygon,false);
            UpdateCharts(); CenterOnBlagoevgrad(); UpdateLabelVisibility(); map.Refresh();
        }

        private void FitZone(BlagoevgradNeighbourhoods.Zone zone)
        {
            map.SetZoomToFitRect(RectLatLng.FromLTRB(zone.Points.Min(p=>p[1])-.0003,
                zone.Points.Max(p=>p[0])+.0003,zone.Points.Max(p=>p[1])+.0003,zone.Points.Min(p=>p[0])-.0003));
        }

        private void CenterOnBlagoevgrad()
        {
            var points = BlagoevgradNeighbourhoods.LoadAllZones().SelectMany(z => z.Points).ToList();
            map.SetZoomToFitRect(RectLatLng.FromLTRB(points.Min(p => p[1]) - .001,
                points.Max(p => p[0]) + .001, points.Max(p => p[1]) + .001,
                points.Min(p => p[0]) - .001));
        }

        private void map_Load(object? sender, EventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            // Apply the city view after GMap completes its own startup and layout.
            map.BeginInvoke(new Action(() =>
            {
                if (IsDisposed || map.IsDisposed) return;
                if (selectedZoneName != null && neighbourhoods != null)
                    FitZone((BlagoevgradNeighbourhoods.Zone)neighbourhoods.Polygons.Single(p => p.Name == selectedZoneName).Tag);
                else CenterOnBlagoevgrad();
                UpdateLabelVisibility();
            }));
        }

        private void SelectZone(BlagoevgradNeighbourhoods.Zone zone)
        {
            if (neighbourhoods == null) return;
            selectedZoneName = zone.Name;
            LoggerMapPin.Select(loggerPins, null);
            foreach (var polygon in neighbourhoods.Polygons)
                BlagoevgradNeighbourhoods.StylePolygon(polygon, polygon.Name == zone.Name);
            updatingSelection = true;
            zoneSelector.SelectedItem = zone.Name;
            updatingSelection = false;
            selectionLabel.Text = zone.Name + " — " + zone.BoundaryNote;
            UpdateCharts();
            UpdateLabelVisibility();
            map.Refresh();
        }

        private void zoneSelector_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (updatingSelection || neighbourhoods == null) return;
            if (zoneSelector.SelectedIndex == 0)
            {
                ShowAllRegions();
                return;
            }
            var polygon = neighbourhoods.Polygons.FirstOrDefault(p => p.Name == zoneSelector.SelectedItem as string);
            if (polygon?.Tag is not BlagoevgradNeighbourhoods.Zone zone) return;
            SelectZone(zone);
            FitZone(zone);
        }

        private void SelectZoneAt(PointLatLng point)
        {
            if (neighbourhoods == null) return;
            // At a shared edge, select the smallest containing neighbourhood consistently.
            var polygon = neighbourhoods.Polygons
                .Where(p => BlagoevgradNeighbourhoods.Contains(p, point))
                .OrderBy(p => PolygonArea(p.Points))
                .FirstOrDefault();
            if (polygon?.Tag is BlagoevgradNeighbourhoods.Zone zone) SelectZone(zone);
        }

        private static double PolygonArea(IList<PointLatLng> points)
        {
            double area = 0;
            for (int i = 0, j = points.Count - 1; i < points.Count; j = i++)
                area += points[j].Lng * points[i].Lat - points[i].Lng * points[j].Lat;
            return Math.Abs(area);
        }

        private void zoomInButton_Click(object? sender, EventArgs e)
        {
            map.Zoom = Math.Min(map.MaxZoom, map.Zoom + 1);
        }

        private void zoomOutButton_Click(object? sender, EventArgs e)
        {
            map.Zoom = Math.Max(map.MinZoom, map.Zoom - 1);
        }

        private void labelsCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateLabelVisibility();
            map.Refresh();
        }

        private void UpdateLabelVisibility()
        {
            if (neighbourhoods == null) return;
            var occupied = new List<Rectangle>();
            foreach (var marker in neighbourhoods.Markers.OrderByDescending(m =>
                ((BlagoevgradNeighbourhoods.Zone)m.Tag).Name == selectedZoneName))
            {
                map.UpdateMarkerLocalPosition(marker);
                var bounds = new Rectangle(marker.LocalPosition, marker.Size);
                bounds.Inflate(4, 3);
                bool visible = labelsCheckBox.Checked && !occupied.Any(r => r.IntersectsWith(bounds));
                marker.IsVisible = visible;
                if (visible) occupied.Add(bounds);
            }
        }
        private void LayoutMap()
        {
            if(mapPanel==null || chartsPanel==null || DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            float s=DeviceDpi/96f;
            int gap=(int)(16*s), width=Math.Min((int)(1500*s),Math.Max(1,ClientSize.Width-(int)(48*s)));
            int height=Math.Min((int)(760*s),Math.Max(1,ClientSize.Height-(int)(48*s)));
            int x=(ClientSize.Width-width)/2, y=(ClientSize.Height-height)/2;
            if(chartsShown && width>=1000*s)
            {
                int chartsWidth=(int)(350*s);
                mapPanel.Bounds=new Rectangle(x,y,width-chartsWidth-gap,height);
                chartsPanel.Bounds=new Rectangle(mapPanel.Right+gap,y,chartsWidth,height);
            }
            else if(chartsShown)
            {
                int chartsHeight=(int)(height*.48);
                mapPanel.Bounds=new Rectangle(x,y,width,height-chartsHeight-gap);
                chartsPanel.Bounds=new Rectangle(x,mapPanel.Bottom+gap,width,chartsHeight);
            }
            else
            {
                int mapWidth=Math.Min((int)(1100*s),width);
                mapPanel.Bounds=new Rectangle((ClientSize.Width-mapWidth)/2,y,mapWidth,height);
            }
            toolbar.WrapContents=true;
            toolbar.Height=(int)((mapPanel.Width<600*s ? 80 : 48)*s);
        }
    }
}
