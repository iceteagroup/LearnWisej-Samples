using System;
using System.Drawing;
using System.Threading;
using Wisej.Web;

namespace VisualOperationsStudio
{
    /// <summary>
    /// A set of browser Canvas 2D examples ported to the documented <see cref="Wisej.Web.Canvas"/> API.
    /// Everything is drawn by <see cref="DrawPlayground"/>, which the Redraw event calls, so the scene
    /// comes back after a resize: the browser keeps pixels, and pixels do not survive.
    /// </summary>
    public partial class CanvasPlaygroundPage : Page
    {
        private readonly VisualOperationsPage operations;

        public CanvasPlaygroundPage(VisualOperationsPage operations)
        {
            InitializeComponent();

            this.operations = operations;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Application.MainPage = this.operations;
        }

        private void canvasPlayground_Redraw(object sender, EventArgs e)
        {
            DrawPlayground();
        }

        // ── the one method that draws ───────────────────────────────────────────

        /// <summary>
        /// Rebuilds the whole scene from nothing. Every Canvas example below begins a new path before it
        /// draws: skip that and a later stroke drags the previous outline along with it, which is the
        /// commonest way a translated browser example goes wrong.
        /// </summary>
        private void DrawPlayground()
        {
            var c = this.canvasPlayground;
            var w = c.Width;
            var h = c.Height;
            if (w <= 0 || h <= 0)
                return;

            c.ClearRect(0, 0, w, h);

            // Four columns, two rows of sections.
            var column = Math.Max(160, w / 4);
            var row = Math.Max(120, (h - 40) / 2);

            DrawPathsAndFills(c, 0, 0, column, row);
            DrawGradients(c, column, 0, column, row);
            DrawTransforms(c, column * 2, 0, column * 2, row);
            DrawState(c, 0, row, w, row);
        }

        // ── section 1: paths, fills and a placed caption ────────────────────────

        private void DrawPathsAndFills(Canvas c, int x, int y, int width, int height)
        {
            SectionTitle(c, "Paths and fills", x, y);

            // A stroked polyline. BeginPath first, always.
            c.BeginPath();
            c.StrokeStyle = Color.FromArgb(21, 101, 216);
            c.LineWidth = 3;
            c.MoveTo(x + 24, y + height - 40);
            c.LineTo(x + 24 + width / 5, y + 60);
            c.LineTo(x + 24 + width / 5 * 2, y + height - 80);
            c.LineTo(x + 24 + width / 5 * 3, y + 90);
            c.Stroke();

            c.FillStyle = Color.FromArgb(31, 157, 107);
            c.FillRect(x + 24, y + height - 34, 70, 22);

            c.FillStyle = Color.FromArgb(232, 161, 60);
            c.FillRect(x + 104, y + height - 34, 70, 22);

            // The browser's text-measuring call is not part of the documented Wisej.NET surface, so the
            // caption is placed with the alignment and baseline properties instead of measured.
            Caption(c, "BeginPath - MoveTo - LineTo - Stroke - FillRect", x + 24, y + height - 8);
        }

        // ── section 2: gradients are objects, not inline strings ────────────────

        private void DrawGradients(Canvas c, int x, int y, int width, int height)
        {
            SectionTitle(c, "Gradients", x, y);

            var bar = new Rectangle(x + 24, y + 50, width - 60, 46);
            var linear = c.CreateLinearGradient(
                bar.Left, bar.Top, bar.Right, bar.Top,
                new object[]
                {
                    // Each stop is an object with a "stop" (0..1) and a "color" the client theme can
                    // resolve - a hex string or a theme colour name, not a System.Drawing.Color.
                    new { stop = 0f, color = "#1f9d6b" },
                    new { stop = 0.6f, color = "#e8a13c" },
                    new { stop = 1f, color = "#d93a3a" },
                });

            c.FillStyle = linear;
            c.FillRect(bar.X, bar.Y, bar.Width, bar.Height);

            var radius = Math.Min(70, height / 3);
            var cx = x + 24 + radius;
            var cy = y + height - radius - 40;
            var radial = c.CreateRadialGradient(
                cx - radius / 3, cy - radius / 3, radius / 8f, cx, cy, radius,
                new object[]
                {
                    new { stop = 0f, color = "#b4d6ff" },
                    new { stop = 1f, color = "#1565d8" },
                });

            c.FillStyle = radial;
            c.BeginPath();
            c.Arc(cx, cy, radius, 0f, 360f, false);
            c.Fill();

            Caption(c, "CreateLinearGradient / CreateRadialGradient", x + 24, y + height - 8);
        }

        // ── section 3: every transform inside Save/Restore ──────────────────────

