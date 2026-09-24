using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace VisualOperationsStudio.Renderers
{
    /// <summary>
    /// The reading bar as the surfaces that own a <see cref="Graphics"/> draw it: a rounded track, the
    /// active part of it in the surface's colour, then the number and the "% of rated load" caption.
    /// Surface 2 calls this with the <c>Graphics</c> that came from <c>PaintEventArgs</c>, surface 4
    /// with one obtained from <c>Graphics.FromImage</c> - the same picture either way, which is the
    /// point the module makes.
    /// </summary>
    public static class ReadingBar
    {
        public const int BarHeight = 26;
        public const int ValueGap = 9;
        public const int ValueHeight = 26;

        /// <summary>The height this renderer needs: the bar, the gap under it and the value line.</summary>
        public const int TotalHeight = BarHeight + ValueGap + ValueHeight;

        private static readonly Color TrackFill = Color.FromArgb(238, 242, 247);
        private static readonly Color TrackBorder = Color.FromArgb(221, 229, 238);
        private static readonly Color ValueColor = Color.FromArgb(13, 27, 42);
        private static readonly Color CaptionColor = Color.FromArgb(123, 139, 156);

        /// <summary>
        /// Draws the bar into <paramref name="area"/>. Nothing is drawn - and nothing throws - when the
        /// area has not been laid out yet.
        /// </summary>
        public static void Draw(Graphics g, Rectangle area, double reading, Color accent)
        {
            if (g == null || area.Width <= 0 || area.Height <= 0)
                return;

            var percent = Math.Min(100.0, Math.Max(0.0, reading));
            var track = new Rectangle(area.X, area.Y, area.Width, Math.Min(BarHeight, area.Height));

            using (var path = RoundedRectangle(track, 5))
            using (var trackBrush = new SolidBrush(TrackFill))
            using (var borderPen = new Pen(TrackBorder, 1f))
            using (var fillBrush = new SolidBrush(accent))
            {
                g.FillPath(trackBrush, path);

                var filled = (int)Math.Round(track.Width * percent / 100.0);
                if (filled > 0)
                {
                    // The fill is clipped to the rounded track instead of being rounded itself, so a
                    // one-percent reading still reads as a sliver and not as a pill.
                    var state = g.BeginContainer();
                    try
                    {
                        g.SetClip(path);
                        g.FillRectangle(fillBrush, track.X, track.Y, filled, track.Height);
                    }
                    finally
                    {
                        g.EndContainer(state);
                    }
                }

                g.DrawPath(borderPen, path);
            }

            DrawValue(g, area, percent, blank: false);
        }

        /// <summary>The same layout with no bar and no number: what a cleared surface looks like.</summary>
        public static void DrawBlank(Graphics g, Rectangle area, string caption)
        {
            if (g == null || area.Width <= 0 || area.Height <= 0)
                return;

            var track = new Rectangle(area.X, area.Y, area.Width, Math.Min(BarHeight, area.Height));
            using (var path = RoundedRectangle(track, 5))
            using (var dashed = new Pen(Color.FromArgb(185, 197, 210), 1.5f) { DashStyle = DashStyle.Dash })
            {
                g.DrawPath(dashed, path);
            }

            DrawValue(g, area, 0, blank: true, caption);
        }

        private static void DrawValue(Graphics g, Rectangle area, double percent, bool blank, string caption = "% of rated load")
        {
            var top = area.Y + BarHeight + ValueGap;
            if (top + 6 > area.Bottom)
                return;

            using (var valueFont = new Font("Segoe UI", 22f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var captionFont = new Font("Segoe UI", 13f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var valueBrush = new SolidBrush(blank ? Color.FromArgb(154, 168, 182) : ValueColor))
            using (var captionBrush = new SolidBrush(CaptionColor))
            {
                var text = blank ? "—" : percent.ToString("0");

                // Measured, then placed at an explicit origin: a DrawString into a rectangle whose
                // height is near the line height clips the glyphs.
                var size = g.MeasureString(text, valueFont);
                if (size.Width > area.Width)
                    return;

                g.DrawString(text, valueFont, valueBrush, area.X, top);

                // A DrawString whose origin sits at or past the right edge of the surface does not
                // clip: it throws DivideByZeroException out of the fill processor. The caption is only
                // drawn when it fits.
                var captionX = area.X + size.Width + 8f;
                var captionSize = g.MeasureString(caption, captionFont);
                if (captionX + captionSize.Width <= area.Right)
                    g.DrawString(caption, captionFont, captionBrush, captionX, top + 8f);
            }
        }

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
