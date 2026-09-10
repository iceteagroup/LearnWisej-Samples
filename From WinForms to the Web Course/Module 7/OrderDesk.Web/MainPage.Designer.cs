namespace OrderDesk
{
    partial class MainPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DetachApplicationEvents();      // Application.ResponsiveProfileChanged is a static event: unsubscribe per page
                if (components != null)
                    components.Dispose();       // also stops and disposes timerAudit
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabs = new Wisej.Web.TabControl();
            this.tabOrders = new Wisej.Web.TabPage();
            this.tabSecurity = new Wisej.Web.TabPage();
            this.tabDashboard = new Wisej.Web.TabPage();
            this.tabReadiness = new Wisej.Web.TabPage();
            // Orders tab — the migrated screen
            this.panelOrders = new Wisej.Web.Panel();
            this.labelOrdersTitle = new Wisej.Web.Label();
            this.labelProfileBadge = new Wisej.Web.Label();
            this.textSearch = new Wisej.Web.TextBox();
            this.toolBar = new Wisej.Web.ToolBar();
            this.toolNew = new Wisej.Web.ToolBarButton();
            this.toolPrint = new Wisej.Web.ToolBarButton();
            this.toolExport = new Wisej.Web.ToolBarButton();
            this.toolRefresh = new Wisej.Web.ToolBarButton();
            this.comboActions = new Wisej.Web.ComboBox();
            this.gridOrders = new Wisej.Web.DataGridView();
            this.colOrder = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTotal = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelDetailTitle = new Wisej.Web.Label();
            this.labelDetail = new Wisej.Web.Label();
            // Orders tab — responsive + theme
            this.labelResponsiveTitle = new Wisej.Web.Label();
            this.labelProfile = new Wisej.Web.Label();
            this.buttonSimDesktop = new Wisej.Web.Button();
            this.buttonSimTablet = new Wisej.Web.Button();
            this.buttonSimPhone = new Wisej.Web.Button();
            this.labelThemeTitle = new Wisej.Web.Label();
            this.labelTheme = new Wisej.Web.Label();
            this.buttonThemeBootstrap = new Wisej.Web.Button();
            this.buttonThemeMaterial = new Wisej.Web.Button();
            this.buttonThemeFluent = new Wisej.Web.Button();
            this.buttonMixin = new Wisej.Web.Button();
            this.labelModernNote = new Wisej.Web.Label();
            // Security tab
            this.labelSecurityTitle = new Wisej.Web.Label();
            this.labelAuth = new Wisej.Web.Label();
            this.buttonSignInKelly = new Wisej.Web.Button();
            this.buttonSignInSam = new Wisej.Web.Button();
            this.buttonSignInBad = new Wisej.Web.Button();
            this.buttonSignOut = new Wisej.Web.Button();
            this.labelDownloadTitle = new Wisej.Web.Label();
            this.textDownload = new Wisej.Web.TextBox();
            this.buttonDownload = new Wisej.Web.Button();
            this.buttonDownloadTraversal = new Wisej.Web.Button();
            this.labelHtmlTitle = new Wisej.Web.Label();
            this.textNotes = new Wisej.Web.TextBox();
            this.buttonRender = new Wisej.Web.Button();
            this.buttonRenderRaw = new Wisej.Web.Button();
            this.captionEncoded = new Wisej.Web.Label();
            this.labelEncoded = new Wisej.Web.Label();
            this.captionSanitized = new Wisej.Web.Label();
            this.labelSanitized = new Wisej.Web.Label();
            this.captionRaw = new Wisej.Web.Label();
            this.labelRaw = new Wisej.Web.Label();
            this.labelAuditTitle = new Wisej.Web.Label();
            this.gridAudit = new Wisej.Web.DataGridView();
            this.colAuditWhen = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAuditOk = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAuditUser = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAuditSession = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAuditAction = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAuditDetail = new Wisej.Web.DataGridViewTextBoxColumn();
            // Dashboard tab
            this.labelDashTitle = new Wisej.Web.Label();
            this.labelLive = new Wisej.Web.Label();
            this.labelKpiOpen = new Wisej.Web.Label();
            this.labelKpiRevenue = new Wisej.Web.Label();
            this.labelKpiInvoiced = new Wisej.Web.Label();
            this.labelKpiOnTime = new Wisej.Web.Label();
            this.canvasChart = new Wisej.Web.Canvas();
            this.labelActivityTitle = new Wisej.Web.Label();
            this.gridActivity = new Wisej.Web.DataGridView();
            this.colActWhen = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colActUser = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colActSession = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colActAction = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colActDetail = new Wisej.Web.DataGridViewTextBoxColumn();
            // Readiness tab
            this.labelReadyTitle = new Wisej.Web.Label();
            this.buttonHealth = new Wisej.Web.Button();
            this.buttonHealthHttp = new Wisej.Web.Button();
            this.buttonReadiness = new Wisej.Web.Button();
            this.buttonStaticAudit = new Wisej.Web.Button();
            this.labelHealth = new Wisej.Web.Label();
            this.gridReadiness = new Wisej.Web.DataGridView();
            this.colReadyNo = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colReadyItem = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colReadyEvidence = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelDeploy = new Wisej.Web.Label();
            // Right column
            this.trace = new OrderDesk.Views.TracePanel();
            this.panelSession = new Wisej.Web.Panel();
            this.labelSessionTitle = new Wisej.Web.Label();
            this.labelSession = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.labelBanner = new Wisej.Web.Label();
            this.buttonSecondSession = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerAudit = new Wisej.Web.Timer(this.components);
            this.tabs.SuspendLayout();
            this.tabOrders.SuspendLayout();
            this.panelOrders.SuspendLayout();
            this.tabSecurity.SuspendLayout();
            this.tabDashboard.SuspendLayout();
            this.tabReadiness.SuspendLayout();
            this.panelSession.SuspendLayout();
            this.SuspendLayout();
            //
            // tabs  (the left column: more than one screen's worth of capstone content)
            //
            this.tabs.Location = new System.Drawing.Point(30, 30);
            this.tabs.Name = "tabs";
            this.tabs.Size = new System.Drawing.Size(640, 624);
            this.tabs.TabPages.AddRange(new Wisej.Web.TabPage[] { this.tabOrders, this.tabSecurity, this.tabDashboard, this.tabReadiness });
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
            //
            // tabOrders  (the migrated screen, modernized: theme, mixin, responsive layouts)
            //
            this.tabOrders.Name = "tabOrders";
            this.tabOrders.Text = "Orders";
            this.tabOrders.Controls.Add(this.panelOrders);
            this.tabOrders.Controls.Add(this.labelResponsiveTitle);
            this.tabOrders.Controls.Add(this.labelProfile);
            this.tabOrders.Controls.Add(this.buttonSimDesktop);
            this.tabOrders.Controls.Add(this.buttonSimTablet);
            this.tabOrders.Controls.Add(this.buttonSimPhone);
            this.tabOrders.Controls.Add(this.labelThemeTitle);
            this.tabOrders.Controls.Add(this.labelTheme);
            this.tabOrders.Controls.Add(this.buttonThemeBootstrap);
            this.tabOrders.Controls.Add(this.buttonThemeMaterial);
            this.tabOrders.Controls.Add(this.buttonThemeFluent);
            this.tabOrders.Controls.Add(this.buttonMixin);
            this.tabOrders.Controls.Add(this.labelModernNote);
            //
            // panelOrders  (the card ResponsiveLayout positions its controls in: x 20…622)
            //
            this.panelOrders.BackColor = System.Drawing.Color.White;
            this.panelOrders.Controls.Add(this.labelOrdersTitle);
            this.panelOrders.Controls.Add(this.labelProfileBadge);
            this.panelOrders.Controls.Add(this.textSearch);
            this.panelOrders.Controls.Add(this.toolBar);
            this.panelOrders.Controls.Add(this.comboActions);
            this.panelOrders.Controls.Add(this.gridOrders);
            this.panelOrders.Controls.Add(this.labelDetailTitle);
            this.panelOrders.Controls.Add(this.labelDetail);
            this.panelOrders.Location = new System.Drawing.Point(0, 0);
            this.panelOrders.Name = "panelOrders";
            this.panelOrders.Size = new System.Drawing.Size(636, 250);
            //
            // labelOrdersTitle / labelProfileBadge
            //
            this.labelOrdersTitle.AutoSize = false;
            this.labelOrdersTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelOrdersTitle.Location = new System.Drawing.Point(20, 12);
            this.labelOrdersTitle.Name = "labelOrdersTitle";
            this.labelOrdersTitle.Size = new System.Drawing.Size(390, 28);
            this.labelOrdersTitle.Text = "Orders · the migrated screen, modernized";
            this.labelProfileBadge.AutoSize = false;
            this.labelProfileBadge.Font = new System.Drawing.Font("default", 9F);
            this.labelProfileBadge.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelProfileBadge.Location = new System.Drawing.Point(410, 16);
            this.labelProfileBadge.Name = "labelProfileBadge";
            this.labelProfileBadge.Size = new System.Drawing.Size(212, 22);
            this.labelProfileBadge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // textSearch  (✓ Watermark instead of a "Search:" label — the modernization toolkit)
            //
            this.textSearch.Location = new System.Drawing.Point(20, 46);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(260, 28);
            this.textSearch.Watermark = "Search orders (customer, id, PO)…";
            this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
            //
            // toolBar  (✓ tool buttons replace the four push buttons of OrdersForm)
            //
            this.toolBar.Dock = Wisej.Web.DockStyle.None;
            this.toolBar.Location = new System.Drawing.Point(20, 80);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(602, 32);
            this.toolBar.Buttons.AddRange(new Wisej.Web.ToolBarButton[] { this.toolNew, this.toolPrint, this.toolExport, this.toolRefresh });
            this.toolBar.ButtonClick += new Wisej.Web.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
            this.toolNew.Name = "toolNew"; this.toolNew.Text = "New Order"; this.toolNew.ToolTipText = "OrderService.Save → CalculateOrderTotal: the reused business logic, unchanged.";
            this.toolPrint.Name = "toolPrint"; this.toolPrint.Text = "Print Invoice"; this.toolPrint.ToolTipText = "Server-generated PDF in a PdfViewer (Module 1/6).";
            this.toolExport.Name = "toolExport"; this.toolExport.Text = "Export"; this.toolExport.ToolTipText = "AuthService.Demand(orders.export) → CsvExport → Application.Download.";
            this.toolRefresh.Name = "toolRefresh"; this.toolRefresh.Text = "Refresh"; this.toolRefresh.ToolTipText = "Reload the grid from the shared repository.";
            //
            // comboActions  (the phone replacement for the toolbar — hidden on desktop and tablet)
            //
            this.comboActions.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboActions.Items.AddRange(new object[] { "New Order", "Print Invoice", "Export", "Refresh" });
            this.comboActions.Location = new System.Drawing.Point(20, 116);
            this.comboActions.Name = "comboActions";
            this.comboActions.Size = new System.Drawing.Size(602, 28);
            this.comboActions.Visible = false;
            this.comboActions.Watermark = "Actions…";
            this.comboActions.SelectedIndexChanged += new System.EventHandler(this.comboActions_SelectedIndexChanged);
            //
            // gridOrders
            //
            this.gridOrders.AllowUserToAddRows = false;
            this.gridOrders.AllowUserToDeleteRows = false;
            this.gridOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colOrder, this.colCustomer, this.colTotal, this.colStatus });
            this.gridOrders.Location = new System.Drawing.Point(20, 116);
            this.gridOrders.MultiSelect = false;
            this.gridOrders.Name = "gridOrders";
            this.gridOrders.ReadOnly = true;
            this.gridOrders.RowHeadersVisible = false;
            this.gridOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOrders.Size = new System.Drawing.Size(380, 118);
            this.gridOrders.SelectionChanged += new System.EventHandler(this.gridOrders_SelectionChanged);
            this.colOrder.HeaderText = "Order"; this.colOrder.Name = "colOrder"; this.colOrder.Width = 60; this.colOrder.ReadOnly = true;
            this.colCustomer.HeaderText = "Customer"; this.colCustomer.Name = "colCustomer"; this.colCustomer.Width = 150; this.colCustomer.ReadOnly = true;
            this.colTotal.HeaderText = "Total"; this.colTotal.Name = "colTotal"; this.colTotal.Width = 90; this.colTotal.ReadOnly = true;
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colTotal.DefaultCellStyle.Format = "N2";
            this.colStatus.HeaderText = "Status"; this.colStatus.Name = "colStatus"; this.colStatus.Width = 78; this.colStatus.ReadOnly = true;
            //
            // labelDetailTitle / labelDetail  (the detail panel ResponsiveLayout moves or hides)
            //
            this.labelDetailTitle.AutoSize = false;
            this.labelDetailTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelDetailTitle.Location = new System.Drawing.Point(416, 116);
            this.labelDetailTitle.Name = "labelDetailTitle";
            this.labelDetailTitle.Size = new System.Drawing.Size(206, 22);
            this.labelDetailTitle.Text = "Order";
            this.labelDetail.AutoSize = false;
            this.labelDetail.Font = new System.Drawing.Font("monospace", 9F);
            this.labelDetail.Location = new System.Drawing.Point(416, 138);
            this.labelDetail.Name = "labelDetail";
            this.labelDetail.Size = new System.Drawing.Size(206, 96);
            this.labelDetail.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // responsive: ClientProfiles.json → three layouts, simulated or real
            //
            this.labelResponsiveTitle.AutoSize = false;
            this.labelResponsiveTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelResponsiveTitle.Location = new System.Drawing.Point(20, 262);
            this.labelResponsiveTitle.Name = "labelResponsiveTitle";
            this.labelResponsiveTitle.Size = new System.Drawing.Size(602, 24);
            this.labelResponsiveTitle.Text = "Responsive · ClientProfiles.json (Phone ≤600 · Tablet 601–1024 · Desktop ≥1025) → three layouts";
            this.labelProfile.AutoSize = false;
            this.labelProfile.Font = new System.Drawing.Font("monospace", 9F);
            this.labelProfile.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelProfile.Location = new System.Drawing.Point(20, 288);
            this.labelProfile.Name = "labelProfile";
            this.labelProfile.Size = new System.Drawing.Size(602, 20);
            this.buttonSimDesktop.Location = new System.Drawing.Point(20, 314);
            this.buttonSimDesktop.Name = "buttonSimDesktop";
            this.buttonSimDesktop.Size = new System.Drawing.Size(140, 28);
            this.buttonSimDesktop.Text = "Simulate desktop";
            this.buttonSimDesktop.ToolTipText = "ResponsiveLayout.Apply(Desktop) — what a browser ≥1025 px wide gets.";
            this.buttonSimDesktop.Click += new System.EventHandler(this.buttonSimDesktop_Click);
            this.buttonSimTablet.Location = new System.Drawing.Point(166, 314);
            this.buttonSimTablet.Name = "buttonSimTablet";
            this.buttonSimTablet.Size = new System.Drawing.Size(140, 28);
            this.buttonSimTablet.Text = "Simulate tablet";
            this.buttonSimTablet.ToolTipText = "ResponsiveLayout.Apply(Tablet) — 601–1024 px.";
            this.buttonSimTablet.Click += new System.EventHandler(this.buttonSimTablet_Click);
            this.buttonSimPhone.Location = new System.Drawing.Point(312, 314);
            this.buttonSimPhone.Name = "buttonSimPhone";
            this.buttonSimPhone.Size = new System.Drawing.Size(140, 28);
            this.buttonSimPhone.Text = "Simulate phone";
            this.buttonSimPhone.ToolTipText = "ResponsiveLayout.Apply(Phone) — ≤600 px.";
            this.buttonSimPhone.Click += new System.EventHandler(this.buttonSimPhone_Click);
            //
            // theme: restyle after parity
            //
            this.labelThemeTitle.AutoSize = false;
            this.labelThemeTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelThemeTitle.Location = new System.Drawing.Point(20, 356);
            this.labelThemeTitle.Name = "labelThemeTitle";
            this.labelThemeTitle.Size = new System.Drawing.Size(602, 24);
            this.labelThemeTitle.Text = "Theme + mixin · restyle without touching a form (Application.LoadTheme)";
            this.labelTheme.AutoSize = false;
            this.labelTheme.Font = new System.Drawing.Font("monospace", 9F);
            this.labelTheme.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTheme.Location = new System.Drawing.Point(20, 382);
            this.labelTheme.Name = "labelTheme";
            this.labelTheme.Size = new System.Drawing.Size(602, 20);
            this.buttonThemeBootstrap.Location = new System.Drawing.Point(20, 408);
            this.buttonThemeBootstrap.Name = "buttonThemeBootstrap";
            this.buttonThemeBootstrap.Size = new System.Drawing.Size(110, 28);
            this.buttonThemeBootstrap.Text = "Bootstrap-4";
            this.buttonThemeBootstrap.Click += new System.EventHandler(this.buttonThemeBootstrap_Click);
            this.buttonThemeMaterial.Location = new System.Drawing.Point(136, 408);
            this.buttonThemeMaterial.Name = "buttonThemeMaterial";
            this.buttonThemeMaterial.Size = new System.Drawing.Size(110, 28);
            this.buttonThemeMaterial.Text = "Material-3";
            this.buttonThemeMaterial.Click += new System.EventHandler(this.buttonThemeMaterial_Click);
            this.buttonThemeFluent.Location = new System.Drawing.Point(252, 408);
            this.buttonThemeFluent.Name = "buttonThemeFluent";
            this.buttonThemeFluent.Size = new System.Drawing.Size(110, 28);
            this.buttonThemeFluent.Text = "FluentDark-5";
            this.buttonThemeFluent.Click += new System.EventHandler(this.buttonThemeFluent_Click);
            this.buttonMixin.Location = new System.Drawing.Point(368, 408);
            this.buttonMixin.Name = "buttonMixin";
            this.buttonMixin.Size = new System.Drawing.Size(180, 28);
            this.buttonMixin.Text = "Apply orderdesk mixin";
            this.buttonMixin.ToolTipText = "Application.LoadTheme(current, new[] { \"orderdesk\" }) — Themes/orderdesk.mixin.theme: button radius 14.";
            this.buttonMixin.Click += new System.EventHandler(this.buttonMixin_Click);
            this.labelModernNote.AutoSize = false;
            this.labelModernNote.Font = new System.Drawing.Font("default", 9F);
            this.labelModernNote.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelModernNote.Location = new System.Drawing.Point(20, 446);
            this.labelModernNote.Name = "labelModernNote";
            this.labelModernNote.Size = new System.Drawing.Size(602, 60);
            this.labelModernNote.Text = "✕ never change visual design and business behaviour in the same step — a bug hides where you cannot see it.\n✓ the theme, mixin and layout buttons change zero lines of Domain/: New Order still computes 1,280.00 through OrderService.Save.";
            this.labelModernNote.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // tabSecurity  (server-side auth · download guard · AllowHtml · audit log)
            //
            this.tabSecurity.Name = "tabSecurity";
            this.tabSecurity.Text = "Security";
            this.tabSecurity.Controls.Add(this.labelSecurityTitle);
            this.tabSecurity.Controls.Add(this.labelAuth);
            this.tabSecurity.Controls.Add(this.buttonSignInKelly);
            this.tabSecurity.Controls.Add(this.buttonSignInSam);
            this.tabSecurity.Controls.Add(this.buttonSignInBad);
            this.tabSecurity.Controls.Add(this.buttonSignOut);
            this.tabSecurity.Controls.Add(this.labelDownloadTitle);
            this.tabSecurity.Controls.Add(this.textDownload);
            this.tabSecurity.Controls.Add(this.buttonDownload);
            this.tabSecurity.Controls.Add(this.buttonDownloadTraversal);
            this.tabSecurity.Controls.Add(this.labelHtmlTitle);
            this.tabSecurity.Controls.Add(this.textNotes);
            this.tabSecurity.Controls.Add(this.buttonRender);
            this.tabSecurity.Controls.Add(this.buttonRenderRaw);
            this.tabSecurity.Controls.Add(this.captionEncoded);
            this.tabSecurity.Controls.Add(this.labelEncoded);
            this.tabSecurity.Controls.Add(this.captionSanitized);
            this.tabSecurity.Controls.Add(this.labelSanitized);
            this.tabSecurity.Controls.Add(this.captionRaw);
            this.tabSecurity.Controls.Add(this.labelRaw);
            this.tabSecurity.Controls.Add(this.labelAuditTitle);
            this.tabSecurity.Controls.Add(this.gridAudit);
            this.labelSecurityTitle.AutoSize = false;
            this.labelSecurityTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelSecurityTitle.Location = new System.Drawing.Point(20, 12);
            this.labelSecurityTitle.Name = "labelSecurityTitle";
            this.labelSecurityTitle.Size = new System.Drawing.Size(602, 28);
            this.labelSecurityTitle.Text = "Security · reviewed as a web app, not a desktop behind the office LAN";
            this.labelAuth.AutoSize = false;
            this.labelAuth.Font = new System.Drawing.Font("monospace", 9F);
            this.labelAuth.Location = new System.Drawing.Point(20, 44);
            this.labelAuth.Name = "labelAuth";
            this.labelAuth.Size = new System.Drawing.Size(602, 20);
            //
            // sign-in buttons: the password never reaches a static, the check runs on the server per action
            //
            this.buttonSignInKelly.Location = new System.Drawing.Point(20, 70);
            this.buttonSignInKelly.Name = "buttonSignInKelly";
            this.buttonSignInKelly.Size = new System.Drawing.Size(120, 28);
            this.buttonSignInKelly.Text = "Sign in kelly";
            this.buttonSignInKelly.ToolTipText = "kelly / northwind-2026 → Manager (read, export, download, delete)";
            this.buttonSignInKelly.Click += new System.EventHandler(this.buttonSignInKelly_Click);
            this.buttonSignInSam.Location = new System.Drawing.Point(146, 70);
            this.buttonSignInSam.Name = "buttonSignInSam";
            this.buttonSignInSam.Size = new System.Drawing.Size(120, 28);
            this.buttonSignInSam.Text = "Sign in sam";
            this.buttonSignInSam.ToolTipText = "sam / fabrikam-2026 → Clerk (read only)";
            this.buttonSignInSam.Click += new System.EventHandler(this.buttonSignInSam_Click);
            this.buttonSignInBad.Location = new System.Drawing.Point(272, 70);
            this.buttonSignInBad.Name = "buttonSignInBad";
            this.buttonSignInBad.Size = new System.Drawing.Size(170, 28);
            this.buttonSignInBad.Text = "kelly, wrong password";
            this.buttonSignInBad.ToolTipText = "The failure path: denied on the server, audited as ✕.";
            this.buttonSignInBad.Click += new System.EventHandler(this.buttonSignInBad_Click);
            this.buttonSignOut.Location = new System.Drawing.Point(448, 70);
            this.buttonSignOut.Name = "buttonSignOut";
            this.buttonSignOut.Size = new System.Drawing.Size(100, 28);
            this.buttonSignOut.Text = "Sign out";
            this.buttonSignOut.Click += new System.EventHandler(this.buttonSignOut_Click);
            //
            // download guard
            //
            this.labelDownloadTitle.AutoSize = false;
            this.labelDownloadTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelDownloadTitle.Location = new System.Drawing.Point(20, 108);
            this.labelDownloadTitle.Name = "labelDownloadTitle";
            this.labelDownloadTitle.Size = new System.Drawing.Size(602, 22);
            this.labelDownloadTitle.Text = "Download guard · permission → resolve under the storage root → exists → Application.Download";
            this.textDownload.Location = new System.Drawing.Point(20, 134);
            this.textDownload.Name = "textDownload";
            this.textDownload.Size = new System.Drawing.Size(300, 28);
            this.textDownload.Text = "exports/orders.csv";
            this.textDownload.Watermark = "relative name under App_Data";
            this.buttonDownload.Location = new System.Drawing.Point(326, 134);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(110, 28);
            this.buttonDownload.Text = "Download";
            this.buttonDownload.ToolTipText = "DownloadGuard.Download(text) — needs files.download (kelly yes, sam no).";
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            this.buttonDownloadTraversal.Location = new System.Drawing.Point(442, 134);
            this.buttonDownloadTraversal.Name = "buttonDownloadTraversal";
            this.buttonDownloadTraversal.Size = new System.Drawing.Size(180, 28);
            this.buttonDownloadTraversal.Text = "Try ..\\Web.config";
            this.buttonDownloadTraversal.ToolTipText = "The attack: a name that walks out of the storage root. Rejected and audited.";
            this.buttonDownloadTraversal.Click += new System.EventHandler(this.buttonDownloadTraversal_Click);
            //
            // AllowHtml: encoded (default) · sanitized · raw
            //
            this.labelHtmlTitle.AutoSize = false;
            this.labelHtmlTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelHtmlTitle.Location = new System.Drawing.Point(20, 172);
            this.labelHtmlTitle.Name = "labelHtmlTitle";
            this.labelHtmlTitle.Size = new System.Drawing.Size(602, 22);
            this.labelHtmlTitle.Text = "AllowHtml · order notes typed by a user: encoded (the default) · sanitized (b, i, br) · raw (✕)";
            this.textNotes.Location = new System.Drawing.Point(20, 198);
            this.textNotes.Name = "textNotes";
            this.textNotes.Size = new System.Drawing.Size(602, 28);
            this.textNotes.Text = "<b>Rush</b> order <img src=x onerror=\"alert(1)\"> <i>ship Friday</i>";
            this.buttonRender.Location = new System.Drawing.Point(20, 232);
            this.buttonRender.Name = "buttonRender";
            this.buttonRender.Size = new System.Drawing.Size(120, 28);
            this.buttonRender.Text = "Render notes";
            this.buttonRender.ToolTipText = "Fills the encoded label (AllowHtml = false) and the sanitized label (HtmlSanitizer.Sanitize).";
            this.buttonRender.Click += new System.EventHandler(this.buttonRender_Click);
            this.buttonRenderRaw.Location = new System.Drawing.Point(146, 232);
            this.buttonRenderRaw.Name = "buttonRenderRaw";
            this.buttonRenderRaw.Size = new System.Drawing.Size(170, 28);
            this.buttonRenderRaw.Text = "Render raw (unsafe)";
            this.buttonRenderRaw.ToolTipText = "AllowHtml = true with the user's text as-is — the injection the lesson warns about.";
            this.buttonRenderRaw.Click += new System.EventHandler(this.buttonRenderRaw_Click);
            this.captionEncoded.AutoSize = false;
            this.captionEncoded.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.captionEncoded.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.captionEncoded.Location = new System.Drawing.Point(20, 268);
            this.captionEncoded.Name = "captionEncoded";
            this.captionEncoded.Size = new System.Drawing.Size(116, 22);
            this.captionEncoded.Text = "✓ encoded";
            this.labelEncoded.AllowHtml = false;
            this.labelEncoded.AutoSize = false;
            this.labelEncoded.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.labelEncoded.Location = new System.Drawing.Point(140, 268);
            this.labelEncoded.Name = "labelEncoded";
            this.labelEncoded.Padding = new Wisej.Web.Padding(6, 0, 6, 0);
            this.labelEncoded.Size = new System.Drawing.Size(482, 22);
            this.captionSanitized.AutoSize = false;
            this.captionSanitized.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.captionSanitized.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.captionSanitized.Location = new System.Drawing.Point(20, 294);
            this.captionSanitized.Name = "captionSanitized";
            this.captionSanitized.Size = new System.Drawing.Size(116, 22);
            this.captionSanitized.Text = "✓ sanitized";
            this.labelSanitized.AllowHtml = true;
            this.labelSanitized.AutoSize = false;
            this.labelSanitized.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.labelSanitized.Location = new System.Drawing.Point(140, 294);
            this.labelSanitized.Name = "labelSanitized";
            this.labelSanitized.Padding = new Wisej.Web.Padding(6, 0, 6, 0);
            this.labelSanitized.Size = new System.Drawing.Size(482, 22);
            this.captionRaw.AutoSize = false;
            this.captionRaw.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.captionRaw.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.captionRaw.Location = new System.Drawing.Point(20, 320);
            this.captionRaw.Name = "captionRaw";
            this.captionRaw.Size = new System.Drawing.Size(116, 22);
            this.captionRaw.Text = "✕ raw";
            this.labelRaw.AllowHtml = true;
            this.labelRaw.AutoSize = false;
            this.labelRaw.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelRaw.Location = new System.Drawing.Point(140, 320);
            this.labelRaw.Name = "labelRaw";
            this.labelRaw.Padding = new Wisej.Web.Padding(6, 0, 6, 0);
            this.labelRaw.Size = new System.Drawing.Size(482, 22);
            //
            // audit grid (process-wide: the second session's actions appear here)
            //
            this.labelAuditTitle.AutoSize = false;
            this.labelAuditTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelAuditTitle.Location = new System.Drawing.Point(20, 350);
            this.labelAuditTitle.Name = "labelAuditTitle";
            this.labelAuditTitle.Size = new System.Drawing.Size(602, 22);
            this.labelAuditTitle.Text = "Audit log · process-wide, every session · a 1 s timer refreshes it only when AuditLog.Version changes";
            this.gridAudit.AllowUserToAddRows = false;
            this.gridAudit.AllowUserToDeleteRows = false;
            this.gridAudit.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colAuditWhen, this.colAuditOk, this.colAuditUser, this.colAuditSession, this.colAuditAction, this.colAuditDetail });
            this.gridAudit.Location = new System.Drawing.Point(20, 374);
            this.gridAudit.MultiSelect = false;
            this.gridAudit.Name = "gridAudit";
            this.gridAudit.ReadOnly = true;
            this.gridAudit.RowHeadersVisible = false;
            this.gridAudit.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridAudit.Size = new System.Drawing.Size(602, 196);
            this.colAuditWhen.HeaderText = "When"; this.colAuditWhen.Name = "colAuditWhen"; this.colAuditWhen.Width = 66; this.colAuditWhen.ReadOnly = true;
            this.colAuditOk.HeaderText = ""; this.colAuditOk.Name = "colAuditOk"; this.colAuditOk.Width = 28; this.colAuditOk.ReadOnly = true;
            this.colAuditUser.HeaderText = "User"; this.colAuditUser.Name = "colAuditUser"; this.colAuditUser.Width = 56; this.colAuditUser.ReadOnly = true;
            this.colAuditSession.HeaderText = "Session"; this.colAuditSession.Name = "colAuditSession"; this.colAuditSession.Width = 64; this.colAuditSession.ReadOnly = true;
            this.colAuditAction.HeaderText = "Action"; this.colAuditAction.Name = "colAuditAction"; this.colAuditAction.Width = 150; this.colAuditAction.ReadOnly = true;
            this.colAuditDetail.HeaderText = "Detail"; this.colAuditDetail.Name = "colAuditDetail"; this.colAuditDetail.Width = 230; this.colAuditDetail.ReadOnly = true;
            //
            // tabDashboard  (the modernization payoff: KPIs, orders-by-status chart, live activity feed)
            //
            this.tabDashboard.Name = "tabDashboard";
            this.tabDashboard.Text = "Dashboard";
            this.tabDashboard.Controls.Add(this.labelDashTitle);
            this.tabDashboard.Controls.Add(this.labelLive);
            this.tabDashboard.Controls.Add(this.labelKpiOpen);
            this.tabDashboard.Controls.Add(this.labelKpiRevenue);
            this.tabDashboard.Controls.Add(this.labelKpiInvoiced);
            this.tabDashboard.Controls.Add(this.labelKpiOnTime);
            this.tabDashboard.Controls.Add(this.canvasChart);
            this.tabDashboard.Controls.Add(this.labelActivityTitle);
            this.tabDashboard.Controls.Add(this.gridActivity);
            this.labelDashTitle.AutoSize = false;
            this.labelDashTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelDashTitle.Location = new System.Drawing.Point(20, 12);
            this.labelDashTitle.Name = "labelDashTitle";
            this.labelDashTitle.Size = new System.Drawing.Size(360, 28);
            this.labelDashTitle.Text = "OrderDesk — Operations dashboard";
            this.labelLive.AutoSize = false;
            this.labelLive.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelLive.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelLive.Location = new System.Drawing.Point(380, 16);
            this.labelLive.Name = "labelLive";
            this.labelLive.Size = new System.Drawing.Size(242, 22);
            this.labelLive.Text = "● live";
            this.labelLive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelKpiOpen.AutoSize = false;
            this.labelKpiOpen.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.labelKpiOpen.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelKpiOpen.ForeColor = System.Drawing.Color.FromArgb(26, 134, 255);
            this.labelKpiOpen.Location = new System.Drawing.Point(20, 48);
            this.labelKpiOpen.Name = "labelKpiOpen";
            this.labelKpiOpen.Size = new System.Drawing.Size(143, 64);
            this.labelKpiOpen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelKpiRevenue.AutoSize = false;
            this.labelKpiRevenue.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.labelKpiRevenue.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelKpiRevenue.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelKpiRevenue.Location = new System.Drawing.Point(173, 48);
            this.labelKpiRevenue.Name = "labelKpiRevenue";
            this.labelKpiRevenue.Size = new System.Drawing.Size(143, 64);
            this.labelKpiRevenue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelKpiInvoiced.AutoSize = false;
            this.labelKpiInvoiced.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.labelKpiInvoiced.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelKpiInvoiced.ForeColor = System.Drawing.Color.FromArgb(125, 90, 224);
            this.labelKpiInvoiced.Location = new System.Drawing.Point(326, 48);
            this.labelKpiInvoiced.Name = "labelKpiInvoiced";
            this.labelKpiInvoiced.Size = new System.Drawing.Size(143, 64);
            this.labelKpiInvoiced.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelKpiOnTime.AutoSize = false;
            this.labelKpiOnTime.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.labelKpiOnTime.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelKpiOnTime.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.labelKpiOnTime.Location = new System.Drawing.Point(479, 48);
            this.labelKpiOnTime.Name = "labelKpiOnTime";
            this.labelKpiOnTime.Size = new System.Drawing.Size(143, 64);
            this.labelKpiOnTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // canvasChart  (Wisej.Web.Canvas: drawing commands issued on the server, rendered by the browser)
            //
            this.canvasChart.BackColor = System.Drawing.Color.White;
            this.canvasChart.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.canvasChart.Location = new System.Drawing.Point(20, 124);
            this.canvasChart.Name = "canvasChart";
            this.canvasChart.Size = new System.Drawing.Size(602, 190);
            this.canvasChart.Redraw += new System.EventHandler(this.canvasChart_Redraw);
            this.labelActivityTitle.AutoSize = false;
            this.labelActivityTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelActivityTitle.Location = new System.Drawing.Point(20, 324);
            this.labelActivityTitle.Name = "labelActivityTitle";
            this.labelActivityTitle.Size = new System.Drawing.Size(602, 22);
            this.labelActivityTitle.Text = "Recent activity · the audit log of every session in this process";
            this.gridActivity.AllowUserToAddRows = false;
            this.gridActivity.AllowUserToDeleteRows = false;
            this.gridActivity.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colActWhen, this.colActUser, this.colActSession, this.colActAction, this.colActDetail });
            this.gridActivity.Location = new System.Drawing.Point(20, 348);
            this.gridActivity.MultiSelect = false;
            this.gridActivity.Name = "gridActivity";
            this.gridActivity.ReadOnly = true;
            this.gridActivity.RowHeadersVisible = false;
            this.gridActivity.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridActivity.Size = new System.Drawing.Size(602, 222);
            this.colActWhen.HeaderText = "When"; this.colActWhen.Name = "colActWhen"; this.colActWhen.Width = 66; this.colActWhen.ReadOnly = true;
            this.colActUser.HeaderText = "User"; this.colActUser.Name = "colActUser"; this.colActUser.Width = 60; this.colActUser.ReadOnly = true;
            this.colActSession.HeaderText = "Session"; this.colActSession.Name = "colActSession"; this.colActSession.Width = 64; this.colActSession.ReadOnly = true;
            this.colActAction.HeaderText = "Action"; this.colActAction.Name = "colActAction"; this.colActAction.Width = 150; this.colActAction.ReadOnly = true;
            this.colActDetail.HeaderText = "Detail"; this.colActDetail.Name = "colActDetail"; this.colActDetail.Width = 254; this.colActDetail.ReadOnly = true;
            //
            // tabReadiness  (health check · /health · readiness checklist · static-state audit · deploy assets)
            //
            this.tabReadiness.Name = "tabReadiness";
            this.tabReadiness.Text = "Readiness & deploy";
            this.tabReadiness.Controls.Add(this.labelReadyTitle);
            this.tabReadiness.Controls.Add(this.buttonHealth);
            this.tabReadiness.Controls.Add(this.buttonHealthHttp);
            this.tabReadiness.Controls.Add(this.buttonReadiness);
            this.tabReadiness.Controls.Add(this.buttonStaticAudit);
            this.tabReadiness.Controls.Add(this.labelHealth);
            this.tabReadiness.Controls.Add(this.gridReadiness);
            this.tabReadiness.Controls.Add(this.labelDeploy);
            this.labelReadyTitle.AutoSize = false;
            this.labelReadyTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelReadyTitle.Location = new System.Drawing.Point(20, 12);
            this.labelReadyTitle.Name = "labelReadyTitle";
            this.labelReadyTitle.Size = new System.Drawing.Size(602, 28);
            this.labelReadyTitle.Text = "Readiness & deploy · publish success is not deployment readiness";
            this.buttonHealth.Location = new System.Drawing.Point(20, 44);
            this.buttonHealth.Name = "buttonHealth";
            this.buttonHealth.Size = new System.Drawing.Size(110, 28);
            this.buttonHealth.Text = "Health check";
            this.buttonHealth.ToolTipText = "HealthCheck.Text(includeSession: true) — the in-session report.";
            this.buttonHealth.Click += new System.EventHandler(this.buttonHealth_Click);
            this.buttonHealthHttp.Location = new System.Drawing.Point(136, 44);
            this.buttonHealthHttp.Name = "buttonHealthHttp";
            this.buttonHealthHttp.Size = new System.Drawing.Size(100, 28);
            this.buttonHealthHttp.Text = "GET /health";
            this.buttonHealthHttp.ToolTipText = "Opens /health in a new tab and fetches it server-side with HttpClient (what a load balancer does).";
            this.buttonHealthHttp.Click += new System.EventHandler(this.buttonHealthHttp_Click);
            this.buttonReadiness.Location = new System.Drawing.Point(242, 44);
            this.buttonReadiness.Name = "buttonReadiness";
            this.buttonReadiness.Size = new System.Drawing.Size(180, 28);
            this.buttonReadiness.Text = "Run readiness checklist";
            this.buttonReadiness.ToolTipText = "ReadinessChecklist.Items — the lesson's eight items with live evidence.";
            this.buttonReadiness.Click += new System.EventHandler(this.buttonReadiness_Click);
            this.buttonStaticAudit.Location = new System.Drawing.Point(428, 44);
            this.buttonStaticAudit.Name = "buttonStaticAudit";
            this.buttonStaticAudit.Size = new System.Drawing.Size(150, 28);
            this.buttonStaticAudit.Text = "Static-state audit";
            this.buttonStaticAudit.ToolTipText = "StaticStateAudit.Run() — every static field in the assembly by reflection.";
            this.buttonStaticAudit.Click += new System.EventHandler(this.buttonStaticAudit_Click);
            this.labelHealth.AutoSize = false;
            this.labelHealth.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.labelHealth.Font = new System.Drawing.Font("monospace", 9F);
            this.labelHealth.Location = new System.Drawing.Point(20, 80);
            this.labelHealth.Name = "labelHealth";
            this.labelHealth.Padding = new Wisej.Web.Padding(8, 6, 8, 6);
            this.labelHealth.Size = new System.Drawing.Size(602, 168);
            this.labelHealth.Text = "press Health check";
            this.labelHealth.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.gridReadiness.AllowUserToAddRows = false;
            this.gridReadiness.AllowUserToDeleteRows = false;
            this.gridReadiness.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colReadyNo, this.colReadyItem, this.colReadyEvidence });
            this.gridReadiness.Location = new System.Drawing.Point(20, 256);
            this.gridReadiness.MultiSelect = false;
            this.gridReadiness.Name = "gridReadiness";
            this.gridReadiness.ReadOnly = true;
            this.gridReadiness.RowHeadersVisible = false;
            this.gridReadiness.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridReadiness.Size = new System.Drawing.Size(602, 196);
            this.colReadyNo.HeaderText = "#"; this.colReadyNo.Name = "colReadyNo"; this.colReadyNo.Width = 30; this.colReadyNo.ReadOnly = true;
            this.colReadyItem.HeaderText = "Readiness item"; this.colReadyItem.Name = "colReadyItem"; this.colReadyItem.Width = 230; this.colReadyItem.ReadOnly = true;
            this.colReadyEvidence.HeaderText = "Evidence"; this.colReadyEvidence.Name = "colReadyEvidence"; this.colReadyEvidence.Width = 334; this.colReadyEvidence.ReadOnly = true;
            this.labelDeploy.AutoSize = false;
            this.labelDeploy.Font = new System.Drawing.Font("monospace", 9F);
            this.labelDeploy.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelDeploy.Location = new System.Drawing.Point(20, 460);
            this.labelDeploy.Name = "labelDeploy";
            this.labelDeploy.Size = new System.Drawing.Size(602, 110);
            this.labelDeploy.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // trace  (the migration log)
            //
            this.trace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.trace.Location = new System.Drawing.Point(690, 30);
            this.trace.Name = "trace";
            this.trace.Size = new System.Drawing.Size(628, 440);
            //
            // panelSession  (status · banner · second session · clear)
            //
            this.panelSession.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelSession.BackColor = System.Drawing.Color.White;
            this.panelSession.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSession.Controls.Add(this.labelSessionTitle);
            this.panelSession.Controls.Add(this.labelSession);
            this.panelSession.Controls.Add(this.labelStatus);
            this.panelSession.Controls.Add(this.labelBanner);
            this.panelSession.Controls.Add(this.buttonSecondSession);
            this.panelSession.Controls.Add(this.buttonClear);
            this.panelSession.Location = new System.Drawing.Point(690, 484);
            this.panelSession.Name = "panelSession";
            this.panelSession.Size = new System.Drawing.Size(628, 170);
            this.labelSessionTitle.AutoSize = false;
            this.labelSessionTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelSessionTitle.Location = new System.Drawing.Point(20, 10);
            this.labelSessionTitle.Name = "labelSessionTitle";
            this.labelSessionTitle.Size = new System.Drawing.Size(590, 26);
            this.labelSessionTitle.Text = "Status · this session";
            this.labelSession.AutoSize = false;
            this.labelSession.Font = new System.Drawing.Font("monospace", 9F);
            this.labelSession.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelSession.Location = new System.Drawing.Point(20, 36);
            this.labelSession.Name = "labelSession";
            this.labelSession.Size = new System.Drawing.Size(590, 18);
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(20, 56);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(590, 22);
            this.labelStatus.Text = "● loading";
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(20, 80);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(590, 52);
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            this.buttonSecondSession.Location = new System.Drawing.Point(20, 136);
            this.buttonSecondSession.Name = "buttonSecondSession";
            this.buttonSecondSession.Size = new System.Drawing.Size(160, 28);
            this.buttonSecondSession.Text = "Open second session ↗";
            this.buttonSecondSession.ToolTipText = "Application.Navigate(Application.Url, \"_blank\") — the multi-user test: sign in as the other user there.";
            this.buttonSecondSession.Click += new System.EventHandler(this.buttonSecondSession_Click);
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(552, 136);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(58, 28);
            this.buttonClear.Text = "Clear";
            this.buttonClear.ToolTipText = "Clear the trace.";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // timerAudit  (the progress path: polls AuditLog.Version once a second, redraws only on change)
            //
            this.timerAudit.Interval = 1000;
            this.timerAudit.Tick += new System.EventHandler(this.timerAudit_Tick);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.trace);
            this.Controls.Add(this.panelSession);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "OrderDesk — Modernize, Secure, Deploy";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.tabs.ResumeLayout(false);
            this.tabOrders.ResumeLayout(false);
            this.panelOrders.ResumeLayout(false);
            this.tabSecurity.ResumeLayout(false);
            this.tabDashboard.ResumeLayout(false);
            this.tabReadiness.ResumeLayout(false);
            this.panelSession.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.TabControl tabs;
        private Wisej.Web.TabPage tabOrders;
        private Wisej.Web.TabPage tabSecurity;
        private Wisej.Web.TabPage tabDashboard;
        private Wisej.Web.TabPage tabReadiness;
        private Wisej.Web.Panel panelOrders;
        private Wisej.Web.Label labelOrdersTitle;
        private Wisej.Web.Label labelProfileBadge;
        private Wisej.Web.TextBox textSearch;
        private Wisej.Web.ToolBar toolBar;
        private Wisej.Web.ToolBarButton toolNew;
        private Wisej.Web.ToolBarButton toolPrint;
        private Wisej.Web.ToolBarButton toolExport;
        private Wisej.Web.ToolBarButton toolRefresh;
        private Wisej.Web.ComboBox comboActions;
        private Wisej.Web.DataGridView gridOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colOrder;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colTotal;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.Label labelDetailTitle;
        private Wisej.Web.Label labelDetail;
        private Wisej.Web.Label labelResponsiveTitle;
        private Wisej.Web.Label labelProfile;
        private Wisej.Web.Button buttonSimDesktop;
        private Wisej.Web.Button buttonSimTablet;
        private Wisej.Web.Button buttonSimPhone;
        private Wisej.Web.Label labelThemeTitle;
        private Wisej.Web.Label labelTheme;
        private Wisej.Web.Button buttonThemeBootstrap;
        private Wisej.Web.Button buttonThemeMaterial;
        private Wisej.Web.Button buttonThemeFluent;
        private Wisej.Web.Button buttonMixin;
        private Wisej.Web.Label labelModernNote;
        private Wisej.Web.Label labelSecurityTitle;
        private Wisej.Web.Label labelAuth;
        private Wisej.Web.Button buttonSignInKelly;
        private Wisej.Web.Button buttonSignInSam;
        private Wisej.Web.Button buttonSignInBad;
        private Wisej.Web.Button buttonSignOut;
        private Wisej.Web.Label labelDownloadTitle;
        private Wisej.Web.TextBox textDownload;
        private Wisej.Web.Button buttonDownload;
        private Wisej.Web.Button buttonDownloadTraversal;
        private Wisej.Web.Label labelHtmlTitle;
        private Wisej.Web.TextBox textNotes;
        private Wisej.Web.Button buttonRender;
        private Wisej.Web.Button buttonRenderRaw;
        private Wisej.Web.Label captionEncoded;
        private Wisej.Web.Label labelEncoded;
        private Wisej.Web.Label captionSanitized;
        private Wisej.Web.Label labelSanitized;
        private Wisej.Web.Label captionRaw;
        private Wisej.Web.Label labelRaw;
        private Wisej.Web.Label labelAuditTitle;
        private Wisej.Web.DataGridView gridAudit;
        private Wisej.Web.DataGridViewTextBoxColumn colAuditWhen;
        private Wisej.Web.DataGridViewTextBoxColumn colAuditOk;
        private Wisej.Web.DataGridViewTextBoxColumn colAuditUser;
        private Wisej.Web.DataGridViewTextBoxColumn colAuditSession;
        private Wisej.Web.DataGridViewTextBoxColumn colAuditAction;
        private Wisej.Web.DataGridViewTextBoxColumn colAuditDetail;
        private Wisej.Web.Label labelDashTitle;
        private Wisej.Web.Label labelLive;
        private Wisej.Web.Label labelKpiOpen;
        private Wisej.Web.Label labelKpiRevenue;
        private Wisej.Web.Label labelKpiInvoiced;
        private Wisej.Web.Label labelKpiOnTime;
        private Wisej.Web.Canvas canvasChart;
        private Wisej.Web.Label labelActivityTitle;
        private Wisej.Web.DataGridView gridActivity;
        private Wisej.Web.DataGridViewTextBoxColumn colActWhen;
        private Wisej.Web.DataGridViewTextBoxColumn colActUser;
        private Wisej.Web.DataGridViewTextBoxColumn colActSession;
        private Wisej.Web.DataGridViewTextBoxColumn colActAction;
        private Wisej.Web.DataGridViewTextBoxColumn colActDetail;
        private Wisej.Web.Label labelReadyTitle;
        private Wisej.Web.Button buttonHealth;
        private Wisej.Web.Button buttonHealthHttp;
        private Wisej.Web.Button buttonReadiness;
        private Wisej.Web.Button buttonStaticAudit;
        private Wisej.Web.Label labelHealth;
        private Wisej.Web.DataGridView gridReadiness;
        private Wisej.Web.DataGridViewTextBoxColumn colReadyNo;
        private Wisej.Web.DataGridViewTextBoxColumn colReadyItem;
        private Wisej.Web.DataGridViewTextBoxColumn colReadyEvidence;
        private Wisej.Web.Label labelDeploy;
        private OrderDesk.Views.TracePanel trace;
        private Wisej.Web.Panel panelSession;
        private Wisej.Web.Label labelSessionTitle;
        private Wisej.Web.Label labelSession;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Button buttonSecondSession;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerAudit;
    }
}
