using System.Globalization;
using WaterNetworkMonitoring.Models;
using WaterNetworkMonitoring.Services;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using WaterNetworkMonitoring.Data;

namespace WaterNetworkMonitoring.Tests;

[TestFixture]
public class AlgorithmTests
{
    [Test]
    public void BalancedSupply_DoesNotReportLeak()
    {
        var data = CleanWaterReadings();
        var balances = Analyze(data);

        Assert.That(balances, Has.Count.EqualTo(16));
        foreach (var balance in balances)
        {
            Assert.That(balance.LeakSuspected, Is.False);
            Assert.That(balance.SuppliedM3, Is.EqualTo(12m));
            Assert.That(balance.ConsumedM3, Is.EqualTo(12m));
        }
    }

    [Test]
    public void SustainedExtraSupply_ReportsLeak()
    {
        var data = CleanWaterReadings(leak: true);
        var balances = Analyze(data);

        foreach (var balance in balances.Where(b => b.To <= data.BaselineEnd))
            Assert.That(balance.LeakSuspected, Is.False);
        Assert.That(balances[^1].LeakSuspected, Is.True);
        Assert.That(balances[^1].PressureDrop, Is.True);
        Assert.That(balances[^1].ExcessM3PerHour, Is.EqualTo(.5m));
    }

    [Test]
    public void MissingLoggerReading_SkipsIncompleteWindow()
    {
        var data = CleanWaterReadings();
        var missingWindowStart = data.MeterReadings[11].TakenAt;
        data.LoggerReadings.RemoveAll(r => r.TakenAt == missingWindowStart.AddMinutes(15));

        var balances = Analyze(data);

        Assert.That(balances, Has.Count.EqualTo(15));
        Assert.That(balances.Any(b => b.From == missingWindowStart), Is.False);
        foreach (var balance in balances)
            Assert.That(balance.LeakSuspected, Is.False);
    }

    [Test]
    public void BalancedReadingsAfterLeak_ClearWarningWhileCusumIsStillPositive()
    {
        var data = CleanWaterReadings(leak: true);
        foreach (var reading in data.LoggerReadings.TakeLast(48))
        {
            reading.FlowM3PerHour = 1m;
            reading.PressureBar = 4m;
        }

        var balances = Analyze(data);

        Assert.That(balances[^2].LeakSuspected, Is.True);
        Assert.That(balances[^1].Cusum, Is.GreaterThan(0m));
        Assert.That(balances[^1].LeakSuspected, Is.False);
    }

    [Test]
    public void PressureDropWithoutWaterDifference_DoesNotReportLeak()
    {
        var data = CleanWaterReadings();
        foreach (var reading in data.LoggerReadings.TakeLast(48))
            reading.PressureBar = 3.5m;

        var balance = Analyze(data)[^1];

        Assert.That(balance.PressureDrop, Is.True);
        Assert.That(balance.ExcessM3PerHour, Is.Zero);
        Assert.That(balance.LeakSuspected, Is.False);
    }

    [Test]
    public void ShortHistory_ReturnsBalancesOnlyWhenDetectionIsOptional()
    {
        var data = CleanWaterReadings();
        var cutoff = data.MeterReadings[9].TakenAt;
        data.LoggerReadings.RemoveAll(reading => reading.TakenAt > cutoff);

        Assert.Throws<ArgumentException>(() => Analyze(data));
        var balances = LeakDetectionService.AnalyzeMeterBalance(data.Logger, data.LoggerReadings,
            new[] { data.Meter }, data.MeterReadings, data.BaselineEnd, requireBaseline: false);

        Assert.That(balances, Has.Count.EqualTo(9));
        Assert.That(balances.All(balance => balance.DetectionThreshold == 0 && !balance.LeakSuspected), Is.True);
    }


    [Test]
    public void NormalDayAndNightFlows_DoNotRequireCleaning()
    {
        var alerts = WasteWaterMaintenance.FindAlerts(WastewaterReadings());

        Assert.That(alerts, Is.Empty);
    }

