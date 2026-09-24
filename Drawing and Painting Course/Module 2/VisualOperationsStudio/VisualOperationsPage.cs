using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using VisualOperationsStudio.Controls;
using VisualOperationsStudio.Models;
using Wisej.Web;

namespace VisualOperationsStudio
{
    /// <summary>
    /// Three <see cref="TelemetryGauge"/> controls over three <see cref="TelemetrySample"/> readings.
    /// The gauges own no state: every value they show comes from the model, and every property they
    /// expose clamps, returns early when nothing changed, and repaints at most once - which is what the
    /// "Repaints this request" readout beside the buttons counts.
    /// The same numbers are published as a sentence through each gauge's <c>AccessibleDescription</c>,
    /// so nothing on this page exists only as pixels.
    /// </summary>
    public partial class VisualOperationsPage : Page
    {
        private static readonly double[] SpindleScript = { 88, 63, 41, 132, 74, 22, 95 };

        /// <summary>Card height as a fraction of card width, taken from the walkthrough's 300 x 267 card.</summary>
        private const double CardAspect = 0.89;
        private const int CardGap = 18;

        private readonly TelemetrySample spindleLoad = new TelemetrySample("Line 3 Press", "Line 3 · Spindle load", "%", 0, 100, 34);
        private readonly TelemetrySample coolantTemp = new TelemetrySample("Line 3 Press", "Line 3 · Coolant temp", "°C", 0, 120, 61);
        private readonly TelemetrySample cycleTime = new TelemetrySample("Line 3 Press", "Line 3 · Cycle time", "s", 0, 90, 47);

        private int readings;

        public VisualOperationsPage()
        {
            InitializeComponent();

            ConfigureGauge(this.gaugeSpindle, this.spindleLoad, 70, 90);
            ConfigureGauge(this.gaugeCoolant, this.coolantTemp, 85, 100);
            ConfigureGauge(this.gaugeCycle, this.cycleTime, 60, 75);

            ShowRepaints(0);
        }

        private void VisualOperationsPage_Load(object sender, EventArgs e)
        {
            LayoutCards();

            // The docked layout has just given each gauge its real size. A painted control that is
            // never invalidated after it is laid out keeps whatever it drew while it measured nothing.
            foreach (var gauge in Gauges)
                gauge.Invalidate();
        }

        /// <summary>
        /// The three cards share the row evenly, and the row keeps the card proportions of the
        /// walkthrough so the painted face never has to be squeezed into a cell it does not fit.
        /// </summary>
        private void pnlBody_Resize(object sender, EventArgs e) => LayoutCards();

        private void LayoutCards()
        {
            var available = this.pnlBody.ClientSize.Width - this.pnlBody.Padding.Horizontal - CardGap * 2;
            if (available <= 0)
                return;

            var cardWidth = available / 3;
            this.pnlCard1.Width = cardWidth;
            this.pnlCard2.Width = cardWidth;
            this.pnlGauges.Height = (int)Math.Round(cardWidth * CardAspect);
        }

        private IEnumerable<TelemetryGauge> Gauges
        {
            get
            {
                yield return this.gaugeSpindle;
                yield return this.gaugeCoolant;
                yield return this.gaugeCycle;
            }
        }

        /// <summary>
        /// A new spindle reading arrives; the other two are re-assigned unchanged. Their setters compare
        /// and return early, so only one gauge asks to be repainted - which is the whole point.
        /// </summary>
        private void btnTakeReading_Click(object sender, EventArgs e)
        {
            var raw = SpindleScript[this.readings % SpindleScript.Length];
            this.readings++;

            this.spindleLoad.Reading = raw;
            ShowRepaints(ApplyModelToGauges());
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.readings = 0;
            this.spindleLoad.Reading = 34;

            ShowRepaints(ApplyModelToGauges());
        }

        private void ConfigureGauge(TelemetryGauge gauge, TelemetrySample source, double warning, double critical)
        {
            gauge.Minimum = source.Minimum;
            gauge.Maximum = source.Maximum;
            gauge.WarningThreshold = warning;
            gauge.CriticalThreshold = critical;
            gauge.Caption = source.Caption;
            gauge.Unit = source.Unit;
            gauge.Value = source.Reading;
        }

        /// <summary>
        /// Pushes the model onto the gauges and returns how many of them actually asked for a repaint.
        /// </summary>
        private int ApplyModelToGauges()
        {
            foreach (var gauge in Gauges)
                gauge.ResetRepaintCount();

            this.gaugeSpindle.Value = this.spindleLoad.Reading;
            this.gaugeCoolant.Value = this.coolantTemp.Reading;
            this.gaugeCycle.Value = this.cycleTime.Reading;

            return Gauges.Sum(gauge => gauge.RepaintCount);
        }

        private void ShowRepaints(int repaints)
        {
            this.lblRepaints.Text = $"Repaints this request: {repaints}";
            this.lblRepaints.ForeColor = repaints > 0
                ? Color.FromArgb(31, 157, 107)
                : Color.FromArgb(90, 107, 125);
        }
    }
}
