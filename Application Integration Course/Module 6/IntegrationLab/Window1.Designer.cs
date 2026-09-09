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
            this.labelBoiler = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.gauge = new IntegrationLab.Controls.SimpleGauge();
            this.buttonSetValue72 = new Wisej.Web.Button();
            this.buttonReadSize = new Wisej.Web.Button();
            this.buttonResetAnimation = new Wisej.Web.Button();
            this.buttonGetState = new Wisej.Web.Button();
            this.labelDto = new Wisej.Web.Label();
            this.labelAlarm = new Wisej.Web.Label();
            this.labelState = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonSet104 = new Wisej.Web.Button();
            this.buttonEvalWidth = new Wisej.Web.Button();
            this.buttonPitfall = new Wisej.Web.Button();
            this.buttonLeak = new Wisej.Web.Button();
            this.buttonStream = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerStream = new Wisej.Web.Timer(this.components);
            this.panelGauge.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGauge  (the gauge card: gauge + the four lesson commands)
            //
            this.panelGauge.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelGauge.BackColor = System.Drawing.Color.White;
            this.panelGauge.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGauge.Controls.Add(this.labelBoiler);
            this.panelGauge.Controls.Add(this.labelStatus);
            this.panelGauge.Controls.Add(this.gauge);
            this.panelGauge.Controls.Add(this.buttonSetValue72);
            this.panelGauge.Controls.Add(this.buttonReadSize);
            this.panelGauge.Controls.Add(this.buttonResetAnimation);
            this.panelGauge.Controls.Add(this.buttonGetState);
            this.panelGauge.Controls.Add(this.labelDto);
            this.panelGauge.Controls.Add(this.labelAlarm);
            this.panelGauge.Controls.Add(this.labelState);
            this.panelGauge.Location = new System.Drawing.Point(30, 30);
            this.panelGauge.Name = "panelGauge";
            this.panelGauge.Size = new System.Drawing.Size(560, 580);
            //
            // labelBoiler
            //
            this.labelBoiler.AutoSize = false;
            this.labelBoiler.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelBoiler.Location = new System.Drawing.Point(24, 18);
            this.labelBoiler.Name = "labelBoiler";
            this.labelBoiler.Size = new System.Drawing.Size(260, 30);
            this.labelBoiler.Text = "Boiler 3";
            //
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(160, 20);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(376, 26);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gauge  (Widget host: the vendor gauge lives inside its container)
            //
            this.gauge.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gauge.Caption = "Boiler 3";
            this.gauge.Location = new System.Drawing.Point(40, 56);
            this.gauge.Maximum = 120D;
            this.gauge.Minimum = 40D;
            this.gauge.Name = "gauge";
            this.gauge.Size = new System.Drawing.Size(480, 244);
            this.gauge.Threshold = 100D;
            this.gauge.Value = 72D;
            this.gauge.WarnAt = 85D;
            //
            // the four lesson commands (matches the walkthrough's finished screen)
            //
            this.buttonSetValue72.Location = new System.Drawing.Point(40, 312);
            this.buttonSetValue72.Name = "buttonSetValue72";
            this.buttonSetValue72.Size = new System.Drawing.Size(232, 36);
            this.buttonSetValue72.Text = "Set value (72)";
            this.buttonSetValue72.ToolTipText = "Call(\"setValue\", 72): one-way, queued, the server does not wait.";
            this.buttonSetValue72.Click += new System.EventHandler(this.buttonSetValue72_Click);
            this.buttonReadSize.Location = new System.Drawing.Point(288, 312);
            this.buttonReadSize.Name = "buttonReadSize";
            this.buttonReadSize.Size = new System.Drawing.Size(232, 36);
            this.buttonReadSize.Text = "Read rendered size";
            this.buttonReadSize.ToolTipText = "await CallAsync(\"getRenderedSize\"): the next statement needs the value.";
            this.buttonReadSize.Click += new System.EventHandler(this.buttonReadSize_Click);
            this.buttonResetAnimation.Location = new System.Drawing.Point(40, 356);
            this.buttonResetAnimation.Name = "buttonResetAnimation";
            this.buttonResetAnimation.Size = new System.Drawing.Size(232, 36);
            this.buttonResetAnimation.Text = "Reset animation";
            this.buttonResetAnimation.ToolTipText = "Call(\"resetAnimation\"): one-way, nothing comes back.";
            this.buttonResetAnimation.Click += new System.EventHandler(this.buttonResetAnimation_Click);
            this.buttonGetState.Location = new System.Drawing.Point(288, 356);
            this.buttonGetState.Name = "buttonGetState";
            this.buttonGetState.Size = new System.Drawing.Size(232, 36);
            this.buttonGetState.Text = "Get selected state";
            this.buttonGetState.ToolTipText = "await CallAsync(\"getSelectedState\") → GaugeStateDto (small, camelCase on the wire).";
            this.buttonGetState.Click += new System.EventHandler(this.buttonGetState_Click);
            //
            // labelDto  (the last awaited result, as wire JSON)
            //
            this.labelDto.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelDto.AutoSize = false;
            this.labelDto.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.labelDto.Font = new System.Drawing.Font("monospace", 9F);
            this.labelDto.ForeColor = System.Drawing.Color.FromArgb(31, 49, 71);
            this.labelDto.Location = new System.Drawing.Point(24, 404);
            this.labelDto.Name = "labelDto";
            this.labelDto.Padding = new Wisej.Web.Padding(10, 6, 10, 6);
            this.labelDto.Size = new System.Drawing.Size(512, 58);
            this.labelDto.Text = "awaited result → (click Read rendered size / Get selected state)";
            this.labelDto.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelAlarm
            //
            this.labelAlarm.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelAlarm.AutoSize = false;
            this.labelAlarm.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelAlarm.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelAlarm.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelAlarm.Location = new System.Drawing.Point(24, 470);
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
            this.labelState.Location = new System.Drawing.Point(24, 522);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(512, 46);
            this.labelState.Text = "";
            this.labelState.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelTrace  (Server ⇄ Client command trace)
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Controls.Add(this.labelTraceFooter);
            this.panelTrace.Location = new System.Drawing.Point(618, 30);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(700, 580);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(660, 30);
            this.labelTraceTitle.Text = "Server ⇄ Client  ·  command trace";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(660, 480);
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 540);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(660, 26);
            this.labelTraceFooter.Text = "→ .NET→JS  Call (queued) · CallAsync / EvalAsync (awaited)   ·   ← JS→.NET  result / event   ·   • server";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonSet104);
            this.panelActions.Controls.Add(this.buttonEvalWidth);
            this.panelActions.Controls.Add(this.buttonPitfall);
            this.panelActions.Controls.Add(this.buttonLeak);
            this.panelActions.Controls.Add(this.buttonStream);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 626);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // buttonSet104  (Call, crosses the threshold)
            //
            this.buttonSet104.Location = new System.Drawing.Point(0, 4);
            this.buttonSet104.Name = "buttonSet104";
            this.buttonSet104.Size = new System.Drawing.Size(170, 36);
            this.buttonSet104.Text = "Set value (104 · alarm)";
            this.buttonSet104.ToolTipText = "Call(\"setValue\", 104): the sweep crosses the threshold and the client raises thresholdExceeded.";
            this.buttonSet104.Click += new System.EventHandler(this.buttonSet104_Click);
            //
            // buttonEvalWidth  (EvalAsync variant)
            //
            this.buttonEvalWidth.Location = new System.Drawing.Point(178, 4);
            this.buttonEvalWidth.Name = "buttonEvalWidth";
            this.buttonEvalWidth.Size = new System.Drawing.Size(110, 36);
            this.buttonEvalWidth.Text = "Eval width";
            this.buttonEvalWidth.ToolTipText = "await EvalAsync(\"return this.getWidth();\")";
            this.buttonEvalWidth.Click += new System.EventHandler(this.buttonEvalWidth_Click);
            //
            // failure paths
            //
            this.buttonPitfall.Location = new System.Drawing.Point(296, 4);
            this.buttonPitfall.Name = "buttonPitfall";
            this.buttonPitfall.Size = new System.Drawing.Size(150, 36);
            this.buttonPitfall.Text = "Camel-case pitfall";
            this.buttonPitfall.ToolTipText = "Reads result.Width (PascalCase) from a camelCase wire object: undefined, no error, wrong data.";
            this.buttonPitfall.Click += new System.EventHandler(this.buttonPitfall_Click);
            this.buttonLeak.Location = new System.Drawing.Point(454, 4);
            this.buttonLeak.Name = "buttonLeak";
            this.buttonLeak.Size = new System.Drawing.Size(170, 36);
            this.buttonLeak.Text = "Leak a domain object";
            this.buttonLeak.ToolTipText = "Assigns a DomainWorkOrder (with its Customer) to Options.debugDump: see how much reaches the browser.";
            this.buttonLeak.Click += new System.EventHandler(this.buttonLeak_Click);
            //
            // buttonStream  (progress path)
            //
            this.buttonStream.Location = new System.Drawing.Point(632, 4);
            this.buttonStream.Name = "buttonStream";
            this.buttonStream.Size = new System.Drawing.Size(150, 36);
            this.buttonStream.Text = "▶ Stream values";
            this.buttonStream.ToolTipText = "A Timer issues one Call(\"setValue\") per tick; nothing is awaited.";
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
            this.timerStream.Interval = 900;
            this.timerStream.Tick += new System.EventHandler(this.timerStream_Tick);
            //
            // Window1
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 700);
            this.Controls.Add(this.panelGauge);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "Window1";
            this.Text = "IntegrationLab — Gauge Commands";
            this.Load += new System.EventHandler(this.Window1_Load);
            this.panelGauge.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGauge;
        private Wisej.Web.Label labelBoiler;
        private Wisej.Web.Label labelStatus;
        private IntegrationLab.Controls.SimpleGauge gauge;
        private Wisej.Web.Button buttonSetValue72;
        private Wisej.Web.Button buttonReadSize;
        private Wisej.Web.Button buttonResetAnimation;
        private Wisej.Web.Button buttonGetState;
        private Wisej.Web.Label labelDto;
        private Wisej.Web.Label labelAlarm;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonSet104;
        private Wisej.Web.Button buttonEvalWidth;
        private Wisej.Web.Button buttonPitfall;
        private Wisej.Web.Button buttonLeak;
        private Wisej.Web.Button buttonStream;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerStream;
    }
}
