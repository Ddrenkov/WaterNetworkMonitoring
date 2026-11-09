using GMap.NET;
using GMap.NET.WindowsForms;
using System.ComponentModel;

namespace WaterNetworkMonitoring
{
    public partial class Loggers : Form
    {
        private GMapOverlay? neighbourhoods;
        private bool updatingSelection;
        private string? selectedZoneName;

        public Loggers()
        {
            InitializeComponent();
            map.Load += map_Load;
            Resize += (_, _) => LayoutMap();
            LayoutMap();
        }

        private void Loggers_Load(object? sender, EventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime || neighbourhoods != null) return;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            map.MapProvider = OSMap.Instance;
            CenterOnZapad();
            neighbourhoods = BlagoevgradNeighbourhoods.CreateOverlay();
            map.Overlays.Add(neighbourhoods);
            foreach (var polygon in neighbourhoods.Polygons) map.UpdatePolygonLocalPosition(polygon);
            foreach (var marker in neighbourhoods.Markers) map.UpdateMarkerLocalPosition(marker);
            foreach (var polygon in neighbourhoods.Polygons) zoneSelector.Items.Add(polygon.Name);
            zoneSelector.SelectedIndex = 0;
            zoneSelector.Enabled = false;
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
                if (args.Button == MouseButtons.Left) SelectZone((BlagoevgradNeighbourhoods.Zone)marker.Tag);
            };
            map.OnPolygonEnter += _ => map.Cursor = Cursors.Hand;
            map.OnPolygonLeave += _ => map.Cursor = Cursors.Default;
            map.OnMarkerEnter += _ => map.Cursor = Cursors.Hand;
            map.OnMarkerLeave += _ => map.Cursor = Cursors.Default;
            map.OnMapZoomChanged += UpdateLabelVisibility;
            map.OnPositionChanged += _ => UpdateLabelVisibility();
            UpdateLabelVisibility();
            map.Refresh();
        }

        private void resetButton_Click(object? sender, EventArgs e)
        {
            if (neighbourhoods == null) return;
            selectedZoneName = null;
            foreach (var polygon in neighbourhoods.Polygons) BlagoevgradNeighbourhoods.StylePolygon(polygon, false);
            CenterOnZapad();
            updatingSelection = true;
            zoneSelector.SelectedIndex = 0;
            updatingSelection = false;
            selectionLabel.Text = "Zapad. Click the blue outline to select it. Drag to pan; scroll to zoom.";
            UpdateLabelVisibility();
            map.Refresh();
        }

        private void CenterOnZapad()
        {
            var zone = BlagoevgradNeighbourhoods.LoadZones().Single(z => z.Name == "Zapad");
            map.SetZoomToFitRect(RectLatLng.FromLTRB(zone.Points.Min(p => p[1]) - .0003,
                zone.Points.Max(p => p[0]) + .0003, zone.Points.Max(p => p[1]) + .0003,
                zone.Points.Min(p => p[0]) - .0003));
        }

        private void map_Load(object? sender, EventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            // Apply the Zapad view after GMap completes its own startup and layout.
            map.BeginInvoke(new Action(() =>
            {
                if (IsDisposed || map.IsDisposed) return;
                CenterOnZapad();
                UpdateLabelVisibility();
            }));
        }

        private void SelectZone(BlagoevgradNeighbourhoods.Zone zone)
        {
            if (neighbourhoods == null) return;
            selectedZoneName = zone.Name;
            foreach (var polygon in neighbourhoods.Polygons)
                BlagoevgradNeighbourhoods.StylePolygon(polygon, polygon.Name == zone.Name);
            updatingSelection = true;
            zoneSelector.SelectedItem = zone.Name;
            updatingSelection = false;
            selectionLabel.Text = zone.Name + " — " + zone.BoundaryNote;
            UpdateLabelVisibility();
            map.Refresh();
        }

        private void zoneSelector_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (updatingSelection || neighbourhoods == null) return;
            if (zoneSelector.SelectedIndex == 0)
            {
                resetButton_Click(this, EventArgs.Empty);
                return;
            }
            var polygon = neighbourhoods.Polygons.FirstOrDefault(p => p.Name == zoneSelector.SelectedItem as string);
            if (polygon?.Tag is not BlagoevgradNeighbourhoods.Zone zone) return;
            SelectZone(zone);
            double north = polygon.Points.Max(p => p.Lat), south = polygon.Points.Min(p => p.Lat);
            double west = polygon.Points.Min(p => p.Lng), east = polygon.Points.Max(p => p.Lng);
            map.SetZoomToFitRect(RectLatLng.FromLTRB(west - .002, north + .002, east + .002, south - .002));
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
            mapPanel.Size = new Size(Math.Min(1100, Math.Max(1, ClientSize.Width - 48)), Math.Min(620, Math.Max(1, ClientSize.Height - 72)));
            mapPanel.Location = new Point(
                (ClientSize.Width - mapPanel.Width) / 2,
                (ClientSize.Height - mapPanel.Height) / 2);
        }
    }
}


