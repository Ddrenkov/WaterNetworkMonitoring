using GMap.NET;
using GMap.NET.WindowsForms;
using System.ComponentModel;
using WaterNetworkMonitoring.Services;

namespace WaterNetworkMonitoring
{
    public partial class DashboardControl : UserControl
    {
        private static readonly Color Blue = Color.FromArgb(37, 99, 180);
        private static readonly Color Orange = Color.FromArgb(195, 100, 35);
        private readonly DashboardData? previewData;
        private DashboardData? data;
        private CancellationTokenSource? loadCancellation;
        private GMapOverlay? buildingPins;
        private GMapOverlay? pipes;
        private BuildingPipeGraph? pipeGraph;
        private int? flaggedBuilding;
        private List<BuildingMapData.Building> buildings = new();
        private bool initialized;
        private bool layingOut;

        public DashboardControl()
        {
            Program.ConfigureTraceOutput();
            InitializeComponent();
            Load += DashboardControl_Load;
            Resize += (_, _) => LayoutDashboard();
            map.Load += (_, _) =>
            {
                if (IsDesigner()) return;
                map.BeginInvoke(new Action(() => { if (!IsDisposed) ResetMap(); }));
            };
            LayoutDashboard();
        }

        // Used by the offline visual checks; the running application always loads SQL Server data.
        internal DashboardControl(DashboardData preview) : this() => previewData = preview;

        private bool IsDesigner() => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        private void periodSelector_SelectedIndexChanged(object? sender, EventArgs e) => ShowSelectedPeriod();
        private async void refreshButton_Click(object? sender, EventArgs e) => await LoadReadingsAsync();
        private void resetMapButton_Click(object? sender, EventArgs e) => ResetMap();
        private void numbersCheckBox_CheckedChanged(object? sender, EventArgs e) => UpdatePins(null);
        private void pipesCheckBox_CheckedChanged(object? sender, EventArgs e) => UpdatePipes(null);
        private void attributionLabel_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) =>
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://www.openstreetmap.org/copyright") { UseShellExecute = true });

        private async void DashboardControl_Load(object? sender, EventArgs e)
        {
            if (IsDesigner() || initialized) return;
            initialized = true;
            InitializeMap();
            if (previewData != null) ShowData(previewData);
            else await LoadReadingsAsync();
        }

        private void InitializeMap()
        {
            var zones = BlagoevgradNeighbourhoods.LoadZones();
            buildings = BuildingMapData.Load(zones);
            GMaps.Instance.Mode = previewData == null ? AccessMode.ServerAndCache : AccessMode.CacheOnly;
            map.MapProvider = OSMap.Instance;
            var regions = BlagoevgradNeighbourhoods.CreateOverlay();
            // The map header already names Zapad; omit the large label over its buildings.
            regions.Markers.Clear();
            map.Overlays.Add(regions);
            try
            {
                pipeGraph = BuildingPipeGraph.Create(buildings, zones.Single(z => z.Name == "Zapad"));
                pipes = new GMapOverlay("dashboard-pipes");
                foreach (var connection in pipeGraph.Connections)
                    pipes.Routes.Add(new GMapRoute(new List<PointLatLng> { connection.From.Position, connection.To.Position }, connection.Id)
                        { Stroke = new Pen(Blue, 2f), IsHitTestVisible = true });
                map.Overlays.Add(pipes);
            }
            catch (InvalidOperationException ex)
            {
                pipesCheckBox.Enabled = false;
                System.Diagnostics.Trace.TraceWarning(ex.Message);
            }
            buildingPins = new GMapOverlay("dashboard-buildings");
            map.Overlays.Add(buildingPins);
            UpdatePins(null);
            map.OnMarkerClick += (marker, args) =>
            {
                if (args.Button != MouseButtons.Left || marker.Tag is not BuildingMapData.Building building) return;
                mapHint.Text = $"Building {building.Number}";
                UpdatePins(building.Number);
            };
            map.OnMarkerEnter += _ => map.Cursor = Cursors.Hand;
            map.OnMarkerLeave += _ => map.Cursor = Cursors.Default;
            ResetMap();
        }

