using System.Drawing;
using Wisej.Web;

namespace VisualOperationsStudio.Controls
{
    /// <summary>
    /// The three window glyphs that sit at the right of the application bar: minimise, maximise and
    /// close. They are painted rather than pasted in as images so the bar needs no image assets.
    /// Like every painted control in this course it derives from <see cref="Panel"/>, subscribes to
    /// <see cref="Control.Paint"/> (an override alone never runs) and invalidates on resize.
    /// </summary>
    public class WindowGlyphs : Panel
    {
        private const int Box = 13;
        private const int Gap = 16;

        public WindowGlyphs()
        {
            ForeColor = Color.White;
            Paint += (sender, e) => PaintGlyphs(e);
            Resize += (sender, e) => Invalidate();
        }

        /// <summary>Distance kept between the close glyph and the right edge of the bar.</summary>
        public int Inset { get; set; } = 18;

        /// <summary>The width this control needs for the three glyphs plus <see cref="Inset"/>.</summary>
        public int RequiredWidth => Inset + Box * 3 + Gap * 2;

        private void PaintGlyphs(PaintEventArgs e)
        {
            var area = e.ClipRectangle;
            if (area.Width <= 0 || area.Height <= 0)
                return;

            var y = area.Y + (area.Height - Box) / 2f;
            var x = area.Right - Inset - Box;

            using (var pen = new Pen(ForeColor, 1.4f))
            {
                // close
                e.Graphics.DrawLine(pen, x + 1.5f, y + 1.5f, x + 10.5f, y + 10.5f);
                e.Graphics.DrawLine(pen, x + 10.5f, y + 1.5f, x + 1.5f, y + 10.5f);

                // maximise
                x -= Box + Gap;
                e.Graphics.DrawRectangle(pen, x + 1f, y + 1f, 10f, 10f);

                // minimise
                x -= Box + Gap;
                e.Graphics.DrawLine(pen, x + 1f, y + 10f, x + 12f, y + 10f);
            }
        }
    }
}
