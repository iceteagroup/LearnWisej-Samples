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
            this.labelGaugeState = new Wisej.Web.Label();
            this.panelKnob = new Wisej.Web.Panel();
            this.labelKnobTitle = new Wisej.Web.Label();
            this.knob = new IntegrationLab.Widgets.KnobWidget();
            this.labelKnobState = new Wisej.Web.Label();
            this.panelChart = new Wisej.Web.Panel();
            this.labelChartTitle = new Wisej.Web.Label();
            this.chart = new IntegrationLab.Widgets.ChartWidget();
            this.labelChartState = new Wisej.Web.Label();
            this.labelNoise = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelBanner = new Wisej.Web.Label();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonGauge72 = new Wisej.Web.Button();
            this.buttonGauge104 = new Wisej.Web.Button();
            this.buttonKnobPlus10 = new Wisej.Web.Button();
            this.buttonChartData = new Wisej.Web.Button();
            this.buttonRecreate = new Wisej.Web.Button();
            this.buttonNoise = new Wisej.Web.Button();
            this.buttonBadPayload = new Wisej.Web.Button();
            this.buttonStream = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerStream = new Wisej.Web.Timer(this.components);
            this.panelGauge.SuspendLayout();
            this.panelKnob.SuspendLayout();
            this.panelChart.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGauge  (card 1)
            //
            this.panelGauge.BackColor = System.Drawing.Color.White;
            this.panelGauge.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGauge.Controls.Add(this.labelGaugeTitle);
            this.panelGauge.Controls.Add(this.gauge);
            this.panelGauge.Controls.Add(this.labelGaugeState);
            this.panelGauge.Location = new System.Drawing.Point(30, 30);
            this.panelGauge.Name = "panelGauge";
            this.panelGauge.Size = new System.Drawing.Size(340, 270);
            //
            // labelGaugeTitle
            //
            this.labelGaugeTitle.AutoSize = false;
            this.labelGaugeTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.labelGaugeTitle.Location = new System.Drawing.Point(16, 10);
            this.labelGaugeTitle.Name = "labelGaugeTitle";
            this.labelGaugeTitle.Size = new System.Drawing.Size(308, 26);
            this.labelGaugeTitle.Text = "Gauge  ·  thresholdCrossed { value, level }";
            //
            // gauge  (GaugeWidget: VendorGauge inside a Widget container)
            //
            this.gauge.Label = "VendorGauge";
            this.gauge.Location = new System.Drawing.Point(20, 40);
            this.gauge.Name = "gauge";
            this.gauge.Size = new System.Drawing.Size(300, 186);
            this.gauge.Threshold = 100D;
            this.gauge.Value = 72D;
            this.gauge.WarnAt = 85D;
            //
            // labelGaugeState
            //
            this.labelGaugeState.AutoSize = false;
            this.labelGaugeState.Font = new System.Drawing.Font("monospace", 8F);
            this.labelGaugeState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelGaugeState.Location = new System.Drawing.Point(16, 232);
            this.labelGaugeState.Name = "labelGaugeState";
            this.labelGaugeState.Size = new System.Drawing.Size(308, 30);
            this.labelGaugeState.Text = "";
            this.labelGaugeState.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelKnob  (card 2)
            //
            this.panelKnob.BackColor = System.Drawing.Color.White;
            this.panelKnob.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelKnob.Controls.Add(this.labelKnobTitle);
            this.panelKnob.Controls.Add(this.knob);
            this.panelKnob.Controls.Add(this.labelKnobState);
            this.panelKnob.Location = new System.Drawing.Point(382, 30);
            this.panelKnob.Name = "panelKnob";
            this.panelKnob.Size = new System.Drawing.Size(340, 270);
            //
            // labelKnobTitle
            //
            this.labelKnobTitle.AutoSize = false;
            this.labelKnobTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.labelKnobTitle.Location = new System.Drawing.Point(16, 10);
            this.labelKnobTitle.Name = "labelKnobTitle";
            this.labelKnobTitle.Size = new System.Drawing.Size(308, 26);
            this.labelKnobTitle.Text = "Knob  ·  valueChanged { value, source }  ·  drag it";
            //
            // knob  (KnobWidget: jQuery-style VendorKnob plugin on an <input>)
            //
            this.knob.Label = "Pressure";
            this.knob.Location = new System.Drawing.Point(80, 40);
            this.knob.Name = "knob";
            this.knob.Size = new System.Drawing.Size(180, 186);
            this.knob.Value = 50D;
            //
            // labelKnobState
            //
            this.labelKnobState.AutoSize = false;
            this.labelKnobState.Font = new System.Drawing.Font("monospace", 8F);
            this.labelKnobState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelKnobState.Location = new System.Drawing.Point(16, 232);
            this.labelKnobState.Name = "labelKnobState";
            this.labelKnobState.Size = new System.Drawing.Size(308, 30);
            this.labelKnobState.Text = "";
            this.labelKnobState.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelChart  (card 3)
            //
            this.panelChart.BackColor = System.Drawing.Color.White;
            this.panelChart.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelChart.Controls.Add(this.labelChartTitle);
            this.panelChart.Controls.Add(this.chart);
            this.panelChart.Controls.Add(this.labelChartState);
            this.panelChart.Controls.Add(this.labelNoise);
            this.panelChart.Location = new System.Drawing.Point(30, 312);
            this.panelChart.Name = "panelChart";
            this.panelChart.Size = new System.Drawing.Size(692, 278);
            //
            // labelChartTitle
            //
            this.labelChartTitle.AutoSize = false;
            this.labelChartTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.labelChartTitle.Location = new System.Drawing.Point(16, 10);
            this.labelChartTitle.Name = "labelChartTitle";
            this.labelChartTitle.Size = new System.Drawing.Size(660, 26);
            this.labelChartTitle.Text = "Chart  ·  click a point  ·  pointClicked { index, label, value }";
            //
            // chart  (ChartWidget: VendorChart, subscribes to every vendor event, forwards one)
            //
            this.chart.Location = new System.Drawing.Point(20, 40);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(652, 180);
            //
            // labelChartState
            //
            this.labelChartState.AutoSize = false;
            this.labelChartState.Font = new System.Drawing.Font("monospace", 8F);
            this.labelChartState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelChartState.Location = new System.Drawing.Point(16, 226);
            this.labelChartState.Name = "labelChartState";
            this.labelChartState.Size = new System.Drawing.Size(660, 22);
            this.labelChartState.Text = "hover, wheel-zoom, legend clicks and renders stay in the browser — only a point click reaches .NET";
            this.labelChartState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelNoise
            //
            this.labelNoise.AutoSize = false;
            this.labelNoise.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.labelNoise.ForeColor = System.Drawing.Color.FromArgb(13, 71, 140);
            this.labelNoise.Location = new System.Drawing.Point(16, 248);
            this.labelNoise.Name = "labelNoise";
            this.labelNoise.Size = new System.Drawing.Size(660, 22);
            this.labelNoise.Text = "events kept in the browser: —   (press “Noise counter”)";
            this.labelNoise.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panelTrace  (Server WidgetEvent log)
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.labelStatus);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Controls.Add(this.labelBanner);
            this.panelTrace.Controls.Add(this.labelTraceFooter);
            this.panelTrace.Location = new System.Drawing.Point(740, 30);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(578, 560);
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
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(392, 16);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(166, 26);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(538, 400);
            //
            // labelBanner  (last .NET event raised)
            //
            this.labelBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(232, 243, 255);
            this.labelBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(13, 71, 140);
            this.labelBanner.Location = new System.Drawing.Point(20, 462);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(538, 48);
            this.labelBanner.Text = "";
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 516);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(538, 34);
            this.labelTraceFooter.Text = "Hover, zoom, render and layout events stay in the browser — only thresholdCrossed, valueChanged and pointClicked reach .NET.   → .NET→JS   ← JS→.NET   • .NET   ✖ rejected";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonGauge72);
            this.panelActions.Controls.Add(this.buttonGauge104);
            this.panelActions.Controls.Add(this.buttonKnobPlus10);
            this.panelActions.Controls.Add(this.buttonChartData);
            this.panelActions.Controls.Add(this.buttonRecreate);
            this.panelActions.Controls.Add(this.buttonNoise);
            this.panelActions.Controls.Add(this.buttonBadPayload);
            this.panelActions.Controls.Add(this.buttonStream);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 606);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // gauge buttons (success path: server sets Value, the gauge crosses the line once)
            //
            this.buttonGauge72.Location = new System.Drawing.Point(0, 4);
            this.buttonGauge72.Name = "buttonGauge72";
            this.buttonGauge72.Size = new System.Drawing.Size(96, 36);
            this.buttonGauge72.Text = "Gauge 72";
            this.buttonGauge72.ToolTipText = "Server sets gauge.Value = 72 (below both lines: no event).";
            this.buttonGauge72.Click += new System.EventHandler(this.buttonGauge72_Click);
            this.buttonGauge104.Location = new System.Drawing.Point(104, 4);
            this.buttonGauge104.Name = "buttonGauge104";
            this.buttonGauge104.Size = new System.Drawing.Size(96, 36);
            this.buttonGauge104.Text = "Gauge 104";
            this.buttonGauge104.ToolTipText = "Server sets gauge.Value = 104: the adapter fires ONE thresholdCrossed { value: 104, level: \"high\" }.";
            this.buttonGauge104.Click += new System.EventHandler(this.buttonGauge104_Click);
            //
            // knob button (server-driven valueChanged, source = "server")
            //
            this.buttonKnobPlus10.Location = new System.Drawing.Point(224, 4);
            this.buttonKnobPlus10.Name = "buttonKnobPlus10";
            this.buttonKnobPlus10.Size = new System.Drawing.Size(96, 36);
            this.buttonKnobPlus10.Text = "Knob +10";
            this.buttonKnobPlus10.ToolTipText = "Server sets knob.Value += 10: the vendor fires inside update(), the adapter reports valueChanged { source: \"server\" } via the deferred handler.";
            this.buttonKnobPlus10.Click += new System.EventHandler(this.buttonKnobPlus10_Click);
            //
            // chart buttons
            //
            this.buttonChartData.Location = new System.Drawing.Point(344, 4);
            this.buttonChartData.Name = "buttonChartData";
            this.buttonChartData.Size = new System.Drawing.Size(130, 36);
            this.buttonChartData.Text = "Chart: new data";
            this.buttonChartData.ToolTipText = "Server replaces labels/series (first-level Options change): setData() on the vendor; its render event stays in the browser.";
            this.buttonChartData.Click += new System.EventHandler(this.buttonChartData_Click);
            this.buttonRecreate.Location = new System.Drawing.Point(482, 4);
            this.buttonRecreate.Name = "buttonRecreate";
            this.buttonRecreate.Size = new System.Drawing.Size(180, 36);
            this.buttonRecreate.Text = "Destroy && recreate chart";
            this.buttonRecreate.ToolTipText = "Toggles the theme; the vendor cannot change it in place, so the adapter destroys + recreates the instance and re-runs wire().";
            this.buttonRecreate.Click += new System.EventHandler(this.buttonRecreate_Click);
            this.buttonNoise.Location = new System.Drawing.Point(670, 4);
            this.buttonNoise.Name = "buttonNoise";
            this.buttonNoise.Size = new System.Drawing.Size(120, 36);
            this.buttonNoise.Text = "Noise counter";
            this.buttonNoise.ToolTipText = "CallAsync(\"getNoiseCount\"): how many vendor events the adapter kept in the browser.";
            this.buttonNoise.Click += new System.EventHandler(this.buttonNoise_Click);
            //
            // failure path
            //
            this.buttonBadPayload.Location = new System.Drawing.Point(814, 4);
            this.buttonBadPayload.Name = "buttonBadPayload";
            this.buttonBadPayload.Size = new System.Drawing.Size(110, 36);
            this.buttonBadPayload.Text = "Bad payload";
            this.buttonBadPayload.ToolTipText = "Call(\"fireBadPayload\"): the adapter fires pointClicked { index: -1 }; server validation rejects it.";
            this.buttonBadPayload.Click += new System.EventHandler(this.buttonBadPayload_Click);
            //
            // progress path
            //
            this.buttonStream.Location = new System.Drawing.Point(948, 4);
            this.buttonStream.Name = "buttonStream";
            this.buttonStream.Size = new System.Drawing.Size(100, 36);
            this.buttonStream.Text = "▶ Stream";
            this.buttonStream.ToolTipText = "A Timer replays gauge readings; thresholdCrossed fires once per crossing, not once per reading.";
            this.buttonStream.Click += new System.EventHandler(this.buttonStream_Click);
            //
            // buttonClear
            //
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(1188, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(100, 36);
            this.buttonClear.Text = "Clear log";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // timerStream  (a Component with no visual surface)
            //
            this.timerStream.Interval = 700;
            this.timerStream.Tick += new System.EventHandler(this.timerStream_Tick);
            //
            // Window1
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelGauge);
            this.Controls.Add(this.panelKnob);
            this.Controls.Add(this.panelChart);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "Window1";
            this.Text = "IntegrationLab — Event Demo";
            this.Load += new System.EventHandler(this.Window1_Load);
            this.panelGauge.ResumeLayout(false);
            this.panelKnob.ResumeLayout(false);
            this.panelChart.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGauge;
        private Wisej.Web.Label labelGaugeTitle;
        private IntegrationLab.Widgets.GaugeWidget gauge;
        private Wisej.Web.Label labelGaugeState;
        private Wisej.Web.Panel panelKnob;
        private Wisej.Web.Label labelKnobTitle;
        private IntegrationLab.Widgets.KnobWidget knob;
        private Wisej.Web.Label labelKnobState;
        private Wisej.Web.Panel panelChart;
        private Wisej.Web.Label labelChartTitle;
        private IntegrationLab.Widgets.ChartWidget chart;
        private Wisej.Web.Label labelChartState;
        private Wisej.Web.Label labelNoise;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonGauge72;
        private Wisej.Web.Button buttonGauge104;
        private Wisej.Web.Button buttonKnobPlus10;
        private Wisej.Web.Button buttonChartData;
        private Wisej.Web.Button buttonRecreate;
        private Wisej.Web.Button buttonNoise;
        private Wisej.Web.Button buttonBadPayload;
        private Wisej.Web.Button buttonStream;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerStream;
    }
}
