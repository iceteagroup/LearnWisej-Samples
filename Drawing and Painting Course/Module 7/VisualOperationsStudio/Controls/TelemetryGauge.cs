using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using VisualOperationsStudio.Diagnostics;
using VisualOperationsStudio.Geometry;
using Wisej.Web;

namespace VisualOperationsStudio.Controls
{
    /// <summary>
    /// A server-painted gauge: inactive track, warning and critical zones, active arc, needle, the
    /// numeric value and a caption. Every setter clamps its input, returns when nothing changed and
    /// then calls <see cref="Wisej.Web.Control.Invalidate()"/> exactly once, so one value change is one
    /// repaint. It derives from <see cref="Panel"/> because a bare <c>Wisej.Web.Control</c> never raises
    /// Paint - it has no paintable client widget of its own.
    /// The coordinate maths lives in <see cref="GaugeGeometry"/>; this class only draws.
    /// </summary>
    public class TelemetryGauge : Panel
    {
        private double value;
        private double minimum;
        private double maximum = 100;
        private double warningThreshold = 70;
        private double criticalThreshold = 90;
        private string caption = string.Empty;
        private string unit = "%";

        public TelemetryGauge()
        {
            BackColor = Color.White;
            ForeColor = Color.FromArgb(13, 27, 42);
            Paint += (sender, e) => PaintGauge(e);

            // A painted control has to repaint when it is resized, because every coordinate it uses
            // came from its own size. It is also what gets the first picture drawn once the docked
            // layout has given this control a size.
            Resize += (sender, e) => Invalidate();
            UpdateAccessibleDescription();
        }

        /// <summary>
        /// How many times this gauge has asked to be repainted since the last
        /// <see cref="ResetRepaintCount"/>. A setter that returns early does not count.
        /// </summary>
        public int RepaintCount { get; private set; }

        public void ResetRepaintCount() => RepaintCount = 0;

        /// <summary>
        /// Raised once per completed paint, carrying the elapsed time, the painted size and how many
        /// parts were drawn, so the page can record them without this control knowing what a
        /// <see cref="RenderMetrics"/> is.
        /// </summary>
        public event EventHandler<RenderedEventArgs> Rendered;

        public double Value
        {
            get => this.value;
            set
            {
                var clamped = Clamp(value);
                if (clamped == this.value)
                    return;

                this.value = clamped;
                UpdateAccessibleDescription();
                RequestRepaint();
            }
        }

        public double Minimum
        {
            get => this.minimum;
            set
            {
                if (value == this.minimum)
                    return;

                this.minimum = value;
                this.value = Clamp(this.value);
                UpdateAccessibleDescription();
                RequestRepaint();
            }
        }

        public double Maximum
        {
            get => this.maximum;
            set
            {
                if (value == this.maximum)
                    return;

                this.maximum = value;
                this.value = Clamp(this.value);
                UpdateAccessibleDescription();
                RequestRepaint();
            }
        }

        public double WarningThreshold
        {
            get => this.warningThreshold;
            set
            {
                var clamped = Clamp(value);
                if (clamped == this.warningThreshold)
                    return;

                this.warningThreshold = clamped;
                RequestRepaint();
            }
        }

        public double CriticalThreshold
        {
            get => this.criticalThreshold;
            set
            {
                var clamped = Clamp(value);
                if (clamped == this.criticalThreshold)
                    return;

                this.criticalThreshold = clamped;
                RequestRepaint();
            }
        }

        public string Caption
        {
            get => this.caption;
            set
            {
                var text = value ?? string.Empty;
                if (text == this.caption)
                    return;

                this.caption = text;
                UpdateAccessibleDescription();
                RequestRepaint();
            }
        }

        public string Unit
        {
            get => this.unit;
            set
            {
                var text = value ?? string.Empty;
                if (text == this.unit)
                    return;

                this.unit = text;
                UpdateAccessibleDescription();
                RequestRepaint();
            }
        }

        /// <summary>The same reading the pixels show, as a sentence. Kept in step by every setter.</summary>
        public string ReadingText =>
            $"{this.caption}: {this.value:0.#} {this.unit}, range {this.minimum:0.#} to {this.maximum:0.#}, {Severity()}";

        /// <summary>
        /// The colour of the active arc. Left empty, the arc takes the colour of the zone the value is
        /// in; set, the arc keeps one colour and the zone bands alone carry the severity - which is how
        /// the capstone screen draws it.
        /// </summary>
        public Color AccentColor { get; set; } = Color.Empty;

        /// <summary>The colour of the numeric reading. Empty means "the same colour as the arc".</summary>
        public Color ValueColor { get; set; } = Color.Empty;

        public Color WarningZoneColor { get; set; } = Color.FromArgb(246, 217, 168);

        public Color CriticalZoneColor { get; set; } = Color.FromArgb(243, 182, 182);