    [Test]
    public void TwoAbnormalReadings_DoNotRequireCleaning()
    {
        var readings = WastewaterReadings();
        AddBlockage(readings, count: 2);

        Assert.That(WasteWaterMaintenance.FindAlerts(readings), Is.Empty);
    }

    [Test]
    public void ThreeDryAbnormalReadings_RequireCleaning()
    {
        var readings = WastewaterReadings();
        AddBlockage(readings, count: 3);

        var alerts = WasteWaterMaintenance.FindAlerts(readings);
        Assert.That(alerts, Has.Count.EqualTo(1));
        var alert = alerts[0];

        Assert.That(alert.LoggerId, Is.EqualTo(300002));
        Assert.That(alert.FlowDropPercent, Is.EqualTo(65m));
        Assert.That(alert.ExtraLevelMetres, Is.EqualTo(.4m));
    }

    [Test]
    public void RainyAbnormalReadings_DoNotRequireCleaning()
    {
        var readings = WastewaterReadings();
        AddBlockage(readings, count: 3);
        foreach (var reading in readings.TakeLast(3)) reading.IsRaining = true;

        Assert.That(WasteWaterMaintenance.FindAlerts(readings), Is.Empty);
    }

    [Test]
    public void HealthyReadingsAfterBlockage_ClearWarning()
    {
        var readings = WastewaterReadings();
        AddBlockage(readings, count: 6);
        Assert.That(WasteWaterMaintenance.FindAlerts(readings), Has.Count.EqualTo(1));
        foreach (var reading in readings.TakeLast(3))
        {
            reading.FlowM3PerHour /= .35m;
            reading.WaterLevelMetres -= .4m;
        }

        Assert.That(WasteWaterMaintenance.FindAlerts(readings), Is.Empty);
    }

    [Test]
    public void MissingNormalHistory_DoesNotRequireCleaning()
    {
        var readings = WastewaterReadings();
        AddBlockage(readings, count: 3);

        Assert.That(WasteWaterMaintenance.FindAlerts(readings.TakeLast(3)), Is.Empty);
        Assert.That(WasteWaterMaintenance.FindAlerts(Array.Empty<WasteWaterReading>()), Is.Empty);
    }

    [Test]
    public void MonthlyConsumption_IsDifferenceBetweenCounters()
    {
        var start = new DateTime(2026, 11, 1, 0, 0, 0);
        var usage = Consumption(start, start.AddMonths(1), 100000, 105000);

        Assert.That(usage.Litres, Is.EqualTo(5000m));
        Assert.That(usage.Estimated, Is.False);
    }

    [Test]
    public void OctoberConsumption_UsesPlainLocalMonthBoundaries()
    {
        var start = new DateTime(2026, 10, 1, 0, 0, 0);
        var end = new DateTime(2026, 11, 1, 0, 0, 0);

        var usage = Consumption(start, end, 100000, 174400);

        Assert.That(usage.Litres, Is.EqualTo(74400m));
        Assert.That(usage.Estimated, Is.False);
    }

    [Test]
    public void MissingMonthBoundary_DoesNotInventConsumption()
    {
        var start = new DateTime(2026, 11, 1, 0, 0, 0);
        var usage = Consumption(start.AddDays(1), start.AddMonths(1), 100000, 105000);

        Assert.That(usage.Litres, Is.Null);
        Assert.That(usage.Note, Is.Not.Empty);
    }

    [Test]
    public void ReadingModels_UseOnePlainLocalTimestamp()
    {
        using var db = new AppDbContext();
        foreach (var type in new[] { typeof(WaterMeter), typeof(LoggerReading),
            typeof(WaterMeterReading), typeof(WasteWaterReading) })
        {
            var entity = db.Model.FindEntityType(type)!;
            var time = entity.FindProperty("TakenAt")!;
            Assert.That(time.ClrType, Is.EqualTo(typeof(DateTime)));
            Assert.That(time.GetColumnType(), Is.EqualTo("datetime2(0)"));
            Assert.That(time.GetValueConverter(), Is.Null);
            Assert.That(entity.FindProperty("LocalTakenAt"), Is.Null);
        }
        Assert.That(db.Database.HasPendingModelChanges(), Is.False);
    }

