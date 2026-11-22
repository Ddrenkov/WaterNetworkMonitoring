using GMap.NET;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;
using NetTopologySuite.IO;
using System.Text.Json;

namespace WaterNetworkMonitoring
{
    // Proposed pipes follow OSM streets. This is a visual graph, not surveyed sewer infrastructure.
    internal sealed class WasteWaterNetwork
    {
        internal sealed record Node(long Id, PointLatLng Position, int? BuildingNumber = null);
        internal sealed record Pipe(long From, long To, string Street, double LengthMetres, int? BuildingNumber = null);
        public IReadOnlyDictionary<long, Node> Nodes { get; }
        public IReadOnlyList<Pipe> Pipes { get; }
        public int ComponentCount { get; }
        private static readonly HashSet<string> ExcludedStreets = new(StringComparer.Ordinal)
        {
            "Иван Михайлов", "Александър Малинов", "бул. Свети Димитър Солунски", "Марица"
        };
        private WasteWaterNetwork(Dictionary<long, Node> nodes, List<Pipe> pipes, int componentCount)
        {
            Nodes = nodes;
            Pipes = pipes;
            ComponentCount = componentCount;

        }

        private sealed class StreetPoint
        {
            public long Id { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
        private sealed class Street
        {
            public string Name { get; set; } = "";
            public List<StreetPoint> Points { get; set; } = new();
        }
        private sealed class Snapshot { public List<Street> Roads { get; set; } = new(); }
        private sealed record Segment(long From, long To, string Street);

        public static WasteWaterNetwork Create(List<BuildingMapData.Building> buildings, BlagoevgradNeighbourhoods.Zone zone)
        {
            using var stream = typeof(WasteWaterNetwork).Assembly.GetManifestResourceStream("WaterNetworkMonitoring.Data.Maps.ZapadStreets.json")
                ?? throw new InvalidOperationException("Zapad street geometry is missing.");
            var snapshot = JsonSerializer.Deserialize<Snapshot>(stream) ?? throw new InvalidOperationException("Invalid street geometry.");
            var factory = new GeometryFactory(new PrecisionModel(), 4326);
            var ring = zone.Points.Select(p => new Coordinate(p[1],p[0])).ToList();
            if (!ring[0].Equals2D(ring[^1])) ring.Add(ring[0].Copy());
            var area = factory.CreatePolygon(ring.ToArray()).Buffer(.0005);
            var nodes = new Dictionary<long, Node>();
            var segments = new Dictionary<(long,long), Segment>();
            foreach (var street in snapshot.Roads.Where(street => !ExcludedStreets.Contains(street.Name)))
                for (int i = 1; i < street.Points.Count; i++)
                {
                    var a = street.Points[i-1]; var b = street.Points[i];
                    if (a.Id == b.Id || (a.Latitude == b.Latitude && a.Longitude == b.Longitude)) continue;
                    var line = factory.CreateLineString(new[] { new Coordinate(a.Longitude,a.Latitude), new Coordinate(b.Longitude,b.Latitude) });
                    if (!area.Covers(line)) continue;
                    var from = new Node(a.Id,new PointLatLng(a.Latitude,a.Longitude));
                    var to = new Node(b.Id,new PointLatLng(b.Latitude,b.Longitude));
                    if (Distance(from.Position,to.Position) < .01) continue;
                    nodes.TryAdd(from.Id,from);
                    nodes.TryAdd(to.Id,to);
                    var key = (Math.Min(from.Id,to.Id),Math.Max(from.Id,to.Id));
                    segments.TryAdd(key,new Segment(key.Item1,key.Item2,street.Name));
                }
            if (segments.Count == 0) throw new InvalidOperationException("No street graph is available in Zapad.");
            var roads = segments.Values.OrderBy(s => s.From).ThenBy(s => s.To).ToArray();
            var splits = roads.ToDictionary(s => s, s => new List<(double Fraction,long Node)> { (0,s.From),(1,s.To) });
            var reader = new WKTReader();
            var footprints = buildings.ToDictionary(b => b.Number,b => string.IsNullOrWhiteSpace(b.FootprintWkt) ? null :
                PreparedGeometryFactory.Prepare(reader.Read(b.FootprintWkt).Buffer(-1e-9)));
            var laterals = new List<Pipe>();
            for (int index = 0; index < buildings.Count; index++)
            {
                var building = buildings[index];
                var options = roads.SelectMany(road =>
                {
                    var a = nodes[road.From].Position; var b = nodes[road.To].Position;
                    double nearest = Projection(building.Position,a,b);
                    return new[] { nearest,0d,.25,.5,.75,1d }.Distinct().Select(t =>
                        (Road:road,Fraction:t,Point:Along(a,b,t),Length:Distance(building.Position,Along(a,b,t))));
                }).OrderBy(o => o.Length);
                bool connected = false;
                foreach (var option in options)
                {
                    var lateral = factory.CreateLineString(new[] { new Coordinate(building.Longitude,building.Latitude),
                        new Coordinate(option.Point.Lng,option.Point.Lat) });
                    bool blocked = footprints.Where(p => p.Key != building.Number).Any(p => p.Value?.Intersects(lateral) == true);
                    if (blocked) continue;
                    var onRoad = splits[option.Road].FirstOrDefault(p => Math.Abs(p.Fraction-option.Fraction) < 1e-8);
                    long junction = onRoad.Node;
                    if (junction == 0)
                    {
                        junction = -10000-building.Number;
                        nodes.Add(junction,new Node(junction,option.Point));
                        splits[option.Road].Add((option.Fraction,junction));
                    }
                    long buildingId = -building.Number;
                    nodes.Add(buildingId,new Node(buildingId,building.Position,building.Number));
                    laterals.Add(new Pipe(buildingId,junction,option.Road.Street,option.Length,building.Number));
                    connected = true;
                    break;
                }
                if (!connected) throw new InvalidOperationException($"No unobstructed street connection for building {building.Number}.");
            }
            var pipes = new List<Pipe>();
            foreach (var road in roads)
            {
                var points = splits[road].OrderBy(p => p.Fraction).ToArray();
                for (int i = 1; i < points.Length; i++)
                    if (Distance(nodes[points[i-1].Node].Position,nodes[points[i].Node].Position) > .01)
                        pipes.Add(new Pipe(points[i-1].Node,points[i].Node,road.Street,
                            Distance(nodes[points[i-1].Node].Position,nodes[points[i].Node].Position)));
            }
            pipes.AddRange(laterals);
            int componentCount = TrimUnusedBranches(nodes,pipes);
            return new WasteWaterNetwork(nodes,pipes,componentCount);
        }

        private static int TrimUnusedBranches(Dictionary<long,Node> nodes,List<Pipe> pipes)
        {
            var adjacent = nodes.Keys.ToDictionary(id => id,_ => new List<long>());
            foreach (var pipe in pipes) { adjacent[pipe.From].Add(pipe.To); adjacent[pipe.To].Add(pipe.From); }
            var seen = new HashSet<long>();
            var componentOf = new Dictionary<long,int>();
            var streetCounts = new Dictionary<int,int>();
            int groups = 0;
            foreach (long id in nodes.Keys)
            {
                if (seen.Contains(id)) continue;
                var component = Reachable(adjacent,id);
                seen.UnionWith(component);
                if (!component.Any(n => nodes[n].BuildingNumber.HasValue)) continue;
                foreach (long node in component) componentOf[node] = groups;
                streetCounts[groups] = pipes.Count(p => p.BuildingNumber == null && component.Contains(p.From));
                groups++;
            }
            pipes.RemoveAll(p => !componentOf.ContainsKey(p.From));
            foreach (long id in nodes.Keys.Where(id => !componentOf.ContainsKey(id)).ToArray()) nodes.Remove(id);
            var attachments = pipes.Where(p => p.BuildingNumber.HasValue).SelectMany(p => new[] { p.From,p.To }).ToHashSet();
            // Remove unused road-end extensions, preserving every building link and each local pipe group.
            while (true)
            {
                var degree = pipes.SelectMany(p => new[] { p.From,p.To }).GroupBy(id => id).ToDictionary(g => g.Key,g => g.Count());
                long? end = nodes.Keys.Where(id => !attachments.Contains(id) && degree.GetValueOrDefault(id) == 1 &&
                    streetCounts[componentOf[id]] > 1).OrderBy(id => id).Select(id => (long?)id).FirstOrDefault();
                if (!end.HasValue) break;
                var pipe = pipes.Single(p => p.From == end.Value || p.To == end.Value);
                pipes.Remove(pipe);
                streetCounts[componentOf[end.Value]]--;
                nodes.Remove(end.Value);
            }
            return groups;
        }

        private static HashSet<long> Reachable(Dictionary<long,List<long>> neighbours,long start)
        {
            var visited = new HashSet<long> { start };
            var queue = new Queue<long>(); queue.Enqueue(start);
            while (queue.TryDequeue(out long id))
                foreach (long next in neighbours[id]) if (visited.Add(next)) queue.Enqueue(next);
            return visited;
        }
        private static PointLatLng Along(PointLatLng a, PointLatLng b, double t) => new(a.Lat+(b.Lat-a.Lat)*t,a.Lng+(b.Lng-a.Lng)*t);
        private static double Projection(PointLatLng p,PointLatLng a,PointLatLng b)
        {
            double scale = Math.Cos(p.Lat*Math.PI/180), dx = (b.Lng-a.Lng)*scale, dy = b.Lat-a.Lat;
            return Math.Clamp(((p.Lng-a.Lng)*scale*dx+(p.Lat-a.Lat)*dy)/(dx*dx+dy*dy),0,1);
        }
        internal static double Distance(PointLatLng a,PointLatLng b)
        {
            double dy = (b.Lat-a.Lat)*Math.PI/180, dx = (b.Lng-a.Lng)*Math.PI/180;
            double h = Math.Pow(Math.Sin(dy/2),2)+Math.Cos(a.Lat*Math.PI/180)*Math.Cos(b.Lat*Math.PI/180)*Math.Pow(Math.Sin(dx/2),2);
            return 12742000*Math.Asin(Math.Sqrt(Math.Clamp(h,0,1)));
        }
    }
}
