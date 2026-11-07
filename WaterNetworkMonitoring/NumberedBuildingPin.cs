using GMap.NET.WindowsForms;
using System.Drawing.Drawing2D;

namespace WaterNetworkMonitoring
{
    internal sealed class NumberedBuildingPin : GMapMarker
    {
        private readonly bool showNumber;
        private readonly bool selected;
        private readonly string number;

        public NumberedBuildingPin(BuildingMapData.Building building, bool showNumber, bool selected)
            : base(building.Position)
        {
            this.showNumber = showNumber || selected;
            this.selected = selected;
            number = building.Number.ToString();
            Tag = building;
            ToolTipText = $"Building #{number}\n{string.Join(", ", building.Zones)}";
            if (!string.IsNullOrWhiteSpace(building.Name)) ToolTipText += "\n" + building.Name;
            if (!string.IsNullOrWhiteSpace(building.Address)) ToolTipText += "\n" + building.Address;
            ToolTipMode = MarkerTooltipMode.OnMouseOver;
            Size = this.showNumber ? new Size(Math.Max(28, number.Length * 8 + 12), 30) : new Size(8, 11);
            Offset = new Point(-Size.Width / 2, -Size.Height);
            IsHitTestVisible = true;
        }

        public override void OnRender(Graphics graphics)
        {
            var state = graphics.Save();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var fill = new SolidBrush(selected ? Color.FromArgb(220, 88, 30) : Color.FromArgb(28, 103, 191));
            using var border = new Pen(Color.White, showNumber ? 1.5f : .7f);
            int x = LocalPosition.X, y = LocalPosition.Y, w = Size.Width, h = Size.Height;
            using var path = new GraphicsPath();
            if (showNumber)
            {
                path.AddArc(x, y, 10, 10, 180, 90);
                path.AddArc(x + w - 10, y, 10, 10, 270, 90);
                path.AddArc(x + w - 10, y + h - 17, 10, 10, 0, 90);
                path.AddLine(x + w / 2 + 4, y + h - 7, x + w / 2, y + h);
                path.AddLine(x + w / 2, y + h, x + w / 2 - 4, y + h - 7);
                path.AddArc(x, y + h - 17, 10, 10, 90, 90);
            }
            else
            {
                path.AddArc(x, y, w, w, 150, 240);
                path.AddLine(x + w, y + 6, x + w / 2, y + h);
            }
            path.CloseFigure();
            graphics.FillPath(fill, path);
            graphics.DrawPath(border, path);
            if (showNumber)
            {
                using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                graphics.DrawString(number, SystemFonts.DefaultFont, Brushes.White,
                    new RectangleF(x, y, w, h - 7), format);
            }
            graphics.Restore(state);
        }
    }
}
