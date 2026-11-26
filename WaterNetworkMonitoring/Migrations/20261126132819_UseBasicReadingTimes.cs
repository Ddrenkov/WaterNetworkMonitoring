using System.Globalization;
using Microsoft.EntityFrameworkCore.Migrations;
using WaterNetworkMonitoring.Data;
using Old = WaterNetworkMonitoring.Migrations.Historical;

#nullable disable

namespace WaterNetworkMonitoring.Migrations
{
    public partial class UseBasicReadingTimes : Migration
    {
        private static readonly (string Table, string Device)[] ReadingTables =
        {
            ("WaterMeters", null), ("LoggerReadings", "LoggerId"),
            ("WaterMeterReadings", "WaterMeterId"), ("WasteWaterReadings", "LoggerId")
        };

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Copy the displayed local clock before removing the old UTC/computed columns.
            foreach (var (table, _) in ReadingTables)
            {
                migrationBuilder.AddColumn<DateTime>("BasicTakenAt", table, type: "datetime2(0)", nullable: true);
                migrationBuilder.Sql($"UPDATE [{table}] SET [BasicTakenAt] = CONVERT(datetime2(0), [TakenAt]);");
            }
            RemoveExampleHistory(migrationBuilder, "BasicTakenAt");
            foreach (var (table, device) in ReadingTables)
            {
                if (device != null)
                {
                    // Real observations with the same local second need manual resolution, not deletion.
                    migrationBuilder.Sql($"""
                        IF EXISTS (SELECT 1 FROM [{table}] GROUP BY [{device}], [BasicTakenAt] HAVING COUNT(*) > 1)
                            THROW 51010, 'Two existing readings share the same local second. No changes were saved.', 1;
                        """);
                    migrationBuilder.DropIndex($"IX_{table}_{device}_TakenAtUtc", table);
                }
                migrationBuilder.DropColumn("TakenAt", table);
                migrationBuilder.DropColumn("TakenAtUtc", table);
                migrationBuilder.RenameColumn("BasicTakenAt", table, "TakenAt");
                migrationBuilder.AlterColumn<DateTime>("TakenAt", table, type: "datetime2(0)", nullable: false,
                    oldClrType: typeof(DateTime), oldType: "datetime2(0)", oldNullable: true);
                if (device != null)
                    migrationBuilder.CreateIndex($"IX_{table}_{device}_TakenAt", table,
                        new[] { device, "TakenAt" }, unique: true);
            }
            WriteBasicHistory(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            RemoveExampleHistory(migrationBuilder, "TakenAt");
            foreach (var (table, device) in ReadingTables)
            {
                if (device != null) migrationBuilder.DropIndex($"IX_{table}_{device}_TakenAt", table);
                migrationBuilder.AddColumn<DateTime>("TakenAtUtc", table, type: "datetime2(2)", nullable: true);
                migrationBuilder.Sql($"""
                    UPDATE [{table}] SET [TakenAtUtc] = CONVERT(datetime2(2),
                        [TakenAt] AT TIME ZONE 'FLE Standard Time' AT TIME ZONE 'UTC');
                    """);
                migrationBuilder.DropColumn("TakenAt", table);
                migrationBuilder.AlterColumn<DateTime>("TakenAtUtc", table, type: "datetime2(2)", nullable: false,
                    oldClrType: typeof(DateTime), oldType: "datetime2(2)", oldNullable: true);
                migrationBuilder.AddColumn<DateTime>("TakenAt", table, type: "datetime2(2)", nullable: false,
                    computedColumnSql: "CONVERT(datetime2(2), [TakenAtUtc] AT TIME ZONE 'UTC' AT TIME ZONE 'FLE Standard Time')",
                    stored: false);
                if (device != null)
                    migrationBuilder.CreateIndex($"IX_{table}_{device}_TakenAtUtc", table,
                        new[] { device, "TakenAtUtc" }, unique: true);
            }
            WriteHistoricalReadings(migrationBuilder);
        }

        private static void RemoveExampleHistory(MigrationBuilder builder, string timeColumn)
        {
            string meterIds = string.Join(",", ZapadOctober2026Seed.Profiles.Select(p => p.Id));
            foreach (var (table, device, ids) in new[]
            {
                ("LoggerReadings", "LoggerId", "200001,200002,200003,200004"),
                ("WaterMeterReadings", "WaterMeterId", meterIds),
                ("WasteWaterReadings", "LoggerId", "300001,300002,300003")
            })
                builder.Sql($"""
                    DELETE FROM [{table}] WHERE [IsSimulated] = 1 AND [{device}] IN ({ids})
                        AND [{timeColumn}] >= '2026-08-31T00:00:00'
                        AND [{timeColumn}] <= '2026-12-01T06:00:00';
                    """);
        }

