using System;
using Microsoft.EntityFrameworkCore.Migrations;
using WaterNetworkMonitoring.Migrations.Historical;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WaterNetworkMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class AddWastewaterSeptember2026Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WaterSystem",
                table: "Loggers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "CleanWater");

            migrationBuilder.CreateTable(
                name: "WasteWaterReadings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoggerId = table.Column<int>(type: "int", nullable: false),
                    TakenAtUtc = table.Column<DateTime>(type: "datetime2(2)", nullable: false),
                    FlowM3PerHour = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    WaterLevelMetres = table.Column<decimal>(type: "decimal(6,3)", precision: 6, scale: 3, nullable: false),
                    IsSimulated = table.Column<bool>(type: "bit", nullable: false),
                    TakenAt = table.Column<DateTime>(type: "datetime2(2)", nullable: false, computedColumnSql: "CONVERT(datetime2(2), [TakenAtUtc] AT TIME ZONE 'UTC' AT TIME ZONE 'FLE Standard Time')", stored: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WasteWaterReadings", x => x.Id);
                    table.CheckConstraint("CK_WasteWaterReadings_Flow", "[FlowM3PerHour] >= 0");
                    table.CheckConstraint("CK_WasteWaterReadings_Level", "[WaterLevelMetres] >= 0");
                    table.ForeignKey(
                        name: "FK_WasteWaterReadings_Loggers_LoggerId",
                        column: x => x.LoggerId,
                        principalTable: "Loggers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            foreach (var logger in WasteWaterSepSeed.GetLoggers())
                migrationBuilder.InsertData("Loggers",
                    new[] { "Id", "Name", "ZoneName", "Latitude", "Longitude", "IsSimulated", "WaterSystem" },
                    new[] { "int", "nvarchar(100)", "nvarchar(50)", "float", "float", "bit", "nvarchar(20)" },
                    new object[] { logger.Id, logger.Name, logger.ZoneName, logger.Latitude, logger.Longitude,
                        logger.IsSimulated, logger.WaterSystem.ToString() });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Loggers_WaterSystem",
                table: "Loggers",
                sql: "[WaterSystem] IN ('CleanWater', 'Wastewater')");

            migrationBuilder.CreateIndex(
                name: "IX_WasteWaterReadings_LoggerId_TakenAtUtc",
                table: "WasteWaterReadings",
                columns: new[] { "LoggerId", "TakenAtUtc" },
                unique: true);

            foreach (var batch in WasteWaterSepSeed.GenerateReadings().Chunk(400))
            {
                var values = new object[batch.Length, 5];
                for (int row = 0; row < batch.Length; row++)
                {
                    values[row, 0] = batch[row].LoggerId;
                    values[row, 1] = batch[row].TakenAt.UtcDateTime;
                    values[row, 2] = batch[row].FlowM3PerHour;
                    values[row, 3] = batch[row].WaterLevelMetres;
                    values[row, 4] = batch[row].IsSimulated;
                }
                migrationBuilder.InsertData("WasteWaterReadings",
                    new[] { "LoggerId", "TakenAtUtc", "FlowM3PerHour", "WaterLevelMetres", "IsSimulated" },
                    new[] { "int", "datetime2(2)", "decimal(12,3)", "decimal(6,3)", "bit" }, values);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WasteWaterReadings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Loggers_WaterSystem",
                table: "Loggers");

            migrationBuilder.DeleteData(
                table: "Loggers",
                keyColumn: "Id",
                keyValue: 300001);

            migrationBuilder.DeleteData(
                table: "Loggers",
                keyColumn: "Id",
                keyValue: 300002);

            migrationBuilder.DeleteData(
                table: "Loggers",
                keyColumn: "Id",
                keyValue: 300003);

            migrationBuilder.DropColumn(
                name: "WaterSystem",
                table: "Loggers");
        }
    }
}
