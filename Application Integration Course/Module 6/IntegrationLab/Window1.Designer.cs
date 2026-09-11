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
            this.gauge = new IntegrationLab.Controls.SimpleGauge();
            this.buttonSetValue72 = new Wisej.Web.Button();
            this.buttonReadSize = new Wisej.Web.Button();
            this.buttonResetAnimation = new Wisej.Web.Button();
            this.buttonGetState = new Wisej.Web.Button();
            this.labelAlarm = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.panelGauge.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGauge
            //
            this.panelGauge.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelGauge.BackColor = System.Drawing.Color.White;
            this.panelGauge.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGauge.Controls.Add(this.labelBoiler);
            this.panelGauge.Controls.Add(this.gauge);
            this.panelGauge.Controls.Add(this.buttonSetValue72);
            this.panelGauge.Controls.Add(this.buttonReadSize);
            this.panelGauge.Controls.Add(this.buttonResetAnimation);
            this.panelGauge.Controls.Add(this.buttonGetState);
            this.panelGauge.Controls.Add(this.labelAlarm);
            this.panelGauge.Location = new System.Drawing.Point(30, 30);
            this.panelGauge.Name = "panelGauge";
            this.panelGauge.Size = new System.Drawing.Size(560, 470);
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
            // gauge
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
            // buttonSetValue72
            //
            this.buttonSetValue72.Location = new System.Drawing.Point(40, 312);
            this.buttonSetValue72.Name = "buttonSetValue72";
            this.buttonSetValue72.Size = new System.Drawing.Size(232, 36);
            this.buttonSetValue72.Text = "Set value (72)";
            this.buttonSetValue72.Click += new System.EventHandler(this.buttonSetValue72_Click);
            //
            // buttonReadSize
            //
            this.buttonReadSize.Location = new System.Drawing.Point(288, 312);
            this.buttonReadSize.Name = "buttonReadSize";
            this.buttonReadSize.Size = new System.Drawing.Size(232, 36);
            this.buttonReadSize.Text = "Read rendered size";
            this.buttonReadSize.Click += new System.EventHandler(this.buttonReadSize_Click);
            //
            // buttonResetAnimation
            //
            this.buttonResetAnimation.Location = new System.Drawing.Point(40, 356);
            this.buttonResetAnimation.Name = "buttonResetAnimation";
            this.buttonResetAnimation.Size = new System.Drawing.Size(232, 36);
            this.buttonResetAnimation.Text = "Reset animation";
            this.buttonResetAnimation.Click += new System.EventHandler(this.buttonResetAnimation_Click);
            //
            // buttonGetState
            //
            this.buttonGetState.Location = new System.Drawing.Point(288, 356);
            this.buttonGetState.Name = "buttonGetState";
            this.buttonGetState.Size = new System.Drawing.Size(232, 36);
            this.buttonGetState.Text = "Get selected state";
            this.buttonGetState.Click += new System.EventHandler(this.buttonGetState_Click);
            //
            // labelAlarm
            //
            this.labelAlarm.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelAlarm.AutoSize = false;
            this.labelAlarm.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelAlarm.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelAlarm.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelAlarm.Location = new System.Drawing.Point(24, 404);
            this.labelAlarm.Name = "labelAlarm";
            this.labelAlarm.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelAlarm.Size = new System.Drawing.Size(512, 44);
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
            this.panelTrace.Size = new System.Drawing.Size(700, 470);
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
            this.listTrace.Size = new System.Drawing.Size(660, 398);
            //
            // Window1
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 530);
            this.Controls.Add(this.panelGauge);
            this.Controls.Add(this.panelTrace);
            this.Name = "Window1";
            this.Text = "IntegrationLab — Gauge Commands";
            this.panelGauge.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGauge;
        private Wisej.Web.Label labelBoiler;
        private IntegrationLab.Controls.SimpleGauge gauge;
        private Wisej.Web.Button buttonSetValue72;
        private Wisej.Web.Button buttonReadSize;
        private Wisej.Web.Button buttonResetAnimation;
        private Wisej.Web.Button buttonGetState;
        private Wisej.Web.Label labelAlarm;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
    }
}
