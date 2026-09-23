using System;
using System.Drawing;
using VisualOperationsStudio.Models;
using Wisej.Web;

namespace VisualOperationsStudio
{
    /// <summary>
    /// One <see cref="TelemetrySample"/> rendered four different ways:
    /// 1. a Label and a ProgressBar, which draw no pixels of ours,
    /// 2. <c>panelPainted</c>, whose Paint handler runs on the server and ships an image,
    /// 3. <c>canvasSurface</c>, which sends drawing commands the browser executes,
    /// 4. <c>picExport</c>, fed by an off-screen Bitmap built with Graphics.FromImage.
    /// </summary>
    public partial class VisualOperationsPage : Page
    {
        private readonly TelemetrySample sample = new TelemetrySample("Line 3 Press", 42);
        private readonly Random random = new Random();

        private int clicks;

        public VisualOperationsPage()
        {
            InitializeComponent();

            RefreshPlainSurface();
            RefreshOffScreenSurface();
        }

        /// <summary>
        /// The Canvas has its laid-out size by Load, so the first scene is drawn here.
        /// Redraw then rebuilds it after every browser resize.
        /// </summary>
        private void VisualOperationsPage_Load(object sender, EventArgs e)
        {
            DrawCanvasScene();
        }

        // ── the one thing that changes the model ────────────────────────────────

        private void btnNewReading_Click(object sender, EventArgs e)
        {
            this.clicks++;

            // Every fourth reading arrives out of range, so the clamp in TelemetrySample.Reading
            // is exercised instead of letting a renderer draw off its surface.
            var raw = this.clicks % 4 == 0
                ? (this.clicks % 8 == 0 ? 132.0 : -15.0)
                : this.random.Next(5, 96);

            this.sample.Reading = raw;

            RefreshPlainSurface();                  // surface 1 - text and a value
            this.panelPainted.Invalidate();         // surface 2 - the server repaints and sends an image
            DrawCanvasScene();                      // surface 3 - commands the browser executes
            RefreshOffScreenSurface();              // surface 4 - image bytes

            this.lblStatus.Text = Math.Abs(raw - this.sample.Reading) > 0.001
                ? $"Reading {raw:0} % clamped to {this.sample.Reading:0} %. All four surfaces agree."
                : $"Reading updated to {this.sample.Reading:0} %. All four surfaces agree.";
        }

        // ── surface 1: no pixels of ours ────────────────────────────────────────

        private void RefreshPlainSurface()
        {
            this.lblReading.Text = $"{this.sample.Reading:0} %";
            this.progressReading.Value = (int)Math.Round(this.sample.Reading);
        }

        // ── surface 2: the server paints, Wisej.NET ships the image ─────────────

        private void panelPainted_Paint(object sender, PaintEventArgs e)
        {
            var area = e.ClipRectangle;
            if (area.Width <= 0 || area.Height <= 0)
                return;

            var filled = (int)(area.Width * this.sample.Reading / 100.0);

            using (var track = new SolidBrush(Color.WhiteSmoke))
            using (var fill = new SolidBrush(Color.SteelBlue))
            {
                e.Graphics.FillRectangle(track, area);
                e.Graphics.FillRectangle(fill, area.X, area.Y, filled, area.Height);
            }

            // Observe the model, render it, change nothing - and never dispose
            // e.Graphics: that surface belongs to Wisej.NET.
        }

        // ── surface 3: the browser draws what the server tells it to ────────────

        private void canvasSurface_Redraw(object sender, EventArgs e)
        {
            DrawCanvasScene();
        }

        /// <summary>
        /// Rebuilds the whole canvas scene from the model. A Redraw handler that only patches the
        /// last change is broken by the first resize, because the browser bitmap is gone by then.
        /// </summary>
        private void DrawCanvasScene()
        {
            var w = this.canvasSurface.Width;
            var h = this.canvasSurface.Height;
            if (w <= 0 || h <= 0)
                return;

            this.canvasSurface.ClearRect(0, 0, w, h);

            this.canvasSurface.FillStyle = Color.WhiteSmoke;
            this.canvasSurface.FillRect(0, 0, w, h);

            this.canvasSurface.FillStyle = Color.MediumPurple;
            this.canvasSurface.FillRect(0, 0, (int)(w * this.sample.Reading / 100.0), h);
        }

        // ── surface 4: an image, produced with no control involved ──────────────

        private void RefreshOffScreenSurface()
        {
            try
            {
                var previous = this.picExport.Image;
                this.picExport.Image = RenderOffScreen(320, 60);
                previous?.Dispose();
            }
            catch (Exception ex)
            {
                this.picExport.Image = null;
                this.lblStatus.Text = $"Off-screen image not available: {ex.Message}. The other three surfaces still show {this.sample.Reading:0} %.";
            }
        }

        private Image RenderOffScreen(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "the off-screen bitmap needs a positive size");

            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            using (var fill = new SolidBrush(Color.Goldenrod))
            {
                g.Clear(Color.White);
                g.FillRectangle(fill, 0, 0, (float)(width * this.sample.Reading / 100.0), height);
            }

            return bitmap;
        }
    }
}
