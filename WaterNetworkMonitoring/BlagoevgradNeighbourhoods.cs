using GMap.NET;
using GMap.NET.WindowsForms;
using System.Text.Json;

namespace WaterNetworkMonitoring
{
    internal static class BlagoevgradNeighbourhoods
    {
        internal sealed class Zone
        {
            public string Name { get; set; } = "";
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string SourceUrl { get; set; } = "";
            public bool IsApproximate { get; set; }
            public string ReferenceImage { get; set; } = "";
            public string BoundaryNote { get; set; } = "";
            public double[][] Points { get; set; } = Array.Empty<double[]>();
            public double[][][] Holes { get; set; } = Array.Empty<double[][]>();
        }

        public static List<Zone> LoadZones()
        {
            using var stream = typeof(BlagoevgradNeighbourhoods).Assembly.GetManifestResourceStream(
                "WaterNetworkMonitoring.Data.Maps.Neighbourhoods.json")
                ?? throw new InvalidOperationException("Neighbourhood map data is missing.");
            return JsonSerializer.Deserialize<List<Zone>>(stream) ?? new List<Zone>();
        }

        public static bool Contains(Zone zone, PointLatLng point)
        {
            return InsideRing(zone.Points, point) && !zone.Holes.Any(hole => InsideRing(hole, point));
        }

        public static GMapOverlay CreateOverlay()
        {
            var zones = LoadZones();
            var overlay = new GMapOverlay("neighbourhoods");
            foreach (var zone in zones)
            {
                var polygon = new RegionPolygon(zone)
                {
                    Tag = zone,
                    IsHitTestVisible = true
                };
                StylePolygon(polygon, false);
                overlay.Polygons.Add(polygon);
                overlay.Markers.Add(new NeighbourhoodLabel(zone));
            }
            return overlay;
        }

        public static bool Contains(GMapPolygon polygon, PointLatLng point)
        {
            var zone = (Zone)polygon.Tag;
            return polygon.IsInside(point) && !zone.Holes.Any(hole => InsideRing(hole, point));
        }

        private static bool InsideRing(double[][] ring, PointLatLng point)
        {
            bool inside = false;
            for (int i = 0, j = ring.Length - 1; i < ring.Length; j = i++)
            {
                if ((ring[i][0] > point.Lat) != (ring[j][0] > point.Lat) &&
                    point.Lng < (ring[j][1] - ring[i][1]) * (point.Lat - ring[i][0]) /
                        (ring[j][0] - ring[i][0]) + ring[i][1]) inside = !inside;
            }
            return inside;
        }

        private sealed class RegionPolygon : GMapPolygon
        {
            private readonly Zone zone;
            public RegionPolygon(Zone zone) : base(zone.Points.Select(p => new PointLatLng(p[0], p[1])).ToList(), zone.Name)
            {
                this.zone = zone;
            }

            public override void OnRender(Graphics graphics)
            {
                if (!IsVisible || LocalPoints.Count < 3 || Overlay?.Control == null) return;
                using var path = new System.Drawing.Drawing2D.GraphicsPath(System.Drawing.Drawing2D.FillMode.Alternate);
                path.AddPolygon(LocalPoints.Select(p => new PointF(p.X, p.Y)).ToArray());
                var map = Overlay.Control;
                var origin = map.MapProvider.Projection.FromLatLngToPixel(Points[0], (int)map.Zoom);
                foreach (var hole in zone.Holes)
                {
                    var points = hole.Select(p =>
                    {
                        var pixel = map.MapProvider.Projection.FromLatLngToPixel(p[0], p[1], (int)map.Zoom);
                        return new PointF(LocalPoints[0].X + pixel.X - origin.X, LocalPoints[0].Y + pixel.Y - origin.Y);
                    }).ToArray();
                    if (points.Length >= 3) path.AddPolygon(points);
                }
                var smoothing = graphics.SmoothingMode;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.FillPath(Fill, path);
                graphics.DrawPath(Stroke, path);
                graphics.SmoothingMode = smoothing;
            }
        }

        public static void StylePolygon(GMapPolygon polygon, bool selected)
        {
            // Keep the original shared GMap default brushes alive.
            if (polygon.Stroke != GMapPolygon.DefaultStroke) polygon.Stroke.Dispose();
            if (polygon.Fill != GMapPolygon.DefaultFill) polygon.Fill.Dispose();
            polygon.Stroke = new Pen(Color.FromArgb(selected ? 255 : 210, 35, 105, 220), selected ? 3 : 1.8f)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
                LineJoin = System.Drawing.Drawing2D.LineJoin.Round
            };
            polygon.Fill = new SolidBrush(Color.FromArgb(selected ? 40 : 16, 65, 145, 235));
        }

        private sealed class NeighbourhoodLabel : GMapMarker
        {
            private readonly string name;
            public NeighbourhoodLabel(Zone zone) : base(new PointLatLng(zone.Latitude, zone.Longitude))
            {
                name = zone.Name;
                Tag = zone;
                ToolTipText = zone.Name + "\n" + zone.BoundaryNote;
                Size = new Size(TextRenderer.MeasureText(name, SystemFonts.DefaultFont).Width + 16, 26);
                Offset = new Point(-Size.Width / 2, -Size.Height / 2);
                IsHitTestVisible = true;
            }

            public override void OnRender(Graphics graphics)
            {
                var bounds = new Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
                using var background = new SolidBrush(Color.FromArgb(235, Color.White));
                using var border = new Pen(Color.FromArgb(180, 145, 180, 225));
                graphics.FillRectangle(background, bounds);
                graphics.DrawRectangle(border, bounds);
                using var textBrush = new SolidBrush(Color.FromArgb(25, 65, 110));
                using var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                graphics.DrawString(name, SystemFonts.DefaultFont, textBrush, bounds, format);
            }
        }
    }
}

