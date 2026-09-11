namespace EnterpriseOps.UI
{
    partial class BrowserCapabilityPanel
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
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
            this.lblCapabilityTitle = new Wisej.Web.Label();
            this.lblCapabilityMode = new Wisej.Web.Label();
            this.lstCapabilities = new Wisej.Web.ListBox();
            this.SuspendLayout();
            //
            // lblCapabilityTitle
            //
            this.lblCapabilityTitle.AutoSize = false;
            this.lblCapabilityTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblCapabilityTitle.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblCapabilityTitle.Location = new System.Drawing.Point(14, 10);
            this.lblCapabilityTitle.Name = "lblCapabilityTitle";
            this.lblCapabilityTitle.Size = new System.Drawing.Size(260, 22);
            this.lblCapabilityTitle.Text = "Browser capabilities";
            //
            // lblCapabilityMode
            //
            this.lblCapabilityMode.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCapabilityMode.AutoSize = false;
            this.lblCapabilityMode.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCapabilityMode.ForeColor = System.Drawing.Color.FromArgb(154, 167, 180);
            this.lblCapabilityMode.Location = new System.Drawing.Point(286, 14);
            this.lblCapabilityMode.Name = "lblCapabilityMode";
            this.lblCapabilityMode.Size = new System.Drawing.Size(140, 18);
            this.lblCapabilityMode.Text = "detection only";
            this.lblCapabilityMode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lstCapabilities
            //
            this.lstCapabilities.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom
                | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstCapabilities.Font = new System.Drawing.Font("monospace", 9F);
            this.lstCapabilities.Location = new System.Drawing.Point(14, 38);
            this.lstCapabilities.Name = "lstCapabilities";
            this.lstCapabilities.Size = new System.Drawing.Size(412, 178);
            this.lstCapabilities.TabIndex = 0;
            //
            // BrowserCapabilityPanel
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.lblCapabilityTitle);
            this.Controls.Add(this.lblCapabilityMode);
            this.Controls.Add(this.lstCapabilities);
            this.Name = "BrowserCapabilityPanel";
            this.Size = new System.Drawing.Size(440, 230);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblCapabilityTitle;
        private Wisej.Web.Label lblCapabilityMode;
        private Wisej.Web.ListBox lstCapabilities;
    }
}
