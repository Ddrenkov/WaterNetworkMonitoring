using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Globalization;
using WaterNetworkMonitoring.Data;

#nullable disable

namespace WaterNetworkMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class AddZapadOctober2026Readings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            RepairZapadMeterNumbers(migrationBuilder);
            migrationBuilder.CreateTable(
                name: "Loggers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZoneName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ParentLoggerId = table.Column<int>(type: "int", nullable: true),
                    SuppliedWaterMeterId = table.Column<int>(type: "int", nullable: true),
                    IsSimulated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loggers", x => x.Id);
                    table.CheckConstraint("CK_Loggers_SixDigitId", "[Id] BETWEEN 100000 AND 999999");
                    table.ForeignKey(
                        name: "FK_Loggers_Loggers_ParentLoggerId",
                        column: x => x.ParentLoggerId,
                        principalTable: "Loggers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loggers_WaterMeters_SuppliedWaterMeterId",
                        column: x => x.SuppliedWaterMeterId,
                        principalTable: "WaterMeters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WaterMeterReadings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WaterMeterId = table.Column<int>(type: "int", nullable: false),
                    Reading = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    TakenAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsSimulated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterMeterReadings", x => x.Id);
                    table.CheckConstraint("CK_WaterMeterReadings_Reading", "LEN([Reading]) = 8 AND [Reading] COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9]%'");
                    table.ForeignKey(
                        name: "FK_WaterMeterReadings_WaterMeters_WaterMeterId",
                        column: x => x.WaterMeterId,
                        principalTable: "WaterMeters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoggerReadings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoggerId = table.Column<int>(type: "int", nullable: false),
                    TakenAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    FlowM3PerHour = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    PressureBar = table.Column<decimal>(type: "decimal(6,3)", precision: 6, scale: 3, nullable: false),
                    IsSimulated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoggerReadings", x => x.Id);
                    table.CheckConstraint("CK_LoggerReadings_Flow", "[FlowM3PerHour] >= 0");
                    table.CheckConstraint("CK_LoggerReadings_Pressure", "[PressureBar] >= 0");
                    table.ForeignKey(
                        name: "FK_LoggerReadings_Loggers_LoggerId",
                        column: x => x.LoggerId,
                        principalTable: "Loggers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoggerReadings_LoggerId_TakenAt",
                table: "LoggerReadings",
                columns: new[] { "LoggerId", "TakenAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loggers_ParentLoggerId",
                table: "Loggers",
                column: "ParentLoggerId");

            migrationBuilder.CreateIndex(
                name: "IX_Loggers_SuppliedWaterMeterId",
                table: "Loggers",
                column: "SuppliedWaterMeterId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterMeterReadings_WaterMeterId_TakenAt",
                table: "WaterMeterReadings",
                columns: new[] { "WaterMeterId", "TakenAt" },
                unique: true);

            var sample = ZapadOctober2026Seed.Generate();
            InsertRows(migrationBuilder, "Loggers",
                new[] { "Id", "Name", "ZoneName", "ParentLoggerId", "SuppliedWaterMeterId", "IsSimulated" },
                new[] { "int", "nvarchar(100)", "nvarchar(50)", "int", "int", "bit" },
                sample.Loggers.Select(l => new object[]
                    { l.Id, l.Name, l.ZoneName, l.ParentLoggerId, l.SuppliedWaterMeterId, l.IsSimulated }));
            InsertRows(migrationBuilder, "LoggerReadings",
                new[] { "Id", "LoggerId", "TakenAt", "FlowM3PerHour", "PressureBar", "IsSimulated" },
                new[] { "bigint", "int", "datetimeoffset", "decimal(12,3)", "decimal(6,3)", "bit" },
                sample.LoggerReadings.Select(r => new object[]
                    { r.Id, r.LoggerId, r.TakenAt, r.FlowM3PerHour, r.PressureBar, r.IsSimulated }));
            InsertRows(migrationBuilder, "WaterMeterReadings",
                new[] { "Id", "WaterMeterId", "Reading", "TakenAt", "IsSimulated" },
                new[] { "bigint", "int", "char(8)", "datetimeoffset", "bit" },
                sample.MeterReadings.Select(r => new object[]
                    { r.Id, r.WaterMeterId, r.Reading, r.TakenAt, r.IsSimulated }));

            // Replace only the original zero-valued 8 November placeholders; keep any user-entered readings.
            // The history includes this November endpoint so the current counter stays above October's counters.
            var latest = sample.MeterReadings.Where(r => r.TakenAt == ZapadOctober2026Seed.SnapshotAt);
            var values = string.Join(",", latest.Select(r => $"({r.WaterMeterId},'{r.Reading}')"));
            var snapshotAt = ZapadOctober2026Seed.SnapshotAt.ToString("O", CultureInfo.InvariantCulture);
            migrationBuilder.Sql($"""
                UPDATE meter SET Reading = latest.Reading
                FROM WaterMeters AS meter
                JOIN (VALUES {values}) AS latest(Id, Reading) ON meter.Id = latest.Id
                WHERE meter.Reading = '00000000' AND meter.TakenAt = '{snapshotAt}';
                """);
        }

        private static void RepairZapadMeterNumbers(MigrationBuilder migrationBuilder)
        {
            // Frozen mappings from the earlier city-wide and 68-building Zapad seeds.
            // Match by device ID, never by a map number that has since changed.
            var originalNumbers = new[] { 49, 50, 51, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 605, 606, 607, 608, 725, 1034, 2131, 2254, 2530, 2531, 2801, 3200, 3201, 3202, 3374, 3375 };
            var mapping = string.Join(",", ZapadOctober2026Seed.Profiles.Select((p, i) =>
                $"({p.Id},{p.MapNumber},{originalNumbers[i]},{p.Id - 100000})"));
            migrationBuilder.Sql($"""
                CREATE TABLE [#ExpectedZapadMeters] (
                    Id int PRIMARY KEY, MapNumber int NOT NULL,
                    OriginalMapNumber int NOT NULL, PreviousMapNumber int NOT NULL
                );
                INSERT INTO [#ExpectedZapadMeters] (Id, MapNumber, OriginalMapNumber, PreviousMapNumber)
                VALUES {mapping};

                IF EXISTS (
                    SELECT 1 FROM WaterMeters AS actual
                    JOIN [#ExpectedZapadMeters] AS expected ON actual.Id = expected.Id
                    WHERE actual.MapNumber NOT IN (expected.MapNumber, expected.OriginalMapNumber, expected.PreviousMapNumber)
                )
                    THROW 51000, 'A Zapad meter has an unrecognized map number. Its existing data was preserved. Check its ID and map number before retrying.', 1;

                IF EXISTS (
                    SELECT 1 FROM WaterMeters
                    WHERE Id = 100052 AND (
                        MapNumber NOT IN (52, 500) OR Reading <> '00000000'
                        OR TakenAt <> '2026-11-08T22:00:00.0000000+02:00'
                    )
                )
                    THROW 51000, 'The excluded building 52 has non-prototype data. Preserve or reassign that data before removing meter 100052.', 1;

                IF EXISTS (
                    SELECT 1 FROM WaterMeters AS actual
                    WHERE actual.MapNumber BETWEEN 1 AND 67 AND actual.Id <> 100052
                    AND NOT EXISTS (SELECT 1 FROM [#ExpectedZapadMeters] AS expected WHERE expected.Id = actual.Id)
                )
                    THROW 51000, 'Another meter already uses a target Zapad map number. Existing data was preserved. Resolve that conflict before retrying.', 1;

                -- The surrounding EF transaction rolls back the entire repair if any step fails.
                -- Temporarily remove uniqueness so swaps and shifted numbers cannot collide.
                DROP INDEX [IX_WaterMeters_MapNumber] ON [WaterMeters];
                DELETE FROM WaterMeters WHERE Id = 100052;

                UPDATE WaterMeters SET MapNumber = (
                    SELECT expected.MapNumber FROM [#ExpectedZapadMeters] AS expected
                    WHERE expected.Id = WaterMeters.Id
                ) WHERE Id IN (SELECT Id FROM [#ExpectedZapadMeters]);

                -- Restore any missing prototype devices using the same fixed six-digit IDs.
                SET IDENTITY_INSERT [WaterMeters] ON;
                INSERT INTO WaterMeters (Id, MapNumber, Reading, TakenAt)
                SELECT expected.Id, expected.MapNumber, '00000000', '2026-11-08T22:00:00.0000000+02:00'
                FROM [#ExpectedZapadMeters] AS expected
                WHERE NOT EXISTS (SELECT 1 FROM WaterMeters AS actual WHERE actual.Id = expected.Id);
                SET IDENTITY_INSERT [WaterMeters] OFF;

                CREATE UNIQUE INDEX [IX_WaterMeters_MapNumber] ON [WaterMeters] ([MapNumber]);
                DROP TABLE [#ExpectedZapadMeters];
                """);
        }

        private static void InsertRows(MigrationBuilder migrationBuilder, string table,
            string[] columns, string[] columnTypes, IEnumerable<object[]> rows)
        {
            // Bounded batches keep generated SQL manageable. EF migration history prevents reimporting.
            foreach (var batch in rows.Chunk(200))
            {
                var values = new object[batch.Length, columns.Length];
                for (int row = 0; row < batch.Length; row++)
                    for (int column = 0; column < columns.Length; column++)
                        values[row, column] = batch[row][column];
                migrationBuilder.InsertData(table, columns, columnTypes, values);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoggerReadings");

            migrationBuilder.DropTable(
                name: "WaterMeterReadings");

            migrationBuilder.DropTable(
                name: "Loggers");
        }
    }
}
