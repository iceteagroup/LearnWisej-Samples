using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using VisualOperationsStudio.Models;

namespace VisualOperationsStudio.Renderers
{
    /// <summary>
    /// Turns a <see cref="TopologyScene"/> into PNG bytes with no Wisej control and no session involved:
    /// a <see cref="Bitmap"/>, a <see cref="Graphics"/> over it, and <c>System.Drawing</c>. There is
    /// deliberately no <c>using Wisej.Web;</c> in this file - that is what makes it runnable from a
    /// background job, a scheduled report or a Linux container with no graphics subsystem underneath.
    /// The picture is laid out in a fixed 560 x 334 design space and scaled to the requested size, so
    /// the file looks the same whether it is 600 px wide or 2000.
    /// </summary>
    public class TopologyImageRenderer
    {
        /// <summary>A typo must not be able to ask for a 4000 x 3000 preview: that is ~48 MB before a single shape is drawn.</summary>
        public const int MinimumSide = 200;

        public const int MaximumSide = 2000;

        private const float DesignWidth = 560f;
        private const float DesignHeight = 334f;

        private static readonly PrivateFontCollection PrivateFonts = new PrivateFontCollection();
        private static readonly object FontLock = new object();
        private static bool privateFontsLoaded;

        private static readonly Color Paper = Color.White;
        private static readonly Color Frame = Color.FromArgb(219, 227, 236);
        private static readonly Color EdgeColor = Color.FromArgb(159, 178, 198);
        private static readonly Color NodeBody = Color.FromArgb(246, 249, 253);
        private static readonly Color Ink = Color.FromArgb(31, 45, 58);
        private static readonly Color Muted = Color.FromArgb(90, 107, 125);
        private static readonly Color Footnote = Color.FromArgb(138, 155, 173);

        /// <summary>The family this renderer actually used, so the output stays explainable.</summary>
        public string ResolvedFontFamily { get; private set; } = "(not resolved yet)";

        /// <summary>How long the last <see cref="Render"/> call took, in milliseconds.</summary>
        public double LastRenderMilliseconds { get; private set; }

        /// <summary>How many nodes and edges the last call actually drew.</summary>
        public int LastObjectsDrawn { get; private set; }

        public byte[] Render(TopologyScene scene, int width, int height)
        {
            var w = Clamp(width);
            var h = Clamp(height);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var drawn = 0;

            using (var bitmap = new Bitmap(w, h))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.Clear(Paper);

                    var scale = Math.Min(w / DesignWidth, h / DesignHeight);

                    // Managed.System.Drawing has no Graphics.Save/Restore: BeginContainer/EndContainer
                    // is the pair, and the restore belongs in a finally block.
                    var state = g.BeginContainer();
                    try
                    {
                        g.TranslateTransform((w - DesignWidth * scale) / 2f, (h - DesignHeight * scale) / 2f);
                        g.ScaleTransform(scale, scale);

                        drawn = DrawDesign(g, scene);
                    }
                    finally
                    {
                        g.EndContainer(state);
                    }
                }

