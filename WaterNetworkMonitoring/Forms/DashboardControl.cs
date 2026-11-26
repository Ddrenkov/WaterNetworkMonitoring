using GMap.NET;
using GMap.NET.WindowsForms;
using System.ComponentModel;
using WaterNetworkMonitoring.Services;

namespace WaterNetworkMonitoring
{
    public partial class DashboardControl : UserControl
    {
        private readonly DashboardData? previewData;
        private DashboardData? data;
        private CancellationTokenSource? loadCancellation;
        private GMapOverlay? buildingPins;
        private GMapOverlay? loggerPins;
        private GMapOverlay? pipes;
        private BuildingPipeGraph? pipeGraph;
        private int? flaggedBuilding;
        private List<BuildingMapData.Building> buildings = new();
        private bool initialized;
        private bool layingOut;
        private bool updatingComparison;

        public DashboardControl()
        {
            Program.ConfigureTraceOutput();
            InitializeComponent();
            Load += DashboardControl_Load;
            Resize += (_, _) => LayoutDashboard();
            map.SizeChanged += (_, _) =>
            {
                if (initialized && IsHandleCreated)
                    BeginInvoke(new Action(() => { if (!IsDisposed) ResetMap(); }));
            };
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
        private void comparisonSelector_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!updatingComparison) UpdateComparison();
        }
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
                pipes = pipeGraph.CreateOverlay("dashboard-pipes");
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
            loggerPins = LoggerMapPin.CreateOverlay();
            map.Overlays.Add(loggerPins);
            map.OnMarkerClick += (marker, args) =>
            {
                if (args.Button == MouseButtons.Left && marker is LoggerMapPin logger)
                {
                    LoggerMapPin.Select(loggerPins, logger);
                    map.Invalidate();
                    return;
                }
                if (args.Button != MouseButtons.Left || marker.Tag is not BuildingMapData.Building building) return;
                LoggerMapPin.Select(loggerPins, null);
                mapHint.Text = $"Building {building.Number}";
                if (data?.Meters.FirstOrDefault(m => m.MapNumber == building.Number) is { } meter)
                {
                    if (meter.LoggerId.HasValue)
                        comparisonSelector.SelectedItem = comparisonSelector.Items.Cast<DashboardData.MeterScope>()
                            .FirstOrDefault(s => s.LoggerId == meter.LoggerId);
                    if (periodSelector.SelectedItem is DashboardData.Month month)
                    {
                        var reading = data.MeterReadings.Where(r => r.WaterMeterId == meter.Id &&
                            r.TakenAt.Year == month.Year && r.TakenAt.Month == month.Number).OrderByDescending(r => r.TakenAt).FirstOrDefault();
                        mapHint.Text = reading == null ? $"Building {building.Number} · meter {meter.Id} · no reading this month"
                            : $"Building {building.Number} · meter {meter.Id} · {reading.Reading} L · {reading.TakenAt:dd MMM HH:mm}";
                    }
                }
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
            pipeGraph.StyleOverlay(pipes,pipesCheckBox.Checked,flaggedBuilding,selected);
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
            LoggerMapPin.Select(loggerPins, null);
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
            var repair = data.LatestCleanWaterRepair(month);
            repairLabel.Text = repair == null ? "Last clean-water repair: none recorded"
                : $"Last clean-water repair: {repair.Date:dd MMM yyyy HH:mm}\n{repair.Place}";
            mapHint.Text = $"{buildings.Count} buildings · click a building for its meter reading";
            statusLabel.Text = report.CompleteMonth ? "" : "Partial period";
            var selectedId = (comparisonSelector.SelectedItem as DashboardData.MeterScope)?.LoggerId ?? report.FlaggedLoggerId;
            updatingComparison = true;
            comparisonSelector.Items.Clear();
            foreach (var scope in data.MeterScopes) comparisonSelector.Items.Add(scope);
            comparisonSelector.Enabled = comparisonSelector.Items.Count > 0;
            comparisonSelector.SelectedItem = comparisonSelector.Items.Cast<DashboardData.MeterScope>()
                .FirstOrDefault(s => s.LoggerId == selectedId) ?? comparisonSelector.Items.Cast<DashboardData.MeterScope>().FirstOrDefault();
            updatingComparison = false;
            UpdateComparison();
        }

