using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using VisualOperationsStudio.Controls;
using VisualOperationsStudio.Geometry;
using VisualOperationsStudio.Models;
using Wisej.Web;

namespace VisualOperationsStudio
{
    /// <summary>
    /// Three <see cref="TelemetryGauge"/> controls over three <see cref="TelemetrySample"/> readings,
    /// and an operations grid whose Health and Trend columns paint themselves. The gauges and the cells
    /// own no state: every value they show comes from the model.
    /// </summary>
    public partial class VisualOperationsPage : Page
    {
        private static readonly double[] SpindleScript = { 88, 63, 41, 132, 74, 22, 95 };

        private readonly TelemetrySample spindleLoad = new TelemetrySample("Line 3 Press", "Line 3 - Spindle load", "%", 0, 100, 34);
        private readonly TelemetrySample coolantTemp = new TelemetrySample("Line 3 Press", "Line 3 - Coolant temp", "°C", 0, 120, 61);
        private readonly TelemetrySample cycleTime = new TelemetrySample("Line 3 Press", "Line 3 - Cycle time", "s", 0, 90, 47);

        private readonly List<MachineStatus> machines = MachineStatusGenerator.Generate(1000);

        private int readings;

        public VisualOperationsPage()
        {
            InitializeComponent();

            ConfigureUserPaintedColumns();
            this.gridOperations.DataSource = this.machines;

            ConfigureGauge(this.gaugeSpindle, this.spindleLoad, 70, 90);
            ConfigureGauge(this.gaugeCoolant, this.coolantTemp, 85, 100);
            ConfigureGauge(this.gaugeCycle, this.cycleTime, 60, 75);

            PublishReadings();
            this.lblStatus.Text = $"Ready. Three gauges, and {this.machines.Count:N0} machines with two painted columns.";
        }

        /// <summary>
        /// User painting comes before cell painting: a column is switched into user-painted mode, and
        /// only then does the grid raise CellPaint for it. The handler is subscribed once, here.
        /// </summary>
        private void ConfigureUserPaintedColumns()
        {
            this.colHealth.UserPaint = true;
            this.colTrend.UserPaint = true;
            this.gridOperations.CellPaint += this.gridOperations_CellPaint;
        }

        /// <summary>
        /// One handler for the whole grid. It returns at once for a header row or a column it does not
        /// own, then takes its geometry from the clip rectangle and the row's values from the bound
        /// object in a single step. No allocation beyond the two renderers, no control creation, no
        /// side effects: this runs again for every cell that scrolls into view.
        /// </summary>
        private void gridOperations_CellPaint(object sender, DataGridViewCellPaintEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var isHealth = e.ColumnIndex == this.colHealth.Index;
            var isTrend = e.ColumnIndex == this.colTrend.Index;
            if (!isHealth && !isTrend)
                return;

            var row = this.gridOperations.Rows[e.RowIndex];
            var machine = row.DataBoundItem as MachineStatus;
            if (machine == null)
                return;

            var area = Rectangle.Inflate(e.ClipRectangle, -6, -7);

            if (isHealth)
                OperationsCellRenderer.DrawHealthBar(e.Graphics, area, machine.Health, row.Selected);
            else
                OperationsCellRenderer.DrawSparkline(e.Graphics, area, machine.RecentReadings, row.Selected);
        }

        /// <summary>The two docked gauges share the row evenly with the filling one.</summary>
        private void pnlGauges_Resize(object sender, EventArgs e)
        {
            var third = Math.Max(GaugeGeometry.MinimumSide, (this.pnlGauges.ClientSize.Width - this.pnlGauges.Padding.Horizontal) / 3);
            this.gaugeSpindle.Width = third;
            this.gaugeCoolant.Width = third;
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

            var repaints = ApplyModelToGauges();

            this.lblStatus.Text = Math.Abs(raw - this.spindleLoad.Reading) > 0.001
                ? $"Spindle reading {raw:0} % clamped to {this.spindleLoad.Reading:0} %. Repaints this request: {repaints}."
                : $"Spindle load now {this.spindleLoad.Reading:0} %. Repaints this request: {repaints}.";
        }

        /// <summary>
        /// The playground is a page of its own. The operations page instance is handed to it so coming
        /// back does not rebuild this screen - and so the session keeps one model, not two.
        /// </summary>
        private void btnPlayground_Click(object sender, EventArgs e)
        {
            Application.MainPage = new CanvasPlaygroundPage(this);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.readings = 0;
            this.spindleLoad.Reading = 34;

            var repaints = ApplyModelToGauges();
            this.lblStatus.Text = $"Reset to the opening readings. Repaints this request: {repaints}.";
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

            PublishReadings();

            return Gauges.Sum(gauge => gauge.RepaintCount);
        }

        /// <summary>
        /// The same three numbers as text. A value that exists only inside a picture has been lost for
        /// part of the audience, so it is published here and through each gauge's AccessibleDescription.
        /// </summary>
        private void PublishReadings()
        {
            this.lblCaption1.Text = this.spindleLoad.Caption;
            this.lblValue1.Text = this.spindleLoad.DisplayValue;
            this.lblCaption2.Text = this.coolantTemp.Caption;
            this.lblValue2.Text = this.coolantTemp.DisplayValue;
            this.lblCaption3.Text = this.cycleTime.Caption;
            this.lblValue3.Text = this.cycleTime.DisplayValue;
        }
    }
}