                return Encode(bitmap, stopwatch, drawn);
            }
        }

        // ── drawing, in the 560 x 334 design space ──────────────────────────────

        private int DrawDesign(Graphics g, TopologyScene scene)
        {
            var drawn = 0;

            using (var framePen = new Pen(Frame, 2f))
                g.DrawRectangle(framePen, 6f, 6f, 548f, 322f);

            using (var label = ResolveFont(16f, FontStyle.Bold))
            using (var caption = ResolveFont(13f, FontStyle.Regular))
            using (var legend = ResolveFont(14f, FontStyle.Regular))
            using (var footnote = ResolveFont(13f, FontStyle.Regular))
            using (var ink = new SolidBrush(Ink))
            using (var muted = new SolidBrush(Muted))
            {
                if (scene == null || scene.Nodes.Count == 0)
                {
                    // An empty scene still produces a readable image, not a blank file.
                    g.DrawString("No nodes to display", label, muted, 24f, 40f);
                    DrawLegend(g, legend, footnote);
                    return 0;
                }

                var transform = FitToPage(scene);

                using (var edgePen = new Pen(EdgeColor, 2.5f))
                {
                    foreach (var edge in scene.Edges)
                    {
                        var from = scene.Find(edge.FromId);
                        var to = scene.Find(edge.ToId);
                        if (from == null || to == null)
                            continue;

                        g.DrawLine(edgePen, transform(from.Centre), transform(to.Centre));
                        drawn++;
                    }
                }

                foreach (var node in scene.Nodes)
                {
                    DrawNode(g, node, transform, label, caption, ink);
                    drawn++;
                }

                DrawLegend(g, legend, footnote);
            }

            return drawn;
        }

        private void DrawNode(Graphics g, NodeModel node, Func<PointF, PointF> transform, Font label, Font caption, Brush ink)
        {
            var topLeft = transform(new PointF(node.Bounds.Left, node.Bounds.Top));
            var bottomRight = transform(new PointF(node.Bounds.Right, node.Bounds.Bottom));
            var box = RectangleF.FromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);

            using (var path = RoundedRectangle(box, 10f))
            using (var body = new SolidBrush(NodeBody))
            using (var border = new Pen(node.Tone, 3f))
            {
                g.FillPath(body, path);
                g.DrawPath(border, path);
            }

            // Measured, then placed at an explicit origin: DrawString into a rectangle whose height is
            // near the line height clips the glyphs.
            var labelSize = g.MeasureString(node.Label, label);
            g.DrawString(node.Label, label, ink, box.X + (box.Width - labelSize.Width) / 2f, box.Y + 25f - labelSize.Height * 0.8f);

            var state = node.StateWord;
            var stateSize = g.MeasureString(state, caption);
            using (var tone = new SolidBrush(node.Tone))
                g.DrawString(state, caption, tone, box.X + (box.Width - stateSize.Width) / 2f, box.Y + 44f - stateSize.Height * 0.8f);
        }

        private void DrawLegend(Graphics g, Font legend, Font footnote)
        {
            var entries = new[]
            {
                new KeyValuePair<string, Color>("healthy", ToneFor("OK")),
                new KeyValuePair<string, Color>("degraded", ToneFor("Warning")),
                new KeyValuePair<string, Color>("critical", ToneFor("Critical")),
            };

            using (var muted = new SolidBrush(Muted))
            {
                var x = 24f;
                foreach (var entry in entries)
                {
                    using (var path = RoundedRectangle(new RectangleF(x, 295f, 16f, 16f), 4f))
                    using (var swatch = new SolidBrush(entry.Value))
                        g.FillPath(swatch, path);

                    g.DrawString(entry.Key, legend, muted, x + 24f, 294f);
                    x += 150f;
                }
            }

            // The family only: the "(shipped)" or "(runtime fallback)" half of the answer belongs in the
            // status line, not in the picture, where it runs into the legend.
            var family = ResolvedFontFamily;
            var bracket = family.IndexOf(" (", StringComparison.Ordinal);
            if (bracket > 0)
                family = family.Substring(0, bracket);

            var note = "font: " + family;
            using (var quiet = new SolidBrush(Footnote))
            {
                var size = g.MeasureString(note, footnote);
                var x = 536f - size.Width;

                // Never print over the last legend entry, and never past the right edge - a DrawString
                // whose origin sits outside the surface throws instead of clipping.
                if (x > 24f + 300f + 90f)
                    g.DrawString(note, footnote, quiet, x, 296f);
            }
        }

        /// <summary>Fits the whole scene into the page, independently of the on-screen pan and zoom.</summary>
        private static Func<PointF, PointF> FitToPage(TopologyScene scene)
        {
            var bounds = scene.Nodes[0].Bounds;
            foreach (var node in scene.Nodes)
                bounds = RectangleF.Union(bounds, node.Bounds);

            // The area left for the graph, above the legend strip.
            const float left = 24f, top = 24f, right = 536f, bottom = 282f;

            var scale = Math.Min(
                (right - left) / Math.Max(1f, bounds.Width),
                (bottom - top) / Math.Max(1f, bounds.Height));

            var offsetX = left + ((right - left) - bounds.Width * scale) / 2f - bounds.X * scale;
            var offsetY = top + ((bottom - top) - bounds.Height * scale) / 2f - bounds.Y * scale;

            return world => new PointF(world.X * scale + offsetX, world.Y * scale + offsetY);
        }

        private static GraphicsPath RoundedRectangle(RectangleF bounds, float radius)
        {
            var d = radius * 2f;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // ── fonts: never assume a family is installed ───────────────────────────

        /// <summary>
        /// Tries an application-supplied family first (any font file dropped into <c>Fonts/</c> beside the
        /// application), then falls back to a family the runtime always has. The result is recorded in
        /// <see cref="ResolvedFontFamily"/> so the output stays explainable on a host you cannot inspect.
        /// </summary>
        public Font ResolveFont(float size, FontStyle style)
        {
            var supplied = LoadSuppliedFamily();
            if (supplied != null)
            {
                try
                {
                    var font = new Font(supplied, size, style, GraphicsUnit.Pixel);
                    ResolvedFontFamily = supplied.Name + " (shipped with the application)";
                    return font;
                }
                catch (ArgumentException)
                {
                    // The family exists but not in this style: fall through to the fallback.
                }
            }

            ResolvedFontFamily = FontFamily.GenericSansSerif.Name + " (runtime fallback)";
            return new Font(FontFamily.GenericSansSerif, size, style, GraphicsUnit.Pixel);
        }

        private static FontFamily LoadSuppliedFamily()
        {
            lock (FontLock)
            {
                if (!privateFontsLoaded)
                {
                    privateFontsLoaded = true;

                    try
                    {
                        var folder = Path.Combine(AppContext.BaseDirectory, "Fonts");
                        if (Directory.Exists(folder))
                        {
                            foreach (var file in Directory.GetFiles(folder, "*.ttf"))
                                PrivateFonts.AddFontFile(file);
                        }
                    }
                    catch (Exception)
                    {
                        // A font that will not load is not a reason to fail an export.
                    }
                }

                return PrivateFonts.Families.Length > 0 ? PrivateFonts.Families[0] : null;
            }
        }

        // ── encoding ────────────────────────────────────────────────────────────

        private byte[] Encode(Bitmap bitmap, System.Diagnostics.Stopwatch stopwatch, int drawn)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);

                stopwatch.Stop();
                LastRenderMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
                LastObjectsDrawn = drawn;

                return stream.ToArray();
            }
        }

        private static int Clamp(int side) => Math.Min(MaximumSide, Math.Max(MinimumSide, side));

        private static Color ToneFor(string severity) =>
            severity == "Critical" ? Color.FromArgb(217, 58, 43) :
            severity == "Warning" ? Color.FromArgb(232, 161, 60) : Color.FromArgb(31, 157, 107);
    }
}
