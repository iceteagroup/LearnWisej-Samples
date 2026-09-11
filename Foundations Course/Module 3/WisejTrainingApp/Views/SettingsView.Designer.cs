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
            this.lblTitle = new Wisej.Web.Label();
            this.lblCompanyName = new Wisej.Web.Label();
            this.txtCompanyName = new Wisej.Web.TextBox();
            this.btnSave = new Wisej.Web.Button();
            this.lblPermission = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "Settings";
            //
            // lblCompanyName
            //
            this.lblCompanyName.AutoSize = true;
            this.lblCompanyName.Location = new System.Drawing.Point(0, 44);
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Text = "Company name";
            //
            // txtCompanyName
            //
            this.txtCompanyName.Location = new System.Drawing.Point(0, 66);
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.Size = new System.Drawing.Size(260, 30);
            this.txtCompanyName.Text = "ServiceDesk Ltd.";
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(0, 110);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 34);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // lblPermission
            //
            this.lblPermission.AutoSize = true;
            this.lblPermission.Location = new System.Drawing.Point(0, 156);
            this.lblPermission.Name = "lblPermission";
            //
            // SettingsView
            //
            this.Controls.Add(this.lblPermission);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtCompanyName);
            this.Controls.Add(this.lblCompanyName);
            this.Controls.Add(this.lblTitle);
            this.Name = "SettingsView";
            this.Size = new System.Drawing.Size(600, 400);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblCompanyName;
        private Wisej.Web.TextBox txtCompanyName;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Label lblPermission;
    }
}
