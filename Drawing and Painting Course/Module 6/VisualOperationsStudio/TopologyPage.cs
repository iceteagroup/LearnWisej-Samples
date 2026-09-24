using System;
using System.Drawing;
using System.IO;
using VisualOperationsStudio.Models;
using VisualOperationsStudio.Renderers;
using Wisej.Web;

namespace VisualOperationsStudio
{
    /// <summary>
    /// The same topology twice: once as <see cref="Wisej.Web.Canvas"/> commands the browser executes,
    /// and once as PNG bytes produced off screen by <see cref="TopologyImageRenderer"/> - a
    /// <c>System.Drawing.Managed</c> renderer with no control, no session and no GDI+ underneath it.
    /// Both read the same <see cref="TopologyScene"/>; the export is never a screenshot of the canvas.
    /// </summary>
    public partial class TopologyPage : Page
    {
        private static readonly Color Surface = Color.FromArgb(247, 250, 253);
        private static readonly Color EdgeColor = Color.FromArgb(168, 188, 207);
        private static readonly Color LabelColor = Color.FromArgb(31, 45, 58);

        /// <summary>
        /// The scene lives on the page instance, so it belongs to this session. A mutable static field
        /// here would be one topology shared by every user of the application.
        /// </summary>
        private readonly TopologyScene scene = TopologyScene.CreateServices();

        private bool dragging;
        private bool panning;
        private bool fitted;
        private PointF lastPointer;
        private int requestsThisGesture;

        public TopologyPage()
        {
            InitializeComponent();
        }

        private void TopologyPage_Load(object sender, EventArgs e)
        {
            // The surface may still measure nothing here, so the first scene is also issued from the
            // canvas's own Resize - the moment the docked layout finally gives it a size.
            this.canvasTopology.Resize += this.canvasTopology_Resize;
            RenderScene();
        }

        private void canvasTopology_Resize(object sender, EventArgs e) => RenderScene();

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

            RenderScene();
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
            this.dragging = false;
            this.panning = false;
            SetStatus(StatusKind.Idle, "Drag a node, or export the scene as a PNG");
        }

        private void canvasTopology_MouseWheel(object sender, MouseEventArgs e)
        {
            this.scene.ZoomAbout(new PointF(e.X, e.Y), e.Delta > 0 ? 1.15f : 1f / 1.15f);
            RenderScene();
        }

        private void canvasTopology_KeyDown(object sender, KeyEventArgs e)
        {
            // A keyboard-only user can still move the selected node.
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
        }

        private void btnFitToView_Click(object sender, EventArgs e)
        {
            this.scene.Fit(this.canvasTopology.Width, this.canvasTopology.Height);
            RenderScene();
        }

        private void btnResetZoom_Click(object sender, EventArgs e)
        {
            this.scene.Zoom = 1f;
            this.scene.PanX = 40;
            this.scene.PanY = 40;
            RenderScene();
        }

        // ── the second renderer: image bytes, with no control involved ──────────

        private void btnExport_Click(object sender, EventArgs e)
        {
            this.btnExport.Text = "Rendering…";
            SetStatus(StatusKind.Busy, "Rendering 1200 × 700 into a Bitmap — no control involved…");

            try
            {
                var renderer = new TopologyImageRenderer();
                var bytes = renderer.Render(this.scene, 1200, 700);
                var name = $"topology-{DateTime.Now:yyyy-MM}.png";

                Application.Download(new MemoryStream(bytes), name);

                SetStatus(
                    StatusKind.Good,
                    $"✓ Rendered off-screen in {renderer.LastRenderMilliseconds:0} ms · 1200 × 700 · " +
                    $"{bytes.Length / 1024:N0} KB · font resolved: {renderer.ResolvedFontFamily}");
            }
            catch (Exception ex)
            {
                // A failed export reaches the user as a message, not as a blank space where a picture
                // was expected.
                SetStatus(StatusKind.Idle, $"Export failed: {ex.Message}. The topology on screen is unaffected.");
            }
            finally
            {
                this.btnExport.Text = "Export PNG";
            }
        }

        // ── the one method that draws the live surface ──────────────────────────

        private void canvasTopology_Redraw(object sender, EventArgs e)
        {
            RenderScene();
        }

        private void RenderScene()
        {
            var c = this.canvasTopology;
            var w = c.Width;
            var h = c.Height;
            if (w <= 0 || h <= 0)
                return;

            if (!this.fitted)
            {
                this.scene.Fit(w, h);
                this.fitted = true;
            }

            c.ClearRect(0, 0, w, h);
            c.FillStyle = Surface;
            c.FillRect(0, 0, w, h);

            var visible = this.scene.VisibleWorld(w, h);

            c.Save();
            try
            {
                c.Translate((int)this.scene.PanX, (int)this.scene.PanY);
                c.Scale(this.scene.Zoom, this.scene.Zoom);

                c.StrokeStyle = EdgeColor;
                c.LineWidth = 3;
                foreach (var edge in this.scene.Edges)
                {
                    var from = this.scene.Find(edge.FromId);
                    var to = this.scene.Find(edge.ToId);
                    if (from == null || to == null)
                        continue;

                    if (!visible.IntersectsWith(RectangleF.Union(from.Bounds, to.Bounds)))
                        continue;

                    c.BeginPath();
                    c.MoveTo((int)from.Centre.X, (int)from.Centre.Y);
                    c.LineTo((int)to.Centre.X, (int)to.Centre.Y);
                    c.Stroke();
                }

                foreach (var node in this.scene.Nodes)
                {
                    if (!visible.IntersectsWith(node.Bounds))
                        continue;

                    DrawNode(c, node);
                }
            }
            finally
            {
                c.Restore();
            }
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

            c.StrokeStyle = node.Tone;
            c.LineWidth = node.Selected ? 4 : 3;
            RoundRect(c, x, y, width, height, 10);
            c.Stroke();

            using (var labelFont = new Font("Segoe UI", 16f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var stateFont = new Font("Segoe UI", 13f, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                c.TextAlign = CanvasTextAlign.Center;
                c.TextBaseline = CanvasTextBaseline.Alphabetic;

                c.TextFont = labelFont;
                c.FillStyle = LabelColor;
                c.FillText(node.Label, x + width / 2, y + 25);

                c.TextFont = stateFont;
                c.FillStyle = node.Tone;
                c.FillText(node.StateWord, x + width / 2, y + 44);
            }

            c.TextAlign = CanvasTextAlign.Left;
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

        // ── the status strip ────────────────────────────────────────────────────

        private enum StatusKind { Idle, Busy, Good }

        private void SetStatus(StatusKind kind, string text)
        {
            this.lblStatus.Text = text;

            switch (kind)
            {
                case StatusKind.Good:
                    this.lblStatus.BackColor = Color.FromArgb(236, 248, 241);
                    this.lblStatus.ForeColor = Color.FromArgb(22, 119, 77);
                    break;
                case StatusKind.Busy:
                    this.lblStatus.BackColor = Color.FromArgb(255, 248, 236);
                    this.lblStatus.ForeColor = Color.FromArgb(138, 91, 18);
                    break;
                default:
                    this.lblStatus.BackColor = Color.FromArgb(246, 248, 251);
                    this.lblStatus.ForeColor = Color.FromArgb(90, 107, 125);
                    break;
            }
        }
    }
}
