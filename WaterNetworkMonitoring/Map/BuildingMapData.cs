using GMap.NET;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;
using NetTopologySuite.IO;
using NetTopologySuite.Operation.Overlay;
using NetTopologySuite.Operation.OverlayNG;
using System.Text.Json;

namespace WaterNetworkMonitoring
{
    internal static class BuildingMapData
    {
        private static readonly GeometryFactory GeometryFactory = new(new PrecisionModel(), 4326);
        // Discard line/point touches and floating-point remnants, not building areas.
        private const double MinimumOverlapArea = 1e-14;

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

        private sealed record Region(string Name, Geometry Geometry, IPreparedGeometry Prepared);

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
            var regions = zones.Select(z =>
            {
                var polygon = GeometryFactory.CreatePolygon(CreateRing(z.Points), z.Holes.Select(CreateRing).ToArray());
                return new Region(z.Name, polygon, PreparedGeometryFactory.Prepare(polygon));
            }).ToArray();
            var reader = new WKTReader();
            var result = new List<Building>();
            foreach (var building in snapshot.Buildings
                .DistinctBy(b => (b.OsmType, b.OsmId))
                .OrderBy(b => b.OsmType, StringComparer.Ordinal).ThenBy(b => b.OsmId))
            {
                if (string.IsNullOrWhiteSpace(building.FootprintWkt))
                {
                    // A few OSM buildings are mapped only as standalone points.
                    building.Zones = zones.Where(z => BlagoevgradNeighbourhoods.Contains(z, building.Position))
                        .Select(z => z.Name).ToArray();
                }
                else
                {
                    AssignFootprint(building, reader.Read(building.FootprintWkt), regions);
                }
                if (building.Zones.Length == 0) continue;
                building.Number = result.Count + 1;
                result.Add(building);
            }
            return result;
        }

        private static LinearRing CreateRing(double[][] points)
        {
            var coordinates = points.Select(p => new Coordinate(p[1], p[0])).ToList();
            if (!coordinates[0].Equals2D(coordinates[^1])) coordinates.Add(coordinates[0].Copy());
            return GeometryFactory.CreateLinearRing(coordinates.ToArray());
        }

        private static void AssignFootprint(Building building, Geometry footprint, IReadOnlyList<Region> regions)
        {
            footprint.SRID = 4326;
            var originalPoint = GeometryFactory.CreatePoint(new Coordinate(building.Longitude, building.Latitude));
            var names = new List<string>();
            Geometry? largestOverlap = null;
            bool keepOriginalPosition = false;
            foreach (var region in regions)
            {
                if (!region.Geometry.EnvelopeInternal.Intersects(footprint.EnvelopeInternal)) continue;
                if (!region.Prepared.Intersects(footprint)) continue;
                var overlap = region.Prepared.Covers(footprint) ? footprint :
                    OverlayNGRobust.Overlay(footprint, region.Geometry, SpatialFunction.Intersection);
                if (overlap.IsEmpty || overlap.Area <= MinimumOverlapArea) continue;
                names.Add(region.Name);
                if (region.Prepared.Contains(originalPoint) && footprint.Contains(originalPoint)) keepOriginalPosition = true;
                if (largestOverlap == null || overlap.Area > largestOverlap.Area) largestOverlap = overlap;
            }
            building.Zones = names.ToArray();
            if (largestOverlap == null || keepOriginalPosition) return;

            // A centre point can sit outside a zone even when the building crosses
            // its edge. Anchor that pin inside the footprint's in-zone portion.
            var anchor = largestOverlap.InteriorPoint;
            building.Latitude = anchor.Y;
            building.Longitude = anchor.X;
        }
    }
}
