using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace VisualOperationsStudio.Controls
{
    /// <summary>
    /// The two drawings the operations grid puts inside a cell. Both are pure: they take a surface and
    /// a rectangle, create and dispose only what they own, and read nothing outside their arguments.
    /// CellPaint is a hot path - it runs again for every cell that scrolls into view - so there is no
    /// file or database access here, and no font or image is created per cell.
    /// </summary>
    public static class OperationsCellRenderer
    {
        /// <summary>
        /// One shared font for every painted cell. A <see cref="Font"/> is immutable and only read while
        /// drawing, so it is safe to share across sessions - unlike a <see cref="Pen"/> or a
        /// <see cref="Brush"/>, which carry state a concurrent paint would corrupt. Creating one per
        /// cell would be a per-cell allocation on the grid's hottest path.
        /// </summary>
        private static readonly Font CellFont = new Font(FontFamily.GenericSansSerif, 11.5f, FontStyle.Bold, GraphicsUnit.Pixel);

        private static readonly Color Track = Color.FromArgb(223, 229, 236);
        private static readonly Color SelectedTrack = Color.FromArgb(210, 222, 238);
        private static readonly Color Value = Color.FromArgb(70, 88, 106);

        /// <summary>Bar geometry, straight out of the walkthrough's Health cell.</summary>
        private const int BarWidth = 110;
        private const int BarHeight = 12;
        private const int BarRadius = 3;
        private const int ValueGap = 9;
        private const int ValueWidth = 34;

        /// <summary>
        /// A threshold-coloured bar with the number beside it. <paramref name="health"/> is clamped, so
        /// a value outside 0..100 cannot draw past the cell.
        /// </summary>
        public static void DrawHealthBar(Graphics graphics, Rectangle bounds, int health, Color tone, bool selected)
        {
            if (bounds.Width <= 2 || bounds.Height <= 2)
                return;

            var clamped = Math.Min(100, Math.Max(0, health));
            var barWidth = Math.Min(BarWidth, Math.Max(0, bounds.Width - ValueGap - ValueWidth));
            if (barWidth <= 2)
                return;

            var bar = new Rectangle(bounds.X, bounds.Y + (bounds.Height - BarHeight) / 2, barWidth, BarHeight);
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

            // The number stays readable: a painted cell that shows only a colour has lost the value.
            var text = clamped + "%";
            using (var label = new SolidBrush(Value))
            {
                var size = graphics.MeasureString(text, CellFont);
                var x = bar.Right + ValueGap;

                // A DrawString whose origin is at or past the right edge of the cell does not clip: it
                // throws out of the fill processor. Measure first, and skip the number when it will not
                // fit rather than losing the whole cell.
                if (x + size.Width <= bounds.Right)
                {
                    graphics.DrawString(
                        text,
                        CellFont,
                        label,
                        x,
                        bounds.Y + (bounds.Height - size.Height) / 2f);
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

            using (var pen = new Pen(selected ? Darken(tone) : tone, 2f) { LineJoin = LineJoin.Round, EndCap = LineCap.Round })
            {
                graphics.DrawLines(pen, points);
            }
        }

        /// <summary>The colour a health score maps to. Low is bad.</summary>
        public static Color ColorFor(int health) => Models.MachineStatus.ToneFor(health);

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
