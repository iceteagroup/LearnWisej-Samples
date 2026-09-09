namespace WisejTrainingApp
{
    partial class DashboardWindow
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
            this.panelDashboard = new Wisej.Web.Panel();
            this.labelDashboardCard = new Wisej.Web.Label();
            this.lblTitle = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlServices = new Wisej.Web.Panel();
            this.lblServerStatus = new Wisej.Web.Label();
            this.lblDatabaseStatus = new Wisej.Web.Label();
            this.lblApiStatus = new Wisej.Web.Label();
            this.btnStart = new Wisej.Web.Button();
            this.btnStop = new Wisej.Web.Button();
            this.btnReset = new Wisej.Web.Button();
            this.btnRefresh = new Wisej.Web.Button();
            this.chkSimulateOutage = new Wisej.Web.CheckBox();
            this.lblHint = new Wisej.Web.Label();
            this.panelCode = new Wisej.Web.Panel();
            this.labelCodeCard = new Wisej.Web.Label();
            this.lblFileCodeBehind = new Wisej.Web.Label();
            this.lblFileDesigner = new Wisej.Web.Label();
            this.lblFileProgram = new Wisej.Web.Label();
            this.lblVocabulary = new Wisej.Web.Label();
            this.panelLog = new Wisej.Web.Panel();
            this.labelLogCard = new Wisej.Web.Label();
            this.lstEventLog = new Wisej.Web.ListBox();
            this.labelLogFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.btnSimulateOutage = new Wisej.Web.Button();
            this.btnRecover = new Wisej.Web.Button();
            this.panelDashboard.SuspendLayout();
            this.pnlServices.SuspendLayout();
            this.panelCode.SuspendLayout();
            this.panelLog.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelDashboard  (the "System Dashboard" card — the lab's controls live here)
            //
            this.panelDashboard.BackColor = System.Drawing.Color.White;
            this.panelDashboard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelDashboard.Controls.Add(this.labelDashboardCard);
            this.panelDashboard.Controls.Add(this.lblTitle);
            this.panelDashboard.Controls.Add(this.lblStatus);
            this.panelDashboard.Controls.Add(this.pnlServices);
            this.panelDashboard.Controls.Add(this.btnStart);
            this.panelDashboard.Controls.Add(this.btnStop);
            this.panelDashboard.Controls.Add(this.btnReset);
            this.panelDashboard.Controls.Add(this.btnRefresh);
            this.panelDashboard.Controls.Add(this.chkSimulateOutage);
            this.panelDashboard.Controls.Add(this.lblHint);
            this.panelDashboard.Location = new System.Drawing.Point(30, 30);
            this.panelDashboard.Name = "panelDashboard";
            this.panelDashboard.Size = new System.Drawing.Size(560, 380);
            //
            // labelDashboardCard
            //
            this.labelDashboardCard.AutoSize = false;
            this.labelDashboardCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelDashboardCard.Location = new System.Drawing.Point(24, 14);
            this.labelDashboardCard.Name = "labelDashboardCard";
            this.labelDashboardCard.Size = new System.Drawing.Size(512, 28);
            this.labelDashboardCard.Text = "Dashboard  ·  design → name → wire Click → run";
            //
            // lblTitle  (the large title label)
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(24, 50);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(512, 40);
            this.lblTitle.Text = "System Dashboard";
            //
            // lblStatus  (starts at Status: Idle; the handlers change Text and ForeColor)
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStatus.Location = new System.Drawing.Point(24, 96);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(512, 26);
            this.lblStatus.Text = "Status: Idle";
            //
            // pnlServices  (a Panel grouping the three service indicators)
            //
            this.pnlServices.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.pnlServices.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlServices.Controls.Add(this.lblServerStatus);
            this.pnlServices.Controls.Add(this.lblDatabaseStatus);
            this.pnlServices.Controls.Add(this.lblApiStatus);
            this.pnlServices.Location = new System.Drawing.Point(24, 130);
            this.pnlServices.Name = "pnlServices";
            this.pnlServices.Size = new System.Drawing.Size(512, 118);
            //
            // lblServerStatus
            //
            this.lblServerStatus.AutoSize = false;
            this.lblServerStatus.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblServerStatus.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblServerStatus.Location = new System.Drawing.Point(16, 12);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(480, 26);
            this.lblServerStatus.Text = "Server: Offline";
            //
            // lblDatabaseStatus
            //
            this.lblDatabaseStatus.AutoSize = false;
            this.lblDatabaseStatus.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblDatabaseStatus.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblDatabaseStatus.Location = new System.Drawing.Point(16, 46);
            this.lblDatabaseStatus.Name = "lblDatabaseStatus";
            this.lblDatabaseStatus.Size = new System.Drawing.Size(480, 26);
            this.lblDatabaseStatus.Text = "Database: Offline";
            //
            // lblApiStatus
            //
            this.lblApiStatus.AutoSize = false;
            this.lblApiStatus.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblApiStatus.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblApiStatus.Location = new System.Drawing.Point(16, 80);
            this.lblApiStatus.Name = "lblApiStatus";
            this.lblApiStatus.Size = new System.Drawing.Size(480, 26);
            this.lblApiStatus.Text = "API Service: Offline";
            //
            // btnStart  (double-click in the Designer created btnStart_Click)
            //
            this.btnStart.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnStart.Location = new System.Drawing.Point(24, 262);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(116, 36);
            this.btnStart.Text = "Start";
            this.btnStart.ToolTipText = "btnStart_Click: every service Online, Status: Running, log \"Dashboard started.\"";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            //
            // btnStop
            //
            this.btnStop.Location = new System.Drawing.Point(148, 262);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(116, 36);
            this.btnStop.Text = "Stop";
            this.btnStop.ToolTipText = "btnStop_Click: every service Offline, Status: Stopped, log \"Dashboard stopped.\"";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            //
            // btnReset
            //
            this.btnReset.Location = new System.Drawing.Point(272, 262);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(116, 36);
            this.btnReset.Text = "Reset";
            this.btnReset.ToolTipText = "btnReset_Click: clears the log, services Offline, Status: Idle, log \"Dashboard reset.\"";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(396, 262);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(116, 36);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.ToolTipText = "btnRefresh_Click: re-checks every service through ServiceMonitor and logs a fresh entry.";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // chkSimulateOutage  (failure switch: Refresh then times out the API Service check)
            //
            this.chkSimulateOutage.Location = new System.Drawing.Point(24, 310);
            this.chkSimulateOutage.Name = "chkSimulateOutage";
            this.chkSimulateOutage.Size = new System.Drawing.Size(512, 26);
            this.chkSimulateOutage.Text = "Simulate API outage on Refresh";
            this.chkSimulateOutage.ToolTipText = "When checked, the next Refresh reports API Service: Degraded (timeout after 2000 ms).";
            this.chkSimulateOutage.CheckedChanged += new System.EventHandler(this.chkSimulateOutage_CheckedChanged);
            //
            // lblHint
            //
            this.lblHint.AutoSize = false;
            this.lblHint.Font = new System.Drawing.Font("monospace", 9F);
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblHint.Location = new System.Drawing.Point(24, 342);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(512, 30);
            this.lblHint.Text = "Handlers: btnStart_Click · btnStop_Click · btnReset_Click · btnRefresh_Click\nAll in DashboardWindow.cs — every log line goes through AddLog(message)";
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelCode  ("Where your code goes" — lesson s7 §2 + the properties / events / methods vocabulary)
            //
            this.panelCode.BackColor = System.Drawing.Color.White;
            this.panelCode.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelCode.Controls.Add(this.labelCodeCard);
            this.panelCode.Controls.Add(this.lblFileCodeBehind);
            this.panelCode.Controls.Add(this.lblFileDesigner);
            this.panelCode.Controls.Add(this.lblFileProgram);
            this.panelCode.Controls.Add(this.lblVocabulary);
            this.panelCode.Location = new System.Drawing.Point(30, 428);
            this.panelCode.Name = "panelCode";
            this.panelCode.Size = new System.Drawing.Size(560, 182);
            //
            // labelCodeCard
            //
            this.labelCodeCard.AutoSize = false;
            this.labelCodeCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelCodeCard.Location = new System.Drawing.Point(24, 14);
            this.labelCodeCard.Name = "labelCodeCard";
            this.labelCodeCard.Size = new System.Drawing.Size(512, 28);
            this.labelCodeCard.Text = "Where your code goes  ·  properties, events, handlers, methods";
            //
            // lblFileCodeBehind
            //
            this.lblFileCodeBehind.AutoSize = false;
            this.lblFileCodeBehind.Font = new System.Drawing.Font("monospace", 9F);
            this.lblFileCodeBehind.Location = new System.Drawing.Point(24, 48);
            this.lblFileCodeBehind.Name = "lblFileCodeBehind";
            this.lblFileCodeBehind.Size = new System.Drawing.Size(512, 20);
            this.lblFileCodeBehind.Text = "DashboardWindow.cs           your Click handlers + AddLog() — edit this";
            //
            // lblFileDesigner
            //
            this.lblFileDesigner.AutoSize = false;
            this.lblFileDesigner.Font = new System.Drawing.Font("monospace", 9F);
            this.lblFileDesigner.Location = new System.Drawing.Point(24, 68);
            this.lblFileDesigner.Name = "lblFileDesigner";
            this.lblFileDesigner.Size = new System.Drawing.Size(512, 20);
            this.lblFileDesigner.Text = "DashboardWindow.Designer.cs  generated layout (InitializeComponent) — don't edit";
            //
            // lblFileProgram
            //
            this.lblFileProgram.AutoSize = false;
            this.lblFileProgram.Font = new System.Drawing.Font("monospace", 9F);
            this.lblFileProgram.Location = new System.Drawing.Point(24, 88);
            this.lblFileProgram.Name = "lblFileProgram";
            this.lblFileProgram.Size = new System.Drawing.Size(512, 20);
            this.lblFileProgram.Text = "Program.cs                   startup: new DashboardWindow().Show()";
            //
            // lblVocabulary  (lesson s7 §1: property · event · event handler · method)
            //
            this.lblVocabulary.AutoSize = false;
            this.lblVocabulary.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblVocabulary.Font = new System.Drawing.Font("monospace", 9F);
            this.lblVocabulary.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblVocabulary.Location = new System.Drawing.Point(24, 114);
            this.lblVocabulary.Name = "lblVocabulary";
            this.lblVocabulary.Padding = new Wisej.Web.Padding(10, 4, 10, 4);
            this.lblVocabulary.Size = new System.Drawing.Size(512, 60);
            this.lblVocabulary.Text = "Property       btnStart.Text = \"Start\"\nEvent          btnStart.Click  →  event handler btnStart_Click(sender, e)\nMethod         lstEventLog.Items.Add(\"12:00:00  Dashboard started.\")";
            this.lblVocabulary.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelLog  (Event Log · every line goes through AddLog)
            //
            this.panelLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelLog.BackColor = System.Drawing.Color.White;
            this.panelLog.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelLog.Controls.Add(this.labelLogCard);
            this.panelLog.Controls.Add(this.lstEventLog);
            this.panelLog.Controls.Add(this.labelLogFooter);
            this.panelLog.Location = new System.Drawing.Point(618, 30);
            this.panelLog.Name = "panelLog";
            this.panelLog.Size = new System.Drawing.Size(700, 580);
            //
            // labelLogCard
            //
            this.labelLogCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLogCard.AutoSize = false;
            this.labelLogCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelLogCard.Location = new System.Drawing.Point(20, 14);
            this.labelLogCard.Name = "labelLogCard";
            this.labelLogCard.Size = new System.Drawing.Size(660, 30);
            this.labelLogCard.Text = "Event Log  ·  AddLog(message) → lstEventLog.Items.Add(DateTime.Now …)";
            //
            // lstEventLog
            //
            this.lstEventLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstEventLog.Font = new System.Drawing.Font("monospace", 9F);
            this.lstEventLog.Location = new System.Drawing.Point(20, 52);
            this.lstEventLog.Name = "lstEventLog";
            this.lstEventLog.Size = new System.Drawing.Size(660, 476);
            //
            // labelLogFooter
            //
            this.labelLogFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLogFooter.AutoSize = false;
            this.labelLogFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLogFooter.Location = new System.Drawing.Point(20, 538);
            this.labelLogFooter.Name = "labelLogFooter";
            this.labelLogFooter.Size = new System.Drawing.Size(660, 26);
            this.labelLogFooter.Text = "browser click → btnX.Click → btnX_Click on the server → labels + log updated → browser refreshed";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.btnSimulateOutage);
            this.panelActions.Controls.Add(this.btnRecover);
            this.panelActions.Location = new System.Drawing.Point(30, 626);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // btnSimulateOutage  (failure path: outage on, then the same Refresh handler)
            //
            this.btnSimulateOutage.Location = new System.Drawing.Point(0, 4);
            this.btnSimulateOutage.Name = "btnSimulateOutage";
            this.btnSimulateOutage.Size = new System.Drawing.Size(280, 36);
            this.btnSimulateOutage.Text = "Simulate API outage + Refresh (failure)";
            this.btnSimulateOutage.ToolTipText = "Checks chkSimulateOutage and runs btnRefresh_Click — the API Service check times out.";
            this.btnSimulateOutage.Click += new System.EventHandler(this.btnSimulateOutage_Click);
            //
            // btnRecover  (recovery: outage off, then the same Refresh handler)
            //
            this.btnRecover.Location = new System.Drawing.Point(288, 4);
            this.btnRecover.Name = "btnRecover";
            this.btnRecover.Size = new System.Drawing.Size(280, 36);
            this.btnRecover.Text = "Clear outage + Refresh (recovery)";
            this.btnRecover.ToolTipText = "Unchecks chkSimulateOutage and runs btnRefresh_Click — the API Service answers again.";
            this.btnRecover.Click += new System.EventHandler(this.btnRecover_Click);
            //
            // DashboardWindow
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelDashboard);
            this.Controls.Add(this.panelCode);
            this.Controls.Add(this.panelLog);
            this.Controls.Add(this.panelActions);
            this.Name = "DashboardWindow";
            this.Text = "WisejTrainingApp — System Dashboard (Module 2)";
            this.Load += new System.EventHandler(this.DashboardWindow_Load);
            this.panelDashboard.ResumeLayout(false);
            this.pnlServices.ResumeLayout(false);
            this.panelCode.ResumeLayout(false);
            this.panelLog.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelDashboard;
        private Wisej.Web.Label labelDashboardCard;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlServices;
        private Wisej.Web.Label lblServerStatus;
        private Wisej.Web.Label lblDatabaseStatus;
        private Wisej.Web.Label lblApiStatus;
        private Wisej.Web.Button btnStart;
        private Wisej.Web.Button btnStop;
        private Wisej.Web.Button btnReset;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.CheckBox chkSimulateOutage;
        private Wisej.Web.Label lblHint;
        private Wisej.Web.Panel panelCode;
        private Wisej.Web.Label labelCodeCard;
        private Wisej.Web.Label lblFileCodeBehind;
        private Wisej.Web.Label lblFileDesigner;
        private Wisej.Web.Label lblFileProgram;
        private Wisej.Web.Label lblVocabulary;
        private Wisej.Web.Panel panelLog;
        private Wisej.Web.Label labelLogCard;
        private Wisej.Web.ListBox lstEventLog;
        private Wisej.Web.Label labelLogFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button btnSimulateOutage;
        private Wisej.Web.Button btnRecover;
    }
}
