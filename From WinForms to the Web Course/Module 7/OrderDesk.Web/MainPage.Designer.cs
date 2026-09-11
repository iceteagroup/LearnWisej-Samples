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
            this.panelHeader = new Wisej.Web.Panel();
            this.labelAuth = new Wisej.Web.Label();
            this.buttonSignInKelly = new Wisej.Web.Button();
            this.buttonSignInSam = new Wisej.Web.Button();
            this.buttonSignOut = new Wisej.Web.Button();
            this.buttonSecondSession = new Wisej.Web.Button();
            this.tabs = new Wisej.Web.TabControl();
            this.tabOrders = new Wisej.Web.TabPage();
            this.tabDashboard = new Wisej.Web.TabPage();
            this.panelOrders = new Wisej.Web.Panel();
            this.labelOrdersTitle = new Wisej.Web.Label();
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
            this.labelNotesTitle = new Wisej.Web.Label();
            this.textNotes = new Wisej.Web.TextBox();
            this.labelSanitized = new Wisej.Web.Label();
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
            this.timerAudit = new Wisej.Web.Timer(this.components);
            this.panelHeader.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabOrders.SuspendLayout();
            this.panelOrders.SuspendLayout();
            this.tabDashboard.SuspendLayout();
            this.SuspendLayout();
            //
            // panelHeader  (sign-in: the user lives in Application.Session, checked on the server per action)
            //
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelHeader.Controls.Add(this.labelAuth);
            this.panelHeader.Controls.Add(this.buttonSignInKelly);
            this.panelHeader.Controls.Add(this.buttonSignInSam);
            this.panelHeader.Controls.Add(this.buttonSignOut);
            this.panelHeader.Controls.Add(this.buttonSecondSession);
            this.panelHeader.Location = new System.Drawing.Point(20, 12);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(660, 46);
            //
            // labelAuth
            //
            this.labelAuth.AutoSize = false;
            this.labelAuth.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelAuth.Location = new System.Drawing.Point(12, 10);
            this.labelAuth.Name = "labelAuth";
            this.labelAuth.Size = new System.Drawing.Size(216, 24);
            this.labelAuth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonSignInKelly
            //
            this.buttonSignInKelly.Location = new System.Drawing.Point(232, 8);
            this.buttonSignInKelly.Name = "buttonSignInKelly";
            this.buttonSignInKelly.Size = new System.Drawing.Size(96, 28);
            this.buttonSignInKelly.Text = "Sign in kelly";
            this.buttonSignInKelly.Click += new System.EventHandler(this.buttonSignInKelly_Click);
            //
            // buttonSignInSam
            //
            this.buttonSignInSam.Location = new System.Drawing.Point(334, 8);
            this.buttonSignInSam.Name = "buttonSignInSam";
            this.buttonSignInSam.Size = new System.Drawing.Size(96, 28);
            this.buttonSignInSam.Text = "Sign in sam";
            this.buttonSignInSam.Click += new System.EventHandler(this.buttonSignInSam_Click);
            //
            // buttonSignOut
            //
            this.buttonSignOut.Location = new System.Drawing.Point(436, 8);
            this.buttonSignOut.Name = "buttonSignOut";
            this.buttonSignOut.Size = new System.Drawing.Size(80, 28);
            this.buttonSignOut.Text = "Sign out";
            this.buttonSignOut.Click += new System.EventHandler(this.buttonSignOut_Click);
            //
            // buttonSecondSession
            //
            this.buttonSecondSession.Location = new System.Drawing.Point(522, 8);
            this.buttonSecondSession.Name = "buttonSecondSession";
            this.buttonSecondSession.Size = new System.Drawing.Size(126, 28);
            this.buttonSecondSession.Text = "Second session ↗";
            this.buttonSecondSession.Click += new System.EventHandler(this.buttonSecondSession_Click);
            //
            // tabs
            //
            this.tabs.Location = new System.Drawing.Point(20, 66);
            this.tabs.Name = "tabs";
            this.tabs.Size = new System.Drawing.Size(660, 520);
            this.tabs.TabPages.AddRange(new Wisej.Web.TabPage[] { this.tabOrders, this.tabDashboard });
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
            //
            // tabOrders
            //
            this.tabOrders.Name = "tabOrders";
            this.tabOrders.Text = "Orders";
            this.tabOrders.Controls.Add(this.panelOrders);
            this.tabOrders.Controls.Add(this.labelNotesTitle);
            this.tabOrders.Controls.Add(this.textNotes);
            this.tabOrders.Controls.Add(this.labelSanitized);
            //
            // panelOrders  (ResponsiveLayout positions its controls in: x 20…622)
            //
            this.panelOrders.BackColor = System.Drawing.Color.White;
            this.panelOrders.Controls.Add(this.labelOrdersTitle);
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
            // labelOrdersTitle
            //
            this.labelOrdersTitle.AutoSize = false;
            this.labelOrdersTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelOrdersTitle.Location = new System.Drawing.Point(20, 12);
            this.labelOrdersTitle.Name = "labelOrdersTitle";
            this.labelOrdersTitle.Size = new System.Drawing.Size(390, 28);
            this.labelOrdersTitle.Text = "Orders";
            //
            // textSearch
            //
            this.textSearch.Location = new System.Drawing.Point(20, 46);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(260, 28);
            this.textSearch.Watermark = "Search orders (customer, id, PO)…";
            this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
            //
            // toolBar
            //
            this.toolBar.Dock = Wisej.Web.DockStyle.None;
            this.toolBar.Location = new System.Drawing.Point(20, 80);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(602, 32);
            this.toolBar.Buttons.AddRange(new Wisej.Web.ToolBarButton[] { this.toolNew, this.toolPrint, this.toolExport, this.toolRefresh });
            this.toolBar.ButtonClick += new Wisej.Web.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
            this.toolNew.Name = "toolNew"; this.toolNew.Text = "New Order";
            this.toolPrint.Name = "toolPrint"; this.toolPrint.Text = "Print Invoice";
            this.toolExport.Name = "toolExport"; this.toolExport.Text = "Export";
            this.toolRefresh.Name = "toolRefresh"; this.toolRefresh.Text = "Refresh";
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
            // labelNotesTitle / textNotes / labelSanitized  (user-entered notes: AllowHtml only after HtmlSanitizer)
            //
            this.labelNotesTitle.AutoSize = false;
            this.labelNotesTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelNotesTitle.Location = new System.Drawing.Point(20, 262);
            this.labelNotesTitle.Name = "labelNotesTitle";
            this.labelNotesTitle.Size = new System.Drawing.Size(602, 22);
            this.labelNotesTitle.Text = "Order note";
            this.textNotes.Location = new System.Drawing.Point(20, 288);
            this.textNotes.Name = "textNotes";
            this.textNotes.Size = new System.Drawing.Size(602, 28);
            this.textNotes.Watermark = "Note for the order — bold, italic and line breaks are kept";
            this.textNotes.TextChanged += new System.EventHandler(this.textNotes_TextChanged);
            this.labelSanitized.AllowHtml = true;
            this.labelSanitized.AutoSize = false;
            this.labelSanitized.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.labelSanitized.Location = new System.Drawing.Point(20, 322);
            this.labelSanitized.Name = "labelSanitized";
            this.labelSanitized.Padding = new Wisej.Web.Padding(6, 4, 6, 4);
            this.labelSanitized.Size = new System.Drawing.Size(602, 48);
            this.labelSanitized.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // tabDashboard
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
            this.labelDashTitle.Text = "Operations dashboard";
            this.labelLive.AutoSize = false;
            this.labelLive.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelLive.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelLive.Location = new System.Drawing.Point(380, 16);
            this.labelLive.Name = "labelLive";
            this.labelLive.Size = new System.Drawing.Size(242, 22);
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
            this.canvasChart.Size = new System.Drawing.Size(602, 170);
            this.canvasChart.Redraw += new System.EventHandler(this.canvasChart_Redraw);
            this.labelActivityTitle.AutoSize = false;
            this.labelActivityTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelActivityTitle.Location = new System.Drawing.Point(20, 304);
            this.labelActivityTitle.Name = "labelActivityTitle";
            this.labelActivityTitle.Size = new System.Drawing.Size(602, 22);
            this.labelActivityTitle.Text = "Recent activity";
            this.gridActivity.AllowUserToAddRows = false;
            this.gridActivity.AllowUserToDeleteRows = false;
            this.gridActivity.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colActWhen, this.colActUser, this.colActSession, this.colActAction, this.colActDetail });
            this.gridActivity.Location = new System.Drawing.Point(20, 328);
            this.gridActivity.MultiSelect = false;
            this.gridActivity.Name = "gridActivity";
            this.gridActivity.ReadOnly = true;
            this.gridActivity.RowHeadersVisible = false;
            this.gridActivity.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridActivity.Size = new System.Drawing.Size(602, 150);
            this.colActWhen.HeaderText = "When"; this.colActWhen.Name = "colActWhen"; this.colActWhen.Width = 66; this.colActWhen.ReadOnly = true;
            this.colActUser.HeaderText = "User"; this.colActUser.Name = "colActUser"; this.colActUser.Width = 60; this.colActUser.ReadOnly = true;
            this.colActSession.HeaderText = "Session"; this.colActSession.Name = "colActSession"; this.colActSession.Width = 64; this.colActSession.ReadOnly = true;
            this.colActAction.HeaderText = "Action"; this.colActAction.Name = "colActAction"; this.colActAction.Width = 150; this.colActAction.ReadOnly = true;
            this.colActDetail.HeaderText = "Detail"; this.colActDetail.Name = "colActDetail"; this.colActDetail.Width = 254; this.colActDetail.ReadOnly = true;
            //
            // timerAudit  (polls AuditLog.Version once a second, redraws only on change)
            //
            this.timerAudit.Interval = 1000;
            this.timerAudit.Tick += new System.EventHandler(this.timerAudit_Tick);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.tabs);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(700, 600);
            this.Text = "OrderDesk";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelHeader.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.tabOrders.ResumeLayout(false);
            this.panelOrders.ResumeLayout(false);
            this.tabDashboard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelHeader;
        private Wisej.Web.Label labelAuth;
        private Wisej.Web.Button buttonSignInKelly;
        private Wisej.Web.Button buttonSignInSam;
        private Wisej.Web.Button buttonSignOut;
        private Wisej.Web.Button buttonSecondSession;
        private Wisej.Web.TabControl tabs;
        private Wisej.Web.TabPage tabOrders;
        private Wisej.Web.TabPage tabDashboard;
        private Wisej.Web.Panel panelOrders;
        private Wisej.Web.Label labelOrdersTitle;
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
        private Wisej.Web.Label labelNotesTitle;
        private Wisej.Web.TextBox textNotes;
        private Wisej.Web.Label labelSanitized;
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
        private Wisej.Web.Timer timerAudit;
    }
}
