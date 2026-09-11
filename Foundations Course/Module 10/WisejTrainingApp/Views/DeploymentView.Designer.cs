namespace WisejTrainingApp.Views
{
    partial class DeploymentView
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
            this.lblPageTitle = new Wisej.Web.Label();
            this.chkReleaseChecks = new Wisej.Web.CheckedListBox();
            this.lblPackageStatus = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblPageTitle
            //
            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Text = "Deployment";
            //
            // chkReleaseChecks
            //
            this.chkReleaseChecks.CheckOnClick = true;
            this.chkReleaseChecks.Items.AddRange(new object[] {
            "Configuration: Web.config reviewed and debug mode off",
            "Security: license key and secrets kept out of the code",
            "Logging: destination configured, no secrets in the log",
            "Theme and static resources included",
            "Target notes written (IIS / Kestrel / cloud)"});
            this.chkReleaseChecks.Location = new System.Drawing.Point(0, 50);
            this.chkReleaseChecks.Name = "chkReleaseChecks";
            this.chkReleaseChecks.Size = new System.Drawing.Size(560, 160);
            this.chkReleaseChecks.AfterItemCheck += new Wisej.Web.ItemCheckEventHandler(this.chkReleaseChecks_AfterItemCheck);
            //
            // lblPackageStatus
            //
            this.lblPackageStatus.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblPackageStatus.Location = new System.Drawing.Point(0, 224);
            this.lblPackageStatus.Name = "lblPackageStatus";
            this.lblPackageStatus.Size = new System.Drawing.Size(560, 24);
            //
            // DeploymentView
            //
            this.Controls.Add(this.lblPackageStatus);
            this.Controls.Add(this.chkReleaseChecks);
            this.Controls.Add(this.lblPageTitle);
            this.Name = "DeploymentView";
            this.Size = new System.Drawing.Size(800, 500);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.CheckedListBox chkReleaseChecks;
        private Wisej.Web.Label lblPackageStatus;
    }
}