        private void UpdateComparison()
        {
            flowChart.SetSeries(); lossChart.SetSeries();
            mapHint.Text = $"{buildings.Count} buildings · click a building for its meter reading";
            flaggedBuilding = null;
            if (data == null || periodSelector.SelectedItem is not DashboardData.Month month ||
                comparisonSelector.SelectedItem is not DashboardData.MeterScope scope) return;
            var comparison = data.CompareMeters(scope.LoggerId, month);
            var balances = comparison.Balances;
            var last = balances.LastOrDefault();
            var monthEnd = data.LoggerReadings.Where(r => r.TakenAt.Year == month.Year && r.TakenAt.Month == month.Number)
                .Select(r => (DateTimeOffset?)r.TakenAt).Max();
            LoggerMapPin.UpdateReadings(loggerPins, data, last?.To ?? monthEnd,
                last?.LeakSuspected == true ? scope.LoggerId : null, pipeGraph);
            LoggerMapPin.Select(loggerPins, null);
            leakRateCaption.Text = "Difference";
            leakStatus.ForeColor = ForeColor;
            if (last == null)
            {
                leakStatus.Text = "No matching readings";
                leakDetails.Text = "Matching logger and meter readings are needed.";
                leakLocation.Text = scope.Name;
                leakRate.Text = "—";
                UpdatePins(null);
                return;
            }
            // Match supply and meter use over the same recorded interval.
            var gap = TimeSpan.FromTicks(balances.Max(b => (b.To - b.From).Ticks) * 3 / 2);
            flowChart.SetSeries(
                new ChartSeries("Supply", Color.FromArgb(30,115,74), balances.Select(b => new ChartPoint(b.To,(double)b.SuppliedM3)).ToList(), gap),
                new ChartSeries("Meters", Color.FromArgb(37,99,180), balances.Select(b => new ChartPoint(b.To,(double)b.ConsumedM3)).ToList(), gap));
            lossChart.SetSeries(new ChartSeries("Difference", Color.Firebrick,
                balances.Select(b => new ChartPoint(b.To,(double)b.ExcessM3PerHour*1000)).ToList(), gap));
            leakStatus.Text = last.DetectionThreshold == 0 ? "Collecting history"
                : last.LeakSuspected ? "Possible leak" : "No unusual difference";
            if (last.LeakSuspected) leakStatus.ForeColor = Color.FromArgb(149,72,24);
            leakDetails.Text = $"{last.SuppliedM3:0.##} m³ supplied · {last.ConsumedM3:0.##} m³ used · {(last.To-last.From).TotalHours:0.#} h";
            leakLocation.Text = $"{scope.Name} · {last.From:dd MMM HH:mm}–{last.To:dd MMM HH:mm}" +
                (comparison.Estimated ? " · estimated meter use" : "");
            leakRate.Text = $"{last.ExcessM3PerHour*1000:N0} L/h";
            if (last.LeakSuspected && scope.MeterCount == 1)
                flaggedBuilding = data.Meters.FirstOrDefault(m => m.LoggerId == scope.LoggerId)?.MapNumber;
            UpdatePins(null);
        }

        private void ClearReadings(string title, string detail)
        {
            leakPanel.BackColor = Color.White;
            leakStatus.Text = title;
            leakStatus.ForeColor = ForeColor;
            leakDetails.Text = detail;
            leakLocation.Text = "";
            repairLabel.Text = "";
            LoggerMapPin.UpdateReadings(loggerPins, null);
            LoggerMapPin.Select(loggerPins, null);
            leakRate.Text = "—";
            leakRateCaption.Text = "Difference";
            flowChart.SetSeries(); lossChart.SetSeries();
            comparisonSelector.Items.Clear();
            comparisonSelector.Enabled = false;
            mapHint.Text = $"{buildings.Count} buildings · proposed connections";
            flaggedBuilding = null;
            UpdatePins(null);
        }

        private void LayoutDashboard()
        {
            if (page == null || layingOut || IsDesigner()) return;
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
                graphGrid.ColumnCount = 2;
                graphGrid.RowCount = 2;
                graphGrid.SetCellPosition(lossChart, narrow ? new TableLayoutPanelCellPosition(0, 1) : new TableLayoutPanelCellPosition(1, 0));
                graphGrid.ColumnCount = narrow ? 1 : 2;
                graphGrid.RowCount = narrow ? 2 : 1;
                graphGrid.ColumnStyles.Clear();
                for (int i = 0; i < graphGrid.ColumnCount; i++)
                    graphGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / graphGrid.ColumnCount));
                graphGrid.RowStyles.Clear();
                for (int i = 0; i < graphGrid.RowCount; i++)
                    graphGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / graphGrid.RowCount));
                foreach (Control chart in graphGrid.Controls) chart.Margin = narrow ? new Padding(0, 0, 0, 10) : new Padding(4, 0, 4, 0);
                int mapHeight = (int)Math.Clamp(width * .32, 280 * s, 340 * s);
                int graphHeight = (int)((narrow ? 460 : 230) * s);
                page.RowStyles[0].Height = 40 * s;
                page.RowStyles[1].Height = 22 * s;
                page.RowStyles[2].Height = mapHeight;
                page.RowStyles[3].Height = (narrow ? 120 : 90) * s;
                page.RowStyles[4].Height = 44 * s;
                page.RowStyles[5].Height = 32 * s;
                page.RowStyles[6].Height = graphHeight;
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
