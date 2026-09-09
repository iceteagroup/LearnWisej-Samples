namespace WisejTrainingApp
{
    partial class Window1
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
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.lblThemeCaption = new Wisej.Web.Label();
            this.cboTheme = new Wisej.Web.ComboBox();
            this.lblCurrentTheme = new Wisej.Web.Label();
            this.pnlNav = new Wisej.Web.Panel();
            this.btnDashboard = new Wisej.Web.Button();
            this.btnTickets = new Wisej.Web.Button();
            this.btnCustomers = new Wisej.Web.Button();
            this.btnJobs = new Wisej.Web.Button();
            this.btnArchitecture = new Wisej.Web.Button();
            this.btnCodeReview = new Wisej.Web.Button();
            this.btnDeployment = new Wisej.Web.Button();
            this.btnNextSteps = new Wisej.Web.Button();
            this.lblNavFooter = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlContent = new Wisej.Web.Panel();
            this.pnlHeader.SuspendLayout();
            this.pnlNav.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (Dock Top · 64 px)
            //
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlHeader.Controls.Add(this.lblAppTitle);
            this.pnlHeader.Controls.Add(this.lblUser);
            this.pnlHeader.Controls.Add(this.lblThemeCaption);
            this.pnlHeader.Controls.Add(this.cboTheme);
            this.pnlHeader.Controls.Add(this.lblCurrentTheme);
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1348, 64);
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.Location = new System.Drawing.Point(24, 14);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(320, 36);
            this.lblAppTitle.Text = "Mini Helpdesk";
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblUser
            //
            this.lblUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblUser.Location = new System.Drawing.Point(690, 14);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(240, 36);
            this.lblUser.Text = "Signed in as: Support Agent";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblThemeCaption
            //
            this.lblThemeCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblThemeCaption.AutoSize = false;
            this.lblThemeCaption.Location = new System.Drawing.Point(950, 14);
            this.lblThemeCaption.Name = "lblThemeCaption";
            this.lblThemeCaption.Size = new System.Drawing.Size(56, 36);
            this.lblThemeCaption.Text = "Theme";
            this.lblThemeCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cboTheme  (Module 8: Application.LoadTheme restyles every screen live)
            //
            this.cboTheme.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.cboTheme.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboTheme.Items.AddRange(new object[] {
            "Bootstrap-4",
            "BootstrapDark-4",
            "Blue-1",
            "Material-3"});
            this.cboTheme.Location = new System.Drawing.Point(1014, 16);
            this.cboTheme.Name = "cboTheme";
            this.cboTheme.SelectedIndex = 0;
            this.cboTheme.Size = new System.Drawing.Size(160, 32);
            this.cboTheme.ToolTipText = "Application.LoadTheme(name) — the whole app restyles without a reload.";
            this.cboTheme.SelectedIndexChanged += new System.EventHandler(this.cboTheme_SelectedIndexChanged);
            //
            // lblCurrentTheme
            //
            this.lblCurrentTheme.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCurrentTheme.AutoSize = false;
            this.lblCurrentTheme.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCurrentTheme.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCurrentTheme.Location = new System.Drawing.Point(1182, 14);
            this.lblCurrentTheme.Name = "lblCurrentTheme";
            this.lblCurrentTheme.Size = new System.Drawing.Size(150, 36);
            this.lblCurrentTheme.Text = "Current: Bootstrap-4";
            this.lblCurrentTheme.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlNav  (Dock Left · 220 px · eight buttons 188×44, 6 px apart)
            //
            this.pnlNav.BackColor = System.Drawing.Color.White;
            this.pnlNav.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlNav.Controls.Add(this.btnDashboard);
            this.pnlNav.Controls.Add(this.btnTickets);
            this.pnlNav.Controls.Add(this.btnCustomers);
            this.pnlNav.Controls.Add(this.btnJobs);
            this.pnlNav.Controls.Add(this.btnArchitecture);
            this.pnlNav.Controls.Add(this.btnCodeReview);
            this.pnlNav.Controls.Add(this.btnDeployment);
            this.pnlNav.Controls.Add(this.btnNextSteps);
            this.pnlNav.Controls.Add(this.lblNavFooter);
            this.pnlNav.Dock = Wisej.Web.DockStyle.Left;
            this.pnlNav.Name = "pnlNav";
            this.pnlNav.Size = new System.Drawing.Size(220, 580);
            //
            // btnDashboard
            //
            this.btnDashboard.Location = new System.Drawing.Point(16, 20);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(188, 44);
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnTickets
            //
            this.btnTickets.Location = new System.Drawing.Point(16, 70);
            this.btnTickets.Name = "btnTickets";
            this.btnTickets.Size = new System.Drawing.Size(188, 44);
            this.btnTickets.Text = "Tickets";
            this.btnTickets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTickets.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnCustomers
            //
            this.btnCustomers.Location = new System.Drawing.Point(16, 120);
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.Size = new System.Drawing.Size(188, 44);
            this.btnCustomers.Text = "Customers";
            this.btnCustomers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomers.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnJobs
            //
            this.btnJobs.Location = new System.Drawing.Point(16, 170);
            this.btnJobs.Name = "btnJobs";
            this.btnJobs.Size = new System.Drawing.Size(188, 44);
            this.btnJobs.Text = "Jobs";
            this.btnJobs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnJobs.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnArchitecture
            //
            this.btnArchitecture.Location = new System.Drawing.Point(16, 220);
            this.btnArchitecture.Name = "btnArchitecture";
            this.btnArchitecture.Size = new System.Drawing.Size(188, 44);
            this.btnArchitecture.Text = "Architecture";
            this.btnArchitecture.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnArchitecture.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnCodeReview
            //
            this.btnCodeReview.Location = new System.Drawing.Point(16, 270);
            this.btnCodeReview.Name = "btnCodeReview";
            this.btnCodeReview.Size = new System.Drawing.Size(188, 44);
            this.btnCodeReview.Text = "Code Review";
            this.btnCodeReview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCodeReview.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnDeployment
            //
            this.btnDeployment.Location = new System.Drawing.Point(16, 320);
            this.btnDeployment.Name = "btnDeployment";
            this.btnDeployment.Size = new System.Drawing.Size(188, 44);
            this.btnDeployment.Text = "Deployment";
            this.btnDeployment.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDeployment.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNextSteps
            //
            this.btnNextSteps.Location = new System.Drawing.Point(16, 370);
            this.btnNextSteps.Name = "btnNextSteps";
            this.btnNextSteps.Size = new System.Drawing.Size(188, 44);
            this.btnNextSteps.Text = "Next Steps";
            this.btnNextSteps.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNextSteps.Click += new System.EventHandler(this.NavButton_Click);
            //
            // lblNavFooter
            //
            this.lblNavFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.lblNavFooter.AutoSize = false;
            this.lblNavFooter.Font = new System.Drawing.Font("monospace", 8F);
            this.lblNavFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNavFooter.Location = new System.Drawing.Point(16, 520);
            this.lblNavFooter.Name = "lblNavFooter";
            this.lblNavFooter.Size = new System.Drawing.Size(188, 44);
            this.lblNavFooter.Text = "Module 10 · Capstone\nWindow1 = shell only\nlogic → Services/";
            this.lblNavFooter.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            //
            // lblStatus  (Dock Bottom · 36 px — the shell's status bar; each screen also has its own lblStatus)
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.White;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(24, 0, 24, 0);
            this.lblStatus.Size = new System.Drawing.Size(1348, 36);
            this.lblStatus.Text = "● starting…";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlContent  (Dock Fill · 24 px padding · one screen at a time, swapped by NavigateTo)
            //
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.pnlContent.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new Wisej.Web.Padding(24);
            //
            // Window1
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlNav);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlHeader);
            this.Name = "Window1";
            this.Text = "WisejTrainingApp — Mini Helpdesk (Capstone)";
            this.Load += new System.EventHandler(this.Window1_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlNav.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Label lblUser;
        private Wisej.Web.Label lblThemeCaption;
        private Wisej.Web.ComboBox cboTheme;
        private Wisej.Web.Label lblCurrentTheme;
        private Wisej.Web.Panel pnlNav;
        private Wisej.Web.Button btnDashboard;
        private Wisej.Web.Button btnTickets;
        private Wisej.Web.Button btnCustomers;
        private Wisej.Web.Button btnJobs;
        private Wisej.Web.Button btnArchitecture;
        private Wisej.Web.Button btnCodeReview;
        private Wisej.Web.Button btnDeployment;
        private Wisej.Web.Button btnNextSteps;
        private Wisej.Web.Label lblNavFooter;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlContent;
    }
}
