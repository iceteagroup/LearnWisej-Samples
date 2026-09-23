using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using VisualOperationsStudio.Controls;
using VisualOperationsStudio.Diagnostics;
using VisualOperationsStudio.Geometry;
using VisualOperationsStudio.Models;
using Wisej.Web;

namespace VisualOperationsStudio
{
    /// <summary>
    /// The capstone screen. Two of the four surfaces live here - the painted gauges and the painted grid
    /// cells - and both read the one <see cref="OperationsModel"/> the session owns. The other two, the
    /// Canvas topology and the PNG export, get the same instance handed to them, so a threshold moved
    /// once moves everywhere.
    /// </summary>
    public partial class VisualOperationsPage : Page
    {
        private static readonly double[] SpindleScript = { 88, 63, 41, 132, 74, 22, 95 };

        /// <summary>The one model behind all four surfaces, owned by this session.</summary>
        private readonly OperationsModel model = new OperationsModel();

        private int readings;

        public VisualOperationsPage()
        {
            InitializeComponent();

            ConfigureUserPaintedColumns();
            this.gridOperations.DataSource = this.model.Machines;

            foreach (var gauge in Gauges)
                gauge.Rendered += this.gauge_Rendered;

            ConfigureGauge(this.gaugeSpindle, this.model.SpindleLoad, 70, 90);
            ConfigureGauge(this.gaugeCoolant, this.model.CoolantTemp, 85, 100);
            ConfigureGauge(this.gaugeCycle, this.model.CycleTime, 60, 75);

            PublishReadings();
            this.lblStatus.Text = $"Ready. Three gauges, and {this.model.Machines.Count:N0} machines with two painted columns.";
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

            this.model.SpindleLoad.Reading = raw;

            var repaints = ApplyModelToGauges();

            this.lblStatus.Text = Math.Abs(raw - this.model.SpindleLoad.Reading) > 0.001
                ? $"Spindle reading {raw:0} % clamped to {this.model.SpindleLoad.Reading:0} %. Repaints this request: {repaints}."
                : $"Spindle load now {this.model.SpindleLoad.Reading:0} %. Repaints this request: {repaints}.";
        }

        /// <summary>Every painted surface reports its own figures to the one metrics recorder.</summary>
        private void gauge_Rendered(object sender, RenderedEventArgs e)
        {
            this.model.Metrics.Record(e.Surface, e.Elapsed, e.Width, e.Height, e.Objects);
        }

        /// <summary>
        /// The topology editor is a page of its own. The operations page instance is handed to it so
        /// coming back does not rebuild this screen - and so the session keeps one model, not two.
        /// </summary>
        private void btnPlayground_Click(object sender, EventArgs e)
        {
            Application.MainPage = new TopologyPage(this, this.model);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.readings = 0;
            this.model.SpindleLoad.Reading = 34;

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

            this.gaugeSpindle.Value = this.model.SpindleLoad.Reading;
            this.gaugeCoolant.Value = this.model.CoolantTemp.Reading;
            this.gaugeCycle.Value = this.model.CycleTime.Reading;

            PublishReadings();

            return Gauges.Sum(gauge => gauge.RepaintCount);
        }

        /// <summary>
        /// Every value the graphics show, as an ordinary table. A reading that exists only inside a
        /// picture has been lost for part of the audience, and the severity chips in the grid carry a
        /// word as well as a colour for the same reason.
        /// </summary>
        private void PublishReadings()
        {
            var counts = this.model.MachineSeverityCounts();
            var rows = new List<NameValue>();

            foreach (var sample in this.model.Readings)
                rows.Add(new NameValue(sample.Caption, sample.DisplayValue));

            rows.Add(new NameValue("Machines - normal", counts[Severity.Normal].ToString("N0")));
            rows.Add(new NameValue("Machines - warning", counts[Severity.Warning].ToString("N0")));
            rows.Add(new NameValue("Machines - critical", counts[Severity.Critical].ToString("N0")));
            rows.Add(new NameValue("Topology nodes", this.model.Topology.Nodes.Count.ToString()));

            var selected = this.model.Topology.Selected;
            rows.Add(new NameValue("Topology selection", selected == null ? "none" : $"{selected.Label} ({selected.Severity})"));

            this.gridValues.DataSource = rows;
        }

        /// <summary>One row of the accessible table.</summary>
        public class NameValue
        {
            public NameValue(string name, string text)
            {
                Name = name;
                Text = text;
            }

            public string Name { get; }

            public string Text { get; }
        }

        // ── capstone diagnostics ────────────────────────────────────────────────

        private void btnDegraded_Click(object sender, EventArgs e)
        {
            Report("Degraded states");
            foreach (var line in DegradedStateCheck.Run())
                Report("  " + line);

            this.lblStatus.Text = "Degraded checks finished - see the report below.";
        }

        private void btnMetrics_Click(object sender, EventArgs e)
        {
            Report("Render metrics (median of what this session has recorded)");

            foreach (var surface in new[] { "gauge.paint", "topology.export" })
            {
                var median = this.model.Metrics.Median(surface);
                Report(median == null
                    ? $"  {surface,-18} not drawn yet in this session"
                    : $"  {surface,-18} {median:0.0} ms over {this.model.Metrics.Count(surface)} renders");
            }

            foreach (var sample in this.model.Metrics.Samples.Reverse().Take(6))
                Report("  " + sample);

            this.lblStatus.Text = "Render metrics written to the report below.";
        }

        private void Report(string line)
        {
            this.listReport.Items.Add(line);
            while (this.listReport.Items.Count > 200)
                this.listReport.Items.RemoveAt(0);

            this.listReport.SelectedIndex = this.listReport.Items.Count - 1;
        }
    }
}
