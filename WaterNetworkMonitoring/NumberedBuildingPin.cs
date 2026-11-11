using GMap.NET.WindowsForms;
using System.Drawing.Drawing2D;

namespace WaterNetworkMonitoring
{
    internal sealed class NumberedBuildingPin : GMapMarker
    {
        private readonly bool showNumber;
        private readonly bool selected;
        private readonly string number;
        private readonly bool compact;

        public NumberedBuildingPin(BuildingMapData.Building building, bool showNumber, bool selected, bool compact = false)
            : base(building.Position)
        {
            this.showNumber = showNumber || selected;
            this.selected = selected;
            this.compact = compact;
            number = building.Number.ToString();
            Tag = building;
            ToolTipText = number;
            ToolTipMode = MarkerTooltipMode.OnMouseOver;
            Size = this.showNumber ? new Size(Math.Max(28, number.Length * 8 + 12), 30) : new Size(8, 11);
            Offset = new Point(-Size.Width / 2, -Size.Height);
            if (compact && this.showNumber)
            {
                Size = selected ? new Size(24, 24) : new Size(20, 20);
                Offset = new Point(-Size.Width / 2, -Size.Height / 2);
            }
            IsHitTestVisible = true;
        }

        public override void OnRender(Graphics graphics)
        {
            var state = graphics.Save();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            if (compact && showNumber)
            {
                using var background = new SolidBrush(selected ? Color.FromArgb(195, 100, 35) : Color.White);
                using var outline = new Pen(selected ? Color.White : Color.FromArgb(37, 99, 180), 1.2f);
                using var ink = new SolidBrush(selected ? Color.White : Color.FromArgb(28, 79, 141));
                using var font = new Font(SystemFonts.DefaultFont.FontFamily, selected ? 8F : 7.5F, FontStyle.Regular);
                using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                var circle = new Rectangle(LocalPosition, Size);
                graphics.FillEllipse(background, circle);
                graphics.DrawEllipse(outline, circle);
                graphics.DrawString(number, font, ink, circle, format);
                graphics.Restore(state);
                return;
            }
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
