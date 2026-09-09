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
            this.labelTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.labelKnob = new Wisej.Web.Label();
            this.gaugeKnob = new Wisej.Web.Widget();
            this.labelKnobBroken = new Wisej.Web.Label();
            this.gaugeKnobBroken = new Wisej.Web.Widget();
            this.labelWrongOrder = new Wisej.Web.Label();
            this.panelWrongOrder = new Wisej.Web.Panel();
            this.labelAlarm = new Wisej.Web.Label();
            this.labelState = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonSet25 = new Wisej.Web.Button();
            this.buttonSet60 = new Wisej.Web.Button();
            this.buttonSet85 = new Wisej.Web.Button();
            this.buttonStream = new Wisej.Web.Button();
            this.buttonWrongOrder = new Wisej.Web.Button();
            this.buttonProof = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerStream = new Wisej.Web.Timer(this.components);
            this.timerWrongOrder = new Wisej.Web.Timer(this.components);
            this.panelGauge.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGauge  (the knob card)
            //
            this.panelGauge.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelGauge.BackColor = System.Drawing.Color.White;
            this.panelGauge.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGauge.Controls.Add(this.labelTitle);
            this.panelGauge.Controls.Add(this.labelStatus);
            this.panelGauge.Controls.Add(this.labelKnob);
            this.panelGauge.Controls.Add(this.gaugeKnob);
            this.panelGauge.Controls.Add(this.labelKnobBroken);
            this.panelGauge.Controls.Add(this.gaugeKnobBroken);
            this.panelGauge.Controls.Add(this.labelWrongOrder);
            this.panelGauge.Controls.Add(this.panelWrongOrder);
            this.panelGauge.Controls.Add(this.labelAlarm);
            this.panelGauge.Controls.Add(this.labelState);
            this.panelGauge.Location = new System.Drawing.Point(30, 30);
            this.panelGauge.Name = "panelGauge";
            this.panelGauge.Size = new System.Drawing.Size(560, 560);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 18);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(260, 30);
            this.labelTitle.Text = "Pressure knob";
            //
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(230, 20);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(306, 26);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // labelKnob  (caption of the context-safe knob)
            //
            this.labelKnob.AutoSize = false;
            this.labelKnob.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelKnob.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelKnob.Location = new System.Drawing.Point(24, 54);
            this.labelKnob.Name = "labelKnob";
            this.labelKnob.Size = new System.Drawing.Size(250, 20);
            this.labelKnob.Text = "gaugeKnob · gauge-init.js · var me = this ✓";
            //
            // gaugeKnob  (Wisej.Web.Widget: context-safe InitScript, Packages in load order)
            //
            this.gaugeKnob.Location = new System.Drawing.Point(24, 76);
            this.gaugeKnob.Name = "gaugeKnob";
            this.gaugeKnob.Packages.AddRange(new Wisej.Web.Widget.Package[] {
                new Wisej.Web.Widget.Package { Name = "jquery-lite", Source = "wwwroot/jquery-lite.js" },
                new Wisej.Web.Widget.Package { Name = "vendor-knob-css", Source = "wwwroot/vendor-knob.css" },
                new Wisej.Web.Widget.Package { Name = "vendor-knob", Source = "wwwroot/vendor-knob.js" }});
            this.gaugeKnob.Size = new System.Drawing.Size(250, 206);
            this.gaugeKnob.WiredEvents = new string[] { "valueChanged", "error" };
            this.gaugeKnob.WidgetEvent += new Wisej.Web.WidgetEventHandler(this.knob_WidgetEvent);
            //
            // labelKnobBroken  (caption of the broken knob)
            //
            this.labelKnobBroken.AutoSize = false;
            this.labelKnobBroken.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelKnobBroken.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.labelKnobBroken.Location = new System.Drawing.Point(286, 54);
            this.labelKnobBroken.Name = "labelKnobBroken";
            this.labelKnobBroken.Size = new System.Drawing.Size(250, 20);
            this.labelKnobBroken.Text = "gaugeKnobBroken · gauge-init.broken.js ✕";
            //
            // gaugeKnobBroken  (Wisej.Web.Widget: same packages, broken InitScript)
            //
            this.gaugeKnobBroken.Location = new System.Drawing.Point(286, 76);
            this.gaugeKnobBroken.Name = "gaugeKnobBroken";
            this.gaugeKnobBroken.Packages.AddRange(new Wisej.Web.Widget.Package[] {
                new Wisej.Web.Widget.Package { Name = "jquery-lite", Source = "wwwroot/jquery-lite.js" },
                new Wisej.Web.Widget.Package { Name = "vendor-knob-css", Source = "wwwroot/vendor-knob.css" },
                new Wisej.Web.Widget.Package { Name = "vendor-knob", Source = "wwwroot/vendor-knob.js" }});
            this.gaugeKnobBroken.Size = new System.Drawing.Size(250, 206);
            this.gaugeKnobBroken.WiredEvents = new string[] { "valueChanged", "contextError", "error" };
            this.gaugeKnobBroken.WidgetEvent += new Wisej.Web.WidgetEventHandler(this.knob_WidgetEvent);
            //
            // labelWrongOrder  (caption of the runtime slot)
            //
            this.labelWrongOrder.AutoSize = false;
            this.labelWrongOrder.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelWrongOrder.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelWrongOrder.Location = new System.Drawing.Point(24, 290);
            this.labelWrongOrder.Name = "labelWrongOrder";
            this.labelWrongOrder.Size = new System.Drawing.Size(512, 20);
            this.labelWrongOrder.Text = "gaugeKnobWrongOrder · created by “Wrong package order” · appears below";
            //
            // panelWrongOrder  (empty until the button creates the third widget inside it)
            //
            this.panelWrongOrder.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.panelWrongOrder.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelWrongOrder.Location = new System.Drawing.Point(24, 312);
            this.panelWrongOrder.Name = "panelWrongOrder";
            this.panelWrongOrder.Padding = new Wisej.Web.Padding(6);
            this.panelWrongOrder.Size = new System.Drawing.Size(512, 94);
            //
            // labelAlarm
            //
            this.labelAlarm.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelAlarm.AutoSize = false;
            this.labelAlarm.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelAlarm.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelAlarm.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelAlarm.Location = new System.Drawing.Point(24, 414);
            this.labelAlarm.Name = "labelAlarm";
            this.labelAlarm.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelAlarm.Size = new System.Drawing.Size(512, 54);
            this.labelAlarm.Text = "";
            this.labelAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelAlarm.Visible = false;
            //
            // labelState  (what the server owns right now + where to look in DevTools)
            //
            this.labelState.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelState.AutoSize = false;
            this.labelState.Font = new System.Drawing.Font("monospace", 9F);
            this.labelState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelState.Location = new System.Drawing.Point(24, 476);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(512, 66);
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
            this.panelTrace.Location = new System.Drawing.Point(618, 30);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(700, 560);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(660, 30);
            this.labelTraceTitle.Text = "Server ⇄ Client  ·  live message trace";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(660, 456);
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 518);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(660, 26);
            this.labelTraceFooter.Text = "state out: Options → init/update   ·   events in: fireWidgetEvent   ·   → .NET→JS   ← JS→.NET   • server";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonSet25);
            this.panelActions.Controls.Add(this.buttonSet60);
            this.panelActions.Controls.Add(this.buttonSet85);
            this.panelActions.Controls.Add(this.buttonStream);
            this.panelActions.Controls.Add(this.buttonWrongOrder);
            this.panelActions.Controls.Add(this.buttonProof);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 606);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // buttons: state out (success path)
            //
            this.buttonSet25.Location = new System.Drawing.Point(0, 4);
            this.buttonSet25.Name = "buttonSet25";
            this.buttonSet25.Size = new System.Drawing.Size(96, 36);
            this.buttonSet25.Text = "Set 25";
            this.buttonSet25.ToolTipText = "Options.value = 25 on the server → update(options, old) on both knobs.";
            this.buttonSet25.Click += new System.EventHandler(this.buttonSet25_Click);
            this.buttonSet60.Location = new System.Drawing.Point(104, 4);
            this.buttonSet60.Name = "buttonSet60";
            this.buttonSet60.Size = new System.Drawing.Size(96, 36);
            this.buttonSet60.Text = "Set 60";
            this.buttonSet60.Click += new System.EventHandler(this.buttonSet60_Click);
            this.buttonSet85.Location = new System.Drawing.Point(208, 4);
            this.buttonSet85.Name = "buttonSet85";
            this.buttonSet85.Size = new System.Drawing.Size(96, 36);
            this.buttonSet85.Text = "Set 85";
            this.buttonSet85.Click += new System.EventHandler(this.buttonSet85_Click);
            //
            // buttonStream (progress path)
            //
            this.buttonStream.Location = new System.Drawing.Point(328, 4);
            this.buttonStream.Name = "buttonStream";
            this.buttonStream.Size = new System.Drawing.Size(120, 36);
            this.buttonStream.Text = "▶ Stream";
            this.buttonStream.ToolTipText = "A Timer replays 14 readings through Options.value.";
            this.buttonStream.Click += new System.EventHandler(this.buttonStream_Click);
            //
            // buttonWrongOrder (failure path 2)
            //
            this.buttonWrongOrder.Location = new System.Drawing.Point(472, 4);
            this.buttonWrongOrder.Name = "buttonWrongOrder";
            this.buttonWrongOrder.Size = new System.Drawing.Size(180, 36);
            this.buttonWrongOrder.Text = "Wrong package order";
            this.buttonWrongOrder.ToolTipText = "Creates a third widget whose Packages list vendor-knob.js BEFORE jquery-lite.js, then checks IsLoaded after 2 s.";
            this.buttonWrongOrder.Click += new System.EventHandler(this.buttonWrongOrder_Click);
            //
            // buttonProof (deliverable 1)
            //
            this.buttonProof.Location = new System.Drawing.Point(676, 4);
            this.buttonProof.Name = "buttonProof";
            this.buttonProof.Size = new System.Drawing.Size(180, 36);
            this.buttonProof.Text = "Open plain-JS proof ↗";
            this.buttonProof.ToolTipText = "Opens wwwroot/proof/knob-proof.html in a new tab: the same plugin with no Wisej.NET.";
            this.buttonProof.Click += new System.EventHandler(this.buttonProof_Click);
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
            // timerWrongOrder  (one-shot: checks IsLoaded 2 s after the wrong-order widget was created)
            //
            this.timerWrongOrder.Interval = 2000;
            this.timerWrongOrder.Tick += new System.EventHandler(this.timerWrongOrder_Tick);
            //
            // Window1
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelGauge);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "Window1";
            this.Text = "IntegrationLab — Knob Demo";
            this.Load += new System.EventHandler(this.Window1_Load);
            this.panelGauge.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGauge;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Label labelKnob;
        private Wisej.Web.Widget gaugeKnob;
        private Wisej.Web.Label labelKnobBroken;
        private Wisej.Web.Widget gaugeKnobBroken;
        private Wisej.Web.Label labelWrongOrder;
        private Wisej.Web.Panel panelWrongOrder;
        private Wisej.Web.Label labelAlarm;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonSet25;
        private Wisej.Web.Button buttonSet60;
        private Wisej.Web.Button buttonSet85;
        private Wisej.Web.Button buttonStream;
        private Wisej.Web.Button buttonWrongOrder;
        private Wisej.Web.Button buttonProof;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerStream;
        private Wisej.Web.Timer timerWrongOrder;
    }
}
