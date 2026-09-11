namespace IntegrationLab
{
    partial class DashboardPage
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
            this.panelActions = new Wisej.Web.Panel();
            this.labelServer = new Wisej.Web.Label();
            this.buttonHigh = new Wisej.Web.Button();
            this.buttonPeak = new Wisej.Web.Button();
            this.buttonIdle = new Wisej.Web.Button();
            this.panelWidgets = new Wisej.Web.Panel();
            this.labelGaugeTitle = new Wisej.Web.Label();
            this.labelKnobTitle = new Wisej.Web.Label();
            this.gauge = new Wisej.Web.Widget();
            this.knob = new Wisej.Web.Widget();
            this.labelAlarm = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.panelActions.SuspendLayout();
            this.panelWidgets.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.SuspendLayout();
            //
            // panelActions  (server toolbar)
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.labelServer);
            this.panelActions.Controls.Add(this.buttonHigh);
            this.panelActions.Controls.Add(this.buttonPeak);
            this.panelActions.Controls.Add(this.buttonIdle);
            this.panelActions.Location = new System.Drawing.Point(30, 20);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // labelServer
            //
            this.labelServer.AutoSize = false;
            this.labelServer.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelServer.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelServer.Location = new System.Drawing.Point(0, 4);
            this.labelServer.Name = "labelServer";
            this.labelServer.Size = new System.Drawing.Size(116, 36);
            this.labelServer.Text = "SERVER (.NET) ⟶";
            this.labelServer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonHigh
            //
            this.buttonHigh.Location = new System.Drawing.Point(120, 4);
            this.buttonHigh.Name = "buttonHigh";
            this.buttonHigh.Size = new System.Drawing.Size(100, 36);
            this.buttonHigh.Text = "Set \"High\"";
            this.buttonHigh.Click += new System.EventHandler(this.buttonHigh_Click);
            //
            // buttonPeak
            //
            this.buttonPeak.Location = new System.Drawing.Point(228, 4);
            this.buttonPeak.Name = "buttonPeak";
            this.buttonPeak.Size = new System.Drawing.Size(100, 36);
            this.buttonPeak.Text = "Set \"Peak\"";
            this.buttonPeak.Click += new System.EventHandler(this.buttonPeak_Click);
            //
            // buttonIdle
            //
            this.buttonIdle.Location = new System.Drawing.Point(336, 4);
            this.buttonIdle.Name = "buttonIdle";
            this.buttonIdle.Size = new System.Drawing.Size(100, 36);
            this.buttonIdle.Text = "Set \"Idle\"";
            this.buttonIdle.Click += new System.EventHandler(this.buttonIdle_Click);
            //
            // panelWidgets  (the card with the two one-off widgets)
            //
            this.panelWidgets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelWidgets.BackColor = System.Drawing.Color.White;
            this.panelWidgets.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelWidgets.Controls.Add(this.labelGaugeTitle);
            this.panelWidgets.Controls.Add(this.labelKnobTitle);
            this.panelWidgets.Controls.Add(this.gauge);
            this.panelWidgets.Controls.Add(this.knob);
            this.panelWidgets.Controls.Add(this.labelAlarm);
            this.panelWidgets.Location = new System.Drawing.Point(30, 76);
            this.panelWidgets.Name = "panelWidgets";
            this.panelWidgets.Size = new System.Drawing.Size(720, 400);
            //
            // labelGaugeTitle
            //
            this.labelGaugeTitle.AutoSize = false;
            this.labelGaugeTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelGaugeTitle.Location = new System.Drawing.Point(24, 16);
            this.labelGaugeTitle.Name = "labelGaugeTitle";
            this.labelGaugeTitle.Size = new System.Drawing.Size(330, 22);
            this.labelGaugeTitle.Text = "Gauge widget";
            //
            // labelKnobTitle
            //
            this.labelKnobTitle.AutoSize = false;
            this.labelKnobTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelKnobTitle.Location = new System.Drawing.Point(376, 16);
            this.labelKnobTitle.Name = "labelKnobTitle";
            this.labelKnobTitle.Size = new System.Drawing.Size(320, 22);
            this.labelKnobTitle.Text = "Knob widget";
            //
            // gauge  (plain Wisej.Web.Widget: Packages + Options + InitScript, no wrapper class)
            //
            this.gauge.Location = new System.Drawing.Point(24, 40);
            this.gauge.Name = "gauge";
            this.gauge.Size = new System.Drawing.Size(330, 280);
            // Packages: everything THIS widget needs, in load order. Stylesheets are packages too.
            this.gauge.Packages.Add(new Wisej.Web.Widget.Package { Name = "vendor-gauge", Source = "wwwroot/vendor-gauge.js" });
            this.gauge.Packages.Add(new Wisej.Web.Widget.Package { Name = "vendor-gauge-css", Source = "wwwroot/vendor-gauge.css" });
            // InitScript: init(options) / update(options, old) / flash(), embedded in the assembly.
            this.gauge.InitScript = IntegrationLab.EmbeddedScript.Read("IntegrationLab.wwwroot.gauge-init.js");
            // Events the client adapter may raise (the documented contract).
            this.gauge.WiredEvents = new string[] { "error" };
            // Options: .NET values serialized to the client. Property names are camel-cased
            // on the way out (MinValue → minValue); the InitScript uses the JavaScript spelling.
            dynamic gaugeOptions = this.gauge.Options;
            gaugeOptions.value = 72D;
            gaugeOptions.range = new { MinValue = 0D, MaxValue = 120D };
            gaugeOptions.bands = DashboardPage.DefaultBands();
            gaugeOptions.label = new { Text = "Boiler 3 — temperature", Units = "°F" };
            gaugeOptions.style = "card";
            //
            // knob  (plain Wisej.Web.Widget hosting a jQuery-style plugin)
            //
            this.knob.Location = new System.Drawing.Point(376, 40);
            this.knob.Name = "knob";
            this.knob.Size = new System.Drawing.Size(320, 280);
            // Packages in order: jQuery FIRST (vendor-knob.js throws if jQuery is missing), then the
            // stylesheet, then the plugin. Listed here even though another widget could have loaded
            // jQuery already: ordering across widgets is not guaranteed, a widget lists its own needs.
            this.knob.Packages.Add(new Wisej.Web.Widget.Package { Name = "jquery", Source = "wwwroot/jquery-lite.js" });
            this.knob.Packages.Add(new Wisej.Web.Widget.Package { Name = "knob-css", Source = "wwwroot/knob.css" });
            this.knob.Packages.Add(new Wisej.Web.Widget.Package { Name = "vendor-knob", Source = "wwwroot/vendor-knob.js" });
            this.knob.InitScript = IntegrationLab.EmbeddedScript.Read("IntegrationLab.wwwroot.knob-init.js");
            this.knob.WiredEvents = new string[] { "valueChanged", "error" };
            dynamic knobOptions = this.knob.Options;
            knobOptions.level = 60D;
            knobOptions.min = 0D;
            knobOptions.max = 100D;
            knobOptions.step = 1D;
            knobOptions.label = "Pump speed";
            knobOptions.units = "%";
            knobOptions.color = "#1a86ff";
            //
            // labelAlarm
            //
            this.labelAlarm.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelAlarm.AutoSize = false;
            this.labelAlarm.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelAlarm.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelAlarm.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelAlarm.Location = new System.Drawing.Point(24, 334);
            this.labelAlarm.Name = "labelAlarm";
            this.labelAlarm.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelAlarm.Size = new System.Drawing.Size(672, 48);
            this.labelAlarm.Text = "";
            this.labelAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelAlarm.Visible = false;
            //
            // panelTrace  (what the server sends to the client)
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Location = new System.Drawing.Point(776, 76);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(542, 400);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(502, 30);
            this.labelTraceTitle.Text = "⟶ Client  ·  options & queued calls";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(502, 332);
            //
            // DashboardPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelActions);
            this.Controls.Add(this.panelWidgets);
            this.Controls.Add(this.panelTrace);
            this.Name = "DashboardPage";
            this.Size = new System.Drawing.Size(1348, 496);
            this.Text = "IntegrationLab — Dashboard";
            this.Load += new System.EventHandler(this.DashboardPage_Load);
            this.panelActions.ResumeLayout(false);
            this.panelWidgets.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Label labelServer;
        private Wisej.Web.Button buttonHigh;
        private Wisej.Web.Button buttonPeak;
        private Wisej.Web.Button buttonIdle;
        private Wisej.Web.Panel panelWidgets;
        private Wisej.Web.Label labelGaugeTitle;
        private Wisej.Web.Label labelKnobTitle;
        private Wisej.Web.Widget gauge;
        private Wisej.Web.Widget knob;
        private Wisej.Web.Label labelAlarm;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
    }
}
