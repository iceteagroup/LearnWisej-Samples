namespace OrderDesk.Shell
{
    partial class AppShell
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuBar = new Wisej.Web.MenuBar();
            this.menuView = new Wisej.Web.MenuItem();
            this.menuViewOrders = new Wisej.Web.MenuItem();
            this.menuViewCustomers = new Wisej.Web.MenuItem();
            this.menuViewReports = new Wisej.Web.MenuItem();
            this.menuViewSeparator = new Wisej.Web.MenuItem();
            this.menuViewOpenOnly = new Wisej.Web.MenuItem();
            this.menuViewAllOrders = new Wisej.Web.MenuItem();
            this.menuReports = new Wisej.Web.MenuItem();
            this.menuReportsPrintInvoice = new Wisej.Web.MenuItem();
            this.menuReportsExport = new Wisej.Web.MenuItem();
            this.menuHelp = new Wisej.Web.MenuItem();
            this.menuHelpAbout = new Wisej.Web.MenuItem();
            this.toolBar = new Wisej.Web.ToolBar();
            this.toolOrders = new Wisej.Web.ToolBarButton();
            this.toolCustomers = new Wisej.Web.ToolBarButton();
            this.toolReports = new Wisej.Web.ToolBarButton();
            this.toolSeparator = new Wisej.Web.ToolBarButton();
            this.toolNewOrder = new Wisej.Web.ToolBarButton();
            this.toolPrintInvoice = new Wisej.Web.ToolBarButton();
            this.toolExport = new Wisej.Web.ToolBarButton();
            this.screenHost = new Wisej.Web.Panel();
            this.statusBar = new Wisej.Web.StatusBar();
            this.statusPanel = new Wisej.Web.StatusBarPanel();
            this.SuspendLayout();
            //
            // menuBar  (View · Reports · Help)
            //
            this.menuBar.Dock = Wisej.Web.DockStyle.Top;
            this.menuBar.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.menuView, this.menuReports, this.menuHelp });
            this.menuBar.Name = "menuBar";
            //
            // View  (the three screens + the two filters OrdersForm had)
            //
            this.menuView.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.menuViewOrders, this.menuViewCustomers, this.menuViewReports, this.menuViewSeparator, this.menuViewOpenOnly, this.menuViewAllOrders });
            this.menuView.Name = "menuView";
            this.menuView.Text = "&View";
            this.menuViewOrders.Name = "menuViewOrders";
            this.menuViewOrders.Text = "&Orders";
            this.menuViewOrders.Click += new System.EventHandler(this.menuViewOrders_Click);
            this.menuViewCustomers.Name = "menuViewCustomers";
            this.menuViewCustomers.Text = "&Customers";
            this.menuViewCustomers.Click += new System.EventHandler(this.menuViewCustomers_Click);
            this.menuViewReports.Name = "menuViewReports";
            this.menuViewReports.Text = "&Reports";
            this.menuViewReports.Click += new System.EventHandler(this.menuViewReports_Click);
            this.menuViewSeparator.Name = "menuViewSeparator";
            this.menuViewSeparator.Text = "-";
            this.menuViewOpenOnly.Name = "menuViewOpenOnly";
            this.menuViewOpenOnly.Text = "Open orders only";
            this.menuViewOpenOnly.Click += new System.EventHandler(this.menuViewOpenOnly_Click);
            this.menuViewAllOrders.Name = "menuViewAllOrders";
            this.menuViewAllOrders.Text = "All orders";
            this.menuViewAllOrders.Click += new System.EventHandler(this.menuViewAllOrders_Click);
            //
            // Reports
            //
            this.menuReports.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.menuReportsPrintInvoice, this.menuReportsExport });
            this.menuReports.Name = "menuReports";
            this.menuReports.Text = "&Reports";
            this.menuReportsPrintInvoice.Name = "menuReportsPrintInvoice";
            this.menuReportsPrintInvoice.Text = "Print &Invoice";
            this.menuReportsPrintInvoice.Click += new System.EventHandler(this.menuReportsPrintInvoice_Click);
            this.menuReportsExport.Name = "menuReportsExport";
            this.menuReportsExport.Text = "&Export orders";
            this.menuReportsExport.Click += new System.EventHandler(this.menuReportsExport_Click);
            //
            // Help
            //
            this.menuHelp.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.menuHelpAbout });
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Text = "&Help";
            this.menuHelpAbout.Name = "menuHelpAbout";
            this.menuHelpAbout.Text = "&About";
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
            //
            // toolBar  (Orders · Customers · Reports | New Order · Print Invoice · Export)
            //
            this.toolBar.Buttons.AddRange(new Wisej.Web.ToolBarButton[] { this.toolOrders, this.toolCustomers, this.toolReports, this.toolSeparator, this.toolNewOrder, this.toolPrintInvoice, this.toolExport });
            this.toolBar.Dock = Wisej.Web.DockStyle.Top;
            this.toolBar.Name = "toolBar";
            this.toolOrders.Name = "toolOrders";
            this.toolOrders.Text = "Orders";
            this.toolOrders.Click += new System.EventHandler(this.toolOrders_Click);
            this.toolCustomers.Name = "toolCustomers";
            this.toolCustomers.Text = "Customers";
            this.toolCustomers.Click += new System.EventHandler(this.toolCustomers_Click);
            this.toolReports.Name = "toolReports";
            this.toolReports.Text = "Reports";
            this.toolReports.Click += new System.EventHandler(this.toolReports_Click);
            this.toolSeparator.Name = "toolSeparator";
            this.toolSeparator.Style = Wisej.Web.ToolBarButtonStyle.Separator;
            this.toolNewOrder.Name = "toolNewOrder";
            this.toolNewOrder.Text = "New Order";
            this.toolNewOrder.Click += new System.EventHandler(this.toolNewOrder_Click);
            this.toolPrintInvoice.Name = "toolPrintInvoice";
            this.toolPrintInvoice.Text = "Print Invoice";
            this.toolPrintInvoice.Click += new System.EventHandler(this.toolPrintInvoice_Click);
            this.toolExport.Name = "toolExport";
            this.toolExport.Text = "Export";
            this.toolExport.Click += new System.EventHandler(this.toolExport_Click);
            //
            // screenHost
            //
            this.screenHost.BackColor = System.Drawing.Color.White;
            this.screenHost.Dock = Wisej.Web.DockStyle.Fill;
            this.screenHost.Name = "screenHost";
            //
            // statusBar
            //
            this.statusBar.Dock = Wisej.Web.DockStyle.Bottom;
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new Wisej.Web.StatusBarPanel[] { this.statusPanel });
            this.statusBar.ShowPanels = true;
            this.statusPanel.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Spring;
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Text = "Ready";
            //
            // AppShell  (docking is applied in reverse Controls order: the Fill control is added first)
            //
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.screenHost);
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.menuBar);
            this.Controls.Add(this.statusBar);
            this.Name = "AppShell";
            this.Size = new System.Drawing.Size(1024, 640);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.MenuBar menuBar;
        private Wisej.Web.MenuItem menuView;
        private Wisej.Web.MenuItem menuViewOrders;
        private Wisej.Web.MenuItem menuViewCustomers;
        private Wisej.Web.MenuItem menuViewReports;
        private Wisej.Web.MenuItem menuViewSeparator;
        private Wisej.Web.MenuItem menuViewOpenOnly;
        private Wisej.Web.MenuItem menuViewAllOrders;
        private Wisej.Web.MenuItem menuReports;
        private Wisej.Web.MenuItem menuReportsPrintInvoice;
        private Wisej.Web.MenuItem menuReportsExport;
        private Wisej.Web.MenuItem menuHelp;
        private Wisej.Web.MenuItem menuHelpAbout;
        private Wisej.Web.ToolBar toolBar;
        private Wisej.Web.ToolBarButton toolOrders;
        private Wisej.Web.ToolBarButton toolCustomers;
        private Wisej.Web.ToolBarButton toolReports;
        private Wisej.Web.ToolBarButton toolSeparator;
        private Wisej.Web.ToolBarButton toolNewOrder;
        private Wisej.Web.ToolBarButton toolPrintInvoice;
        private Wisej.Web.ToolBarButton toolExport;
        private Wisej.Web.Panel screenHost;
        private Wisej.Web.StatusBar statusBar;
        private Wisej.Web.StatusBarPanel statusPanel;
    }
}
