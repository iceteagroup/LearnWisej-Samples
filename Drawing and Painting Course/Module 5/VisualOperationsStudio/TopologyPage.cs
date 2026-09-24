using System;
using System.Drawing;
using VisualOperationsStudio.Models;
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
        private static readonly Color Surface = Color.FromArgb(247, 249, 252);
        private static readonly Color GridLine = Color.FromArgb(227, 234, 243);
        private static readonly Color EdgeColor = Color.FromArgb(157, 180, 204);
        private static readonly Color NodeBorder = Color.FromArgb(205, 217, 230);
        private static readonly Color SelectedBorder = Color.FromArgb(21, 101, 216);
        private static readonly Color FocusRing = Color.FromArgb(125, 90, 224);
        private static readonly Color LabelColor = Color.FromArgb(31, 45, 58);
        private static readonly Color IdColor = Color.FromArgb(107, 127, 148);

        private const int GridStep = 28;

        /// <summary>
        /// The scene lives on the page instance, so it belongs to this session. A mutable static field
        /// here would be one plant shared by every user of the application.
        /// </summary>
        private readonly TopologyScene scene = TopologyScene.CreatePlant();

        private bool dragging;
        private bool panning;
        private PointF lastPointer;
        private int requestsThisGesture;
        private bool syncingList;
        private int culledLastRender;

        public TopologyPage()
        {
            InitializeComponent();

            BuildNodeList();
        }

        private void TopologyPage_Load(object sender, EventArgs e)
        {
            this.pnlZoom.BringToFront();

            // The surface may still measure nothing here, so the first scene is also issued from the
            // canvas's own Resize - the moment the docked layout finally gives it a size.
            this.canvasTopology.Resize += this.canvasTopology_Resize;

            PlaceZoomControls();
            RenderScene();
            ShowZoom();
        }

        private void canvasTopology_Resize(object sender, EventArgs e)
        {
            PlaceZoomControls();
            RenderScene();
        }

        /// <summary>The zoom controls float over the bottom left corner of the surface.</summary>
        private void PlaceZoomControls()
        {
            var host = this.pnlCanvasHost.ClientSize;
            if (host.Height <= 0)
                return;

            this.pnlZoom.Location = new Point(12, Math.Max(0, host.Height - this.pnlZoom.Height - 12));
        }

        // ── input: change state, then render. Nothing is drawn inside a handler. ──

        private void canvasTopology_MouseDown(object sender, MouseEventArgs e)
        {
            var pointer = new PointF(e.X, e.Y);
            this.lastPointer = pointer;
            this.requestsThisGesture = 0;

            var hit = this.scene.HitTest(pointer);
            this.scene.Select(hit);
            this.scene.Focus(hit);

            this.dragging = hit != null;
            this.panning = hit == null;

            SyncNodeList();
            RenderScene();

            SetStatus(hit == null
                ? "Ready · click a node to select it"
                : $"Selected: {hit.Label}", hit == null ? StatusTone.Idle : StatusTone.Ok);
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

                    SetStatus($"Dragging {node.Label} · {this.requestsThisGesture} render requests", StatusTone.Warn);
                }
            }
            else
            {
                this.scene.PanX += dx;
                this.scene.PanY += dy;
                SetStatus($"Panning · {this.requestsThisGesture} render requests", StatusTone.Warn);
            }

            // Every pointer move is one request and one render. That is the number the lab measures.
            RenderScene();
        }

        private void canvasTopology_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.dragging)
            {
                var node = this.scene.Selected;
                if (node != null)
                    SetStatus($"{node.Label} moved to world {node.Bounds.X:0}, {node.Bounds.Y:0} · gesture ended", StatusTone.Ok);
            }
            else if (this.panning)
            {
                SetStatus($"Pan ended · {this.requestsThisGesture} requests, one render each", StatusTone.Ok);
            }

            this.dragging = false;
            this.panning = false;
        }

        private void canvasTopology_MouseWheel(object sender, MouseEventArgs e)
        {
            this.scene.ZoomAbout(new PointF(e.X, e.Y), e.Delta > 0 ? 1.15f : 1f / 1.15f);
            RenderScene();
            ShowZoom();

            var selected = this.scene.Selected;
            SetStatus(
                selected == null
                    ? $"Zoom {this.scene.Zoom:0.00}x about the pointer"
                    : $"Zoom {this.scene.Zoom:0.00}x about the pointer · {selected.Label} still selected",
                StatusTone.Idle);
        }

        private void canvasTopology_KeyDown(object sender, KeyEventArgs e)
        {
            // A keyboard-only user can still reach every node and move the selected one.
            if (e.KeyCode == Keys.Tab)
            {
                SelectNext(e.Shift ? -1 : 1);
                e.Handled = true;
                return;
            }

            var node = this.scene.Selected;
            if (node == null)
                return;

            const float stepSize = 12f;
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
            SetStatus($"{node.Label} selected with the keyboard · nudged {stepSize:0} world units", StatusTone.Ok);
        }

        private void listNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.syncingList || this.listNodes.SelectedIndex < 0)
                return;

            var node = this.scene.Nodes[this.listNodes.SelectedIndex];
            this.scene.Select(node);
            this.scene.Focus(node);

            RenderScene();
            SetStatus($"{node.Label} selected in the list · same scene, same selection", StatusTone.Ok);
        }

        private void btnZoomIn_Click(object sender, EventArgs e) => ZoomFromCentre(1.25f);

        private void btnZoomOut_Click(object sender, EventArgs e) => ZoomFromCentre(1f / 1.25f);

        private void btnFit_Click(object sender, EventArgs e)
        {
            this.scene.Fit(this.canvasTopology.Width, this.canvasTopology.Height);
            RenderScene();
            ShowZoom();
            SetStatus($"Fit to view · zoom {this.scene.Zoom:0.00}x", StatusTone.Idle);
        }

        private void ZoomFromCentre(float factor)
        {
            this.scene.ZoomAbout(new PointF(this.canvasTopology.Width / 2f, this.canvasTopology.Height / 2f), factor);
            RenderScene();
            ShowZoom();
            SetStatus($"Zoom {this.scene.Zoom:0.00}x about the centre", StatusTone.Idle);
        }

        private void ShowZoom() => this.lblZoom.Text = $"{this.scene.Zoom:0.00}x";

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
            c.FillStyle = Surface;
            c.FillRect(0, 0, w, h);

            DrawGrid(c, w, h);

            var visible = this.scene.VisibleWorld(w, h);
            var drawn = 0;
            var culled = 0;

            c.Save();
            try
            {
                c.Translate((int)this.scene.PanX, (int)this.scene.PanY);
                c.Scale(this.scene.Zoom, this.scene.Zoom);

                c.StrokeStyle = EdgeColor;
                c.LineWidth = 2;
                foreach (var edge in this.scene.Edges)
                {
                    var from = this.scene.Find(edge.FromId);
                    var to = this.scene.Find(edge.ToId);
                    if (from == null || to == null)
                        continue;

                    if (!visible.IntersectsWith(RectangleF.Union(from.Bounds, to.Bounds)))
                        continue;

                    c.BeginPath();
                    c.MoveTo((int)from.Bounds.Right, (int)(from.Bounds.Y + from.Bounds.Height / 2f));
                    c.LineTo((int)to.Bounds.X, (int)(to.Bounds.Y + to.Bounds.Height / 2f));
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

            if (culled > 0 && culled != this.culledLastRender)
            {
                SetStatus(
                    $"Rendered {drawn} of {this.scene.Nodes.Count} nodes · the rest are outside the viewport",
                    StatusTone.Warn);
            }

            this.culledLastRender = culled;
        }

        /// <summary>
        /// The background grid, drawn in screen space before the viewport transform, so its lines stay
        /// one pixel wide whatever the zoom is.
        /// </summary>
        private void DrawGrid(Canvas c, int w, int h)
        {
            var step = GridStep * this.scene.Zoom;
            if (step < 6f)
                return;

            c.StrokeStyle = GridLine;
            c.LineWidth = 1;
            c.BeginPath();

            for (var x = this.scene.PanX % step; x < w; x += step)
            {
                c.MoveTo((int)x, 0);
                c.LineTo((int)x, h);
            }

            for (var y = this.scene.PanY % step; y < h; y += step)
            {
                c.MoveTo(0, (int)y);
                c.LineTo(w, (int)y);
            }

            c.Stroke();
        }

        private void DrawNode(Canvas c, NodeModel node)
        {
            var x = (int)node.Bounds.X;
            var y = (int)node.Bounds.Y;
            var width = (int)node.Bounds.Width;
            var height = (int)node.Bounds.Height;

            c.FillStyle = Color.White;
            RoundRect(c, x, y, width, height, 10);
            c.Fill();

            // The health band across the top: the only colour the node body carries.
            c.FillStyle = node.Tone;
            RoundRect(c, x, y, width, 7, 3);
            c.Fill();

            c.StrokeStyle = node.Selected ? SelectedBorder : NodeBorder;
            c.LineWidth = node.Selected ? 3 : 1;
            RoundRect(c, x, y, width, height, 10);
            c.Stroke();

            using (var labelFont = new Font("Segoe UI", 17f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var idFont = new Font("Consolas", 13f, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                c.TextAlign = CanvasTextAlign.Left;
                c.TextBaseline = CanvasTextBaseline.Alphabetic;

                c.TextFont = labelFont;
                c.FillStyle = LabelColor;
                c.FillText(node.Label, x + 14, y + 36);

                c.TextFont = idFont;
                c.FillStyle = IdColor;
                c.FillText(node.Id, x + 14, y + 60);
            }

            if (node.Selected)
            {
                // Four handles, big enough for a thumb: nothing on this surface is a DOM control.
                c.FillStyle = SelectedBorder;
                foreach (var corner in new[]
                {
                    new Point(x, y), new Point(x + width, y),
                    new Point(x, y + height), new Point(x + width, y + height),
                })
                {
                    RoundRect(c, corner.X - 5, corner.Y - 5, 10, 10, 2);
                    c.Fill();
                }
            }

            if (node.Focused && !node.Selected)
            {
                c.Save();
                try
                {
                    c.SetLineDash(new[] { 5, 4 });
                    c.StrokeStyle = FocusRing;
                    c.LineWidth = 2;
                    RoundRect(c, x - 7, y - 7, width + 14, height + 14, 13);
                    c.Stroke();
                }
                finally
                {
                    c.Restore();
                }
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

        // ── the list beside the surface, showing the same nodes ─────────────────

        /// <summary>
        /// Nothing on the canvas is a DOM control, so the same scene is published as a list: a
        /// keyboard-only or screen-reader user reaches every node through it.
        /// </summary>
        private void BuildNodeList()
        {
            this.syncingList = true;
            try
            {
                this.listNodes.Items.Clear();
                foreach (var node in this.scene.Nodes)
                {
                    var tone = ColorTranslator.ToHtml(node.Tone);
                    this.listNodes.Items.Add(
                        "<span style=\"display:inline-flex;align-items:center;gap:8px\">" +
                        $"<span style=\"width:8px;height:8px;border-radius:999px;background:{tone}\"></span>" +
                        $"{node.Label}</span>");
                }
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

        private void SelectNext(int direction)
        {
            var current = this.scene.Nodes.IndexOf(this.scene.Selected);
            var count = this.scene.Nodes.Count;
            var next = this.scene.Nodes[((current + direction) % count + count) % count];

            this.scene.Select(next);
            this.scene.Focus(next);

            SyncNodeList();
            RenderScene();
            SetStatus($"{next.Label} selected with the keyboard", StatusTone.Ok);
        }

        // ── the status strip ────────────────────────────────────────────────────

        private enum StatusTone { Idle, Ok, Warn, Bad }

        private void SetStatus(string text, StatusTone tone)
        {
            this.lblStatus.Text = text;
            this.lblStatus.ForeColor =
                tone == StatusTone.Ok ? Color.FromArgb(31, 157, 107) :
                tone == StatusTone.Bad ? Color.FromArgb(192, 57, 43) :
                tone == StatusTone.Warn ? Color.FromArgb(184, 118, 15) :
                Color.FromArgb(90, 107, 125);
        }
    }
}
