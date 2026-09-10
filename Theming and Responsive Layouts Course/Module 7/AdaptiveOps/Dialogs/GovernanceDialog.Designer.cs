namespace AdaptiveOps.Dialogs
{
    partial class GovernanceDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.lblSummary = new Wisej.Web.Label();
            this.gridRules = new Wisej.Web.DataGridView();
            this.colResult = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colRule = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colEvidence = new Wisej.Web.DataGridViewTextBoxColumn();
            this.footerPanel = new Wisej.Web.Panel();
            this.lblFooter = new Wisej.Web.Label();
            this.btnClose = new Wisej.Web.Button();
            this.footerPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // lblSummary  (status-label appearance: ok / error theme states + icon + text)
            //
            this.lblSummary.AccessibleName = "Governance review summary";
            this.lblSummary.AppearanceKey = "status-label";
            this.lblSummary.AutoEllipsis = true;
            this.lblSummary.AutoSize = false;
            this.lblSummary.CssClass = "gov-summary";
            this.lblSummary.Dock = Wisej.Web.DockStyle.Top;
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Padding = new Wisej.Web.Padding(0, 0, 0, 8);
            this.lblSummary.Size = new System.Drawing.Size(744, 36);
            this.lblSummary.TabStop = false;
            this.lblSummary.Text = "Running the review…";
            this.lblSummary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // gridRules  (Dock = Fill · Result 90 · Rule 320 · Evidence fills)
            //
            this.gridRules.AccessibleName = "Governance rules";
            this.gridRules.AllowUserToAddRows = false;
            this.gridRules.AllowUserToDeleteRows = false;
            this.gridRules.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridRules.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colResult,
            this.colRule,
            this.colEvidence});
            this.gridRules.Dock = Wisej.Web.DockStyle.Fill;
            this.gridRules.MinimumSize = new System.Drawing.Size(320, 160);
            this.gridRules.MultiSelect = false;
            this.gridRules.Name = "gridRules";
            this.gridRules.ReadOnly = true;
            this.gridRules.RowHeadersVisible = false;
            this.gridRules.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridRules.TabIndex = 1;
            this.colResult.FillWeight = 70F;
            this.colResult.HeaderText = "Result";
            this.colResult.MinimumWidth = 80;
            this.colResult.Name = "colResult";
            this.colResult.ReadOnly = true;
            this.colRule.FillWeight = 260F;
            this.colRule.HeaderText = "Rule";
            this.colRule.MinimumWidth = 200;
            this.colRule.Name = "colRule";
            this.colRule.ReadOnly = true;
            this.colEvidence.FillWeight = 400F;
            this.colEvidence.HeaderText = "Evidence (computed now)";
            this.colEvidence.MinimumWidth = 200;
            this.colEvidence.Name = "colEvidence";
            this.colEvidence.ReadOnly = true;
            //
            // footerPanel  (Dock = Bottom)
            //
            this.footerPanel.Controls.Add(this.lblFooter);
            this.footerPanel.Controls.Add(this.btnClose);
            this.footerPanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Padding = new Wisej.Web.Padding(0, 8, 0, 0);
            this.footerPanel.Size = new System.Drawing.Size(744, 44);
            this.footerPanel.TabIndex = 2;
            this.footerPanel.TabStop = false;
            this.lblFooter.AppearanceKey = "mono-label";
            this.lblFooter.AutoEllipsis = true;
            this.lblFooter.AutoSize = false;
            this.lblFooter.Dock = Wisej.Web.DockStyle.Fill;
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.TabStop = false;
            this.lblFooter.Text = "";
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.AccessibleName = "Close the governance review";
            this.btnClose.Dock = Wisej.Web.DockStyle.Right;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 36);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.ToolTipText = "Close the review";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // GovernanceDialog
            //
            this.AccessibleName = "Governance review dialog";
            this.CancelButton = this.btnClose;
            this.Controls.Add(this.gridRules);
            this.Controls.Add(this.footerPanel);
            this.Controls.Add(this.lblSummary);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(360, 300);
            this.Name = "GovernanceDialog";
            this.Padding = new Wisej.Web.Padding(12);
            this.Size = new System.Drawing.Size(860, 560);
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "Governance review · theme, CSS, profiles, accessibility, layout";
            this.footerPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblSummary;
        private Wisej.Web.DataGridView gridRules;
        private Wisej.Web.DataGridViewTextBoxColumn colResult;
        private Wisej.Web.DataGridViewTextBoxColumn colRule;
        private Wisej.Web.DataGridViewTextBoxColumn colEvidence;
        private Wisej.Web.Panel footerPanel;
        private Wisej.Web.Label lblFooter;
        private Wisej.Web.Button btnClose;
    }
}
