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
            public string BoundaryNote { get; set; } = "";
            public double[][] Points { get; set; } = Array.Empty<double[]>();
            public double[][][] Holes { get; set; } = Array.Empty<double[][]>();
        }

        public static List<Zone> LoadAllZones()
        {
            using var stream = typeof(BlagoevgradNeighbourhoods).Assembly.GetManifestResourceStream(
                "WaterNetworkMonitoring.Data.Maps.Neighbourhoods.json")
                ?? throw new InvalidOperationException("Neighbourhood map data is missing.");
            return JsonSerializer.Deserialize<List<Zone>>(stream) ?? new List<Zone>();
        }

        public static List<Zone> LoadZones()
        {
            return LoadAllZones().Where(zone => zone.Name == "Zapad").ToList();
        }

        private static GMapPolygon CreatePolygon(Zone zone)
        {
            var points = new List<PointLatLng>();
            foreach (var point in zone.Points)
                points.Add(new PointLatLng(point[0], point[1]));

            return new GMapPolygon(points, zone.Name)
            {
                Tag = zone,
                IsHitTestVisible = true
            };
        }

        public static GMapOverlay CreateOverlay() => CreateOverlay(LoadZones());

        public static GMapOverlay CreateOverlay(IEnumerable<Zone> zones)
        {
            var overlay = new GMapOverlay("neighbourhoods");
            foreach (var zone in zones)
            {
                var polygon = CreatePolygon(zone);
                StylePolygon(polygon, false);
                overlay.Polygons.Add(polygon);
            }
            return overlay;
        }

        public static bool Contains(Zone zone, PointLatLng point) => CreatePolygon(zone).IsInside(point);

        public static bool Contains(GMapPolygon polygon, PointLatLng point) => polygon.IsInside(point);

        public static void StylePolygon(GMapPolygon polygon, bool selected)
        {
            if (polygon.Stroke != GMapPolygon.DefaultStroke) polygon.Stroke.Dispose();
            if (polygon.Fill != GMapPolygon.DefaultFill) polygon.Fill.Dispose();

            polygon.Stroke = new Pen(Color.FromArgb(selected ? 255 : 210, 35, 105, 220), selected ? 3 : 1.8f);
            polygon.Fill = new SolidBrush(Color.FromArgb(selected ? 40 : 16, 65, 145, 235));
        }
    }
}
