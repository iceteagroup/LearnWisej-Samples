namespace WisejTrainingApp
{
    partial class MainWindow
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
            this.panelHeader = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.lblModule = new Wisej.Web.Label();
            this.lblThemeCaption = new Wisej.Web.Label();
            this.cboTheme = new Wisej.Web.ComboBox();
            this.lblCurrentTheme = new Wisej.Web.Label();
            this.panelNav = new Wisej.Web.Panel();
            this.btnDashboard = new Wisej.Web.Button();
            this.btnTickets = new Wisej.Web.Button();
            this.btnCustomers = new Wisej.Web.Button();
            this.btnReports = new Wisej.Web.Button();
            this.btnSettings = new Wisej.Web.Button();
            this.panelContent = new Wisej.Web.Panel();
            this.panelHeader.SuspendLayout();
            this.panelNav.SuspendLayout();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.Controls.Add(this.lblAppTitle);
            this.panelHeader.Controls.Add(this.lblModule);
            this.panelHeader.Controls.Add(this.lblThemeCaption);
            this.panelHeader.Controls.Add(this.cboTheme);
            this.panelHeader.Controls.Add(this.lblCurrentTheme);
            this.panelHeader.Dock = Wisej.Web.DockStyle.Top;
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1100, 68);
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = true;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.Location = new System.Drawing.Point(20, 8);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Text = "ServiceDesk";
            //
            // lblModule
            //
            this.lblModule.AutoSize = true;
            this.lblModule.Location = new System.Drawing.Point(20, 40);
            this.lblModule.Name = "lblModule";
            this.lblModule.Text = "Module 7 · Theming and UI modernization";
            //
            // lblThemeCaption
            //
            this.lblThemeCaption.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
            this.lblThemeCaption.AutoSize = true;
            this.lblThemeCaption.Location = new System.Drawing.Point(660, 24);
            this.lblThemeCaption.Name = "lblThemeCaption";
            this.lblThemeCaption.Text = "Theme";
            //
            // cboTheme
            //
            this.cboTheme.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
            this.cboTheme.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboTheme.Items.AddRange(new object[] {
            "Bootstrap-4",
            "BootstrapDark-4",
            "Blue-1",
            "Classic-2"});
            this.cboTheme.Location = new System.Drawing.Point(712, 18);
            this.cboTheme.Name = "cboTheme";
            this.cboTheme.Size = new System.Drawing.Size(170, 30);
            this.cboTheme.SelectedIndexChanged += new System.EventHandler(this.cboTheme_SelectedIndexChanged);
            //
            // lblCurrentTheme
            //
            this.lblCurrentTheme.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
            this.lblCurrentTheme.Location = new System.Drawing.Point(896, 24);
            this.lblCurrentTheme.Name = "lblCurrentTheme";
            this.lblCurrentTheme.Size = new System.Drawing.Size(190, 20);
            this.lblCurrentTheme.Text = "Current theme:";
            //
            // panelNav
            //
            this.panelNav.Controls.Add(this.btnDashboard);
            this.panelNav.Controls.Add(this.btnTickets);
            this.panelNav.Controls.Add(this.btnCustomers);
            this.panelNav.Controls.Add(this.btnReports);
            this.panelNav.Controls.Add(this.btnSettings);
            this.panelNav.Dock = Wisej.Web.DockStyle.Left;
            this.panelNav.Name = "panelNav";
            this.panelNav.Size = new System.Drawing.Size(180, 572);
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
            // btnReports
            //
            this.btnReports.Location = new System.Drawing.Point(12, 148);
            this.btnReports.Name = "btnReports";
            this.btnReports.Size = new System.Drawing.Size(156, 36);
            this.btnReports.Text = "Reports";
            this.btnReports.Click += new System.EventHandler(this.btnReports_Click);
            //
            // btnSettings
            //
            this.btnSettings.Location = new System.Drawing.Point(12, 192);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(156, 36);
            this.btnSettings.Text = "Settings";
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            //
            // panelContent
            //
            this.panelContent.Dock = Wisej.Web.DockStyle.Fill;
            this.panelContent.Name = "panelContent";
            this.panelContent.Padding = new Wisej.Web.Padding(20);
            //
            // MainWindow
            //
            this.ClientSize = new System.Drawing.Size(1100, 640);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelNav);
            this.Controls.Add(this.panelHeader);
            this.Name = "MainWindow";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "ServiceDesk";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelNav.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelHeader;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Label lblModule;
        private Wisej.Web.Label lblThemeCaption;
        private Wisej.Web.ComboBox cboTheme;
        private Wisej.Web.Label lblCurrentTheme;
        private Wisej.Web.Panel panelNav;
        private Wisej.Web.Button btnDashboard;
        private Wisej.Web.Button btnTickets;
        private Wisej.Web.Button btnCustomers;
        private Wisej.Web.Button btnReports;
        private Wisej.Web.Button btnSettings;
        private Wisej.Web.Panel panelContent;
    }
}
