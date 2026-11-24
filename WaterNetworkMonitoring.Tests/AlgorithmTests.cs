using System.Globalization;
using WaterNetworkMonitoring.Models;
using WaterNetworkMonitoring.Services;
using NUnit.Framework;

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
        var start = new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.FromHours(2));
        var usage = Consumption(start, start.AddMonths(1), 100000, 105000);

        Assert.That(usage.Litres, Is.EqualTo(5000m));
        Assert.That(usage.Estimated, Is.False);
    }

    [Test]
    public void OctoberConsumption_UsesLocalMonthAcrossClockChange()
    {
        var start = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.FromHours(3));
        var end = new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.FromHours(2));

        var usage = Consumption(start, end, 100000, 174500);

        Assert.That(usage.Litres, Is.EqualTo(74500m));
        Assert.That(usage.Estimated, Is.False);
    }

    [Test]
    public void MissingMonthBoundary_DoesNotInventConsumption()
    {
        var start = new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.FromHours(2));
        var usage = Consumption(start.AddDays(1), start.AddMonths(1), 100000, 105000);

        Assert.That(usage.Litres, Is.Null);
        Assert.That(usage.Note, Is.Not.Empty);
    }

    // Independent examples: no database, map controls or project seeds are loaded.
    private sealed record CleanWaterExample(PressureLogger Logger, WaterMeter Meter,
        List<LoggerReading> LoggerReadings, List<WaterMeterReading> MeterReadings,
        DateTimeOffset BaselineEnd);

    private static CleanWaterExample CleanWaterReadings(bool leak = false)
    {
        var start = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.FromHours(2));
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
        var start = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.FromHours(2));
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
        DateTimeOffset first, DateTimeOffset last, int firstCounter, int lastCounter)
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
