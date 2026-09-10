namespace AdaptiveOps.Shell
{
    partial class LayoutCostCard
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
            this.lblValue = new Wisej.Web.Label();
            this.costBar = new Wisej.Web.TableLayoutPanel();
            this.barFill = new Wisej.Web.Panel();
            this.barTrack = new Wisej.Web.Panel();
            this.costBar.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AppearanceKey = "metric-title";
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.AutoSize = false;
            this.lblTitle.CssClass = "metric-title";
            this.lblTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(116, 18);
            this.lblTitle.TabStop = false;
            this.lblTitle.Text = "LAYOUT COST";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // costBar  (Dock = Bottom · two Percent columns = used / free share of the control budget)
            //
            this.costBar.ColumnCount = 2;
            this.costBar.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            this.costBar.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 75F));
            this.costBar.Controls.Add(this.barFill, 0, 0);
            this.costBar.Controls.Add(this.barTrack, 1, 0);
            this.costBar.CssClass = "cost-bar";
            this.costBar.Dock = Wisej.Web.DockStyle.Bottom;
            this.costBar.Name = "costBar";
            this.costBar.RowCount = 1;
            this.costBar.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
            this.costBar.Size = new System.Drawing.Size(116, 6);
            this.costBar.TabStop = false;
            //
            // barFill / barTrack  (theme appearances cost-bar-fill / cost-bar-track)
            //
            this.barFill.AppearanceKey = "cost-bar-fill";
            this.barFill.Dock = Wisej.Web.DockStyle.Fill;
            this.barFill.Margin = new Wisej.Web.Padding(0);
            this.barFill.Name = "barFill";
            this.barFill.TabStop = false;
            this.barTrack.AppearanceKey = "cost-bar-track";
            this.barTrack.Dock = Wisej.Web.DockStyle.Fill;
            this.barTrack.Margin = new Wisej.Web.Padding(0);
            this.barTrack.Name = "barTrack";
            this.barTrack.TabStop = false;
            //
            // lblValue  (subheading size so "187 ctrls" fits a 140 px card)
            //
            this.lblValue.AppearanceKey = "subheading-label";
            this.lblValue.AutoEllipsis = true;
            this.lblValue.AutoSize = false;
            this.lblValue.CssClass = "metric-value";
            this.lblValue.Dock = Wisej.Web.DockStyle.Fill;
            this.lblValue.Name = "lblValue";
            this.lblValue.TabStop = false;
            this.lblValue.Text = "– ctrls";
            this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // LayoutCostCard
            //
            this.AppearanceKey = "metric-card";
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.costBar);
            this.Controls.Add(this.lblTitle);
            this.CssClass = "metric-card cost-card";
            this.Name = "LayoutCostCard";
            this.Padding = new Wisej.Web.Padding(12, 10, 12, 10);
            this.Size = new System.Drawing.Size(140, 84);
            this.TabStop = false;
            this.costBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblValue;
        private Wisej.Web.TableLayoutPanel costBar;
        private Wisej.Web.Panel barFill;
        private Wisej.Web.Panel barTrack;
    }
}
