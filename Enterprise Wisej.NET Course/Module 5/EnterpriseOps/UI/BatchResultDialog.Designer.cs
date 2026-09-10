namespace EnterpriseOps.UI
{
    partial class BatchResultDialog
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
            this.pnlReportHeader = new Wisej.Web.Panel();
            this.lblReportTitle = new Wisej.Web.Label();
            this.lblReportCorrelation = new Wisej.Web.Label();
            this.dgvResults = new Wisej.Web.DataGridView();
            this.colGlyph = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colResultNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOutcome = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colReason = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblReportFooter = new Wisej.Web.Label();
            this.btnRetry = new Wisej.Web.Button();
            this.btnCloseReport = new Wisej.Web.Button();
            this.pnlReportHeader.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlReportHeader  (amber band: "Batch result — 2 succeeded, 1 failed" + the correlation id)
            //
            this.pnlReportHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlReportHeader.BackColor = System.Drawing.Color.FromArgb(255, 248, 236);
            this.pnlReportHeader.Controls.Add(this.lblReportTitle);
            this.pnlReportHeader.Controls.Add(this.lblReportCorrelation);
            this.pnlReportHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlReportHeader.Name = "pnlReportHeader";
            this.pnlReportHeader.Size = new System.Drawing.Size(720, 48);
            //
            // lblReportTitle
            //
            this.lblReportTitle.AutoSize = false;
            this.lblReportTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblReportTitle.ForeColor = System.Drawing.Color.FromArgb(122, 82, 16);
            this.lblReportTitle.Location = new System.Drawing.Point(20, 0);
            this.lblReportTitle.Name = "lblReportTitle";
            this.lblReportTitle.Size = new System.Drawing.Size(440, 48);
            this.lblReportTitle.Text = "Batch result";
            this.lblReportTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblReportCorrelation
            //
            this.lblReportCorrelation.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblReportCorrelation.AutoSize = false;
            this.lblReportCorrelation.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblReportCorrelation.ForeColor = System.Drawing.Color.FromArgb(154, 122, 58);
            this.lblReportCorrelation.Location = new System.Drawing.Point(460, 0);
            this.lblReportCorrelation.Name = "lblReportCorrelation";
            this.lblReportCorrelation.Size = new System.Drawing.Size(240, 48);
            this.lblReportCorrelation.Text = "correlation —";
            this.lblReportCorrelation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dgvResults  (one line per row of the batch: outcome + reason — never a single "done")
            //
            this.dgvResults.AllowUserToAddRows = false;
            this.dgvResults.AllowUserToDeleteRows = false;
            this.dgvResults.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvResults.AutoGenerateColumns = false;
            this.dgvResults.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResults.BackColor = System.Drawing.Color.White;
            this.dgvResults.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colGlyph,
            this.colResultNumber,
            this.colOutcome,
            this.colReason});
            this.dgvResults.Location = new System.Drawing.Point(20, 64);
            this.dgvResults.MultiSelect = false;
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.ReadOnly = true;
            this.dgvResults.RowHeadersVisible = false;
            this.dgvResults.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResults.Size = new System.Drawing.Size(680, 272);
            //
            // colGlyph
            //
            this.colGlyph.DataPropertyName = "Glyph";
            this.colGlyph.FillWeight = 6F;
            this.colGlyph.HeaderText = "";
            this.colGlyph.Name = "colGlyph";
            this.colGlyph.ReadOnly = true;
            this.colGlyph.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // colResultNumber
            //
            this.colResultNumber.DataPropertyName = "Number";
            this.colResultNumber.FillWeight = 16F;
            this.colResultNumber.HeaderText = "Work order";
            this.colResultNumber.Name = "colResultNumber";
            this.colResultNumber.ReadOnly = true;
            this.colResultNumber.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // colOutcome
            //
            this.colOutcome.DataPropertyName = "OutcomeText";
            this.colOutcome.FillWeight = 16F;
            this.colOutcome.HeaderText = "Outcome";
            this.colOutcome.Name = "colOutcome";
            this.colOutcome.ReadOnly = true;
            this.colOutcome.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // colReason
            //
            this.colReason.DataPropertyName = "Message";
            this.colReason.FillWeight = 62F;
            this.colReason.HeaderText = "Reason";
            this.colReason.Name = "colReason";
            this.colReason.ReadOnly = true;
            this.colReason.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // lblReportFooter
            //
            this.lblReportFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.lblReportFooter.AutoSize = false;
            this.lblReportFooter.Font = new System.Drawing.Font("monospace", 9F);
            this.lblReportFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblReportFooter.Location = new System.Drawing.Point(20, 350);
            this.lblReportFooter.Name = "lblReportFooter";
            this.lblReportFooter.Size = new System.Drawing.Size(400, 34);
            this.lblReportFooter.Text = "";
            this.lblReportFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnRetry
            //
            this.btnRetry.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.btnRetry.Enabled = false;
            this.btnRetry.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnRetry.Location = new System.Drawing.Point(430, 350);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(170, 34);
            this.btnRetry.Text = "Retry failed rows";
            this.btnRetry.ToolTipText = "Closes with DialogResult.Retry — the page re-reads the failed rows only and runs the batch again.";
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            //
            // btnCloseReport
            //
            this.btnCloseReport.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.btnCloseReport.Location = new System.Drawing.Point(610, 350);
            this.btnCloseReport.Name = "btnCloseReport";
            this.btnCloseReport.Size = new System.Drawing.Size(90, 34);
            this.btnCloseReport.Text = "Close";
            this.btnCloseReport.Click += new System.EventHandler(this.btnCloseReport_Click);
            //
            // BatchResultDialog
            //
            this.AcceptButton = this.btnCloseReport;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(720, 400);
            this.Controls.Add(this.pnlReportHeader);
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.lblReportFooter);
            this.Controls.Add(this.btnRetry);
            this.Controls.Add(this.btnCloseReport);
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.Fixed;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BatchResultDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Batch result";
            this.pnlReportHeader.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlReportHeader;
        private Wisej.Web.Label lblReportTitle;
        private Wisej.Web.Label lblReportCorrelation;
        private Wisej.Web.DataGridView dgvResults;
        private Wisej.Web.DataGridViewTextBoxColumn colGlyph;
        private Wisej.Web.DataGridViewTextBoxColumn colResultNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colOutcome;
        private Wisej.Web.DataGridViewTextBoxColumn colReason;
        private Wisej.Web.Label lblReportFooter;
        private Wisej.Web.Button btnRetry;
        private Wisej.Web.Button btnCloseReport;
    }
}
