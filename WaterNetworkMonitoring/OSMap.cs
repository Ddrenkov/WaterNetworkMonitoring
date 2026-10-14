using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.Projections;
using System.Net.Http;

namespace WaterNetworkMonitoring
{
    internal sealed class OSMap : GMapProvider
    {
        public static readonly OSMap Instance = new();
        private static readonly HttpClient Client = CreateClient();
        private static readonly string CacheFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WaterNetworkMonitoring", "OsmTiles");

        private OSMap()
        {
            BypassCache = true;
            Copyright = "© OpenStreetMap contributors";
        }

        public override Guid Id { get; } = new("C6150F36-AC06-4A44-A44F-43F0C65D040F");
        public override string Name => "WaterNetworkMonitoring OpenStreetMap";
        public override PureProjection Projection => MercatorProjection.Instance;
        public override GMapProvider[] Overlays => new GMapProvider[] { this };

        private static HttpClient CreateClient()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("WaterNetworkMonitoring/1.0");
            return client;
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            string tilePath = Path.Combine(CacheFolder, zoom.ToString(), pos.X.ToString(), $"{pos.Y}.png");
            if (File.Exists(tilePath) && DateTime.UtcNow - File.GetLastWriteTimeUtc(tilePath) < TimeSpan.FromDays(7))
                return GetTileImageFromArray(File.ReadAllBytes(tilePath));

            using var response = Client.GetAsync($"https://tile.openstreetmap.org/{zoom}/{pos.X}/{pos.Y}.png").GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            byte[] bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            var image = GetTileImageFromArray(bytes);
            Directory.CreateDirectory(Path.GetDirectoryName(tilePath)!);
            File.WriteAllBytes(tilePath, bytes);
            return image;
        }
    }
}
