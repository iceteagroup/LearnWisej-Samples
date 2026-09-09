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
            this.components = new System.ComponentModel.Container();
            this.panelWidgets = new Wisej.Web.Panel();
            this.labelTitle = new Wisej.Web.Label();
            this.labelChip = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.labelGaugeTitle = new Wisej.Web.Label();
            this.labelKnobTitle = new Wisej.Web.Label();
            this.gauge = new Wisej.Web.Widget();
            this.knob = new Wisej.Web.Widget();
            this.labelAlarm = new Wisej.Web.Label();
            this.labelState = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.labelServer = new Wisej.Web.Label();
            this.buttonHigh = new Wisej.Web.Button();
            this.buttonPeak = new Wisej.Web.Button();
            this.buttonIdle = new Wisej.Web.Button();
            this.buttonStream = new Wisej.Web.Button();
            this.buttonNested = new Wisej.Web.Button();
            this.buttonNotify = new Wisej.Web.Button();
            this.buttonRecreate = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerStream = new Wisej.Web.Timer(this.components);
            this.panelWidgets.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelWidgets  (the card with the two one-off widgets)
            //
            this.panelWidgets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelWidgets.BackColor = System.Drawing.Color.White;
            this.panelWidgets.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelWidgets.Controls.Add(this.labelTitle);
            this.panelWidgets.Controls.Add(this.labelChip);
            this.panelWidgets.Controls.Add(this.labelStatus);
            this.panelWidgets.Controls.Add(this.labelGaugeTitle);
            this.panelWidgets.Controls.Add(this.labelKnobTitle);
            this.panelWidgets.Controls.Add(this.gauge);
            this.panelWidgets.Controls.Add(this.knob);
            this.panelWidgets.Controls.Add(this.labelAlarm);
            this.panelWidgets.Controls.Add(this.labelState);
            this.panelWidgets.Location = new System.Drawing.Point(30, 30);
            this.panelWidgets.Name = "panelWidgets";
            this.panelWidgets.Size = new System.Drawing.Size(720, 560);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 18);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(290, 30);
            this.labelTitle.Text = "One-off widgets — prototype";
            //
            // labelChip
            //
            this.labelChip.AutoSize = false;
            this.labelChip.BackColor = System.Drawing.Color.FromArgb(241, 238, 252);
            this.labelChip.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelChip.ForeColor = System.Drawing.Color.FromArgb(91, 70, 201);
            this.labelChip.Location = new System.Drawing.Point(322, 21);
            this.labelChip.Name = "labelChip";
            this.labelChip.Size = new System.Drawing.Size(150, 24);
            this.labelChip.Text = "Wisej.Web.Widget ×2";
            this.labelChip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(480, 20);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(216, 26);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // labelGaugeTitle / labelKnobTitle
            //
            this.labelGaugeTitle.AutoSize = false;
            this.labelGaugeTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelGaugeTitle.Location = new System.Drawing.Point(24, 58);
            this.labelGaugeTitle.Name = "labelGaugeTitle";
            this.labelGaugeTitle.Size = new System.Drawing.Size(330, 22);
            this.labelGaugeTitle.Text = "Gauge widget";
            this.labelKnobTitle.AutoSize = false;
            this.labelKnobTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelKnobTitle.Location = new System.Drawing.Point(376, 58);
            this.labelKnobTitle.Name = "labelKnobTitle";
            this.labelKnobTitle.Size = new System.Drawing.Size(320, 22);
            this.labelKnobTitle.Text = "Knob widget";
            //
            // gauge  (plain Wisej.Web.Widget: Packages + Options + InitScript, no wrapper class)
            //
            this.gauge.Location = new System.Drawing.Point(24, 82);
            this.gauge.Name = "gauge";
            this.gauge.Size = new System.Drawing.Size(330, 280);
            // Packages: everything THIS widget needs, in load order. Stylesheets are packages too.
            this.gauge.Packages.Add(new Wisej.Web.Widget.Package { Name = "vendor-gauge", Source = "wwwroot/vendor-gauge.js" });
            this.gauge.Packages.Add(new Wisej.Web.Widget.Package { Name = "vendor-gauge-css", Source = "wwwroot/vendor-gauge.css" });
            // InitScript: init(options) / update(options, old) / flash(), embedded in the assembly.
            this.gauge.InitScript = IntegrationLab.EmbeddedScript.Read("IntegrationLab.wwwroot.gauge-init.js");
            // Events the client adapter may raise (the documented contract).
            this.gauge.WiredEvents = new string[] { "initialized", "recreated", "error" };
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
            this.knob.Location = new System.Drawing.Point(376, 82);
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
            // labelAlarm  (banner: appears and disappears)
            //
            this.labelAlarm.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelAlarm.AutoSize = false;
            this.labelAlarm.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelAlarm.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelAlarm.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelAlarm.Location = new System.Drawing.Point(24, 376);
            this.labelAlarm.Name = "labelAlarm";
            this.labelAlarm.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelAlarm.Size = new System.Drawing.Size(672, 48);
            this.labelAlarm.Text = "";
            this.labelAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelAlarm.Visible = false;
            //
            // labelState  (what the server owns right now)
            //
            this.labelState.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelState.AutoSize = false;
            this.labelState.Font = new System.Drawing.Font("monospace", 9F);
            this.labelState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelState.Location = new System.Drawing.Point(24, 432);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(672, 110);
            this.labelState.Text = "";
            this.labelState.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelTrace  (Server ⇄ Client live message trace)
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Controls.Add(this.labelTraceFooter);
            this.panelTrace.Location = new System.Drawing.Point(776, 30);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(542, 560);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(502, 30);
            this.labelTraceTitle.Text = "Server ⇄ Client  ·  live message trace";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(502, 456);
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 518);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(502, 26);
            this.labelTraceFooter.Text = "Options → update(options, old)   ·   Call → queued, flushed with the next response   ·   → .NET→JS   ← JS→.NET   • server";
            //
            // panelActions  (bottom bar: the server buttons)
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.labelServer);
            this.panelActions.Controls.Add(this.buttonHigh);
            this.panelActions.Controls.Add(this.buttonPeak);
            this.panelActions.Controls.Add(this.buttonIdle);
            this.panelActions.Controls.Add(this.buttonStream);
            this.panelActions.Controls.Add(this.buttonNested);
            this.panelActions.Controls.Add(this.buttonNotify);
            this.panelActions.Controls.Add(this.buttonRecreate);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 606);
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
            // buttons: change Options and queue a Call (success path)
            //
            this.buttonHigh.Location = new System.Drawing.Point(120, 4);
            this.buttonHigh.Name = "buttonHigh";
            this.buttonHigh.Size = new System.Drawing.Size(100, 36);
            this.buttonHigh.Text = "Set \"High\"";
            this.buttonHigh.ToolTipText = "gauge.Options.value = 88; knob.Options.level = 78; gauge.Call(\"flash\")";
            this.buttonHigh.Click += new System.EventHandler(this.buttonHigh_Click);
            this.buttonPeak.Location = new System.Drawing.Point(228, 4);
            this.buttonPeak.Name = "buttonPeak";
            this.buttonPeak.Size = new System.Drawing.Size(100, 36);
            this.buttonPeak.Text = "Set \"Peak\"";
            this.buttonPeak.ToolTipText = "gauge.Options.value = 104 + tighter bands; knob.Options.level = 92; knob.Call(\"pulse\")";
            this.buttonPeak.Click += new System.EventHandler(this.buttonPeak_Click);
            this.buttonIdle.Location = new System.Drawing.Point(336, 4);
            this.buttonIdle.Name = "buttonIdle";
            this.buttonIdle.Size = new System.Drawing.Size(100, 36);
            this.buttonIdle.Text = "Set \"Idle\"";
            this.buttonIdle.ToolTipText = "gauge.Options.value = 64 + default bands; knob.Options.level = 40; gauge.Call(\"flash\")";
            this.buttonIdle.Click += new System.EventHandler(this.buttonIdle_Click);
            //
            // buttonStream (progress path)
            //
            this.buttonStream.Location = new System.Drawing.Point(460, 4);
            this.buttonStream.Name = "buttonStream";
            this.buttonStream.Size = new System.Drawing.Size(120, 36);
            this.buttonStream.Text = "▶ Stream";
            this.buttonStream.ToolTipText = "A Timer pushes values 20 → 95 → 20 through Options; update(options, old) re-syncs both widgets.";
            this.buttonStream.Click += new System.EventHandler(this.buttonStream_Click);
            //
            // failure path + recovery: nested change without notify, then Update()
            //
            this.buttonNested.Location = new System.Drawing.Point(604, 4);
            this.buttonNested.Name = "buttonNested";
            this.buttonNested.Size = new System.Drawing.Size(196, 36);
            this.buttonNested.Text = "Change nested (no notify)";
            this.buttonNested.ToolTipText = "Mutates gauge.Options.bands[0].color in place: nested changes are NOT detected, nothing renders.";
            this.buttonNested.Click += new System.EventHandler(this.buttonNested_Click);
            this.buttonNotify.Location = new System.Drawing.Point(808, 4);
            this.buttonNotify.Name = "buttonNotify";
            this.buttonNotify.Size = new System.Drawing.Size(140, 36);
            this.buttonNotify.Text = "Notify / Update()";
            this.buttonNotify.ToolTipText = "gauge.Update(): re-renders Options so the nested change reaches update(options, old).";
            this.buttonNotify.Click += new System.EventHandler(this.buttonNotify_Click);
            //
            // buttonRecreate (the option the vendor cannot take after construction)
            //
            this.buttonRecreate.Location = new System.Drawing.Point(956, 4);
            this.buttonRecreate.Name = "buttonRecreate";
            this.buttonRecreate.Size = new System.Drawing.Size(150, 36);
            this.buttonRecreate.Text = "Destroy && recreate";
            this.buttonRecreate.ToolTipText = "Toggles gauge.Options.style (card / compact): the InitScript destroys and recreates the vendor instance.";
            this.buttonRecreate.Click += new System.EventHandler(this.buttonRecreate_Click);
            //
            // buttonClear
            //
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(1178, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(110, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // timerStream  (a Component with no visual surface)
            //
            this.timerStream.Interval = 600;
            this.timerStream.Tick += new System.EventHandler(this.timerStream_Tick);
            //
            // DashboardPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelWidgets);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "DashboardPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "IntegrationLab — Dashboard";
            this.Load += new System.EventHandler(this.DashboardPage_Load);
            this.panelWidgets.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelWidgets;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label labelChip;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Label labelGaugeTitle;
        private Wisej.Web.Label labelKnobTitle;
        private Wisej.Web.Widget gauge;
        private Wisej.Web.Widget knob;
        private Wisej.Web.Label labelAlarm;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Label labelServer;
        private Wisej.Web.Button buttonHigh;
        private Wisej.Web.Button buttonPeak;
        private Wisej.Web.Button buttonIdle;
        private Wisej.Web.Button buttonStream;
        private Wisej.Web.Button buttonNested;
        private Wisej.Web.Button buttonNotify;
        private Wisej.Web.Button buttonRecreate;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerStream;
    }
}
