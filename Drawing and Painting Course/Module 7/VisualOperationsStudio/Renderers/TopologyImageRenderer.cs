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
    /// </summary>
    public class TopologyImageRenderer
    {
        /// <summary>A typo must not be able to ask for a 4000 x 3000 preview: that is ~48 MB before a single shape is drawn.</summary>
        public const int MinimumSide = 200;

        public const int MaximumSide = 2000;

        private static readonly PrivateFontCollection PrivateFonts = new PrivateFontCollection();
        private static readonly object FontLock = new object();
        private static bool privateFontsLoaded;

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
                    g.Clear(Color.White);

                    using (var title = ResolveFont(14f, FontStyle.Bold))
                    using (var label = ResolveFont(10f, FontStyle.Bold))
                    using (var caption = ResolveFont(8.5f, FontStyle.Regular))
                    using (var ink = new SolidBrush(Color.FromArgb(13, 27, 42)))
                    using (var muted = new SolidBrush(Color.FromArgb(90, 107, 125)))
                    using (var edgePen = new Pen(Color.FromArgb(154, 168, 182), 2f))
                    {
                        g.DrawString("Visual Operations Studio - asset topology", title, ink, 24f, 20f);

                        if (scene == null || scene.Nodes.Count == 0)
                        {
                            // An empty scene still produces a readable image, not a blank file.
                            g.DrawString("No nodes to display", label, muted, 24f, 60f);
                            DrawLegend(g, label, caption, w, h);
                            return Encode(bitmap, stopwatch, 0);
                        }

                        var transform = FitToPage(scene, w, h);

                        foreach (var edge in scene.Edges)
                        {
                            var from = scene.Find(edge.FromId);
                            var to = scene.Find(edge.ToId);
                            if (from == null || to == null)
                                continue;

                            g.DrawLine(edgePen, transform(from.Centre), transform(to.Centre));
                            drawn++;
                        }

                        foreach (var node in scene.Nodes)
                        {
                            DrawNode(g, node, transform, label, caption, ink, muted);
                            drawn++;
                        }

                        DrawLegend(g, label, caption, w, h);
                    }
                }

                return Encode(bitmap, stopwatch, drawn);
            }
        }

        // ── drawing ─────────────────────────────────────────────────────────────

        private void DrawNode(Graphics g, NodeModel node, Func<PointF, PointF> transform, Font label, Font caption, Brush ink, Brush muted)
        {
            var topLeft = transform(new PointF(node.Bounds.Left, node.Bounds.Top));
            var bottomRight = transform(new PointF(node.Bounds.Right, node.Bounds.Bottom));
            var box = RectangleF.FromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);

            using (var path = new GraphicsPath())
            using (var body = new SolidBrush(BodyFor(node.Severity)))
            using (var border = new Pen(BorderFor(node.Severity), node.Selected ? 3f : 1.6f))
            {
                path.AddRectangle(box);
                g.FillPath(body, path);
                g.DrawPath(border, path);
            }

            var labelSize = g.MeasureString(node.Label, label);
            g.DrawString(node.Label, label, ink, box.X + (box.Width - labelSize.Width) / 2f, box.Y + box.Height / 2f - labelSize.Height);

            var captionSize = g.MeasureString(node.Severity, caption);
            g.DrawString(node.Severity, caption, muted, box.X + (box.Width - captionSize.Width) / 2f, box.Y + box.Height / 2f + 2f);
        }

        private static void DrawLegend(Graphics g, Font label, Font caption, int width, int height)
        {
            var entries = new[]
            {
                new KeyValuePair<string, Color>("Normal", BorderFor("Normal")),
                new KeyValuePair<string, Color>("Warning", BorderFor("Warning")),
                new KeyValuePair<string, Color>("Critical", BorderFor("Critical")),
            };

            var y = height - 34f;
            var x = 24f;

            using (var muted = new SolidBrush(Color.FromArgb(90, 107, 125)))
            {
                g.DrawString("Status", caption, muted, x, y - 18f);

                foreach (var entry in entries)
                {
                    using (var swatch = new SolidBrush(entry.Value))
                        g.FillRectangle(swatch, x, y, 14f, 14f);

                    g.DrawString(entry.Key, caption, muted, x + 20f, y);
                    x += 20f + g.MeasureString(entry.Key, caption).Width + 18f;
                }
            }
        }

        /// <summary>Fits the whole scene into the page, independently of the on-screen pan and zoom.</summary>
        private static Func<PointF, PointF> FitToPage(TopologyScene scene, int width, int height)
        {
            var bounds = scene.Nodes[0].Bounds;
            foreach (var node in scene.Nodes)
                bounds = RectangleF.Union(bounds, node.Bounds);

            bounds.Inflate(40f, 40f);

            var margin = 56f;
            var scale = Math.Min(
                (width - margin * 2) / Math.Max(1f, bounds.Width),
                (height - margin * 2 - 40f) / Math.Max(1f, bounds.Height));

            var offsetX = margin - bounds.X * scale;
            var offsetY = margin + 20f - bounds.Y * scale;

            return world => new PointF(world.X * scale + offsetX, world.Y * scale + offsetY);
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
                    var font = new Font(supplied, size, style);
                    ResolvedFontFamily = supplied.Name + " (shipped with the application)";
                    return font;
                }
                catch (ArgumentException)
                {
                    // The family exists but not in this style: fall through to the fallback.
                }
            }

            ResolvedFontFamily = FontFamily.GenericSansSerif.Name + " (runtime fallback)";
            return new Font(FontFamily.GenericSansSerif, size, style);
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

        private static Color BodyFor(string severity) =>
            severity == "Critical" ? Color.FromArgb(253, 236, 236) :
            severity == "Warning" ? Color.FromArgb(253, 243, 226) : Color.White;

        private static Color BorderFor(string severity) =>
            severity == "Critical" ? Color.FromArgb(217, 58, 58) :
            severity == "Warning" ? Color.FromArgb(232, 161, 60) : Color.FromArgb(31, 157, 107);
    }
}
