using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WaterNetworkMonitoring.Data;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Services
{
    internal sealed class WasteWaterData
    {
        public List<PressureLogger> Loggers { get; init; } = new();
        public List<WasteWaterReading> Readings { get; init; } = new();
        public List<WasteWaterReading> LatestReadings { get; init; } = new();

        public static async Task<WasteWaterData> LoadAsync(CancellationToken cancellationToken)
        {
            using var db = new AppDbContext();
            var connection = new SqlConnectionStringBuilder(db.Database.GetConnectionString())
                { ConnectTimeout = 5, ConnectRetryCount = 0 };
            db.Database.SetConnectionString(connection.ConnectionString);
            db.Database.SetCommandTimeout(10);
            var loggers = await db.Loggers.AsNoTracking()
                .Where(l => l.ZoneName == "Zapad" && l.WaterSystem == WaterSystem.Wastewater)
                .OrderBy(l => l.Id).ToListAsync(cancellationToken);
            var ids = loggers.Select(l => l.Id).ToArray();
            if (ids.Length == 0) return new WasteWaterData { Loggers = loggers };
            var readings = await db.WasteWaterReadings.AsNoTracking().Where(r => ids.Contains(r.LoggerId))
                .OrderBy(r => r.TakenAt)
                .ToListAsync(cancellationToken);
            return new WasteWaterData { Loggers = loggers, Readings = readings,
                LatestReadings = readings.GroupBy(r => r.LoggerId).Select(g => g.Last()).ToList() };
        }
    }
}
