namespace AdaptiveOps.Shell
{
    partial class MetricCard
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
            this.strip = new Wisej.Web.Panel();
            this.lblTitle = new Wisej.Web.Label();
            this.lblValue = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // strip  (4-px accent along the top edge; docked, so it follows the card width in every engine)
            //
            this.strip.BackColor = System.Drawing.Color.FromArgb(36, 84, 166);
            this.strip.Dock = Wisej.Web.DockStyle.Top;
            this.strip.Name = "strip";
            this.strip.Size = new System.Drawing.Size(190, 4);
            //
            // lblTitle  (anchored Left|Right so it follows the card when a Table cell or a Flex weight resizes it)
            //
            this.lblTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(166, 16);
            this.lblTitle.Text = "METRIC";
            //
            // lblValue
            //
            this.lblValue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblValue.AutoEllipsis = true;
            this.lblValue.AutoSize = false;
            this.lblValue.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.lblValue.Location = new System.Drawing.Point(12, 30);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(166, 36);
            this.lblValue.Text = "–";
            //
            // MetricCard
            //
            // Size is the card's own size (what Flow uses); MinimumSize is what the Flex weights may not go
            // below; MaximumSize pins the height so AlignY has a dimension the card does not fill.
            // Margin is the card's distance to its neighbours — honoured by Flow, Table and Flex, ignored by Dock.
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.strip);
            this.Margin = new Wisej.Web.Padding(4);
            this.MaximumSize = new System.Drawing.Size(0, 76);
            this.MinimumSize = new System.Drawing.Size(140, 76);
            this.Name = "MetricCard";
            this.Size = new System.Drawing.Size(192, 76);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel strip;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblValue;
    }
}
