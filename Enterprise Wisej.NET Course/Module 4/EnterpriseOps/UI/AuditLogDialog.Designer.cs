namespace EnterpriseOps.UI
{
    partial class AuditLogDialog
    {
        private System.ComponentModel.IContainer components = null;

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
            this.lblAuditTitle = new Wisej.Web.Label();
            this.dgvAudit = new Wisej.Web.DataGridView();
            this.colTime = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAction = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOutcome = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colErrorCode = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colWorkOrder = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colUser = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCorrelation = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDetail = new Wisej.Web.DataGridViewTextBoxColumn();
            this.btnClose = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // lblAuditTitle
            //
            this.lblAuditTitle.AutoSize = false;
            this.lblAuditTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblAuditTitle.Location = new System.Drawing.Point(20, 16);
            this.lblAuditTitle.Name = "lblAuditTitle";
            this.lblAuditTitle.Size = new System.Drawing.Size(880, 28);
            this.lblAuditTitle.Text = "Audit log";
            //
            // dgvAudit
            //
            this.dgvAudit.AllowUserToAddRows = false;
            this.dgvAudit.AllowUserToDeleteRows = false;
            this.dgvAudit.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvAudit.AutoGenerateColumns = false;
            this.dgvAudit.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAudit.BackColor = System.Drawing.Color.White;
            this.dgvAudit.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colTime,
            this.colAction,
            this.colOutcome,
            this.colErrorCode,
            this.colWorkOrder,
            this.colUser,
            this.colCorrelation,
            this.colDetail});
            this.dgvAudit.Location = new System.Drawing.Point(20, 52);
            this.dgvAudit.MultiSelect = false;
            this.dgvAudit.Name = "dgvAudit";
            this.dgvAudit.ReadOnly = true;
            this.dgvAudit.RowHeadersVisible = false;
            this.dgvAudit.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAudit.Size = new System.Drawing.Size(880, 372);
            //
            // columns
            //
            this.colTime.DataPropertyName = "TimestampUtc";
            this.colTime.FillWeight = 13F;
            this.colTime.HeaderText = "Time (UTC)";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colAction.DataPropertyName = "Action";
            this.colAction.FillWeight = 9F;
            this.colAction.HeaderText = "Action";
            this.colAction.Name = "colAction";
            this.colAction.ReadOnly = true;
            this.colOutcome.DataPropertyName = "Outcome";
            this.colOutcome.FillWeight = 10F;
            this.colOutcome.HeaderText = "Outcome";
            this.colOutcome.Name = "colOutcome";
            this.colOutcome.ReadOnly = true;
            this.colErrorCode.DataPropertyName = "ErrorCode";
            this.colErrorCode.FillWeight = 15F;
            this.colErrorCode.HeaderText = "Code";
            this.colErrorCode.Name = "colErrorCode";
            this.colErrorCode.ReadOnly = true;
            this.colWorkOrder.DataPropertyName = "WorkOrderId";
            this.colWorkOrder.FillWeight = 8F;
            this.colWorkOrder.HeaderText = "WO";
            this.colWorkOrder.Name = "colWorkOrder";
            this.colWorkOrder.ReadOnly = true;
            this.colUser.DataPropertyName = "UserId";
            this.colUser.FillWeight = 11F;
            this.colUser.HeaderText = "User";
            this.colUser.Name = "colUser";
            this.colUser.ReadOnly = true;
            this.colCorrelation.DataPropertyName = "CorrelationId";
            this.colCorrelation.FillWeight = 11F;
            this.colCorrelation.HeaderText = "Correlation";
            this.colCorrelation.Name = "colCorrelation";
            this.colCorrelation.ReadOnly = true;
            this.colDetail.DataPropertyName = "Detail";
            this.colDetail.FillWeight = 33F;
            this.colDetail.HeaderText = "Detail";
            this.colDetail.Name = "colDetail";
            this.colDetail.ReadOnly = true;
            //
            // btnClose
            //
            this.btnClose.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.btnClose.Location = new System.Drawing.Point(790, 434);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 38);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // AuditLogDialog
            //
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(920, 492);
            this.Controls.Add(this.lblAuditTitle);
            this.Controls.Add(this.dgvAudit);
            this.Controls.Add(this.btnClose);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AuditLogDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Audit log";
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblAuditTitle;
        private Wisej.Web.DataGridView dgvAudit;
        private Wisej.Web.DataGridViewTextBoxColumn colTime;
        private Wisej.Web.DataGridViewTextBoxColumn colAction;
        private Wisej.Web.DataGridViewTextBoxColumn colOutcome;
        private Wisej.Web.DataGridViewTextBoxColumn colErrorCode;
        private Wisej.Web.DataGridViewTextBoxColumn colWorkOrder;
        private Wisej.Web.DataGridViewTextBoxColumn colUser;
        private Wisej.Web.DataGridViewTextBoxColumn colCorrelation;
        private Wisej.Web.DataGridViewTextBoxColumn colDetail;
        private Wisej.Web.Button btnClose;
    }
}
