using GMap.NET;
using GMap.NET.WindowsForms;
using System.ComponentModel;

namespace WaterNetworkMonitoring
{
    public partial class WaterMeters : Form
    {
        private static readonly PointLatLng CityCenter = new(42.012, 23.097);
        private readonly GMapOverlay pins = new("buildings");
        private List<BuildingMapData.Building> buildings = new();
        private List<BlagoevgradNeighbourhoods.Zone> zones = new();
        private bool initialized;
        private int? selectedNumber;

        public WaterMeters()
        {
            InitializeComponent();
            refreshTimer.Tick += (_, _) => { refreshTimer.Stop(); UpdatePins(); };
            map.Load += (_, _) =>
            {
                if (!IsDesigner()) map.BeginInvoke(new Action(() =>
                {
                    if (IsDisposed) return;
                    CenterOnCity();
                    UpdatePins();
                }));
            };
            Resize += (_, _) => { LayoutMap(); QueueRefresh(); };
            LayoutMap();
        }

        private bool IsDesigner() => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        private void WaterMeters_Load(object? sender, EventArgs e)
        {
            if (IsDesigner() || initialized) return;
            zones = BlagoevgradNeighbourhoods.LoadZones();
            buildings = BuildingMapData.Load(zones);
            initialized = true;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            map.MapProvider = OSMap.Instance;
            // The zones select buildings only; this map deliberately has no polygon overlay.
            map.Overlays.Add(pins);
            zoneSelector.Items.Add("All regions");
            foreach (var zone in zones.OrderBy(z => z.Name)) zoneSelector.Items.Add(zone.Name);
            zoneSelector.SelectedIndex = 0;
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
            CenterOnCity();
            UpdatePins();
        }

        private void CenterOnCity()
        {
            map.Zoom = 14;
            map.Position = CityCenter;
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
                    var marker = new NumberedBuildingPin(building, map.Zoom >= 17, building.Number == selectedNumber);
                    pins.Markers.Add(marker);
                    map.UpdateMarkerLocalPosition(marker);
                }
            }
            finally { map.HoldInvalidation = false; }
            if (selectedNumber == null)
                statusLabel.Text = $"{filtered.Count:N0} mapped buildings • Pins 1–{buildings.Count:N0} • Zoom in for numbers";
            map.Refresh();
        }

        private void zoneSelector_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!initialized) return;
            selectedNumber = null;
            var zone = zones.FirstOrDefault(z => z.Name == zoneSelector.SelectedItem as string);
            if (zone == null) CenterOnCity();
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
            CenterOnCity();
            UpdatePins();
        }

        private void zoomInButton_Click(object? sender, EventArgs e) => map.Zoom = Math.Min(map.MaxZoom, map.Zoom + 1);
        private void zoomOutButton_Click(object? sender, EventArgs e) => map.Zoom = Math.Max(map.MinZoom, map.Zoom - 1);

        private void LayoutMap()
        {
            mapPanel.Size = new Size(Math.Min(1100, Math.Max(1, ClientSize.Width - 48)),
                Math.Min(620, Math.Max(1, ClientSize.Height - 72)));
            mapPanel.Location = new Point((ClientSize.Width - mapPanel.Width) / 2, (ClientSize.Height - mapPanel.Height) / 2);
        }
    }
}
