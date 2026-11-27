using GMap.NET;
using GMap.NET.WindowsForms;
using System.Text.Json;

namespace WaterNetworkMonitoring
{
    internal sealed class BuildingPipeGraph
    {
        internal sealed record Connection(
            BuildingMapData.Building From,
            BuildingMapData.Building To)
        {
            public string Id => $"{From.Number}-{To.Number}";

            public bool Touches(int? number) =>
                From.Number == number || To.Number == number;

            public int Other(int number)
            {
                if (From.Number == number)
                    return To.Number;

                if (To.Number == number)
                    return From.Number;

                throw new ArgumentException("Building is not on this connection.");
            }
        }

        private sealed class PipeData
        {
            public int From { get; set; }
            public int To { get; set; }
        }

        private sealed class PipeSnapshot
        {
            public List<PipeData> Connections { get; set; } = new();
        }

        public IReadOnlyList<Connection> Connections { get; }

        private readonly Dictionary<int, List<Connection>> neighbours;

        private BuildingPipeGraph(
            List<BuildingMapData.Building> buildings,
            List<Connection> connections)
        {
            Connections = connections;

            neighbours = buildings.ToDictionary(
                building => building.Number,
                _ => new List<Connection>());

            foreach (var pipe in connections)
            {
                neighbours[pipe.From.Number].Add(pipe);
                neighbours[pipe.To.Number].Add(pipe);
            }
        }

        public static BuildingPipeGraph Create(
            List<BuildingMapData.Building> buildings,
            BlagoevgradNeighbourhoods.Zone zone)
        {
            if (zone.Name != "Zapad")
                throw new InvalidOperationException(
                    "The pipe graph is only available for Zapad.");

            using var stream =
                typeof(BuildingPipeGraph).Assembly.GetManifestResourceStream(
                    "WaterNetworkMonitoring.Data.Maps.CleanWaterPipes.json")
                ?? throw new InvalidOperationException(
                    "Clean water pipe data is missing.");

            var data = JsonSerializer.Deserialize<PipeSnapshot>(stream)
                ?? throw new InvalidOperationException(
                    "Clean water pipe data is invalid.");

            var buildingsByNumber =
                buildings.ToDictionary(building => building.Number);

            var connections = data.Connections.Select(pipe =>
                new Connection(
                    buildingsByNumber[pipe.From],
                    buildingsByNumber[pipe.To]))
                .ToList();

            var graph = new BuildingPipeGraph(buildings, connections);

            if (graph.ReachableFrom(buildings[0].Number).Count != buildings.Count)
                throw new InvalidOperationException(
                    "A building is disconnected from the graph.");

            return graph;
        }

        public PointLatLng PositionOnPipe(PointLatLng desired, int? servedBuilding = null)
        {
            var candidates = servedBuilding.HasValue
                ? Connections.Where(p => p.Touches(servedBuilding)).ToList()
                : Connections.ToList();

            if (candidates.Count == 0)
                return desired;

            double scale = Math.Cos(desired.Lat * Math.PI / 180);

            var nearest = candidates.Select(pipe =>
            {
                double dx = (pipe.To.Longitude - pipe.From.Longitude) * scale;
                double dy = pipe.To.Latitude - pipe.From.Latitude;

                double t =
                    ((desired.Lng - pipe.From.Longitude) * scale * dx +
                     (desired.Lat - pipe.From.Latitude) * dy)
                    / (dx * dx + dy * dy);

                t = Math.Clamp(t, .15, .85);

                var point = new PointLatLng(
                    pipe.From.Latitude + t * dy,
                    pipe.From.Longitude +
                    t * (pipe.To.Longitude - pipe.From.Longitude));

                double distance =
                    Math.Pow((point.Lng - desired.Lng) * scale, 2) +
                    Math.Pow(point.Lat - desired.Lat, 2);

                return (Point: point, Distance: distance);

            }).OrderBy(p => p.Distance).First();

            return nearest.Point;
        }

        public GMapOverlay CreateOverlay(string id)
        {
            var overlay = new GMapOverlay(id);

            foreach (var pipe in Connections)
            {
                var route = new GMapRoute(
                    new List<PointLatLng>
                    {
                        pipe.From.Position,
                        pipe.To.Position
                    },
                    pipe.Id);

                route.Stroke =
                    new Pen(Color.FromArgb(37, 99, 180), 1.5f);

                route.IsHitTestVisible = true;

                overlay.Routes.Add(route);
            }

            return overlay;
        }

        public void StyleOverlay(
            GMapOverlay overlay,
            bool visible,
            int? flaggedBuilding,
            int? selectedBuilding = null)
        {
            foreach (var route in overlay.Routes)
            {
                var pipe =
                    Connections.First(p => p.Id == route.Name);

                bool flagged = pipe.Touches(flaggedBuilding);
                bool selected = pipe.Touches(selectedBuilding);

                route.Stroke.Dispose();

                route.Stroke = new Pen(
                    flagged
                        ? Color.Firebrick
                        : Color.FromArgb(37, 99, 180),
                    flagged || selected ? 3f : 1.5f);

                route.IsVisible = visible;
            }
        }

        // BFS
        public HashSet<int> ReachableFrom(int buildingNumber)
        {
            var reached = new HashSet<int>();
            var queue = new Queue<int>();

            reached.Add(buildingNumber);
            queue.Enqueue(buildingNumber);

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                foreach (var pipe in neighbours[current])
                {
                    int next = pipe.Other(current);

                    if (reached.Add(next))
                        queue.Enqueue(next);
                }
            }

            return reached;
        }
    }
}