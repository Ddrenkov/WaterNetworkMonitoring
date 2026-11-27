using GMap.NET;
using System.Text.Json;

namespace WaterNetworkMonitoring
{
    internal sealed class WasteWaterNetwork
    {
        internal sealed record Node(long Id, double Latitude, double Longitude, int? BuildingNumber = null)
        {
            public PointLatLng Position => new(Latitude, Longitude);
        }
        internal sealed record Pipe(long From, long To, string Street, double LengthMetres, int? BuildingNumber = null);

        private sealed class Snapshot
        {
            public List<Node> Nodes { get; set; } = new();
            public List<Pipe> Pipes { get; set; } = new();
        }

        public IReadOnlyDictionary<long, Node> Nodes { get; }
        public IReadOnlyList<Pipe> Pipes { get; }
        public int ComponentCount { get; }

        private WasteWaterNetwork(Snapshot data)
        {
            Nodes = data.Nodes.ToDictionary(node => node.Id);
            Pipes = data.Pipes;
            ComponentCount = CountComponents();
        }

        public static WasteWaterNetwork Load()
        {
            using var stream = typeof(WasteWaterNetwork).Assembly.GetManifestResourceStream(
                "WaterNetworkMonitoring.Data.Maps.WasteWaterNetwork.json")
                ?? throw new InvalidOperationException("Wastewater network data is missing.");
            var data = JsonSerializer.Deserialize<Snapshot>(stream)
                ?? throw new InvalidOperationException("Wastewater network data is invalid.");
            if (data.Nodes.Count == 0)
                throw new InvalidOperationException("The wastewater network has no nodes.");
            return new WasteWaterNetwork(data);
        }

        private int CountComponents()
        {
            var neighbours = Nodes.Keys.ToDictionary(id => id, _ => new List<long>());
            foreach (var pipe in Pipes)
            {
                if (!Nodes.ContainsKey(pipe.From) || !Nodes.ContainsKey(pipe.To))
                    throw new InvalidOperationException("A wastewater pipe refers to an unknown node.");
                neighbours[pipe.From].Add(pipe.To);
                neighbours[pipe.To].Add(pipe.From);
            }

            var visited = new HashSet<long>();
            var queue = new Queue<long>();
            int components = 0;
            foreach (long start in Nodes.Keys)
            {
                if (!visited.Add(start)) continue;
                components++;
                queue.Enqueue(start);
                while (queue.Count > 0)
                {
                    long current = queue.Dequeue();
                    foreach (long next in neighbours[current])
                        if (visited.Add(next)) queue.Enqueue(next);
                }
            }
            return components;
        }
    }
}
