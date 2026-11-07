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
            public int Number { get; set; }
            public string[] Zones { get; set; } = Array.Empty<string>();
            public PointLatLng Position => new PointLatLng(Latitude, Longitude);
        }

        internal sealed class Snapshot
        {
            public string RetrievedAtUtc { get; set; } = "";
            public List<Building> Buildings { get; set; } = new();
        }

        public static List<Building> Load(IReadOnlyList<BlagoevgradNeighbourhoods.Zone> zones)
        {
            using var stream = typeof(BuildingMapData).Assembly.GetManifestResourceStream(
                "WaterNetworkMonitoring.Data.Maps.Buildings.json")
                ?? throw new InvalidOperationException("Building map data is missing.");
            var snapshot = JsonSerializer.Deserialize<Snapshot>(stream)
                ?? throw new InvalidOperationException("Building map data is invalid.");
            var result = new List<Building>();
            foreach (var building in snapshot.Buildings
                .DistinctBy(b => (b.OsmType, b.OsmId))
                .OrderBy(b => b.OsmType, StringComparer.Ordinal).ThenBy(b => b.OsmId))
            {
                building.Zones = zones.Where(z => BlagoevgradNeighbourhoods.Contains(z, building.Position))
                    .Select(z => z.Name).ToArray();
                if (building.Zones.Length == 0) continue;
                building.Number = result.Count + 1;
                result.Add(building);
            }
            return result;
        }
    }
}