        private void UpdatePins(int? selected)
        {
            if (buildingPins == null) return;
            foreach (var marker in buildingPins.Markers) marker.Dispose();
            buildingPins.Markers.Clear();
            foreach (var building in buildings.OrderBy(b => b.Number == selected))
                buildingPins.Markers.Add(new NumberedBuildingPin(building, numbersCheckBox.Checked, building.Number == selected, compact: true));
            UpdatePipes(selected);
            map.Invalidate();
        }

        private void UpdatePipes(int? selected)
        {
            if (pipes == null || pipeGraph == null) return;
            foreach (var route in pipes.Routes)
            {
                var connection = pipeGraph.Connections.First(p => p.Id == route.Name);
                bool flagged = connection.Touches(flaggedBuilding);
                route.Stroke.Dispose();
                route.Stroke = new Pen(flagged ? Color.Firebrick : Blue, flagged || connection.Touches(selected) ? 3f : 1.5f);
                route.IsVisible = pipesCheckBox.Checked;
            }
            map.Invalidate();
        }

        private void ResetMap()
        {
            if (!initialized || map.MapProvider == null) return;
            var zone = BlagoevgradNeighbourhoods.LoadZones().Single(z => z.Name == "Zapad");
            map.SetZoomToFitRect(RectLatLng.FromLTRB(zone.Points.Min(p => p[1]) - .0003,
                zone.Points.Max(p => p[0]) + .0003, zone.Points.Max(p => p[1]) + .0003,
                zone.Points.Min(p => p[0]) - .0003));
            foreach (var overlay in map.Overlays)
            {
                foreach (var polygon in overlay.Polygons) map.UpdatePolygonLocalPosition(polygon);
                foreach (var marker in overlay.Markers) map.UpdateMarkerLocalPosition(marker);
            }
            mapHint.Text = $"{buildings.Count} buildings · proposed connections";
        }

        private async Task LoadReadingsAsync()
        {
            loadCancellation?.Cancel();
            loadCancellation?.Dispose();
            loadCancellation = new CancellationTokenSource();
            var token = loadCancellation.Token;
            refreshButton.Enabled = false;
            statusLabel.Text = "Loading readings…";
            try
            {
                var result = await DashboardData.LoadAsync(token);
                if (!IsDisposed && !Disposing && !token.IsCancellationRequested) ShowData(result);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) when (ex is Microsoft.Data.SqlClient.SqlException or InvalidOperationException or TimeoutException)
            {
                if (IsDisposed || Disposing || token.IsCancellationRequested) return;
                data = null;
                periodSelector.Items.Clear();
                periodSelector.Enabled = false;
                ClearReadings("Readings unavailable", "Check the connection.");
                statusLabel.Text = "Refresh to retry.";
                System.Diagnostics.Trace.TraceError("Dashboard readings: " + ex.Message);
            }
            finally
            {
                if (!IsDisposed && !Disposing) refreshButton.Enabled = true;
            }
        }

        private void ShowData(DashboardData result)
        {
            data = result;
            var months = result.Months;
            periodSelector.Items.Clear();
            foreach (var month in months) periodSelector.Items.Add(month);
            periodSelector.Enabled = months.Count > 0;
            if (months.Count == 0)
            {
                ClearReadings("No readings", "Add readings to the database.");
                statusLabel.Text = "No data.";
                return;
            }
            // Start on the month with the most observations, rather than a short boundary day.
            periodSelector.SelectedItem = months.OrderByDescending(m => m.Count).First();
        }

        private void ShowSelectedPeriod()
        {
            if (data == null || periodSelector.SelectedItem is not DashboardData.Month month) return;
            var report = data.Report(month);
            var dayGap = TimeSpan.FromHours(26);
            flowChart.SetSeries(new ChartSeries("Inlet", Blue, report.InletDays, dayGap));
            pressureChart.SetSeries(new ChartSeries("Inlet", Blue, report.InletPressure, dayGap),
                new ChartSeries("Branch", Orange, report.BranchPressure, dayGap));
            lossChart.SetSeries(new ChartSeries("Difference", Color.Firebrick,
                report.Balances.Select(b => new ChartPoint(b.To, (double)b.ExcessM3PerHour * 1000)).ToList(), TimeSpan.FromHours(14)));
            statusLabel.Text = report.CompleteMonth ? "" : "Partial period";
            leakLoggers.Text = report.LoggerSummary;
            flaggedBuilding = report.LatestLeak == null ? null : report.BuildingNumber;
            if (report.LatestLeak is { } leak)
            {
                leakPanel.BackColor = SystemColors.Control;
                leakStatus.Text = "Possible leak · Skaptopara";
                leakStatus.ForeColor = Color.FromArgb(149, 72, 24);
                leakDetails.Text = $"Building {report.BuildingNumber} · meter {report.MeterId} · flagged {report.FirstLeak!.To:dd MMM HH:mm}";
                leakRate.Text = $"{Math.Max(0, leak.ExcessM3PerHour) * 1000:N0} L/h";
                leakRateCaption.Text = "Estimated loss";
                UpdatePins(report.BuildingNumber);
            }
            else
            {
                leakPanel.BackColor = SystemColors.Control;
                leakStatus.Text = report.AnalysisNote == null ? "No leak detected" : "Not enough data";
                leakStatus.ForeColor = ForeColor;
                leakDetails.Text = report.AnalysisNote == null ? "Readings are balanced." : "Matching meter readings are needed.";
                leakRate.Text = "—";
                leakRateCaption.Text = "Estimated loss";
                UpdatePins(null);
            }
        }

