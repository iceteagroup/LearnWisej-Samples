using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using VisualOperationsStudio.Controls;
using VisualOperationsStudio.Diagnostics;
using VisualOperationsStudio.Models;
using VisualOperationsStudio.Renderers;
using Wisej.Web;

namespace VisualOperationsStudio
{
    /// <summary>
    /// The capstone screen: all four surfaces on one page, all reading the one
    /// <see cref="OperationsModel"/> the session owns. Control Paint draws the gauge on the server,
    /// CellPaint draws the Health and Trend cells, Wisej.Web.Canvas sends the topology to the browser,
    /// and System.Drawing.Managed turns the same scene into PNG bytes - so a threshold moved once moves
    /// everywhere, and the export can never contradict the screen it came from.
    /// </summary>
    public partial class VisualOperationsPage : Page
    {
        private const int CanvasSceneWidth = 320;
        private const int CanvasSceneHeight = 250;

        private static readonly Color CanvasSurface = Color.FromArgb(248, 250, 252);
        private static readonly Color EdgeColor = Color.FromArgb(185, 201, 220);
        private static readonly Color SelectionColor = Color.FromArgb(21, 101, 216);

        /// <summary>The one model behind all four surfaces, owned by this session.</summary>
        private readonly OperationsModel model = new OperationsModel();

        private bool dragging;
        private PointF lastPointer;

        public VisualOperationsPage()
        {
            InitializeComponent();

            ConfigureUserPaintedColumns();
            this.gridAssets.DataSource = this.model.Assets;

            ConfigureGauge();
            this.gaugeCpu.Rendered += this.gauge_Rendered;

            PublishAccessibleText();
        }

        private void VisualOperationsPage_Load(object sender, EventArgs e)
        {
            this.cardCanvas.Resize += this.cardCanvas_Resize;
            SizeCanvas();
            RenderTopology();

            // "It degrades gracefully" is a claim, not a fact, until something runs it. The four bad
            // states are exercised on every start; only a failure is allowed to reach the screen.
            var failures = DegradedStateCheck.Run().Where(line => line.Contains("FAILED")).ToList();
            if (failures.Count > 0)
            {
                this.lblAccessibleTable.ForeColor = Color.FromArgb(180, 47, 47);
                this.lblAccessibleTable.Text = "Degraded-state check failed · " + string.Join(" · ", failures);
            }

            PublishDiagnostics(failures.Count);
        }

        // ── surface 1: Control Paint, on the server ─────────────────────────────

        private void ConfigureGauge()
        {
            var sample = this.model.Cpu;

            this.gaugeCpu.Minimum = sample.Minimum;
            this.gaugeCpu.Maximum = sample.Maximum;
            this.gaugeCpu.WarningThreshold = AssetStatus.WarningAbove;
            this.gaugeCpu.CriticalThreshold = AssetStatus.CriticalAbove;
            this.gaugeCpu.Caption = sample.Caption;
            this.gaugeCpu.Unit = sample.Unit;
            this.gaugeCpu.Value = sample.Reading;

            // The capstone face keeps one arc colour and lets the zone bands carry the severity.
            this.gaugeCpu.AccentColor = SelectionColor;
            this.gaugeCpu.ValueColor = Color.FromArgb(13, 27, 42);
            this.gaugeCpu.WarningZoneColor = Color.FromArgb(232, 161, 60);
            this.gaugeCpu.CriticalZoneColor = Color.FromArgb(224, 90, 90);
        }

        /// <summary>Every painted surface reports its own figures to the one metrics recorder.</summary>
        private void gauge_Rendered(object sender, RenderedEventArgs e)
        {
            this.model.Metrics.Record(e.Surface, e.Elapsed, e.Width, e.Height, e.Objects);
        }

        // ── surface 2: CellPaint, one subscription for the whole grid ───────────

        private void ConfigureUserPaintedColumns()
        {
            this.colHealth.UserPaint = true;
            this.colTrend.UserPaint = true;
            this.gridAssets.CellPaint += this.gridAssets_CellPaint;
        }

