using GMap.NET;
using GMap.NET.WindowsForms;
using WaterNetworkMonitoring.Models;
using WaterNetworkMonitoring.Data;
using WaterNetworkMonitoring.Services;

namespace WaterNetworkMonitoring
{
    internal sealed class LoggerMapPin : GMapMarker
    {
        private readonly string label;
        private readonly Color color;
        private readonly string defaultName;
        public int LoggerId { get; }
        private readonly BuildingPipeGraph? pipeline;

        private bool suspectedLeak;
        private LoggerMapPin(PressureLogger logger, BuildingPipeGraph? pipeline) : base(new PointLatLng(logger.Latitude,logger.Longitude))
        {
            this.pipeline = pipeline;
            LoggerId=logger.Id; label=$"{LabelFor(logger.Id)} · {logger.Id}";
            color=ColorFor(logger.Id); defaultName=logger.Name;
            Size=new Size(96,24);
            bool left=logger.Id is 200001 or 200004, above=logger.Id is 200001 or 200003;
            Offset=new Point(left ? -Size.Width-6 : 6, above ? -Size.Height-6 : 6);
            IsHitTestVisible=true; ToolTipMode=MarkerTooltipMode.OnMouseOver;
            SetReading(logger,null);
        }

        public static string LabelFor(int id) => id is >= 200001 and <= 200004 ? $"L{id-200000}" : id.ToString();
        public static Color ColorFor(int id) => id switch
        {
            200001 => Color.FromArgb(30,115,74),
            200002 => Color.FromArgb(107,65,170),
            200003 => Color.FromArgb(37,99,180),
            _ => Color.FromArgb(190,106,25)
        };

        public static GMapOverlay CreateOverlay(BuildingPipeGraph? pipeline = null)
        {
            var overlay=new GMapOverlay("zapad-loggers");
            foreach(var logger in ZapadFourLoggerSeed.GetLoggers()) overlay.Markers.Add(new LoggerMapPin(logger,pipeline));
            return overlay;
        }

        public static void UpdateReadings(GMapOverlay? overlay, DashboardData? data, DateTimeOffset? through = null, int? flaggedLoggerId = null)
        {
            if (overlay == null) return;
            foreach (var pin in overlay.Markers.OfType<LoggerMapPin>())
            {
                pin.suspectedLeak=pin.LoggerId==flaggedLoggerId;
                pin.SetReading(data?.Loggers.FirstOrDefault(l => l.Id == pin.LoggerId),
                    data?.LoggerReadings.Where(r => r.LoggerId == pin.LoggerId && (!through.HasValue || r.TakenAt <= through.Value))
                        .OrderByDescending(r => r.TakenAt).FirstOrDefault());
            }
        }

        public static void Select(GMapOverlay? overlay, LoggerMapPin? selected)
        {
            if (overlay == null) return;
            foreach (var pin in overlay.Markers.OfType<LoggerMapPin>())
                pin.ToolTipMode = pin == selected ? MarkerTooltipMode.Always : MarkerTooltipMode.OnMouseOver;
        }

        private void SetReading(PressureLogger? logger, LoggerReading? reading)
        {
            if (logger != null && logger.Latitude != 0 && logger.Longitude != 0)
            {
                var desired = new PointLatLng(logger.Latitude,logger.Longitude);
                Position = pipeline?.PositionOnPipe(desired,LoggerId) ?? desired;
            }
            ToolTipText = $"{logger?.Name ?? defaultName} ({LoggerId})\n" + (reading == null
                ? "No reading available"
                : $"Flow: {reading.FlowM3PerHour:0.###} m³/h\nPressure: {reading.PressureBar:0.###} bar\n{reading.TakenAt:dd MMM yyyy HH:mm}")
                + "\nApproximate project location";
        }

        public override void OnRender(Graphics graphics)
        {
            var anchor = new Point(LocalPosition.X - Offset.X, LocalPosition.Y - Offset.Y);
            var bounds = new Rectangle(LocalPosition, Size);
            using var pen = new Pen(suspectedLeak ? Color.Firebrick : color, 2);
            using var fill = new SolidBrush(suspectedLeak ? Color.Firebrick : color);
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
