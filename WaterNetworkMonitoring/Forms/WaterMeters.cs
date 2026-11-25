using GMap.NET;
using GMap.NET.WindowsForms;
using System.ComponentModel;
using WaterNetworkMonitoring.Services;

namespace WaterNetworkMonitoring
{
    public partial class WaterMeters : Form
    {
        private readonly GMapOverlay pins = new("buildings");
        private List<BuildingMapData.Building> buildings = new();
        private List<BlagoevgradNeighbourhoods.Zone> zones = new();
        private bool initialized;
        private int? selectedNumber;
        private readonly WaterMeterConsumptionData? previewData;
        private WaterMeterConsumptionData? consumption;
        private CancellationTokenSource? loadCancellation;
        private bool selectingMeter;

        public WaterMeters()
        {
            Program.ConfigureTraceOutput();
            InitializeComponent();
            refreshTimer.Tick += (_, _) => { refreshTimer.Stop(); UpdatePins(); };
            map.Load += (_, _) =>
            {
                if (!IsDesigner()) map.BeginInvoke(new Action(() =>
                {
                    if (IsDisposed) return;
                    CenterOnZapad();
                    UpdatePins();
                }));
            };
            Resize += (_, _) => { LayoutMap(); QueueRefresh(); };
            map.SizeChanged += (_, _) =>
            {
                if (initialized && IsHandleCreated)
                    BeginInvoke(new Action(() => { if (!IsDisposed) { CenterOnZapad(); UpdatePins(); } }));
            };
            LayoutMap();
        }

        internal WaterMeters(WaterMeterConsumptionData preview) : this() => previewData = preview;

        private bool IsDesigner() => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        private async void WaterMeters_Load(object? sender, EventArgs e)
        {
            if (IsDesigner() || initialized) return;
            zones = BlagoevgradNeighbourhoods.LoadZones();
            buildings = BuildingMapData.Load(zones);
            initialized = true;
            GMaps.Instance.Mode = previewData == null ? AccessMode.ServerAndCache : AccessMode.CacheOnly;
            map.MapProvider = OSMap.Instance;
            // The zones select buildings only; this map deliberately has no polygon overlay.
            map.Overlays.Add(pins);
            foreach (var zone in zones.OrderBy(z => z.Name)) zoneSelector.Items.Add(zone.Name);
            zoneSelector.SelectedIndex = 0;
            zoneSelector.Enabled = false;
            buildingNumber.Maximum = Math.Max(1, buildings.Count);
            findButton.Enabled = buildings.Count > 0;
            map.OnPositionChanged += _ => QueueRefresh();
            map.OnMapZoomChanged += QueueRefresh;
            map.OnMarkerClick += (marker, args) =>
            {
                if (args.Button == MouseButtons.Left && marker.Tag is BuildingMapData.Building building)
                    SelectBuilding(building, false);
            };
            map.OnMarkerEnter += _ => map.Cursor = Cursors.Hand;
            map.OnMarkerLeave += _ => map.Cursor = Cursors.Default;
            CenterOnZapad();
            UpdatePins();
            await LoadConsumptionAsync();
        }

        private void CenterOnZapad()
        {
            var zone = zones.FirstOrDefault(z => z.Name == "Zapad");
            if (zone == null) return;
            map.SetZoomToFitRect(RectLatLng.FromLTRB(zone.Points.Min(p => p[1]) - .0003,
                zone.Points.Max(p => p[0]) + .0003, zone.Points.Max(p => p[1]) + .0003,
                zone.Points.Min(p => p[0]) - .0003));
        }

        private IEnumerable<BuildingMapData.Building> FilteredBuildings() =>
            zoneSelector.SelectedIndex <= 0 ? buildings :
                buildings.Where(b => b.Zones.Contains((string)zoneSelector.SelectedItem!));

        private void QueueRefresh()
        {
            if (!initialized || IsDisposed || !IsHandleCreated) return;
            if (InvokeRequired) { BeginInvoke(new Action(QueueRefresh)); return; }
            refreshTimer.Stop();
            refreshTimer.Start();
        }

        private void UpdatePins()
        {
            if (!initialized || !map.IsHandleCreated || IsDisposed) return;
            var area = map.ViewArea;
            if (area.IsEmpty) return;
            var filtered = FilteredBuildings().ToList();
            // Keep all records numbered, but only create drawing objects for the current viewport.
            double latMargin = area.HeightLat * .1, lngMargin = area.WidthLng * .1;
            var visible = filtered.Where(b => b.Latitude >= area.Bottom - latMargin &&
                b.Latitude <= area.Top + latMargin && b.Longitude >= area.Left - lngMargin &&
                b.Longitude <= area.Right + lngMargin).OrderBy(b => b.Number == selectedNumber).ToList();
            map.HoldInvalidation = true;
            try
            {
                foreach (var marker in pins.Markers) marker.Dispose();
                pins.Markers.Clear();
                foreach (var building in visible)
                {
                    var marker = new NumberedBuildingPin(building, true, building.Number == selectedNumber);
                    pins.Markers.Add(marker);
                    map.UpdateMarkerLocalPosition(marker);
                }
            }
            finally { map.HoldInvalidation = false; }
            if (selectedNumber == null)
                statusLabel.Text = $"{filtered.Count:N0} mapped buildings • Pins 1–{buildings.Count:N0} • Zapad";
            map.Refresh();
        }

