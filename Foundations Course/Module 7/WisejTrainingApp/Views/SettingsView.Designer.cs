namespace WisejTrainingApp.Views
{
    partial class SettingsView
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
            this.lblPageDescription = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblPageTitle
            //
            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Text = "Settings";
            //
            // lblPageDescription
            //
            this.lblPageDescription.AutoSize = true;
            this.lblPageDescription.Location = new System.Drawing.Point(0, 38);
            this.lblPageDescription.Name = "lblPageDescription";
            this.lblPageDescription.Text = "Company details and notification preferences.";
            //
            // SettingsView
            //
            this.Controls.Add(this.lblPageDescription);
            this.Controls.Add(this.lblPageTitle);
            this.Name = "SettingsView";
            this.Size = new System.Drawing.Size(660, 460);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.Label lblPageDescription;
    }
}
