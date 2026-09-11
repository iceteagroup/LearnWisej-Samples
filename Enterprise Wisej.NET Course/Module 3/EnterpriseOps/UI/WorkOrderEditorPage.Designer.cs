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
            this.pnlWork = new Wisej.Web.Panel();
            this.lblQueueTitle = new Wisej.Web.Label();
            this.dgvWorkQueue = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colWorkOrder = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colState = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssigned = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colVersion = new Wisej.Web.DataGridViewTextBoxColumn();
            this.btnOpen = new Wisej.Web.Button();
            this.pnlEditor = new Wisej.Web.Panel();
            this.lblEditCaption = new Wisej.Web.Label();
            this.lblTitleCaption = new Wisej.Web.Label();
            this.txtTitle = new Wisej.Web.TextBox();
            this.lblStatusCaption = new Wisej.Web.Label();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.lblVersionCaption = new Wisej.Web.Label();
            this.txtVersion = new Wisej.Web.TextBox();
            this.btnSave = new Wisej.Web.Button();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlWork.SuspendLayout();
            this.pnlEditor.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblTenantCaption);
            this.pnlHeader.Controls.Add(this.cboTenant);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(860, 44);
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
            this.lblTenantCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblTenantCaption.AutoSize = false;
            this.lblTenantCaption.Font = new System.Drawing.Font("default", 9F);
            this.lblTenantCaption.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblTenantCaption.Location = new System.Drawing.Point(538, 0);
            this.lblTenantCaption.Name = "lblTenantCaption";
            this.lblTenantCaption.Size = new System.Drawing.Size(60, 44);
            this.lblTenantCaption.Text = "Tenant";
            this.lblTenantCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cboTenant
            //
            this.cboTenant.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.cboTenant.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboTenant.Location = new System.Drawing.Point(606, 9);
            this.cboTenant.Name = "cboTenant";
            this.cboTenant.Size = new System.Drawing.Size(230, 26);
            this.cboTenant.SelectedIndexChanged += new System.EventHandler(this.cboTenant_SelectedIndexChanged);
            //
            // pnlWork
            //
            this.pnlWork.BackColor = System.Drawing.Color.White;
            this.pnlWork.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlWork.Controls.Add(this.lblQueueTitle);
            this.pnlWork.Controls.Add(this.dgvWorkQueue);
            this.pnlWork.Controls.Add(this.btnOpen);
            this.pnlWork.Controls.Add(this.pnlEditor);
            this.pnlWork.Controls.Add(this.lblBanner);
            this.pnlWork.Controls.Add(this.lblStatusBar);
            this.pnlWork.Location = new System.Drawing.Point(24, 64);
            this.pnlWork.Name = "pnlWork";
            this.pnlWork.Size = new System.Drawing.Size(812, 492);
            //
            // lblQueueTitle
            //
            this.lblQueueTitle.AutoSize = false;
            this.lblQueueTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblQueueTitle.Location = new System.Drawing.Point(16, 10);
            this.lblQueueTitle.Name = "lblQueueTitle";
            this.lblQueueTitle.Size = new System.Drawing.Size(776, 26);
            this.lblQueueTitle.Text = "Work queue";
            //
            // dgvWorkQueue
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
            // colVersion
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
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            //
            // pnlEditor
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
            this.pnlEditor.Controls.Add(this.btnSave);
            this.pnlEditor.Location = new System.Drawing.Point(20, 242);
            this.pnlEditor.Name = "pnlEditor";
            this.pnlEditor.Size = new System.Drawing.Size(772, 148);
            //
            // lblEditCaption
            //
            this.lblEditCaption.AutoSize = false;
            this.lblEditCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblEditCaption.Location = new System.Drawing.Point(14, 8);
            this.lblEditCaption.Name = "lblEditCaption";
            this.lblEditCaption.Size = new System.Drawing.Size(744, 22);
            this.lblEditCaption.Text = "No work order open";
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
            this.txtTitle.Size = new System.Drawing.Size(744, 30);
            //
            // lblStatusCaption
            //
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblStatusCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblStatusCaption.Location = new System.Drawing.Point(14, 92);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(120, 18);
            this.lblStatusCaption.Text = "STATUS";
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(14, 110);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(300, 30);
            //
            // lblVersionCaption
            //
            this.lblVersionCaption.AutoSize = false;
            this.lblVersionCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblVersionCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblVersionCaption.Location = new System.Drawing.Point(328, 92);
            this.lblVersionCaption.Name = "lblVersionCaption";
            this.lblVersionCaption.Size = new System.Drawing.Size(140, 18);
            this.lblVersionCaption.Text = "VERSION TOKEN";
            //
            // txtVersion
            //
            this.txtVersion.Font = new System.Drawing.Font("monospace", 9F);
            this.txtVersion.Location = new System.Drawing.Point(328, 110);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.ReadOnly = true;
            this.txtVersion.Size = new System.Drawing.Size(180, 30);
            this.txtVersion.Text = "—";
            //
            // btnSave
            //
            this.btnSave.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(646, 106);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(112, 36);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // lblBanner
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(20, 398);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(772, 40);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatusBar
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(20, 446);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(772, 30);
            this.lblStatusBar.Text = "Loading…";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // WorkOrderEditorPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlWork);
            this.Name = "WorkOrderEditorPage";
            this.Size = new System.Drawing.Size(860, 580);
            this.Text = "EnterpriseOps — Work Order Editor";
            this.Load += new System.EventHandler(this.WorkOrderEditorPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlWork.ResumeLayout(false);
            this.pnlEditor.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblTenantCaption;
        private Wisej.Web.ComboBox cboTenant;
        private Wisej.Web.Panel pnlWork;
        private Wisej.Web.Label lblQueueTitle;
        private Wisej.Web.DataGridView dgvWorkQueue;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colWorkOrder;
        private Wisej.Web.DataGridViewTextBoxColumn colState;
        private Wisej.Web.DataGridViewTextBoxColumn colAssigned;
        private Wisej.Web.DataGridViewTextBoxColumn colVersion;
        private Wisej.Web.Button btnOpen;
        private Wisej.Web.Panel pnlEditor;
        private Wisej.Web.Label lblEditCaption;
        private Wisej.Web.Label lblTitleCaption;
        private Wisej.Web.TextBox txtTitle;
        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.Label lblVersionCaption;
        private Wisej.Web.TextBox txtVersion;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatusBar;
    }
}