        private void zoneSelector_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!initialized) return;
            selectedNumber = null;
            var zone = zones.FirstOrDefault(z => z.Name == zoneSelector.SelectedItem as string);
            if (zone == null) CenterOnZapad();
            else map.SetZoomToFitRect(RectLatLng.FromLTRB(zone.Points.Min(p => p[1]) - .0005,
                zone.Points.Max(p => p[0]) + .0005, zone.Points.Max(p => p[1]) + .0005,
                zone.Points.Min(p => p[0]) - .0005));
            UpdatePins();
        }

        private void SelectBuilding(BuildingMapData.Building building, bool center)
        {
            selectedNumber = building.Number;
            buildingNumber.Value = building.Number;
            if (center) { map.Zoom = 18; map.Position = building.Position; }
            statusLabel.Text = building.Number.ToString();
            if (!selectingMeter)
            {
                var row = consumptionGrid.Rows.Cast<DataGridViewRow>()
                    .FirstOrDefault(r => (r.Tag as WaterMeterConsumptionData.MeterUsage)?.Meter.MapNumber == building.Number);
                if (row != null)
                {
                    consumptionGrid.CurrentCell = row.Cells[0];
                    consumptionGrid.FirstDisplayedScrollingRowIndex = row.Index;
                }
            }
            UpdatePins();
        }

        private void findButton_Click(object? sender, EventArgs e)
        {
            var building = buildings.FirstOrDefault(b => b.Number == (int)buildingNumber.Value);
            if (building == null) return;
            if (!FilteredBuildings().Contains(building)) zoneSelector.SelectedIndex = 0;
            SelectBuilding(building, true);
        }

        private void resetButton_Click(object? sender, EventArgs e)
        {
            selectedNumber = null;
            zoneSelector.SelectedIndex = 0;
            CenterOnZapad();
            UpdatePins();
        }

        private void zoomInButton_Click(object? sender, EventArgs e) => map.Zoom = Math.Min(map.MaxZoom, map.Zoom + 1);
        private void zoomOutButton_Click(object? sender, EventArgs e) => map.Zoom = Math.Max(map.MinZoom, map.Zoom - 1);

        private async void reloadConsumptionButton_Click(object? sender, EventArgs e) => await LoadConsumptionAsync();

        private async Task LoadConsumptionAsync()
        {
            loadCancellation?.Cancel();
            loadCancellation?.Dispose();
            loadCancellation = new CancellationTokenSource();
            var token = loadCancellation.Token;
            reloadConsumptionButton.Enabled = false;
            consumptionStatus.Text = "Loading readings…";
            try
            {
                var result = previewData ?? await WaterMeterConsumptionData.LoadAsync(token);
                if (!IsDisposed && !Disposing && !token.IsCancellationRequested) ShowConsumption(result);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) when (ex is Microsoft.Data.SqlClient.SqlException or InvalidOperationException or TimeoutException)
            {
                if (IsDisposed || Disposing || token.IsCancellationRequested) return;
                consumption = null;
                consumptionGrid.Rows.Clear();
                consumptionGrid.Columns.Clear();
                consumptionChart.SetSeries();
                consumptionChart.Heading = "Select a meter";
                consumptionStatus.Text = "Readings unavailable. Refresh to retry.";
                System.Diagnostics.Trace.TraceError("Water meter consumption: " + ex.Message);
            }
            finally
            {
                if (!IsDisposed && !Disposing) reloadConsumptionButton.Enabled = true;
            }
        }

        private void ShowConsumption(WaterMeterConsumptionData result)
        {
            consumption = result;
            selectingMeter = true;
            consumptionGrid.SuspendLayout();
            try
            {
                consumptionGrid.Rows.Clear();
                consumptionGrid.Columns.Clear();
                consumptionGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Building", HeaderText = "Building",
                    ValueType = typeof(int), AutoSizeMode = DataGridViewAutoSizeColumnMode.None, Width = 74, SortMode = DataGridViewColumnSortMode.Automatic });
                consumptionGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "MeterId", HeaderText = "Meter ID",
                    ValueType = typeof(int), AutoSizeMode = DataGridViewAutoSizeColumnMode.None, Width = 94, SortMode = DataGridViewColumnSortMode.Automatic });
                foreach (var month in result.Months)
                    consumptionGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = $"M{month.Year}{month.Number:00}", HeaderText = month.ToString() + " (L)",
                        ValueType = typeof(decimal), MinimumWidth = 112, SortMode = DataGridViewColumnSortMode.Automatic,
                        DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", NullValue = "—", Alignment = DataGridViewContentAlignment.MiddleRight } });
                foreach (var usage in result.Meters)
                {
                    int index = consumptionGrid.Rows.Add();
                    var row = consumptionGrid.Rows[index];
                    row.Tag = usage;
                    row.Cells[0].Value = usage.Meter.MapNumber;
                    row.Cells[1].Value = usage.Meter.Id;
                    for (int i = 0; i < result.Months.Count; i++)
                    {
                        var value = usage.Months[result.Months[i]];
                        row.Cells[i + 2].Value = value.Litres;
                        row.Cells[i + 2].Tag = value;
                        row.Cells[i + 2].ToolTipText = value.Note;
                    }
                }
                consumptionStatus.Text = result.Months.Count == 0 ? "No complete monthly readings yet."
                    : $"{result.Meters.Count} meters · {result.Months.Count} months";
            }
            finally
            {
                consumptionGrid.ResumeLayout();
                selectingMeter = false;
            }
            var selected = consumptionGrid.Rows.Cast<DataGridViewRow>().FirstOrDefault(r =>
                (r.Tag as WaterMeterConsumptionData.MeterUsage)?.Meter.MapNumber == selectedNumber)
                ?? consumptionGrid.Rows.Cast<DataGridViewRow>().FirstOrDefault();
            if (selected != null) consumptionGrid.CurrentCell = selected.Cells[0];
            ShowSelectedConsumption();
        }

        private void consumptionGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 2) return;
            if (consumptionGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag is WaterMeterConsumptionData.Consumption { Litres: { } litres, Estimated: true })
            {
                e.Value = "≈ " + litres.ToString("N0");
                e.FormattingApplied = true;
            }
        }

        private void consumptionGrid_CurrentCellChanged(object? sender, EventArgs e)
        {
            if (selectingMeter) return;
            ShowSelectedConsumption();
            if (consumptionGrid.CurrentRow?.Tag is not WaterMeterConsumptionData.MeterUsage usage) return;
            var building = buildings.FirstOrDefault(b => b.Number == usage.Meter.MapNumber);
            if (building == null) return;
            selectingMeter = true;
            try { SelectBuilding(building, false); }
            finally { selectingMeter = false; }
        }

        private void ShowSelectedConsumption()
        {
            consumptionChart.SetSeries();
            if (consumption == null || consumptionGrid.CurrentRow?.Tag is not WaterMeterConsumptionData.MeterUsage usage)
            {
                consumptionChart.Heading = "Select a meter";
                return;
            }
            consumptionChart.Heading = $"Building {usage.Meter.MapNumber} · {usage.Meter.Id}";
            var points = consumption.Months.Where(m => usage.Months[m].Litres.HasValue)
                .Select(m => new ChartPoint(WaterMeterConsumptionData.ChartTime(m), (double)usage.Months[m].Litres!.Value)).ToList();
            consumptionChart.SetSeries(new ChartSeries("Consumption", Color.SteelBlue, points, TimeSpan.FromDays(40)));
        }

        private void LayoutMap()
        {
            if (page == null || IsDesigner()) return;
            float scale = DeviceDpi / 96f;
            int gap = (int)(24 * scale);
            int available = ClientSize.Width - gap * 2 - SystemInformation.VerticalScrollBarWidth;
            int width = Math.Min((int)(1100 * scale), Math.Max((int)(280 * scale), available));
            bool narrow = width < 900 * scale;
            page.SuspendLayout();
            consumptionPanel.SuspendLayout();
            consumptionPanel.ColumnCount = 2;
            consumptionPanel.RowCount = 2;
            consumptionPanel.SetCellPosition(consumptionChart, narrow ? new TableLayoutPanelCellPosition(0, 1) : new TableLayoutPanelCellPosition(1, 0));
            consumptionPanel.ColumnCount = narrow ? 1 : 2;
            consumptionPanel.RowCount = narrow ? 2 : 1;
            consumptionPanel.ColumnStyles.Clear();
            consumptionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, narrow ? 100 : 68));
            if (!narrow) consumptionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
            consumptionPanel.RowStyles.Clear();
            consumptionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, narrow ? 55 : 100));
            if (narrow) consumptionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            consumptionGrid.Margin = narrow ? new Padding(0, 0, 0, 10) : new Padding(0, 0, 10, 0);
            page.RowStyles[0].Height = (float)Math.Clamp(width * .34, 300 * scale, 400 * scale);
            page.RowStyles[1].Height = 38 * scale;
            page.RowStyles[2].Height = (narrow ? 530 : 320) * scale;
            page.RowStyles[3].Height = (narrow ? 48 : 32) * scale;
            int height = page.RowStyles.Cast<RowStyle>().Sum(r => (int)r.Height);
            page.Size = new Size(width, height);
            page.Location = new Point(gap + Math.Max(0, (available-width)/2), gap + AutoScrollPosition.Y);
            AutoScrollMinSize = new Size(0, height+gap*2);
            consumptionPanel.ResumeLayout();
            page.ResumeLayout();
        }
    }
}
