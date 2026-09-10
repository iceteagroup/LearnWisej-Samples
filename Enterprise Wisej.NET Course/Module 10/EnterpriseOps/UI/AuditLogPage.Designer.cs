namespace EnterpriseOps.UI
{
    partial class AuditLogPage
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
            this.lblTenant = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.lblCorrelation = new Wisej.Web.Label();
            this.pnlWork = new Wisej.Web.Panel();
            this.lblWorkTitle = new Wisej.Web.Label();
            this.btnApprove = new Wisej.Web.Button();
            this.btnApproveExport = new Wisej.Web.Button();
            this.btnExport = new Wisej.Web.Button();
            this.dgvWorkOrders = new Wisej.Web.DataGridView();
            this.colRef = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colApprovedBy = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblStatus = new Wisej.Web.Label();
            this.lblBanner = new Wisej.Web.Label();
            this.pnlAudit = new Wisej.Web.Panel();
            this.lblAuditTitle = new Wisej.Web.Label();
            this.lblAuditScope = new Wisej.Web.Label();
            this.cboUser = new Wisej.Web.ComboBox();
            this.cboPermission = new Wisej.Web.ComboBox();
            this.cboResult = new Wisej.Web.ComboBox();
            this.lblAuditCount = new Wisej.Web.Label();
            this.dgvAudit = new Wisej.Web.DataGridView();
            this.colTime = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colUser = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTenant = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPermission = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colResult = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDetail = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.lblTraceFooter = new Wisej.Web.Label();
            this.pnlNote = new Wisej.Web.Panel();
            this.lblNoteTitle = new Wisej.Web.Label();
            this.lblNoteMode = new Wisej.Web.Label();
            this.lblNoteSource = new Wisej.Web.Label();
            this.pnlNoteRender = new Wisej.Web.Panel();
            this.lblNote = new Wisej.Web.Label();
            this.lblNoteWarning = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnSwitchIdentity = new Wisej.Web.Button();
            this.btnBreakUi = new Wisej.Web.Button();
            this.btnCrossTenant = new Wisej.Web.Button();
            this.btnUnsafeHtml = new Wisej.Web.Button();
            this.btnSafeHtml = new Wisej.Web.Button();
            this.btnHtmlReview = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlWork.SuspendLayout();
            this.pnlAudit.SuspendLayout();
            this.pnlTrace.SuspendLayout();
            this.pnlNote.SuspendLayout();
            this.pnlNoteRender.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (slim header bar: screen name · tenant · signed-in user · correlation id)
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
            this.lblTitle.Size = new System.Drawing.Size(420, 44);
            this.lblTitle.Text = "EnterpriseOps — Work queue + audit";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTenant  (text comes from the 'tid' claim — an AllowHtml review item)
            //
            this.lblTenant.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblTenant.AutoSize = false;
            this.lblTenant.Font = new System.Drawing.Font("default", 9F);
            this.lblTenant.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblTenant.Location = new System.Drawing.Point(700, 0);
            this.lblTenant.Name = "lblTenant";
            this.lblTenant.Size = new System.Drawing.Size(160, 44);
            this.lblTenant.Text = "tenant: —";
            this.lblTenant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUser  (text comes from the 'name' claim — an AllowHtml review item)
            //
            this.lblUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(866, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(290, 44);
            this.lblUser.Text = "not signed in";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCorrelation
            //
            this.lblCorrelation.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCorrelation.AutoSize = false;
            this.lblCorrelation.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCorrelation.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblCorrelation.Location = new System.Drawing.Point(1160, 0);
            this.lblCorrelation.Name = "lblCorrelation";
            this.lblCorrelation.Size = new System.Drawing.Size(164, 44);
            this.lblCorrelation.Text = "corr —";
            this.lblCorrelation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlWork  (the work queue and the three sensitive commands)
            //
            this.pnlWork.BackColor = System.Drawing.Color.White;
            this.pnlWork.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlWork.Controls.Add(this.lblWorkTitle);
            this.pnlWork.Controls.Add(this.btnApprove);
            this.pnlWork.Controls.Add(this.btnApproveExport);
            this.pnlWork.Controls.Add(this.btnExport);
            this.pnlWork.Controls.Add(this.dgvWorkOrders);
            this.pnlWork.Controls.Add(this.lblStatus);
            this.pnlWork.Controls.Add(this.lblBanner);
            this.pnlWork.Location = new System.Drawing.Point(24, 64);
            this.pnlWork.Name = "pnlWork";
            this.pnlWork.Size = new System.Drawing.Size(812, 226);
            //
            // lblWorkTitle
            //
            this.lblWorkTitle.AutoSize = false;
            this.lblWorkTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblWorkTitle.Location = new System.Drawing.Point(16, 12);
            this.lblWorkTitle.Name = "lblWorkTitle";
            this.lblWorkTitle.Size = new System.Drawing.Size(300, 26);
            this.lblWorkTitle.Text = "Work queue";
            //
            // btnApprove  (success path: the service demands ApproveWorkOrders on the record's tenant)
            //
            this.btnApprove.Location = new System.Drawing.Point(322, 10);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(146, 34);
            this.btnApprove.Text = "✔ Approve";
            this.btnApprove.ToolTipText = "btnApprove_Click → await _workOrders.ApproveAsync(ctx, id) → Demand(ApproveWorkOrders, record tenant).";
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            //
            // btnApproveExport  (dual control: releases a pending export, never one you requested yourself)
            //
            this.btnApproveExport.Location = new System.Drawing.Point(476, 10);
            this.btnApproveExport.Name = "btnApproveExport";
            this.btnApproveExport.Size = new System.Drawing.Size(186, 34);
            this.btnApproveExport.Text = "Approve pending export";
            this.btnApproveExport.ToolTipText = "btnApproveExport_Click → await _exports.ApproveExportAsync(ctx, id) → Demand(ApproveExport) + separation of duties.";
            this.btnApproveExport.Click += new System.EventHandler(this.btnApproveExport_Click);
            //
            // btnExport  (the walkthrough's button: hidden unless Has(ExportData) — and the failure path re-enables it)
            //
            this.btnExport.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnExport.Location = new System.Drawing.Point(670, 10);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(126, 34);
            this.btnExport.Text = "⤓ Export data";
            this.btnExport.ToolTipText = "btnExport_Click → await _exports.RequestExportAsync(ctx) → Demand(ExportData) inside the service.";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            //
            // dgvWorkOrders  (bound to WorkQueueRow, the projection — never the entity)
            //
            this.dgvWorkOrders.AllowUserToAddRows = false;
            this.dgvWorkOrders.AllowUserToDeleteRows = false;
            this.dgvWorkOrders.AutoGenerateColumns = false;
            this.dgvWorkOrders.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvWorkOrders.BackColor = System.Drawing.Color.White;
            this.dgvWorkOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colRef,
            this.colTitle,
            this.colStatus,
            this.colPriority,
            this.colApprovedBy});
            this.dgvWorkOrders.Location = new System.Drawing.Point(16, 54);
            this.dgvWorkOrders.MultiSelect = false;
            this.dgvWorkOrders.Name = "dgvWorkOrders";
            this.dgvWorkOrders.ReadOnly = true;
            this.dgvWorkOrders.RowHeadersVisible = false;
            this.dgvWorkOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWorkOrders.Size = new System.Drawing.Size(780, 92);
            //
            // colRef
            //
            this.colRef.DataPropertyName = "Reference";
            this.colRef.FillWeight = 12F;
            this.colRef.HeaderText = "Ref";
            this.colRef.Name = "colRef";
            this.colRef.ReadOnly = true;
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.FillWeight = 44F;
            this.colTitle.HeaderText = "Work order";
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.FillWeight = 15F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            //
            // colPriority
            //
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.FillWeight = 14F;
            this.colPriority.HeaderText = "Priority";
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            //
            // colApprovedBy
            //
            this.colApprovedBy.DataPropertyName = "ApprovedBy";
            this.colApprovedBy.FillWeight = 15F;
            this.colApprovedBy.HeaderText = "Approved by";
            this.colApprovedBy.Name = "colApprovedBy";
            this.colApprovedBy.ReadOnly = true;
            //
            // lblStatus  (green ok · amber warning · red error — the one-line state of the last command)
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(16, 150);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(780, 22);
            this.lblStatus.Text = "● waiting for the sign-in gate";
            //
            // lblBanner  (the denied / failed banner; hidden until a service has something to say)
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(16, 176);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(780, 42);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // pnlAudit  (the lab deliverable: the audit log screen)
            //
            this.pnlAudit.BackColor = System.Drawing.Color.White;
            this.pnlAudit.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlAudit.Controls.Add(this.lblAuditTitle);
            this.pnlAudit.Controls.Add(this.lblAuditScope);
            this.pnlAudit.Controls.Add(this.cboUser);
            this.pnlAudit.Controls.Add(this.cboPermission);
            this.pnlAudit.Controls.Add(this.cboResult);
            this.pnlAudit.Controls.Add(this.lblAuditCount);
            this.pnlAudit.Controls.Add(this.dgvAudit);
            this.pnlAudit.Controls.Add(this.lblStatusBar);
            this.pnlAudit.Location = new System.Drawing.Point(24, 298);
            this.pnlAudit.Name = "pnlAudit";
            this.pnlAudit.Size = new System.Drawing.Size(812, 272);
            //
            // lblAuditTitle
            //
            this.lblAuditTitle.AutoSize = false;
            this.lblAuditTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblAuditTitle.Location = new System.Drawing.Point(16, 10);
            this.lblAuditTitle.Name = "lblAuditTitle";
            this.lblAuditTitle.Size = new System.Drawing.Size(340, 26);
            this.lblAuditTitle.Text = "Audit log — sensitive commands";
            //
            // lblAuditScope  (says whether the caller is seeing the tenant or only their own entries)
            //
            this.lblAuditScope.AutoSize = false;
            this.lblAuditScope.Font = new System.Drawing.Font("default", 9F);
            this.lblAuditScope.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblAuditScope.Location = new System.Drawing.Point(360, 12);
            this.lblAuditScope.Name = "lblAuditScope";
            this.lblAuditScope.Size = new System.Drawing.Size(436, 24);
            this.lblAuditScope.Text = "";
            this.lblAuditScope.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cboUser
            //
            this.cboUser.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboUser.Location = new System.Drawing.Point(16, 44);
            this.cboUser.Name = "cboUser";
            this.cboUser.Size = new System.Drawing.Size(150, 32);
            this.cboUser.ToolTipText = "User: any — the filter is applied by AuditQueryService, not by the grid.";
            this.cboUser.SelectedIndexChanged += new System.EventHandler(this.auditFilter_Changed);
            //
            // cboPermission
            //
            this.cboPermission.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPermission.Location = new System.Drawing.Point(176, 44);
            this.cboPermission.Name = "cboPermission";
            this.cboPermission.Size = new System.Drawing.Size(190, 32);
            this.cboPermission.ToolTipText = "Permission: any";
            this.cboPermission.SelectedIndexChanged += new System.EventHandler(this.auditFilter_Changed);
            //
            // cboResult
            //
            this.cboResult.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboResult.Location = new System.Drawing.Point(376, 44);
            this.cboResult.Name = "cboResult";
            this.cboResult.Size = new System.Drawing.Size(130, 32);
            this.cboResult.ToolTipText = "Result: any · OK · DENIED · PENDING · FAILED";
            this.cboResult.SelectedIndexChanged += new System.EventHandler(this.auditFilter_Changed);
            //
            // lblAuditCount
            //
            this.lblAuditCount.AutoSize = false;
            this.lblAuditCount.Font = new System.Drawing.Font("default", 9F);
            this.lblAuditCount.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblAuditCount.Location = new System.Drawing.Point(516, 44);
            this.lblAuditCount.Name = "lblAuditCount";
            this.lblAuditCount.Size = new System.Drawing.Size(280, 32);
            this.lblAuditCount.Text = "";
            this.lblAuditCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dgvAudit  (the walkthrough's grid: time · user · tenant · permission demanded · result, plus the detail)
            //
            this.dgvAudit.AllowUserToAddRows = false;
            this.dgvAudit.AllowUserToDeleteRows = false;
            this.dgvAudit.AutoGenerateColumns = false;
            this.dgvAudit.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAudit.BackColor = System.Drawing.Color.White;
            this.dgvAudit.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colTime,
            this.colUser,
            this.colTenant,
            this.colPermission,
            this.colResult,
            this.colDetail});
            this.dgvAudit.Location = new System.Drawing.Point(16, 86);
            this.dgvAudit.MultiSelect = false;
            this.dgvAudit.Name = "dgvAudit";
            this.dgvAudit.ReadOnly = true;
            this.dgvAudit.RowHeadersVisible = false;
            this.dgvAudit.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAudit.Size = new System.Drawing.Size(780, 138);
            //
            // colTime
            //
            this.colTime.DataPropertyName = "Time";
            this.colTime.FillWeight = 9F;
            this.colTime.HeaderText = "Time";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            //
            // colUser  (user ids arrive from the identity provider — an AllowHtml review item)
            //
            this.colUser.DataPropertyName = "User";
            this.colUser.FillWeight = 12F;
            this.colUser.HeaderText = "User";
            this.colUser.Name = "colUser";
            this.colUser.ReadOnly = true;
            //
            // colTenant
            //
            this.colTenant.DataPropertyName = "Tenant";
            this.colTenant.FillWeight = 10F;
            this.colTenant.HeaderText = "Tenant";
            this.colTenant.Name = "colTenant";
            this.colTenant.ReadOnly = true;
            //
            // colPermission
            //
            this.colPermission.DataPropertyName = "Action";
            this.colPermission.FillWeight = 20F;
            this.colPermission.HeaderText = "Permission demanded";
            this.colPermission.Name = "colPermission";
            this.colPermission.ReadOnly = true;
            //
            // colResult
            //
            this.colResult.DataPropertyName = "Result";
            this.colResult.FillWeight = 10F;
            this.colResult.HeaderText = "Result";
            this.colResult.Name = "colResult";
            this.colResult.ReadOnly = true;
            //
            // colDetail  (service-written detail that quotes user ids and targets — an AllowHtml review item)
            //
            this.colDetail.DataPropertyName = "Detail";
            this.colDetail.FillWeight = 39F;
            this.colDetail.HeaderText = "Detail · correlation id";
            this.colDetail.Name = "colDetail";
            this.colDetail.ReadOnly = true;
            //
            // lblStatusBar  (the walkthrough's dark footer line)
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(16, 232);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(780, 30);
            this.lblStatusBar.Text = "Not signed in — the gate has not run yet";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlTrace  (Server · live activity trace)
            //
            this.pnlTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlTrace.BackColor = System.Drawing.Color.White;
            this.pnlTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTrace.Controls.Add(this.lblTraceTitle);
            this.pnlTrace.Controls.Add(this.lstTrace);
            this.pnlTrace.Controls.Add(this.lblTraceFooter);
            this.pnlTrace.Location = new System.Drawing.Point(852, 64);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(472, 296);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(440, 28);
            this.lblTraceTitle.Text = "Server · live activity trace";
            //
            // lstTrace  (quotes provider group names and user ids — an AllowHtml review item)
            //
            this.lstTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTrace.Location = new System.Drawing.Point(16, 46);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(440, 208);
            //
            // lblTraceFooter
            //
            this.lblTraceFooter.AutoSize = false;
            this.lblTraceFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTraceFooter.Location = new System.Drawing.Point(16, 258);
            this.lblTraceFooter.Name = "lblTraceFooter";
            this.lblTraceFooter.Size = new System.Drawing.Size(440, 28);
            this.lblTraceFooter.Text = "UI → · Identity: · Security: · Service: · Data: · Audit: · UI ←";
            //
            // pnlNote  (the safe-HTML review: one untrusted note, three renderings)
            //
            this.pnlNote.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlNote.BackColor = System.Drawing.Color.White;
            this.pnlNote.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlNote.Controls.Add(this.lblNoteTitle);
            this.pnlNote.Controls.Add(this.lblNoteMode);
            this.pnlNote.Controls.Add(this.lblNoteSource);
            this.pnlNote.Controls.Add(this.pnlNoteRender);
            this.pnlNote.Controls.Add(this.lblNoteWarning);
            this.pnlNote.Location = new System.Drawing.Point(852, 372);
            this.pnlNote.Name = "pnlNote";
            this.pnlNote.Size = new System.Drawing.Size(472, 198);
            //
            // lblNoteTitle
            //
            this.lblNoteTitle.AutoSize = false;
            this.lblNoteTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblNoteTitle.Location = new System.Drawing.Point(16, 10);
            this.lblNoteTitle.Name = "lblNoteTitle";
            this.lblNoteTitle.Size = new System.Drawing.Size(280, 24);
            this.lblNoteTitle.Text = "Customer note — untrusted";
            //
            // lblNoteMode
            //
            this.lblNoteMode.AutoSize = false;
            this.lblNoteMode.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblNoteMode.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblNoteMode.Location = new System.Drawing.Point(300, 10);
            this.lblNoteMode.Name = "lblNoteMode";
            this.lblNoteMode.Size = new System.Drawing.Size(156, 24);
            this.lblNoteMode.Text = "AllowHtml = false";
            this.lblNoteMode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblNoteSource  (author and source arrive from outside — an AllowHtml review item)
            //
            this.lblNoteSource.AutoSize = false;
            this.lblNoteSource.Font = new System.Drawing.Font("monospace", 8F);
            this.lblNoteSource.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNoteSource.Location = new System.Drawing.Point(16, 38);
            this.lblNoteSource.Name = "lblNoteSource";
            this.lblNoteSource.Size = new System.Drawing.Size(440, 32);
            this.lblNoteSource.Text = "";
            //
            // pnlNoteRender
            //
            this.pnlNoteRender.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlNoteRender.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlNoteRender.Controls.Add(this.lblNote);
            this.pnlNoteRender.Location = new System.Drawing.Point(16, 74);
            this.pnlNoteRender.Name = "pnlNoteRender";
            this.pnlNoteRender.Size = new System.Drawing.Size(440, 72);
            //
            // lblNote  (THE surface under review: the customer note itself)
            //
            this.lblNote.AllowHtml = false;
            this.lblNote.AutoSize = false;
            this.lblNote.Font = new System.Drawing.Font("default", 9F);
            this.lblNote.Location = new System.Drawing.Point(10, 6);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(418, 60);
            this.lblNote.Text = "";
            //
            // lblNoteWarning
            //
            this.lblNoteWarning.AutoSize = false;
            this.lblNoteWarning.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblNoteWarning.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblNoteWarning.Location = new System.Drawing.Point(16, 150);
            this.lblNoteWarning.Name = "lblNoteWarning";
            this.lblNoteWarning.Size = new System.Drawing.Size(440, 38);
            this.lblNoteWarning.Text = "";
            //
            // pnlActions  (identity switch · the failure paths · the recoveries · the review · clear)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.Controls.Add(this.btnSwitchIdentity);
            this.pnlActions.Controls.Add(this.btnBreakUi);
            this.pnlActions.Controls.Add(this.btnCrossTenant);
            this.pnlActions.Controls.Add(this.btnUnsafeHtml);
            this.pnlActions.Controls.Add(this.btnSafeHtml);
            this.pnlActions.Controls.Add(this.btnHtmlReview);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Location = new System.Drawing.Point(24, 584);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1300, 44);
            //
            // btnSwitchIdentity
            //
            this.btnSwitchIdentity.Location = new System.Drawing.Point(0, 4);
            this.btnSwitchIdentity.Name = "btnSwitchIdentity";
            this.btnSwitchIdentity.Size = new System.Drawing.Size(170, 36);
            this.btnSwitchIdentity.Text = "Switch identity (SSO)";
            this.btnSwitchIdentity.ToolTipText = "Signs out and reopens the gate. The audit log keeps every identity's entries — that is what it is for.";
            this.btnSwitchIdentity.Click += new System.EventHandler(this.btnSwitchIdentity_Click);
            //
            // btnBreakUi  (failure path 1, first half: the UI bug from the walkthrough)
            //
            this.btnBreakUi.Location = new System.Drawing.Point(178, 4);
            this.btnBreakUi.Name = "btnBreakUi";
            this.btnBreakUi.Size = new System.Drawing.Size(206, 36);
            this.btnBreakUi.Text = "Break the UI: enable Export";
            this.btnBreakUi.ToolTipText = "Forces btnExport visible and enabled for a caller without ExportData — then click Export data.";
            this.btnBreakUi.Click += new System.EventHandler(this.btnBreakUi_Click);
            //
            // btnCrossTenant  (failure path 2: a record from another tenant)
            //
            this.btnCrossTenant.Location = new System.Drawing.Point(392, 4);
            this.btnCrossTenant.Name = "btnCrossTenant";
            this.btnCrossTenant.Size = new System.Drawing.Size(194, 36);
            this.btnCrossTenant.Text = "Fail: cross-tenant approve";
            this.btnCrossTenant.ToolTipText = "Approves a work order of another tenant: the tenant guard rejects it before the role store is consulted.";
            this.btnCrossTenant.Click += new System.EventHandler(this.btnCrossTenant_Click);
            //
            // btnUnsafeHtml  (failure path 3: the AllowHtml mistake)
            //
            this.btnUnsafeHtml.Location = new System.Drawing.Point(594, 4);
            this.btnUnsafeHtml.Name = "btnUnsafeHtml";
            this.btnUnsafeHtml.Size = new System.Drawing.Size(206, 36);
            this.btnUnsafeHtml.Text = "Fail: render note as raw HTML";
            this.btnUnsafeHtml.ToolTipText = "Sets lblNote.AllowHtml = true with the raw customer note: the payload becomes markup.";
            this.btnUnsafeHtml.Click += new System.EventHandler(this.btnUnsafeHtml_Click);
            //
            // btnSafeHtml  (recovery 3)
            //
            this.btnSafeHtml.Location = new System.Drawing.Point(808, 4);
            this.btnSafeHtml.Name = "btnSafeHtml";
            this.btnSafeHtml.Size = new System.Drawing.Size(186, 36);
            this.btnSafeHtml.Text = "Recover: escape / sanitize";
            this.btnSafeHtml.ToolTipText = "Cycles the safe renderings: AllowHtml = false, then the allow-list sanitizer.";
            this.btnSafeHtml.Click += new System.EventHandler(this.btnSafeHtml_Click);
            //
            // btnHtmlReview
            //
            this.btnHtmlReview.Location = new System.Drawing.Point(1002, 4);
            this.btnHtmlReview.Name = "btnHtmlReview";
            this.btnHtmlReview.Size = new System.Drawing.Size(176, 36);
            this.btnHtmlReview.Text = "AllowHtml review";
            this.btnHtmlReview.ToolTipText = "Walks the live control tree for every surface with an AllowHtml property and reports the findings.";
            this.btnHtmlReview.Click += new System.EventHandler(this.btnHtmlReview_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearTrace.Location = new System.Drawing.Point(1190, 4);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(110, 36);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.ToolTipText = "Empties the trace card. The audit log is append-only and is never cleared by a button.";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // AuditLogPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlWork);
            this.Controls.Add(this.pnlAudit);
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlNote);
            this.Controls.Add(this.pnlActions);
            this.Name = "AuditLogPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "EnterpriseOps — Work queue + audit";
            this.Load += new System.EventHandler(this.AuditLogPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlWork.ResumeLayout(false);
            this.pnlAudit.ResumeLayout(false);
            this.pnlTrace.ResumeLayout(false);
            this.pnlNoteRender.ResumeLayout(false);
            this.pnlNote.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblTenant;
        private Wisej.Web.Label lblUser;
        private Wisej.Web.Label lblCorrelation;
        private Wisej.Web.Panel pnlWork;
        private Wisej.Web.Label lblWorkTitle;
        private Wisej.Web.Button btnApprove;
        private Wisej.Web.Button btnApproveExport;
        private Wisej.Web.Button btnExport;
        private Wisej.Web.DataGridView dgvWorkOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colRef;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colApprovedBy;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Panel pnlAudit;
        private Wisej.Web.Label lblAuditTitle;
        private Wisej.Web.Label lblAuditScope;
        private Wisej.Web.ComboBox cboUser;
        private Wisej.Web.ComboBox cboPermission;
        private Wisej.Web.ComboBox cboResult;
        private Wisej.Web.Label lblAuditCount;
        private Wisej.Web.DataGridView dgvAudit;
        private Wisej.Web.DataGridViewTextBoxColumn colTime;
        private Wisej.Web.DataGridViewTextBoxColumn colUser;
        private Wisej.Web.DataGridViewTextBoxColumn colTenant;
        private Wisej.Web.DataGridViewTextBoxColumn colPermission;
        private Wisej.Web.DataGridViewTextBoxColumn colResult;
        private Wisej.Web.DataGridViewTextBoxColumn colDetail;
        private Wisej.Web.Label lblStatusBar;
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Label lblTraceFooter;
        private Wisej.Web.Panel pnlNote;
        private Wisej.Web.Label lblNoteTitle;
        private Wisej.Web.Label lblNoteMode;
        private Wisej.Web.Label lblNoteSource;
        private Wisej.Web.Panel pnlNoteRender;
        private Wisej.Web.Label lblNote;
        private Wisej.Web.Label lblNoteWarning;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnSwitchIdentity;
        private Wisej.Web.Button btnBreakUi;
        private Wisej.Web.Button btnCrossTenant;
        private Wisej.Web.Button btnUnsafeHtml;
        private Wisej.Web.Button btnSafeHtml;
        private Wisej.Web.Button btnHtmlReview;
        private Wisej.Web.Button btnClearTrace;
    }
}
