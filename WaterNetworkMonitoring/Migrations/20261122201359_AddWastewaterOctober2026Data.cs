using Microsoft.EntityFrameworkCore.Migrations;
using WaterNetworkMonitoring.Migrations.Historical;

#nullable disable

namespace WaterNetworkMonitoring.Migrations
{
    public partial class AddWastewaterOctober2026Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1 FROM [WasteWaterReadings]
                    WHERE [LoggerId] IN (300001, 300002, 300003)
                      AND [TakenAtUtc] > '2026-09-30T21:00:00'
                      AND [TakenAtUtc] <= '2026-10-31T22:00:00'
                )
                    THROW 51000, 'October wastewater readings already exist for the example loggers; no data was replaced.', 1;
                """);

            foreach (var batch in WasteWaterOctSeed.GenerateReadings().Chunk(400))
            {
                var values = new object[batch.Length, 6];
                for (int row = 0; row < batch.Length; row++)
                {
                    values[row, 0] = batch[row].LoggerId;
                    values[row, 1] = batch[row].TakenAt.UtcDateTime;
                    values[row, 2] = batch[row].FlowM3PerHour;
                    values[row, 3] = batch[row].WaterLevelMetres;
                    values[row, 4] = batch[row].IsSimulated;
                    values[row, 5] = batch[row].IsRaining ? "Yes" : "No";
                }
                migrationBuilder.InsertData("WasteWaterReadings",
                    new[] { "LoggerId", "TakenAtUtc", "FlowM3PerHour", "WaterLevelMetres", "IsSimulated", "Rain" },
                    new[] { "int", "datetime2(2)", "decimal(12,3)", "decimal(6,3)", "bit", "varchar(3)" }, values);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM [WasteWaterReadings]
                WHERE [LoggerId] IN (300001, 300002, 300003)
                  AND [TakenAtUtc] > '2026-09-30T21:00:00'
                  AND [TakenAtUtc] <= '2026-10-31T22:00:00'
                  AND [IsSimulated] = 1;
                """);
        }
    }
}
