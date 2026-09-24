using System;
using System.Drawing;
using VisualOperationsStudio.Models;
using VisualOperationsStudio.Renderers;
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
        private static readonly Color PaintColor = Color.FromArgb(21, 101, 216);
        private static readonly Color CanvasColor = Color.FromArgb(125, 90, 224);
        private static readonly Color ImageColor = Color.FromArgb(232, 161, 60);

        private static readonly Color TrackFill = Color.FromArgb(238, 242, 247);
        private static readonly Color TrackBorder = Color.FromArgb(221, 229, 238);

        private readonly TelemetrySample sample = new TelemetrySample("Line 3 Press", 42);
        private readonly Random random = new Random();

        private int clicks;
        private bool loaded;

        public VisualOperationsPage()
        {
            InitializeComponent();

            RefreshPlainSurface();
        }

        /// <summary>
        /// The Canvas has its laid-out size by Load, so the first scene is drawn here.
        /// Redraw then rebuilds it after every browser resize.
        /// </summary>
        private void VisualOperationsPage_Load(object sender, EventArgs e)
        {
            LayoutRows();

            // The docked layout has just given every surface its real size, so each one is asked for a
            // first picture here. A painted control that is never invalidated after it is laid out shows
            // whatever it produced while it still measured nothing.
            this.panelPainted.Invalidate();
            DrawCanvasScene();
            RefreshOffScreenSurface();

            this.loaded = true;
        }

        // ── layout: two cards per row, 16 px apart ──────────────────────────────

        private void row_Resize(object sender, EventArgs e) => LayoutRow(sender as Panel);

        private void LayoutRows()
        {
            LayoutRow(this.pnlRow1);
            LayoutRow(this.pnlRow2);
        }

        private static void LayoutRow(Panel row)
        {
            if (row == null)
                return;

            var available = row.ClientSize.Width - 16;
            if (available <= 0)
                return;

            foreach (Control child in row.Controls)
            {
                if (child.Dock == DockStyle.Left && !(child.Name ?? string.Empty).StartsWith("pnlColGap"))
                    child.Width = available / 2;
            }
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

            if (Math.Abs(raw - this.sample.Reading) > 0.001)
                SetStatus($"Reading {raw:0} % clamped to {this.sample.Reading:0} % — all four surfaces agree.", StatusTone.Bad);
            else
                SetStatus($"All four surfaces show {this.sample.Reading:0} % — from one TelemetrySample.", StatusTone.Good);
        }

        // ── surface 1: no pixels of ours ────────────────────────────────────────

        private void RefreshPlainSurface()
        {
            this.lblReading.Text = $"{this.sample.Reading:0}";
            this.progressReading.Value = (int)Math.Round(this.sample.Reading);
        }

        // ── surface 2: the server paints, Wisej.NET ships the image ─────────────

        private void panelPainted_Paint(object sender, PaintEventArgs e)
        {
            var area = e.ClipRectangle;
            if (area.Width <= 0 || area.Height <= 0)
                return;

            ReadingBar.Draw(e.Graphics, area, this.sample.Reading, PaintColor);

            // Observe the model, render it, change nothing - and never dispose
            // e.Graphics: that surface belongs to Wisej.NET.
        }

        /// <summary>
        /// A painted control has to repaint when it is resized: every coordinate the handler uses came
        /// from the control's own size.
        /// </summary>
        private void panelPainted_Resize(object sender, EventArgs e) => this.panelPainted.Invalidate();

        // ── surface 3: the browser draws what the server tells it to ────────────

        private void canvasSurface_Redraw(object sender, EventArgs e)
        {
            // The browser threw the bitmap away when the surface was resized. Nothing was recovered;
            // the scene is rebuilt from the model, which is the only place the reading lives.
            DrawCanvasScene();

            // The first Redraw arrives with the page's own layout, before the user has done anything.
            if (this.loaded)
                SetStatus("Redraw rebuilt the canvas scene from the model.", StatusTone.Good);
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

            var c = this.canvasSurface;
            c.ClearRect(0, 0, w, h);

            var barHeight = Math.Min(ReadingBar.BarHeight, h);
            var filled = (int)Math.Round(w * this.sample.Reading / 100.0);

            // the rounded track, drawn as a path because the 2D context has no rounded rectangle
            RoundedPath(c, 0, 0, w - 1, barHeight - 1, 5);
            c.FillStyle = TrackFill;
            c.Fill();
            c.StrokeStyle = TrackBorder;
            c.LineWidth = 1;
            c.Stroke();

            if (filled > 0)
            {
                // Save / Restore around the clip, so the text below is not clipped away too.
                c.Save();
                try
                {
                    RoundedPath(c, 0, 0, w - 1, barHeight - 1, 5);
                    c.Clip();
                    c.FillStyle = CanvasColor;
                    c.FillRect(0, 0, filled, barHeight);
                }
                finally
                {
                    c.Restore();
                }
            }

            var top = barHeight + ReadingBar.ValueGap;
            if (top + 6 > h)
                return;

            c.TextAlign = CanvasTextAlign.Left;
            c.TextBaseline = CanvasTextBaseline.Top;

            using (var valueFont = new Font("Segoe UI", 22f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var captionFont = new Font("Segoe UI", 13f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                var reading = $"{this.sample.Reading:0}";

                // There is no measureText on the 2D context: the width comes from an off-screen
                // Graphics, the same one surface 4 uses.
                var valueWidth = MeasureText(reading, valueFont);

                c.TextFont = valueFont;
                c.FillStyle = Color.FromArgb(13, 27, 42);
                c.FillText(reading, 0, top);

                c.TextFont = captionFont;
                c.FillStyle = Color.FromArgb(123, 139, 156);
                c.FillText("% of rated load", (int)Math.Round(valueWidth) + 8, top + 8);
            }
        }

        private static void RoundedPath(Canvas c, int x, int y, int w, int h, int r)
        {
            // Angles are degrees on Wisej.Web.Canvas, not radians, and every coordinate is an int.
            c.BeginPath();
            c.MoveTo(x + r, y);
            c.LineTo(x + w - r, y);
            c.Arc(x + w - r, y + r, r, 270, 360, false);
            c.LineTo(x + w, y + h - r);
            c.Arc(x + w - r, y + h - r, r, 0, 90, false);
            c.LineTo(x + r, y + h);
            c.Arc(x + r, y + h - r, r, 90, 180, false);
            c.LineTo(x, y + r);
            c.Arc(x + r, y + r, r, 180, 270, false);
            c.ClosePath();
        }

        private static float MeasureText(string text, Font font)
        {
            using (var bitmap = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(bitmap))
            {
                return g.MeasureString(text, font).Width;
            }
        }

        // ── surface 4: an image, produced with no control involved ──────────────

        private void picExport_Resize(object sender, EventArgs e) => RefreshOffScreenSurface();

        private void RefreshOffScreenSurface()
        {
            // Before the docked layout runs, this control measures almost nothing. There is no picture
            // to make yet, and no failure either.
            if (this.picExport.Width < 16 || this.picExport.Height < 16)
                return;

            try
            {
                var previous = this.picExport.Image;
                this.picExport.Image = RenderOffScreen(this.picExport.Width, this.picExport.Height);
                previous?.Dispose();
            }
            catch (Exception ex)
            {
                this.picExport.Image = null;
                SetStatus(
                    $"Off-screen image not available: {ex.Message}. The other three surfaces still show {this.sample.Reading:0} %.",
                    StatusTone.Bad);
            }
        }

        private Image RenderOffScreen(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "the off-screen bitmap needs a positive size");

            var bitmap = new Bitmap(width, height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                ReadingBar.Draw(g, new Rectangle(0, 0, width, height), this.sample.Reading, ImageColor);
            }

            return bitmap;
        }

        // ── the status strip ────────────────────────────────────────────────────

        private enum StatusTone { Idle, Good, Bad }

        private void SetStatus(string text, StatusTone tone)
        {
            this.lblStatus.Text = text;

            switch (tone)
            {
                case StatusTone.Good:
                    this.lblStatus.BackColor = Color.FromArgb(230, 246, 238);
                    this.lblStatus.ForeColor = Color.FromArgb(23, 128, 79);
                    break;
                case StatusTone.Bad:
                    this.lblStatus.BackColor = Color.FromArgb(253, 236, 236);
                    this.lblStatus.ForeColor = Color.FromArgb(180, 47, 47);
                    break;
                default:
                    this.lblStatus.BackColor = Color.FromArgb(244, 247, 250);
                    this.lblStatus.ForeColor = Color.FromArgb(90, 107, 125);
                    break;
            }
        }
    }
}