        /// <summary>
        /// One handler for the whole grid. It returns at once for a header row or a column it does not
        /// own, then takes its geometry from the clip rectangle and the row's values from the bound
        /// object in a single step. This runs again for every cell that scrolls into view, on request
        /// threads, so it allocates nothing it does not dispose and changes no application state.
        /// </summary>
        private void gridAssets_CellPaint(object sender, DataGridViewCellPaintEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var isHealth = e.ColumnIndex == this.colHealth.Index;
            var isTrend = e.ColumnIndex == this.colTrend.Index;
            if (!isHealth && !isTrend)
                return;

            var row = this.gridAssets.Rows[e.RowIndex];
            var asset = row.DataBoundItem as AssetStatus;
            if (asset == null)
                return;

            var area = Rectangle.Inflate(e.ClipRectangle, -12, -10);

            if (isHealth)
                OperationsCellRenderer.DrawLoadBar(e.Graphics, area, asset.Load, asset.Tone, row.Selected);
            else
                OperationsCellRenderer.DrawSparkline(e.Graphics, area, asset.Trend, asset.Tone, row.Selected);
        }

        // ── surface 3: Wisej.Web.Canvas, drawn by the browser ───────────────────

        private void cardCanvas_Resize(object sender, EventArgs e)
        {
            SizeCanvas();
            RenderTopology();
        }

        /// <summary>Keeps the topology card's aspect ratio, so the scene never has to be squeezed.</summary>
        private void SizeCanvas()
        {
            var width = this.cardCanvas.ClientSize.Width;
            if (width <= 0)
                return;

            this.canvasTopology.Height = (int)Math.Round(width * (double)CanvasSceneHeight / CanvasSceneWidth);
        }

        private void canvasTopology_Redraw(object sender, EventArgs e) => RenderTopology();

        private void canvasTopology_MouseDown(object sender, MouseEventArgs e)
        {
            var pointer = new PointF(e.X, e.Y);
            this.lastPointer = pointer;

            var hit = this.model.Topology.HitTest(ToScene(pointer));
            if (hit != null)
                this.model.Topology.Select(hit);

            this.dragging = hit != null;
            RenderTopology();
        }

        private void canvasTopology_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.dragging)
                return;

            var node = this.model.Topology.Selected;
            if (node == null)
                return;

            var scale = SceneScale();
            if (scale <= 0f)
                return;

            var bounds = node.Bounds;
            bounds.X += (e.X - this.lastPointer.X) / scale;
            bounds.Y += (e.Y - this.lastPointer.Y) / scale;
            node.Bounds = bounds;

