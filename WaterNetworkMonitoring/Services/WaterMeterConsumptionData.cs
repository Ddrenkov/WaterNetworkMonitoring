using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WaterNetworkMonitoring.Data;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Services
{
    internal sealed class WaterMeterConsumptionData
    {
        internal sealed record Month(int Year, int Number)
        {
            public override string ToString() => new DateTime(Year, Number, 1).ToString("MMM yyyy");
        }
        internal sealed record Consumption(decimal? Litres, bool Estimated, string Note);
        internal sealed record MeterUsage(WaterMeter Meter, Dictionary<Month, Consumption> Months);
        public List<Month> Months { get; } = new();
        public List<MeterUsage> Meters { get; } = new();

        internal WaterMeterConsumptionData(IEnumerable<WaterMeter> meters, IEnumerable<WaterMeterReading> readings)
        {
            var histories = readings.GroupBy(r => r.WaterMeterId)
                .ToDictionary(g => g.Key, g => g.OrderBy(r => r.TakenAt).ToArray());
            var times = histories.Values.SelectMany(r => r).Select(r => r.TakenAt).ToArray();
            if (times.Length > 0)
            {
                var first = new DateTime(times.Min().Year, times.Min().Month, 1);
                var last = new DateTime(times.Max().Year, times.Max().Month, 1);
                first = first < last.AddMonths(-12) ? last.AddMonths(-12) : first;
                for (var date = first; date < last.AddMonths(1); date = date.AddMonths(1))
                    Months.Add(new Month(date.Year, date.Month));
            }
            foreach (var meter in meters.OrderBy(m => m.MapNumber))
            {
                var history = histories.GetValueOrDefault(meter.Id) ?? Array.Empty<WaterMeterReading>();
                Meters.Add(new MeterUsage(meter, Months.ToDictionary(m => m, m => Calculate(history, m))));
            }
            // A boundary observation in August or December is not a complete month of consumption.
            Months.RemoveAll(month => !Meters.Any(m => m.Months[month].Litres.HasValue));
        }

        internal static async Task<WaterMeterConsumptionData> LoadAsync(CancellationToken cancellationToken)
        {
            using var db = new AppDbContext();
            var connection = new SqlConnectionStringBuilder(db.Database.GetConnectionString())
                { ConnectTimeout = 5, ConnectRetryCount = 0 };
            db.Database.SetConnectionString(connection.ConnectionString);
            db.Database.SetCommandTimeout(10);
            var meters = await db.WaterMeters.AsNoTracking().OrderBy(m => m.MapNumber).ToListAsync(cancellationToken);
            var ids = meters.Select(m => m.Id).ToArray();
            var latest = await db.WaterMeterReadings.AsNoTracking().Where(r => ids.Contains(r.WaterMeterId))
                .OrderByDescending(r => r.TakenAt).Select(r => (DateTime?)r.TakenAt).FirstOrDefaultAsync(cancellationToken);
            var readings = new List<WaterMeterReading>();
            if (latest.HasValue)
            {
                var date = latest.Value;
                var from = new DateTime(date.Year, date.Month, 1).AddMonths(-12).AddDays(-2);
                readings = await db.WaterMeterReadings.AsNoTracking()
                    .Where(r => ids.Contains(r.WaterMeterId) && r.TakenAt >= from && r.TakenAt <= latest.Value)
                    .OrderBy(r => r.TakenAt).ToListAsync(cancellationToken);
            }
            return new WaterMeterConsumptionData(meters, readings);
        }

        private static Consumption Calculate(WaterMeterReading[] history, Month month)
        {
            var from = new DateTime(month.Year, month.Number, 1);
            var to = new DateTime(month.Year, month.Number, 1).AddMonths(1);
            var left = history.LastOrDefault(r => r.TakenAt <= from);
            var right = history.FirstOrDefault(r => r.TakenAt >= to);
            if (left == null || right == null) return new(null, false, "Readings before and after the month are needed.");
            var window = history.Where(r => r.TakenAt >= left.TakenAt && r.TakenAt <= right.TakenAt).ToArray();
            if (window.DistinctBy(r => r.TakenAt).Count() != window.Length)
                return new(null, false, "Duplicate reading timestamps.");
            if (window.Any(r => r.Reading.Length != 8 || r.Reading.Any(c => c is < '0' or > '9')))
                return new(null, false, "Invalid meter counter.");
            if (window.Zip(window.Skip(1), (a, b) => Counter(b) < Counter(a)).Any(reset => reset))
                return new(null, false, "Meter reset or rollover needs review.");
            var gaps = history.Zip(history.Skip(1), (a, b) => (b.TakenAt-a.TakenAt).TotalSeconds).Where(g => g > 0).Order().ToArray();
            double typicalGap = gaps.Length == 0 ? 0 : gaps[gaps.Length / 2];
            decimal? start = CounterAt(window, from, typicalGap), end = CounterAt(window, to, typicalGap);
            if (!start.HasValue || !end.HasValue) return new(null, false, "A month boundary has missing readings.");
            bool estimated = left.TakenAt != from || right.TakenAt != to;
            return new(end.Value-start.Value, estimated, estimated
                ? "Midnight counters estimated between the surrounding readings."
                : "Difference between recorded counters at both month boundaries.");
        }

        private static decimal? CounterAt(WaterMeterReading[] rows, DateTime time, double typicalGap)
        {
            var left = rows.LastOrDefault(r => r.TakenAt <= time);
            if (left == null) return null;
            if (left.TakenAt == time) return Counter(left);
            var right = rows.FirstOrDefault(r => r.TakenAt > time);
            if (right == null || (right.TakenAt-left.TakenAt).TotalSeconds > typicalGap * 1.5) return null;
            return Counter(left) + (Counter(right)-Counter(left)) *
                (decimal)((time-left.TakenAt).TotalSeconds/(right.TakenAt-left.TakenAt).TotalSeconds);
        }
        private static decimal Counter(WaterMeterReading row) => decimal.Parse(row.Reading, CultureInfo.InvariantCulture);
        internal static DateTime ChartTime(Month month) => new DateTime(month.Year, month.Number, 1, 12, 0, 0);
    }
}
