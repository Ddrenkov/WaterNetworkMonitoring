using System.Collections;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using WaterNetworkMonitoring.Data;
using WaterNetworkMonitoring.Models;
using WaterNetworkMonitoring.Services;

void Check(bool value, string message) { if (!value) throw new Exception(message); }
void Reject(Action action, string message)
{
    try { action(); } catch (ArgumentException) { return; }
    throw new Exception(message);
}
DateTimeOffset Local(int day, int hour, int offset = 2) => new(2026, 10, day, hour, 0, 0, TimeSpan.FromHours(offset));
using var db = new AppDbContext();
var migrations = db.GetService<IMigrationsAssembly>();
var key = migrations.Migrations.Single(m => m.Key.EndsWith("AddZapadOctober2026Readings"));
Migration CreateMigration() => migrations.CreateMigration(key.Value, db.Database.ProviderName!);
var migration = CreateMigration();
MigrationRepairChecks.Run(migration);
var inserts = migration.UpOperations.OfType<InsertDataOperation>().ToArray();
Dictionary<string, object?>[] Rows(string table) => inserts.Where(i => i.Table == table)
    .SelectMany(i => Enumerable.Range(0, i.Values.GetLength(0)).Select(row =>
        i.Columns.Select((name, col) => (name, value: i.Values[row, col]))
            .ToDictionary(v => v.name, v => v.value))).ToArray();
var loggers = Rows("Loggers").Select(r => new PressureLogger
{
    Id = (int)r["Id"]!, Name = (string)r["Name"]!, ZoneName = (string)r["ZoneName"]!,
    ParentLoggerId = (int?)r["ParentLoggerId"], SuppliedWaterMeterId = (int?)r["SuppliedWaterMeterId"],
    IsSimulated = (bool)r["IsSimulated"]!
}).ToArray();
var loggerReadings = Rows("LoggerReadings").Select(r => new LoggerReading
{
    Id = (long)r["Id"]!, LoggerId = (int)r["LoggerId"]!, TakenAt = (DateTimeOffset)r["TakenAt"]!,
    FlowM3PerHour = (decimal)r["FlowM3PerHour"]!, PressureBar = (decimal)r["PressureBar"]!,
    IsSimulated = (bool)r["IsSimulated"]!
}).ToArray();
var meters = Rows("WaterMeterReadings").Select(r => new WaterMeterReading
{
    Id = (long)r["Id"]!, WaterMeterId = (int)r["WaterMeterId"]!, Reading = (string)r["Reading"]!,
    TakenAt = (DateTimeOffset)r["TakenAt"]!, IsSimulated = (bool)r["IsSimulated"]!
}).ToArray();
Check(loggers.Length == 2 && loggers.All(l => l.IsSimulated && l.ZoneName == "Zapad"), "Logger setup");
var branch = loggers.Single(l => l.SuppliedWaterMeterId.HasValue);
Check(branch.Id == 200002 && branch.ParentLoggerId == 200001 && branch.SuppliedWaterMeterId == 100058, "Branch connectivity");
Check(meters.Length == 4355 && meters.Select(m => m.WaterMeterId).Distinct().Count() == 67, "Meter history size");
Check(meters.All(m => m.IsSimulated && m.Reading.Length == 8 && m.Reading.All(char.IsAsciiDigit)), "Counter format/provenance");
Check(meters.Select(m => (m.WaterMeterId, m.TakenAt)).Distinct().Count() == meters.Length, "Duplicate meter timestamp");
foreach (var meter in meters.GroupBy(m => m.WaterMeterId))
{
    var ordered = meter.OrderBy(r => r.TakenAt).ToArray();
    var october = ordered.Where(r => r.TakenAt.Month == 10).ToArray();
    Check(october.Length == 62 && october.GroupBy(r => r.TakenAt.Day).All(d => d.Count() == 2), "Not two readings per local day");
    Check(october.All(r => r.TakenAt.Minute == 0) &&
        (october.All(r => r.TakenAt.Hour is 2 or 14) || october.All(r => r.TakenAt.Hour is 6 or 18)), "Meter schedules");
    Check(ordered.Select(r => int.Parse(r.Reading)).SequenceEqual(ordered.Select(r => int.Parse(r.Reading)).Order()), "Counter decreased");
    Check(ordered[^1].TakenAt == new DateTimeOffset(2026, 11, 8, 22, 0, 0, TimeSpan.FromHours(2)), "November snapshot missing");
}
Check(meters.Where(r => r.WaterMeterId == 100001).Select(r => r.Reading).Distinct().Count() == 1, "Roof has fictitious demand");
Check(loggerReadings.Length == 6104 && loggerReadings.All(r => r.IsSimulated && r.FlowM3PerHour >= 0 && r.PressureBar > 0), "Logger values/count");
Check(loggerReadings.Select(r => (r.LoggerId, r.TakenAt)).Distinct().Count() == loggerReadings.Length, "Duplicate logger timestamp");
foreach (var logger in loggerReadings.GroupBy(r => r.LoggerId))
{
    var ordered = logger.OrderBy(r => r.TakenAt).ToArray();
    Check(ordered.Zip(ordered.Skip(1)).All(p => p.Second.TakenAt - p.First.TakenAt == TimeSpan.FromMinutes(15)), "Quarter-hour gap");
    Check(ordered.Count(r => r.TakenAt.Month == 10) == 2980, "Incorrect October DST interval count");
    Check(ordered.Count(r => r.TakenAt.Date == new DateTime(2026, 10, 25)) == 100, "Clock change not handled");
    Check(ordered.Any(r => r.TakenAt.Offset == TimeSpan.FromHours(3)) && ordered.Any(r => r.TakenAt.Offset == TimeSpan.FromHours(2)), "Missing DST offset");
}
var inletByTime = loggerReadings.Where(r => r.LoggerId == 200001).ToDictionary(r => r.TakenAt);
Check(loggerReadings.Where(r => r.LoggerId == branch.Id).All(r => r.FlowM3PerHour <= inletByTime[r.TakenAt].FlowM3PerHour), "Branch exceeds inlet");

