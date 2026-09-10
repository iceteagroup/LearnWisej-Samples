namespace EnterpriseOps.UI
{
    partial class WorkOrderEditorPage
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
            this.lblTenantCaption = new Wisej.Web.Label();
            this.cboTenant = new Wisej.Web.ComboBox();
            this.lblUser = new Wisej.Web.Label();
            this.lblCorrelation = new Wisej.Web.Label();
            this.pnlWork = new Wisej.Web.Panel();
            this.lblQueueTitle = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.dgvWorkQueue = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colWorkOrder = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colState = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssigned = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colVersion = new Wisej.Web.DataGridViewTextBoxColumn();
            this.btnOpen = new Wisej.Web.Button();
            this.lblOpenHint = new Wisej.Web.Label();
            this.pnlEditor = new Wisej.Web.Panel();
            this.lblEditCaption = new Wisej.Web.Label();
            this.lblTitleCaption = new Wisej.Web.Label();
            this.txtTitle = new Wisej.Web.TextBox();
            this.lblStatusCaption = new Wisej.Web.Label();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.lblVersionCaption = new Wisej.Web.Label();
            this.txtVersion = new Wisej.Web.TextBox();
            this.lblEditHint = new Wisej.Web.Label();
            this.btnSave = new Wisej.Web.Button();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.lblTraceFooter = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnOtherSession = new Wisej.Web.Button();
            this.btnCrossTenant = new Wisej.Web.Button();
            this.btnSpoofTenant = new Wisej.Web.Button();
            this.btnStaticLeak = new Wisej.Web.Button();
            this.btnAudit = new Wisej.Web.Button();
            this.btnReloadLatest = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlWork.SuspendLayout();
            this.pnlEditor.SuspendLayout();
            this.pnlTrace.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (slim header: screen name · tenant selection · user · correlation id)
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblTenantCaption);
            this.pnlHeader.Controls.Add(this.cboTenant);
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
            this.lblTitle.Size = new System.Drawing.Size(420, 44);
            this.lblTitle.Text = "EnterpriseOps — Work Order Editor";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTenantCaption
            //
            this.lblTenantCaption.AutoSize = false;
            this.lblTenantCaption.Font = new System.Drawing.Font("default", 9F);
            this.lblTenantCaption.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblTenantCaption.Location = new System.Drawing.Point(452, 0);
            this.lblTenantCaption.Name = "lblTenantCaption";
            this.lblTenantCaption.Size = new System.Drawing.Size(60, 44);
            this.lblTenantCaption.Text = "Tenant";
            this.lblTenantCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cboTenant  (only the tenants the verified claims entitle this user to — the list is a convenience,
            //             never the authority: SessionContext.SwitchTenant checks the entitlement again)
            //
            this.cboTenant.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboTenant.Location = new System.Drawing.Point(520, 9);
            this.cboTenant.Name = "cboTenant";
            this.cboTenant.Size = new System.Drawing.Size(230, 26);
            this.cboTenant.ToolTipText = "cboTenant_SelectedIndexChanged → SessionContext.SwitchTenant(id). The dropdown asks; the session decides.";
            this.cboTenant.SelectedIndexChanged += new System.EventHandler(this.cboTenant_SelectedIndexChanged);
            //
            // lblUser
            //
            this.lblUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(790, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(310, 44);
            this.lblUser.Text = "Signed in: ana.ops · Manager";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCorrelation
            //
            this.lblCorrelation.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCorrelation.AutoSize = false;
            this.lblCorrelation.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCorrelation.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblCorrelation.Location = new System.Drawing.Point(1110, 0);
            this.lblCorrelation.Name = "lblCorrelation";
            this.lblCorrelation.Size = new System.Drawing.Size(214, 44);
            this.lblCorrelation.Text = "corr —";
            this.lblCorrelation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlWork  (the work queue for the current tenant + the editor for one work order)
            //
            this.pnlWork.BackColor = System.Drawing.Color.White;
            this.pnlWork.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlWork.Controls.Add(this.lblQueueTitle);
            this.pnlWork.Controls.Add(this.lblStatus);
            this.pnlWork.Controls.Add(this.dgvWorkQueue);
            this.pnlWork.Controls.Add(this.btnOpen);
            this.pnlWork.Controls.Add(this.lblOpenHint);
            this.pnlWork.Controls.Add(this.pnlEditor);
            this.pnlWork.Controls.Add(this.lblBanner);
            this.pnlWork.Controls.Add(this.lblStatusBar);
            this.pnlWork.Location = new System.Drawing.Point(24, 64);
            this.pnlWork.Name = "pnlWork";
            this.pnlWork.Size = new System.Drawing.Size(812, 506);
            //
            // lblQueueTitle
            //
            this.lblQueueTitle.AutoSize = false;
            this.lblQueueTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblQueueTitle.Location = new System.Drawing.Point(16, 10);
            this.lblQueueTitle.Name = "lblQueueTitle";
            this.lblQueueTitle.Size = new System.Drawing.Size(420, 26);
            this.lblQueueTitle.Text = "Work queue";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(440, 12);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(352, 24);
            this.lblStatus.Text = "● ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dgvWorkQueue  (bound to WorkQueueRow — the projection, never the entity; tenant-scoped by the service)
            //
            this.dgvWorkQueue.AllowUserToAddRows = false;
            this.dgvWorkQueue.AllowUserToDeleteRows = false;
            this.dgvWorkQueue.AutoGenerateColumns = false;
            this.dgvWorkQueue.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvWorkQueue.BackColor = System.Drawing.Color.White;
            this.dgvWorkQueue.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colId,
            this.colWorkOrder,
            this.colState,
            this.colAssigned,
            this.colVersion});
            this.dgvWorkQueue.Location = new System.Drawing.Point(20, 44);
            this.dgvWorkQueue.MultiSelect = false;
            this.dgvWorkQueue.Name = "dgvWorkQueue";
            this.dgvWorkQueue.ReadOnly = true;
            this.dgvWorkQueue.RowHeadersVisible = false;
            this.dgvWorkQueue.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWorkQueue.Size = new System.Drawing.Size(772, 148);
            //
            // colId
            //
            this.colId.DataPropertyName = "Id";
            this.colId.FillWeight = 10F;
            this.colId.HeaderText = "Id";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            //
            // colWorkOrder
            //
            this.colWorkOrder.DataPropertyName = "Title";
            this.colWorkOrder.FillWeight = 46F;
            this.colWorkOrder.HeaderText = "Work order";
            this.colWorkOrder.Name = "colWorkOrder";
            this.colWorkOrder.ReadOnly = true;
            //
            // colState
            //
            this.colState.DataPropertyName = "Status";
            this.colState.FillWeight = 16F;
            this.colState.HeaderText = "Status";
            this.colState.Name = "colState";
            this.colState.ReadOnly = true;
            //
            // colAssigned
            //
            this.colAssigned.DataPropertyName = "AssignedTo";
            this.colAssigned.FillWeight = 18F;
            this.colAssigned.HeaderText = "Assigned to";
            this.colAssigned.Name = "colAssigned";
            this.colAssigned.ReadOnly = true;
            //
            // colVersion  (the concurrency token, visible in the queue so a change is obvious)
            //
            this.colVersion.DataPropertyName = "Version";
            this.colVersion.FillWeight = 10F;
            this.colVersion.HeaderText = "Version";
            this.colVersion.Name = "colVersion";
            this.colVersion.ReadOnly = true;
            //
            // btnOpen
            //
            this.btnOpen.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnOpen.Location = new System.Drawing.Point(20, 200);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(196, 34);
            this.btnOpen.Text = "Open selected in editor";
            this.btnOpen.ToolTipText = "btnOpen_Click → await _workOrders.OpenAsync(CurrentContext, id). The TenantGuard runs before the record is read.";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            //
            // lblOpenHint
            //
            this.lblOpenHint.AutoSize = false;
            this.lblOpenHint.Font = new System.Drawing.Font("default", 8F);
            this.lblOpenHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblOpenHint.Location = new System.Drawing.Point(226, 200);
            this.lblOpenHint.Name = "lblOpenHint";
            this.lblOpenHint.Size = new System.Drawing.Size(566, 34);
            this.lblOpenHint.Text = "The version token is read when the record is loaded and checked when it is saved — this tab owns it, not the session.";
            this.lblOpenHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlEditor  (tab-scoped state: one open work order, one token)
            //
            this.pnlEditor.BackColor = System.Drawing.Color.FromArgb(247, 250, 253);
            this.pnlEditor.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlEditor.Controls.Add(this.lblEditCaption);
            this.pnlEditor.Controls.Add(this.lblTitleCaption);
            this.pnlEditor.Controls.Add(this.txtTitle);
            this.pnlEditor.Controls.Add(this.lblStatusCaption);
            this.pnlEditor.Controls.Add(this.cboStatus);
            this.pnlEditor.Controls.Add(this.lblVersionCaption);
            this.pnlEditor.Controls.Add(this.txtVersion);
            this.pnlEditor.Controls.Add(this.lblEditHint);
            this.pnlEditor.Controls.Add(this.btnSave);
            this.pnlEditor.Location = new System.Drawing.Point(20, 242);
            this.pnlEditor.Name = "pnlEditor";
            this.pnlEditor.Size = new System.Drawing.Size(772, 152);
            //
            // lblEditCaption
            //
            this.lblEditCaption.AutoSize = false;
            this.lblEditCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblEditCaption.Location = new System.Drawing.Point(14, 8);
            this.lblEditCaption.Name = "lblEditCaption";
            this.lblEditCaption.Size = new System.Drawing.Size(744, 22);
            this.lblEditCaption.Text = "Editing — nothing open";
            //
            // lblTitleCaption
            //
            this.lblTitleCaption.AutoSize = false;
            this.lblTitleCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblTitleCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblTitleCaption.Location = new System.Drawing.Point(14, 36);
            this.lblTitleCaption.Name = "lblTitleCaption";
            this.lblTitleCaption.Size = new System.Drawing.Size(120, 18);
            this.lblTitleCaption.Text = "TITLE";
            //
            // txtTitle
            //
            this.txtTitle.Location = new System.Drawing.Point(14, 56);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(464, 30);
            //
            // lblStatusCaption
            //
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblStatusCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblStatusCaption.Location = new System.Drawing.Point(492, 36);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(120, 18);
            this.lblStatusCaption.Text = "STATUS";
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(492, 56);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(140, 30);
            //
            // lblVersionCaption
            //
            this.lblVersionCaption.AutoSize = false;
            this.lblVersionCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblVersionCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblVersionCaption.Location = new System.Drawing.Point(646, 36);
            this.lblVersionCaption.Name = "lblVersionCaption";
            this.lblVersionCaption.Size = new System.Drawing.Size(112, 18);
            this.lblVersionCaption.Text = "VERSION TOKEN";
            //
            // txtVersion  (read-only: the UI displays the token and hands it back untouched, it never invents one)
            //
            this.txtVersion.Font = new System.Drawing.Font("monospace", 9F);
            this.txtVersion.Location = new System.Drawing.Point(646, 56);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.ReadOnly = true;
            this.txtVersion.Size = new System.Drawing.Size(112, 30);
            this.txtVersion.Text = "—";
            //
            // lblEditHint
            //
            this.lblEditHint.AutoSize = false;
            this.lblEditHint.Font = new System.Drawing.Font("default", 8F);
            this.lblEditHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblEditHint.Location = new System.Drawing.Point(14, 98);
            this.lblEditHint.Name = "lblEditHint";
            this.lblEditHint.Size = new System.Drawing.Size(500, 42);
            this.lblEditHint.Text = "Save sends the token back with the command; WorkOrderService compares it before anything is written.";
            //
            // btnSave  (success path — and the failure path, once another session has saved first)
            //
            this.btnSave.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(646, 100);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(112, 38);
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "btnSave_Click → await _workOrders.SaveAsync(CurrentContext, command) → ShowSaveResultAsync(result).";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // lblBanner  (what just happened, in the user's words; hidden until something needs saying)
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(20, 402);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(772, 40);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatusBar  (the dark footer: the command, the tenant and the correlation id of the last action)
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(20, 450);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(772, 36);
            this.lblStatusBar.Text = "Loading…";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlTrace  (Server · live activity trace)
            //
            this.pnlTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlTrace.BackColor = System.Drawing.Color.White;
            this.pnlTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTrace.Controls.Add(this.lblTraceTitle);
            this.pnlTrace.Controls.Add(this.lstTrace);
            this.pnlTrace.Controls.Add(this.lblTraceFooter);
            this.pnlTrace.Location = new System.Drawing.Point(852, 64);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(472, 506);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(440, 28);
            this.lblTraceTitle.Text = "Server · live activity trace";
            //
            // lstTrace
            //
            this.lstTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTrace.Location = new System.Drawing.Point(16, 46);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(440, 416);
            //
            // lblTraceFooter
            //
            this.lblTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceFooter.AutoSize = false;
            this.lblTraceFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTraceFooter.Location = new System.Drawing.Point(16, 468);
            this.lblTraceFooter.Name = "lblTraceFooter";
            this.lblTraceFooter.Size = new System.Drawing.Size(440, 28);
            this.lblTraceFooter.Text = "UI → · Session: · Security: · Service: · Data: · Audit: · UI ←";
            //
            // pnlActions  (bottom bar: four failure paths, the audit, the recovery, clear)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.Controls.Add(this.btnOtherSession);
            this.pnlActions.Controls.Add(this.btnCrossTenant);
            this.pnlActions.Controls.Add(this.btnSpoofTenant);
            this.pnlActions.Controls.Add(this.btnStaticLeak);
            this.pnlActions.Controls.Add(this.btnAudit);
            this.pnlActions.Controls.Add(this.btnReloadLatest);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Location = new System.Drawing.Point(24, 584);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1300, 44);
            //
            // btnOtherSession  (failure setup: a second SessionContext saves the same record first)
            //
            this.btnOtherSession.Location = new System.Drawing.Point(0, 4);
            this.btnOtherSession.Name = "btnOtherSession";
            this.btnOtherSession.Size = new System.Drawing.Size(210, 36);
            this.btnOtherSession.Text = "Fail: other session saves";
            this.btnOtherSession.ToolTipText = "ben.tech signs in in a second session, opens the same work order and saves it — the version moves on and your token goes stale.";
            this.btnOtherSession.Click += new System.EventHandler(this.btnOtherSession_Click);
            //
            // btnCrossTenant  (failure path: the TenantGuard rejects a record owned by another customer)
            //
            this.btnCrossTenant.Location = new System.Drawing.Point(216, 4);
            this.btnCrossTenant.Name = "btnCrossTenant";
            this.btnCrossTenant.Size = new System.Drawing.Size(196, 36);
            this.btnCrossTenant.Text = "Fail: cross-tenant read";
            this.btnCrossTenant.ToolTipText = "Opens a work order that belongs to another tenant: TenantGuard.DemandTenant throws before the record is read.";
            this.btnCrossTenant.Click += new System.EventHandler(this.btnCrossTenant_Click);
            //
            // btnSpoofTenant  (failure path: a tenant id the dropdown never offered)
            //
            this.btnSpoofTenant.Location = new System.Drawing.Point(418, 4);
            this.btnSpoofTenant.Name = "btnSpoofTenant";
            this.btnSpoofTenant.Size = new System.Drawing.Size(206, 36);
            this.btnSpoofTenant.Text = "Fail: spoofed tenant value";
            this.btnSpoofTenant.ToolTipText = "Simulates a browser sending a tenant the user is not entitled to. SessionContext.SwitchTenant rejects it; the context never carries it.";
            this.btnSpoofTenant.Click += new System.EventHandler(this.btnSpoofTenant_Click);
            //
            // btnStaticLeak  (failure path: the anti-pattern, demonstrated safely)
            //
            this.btnStaticLeak.Location = new System.Drawing.Point(630, 4);
            this.btnStaticLeak.Name = "btnStaticLeak";
            this.btnStaticLeak.Size = new System.Drawing.Size(186, 36);
            this.btnStaticLeak.Text = "Fail: static-state leak";
            this.btnStaticLeak.ToolTipText = "LegacyCurrentUser holds the current user in a static; a second sign-in overwrites it for everybody.";
            this.btnStaticLeak.Click += new System.EventHandler(this.btnStaticLeak_Click);
            //
            // btnAudit  (the deliverable, run live over this assembly)
            //
            this.btnAudit.Location = new System.Drawing.Point(822, 4);
            this.btnAudit.Name = "btnAudit";
            this.btnAudit.Size = new System.Drawing.Size(196, 36);
            this.btnAudit.Text = "Run static-state audit";
            this.btnAudit.ToolTipText = "Reflects over every static field in the assembly and classifies it: documented, immutable, or a finding.";
            this.btnAudit.Click += new System.EventHandler(this.btnAudit_Click);
            //
            // btnReloadLatest  (the recovery, also reachable from the conflict dialog)
            //
            this.btnReloadLatest.Location = new System.Drawing.Point(1024, 4);
            this.btnReloadLatest.Name = "btnReloadLatest";
            this.btnReloadLatest.Size = new System.Drawing.Size(166, 36);
            this.btnReloadLatest.Text = "Recover: reload latest";
            this.btnReloadLatest.ToolTipText = "Re-opens the work order at its current version — the same path the conflict dialog's \"Reload latest\" runs.";
            this.btnReloadLatest.Click += new System.EventHandler(this.btnReloadLatest_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearTrace.Location = new System.Drawing.Point(1190, 4);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(110, 36);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // WorkOrderEditorPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlWork);
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlActions);
            this.Name = "WorkOrderEditorPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "EnterpriseOps — Work Order Editor";
            this.Load += new System.EventHandler(this.WorkOrderEditorPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlWork.ResumeLayout(false);
            this.pnlEditor.ResumeLayout(false);
            this.pnlTrace.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblTenantCaption;
        private Wisej.Web.ComboBox cboTenant;
        private Wisej.Web.Label lblUser;
        private Wisej.Web.Label lblCorrelation;
        private Wisej.Web.Panel pnlWork;
        private Wisej.Web.Label lblQueueTitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.DataGridView dgvWorkQueue;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colWorkOrder;
        private Wisej.Web.DataGridViewTextBoxColumn colState;
        private Wisej.Web.DataGridViewTextBoxColumn colAssigned;
        private Wisej.Web.DataGridViewTextBoxColumn colVersion;
        private Wisej.Web.Button btnOpen;
        private Wisej.Web.Label lblOpenHint;
        private Wisej.Web.Panel pnlEditor;
        private Wisej.Web.Label lblEditCaption;
        private Wisej.Web.Label lblTitleCaption;
        private Wisej.Web.TextBox txtTitle;
        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.Label lblVersionCaption;
        private Wisej.Web.TextBox txtVersion;
        private Wisej.Web.Label lblEditHint;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatusBar;
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Label lblTraceFooter;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnOtherSession;
        private Wisej.Web.Button btnCrossTenant;
        private Wisej.Web.Button btnSpoofTenant;
        private Wisej.Web.Button btnStaticLeak;
        private Wisej.Web.Button btnAudit;
        private Wisej.Web.Button btnReloadLatest;
        private Wisej.Web.Button btnClearTrace;
    }
}
