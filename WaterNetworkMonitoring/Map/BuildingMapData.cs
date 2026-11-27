using GMap.NET;
using System.Text.Json;

namespace WaterNetworkMonitoring
{
    internal static class BuildingMapData
    {
        internal sealed class Building
        {
            public string OsmType { get; set; } = "";
            public long OsmId { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string Name { get; set; } = "";
            public string Address { get; set; } = "";
            public string BuildingType { get; set; } = "";
            public string FootprintWkt { get; set; } = "";
            public int Number { get; set; }
            public string[] Zones { get; set; } = Array.Empty<string>();
            public PointLatLng Position => new PointLatLng(Latitude, Longitude);
        }

        private sealed class Snapshot
        {
            public List<Building> Buildings { get; set; } = new();
        }

        public static List<Building> Load()
        {
            using var stream = typeof(BuildingMapData).Assembly.GetManifestResourceStream(
                "WaterNetworkMonitoring.Data.Maps.Buildings.json")
                ?? throw new InvalidOperationException("Building map data is missing.");
            var snapshot = JsonSerializer.Deserialize<Snapshot>(stream)
                ?? throw new InvalidOperationException("Building map data is invalid.");

            return snapshot.Buildings.OrderBy(building => building.Number).ToList();
        }
    }
}
