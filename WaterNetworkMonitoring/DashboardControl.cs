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
            mapHint.Text = $"{buildings.Count} buildings · drag to move, scroll to zoom";
        }

        private async Task LoadReadingsAsync()
        {
            loadCancellation?.Cancel();
            loadCancellation?.Dispose();
            loadCancellation = new CancellationTokenSource();
            var token = loadCancellation.Token;
            refreshButton.Enabled = false;
            statusLabel.Text = "Loading database readings…";
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
                ClearReadings("Readings unavailable", "Check the database connection and click Refresh.");
                statusLabel.Text = "Could not load readings from the database. The map is still available.";
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
                ClearReadings("No readings yet", "Readings will appear here when they are available in the database.");
                statusLabel.Text = "No Zapad logger readings found.";
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
            flowChart.SetSeries(new ChartSeries("Zapad inlet", Blue, report.InletDays, dayGap));
            pressureChart.SetSeries(new ChartSeries("Zapad inlet", Blue, report.InletPressure, dayGap),
                new ChartSeries("Skaptopara branch", Orange, report.BranchPressure, dayGap));
            branchChart.SetSeries(
                new ChartSeries("Supplied", Blue, report.Balances.Select(b => new ChartPoint(b.To, (double)b.SuppliedM3)).ToList(), TimeSpan.FromHours(14)),
                new ChartSeries("Metered use", Orange, report.Balances.Select(b => new ChartPoint(b.To, (double)b.ConsumedM3)).ToList(), TimeSpan.FromHours(14)));
            flowChart.Subtitle = "Daily average from 15-minute readings";
            pressureChart.Subtitle = "Daily average from 15-minute readings";
            branchChart.Subtitle = "Matching meter windows · supplied water includes any loss";
            statusLabel.Text = $"{report.Period} · {(report.IsSimulated ? "Simulated readings" : "Database readings")} · " +
                $"{(report.CompleteMonth ? "Water supplied" : "Recorded supply (partial month)")}: {report.SuppliedM3:N1} m³";
            if (report.LatestLeak is { } leak)
            {
                leakPanel.BackColor = Color.FromArgb(255, 248, 238);
                leakStatus.Text = "Possible leak · Skaptopara";
                leakStatus.ForeColor = Color.FromArgb(149, 72, 24);
                leakDetails.Text = $"Building {report.BuildingNumber} · meter {report.MeterId}\n" +
                    $"First flagged {report.FirstLeak!.To:dd MMM, HH:mm} · latest confirmation {leak.To:dd MMM, HH:mm}";
                leakRate.Text = $"{Math.Max(0, leak.ExcessM3PerHour) * 1000:N0} L/h";
                leakRateCaption.Text = leak.PressureDrop ? "Estimated loss · pressure also lower" : "Estimated loss from the branch balance";
                UpdatePins(report.BuildingNumber);
            }
            else
            {
                leakPanel.BackColor = Color.White;
                leakStatus.Text = report.AnalysisNote == null ? "No leak detected in this period" : "Not enough data to assess leaks";
                leakStatus.ForeColor = ForeColor;
                leakDetails.Text = report.AnalysisNote ?? "Complete branch readings show no persistent water imbalance.";
                leakRate.Text = "—";
                leakRateCaption.Text = "Estimated loss";
                UpdatePins(null);
            }
        }

        private void ClearReadings(string title, string detail)
        {
            leakPanel.BackColor = Color.White;
            leakStatus.Text = title;
            leakStatus.ForeColor = ForeColor;
            leakDetails.Text = detail;
            leakRate.Text = "—";
            leakRateCaption.Text = "Estimated loss";
            flowChart.SetSeries(); pressureChart.SetSeries(); branchChart.SetSeries();
            UpdatePins(null);
        }

        private void LayoutDashboard()
        {
            if (page == null || layingOut) return;
            layingOut = true;
            try
            {
                float s = DeviceDpi / 96f;
                int gap = (int)(24 * s);
                int available = ClientSize.Width - gap * 2 - (VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0);
                int width = Math.Min((int)(1120 * s), Math.Max((int)(280 * s), available));
                bool narrow = width < 760 * s;
                page.SuspendLayout();
                graphGrid.SuspendLayout();
                graphGrid.ColumnCount = 2;
                graphGrid.RowCount = 2;
                graphGrid.SetCellPosition(pressureChart, narrow ? new TableLayoutPanelCellPosition(0, 1) : new TableLayoutPanelCellPosition(1, 0));
                graphGrid.ColumnCount = narrow ? 1 : 2;
                graphGrid.RowCount = narrow ? 2 : 1;
                graphGrid.ColumnStyles.Clear();
                graphGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, narrow ? 100 : 50));
                if (!narrow) graphGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                graphGrid.RowStyles.Clear();
                graphGrid.RowStyles.Add(new RowStyle(SizeType.Percent, narrow ? 50 : 100));
                if (narrow) graphGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
                int mapHeight = (int)Math.Clamp(width * .42, 340 * s, 470 * s);
                int graphHeight = (int)((narrow ? 500 : 250) * s);
                page.RowStyles[0].Height = 48 * s;
                page.RowStyles[1].Height = 28 * s;
                page.RowStyles[2].Height = mapHeight;
                page.RowStyles[3].Height = (narrow ? 182 : 140) * s;
                page.RowStyles[4].Height = graphHeight;
                page.RowStyles[5].Height = 250 * s;
                int height = page.RowStyles.Cast<RowStyle>().Sum(r => (int)r.Height);
                page.Size = new Size(width, height);
                // AutoScroll uses logical coordinates; retain the current vertical scroll position.
                page.Location = new Point(Math.Max(gap, (ClientSize.Width - width) / 2), gap + AutoScrollPosition.Y);
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