    [Test]
    public void OctoberSeeds_KeepRegularLocalClockIntervals()
    {
        var clean = ZapadFourLoggerSeed.GenerateReadings().Where(r =>
            r.TakenAt > new DateTime(2026, 10, 1) && r.TakenAt <= new DateTime(2026, 11, 1));
        foreach (var group in clean.GroupBy(r => r.LoggerId))
        {
            var times = group.Select(r => r.TakenAt).Order().ToArray();
            Assert.That(times, Has.Length.EqualTo(31 * 96));
            Assert.That(times.All(t => t.Kind == DateTimeKind.Unspecified), Is.True);
            Assert.That(times.Zip(times.Skip(1), (a, b) => b - a),
                Is.All.EqualTo(TimeSpan.FromMinutes(15)));
        }
        foreach (var group in WasteWaterOctSeed.GenerateReadings().GroupBy(r => r.LoggerId))
        {
            var times = group.Select(r => r.TakenAt).Order().ToArray();
            Assert.That(times, Has.Length.EqualTo(31 * 12));
            Assert.That(times.Zip(times.Skip(1), (a, b) => b - a),
                Is.All.EqualTo(TimeSpan.FromHours(2)));
        }
    }

    [Test]
    public void ThreeMonthCleanWaterSeeds_PreserveLeakAndRepairResults()
    {
        var october = ZapadOctober2026Seed.Generate();
        var other = SepNovSeed.Generate();
        var meters = ZapadWaterMeterSeed.GetReadings();
        foreach (var meter in meters) meter.LoggerId = ZapadFourLoggerSeed.LoggerForMeter(meter.Id);
        var data = new DashboardData
        {
            Loggers = ZapadFourLoggerSeed.GetLoggers().ToList(), Meters = meters.ToList(),
            LoggerReadings = ZapadFourLoggerSeed.GenerateReadings().Concat(other.LoggerReadings)
                .GroupBy(r => (r.LoggerId, r.TakenAt)).Select(g => g.Last()).ToList(),
            MeterReadings = october.MeterReadings.Concat(other.MeterReadings)
                .GroupBy(r => (r.WaterMeterId, r.TakenAt)).Select(g => g.Last()).ToList(),
            Repairs = SepNovSeed.GetRepairs().ToList()
        };
        var september = data.Report(new(2026, 9, 0));
        var octoberReport = data.Report(new(2026, 10, 0));
        var november = data.Report(new(2026, 11, 0));
        Assert.That(september.LatestLeak, Is.Null);
        Assert.That(november.LatestLeak, Is.Null);
        Assert.That(octoberReport.FlaggedLoggerId, Is.EqualTo(200002));
        Assert.That(octoberReport.LatestLeak!.ExcessM3PerHour, Is.EqualTo(.5m).Within(.001m));
        Assert.That(new[] { september, octoberReport, november }.All(r => r.CompleteMonth), Is.True);
        var usage = new WaterMeterConsumptionData(meters, data.MeterReadings);
        Assert.That(usage.Months, Has.Count.EqualTo(3));
        Assert.That(usage.Meters.All(m => usage.Months.All(month => m.Months[month].Litres >= 0)), Is.True);
    }

