namespace TicketOps.Views
{
    partial class ImportPage
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
            this.panelScreen = new Wisej.Web.Panel();
            this.labelScreenTitle = new Wisej.Web.Label();
            this.statusBanner = new TicketOps.Controls.StatusBanner();
            this.labelFile = new Wisej.Web.Label();
            this.buttonStartImport = new Wisej.Web.Button();
            this.buttonCancelImport = new Wisej.Web.Button();
            this.buttonRefreshList = new Wisej.Web.Button();
            this.labelTicketCount = new Wisej.Web.Label();
            this.labelProgressCaption = new Wisej.Web.Label();
            this.labelRowsDone = new Wisej.Web.Label();
            this.labelPercent = new Wisej.Web.Label();
            this.progressImport = new Wisej.Web.ProgressBar();
            this.labelLogCaption = new Wisej.Web.Label();
            this.listImportLog = new Wisej.Web.ListBox();
            this.labelState = new Wisej.Web.Label();
            this.tracePanel = new TicketOps.Diagnostics.ActivityTracePanel();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonWrongHeader = new Wisej.Web.Button();
            this.buttonStartTwice = new Wisej.Web.Button();
            this.buttonOutage = new Wisej.Web.Button();
            this.buttonResetData = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.panelScreen.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen  (Ticket Import: file, buttons, progress, import log, server state — display and input only)
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.labelFile);
            this.panelScreen.Controls.Add(this.buttonStartImport);
            this.panelScreen.Controls.Add(this.buttonCancelImport);
            this.panelScreen.Controls.Add(this.buttonRefreshList);
            this.panelScreen.Controls.Add(this.labelTicketCount);
            this.panelScreen.Controls.Add(this.labelProgressCaption);
            this.panelScreen.Controls.Add(this.labelRowsDone);
            this.panelScreen.Controls.Add(this.labelPercent);
            this.panelScreen.Controls.Add(this.progressImport);
            this.panelScreen.Controls.Add(this.labelLogCaption);
            this.panelScreen.Controls.Add(this.listImportLog);
            this.panelScreen.Controls.Add(this.labelState);
            this.panelScreen.Location = new System.Drawing.Point(30, 30);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(760, 560);
            //
            // labelScreenTitle
            //
            this.labelScreenTitle.AutoSize = false;
            this.labelScreenTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelScreenTitle.Location = new System.Drawing.Point(24, 18);
            this.labelScreenTitle.Name = "labelScreenTitle";
            this.labelScreenTitle.Size = new System.Drawing.Size(300, 30);
            this.labelScreenTitle.Text = "Ticket Import (CSV)";
            //
            // statusBanner  (Controls/StatusBanner: "● state" + banner line)
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // labelFile  (which file, how many rows, which columns)
            //
            this.labelFile.AutoSize = false;
            this.labelFile.Font = new System.Drawing.Font("monospace", 9F);
            this.labelFile.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelFile.Location = new System.Drawing.Point(24, 52);
            this.labelFile.Name = "labelFile";
            this.labelFile.Size = new System.Drawing.Size(712, 20);
            this.labelFile.Text = "opening the sample file…";
            //
            // buttonStartImport  (success + progress path: the background task launcher)
            //
            this.buttonStartImport.Location = new System.Drawing.Point(24, 86);
            this.buttonStartImport.Name = "buttonStartImport";
            this.buttonStartImport.Size = new System.Drawing.Size(180, 36);
            this.buttonStartImport.Text = "▶ Start import";
            this.buttonStartImport.ToolTipText = "Success + progress path: Application.StartTask runs IImportService.ImportAsync off the round-trip; progress is pushed with Application.Update(this, …) every 25 rows. Bad rows are logged and skipped.";
            this.buttonStartImport.Click += new System.EventHandler(this.buttonStartImport_Click);
            //
            // buttonCancelImport  (cooperative cancellation)
            //
            this.buttonCancelImport.Enabled = false;
            this.buttonCancelImport.Location = new System.Drawing.Point(214, 86);
            this.buttonCancelImport.Name = "buttonCancelImport";
            this.buttonCancelImport.Size = new System.Drawing.Size(120, 36);
            this.buttonCancelImport.Text = "⏹ Cancel";
            this.buttonCancelImport.ToolTipText = "Cancellation: _importCancel.Cancel() — the task checks the token before every row and stops between two rows; imported rows stay, Start becomes Resume.";
            this.buttonCancelImport.Click += new System.EventHandler(this.buttonCancelImport_Click);
            //
            // buttonRefreshList  (the request thread reads while the task writes)
            //
            this.buttonRefreshList.Location = new System.Drawing.Point(344, 86);
            this.buttonRefreshList.Name = "buttonRefreshList";
            this.buttonRefreshList.Size = new System.Drawing.Size(140, 36);
            this.buttonRefreshList.Text = "↻ Refresh list";
            this.buttonRefreshList.ToolTipText = "Click it DURING the import: the request thread reads the repository (under its lock) while the task writes — the screen was never blocked.";
            this.buttonRefreshList.Click += new System.EventHandler(this.buttonRefreshList_Click);
            //
            // labelTicketCount
            //
            this.labelTicketCount.AutoSize = false;
            this.labelTicketCount.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelTicketCount.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.labelTicketCount.Location = new System.Drawing.Point(494, 92);
            this.labelTicketCount.Name = "labelTicketCount";
            this.labelTicketCount.Size = new System.Drawing.Size(242, 24);
            this.labelTicketCount.Text = "… tickets in the repository";
            this.labelTicketCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // progress: caption, rows done, percent, bar
            //
            this.labelProgressCaption.AutoSize = false;
            this.labelProgressCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelProgressCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelProgressCaption.Location = new System.Drawing.Point(24, 136);
            this.labelProgressCaption.Name = "labelProgressCaption";
            this.labelProgressCaption.Size = new System.Drawing.Size(120, 20);
            this.labelProgressCaption.Text = "PROGRESS";
            this.labelRowsDone.AutoSize = false;
            this.labelRowsDone.Font = new System.Drawing.Font("monospace", 9F);
            this.labelRowsDone.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelRowsDone.Location = new System.Drawing.Point(150, 136);
            this.labelRowsDone.Name = "labelRowsDone";
            this.labelRowsDone.Size = new System.Drawing.Size(500, 20);
            this.labelRowsDone.Text = "0 of 0 rows";
            this.labelRowsDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelPercent.AutoSize = false;
            this.labelPercent.Font = new System.Drawing.Font("monospace", 10F, System.Drawing.FontStyle.Bold);
            this.labelPercent.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.labelPercent.Location = new System.Drawing.Point(656, 136);
            this.labelPercent.Name = "labelPercent";
            this.labelPercent.Size = new System.Drawing.Size(80, 20);
            this.labelPercent.Text = "0%";
            this.labelPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.progressImport.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.progressImport.Location = new System.Drawing.Point(24, 160);
            this.progressImport.Maximum = 100;
            this.progressImport.Name = "progressImport";
            this.progressImport.Size = new System.Drawing.Size(712, 18);
            //
            // import log: caption + list (per-row errors are listed here without stopping the run)
            //
            this.labelLogCaption.AutoSize = false;
            this.labelLogCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelLogCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelLogCaption.Location = new System.Drawing.Point(24, 190);
            this.labelLogCaption.Name = "labelLogCaption";
            this.labelLogCaption.Size = new System.Drawing.Size(400, 20);
            this.labelLogCaption.Text = "IMPORT LOG  ·  ⚠ skipped row   ✔ done   ⏹ cancelled   ✖ stopped";
            this.listImportLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listImportLog.Font = new System.Drawing.Font("monospace", 9F);
            this.listImportLog.Location = new System.Drawing.Point(24, 212);
            this.listImportLog.Name = "listImportLog";
            this.listImportLog.Size = new System.Drawing.Size(712, 244);
            //
            // labelState  (what the server owns right now — the fields both threads share)
            //
            this.labelState.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelState.AutoSize = false;
            this.labelState.Font = new System.Drawing.Font("monospace", 8.5F);
            this.labelState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelState.Location = new System.Drawing.Point(24, 466);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(712, 80);
            this.labelState.Text = "SERVER STATE (this session only)";
            //
            // tracePanel  (Diagnostics: the live activity trace)
            //
            this.tracePanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.tracePanel.Location = new System.Drawing.Point(810, 30);
            this.tracePanel.Name = "tracePanel";
            this.tracePanel.Size = new System.Drawing.Size(508, 560);
            //
            // panelActions  (bottom bar: validation failure / sync guard / outage + recovery / reset / clear)
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonWrongHeader);
            this.panelActions.Controls.Add(this.buttonStartTwice);
            this.panelActions.Controls.Add(this.buttonOutage);
            this.panelActions.Controls.Add(this.buttonResetData);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 606);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // bottom bar buttons
            //
            this.buttonWrongHeader.Location = new System.Drawing.Point(0, 4);
            this.buttonWrongHeader.Name = "buttonWrongHeader";
            this.buttonWrongHeader.Size = new System.Drawing.Size(210, 36);
            this.buttonWrongHeader.Text = "Import wrong-header file";
            this.buttonWrongHeader.ToolTipText = "Failure path (validation): IImportService.OpenAsync rejects a CSV whose header lacks the required columns — a result, not an exception; no task starts.";
            this.buttonWrongHeader.Click += new System.EventHandler(this.buttonWrongHeader_Click);
            this.buttonStartTwice.Location = new System.Drawing.Point(220, 4);
            this.buttonStartTwice.Name = "buttonStartTwice";
            this.buttonStartTwice.Size = new System.Drawing.Size(170, 36);
            this.buttonStartTwice.Text = "Start import twice";
            this.buttonStartTwice.ToolTipText = "Synchronization: two starts in one request — the second is refused by the lock-guarded _isRunning flag, not by a disabled button.";
            this.buttonStartTwice.Click += new System.EventHandler(this.buttonStartTwice_Click);
            this.buttonOutage.Location = new System.Drawing.Point(400, 4);
            this.buttonOutage.Name = "buttonOutage";
            this.buttonOutage.Size = new System.Drawing.Size(200, 36);
            this.buttonOutage.Text = "Simulate data outage";
            this.buttonOutage.ToolTipText = "Error path + recovery: click it DURING an import — the next write throws, the run stops cleanly before that row and reports where to resume. Click again to recover, then ▶ Resume import.";
            this.buttonOutage.Click += new System.EventHandler(this.buttonOutage_Click);
            this.buttonResetData.Location = new System.Drawing.Point(610, 4);
            this.buttonResetData.Name = "buttonResetData";
            this.buttonResetData.Size = new System.Drawing.Size(170, 36);
            this.buttonResetData.Text = "Reset sample data";
            this.buttonResetData.ToolTipText = "Re-seeds the repository and moves the resume row back to 1 so the full run can be repeated. Refused while an import is running.";
            this.buttonResetData.Click += new System.EventHandler(this.buttonResetData_Click);
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(1148, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(140, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // ImportPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelScreen);
            this.Controls.Add(this.tracePanel);
            this.Controls.Add(this.panelActions);
            this.Name = "ImportPage";
            this.Text = "TicketOps Console — Module 7 · Background tasks, real-time updates & synchronization";
            this.Load += new System.EventHandler(this.ImportPage_Load);
            this.panelScreen.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label labelFile;
        private Wisej.Web.Button buttonStartImport;
        private Wisej.Web.Button buttonCancelImport;
        private Wisej.Web.Button buttonRefreshList;
        private Wisej.Web.Label labelTicketCount;
        private Wisej.Web.Label labelProgressCaption;
        private Wisej.Web.Label labelRowsDone;
        private Wisej.Web.Label labelPercent;
        private Wisej.Web.ProgressBar progressImport;
        private Wisej.Web.Label labelLogCaption;
        private Wisej.Web.ListBox listImportLog;
        private Wisej.Web.Label labelState;
        private TicketOps.Diagnostics.ActivityTracePanel tracePanel;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonWrongHeader;
        private Wisej.Web.Button buttonStartTwice;
        private Wisej.Web.Button buttonOutage;
        private Wisej.Web.Button buttonResetData;
        private Wisej.Web.Button buttonClear;
    }
}