            this.lastPointer = new PointF(e.X, e.Y);
            RenderTopology();
        }

        private void canvasTopology_MouseUp(object sender, MouseEventArgs e) => this.dragging = false;

        private void btnResetView_Click(object sender, EventArgs e)
        {
            var fresh = TopologyScene.CreateAssets();
            for (var i = 0; i < this.model.Topology.Nodes.Count && i < fresh.Nodes.Count; i++)
                this.model.Topology.Nodes[i].Bounds = fresh.Nodes[i].Bounds;

            RenderTopology();
        }

        private float SceneScale()
        {
            var w = this.canvasTopology.Width;
            var h = this.canvasTopology.Height;
            if (w <= 0 || h <= 0)
                return 0f;

            return Math.Min(w / (float)CanvasSceneWidth, h / (float)CanvasSceneHeight);
        }

        private PointF ToScene(PointF screen)
        {
            var scale = SceneScale();
            if (scale <= 0f)
                return screen;

            var originX = (this.canvasTopology.Width - CanvasSceneWidth * scale) / 2f;
            var originY = (this.canvasTopology.Height - CanvasSceneHeight * scale) / 2f;

            return new PointF((screen.X - originX) / scale, (screen.Y - originY) / scale);
        }

        /// <summary>
        /// The whole scene, rebuilt from the model. The browser keeps pixels and pixels do not survive,
        /// so Redraw calls exactly this method after every resize.
        /// </summary>
        private void RenderTopology()
        {
            var c = this.canvasTopology;
            var w = c.Width;
            var h = c.Height;
            if (w <= 0 || h <= 0)
                return;

            var scale = SceneScale();
            if (scale <= 0f)
                return;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            c.ClearRect(0, 0, w, h);
            c.FillStyle = CanvasSurface;
            c.FillRect(0, 0, w, h);

            c.Save();
            try
            {
                c.Translate((int)Math.Round((w - CanvasSceneWidth * scale) / 2f), (int)Math.Round((h - CanvasSceneHeight * scale) / 2f));
                c.Scale(scale, scale);

                c.StrokeStyle = EdgeColor;
                c.LineWidth = 2;
                foreach (var edge in this.model.Topology.Edges)
                {
                    var from = this.model.Topology.Find(edge.FromId);
                    var to = this.model.Topology.Find(edge.ToId);
                    if (from == null || to == null)
                        continue;

                    c.BeginPath();
                    c.MoveTo((int)from.Centre.X, (int)from.Centre.Y);
                    c.LineTo((int)to.Centre.X, (int)to.Centre.Y);
                    c.Stroke();
                }

                foreach (var node in this.model.Topology.Nodes)
                    DrawNode(c, node);
            }
            finally
            {
                c.Restore();
            }

            stopwatch.Stop();
            this.model.Metrics.Record(
                "topology.canvas",
                stopwatch.Elapsed,
                w,
                h,
                this.model.Topology.Nodes.Count + this.model.Topology.Edges.Count);
        }

        private void DrawNode(Canvas c, NodeModel node)
        {
            var x = (int)node.Bounds.X;
            var y = (int)node.Bounds.Y;
            var width = (int)node.Bounds.Width;
            var height = (int)node.Bounds.Height;
            var outline = node.Selected ? SelectionColor : node.Tone;

            c.FillStyle = Color.White;
            RoundRect(c, x, y, width, height, 7);
            c.Fill();

            c.StrokeStyle = outline;
            c.LineWidth = node.Selected ? 3 : 2;
            RoundRect(c, x, y, width, height, 7);
            c.Stroke();

            using (var font = new Font("Segoe UI", node.Selected ? 13f : 12.5f, node.Selected ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel))
            {
                c.TextFont = font;
                c.TextAlign = CanvasTextAlign.Center;
                c.TextBaseline = CanvasTextBaseline.Middle;
                c.FillStyle = node.Selected ? SelectionColor : node.TextTone;
                c.FillText(node.Label, x + width / 2, y + height / 2);
                c.TextAlign = CanvasTextAlign.Left;
            }
        }

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

        // ── surface 4: System.Drawing.Managed, no control involved ──────────────

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var renderer = new TopologyImageRenderer();
                var bytes = renderer.Render(this.model.Topology, 1200, 700);

                this.model.Metrics.Record(
                    "topology.export",
                    TimeSpan.FromMilliseconds(renderer.LastRenderMilliseconds),
                    1200,
                    700,
                    renderer.LastObjectsDrawn);

                Application.Download(new MemoryStream(bytes), $"topology-{DateTime.Now:yyyy-MM}.png");
                PublishDiagnostics(0);
            }
            catch (Exception ex)
            {
                // A failed export reaches the user as a message, not as a blank space where a picture
                // was expected.
                this.lblAccessibleTable.ForeColor = Color.FromArgb(180, 47, 47);
                this.lblAccessibleTable.Text = $"Export failed: {ex.Message}. The three live surfaces are unaffected.";
            }
        }

        // ── the textual equivalent, beside the pixels ───────────────────────────

        /// <summary>
        /// Every value the graphics show, as ordinary text. A reading that exists only inside a picture
        /// has been lost for part of the audience, which is why the gauge also carries its reading as a
        /// sentence and every severity chip carries a shape as well as a colour.
        /// </summary>
        private void PublishAccessibleText()
        {
            var severity = this.model.SeverityOf(this.model.Cpu).ToString().ToLowerInvariant();

            this.lblAccessibleReading.Text =
                $"AccessibleDescription: <b>&#8220;{this.model.Cpu.MachineName} CPU {this.model.Cpu.Reading:0}%, {severity}&#8221;</b>";

            this.lblAccessibleTable.Text = this.model.AccessibleTable();
            this.gaugeCpu.AccessibleDescription = this.gaugeCpu.ReadingText;
        }

        /// <summary>
        /// The measured figures behind the capstone notes. They are published through the page's
        /// accessible description rather than as another panel on screen: the numbers belong in
        /// CapstoneNotes.md, not in the operator's way.
        /// </summary>
        private void PublishDiagnostics(int failures)
        {
            var text = new StringBuilder("Visual Operations Studio capstone. ");

            foreach (var surface in new[] { "gauge.paint", "topology.canvas", "topology.export" })
            {
                var median = this.model.Metrics.Median(surface);
                if (median.HasValue)
                    text.Append($"{surface} median {median.Value:0.0} ms over {this.model.Metrics.Count(surface)} renders. ");
            }

            text.Append(failures == 0
                ? "Degraded-state checks: empty data, zero size, inverted range and missing font all degraded without throwing."
                : $"Degraded-state checks: {failures} failed.");

            AccessibleDescription = text.ToString();
        }
    }
}
