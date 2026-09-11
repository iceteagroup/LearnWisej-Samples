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
            this.gaugeTemperature = new IntegrationLab.Widgets.TemperatureGauge();
            this.labelAlarm = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonSet72 = new Wisej.Web.Button();
            this.buttonSet88 = new Wisej.Web.Button();
            this.buttonSet104 = new Wisej.Web.Button();
            this.buttonSet79 = new Wisej.Web.Button();
            this.buttonStream = new Wisej.Web.Button();
            this.buttonInvalid = new Wisej.Web.Button();
            this.buttonCorrupt = new Wisej.Web.Button();
            this.buttonResync = new Wisej.Web.Button();
            this.timerStream = new Wisej.Web.Timer(this.components);
            this.panelGauge.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGauge
            //
            this.panelGauge.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelGauge.BackColor = System.Drawing.Color.White;
            this.panelGauge.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGauge.Controls.Add(this.labelBoiler);
            this.panelGauge.Controls.Add(this.labelStatus);
            this.panelGauge.Controls.Add(this.gaugeTemperature);
            this.panelGauge.Controls.Add(this.labelAlarm);
            this.panelGauge.Location = new System.Drawing.Point(30, 30);
            this.panelGauge.Name = "panelGauge";
            this.panelGauge.Size = new System.Drawing.Size(560, 480);
            //
            // labelBoiler
            //
            this.labelBoiler.AutoSize = false;
            this.labelBoiler.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelBoiler.Location = new System.Drawing.Point(24, 18);
            this.labelBoiler.Name = "labelBoiler";
            this.labelBoiler.Size = new System.Drawing.Size(260, 30);
            this.labelBoiler.Text = "Boiler 3 — live temperature";
            //
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(300, 20);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(236, 26);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gaugeTemperature
            //
            this.gaugeTemperature.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gaugeTemperature.Label = "VendorGauge";
            this.gaugeTemperature.Location = new System.Drawing.Point(40, 60);
            this.gaugeTemperature.Maximum = 120D;
            this.gaugeTemperature.Minimum = 40D;
            this.gaugeTemperature.Name = "gaugeTemperature";
            this.gaugeTemperature.Size = new System.Drawing.Size(480, 340);
            this.gaugeTemperature.Threshold = 100D;
            this.gaugeTemperature.Value = 72D;
            this.gaugeTemperature.WarnAt = 85D;
            //
            // labelAlarm
            //
            this.labelAlarm.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelAlarm.AutoSize = false;
            this.labelAlarm.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelAlarm.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelAlarm.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelAlarm.Location = new System.Drawing.Point(24, 414);
            this.labelAlarm.Name = "labelAlarm";
            this.labelAlarm.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelAlarm.Size = new System.Drawing.Size(512, 48);
            this.labelAlarm.Text = "";
            this.labelAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelAlarm.Visible = false;
            //
            // panelTrace
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Location = new System.Drawing.Point(618, 30);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(700, 480);
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
            this.listTrace.Size = new System.Drawing.Size(660, 408);
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonSet72);
            this.panelActions.Controls.Add(this.buttonSet88);
            this.panelActions.Controls.Add(this.buttonSet104);
            this.panelActions.Controls.Add(this.buttonSet79);
            this.panelActions.Controls.Add(this.buttonStream);
            this.panelActions.Controls.Add(this.buttonInvalid);
            this.panelActions.Controls.Add(this.buttonCorrupt);
            this.panelActions.Controls.Add(this.buttonResync);
            this.panelActions.Location = new System.Drawing.Point(30, 526);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // buttonSet72
            //
            this.buttonSet72.Location = new System.Drawing.Point(0, 4);
            this.buttonSet72.Name = "buttonSet72";
            this.buttonSet72.Size = new System.Drawing.Size(96, 36);
            this.buttonSet72.Text = "Set 72°";
            this.buttonSet72.Click += new System.EventHandler(this.buttonSet72_Click);
            //
            // buttonSet88
            //
            this.buttonSet88.Location = new System.Drawing.Point(104, 4);
            this.buttonSet88.Name = "buttonSet88";
            this.buttonSet88.Size = new System.Drawing.Size(96, 36);
            this.buttonSet88.Text = "Set 88°";
            this.buttonSet88.Click += new System.EventHandler(this.buttonSet88_Click);
            //
            // buttonSet104
            //
            this.buttonSet104.Location = new System.Drawing.Point(208, 4);
            this.buttonSet104.Name = "buttonSet104";
            this.buttonSet104.Size = new System.Drawing.Size(96, 36);
            this.buttonSet104.Text = "Set 104°";
            this.buttonSet104.Click += new System.EventHandler(this.buttonSet104_Click);
            //
            // buttonSet79
            //
            this.buttonSet79.Location = new System.Drawing.Point(312, 4);
            this.buttonSet79.Name = "buttonSet79";
            this.buttonSet79.Size = new System.Drawing.Size(96, 36);
            this.buttonSet79.Text = "Set 79°";
            this.buttonSet79.Click += new System.EventHandler(this.buttonSet79_Click);
            //
            // buttonStream
            //
            this.buttonStream.Location = new System.Drawing.Point(432, 4);
            this.buttonStream.Name = "buttonStream";
            this.buttonStream.Size = new System.Drawing.Size(160, 36);
            this.buttonStream.Text = "▶ Stream readings";
            this.buttonStream.Click += new System.EventHandler(this.buttonStream_Click);
            //
            // buttonInvalid
            //
            this.buttonInvalid.Location = new System.Drawing.Point(616, 4);
            this.buttonInvalid.Name = "buttonInvalid";
            this.buttonInvalid.Size = new System.Drawing.Size(96, 36);
            this.buttonInvalid.Text = "Set 150°";
            this.buttonInvalid.Click += new System.EventHandler(this.buttonInvalid_Click);
            //
            // buttonCorrupt
            //
            this.buttonCorrupt.Location = new System.Drawing.Point(720, 4);
            this.buttonCorrupt.Name = "buttonCorrupt";
            this.buttonCorrupt.Size = new System.Drawing.Size(150, 36);
            this.buttonCorrupt.Text = "Corrupt payload";
            this.buttonCorrupt.Click += new System.EventHandler(this.buttonCorrupt_Click);
            //
            // buttonResync
            //
            this.buttonResync.Location = new System.Drawing.Point(878, 4);
            this.buttonResync.Name = "buttonResync";
            this.buttonResync.Size = new System.Drawing.Size(150, 36);
            this.buttonResync.Text = "Resync from server";
            this.buttonResync.Click += new System.EventHandler(this.buttonResync_Click);
            //
            // timerStream
            //
            this.timerStream.Interval = 700;
            this.timerStream.Tick += new System.EventHandler(this.timerStream_Tick);
            //
            // Window1
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 600);
            this.Controls.Add(this.panelGauge);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "Window1";
            this.Text = "IntegrationLab — Sensor Monitor";
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
        private IntegrationLab.Widgets.TemperatureGauge gaugeTemperature;
        private Wisej.Web.Label labelAlarm;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonSet72;
        private Wisej.Web.Button buttonSet88;
        private Wisej.Web.Button buttonSet104;
        private Wisej.Web.Button buttonSet79;
        private Wisej.Web.Button buttonStream;
        private Wisej.Web.Button buttonInvalid;
        private Wisej.Web.Button buttonCorrupt;
        private Wisej.Web.Button buttonResync;
        private Wisej.Web.Timer timerStream;
    }
}
