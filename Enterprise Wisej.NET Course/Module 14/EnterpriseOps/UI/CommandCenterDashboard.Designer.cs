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
            if (disposing)
            {
                DetachTrace();                  // the session's trace outlives this screen — see the code-behind
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
            this.lblTenant = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.lblCorrelation = new Wisej.Web.Label();
            this.pnlDashboard = new Wisej.Web.Panel();
            this.btnRefresh = new Wisej.Web.Button();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.lblStatus = new Wisej.Web.Label();
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
            this.dgvHealth = new Wisej.Web.DataGridView();
            this.colProbe = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colProbeState = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colProbeDetail = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colProbeMs = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.lblTraceFooter = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnApprove = new Wisej.Web.Button();
            this.btnRunHealth = new Wisej.Web.Button();
            this.btnCancelHealth = new Wisej.Web.Button();
            this.btnStaleVersion = new Wisej.Web.Button();
            this.btnSwitchUser = new Wisej.Web.Button();
            this.btnReload = new Wisej.Web.Button();
            this.btnCapstoneReview = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlDashboard.SuspendLayout();
            this.pnlHealth.SuspendLayout();
            this.pnlTrace.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (slim header bar: screen name · tenant · user · correlation id)
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblTenant);
            this.pnlHeader.Controls.Add(this.lblUser);
            this.pnlHeader.Controls.Add(this.lblCorrelation);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1348, 44);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(24, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(560, 44);
            this.lblTitle.Text = "EnterpriseOps — Command Center · capstone";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTenant
            //
            this.lblTenant.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblTenant.AutoSize = false;
            this.lblTenant.Font = new System.Drawing.Font("default", 9F);
            this.lblTenant.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblTenant.Location = new System.Drawing.Point(700, 0);
            this.lblTenant.Name = "lblTenant";
            this.lblTenant.Size = new System.Drawing.Size(180, 44);
            this.lblTenant.Text = "tenant: contoso";
            this.lblTenant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUser
            //
            this.lblUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(886, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(270, 44);
            this.lblUser.Text = "Signed in: ana.ops · Manager";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCorrelation
            //
            this.lblCorrelation.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCorrelation.AutoSize = false;
            this.lblCorrelation.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCorrelation.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblCorrelation.Location = new System.Drawing.Point(1162, 0);
            this.lblCorrelation.Name = "lblCorrelation";
            this.lblCorrelation.Size = new System.Drawing.Size(162, 44);
            this.lblCorrelation.Text = "corr —";
            this.lblCorrelation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlDashboard  (KPI cards from a service + the tenant-scoped work queue)
            //
            this.pnlDashboard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.pnlDashboard.BackColor = System.Drawing.Color.White;
            this.pnlDashboard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlDashboard.Controls.Add(this.btnRefresh);
            this.pnlDashboard.Controls.Add(this.cboStatus);
            this.pnlDashboard.Controls.Add(this.lblStatus);
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
            this.btnRefresh.ToolTipText = "btnRefresh_Click → _dashboard.GetKpisAsync + _workOrders.GetQueueAsync. Two service calls, no logic here.";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // cboStatus  (the queue filter — the service turns it into a where clause)
            //
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Items.AddRange(new object[] { "Open", "Escalated", "All" });
            this.cboStatus.Location = new System.Drawing.Point(146, 12);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(140, 34);
            this.cboStatus.ToolTipText = "WorkQueueQuery.StatusFilter — \"open\" is defined once, in DashboardService.OpenStatuses.";
            this.cboStatus.SelectedIndexChanged += new System.EventHandler(this.cboStatus_SelectedIndexChanged);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(296, 16);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(588, 26);
            this.lblStatus.Text = "● ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // the five KPI tiles — every number is computed by DashboardService, never here
            //
            this.kpiOpen.Accent = System.Drawing.Color.FromArgb(21, 101, 216);
            this.kpiOpen.Caption = "OPEN";
            this.kpiOpen.Footnote = "New → Escalated";
            this.kpiOpen.Location = new System.Drawing.Point(16, 54);
            this.kpiOpen.Name = "kpiOpen";
            this.kpiOpen.Size = new System.Drawing.Size(166, 80);
            this.kpiOpen.Value = "—";
            this.kpiEscalated.Accent = System.Drawing.Color.FromArgb(224, 86, 59);
            this.kpiEscalated.Caption = "ESCALATED";
            this.kpiEscalated.Footnote = "needs a manager";
            this.kpiEscalated.Location = new System.Drawing.Point(190, 54);
            this.kpiEscalated.Name = "kpiEscalated";
            this.kpiEscalated.Size = new System.Drawing.Size(166, 80);
            this.kpiEscalated.Value = "—";
            this.kpiDueToday.Accent = System.Drawing.Color.FromArgb(232, 161, 60);
            this.kpiDueToday.Caption = "DUE TODAY";
            this.kpiDueToday.Footnote = "open, due today";
            this.kpiDueToday.Location = new System.Drawing.Point(364, 54);
            this.kpiDueToday.Name = "kpiDueToday";
            this.kpiDueToday.Size = new System.Drawing.Size(166, 80);
            this.kpiDueToday.Value = "—";
            this.kpiOverdue.Accent = System.Drawing.Color.FromArgb(192, 57, 43);
            this.kpiOverdue.Caption = "OVERDUE";
            this.kpiOverdue.Footnote = "open, past due";
            this.kpiOverdue.Location = new System.Drawing.Point(538, 54);
            this.kpiOverdue.Name = "kpiOverdue";
            this.kpiOverdue.Size = new System.Drawing.Size(166, 80);
            this.kpiOverdue.Value = "—";
            this.kpiCompleted.Accent = System.Drawing.Color.FromArgb(31, 157, 87);
            this.kpiCompleted.Caption = "DONE · 7 DAYS";
            this.kpiCompleted.Footnote = "signed off";
            this.kpiCompleted.Location = new System.Drawing.Point(712, 54);
            this.kpiCompleted.Name = "kpiCompleted";
            this.kpiCompleted.Size = new System.Drawing.Size(166, 80);
            this.kpiCompleted.Value = "—";
            //
            // dgvWorkQueue  (bound to WorkQueueRow — the projection, never the entity)
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
            // the work-queue columns
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
            this.colVersion.ToolTipText = "The optimistic-concurrency token the approve command has to echo back.";
            this.colVersion.Width = 50;
            //
            // lblBanner  (the failure banner: refusals and stale versions, never a stack trace)
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
            // lblStatusBar  (the dark footer: which service answered, how many rows, how long)
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
            // pnlHealth  (the diagnostics / health card — six probes, run as a progress path)
            //
            this.pnlHealth.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.pnlHealth.BackColor = System.Drawing.Color.White;
            this.pnlHealth.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlHealth.Controls.Add(this.lblHealthTitle);
            this.pnlHealth.Controls.Add(this.lblHealthStatus);
            this.pnlHealth.Controls.Add(this.dgvHealth);
            this.pnlHealth.Location = new System.Drawing.Point(16, 470);
            this.pnlHealth.Name = "pnlHealth";
            this.pnlHealth.Size = new System.Drawing.Size(900, 146);
            //
            // lblHealthTitle
            //
            this.lblHealthTitle.AutoSize = false;
            this.lblHealthTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblHealthTitle.Location = new System.Drawing.Point(16, 8);
            this.lblHealthTitle.Name = "lblHealthTitle";
            this.lblHealthTitle.Size = new System.Drawing.Size(420, 24);
            this.lblHealthTitle.Text = "Diagnostics · health probes";
            //
            // lblHealthStatus
            //
            this.lblHealthStatus.AutoSize = false;
            this.lblHealthStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblHealthStatus.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblHealthStatus.Location = new System.Drawing.Point(440, 8);
            this.lblHealthStatus.Name = "lblHealthStatus";
            this.lblHealthStatus.Size = new System.Drawing.Size(444, 24);
            this.lblHealthStatus.Text = "● not run";
            this.lblHealthStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dgvHealth
            //
            this.dgvHealth.AllowUserToAddRows = false;
            this.dgvHealth.AllowUserToDeleteRows = false;
            this.dgvHealth.AutoGenerateColumns = false;
            this.dgvHealth.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHealth.BackColor = System.Drawing.Color.White;
            this.dgvHealth.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colProbe,
            this.colProbeState,
            this.colProbeDetail,
            this.colProbeMs});
            this.dgvHealth.Location = new System.Drawing.Point(16, 38);
            this.dgvHealth.MultiSelect = false;
            this.dgvHealth.Name = "dgvHealth";
            this.dgvHealth.ReadOnly = true;
            this.dgvHealth.RowHeadersVisible = false;
            this.dgvHealth.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHealth.Size = new System.Drawing.Size(868, 94);
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
            // pnlTrace  (Server · live activity trace — one line per layer decision)
            //
            this.pnlTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlTrace.BackColor = System.Drawing.Color.White;
            this.pnlTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTrace.Controls.Add(this.lblTraceTitle);
            this.pnlTrace.Controls.Add(this.lstTrace);
            this.pnlTrace.Controls.Add(this.lblTraceFooter);
            this.pnlTrace.Location = new System.Drawing.Point(932, 56);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(400, 560);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Location = new System.Drawing.Point(14, 10);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(368, 26);
            this.lblTraceTitle.Text = "Server · live activity trace";
            //
            // lstTrace
            //
            this.lstTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTrace.Location = new System.Drawing.Point(14, 42);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(368, 482);
            //
            // lblTraceFooter
            //
            this.lblTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceFooter.AutoSize = false;
            this.lblTraceFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTraceFooter.Location = new System.Drawing.Point(14, 528);
            this.lblTraceFooter.Name = "lblTraceFooter";
            this.lblTraceFooter.Size = new System.Drawing.Size(368, 24);
            this.lblTraceFooter.Text = "UI → · Security: · Service: · Data: · Job: · Review: · Docs:";
            //
            // pnlActions  (success · progress · failure · recovery · navigation · clear)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.Controls.Add(this.btnApprove);
            this.pnlActions.Controls.Add(this.btnRunHealth);
            this.pnlActions.Controls.Add(this.btnCancelHealth);
            this.pnlActions.Controls.Add(this.btnStaleVersion);
            this.pnlActions.Controls.Add(this.btnSwitchUser);
            this.pnlActions.Controls.Add(this.btnReload);
            this.pnlActions.Controls.Add(this.btnCapstoneReview);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Location = new System.Drawing.Point(16, 624);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1316, 44);
            //
            // btnApprove  (the success path: the approve command, permission checked, audited)
            //
            this.btnApprove.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnApprove.Location = new System.Drawing.Point(0, 4);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(156, 36);
            this.btnApprove.Text = "✓ Approve selected";
            this.btnApprove.ToolTipText = "ApproveWorkOrderCommand → WorkOrderService.ApproveAsync: permission, tenant, version, audit line.";
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            //
            // btnRunHealth  (the progress path: six probes, one at a time, cancellable)
            //
            this.btnRunHealth.Location = new System.Drawing.Point(162, 4);
            this.btnRunHealth.Name = "btnRunHealth";
            this.btnRunHealth.Size = new System.Drawing.Size(156, 36);
            this.btnRunHealth.Text = "▶ Run health check";
            this.btnRunHealth.ToolTipText = "DiagnosticsService.RunAsync — each probe reports its evidence as it finishes.";
            this.btnRunHealth.Click += new System.EventHandler(this.btnRunHealth_Click);
            //
            // btnCancelHealth
            //
            this.btnCancelHealth.Enabled = false;
            this.btnCancelHealth.Location = new System.Drawing.Point(324, 4);
            this.btnCancelHealth.Name = "btnCancelHealth";
            this.btnCancelHealth.Size = new System.Drawing.Size(96, 36);
            this.btnCancelHealth.Text = "■ Cancel";
            this.btnCancelHealth.ToolTipText = "The CancellationTokenSource lives in an instance field; the run stops at the next probe.";
            this.btnCancelHealth.Click += new System.EventHandler(this.btnCancelHealth_Click);
            //
            // btnStaleVersion  (failure path 1: somebody else saved the row first)
            //
            this.btnStaleVersion.Location = new System.Drawing.Point(426, 4);
            this.btnStaleVersion.Name = "btnStaleVersion";
            this.btnStaleVersion.Size = new System.Drawing.Size(186, 36);
            this.btnStaleVersion.Text = "Fail: stale version";
            this.btnStaleVersion.ToolTipText = "Bumps the selected row's version behind the screen's back, then approves: the store rejects the write.";
            this.btnStaleVersion.Click += new System.EventHandler(this.btnStaleVersion_Click);
            //
            // btnSwitchUser  (failure path 2 / recovery: permission denied, then back)
            //
            this.btnSwitchUser.Location = new System.Drawing.Point(618, 4);
            this.btnSwitchUser.Name = "btnSwitchUser";
            this.btnSwitchUser.Size = new System.Drawing.Size(206, 36);
            this.btnSwitchUser.Text = "Switch to ben.tech";
            this.btnSwitchUser.ToolTipText = "A Technician may view the queue and nothing else. Approve is refused in the service and the denial is audited.";
            this.btnSwitchUser.Click += new System.EventHandler(this.btnSwitchUser_Click);
            //
            // btnReload  (the recovery: read the queue again and the version is current)
            //
            this.btnReload.Location = new System.Drawing.Point(830, 4);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(168, 36);
            this.btnReload.Text = "Recover: reload queue";
            this.btnReload.ToolTipText = "The same refresh path — after it, the approve carries the version the store has.";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            //
            // btnCapstoneReview  (to the second screen)
            //
            this.btnCapstoneReview.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnCapstoneReview.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnCapstoneReview.Location = new System.Drawing.Point(1004, 4);
            this.btnCapstoneReview.Name = "btnCapstoneReview";
            this.btnCapstoneReview.Size = new System.Drawing.Size(190, 36);
            this.btnCapstoneReview.Text = "Capstone review →";
            this.btnCapstoneReview.ToolTipText = "The delivery screen: prompt library, review checklist, documentation index, package self-check.";
            this.btnCapstoneReview.Click += new System.EventHandler(this.btnCapstoneReview_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearTrace.Location = new System.Drawing.Point(1200, 4);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(116, 36);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // CommandCenterDashboard
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlDashboard);
            this.Controls.Add(this.pnlHealth);
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlActions);
            this.Name = "CommandCenterDashboard";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "EnterpriseOps — Command Center";
            this.Load += new System.EventHandler(this.CommandCenterDashboard_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlDashboard.ResumeLayout(false);
            this.pnlHealth.ResumeLayout(false);
            this.pnlTrace.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblTenant;
        private Wisej.Web.Label lblUser;
        private Wisej.Web.Label lblCorrelation;
        private Wisej.Web.Panel pnlDashboard;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.Label lblStatus;
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
        private Wisej.Web.DataGridView dgvHealth;
        private Wisej.Web.DataGridViewTextBoxColumn colProbe;
        private Wisej.Web.DataGridViewTextBoxColumn colProbeState;
        private Wisej.Web.DataGridViewTextBoxColumn colProbeDetail;
        private Wisej.Web.DataGridViewTextBoxColumn colProbeMs;
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Label lblTraceFooter;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnApprove;
        private Wisej.Web.Button btnRunHealth;
        private Wisej.Web.Button btnCancelHealth;
        private Wisej.Web.Button btnStaleVersion;
        private Wisej.Web.Button btnSwitchUser;
        private Wisej.Web.Button btnReload;
        private Wisej.Web.Button btnCapstoneReview;
        private Wisej.Web.Button btnClearTrace;
    }
}
