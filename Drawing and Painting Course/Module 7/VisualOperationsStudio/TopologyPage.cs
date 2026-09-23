using System;
using System.Drawing;
using System.IO;
using VisualOperationsStudio.Diagnostics;
using VisualOperationsStudio.Models;
using VisualOperationsStudio.Renderers;
using Wisej.Web;

namespace VisualOperationsStudio
{
    /// <summary>
    /// The Canvas playground turned into a topology editor. The Canvas forgets and the model must not:
    /// input changes <see cref="TopologyScene"/>, and <see cref="RenderScene"/> is the only method that
    /// draws. The Redraw event calls that same method, which is why a browser resize simply rebuilds
    /// the picture.
    /// </summary>
    public partial class TopologyPage : Page
    {
        private readonly VisualOperationsPage operations;

        /// <summary>
        /// The same model the operations page uses: this page renders it, it does not own it. The scene
        /// therefore belongs to the session, not to a mutable static field that every user would share.
        /// </summary>
        private readonly OperationsModel model;

        private TopologyScene scene => this.model.Topology;

        /// <summary>The export size after the capstone optimisation (it was 1600 x 900 in Module 6).</summary>
        private const int ExportWidth = 1000;

        private const int ExportHeight = 560;

        private bool dragging;
        private bool panning;
        private PointF lastPointer;
        private int requestsThisGesture;
        private bool syncingList;

        public TopologyPage(VisualOperationsPage operations, OperationsModel model)
        {
            InitializeComponent();

            this.operations = operations;
            this.model = model;

            BuildNodeList();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Application.MainPage = this.operations;
        }

        // ── input: change state, then render. Nothing is drawn inside a handler. ──

        private void canvasTopology_MouseDown(object sender, MouseEventArgs e)
        {
            var pointer = new PointF(e.X, e.Y);
            this.lastPointer = pointer;
            this.requestsThisGesture = 0;

            var hit = this.scene.HitTest(pointer);
            this.scene.Select(hit);

            this.dragging = hit != null;
            this.panning = hit == null;

            SyncNodeList();
            RenderScene();
            ReportSelection();
        }

        private void canvasTopology_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.dragging && !this.panning)
                return;

            this.requestsThisGesture++;

            var pointer = new PointF(e.X, e.Y);
            var dx = pointer.X - this.lastPointer.X;
            var dy = pointer.Y - this.lastPointer.Y;
            this.lastPointer = pointer;

            if (this.dragging)
            {
                var node = this.scene.Selected;
                if (node != null)
                {
                    // The pointer moved in screen pixels; the node moves in world units.
                    var bounds = node.Bounds;
                    bounds.X += dx / this.scene.Zoom;
                    bounds.Y += dy / this.scene.Zoom;
                    node.Bounds = bounds;
                }
            }
            else
            {
                this.scene.PanX += dx;
                this.scene.PanY += dy;
            }

