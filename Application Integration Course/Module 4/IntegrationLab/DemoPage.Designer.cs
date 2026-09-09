namespace IntegrationLab
{
    partial class DemoPage
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
            this.simpleGauge1 = new IntegrationLab.Controls.SimpleGauge();
            this.simpleGauge2 = new IntegrationLab.Controls.SimpleGauge();
            this.labelSecond = new Wisej.Web.Label();
            this.labelAlarm = new Wisej.Web.Label();
            this.labelState = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonValue72 = new Wisej.Web.Button();
            this.buttonValue90 = new Wisej.Web.Button();
            this.buttonMax120 = new Wisej.Web.Button();
            this.buttonValue78 = new Wisej.Web.Button();
            this.buttonInvalid = new Wisej.Web.Button();
            this.buttonToggleAnimation = new Wisej.Web.Button();
            this.buttonStream = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerStream = new Wisej.Web.Timer(this.components);
            this.panelGauge.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGauge  (the gauge card)
            //
            this.panelGauge.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelGauge.BackColor = System.Drawing.Color.White;
            this.panelGauge.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGauge.Controls.Add(this.labelTitle);
            this.panelGauge.Controls.Add(this.labelStatus);
            this.panelGauge.Controls.Add(this.simpleGauge1);
            this.panelGauge.Controls.Add(this.simpleGauge2);
            this.panelGauge.Controls.Add(this.labelSecond);
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
            this.labelTitle.Size = new System.Drawing.Size(320, 30);
            this.labelTitle.Text = "SimpleGauge — from the Toolbox";
            //
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(350, 20);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(186, 26);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // simpleGauge1  (dropped from the Toolbox: typed properties only)
            //
            this.simpleGauge1.Caption = "Boiler 3";
            this.simpleGauge1.Location = new System.Drawing.Point(40, 56);
            this.simpleGauge1.Name = "simpleGauge1";
            this.simpleGauge1.Size = new System.Drawing.Size(480, 250);
            this.simpleGauge1.Value = 72D;
            this.simpleGauge1.ValueChanged += new System.EventHandler<IntegrationLab.Controls.GaugeValueChangedEventArgs>(this.simpleGauge_ValueChanged);
            this.simpleGauge1.ThresholdExceeded += new System.EventHandler<IntegrationLab.Controls.GaugeEventArgs>(this.simpleGauge_ThresholdExceeded);
            this.simpleGauge1.WidgetError += new System.EventHandler<IntegrationLab.Controls.GaugeErrorEventArgs>(this.simpleGauge_WidgetError);
            this.simpleGauge1.Trace += new System.EventHandler<IntegrationLab.Controls.TraceEventArgs>(this.simpleGauge_Trace);
            //
            // simpleGauge2  (a second instance of the same class, different typed properties)
            //
            this.simpleGauge2.Caption = "Chiller 1";
            this.simpleGauge2.Location = new System.Drawing.Point(40, 314);
            this.simpleGauge2.Maximum = 60D;
            this.simpleGauge2.Name = "simpleGauge2";
            this.simpleGauge2.Size = new System.Drawing.Size(200, 100);
            this.simpleGauge2.Threshold = 50D;
            this.simpleGauge2.Value = 18D;
            this.simpleGauge2.ValueChanged += new System.EventHandler<IntegrationLab.Controls.GaugeValueChangedEventArgs>(this.simpleGauge_ValueChanged);
            this.simpleGauge2.ThresholdExceeded += new System.EventHandler<IntegrationLab.Controls.GaugeEventArgs>(this.simpleGauge_ThresholdExceeded);
            this.simpleGauge2.WidgetError += new System.EventHandler<IntegrationLab.Controls.GaugeErrorEventArgs>(this.simpleGauge_WidgetError);
            this.simpleGauge2.Trace += new System.EventHandler<IntegrationLab.Controls.TraceEventArgs>(this.simpleGauge_Trace);
            //
            // labelSecond
            //
            this.labelSecond.AutoSize = false;
            this.labelSecond.Font = new System.Drawing.Font("monospace", 9F);
            this.labelSecond.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelSecond.Location = new System.Drawing.Point(256, 314);
            this.labelSecond.Name = "labelSecond";
            this.labelSecond.Size = new System.Drawing.Size(280, 100);
            this.labelSecond.Text = "simpleGauge2 — same class, second instance.\nOnly typed properties differ:\nCaption = \"Chiller 1\"\nMaximum = 60, Threshold = 50\nNo InitScript. No Packages. No vendor name.";
            this.labelSecond.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelAlarm
            //
            this.labelAlarm.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelAlarm.AutoSize = false;
            this.labelAlarm.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelAlarm.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelAlarm.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelAlarm.Location = new System.Drawing.Point(24, 420);
            this.labelAlarm.Name = "labelAlarm";
            this.labelAlarm.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelAlarm.Size = new System.Drawing.Size(512, 44);
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
            this.labelState.Location = new System.Drawing.Point(24, 470);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(512, 76);
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
            this.labelTraceFooter.Text = "page: typed properties only   ·   wrapper: Options + client calls   ·   → .NET→JS   ← JS→.NET   • server";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonValue72);
            this.panelActions.Controls.Add(this.buttonValue90);
            this.panelActions.Controls.Add(this.buttonMax120);
            this.panelActions.Controls.Add(this.buttonValue78);
            this.panelActions.Controls.Add(this.buttonInvalid);
            this.panelActions.Controls.Add(this.buttonToggleAnimation);
            this.panelActions.Controls.Add(this.buttonStream);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 606);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // buttons: the walkthrough statements (success path)
            //
            this.buttonValue72.Location = new System.Drawing.Point(0, 4);
            this.buttonValue72.Name = "buttonValue72";
            this.buttonValue72.Size = new System.Drawing.Size(104, 36);
            this.buttonValue72.Text = "Value = 72";
            this.buttonValue72.Click += new System.EventHandler(this.buttonValue72_Click);
            this.buttonValue90.Location = new System.Drawing.Point(112, 4);
            this.buttonValue90.Name = "buttonValue90";
            this.buttonValue90.Size = new System.Drawing.Size(104, 36);
            this.buttonValue90.Text = "Value = 90";
            this.buttonValue90.ToolTipText = "Crosses Threshold (85): ThresholdExceeded fires in C#.";
            this.buttonValue90.Click += new System.EventHandler(this.buttonValue90_Click);
            this.buttonMax120.Location = new System.Drawing.Point(224, 4);
            this.buttonMax120.Name = "buttonMax120";
            this.buttonMax120.Size = new System.Drawing.Size(206, 36);
            this.buttonMax120.Text = "Maximum = 120; Value = 45";
            this.buttonMax120.ToolTipText = "Two typed properties, one client update.";
            this.buttonMax120.Click += new System.EventHandler(this.buttonMax120_Click);
            this.buttonValue78.Location = new System.Drawing.Point(438, 4);
            this.buttonValue78.Name = "buttonValue78";
            this.buttonValue78.Size = new System.Drawing.Size(104, 36);
            this.buttonValue78.Text = "Value = 78";
            this.buttonValue78.Click += new System.EventHandler(this.buttonValue78_Click);
            //
            // failure path
            //
            this.buttonInvalid.Location = new System.Drawing.Point(566, 4);
            this.buttonInvalid.Name = "buttonInvalid";
            this.buttonInvalid.Size = new System.Drawing.Size(156, 36);
            this.buttonInvalid.Text = "Value = 200 (invalid)";
            this.buttonInvalid.ToolTipText = "The typed setter throws ArgumentOutOfRangeException; nothing is rendered.";
            this.buttonInvalid.Click += new System.EventHandler(this.buttonInvalid_Click);
            //
            // buttonToggleAnimation (visual property)
            //
            this.buttonToggleAnimation.Location = new System.Drawing.Point(730, 4);
            this.buttonToggleAnimation.Name = "buttonToggleAnimation";
            this.buttonToggleAnimation.Size = new System.Drawing.Size(186, 36);
            this.buttonToggleAnimation.Text = "AnimationEnabled = false";
            this.buttonToggleAnimation.ToolTipText = "Toggles AnimationEnabled on both gauges: sweep vs jump.";
            this.buttonToggleAnimation.Click += new System.EventHandler(this.buttonToggleAnimation_Click);
            //
            // buttonStream (progress path)
            //
            this.buttonStream.Location = new System.Drawing.Point(924, 4);
            this.buttonStream.Name = "buttonStream";
            this.buttonStream.Size = new System.Drawing.Size(170, 36);
            this.buttonStream.Text = "▶ Stream both gauges";
            this.buttonStream.Click += new System.EventHandler(this.buttonStream_Click);
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
            this.timerStream.Interval = 700;
            this.timerStream.Tick += new System.EventHandler(this.timerStream_Tick);
            //
            // DemoPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelGauge);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "DemoPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "IntegrationLab — Demo";
            this.Load += new System.EventHandler(this.DemoPage_Load);
            this.panelGauge.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGauge;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label labelStatus;
        private IntegrationLab.Controls.SimpleGauge simpleGauge1;
        private IntegrationLab.Controls.SimpleGauge simpleGauge2;
        private Wisej.Web.Label labelSecond;
        private Wisej.Web.Label labelAlarm;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonValue72;
        private Wisej.Web.Button buttonValue90;
        private Wisej.Web.Button buttonMax120;
        private Wisej.Web.Button buttonValue78;
        private Wisej.Web.Button buttonInvalid;
        private Wisej.Web.Button buttonToggleAnimation;
        private Wisej.Web.Button buttonStream;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerStream;
    }
}
