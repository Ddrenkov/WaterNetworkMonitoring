using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;
using NetTopologySuite.IO;

namespace WaterNetworkMonitoring
{
    // An illustrative weighted, undirected graph. Actual underground pipe locations are unknown.
    // Buildings are nodes; clear building-to-building connections are candidate edges.
    internal sealed class BuildingPipeGraph
    {
        internal sealed record Connection(BuildingMapData.Building From, BuildingMapData.Building To, double LengthMetres)
        {
            public string Id => $"{From.Number}-{To.Number}";
            public bool Touches(int? number) => From.Number == number || To.Number == number;
            public int Other(int number) => From.Number == number ? To.Number : To.Number == number ? From.Number :
                throw new ArgumentException("Building is not on this connection.");
        }

        public IReadOnlyList<Connection> Connections { get; }
        private readonly Dictionary<int, List<Connection>> neighbours;

        private BuildingPipeGraph(List<BuildingMapData.Building> buildings, List<Connection> connections)
        {
            Connections = connections;
            neighbours = buildings.ToDictionary(b => b.Number, _ => new List<Connection>());
            foreach (var pipe in connections)
            {
                neighbours[pipe.From.Number].Add(pipe);
                neighbours[pipe.To.Number].Add(pipe);
            }
        }

        public static BuildingPipeGraph Create(List<BuildingMapData.Building> buildings, BlagoevgradNeighbourhoods.Zone zone)
        {
            if (zone.Name != "Zapad" || buildings.Count == 0 || buildings.Any(b => !b.Zones.Contains("Zapad")))
                throw new InvalidOperationException("The pipe graph requires Zapad buildings.");
            var factory = new GeometryFactory(new PrecisionModel(), 4326);
            LinearRing Ring(double[][] points)
            {
                var coordinates = points.Select(p => new Coordinate(p[1], p[0])).ToList();
                if (!coordinates[0].Equals2D(coordinates[^1])) coordinates.Add(coordinates[0].Copy());
                return factory.CreateLinearRing(coordinates.ToArray());
            }
            var boundary = factory.CreatePolygon(Ring(zone.Points), zone.Holes.Select(Ring).ToArray()).Buffer(1e-9);
            var reader = new WKTReader();
            var footprints = buildings.Select(b => string.IsNullOrWhiteSpace(b.FootprintWkt) ? null :
                PreparedGeometryFactory.Prepare(reader.Read(b.FootprintWkt).Buffer(-1e-9))).ToArray();
            var candidates = new List<Connection>();
            for (int i = 0; i < buildings.Count; i++)
                for (int j = i + 1; j < buildings.Count; j++)
                {
                    var a = buildings[i];
                    var b = buildings[j];
                    var line = factory.CreateLineString(new[] { new Coordinate(a.Longitude, a.Latitude), new Coordinate(b.Longitude, b.Latitude) });
                    if (!boundary.Covers(line)) continue;
                    bool blocked = false;
                    for (int k = 0; k < footprints.Length; k++)
                        if (k != i && k != j && footprints[k]?.Intersects(line) == true) { blocked = true; break; }
                    if (!blocked) candidates.Add(new Connection(a, b, DistanceMetres(a, b)));
                }
            candidates = candidates.OrderBy(p => p.LengthMetres).ThenBy(p => p.From.Number).ThenBy(p => p.To.Number).ToList();

            // Keep Skaptopara as a leaf, matching the existing logger that supplies only its meter.
            var branch = buildings.FirstOrDefault(b => b.OsmType == "way" && b.OsmId == 992665907);
            var backbone = buildings.Where(b => b != branch).ToList();
            if (backbone.Count == 0) throw new InvalidOperationException("No network backbone is available.");
            var visited = new HashSet<int> { backbone[0].Number };
            var pipes = new List<Connection>();
            // Prim's algorithm: add the shortest edge joining the tree to an unvisited building.
            while (visited.Count < backbone.Count)
            {
                var next = candidates.FirstOrDefault(p => p.From != branch && p.To != branch &&
                    visited.Contains(p.From.Number) != visited.Contains(p.To.Number));
                if (next == null) throw new InvalidOperationException("No clear connection exists for every Zapad building.");
                pipes.Add(next);
                visited.Add(next.From.Number);
                visited.Add(next.To.Number);
            }
            if (branch != null)
            {
                var service = candidates.FirstOrDefault(p => p.Touches(branch.Number))
                    ?? throw new InvalidOperationException("No clear Skaptopara connection is available.");
                pipes.Add(service);
            }
            var graph = new BuildingPipeGraph(buildings, pipes);
            if (graph.ReachableFrom(buildings[0].Number).Count != buildings.Count)
                throw new InvalidOperationException("A building is disconnected from the graph.");
            return graph;
        }

        public IReadOnlyList<Connection> ConnectionsAt(int buildingNumber) => neighbours[buildingNumber];

        // Breadth-first search verifies which building nodes can be reached through the graph.
        public HashSet<int> ReachableFrom(int buildingNumber)
        {
            var reached = new HashSet<int> { buildingNumber };
            var queue = new Queue<int>();
            queue.Enqueue(buildingNumber);
            while (queue.TryDequeue(out int number))
                foreach (var pipe in neighbours[number])
                    if (reached.Add(pipe.Other(number))) queue.Enqueue(pipe.Other(number));
            return reached;
        }

        private static double DistanceMetres(BuildingMapData.Building a, BuildingMapData.Building b)
        {
            const double radians = Math.PI / 180;
            double lat = (b.Latitude - a.Latitude) * radians;
            double lon = (b.Longitude - a.Longitude) * radians;
            double h = Math.Pow(Math.Sin(lat / 2), 2) + Math.Cos(a.Latitude * radians) * Math.Cos(b.Latitude * radians) * Math.Pow(Math.Sin(lon / 2), 2);
            return 6371000 * 2 * Math.Asin(Math.Sqrt(Math.Clamp(h, 0, 1)));
        }
    }
}
