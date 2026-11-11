using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace WaterNetworkMonitoring
{
    internal sealed record ChartPoint(DateTimeOffset Time, double Value);
    internal sealed record ChartSeries(string Name, Color Color, IReadOnlyList<ChartPoint> Points, TimeSpan MaximumGap);

    public sealed class DashboardChart : Control
    {
        private IReadOnlyList<ChartSeries> series = Array.Empty<ChartSeries>();
        private string heading = "Readings", unit = "", subtitle = "";
        public DashboardChart()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
            ForeColor = Color.FromArgb(36, 45, 56);
            SetStyle(ControlStyles.ResizeRedraw, true);
            Size = new Size(500, 250);
        }

        [DefaultValue("Readings")]
        public string Heading { get => heading; set { heading = value; Invalidate(); } }
        [DefaultValue("")]
        public string Unit { get => unit; set { unit = value; Invalidate(); } }
        [DefaultValue("")]
        public string Subtitle { get => subtitle; set { subtitle = value; Invalidate(); } }
        [DefaultValue(true)]
        public bool StartAtZero { get; set; } = true;
        internal void SetSeries(params ChartSeries[] values) { series = values; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float s = DeviceDpi / 96f;
            using var titleFont = new Font(Font, FontStyle.Bold);
            using var smallFont = new Font(Font.FontFamily, Math.Max(7, Font.Size - 1));
            using var text = new SolidBrush(ForeColor);
            using var muted = new SolidBrush(Color.FromArgb(100, 110, 122));
            using var border = new Pen(Color.FromArgb(220, 225, 231));
            using var grid = new Pen(Color.FromArgb(235, 238, 242));
            g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
            g.DrawString(Heading, titleFont, text, 16 * s, 12 * s);
            g.DrawString(Subtitle, smallFont, muted, 16 * s, 36 * s);
            var values = series.SelectMany(v => v.Points).Where(p => double.IsFinite(p.Value)).ToArray();
            if (values.Length == 0)
            {
                using var centered = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("No readings to display", Font, muted, new RectangleF(0, 60 * s, Width, Height - 60 * s), centered);
                return;
            }
            var plot = new RectangleF(58 * s, 75 * s, Width - 78 * s, Height - 126 * s);
            if (plot.Width < 30 || plot.Height < 20) return;
            double max = values.Max(v => v.Value), min = StartAtZero ? 0 : values.Min(v => v.Value);
            double padding = Math.Max((max - min) * .12, StartAtZero ? .1 : .05);
            max += padding;
            if (!StartAtZero) min -= padding;
            if (max <= min) max = min + 1;
            var first = values.Min(p => p.Time);
            var last = values.Max(p => p.Time);
            double duration = Math.Max(1, (last - first).TotalSeconds);
            float X(DateTimeOffset time) => plot.Left + (float)((time - first).TotalSeconds / duration) * plot.Width;
            float Y(double value) => plot.Bottom - (float)((value - min) / (max - min)) * plot.Height;
            using var right = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
            for (int i = 0; i <= 4; i++)
            {
                float y = plot.Top + plot.Height * i / 4;
                g.DrawLine(grid, plot.Left, y, plot.Right, y);
                g.DrawString((max - (max - min) * i / 4).ToString("0.##", CultureInfo.InvariantCulture), smallFont, muted,
                    new RectangleF(2, y - 9 * s, plot.Left - 8 * s, 18 * s), right);
            }
            using var centeredLabel = new StringFormat { Alignment = StringAlignment.Center };
            for (int i = 0; i < 4; i++)
            {
                var time = first.AddSeconds(duration * i / 3);
                g.DrawString(time.ToString("dd MMM"), smallFont, muted, X(time), plot.Bottom + 7 * s, centeredLabel);
            }
            g.DrawString(Unit, smallFont, muted, plot.Left, plot.Top - 19 * s);
            foreach (var line in series)
            {
                using var pen = new Pen(line.Color, 2 * s) { LineJoin = LineJoin.Round };
                using var fill = new SolidBrush(line.Color);
                ChartPoint? previous = null;
                foreach (var point in line.Points.OrderBy(p => p.Time))
                {
                    if (!double.IsFinite(point.Value)) { previous = null; continue; }
                    if (previous != null && point.Time - previous.Time <= line.MaximumGap)
                        g.DrawLine(pen, X(previous.Time), Y(previous.Value), X(point.Time), Y(point.Value));
                    g.FillEllipse(fill, X(point.Time) - 2 * s, Y(point.Value) - 2 * s, 4 * s, 4 * s);
                    previous = point;
                }
            }
            float legendX = plot.Left;
            foreach (var line in series)
            {
                using var pen = new Pen(line.Color, 3 * s);
                float y = Height - 17 * s;
                g.DrawLine(pen, legendX, y, legendX + 15 * s, y);
                g.DrawString(line.Name, smallFont, muted, legendX + 20 * s, y - 8 * s);
                legendX += 34 * s + g.MeasureString(line.Name, smallFont).Width;
            }
        }
    }
}
