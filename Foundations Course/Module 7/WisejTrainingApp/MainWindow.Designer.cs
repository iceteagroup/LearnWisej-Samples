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
            this.components = new System.ComponentModel.Container();
            this.panelHeader = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.lblModule = new Wisej.Web.Label();
            this.lblThemeCaption = new Wisej.Web.Label();
            this.cboTheme = new Wisej.Web.ComboBox();
            this.lblCurrentTheme = new Wisej.Web.Label();
            this.btnTryUnknownTheme = new Wisej.Web.Button();
            this.panelNav = new Wisej.Web.Panel();
            this.lblNavHeading = new Wisej.Web.Label();
            this.btnDashboard = new Wisej.Web.Button();
            this.btnTickets = new Wisej.Web.Button();
            this.btnCustomers = new Wisej.Web.Button();
            this.btnReports = new Wisej.Web.Button();
            this.btnSettings = new Wisej.Web.Button();
            this.lblNavFooter = new Wisej.Web.Label();
            this.panelContent = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.panelHeader.SuspendLayout();
            this.panelNav.SuspendLayout();
            this.SuspendLayout();
            //
            // panelHeader  (Dock Top · title, module label, theme selector)
            //
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.lblAppTitle);
            this.panelHeader.Controls.Add(this.lblModule);
            this.panelHeader.Controls.Add(this.lblThemeCaption);
            this.panelHeader.Controls.Add(this.cboTheme);
            this.panelHeader.Controls.Add(this.lblCurrentTheme);
            this.panelHeader.Controls.Add(this.btnTryUnknownTheme);
            this.panelHeader.Dock = Wisej.Web.DockStyle.Top;
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1348, 64);
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.Location = new System.Drawing.Point(24, 10);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(240, 30);
            this.lblAppTitle.Text = "ServiceDesk";
            //
            // lblModule
            //
            this.lblModule.AutoSize = false;
            this.lblModule.Font = new System.Drawing.Font("default", 9F);
            this.lblModule.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblModule.Location = new System.Drawing.Point(24, 38);
            this.lblModule.Name = "lblModule";
            this.lblModule.Size = new System.Drawing.Size(400, 20);
            this.lblModule.Text = "Module 7 · Theming & UI Modernization";
            //
            // lblThemeCaption
            //
            this.lblThemeCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblThemeCaption.AutoSize = false;
            this.lblThemeCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblThemeCaption.Location = new System.Drawing.Point(1064, 16);
            this.lblThemeCaption.Name = "lblThemeCaption";
            this.lblThemeCaption.Size = new System.Drawing.Size(52, 32);
            this.lblThemeCaption.Text = "Theme";
            this.lblThemeCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cboTheme  (lab step 3: the theme selector — DropDownList, one entry per built-in theme)
            //
            this.cboTheme.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.cboTheme.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboTheme.Items.AddRange(new object[] {
            "Bootstrap-4",
            "BootstrapDark-4",
            "Blue-1",
            "Classic-2",
            "Material-3",
            "FluentLight-5"});
            this.cboTheme.Location = new System.Drawing.Point(1124, 16);
            this.cboTheme.Name = "cboTheme";
            this.cboTheme.Size = new System.Drawing.Size(200, 32);
            this.cboTheme.ToolTipText = "Applies a built-in Wisej.NET theme live — the look changes, not what the app does.";
            this.cboTheme.SelectedIndex = 0;
            this.cboTheme.SelectedIndexChanged += new System.EventHandler(this.cboTheme_SelectedIndexChanged);
            //
            // lblCurrentTheme
            //
            this.lblCurrentTheme.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCurrentTheme.AutoSize = false;
            this.lblCurrentTheme.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblCurrentTheme.Location = new System.Drawing.Point(830, 16);
            this.lblCurrentTheme.Name = "lblCurrentTheme";
            this.lblCurrentTheme.Size = new System.Drawing.Size(226, 32);
            this.lblCurrentTheme.Text = "Current theme: Bootstrap-4";
            this.lblCurrentTheme.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnTryUnknownTheme  (failure path: a theme name that does not exist)
            //
            this.btnTryUnknownTheme.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnTryUnknownTheme.Location = new System.Drawing.Point(640, 16);
            this.btnTryUnknownTheme.Name = "btnTryUnknownTheme";
            this.btnTryUnknownTheme.Size = new System.Drawing.Size(178, 32);
            this.btnTryUnknownTheme.Text = "Try an unknown theme";
            this.btnTryUnknownTheme.ToolTipText = "Asks for theme \"Foo-9\": the app reports it, keeps the current theme and logs it. Pick a real theme to recover.";
            this.btnTryUnknownTheme.Click += new System.EventHandler(this.btnTryUnknownTheme_Click);
            //
            // panelNav  (Dock Left · Dashboard · Tickets · Customers · Reports · Settings)
            //
            this.panelNav.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.panelNav.Controls.Add(this.lblNavHeading);
            this.panelNav.Controls.Add(this.btnDashboard);
            this.panelNav.Controls.Add(this.btnTickets);
            this.panelNav.Controls.Add(this.btnCustomers);
            this.panelNav.Controls.Add(this.btnReports);
            this.panelNav.Controls.Add(this.btnSettings);
            this.panelNav.Controls.Add(this.lblNavFooter);
            this.panelNav.Dock = Wisej.Web.DockStyle.Left;
            this.panelNav.Name = "panelNav";
            this.panelNav.Size = new System.Drawing.Size(200, 620);
            //
            // lblNavHeading
            //
            this.lblNavHeading.AutoSize = false;
            this.lblNavHeading.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblNavHeading.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNavHeading.Location = new System.Drawing.Point(16, 20);
            this.lblNavHeading.Name = "lblNavHeading";
            this.lblNavHeading.Size = new System.Drawing.Size(168, 20);
            this.lblNavHeading.Text = "NAVIGATION";
            //
            // btnDashboard  (nav buttons: 168 × 40, 48 px apart — SetActiveButton() styles the selected one)
            //
            this.btnDashboard.Location = new System.Drawing.Point(16, 48);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(168, 40);
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            //
            // btnTickets
            //
            this.btnTickets.Location = new System.Drawing.Point(16, 96);
            this.btnTickets.Name = "btnTickets";
            this.btnTickets.Size = new System.Drawing.Size(168, 40);
            this.btnTickets.Text = "Tickets";
            this.btnTickets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTickets.Click += new System.EventHandler(this.btnTickets_Click);
            //
            // btnCustomers
            //
            this.btnCustomers.Location = new System.Drawing.Point(16, 144);
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.Size = new System.Drawing.Size(168, 40);
            this.btnCustomers.Text = "Customers";
            this.btnCustomers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomers.Click += new System.EventHandler(this.btnCustomers_Click);
            //
            // btnReports
            //
            this.btnReports.Location = new System.Drawing.Point(16, 192);
            this.btnReports.Name = "btnReports";
            this.btnReports.Size = new System.Drawing.Size(168, 40);
            this.btnReports.Text = "Reports";
            this.btnReports.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReports.Click += new System.EventHandler(this.btnReports_Click);
            //
            // btnSettings
            //
            this.btnSettings.Location = new System.Drawing.Point(16, 240);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(168, 40);
            this.btnSettings.Text = "Settings";
            this.btnSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            //
            // lblNavFooter
            //
            this.lblNavFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.lblNavFooter.AutoSize = false;
            this.lblNavFooter.Font = new System.Drawing.Font("monospace", 8F);
            this.lblNavFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNavFooter.Location = new System.Drawing.Point(16, 560);
            this.lblNavFooter.Name = "lblNavFooter";
            this.lblNavFooter.Size = new System.Drawing.Size(168, 48);
            this.lblNavFooter.Text = "NavigateTo(page)\nSetActiveButton(btn)";
            this.lblNavFooter.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            //
            // panelContent  (Dock Fill · one Views/*View at a time)
            //
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.panelContent.Dock = Wisej.Web.DockStyle.Fill;
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1148, 620);
            //
            // lblStatus  (Dock Bottom)
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.White;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(24, 0, 24, 0);
            this.lblStatus.Size = new System.Drawing.Size(1348, 36);
            this.lblStatus.Text = "● ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // MainWindow
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 720);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelNav);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.panelHeader);
            this.Name = "MainWindow";
            this.Text = "WisejTrainingApp — Modern dashboard (Module 7)";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.panelHeader.ResumeLayout(false);
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
        private Wisej.Web.Button btnTryUnknownTheme;
        private Wisej.Web.Panel panelNav;
        private Wisej.Web.Label lblNavHeading;
        private Wisej.Web.Button btnDashboard;
        private Wisej.Web.Button btnTickets;
        private Wisej.Web.Button btnCustomers;
        private Wisej.Web.Button btnReports;
        private Wisej.Web.Button btnSettings;
        private Wisej.Web.Label lblNavFooter;
        private Wisej.Web.Panel panelContent;
        private Wisej.Web.Label lblStatus;
    }
}