            RenderScene();
        }

        private void canvasTopology_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.dragging || this.panning)
            {
                this.lblStatus.Text =
                    $"{(this.dragging ? "Drag" : "Pan")} finished: {this.requestsThisGesture} requests, " +
                    $"one render each. {SelectionText()}";
            }

            this.dragging = false;
            this.panning = false;
        }

        private void canvasTopology_MouseWheel(object sender, MouseEventArgs e)
        {
            this.scene.ZoomAbout(new PointF(e.X, e.Y), e.Delta > 0 ? 1.15f : 1f / 1.15f);
            RenderScene();
            ReportSelection();
        }

        private void canvasTopology_KeyDown(object sender, KeyEventArgs e)
        {
            // A keyboard-only user can still reach every node and move the selected one.
            if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Down && e.Control)
            {
                SelectNext();
                e.Handled = true;
                return;
            }

            var node = this.scene.Selected;
            if (node == null)
                return;

            var stepSize = e.Shift ? 20f : 5f;
            var bounds = node.Bounds;

            switch (e.KeyCode)
            {
                case Keys.Left: bounds.X -= stepSize; break;
                case Keys.Right: bounds.X += stepSize; break;
                case Keys.Up: bounds.Y -= stepSize; break;
                case Keys.Down: bounds.Y += stepSize; break;
                default: return;
            }

            node.Bounds = bounds;
            e.Handled = true;

            RenderScene();
            ReportSelection();
        }

        private void listNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.syncingList || this.listNodes.SelectedIndex < 0)
                return;

            this.scene.Select(this.scene.Nodes[this.listNodes.SelectedIndex]);
            RenderScene();
            ReportSelection();
        }

        /// <summary>
        /// The export never screenshots the Canvas: it renders the same model again, off screen, with no
        /// control and no session involved, and hands the bytes to the browser as a download.
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var renderer = new TopologyImageRenderer();

                // The optimisation this capstone measured: a 1000 x 560 export instead of 1600 x 900.
                // The picture is the same seven nodes; the encoder has 3.1x fewer pixels to compress,
                // and PNG encoding was almost all of the old figure.
                var bytes = renderer.Render(this.scene, ExportWidth, ExportHeight);

                this.model.Metrics.Record(
                    "topology.export",
                    TimeSpan.FromMilliseconds(renderer.LastRenderMilliseconds),
                    ExportWidth,
                    ExportHeight,
                    renderer.LastObjectsDrawn);

                Application.Download(new MemoryStream(bytes), "plant-topology.png");

                this.lblStatus.Text =
                    $"Exported {bytes.Length / 1024:N0} KB of PNG in {renderer.LastRenderMilliseconds:0} ms, " +
                    $"{renderer.LastObjectsDrawn} objects drawn, font: {renderer.ResolvedFontFamily}.";
            }
            catch (Exception ex)
            {
                // A failed export reaches the user as a message, not as a blank space where a picture
                // was expected.
                this.lblStatus.Text = $"Export failed: {ex.Message}. The topology on screen is unaffected.";
            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e) => ZoomFromCentre(1.25f);

        private void btnZoomOut_Click(object sender, EventArgs e) => ZoomFromCentre(1f / 1.25f);

        private void btnResetView_Click(object sender, EventArgs e)
        {
            this.scene.Zoom = 1f;
            this.scene.PanX = 40;
            this.scene.PanY = 40;
            RenderScene();
            ReportSelection();
        }

        private void ZoomFromCentre(float factor)
        {
            this.scene.ZoomAbout(new PointF(this.canvasTopology.Width / 2f, this.canvasTopology.Height / 2f), factor);
            RenderScene();
            ReportSelection();
        }

        // ── the one method that draws ───────────────────────────────────────────

        private void canvasTopology_Redraw(object sender, EventArgs e)
        {
            RenderScene();
        }

        /// <summary>
        /// Clears the surface, applies the viewport once with Translate and Scale, draws the edges and
        /// then the nodes, and restores. Everything outside the visible world rectangle is skipped.
        /// </summary>
        private void RenderScene()
        {
            var c = this.canvasTopology;
            var w = c.Width;
            var h = c.Height;
            if (w <= 0 || h <= 0)
                return;

            c.ClearRect(0, 0, w, h);
            c.FillStyle = Color.FromArgb(248, 250, 252);
            c.FillRect(0, 0, w, h);

            var visible = this.scene.VisibleWorld(w, h);
            var drawn = 0;
            var culled = 0;

            c.Save();
            try
            {
                c.Translate((int)this.scene.PanX, (int)this.scene.PanY);
                c.Scale(this.scene.Zoom, this.scene.Zoom);

                c.StrokeStyle = Color.FromArgb(154, 168, 182);
                c.LineWidth = 2;
                foreach (var edge in this.scene.Edges)
                {
                    var from = this.scene.Find(edge.FromId);
                    var to = this.scene.Find(edge.ToId);
                    if (from == null || to == null)
                        continue;

                    if (!visible.IntersectsWith(RectangleF.Union(from.Bounds, to.Bounds)))
                    {
                        culled++;
                        continue;
                    }

                    c.BeginPath();
                    c.MoveTo((int)from.Centre.X, (int)from.Centre.Y);
                    c.LineTo((int)to.Centre.X, (int)to.Centre.Y);
                    c.Stroke();
                }

                foreach (var node in this.scene.Nodes)
                {
                    if (!visible.IntersectsWith(node.Bounds))
                    {
                        culled++;
                        continue;
                    }

                    DrawNode(c, node);
                    drawn++;
                }
            }
            finally
            {
                c.Restore();
            }

            this.lblNodesHeader.Text = $"The same nodes, as a list  ({drawn} drawn, {culled} culled)";
        }

        private void DrawNode(Canvas c, NodeModel node)
        {
            var body = node.Severity == "Critical" ? Color.FromArgb(253, 236, 236)
                : node.Severity == "Warning" ? Color.FromArgb(253, 243, 226)
                : Color.White;

            var border = node.Severity == "Critical" ? Color.FromArgb(217, 58, 58)
                : node.Severity == "Warning" ? Color.FromArgb(232, 161, 60)
                : Color.FromArgb(200, 212, 226);

            c.FillStyle = body;
            c.FillRect((int)node.Bounds.X, (int)node.Bounds.Y, (int)node.Bounds.Width, (int)node.Bounds.Height);

            c.StrokeStyle = node.Selected ? Color.FromArgb(21, 101, 216) : border;
            c.LineWidth = node.Selected ? 3 : 2;
            c.BeginPath();
            c.Rect((int)node.Bounds.X, (int)node.Bounds.Y, (int)node.Bounds.Width, (int)node.Bounds.Height);
            c.Stroke();

            c.TextFont = new Font("default", 10F, FontStyle.Bold);
            c.TextAlign = CanvasTextAlign.Center;
            c.TextBaseline = CanvasTextBaseline.Middle;
            c.FillStyle = Color.FromArgb(13, 27, 42);
            c.FillText(node.Label, (int)node.Centre.X, (int)node.Centre.Y - 6);

            c.TextFont = new Font("default", 9F, FontStyle.Regular);
            c.FillStyle = Color.FromArgb(90, 107, 125);
            c.FillText(node.Severity, (int)node.Centre.X, (int)node.Centre.Y + 12);
        }

        // ── the list beside the surface, showing the same nodes ─────────────────

        private void BuildNodeList()
        {
            this.syncingList = true;
            try
            {
                this.listNodes.Items.Clear();
                foreach (var node in this.scene.Nodes)
                    this.listNodes.Items.Add($"{node.Label}  -  {node.Severity}");
            }
            finally
            {
                this.syncingList = false;
            }
        }

        private void SyncNodeList()
        {
            this.syncingList = true;
            try
            {
                this.listNodes.SelectedIndex = this.scene.Nodes.IndexOf(this.scene.Selected);
            }
            finally
            {
                this.syncingList = false;
            }
        }

        private void SelectNext()
        {
            var current = this.scene.Nodes.IndexOf(this.scene.Selected);
            var next = this.scene.Nodes[(current + 1 + this.scene.Nodes.Count) % this.scene.Nodes.Count];

            this.scene.Select(next);
            SyncNodeList();
            RenderScene();
            ReportSelection();
        }

        private void ReportSelection()
        {
            SyncNodeList();
            this.lblStatus.Text = SelectionText();
        }

        private string SelectionText()
        {
            var node = this.scene.Selected;
            var view = $"zoom {this.scene.Zoom:0.00}x, pan {this.scene.PanX:0}/{this.scene.PanY:0}";

            return node == null
                ? $"Nothing selected. View: {view}."
                : $"{node.Label} selected at {node.Bounds.X:0},{node.Bounds.Y:0} in world units. View: {view}.";
        }
    }
}