        private static void WriteBasicHistory(MigrationBuilder builder)
        {
            var october = ZapadOctober2026Seed.Generate();
            var other = SepNovSeed.Generate();
            var loggers = ZapadFourLoggerSeed.GenerateReadings().Concat(other.LoggerReadings)
                .GroupBy(r => (r.LoggerId, r.TakenAt)).Select(g => g.Last());
            var meters = october.MeterReadings.Concat(other.MeterReadings)
                .GroupBy(r => (r.WaterMeterId, r.TakenAt)).Select(g => g.Last());
            var waste = WasteWaterSepSeed.GenerateTwoHourReadings()
                .Concat(WasteWaterOctSeed.GenerateReadings()).Concat(WasteWaterNovSeed.GenerateReadings());
            WriteRows(builder, "LoggerReadings", "LoggerId", "TakenAt",
                new[] { "FlowM3PerHour", "PressureBar", "IsSimulated" }, loggers.Select(r =>
                    new object[] { r.LoggerId, r.TakenAt, r.FlowM3PerHour, r.PressureBar, true }));
            WriteRows(builder, "WaterMeterReadings", "WaterMeterId", "TakenAt",
                new[] { "Reading", "IsSimulated" }, meters.Select(r =>
                    new object[] { r.WaterMeterId, r.TakenAt, r.Reading, true }));
            WriteRows(builder, "WasteWaterReadings", "LoggerId", "TakenAt",
                new[] { "FlowM3PerHour", "WaterLevelMetres", "Rain", "IsSimulated" }, waste.Select(r =>
                    new object[] { r.LoggerId, r.TakenAt, r.FlowM3PerHour, r.WaterLevelMetres, r.IsRaining ? "Yes" : "No", true }));
        }

        private static void WriteHistoricalReadings(MigrationBuilder builder)
        {
            var october = Old.ZapadOctober2026Seed.Generate();
            var other = Old.SepNovSeed.Generate();
            var loggers = Old.ZapadFourLoggerSeed.GenerateReadings().Concat(other.LoggerReadings)
                .GroupBy(r => (r.LoggerId, r.TakenAt)).Select(g => g.Last());
            var meters = october.MeterReadings.Concat(other.MeterReadings)
                .GroupBy(r => (r.WaterMeterId, r.TakenAt)).Select(g => g.Last());
            var waste = Old.WasteWaterSepSeed.GenerateTwoHourReadings()
                .Concat(Old.WasteWaterOctSeed.GenerateReadings()).Concat(Old.WasteWaterNovSeed.GenerateReadings());
            WriteRows(builder, "LoggerReadings", "LoggerId", "TakenAtUtc",
                new[] { "FlowM3PerHour", "PressureBar", "IsSimulated" }, loggers.Select(r =>
                    new object[] { r.LoggerId, r.TakenAt.UtcDateTime, r.FlowM3PerHour, r.PressureBar, true }));
            WriteRows(builder, "WaterMeterReadings", "WaterMeterId", "TakenAtUtc",
                new[] { "Reading", "IsSimulated" }, meters.Select(r =>
                    new object[] { r.WaterMeterId, r.TakenAt.UtcDateTime, r.Reading, true }));
            WriteRows(builder, "WasteWaterReadings", "LoggerId", "TakenAtUtc",
                new[] { "FlowM3PerHour", "WaterLevelMetres", "Rain", "IsSimulated" }, waste.Select(r =>
                    new object[] { r.LoggerId, r.TakenAt.UtcDateTime, r.FlowM3PerHour, r.WaterLevelMetres, r.IsRaining ? "Yes" : "No", true }));
        }

        private static void WriteRows(MigrationBuilder builder, string table, string device, string time,
            string[] measurements, IEnumerable<object[]> rows)
        {
            var columns = new[] { device, time }.Concat(measurements).ToArray();
            string names = string.Join(",", columns.Select(c => $"[{c}]"));
            foreach (var batch in rows.Chunk(400))
            {
                string values = string.Join(",\n", batch.Select(row => $"({string.Join(",", row.Select(SqlValue))})"));
                builder.Sql($"""
                    INSERT INTO [{table}] ({names})
                    SELECT {string.Join(",", columns.Select(c => $"source.[{c}]"))}
                    FROM (VALUES {values}) AS source({names})
                    WHERE NOT EXISTS (SELECT 1 FROM [{table}] AS target
                        WHERE target.[{device}] = source.[{device}] AND target.[{time}] = source.[{time}]);
                    """);
            }
        }

        private static string SqlValue(object value) => value switch
        {
            DateTime time => $"CONVERT(datetime2,{"'" + time.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) + "'"},126)",
            string text => "'" + text.Replace("'", "''") + "'",
            bool flag => flag ? "1" : "0",
            _ => Convert.ToString(value, CultureInfo.InvariantCulture)
        };
    }
}
