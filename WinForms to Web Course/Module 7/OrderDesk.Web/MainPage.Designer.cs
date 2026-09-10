namespace OrderDesk
{
    partial class MainPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used. The page unsubscribes from the process-wide AuditLog and the
        /// session-wide Application events: both outlive the page, so a forgotten handler would keep a dead
        /// page alive (the Module 3 "closed but not disposed" lesson, applied to events).
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Services.AuditLog.Added -= this.AuditLog_Added;
                Wisej.Web.Application.ResponsiveProfileChanged -= this.Application_ResponsiveProfileChanged;
                Wisej.Web.Application.BrowserSizeChanged -= this.Application_BrowserSizeChanged;
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelContent = new Wisej.Web.Panel();
            this.panelViews = new Wisej.Web.Panel();
            this.dashboardView = new OrderDesk.Pages.DashboardView();
            this.ordersView = new OrderDesk.Pages.OrdersView();
            this.customersView = new OrderDesk.Pages.CustomersView();
            this.reportsView = new OrderDesk.Pages.ReportsView();
            this.settingsView = new OrderDesk.Pages.SettingsView();
            this.panelNav = new Wisej.Web.Panel();
            this.buttonNavDashboard = new Wisej.Web.Button();
            this.buttonNavOrders = new Wisej.Web.Button();
            this.buttonNavCustomers = new Wisej.Web.Button();
            this.buttonNavReports = new Wisej.Web.Button();
            this.buttonNavSettings = new Wisej.Web.Button();
            this.panelButtons = new Wisej.Web.Panel();
            this.labelBanner = new Wisej.Web.Label();
            this.buttonRefresh = new Wisej.Web.Button();
            this.buttonSimulate = new Wisej.Web.Button();
            this.buttonResponsive = new Wisej.Web.Button();
            this.buttonGuardedDownload = new Wisej.Web.Button();
            this.buttonHealth = new Wisej.Web.Button();
            this.buttonReadiness = new Wisej.Web.Button();
            this.panelAppBar = new Wisej.Web.Panel();
            this.labelTitle = new Wisej.Web.Label();
            this.comboTheme = new Wisej.Web.ComboBox();
            this.comboUser = new Wisej.Web.ComboBox();
            this.labelProfile = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.trace = new OrderDesk.Shared.TracePanel();
            this.panelContent.SuspendLayout();
            this.panelViews.SuspendLayout();
            this.panelNav.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.panelAppBar.SuspendLayout();
            this.SuspendLayout();
            //
            // panelContent  (everything left of the trace: app bar, nav rail, views, button bar)
            //
            this.panelContent.BackColor = OrderDesk.Shared.Palette.PageBackground;
            this.panelContent.Controls.Add(this.panelViews);
            this.panelContent.Controls.Add(this.panelNav);
            this.panelContent.Controls.Add(this.panelButtons);
            this.panelContent.Controls.Add(this.panelAppBar);
            this.panelContent.Dock = Wisej.Web.DockStyle.Fill;
            this.panelContent.Name = "panelContent";
            //
            // panelViews  (hosts the five navigation views, Dock Fill, one visible at a time)
            //
            this.panelViews.BackColor = OrderDesk.Shared.Palette.PageBackground;
            this.panelViews.Controls.Add(this.dashboardView);
            this.panelViews.Controls.Add(this.ordersView);
            this.panelViews.Controls.Add(this.customersView);
            this.panelViews.Controls.Add(this.reportsView);
            this.panelViews.Controls.Add(this.settingsView);
            this.panelViews.Dock = Wisej.Web.DockStyle.Fill;
            this.panelViews.Name = "panelViews";
            //
            // views
            //
            this.dashboardView.Name = "dashboardView";
            this.dashboardView.Visible = true;
            this.ordersView.Name = "ordersView";
            this.ordersView.Visible = false;
            this.customersView.Name = "customersView";
            this.customersView.Visible = false;
            this.reportsView.Name = "reportsView";
            this.reportsView.Visible = false;
            this.settingsView.Name = "settingsView";
            this.settingsView.Visible = false;
            //
            // panelNav  (left navigation rail: Dashboard · Orders · Customers · Reports · Settings)
            //
            this.panelNav.BackColor = OrderDesk.Shared.Palette.CardBackground;
            this.panelNav.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelNav.Controls.Add(this.buttonNavDashboard);
            this.panelNav.Controls.Add(this.buttonNavOrders);
            this.panelNav.Controls.Add(this.buttonNavCustomers);
            this.panelNav.Controls.Add(this.buttonNavReports);
            this.panelNav.Controls.Add(this.buttonNavSettings);
            this.panelNav.Dock = Wisej.Web.DockStyle.Left;
            this.panelNav.Name = "panelNav";
            this.panelNav.Width = 150;
            //
            // nav buttons
            //
            this.buttonNavDashboard.Location = new System.Drawing.Point(8, 12);
            this.buttonNavDashboard.Name = "buttonNavDashboard";
            this.buttonNavDashboard.Size = new System.Drawing.Size(132, 36);
            this.buttonNavDashboard.Tag = "Dashboard";
            this.buttonNavDashboard.Text = "⌂  Dashboard";
            this.buttonNavDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonNavDashboard.Click += new System.EventHandler(this.buttonNav_Click);
            this.buttonNavOrders.Location = new System.Drawing.Point(8, 54);
            this.buttonNavOrders.Name = "buttonNavOrders";
            this.buttonNavOrders.Size = new System.Drawing.Size(132, 36);
            this.buttonNavOrders.Tag = "Orders";
            this.buttonNavOrders.Text = "≡  Orders";
            this.buttonNavOrders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonNavOrders.Click += new System.EventHandler(this.buttonNav_Click);
            this.buttonNavCustomers.Location = new System.Drawing.Point(8, 96);
            this.buttonNavCustomers.Name = "buttonNavCustomers";
            this.buttonNavCustomers.Size = new System.Drawing.Size(132, 36);
            this.buttonNavCustomers.Tag = "Customers";
            this.buttonNavCustomers.Text = "☺  Customers";
            this.buttonNavCustomers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonNavCustomers.Click += new System.EventHandler(this.buttonNav_Click);
            this.buttonNavReports.Location = new System.Drawing.Point(8, 138);
            this.buttonNavReports.Name = "buttonNavReports";
            this.buttonNavReports.Size = new System.Drawing.Size(132, 36);
            this.buttonNavReports.Tag = "Reports";
            this.buttonNavReports.Text = "▤  Reports";
            this.buttonNavReports.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonNavReports.Click += new System.EventHandler(this.buttonNav_Click);
            this.buttonNavSettings.Location = new System.Drawing.Point(8, 180);
            this.buttonNavSettings.Name = "buttonNavSettings";
            this.buttonNavSettings.Size = new System.Drawing.Size(132, 36);
            this.buttonNavSettings.Tag = "Settings";
            this.buttonNavSettings.Text = "⚙  Settings";
            this.buttonNavSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonNavSettings.Click += new System.EventHandler(this.buttonNav_Click);
            //
            // panelButtons  (bottom bar: banner + the lab buttons; buttons at page y ≈ 694)
            //
            this.panelButtons.BackColor = OrderDesk.Shared.Palette.PageBackground;
            this.panelButtons.Controls.Add(this.labelBanner);
            this.panelButtons.Controls.Add(this.buttonRefresh);
            this.panelButtons.Controls.Add(this.buttonSimulate);
            this.panelButtons.Controls.Add(this.buttonResponsive);
            this.panelButtons.Controls.Add(this.buttonGuardedDownload);
            this.panelButtons.Controls.Add(this.buttonHealth);
            this.panelButtons.Controls.Add(this.buttonReadiness);
            this.panelButtons.Dock = Wisej.Web.DockStyle.Bottom;
            this.panelButtons.Height = 100;
            this.panelButtons.Name = "panelButtons";
            //
            // labelBanner  (alarm / finding banner: hidden until something happens)
            //
            this.labelBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner.AutoSize = false;
            this.labelBanner.AutoEllipsis = true;
            this.labelBanner.BackColor = OrderDesk.Shared.Palette.BadSoft;
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = OrderDesk.Shared.Palette.Bad;
            this.labelBanner.Location = new System.Drawing.Point(12, 4);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.labelBanner.Size = new System.Drawing.Size(816, 24);
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // lab buttons (one row)
            //
            this.buttonRefresh.Location = new System.Drawing.Point(12, 34);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(140, 34);
            this.buttonRefresh.Text = "Refresh dashboard ✓";
            this.buttonRefresh.ToolTipText = "Success path: DashboardMetrics.Compute(OrderService) → KPI cards, chart, recent grid; writes an audit entry pushed to every open page.";
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            this.buttonSimulate.Location = new System.Drawing.Point(160, 34);
            this.buttonSimulate.Name = "buttonSimulate";
            this.buttonSimulate.Size = new System.Drawing.Size(120, 34);
            this.buttonSimulate.Text = "Simulate activity";
            this.buttonSimulate.ToolTipText = "Progress path: Application.StartTask plays 5 audit events as sam (session B) and pushes each into the feed with Application.Update(page).";
            this.buttonSimulate.Click += new System.EventHandler(this.buttonSimulate_Click);
            this.buttonResponsive.Location = new System.Drawing.Point(288, 34);
            this.buttonResponsive.Name = "buttonResponsive";
            this.buttonResponsive.Size = new System.Drawing.Size(125, 34);
            this.buttonResponsive.Text = "Responsive check";
            this.buttonResponsive.ToolTipText = "Logs Application.ActiveProfile.Name, Application.Browser.Size / Device and re-applies the layout for the active profile.";
            this.buttonResponsive.Click += new System.EventHandler(this.buttonResponsive_Click);
            this.buttonGuardedDownload.Location = new System.Drawing.Point(421, 34);
            this.buttonGuardedDownload.Name = "buttonGuardedDownload";
            this.buttonGuardedDownload.Size = new System.Drawing.Size(130, 34);
            this.buttonGuardedDownload.Text = "Guarded download";
            this.buttonGuardedDownload.ToolTipText = "Failure/recovery: customers-with-taxid.csv needs Manager — kelly (Clerk) gets 403, dana (Manager) gets the file. Pick the user in the app bar.";
            this.buttonGuardedDownload.Click += new System.EventHandler(this.buttonGuardedDownload_Click);
            this.buttonHealth.Location = new System.Drawing.Point(559, 34);
            this.buttonHealth.Name = "buttonHealth";
            this.buttonHealth.Size = new System.Drawing.Size(100, 34);
            this.buttonHealth.Text = "Health check";
            this.buttonHealth.ToolTipText = "Builds the same JSON GET /health returns (Startup.cs app.MapGet) and shows it in the Deployment card.";
            this.buttonHealth.Click += new System.EventHandler(this.buttonHealth_Click);
            this.buttonReadiness.Location = new System.Drawing.Point(667, 34);
            this.buttonReadiness.Name = "buttonReadiness";
            this.buttonReadiness.Size = new System.Drawing.Size(135, 34);
            this.buttonReadiness.Text = "Readiness check ✓";
            this.buttonReadiness.ToolTipText = "Runs the automated parts of the final readiness checklist (static-field scan, desktop references, open forms, AllowHtml usage) and reports N/8.";
            this.buttonReadiness.Click += new System.EventHandler(this.buttonReadiness_Click);
            //
            // panelAppBar  ("OrderDesk · Operations": title, theme switch, user, profile chip, status)
            //
            this.panelAppBar.BackColor = OrderDesk.Shared.Palette.Accent;
            this.panelAppBar.Controls.Add(this.labelTitle);
            this.panelAppBar.Controls.Add(this.comboTheme);
            this.panelAppBar.Controls.Add(this.comboUser);
            this.panelAppBar.Controls.Add(this.labelProfile);
            this.panelAppBar.Controls.Add(this.labelStatus);
            this.panelAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.panelAppBar.Height = 44;
            this.panelAppBar.Name = "panelAppBar";
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(16, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(240, 44);
            this.labelTitle.Text = "OrderDesk · Operations";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // comboTheme  (Application.LoadTheme)
            //
            this.comboTheme.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.comboTheme.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboTheme.Items.AddRange(new object[] { "Bootstrap-4", "Material-3", "FluentDark-5" });
            this.comboTheme.Location = new System.Drawing.Point(262, 8);
            this.comboTheme.Name = "comboTheme";
            this.comboTheme.Size = new System.Drawing.Size(124, 28);
            this.comboTheme.ToolTipText = "Theme — Application.LoadTheme(name) restyles the running app; no form is rewritten.";
            this.comboTheme.SelectedIndexChanged += new System.EventHandler(this.comboTheme_SelectedIndexChanged);
            //
            // comboUser  (UserContext.Current.User — per session, never a static)
            //
            this.comboUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.comboUser.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboUser.Items.AddRange(new object[] { "kelly · Clerk", "dana · Manager" });
            this.comboUser.Location = new System.Drawing.Point(394, 8);
            this.comboUser.Name = "comboUser";
            this.comboUser.Size = new System.Drawing.Size(124, 28);
            this.comboUser.ToolTipText = "Signed-in user of THIS session (UserContext in Application.Session). The guarded download checks the role on the server.";
            this.comboUser.SelectedIndexChanged += new System.EventHandler(this.comboUser_SelectedIndexChanged);
            //
            // labelProfile  (active client profile + browser size)
            //
            this.labelProfile.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelProfile.AutoSize = false;
            this.labelProfile.Font = new System.Drawing.Font("default", 9F);
            this.labelProfile.ForeColor = System.Drawing.Color.White;
            this.labelProfile.Location = new System.Drawing.Point(526, 0);
            this.labelProfile.Name = "labelProfile";
            this.labelProfile.Size = new System.Drawing.Size(196, 44);
            this.labelProfile.Text = "profile —";
            this.labelProfile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // labelStatus  (● idle / working / alarm)
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.White;
            this.labelStatus.Location = new System.Drawing.Point(728, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(100, 44);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // trace  (Server ⇄ Client migration trace, docked right)
            //
            this.trace.Dock = Wisej.Web.DockStyle.Right;
            this.trace.Name = "trace";
            this.trace.Width = 560;
            //
            // MainPage
            //
            this.BackColor = OrderDesk.Shared.Palette.PageBackground;
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.trace);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1400, 760);
            this.Text = "OrderDesk · Operations";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelContent.ResumeLayout(false);
            this.panelViews.ResumeLayout(false);
            this.panelNav.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.panelAppBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelContent;
        private Wisej.Web.Panel panelViews;
        private OrderDesk.Pages.DashboardView dashboardView;
        private OrderDesk.Pages.OrdersView ordersView;
        private OrderDesk.Pages.CustomersView customersView;
        private OrderDesk.Pages.ReportsView reportsView;
        private OrderDesk.Pages.SettingsView settingsView;
        private Wisej.Web.Panel panelNav;
        private Wisej.Web.Button buttonNavDashboard;
        private Wisej.Web.Button buttonNavOrders;
        private Wisej.Web.Button buttonNavCustomers;
        private Wisej.Web.Button buttonNavReports;
        private Wisej.Web.Button buttonNavSettings;
        private Wisej.Web.Panel panelButtons;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Button buttonRefresh;
        private Wisej.Web.Button buttonSimulate;
        private Wisej.Web.Button buttonResponsive;
        private Wisej.Web.Button buttonGuardedDownload;
        private Wisej.Web.Button buttonHealth;
        private Wisej.Web.Button buttonReadiness;
        private Wisej.Web.Panel panelAppBar;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.ComboBox comboTheme;
        private Wisej.Web.ComboBox comboUser;
        private Wisej.Web.Label labelProfile;
        private Wisej.Web.Label labelStatus;
        private OrderDesk.Shared.TracePanel trace;
    }
}
