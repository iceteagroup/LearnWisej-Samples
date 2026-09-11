namespace EnterpriseOps.UI
{
    partial class CommandCenterShell
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
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblScreenName = new Wisej.Web.Label();
            this.commandPaletteHost = new EnterpriseOps.Interop.CommandPaletteHost();
            this.capabilityPanel = new EnterpriseOps.UI.BrowserCapabilityPanel();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblScreenName);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1108, 44);
            //
            // lblScreenName
            //
            this.lblScreenName.AutoSize = false;
            this.lblScreenName.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblScreenName.ForeColor = System.Drawing.Color.White;
            this.lblScreenName.Location = new System.Drawing.Point(20, 11);
            this.lblScreenName.Name = "lblScreenName";
            this.lblScreenName.Size = new System.Drawing.Size(400, 22);
            this.lblScreenName.Text = "EnterpriseOps — Command Center";
            //
            // commandPaletteHost
            //
            this.commandPaletteHost.Location = new System.Drawing.Point(16, 56);
            this.commandPaletteHost.Name = "commandPaletteHost";
            this.commandPaletteHost.Size = new System.Drawing.Size(620, 230);
            this.commandPaletteHost.TabIndex = 0;
            //
            // capabilityPanel
            //
            this.capabilityPanel.Location = new System.Drawing.Point(652, 56);
            this.capabilityPanel.Name = "capabilityPanel";
            this.capabilityPanel.Size = new System.Drawing.Size(440, 230);
            this.capabilityPanel.TabIndex = 1;
            //
            // lblBanner
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 236);
            this.lblBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(124, 42, 42);
            this.lblBanner.Location = new System.Drawing.Point(16, 298);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.lblBanner.Size = new System.Drawing.Size(1076, 26);
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatus.Font = new System.Drawing.Font("monospace", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatus.Location = new System.Drawing.Point(0, 336);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.lblStatus.Size = new System.Drawing.Size(1108, 30);
            this.lblStatus.Text = "Command Center — Ctrl+K opens the palette";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // CommandCenterShell
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.commandPaletteHost);
            this.Controls.Add(this.capabilityPanel);
            this.Controls.Add(this.lblBanner);
            this.Controls.Add(this.lblStatus);
            this.Name = "CommandCenterShell";
            this.Size = new System.Drawing.Size(1108, 366);
            this.Text = "EnterpriseOps — Command Center";
            this.Load += new System.EventHandler(this.CommandCenterShell_Load);
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblScreenName;
        private EnterpriseOps.Interop.CommandPaletteHost commandPaletteHost;
        private EnterpriseOps.UI.BrowserCapabilityPanel capabilityPanel;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatus;
    }
}
