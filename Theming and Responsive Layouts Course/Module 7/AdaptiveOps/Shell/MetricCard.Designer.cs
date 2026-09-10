namespace AdaptiveOps.Shell
{
    partial class MetricCard
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
            this.SuspendLayout();
            //
            // lblTitle  (theme appearance metric-title: cardTitle font + textMuted; CSS class metric-title: uppercase + letter-spacing)
            //
            this.lblTitle.AppearanceKey = "metric-title";
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.AutoSize = false;
            this.lblTitle.CssClass = "metric-title";
            this.lblTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(116, 18);
            this.lblTitle.TabStop = false;
            this.lblTitle.Text = "METRIC";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblValue  (theme appearance metric-value: cardValue font)
            //
            this.lblValue.AppearanceKey = "metric-value";
            this.lblValue.AutoEllipsis = true;
            this.lblValue.AutoSize = false;
            this.lblValue.CssClass = "metric-value";
            this.lblValue.Dock = Wisej.Web.DockStyle.Fill;
            this.lblValue.Name = "lblValue";
            this.lblValue.TabStop = false;
            this.lblValue.Text = "–";
            this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // MetricCard  (theme appearance metric-card; the strip colour of Module 1 became the card border + shadow tokens)
            //
            this.AppearanceKey = "metric-card";
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.lblTitle);
            this.CssClass = "metric-card";
            this.Name = "MetricCard";
            this.Padding = new Wisej.Web.Padding(12, 10, 12, 8);
            this.Size = new System.Drawing.Size(140, 84);
            this.TabStop = false;
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblValue;
    }
}
