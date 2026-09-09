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
            this.labelStripCaption = new Wisej.Web.Label();
            this.clockLabel = new Wisej.Web.Label();
            this.connectionLabel = new Wisej.Web.Label();
            this.activityLabel = new Wisej.Web.Label();
            this.labelLoadCaption = new Wisej.Web.Label();
            this.serverLoadBar = new Wisej.Web.ProgressBar();
            this.loadValueLabel = new Wisej.Web.Label();
            this.panelMechanisms = new Wisej.Web.Panel();
            this.labelTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.labelMechanism1 = new Wisej.Web.Label();
            this.statusLabel = new Wisej.Web.Label();
            this.labelMechanism2 = new Wisej.Web.Label();
            this.pushStatusLabel = new Wisej.Web.Label();
            this.labelMechanism3 = new Wisej.Web.Label();
            this.pollingLabel = new Wisej.Web.Label();
            this.labelCadence = new Wisej.Web.Label();
            this.cadenceResultLabel = new Wisej.Web.Label();
            this.labelBanner = new Wisej.Web.Label();
            this.labelState = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.refreshButton = new Wisej.Web.Button();
            this.serverEventButton = new Wisej.Web.Button();
            this.startButton = new Wisej.Web.Button();
            this.stopButton = new Wisej.Web.Button();
            this.faultButton = new Wisej.Web.Button();
            this.burstButton = new Wisej.Web.Button();
            this.batchedButton = new Wisej.Web.Button();
            this.clearButton = new Wisej.Web.Button();
            this.statusPanel.SuspendLayout();
            this.panelMechanisms.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // statusPanel  (the live status strip — lab task 1)
            //
            this.statusPanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusPanel.BackColor = System.Drawing.Color.White;
            this.statusPanel.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.statusPanel.Controls.Add(this.labelStripCaption);
            this.statusPanel.Controls.Add(this.clockLabel);
            this.statusPanel.Controls.Add(this.connectionLabel);
            this.statusPanel.Controls.Add(this.activityLabel);
            this.statusPanel.Controls.Add(this.labelLoadCaption);
            this.statusPanel.Controls.Add(this.serverLoadBar);
            this.statusPanel.Controls.Add(this.loadValueLabel);
            this.statusPanel.Location = new System.Drawing.Point(30, 18);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(1288, 92);
            //
            // labelStripCaption
            //
            this.labelStripCaption.AutoSize = false;
            this.labelStripCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelStripCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelStripCaption.Location = new System.Drawing.Point(16, 8);
            this.labelStripCaption.Name = "labelStripCaption";
            this.labelStripCaption.Size = new System.Drawing.Size(200, 18);
            this.labelStripCaption.Text = "TICKETOPS LIVE · STATUS STRIP";
            //
            // clockLabel  (updated by the heartbeat, once per second)
            //
            this.clockLabel.AutoSize = false;
            this.clockLabel.Font = new System.Drawing.Font("monospace", 22F, System.Drawing.FontStyle.Bold);
            this.clockLabel.Location = new System.Drawing.Point(16, 30);
            this.clockLabel.Name = "clockLabel";
            this.clockLabel.Size = new System.Drawing.Size(200, 44);
            this.clockLabel.Text = "--:--:--";
            this.clockLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // connectionLabel  (WebSocket vs HTTP-only)
            //
            this.connectionLabel.AutoSize = false;
            this.connectionLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.connectionLabel.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.connectionLabel.Location = new System.Drawing.Point(236, 30);
            this.connectionLabel.Name = "connectionLabel";
            this.connectionLabel.Size = new System.Drawing.Size(470, 24);
            this.connectionLabel.Text = "○ connecting…";
            //
            // activityLabel  (heartbeat message)
            //
            this.activityLabel.AutoSize = false;
            this.activityLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.activityLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.activityLabel.Location = new System.Drawing.Point(236, 56);
            this.activityLabel.Name = "activityLabel";
            this.activityLabel.Size = new System.Drawing.Size(470, 22);
            this.activityLabel.Text = "Heartbeat not running";
            //
            // labelLoadCaption
            //
            this.labelLoadCaption.AutoSize = false;
            this.labelLoadCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelLoadCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLoadCaption.Location = new System.Drawing.Point(740, 8);
            this.labelLoadCaption.Name = "labelLoadCaption";
            this.labelLoadCaption.Size = new System.Drawing.Size(200, 18);
            this.labelLoadCaption.Text = "SERVER LOAD (simulated)";
            //
            // serverLoadBar
            //
            this.serverLoadBar.Location = new System.Drawing.Point(740, 36);
            this.serverLoadBar.Name = "serverLoadBar";
            this.serverLoadBar.Size = new System.Drawing.Size(380, 24);
            this.serverLoadBar.Value = 0;
            //
            // loadValueLabel
            //
            this.loadValueLabel.AutoSize = false;
            this.loadValueLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.loadValueLabel.Location = new System.Drawing.Point(1136, 28);
            this.loadValueLabel.Name = "loadValueLabel";
            this.loadValueLabel.Size = new System.Drawing.Size(130, 36);
            this.loadValueLabel.Text = "0 %";
            this.loadValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // panelMechanisms  (the three update mechanisms + cadence experiment)
            //
            this.panelMechanisms.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelMechanisms.BackColor = System.Drawing.Color.White;
            this.panelMechanisms.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelMechanisms.Controls.Add(this.labelTitle);
            this.panelMechanisms.Controls.Add(this.labelStatus);
            this.panelMechanisms.Controls.Add(this.labelMechanism1);
            this.panelMechanisms.Controls.Add(this.statusLabel);
            this.panelMechanisms.Controls.Add(this.labelMechanism2);
            this.panelMechanisms.Controls.Add(this.pushStatusLabel);
            this.panelMechanisms.Controls.Add(this.labelMechanism3);
            this.panelMechanisms.Controls.Add(this.pollingLabel);
            this.panelMechanisms.Controls.Add(this.labelCadence);
            this.panelMechanisms.Controls.Add(this.cadenceResultLabel);
            this.panelMechanisms.Controls.Add(this.labelBanner);
            this.panelMechanisms.Controls.Add(this.labelState);
            this.panelMechanisms.Location = new System.Drawing.Point(30, 126);
            this.panelMechanisms.Name = "panelMechanisms";
            this.panelMechanisms.Size = new System.Drawing.Size(700, 472);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 16);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(360, 30);
            this.labelTitle.Text = "Three ways an update reaches the browser";
            //
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(390, 18);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(286, 26);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // labelMechanism1 / statusLabel  (in-request)
            //
            this.labelMechanism1.AutoSize = false;
            this.labelMechanism1.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelMechanism1.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.labelMechanism1.Location = new System.Drawing.Point(24, 58);
            this.labelMechanism1.Name = "labelMechanism1";
            this.labelMechanism1.Size = new System.Drawing.Size(652, 20);
            this.labelMechanism1.Text = "1 · in-request   refreshButton_Click → the response carries the change";
            this.statusLabel.AutoSize = false;
            this.statusLabel.Font = new System.Drawing.Font("default", 10F);
            this.statusLabel.Location = new System.Drawing.Point(24, 80);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(652, 24);
            this.statusLabel.Text = "click “Refresh (in-request)”";
            //
            // labelMechanism2 / pushStatusLabel  (out-of-bound push)
            //
            this.labelMechanism2.AutoSize = false;
            this.labelMechanism2.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelMechanism2.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelMechanism2.Location = new System.Drawing.Point(24, 114);
            this.labelMechanism2.Name = "labelMechanism2";
            this.labelMechanism2.Size = new System.Drawing.Size(652, 20);
            this.labelMechanism2.Text = "2 · WebSocket push   Application.StartTask → controls → Application.Update(this)";
            this.pushStatusLabel.AutoSize = false;
            this.pushStatusLabel.Font = new System.Drawing.Font("default", 10F);
            this.pushStatusLabel.Location = new System.Drawing.Point(24, 136);
            this.pushStatusLabel.Name = "pushStatusLabel";
            this.pushStatusLabel.Size = new System.Drawing.Size(652, 24);
            this.pushStatusLabel.Text = "click “Server event ×5” or start the heartbeat";
            //
            // labelMechanism3 / pollingLabel  (polling fallback)
            //
            this.labelMechanism3.AutoSize = false;
            this.labelMechanism3.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelMechanism3.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.labelMechanism3.Location = new System.Drawing.Point(24, 170);
            this.labelMechanism3.Name = "labelMechanism3";
            this.labelMechanism3.Size = new System.Drawing.Size(652, 20);
            this.labelMechanism3.Text = "3 · polling fallback   StartPolling(1000) when a task starts without a WebSocket · EndPolling() after";
            this.pollingLabel.AutoSize = false;
            this.pollingLabel.Font = new System.Drawing.Font("default", 10F);
            this.pollingLabel.Location = new System.Drawing.Point(24, 192);
            this.pollingLabel.Name = "pollingLabel";
            this.pollingLabel.Size = new System.Drawing.Size(652, 24);
            this.pollingLabel.Text = "";
            //
            // labelCadence / cadenceResultLabel
            //
            this.labelCadence.AutoSize = false;
            this.labelCadence.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelCadence.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelCadence.Location = new System.Drawing.Point(24, 226);
            this.labelCadence.Name = "labelCadence";
            this.labelCadence.Size = new System.Drawing.Size(652, 20);
            this.labelCadence.Text = "cadence experiment   100 model changes 10 ms apart — push all, or push every 10th";
            this.cadenceResultLabel.AutoSize = false;
            this.cadenceResultLabel.Font = new System.Drawing.Font("default", 10F);
            this.cadenceResultLabel.Location = new System.Drawing.Point(24, 248);
            this.cadenceResultLabel.Name = "cadenceResultLabel";
            this.cadenceResultLabel.Size = new System.Drawing.Size(652, 44);
            this.cadenceResultLabel.Text = "click “Burst 100 @ 10 ms” then “Batched (10 pushes)” and compare the push counts";
            this.cadenceResultLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelBanner
            //
            this.labelBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(24, 304);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(652, 54);
            this.labelBanner.Text = "";
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // labelState  (what the server owns right now)
            //
            this.labelState.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelState.AutoSize = false;
            this.labelState.Font = new System.Drawing.Font("monospace", 9F);
            this.labelState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelState.Location = new System.Drawing.Point(24, 368);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(652, 88);
            this.labelState.Text = "";
            this.labelState.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelTrace  (Server → Browser live push trace)
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Controls.Add(this.labelTraceFooter);
            this.panelTrace.Location = new System.Drawing.Point(748, 126);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(570, 472);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(530, 30);
            this.labelTraceTitle.Text = "Server → Browser  ·  live push trace";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(530, 370);
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 430);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(530, 26);
            this.labelTraceFooter.Text = "→ push = Application.Update from a task   ·   ← request = the browser asked   ·   • server = decision";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.refreshButton);
            this.panelActions.Controls.Add(this.serverEventButton);
            this.panelActions.Controls.Add(this.startButton);
            this.panelActions.Controls.Add(this.stopButton);
            this.panelActions.Controls.Add(this.faultButton);
            this.panelActions.Controls.Add(this.burstButton);
            this.panelActions.Controls.Add(this.batchedButton);
            this.panelActions.Controls.Add(this.clearButton);
            this.panelActions.Location = new System.Drawing.Point(30, 614);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // refreshButton  (mechanism 1 — success path)
            //
            this.refreshButton.Location = new System.Drawing.Point(0, 4);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(160, 36);
            this.refreshButton.Text = "Refresh (in-request)";
            this.refreshButton.ToolTipText = "The click handler changes a label; the change travels back with the response of the click.";
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            //
            // serverEventButton  (mechanism 2 — success path)
            //
            this.serverEventButton.Location = new System.Drawing.Point(168, 4);
            this.serverEventButton.Name = "serverEventButton";
            this.serverEventButton.Size = new System.Drawing.Size(170, 36);
            this.serverEventButton.Text = "Server event ×5 (push)";
            this.serverEventButton.ToolTipText = "Application.StartTask runs 5 steps and pushes each one with Application.Update(this).";
            this.serverEventButton.Click += new System.EventHandler(this.serverEventButton_Click);
            //
            // startButton / stopButton  (progress path — the heartbeat)
            //
            this.startButton.Location = new System.Drawing.Point(362, 4);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(150, 36);
            this.startButton.Text = "▶ Start heartbeat";
            this.startButton.ToolTipText = "One push per second: clock, load, activity. Clicking twice does not start a second loop.";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(520, 4);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(90, 36);
            this.stopButton.Text = "■ Stop";
            this.stopButton.ToolTipText = "Sets _running = false; the loop exits within one second.";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            //
            // faultButton  (failure path)
            //
            this.faultButton.Location = new System.Drawing.Point(634, 4);
            this.faultButton.Name = "faultButton";
            this.faultButton.Size = new System.Drawing.Size(120, 36);
            this.faultButton.Text = "Inject fault";
            this.faultButton.ToolTipText = "The next heartbeat throws inside the task: caught, logged, safe message, buttons re-enabled in finally.";
            this.faultButton.Click += new System.EventHandler(this.faultButton_Click);
            //
            // burstButton / batchedButton  (cadence experiment)
            //
            this.burstButton.Location = new System.Drawing.Point(778, 4);
            this.burstButton.Name = "burstButton";
            this.burstButton.Size = new System.Drawing.Size(160, 36);
            this.burstButton.Text = "Burst 100 @ 10 ms";
            this.burstButton.ToolTipText = "Anti-pattern: 100 changes, 100 pushes, 10 ms apart.";
            this.burstButton.Click += new System.EventHandler(this.burstButton_Click);
            this.batchedButton.Location = new System.Drawing.Point(946, 4);
            this.batchedButton.Name = "batchedButton";
            this.batchedButton.Size = new System.Drawing.Size(160, 36);
            this.batchedButton.Text = "Batched (10 pushes)";
            this.batchedButton.ToolTipText = "The same 100 changes, pushed every 10th change.";
            this.batchedButton.Click += new System.EventHandler(this.batchedButton_Click);
            //
            // clearButton
            //
            this.clearButton.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.clearButton.Location = new System.Drawing.Point(1178, 4);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(110, 36);
            this.clearButton.Text = "Clear trace";
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.panelMechanisms);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 700);
            this.Text = "TicketOps Live — Live Status Strip";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.statusPanel.ResumeLayout(false);
            this.panelMechanisms.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel statusPanel;
        private Wisej.Web.Label labelStripCaption;
        private Wisej.Web.Label clockLabel;
        private Wisej.Web.Label connectionLabel;
        private Wisej.Web.Label activityLabel;
        private Wisej.Web.Label labelLoadCaption;
        private Wisej.Web.ProgressBar serverLoadBar;
        private Wisej.Web.Label loadValueLabel;
        private Wisej.Web.Panel panelMechanisms;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Label labelMechanism1;
        private Wisej.Web.Label statusLabel;
        private Wisej.Web.Label labelMechanism2;
        private Wisej.Web.Label pushStatusLabel;
        private Wisej.Web.Label labelMechanism3;
        private Wisej.Web.Label pollingLabel;
        private Wisej.Web.Label labelCadence;
        private Wisej.Web.Label cadenceResultLabel;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button refreshButton;
        private Wisej.Web.Button serverEventButton;
        private Wisej.Web.Button startButton;
        private Wisej.Web.Button stopButton;
        private Wisej.Web.Button faultButton;
        private Wisej.Web.Button burstButton;
        private Wisej.Web.Button batchedButton;
        private Wisej.Web.Button clearButton;
    }
}
