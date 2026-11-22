using Microsoft.EntityFrameworkCore.Migrations;
using WaterNetworkMonitoring.Data;
using WaterNetworkMonitoring.Models;

#nullable disable

namespace WaterNetworkMonitoring.Migrations
{
    public partial class UseTwoHourWastewaterReadingsAndRain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rain", table: "WasteWaterReadings", type: "varchar(3)",
                unicode: false, maxLength: 3, nullable: false, defaultValue: "No");

            migrationBuilder.AddCheckConstraint(
                name: "CK_WasteWaterReadings_Rain", table: "WasteWaterReadings",
                sql: "[Rain] IN ('Yes', 'No')");

            ReplaceSeptemberReadings(migrationBuilder, WasteWaterSepSeed.GenerateTwoHourReadings());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ReplaceSeptemberReadings(migrationBuilder, WasteWaterSepSeed.GenerateReadings());
            migrationBuilder.DropCheckConstraint("CK_WasteWaterReadings_Rain", "WasteWaterReadings");
            migrationBuilder.DropColumn("Rain", "WasteWaterReadings");
        }

        private static void ReplaceSeptemberReadings(MigrationBuilder migrationBuilder,
            IEnumerable<WasteWaterReading> readings)
        {
            // These UTC endpoints cover September's local two-hour intervals.
            // Only the three wastewater loggers' example data may be replaced.
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1 FROM [WasteWaterReadings]
                    WHERE [LoggerId] IN (300001, 300002, 300003)
                      AND [TakenAtUtc] > '2026-08-31T21:00:00'
                      AND [TakenAtUtc] <= '2026-09-30T21:00:00'
                      AND [IsSimulated] = 0
                )
                    THROW 51000, 'Real wastewater readings exist in September; the example seed cannot replace them.', 1;

                DELETE FROM [WasteWaterReadings]
                WHERE [LoggerId] IN (300001, 300002, 300003)
                  AND [TakenAtUtc] > '2026-08-31T21:00:00'
                  AND [TakenAtUtc] <= '2026-09-30T21:00:00'
                  AND [IsSimulated] = 1;
                """);

            foreach (var batch in readings.Chunk(400))
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
    }
}
