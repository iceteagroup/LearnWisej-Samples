namespace WisejTrainingApp
{
    partial class JobsWindow
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
            this.panelRunner = new Wisej.Web.Panel();
            this.labelRunnerCard = new Wisej.Web.Label();
            this.btnStartImport = new Wisej.Web.Button();
            this.btnStartExport = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.btnClearLog = new Wisej.Web.Button();
            this.chkSimulateError = new Wisej.Web.CheckBox();
            this.progressBar = new Wisej.Web.ProgressBar();
            this.lblStatus = new Wisej.Web.Label();
            this.lblStatusHint = new Wisej.Web.Label();
            this.lblJobInfo = new Wisej.Web.Label();
            this.lblHint = new Wisej.Web.Label();
            this.panelSession = new Wisej.Web.Panel();
            this.labelSessionCard = new Wisej.Web.Label();
            this.btnRefreshState = new Wisej.Web.Button();
            this.lblSessionId = new Wisej.Web.Label();
            this.lblSessionJobs = new Wisej.Web.Label();
            this.lblLastJob = new Wisej.Web.Label();
            this.lblServerJobs = new Wisej.Web.Label();
            this.lblStaticWarning = new Wisej.Web.Label();
            this.panelLog = new Wisej.Web.Panel();
            this.labelLogCard = new Wisej.Web.Label();
            this.lstLog = new Wisej.Web.ListBox();
            this.labelLogFooter = new Wisej.Web.Label();
            this.panelRunner.SuspendLayout();
            this.panelSession.SuspendLayout();
            this.panelLog.SuspendLayout();
            this.SuspendLayout();
            //
            // panelRunner  (the "Job runner" card — every control the lab guide names lives here)
            //
            this.panelRunner.BackColor = System.Drawing.Color.White;
            this.panelRunner.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelRunner.Controls.Add(this.labelRunnerCard);
            this.panelRunner.Controls.Add(this.btnStartImport);
            this.panelRunner.Controls.Add(this.btnStartExport);
            this.panelRunner.Controls.Add(this.btnCancel);
            this.panelRunner.Controls.Add(this.btnClearLog);
            this.panelRunner.Controls.Add(this.chkSimulateError);
            this.panelRunner.Controls.Add(this.progressBar);
            this.panelRunner.Controls.Add(this.lblStatus);
            this.panelRunner.Controls.Add(this.lblStatusHint);
            this.panelRunner.Controls.Add(this.lblJobInfo);
            this.panelRunner.Controls.Add(this.lblHint);
            this.panelRunner.Location = new System.Drawing.Point(30, 30);
            this.panelRunner.Name = "panelRunner";
            this.panelRunner.Size = new System.Drawing.Size(560, 372);
            //
            // labelRunnerCard
            //
            this.labelRunnerCard.AutoSize = false;
            this.labelRunnerCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelRunnerCard.Location = new System.Drawing.Point(24, 14);
            this.labelRunnerCard.Name = "labelRunnerCard";
            this.labelRunnerCard.Size = new System.Drawing.Size(512, 28);
            this.labelRunnerCard.Text = "Job runner  ·  start → progress → complete / cancel / fail → reset";
            //
            // btnStartImport  (double-click in the Designer created btnStartImport_Click)
            //
            this.btnStartImport.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnStartImport.Location = new System.Drawing.Point(24, 50);
            this.btnStartImport.Name = "btnStartImport";
            this.btnStartImport.Size = new System.Drawing.Size(122, 36);
            this.btnStartImport.Text = "Start Import";
            this.btnStartImport.ToolTipText = "btnStartImport_Click: try { SetJobRunning(true); await RunImportJobAsync(); } catch { LogError } finally { SetJobRunning(false) }";
            this.btnStartImport.Click += new System.EventHandler(this.btnStartImport_Click);
            //
            // btnStartExport
            //
            this.btnStartExport.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnStartExport.Location = new System.Drawing.Point(154, 50);
            this.btnStartExport.Name = "btnStartExport";
            this.btnStartExport.Size = new System.Drawing.Size(122, 36);
            this.btnStartExport.Text = "Start Export";
            this.btnStartExport.ToolTipText = "Same handler pattern as Start Import; the job comes from JobCatalog.ExportJob().";
            this.btnStartExport.Click += new System.EventHandler(this.btnStartExport_Click);
            //
            // btnCancel  (disabled until a job runs — SetJobRunning(true) enables it)
            //
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(284, 50);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(116, 36);
            this.btnCancel.Text = "Cancel Job";
            this.btnCancel.ToolTipText = "Cancels the CancellationTokenSource of THIS session's job (_currentJob, an instance field).";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnClearLog
            //
            this.btnClearLog.Location = new System.Drawing.Point(408, 50);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(128, 36);
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            // chkSimulateError  (the failure path: RunJobAsync throws at step 4 when this is checked)
            //
            this.chkSimulateError.Location = new System.Drawing.Point(24, 96);
            this.chkSimulateError.Name = "chkSimulateError";
            this.chkSimulateError.Size = new System.Drawing.Size(512, 24);
            this.chkSimulateError.Text = "Simulate error at step 4  (InvalidOperationException with developer detail — the user sees a safe message)";
            this.chkSimulateError.CheckedChanged += new System.EventHandler(this.chkSimulateError_CheckedChanged);
            //
            // progressBar
            //
            this.progressBar.Location = new System.Drawing.Point(24, 130);
            this.progressBar.Maximum = 100;
            this.progressBar.Minimum = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(512, 22);
            this.progressBar.Value = 0;
            //
            // lblStatus  (the SAFE message — the only thing the user reads about a failure)
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(24, 160);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(512, 26);
            this.lblStatus.Text = "● ready";
            //
            // lblStatusHint
            //
            this.lblStatusHint.AutoSize = false;
            this.lblStatusHint.Font = new System.Drawing.Font("monospace", 8F);
            this.lblStatusHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStatusHint.Location = new System.Drawing.Point(24, 186);
            this.lblStatusHint.Name = "lblStatusHint";
            this.lblStatusHint.Size = new System.Drawing.Size(512, 20);
            this.lblStatusHint.Text = "state: IDLE";
            //
            // lblJobInfo  (job name, step x/y, elapsed, token)
            //
            this.lblJobInfo.AutoSize = false;
            this.lblJobInfo.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblJobInfo.Font = new System.Drawing.Font("monospace", 9F);
            this.lblJobInfo.Location = new System.Drawing.Point(24, 212);
            this.lblJobInfo.Name = "lblJobInfo";
            this.lblJobInfo.Padding = new Wisej.Web.Padding(12, 8, 12, 8);
            this.lblJobInfo.Size = new System.Drawing.Size(512, 88);
            this.lblJobInfo.Text = "Job:      —\nStep:     —\nElapsed:  —\nToken:    —";
            this.lblJobInfo.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // lblHint
            //
            this.lblHint.AutoSize = false;
            this.lblHint.Font = new System.Drawing.Font("monospace", 9F);
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblHint.Location = new System.Drawing.Point(24, 308);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(512, 56);
            this.lblHint.Text = "Controls: btnStartImport · btnStartExport · btnCancel · btnClearLog · chkSimulateError · progressBar · lblStatus · lstLog\nHandler:  btnStartImport_Click → try { SetJobRunning(true); await RunImportJobAsync(); }\n          catch (Exception ex) { LogError(ex); safe message } finally { SetJobRunning(false); }";
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelSession  (Session vs shared state — the static-field trap, visible)
            //
            this.panelSession.BackColor = System.Drawing.Color.White;
            this.panelSession.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSession.Controls.Add(this.labelSessionCard);
            this.panelSession.Controls.Add(this.btnRefreshState);
            this.panelSession.Controls.Add(this.lblSessionId);
            this.panelSession.Controls.Add(this.lblSessionJobs);
            this.panelSession.Controls.Add(this.lblLastJob);
            this.panelSession.Controls.Add(this.lblServerJobs);
            this.panelSession.Controls.Add(this.lblStaticWarning);
            this.panelSession.Location = new System.Drawing.Point(30, 420);
            this.panelSession.Name = "panelSession";
            this.panelSession.Size = new System.Drawing.Size(560, 250);
            //
            // labelSessionCard
            //
            this.labelSessionCard.AutoSize = false;
            this.labelSessionCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelSessionCard.Location = new System.Drawing.Point(24, 14);
            this.labelSessionCard.Name = "labelSessionCard";
            this.labelSessionCard.Size = new System.Drawing.Size(360, 28);
            this.labelSessionCard.Text = "Session vs shared state  ·  the static-field trap";
            //
            // btnRefreshState
            //
            this.btnRefreshState.Location = new System.Drawing.Point(396, 12);
            this.btnRefreshState.Name = "btnRefreshState";
            this.btnRefreshState.Size = new System.Drawing.Size(140, 32);
            this.btnRefreshState.Text = "Refresh counters";
            this.btnRefreshState.ToolTipText = "Re-reads the instance counter, the session bag and the static counter — use it in a second tab.";
            this.btnRefreshState.Click += new System.EventHandler(this.btnRefreshState_Click);
            //
            // lblSessionId
            //
            this.lblSessionId.AutoSize = false;
            this.lblSessionId.Font = new System.Drawing.Font("monospace", 9F);
            this.lblSessionId.Location = new System.Drawing.Point(24, 52);
            this.lblSessionId.Name = "lblSessionId";
            this.lblSessionId.Size = new System.Drawing.Size(512, 22);
            this.lblSessionId.Text = "Session: …";
            //
            // lblSessionJobs  (instance field — per user)
            //
            this.lblSessionJobs.AutoSize = false;
            this.lblSessionJobs.Font = new System.Drawing.Font("monospace", 9F);
            this.lblSessionJobs.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblSessionJobs.Location = new System.Drawing.Point(24, 76);
            this.lblSessionJobs.Name = "lblSessionJobs";
            this.lblSessionJobs.Size = new System.Drawing.Size(512, 22);
            this.lblSessionJobs.Text = "Jobs run in this session: 0";
            //
            // lblLastJob  (Application.Session bag — per user)
            //
            this.lblLastJob.AutoSize = false;
            this.lblLastJob.AutoEllipsis = true;
            this.lblLastJob.Font = new System.Drawing.Font("monospace", 9F);
            this.lblLastJob.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblLastJob.Location = new System.Drawing.Point(24, 100);
            this.lblLastJob.Name = "lblLastJob";
            this.lblLastJob.Size = new System.Drawing.Size(512, 22);
            this.lblLastJob.Text = "Last job (session bag): —";
            //
            // lblServerJobs  (static field — shared by every session on this server)
            //
            this.lblServerJobs.AutoSize = false;
            this.lblServerJobs.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblServerJobs.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblServerJobs.Location = new System.Drawing.Point(24, 134);
            this.lblServerJobs.Name = "lblServerJobs";
            this.lblServerJobs.Size = new System.Drawing.Size(512, 22);
            this.lblServerJobs.Text = "Jobs run on this server (static): 0";
            //
            // lblStaticWarning
            //
            this.lblStaticWarning.AutoSize = false;
            this.lblStaticWarning.BackColor = System.Drawing.Color.FromArgb(255, 247, 232);
            this.lblStaticWarning.ForeColor = System.Drawing.Color.FromArgb(150, 92, 20);
            this.lblStaticWarning.Location = new System.Drawing.Point(24, 162);
            this.lblStaticWarning.Name = "lblStaticWarning";
            this.lblStaticWarning.Padding = new Wisej.Web.Padding(12, 8, 12, 8);
            this.lblStaticWarning.Size = new System.Drawing.Size(512, 72);
            this.lblStaticWarning.Text = "⚠ static = shared by every user — open a second browser tab and run a job there: its session counter starts at 0, but the static counter keeps counting for both tabs. Per-user state (the current job, its token, its progress) belongs in instance fields or Application.Session, never in a static field.";
            this.lblStaticWarning.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelLog  (Job log · developer detail, timestamped)
            //
            this.panelLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelLog.BackColor = System.Drawing.Color.White;
            this.panelLog.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelLog.Controls.Add(this.labelLogCard);
            this.panelLog.Controls.Add(this.lstLog);
            this.panelLog.Controls.Add(this.labelLogFooter);
            this.panelLog.Location = new System.Drawing.Point(618, 30);
            this.panelLog.Name = "panelLog";
            this.panelLog.Size = new System.Drawing.Size(700, 640);
            //
            // labelLogCard
            //
            this.labelLogCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLogCard.AutoSize = false;
            this.labelLogCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelLogCard.Location = new System.Drawing.Point(20, 14);
            this.labelLogCard.Name = "labelLogCard";
            this.labelLogCard.Size = new System.Drawing.Size(660, 30);
            this.labelLogCard.Text = "Job log  ·  timestamp + job name on every line (developer detail)";
            //
            // lstLog
            //
            this.lstLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstLog.Font = new System.Drawing.Font("monospace", 9F);
            this.lstLog.Location = new System.Drawing.Point(20, 52);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(660, 536);
            //
            // labelLogFooter
            //
            this.labelLogFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLogFooter.AutoSize = false;
            this.labelLogFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLogFooter.Location = new System.Drawing.Point(20, 598);
            this.labelLogFooter.Name = "labelLogFooter";
            this.labelLogFooter.Size = new System.Drawing.Size(660, 26);
            this.labelLogFooter.Text = "click → handler → await each step → Application.Update(this) pushes progress → finally resets the buttons";
            //
            // JobsWindow
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 700);
            this.Controls.Add(this.panelRunner);
            this.Controls.Add(this.panelSession);
            this.Controls.Add(this.panelLog);
            this.Name = "JobsWindow";
            this.Text = "WisejTrainingApp — Background jobs (Module 6)";
            this.Load += new System.EventHandler(this.JobsWindow_Load);
            this.panelRunner.ResumeLayout(false);
            this.panelSession.ResumeLayout(false);
            this.panelLog.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelRunner;
        private Wisej.Web.Label labelRunnerCard;
        private Wisej.Web.Button btnStartImport;
        private Wisej.Web.Button btnStartExport;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Button btnClearLog;
        private Wisej.Web.CheckBox chkSimulateError;
        private Wisej.Web.ProgressBar progressBar;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblStatusHint;
        private Wisej.Web.Label lblJobInfo;
        private Wisej.Web.Label lblHint;
        private Wisej.Web.Panel panelSession;
        private Wisej.Web.Label labelSessionCard;
        private Wisej.Web.Button btnRefreshState;
        private Wisej.Web.Label lblSessionId;
        private Wisej.Web.Label lblSessionJobs;
        private Wisej.Web.Label lblLastJob;
        private Wisej.Web.Label lblServerJobs;
        private Wisej.Web.Label lblStaticWarning;
        private Wisej.Web.Panel panelLog;
        private Wisej.Web.Label labelLogCard;
        private Wisej.Web.ListBox lstLog;
        private Wisej.Web.Label labelLogFooter;
    }
}