        private void PaintGauge(PaintEventArgs e)
        {
            var area = e.ClipRectangle;
            if (area.Width <= 0 || area.Height <= 0)
                return;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var parts = 0;

            var layout = GaugeGeometry.Measure(area, GaugeGeometry.Normalize(this.value, this.minimum, this.maximum));
            if (layout.IsEmpty)
                return;

            var warnAt = GaugeGeometry.Normalize(this.warningThreshold, this.minimum, this.maximum);
            var critAt = GaugeGeometry.Normalize(this.criticalThreshold, this.minimum, this.maximum);
            var active = AccentColor.IsEmpty ? ZoneColor() : AccentColor;

            using (var track = new Pen(Color.FromArgb(231, 237, 244), layout.RingThickness))
            using (var activePen = new Pen(active, layout.RingThickness) { EndCap = LineCap.Round })
            using (var warning = new SolidBrush(WarningZoneColor))
            using (var critical = new SolidBrush(CriticalZoneColor))
            using (var hole = new SolidBrush(BackColor))
            {
                e.Graphics.DrawArc(track, layout.RingBounds, layout.StartAngle, layout.FullSweep);

                // FillPie fills back to the centre, so the zones are carved back to a ring afterwards.
                e.Graphics.FillPie(warning, layout.RingBounds, GaugeGeometry.AngleFor(warnAt), GaugeGeometry.SweepBetween(warnAt, critAt));
                e.Graphics.FillPie(critical, layout.RingBounds, GaugeGeometry.AngleFor(critAt), GaugeGeometry.SweepBetween(critAt, 1f));
                e.Graphics.FillEllipse(hole, layout.InnerBounds);

                if (layout.ValueSweep > 0f)
                    e.Graphics.DrawArc(activePen, layout.RingBounds, layout.StartAngle, layout.ValueSweep);
            }

            parts += 4;   // track, two zone pies, active arc

            DrawNeedle(e.Graphics, layout);
            DrawText(e.Graphics, layout, ValueColor.IsEmpty ? active : ValueColor);
            parts += 3;   // needle, value, caption

            stopwatch.Stop();
            Rendered?.Invoke(this, new RenderedEventArgs("gauge.paint", stopwatch.Elapsed, area.Width, area.Height, parts));
        }

        private void DrawNeedle(Graphics g, GaugeLayout layout)
        {
            // The needle is the one part that transforms the surface, so its state is saved and
            // restored in a finally block - an unrestored transform shifts everything drawn after it.
            // Managed.System.Drawing has no Graphics.Save/Restore: BeginContainer/EndContainer is the pair.
            var state = g.BeginContainer();
            try
            {
                g.TranslateTransform(layout.Centre.X, layout.Centre.Y);
                g.RotateTransform(layout.NeedleAngle);

                using (var path = new GraphicsPath())
                using (var needle = new SolidBrush(Color.FromArgb(43, 52, 64)))
                {
                    path.AddPolygon(layout.NeedlePoints);
                    g.FillPath(needle, path);
                    g.FillEllipse(needle, -layout.RingThickness * 0.45f, -layout.RingThickness * 0.45f, layout.RingThickness * 0.9f, layout.RingThickness * 0.9f);
                }
            }
            finally
            {
                g.EndContainer(state);
            }
        }

        private void DrawText(Graphics g, GaugeLayout layout, Color active)
        {
            var reading = $"{this.value:0.#}{(string.IsNullOrEmpty(this.unit) ? string.Empty : " " + this.unit)}";

            using (var valueFont = ResolveFont(GaugeGeometry.ValueFontSize(layout), FontStyle.Bold))
            using (var captionFont = ResolveFont(GaugeGeometry.CaptionFontSize(layout), FontStyle.Regular))
            using (var valueBrush = new SolidBrush(active))
            using (var captionBrush = new SolidBrush(Color.FromArgb(90, 107, 125)))
            {
                // Both strings are measured first and then placed: a layout rectangle whose height is
                // a little under the line height clips the glyphs, an explicit origin never does.
                var valueSize = g.MeasureString(reading, valueFont);
                g.DrawString(
                    reading,
                    valueFont,
                    valueBrush,
                    layout.ValueBounds.X + (layout.ValueBounds.Width - valueSize.Width) / 2f,
                    layout.ValueBounds.Y);

                var captionSize = g.MeasureString(this.caption, captionFont);
                if (captionSize.Width <= layout.CaptionBounds.Width)
                {
                    g.DrawString(
                        this.caption,
                        captionFont,
                        captionBrush,
                        layout.CaptionBounds.X + (layout.CaptionBounds.Width - captionSize.Width) / 2f,
                        layout.CaptionBounds.Y);
                }
            }
        }

        /// <summary>
        /// Never assume a family is installed: an unknown one falls back to the generic sans serif
        /// the runtime always has.
        /// </summary>
        private static Font ResolveFont(float size, FontStyle style)
        {
            try
            {
                return new Font("Segoe UI", size, style);
            }
            catch (ArgumentException)
            {
                return new Font(FontFamily.GenericSansSerif, size, style);
            }
        }

        private Color ZoneColor()
        {
            if (this.maximum <= this.minimum)
                return Color.FromArgb(154, 168, 182);

            if (this.value >= this.criticalThreshold)
                return Color.FromArgb(217, 58, 58);

            return this.value >= this.warningThreshold
                ? Color.FromArgb(232, 161, 60)
                : Color.FromArgb(31, 157, 107);
        }

        private string Severity()
        {
            if (this.maximum <= this.minimum)
                return "range not set";

            if (this.value >= this.criticalThreshold)
                return "critical";

            return this.value >= this.warningThreshold ? "warning" : "normal";
        }

        private double Clamp(double candidate)
        {
            var low = Math.Min(this.minimum, this.maximum);
            var high = Math.Max(this.minimum, this.maximum);
            return Math.Min(high, Math.Max(low, candidate));
        }

        private void RequestRepaint()
        {
            RepaintCount++;
            Invalidate();
        }

        private void UpdateAccessibleDescription() => AccessibleDescription = ReadingText;
    }
}
