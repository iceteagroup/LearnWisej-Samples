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
            this.lblCapabilityFallback = new Wisej.Web.Label();
            this.lblCapabilitySummary = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblCapabilityTitle
            //
            this.lblCapabilityTitle.AutoSize = false;
            this.lblCapabilityTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblCapabilityTitle.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblCapabilityTitle.Location = new System.Drawing.Point(14, 10);
            this.lblCapabilityTitle.Name = "lblCapabilityTitle";
            this.lblCapabilityTitle.Size = new System.Drawing.Size(300, 22);
            this.lblCapabilityTitle.Text = "Browser capabilities";
            //
            // lblCapabilityMode
            //
            this.lblCapabilityMode.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCapabilityMode.AutoSize = false;
            this.lblCapabilityMode.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCapabilityMode.ForeColor = System.Drawing.Color.FromArgb(154, 167, 180);
            this.lblCapabilityMode.Location = new System.Drawing.Point(370, 14);
            this.lblCapabilityMode.Name = "lblCapabilityMode";
            this.lblCapabilityMode.Size = new System.Drawing.Size(260, 18);
            this.lblCapabilityMode.Text = "detection only — never a permission";
            this.lblCapabilityMode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lstCapabilities
            //
            this.lstCapabilities.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom
                | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstCapabilities.Font = new System.Drawing.Font("monospace", 9F);
            this.lstCapabilities.Location = new System.Drawing.Point(14, 38);
            this.lstCapabilities.Name = "lstCapabilities";
            this.lstCapabilities.Size = new System.Drawing.Size(616, 138);
            this.lstCapabilities.TabIndex = 0;
            //
            // lblCapabilityFallback
            //
            this.lblCapabilityFallback.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblCapabilityFallback.AutoSize = false;
            this.lblCapabilityFallback.Font = new System.Drawing.Font("default", 9F);
            this.lblCapabilityFallback.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblCapabilityFallback.Location = new System.Drawing.Point(14, 182);
            this.lblCapabilityFallback.Name = "lblCapabilityFallback";
            this.lblCapabilityFallback.Size = new System.Drawing.Size(616, 18);
            this.lblCapabilityFallback.Text = "No fallback needed yet.";
            //
            // lblCapabilitySummary
            //
            this.lblCapabilitySummary.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblCapabilitySummary.AutoSize = false;
            this.lblCapabilitySummary.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCapabilitySummary.ForeColor = System.Drawing.Color.FromArgb(106, 125, 146);
            this.lblCapabilitySummary.Location = new System.Drawing.Point(14, 202);
            this.lblCapabilitySummary.Name = "lblCapabilitySummary";
            this.lblCapabilitySummary.Size = new System.Drawing.Size(616, 18);
            this.lblCapabilitySummary.Text = "No capability report yet — the widget has not reported.";
            //
            // BrowserCapabilityPanel
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.lblCapabilityTitle);
            this.Controls.Add(this.lblCapabilityMode);
            this.Controls.Add(this.lstCapabilities);
            this.Controls.Add(this.lblCapabilityFallback);
            this.Controls.Add(this.lblCapabilitySummary);
            this.Name = "BrowserCapabilityPanel";
            this.Size = new System.Drawing.Size(644, 228);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblCapabilityTitle;
        private Wisej.Web.Label lblCapabilityMode;
        private Wisej.Web.ListBox lstCapabilities;
        private Wisej.Web.Label lblCapabilityFallback;
        private Wisej.Web.Label lblCapabilitySummary;
    }
}