// Verify the database IDs against the actual embedded map, not just another seed list.
var assembly = typeof(WaterMeter).Assembly;
var zones = assembly.GetType("WaterNetworkMonitoring.BlagoevgradNeighbourhoods")!.GetMethod("LoadZones")!.Invoke(null, null);
var buildings = ((IEnumerable)assembly.GetType("WaterNetworkMonitoring.BuildingMapData")!
    .GetMethod("Load")!.Invoke(null, new[] { zones })!).Cast<object>().ToArray();
var seedType = assembly.GetType("WaterNetworkMonitoring.Data.ZapadOctober2026Seed")!;
var profiles = ((IEnumerable)seedType.GetField("Profiles", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!).Cast<object>().ToArray();
T Property<T>(object value, string name) => (T)value.GetType().GetProperty(name)!.GetValue(value)!;
var model = db.GetService<IDesignTimeModel>().Model;
var deviceSeed = model.FindEntityType(typeof(WaterMeter))!.GetSeedData().ToArray();
Check(buildings.Length == 67 && profiles.Length == 67 && deviceSeed.Length == 67, "Device/map count mismatch");
foreach (var profile in profiles)
{
    int number = Property<int>(profile, "MapNumber"), id = Property<int>(profile, "Id");
    Check(Property<long>(buildings[number - 1], "OsmId") == Property<long>(profile, "OsmId"), "Map building ID mismatch");
    Check(deviceSeed.Any(s => (int)s["Id"]! == id && (int)s["MapNumber"]! == number), "Device ID changed");
}
Check(model.FindEntityType(typeof(LoggerReading))!.GetIndexes().Any(i => i.IsUnique && i.Properties.Count == 2), "Logger uniqueness missing");
Check(model.FindEntityType(typeof(WaterMeterReading))!.GetForeignKeys().Single().PrincipalEntityType.ClrType == typeof(WaterMeter), "Meter FK missing");

// An independent volume balance confirms that leakage bypasses the building's counter.
var baselineEnd = Local(29, 18);
var result = LeakDetectionService.AnalyzeBranch(branch, loggerReadings, meters, baselineEnd);
Check(result.Where(r => r.To <= Local(30, 6)).All(r => r.ExcessM3PerHour == 0 && !r.LeakSuspected), "False alarm before leak");
var firstAlarm = result.First(r => r.LeakSuspected);
Check(firstAlarm.To == Local(30, 18) && firstAlarm.ExcessM3PerHour == 0.5m && firstAlarm.PressureDrop, "Leak detection/volume mismatch");
Check(firstAlarm.SuppliedM3 - firstAlarm.ConsumedM3 == 6m, "12-hour leak should be 6000 litres");
var shiftedLogger = loggerReadings.Select(r => new LoggerReading { LoggerId = r.LoggerId, TakenAt = r.TakenAt.AddDays(5), FlowM3PerHour = r.FlowM3PerHour, PressureBar = r.PressureBar });
var shiftedMeter = meters.Select(r => new WaterMeterReading { WaterMeterId = r.WaterMeterId, TakenAt = r.TakenAt.AddDays(5), Reading = r.Reading });
Check(LeakDetectionService.AnalyzeBranch(branch, shiftedLogger, shiftedMeter, baselineEnd.AddDays(5)).First(r => r.LeakSuspected).To == firstAlarm.To.AddDays(5), "Detector depends on planted calendar date");
var incomplete = loggerReadings.Where(r => !(r.LoggerId == branch.Id && r.TakenAt == Local(30, 7)));
Check(!LeakDetectionService.AnalyzeBranch(branch, incomplete, meters, baselineEnd).Any(r => r.To == firstAlarm.To), "Incomplete window generated an alarm");
Reject(() => LeakDetectionService.AnalyzeBranch(branch, loggerReadings.Append(loggerReadings.First(r => r.LoggerId == branch.Id)), meters, baselineEnd), "Duplicate reading accepted");
Reject(() => LeakDetectionService.AnalyzeBranch(branch, loggerReadings, meters, Local(1, 6, 3)), "Short baseline accepted");

var secondInserts = CreateMigration().UpOperations.OfType<InsertDataOperation>().ToArray();
Check(inserts.Length == secondInserts.Length, "Generation is not repeatable");
for (int i = 0; i < inserts.Length; i++)
    Check(inserts[i].Values.Cast<object?>().SequenceEqual(secondInserts[i].Values.Cast<object?>()), "Generation changed between runs");
Check(migration.UpOperations.OfType<CreateTableOperation>().Select(t => t.Name).Order().SequenceEqual(new[] { "LoggerReadings", "Loggers", "WaterMeterReadings" }.Order()), "Unexpected schema operations");
Check(!migration.UpOperations.OfType<DeleteDataOperation>().Any(), "Existing data deleted");
Check(migration.DownOperations.OfType<DropTableOperation>().Count() == 3, "Rollback missing tables");
Console.WriteLine($"PASS: 67 devices; 4154 October meter readings + 201 boundary/snapshot rows; 5960 October logger readings + 144 boundary rows; all map IDs, schedules, offsets, FKs and duplicate protections verified.");
Console.WriteLine($"PASS: repeatable migration; no pre-leak alarms; first confirmed branch leak {firstAlarm.To:O}, 500 L/hour, 6000 L lost over 12 hours; missing/duplicate data and shifted dates checked.");
