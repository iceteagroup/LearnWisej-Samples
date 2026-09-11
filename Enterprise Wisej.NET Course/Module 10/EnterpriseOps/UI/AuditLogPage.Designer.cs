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
            this.lblBanner = new Wisej.Web.Label();
            this.pnlNote = new Wisej.Web.Panel();
            this.lblNoteTitle = new Wisej.Web.Label();
            this.lblNoteSource = new Wisej.Web.Label();
            this.pnlNoteRender = new Wisej.Web.Panel();
            this.lblNote = new Wisej.Web.Label();
            this.pnlAudit = new Wisej.Web.Panel();
            this.lblAuditTitle = new Wisej.Web.Label();
            this.cboUser = new Wisej.Web.ComboBox();
            this.cboPermission = new Wisej.Web.ComboBox();
            this.cboResult = new Wisej.Web.ComboBox();
            this.dgvAudit = new Wisej.Web.DataGridView();
            this.colTime = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colUser = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTenant = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPermission = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colResult = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDetail = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlWork.SuspendLayout();
            this.pnlNote.SuspendLayout();
            this.pnlNoteRender.SuspendLayout();
            this.pnlAudit.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1256, 44);
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
            // pnlWork
            //
            this.pnlWork.BackColor = System.Drawing.Color.White;
            this.pnlWork.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlWork.Controls.Add(this.lblWorkTitle);
            this.pnlWork.Controls.Add(this.btnApprove);
            this.pnlWork.Controls.Add(this.btnApproveExport);
            this.pnlWork.Controls.Add(this.btnExport);
            this.pnlWork.Controls.Add(this.dgvWorkOrders);
            this.pnlWork.Controls.Add(this.lblBanner);
            this.pnlWork.Location = new System.Drawing.Point(24, 64);
            this.pnlWork.Name = "pnlWork";
            this.pnlWork.Size = new System.Drawing.Size(812, 202);
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
            // btnApprove
            //
            this.btnApprove.Location = new System.Drawing.Point(322, 10);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(146, 34);
            this.btnApprove.Text = "✔ Approve";
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            //
            // btnApproveExport
            //
            this.btnApproveExport.Location = new System.Drawing.Point(476, 10);
            this.btnApproveExport.Name = "btnApproveExport";
            this.btnApproveExport.Size = new System.Drawing.Size(186, 34);
            this.btnApproveExport.Text = "Approve pending export";
            this.btnApproveExport.Click += new System.EventHandler(this.btnApproveExport_Click);
            //
            // btnExport
            //
            this.btnExport.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnExport.Location = new System.Drawing.Point(670, 10);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(126, 34);
            this.btnExport.Text = "⤓ Export data";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            //
            // dgvWorkOrders
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
            // lblBanner
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(16, 152);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(780, 42);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // pnlNote
            //
            this.pnlNote.BackColor = System.Drawing.Color.White;
            this.pnlNote.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlNote.Controls.Add(this.lblNoteTitle);
            this.pnlNote.Controls.Add(this.lblNoteSource);
            this.pnlNote.Controls.Add(this.pnlNoteRender);
            this.pnlNote.Location = new System.Drawing.Point(852, 64);
            this.pnlNote.Name = "pnlNote";
            this.pnlNote.Size = new System.Drawing.Size(380, 202);
            //
            // lblNoteTitle
            //
            this.lblNoteTitle.AutoSize = false;
            this.lblNoteTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblNoteTitle.Location = new System.Drawing.Point(16, 12);
            this.lblNoteTitle.Name = "lblNoteTitle";
            this.lblNoteTitle.Size = new System.Drawing.Size(348, 26);
            this.lblNoteTitle.Text = "Customer note";
            //
            // lblNoteSource
            //
            this.lblNoteSource.AutoSize = false;
            this.lblNoteSource.Font = new System.Drawing.Font("monospace", 8F);
            this.lblNoteSource.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNoteSource.Location = new System.Drawing.Point(16, 40);
            this.lblNoteSource.Name = "lblNoteSource";
            this.lblNoteSource.Size = new System.Drawing.Size(348, 22);
            this.lblNoteSource.Text = "";
            //
            // pnlNoteRender
            //
            this.pnlNoteRender.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlNoteRender.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlNoteRender.Controls.Add(this.lblNote);
            this.pnlNoteRender.Location = new System.Drawing.Point(16, 66);
            this.pnlNoteRender.Name = "pnlNoteRender";
            this.pnlNoteRender.Size = new System.Drawing.Size(348, 120);
            //
            // lblNote
            //
            this.lblNote.AllowHtml = false;
            this.lblNote.AutoSize = false;
            this.lblNote.Font = new System.Drawing.Font("default", 9F);
            this.lblNote.Location = new System.Drawing.Point(10, 6);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(326, 106);
            this.lblNote.Text = "";
            //
            // pnlAudit
            //
            this.pnlAudit.BackColor = System.Drawing.Color.White;
            this.pnlAudit.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlAudit.Controls.Add(this.lblAuditTitle);
            this.pnlAudit.Controls.Add(this.cboUser);
            this.pnlAudit.Controls.Add(this.cboPermission);
            this.pnlAudit.Controls.Add(this.cboResult);
            this.pnlAudit.Controls.Add(this.dgvAudit);
            this.pnlAudit.Controls.Add(this.lblStatusBar);
            this.pnlAudit.Location = new System.Drawing.Point(24, 282);
            this.pnlAudit.Name = "pnlAudit";
            this.pnlAudit.Size = new System.Drawing.Size(1208, 272);
            //
            // lblAuditTitle
            //
            this.lblAuditTitle.AutoSize = false;
            this.lblAuditTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblAuditTitle.Location = new System.Drawing.Point(16, 10);
            this.lblAuditTitle.Name = "lblAuditTitle";
            this.lblAuditTitle.Size = new System.Drawing.Size(400, 26);
            this.lblAuditTitle.Text = "Audit log — sensitive commands";
            //
            // cboUser
            //
            this.cboUser.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboUser.Location = new System.Drawing.Point(16, 44);
            this.cboUser.Name = "cboUser";
            this.cboUser.Size = new System.Drawing.Size(150, 32);
            this.cboUser.SelectedIndexChanged += new System.EventHandler(this.auditFilter_Changed);
            //
            // cboPermission
            //
            this.cboPermission.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPermission.Location = new System.Drawing.Point(176, 44);
            this.cboPermission.Name = "cboPermission";
            this.cboPermission.Size = new System.Drawing.Size(190, 32);
            this.cboPermission.SelectedIndexChanged += new System.EventHandler(this.auditFilter_Changed);
            //
            // cboResult
            //
            this.cboResult.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboResult.Location = new System.Drawing.Point(376, 44);
            this.cboResult.Name = "cboResult";
            this.cboResult.Size = new System.Drawing.Size(130, 32);
            this.cboResult.SelectedIndexChanged += new System.EventHandler(this.auditFilter_Changed);
            //
            // dgvAudit
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
            this.dgvAudit.Size = new System.Drawing.Size(1176, 138);
            //
            // colTime
            //
            this.colTime.DataPropertyName = "Time";
            this.colTime.FillWeight = 9F;
            this.colTime.HeaderText = "Time";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            //
            // colUser
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
            // colDetail
            //
            this.colDetail.DataPropertyName = "Detail";
            this.colDetail.FillWeight = 39F;
            this.colDetail.HeaderText = "Detail · correlation id";
            this.colDetail.Name = "colDetail";
            this.colDetail.ReadOnly = true;
            //
            // lblStatusBar
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(16, 232);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(1176, 30);
            this.lblStatusBar.Text = "Not signed in";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // AuditLogPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlWork);
            this.Controls.Add(this.pnlNote);
            this.Controls.Add(this.pnlAudit);
            this.Name = "AuditLogPage";
            this.Size = new System.Drawing.Size(1256, 578);
            this.Text = "EnterpriseOps — Work queue + audit";
            this.Load += new System.EventHandler(this.AuditLogPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlWork.ResumeLayout(false);
            this.pnlNoteRender.ResumeLayout(false);
            this.pnlNote.ResumeLayout(false);
            this.pnlAudit.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
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
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Panel pnlNote;
        private Wisej.Web.Label lblNoteTitle;
        private Wisej.Web.Label lblNoteSource;
        private Wisej.Web.Panel pnlNoteRender;
        private Wisej.Web.Label lblNote;
        private Wisej.Web.Panel pnlAudit;
        private Wisej.Web.Label lblAuditTitle;
        private Wisej.Web.ComboBox cboUser;
        private Wisej.Web.ComboBox cboPermission;
        private Wisej.Web.ComboBox cboResult;
        private Wisej.Web.DataGridView dgvAudit;
        private Wisej.Web.DataGridViewTextBoxColumn colTime;
        private Wisej.Web.DataGridViewTextBoxColumn colUser;
        private Wisej.Web.DataGridViewTextBoxColumn colTenant;
        private Wisej.Web.DataGridViewTextBoxColumn colPermission;
        private Wisej.Web.DataGridViewTextBoxColumn colResult;
        private Wisej.Web.DataGridViewTextBoxColumn colDetail;
        private Wisej.Web.Label lblStatusBar;
    }
}
