using System;
using System.Collections.Generic;
using System.Drawing;

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
        private static readonly Font CellFont = new Font(FontFamily.GenericSansSerif, 8.25f, FontStyle.Bold);

        private static readonly Color Track = Color.FromArgb(238, 242, 247);
        private static readonly Color Good = Color.FromArgb(31, 157, 107);
        private static readonly Color Warning = Color.FromArgb(232, 161, 60);
        private static readonly Color Critical = Color.FromArgb(217, 58, 58);

        /// <summary>
        /// A threshold-coloured bar. <paramref name="health"/> is clamped, so a value outside 0..100
        /// cannot draw past the cell.
        /// </summary>
        public static void DrawHealthBar(Graphics graphics, Rectangle bounds, int health, bool selected)
        {
            if (bounds.Width <= 2 || bounds.Height <= 2)
                return;

            var clamped = Math.Min(100, Math.Max(0, health));
            var filled = (int)Math.Round(bounds.Width * clamped / 100.0);

            using (var track = new SolidBrush(selected ? Color.FromArgb(215, 225, 236) : Track))
            using (var fill = new SolidBrush(ColorFor(clamped)))
            {
                graphics.FillRectangle(track, bounds);

                if (filled > 0)
                    graphics.FillRectangle(fill, bounds.X, bounds.Y, filled, bounds.Height);
            }

            // The number stays readable: a painted cell that shows only a colour has lost the value.
            var text = clamped + " %";
            using (var label = new SolidBrush(Color.FromArgb(13, 27, 42)))
            {
                var size = graphics.MeasureString(text, CellFont);
                graphics.DrawString(
                    text,
                    CellFont,
                    label,
                    bounds.Right - size.Width - 4,
                    bounds.Y + (bounds.Height - size.Height) / 2f);
            }
        }

        /// <summary>
        /// A polyline over the readings, normalised to the rectangle. Fewer than two points is not a
        /// line, so the cell is left empty instead of throwing.
        /// </summary>
        public static void DrawSparkline(Graphics graphics, Rectangle bounds, IReadOnlyList<double> readings, bool selected)
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

            using (var pen = new Pen(selected ? Color.FromArgb(13, 27, 42) : Color.FromArgb(21, 101, 216), 1.6f))
            {
                graphics.DrawLines(pen, points);
            }
        }

        /// <summary>The colour a health score maps to. Low is bad.</summary>
        public static Color ColorFor(int health) =>
            health < Models.MachineStatus.CriticalBelow ? Critical :
            health < Models.MachineStatus.WarningBelow ? Warning : Good;
    }
}
