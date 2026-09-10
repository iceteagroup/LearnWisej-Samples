namespace OrderDesk
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
            this.appBar = new Wisej.Web.Panel();
            this.labelTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.trace = new OrderDesk.Shared.TracePanel();
            this.shell = new Wisej.Web.Panel();
            this.menuBar = new Wisej.Web.MenuBar();
            this.fileMenu = new Wisej.Web.MenuItem();
            this.exitMenuItem = new Wisej.Web.MenuItem();
            this.editMenu = new Wisej.Web.MenuItem();
            this.editSelectedMenuItem = new Wisej.Web.MenuItem();
            this.newOrderMenuItem = new Wisej.Web.MenuItem();
            this.viewMenu = new Wisej.Web.MenuItem();
            this.viewOrdersMenuItem = new Wisej.Web.MenuItem();
            this.viewCustomersMenuItem = new Wisej.Web.MenuItem();
            this.viewReportsMenuItem = new Wisej.Web.MenuItem();
            this.reportsMenu = new Wisej.Web.MenuItem();
            this.reportOpenOrdersMenuItem = new Wisej.Web.MenuItem();
            this.reportInvoicesMenuItem = new Wisej.Web.MenuItem();
            this.helpMenu = new Wisej.Web.MenuItem();
            this.aboutMenuItem = new Wisej.Web.MenuItem();
            this.statusBar = new Wisej.Web.StatusBar();
            this.statusPanelMain = new Wisej.Web.StatusBarPanel();
            this.statusPanelDialogs = new Wisej.Web.StatusBarPanel();
            this.statusPanelPage = new Wisej.Web.StatusBarPanel();
            this.buttonBar = new Wisej.Web.Panel();
            this.buttonEditSelected = new Wisej.Web.Button();
            this.buttonLeak = new Wisej.Web.Button();
            this.buttonDisposeFix = new Wisej.Web.Button();
            this.buttonToast = new Wisej.Web.Button();
            this.buttonTabOrder = new Wisej.Web.Button();
            this.labelBanner = new Wisej.Web.Label();
            this.navPanel = new Wisej.Web.Panel();
            this.labelBrand = new Wisej.Web.Label();
            this.navOrders = new Wisej.Web.Button();
            this.navCustomers = new Wisej.Web.Button();
            this.navReports = new Wisej.Web.Button();
            this.pageHost = new Wisej.Web.Panel();
            this.timerTabOrder = new Wisej.Web.Timer(this.components);
            this.appBar.SuspendLayout();
            this.shell.SuspendLayout();
            this.buttonBar.SuspendLayout();
            this.navPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // appBar
            //
            this.appBar.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.appBar.Controls.Add(this.labelTitle);
            this.appBar.Controls.Add(this.labelStatus);
            this.appBar.Dock = Wisej.Web.DockStyle.Top;
            this.appBar.Name = "appBar";
            this.appBar.Size = new System.Drawing.Size(1400, 44);
            this.appBar.TabStop = false;
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Padding = new Wisej.Web.Padding(20, 0, 0, 0);
            this.labelTitle.Text = "OrderDesk — Module 3 · Forms, Navigation, Layout && Modal Workflow";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelStatus  (● idle / working / alarm)
            //
            this.labelStatus.AutoSize = false;
            this.labelStatus.Dock = Wisej.Web.DockStyle.Right;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.White;
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Padding = new Wisej.Web.Padding(0, 0, 20, 0);
            this.labelStatus.Size = new System.Drawing.Size(140, 44);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // trace
            //
            this.trace.Dock = Wisej.Web.DockStyle.Right;
            this.trace.Name = "trace";
            this.trace.Size = new System.Drawing.Size(560, 716);
            this.trace.TabStop = false;
            //
            // shell  (menu bar + nav + page host + button bar + status bar — the WinForms Form chrome)
            //
            this.shell.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.shell.Controls.Add(this.pageHost);
            this.shell.Controls.Add(this.navPanel);
            this.shell.Controls.Add(this.labelBanner);
            this.shell.Controls.Add(this.buttonBar);
            this.shell.Controls.Add(this.statusBar);
            this.shell.Controls.Add(this.menuBar);
            this.shell.Dock = Wisej.Web.DockStyle.Fill;
            this.shell.Name = "shell";
            this.shell.Size = new System.Drawing.Size(840, 716);
            //
            // menuBar  (was MenuStrip: File · Edit · View · Reports · Help)
            //
            this.menuBar.Dock = Wisej.Web.DockStyle.Top;
            this.menuBar.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.fileMenu, this.editMenu, this.viewMenu, this.reportsMenu, this.helpMenu });
            this.menuBar.Name = "menuBar";
            this.menuBar.Size = new System.Drawing.Size(840, 32);
            this.fileMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.exitMenuItem });
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Text = "File";
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            this.editMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.editSelectedMenuItem, this.newOrderMenuItem });
            this.editMenu.Name = "editMenu";
            this.editMenu.Text = "Edit";
            this.editSelectedMenuItem.Name = "editSelectedMenuItem";
            this.editSelectedMenuItem.Text = "Edit selected order…";
            this.editSelectedMenuItem.Click += new System.EventHandler(this.editSelectedMenuItem_Click);
            this.newOrderMenuItem.Name = "newOrderMenuItem";
            this.newOrderMenuItem.Text = "New order…";
            this.newOrderMenuItem.Click += new System.EventHandler(this.newOrderMenuItem_Click);
            this.viewMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.viewOrdersMenuItem, this.viewCustomersMenuItem, this.viewReportsMenuItem });
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Text = "View";
            this.viewOrdersMenuItem.Name = "viewOrdersMenuItem";
            this.viewOrdersMenuItem.Text = "Orders";
            this.viewOrdersMenuItem.Click += new System.EventHandler(this.viewOrdersMenuItem_Click);
            this.viewCustomersMenuItem.Name = "viewCustomersMenuItem";
            this.viewCustomersMenuItem.Text = "Customers";
            this.viewCustomersMenuItem.Click += new System.EventHandler(this.viewCustomersMenuItem_Click);
            this.viewReportsMenuItem.Name = "viewReportsMenuItem";
            this.viewReportsMenuItem.Text = "Reports";
            this.viewReportsMenuItem.Click += new System.EventHandler(this.viewReportsMenuItem_Click);
            this.reportsMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.reportOpenOrdersMenuItem, this.reportInvoicesMenuItem });
            this.reportsMenu.Name = "reportsMenu";
            this.reportsMenu.Text = "Reports";
            this.reportOpenOrdersMenuItem.Name = "reportOpenOrdersMenuItem";
            this.reportOpenOrdersMenuItem.Text = "Open orders by customer";
            this.reportOpenOrdersMenuItem.Click += new System.EventHandler(this.reportOpenOrdersMenuItem_Click);
            this.reportInvoicesMenuItem.Name = "reportInvoicesMenuItem";
            this.reportInvoicesMenuItem.Text = "Invoices this month";
            this.reportInvoicesMenuItem.Click += new System.EventHandler(this.reportInvoicesMenuItem_Click);
            this.helpMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.aboutMenuItem });
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Text = "Help";
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Text = "About";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            //
            // statusBar  (was StatusStrip + ToolStripStatusLabel)
            //
            this.statusBar.Dock = Wisej.Web.DockStyle.Bottom;
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new Wisej.Web.StatusBarPanel[] { this.statusPanelMain, this.statusPanelDialogs, this.statusPanelPage });
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(840, 26);
            this.statusPanelMain.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Spring;
            this.statusPanelMain.Name = "statusPanelMain";
            this.statusPanelMain.Text = "Ready · multi-user web";
            this.statusPanelDialogs.Name = "statusPanelDialogs";
            this.statusPanelDialogs.Text = "Dialogs alive: 0";
            this.statusPanelDialogs.Width = 160;
            this.statusPanelPage.Name = "statusPanelPage";
            this.statusPanelPage.Text = "Orders";
            this.statusPanelPage.Width = 120;
            //
            // buttonBar  (the lab buttons: success · failure · recovery · progress)
            //
            this.buttonBar.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.buttonBar.Controls.Add(this.buttonEditSelected);
            this.buttonBar.Controls.Add(this.buttonLeak);
            this.buttonBar.Controls.Add(this.buttonDisposeFix);
            this.buttonBar.Controls.Add(this.buttonToast);
            this.buttonBar.Controls.Add(this.buttonTabOrder);
            this.buttonBar.Dock = Wisej.Web.DockStyle.Bottom;
            this.buttonBar.Name = "buttonBar";
            this.buttonBar.Size = new System.Drawing.Size(840, 52);
            //
            // buttonEditSelected
            //
            this.buttonEditSelected.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.buttonEditSelected.ForeColor = System.Drawing.Color.White;
            this.buttonEditSelected.Location = new System.Drawing.Point(12, 10);
            this.buttonEditSelected.Name = "buttonEditSelected";
            this.buttonEditSelected.Size = new System.Drawing.Size(120, 32);
            this.buttonEditSelected.TabIndex = 0;
            this.buttonEditSelected.Text = "Edit selected ✓";
            this.buttonEditSelected.ToolTipText = "using (var dlg = new EditOrderDialog(order)) { if (dlg.ShowDialog() == DialogResult.OK) … } → Toast";
            this.buttonEditSelected.Click += new System.EventHandler(this.buttonEditSelected_Click);
            //
            // buttonLeak
            //
            this.buttonLeak.Location = new System.Drawing.Point(140, 10);
            this.buttonLeak.Name = "buttonLeak";
            this.buttonLeak.Size = new System.Drawing.Size(120, 32);
            this.buttonLeak.TabIndex = 1;
            this.buttonLeak.Text = "Leak dialogs ✕";
            this.buttonLeak.ToolTipText = "The WinForms habit: 5 × new EditOrderDialog().ShowDialog() … closed, never disposed";
            this.buttonLeak.Click += new System.EventHandler(this.buttonLeak_Click);
            //
            // buttonDisposeFix
            //
            this.buttonDisposeFix.Location = new System.Drawing.Point(268, 10);
            this.buttonDisposeFix.Name = "buttonDisposeFix";
            this.buttonDisposeFix.Size = new System.Drawing.Size(110, 32);
            this.buttonDisposeFix.TabIndex = 2;
            this.buttonDisposeFix.Text = "Dispose fix ✓";
            this.buttonDisposeFix.ToolTipText = "Dispose the leaked dialogs, then run the same 5 through a using block";
            this.buttonDisposeFix.Click += new System.EventHandler(this.buttonDisposeFix_Click);
            //
            // buttonToast
            //
            this.buttonToast.Location = new System.Drawing.Point(386, 10);
            this.buttonToast.Name = "buttonToast";
            this.buttonToast.Size = new System.Drawing.Size(236, 32);
            this.buttonToast.TabIndex = 3;
            this.buttonToast.Text = "Saved via MessageBox ✕ → Toast ✓";
            this.buttonToast.ToolTipText = "Before: MessageBox.Show(\"Saved.\") blocks until OK. After: Notify.Saved → Toast";
            this.buttonToast.Click += new System.EventHandler(this.buttonToast_Click);
            //
            // buttonTabOrder
            //
            this.buttonTabOrder.Location = new System.Drawing.Point(630, 10);
            this.buttonTabOrder.Name = "buttonTabOrder";
            this.buttonTabOrder.Size = new System.Drawing.Size(140, 32);
            this.buttonTabOrder.TabIndex = 4;
            this.buttonTabOrder.Text = "Tab order / docking";
            this.buttonTabOrder.ToolTipText = "Walks the OrdersPage controls in tab order, highlighting each and logging TabIndex / Dock";
            this.buttonTabOrder.Click += new System.EventHandler(this.buttonTabOrder_Click);
            //
            // labelBanner  (finding / alarm banner; hidden until a path reports)
            //
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Dock = Wisej.Web.DockStyle.Bottom;
            this.labelBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.labelBanner.Size = new System.Drawing.Size(840, 34);
            this.labelBanner.TabStop = false;
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // navPanel  (left navigation: Orders · Customers · Reports — the video's dark rail)
            //
            this.navPanel.BackColor = System.Drawing.Color.FromArgb(22, 20, 46);
            this.navPanel.Controls.Add(this.labelBrand);
            this.navPanel.Controls.Add(this.navOrders);
            this.navPanel.Controls.Add(this.navCustomers);
            this.navPanel.Controls.Add(this.navReports);
            this.navPanel.Dock = Wisej.Web.DockStyle.Left;
            this.navPanel.Name = "navPanel";
            this.navPanel.Size = new System.Drawing.Size(160, 600);
            //
            // labelBrand
            //
            this.labelBrand.AutoSize = false;
            this.labelBrand.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelBrand.ForeColor = System.Drawing.Color.White;
            this.labelBrand.Location = new System.Drawing.Point(12, 14);
            this.labelBrand.Name = "labelBrand";
            this.labelBrand.Size = new System.Drawing.Size(136, 26);
            this.labelBrand.TabStop = false;
            this.labelBrand.Text = "OrderDesk";
            this.labelBrand.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // navOrders / navCustomers / navReports
            //
            this.navOrders.Location = new System.Drawing.Point(12, 56);
            this.navOrders.Name = "navOrders";
            this.navOrders.Size = new System.Drawing.Size(136, 34);
            this.navOrders.TabIndex = 0;
            this.navOrders.Text = "Orders";
            this.navOrders.Click += new System.EventHandler(this.navOrders_Click);
            this.navCustomers.Location = new System.Drawing.Point(12, 96);
            this.navCustomers.Name = "navCustomers";
            this.navCustomers.Size = new System.Drawing.Size(136, 34);
            this.navCustomers.TabIndex = 1;
            this.navCustomers.Text = "Customers";
            this.navCustomers.Click += new System.EventHandler(this.navCustomers_Click);
            this.navReports.Location = new System.Drawing.Point(12, 136);
            this.navReports.Name = "navReports";
            this.navReports.Size = new System.Drawing.Size(136, 34);
            this.navReports.TabIndex = 2;
            this.navReports.Text = "Reports";
            this.navReports.Click += new System.EventHandler(this.navReports_Click);
            //
            // pageHost  (the three ported screens are docked here; one visible at a time)
            //
            this.pageHost.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.pageHost.Dock = Wisej.Web.DockStyle.Fill;
            this.pageHost.Name = "pageHost";
            this.pageHost.Padding = new Wisej.Web.Padding(12);
            this.pageHost.Size = new System.Drawing.Size(680, 600);
            //
            // timerTabOrder
            //
            this.timerTabOrder.Interval = 700;
            this.timerTabOrder.Tick += new System.EventHandler(this.timerTabOrder_Tick);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.shell);
            this.Controls.Add(this.trace);
            this.Controls.Add(this.appBar);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1400, 760);
            this.Text = "OrderDesk — Module 3";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.appBar.ResumeLayout(false);
            this.shell.ResumeLayout(false);
            this.buttonBar.ResumeLayout(false);
            this.navPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel appBar;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label labelStatus;
        private OrderDesk.Shared.TracePanel trace;
        private Wisej.Web.Panel shell;
        private Wisej.Web.MenuBar menuBar;
        private Wisej.Web.MenuItem fileMenu;
        private Wisej.Web.MenuItem exitMenuItem;
        private Wisej.Web.MenuItem editMenu;
        private Wisej.Web.MenuItem editSelectedMenuItem;
        private Wisej.Web.MenuItem newOrderMenuItem;
        private Wisej.Web.MenuItem viewMenu;
        private Wisej.Web.MenuItem viewOrdersMenuItem;
        private Wisej.Web.MenuItem viewCustomersMenuItem;
        private Wisej.Web.MenuItem viewReportsMenuItem;
        private Wisej.Web.MenuItem reportsMenu;
        private Wisej.Web.MenuItem reportOpenOrdersMenuItem;
        private Wisej.Web.MenuItem reportInvoicesMenuItem;
        private Wisej.Web.MenuItem helpMenu;
        private Wisej.Web.MenuItem aboutMenuItem;
        private Wisej.Web.StatusBar statusBar;
        private Wisej.Web.StatusBarPanel statusPanelMain;
        private Wisej.Web.StatusBarPanel statusPanelDialogs;
        private Wisej.Web.StatusBarPanel statusPanelPage;
        private Wisej.Web.Panel buttonBar;
        private Wisej.Web.Button buttonEditSelected;
        private Wisej.Web.Button buttonLeak;
        private Wisej.Web.Button buttonDisposeFix;
        private Wisej.Web.Button buttonToast;
        private Wisej.Web.Button buttonTabOrder;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Panel navPanel;
        private Wisej.Web.Label labelBrand;
        private Wisej.Web.Button navOrders;
        private Wisej.Web.Button navCustomers;
        private Wisej.Web.Button navReports;
        private Wisej.Web.Panel pageHost;
        private Wisej.Web.Timer timerTabOrder;
    }
}
