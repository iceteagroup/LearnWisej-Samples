namespace TicketOpsLive
{
    partial class MainPage
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
            this.statusPanel = new Wisej.Web.Panel();
            this.clockLabel = new Wisej.Web.Label();
            this.connectionLabel = new Wisej.Web.Label();
            this.activityLabel = new Wisej.Web.Label();
            this.labelLoadCaption = new Wisej.Web.Label();
            this.serverLoadBar = new Wisej.Web.ProgressBar();
            this.loadValueLabel = new Wisej.Web.Label();
            this.panelConsole = new Wisej.Web.Panel();
            this.labelStatusCaption = new Wisej.Web.Label();
            this.statusLabel = new Wisej.Web.Label();
            this.labelPushCaption = new Wisej.Web.Label();
            this.pushStatusLabel = new Wisej.Web.Label();
            this.refreshButton = new Wisej.Web.Button();
            this.serverEventButton = new Wisej.Web.Button();
            this.startButton = new Wisej.Web.Button();
            this.stopButton = new Wisej.Web.Button();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.statusPanel.SuspendLayout();
            this.panelConsole.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.SuspendLayout();
            //
            // statusPanel
            //
            this.statusPanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusPanel.BackColor = System.Drawing.Color.White;
            this.statusPanel.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.statusPanel.Controls.Add(this.clockLabel);
            this.statusPanel.Controls.Add(this.connectionLabel);
            this.statusPanel.Controls.Add(this.activityLabel);
            this.statusPanel.Controls.Add(this.labelLoadCaption);
            this.statusPanel.Controls.Add(this.serverLoadBar);
            this.statusPanel.Controls.Add(this.loadValueLabel);
            this.statusPanel.Location = new System.Drawing.Point(20, 18);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(960, 80);
            //
            // clockLabel
            //
            this.clockLabel.AutoSize = false;
            this.clockLabel.Font = new System.Drawing.Font("monospace", 22F, System.Drawing.FontStyle.Bold);
            this.clockLabel.Location = new System.Drawing.Point(16, 16);
            this.clockLabel.Name = "clockLabel";
            this.clockLabel.Size = new System.Drawing.Size(170, 44);
            this.clockLabel.Text = "--:--:--";
            this.clockLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // connectionLabel
            //
            this.connectionLabel.AutoSize = false;
            this.connectionLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.connectionLabel.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.connectionLabel.Location = new System.Drawing.Point(200, 14);
            this.connectionLabel.Name = "connectionLabel";
            this.connectionLabel.Size = new System.Drawing.Size(420, 24);
            this.connectionLabel.Text = "○ connecting…";
            //
            // activityLabel
            //
            this.activityLabel.AutoSize = false;
            this.activityLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.activityLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.activityLabel.Location = new System.Drawing.Point(200, 42);
            this.activityLabel.Name = "activityLabel";
            this.activityLabel.Size = new System.Drawing.Size(420, 22);
            this.activityLabel.Text = "Heartbeat not running";
            //
            // labelLoadCaption
            //
            this.labelLoadCaption.AutoSize = false;
            this.labelLoadCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelLoadCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLoadCaption.Location = new System.Drawing.Point(640, 12);
            this.labelLoadCaption.Name = "labelLoadCaption";
            this.labelLoadCaption.Size = new System.Drawing.Size(200, 18);
            this.labelLoadCaption.Text = "SERVER LOAD";
            //
            // serverLoadBar
            //
            this.serverLoadBar.Location = new System.Drawing.Point(640, 36);
            this.serverLoadBar.Name = "serverLoadBar";
            this.serverLoadBar.Size = new System.Drawing.Size(220, 24);
            this.serverLoadBar.Value = 0;
            //
            // loadValueLabel
            //
            this.loadValueLabel.AutoSize = false;
            this.loadValueLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.loadValueLabel.Location = new System.Drawing.Point(866, 30);
            this.loadValueLabel.Name = "loadValueLabel";
            this.loadValueLabel.Size = new System.Drawing.Size(80, 36);
            this.loadValueLabel.Text = "0 %";
            this.loadValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // panelConsole
            //
            this.panelConsole.BackColor = System.Drawing.Color.White;
            this.panelConsole.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelConsole.Controls.Add(this.labelStatusCaption);
            this.panelConsole.Controls.Add(this.statusLabel);
            this.panelConsole.Controls.Add(this.labelPushCaption);
            this.panelConsole.Controls.Add(this.pushStatusLabel);
            this.panelConsole.Controls.Add(this.refreshButton);
            this.panelConsole.Controls.Add(this.serverEventButton);
            this.panelConsole.Controls.Add(this.startButton);
            this.panelConsole.Controls.Add(this.stopButton);
            this.panelConsole.Location = new System.Drawing.Point(20, 112);
            this.panelConsole.Name = "panelConsole";
            this.panelConsole.Size = new System.Drawing.Size(420, 236);
            //
            // labelStatusCaption
            //
            this.labelStatusCaption.AutoSize = false;
            this.labelStatusCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelStatusCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelStatusCaption.Location = new System.Drawing.Point(20, 16);
            this.labelStatusCaption.Name = "labelStatusCaption";
            this.labelStatusCaption.Size = new System.Drawing.Size(200, 18);
            this.labelStatusCaption.Text = "STATUS";
            //
            // statusLabel
            //
            this.statusLabel.AutoSize = false;
            this.statusLabel.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.statusLabel.Location = new System.Drawing.Point(20, 36);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(380, 24);
            this.statusLabel.Text = "Online";
            //
            // labelPushCaption
            //
            this.labelPushCaption.AutoSize = false;
            this.labelPushCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelPushCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelPushCaption.Location = new System.Drawing.Point(20, 72);
            this.labelPushCaption.Name = "labelPushCaption";
            this.labelPushCaption.Size = new System.Drawing.Size(200, 18);
            this.labelPushCaption.Text = "PUSH";
            //
            // pushStatusLabel
            //
            this.pushStatusLabel.AutoSize = false;
            this.pushStatusLabel.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.pushStatusLabel.Location = new System.Drawing.Point(20, 92);
            this.pushStatusLabel.Name = "pushStatusLabel";
            this.pushStatusLabel.Size = new System.Drawing.Size(380, 24);
            this.pushStatusLabel.Text = "idle";
            //
            // refreshButton
            //
            this.refreshButton.Location = new System.Drawing.Point(20, 132);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(110, 36);
            this.refreshButton.Text = "Refresh";
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            //
            // serverEventButton
            //
            this.serverEventButton.Location = new System.Drawing.Point(138, 132);
            this.serverEventButton.Name = "serverEventButton";
            this.serverEventButton.Size = new System.Drawing.Size(160, 36);
            this.serverEventButton.Text = "Start server event";
            this.serverEventButton.Click += new System.EventHandler(this.serverEventButton_Click);
            //
            // startButton
            //
            this.startButton.Location = new System.Drawing.Point(20, 180);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(150, 36);
            this.startButton.Text = "▶ Start heartbeat";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            //
            // stopButton
            //
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(178, 180);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(90, 36);
            this.stopButton.Text = "■ Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            //
            // panelTrace
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Location = new System.Drawing.Point(456, 112);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(524, 236);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 12);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(300, 26);
            this.labelTraceTitle.Text = "Update trace";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 44);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(484, 172);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.panelConsole);
            this.Controls.Add(this.panelTrace);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1000, 370);
            this.Text = "TicketOps Live — Operations Console";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.statusPanel.ResumeLayout(false);
            this.panelConsole.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel statusPanel;
        private Wisej.Web.Label clockLabel;
        private Wisej.Web.Label connectionLabel;
        private Wisej.Web.Label activityLabel;
        private Wisej.Web.Label labelLoadCaption;
        private Wisej.Web.ProgressBar serverLoadBar;
        private Wisej.Web.Label loadValueLabel;
        private Wisej.Web.Panel panelConsole;
        private Wisej.Web.Label labelStatusCaption;
        private Wisej.Web.Label statusLabel;
        private Wisej.Web.Label labelPushCaption;
        private Wisej.Web.Label pushStatusLabel;
        private Wisej.Web.Button refreshButton;
        private Wisej.Web.Button serverEventButton;
        private Wisej.Web.Button startButton;
        private Wisej.Web.Button stopButton;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
    }
}
