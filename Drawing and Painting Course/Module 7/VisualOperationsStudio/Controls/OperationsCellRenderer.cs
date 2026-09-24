using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace VisualOperationsStudio.Controls
{
    /// <summary>
    /// The two drawings the capstone grid puts inside a cell. Both are pure: they take a surface, a
    /// rectangle and a colour, create and dispose only what they own, and read nothing outside their
    /// arguments. CellPaint is a hot path - it runs again for every cell that scrolls into view, on
    /// request threads - so there is no file or database access here, and no font or image is created
    /// per cell.
    /// </summary>
    public static class OperationsCellRenderer
    {
        private static readonly Color Track = Color.FromArgb(238, 242, 247);
        private static readonly Color SelectedTrack = Color.FromArgb(214, 226, 240);

        private const int BarHeight = 13;
        private const int BarRadius = 3;

        /// <summary>
        /// The load bar: an inactive track and an active rectangle whose width is proportional to the
        /// clamped value. <paramref name="load"/> is clamped, so a value outside 0..100 cannot draw past
        /// the cell.
        /// </summary>
        public static void DrawLoadBar(Graphics graphics, Rectangle bounds, int load, Color tone, bool selected)
        {
            if (bounds.Width <= 2 || bounds.Height <= 2)
                return;

            var clamped = Math.Min(100, Math.Max(0, load));
            var bar = new Rectangle(bounds.X, bounds.Y + (bounds.Height - BarHeight) / 2, bounds.Width, BarHeight);
            var filled = (int)Math.Round(bar.Width * clamped / 100.0);

            using (var path = RoundedRectangle(bar, BarRadius))
            using (var track = new SolidBrush(selected ? SelectedTrack : Track))
            using (var fill = new SolidBrush(tone))
            {
                graphics.FillPath(track, path);

                if (filled > 0)
                {
                    var state = graphics.BeginContainer();
                    try
                    {
                        graphics.SetClip(path);
                        graphics.FillRectangle(fill, bar.X, bar.Y, filled, bar.Height);
                    }
                    finally
                    {
                        graphics.EndContainer(state);
                    }
                }
            }
        }

        /// <summary>
        /// A polyline over the readings, normalised to the rectangle. Fewer than two points is not a
        /// line, so the cell is left empty instead of throwing.
        /// </summary>
        public static void DrawSparkline(Graphics graphics, Rectangle bounds, IReadOnlyList<double> readings, Color tone, bool selected)
        {
            if (bounds.Width <= 2 || bounds.Height <= 2)
                return;

            if (readings == null || readings.Count < 2)
                return;

            var low = double.MaxValue;
            var high = double.MinValue;
            for (var i = 0; i < readings.Count; i++)
            {
                if (readings[i] < low) low = readings[i];
                if (readings[i] > high) high = readings[i];
            }

            var span = high - low;
            if (span <= 0)
                span = 1;

            var points = new PointF[readings.Count];
            var step = (float)bounds.Width / (readings.Count - 1);

            for (var i = 0; i < readings.Count; i++)
            {
                var normalized = (float)((readings[i] - low) / span);
                points[i] = new PointF(
                    bounds.X + i * step,
                    bounds.Bottom - normalized * bounds.Height);
            }

            using (var pen = new Pen(selected ? Darken(tone) : tone, 1.8f) { LineJoin = LineJoin.Round, EndCap = LineCap.Round })
            {
                graphics.DrawLines(pen, points);
            }
        }

        private static Color Darken(Color color) =>
            Color.FromArgb((int)(color.R * 0.72), (int)(color.G * 0.72), (int)(color.B * 0.72));

        private static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
        {
            var d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d - 1, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d - 1, bounds.Bottom - d - 1, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d - 1, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
