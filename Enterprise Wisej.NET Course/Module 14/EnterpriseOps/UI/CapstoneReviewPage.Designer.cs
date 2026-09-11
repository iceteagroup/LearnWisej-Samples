namespace EnterpriseOps.UI
{
    partial class CapstoneReviewPage
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
            this.btnDashboard = new Wisej.Web.Button();
            this.tabCapstone = new Wisej.Web.TabControl();
            this.tabPackage = new Wisej.Web.TabPage();
            this.btnVerifyPackage = new Wisej.Web.Button();
            this.lblPackageStatus = new Wisej.Web.Label();
            this.dgvPackage = new Wisej.Web.DataGridView();
            this.colDeliverable = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDeliverablePath = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDeliverableRequired = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDeliverableResult = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDeliverableDetail = new Wisej.Web.DataGridViewTextBoxColumn();
            this.tabDocs = new Wisej.Web.TabPage();
            this.lblDocsStatus = new Wisej.Web.Label();
            this.dgvDocs = new Wisej.Web.DataGridView();
            this.colDocId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDocTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDocPurpose = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDocPath = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDocModule = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDocVerified = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDocResolves = new Wisej.Web.DataGridViewTextBoxColumn();
            this.tabPrompts = new Wisej.Web.TabPage();
            this.lblPromptsStatus = new Wisej.Web.Label();
            this.lstPrompts = new Wisej.Web.ListBox();
            this.txtPrompt = new Wisej.Web.TextBox();
            this.tabReview = new Wisej.Web.TabPage();
            this.btnLoadDraft = new Wisej.Web.Button();
            this.btnLoadFixed = new Wisej.Web.Button();
            this.btnReviewGeneratedCode = new Wisej.Web.Button();
            this.btnSignDecision = new Wisej.Web.Button();
            this.lblVerdict = new Wisej.Web.Label();
            this.lblReviewSource = new Wisej.Web.Label();
            this.txtGeneratedCode = new Wisej.Web.TextBox();
            this.dgvFindings = new Wisej.Web.DataGridView();
            this.colRule = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colSeverity = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colLine = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colFinding = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colEvidence = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colFix = new Wisej.Web.DataGridViewTextBoxColumn();
            this.tabChecklist = new Wisej.Web.TabPage();
            this.lblChecklistTitle = new Wisej.Web.Label();
            this.dgvChecklist = new Wisej.Web.DataGridView();
            this.colRuleId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colRuleQuestion = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colRuleSeverity = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colRuleReference = new Wisej.Web.DataGridViewTextBoxColumn();
            this.tabDecisions = new Wisej.Web.TabPage();
            this.lblDecisionsStatus = new Wisej.Web.Label();
            this.dgvDecisions = new Wisej.Web.DataGridView();
            this.colDecidedAt = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDecisionPr = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDecisionVerdict = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDecisionAuthor = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDecisionReviewer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDecisionReason = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlHeader.SuspendLayout();
            this.tabCapstone.SuspendLayout();
            this.tabPackage.SuspendLayout();
            this.tabDocs.SuspendLayout();
            this.tabPrompts.SuspendLayout();
            this.tabReview.SuspendLayout();
            this.tabChecklist.SuspendLayout();
            this.tabDecisions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.btnDashboard);
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
            this.lblTitle.Text = "EnterpriseOps — Capstone review";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnDashboard
            //
            this.btnDashboard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnDashboard.Location = new System.Drawing.Point(768, 6);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(148, 32);
            this.btnDashboard.Text = "← Command Center";
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            //
            // tabCapstone
            //
            this.tabCapstone.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.tabCapstone.Controls.Add(this.tabPackage);
            this.tabCapstone.Controls.Add(this.tabPrompts);
            this.tabCapstone.Controls.Add(this.tabChecklist);
            this.tabCapstone.Controls.Add(this.tabReview);
            this.tabCapstone.Controls.Add(this.tabDocs);
            this.tabCapstone.Controls.Add(this.tabDecisions);
            this.tabCapstone.Location = new System.Drawing.Point(16, 56);
            this.tabCapstone.Name = "tabCapstone";
            this.tabCapstone.SelectedIndex = 0;
            this.tabCapstone.Size = new System.Drawing.Size(900, 560);
            //
            // tabPackage
            //
            this.tabPackage.BackColor = System.Drawing.Color.White;
            this.tabPackage.Controls.Add(this.btnVerifyPackage);
            this.tabPackage.Controls.Add(this.lblPackageStatus);
            this.tabPackage.Controls.Add(this.dgvPackage);
            this.tabPackage.Name = "tabPackage";
            this.tabPackage.Text = "Capstone package";
            //
            // btnVerifyPackage
            //
            this.btnVerifyPackage.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnVerifyPackage.Location = new System.Drawing.Point(12, 8);
            this.btnVerifyPackage.Name = "btnVerifyPackage";
            this.btnVerifyPackage.Size = new System.Drawing.Size(170, 30);
            this.btnVerifyPackage.Text = "✓ Verify package";
            this.btnVerifyPackage.Click += new System.EventHandler(this.btnVerifyPackage_Click);
            //
            // lblPackageStatus
            //
            this.lblPackageStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblPackageStatus.AutoSize = false;
            this.lblPackageStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblPackageStatus.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblPackageStatus.Location = new System.Drawing.Point(194, 10);
            this.lblPackageStatus.Name = "lblPackageStatus";
            this.lblPackageStatus.Size = new System.Drawing.Size(678, 26);
            this.lblPackageStatus.Text = "● not verified";
            //
            // dgvPackage
            //
            this.dgvPackage.AllowUserToAddRows = false;
            this.dgvPackage.AllowUserToDeleteRows = false;
            this.dgvPackage.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvPackage.AutoGenerateColumns = false;
            this.dgvPackage.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPackage.BackColor = System.Drawing.Color.White;
            this.dgvPackage.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colDeliverable,
            this.colDeliverablePath,
            this.colDeliverableRequired,
            this.colDeliverableResult,
            this.colDeliverableDetail});
            this.dgvPackage.Location = new System.Drawing.Point(12, 46);
            this.dgvPackage.MultiSelect = false;
            this.dgvPackage.Name = "dgvPackage";
            this.dgvPackage.ReadOnly = true;
            this.dgvPackage.RowHeadersVisible = false;
            this.dgvPackage.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPackage.Size = new System.Drawing.Size(860, 464);
            this.colDeliverable.DataPropertyName = "Deliverable";
            this.colDeliverable.HeaderText = "Deliverable";
            this.colDeliverable.Name = "colDeliverable";
            this.colDeliverable.Width = 250;
            this.colDeliverablePath.DataPropertyName = "Path";
            this.colDeliverablePath.HeaderText = "File";
            this.colDeliverablePath.Name = "colDeliverablePath";
            this.colDeliverablePath.Width = 250;
            this.colDeliverableRequired.DataPropertyName = "Required";
            this.colDeliverableRequired.HeaderText = "Required";
            this.colDeliverableRequired.Name = "colDeliverableRequired";
            this.colDeliverableRequired.Width = 80;
            this.colDeliverableResult.DataPropertyName = "Passed";
            this.colDeliverableResult.HeaderText = "Pass";
            this.colDeliverableResult.Name = "colDeliverableResult";
            this.colDeliverableResult.Width = 60;
            this.colDeliverableDetail.DataPropertyName = "Detail";
            this.colDeliverableDetail.HeaderText = "Evidence";
            this.colDeliverableDetail.Name = "colDeliverableDetail";
            this.colDeliverableDetail.Width = 380;
            //
            // tabPrompts
            //
            this.tabPrompts.BackColor = System.Drawing.Color.White;
            this.tabPrompts.Controls.Add(this.lblPromptsStatus);
            this.tabPrompts.Controls.Add(this.lstPrompts);
            this.tabPrompts.Controls.Add(this.txtPrompt);
            this.tabPrompts.Name = "tabPrompts";
            this.tabPrompts.Text = "AI prompt library";
            //
            // lblPromptsStatus
            //
            this.lblPromptsStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblPromptsStatus.AutoSize = false;
            this.lblPromptsStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblPromptsStatus.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblPromptsStatus.Location = new System.Drawing.Point(12, 10);
            this.lblPromptsStatus.Name = "lblPromptsStatus";
            this.lblPromptsStatus.Size = new System.Drawing.Size(860, 26);
            this.lblPromptsStatus.Text = "docs/PromptLibrary.md";
            //
            // lstPrompts
            //
            this.lstPrompts.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.lstPrompts.Location = new System.Drawing.Point(12, 42);
            this.lstPrompts.Name = "lstPrompts";
            this.lstPrompts.Size = new System.Drawing.Size(280, 468);
            this.lstPrompts.SelectedIndexChanged += new System.EventHandler(this.lstPrompts_SelectedIndexChanged);
            //
            // txtPrompt
            //
            this.txtPrompt.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtPrompt.Font = new System.Drawing.Font("monospace", 9F);
            this.txtPrompt.Location = new System.Drawing.Point(300, 42);
            this.txtPrompt.Multiline = true;
            this.txtPrompt.Name = "txtPrompt";
            this.txtPrompt.ReadOnly = true;
            this.txtPrompt.Size = new System.Drawing.Size(572, 468);
            //
            // tabChecklist
            //
            this.tabChecklist.BackColor = System.Drawing.Color.White;
            this.tabChecklist.Controls.Add(this.lblChecklistTitle);
            this.tabChecklist.Controls.Add(this.dgvChecklist);
            this.tabChecklist.Name = "tabChecklist";
            this.tabChecklist.Text = "Review checklist";
            //
            // lblChecklistTitle
            //
            this.lblChecklistTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblChecklistTitle.AutoSize = false;
            this.lblChecklistTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblChecklistTitle.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblChecklistTitle.Location = new System.Drawing.Point(12, 10);
            this.lblChecklistTitle.Name = "lblChecklistTitle";
            this.lblChecklistTitle.Size = new System.Drawing.Size(860, 26);
            this.lblChecklistTitle.Text = "docs/GeneratedCodeReviewChecklist.md";
            //
            // dgvChecklist
            //
            this.dgvChecklist.AllowUserToAddRows = false;
            this.dgvChecklist.AllowUserToDeleteRows = false;
            this.dgvChecklist.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvChecklist.AutoGenerateColumns = false;
            this.dgvChecklist.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvChecklist.BackColor = System.Drawing.Color.White;
            this.dgvChecklist.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colRuleId,
            this.colRuleQuestion,
            this.colRuleSeverity,
            this.colRuleReference});
            this.dgvChecklist.Location = new System.Drawing.Point(12, 42);
            this.dgvChecklist.MultiSelect = false;
            this.dgvChecklist.Name = "dgvChecklist";
            this.dgvChecklist.ReadOnly = true;
            this.dgvChecklist.RowHeadersVisible = false;
            this.dgvChecklist.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChecklist.Size = new System.Drawing.Size(860, 468);
            this.colRuleId.DataPropertyName = "Id";
            this.colRuleId.HeaderText = "#";
            this.colRuleId.Name = "colRuleId";
            this.colRuleId.Width = 50;
            this.colRuleQuestion.DataPropertyName = "Question";
            this.colRuleQuestion.HeaderText = "Question";
            this.colRuleQuestion.Name = "colRuleQuestion";
            this.colRuleQuestion.Width = 500;
            this.colRuleSeverity.DataPropertyName = "Severity";
            this.colRuleSeverity.HeaderText = "Severity";
            this.colRuleSeverity.Name = "colRuleSeverity";
            this.colRuleSeverity.Width = 90;
            this.colRuleReference.DataPropertyName = "DocReference";
            this.colRuleReference.HeaderText = "Reference";
            this.colRuleReference.Name = "colRuleReference";
            this.colRuleReference.Width = 320;
            //
            // tabReview
            //
            this.tabReview.BackColor = System.Drawing.Color.White;
            this.tabReview.Controls.Add(this.btnLoadDraft);
            this.tabReview.Controls.Add(this.btnLoadFixed);
            this.tabReview.Controls.Add(this.btnReviewGeneratedCode);
            this.tabReview.Controls.Add(this.btnSignDecision);
            this.tabReview.Controls.Add(this.lblVerdict);
            this.tabReview.Controls.Add(this.lblReviewSource);
            this.tabReview.Controls.Add(this.txtGeneratedCode);
            this.tabReview.Controls.Add(this.dgvFindings);
            this.tabReview.Name = "tabReview";
            this.tabReview.Text = "Generated-code review";
            //
            // btnLoadDraft
            //
            this.btnLoadDraft.Location = new System.Drawing.Point(12, 8);
            this.btnLoadDraft.Name = "btnLoadDraft";
            this.btnLoadDraft.Size = new System.Drawing.Size(150, 30);
            this.btnLoadDraft.Text = "Load AI draft #214";
            this.btnLoadDraft.Click += new System.EventHandler(this.btnLoadDraft_Click);
            //
            // btnLoadFixed
            //
            this.btnLoadFixed.Location = new System.Drawing.Point(168, 8);
            this.btnLoadFixed.Name = "btnLoadFixed";
            this.btnLoadFixed.Size = new System.Drawing.Size(150, 30);
            this.btnLoadFixed.Text = "Load rev 2 (fixed)";
            this.btnLoadFixed.Click += new System.EventHandler(this.btnLoadFixed_Click);
            //
            // btnReviewGeneratedCode
            //
            this.btnReviewGeneratedCode.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnReviewGeneratedCode.Location = new System.Drawing.Point(324, 8);
            this.btnReviewGeneratedCode.Name = "btnReviewGeneratedCode";
            this.btnReviewGeneratedCode.Size = new System.Drawing.Size(200, 30);
            this.btnReviewGeneratedCode.Text = "▶ Review generated code";
            this.btnReviewGeneratedCode.Click += new System.EventHandler(this.btnReviewGeneratedCode_Click);
            //
            // btnSignDecision
            //
            this.btnSignDecision.Location = new System.Drawing.Point(530, 8);
            this.btnSignDecision.Name = "btnSignDecision";
            this.btnSignDecision.Size = new System.Drawing.Size(150, 30);
            this.btnSignDecision.Text = "Sign the decision";
            this.btnSignDecision.Click += new System.EventHandler(this.btnSignDecision_Click);
            //
            // lblVerdict
            //
            this.lblVerdict.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblVerdict.AutoSize = false;
            this.lblVerdict.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.lblVerdict.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblVerdict.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblVerdict.Location = new System.Drawing.Point(12, 44);
            this.lblVerdict.Name = "lblVerdict";
            this.lblVerdict.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.lblVerdict.Size = new System.Drawing.Size(860, 30);
            this.lblVerdict.Text = "● pending review — load a draft, or paste a change, then press \"Review generated code\"";
            this.lblVerdict.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblReviewSource
            //
            this.lblReviewSource.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblReviewSource.AutoSize = false;
            this.lblReviewSource.Font = new System.Drawing.Font("default", 8F);
            this.lblReviewSource.ForeColor = System.Drawing.Color.FromArgb(120, 136, 153);
            this.lblReviewSource.Location = new System.Drawing.Point(12, 78);
            this.lblReviewSource.Name = "lblReviewSource";
            this.lblReviewSource.Size = new System.Drawing.Size(860, 18);
            this.lblReviewSource.Text = "Pull request #214 · generated by an AI assistant";
            //
            // txtGeneratedCode
            //
            this.txtGeneratedCode.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtGeneratedCode.Font = new System.Drawing.Font("monospace", 9F);
            this.txtGeneratedCode.Location = new System.Drawing.Point(12, 98);
            this.txtGeneratedCode.Multiline = true;
            this.txtGeneratedCode.Name = "txtGeneratedCode";
            this.txtGeneratedCode.Size = new System.Drawing.Size(860, 200);
            //
            // dgvFindings
            //
            this.dgvFindings.AllowUserToAddRows = false;
            this.dgvFindings.AllowUserToDeleteRows = false;
            this.dgvFindings.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvFindings.AutoGenerateColumns = false;
            this.dgvFindings.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFindings.BackColor = System.Drawing.Color.White;
            this.dgvFindings.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colRule,
            this.colSeverity,
            this.colLine,
            this.colFinding,
            this.colEvidence,
            this.colFix});
            this.dgvFindings.Location = new System.Drawing.Point(12, 306);
            this.dgvFindings.MultiSelect = false;
            this.dgvFindings.Name = "dgvFindings";
            this.dgvFindings.ReadOnly = true;
            this.dgvFindings.RowHeadersVisible = false;
            this.dgvFindings.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFindings.Size = new System.Drawing.Size(860, 204);
            this.colRule.DataPropertyName = "RuleId";
            this.colRule.HeaderText = "Rule";
            this.colRule.Name = "colRule";
            this.colRule.Width = 50;
            this.colSeverity.DataPropertyName = "Severity";
            this.colSeverity.HeaderText = "Severity";
            this.colSeverity.Name = "colSeverity";
            this.colSeverity.Width = 80;
            this.colLine.DataPropertyName = "Line";
            this.colLine.HeaderText = "Line";
            this.colLine.Name = "colLine";
            this.colLine.Width = 50;
            this.colFinding.DataPropertyName = "Message";
            this.colFinding.HeaderText = "Finding";
            this.colFinding.Name = "colFinding";
            this.colFinding.Width = 420;
            this.colEvidence.DataPropertyName = "Evidence";
            this.colEvidence.HeaderText = "Evidence (the line)";
            this.colEvidence.Name = "colEvidence";
            this.colEvidence.Width = 300;
            this.colFix.DataPropertyName = "Fix";
            this.colFix.HeaderText = "Fix";
            this.colFix.Name = "colFix";
            this.colFix.Width = 380;
            //
            // tabDocs
            //
            this.tabDocs.BackColor = System.Drawing.Color.White;
            this.tabDocs.Controls.Add(this.lblDocsStatus);
            this.tabDocs.Controls.Add(this.dgvDocs);
            this.tabDocs.Name = "tabDocs";
            this.tabDocs.Text = "Documentation index";
            //
            // lblDocsStatus
            //
            this.lblDocsStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblDocsStatus.AutoSize = false;
            this.lblDocsStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblDocsStatus.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblDocsStatus.Location = new System.Drawing.Point(12, 10);
            this.lblDocsStatus.Name = "lblDocsStatus";
            this.lblDocsStatus.Size = new System.Drawing.Size(860, 26);
            this.lblDocsStatus.Text = "docs/index.json";
            //
            // dgvDocs
            //
            this.dgvDocs.AllowUserToAddRows = false;
            this.dgvDocs.AllowUserToDeleteRows = false;
            this.dgvDocs.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvDocs.AutoGenerateColumns = false;
            this.dgvDocs.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDocs.BackColor = System.Drawing.Color.White;
            this.dgvDocs.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colDocId,
            this.colDocTitle,
            this.colDocPurpose,
            this.colDocPath,
            this.colDocModule,
            this.colDocVerified,
            this.colDocResolves});
            this.dgvDocs.Location = new System.Drawing.Point(12, 42);
            this.dgvDocs.MultiSelect = false;
            this.dgvDocs.Name = "dgvDocs";
            this.dgvDocs.ReadOnly = true;
            this.dgvDocs.RowHeadersVisible = false;
            this.dgvDocs.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDocs.Size = new System.Drawing.Size(860, 468);
            this.colDocId.DataPropertyName = "Id";
            this.colDocId.HeaderText = "Id";
            this.colDocId.Name = "colDocId";
            this.colDocId.Width = 150;
            this.colDocTitle.DataPropertyName = "Title";
            this.colDocTitle.HeaderText = "Title";
            this.colDocTitle.Name = "colDocTitle";
            this.colDocTitle.Width = 230;
            this.colDocPurpose.DataPropertyName = "Purpose";
            this.colDocPurpose.HeaderText = "Purpose";
            this.colDocPurpose.Name = "colDocPurpose";
            this.colDocPurpose.Width = 420;
            this.colDocPath.DataPropertyName = "Path";
            this.colDocPath.HeaderText = "Path";
            this.colDocPath.Name = "colDocPath";
            this.colDocPath.Width = 250;
            this.colDocModule.DataPropertyName = "Module";
            this.colDocModule.HeaderText = "Module";
            this.colDocModule.Name = "colDocModule";
            this.colDocModule.Width = 70;
            this.colDocVerified.DataPropertyName = "LastVerified";
            this.colDocVerified.HeaderText = "Verified";
            this.colDocVerified.Name = "colDocVerified";
            this.colDocVerified.Width = 100;
            this.colDocResolves.DataPropertyName = "Exists";
            this.colDocResolves.HeaderText = "Resolves";
            this.colDocResolves.Name = "colDocResolves";
            this.colDocResolves.Width = 80;
            //
            // tabDecisions
            //
            this.tabDecisions.BackColor = System.Drawing.Color.White;
            this.tabDecisions.Controls.Add(this.lblDecisionsStatus);
            this.tabDecisions.Controls.Add(this.dgvDecisions);
            this.tabDecisions.Name = "tabDecisions";
            this.tabDecisions.Text = "AI usage notes";
            //
            // lblDecisionsStatus
            //
            this.lblDecisionsStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblDecisionsStatus.AutoSize = false;
            this.lblDecisionsStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblDecisionsStatus.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblDecisionsStatus.Location = new System.Drawing.Point(12, 10);
            this.lblDecisionsStatus.Name = "lblDecisionsStatus";
            this.lblDecisionsStatus.Size = new System.Drawing.Size(860, 26);
            this.lblDecisionsStatus.Text = "No decision signed in this session — docs/AIUsageNotes.md holds the written record.";
            //
            // dgvDecisions
            //
            this.dgvDecisions.AllowUserToAddRows = false;
            this.dgvDecisions.AllowUserToDeleteRows = false;
            this.dgvDecisions.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvDecisions.AutoGenerateColumns = false;
            this.dgvDecisions.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDecisions.BackColor = System.Drawing.Color.White;
            this.dgvDecisions.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colDecidedAt,
            this.colDecisionPr,
            this.colDecisionVerdict,
            this.colDecisionAuthor,
            this.colDecisionReviewer,
            this.colDecisionReason});
            this.dgvDecisions.Location = new System.Drawing.Point(12, 42);
            this.dgvDecisions.MultiSelect = false;
            this.dgvDecisions.Name = "dgvDecisions";
            this.dgvDecisions.ReadOnly = true;
            this.dgvDecisions.RowHeadersVisible = false;
            this.dgvDecisions.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDecisions.Size = new System.Drawing.Size(860, 468);
            this.colDecidedAt.DataPropertyName = "DecidedUtc";
            this.colDecidedAt.HeaderText = "When (UTC)";
            this.colDecidedAt.Name = "colDecidedAt";
            this.colDecidedAt.Width = 150;
            this.colDecisionPr.DataPropertyName = "PullRequest";
            this.colDecisionPr.HeaderText = "Change";
            this.colDecisionPr.Name = "colDecisionPr";
            this.colDecisionPr.Width = 250;
            this.colDecisionVerdict.DataPropertyName = "Verdict";
            this.colDecisionVerdict.HeaderText = "Verdict";
            this.colDecisionVerdict.Name = "colDecisionVerdict";
            this.colDecisionVerdict.Width = 150;
            this.colDecisionAuthor.DataPropertyName = "Author";
            this.colDecisionAuthor.HeaderText = "Author";
            this.colDecisionAuthor.Name = "colDecisionAuthor";
            this.colDecisionAuthor.Width = 110;
            this.colDecisionReviewer.DataPropertyName = "Reviewer";
            this.colDecisionReviewer.HeaderText = "Signed by";
            this.colDecisionReviewer.Name = "colDecisionReviewer";
            this.colDecisionReviewer.Width = 110;
            this.colDecisionReason.DataPropertyName = "Reason";
            this.colDecisionReason.HeaderText = "Why";
            this.colDecisionReason.Name = "colDecisionReason";
            this.colDecisionReason.Width = 420;
            //
            // CapstoneReviewPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.tabCapstone);
            this.Name = "CapstoneReviewPage";
            this.Size = new System.Drawing.Size(932, 632);
            this.Text = "EnterpriseOps — Capstone review";
            this.Load += new System.EventHandler(this.CapstoneReviewPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.tabCapstone.ResumeLayout(false);
            this.tabPackage.ResumeLayout(false);
            this.tabDocs.ResumeLayout(false);
            this.tabPrompts.ResumeLayout(false);
            this.tabReview.ResumeLayout(false);
            this.tabChecklist.ResumeLayout(false);
            this.tabDecisions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Button btnDashboard;
        private Wisej.Web.TabControl tabCapstone;
        private Wisej.Web.TabPage tabPackage;
        private Wisej.Web.Button btnVerifyPackage;
        private Wisej.Web.Label lblPackageStatus;
        private Wisej.Web.DataGridView dgvPackage;
        private Wisej.Web.DataGridViewTextBoxColumn colDeliverable;
        private Wisej.Web.DataGridViewTextBoxColumn colDeliverablePath;
        private Wisej.Web.DataGridViewTextBoxColumn colDeliverableRequired;
        private Wisej.Web.DataGridViewTextBoxColumn colDeliverableResult;
        private Wisej.Web.DataGridViewTextBoxColumn colDeliverableDetail;
        private Wisej.Web.TabPage tabDocs;
        private Wisej.Web.Label lblDocsStatus;
        private Wisej.Web.DataGridView dgvDocs;
        private Wisej.Web.DataGridViewTextBoxColumn colDocId;
        private Wisej.Web.DataGridViewTextBoxColumn colDocTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colDocPurpose;
        private Wisej.Web.DataGridViewTextBoxColumn colDocPath;
        private Wisej.Web.DataGridViewTextBoxColumn colDocModule;
        private Wisej.Web.DataGridViewTextBoxColumn colDocVerified;
        private Wisej.Web.DataGridViewTextBoxColumn colDocResolves;
        private Wisej.Web.TabPage tabPrompts;
        private Wisej.Web.Label lblPromptsStatus;
        private Wisej.Web.ListBox lstPrompts;
        private Wisej.Web.TextBox txtPrompt;
        private Wisej.Web.TabPage tabReview;
        private Wisej.Web.Button btnLoadDraft;
        private Wisej.Web.Button btnLoadFixed;
        private Wisej.Web.Button btnReviewGeneratedCode;
        private Wisej.Web.Button btnSignDecision;
        private Wisej.Web.Label lblVerdict;
        private Wisej.Web.Label lblReviewSource;
        private Wisej.Web.TextBox txtGeneratedCode;
        private Wisej.Web.DataGridView dgvFindings;
        private Wisej.Web.DataGridViewTextBoxColumn colRule;
        private Wisej.Web.DataGridViewTextBoxColumn colSeverity;
        private Wisej.Web.DataGridViewTextBoxColumn colLine;
        private Wisej.Web.DataGridViewTextBoxColumn colFinding;
        private Wisej.Web.DataGridViewTextBoxColumn colEvidence;
        private Wisej.Web.DataGridViewTextBoxColumn colFix;
        private Wisej.Web.TabPage tabChecklist;
        private Wisej.Web.Label lblChecklistTitle;
        private Wisej.Web.DataGridView dgvChecklist;
        private Wisej.Web.DataGridViewTextBoxColumn colRuleId;
        private Wisej.Web.DataGridViewTextBoxColumn colRuleQuestion;
        private Wisej.Web.DataGridViewTextBoxColumn colRuleSeverity;
        private Wisej.Web.DataGridViewTextBoxColumn colRuleReference;
        private Wisej.Web.TabPage tabDecisions;
        private Wisej.Web.Label lblDecisionsStatus;
        private Wisej.Web.DataGridView dgvDecisions;
        private Wisej.Web.DataGridViewTextBoxColumn colDecidedAt;
        private Wisej.Web.DataGridViewTextBoxColumn colDecisionPr;
        private Wisej.Web.DataGridViewTextBoxColumn colDecisionVerdict;
        private Wisej.Web.DataGridViewTextBoxColumn colDecisionAuthor;
        private Wisej.Web.DataGridViewTextBoxColumn colDecisionReviewer;
        private Wisej.Web.DataGridViewTextBoxColumn colDecisionReason;
    }
}