    [Test]
    public void WastewaterSeeds_PreserveHealthySeptemberAndBlockedNovember()
    {
        var september = WasteWaterSepSeed.GenerateTwoHourReadings();
        var october = WasteWaterOctSeed.GenerateReadings();
        var november = WasteWaterNovSeed.GenerateReadings();
        Assert.That(september, Has.Count.EqualTo(3 * 30 * 12));
        Assert.That(november, Has.Count.EqualTo(3 * 30 * 12));
        Assert.That(WasteWaterMaintenance.FindAlerts(september), Is.Empty);
        var alerts = WasteWaterMaintenance.FindAlerts(september.Concat(october).Concat(november));
        Assert.That(alerts, Has.Count.EqualTo(1));
        Assert.That(alerts[0].LoggerId, Is.EqualTo(300002));
        Assert.That(alerts[0].FlowDropPercent, Is.GreaterThan(60m));
        Assert.That(alerts[0].ExtraLevelMetres, Is.GreaterThan(.45m));
    }

    // Independent examples: no database, map controls or project seeds are loaded.
    private sealed record CleanWaterExample(PressureLogger Logger, WaterMeter Meter,
        List<LoggerReading> LoggerReadings, List<WaterMeterReading> MeterReadings,
        DateTime BaselineEnd);

    private static CleanWaterExample CleanWaterReadings(bool leak = false)
    {
        var start = new DateTime(2026, 2, 1, 0, 0, 0);
        var logger = new PressureLogger { Id = 200001 };
        var meter = new WaterMeter { Id = 100001, LoggerId = logger.Id };
        var loggerReadings = new List<LoggerReading>();
        var meterReadings = new List<WaterMeterReading>();
        // Sixteen 12-hour meter windows; the first ten establish normal behaviour.
        for (int window = 0; window <= 16; window++)
            meterReadings.Add(new WaterMeterReading
            {
                WaterMeterId = meter.Id, TakenAt = start.AddHours(window * 12),
                Reading = (100000 + window * 12000).ToString("D8", CultureInfo.InvariantCulture)
            });
        for (int quarter = 1; quarter <= 16 * 48; quarter++)
        {
            bool leaking = leak && quarter > 10 * 48;
            loggerReadings.Add(new LoggerReading
            {
                LoggerId = logger.Id, TakenAt = start.AddMinutes(quarter * 15),
                FlowM3PerHour = leaking ? 1.5m : 1m, PressureBar = leaking ? 3.5m : 4m
            });
        }
        return new(logger, meter, loggerReadings, meterReadings, start.AddHours(10 * 12));
    }

    private static IReadOnlyList<BranchBalance> Analyze(CleanWaterExample data) =>
        LeakDetectionService.AnalyzeSubzone(data.Logger, data.LoggerReadings,
            new[] { data.Meter }, data.MeterReadings, data.BaselineEnd);

    private static List<WasteWaterReading> WastewaterReadings()
    {
        var start = new DateTime(2026, 2, 1, 0, 0, 0);
        return Enumerable.Range(1, 120).Select(slot =>
        {
            var time = start.AddHours(slot * 2);
            return new WasteWaterReading
            {
                LoggerId = 300002, TakenAt = time,
                FlowM3PerHour = time.Hour < 6 ? 1m : 6m, WaterLevelMetres = .2m
            };
        }).ToList();
    }

    private static void AddBlockage(List<WasteWaterReading> readings, int count)
    {
        foreach (var reading in readings.TakeLast(count))
        {
            reading.FlowM3PerHour *= .35m;
            reading.WaterLevelMetres += .4m;
        }
    }

    private static WaterMeterConsumptionData.Consumption Consumption(
        DateTime first, DateTime last, int firstCounter, int lastCounter)
    {
        var meter = new WaterMeter { Id = 100001, MapNumber = 1 };
        var readings = new[]
        {
            new WaterMeterReading { WaterMeterId = meter.Id, TakenAt = first,
                Reading = firstCounter.ToString("D8", CultureInfo.InvariantCulture) },
            new WaterMeterReading { WaterMeterId = meter.Id, TakenAt = last,
                Reading = lastCounter.ToString("D8", CultureInfo.InvariantCulture) }
        };
        var data = new WaterMeterConsumptionData(new[] { meter }, readings);
        return data.Meters.Single().Months[new(first.Year, first.Month)];
    }
}
