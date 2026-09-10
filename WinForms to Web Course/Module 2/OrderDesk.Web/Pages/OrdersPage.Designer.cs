namespace OrderDesk.Pages
{
    partial class OrdersPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code   (was: Windows Form Designer generated code)

        // LegacyOrderDesk/OrdersForm.Designer.cs after the namespace swap. Every line that had to
        // change carries a "was:" comment — these are the 14 compiler errors of the first build.
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuBar = new Wisej.Web.MenuBar();                             // was: System.Windows.Forms.MenuStrip
            this.fileMenu = new Wisej.Web.MenuItem();                           // was: System.Windows.Forms.ToolStripMenuItem
            this.exitMenuItem = new Wisej.Web.MenuItem();
            this.editMenu = new Wisej.Web.MenuItem();
            this.viewMenu = new Wisej.Web.MenuItem();
            this.reportsMenu = new Wisej.Web.MenuItem();
            this.helpMenu = new Wisej.Web.MenuItem();
            this.aboutMenuItem = new Wisej.Web.MenuItem();
            this.ordersGrid = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTotal = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.detailPanel = new Wisej.Web.Panel();
            this.detailTitle = new Wisej.Web.Label();
            this.labelCustomer = new Wisej.Web.Label();
            this.detailCustomer = new Wisej.Web.TextBox();
            this.labelPo = new Wisej.Web.Label();
            this.detailPo = new Wisej.Web.TextBox();
            this.labelLines = new Wisej.Web.Label();
            this.detailLines = new Wisej.Web.TextBox();
            this.newOrderButton = new Wisej.Web.Button();
            this.printInvoiceButton = new Wisej.Web.Button();
            this.exportButton = new Wisej.Web.Button();
            this.statusBar = new Wisej.Web.StatusBar();                         // was: System.Windows.Forms.StatusStrip
            this.statusLabel = new Wisej.Web.StatusBarPanel();                  // was: System.Windows.Forms.ToolStripStatusLabel
            ((System.ComponentModel.ISupportInitialize)(this.ordersGrid)).BeginInit();
            this.detailPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // menuBar
            //
            this.menuBar.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.fileMenu, this.editMenu, this.viewMenu, this.reportsMenu, this.helpMenu });   // was: Items.AddRange(new ToolStripItem[] …)
            this.menuBar.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.menuBar.Location = new System.Drawing.Point(0, 0);
            this.menuBar.Name = "menuBar";
            this.menuBar.Size = new System.Drawing.Size(720, 28);
            this.fileMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.exitMenuItem });   // was: DropDownItems.AddRange
            this.fileMenu.Text = "File";
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            this.editMenu.Text = "Edit";
            this.viewMenu.Text = "View";
            this.reportsMenu.Text = "Reports";
            this.helpMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.aboutMenuItem });   // was: DropDownItems.AddRange
            this.helpMenu.Text = "Help";
            this.aboutMenuItem.Text = "About";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            //
            // ordersGrid
            //
            this.ordersGrid.AllowUserToAddRows = false;
            this.ordersGrid.AllowUserToDeleteRows = false;
            this.ordersGrid.AutoGenerateColumns = false;
            this.ordersGrid.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.ordersGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colId, this.colCustomer, this.colTotal, this.colStatus });
            this.ordersGrid.Location = new System.Drawing.Point(0, 28);
            this.ordersGrid.MultiSelect = false;
            this.ordersGrid.Name = "ordersGrid";
            this.ordersGrid.ReadOnly = true;
            this.ordersGrid.RowHeadersVisible = false;
            this.ordersGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.ordersGrid.Size = new System.Drawing.Size(496, 289);
            this.ordersGrid.CellDoubleClick += this.ordersGrid_CellDoubleClick;
            this.ordersGrid.SelectionChanged += new System.EventHandler(this.ordersGrid_SelectionChanged);
            this.colId.DataPropertyName = "Id";
            this.colId.HeaderText = "Order";
            this.colId.Width = 70;
            this.colCustomer.DataPropertyName = "CustomerName";
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colTotal.DataPropertyName = "Total";
            this.colTotal.HeaderText = "Total";
            this.colTotal.Width = 90;
            this.colTotal.DefaultCellStyle.Format = "C2";
            this.colTotal.DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.GetCultureInfo("en-US");   // the desktop used the PC's regional settings; the server formats for every browser
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Width = 80;
            //
            // detailPanel  (292 → 250 px tall: the hosted view is shorter than the 341 px form, so the rows are packed tighter)
            //
            this.detailPanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.detailPanel.BorderStyle = Wisej.Web.BorderStyle.Solid;        // was: BorderStyle.FixedSingle
            this.detailPanel.Controls.Add(this.detailTitle);
            this.detailPanel.Controls.Add(this.labelCustomer);
            this.detailPanel.Controls.Add(this.detailCustomer);
            this.detailPanel.Controls.Add(this.labelPo);
            this.detailPanel.Controls.Add(this.detailPo);
            this.detailPanel.Controls.Add(this.labelLines);
            this.detailPanel.Controls.Add(this.detailLines);
            this.detailPanel.Controls.Add(this.newOrderButton);
            this.detailPanel.Controls.Add(this.printInvoiceButton);
            this.detailPanel.Controls.Add(this.exportButton);
            this.detailPanel.Location = new System.Drawing.Point(500, 28);
            this.detailPanel.Name = "detailPanel";
            this.detailPanel.Size = new System.Drawing.Size(220, 289);
            this.detailTitle.AutoSize = true;
            this.detailTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);   // was: "Segoe UI", 9F
            this.detailTitle.Location = new System.Drawing.Point(12, 8);
            this.detailTitle.Text = "Order 1042";
            this.labelCustomer.AutoSize = true;
            this.labelCustomer.Location = new System.Drawing.Point(12, 30);
            this.labelCustomer.Text = "Customer";
            this.detailCustomer.Location = new System.Drawing.Point(12, 46);
            this.detailCustomer.ReadOnly = true;
            this.detailCustomer.Size = new System.Drawing.Size(192, 22);
            this.labelPo.AutoSize = true;
            this.labelPo.Location = new System.Drawing.Point(12, 72);
            this.labelPo.Text = "PO #";
            this.detailPo.Location = new System.Drawing.Point(12, 88);
            this.detailPo.ReadOnly = true;
            this.detailPo.Size = new System.Drawing.Size(192, 22);
            this.labelLines.AutoSize = true;
            this.labelLines.Location = new System.Drawing.Point(12, 114);
            this.labelLines.Text = "Lines";
            this.detailLines.Location = new System.Drawing.Point(12, 130);
            this.detailLines.ReadOnly = true;
            this.detailLines.Size = new System.Drawing.Size(192, 22);
            this.newOrderButton.Location = new System.Drawing.Point(12, 162);
            this.newOrderButton.Size = new System.Drawing.Size(192, 24);
            this.newOrderButton.Text = "New Order";
            this.newOrderButton.Click += new System.EventHandler(this.newOrderButton_Click);
            this.printInvoiceButton.Location = new System.Drawing.Point(12, 190);
            this.printInvoiceButton.Size = new System.Drawing.Size(192, 24);
            this.printInvoiceButton.Text = "Print Invoice";
            this.printInvoiceButton.Click += new System.EventHandler(this.printInvoiceButton_Click);
            this.exportButton.Location = new System.Drawing.Point(12, 218);
            this.exportButton.Size = new System.Drawing.Size(192, 24);
            this.exportButton.Text = "Export to Excel";
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            //
            // statusBar
            //
            this.statusBar.Panels.AddRange(new Wisej.Web.StatusBarPanel[] { this.statusLabel });   // was: Items.AddRange(new ToolStripItem[] …)
            this.statusBar.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBar.Location = new System.Drawing.Point(0, 317);
            this.statusBar.Name = "statusBar";
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(720, 24);
            this.statusLabel.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Spring;
            this.statusLabel.Text = "Ready · single-user desktop";
            //
            // OrdersPage  (was: OrdersForm)
            //
            this.Size = new System.Drawing.Size(720, 341);                      // was: ClientSize (a hosted view has no window frame)
            this.Controls.Add(this.ordersGrid);
            this.Controls.Add(this.detailPanel);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuBar);
            //  was: this.MainMenuStrip = this.menuStrip;            — no such property; the MenuBar is a control
            this.MinimumSize = new System.Drawing.Size(640, 300);
            this.Name = "OrdersPage";
            //  was: this.StartPosition = FormStartPosition.CenterScreen;  — there is no screen to center on
            this.Text = "LegacyOrderDesk — Orders";
            this.Load += new System.EventHandler(this.OrdersPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ordersGrid)).EndInit();
            this.detailPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.MenuBar menuBar;                     // was: System.Windows.Forms.MenuStrip
        private Wisej.Web.MenuItem fileMenu;                   // was: System.Windows.Forms.ToolStripMenuItem
        private Wisej.Web.MenuItem exitMenuItem;
        private Wisej.Web.MenuItem editMenu;
        private Wisej.Web.MenuItem viewMenu;
        private Wisej.Web.MenuItem reportsMenu;
        private Wisej.Web.MenuItem helpMenu;
        private Wisej.Web.MenuItem aboutMenuItem;
        private Wisej.Web.DataGridView ordersGrid;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colTotal;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.Panel detailPanel;
        private Wisej.Web.Label detailTitle;
        private Wisej.Web.Label labelCustomer;
        private Wisej.Web.TextBox detailCustomer;
        private Wisej.Web.Label labelPo;
        private Wisej.Web.TextBox detailPo;
        private Wisej.Web.Label labelLines;
        private Wisej.Web.TextBox detailLines;
        private Wisej.Web.Button newOrderButton;
        private Wisej.Web.Button printInvoiceButton;
        private Wisej.Web.Button exportButton;
        private Wisej.Web.StatusBar statusBar;                 // was: System.Windows.Forms.StatusStrip
        private Wisej.Web.StatusBarPanel statusLabel;          // was: System.Windows.Forms.ToolStripStatusLabel
    }
}
