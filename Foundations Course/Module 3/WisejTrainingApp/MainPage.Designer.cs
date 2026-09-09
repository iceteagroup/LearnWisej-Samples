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
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.lblBreadcrumb = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.lblRoleCaption = new Wisej.Web.Label();
            this.cboRole = new Wisej.Web.ComboBox();
            this.pnlNav = new Wisej.Web.Panel();
            this.lblNavCaption = new Wisej.Web.Label();
            this.btnDashboard = new Wisej.Web.Button();
            this.btnTickets = new Wisej.Web.Button();
            this.btnCustomers = new Wisej.Web.Button();
            this.btnSettings = new Wisej.Web.Button();
            this.lblNavHint = new Wisej.Web.Label();
            this.pnlContent = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlNav.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (Dock = Top: app name, current user, breadcrumb, role picker)
            //
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlHeader.Controls.Add(this.lblAppTitle);
            this.pnlHeader.Controls.Add(this.lblBreadcrumb);
            this.pnlHeader.Controls.Add(this.lblUser);
            this.pnlHeader.Controls.Add(this.lblRoleCaption);
            this.pnlHeader.Controls.Add(this.cboRole);
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1348, 64);
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.Location = new System.Drawing.Point(24, 8);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(420, 30);
            this.lblAppTitle.Text = "ServiceDesk Application";
            //
            // lblBreadcrumb  (updated by NavigateTo: "Home / Tickets")
            //
            this.lblBreadcrumb.AutoSize = false;
            this.lblBreadcrumb.Font = new System.Drawing.Font("default", 9F);
            this.lblBreadcrumb.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblBreadcrumb.Location = new System.Drawing.Point(24, 38);
            this.lblBreadcrumb.Name = "lblBreadcrumb";
            this.lblBreadcrumb.Size = new System.Drawing.Size(420, 20);
            this.lblBreadcrumb.Text = "Home / Dashboard";
            //
            // lblUser  (anchored Top + Right so it stays at the right edge when the browser resizes)
            //
            this.lblUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblUser.Location = new System.Drawing.Point(888, 8);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(436, 24);
            this.lblUser.Text = "Signed in as: Support Agent";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblRoleCaption
            //
            this.lblRoleCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblRoleCaption.AutoSize = false;
            this.lblRoleCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblRoleCaption.Location = new System.Drawing.Point(1024, 34);
            this.lblRoleCaption.Name = "lblRoleCaption";
            this.lblRoleCaption.Size = new System.Drawing.Size(120, 26);
            this.lblRoleCaption.Text = "Role (no login yet):";
            this.lblRoleCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cboRole  (a stand-in for login: switch role → ApplyPermissions + rebuild the current view)
            //
            this.cboRole.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.cboRole.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboRole.Items.AddRange(new object[] { "Support Agent", "Manager" });
            this.cboRole.Location = new System.Drawing.Point(1150, 34);
            this.cboRole.Name = "cboRole";
            this.cboRole.Size = new System.Drawing.Size(174, 26);
            this.cboRole.ToolTipText = "Pretend to be a Support Agent or a Manager; the views apply the role matrix.";
            this.cboRole.SelectedIndexChanged += new System.EventHandler(this.cboRole_SelectedIndexChanged);
            //
            // pnlNav  (Dock = Left: the pages the user can open)
            //
            this.pnlNav.BackColor = System.Drawing.Color.White;
            this.pnlNav.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlNav.Controls.Add(this.lblNavCaption);
            this.pnlNav.Controls.Add(this.btnDashboard);
            this.pnlNav.Controls.Add(this.btnTickets);
            this.pnlNav.Controls.Add(this.btnCustomers);
            this.pnlNav.Controls.Add(this.btnSettings);
            this.pnlNav.Controls.Add(this.lblNavHint);
            this.pnlNav.Dock = Wisej.Web.DockStyle.Left;
            this.pnlNav.Location = new System.Drawing.Point(0, 64);
            this.pnlNav.Name = "pnlNav";
            this.pnlNav.Size = new System.Drawing.Size(220, 584);
            //
            // lblNavCaption
            //
            this.lblNavCaption.AutoSize = false;
            this.lblNavCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblNavCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNavCaption.Location = new System.Drawing.Point(16, 16);
            this.lblNavCaption.Name = "lblNavCaption";
            this.lblNavCaption.Size = new System.Drawing.Size(188, 20);
            this.lblNavCaption.Text = "NAVIGATION";
            //
            // btnDashboard  (Anchor Top + Left + Right inside the nav panel: even width)
            //
            this.btnDashboard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnDashboard.Location = new System.Drawing.Point(16, 44);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(188, 40);
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            //
            // btnTickets
            //
            this.btnTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnTickets.Location = new System.Drawing.Point(16, 92);
            this.btnTickets.Name = "btnTickets";
            this.btnTickets.Size = new System.Drawing.Size(188, 40);
            this.btnTickets.Text = "Tickets";
            this.btnTickets.Click += new System.EventHandler(this.btnTickets_Click);
            //
            // btnCustomers
            //
            this.btnCustomers.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnCustomers.Location = new System.Drawing.Point(16, 140);
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.Size = new System.Drawing.Size(188, 40);
            this.btnCustomers.Text = "Customers";
            this.btnCustomers.Click += new System.EventHandler(this.btnCustomers_Click);
            //
            // btnSettings
            //
            this.btnSettings.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnSettings.Location = new System.Drawing.Point(16, 188);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(188, 40);
            this.btnSettings.Text = "Settings";
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            //
            // lblNavHint
            //
            this.lblNavHint.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblNavHint.AutoSize = false;
            this.lblNavHint.Font = new System.Drawing.Font("monospace", 8F);
            this.lblNavHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNavHint.Location = new System.Drawing.Point(16, 480);
            this.lblNavHint.Name = "lblNavHint";
            this.lblNavHint.Size = new System.Drawing.Size(188, 88);
            this.lblNavHint.Text = "Each button:\n  NavigateTo(\"…\")\n\nShell = Page\nHeader Top · Nav Left\nContent Fill · Status Bottom";
            this.lblNavHint.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            //
            // pnlContent  (Dock = Fill: the current view lives here and nowhere else)
            //
            this.pnlContent.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(220, 64);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new Wisej.Web.Padding(24);
            this.pnlContent.Size = new System.Drawing.Size(1128, 584);
            //
            // lblStatus  (Dock = Bottom: "hh:mm:ss tt - Opened Tickets page.")
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.White;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(0, 648);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(24, 0, 24, 0);
            this.lblStatus.Size = new System.Drawing.Size(1348, 32);
            this.lblStatus.Text = "Application shell loading…";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // MainPage  (a Page: fills the browser, no window chrome, nothing to minimize)
            //
            // Dock order matters: the Fill panel is added first (it is laid out last), then Bottom,
            // then Left, then Top — the same order the Designer writes for a docked layout.
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlNav);
            this.Controls.Add(this.pnlHeader);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "WisejTrainingApp — Application shell (Module 3)";
            this.pnlHeader.ResumeLayout(false);
            this.pnlNav.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Label lblBreadcrumb;
        private Wisej.Web.Label lblUser;
        private Wisej.Web.Label lblRoleCaption;
        private Wisej.Web.ComboBox cboRole;
        private Wisej.Web.Panel pnlNav;
        private Wisej.Web.Label lblNavCaption;
        private Wisej.Web.Button btnDashboard;
        private Wisej.Web.Button btnTickets;
        private Wisej.Web.Button btnCustomers;
        private Wisej.Web.Button btnSettings;
        private Wisej.Web.Label lblNavHint;
        private Wisej.Web.Panel pnlContent;
        private Wisej.Web.Label lblStatus;
    }
}
