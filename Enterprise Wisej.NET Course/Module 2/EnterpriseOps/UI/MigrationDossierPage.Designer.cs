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
            this.lblTenant = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.lblCorrelation = new Wisej.Web.Label();
            this.pnlDossier = new Wisej.Web.Panel();
            this.btnBuildDossier = new Wisej.Web.Button();
            this.btnRunPath = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.lblStatus = new Wisej.Web.Label();
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
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.lblTraceFooter = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnRollback = new Wisej.Web.Button();
            this.btnMapTheme = new Wisej.Web.Button();
            this.btnSwitchUser = new Wisej.Web.Button();
            this.btnWorkOrders = new Wisej.Web.Button();
            this.btnMemo = new Wisej.Web.Button();
            this.btnReset = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlDossier.SuspendLayout();
            this.tabDossier.SuspendLayout();
            this.tabPageDossier.SuspendLayout();
            this.tabPageInventory.SuspendLayout();
            this.tabPageCompatibility.SuspendLayout();
            this.tabPageSteps.SuspendLayout();
            this.tabPageFlows.SuspendLayout();
            this.tabPageMemo.SuspendLayout();
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
            this.lblTitle.Size = new System.Drawing.Size(640, 44);
            this.lblTitle.Text = "EnterpriseOps — Migration Dossier · TicketOps Console → EnterpriseOps baseline";
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
            this.lblTenant.Size = new System.Drawing.Size(160, 44);
            this.lblTenant.Text = "tenant: contoso";
            this.lblTenant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUser
            //
            this.lblUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(870, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(280, 44);
            this.lblUser.Text = "Signed in: ana.ops · Manager";
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
            // pnlDossier  (the dossier card: toolbar row, the six dossier tabs, the banner, the dark footer)
            //
            this.pnlDossier.BackColor = System.Drawing.Color.White;
            this.pnlDossier.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlDossier.Controls.Add(this.btnBuildDossier);
            this.pnlDossier.Controls.Add(this.btnRunPath);
            this.pnlDossier.Controls.Add(this.btnCancel);
            this.pnlDossier.Controls.Add(this.lblStatus);
            this.pnlDossier.Controls.Add(this.tabDossier);
            this.pnlDossier.Controls.Add(this.lblBanner);
            this.pnlDossier.Controls.Add(this.lblStatusBar);
            this.pnlDossier.Location = new System.Drawing.Point(24, 64);
            this.pnlDossier.Name = "pnlDossier";
            this.pnlDossier.Size = new System.Drawing.Size(860, 512);
            //
            // btnBuildDossier  (success path: the assessment service computes risk for every dossier row)
            //
            this.btnBuildDossier.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnBuildDossier.Location = new System.Drawing.Point(20, 14);
            this.btnBuildDossier.Name = "btnBuildDossier";
            this.btnBuildDossier.Size = new System.Drawing.Size(170, 36);
            this.btnBuildDossier.Text = "Build dossier";
            this.btnBuildDossier.ToolTipText = "btnBuildDossier_Click → await _assessment.AssessAsync(CurrentContext). Inventory before action: 7 areas, 14 inventory items, 9 compatibility entries, risk computed by one rule.";
            this.btnBuildDossier.Click += new System.EventHandler(this.btnBuildDossier_Click);
            //
            // btnRunPath  (progress path: seven verifiable steps, each with a fallback point)
            //
            this.btnRunPath.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnRunPath.Location = new System.Drawing.Point(198, 14);
            this.btnRunPath.Name = "btnRunPath";
            this.btnRunPath.Size = new System.Drawing.Size(210, 36);
            this.btnRunPath.Text = "▶ Run migration path";
            this.btnRunPath.ToolTipText = "btnRunPath_Click → await _workflow.RunAsync(...). Steps 1–3 pass; step 4 \"Check themes\" fails on the visual diff because the 3.x theme was never mapped.";
            this.btnRunPath.Click += new System.EventHandler(this.btnRunPath_Click);
            //
            // btnCancel  (stays enabled while the path runs — the CancellationTokenSource lives in an instance field)
            //
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(416, 14);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 36);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.ToolTipText = "Cancels the running migration path through the CancellationTokenSource kept in an instance field.";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.lblStatus.Location = new System.Drawing.Point(520, 19);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(320, 26);
            this.lblStatus.Text = "● dossier not built";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // tabDossier  (the five deliverable tables plus the decision memo)
            //
            this.tabDossier.Location = new System.Drawing.Point(20, 62);
            this.tabDossier.Name = "tabDossier";
            this.tabDossier.SelectedIndex = 0;
            this.tabDossier.Size = new System.Drawing.Size(820, 346);
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
            // dgvDossier  (the migration dossier: what changes · what it risks · how it is proven · how to get back)
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
            // dgvInventory  (current-state and target-state inventory, side by side)
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
            // dgvCompatibility  (framework × Wisej.NET × packages — decided before any code changes)
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
            // dgvSteps  (the seven verifiable steps, each with its check and its fallback point)
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
            // dgvFlows  (the ten key flows — they protect behavior, not compilation)
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
            // txtMemo  (written by the service from evidence — never typed here)
            //
            this.txtMemo.Dock = Wisej.Web.DockStyle.Fill;
            this.txtMemo.Font = new System.Drawing.Font("monospace", 9F);
            this.txtMemo.Multiline = true;
            this.txtMemo.Name = "txtMemo";
            this.txtMemo.ReadOnly = true;
            this.txtMemo.ScrollBars = Wisej.Web.ScrollBars.Both;
            this.txtMemo.Text = "Click \"Write decision memo\" — the memo is generated from the dossier, the steps and the last harness run.";
            //
            // lblBanner  (failure / recovery banner; hidden until something needs saying)
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(20, 416);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(820, 36);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatusBar  (the dark footer: the harness verdict, exactly as the video prints it)
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(20, 458);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(820, 36);
            this.lblStatusBar.Text = "Regression harness: not run";
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
            this.pnlTrace.Location = new System.Drawing.Point(900, 64);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(424, 512);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(392, 28);
            this.lblTraceTitle.Text = "Server · live activity trace";
            //
            // lstTrace
            //
            this.lstTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTrace.Location = new System.Drawing.Point(16, 46);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(392, 422);
            //
            // lblTraceFooter
            //
            this.lblTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceFooter.AutoSize = false;
            this.lblTraceFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTraceFooter.Location = new System.Drawing.Point(16, 474);
            this.lblTraceFooter.Name = "lblTraceFooter";
            this.lblTraceFooter.Size = new System.Drawing.Size(392, 28);
            this.lblTraceFooter.Text = "UI → · Service: · Data: · Security: · Job:   — the buffer survives navigation (flow 9)";
            //
            // pnlActions  (bottom bar: recovery, failure path, navigation, memo, reset, clear)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.Controls.Add(this.btnRollback);
            this.pnlActions.Controls.Add(this.btnMapTheme);
            this.pnlActions.Controls.Add(this.btnSwitchUser);
            this.pnlActions.Controls.Add(this.btnWorkOrders);
            this.pnlActions.Controls.Add(this.btnMemo);
            this.pnlActions.Controls.Add(this.btnReset);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Location = new System.Drawing.Point(24, 592);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1300, 44);
            //
            // btnRollback  (recovery 1: back to the failed step's fallback point)
            //
            this.btnRollback.Location = new System.Drawing.Point(0, 4);
            this.btnRollback.Name = "btnRollback";
            this.btnRollback.Size = new System.Drawing.Size(210, 36);
            this.btnRollback.Text = "↶ Roll back failed step";
            this.btnRollback.ToolTipText = "btnRollback_Click → await _workflow.RollbackAsync(...). Restores the fallback point of the failed step only; steps after it are never started.";
            this.btnRollback.Click += new System.EventHandler(this.btnRollback_Click);
            //
            // btnMapTheme  (recovery 2: resource mapping — port the 3.x theme to a 4.x mixin)
            //
            this.btnMapTheme.Location = new System.Drawing.Point(218, 4);
            this.btnMapTheme.Name = "btnMapTheme";
            this.btnMapTheme.Size = new System.Drawing.Size(190, 36);
            this.btnMapTheme.Text = "Map theme mixin";
            this.btnMapTheme.ToolTipText = "btnMapTheme_Click → _workflow.MapThemeMixin(...). Refused while step 4 is Failed: fix on the fallback point, never on the broken build.";
            this.btnMapTheme.Click += new System.EventHandler(this.btnMapTheme_Click);
            //
            // btnSwitchUser  (failure path: a Technician may not change the build for everyone)
            //
            this.btnSwitchUser.Location = new System.Drawing.Point(416, 4);
            this.btnSwitchUser.Name = "btnSwitchUser";
            this.btnSwitchUser.Size = new System.Drawing.Size(230, 36);
            this.btnSwitchUser.Text = "Fail: map theme as ben.tech";
            this.btnSwitchUser.ToolTipText = "Signs in as ben.tech (Technician) and asks for the same mapping — PermissionService denies it server-side. Click again to come back as ana.ops.";
            this.btnSwitchUser.Click += new System.EventHandler(this.btnSwitchUser_Click);
            //
            // btnWorkOrders  (navigation: the migrated screen, painted from the current theme map)
            //
            this.btnWorkOrders.Location = new System.Drawing.Point(654, 4);
            this.btnWorkOrders.Name = "btnWorkOrders";
            this.btnWorkOrders.Size = new System.Drawing.Size(210, 36);
            this.btnWorkOrders.Text = "Open WorkOrdersPage →";
            this.btnWorkOrders.ToolTipText = "The migrated TicketOps screen. It paints itself from ThemeService.Current, so the visual diff is visible, not described.";
            this.btnWorkOrders.Click += new System.EventHandler(this.btnWorkOrders_Click);
            //
            // btnMemo
            //
            this.btnMemo.Location = new System.Drawing.Point(872, 4);
            this.btnMemo.Name = "btnMemo";
            this.btnMemo.Size = new System.Drawing.Size(190, 36);
            this.btnMemo.Text = "Write decision memo";
            this.btnMemo.ToolTipText = "btnMemo_Click → _assessment.BuildDecisionMemo(...). Written from the evidence the run produced — steps, harness verdict, theme diff.";
            this.btnMemo.Click += new System.EventHandler(this.btnMemo_Click);
            //
            // btnReset
            //
            this.btnReset.Location = new System.Drawing.Point(1070, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(110, 36);
            this.btnReset.Text = "Reset";
            this.btnReset.ToolTipText = "Back to the state right after the package upgrade: 7 steps pending, flows not run, dossier rows open, theme unmapped.";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
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
            // MigrationDossierPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlDossier);
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlActions);
            this.Name = "MigrationDossierPage";
            this.Size = new System.Drawing.Size(1348, 680);
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
        private Wisej.Web.Panel pnlDossier;
        private Wisej.Web.Button btnBuildDossier;
        private Wisej.Web.Button btnRunPath;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Label lblStatus;
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
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Label lblTraceFooter;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnRollback;
        private Wisej.Web.Button btnMapTheme;
        private Wisej.Web.Button btnSwitchUser;
        private Wisej.Web.Button btnWorkOrders;
        private Wisej.Web.Button btnMemo;
        private Wisej.Web.Button btnReset;
        private Wisej.Web.Button btnClearTrace;
    }
}
