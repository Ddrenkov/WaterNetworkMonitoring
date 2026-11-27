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
            var histories = readings.GroupBy(reading => reading.WaterMeterId)
                .ToDictionary(group => group.Key, group => group.OrderBy(reading => reading.TakenAt).ToArray());
            Months.AddRange(FindMonths(histories.Values.SelectMany(history => history)));

            foreach (var meter in meters.OrderBy(meter => meter.MapNumber))
            {
                var history = histories.GetValueOrDefault(meter.Id) ?? Array.Empty<WaterMeterReading>();
                var consumption = new Dictionary<Month, Consumption>();
                foreach (var month in Months)
                    consumption.Add(month, Calculate(history, month));
                Meters.Add(new MeterUsage(meter, consumption));
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

        private static List<Month> FindMonths(IEnumerable<WaterMeterReading> readings)
        {
            var months = new List<Month>();
            var times = readings.Select(reading => reading.TakenAt).ToArray();
            if (times.Length == 0) return months;

            var firstTime = times.Min();
            var lastTime = times.Max();
            var first = new DateTime(firstTime.Year, firstTime.Month, 1);
            var last = new DateTime(lastTime.Year, lastTime.Month, 1);
            if (first < last.AddMonths(-12)) first = last.AddMonths(-12);
            for (var date = first; date < last.AddMonths(1); date = date.AddMonths(1))
                months.Add(new Month(date.Year, date.Month));
            return months;
        }

        private static Consumption Calculate(WaterMeterReading[] history, Month month)
        {
            var from = new DateTime(month.Year, month.Number, 1);
            var to = from.AddMonths(1);
            var left = history.LastOrDefault(reading => reading.TakenAt <= from);
            var right = history.FirstOrDefault(reading => reading.TakenAt >= to);
            if (left == null || right == null)
                return new Consumption(null, false, "Readings before and after the month are needed.");

            var window = history.Where(reading => reading.TakenAt >= left.TakenAt && reading.TakenAt <= right.TakenAt).ToArray();
            string? error = ValidateReadings(window);
            if (error != null) return new Consumption(null, false, error);

            double typicalGap = TypicalGapSeconds(history);
            decimal? start = CounterAt(window, from, typicalGap);
            decimal? end = CounterAt(window, to, typicalGap);
            if (!start.HasValue || !end.HasValue)
                return new Consumption(null, false, "A month boundary has missing readings.");

            decimal litres = end.Value - start.Value;
            bool estimated = left.TakenAt != from || right.TakenAt != to;
            if (estimated)
                return new Consumption(litres, true, "Midnight counters estimated between the surrounding readings.");
            return new Consumption(litres, false, "Difference between recorded counters at both month boundaries.");
        }

        private static string? ValidateReadings(WaterMeterReading[] rows)
        {
            for (int i = 1; i < rows.Length; i++)
                if (rows[i].TakenAt == rows[i - 1].TakenAt)
                    return "Duplicate reading timestamps.";

            foreach (var row in rows)
            {
                if (row.Reading.Length != 8) return "Invalid meter counter.";
                foreach (char digit in row.Reading)
                    if (digit < '0' || digit > '9') return "Invalid meter counter.";
            }

            for (int i = 1; i < rows.Length; i++)
                if (Counter(rows[i]) < Counter(rows[i - 1]))
                    return "Meter reset or rollover needs review.";
            return null;
        }

        private static double TypicalGapSeconds(WaterMeterReading[] rows)
        {
            var gaps = new List<double>();
            for (int i = 1; i < rows.Length; i++)
            {
                double gap = (rows[i].TakenAt - rows[i - 1].TakenAt).TotalSeconds;
                if (gap > 0) gaps.Add(gap);
            }
            gaps.Sort();
            if (gaps.Count == 0) return 0;
            return gaps[gaps.Count / 2];
        }

        private static decimal? CounterAt(WaterMeterReading[] rows, DateTime time, double typicalGap)
        {
            var left = rows.LastOrDefault(reading => reading.TakenAt <= time);
            if (left == null) return null;
            if (left.TakenAt == time) return Counter(left);

            var right = rows.FirstOrDefault(reading => reading.TakenAt > time);
            if (right == null) return null;
            double gap = (right.TakenAt - left.TakenAt).TotalSeconds;
            if (gap > typicalGap * 1.5) return null;

            decimal fraction = (decimal)((time - left.TakenAt).TotalSeconds / gap);
            return Counter(left) + (Counter(right) - Counter(left)) * fraction;
        }

        private static decimal Counter(WaterMeterReading row) => decimal.Parse(row.Reading, CultureInfo.InvariantCulture);
        internal static DateTime ChartTime(Month month) => new DateTime(month.Year, month.Number, 1, 12, 0, 0);
    }
}
