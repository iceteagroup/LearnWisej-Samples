namespace OperationsConsole
{
    partial class MainPage
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

        #region Wisej Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new Wisej.Web.Label();
            this.lblProfile = new Wisej.Web.Label();
            this.btnToast = new Wisej.Web.Button();
            this.btnAlert = new Wisej.Web.Button();
            this.lstEventLog = new Wisej.Web.ListBox();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(24, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(600, 32);
            this.lblTitle.Text = "Operations Console — template";
            // 
            // lblProfile
            // 
            this.lblProfile.Location = new System.Drawing.Point(24, 60);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(600, 24);
            this.lblProfile.Text = "Profile:";
            // 
            // btnToast
            // 
            this.btnToast.Location = new System.Drawing.Point(24, 100);
            this.btnToast.Name = "btnToast";
            this.btnToast.Size = new System.Drawing.Size(140, 32);
            this.btnToast.Text = "Show Toast";
            this.btnToast.Click += new System.EventHandler(this.btnToast_Click);
            // 
            // btnAlert
            // 
            this.btnAlert.Location = new System.Drawing.Point(176, 100);
            this.btnAlert.Name = "btnAlert";
            this.btnAlert.Size = new System.Drawing.Size(140, 32);
            this.btnAlert.Text = "Show AlertBox";
            this.btnAlert.Click += new System.EventHandler(this.btnAlert_Click);
            // 
            // lstEventLog
            // 
            this.lstEventLog.Font = new System.Drawing.Font("monospace", 9F);
            this.lstEventLog.Location = new System.Drawing.Point(24, 150);
            this.lstEventLog.Name = "lstEventLog";
            this.lstEventLog.Size = new System.Drawing.Size(600, 300);
            // 
            // MainPage
            // 
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.lstEventLog);
            this.Controls.Add(this.btnAlert);
            this.Controls.Add(this.btnToast);
            this.Controls.Add(this.lblProfile);
            this.Controls.Add(this.lblTitle);
            this.Name = "MainPage";
            this.Text = "Operations Console";
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblProfile;
        private Wisej.Web.Button btnToast;
        private Wisej.Web.Button btnAlert;
        private Wisej.Web.ListBox lstEventLog;
    }
}
