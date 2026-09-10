namespace AdaptiveOps.Views
{
    partial class ReportsView
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
            this.lblTitle = new Wisej.Web.Label();
            this.lblNote = new Wisej.Web.Label();
            this.tableReport = new Wisej.Web.TableLayoutPanel();
            this.hdrOwner = new Wisej.Web.Label();
            this.hdrOpen = new Wisej.Web.Label();
            this.hdrOverdue = new Wisej.Web.Label();
            this.tableReport.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AppearanceKey = "subheading-label";
            this.lblTitle.AutoSize = false;
            this.lblTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(600, 28);
            this.lblTitle.TabStop = false;
            this.lblTitle.Text = "Reports · open and overdue tickets by owner";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblNote  (Dock = Bottom · when the view was created and what it cost)
            //
            this.lblNote.AppearanceKey = "mono-label";
            this.lblNote.AutoEllipsis = true;
            this.lblNote.AutoSize = false;
            this.lblNote.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(600, 22);
            this.lblNote.TabStop = false;
            this.lblNote.Text = "";
            this.lblNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tableReport  (Dock = Top · 3 columns: Owner Percent 100, Open Absolute 90, Overdue Absolute 90 · header row in the Designer, data rows in Bind)
            //
            this.tableReport.ColumnCount = 3;
            this.tableReport.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 100F));
            this.tableReport.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Absolute, 90F));
            this.tableReport.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Absolute, 90F));
            this.tableReport.Dock = Wisej.Web.DockStyle.Top;
            this.tableReport.GrowStyle = Wisej.Web.TableLayoutPanelGrowStyle.AddRows;
            this.tableReport.MinimumSize = new System.Drawing.Size(240, 32);
            this.tableReport.Name = "tableReport";
            this.tableReport.RowCount = 1;
            this.tableReport.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 32F));
            this.tableReport.Size = new System.Drawing.Size(600, 220);
            this.tableReport.TabStop = false;
            InitHeader(this.hdrOwner, "OWNER", System.Drawing.ContentAlignment.MiddleLeft);
            InitHeader(this.hdrOpen, "OPEN", System.Drawing.ContentAlignment.MiddleRight);
            InitHeader(this.hdrOverdue, "OVERDUE", System.Drawing.ContentAlignment.MiddleRight);
            this.tableReport.Controls.Add(this.hdrOwner, 0, 0);
            this.tableReport.Controls.Add(this.hdrOpen, 1, 0);
            this.tableReport.Controls.Add(this.hdrOverdue, 2, 0);
            //
            // ReportsView  (theme appearance surface-card)
            //
            this.AppearanceKey = "surface-card";
            this.Controls.Add(this.tableReport);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.lblTitle);
            this.MinimumSize = new System.Drawing.Size(280, 200);
            this.Name = "ReportsView";
            this.Padding = new Wisej.Web.Padding(12);
            this.Size = new System.Drawing.Size(624, 400);
            this.TabStop = false;
            this.tableReport.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private static void InitHeader(Wisej.Web.Label label, string text, System.Drawing.ContentAlignment align)
        {
            label.AppearanceKey = "metric-title";
            label.AutoSize = false;
            label.CssClass = "metric-title";
            label.Dock = Wisej.Web.DockStyle.Fill;
            label.Margin = new Wisej.Web.Padding(0);
            label.Name = "hdr" + text;
            label.TabStop = false;
            label.Text = text;
            label.TextAlign = align;
        }

        #endregion

        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblNote;
        private Wisej.Web.TableLayoutPanel tableReport;
        private Wisej.Web.Label hdrOwner;
        private Wisej.Web.Label hdrOpen;
        private Wisej.Web.Label hdrOverdue;
    }
}
