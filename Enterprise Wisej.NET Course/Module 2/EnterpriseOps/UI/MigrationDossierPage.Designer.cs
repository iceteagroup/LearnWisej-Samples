namespace EnterpriseOps.UI
{
    partial class MigrationDossierPage
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
            this.pnlDossier = new Wisej.Web.Panel();
            this.btnBuildDossier = new Wisej.Web.Button();
            this.btnRunPath = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.btnRollback = new Wisej.Web.Button();
            this.btnMapTheme = new Wisej.Web.Button();
            this.btnWorkOrders = new Wisej.Web.Button();
            this.tabDossier = new Wisej.Web.TabControl();
            this.tabPageDossier = new Wisej.Web.TabPage();
            this.dgvDossier = new Wisej.Web.DataGridView();
            this.colArea = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCurrent = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTarget = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colRisk = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colProof = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colRollback = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colRowState = new Wisej.Web.DataGridViewTextBoxColumn();
            this.tabPageInventory = new Wisej.Web.TabPage();
            this.dgvInventory = new Wisej.Web.DataGridView();
            this.colCategory = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colItem = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCurrentState = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTargetState = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colNote = new Wisej.Web.DataGridViewTextBoxColumn();
            this.tabPageCompatibility = new Wisej.Web.TabPage();
            this.dgvCompatibility = new Wisej.Web.DataGridView();
            this.colComponent = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colFromVersion = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colToVersion = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colSupported = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colMatrixRisk = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colMitigation = new Wisej.Web.DataGridViewTextBoxColumn();
            this.tabPageSteps = new Wisej.Web.TabPage();
            this.dgvSteps = new Wisej.Web.DataGridView();
            this.colStepNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStepName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStepCheck = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colFallbackPoint = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStepState = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStepDetail = new Wisej.Web.DataGridViewTextBoxColumn();
            this.tabPageFlows = new Wisej.Web.TabPage();
            this.dgvFlows = new Wisej.Web.DataGridView();
            this.colFlowId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colFlowName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colFlowScreen = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colFlowCategory = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colFlowExpected = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colFlowResult = new Wisej.Web.DataGridViewTextBoxColumn();
            this.tabPageMemo = new Wisej.Web.TabPage();
            this.txtMemo = new Wisej.Web.TextBox();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlDossier.SuspendLayout();
            this.tabDossier.SuspendLayout();
            this.tabPageDossier.SuspendLayout();
            this.tabPageInventory.SuspendLayout();
            this.tabPageCompatibility.SuspendLayout();
            this.tabPageSteps.SuspendLayout();
            this.tabPageFlows.SuspendLayout();
            this.tabPageMemo.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1048, 44);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(24, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1000, 44);
            this.lblTitle.Text = "EnterpriseOps — Migration Dossier · TicketOps Console → EnterpriseOps baseline";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlDossier
            //
            this.pnlDossier.BackColor = System.Drawing.Color.White;
            this.pnlDossier.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlDossier.Controls.Add(this.btnBuildDossier);
            this.pnlDossier.Controls.Add(this.btnRunPath);
            this.pnlDossier.Controls.Add(this.btnCancel);
            this.pnlDossier.Controls.Add(this.btnRollback);
            this.pnlDossier.Controls.Add(this.btnMapTheme);
            this.pnlDossier.Controls.Add(this.btnWorkOrders);
            this.pnlDossier.Controls.Add(this.tabDossier);
            this.pnlDossier.Controls.Add(this.lblBanner);
            this.pnlDossier.Controls.Add(this.lblStatusBar);
            this.pnlDossier.Location = new System.Drawing.Point(24, 64);
            this.pnlDossier.Name = "pnlDossier";
            this.pnlDossier.Size = new System.Drawing.Size(1000, 512);
            //
            // btnBuildDossier
            //
            this.btnBuildDossier.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnBuildDossier.Location = new System.Drawing.Point(20, 14);
            this.btnBuildDossier.Name = "btnBuildDossier";
            this.btnBuildDossier.Size = new System.Drawing.Size(160, 36);
            this.btnBuildDossier.Text = "Build dossier";
            this.btnBuildDossier.Click += new System.EventHandler(this.btnBuildDossier_Click);
            //
            // btnRunPath
            //
            this.btnRunPath.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnRunPath.Location = new System.Drawing.Point(188, 14);
            this.btnRunPath.Name = "btnRunPath";
            this.btnRunPath.Size = new System.Drawing.Size(200, 36);
            this.btnRunPath.Text = "▶ Run migration path";
            this.btnRunPath.Click += new System.EventHandler(this.btnRunPath_Click);
            //
            // btnCancel
            //
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(396, 14);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 36);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnRollback
            //
            this.btnRollback.Location = new System.Drawing.Point(506, 14);
            this.btnRollback.Name = "btnRollback";
            this.btnRollback.Size = new System.Drawing.Size(190, 36);
            this.btnRollback.Text = "↶ Roll back failed step";
            this.btnRollback.Click += new System.EventHandler(this.btnRollback_Click);
            //
            // btnMapTheme
            //
            this.btnMapTheme.Location = new System.Drawing.Point(704, 14);
            this.btnMapTheme.Name = "btnMapTheme";
            this.btnMapTheme.Size = new System.Drawing.Size(160, 36);
            this.btnMapTheme.Text = "Map theme mixin";
            this.btnMapTheme.Click += new System.EventHandler(this.btnMapTheme_Click);
            //
            // btnWorkOrders
            //
            this.btnWorkOrders.Location = new System.Drawing.Point(872, 14);
            this.btnWorkOrders.Name = "btnWorkOrders";
            this.btnWorkOrders.Size = new System.Drawing.Size(108, 36);
            this.btnWorkOrders.Text = "Work orders →";
            this.btnWorkOrders.Click += new System.EventHandler(this.btnWorkOrders_Click);
            //
            // tabDossier
            //
            this.tabDossier.Location = new System.Drawing.Point(20, 62);
            this.tabDossier.Name = "tabDossier";
            this.tabDossier.SelectedIndex = 0;
            this.tabDossier.Size = new System.Drawing.Size(960, 346);
            this.tabDossier.TabPages.AddRange(new Wisej.Web.TabPage[] {
            this.tabPageDossier,
            this.tabPageInventory,
            this.tabPageCompatibility,
            this.tabPageSteps,
            this.tabPageFlows,
            this.tabPageMemo});
            //
            // tabPageDossier
            //
            this.tabPageDossier.Controls.Add(this.dgvDossier);
            this.tabPageDossier.Name = "tabPageDossier";
            this.tabPageDossier.Text = "Dossier";
            //
            // dgvDossier
            //
            this.dgvDossier.AllowUserToAddRows = false;
            this.dgvDossier.AllowUserToDeleteRows = false;
            this.dgvDossier.AutoGenerateColumns = false;
            this.dgvDossier.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDossier.BackColor = System.Drawing.Color.White;
            this.dgvDossier.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colArea,
            this.colCurrent,
            this.colTarget,
            this.colRisk,
            this.colProof,
            this.colRollback,
            this.colRowState});
            this.dgvDossier.Dock = Wisej.Web.DockStyle.Fill;
            this.dgvDossier.MultiSelect = false;
            this.dgvDossier.Name = "dgvDossier";
            this.dgvDossier.ReadOnly = true;
            this.dgvDossier.RowHeadersVisible = false;
            this.dgvDossier.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            //
            // colArea / colCurrent / colTarget / colRisk / colProof / colRollback / colRowState
            //
            this.colArea.DataPropertyName = "Area";
            this.colArea.FillWeight = 17F;
            this.colArea.HeaderText = "Area";
            this.colArea.Name = "colArea";
            this.colArea.ReadOnly = true;
            this.colCurrent.DataPropertyName = "Current";
            this.colCurrent.FillWeight = 16F;
            this.colCurrent.HeaderText = "Current";
            this.colCurrent.Name = "colCurrent";
            this.colCurrent.ReadOnly = true;
            this.colTarget.DataPropertyName = "Target";
            this.colTarget.FillWeight = 15F;
            this.colTarget.HeaderText = "Target";
            this.colTarget.Name = "colTarget";
            this.colTarget.ReadOnly = true;
            this.colRisk.DataPropertyName = "RiskCode";
            this.colRisk.FillWeight = 6F;
            this.colRisk.HeaderText = "Risk";
            this.colRisk.Name = "colRisk";
            this.colRisk.ReadOnly = true;
            this.colProof.DataPropertyName = "RegressionProof";
            this.colProof.FillWeight = 17F;
            this.colProof.HeaderText = "Regression proof";
            this.colProof.Name = "colProof";
            this.colProof.ReadOnly = true;
            this.colRollback.DataPropertyName = "RollbackPoint";
            this.colRollback.FillWeight = 15F;
            this.colRollback.HeaderText = "Rollback";
            this.colRollback.Name = "colRollback";
            this.colRollback.ReadOnly = true;
            this.colRowState.DataPropertyName = "StateText";
            this.colRowState.FillWeight = 14F;
            this.colRowState.HeaderText = "State";
            this.colRowState.Name = "colRowState";
            this.colRowState.ReadOnly = true;
            //
            // tabPageInventory
            //
            this.tabPageInventory.Controls.Add(this.dgvInventory);
            this.tabPageInventory.Name = "tabPageInventory";
            this.tabPageInventory.Text = "Inventory";
            //
            // dgvInventory
            //
            this.dgvInventory.AllowUserToAddRows = false;
            this.dgvInventory.AllowUserToDeleteRows = false;
            this.dgvInventory.AutoGenerateColumns = false;
            this.dgvInventory.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInventory.BackColor = System.Drawing.Color.White;
            this.dgvInventory.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colCategory,
            this.colItem,
            this.colCurrentState,
            this.colTargetState,
            this.colNote});
            this.dgvInventory.Dock = Wisej.Web.DockStyle.Fill;
            this.dgvInventory.MultiSelect = false;
            this.dgvInventory.Name = "dgvInventory";
            this.dgvInventory.ReadOnly = true;
            this.dgvInventory.RowHeadersVisible = false;
            this.dgvInventory.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            //
            // colCategory / colItem / colCurrentState / colTargetState / colNote
            //
            this.colCategory.DataPropertyName = "CategoryText";
            this.colCategory.FillWeight = 11F;
            this.colCategory.HeaderText = "Category";
            this.colCategory.Name = "colCategory";
            this.colCategory.ReadOnly = true;
            this.colItem.DataPropertyName = "Name";
            this.colItem.FillWeight = 20F;
            this.colItem.HeaderText = "Item";
            this.colItem.Name = "colItem";
            this.colItem.ReadOnly = true;
            this.colCurrentState.DataPropertyName = "CurrentState";
            this.colCurrentState.FillWeight = 22F;
            this.colCurrentState.HeaderText = "Current state";
            this.colCurrentState.Name = "colCurrentState";
            this.colCurrentState.ReadOnly = true;
            this.colTargetState.DataPropertyName = "TargetState";
            this.colTargetState.FillWeight = 24F;
            this.colTargetState.HeaderText = "Target state";
            this.colTargetState.Name = "colTargetState";
            this.colTargetState.ReadOnly = true;
            this.colNote.DataPropertyName = "Note";
            this.colNote.FillWeight = 23F;
            this.colNote.HeaderText = "Note";
            this.colNote.Name = "colNote";
            this.colNote.ReadOnly = true;
            //
            // tabPageCompatibility
            //
            this.tabPageCompatibility.Controls.Add(this.dgvCompatibility);
            this.tabPageCompatibility.Name = "tabPageCompatibility";
            this.tabPageCompatibility.Text = "Compatibility / risk";
            //
            // dgvCompatibility
            //
            this.dgvCompatibility.AllowUserToAddRows = false;
            this.dgvCompatibility.AllowUserToDeleteRows = false;
            this.dgvCompatibility.AutoGenerateColumns = false;
            this.dgvCompatibility.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCompatibility.BackColor = System.Drawing.Color.White;
            this.dgvCompatibility.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colComponent,
            this.colFromVersion,
            this.colToVersion,
            this.colSupported,
            this.colMatrixRisk,
            this.colMitigation});
            this.dgvCompatibility.Dock = Wisej.Web.DockStyle.Fill;
            this.dgvCompatibility.MultiSelect = false;
            this.dgvCompatibility.Name = "dgvCompatibility";
            this.dgvCompatibility.ReadOnly = true;
            this.dgvCompatibility.RowHeadersVisible = false;
            this.dgvCompatibility.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            //
            // colComponent / colFromVersion / colToVersion / colSupported / colMatrixRisk / colMitigation
            //
            this.colComponent.DataPropertyName = "Component";
            this.colComponent.FillWeight = 21F;
            this.colComponent.HeaderText = "Component";
            this.colComponent.Name = "colComponent";
            this.colComponent.ReadOnly = true;
            this.colFromVersion.DataPropertyName = "CurrentVersion";
            this.colFromVersion.FillWeight = 14F;
            this.colFromVersion.HeaderText = "Current";
            this.colFromVersion.Name = "colFromVersion";
            this.colFromVersion.ReadOnly = true;
            this.colToVersion.DataPropertyName = "TargetVersion";
            this.colToVersion.FillWeight = 20F;
            this.colToVersion.HeaderText = "Target";
            this.colToVersion.Name = "colToVersion";
            this.colToVersion.ReadOnly = true;
            this.colSupported.DataPropertyName = "SupportedText";
            this.colSupported.FillWeight = 12F;
            this.colSupported.HeaderText = "Supported";
            this.colSupported.Name = "colSupported";
            this.colSupported.ReadOnly = true;
            this.colMatrixRisk.DataPropertyName = "RiskCode";
            this.colMatrixRisk.FillWeight = 6F;
            this.colMatrixRisk.HeaderText = "Risk";
            this.colMatrixRisk.Name = "colMatrixRisk";
            this.colMatrixRisk.ReadOnly = true;
            this.colMitigation.DataPropertyName = "Mitigation";
            this.colMitigation.FillWeight = 27F;
            this.colMitigation.HeaderText = "Mitigation";
            this.colMitigation.Name = "colMitigation";
            this.colMitigation.ReadOnly = true;
            //
            // tabPageSteps
            //
            this.tabPageSteps.Controls.Add(this.dgvSteps);
            this.tabPageSteps.Name = "tabPageSteps";
            this.tabPageSteps.Text = "Incremental path";
            //
            // dgvSteps
            //
            this.dgvSteps.AllowUserToAddRows = false;
            this.dgvSteps.AllowUserToDeleteRows = false;
            this.dgvSteps.AutoGenerateColumns = false;
            this.dgvSteps.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSteps.BackColor = System.Drawing.Color.White;
            this.dgvSteps.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colStepNumber,
            this.colStepName,
            this.colStepCheck,
            this.colFallbackPoint,
            this.colStepState,
            this.colStepDetail});
            this.dgvSteps.Dock = Wisej.Web.DockStyle.Fill;
            this.dgvSteps.MultiSelect = false;
            this.dgvSteps.Name = "dgvSteps";
            this.dgvSteps.ReadOnly = true;
            this.dgvSteps.RowHeadersVisible = false;
            this.dgvSteps.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            //
            // colStepNumber / colStepName / colStepCheck / colFallbackPoint / colStepState / colStepDetail
            //
            this.colStepNumber.DataPropertyName = "Number";
            this.colStepNumber.FillWeight = 4F;
            this.colStepNumber.HeaderText = "#";
            this.colStepNumber.Name = "colStepNumber";
            this.colStepNumber.ReadOnly = true;
            this.colStepName.DataPropertyName = "Name";
            this.colStepName.FillWeight = 15F;
            this.colStepName.HeaderText = "Step";
            this.colStepName.Name = "colStepName";
            this.colStepName.ReadOnly = true;
            this.colStepCheck.DataPropertyName = "Check";
            this.colStepCheck.FillWeight = 22F;
            this.colStepCheck.HeaderText = "Check that proves it";
            this.colStepCheck.Name = "colStepCheck";
            this.colStepCheck.ReadOnly = true;
            this.colFallbackPoint.DataPropertyName = "FallbackPoint";
            this.colFallbackPoint.FillWeight = 15F;
            this.colFallbackPoint.HeaderText = "Fallback point";
            this.colFallbackPoint.Name = "colFallbackPoint";
            this.colFallbackPoint.ReadOnly = true;
            this.colStepState.DataPropertyName = "StateGlyph";
            this.colStepState.FillWeight = 5F;
            this.colStepState.HeaderText = "";
            this.colStepState.Name = "colStepState";
            this.colStepState.ReadOnly = true;
            this.colStepDetail.DataPropertyName = "Detail";
            this.colStepDetail.FillWeight = 39F;
            this.colStepDetail.HeaderText = "Last outcome";
            this.colStepDetail.Name = "colStepDetail";
            this.colStepDetail.ReadOnly = true;
            //
            // tabPageFlows
            //
            this.tabPageFlows.Controls.Add(this.dgvFlows);
            this.tabPageFlows.Name = "tabPageFlows";
            this.tabPageFlows.Text = "Regression plan (10 flows)";
            //
            // dgvFlows
            //
            this.dgvFlows.AllowUserToAddRows = false;
            this.dgvFlows.AllowUserToDeleteRows = false;
            this.dgvFlows.AutoGenerateColumns = false;
            this.dgvFlows.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFlows.BackColor = System.Drawing.Color.White;
            this.dgvFlows.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colFlowId,
            this.colFlowName,
            this.colFlowScreen,
            this.colFlowCategory,
            this.colFlowExpected,
            this.colFlowResult});
            this.dgvFlows.Dock = Wisej.Web.DockStyle.Fill;
            this.dgvFlows.MultiSelect = false;
            this.dgvFlows.Name = "dgvFlows";
            this.dgvFlows.ReadOnly = true;
            this.dgvFlows.RowHeadersVisible = false;
            this.dgvFlows.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            //
            // colFlowId / colFlowName / colFlowScreen / colFlowCategory / colFlowExpected / colFlowResult
            //
            this.colFlowId.DataPropertyName = "Id";
            this.colFlowId.FillWeight = 4F;
            this.colFlowId.HeaderText = "#";
            this.colFlowId.Name = "colFlowId";
            this.colFlowId.ReadOnly = true;
            this.colFlowName.DataPropertyName = "Name";
            this.colFlowName.FillWeight = 21F;
            this.colFlowName.HeaderText = "Flow";
            this.colFlowName.Name = "colFlowName";
            this.colFlowName.ReadOnly = true;
            this.colFlowScreen.DataPropertyName = "Screen";
            this.colFlowScreen.FillWeight = 13F;
            this.colFlowScreen.HeaderText = "Screen";
            this.colFlowScreen.Name = "colFlowScreen";
            this.colFlowScreen.ReadOnly = true;
            this.colFlowCategory.DataPropertyName = "CategoryText";
            this.colFlowCategory.FillWeight = 11F;
            this.colFlowCategory.HeaderText = "Category";
            this.colFlowCategory.Name = "colFlowCategory";
            this.colFlowCategory.ReadOnly = true;
            this.colFlowExpected.DataPropertyName = "Expected";
            this.colFlowExpected.FillWeight = 25F;
            this.colFlowExpected.HeaderText = "Expected (3.5 behavior)";
            this.colFlowExpected.Name = "colFlowExpected";
            this.colFlowExpected.ReadOnly = true;
            this.colFlowResult.DataPropertyName = "ResultText";
            this.colFlowResult.FillWeight = 26F;
            this.colFlowResult.HeaderText = "Result on 4";
            this.colFlowResult.Name = "colFlowResult";
            this.colFlowResult.ReadOnly = true;
            //
            // tabPageMemo
            //
            this.tabPageMemo.Controls.Add(this.txtMemo);
            this.tabPageMemo.Name = "tabPageMemo";
            this.tabPageMemo.Text = "Decision memo";
            //
            // txtMemo
            //
            this.txtMemo.Dock = Wisej.Web.DockStyle.Fill;
            this.txtMemo.Font = new System.Drawing.Font("monospace", 9F);
            this.txtMemo.Multiline = true;
            this.txtMemo.Name = "txtMemo";
            this.txtMemo.ReadOnly = true;
            this.txtMemo.ScrollBars = Wisej.Web.ScrollBars.Both;
            this.txtMemo.Text = "Build the dossier first — the memo is written from the dossier, the steps and the last harness run.";
            //
            // lblBanner
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(20, 416);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(960, 36);
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
            this.lblStatusBar.Location = new System.Drawing.Point(20, 458);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(960, 36);
            this.lblStatusBar.Text = "Regression harness: not run";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // MigrationDossierPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlDossier);
            this.Name = "MigrationDossierPage";
            this.Size = new System.Drawing.Size(1048, 600);
            this.Text = "EnterpriseOps — Migration Dossier";
            this.Load += new System.EventHandler(this.MigrationDossierPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlDossier.ResumeLayout(false);
            this.tabDossier.ResumeLayout(false);
            this.tabPageDossier.ResumeLayout(false);
            this.tabPageInventory.ResumeLayout(false);
            this.tabPageCompatibility.ResumeLayout(false);
            this.tabPageSteps.ResumeLayout(false);
            this.tabPageFlows.ResumeLayout(false);
            this.tabPageMemo.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Panel pnlDossier;
        private Wisej.Web.Button btnBuildDossier;
        private Wisej.Web.Button btnRunPath;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Button btnRollback;
        private Wisej.Web.Button btnMapTheme;
        private Wisej.Web.Button btnWorkOrders;
        private Wisej.Web.TabControl tabDossier;
        private Wisej.Web.TabPage tabPageDossier;
        private Wisej.Web.DataGridView dgvDossier;
        private Wisej.Web.DataGridViewTextBoxColumn colArea;
        private Wisej.Web.DataGridViewTextBoxColumn colCurrent;
        private Wisej.Web.DataGridViewTextBoxColumn colTarget;
        private Wisej.Web.DataGridViewTextBoxColumn colRisk;
        private Wisej.Web.DataGridViewTextBoxColumn colProof;
        private Wisej.Web.DataGridViewTextBoxColumn colRollback;
        private Wisej.Web.DataGridViewTextBoxColumn colRowState;
        private Wisej.Web.TabPage tabPageInventory;
        private Wisej.Web.DataGridView dgvInventory;
        private Wisej.Web.DataGridViewTextBoxColumn colCategory;
        private Wisej.Web.DataGridViewTextBoxColumn colItem;
        private Wisej.Web.DataGridViewTextBoxColumn colCurrentState;
        private Wisej.Web.DataGridViewTextBoxColumn colTargetState;
        private Wisej.Web.DataGridViewTextBoxColumn colNote;
        private Wisej.Web.TabPage tabPageCompatibility;
        private Wisej.Web.DataGridView dgvCompatibility;
        private Wisej.Web.DataGridViewTextBoxColumn colComponent;
        private Wisej.Web.DataGridViewTextBoxColumn colFromVersion;
        private Wisej.Web.DataGridViewTextBoxColumn colToVersion;
        private Wisej.Web.DataGridViewTextBoxColumn colSupported;
        private Wisej.Web.DataGridViewTextBoxColumn colMatrixRisk;
        private Wisej.Web.DataGridViewTextBoxColumn colMitigation;
        private Wisej.Web.TabPage tabPageSteps;
        private Wisej.Web.DataGridView dgvSteps;
        private Wisej.Web.DataGridViewTextBoxColumn colStepNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colStepName;
        private Wisej.Web.DataGridViewTextBoxColumn colStepCheck;
        private Wisej.Web.DataGridViewTextBoxColumn colFallbackPoint;
        private Wisej.Web.DataGridViewTextBoxColumn colStepState;
        private Wisej.Web.DataGridViewTextBoxColumn colStepDetail;
        private Wisej.Web.TabPage tabPageFlows;
        private Wisej.Web.DataGridView dgvFlows;
        private Wisej.Web.DataGridViewTextBoxColumn colFlowId;
        private Wisej.Web.DataGridViewTextBoxColumn colFlowName;
        private Wisej.Web.DataGridViewTextBoxColumn colFlowScreen;
        private Wisej.Web.DataGridViewTextBoxColumn colFlowCategory;
        private Wisej.Web.DataGridViewTextBoxColumn colFlowExpected;
        private Wisej.Web.DataGridViewTextBoxColumn colFlowResult;
        private Wisej.Web.TabPage tabPageMemo;
        private Wisej.Web.TextBox txtMemo;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatusBar;
    }
}
