namespace EnterpriseOps.UI
{
    partial class ImportCenterPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used. The page also detaches from the session-scoped trace and from
        /// the process-wide job store here: the page dies, the job does not.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DetachFromSession();
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblTitle = new Wisej.Web.Label();
            this.lblContext = new Wisej.Web.Label();
            this.btnBell = new Wisej.Web.Button();
            this.pnlQueue = new Wisej.Web.Panel();
            this.lblQueueTitle = new Wisej.Web.Label();
            this.dgvJobs = new Wisej.Web.DataGridView();
            this.colNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDescription = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colProgress = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colRows = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStartedBy = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStarted = new Wisej.Web.DataGridViewTextBoxColumn();
            this.prgJob = new Wisej.Web.ProgressBar();
            this.lblPercent = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.lblBanner = new Wisej.Web.Label();
            this.pnlJobDetail = new EnterpriseOps.UI.JobDetailPanel();
            this.pnlNotifications = new Wisej.Web.Panel();
            this.lblNotificationsTitle = new Wisej.Web.Label();
            this.btnMarkRead = new Wisej.Web.Button();
            this.lstNotifications = new Wisej.Web.ListBox();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.pnlActions = new Wisej.Web.Panel();
            this.cboFile = new Wisej.Web.ComboBox();
            this.btnStartImport = new Wisej.Web.Button();
            this.btnCancelJob = new Wisej.Web.Button();
            this.btnReimport = new Wisej.Web.Button();
            this.btnMalformedFile = new Wisej.Web.Button();
            this.btnAntiPattern = new Wisej.Web.Button();
            this.btnReopenPage = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlQueue.SuspendLayout();
            this.pnlNotifications.SuspendLayout();
            this.pnlTrace.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (screen name · tenant · user · correlation id · the bell)
            //
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblContext);
            this.pnlHeader.Controls.Add(this.btnBell);
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1348, 52);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(16, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(220, 30);
            this.lblTitle.Text = "Import Center";
            //
            // lblContext
            //
            this.lblContext.AutoSize = false;
            this.lblContext.Font = new System.Drawing.Font("monospace", 9F);
            this.lblContext.ForeColor = System.Drawing.Color.FromArgb(110, 126, 142);
            this.lblContext.Location = new System.Drawing.Point(244, 16);
            this.lblContext.Name = "lblContext";
            this.lblContext.Size = new System.Drawing.Size(880, 20);
            this.lblContext.Text = "tenant — · user — · session —";
            //
            // btnBell
            //
            this.btnBell.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnBell.Location = new System.Drawing.Point(1148, 10);
            this.btnBell.Name = "btnBell";
            this.btnBell.Size = new System.Drawing.Size(184, 30);
            this.btnBell.Text = "🔔 Notifications";
            this.btnBell.Click += new System.EventHandler(this.btnBell_Click);
            //
            // pnlQueue  (the job queue grid + the progress observer's output)
            //
            this.pnlQueue.BackColor = System.Drawing.Color.White;
            this.pnlQueue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlQueue.Controls.Add(this.lblQueueTitle);
            this.pnlQueue.Controls.Add(this.dgvJobs);
            this.pnlQueue.Controls.Add(this.prgJob);
            this.pnlQueue.Controls.Add(this.lblPercent);
            this.pnlQueue.Controls.Add(this.lblStatus);
            this.pnlQueue.Controls.Add(this.lblBanner);
            this.pnlQueue.Location = new System.Drawing.Point(16, 64);
            this.pnlQueue.Name = "pnlQueue";
            this.pnlQueue.Size = new System.Drawing.Size(884, 248);
            //
            // lblQueueTitle
            //
            this.lblQueueTitle.AutoSize = false;
            this.lblQueueTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblQueueTitle.Location = new System.Drawing.Point(16, 10);
            this.lblQueueTitle.Name = "lblQueueTitle";
            this.lblQueueTitle.Size = new System.Drawing.Size(852, 24);
            this.lblQueueTitle.Text = "Job queue";
            //
            // dgvJobs
            //
            this.dgvJobs.AutoGenerateColumns = false;
            this.dgvJobs.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.colNumber,
                this.colDescription,
                this.colStatus,
                this.colProgress,
                this.colRows,
                this.colStartedBy,
                this.colStarted});
            this.dgvJobs.Location = new System.Drawing.Point(16, 38);
            this.dgvJobs.MultiSelect = false;
            this.dgvJobs.Name = "dgvJobs";
            this.dgvJobs.ReadOnly = true;
            this.dgvJobs.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvJobs.Size = new System.Drawing.Size(852, 142);
            this.dgvJobs.SelectionChanged += new System.EventHandler(this.dgvJobs_SelectionChanged);
            //
            // colNumber
            //
            this.colNumber.DataPropertyName = "Number";
            this.colNumber.HeaderText = "Job";
            this.colNumber.Name = "colNumber";
            this.colNumber.Width = 90;
            //
            // colDescription
            //
            this.colDescription.DataPropertyName = "Description";
            this.colDescription.HeaderText = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.Width = 262;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 145;
            //
            // colProgress
            //
            this.colProgress.DataPropertyName = "Progress";
            this.colProgress.HeaderText = "Progress";
            this.colProgress.Name = "colProgress";
            this.colProgress.Width = 70;
            //
            // colRows
            //
            this.colRows.DataPropertyName = "Rows";
            this.colRows.HeaderText = "Rows";
            this.colRows.Name = "colRows";
            this.colRows.Width = 95;
            //
            // colStartedBy
            //
            this.colStartedBy.DataPropertyName = "StartedBy";
            this.colStartedBy.HeaderText = "Started by";
            this.colStartedBy.Name = "colStartedBy";
            this.colStartedBy.Width = 95;
            //
            // colStarted
            //
            this.colStarted.DataPropertyName = "Started";
            this.colStarted.HeaderText = "Started";
            this.colStarted.Name = "colStarted";
            this.colStarted.Width = 80;
            //
            // prgJob
            //
            this.prgJob.Location = new System.Drawing.Point(16, 190);
            this.prgJob.Maximum = 100;
            this.prgJob.Name = "prgJob";
            this.prgJob.Size = new System.Drawing.Size(500, 20);
            //
            // lblPercent
            //
            this.lblPercent.AutoSize = false;
            this.lblPercent.Font = new System.Drawing.Font("monospace", 9F);
            this.lblPercent.Location = new System.Drawing.Point(524, 191);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(60, 20);
            this.lblPercent.Text = "0%";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(590, 191);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(278, 20);
            this.lblStatus.Text = "idle";
            //
            // lblBanner
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 240, 236);
            this.lblBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblBanner.Location = new System.Drawing.Point(16, 216);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Size = new System.Drawing.Size(852, 22);
            this.lblBanner.Text = "";
            this.lblBanner.Visible = false;
            //
            // pnlJobDetail  (the job detail screen — a UserControl, so it can move to its own page unchanged)
            //
            this.pnlJobDetail.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.pnlJobDetail.Location = new System.Drawing.Point(16, 320);
            this.pnlJobDetail.Name = "pnlJobDetail";
            this.pnlJobDetail.Size = new System.Drawing.Size(884, 274);
            //
            // pnlNotifications  (the bell's panel: records with read / unread state)
            //
            this.pnlNotifications.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.pnlNotifications.BackColor = System.Drawing.Color.White;
            this.pnlNotifications.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlNotifications.Controls.Add(this.lblNotificationsTitle);
            this.pnlNotifications.Controls.Add(this.btnMarkRead);
            this.pnlNotifications.Controls.Add(this.lstNotifications);
            this.pnlNotifications.Location = new System.Drawing.Point(916, 64);
            this.pnlNotifications.Name = "pnlNotifications";
            this.pnlNotifications.Size = new System.Drawing.Size(416, 196);
            //
            // lblNotificationsTitle
            //
            this.lblNotificationsTitle.AutoSize = false;
            this.lblNotificationsTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblNotificationsTitle.Location = new System.Drawing.Point(14, 10);
            this.lblNotificationsTitle.Name = "lblNotificationsTitle";
            this.lblNotificationsTitle.Size = new System.Drawing.Size(270, 24);
            this.lblNotificationsTitle.Text = "Notifications";
            //
            // btnMarkRead
            //
            this.btnMarkRead.Location = new System.Drawing.Point(292, 8);
            this.btnMarkRead.Name = "btnMarkRead";
            this.btnMarkRead.Size = new System.Drawing.Size(110, 28);
            this.btnMarkRead.Text = "Mark all read";
            this.btnMarkRead.Click += new System.EventHandler(this.btnMarkRead_Click);
            //
            // lstNotifications
            //
            this.lstNotifications.Font = new System.Drawing.Font("monospace", 9F);
            this.lstNotifications.Location = new System.Drawing.Point(14, 42);
            this.lstNotifications.Name = "lstNotifications";
            this.lstNotifications.Size = new System.Drawing.Size(388, 142);
            //
            // pnlTrace  (Server · live activity trace)
            //
            this.pnlTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.pnlTrace.BackColor = System.Drawing.Color.White;
            this.pnlTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTrace.Controls.Add(this.lblTraceTitle);
            this.pnlTrace.Controls.Add(this.lstTrace);
            this.pnlTrace.Location = new System.Drawing.Point(916, 268);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(416, 326);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Location = new System.Drawing.Point(14, 10);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(388, 24);
            this.lblTraceTitle.Text = "Server · live activity trace";
            //
            // lstTrace
            //
            this.lstTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTrace.Location = new System.Drawing.Point(14, 42);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(388, 272);
            //
            // pnlActions  (success · progress · failure · recovery · clear)
            //
            this.pnlActions.BackColor = System.Drawing.Color.White;
            this.pnlActions.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlActions.Controls.Add(this.cboFile);
            this.pnlActions.Controls.Add(this.btnStartImport);
            this.pnlActions.Controls.Add(this.btnCancelJob);
            this.pnlActions.Controls.Add(this.btnReimport);
            this.pnlActions.Controls.Add(this.btnMalformedFile);
            this.pnlActions.Controls.Add(this.btnAntiPattern);
            this.pnlActions.Controls.Add(this.btnReopenPage);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActions.Location = new System.Drawing.Point(0, 614);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1348, 66);
            //
            // cboFile
            //
            this.cboFile.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboFile.Location = new System.Drawing.Point(16, 18);
            this.cboFile.Name = "cboFile";
            this.cboFile.Size = new System.Drawing.Size(300, 30);
            //
            // btnStartImport
            //
            this.btnStartImport.Location = new System.Drawing.Point(326, 18);
            this.btnStartImport.Name = "btnStartImport";
            this.btnStartImport.Size = new System.Drawing.Size(140, 30);
            this.btnStartImport.Text = "+ New import…";
            this.btnStartImport.Click += new System.EventHandler(this.btnStartImport_Click);
            //
            // btnCancelJob
            //
            this.btnCancelJob.Location = new System.Drawing.Point(474, 18);
            this.btnCancelJob.Name = "btnCancelJob";
            this.btnCancelJob.Size = new System.Drawing.Size(104, 30);
            this.btnCancelJob.Text = "Cancel job";
            this.btnCancelJob.Click += new System.EventHandler(this.btnCancelJob_Click);
            //
            // btnReimport
            //
            this.btnReimport.Location = new System.Drawing.Point(586, 18);
            this.btnReimport.Name = "btnReimport";
            this.btnReimport.Size = new System.Drawing.Size(170, 30);
            this.btnReimport.Text = "Re-import (idempotent)";
            this.btnReimport.Click += new System.EventHandler(this.btnReimport_Click);
            //
            // btnMalformedFile
            //
            this.btnMalformedFile.Location = new System.Drawing.Point(764, 18);
            this.btnMalformedFile.Name = "btnMalformedFile";
            this.btnMalformedFile.Size = new System.Drawing.Size(186, 30);
            this.btnMalformedFile.Text = "Failure: malformed file";
            this.btnMalformedFile.Click += new System.EventHandler(this.btnMalformedFile_Click);
            //
            // btnAntiPattern
            //
            this.btnAntiPattern.Location = new System.Drawing.Point(958, 18);
            this.btnAntiPattern.Name = "btnAntiPattern";
            this.btnAntiPattern.Size = new System.Drawing.Size(200, 30);
            this.btnAntiPattern.Text = "Anti-pattern: push every row";
            this.btnAntiPattern.Click += new System.EventHandler(this.btnAntiPattern_Click);
            //
            // btnReopenPage
            //
            this.btnReopenPage.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnReopenPage.Location = new System.Drawing.Point(1166, 18);
            this.btnReopenPage.Name = "btnReopenPage";
            this.btnReopenPage.Size = new System.Drawing.Size(60, 30);
            this.btnReopenPage.Text = "Reopen";
            this.btnReopenPage.Click += new System.EventHandler(this.btnReopenPage_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearTrace.Location = new System.Drawing.Point(1234, 18);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(98, 30);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // ImportCenterPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlQueue);
            this.Controls.Add(this.pnlJobDetail);
            this.Controls.Add(this.pnlNotifications);
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlActions);
            this.Name = "ImportCenterPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "Import Center";
            this.Load += new System.EventHandler(this.ImportCenterPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlQueue.ResumeLayout(false);
            this.pnlNotifications.ResumeLayout(false);
            this.pnlTrace.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblContext;
        private Wisej.Web.Button btnBell;
        private Wisej.Web.Panel pnlQueue;
        private Wisej.Web.Label lblQueueTitle;
        private Wisej.Web.DataGridView dgvJobs;
        private Wisej.Web.DataGridViewTextBoxColumn colNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colDescription;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colProgress;
        private Wisej.Web.DataGridViewTextBoxColumn colRows;
        private Wisej.Web.DataGridViewTextBoxColumn colStartedBy;
        private Wisej.Web.DataGridViewTextBoxColumn colStarted;
        private Wisej.Web.ProgressBar prgJob;
        private Wisej.Web.Label lblPercent;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblBanner;
        private EnterpriseOps.UI.JobDetailPanel pnlJobDetail;
        private Wisej.Web.Panel pnlNotifications;
        private Wisej.Web.Label lblNotificationsTitle;
        private Wisej.Web.Button btnMarkRead;
        private Wisej.Web.ListBox lstNotifications;
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.ComboBox cboFile;
        private Wisej.Web.Button btnStartImport;
        private Wisej.Web.Button btnCancelJob;
        private Wisej.Web.Button btnReimport;
        private Wisej.Web.Button btnMalformedFile;
        private Wisej.Web.Button btnAntiPattern;
        private Wisej.Web.Button btnReopenPage;
        private Wisej.Web.Button btnClearTrace;
    }
}