        private void ClearReadings(string title, string detail)
        {
            leakPanel.BackColor = SystemColors.Control;
            leakStatus.Text = title;
            leakStatus.ForeColor = ForeColor;
            leakDetails.Text = detail;
            leakLoggers.Text = "";
            leakRate.Text = "—";
            leakRateCaption.Text = "Estimated loss";
            flowChart.SetSeries(); pressureChart.SetSeries(); lossChart.SetSeries();
            flaggedBuilding = null;
            UpdatePins(null);
        }

        private void LayoutDashboard()
        {
            if (page == null || layingOut) return;
            layingOut = true;
            try
            {
                float s = DeviceDpi / 96f;
                int gap = (int)(16 * s);
                // Leave room for the scrollbar before the taller layout makes it appear.
                int available = ClientSize.Width - gap * 2 - SystemInformation.VerticalScrollBarWidth;
                int width = Math.Min((int)(1120 * s), Math.Max((int)(280 * s), available));
                bool narrow = width < 850 * s;
                page.SuspendLayout();
                graphGrid.SuspendLayout();
                graphGrid.ColumnCount = 3;
                graphGrid.RowCount = 3;
                graphGrid.SetCellPosition(pressureChart, narrow ? new TableLayoutPanelCellPosition(0, 1) : new TableLayoutPanelCellPosition(1, 0));
                graphGrid.SetCellPosition(lossChart, narrow ? new TableLayoutPanelCellPosition(0, 2) : new TableLayoutPanelCellPosition(2, 0));
                graphGrid.ColumnCount = narrow ? 1 : 3;
                graphGrid.RowCount = narrow ? 3 : 1;
                graphGrid.ColumnStyles.Clear();
                for (int i = 0; i < graphGrid.ColumnCount; i++)
                    graphGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / graphGrid.ColumnCount));
                graphGrid.RowStyles.Clear();
                for (int i = 0; i < graphGrid.RowCount; i++)
                    graphGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / graphGrid.RowCount));
                foreach (Control chart in graphGrid.Controls) chart.Margin = narrow ? new Padding(0, 0, 0, 10) : new Padding(4, 0, 4, 0);
                int mapHeight = (int)Math.Clamp(width * .32, 280 * s, 360 * s);
                int graphHeight = (int)((narrow ? 630 : 210) * s);
                page.RowStyles[0].Height = 40 * s;
                page.RowStyles[1].Height = 22 * s;
                page.RowStyles[2].Height = mapHeight;
                page.RowStyles[3].Height = (narrow ? 122 : 100) * s;
                page.RowStyles[4].Height = graphHeight;
                int height = page.RowStyles.Cast<RowStyle>().Sum(r => (int)r.Height);
                page.Size = new Size(width, height);
                // AutoScroll uses logical coordinates; retain the current vertical scroll position.
                page.Location = new Point(gap + Math.Max(0, (available - width) / 2), gap + AutoScrollPosition.Y);
                AutoScrollMinSize = new Size(0, height + gap * 2);
                leakContent.ColumnStyles[0].Width = narrow ? 62 : 72;
                leakContent.ColumnStyles[1].Width = narrow ? 38 : 28;
                graphGrid.ResumeLayout();
                page.ResumeLayout();
                PerformLayout();
            }
            finally { layingOut = false; }
        }
    }
}
