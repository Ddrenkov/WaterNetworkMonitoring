using System.Globalization;
using Microsoft.EntityFrameworkCore.Migrations;
using WaterNetworkMonitoring.Migrations.Historical;
using WaterNetworkMonitoring.Models;
using LoggerReading = WaterNetworkMonitoring.Migrations.Historical.LoggerReading;
using WaterMeterReading = WaterNetworkMonitoring.Migrations.Historical.WaterMeterReading;

#nullable disable

namespace WaterNetworkMonitoring.Migrations
{
    public partial class AddZapadSeptemberAndNovember2026Readings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sample = SepNovSeed.Generate();
            string ids = string.Join(",", ZapadOctober2026Seed.Profiles.Select(p => p.Id));
            migrationBuilder.Sql($"""
                IF EXISTS (SELECT 1 FROM LoggerReadings WHERE LoggerId BETWEEN 200001 AND 200004
                    AND IsSimulated=0 AND ((TakenAtUtc>'2026-08-31T21:00:00' AND TakenAtUtc<='2026-09-30T21:00:00')
                        OR (TakenAtUtc>'2026-10-31T16:00:00' AND TakenAtUtc<='2026-11-30T22:00:00')))
                    OR EXISTS (SELECT 1 FROM WaterMeterReadings WHERE WaterMeterId IN ({ids}) AND IsSimulated=0
                        AND ((TakenAtUtc>='2026-08-30T21:00:00' AND TakenAtUtc<'2026-09-30T21:00:00')
                            OR (TakenAtUtc>='2026-10-31T22:00:00' AND TakenAtUtc<='2026-12-01T04:00:00')))
                    THROW 51002, 'The September/November sample must not overwrite real readings.', 1;
                """);
            WriteMeters(migrationBuilder, sample.MeterReadings);
            WriteLoggers(migrationBuilder, sample.LoggerReadings);
            UpdateCurrentCounters(migrationBuilder, sample, undo: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sample = SepNovSeed.Generate();
            var original = ZapadOctober2026Seed.Generate();
            var oldLoggers = ZapadFourLoggerSeed.GenerateReadings();
            var oldLoggerKeys = oldLoggers.Select(r => (r.LoggerId, r.TakenAt)).ToHashSet();
            var oldMeterKeys = original.MeterReadings.Select(r => (r.WaterMeterId, r.TakenAt)).ToHashSet();
            UpdateCurrentCounters(migrationBuilder, sample, undo: true);
            foreach (var batch in sample.LoggerReadings.Where(r => !oldLoggerKeys.Contains((r.LoggerId, r.TakenAt))).Chunk(400))
                migrationBuilder.Sql($"""
                    DELETE target FROM LoggerReadings AS target
                    JOIN (VALUES {LoggerValues(batch)}) AS source(Id,Time,Flow,Pressure)
                        ON target.LoggerId=source.Id AND target.TakenAtUtc=source.Time
                    WHERE target.IsSimulated=1 AND target.FlowM3PerHour=source.Flow AND target.PressureBar=source.Pressure;
                    """);
            foreach (var batch in sample.MeterReadings.Where(r => !oldMeterKeys.Contains((r.WaterMeterId, r.TakenAt))).Chunk(400))
                migrationBuilder.Sql($"""
                    DELETE target FROM WaterMeterReadings AS target
                    JOIN (VALUES {MeterValues(batch)}) AS source(Id,Time,Reading)
                        ON target.WaterMeterId=source.Id AND target.TakenAtUtc=source.Time
                    WHERE target.IsSimulated=1 AND target.Reading=source.Reading;
                    """);
            var newLoggerKeys = sample.LoggerReadings.Select(r => (r.LoggerId, r.TakenAt)).ToHashSet();
            WriteLoggers(migrationBuilder, oldLoggers.Where(r => newLoggerKeys.Contains((r.LoggerId, r.TakenAt))));
            // Existing meter observations have identical counters; only newly inserted observations are removed.
        }

        private static void WriteLoggers(MigrationBuilder migrationBuilder, IEnumerable<LoggerReading> readings)
        {
            foreach (var batch in readings.Chunk(400))
                migrationBuilder.Sql($"""
                    MERGE LoggerReadings AS target
                    USING (VALUES {LoggerValues(batch)}) AS source(Id,Time,Flow,Pressure)
                    ON target.LoggerId=source.Id AND target.TakenAtUtc=source.Time
                    WHEN MATCHED AND target.IsSimulated=1 THEN UPDATE SET FlowM3PerHour=source.Flow,PressureBar=source.Pressure
                    WHEN NOT MATCHED THEN INSERT (LoggerId,TakenAtUtc,FlowM3PerHour,PressureBar,IsSimulated)
                        VALUES(source.Id,source.Time,source.Flow,source.Pressure,1);
                    """);
        }

        private static void WriteMeters(MigrationBuilder migrationBuilder, IEnumerable<WaterMeterReading> readings)
        {
            foreach (var batch in readings.Chunk(400))
                migrationBuilder.Sql($"""
                    MERGE WaterMeterReadings AS target
                    USING (VALUES {MeterValues(batch)}) AS source(Id,Time,Reading)
                    ON target.WaterMeterId=source.Id AND target.TakenAtUtc=source.Time
                    WHEN MATCHED AND target.IsSimulated=1 THEN UPDATE SET Reading=source.Reading
                    WHEN NOT MATCHED THEN INSERT (WaterMeterId,TakenAtUtc,Reading,IsSimulated)
                        VALUES(source.Id,source.Time,source.Reading,1);
                    """);
        }

        private static void UpdateCurrentCounters(MigrationBuilder migrationBuilder, SepNovSeed.SampleData sample, bool undo)
        {
            var original = ZapadOctober2026Seed.Generate().MeterReadings
                .Where(r => r.TakenAt == ZapadOctober2026Seed.SnapshotAt).ToDictionary(r => r.WaterMeterId);
            var latest = sample.MeterReadings.GroupBy(r => r.WaterMeterId).Select(g => g.MaxBy(r => r.TakenAt));
            string values = string.Join(",\n", latest.Select(r =>
            {
                var from = undo ? r : original[r.WaterMeterId];
                var to = undo ? original[r.WaterMeterId] : r;
                return $"({r.WaterMeterId},'{from.Reading}',{Time(from.TakenAt)},'{to.Reading}',{Time(to.TakenAt)})";
            }));
            migrationBuilder.Sql($"""
                UPDATE meter SET Reading=source.NewReading,TakenAtUtc=source.NewTime
                FROM WaterMeters AS meter
                JOIN (VALUES {values}) AS source(Id,OldReading,OldTime,NewReading,NewTime) ON meter.Id=source.Id
                WHERE meter.Reading=source.OldReading AND meter.TakenAtUtc=source.OldTime;
                """);
        }

        private static string LoggerValues(IEnumerable<LoggerReading> rows) => string.Join(",\n", rows.Select(r =>
            $"({r.LoggerId},{Time(r.TakenAt)},{Number(r.FlowM3PerHour)},{Number(r.PressureBar)})"));
        private static string MeterValues(IEnumerable<WaterMeterReading> rows) => string.Join(",\n", rows.Select(r =>
            $"({r.WaterMeterId},{Time(r.TakenAt)},'{r.Reading}')"));
        private static string Time(DateTimeOffset time) => $"CONVERT(datetime2(2),'{time.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.ff", CultureInfo.InvariantCulture)}',126)";
        private static string Number(decimal value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
