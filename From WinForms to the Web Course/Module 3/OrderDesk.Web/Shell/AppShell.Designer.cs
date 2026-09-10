namespace OrderDesk.Shell
{
    partial class AppShell
    {
        // Designer-owned file: InitializeComponent, the component container and the field declarations.
        // Migration logic lives in AppShell.cs (NavigateTo, the screen cache, the Trace event).
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
            this.menuBar = new Wisej.Web.MenuBar();                                   // ✓ was: System.Windows.Forms.MenuStrip
            this.menuFile = new Wisej.Web.MenuItem();                                 // ✓ was: ToolStripMenuItem (×14)
            this.menuFileSettings = new Wisej.Web.MenuItem();
            this.menuFileExit = new Wisej.Web.MenuItem();
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
            this.toolBar = new Wisej.Web.ToolBar();                                   // ✓ was: System.Windows.Forms.ToolStrip (LegacyOrderDesk kept the actions as buttons in the detail GroupBox)
            this.toolOrders = new Wisej.Web.ToolBarButton();                          // ✓ was: ToolStripButton
            this.toolCustomers = new Wisej.Web.ToolBarButton();
            this.toolReports = new Wisej.Web.ToolBarButton();
            this.toolSeparator = new Wisej.Web.ToolBarButton();
            this.toolNewOrder = new Wisej.Web.ToolBarButton();
            this.toolPrintInvoice = new Wisej.Web.ToolBarButton();
            this.toolExport = new Wisej.Web.ToolBarButton();
            this.screenHost = new Wisej.Web.Panel();                                  // ✓ the content host: one UserControl per screen, Dock = Fill, swapped by NavigateTo
            this.statusBar = new Wisej.Web.StatusBar();                               // ✓ was: System.Windows.Forms.StatusStrip
            this.statusPanel = new Wisej.Web.StatusBarPanel();                        // ✓ was: ToolStripStatusLabel
            this.SuspendLayout();
            //
            // menuBar  (File · View · Reports · Help — the MenuStrip of OrdersForm, item for item)
            //
            this.menuBar.Dock = Wisej.Web.DockStyle.Top;
            this.menuBar.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.menuFile, this.menuView, this.menuReports, this.menuHelp });   // ✓ was: menuStrip.Items.AddRange(new ToolStripItem[] …)
            this.menuBar.Name = "menuBar";
            //
            // File
            //
            this.menuFile.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.menuFileSettings, this.menuFileExit });   // ✓ was: DropDownItems.AddRange
            this.menuFile.Name = "menuFile";
            this.menuFile.Text = "&File";
            this.menuFileSettings.Name = "menuFileSettings";
            this.menuFileSettings.Text = "&Settings…";
            this.menuFileSettings.Click += new System.EventHandler(this.menuFileSettings_Click);
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.Text = "E&xit";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
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
            this.toolOrders.ToolTipText = "NavigateTo(\"Orders\")";
            this.toolOrders.Click += new System.EventHandler(this.toolOrders_Click);
            this.toolCustomers.Name = "toolCustomers";
            this.toolCustomers.Text = "Customers";
            this.toolCustomers.ToolTipText = "NavigateTo(\"Customers\")";
            this.toolCustomers.Click += new System.EventHandler(this.toolCustomers_Click);
            this.toolReports.Name = "toolReports";
            this.toolReports.Text = "Reports";
            this.toolReports.ToolTipText = "NavigateTo(\"Reports\")";
            this.toolReports.Click += new System.EventHandler(this.toolReports_Click);
            this.toolSeparator.Name = "toolSeparator";
            this.toolSeparator.Style = Wisej.Web.ToolBarButtonStyle.Separator;
            this.toolNewOrder.Name = "toolNewOrder";
            this.toolNewOrder.Text = "New Order";
            this.toolNewOrder.ToolTipText = "Opens the ported EditOrderDialog for a new order (disposed by the caller).";
            this.toolNewOrder.Click += new System.EventHandler(this.toolNewOrder_Click);
            this.toolPrintInvoice.Name = "toolPrintInvoice";
            this.toolPrintInvoice.Text = "Print Invoice";
            this.toolPrintInvoice.ToolTipText = "Server PDF in a PdfViewer for the selected order (Reports screen).";
            this.toolPrintInvoice.Click += new System.EventHandler(this.toolPrintInvoice_Click);
            this.toolExport.Name = "toolExport";
            this.toolExport.Text = "Export";
            this.toolExport.ToolTipText = "orders.csv built in memory → Application.Download.";
            this.toolExport.Click += new System.EventHandler(this.toolExport_Click);
            //
            // screenHost  (the content area: whatever screen NavigateTo selected, Dock = Fill)
            //
            this.screenHost.BackColor = System.Drawing.Color.White;
            this.screenHost.Dock = Wisej.Web.DockStyle.Fill;
            this.screenHost.Name = "screenHost";
            //
            // statusBar
            //
            this.statusBar.Dock = Wisej.Web.DockStyle.Bottom;
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new Wisej.Web.StatusBarPanel[] { this.statusPanel });   // ✓ was: statusStrip.Items.AddRange
            this.statusBar.ShowPanels = true;
            this.statusPanel.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Spring;
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Text = "Ready";
            //
            // AppShell
            //
            // Dock order matters: the Fill control is added first, then the Top/Bottom bars. Docking is applied in
            // reverse order of the Controls collection, so statusBar and menuBar take their edges first, toolBar
            // docks under the menu and screenHost gets what is left — the same rule the WinForms designer follows.
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.screenHost);
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.menuBar);
            this.Controls.Add(this.statusBar);
            this.Name = "AppShell";
            this.Size = new System.Drawing.Size(616, 396);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.MenuBar menuBar;
        private Wisej.Web.MenuItem menuFile;
        private Wisej.Web.MenuItem menuFileSettings;
        private Wisej.Web.MenuItem menuFileExit;
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
