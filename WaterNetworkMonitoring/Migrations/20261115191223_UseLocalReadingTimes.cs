using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterNetworkMonitoring.Migrations
{
    public partial class UseLocalReadingTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (string table in new[] { "LoggerReadings", "WaterMeterReadings", "WaterMeters" })
            {
                string device = table == "LoggerReadings" ? "LoggerId" : "WaterMeterId";
                if (table != "WaterMeters")
                    migrationBuilder.DropIndex($"IX_{table}_{device}_TakenAt", table);

                migrationBuilder.RenameColumn("TakenAt", table, "TakenAtUtc");
                // Preserve each instant before removing the offset from the column type.
                migrationBuilder.Sql($"UPDATE [{table}] SET [TakenAtUtc] = SWITCHOFFSET([TakenAtUtc], '+00:00');");
                migrationBuilder.AlterColumn<DateTime>("TakenAtUtc", table, type: "datetime2(2)", nullable: false,
                    oldClrType: typeof(DateTimeOffset), oldType: "datetimeoffset");
                migrationBuilder.AddColumn<DateTime>("TakenAt", table, type: "datetime2(2)", nullable: false,
                    computedColumnSql: "CONVERT(datetime2(2), [TakenAtUtc] AT TIME ZONE 'UTC' AT TIME ZONE 'FLE Standard Time')",
                    stored: false);

                if (table != "WaterMeters")
                    migrationBuilder.CreateIndex($"IX_{table}_{device}_TakenAtUtc", table,
                        new[] { device, "TakenAtUtc" }, unique: true);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (string table in new[] { "LoggerReadings", "WaterMeterReadings", "WaterMeters" })
            {
                string device = table == "LoggerReadings" ? "LoggerId" : "WaterMeterId";
                if (table != "WaterMeters")
                    migrationBuilder.DropIndex($"IX_{table}_{device}_TakenAtUtc", table);

                // Only the derived display column is removed; the original instants remain.
                migrationBuilder.DropColumn("TakenAt", table);
                migrationBuilder.AlterColumn<DateTimeOffset>("TakenAtUtc", table, type: "datetimeoffset", nullable: false,
                    oldClrType: typeof(DateTime), oldType: "datetime2(2)");
                migrationBuilder.Sql($"UPDATE [{table}] SET [TakenAtUtc] = [TakenAtUtc] AT TIME ZONE 'FLE Standard Time';");
                migrationBuilder.RenameColumn("TakenAtUtc", table, "TakenAt");

                if (table != "WaterMeters")
                    migrationBuilder.CreateIndex($"IX_{table}_{device}_TakenAt", table,
                        new[] { device, "TakenAt" }, unique: true);
            }
        }
    }
}
