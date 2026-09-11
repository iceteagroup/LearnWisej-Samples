namespace WisejTrainingApp
{
    partial class MainPage
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
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.lblBreadcrumb = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.pnlNav = new Wisej.Web.Panel();
            this.btnDashboard = new Wisej.Web.Button();
            this.btnTickets = new Wisej.Web.Button();
            this.btnCustomers = new Wisej.Web.Button();
            this.btnSettings = new Wisej.Web.Button();
            this.pnlContent = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlNav.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.pnlHeader.Controls.Add(this.lblAppTitle);
            this.pnlHeader.Controls.Add(this.lblBreadcrumb);
            this.pnlHeader.Controls.Add(this.lblUser);
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1000, 60);
            //
            // lblAppTitle
            //
            this.lblAppTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Location = new System.Drawing.Point(16, 6);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(320, 28);
            this.lblAppTitle.Text = "ServiceDesk Application";
            //
            // lblBreadcrumb
            //
            this.lblBreadcrumb.ForeColor = System.Drawing.Color.White;
            this.lblBreadcrumb.Location = new System.Drawing.Point(16, 34);
            this.lblBreadcrumb.Name = "lblBreadcrumb";
            this.lblBreadcrumb.Size = new System.Drawing.Size(320, 20);
            this.lblBreadcrumb.Text = "Home / Dashboard";
            //
            // lblUser
            //
            this.lblUser.Dock = Wisej.Web.DockStyle.Right;
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Name = "lblUser";
            this.lblUser.Padding = new Wisej.Web.Padding(0, 0, 16, 0);
            this.lblUser.Size = new System.Drawing.Size(280, 60);
            this.lblUser.Text = "Signed in as:";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlNav
            //
            this.pnlNav.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.pnlNav.Controls.Add(this.btnDashboard);
            this.pnlNav.Controls.Add(this.btnTickets);
            this.pnlNav.Controls.Add(this.btnCustomers);
            this.pnlNav.Controls.Add(this.btnSettings);
            this.pnlNav.Dock = Wisej.Web.DockStyle.Left;
            this.pnlNav.Name = "pnlNav";
            this.pnlNav.Size = new System.Drawing.Size(180, 600);
            //
            // btnDashboard
            //
            this.btnDashboard.Location = new System.Drawing.Point(12, 16);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(156, 36);
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            //
            // btnTickets
            //
            this.btnTickets.Location = new System.Drawing.Point(12, 60);
            this.btnTickets.Name = "btnTickets";
            this.btnTickets.Size = new System.Drawing.Size(156, 36);
            this.btnTickets.Text = "Tickets";
            this.btnTickets.Click += new System.EventHandler(this.btnTickets_Click);
            //
            // btnCustomers
            //
            this.btnCustomers.Location = new System.Drawing.Point(12, 104);
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.Size = new System.Drawing.Size(156, 36);
            this.btnCustomers.Text = "Customers";
            this.btnCustomers.Click += new System.EventHandler(this.btnCustomers_Click);
            //
            // btnSettings
            //
            this.btnSettings.Location = new System.Drawing.Point(12, 148);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(156, 36);
            this.btnSettings.Text = "Settings";
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            //
            // pnlContent
            //
            this.pnlContent.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new Wisej.Web.Padding(24);
            //
            // lblStatus
            //
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatus.Size = new System.Drawing.Size(1000, 28);
            this.lblStatus.Text = "Ready.";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // MainPage
            //
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlNav);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlHeader);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1000, 700);
            this.Text = "ServiceDesk Application";
            this.pnlHeader.ResumeLayout(false);
            this.pnlNav.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Label lblBreadcrumb;
        private Wisej.Web.Label lblUser;
        private Wisej.Web.Panel pnlNav;
        private Wisej.Web.Button btnDashboard;
        private Wisej.Web.Button btnTickets;
        private Wisej.Web.Button btnCustomers;
        private Wisej.Web.Button btnSettings;
        private Wisej.Web.Panel pnlContent;
        private Wisej.Web.Label lblStatus;
    }
}