        private void DrawTransforms(Canvas c, int x, int y, int width, int height)
        {
            SectionTitle(c, "Transforms - each bracketed by Save/Restore", x, y);

            var box = Math.Min(54, height / 4);
            var baseY = y + 60;
            var step = Math.Max(120, width / 4);

            // Translate
            c.Save();
            c.Translate(x + 24, baseY);
            c.FillStyle = Color.FromArgb(21, 101, 216);
            c.FillRect(0, 0, box, box);
            c.Restore();

            // Scale
            c.Save();
            c.Translate(x + 24 + step, baseY);
            c.Scale(1.4f, 0.7f);
            c.FillStyle = Color.FromArgb(31, 157, 107);
            c.FillRect(0, 0, box, box);
            c.Restore();

            // Rotate: the browser example rotates by Math.PI / 4 radians.
            // Wisej.NET Canvas.Rotate takes degrees, so Math.PI / 4 == 45.
            c.Save();
            c.Translate(x + 24 + step * 2 + box / 2, baseY + box / 2);
            c.Rotate(45f);
            c.FillStyle = Color.FromArgb(232, 161, 60);
            c.FillRect(-box / 2, -box / 2, box, box);
            c.Restore();

            // SetTransform: scale, skew and translate in one call.
            c.Save();
            c.SetTransform(1f, 0.32f, 0f, 1f, x + 24 + step * 3, baseY);
            c.FillStyle = Color.FromArgb(125, 90, 224);
            c.FillRect(0, 0, box, box);
            c.Restore();

            var labelY = baseY + box + 18;
            Caption(c, "Translate", x + 24, labelY);
            Caption(c, "Scale", x + 24 + step, labelY);
            Caption(c, "Rotate(45) = Math.PI / 4", x + 24 + step * 2, labelY);
            Caption(c, "SetTransform", x + 24 + step * 3, labelY);
        }

        // ── section 4: clip, alpha, dash and shadow are all Canvas state ────────

        private void DrawState(Canvas c, int x, int y, int width, int height)
        {
            SectionTitle(c, "State - clip, alpha, dash, shadow", x, y);

            var top = y + 54;
            var size = Math.Min(110, height - 90);
            var column = Math.Max(180, width / 4);

            // A clipping region, inside its own save/restore pair.
            c.Save();
            c.BeginPath();
            c.Arc(x + 24 + size / 2, top + size / 2, size / 2, 0f, 360f, false);
            c.Clip();
            c.FillStyle = Color.FromArgb(21, 101, 216);
            c.FillRect(x + 24, top, size, size);
            c.FillStyle = Color.FromArgb(255, 255, 255);
            c.FillRect(x + 24, top + size / 2, size, size / 2);
            c.Restore();

            // Reduced transparency.
            c.Save();
            c.GlobalAlpha = 0.35f;
            c.FillStyle = Color.FromArgb(217, 58, 58);
            c.FillRect(x + 24 + column, top, size, size);
            c.FillStyle = Color.FromArgb(31, 157, 107);
            c.FillRect(x + 24 + column + size / 3, top + size / 3, size, size);
            c.Restore();

            // A dashed rule.
            c.Save();
            c.SetLineDash(new[] { 12, 8 });
            c.StrokeStyle = Color.FromArgb(90, 107, 125);
            c.LineWidth = 2;
            c.BeginPath();
            c.MoveTo(x + 24 + column * 2, top + size / 2);
            c.LineTo(x + 24 + column * 2 + size, top + size / 2);
            c.Stroke();
            c.Restore();

            // A shadowed card.
            c.Save();
            c.ShadowColor = Color.FromArgb(120, 13, 27, 42);
            c.ShadowBlur = 14;
            c.ShadowOffsetX = 4;
            c.ShadowOffsetY = 6;
            c.FillStyle = Color.White;
            c.FillRect(x + 24 + column * 3, top + 10, size + 30, size - 20);
            c.Restore();

            var captionY = top + size + 20;
            Caption(c, "Clip()", x + 24, captionY);
            Caption(c, "GlobalAlpha", x + 24 + column, captionY);
            Caption(c, "SetLineDash", x + 24 + column * 2, captionY);
            Caption(c, "ShadowColor / ShadowBlur / ShadowOffset", x + 24 + column * 3, captionY);
        }

        /// <summary>
        /// Text is placed with TextAlign and TextBaseline rather than measured: the browser's
        /// measureText call is not part of the documented Wisej.NET Canvas surface.
        /// </summary>
        private void Caption(Canvas c, string text, int x, int y)
        {
            c.TextFont = new Font("default", 10F, FontStyle.Regular);
            c.TextAlign = CanvasTextAlign.Left;
            c.TextBaseline = CanvasTextBaseline.Top;
            c.FillStyle = Color.FromArgb(90, 107, 125);
            c.FillText(text, x, y);
        }

        private void SectionTitle(Canvas c, string text, int x, int y)
        {
            c.TextFont = new Font("default", 11F, FontStyle.Bold);
            c.TextAlign = CanvasTextAlign.Left;
            c.TextBaseline = CanvasTextBaseline.Top;
            c.FillStyle = Color.FromArgb(13, 27, 42);
            c.FillText(text, x + 24, y + 18);
        }

        // ── the one place LiveUpdate is turned on ───────────────────────────────

        /// <summary>
        /// LiveUpdate stays off for the main render, so the five sections arrive in one update. It is
        /// switched on here only to let a long server-side operation reveal its progress as it works -
        /// it is not a way to animate, and a request per frame is the wrong architecture.
        /// </summary>
        private void btnProgressive_Click(object sender, EventArgs e)
        {
            var c = this.canvasPlayground;
            var w = c.Width;
            var h = c.Height;
            if (w <= 0 || h <= 0)
                return;

            c.LiveUpdate = true;
            try
            {
                DrawPlayground();

                var y = h - 70;
                for (var step = 0; step < 8; step++)
                {
                    c.FillStyle = Color.FromArgb(21, 101, 216);
                    c.FillRect(24 + step * 44, y, 34, 22);
                    Thread.Sleep(140);
                }
            }
            finally
            {
                c.LiveUpdate = false;
            }

            this.lblStatus.Text =
                "Progressive draw: LiveUpdate on sends each step as its own message (8 here). " +
                "The main render sends the whole scene in one.";
        }
    }
}
