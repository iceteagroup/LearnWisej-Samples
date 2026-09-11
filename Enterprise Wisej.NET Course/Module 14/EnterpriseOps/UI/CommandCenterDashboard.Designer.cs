namespace EnterpriseOps.UI
{
    partial class CommandCenterDashboard
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
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblTitle = new Wisej.Web.Label();
            this.btnCapstoneReview = new Wisej.Web.Button();
            this.pnlDashboard = new Wisej.Web.Panel();
            this.btnRefresh = new Wisej.Web.Button();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.btnApprove = new Wisej.Web.Button();
            this.lblUser = new Wisej.Web.Label();
            this.kpiOpen = new EnterpriseOps.Controls.KpiTile();
            this.kpiEscalated = new EnterpriseOps.Controls.KpiTile();
            this.kpiDueToday = new EnterpriseOps.Controls.KpiTile();
            this.kpiOverdue = new EnterpriseOps.Controls.KpiTile();
            this.kpiCompleted = new EnterpriseOps.Controls.KpiTile();
            this.dgvWorkQueue = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colSite = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssignedTo = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colVersion = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlHealth = new Wisej.Web.Panel();
            this.lblHealthTitle = new Wisej.Web.Label();
            this.lblHealthStatus = new Wisej.Web.Label();
            this.btnRunHealth = new Wisej.Web.Button();
            this.btnCancelHealth = new Wisej.Web.Button();
            this.dgvHealth = new Wisej.Web.DataGridView();
            this.colProbe = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colProbeState = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colProbeDetail = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colProbeMs = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlHeader.SuspendLayout();
            this.pnlDashboard.SuspendLayout();
            this.pnlHealth.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.btnCapstoneReview);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(932, 44);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(24, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(420, 44);
            this.lblTitle.Text = "EnterpriseOps — Command Center";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnCapstoneReview
            //
            this.btnCapstoneReview.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnCapstoneReview.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnCapstoneReview.Location = new System.Drawing.Point(756, 6);
            this.btnCapstoneReview.Name = "btnCapstoneReview";
            this.btnCapstoneReview.Size = new System.Drawing.Size(160, 32);
            this.btnCapstoneReview.Text = "Capstone review →";
            this.btnCapstoneReview.Click += new System.EventHandler(this.btnCapstoneReview_Click);
            //
            // pnlDashboard
            //
            this.pnlDashboard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlDashboard.BackColor = System.Drawing.Color.White;
            this.pnlDashboard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlDashboard.Controls.Add(this.btnRefresh);
            this.pnlDashboard.Controls.Add(this.cboStatus);
            this.pnlDashboard.Controls.Add(this.btnApprove);
            this.pnlDashboard.Controls.Add(this.lblUser);
            this.pnlDashboard.Controls.Add(this.kpiOpen);
            this.pnlDashboard.Controls.Add(this.kpiEscalated);
            this.pnlDashboard.Controls.Add(this.kpiDueToday);
            this.pnlDashboard.Controls.Add(this.kpiOverdue);
            this.pnlDashboard.Controls.Add(this.kpiCompleted);
            this.pnlDashboard.Controls.Add(this.dgvWorkQueue);
            this.pnlDashboard.Controls.Add(this.lblBanner);
            this.pnlDashboard.Controls.Add(this.lblStatusBar);
            this.pnlDashboard.Location = new System.Drawing.Point(16, 56);
            this.pnlDashboard.Name = "pnlDashboard";
            this.pnlDashboard.Size = new System.Drawing.Size(900, 406);
            //
            // btnRefresh
            //
            this.btnRefresh.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Location = new System.Drawing.Point(16, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 34);
            this.btnRefresh.Text = "⟳ Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Items.AddRange(new object[] { "Open", "Escalated", "All" });
            this.cboStatus.Location = new System.Drawing.Point(146, 12);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(140, 34);
            this.cboStatus.SelectedIndexChanged += new System.EventHandler(this.cboStatus_SelectedIndexChanged);
            //
            // btnApprove
            //
            this.btnApprove.Enabled = false;
            this.btnApprove.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnApprove.Location = new System.Drawing.Point(296, 12);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(156, 34);
            this.btnApprove.Text = "✓ Approve selected";
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            //
            // lblUser
            //
            this.lblUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.Font = new System.Drawing.Font("default", 10F);
            this.lblUser.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblUser.Location = new System.Drawing.Point(484, 16);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(400, 26);
            this.lblUser.Text = "Signed in: ana.ops · Manager";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // kpiOpen
            //
            this.kpiOpen.Accent = System.Drawing.Color.FromArgb(21, 101, 216);
            this.kpiOpen.Caption = "OPEN";
            this.kpiOpen.Footnote = "New → Escalated";
            this.kpiOpen.Location = new System.Drawing.Point(16, 54);
            this.kpiOpen.Name = "kpiOpen";
            this.kpiOpen.Size = new System.Drawing.Size(166, 80);
            this.kpiOpen.Value = "—";
            //
            // kpiEscalated
            //
            this.kpiEscalated.Accent = System.Drawing.Color.FromArgb(224, 86, 59);
            this.kpiEscalated.Caption = "ESCALATED";
            this.kpiEscalated.Footnote = "needs a manager";
            this.kpiEscalated.Location = new System.Drawing.Point(190, 54);
            this.kpiEscalated.Name = "kpiEscalated";
            this.kpiEscalated.Size = new System.Drawing.Size(166, 80);
            this.kpiEscalated.Value = "—";
            //
            // kpiDueToday
            //
            this.kpiDueToday.Accent = System.Drawing.Color.FromArgb(232, 161, 60);
            this.kpiDueToday.Caption = "DUE TODAY";
            this.kpiDueToday.Footnote = "open, due today";
            this.kpiDueToday.Location = new System.Drawing.Point(364, 54);
            this.kpiDueToday.Name = "kpiDueToday";
            this.kpiDueToday.Size = new System.Drawing.Size(166, 80);
            this.kpiDueToday.Value = "—";
            //
            // kpiOverdue
            //
            this.kpiOverdue.Accent = System.Drawing.Color.FromArgb(192, 57, 43);
            this.kpiOverdue.Caption = "OVERDUE";
            this.kpiOverdue.Footnote = "open, past due";
            this.kpiOverdue.Location = new System.Drawing.Point(538, 54);
            this.kpiOverdue.Name = "kpiOverdue";
            this.kpiOverdue.Size = new System.Drawing.Size(166, 80);
            this.kpiOverdue.Value = "—";
            //
            // kpiCompleted
            //
            this.kpiCompleted.Accent = System.Drawing.Color.FromArgb(31, 157, 87);
            this.kpiCompleted.Caption = "DONE · 7 DAYS";
            this.kpiCompleted.Footnote = "signed off";
            this.kpiCompleted.Location = new System.Drawing.Point(712, 54);
            this.kpiCompleted.Name = "kpiCompleted";
            this.kpiCompleted.Size = new System.Drawing.Size(166, 80);
            this.kpiCompleted.Value = "—";
            //
            // dgvWorkQueue
            //
            this.dgvWorkQueue.AllowUserToAddRows = false;
            this.dgvWorkQueue.AllowUserToDeleteRows = false;
            this.dgvWorkQueue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvWorkQueue.AutoGenerateColumns = false;
            this.dgvWorkQueue.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvWorkQueue.BackColor = System.Drawing.Color.White;
            this.dgvWorkQueue.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colId,
            this.colTitle,
            this.colCustomer,
            this.colSite,
            this.colStatus,
            this.colPriority,
            this.colAssignedTo,
            this.colDue,
            this.colVersion});
            this.dgvWorkQueue.Location = new System.Drawing.Point(16, 142);
            this.dgvWorkQueue.MultiSelect = false;
            this.dgvWorkQueue.Name = "dgvWorkQueue";
            this.dgvWorkQueue.ReadOnly = true;
            this.dgvWorkQueue.RowHeadersVisible = false;
            this.dgvWorkQueue.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWorkQueue.Size = new System.Drawing.Size(868, 180);
            this.dgvWorkQueue.SelectionChanged += new System.EventHandler(this.dgvWorkQueue_SelectionChanged);
            //
            // work-queue columns
            //
            this.colId.DataPropertyName = "Id";
            this.colId.HeaderText = "#";
            this.colId.Name = "colId";
            this.colId.Width = 60;
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.HeaderText = "Work order";
            this.colTitle.Name = "colTitle";
            this.colTitle.Width = 210;
            this.colCustomer.DataPropertyName = "Customer";
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.Name = "colCustomer";
            this.colCustomer.Width = 120;
            this.colSite.DataPropertyName = "Site";
            this.colSite.HeaderText = "Site";
            this.colSite.Name = "colSite";
            this.colSite.Width = 100;
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 90;
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.HeaderText = "Priority";
            this.colPriority.Name = "colPriority";
            this.colPriority.Width = 80;
            this.colAssignedTo.DataPropertyName = "AssignedTo";
            this.colAssignedTo.HeaderText = "Assigned to";
            this.colAssignedTo.Name = "colAssignedTo";
            this.colAssignedTo.Width = 100;
            this.colDue.DataPropertyName = "Due";
            this.colDue.HeaderText = "Due";
            this.colDue.Name = "colDue";
            this.colDue.Width = 90;
            this.colVersion.DataPropertyName = "Version";
            this.colVersion.HeaderText = "v";
            this.colVersion.Name = "colVersion";
            this.colVersion.Width = 50;
            //
            // lblBanner
            //
            this.lblBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 240, 236);
            this.lblBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.lblBanner.Location = new System.Drawing.Point(16, 330);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(868, 30);
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatusBar
            //
            this.lblStatusBar.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(16, 366);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(868, 30);
            this.lblStatusBar.Text = "Loading…";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlHealth
            //
            this.pnlHealth.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHealth.BackColor = System.Drawing.Color.White;
            this.pnlHealth.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlHealth.Controls.Add(this.lblHealthTitle);
            this.pnlHealth.Controls.Add(this.lblHealthStatus);
            this.pnlHealth.Controls.Add(this.btnRunHealth);
            this.pnlHealth.Controls.Add(this.btnCancelHealth);
            this.pnlHealth.Controls.Add(this.dgvHealth);
            this.pnlHealth.Location = new System.Drawing.Point(16, 470);
            this.pnlHealth.Name = "pnlHealth";
            this.pnlHealth.Size = new System.Drawing.Size(900, 156);
            //
            // lblHealthTitle
            //
            this.lblHealthTitle.AutoSize = false;
            this.lblHealthTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblHealthTitle.Location = new System.Drawing.Point(16, 10);
            this.lblHealthTitle.Name = "lblHealthTitle";
            this.lblHealthTitle.Size = new System.Drawing.Size(260, 26);
            this.lblHealthTitle.Text = "Diagnostics · health probes";
            //
            // lblHealthStatus
            //
            this.lblHealthStatus.AutoSize = false;
            this.lblHealthStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblHealthStatus.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblHealthStatus.Location = new System.Drawing.Point(284, 10);
            this.lblHealthStatus.Name = "lblHealthStatus";
            this.lblHealthStatus.Size = new System.Drawing.Size(340, 26);
            this.lblHealthStatus.Text = "● not run";
            //
            // btnRunHealth
            //
            this.btnRunHealth.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnRunHealth.Location = new System.Drawing.Point(632, 6);
            this.btnRunHealth.Name = "btnRunHealth";
            this.btnRunHealth.Size = new System.Drawing.Size(160, 32);
            this.btnRunHealth.Text = "▶ Run health check";
            this.btnRunHealth.Click += new System.EventHandler(this.btnRunHealth_Click);
            //
            // btnCancelHealth
            //
            this.btnCancelHealth.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnCancelHealth.Enabled = false;
            this.btnCancelHealth.Location = new System.Drawing.Point(798, 6);
            this.btnCancelHealth.Name = "btnCancelHealth";
            this.btnCancelHealth.Size = new System.Drawing.Size(86, 32);
            this.btnCancelHealth.Text = "■ Cancel";
            this.btnCancelHealth.Click += new System.EventHandler(this.btnCancelHealth_Click);
            //
            // dgvHealth
            //
            this.dgvHealth.AllowUserToAddRows = false;
            this.dgvHealth.AllowUserToDeleteRows = false;
            this.dgvHealth.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvHealth.AutoGenerateColumns = false;
            this.dgvHealth.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHealth.BackColor = System.Drawing.Color.White;
            this.dgvHealth.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colProbe,
            this.colProbeState,
            this.colProbeDetail,
            this.colProbeMs});
            this.dgvHealth.Location = new System.Drawing.Point(16, 44);
            this.dgvHealth.MultiSelect = false;
            this.dgvHealth.Name = "dgvHealth";
            this.dgvHealth.ReadOnly = true;
            this.dgvHealth.RowHeadersVisible = false;
            this.dgvHealth.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHealth.Size = new System.Drawing.Size(868, 98);
            this.colProbe.DataPropertyName = "Name";
            this.colProbe.HeaderText = "Probe";
            this.colProbe.Name = "colProbe";
            this.colProbe.Width = 160;
            this.colProbeState.DataPropertyName = "State";
            this.colProbeState.HeaderText = "State";
            this.colProbeState.Name = "colProbeState";
            this.colProbeState.Width = 90;
            this.colProbeDetail.DataPropertyName = "Detail";
            this.colProbeDetail.HeaderText = "Evidence";
            this.colProbeDetail.Name = "colProbeDetail";
            this.colProbeDetail.Width = 540;
            this.colProbeMs.DataPropertyName = "DurationMs";
            this.colProbeMs.HeaderText = "ms";
            this.colProbeMs.Name = "colProbeMs";
            this.colProbeMs.Width = 60;
            //
            // CommandCenterDashboard
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlDashboard);
            this.Controls.Add(this.pnlHealth);
            this.Name = "CommandCenterDashboard";
            this.Size = new System.Drawing.Size(932, 642);
            this.Text = "EnterpriseOps — Command Center";
            this.Load += new System.EventHandler(this.CommandCenterDashboard_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlDashboard.ResumeLayout(false);
            this.pnlHealth.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Button btnCapstoneReview;
        private Wisej.Web.Panel pnlDashboard;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.Button btnApprove;
        private Wisej.Web.Label lblUser;
        private EnterpriseOps.Controls.KpiTile kpiOpen;
        private EnterpriseOps.Controls.KpiTile kpiEscalated;
        private EnterpriseOps.Controls.KpiTile kpiDueToday;
        private EnterpriseOps.Controls.KpiTile kpiOverdue;
        private EnterpriseOps.Controls.KpiTile kpiCompleted;
        private Wisej.Web.DataGridView dgvWorkQueue;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colSite;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colAssignedTo;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.DataGridViewTextBoxColumn colVersion;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatusBar;
        private Wisej.Web.Panel pnlHealth;
        private Wisej.Web.Label lblHealthTitle;
        private Wisej.Web.Label lblHealthStatus;
        private Wisej.Web.Button btnRunHealth;
        private Wisej.Web.Button btnCancelHealth;
        private Wisej.Web.DataGridView dgvHealth;
        private Wisej.Web.DataGridViewTextBoxColumn colProbe;
        private Wisej.Web.DataGridViewTextBoxColumn colProbeState;
        private Wisej.Web.DataGridViewTextBoxColumn colProbeDetail;
        private Wisej.Web.DataGridViewTextBoxColumn colProbeMs;
    }
}
