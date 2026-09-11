namespace IntegrationLab
{
    partial class Window1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelGauge = new Wisej.Web.Panel();
            this.labelGaugeTitle = new Wisej.Web.Label();
            this.gauge = new IntegrationLab.Widgets.GaugeWidget();
            this.panelKnob = new Wisej.Web.Panel();
            this.labelKnobTitle = new Wisej.Web.Label();
            this.knob = new IntegrationLab.Widgets.KnobWidget();
            this.panelChart = new Wisej.Web.Panel();
            this.labelChartTitle = new Wisej.Web.Label();
            this.chart = new IntegrationLab.Widgets.ChartWidget();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.timerStream = new Wisej.Web.Timer(this.components);
            this.panelGauge.SuspendLayout();
            this.panelKnob.SuspendLayout();
            this.panelChart.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGauge
            //
            this.panelGauge.BackColor = System.Drawing.Color.White;
            this.panelGauge.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGauge.Controls.Add(this.labelGaugeTitle);
            this.panelGauge.Controls.Add(this.gauge);
            this.panelGauge.Location = new System.Drawing.Point(30, 30);
            this.panelGauge.Name = "panelGauge";
            this.panelGauge.Size = new System.Drawing.Size(340, 240);
            //
            // labelGaugeTitle
            //
            this.labelGaugeTitle.AutoSize = false;
            this.labelGaugeTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.labelGaugeTitle.Location = new System.Drawing.Point(16, 10);
            this.labelGaugeTitle.Name = "labelGaugeTitle";
            this.labelGaugeTitle.Size = new System.Drawing.Size(308, 26);
            this.labelGaugeTitle.Text = "Gauge";
            //
            // gauge
            //
            this.gauge.Label = "VendorGauge";
            this.gauge.Location = new System.Drawing.Point(20, 40);
            this.gauge.Name = "gauge";
            this.gauge.Size = new System.Drawing.Size(300, 186);
            this.gauge.Threshold = 100D;
            this.gauge.Value = 72D;
            this.gauge.WarnAt = 85D;
            //
            // panelKnob
            //
            this.panelKnob.BackColor = System.Drawing.Color.White;
            this.panelKnob.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelKnob.Controls.Add(this.labelKnobTitle);
            this.panelKnob.Controls.Add(this.knob);
            this.panelKnob.Location = new System.Drawing.Point(382, 30);
            this.panelKnob.Name = "panelKnob";
            this.panelKnob.Size = new System.Drawing.Size(340, 240);
            //
            // labelKnobTitle
            //
            this.labelKnobTitle.AutoSize = false;
            this.labelKnobTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.labelKnobTitle.Location = new System.Drawing.Point(16, 10);
            this.labelKnobTitle.Name = "labelKnobTitle";
            this.labelKnobTitle.Size = new System.Drawing.Size(308, 26);
            this.labelKnobTitle.Text = "Knob";
            //
            // knob
            //
            this.knob.Label = "Pressure";
            this.knob.Location = new System.Drawing.Point(80, 40);
            this.knob.Name = "knob";
            this.knob.Size = new System.Drawing.Size(180, 186);
            this.knob.Value = 50D;
            //
            // panelChart
            //
            this.panelChart.BackColor = System.Drawing.Color.White;
            this.panelChart.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelChart.Controls.Add(this.labelChartTitle);
            this.panelChart.Controls.Add(this.chart);
            this.panelChart.Location = new System.Drawing.Point(30, 282);
            this.panelChart.Name = "panelChart";
            this.panelChart.Size = new System.Drawing.Size(692, 236);
            //
            // labelChartTitle
            //
            this.labelChartTitle.AutoSize = false;
            this.labelChartTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.labelChartTitle.Location = new System.Drawing.Point(16, 10);
            this.labelChartTitle.Name = "labelChartTitle";
            this.labelChartTitle.Size = new System.Drawing.Size(660, 26);
            this.labelChartTitle.Text = "Chart  ·  click a point";
            //
            // chart
            //
            this.chart.Location = new System.Drawing.Point(20, 40);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(652, 180);
            //
            // panelTrace
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Location = new System.Drawing.Point(740, 30);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(578, 488);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(360, 30);
            this.labelTraceTitle.Text = "Server WidgetEvent log";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(538, 416);
            //
            // timerStream
            //
            this.timerStream.Interval = 700;
            this.timerStream.Tick += new System.EventHandler(this.timerStream_Tick);
            //
            // Window1
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 548);
            this.Controls.Add(this.panelGauge);
            this.Controls.Add(this.panelKnob);
            this.Controls.Add(this.panelChart);
            this.Controls.Add(this.panelTrace);
            this.Name = "Window1";
            this.Text = "IntegrationLab — Event Demo";
            this.Load += new System.EventHandler(this.Window1_Load);
            this.panelGauge.ResumeLayout(false);
            this.panelKnob.ResumeLayout(false);
            this.panelChart.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGauge;
        private Wisej.Web.Label labelGaugeTitle;
        private IntegrationLab.Widgets.GaugeWidget gauge;
        private Wisej.Web.Panel panelKnob;
        private Wisej.Web.Label labelKnobTitle;
        private IntegrationLab.Widgets.KnobWidget knob;
        private Wisej.Web.Panel panelChart;
        private Wisej.Web.Label labelChartTitle;
        private IntegrationLab.Widgets.ChartWidget chart;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Timer timerStream;
    }
}
