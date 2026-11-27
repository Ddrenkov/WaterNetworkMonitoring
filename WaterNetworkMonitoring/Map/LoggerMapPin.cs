using GMap.NET;
using GMap.NET.WindowsForms;
using WaterNetworkMonitoring.Models;
using WaterNetworkMonitoring.Services;

namespace WaterNetworkMonitoring
{
    internal sealed class LoggerMapPin : GMapMarker
    {
        private readonly string label;
        private readonly Color color;
        private bool suspectedLeak;
        public int LoggerId { get; }

        private LoggerMapPin(PressureLogger logger) : base(new PointLatLng(logger.Latitude, logger.Longitude))
        {
            LoggerId = logger.Id;
            string shortName = LabelFor(LoggerId);
            label = shortName == LoggerId.ToString() ? shortName : $"{shortName} · {LoggerId}";
            color = ColorFor(LoggerId);
            Size = new Size(96, 24);
            bool left = LoggerId is 200001 or 200004;
            bool above = LoggerId is 200001 or 200003;
            Offset = new Point(left ? -Size.Width - 6 : 6, above ? -Size.Height - 6 : 6);
            IsHitTestVisible = true;
            ToolTipMode = MarkerTooltipMode.OnMouseOver;
        }

        public static string LabelFor(int id) => id is >= 200001 and <= 200004 ? $"L{id - 200000}" : id.ToString();
        public static Color ColorFor(int id) => id switch
        {
            200001 => Color.FromArgb(30, 115, 74),
            200002 => Color.FromArgb(107, 65, 170),
            200003 => Color.FromArgb(37, 99, 180),
            200004 => Color.FromArgb(190, 106, 25),
            _ => new[] { Color.SeaGreen, Color.SteelBlue, Color.DarkViolet, Color.DarkOrange }[(int)((uint)id % 4)]
        };

        public static GMapOverlay CreateOverlay() => new("zapad-loggers");

        public static void UpdateReadings(GMapOverlay? overlay, DashboardData? data,
            DateTime? through = null, int? flaggedLoggerId = null, BuildingPipeGraph? pipeline = null)
        {
            if (overlay == null) return;
            var loggers = data?.Loggers.Where(HasValidPosition).ToList() ?? new List<PressureLogger>();
            RemoveMissingPins(overlay, loggers);
            if (data == null) return;

            foreach (var logger in loggers)
            {
                var pin = overlay.Markers.OfType<LoggerMapPin>().FirstOrDefault(p => p.LoggerId == logger.Id);
                if (pin == null)
                {
                    pin = new LoggerMapPin(logger);
                    overlay.Markers.Add(pin);
                }
                var reading = LatestReading(data.LoggerReadings, logger.Id, through);
                var meters = data.Meters.Where(m => m.LoggerId == logger.Id).ToList();
                int? building = meters.Count == 1 ? meters[0].MapNumber : null;
                pin.suspectedLeak = logger.Id == flaggedLoggerId;
                pin.SetReading(logger, reading, building, pipeline);
            }
        }

        private static bool HasValidPosition(PressureLogger logger) =>
            double.IsFinite(logger.Latitude) && double.IsFinite(logger.Longitude) &&
            logger.Latitude != 0 && logger.Longitude != 0 &&
            Math.Abs(logger.Latitude) <= 90 && Math.Abs(logger.Longitude) <= 180;

        private static void RemoveMissingPins(GMapOverlay overlay, List<PressureLogger> loggers)
        {
            var ids = loggers.Select(logger => logger.Id).ToHashSet();
            foreach (var pin in overlay.Markers.OfType<LoggerMapPin>().ToList())
            {
                if (ids.Contains(pin.LoggerId)) continue;
                overlay.Markers.Remove(pin);
                pin.Dispose();
            }
        }

        private static LoggerReading? LatestReading(IEnumerable<LoggerReading> readings, int loggerId, DateTime? through) =>
            readings.Where(r => r.LoggerId == loggerId && (!through.HasValue || r.TakenAt <= through.Value))
                .OrderByDescending(r => r.TakenAt).FirstOrDefault();

        private void SetReading(PressureLogger logger, LoggerReading? reading, int? building, BuildingPipeGraph? pipeline)
        {
            var position = new PointLatLng(logger.Latitude, logger.Longitude);
            Position = pipeline?.PositionOnPipe(position, building) ?? position;
            string details = reading == null ? "No reading available"
                : $"Flow: {reading.FlowM3PerHour:0.###} m³/h\nPressure: {reading.PressureBar:0.###} bar\n{reading.TakenAt:dd MMM yyyy HH:mm:ss}";
            ToolTipText = $"{logger.Name} ({LoggerId})\n{details}\nApproximate project location";
        }

        public static void Select(GMapOverlay? overlay, LoggerMapPin? selected)
        {
            if (overlay == null) return;
            foreach (var pin in overlay.Markers.OfType<LoggerMapPin>())
                pin.ToolTipMode = pin == selected ? MarkerTooltipMode.Always : MarkerTooltipMode.OnMouseOver;
        }

        public override void OnRender(Graphics graphics)
        {
            var anchor = new Point(LocalPosition.X - Offset.X, LocalPosition.Y - Offset.Y);
            var bounds = new Rectangle(LocalPosition, Size);
            Color pinColor = suspectedLeak ? Color.Firebrick : color;
            using var pen = new Pen(pinColor, 2);
            using var fill = new SolidBrush(pinColor);
            graphics.DrawLine(pen, anchor, new Point(Offset.X < 0 ? bounds.Right : bounds.Left,
                Offset.Y < 0 ? bounds.Bottom : bounds.Top));
            graphics.FillEllipse(fill, anchor.X - 4, anchor.Y - 4, 8, 8);
            graphics.FillRectangle(fill, bounds);
            graphics.DrawRectangle(Pens.White, bounds);
            using var font = new Font(SystemFonts.DefaultFont.FontFamily, 8F);
            using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            graphics.DrawString(label, font, Brushes.White, bounds, format);
        }
    }
}
