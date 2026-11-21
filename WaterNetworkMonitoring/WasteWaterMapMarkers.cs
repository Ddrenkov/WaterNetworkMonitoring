using GMap.NET;
using GMap.NET.WindowsForms;

namespace WaterNetworkMonitoring
{
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
