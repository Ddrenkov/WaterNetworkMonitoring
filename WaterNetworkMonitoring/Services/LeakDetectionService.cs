using System.Globalization;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Services
{
    public sealed record BranchBalance(DateTime From, DateTime To,
        decimal SuppliedM3, decimal ConsumedM3, decimal ExcessM3PerHour,
        decimal Cusum, bool LeakSuspected, bool PressureDrop)
    {
        public decimal DetectionThreshold { get; init; }
    }

    public static class LeakDetectionService
    {
        public static IReadOnlyList<BranchBalance> AnalyzeBranch(PressureLogger branch,
            IEnumerable<LoggerReading> loggerReadings, IEnumerable<WaterMeterReading> meterReadings,
            DateTime baselineEnd)
        {
            if (branch.SuppliedWaterMeterId is not int id)
                throw new ArgumentException("Choose a logger with an assigned meter.");
            return AnalyzeMeterBalance(branch,loggerReadings,
                new[] { new WaterMeter { Id=id, LoggerId=branch.Id } },meterReadings,baselineEnd);
        }

        public static IReadOnlyList<BranchBalance> AnalyzeSubzone(PressureLogger branch,
            IEnumerable<LoggerReading> loggerReadings, IEnumerable<WaterMeter> meters,
            IEnumerable<WaterMeterReading> meterReadings, DateTime baselineEnd)
            => AnalyzeMeterBalance(branch, loggerReadings, meters.Where(m => m.LoggerId == branch.Id), meterReadings, baselineEnd);

        public static IReadOnlyList<BranchBalance> AnalyzeMeterBalance(PressureLogger supply,
            IEnumerable<LoggerReading> loggerReadings, IEnumerable<WaterMeter> meters,
            IEnumerable<WaterMeterReading> meterReadings, DateTime baselineEnd, bool requireBaseline = true)
        {
            var ids = meters.Select(m => m.Id).ToHashSet();
            if (ids.Count == 0) throw new ArgumentException("No meters are assigned to this logger.");
            var histories = meterReadings.Where(r => ids.Contains(r.WaterMeterId)).GroupBy(r => r.WaterMeterId)
                .ToDictionary(g => g.Key, g => g.OrderBy(r => r.TakenAt).ToArray());
            if (histories.Count != ids.Count) throw new ArgumentException("Some meters have no readings.");
            if (histories.Values.Any(rows => rows.DistinctBy(r => r.TakenAt).Count() != rows.Length))
                throw new ArgumentException("Duplicate meter timestamps cannot be analyzed.");
            var rows = loggerReadings.Where(r => r.LoggerId == supply.Id).OrderBy(r => r.TakenAt).ToArray();
            if (rows.DistinctBy(r => r.TakenAt).Count() != rows.Length)
                throw new ArgumentException("Duplicate logger timestamps cannot be analyzed.");
            if (rows.Any(r => r.FlowM3PerHour < 0 || r.PressureBar < 0))
                throw new ArgumentException("Logger readings must be nonnegative.");
            if (rows.Length == 0) return Array.Empty<BranchBalance>();
            // Use an actual meter's observation times, rather than fixed demo hours.
            var boundaries = histories.OrderBy(h => h.Key).First().Value.Select(r => r.TakenAt)
                .Where(t => t >= rows[0].TakenAt.AddMinutes(-15) && t <= rows[^1].TakenAt).ToArray();
            var balances = new List<(BranchBalance Balance, decimal Pressure)>();
            for (int i=1; i<boundaries.Length; i++)
            {
                var from=boundaries[i-1]; var to=boundaries[i];
                decimal supplied=0, pressure=0; var covered=from;
                var window=rows.Where(r => r.TakenAt>from && r.TakenAt.AddMinutes(-15)<to);
                foreach(var row in window)
                {
                    var left=row.TakenAt.AddMinutes(-15)>from ? row.TakenAt.AddMinutes(-15) : from;
                    var right=row.TakenAt<to ? row.TakenAt : to;
                    if(left!=covered) break;
                    decimal hours=(decimal)(right-left).TotalHours;
                    supplied+=row.FlowM3PerHour*hours; pressure+=row.PressureBar*hours; covered=right;
                }
                if(covered!=to || to<=from) continue;
                decimal litres=0; bool complete=true;
                foreach(var history in histories.Values)
                {
                    decimal? first=CounterAt(history,from),last=CounterAt(history,to);
                    if(!first.HasValue || !last.HasValue) { complete=false; break; }
                    if(last<first) throw new ArgumentException("A meter reset needs separate handling.");
                    litres+=last.Value-first.Value;
                }
                if(!complete) continue;
                decimal duration=(decimal)(to-from).TotalHours,consumed=litres/1000m;
                balances.Add((new BranchBalance(from,to,supplied,consumed,(supplied-consumed)/duration,0,false,false),pressure/duration));
            }
            return ApplyCusum(balances, baselineEnd, requireBaseline);
        }

        public static IReadOnlyList<BranchBalance> AnalyzeInlet(PressureLogger inlet,
            IEnumerable<PressureLogger> loggers, IEnumerable<LoggerReading> readings, DateTime baselineEnd)
        {
            var children = loggers.Where(l => l.ParentLoggerId == inlet.Id).Select(l => l.Id).ToHashSet();
            if (children.Count == 0) throw new ArgumentException("No downstream loggers are configured.");
            var rows = readings.Where(r => r.LoggerId == inlet.Id || children.Contains(r.LoggerId)).ToArray();
            if (rows.Any(r => r.FlowM3PerHour < 0 || r.PressureBar < 0))
                throw new ArgumentException("Logger readings must be nonnegative.");
            var lookup = rows.ToDictionary(r => (r.LoggerId,r.TakenAt));
            var balances = new List<(BranchBalance Balance, decimal Pressure)>();
            foreach (var row in rows.Where(r => r.LoggerId == inlet.Id).OrderBy(r => r.TakenAt))
            {
                if (children.Any(id => !lookup.ContainsKey((id,row.TakenAt)))) continue;
                decimal downstream = children.Sum(id => lookup[(id,row.TakenAt)].FlowM3PerHour);
                balances.Add((new BranchBalance(row.TakenAt.AddMinutes(-15),row.TakenAt,
                    row.FlowM3PerHour*.25m,downstream*.25m,row.FlowM3PerHour-downstream,0,false,false),row.PressureBar));
            }
            return ApplyCusum(balances, baselineEnd);
        }

        private static decimal? CounterAt(WaterMeterReading[] rows, DateTime time)
        {
            var left = rows.LastOrDefault(r => r.TakenAt <= time);
            if (left == null) return null;
            int start = Counter(left.Reading);
            if (left.TakenAt == time) return start;
            var right = rows.FirstOrDefault(r => r.TakenAt > time);
            if (right == null) return null;
            var gaps=rows.Zip(rows.Skip(1),(a,b)=>(b.TakenAt-a.TakenAt).TotalSeconds).Order().ToArray();
            if(gaps.Length==0 || (right.TakenAt-left.TakenAt).TotalSeconds>gaps[gaps.Length/2]*1.5) return null;
            int end = Counter(right.Reading);
            if (end < start) throw new ArgumentException("A meter reset needs separate handling.");
            // Estimate between nearby observations when meter schedules differ.
            return start+(end-start)*(decimal)((time-left.TakenAt).TotalSeconds/(right.TakenAt-left.TakenAt).TotalSeconds);
        }

        private static IReadOnlyList<BranchBalance> ApplyCusum(List<(BranchBalance Balance, decimal Pressure)> balances,
            DateTime baselineEnd, bool requireBaseline = true)
        {
            var baseline = balances.Where(b => b.Balance.To <= baselineEnd).ToArray();
            if(baseline.Length<10) baseline=balances.Take(10).ToArray();
            if(baseline.Length<10)
            {
                if(requireBaseline) throw new ArgumentException("More normal history is needed for leak detection.");
                return balances.Select(b=>b.Balance).ToList();
            }
            if(baseline[^1].Balance.To>baselineEnd) baselineEnd=baseline[^1].Balance.To;
            decimal mean = baseline.Average(b => b.Balance.ExcessM3PerHour);
            decimal variance = baseline.Sum(b => (b.Balance.ExcessM3PerHour - mean) *
                (b.Balance.ExcessM3PerHour - mean)) / (baseline.Length - 1);
            decimal sigma = (decimal)Math.Sqrt((double)variance);
            decimal allowance = Math.Max(0.05m, sigma * 0.5m);
            decimal threshold = Math.Max(0.20m, sigma * 5m);
            decimal baselinePressure = baseline.Average(b => b.Pressure);
            decimal cusum = 0;
            DateTime? previousEnd = null;
            var result = new List<BranchBalance>();
            foreach (var (balance, pressure) in balances)
            {
                if (previousEnd != balance.From) cusum = 0; // Missing windows break persistence.
                if (balance.From >= baselineEnd)
                    cusum = Math.Max(0, cusum + balance.ExcessM3PerHour - mean - allowance);
                result.Add(balance with
                {
                    Cusum = cusum, DetectionThreshold = balance.From >= baselineEnd ? threshold : 0,
                    // An old accumulated score must not keep an alarm active after flow balances again.
                    LeakSuspected = cusum > threshold && balance.ExcessM3PerHour > mean + allowance,
                    PressureDrop = pressure < baselinePressure - 0.2m
                });
                previousEnd = balance.To;
            }
            return result;
        }

        private static int Counter(string reading)
        {
            if (reading.Length != 8 || reading.Any(c => c is < '0' or > '9'))
                throw new ArgumentException("Meter counters must contain eight digits.");
            return int.Parse(reading, CultureInfo.InvariantCulture);
        }
    }
}
