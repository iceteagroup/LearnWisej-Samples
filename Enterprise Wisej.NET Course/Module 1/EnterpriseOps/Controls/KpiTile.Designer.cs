namespace EnterpriseOps.Controls
{
    partial class KpiTile
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
            this.pnlFrame = new Wisej.Web.Panel();
            this.lblCaption = new Wisej.Web.Label();
            this.lblValue = new Wisej.Web.Label();
            this.pnlFrame.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlFrame  (the white card border; the tile itself has no border of its own)
            //
            this.pnlFrame.BackColor = System.Drawing.Color.White;
            this.pnlFrame.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlFrame.Controls.Add(this.lblCaption);
            this.pnlFrame.Controls.Add(this.lblValue);
            this.pnlFrame.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlFrame.Location = new System.Drawing.Point(0, 0);
            this.pnlFrame.Name = "pnlFrame";
            this.pnlFrame.Size = new System.Drawing.Size(248, 84);
            //
            // lblCaption
            //
            this.lblCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblCaption.AutoSize = false;
            this.lblCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCaption.Location = new System.Drawing.Point(16, 10);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(216, 18);
            this.lblCaption.Text = "CAPTION";
            //
            // lblValue
            //
            this.lblValue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblValue.AutoSize = false;
            this.lblValue.Font = new System.Drawing.Font("default", 26F, System.Drawing.FontStyle.Bold);
            this.lblValue.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblValue.Location = new System.Drawing.Point(16, 30);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(216, 44);
            this.lblValue.Text = "0";
            this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // KpiTile
            //
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pnlFrame);
            this.Name = "KpiTile";
            this.Size = new System.Drawing.Size(248, 84);
            this.pnlFrame.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlFrame;
        private Wisej.Web.Label lblCaption;
        private Wisej.Web.Label lblValue;
    }
}
