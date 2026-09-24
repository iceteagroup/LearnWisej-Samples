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
    /// The five sections are laid out in a fixed 940 x 428 scene and mapped onto the real surface with a
    /// single Translate/Scale pair, so the composition is the same at any window size.
    /// </summary>
    public partial class CanvasPlaygroundPage : Page
    {
        private const int SceneWidth = 940;
        private const int SceneHeight = 428;

        private static readonly Color Background = Color.FromArgb(14, 21, 32);
        private static readonly Color LabelColor = Color.FromArgb(111, 143, 176);
        private static readonly Color CellStroke = Color.FromArgb(39, 55, 74);

        private static readonly string[] TransformLabels =
        {
            "Save · Translate · Restore",
            "Save · Scale · Restore",
            "Save · Rotate(45) · Restore",
            "Save · SetTransform · Restore",
        };

        private static readonly Color[] TransformColors =
        {
            Color.FromArgb(26, 134, 255),
            Color.FromArgb(31, 174, 90),
            Color.FromArgb(232, 161, 60),
            Color.FromArgb(199, 125, 255),
        };

        private static readonly int[] CellX = { 20, 250, 480, 710 };

        /// <summary>How many of the five sections the last render issued. Redraw repeats that state.</summary>
        private int sections = 5;

        /// <summary>The "Resize surface" button toggles this, which is what clears the browser bitmap.</summary>
        private bool narrowed;

        public CanvasPlaygroundPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The canvas already reports its laid-out size by Load, so the first scene is issued here.
        /// Redraw is not guaranteed to arrive before the page is shown, and a scene drawn only from
        /// Redraw leaves the surface empty until something resizes it.
        /// </summary>
        private void CanvasPlaygroundPage_Load(object sender, EventArgs e)
        {
            DrawPlayground();
        }

        private void canvasPlayground_Redraw(object sender, EventArgs e)
        {
            // The browser threw the bitmap away. Nothing is recovered: the scene is issued again.
            DrawPlayground();

            if (this.narrowed || this.sections == 5)
                this.lblStatus.Text = "Scene rebuilt from the model — nothing was recovered";
        }

        private void btnRender_Click(object sender, EventArgs e)
        {
            this.sections = 5;
            DrawPlayground();
            this.lblStatus.Text = "Scene complete · one UI update, LiveUpdate = false";
        }

        /// <summary>
        /// Changes the surface size. The browser clears the bitmap when it does that and raises Redraw,
        /// which is the failure a scene drawn once in Load never survives.
        /// </summary>
        private void btnResizeSurface_Click(object sender, EventArgs e)
        {
            this.narrowed = !this.narrowed;
            this.pnlSurface.Padding = this.narrowed
                ? new Padding(200, 4, 200, 90)
                : new Padding(20, 4, 20, 8);

            this.lblStatus.Text = "Surface cleared · waiting for Redraw";
        }

        // ── the one method that draws ───────────────────────────────────────────

        private void DrawPlayground() => DrawPlayground(this.sections);

        /// <summary>
        /// Rebuilds the whole scene from nothing. Every Canvas example below begins a new path before it
        /// draws: skip that and a later stroke drags the previous outline along with it, which is the
        /// commonest way a translated browser example goes wrong.
        /// </summary>
        private void DrawPlayground(int show)
        {
            var c = this.canvasPlayground;
            var w = c.Width;
            var h = c.Height;
            if (w <= 0 || h <= 0)
                return;

            c.ClearRect(0, 0, w, h);
            c.FillStyle = Background;
            c.FillRect(0, 0, w, h);

            var scale = Math.Min(w / (float)SceneWidth, h / (float)SceneHeight);
            if (scale <= 0f)
                return;

            var originX = (int)Math.Round((w - SceneWidth * scale) / 2f);
            var originY = (int)Math.Round((h - SceneHeight * scale) / 2f);

            // One Save/Restore pair around the whole scene transform. Everything inside works in the
            // fixed 940 x 428 scene coordinates.
            c.Save();
            try
            {
                c.Translate(originX, originY);
                c.Scale(scale, scale);

                if (show >= 1) DrawPathSection(c);
                if (show >= 2) DrawRectangleSection(c);
                if (show >= 3) DrawGradientSection(c);
                if (show >= 4) DrawTransformSection(c, scale, originX, originY);
                if (show >= 5) DrawStateSection(c);
            }
            finally
            {
                c.Restore();
            }
        }

        // ── 1: BeginPath · MoveTo · LineTo · Stroke ─────────────────────────────

        private void DrawPathSection(Canvas c)
        {
            Label(c, "BeginPath · MoveTo · LineTo · Stroke", 20, 28);

            int[,] points =
            {
                { 20, 150 }, { 60, 110 }, { 100, 132 }, { 140, 84 },
                { 180, 106 }, { 220, 68 }, { 260, 94 },
            };

            c.BeginPath();
            c.StrokeStyle = Color.FromArgb(79, 195, 247);
            c.LineWidth = 3;
            c.LineCap = CanvasLineCap.Round;
            c.LineJoin = CanvasLineJoin.Round;
            c.MoveTo(points[0, 0], points[0, 1]);
            for (var i = 1; i < points.GetLength(0); i++)
                c.LineTo(points[i, 0], points[i, 1]);

            // The polyline only appears at Stroke(): nothing is drawn while the path is being built.
            c.Stroke();
            c.LineCap = CanvasLineCap.Butt;
        }

        // ── 2: FillRect · TextAlign · TextBaseline ──────────────────────────────

        private void DrawRectangleSection(Canvas c)
        {
            Label(c, "FillRect · TextAlign · TextBaseline", 300, 28);

            c.FillStyle = Color.FromArgb(47, 111, 208);
            RoundRect(c, 300, 42, 112, 54, 3);
            c.Fill();

            c.FillStyle = Color.FromArgb(31, 174, 90);
            RoundRect(c, 300, 106, 168, 26, 3);
            c.Fill();

            c.Save();
            try
            {
                c.SetLineDash(new[] { 3, 3 });
                c.StrokeStyle = Color.FromArgb(66, 86, 109);
                c.LineWidth = 1;
                c.BeginPath();
                c.MoveTo(384, 140);
                c.LineTo(384, 156);
                c.Stroke();
            }
            finally
            {
                c.Restore();
            }

            // There is no measureText on this surface: the caption is centred with TextAlign instead.
            using (var font = new Font("Segoe UI", 15f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                c.TextFont = font;
                c.TextAlign = CanvasTextAlign.Center;
                c.TextBaseline = CanvasTextBaseline.Alphabetic;
                c.FillStyle = Color.FromArgb(214, 228, 243);
                c.FillText("Zone A · 72 %", 384, 172);
                c.TextAlign = CanvasTextAlign.Left;
            }
        }

        // ── 3: gradients are objects, not inline strings ────────────────────────

        private void DrawGradientSection(Canvas c)
        {
            Label(c, "CreateLinearGradient · CreateRadialGradient", 580, 28);

            // Each stop is an object with a "stop" (0..1) and a "color". An array of pairs compiles and
            // then fills near-black, which is the one mistake this call punishes silently.
            var linear = c.CreateLinearGradient(580, 42, 910, 42, new object[]
            {
                new { stop = 0f, color = "#1a86ff" },
                new { stop = 0.5f, color = "#7d5ae0" },
                new { stop = 1f, color = "#e05a8a" },
            });

            c.FillStyle = linear;
            RoundRect(c, 580, 42, 330, 38, 6);
            c.Fill();

            var radial = c.CreateRadialGradient(617, 120, 0f, 628, 134, 38f, new object[]
            {
                new { stop = 0f, color = "#ffeec2" },
                new { stop = 0.45f, color = "#e8a13c" },
                new { stop = 1f, color = "#7d3f0c" },
            });

            c.FillStyle = radial;
            c.BeginPath();
            c.Arc(628, 134, 38, 0f, 360f, false);       // a full circle is 0 to 360 degrees, not 2 * PI
            c.Fill();

            using (var font = new Font("Segoe UI", 13f, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                c.TextFont = font;
                c.TextAlign = CanvasTextAlign.Left;
                c.TextBaseline = CanvasTextBaseline.Alphabetic;
                c.FillStyle = Color.FromArgb(159, 192, 232);
                c.FillText("colour stops in,", 684, 128);
                c.FillText("one style object out", 684, 146);
            }
        }

        // ── 4: four transforms, each bracketed by Save and Restore ──────────────

        private void DrawTransformSection(Canvas c, float sceneScale, int originX, int originY)
        {
            for (var i = 0; i < 4; i++)
            {
                var x = CellX[i];
                const int y = 178;

                Cell(c, x, y, 210, 112, TransformLabels[i]);

                c.Save();
                try
                {
                    switch (i)
                    {
                        case 0:
                            c.Translate(x + 62, y + 62);
                            break;
                        case 1:
                            c.Translate(x + 62, y + 62);
                            c.Scale(1.45f, 1.45f);
                            break;
                        case 2:
                            c.Translate(x + 62, y + 62);
                            c.Rotate(45f);              // the browser example's rotate(Math.PI / 4)
                            break;
                        default:
                            // SetTransform REPLACES the current matrix, scene transform included, so the
                            // scene's scale and origin have to be folded into the call by hand.
                            c.SetTransform(
                                sceneScale,
                                0.28f * sceneScale,
                                -0.18f * sceneScale,
                                sceneScale,
                                originX + (int)Math.Round((x + 62) * sceneScale),
                                originY + (int)Math.Round((y + 62) * sceneScale));
                            break;
                    }

                    c.GlobalAlpha = 0.92f;
                    c.FillStyle = TransformColors[i];
                    RoundRect(c, -26, -16, 52, 32, 4);
                    c.Fill();
                    c.GlobalAlpha = 1f;
                }
                finally
                {
                    // Take this Restore out and every section drawn after it inherits the transform.
                    c.Restore();
                }
            }
        }

        // ── 5: clip, alpha, dash and shadow are all Canvas state ────────────────

        private void DrawStateSection(Canvas c)
        {
            const int y = 304;

            // Clip()
            Cell(c, CellX[0], y, 210, 108, "Clip()");
            c.Save();
            try
            {
                c.BeginPath();
                c.Arc(CellX[0] + 66, y + 58, 42, 0f, 360f, false);
                c.Clip();

                c.FillStyle = Color.FromArgb(79, 195, 247);
                for (var k = 0; k < 8; k++)
                    c.FillRect(CellX[0] + 24 + k * 12, y + 16, 7, 86);
            }
            finally
            {
                c.Restore();
            }

            c.Save();
            try
            {
                c.SetLineDash(new[] { 4, 4 });
                c.StrokeStyle = Color.FromArgb(45, 66, 86);
                c.LineWidth = 1;
                c.BeginPath();
                c.Arc(CellX[0] + 66, y + 58, 42, 0f, 360f, false);
                c.Stroke();
            }
            finally
            {
                c.Restore();
            }

            // GlobalAlpha
            Cell(c, CellX[1], y, 210, 108, "GlobalAlpha");
            c.Save();
            try
            {
                c.GlobalAlpha = 0.4f;
                c.FillStyle = Color.FromArgb(26, 134, 255);
                RoundRect(c, CellX[1] + 34, y + 34, 70, 58, 5);
                c.Fill();

                c.FillStyle = Color.FromArgb(224, 90, 138);
                RoundRect(c, CellX[1] + 78, y + 46, 70, 46, 5);
                c.Fill();
            }
            finally
            {
                c.Restore();
            }

            // SetLineDash
            Cell(c, CellX[2], y, 210, 108, "SetLineDash");
            c.Save();
            try
            {
                c.LineWidth = 3;
                c.SetLineDash(new[] { 14, 7 });
                c.StrokeStyle = Color.FromArgb(126, 224, 160);
                c.BeginPath();
                c.MoveTo(CellX[2] + 22, y + 52);
                c.LineTo(CellX[2] + 188, y + 52);
                c.Stroke();

                c.SetLineDash(new[] { 3, 6 });
                c.StrokeStyle = Color.FromArgb(255, 210, 125);
                c.BeginPath();
                c.MoveTo(CellX[2] + 22, y + 82);
                c.LineTo(CellX[2] + 188, y + 82);
                c.Stroke();
            }
            finally
            {
                c.Restore();
            }

            // Shadow
            Cell(c, CellX[3], y, 210, 108, "ShadowBlur · ShadowOffset");
            c.Save();
            try
            {
                c.ShadowColor = Color.FromArgb(128, 0, 0, 0);
                c.ShadowBlur = 10;
                c.ShadowOffsetX = 6;
                c.ShadowOffsetY = 8;
                c.FillStyle = Color.FromArgb(234, 242, 251);
                RoundRect(c, CellX[3] + 40, y + 34, 118, 50, 7);
                c.Fill();
            }
            finally
            {
                c.Restore();
            }

            using (var font = new Font("Segoe UI", 13f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                c.TextFont = font;
                c.TextAlign = CanvasTextAlign.Center;
                c.TextBaseline = CanvasTextBaseline.Alphabetic;
                c.FillStyle = Color.FromArgb(34, 64, 94);
                c.FillText("Node 12", CellX[3] + 99, y + 64);
                c.TextAlign = CanvasTextAlign.Left;
            }
        }

        // ── small helpers ───────────────────────────────────────────────────────

        /// <summary>One of the eight dark cells the transform and state sections are drawn inside.</summary>
        private void Cell(Canvas c, int x, int y, int width, int height, string caption)
        {
            c.Save();
            try
            {
                c.GlobalAlpha = 0.03f;
                c.FillStyle = Color.White;
                RoundRect(c, x, y, width, height, 9);
                c.Fill();
            }
            finally
            {
                c.Restore();
            }

            c.StrokeStyle = CellStroke;
            c.LineWidth = 1;
            RoundRect(c, x, y, width, height, 9);
            c.Stroke();

            using (var font = new Font("Consolas", 10.5f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                c.TextFont = font;
                c.TextAlign = CanvasTextAlign.Left;
                c.TextBaseline = CanvasTextBaseline.Alphabetic;
                c.FillStyle = LabelColor;
                c.FillText(caption, x + 10, y + 16);
            }
        }

        /// <summary>
        /// Text is placed with TextAlign and TextBaseline rather than measured: the browser's
        /// measureText call is not part of the documented Wisej.NET Canvas surface.
        /// </summary>
        private void Label(Canvas c, string text, int x, int y)
        {
            using (var font = new Font("Consolas", 11f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                c.TextFont = font;
                c.TextAlign = CanvasTextAlign.Left;
                c.TextBaseline = CanvasTextBaseline.Alphabetic;
                c.FillStyle = LabelColor;
                c.FillText(text, x, y);
            }
        }

        /// <summary>A rounded rectangle path. Angles are degrees on this surface, not radians.</summary>
        private static void RoundRect(Canvas c, int x, int y, int width, int height, int radius)
        {
            var r = Math.Max(0, Math.Min(radius, Math.Min(width, height) / 2));

            c.BeginPath();
            c.MoveTo(x + r, y);
            c.LineTo(x + width - r, y);
            c.Arc(x + width - r, y + r, r, 270f, 360f, false);
            c.LineTo(x + width, y + height - r);
            c.Arc(x + width - r, y + height - r, r, 0f, 90f, false);
            c.LineTo(x + r, y + height);
            c.Arc(x + r, y + height - r, r, 90f, 180f, false);
            c.LineTo(x, y + r);
            c.Arc(x + r, y + r, r, 180f, 270f, false);
            c.ClosePath();
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
            if (c.Width <= 0 || c.Height <= 0)
                return;

            c.LiveUpdate = true;
            this.lblLiveUpdate.Text = "LiveUpdate = true";
            try
            {
                for (var step = 1; step <= 5; step++)
                {
                    this.sections = step;
                    DrawPlayground(step);
                    this.lblStatus.Text = $"Rendering · DrawPlayground() section {step} of 5";
                    Thread.Sleep(450);
                }
            }
            finally
            {
                c.LiveUpdate = false;
                this.lblLiveUpdate.Text = "LiveUpdate = false";
            }

            this.sections = 5;
            this.lblStatus.Text = "Scene complete · one UI update, LiveUpdate = false";
        }
    }
}
