namespace AdaptiveOps.Shell
{
    partial class StatusBar
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
            this.lblStatus = new Wisej.Web.Label();
            this.lblBrowser = new Wisej.Web.Label();
            this.lblTheme = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblStatus  (● ready / warn / error · Dock = Left)
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Left;
            this.lblStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(200, 22);
            this.lblStatus.Text = "● starting";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblBrowser  ("Browser 1348 × 680 px · Desktop" · Dock = Fill takes what is left)
            //
            this.lblBrowser.AutoEllipsis = true;
            this.lblBrowser.AutoSize = false;
            this.lblBrowser.Dock = Wisej.Web.DockStyle.Fill;
            this.lblBrowser.Font = new System.Drawing.Font("monospace", 9F);
            this.lblBrowser.Name = "lblBrowser";
            this.lblBrowser.Text = "Browser size: not read yet";
            this.lblBrowser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTheme  (Dock = Right)
            //
            this.lblTheme.AutoEllipsis = true;
            this.lblTheme.AutoSize = false;
            this.lblTheme.Dock = Wisej.Web.DockStyle.Right;
            this.lblTheme.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(300, 22);
            this.lblTheme.Text = "theme: –";
            this.lblTheme.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // StatusBar
            //
            // Three docked labels. Child order again: lblStatus is added LAST so it is docked FIRST
            // (Left), lblTheme takes the Right edge, lblBrowser (Fill) gets what is left.
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.lblBrowser);
            this.Controls.Add(this.lblTheme);
            this.Controls.Add(this.lblStatus);
            this.Name = "StatusBar";
            this.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.Size = new System.Drawing.Size(1332, 24);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblBrowser;
        private Wisej.Web.Label lblTheme;
    }
}
