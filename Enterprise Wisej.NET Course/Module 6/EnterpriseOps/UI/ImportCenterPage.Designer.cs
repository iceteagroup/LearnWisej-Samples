namespace EnterpriseOps.UI
{
    partial class ImportCenterPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used. The page also stops its progress observer here:
        /// the page goes away, the job does not.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopObserver();
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
            this.btnBell = new Wisej.Web.Button();
            this.cboFile = new Wisej.Web.ComboBox();
            this.btnStartImport = new Wisej.Web.Button();
            this.btnCancelJob = new Wisej.Web.Button();
            this.pnlQueue = new Wisej.Web.Panel();
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
            this.lblBanner = new Wisej.Web.Label();
            this.pnlJobDetail = new EnterpriseOps.UI.JobDetailPanel();
            this.pnlNotifications = new Wisej.Web.Panel();
            this.lblNotificationsTitle = new Wisej.Web.Label();
            this.btnMarkRead = new Wisej.Web.Button();
            this.lstNotifications = new Wisej.Web.ListBox();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlQueue.SuspendLayout();
            this.pnlNotifications.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.btnBell);
            this.pnlHeader.Controls.Add(this.cboFile);
            this.pnlHeader.Controls.Add(this.btnStartImport);
            this.pnlHeader.Controls.Add(this.btnCancelJob);
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
            // btnBell
            //
            this.btnBell.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnBell.Location = new System.Drawing.Point(704, 11);
            this.btnBell.Name = "btnBell";
            this.btnBell.Size = new System.Drawing.Size(60, 30);
            this.btnBell.Text = "🔔";
            this.btnBell.Click += new System.EventHandler(this.btnBell_Click);
            //
            // cboFile
            //
            this.cboFile.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.cboFile.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboFile.Location = new System.Drawing.Point(772, 11);
            this.cboFile.Name = "cboFile";
            this.cboFile.Size = new System.Drawing.Size(300, 30);
            //
            // btnStartImport
            //
            this.btnStartImport.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnStartImport.Location = new System.Drawing.Point(1080, 11);
            this.btnStartImport.Name = "btnStartImport";
            this.btnStartImport.Size = new System.Drawing.Size(140, 30);
            this.btnStartImport.Text = "+ New import…";
            this.btnStartImport.Click += new System.EventHandler(this.btnStartImport_Click);
            //
            // btnCancelJob
            //
            this.btnCancelJob.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnCancelJob.Location = new System.Drawing.Point(1228, 11);
            this.btnCancelJob.Name = "btnCancelJob";
            this.btnCancelJob.Size = new System.Drawing.Size(104, 30);
            this.btnCancelJob.Text = "Cancel job";
            this.btnCancelJob.Click += new System.EventHandler(this.btnCancelJob_Click);
            //
            // pnlQueue
            //
            this.pnlQueue.BackColor = System.Drawing.Color.White;
            this.pnlQueue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlQueue.Controls.Add(this.dgvJobs);
            this.pnlQueue.Controls.Add(this.prgJob);
            this.pnlQueue.Controls.Add(this.lblPercent);
            this.pnlQueue.Controls.Add(this.lblBanner);
            this.pnlQueue.Location = new System.Drawing.Point(16, 64);
            this.pnlQueue.Name = "pnlQueue";
            this.pnlQueue.Size = new System.Drawing.Size(884, 226);
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
            this.dgvJobs.Location = new System.Drawing.Point(16, 16);
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
            this.prgJob.Location = new System.Drawing.Point(16, 168);
            this.prgJob.Maximum = 100;
            this.prgJob.Name = "prgJob";
            this.prgJob.Size = new System.Drawing.Size(780, 20);
            //
            // lblPercent
            //
            this.lblPercent.AutoSize = false;
            this.lblPercent.Font = new System.Drawing.Font("monospace", 9F);
            this.lblPercent.Location = new System.Drawing.Point(804, 169);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(64, 20);
            this.lblPercent.Text = "0%";
            //
            // lblBanner
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 240, 236);
            this.lblBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblBanner.Location = new System.Drawing.Point(16, 194);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Size = new System.Drawing.Size(852, 22);
            this.lblBanner.Text = "";
            this.lblBanner.Visible = false;
            //
            // pnlJobDetail
            //
            this.pnlJobDetail.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.pnlJobDetail.Location = new System.Drawing.Point(16, 298);
            this.pnlJobDetail.Name = "pnlJobDetail";
            this.pnlJobDetail.Size = new System.Drawing.Size(884, 274);
            //
            // pnlNotifications
            //
            this.pnlNotifications.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.pnlNotifications.BackColor = System.Drawing.Color.White;
            this.pnlNotifications.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlNotifications.Controls.Add(this.lblNotificationsTitle);
            this.pnlNotifications.Controls.Add(this.btnMarkRead);
            this.pnlNotifications.Controls.Add(this.lstNotifications);
            this.pnlNotifications.Location = new System.Drawing.Point(916, 64);
            this.pnlNotifications.Name = "pnlNotifications";
            this.pnlNotifications.Size = new System.Drawing.Size(416, 508);
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
            this.btnMarkRead.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnMarkRead.Location = new System.Drawing.Point(292, 8);
            this.btnMarkRead.Name = "btnMarkRead";
            this.btnMarkRead.Size = new System.Drawing.Size(110, 28);
            this.btnMarkRead.Text = "Mark all read";
            this.btnMarkRead.Click += new System.EventHandler(this.btnMarkRead_Click);
            //
            // lstNotifications
            //
            this.lstNotifications.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstNotifications.Font = new System.Drawing.Font("monospace", 9F);
            this.lstNotifications.Location = new System.Drawing.Point(14, 42);
            this.lstNotifications.Name = "lstNotifications";
            this.lstNotifications.Size = new System.Drawing.Size(388, 452);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("monospace", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatus.Location = new System.Drawing.Point(0, 588);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.lblStatus.Size = new System.Drawing.Size(1348, 28);
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ImportCenterPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlQueue);
            this.Controls.Add(this.pnlJobDetail);
            this.Controls.Add(this.pnlNotifications);
            this.Controls.Add(this.lblStatus);
            this.Name = "ImportCenterPage";
            this.Size = new System.Drawing.Size(1348, 616);
            this.Text = "Import Center";
            this.Load += new System.EventHandler(this.ImportCenterPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlQueue.ResumeLayout(false);
            this.pnlNotifications.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Button btnBell;
        private Wisej.Web.ComboBox cboFile;
        private Wisej.Web.Button btnStartImport;
        private Wisej.Web.Button btnCancelJob;
        private Wisej.Web.Panel pnlQueue;
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
        private Wisej.Web.Label lblBanner;
        private EnterpriseOps.UI.JobDetailPanel pnlJobDetail;
        private Wisej.Web.Panel pnlNotifications;
        private Wisej.Web.Label lblNotificationsTitle;
        private Wisej.Web.Button btnMarkRead;
        private Wisej.Web.ListBox lstNotifications;
        private Wisej.Web.Label lblStatus;
    }
}
