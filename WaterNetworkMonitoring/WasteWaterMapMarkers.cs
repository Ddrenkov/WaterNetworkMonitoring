using GMap.NET;
using GMap.NET.WindowsForms;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring
{
    internal sealed class WasteWaterLoggerPin : GMapMarker
    {
        private readonly string label;
        public int LoggerId { get; }
        private readonly Color color;
        public WasteWaterLoggerPin(PressureLogger logger, WasteWaterReading? reading) : base(new PointLatLng(logger.Latitude,logger.Longitude))
        {
            LoggerId = logger.Id;
            label = logger.Name;
            color = logger.Id switch { 300001 => Color.FromArgb(35,112,91), 300002 => Color.FromArgb(110,70,150), _ => Color.FromArgb(40,87,155) };
            Size = new Size(54,24);
            Offset = new Point(-27,-36);
            IsHitTestVisible = true;
            ToolTipMode = MarkerTooltipMode.OnMouseOver;
            ToolTipText = $"Wastewater logger {logger.Name} ({logger.Id})\n" + (reading == null ? "No readings recorded"
                : $"Flow: {reading.FlowM3PerHour:0.###} m³/h\nWater level: {reading.WaterLevelMetres:0.###} m\nRain during last 2 h: {(reading.IsRaining ? "Yes" : "No")}\n{reading.TakenAt:dd MMM yyyy HH:mm}");
        }
        public override void OnRender(Graphics graphics)
        {
            var anchor = new Point(LocalPosition.X-Offset.X,LocalPosition.Y-Offset.Y);
            using var fill = new SolidBrush(color);
            using var pen = new Pen(color,2);
            graphics.DrawLine(pen,anchor,new Point(LocalPosition.X+Size.Width/2,LocalPosition.Y+Size.Height));
            graphics.FillEllipse(fill,anchor.X-4,anchor.Y-4,8,8);
            var bounds = new Rectangle(LocalPosition,Size);
            graphics.FillRectangle(fill,bounds);
            graphics.DrawRectangle(Pens.White,bounds);
            using var font = new Font("Segoe UI",9F);
            using var format = new StringFormat { Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Center };
            graphics.DrawString(label,font,Brushes.White,bounds,format);
        }
    }

    internal sealed class WasteWaterJunctionPin : GMapMarker
    {
        public WasteWaterJunctionPin(PointLatLng position) : base(position)
        {
            Size = new Size(6,6); Offset = new Point(-3,-3); IsHitTestVisible = false;
        }
        public override void OnRender(Graphics graphics)
        {
            graphics.FillEllipse(Brushes.White,LocalPosition.X,LocalPosition.Y,6,6);
            using var pen = new Pen(Color.FromArgb(111,73,43),1.2f);
            graphics.DrawEllipse(pen,LocalPosition.X,LocalPosition.Y,6,6);
        }
    }
}
