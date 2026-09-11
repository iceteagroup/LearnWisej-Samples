namespace OrderDesk
{
    partial class OrdersForm
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
            this.menuStrip = new Wisej.Web.MenuBar();
            this.fileMenu = new Wisej.Web.MenuItem();
            this.settingsMenuItem = new Wisej.Web.MenuItem();
            this.exitMenuItem = new Wisej.Web.MenuItem();
            this.viewMenu = new Wisej.Web.MenuItem();
            this.filterOpenMenuItem = new Wisej.Web.MenuItem();
            this.filterAllMenuItem = new Wisej.Web.MenuItem();
            this.reportsMenu = new Wisej.Web.MenuItem();
            this.printInvoiceMenuItem = new Wisej.Web.MenuItem();
            this.helpMenu = new Wisej.Web.MenuItem();
            this.aboutMenuItem = new Wisej.Web.MenuItem();
            this.ordersGrid = new Wisej.Web.DataGridView();
            this.orderColumn = new Wisej.Web.DataGridViewTextBoxColumn();
            this.customerColumn = new Wisej.Web.DataGridViewTextBoxColumn();
            this.totalColumn = new Wisej.Web.DataGridViewTextBoxColumn();
            this.statusColumn = new Wisej.Web.DataGridViewTextBoxColumn();
            this.detailGroup = new Wisej.Web.GroupBox();
            this.customerLabel = new Wisej.Web.Label();
            this.customerValue = new Wisej.Web.Label();
            this.poLabel = new Wisej.Web.Label();
            this.poValue = new Wisej.Web.Label();
            this.linesLabel = new Wisej.Web.Label();
            this.linesValue = new Wisej.Web.Label();
            this.newOrderButton = new Wisej.Web.Button();
            this.printInvoiceButton = new Wisej.Web.Button();
            this.exportButton = new Wisej.Web.Button();
            this.attachButton = new Wisej.Web.Button();
            this.statusStrip = new Wisej.Web.StatusBar();
            this.statusLabel = new Wisej.Web.StatusBarPanel();
            ((System.ComponentModel.ISupportInitialize)(this.ordersGrid)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.detailGroup.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip  (File · View · Reports · Help)
            //
            this.fileMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.settingsMenuItem, this.exitMenuItem });
            this.fileMenu.Text = "&File";
            this.settingsMenuItem.Text = "&Settings…";
            this.settingsMenuItem.Click += new System.EventHandler(this.settingsMenuItem_Click);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            this.viewMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.filterOpenMenuItem, this.filterAllMenuItem });
            this.viewMenu.Text = "&View";
            this.filterOpenMenuItem.Text = "Open orders only";
            this.filterOpenMenuItem.Click += new System.EventHandler(this.filterOpenMenuItem_Click);
            this.filterAllMenuItem.Text = "All orders";
            this.filterAllMenuItem.Click += new System.EventHandler(this.filterAllMenuItem_Click);
            this.reportsMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.printInvoiceMenuItem });
            this.reportsMenu.Text = "&Reports";
            this.printInvoiceMenuItem.Text = "Print &Invoice";
            this.printInvoiceMenuItem.Click += new System.EventHandler(this.printInvoiceButton_Click);
            this.helpMenu.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.aboutMenuItem });
            this.helpMenu.Text = "&Help";
            this.aboutMenuItem.Text = "&About";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            this.menuStrip.MenuItems.AddRange(new Wisej.Web.MenuItem[] { this.fileMenu, this.viewMenu, this.reportsMenu, this.helpMenu });
            this.menuStrip.Dock = Wisej.Web.DockStyle.Top;
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            //
            // ordersGrid
            //
            this.ordersGrid.AllowUserToAddRows = false;
            this.ordersGrid.AllowUserToDeleteRows = false;
            this.ordersGrid.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.ordersGrid.AutoGenerateColumns = false;
            this.ordersGrid.ColumnHeadersHeightSizeMode = Wisej.Web.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ordersGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.orderColumn, this.customerColumn, this.totalColumn, this.statusColumn });
            this.ordersGrid.Location = new System.Drawing.Point(12, 36);
            this.ordersGrid.MultiSelect = false;
            this.ordersGrid.Name = "ordersGrid";
            this.ordersGrid.ReadOnly = true;
            this.ordersGrid.RowHeadersVisible = false;
            this.ordersGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.ordersGrid.Size = new System.Drawing.Size(460, 300);
            this.ordersGrid.TabIndex = 0;
            this.ordersGrid.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.ordersGrid_CellDoubleClick);
            this.ordersGrid.SelectionChanged += new System.EventHandler(this.ordersGrid_SelectionChanged);
            this.orderColumn.DataPropertyName = "Id";
            this.orderColumn.HeaderText = "Order";
            this.orderColumn.Name = "orderColumn";
            this.orderColumn.Width = 70;
            this.customerColumn.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.customerColumn.DataPropertyName = "CustomerName";
            this.customerColumn.HeaderText = "Customer";
            this.customerColumn.Name = "customerColumn";
            this.totalColumn.DataPropertyName = "Total";
            this.totalColumn.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.totalColumn.DefaultCellStyle.Format = "C2";
            this.totalColumn.HeaderText = "Total";
            this.totalColumn.Name = "totalColumn";
            this.totalColumn.Width = 100;
            this.statusColumn.DataPropertyName = "Status";
            this.statusColumn.HeaderText = "Status";
            this.statusColumn.Name = "statusColumn";
            this.statusColumn.Width = 90;
            //
            // detailGroup
            //
            this.detailGroup.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.detailGroup.Controls.Add(this.customerLabel);
            this.detailGroup.Controls.Add(this.customerValue);
            this.detailGroup.Controls.Add(this.poLabel);
            this.detailGroup.Controls.Add(this.poValue);
            this.detailGroup.Controls.Add(this.linesLabel);
            this.detailGroup.Controls.Add(this.linesValue);
            this.detailGroup.Controls.Add(this.newOrderButton);
            this.detailGroup.Controls.Add(this.printInvoiceButton);
            this.detailGroup.Controls.Add(this.exportButton);
            this.detailGroup.Controls.Add(this.attachButton);
            this.detailGroup.Location = new System.Drawing.Point(484, 36);
            this.detailGroup.Name = "detailGroup";
            this.detailGroup.Size = new System.Drawing.Size(220, 300);
            this.detailGroup.Text = "Order";
            this.customerLabel.AutoSize = true; this.customerLabel.Location = new System.Drawing.Point(12, 28); this.customerLabel.Text = "Customer";
            this.customerValue.AutoSize = true; this.customerValue.Location = new System.Drawing.Point(90, 28); this.customerValue.Name = "customerValue";
            this.poLabel.AutoSize = true; this.poLabel.Location = new System.Drawing.Point(12, 52); this.poLabel.Text = "PO #";
            this.poValue.AutoSize = true; this.poValue.Location = new System.Drawing.Point(90, 52); this.poValue.Name = "poValue";
            this.linesLabel.AutoSize = true; this.linesLabel.Location = new System.Drawing.Point(12, 76); this.linesLabel.Text = "Lines";
            this.linesValue.AutoSize = true; this.linesValue.Location = new System.Drawing.Point(90, 76); this.linesValue.Name = "linesValue";
            this.newOrderButton.Location = new System.Drawing.Point(12, 130); this.newOrderButton.Size = new System.Drawing.Size(196, 30); this.newOrderButton.Text = "New Order";
            this.newOrderButton.Click += new System.EventHandler(this.newOrderButton_Click);
            this.printInvoiceButton.Location = new System.Drawing.Point(12, 166); this.printInvoiceButton.Size = new System.Drawing.Size(196, 30); this.printInvoiceButton.Text = "Print Invoice";
            this.printInvoiceButton.Click += new System.EventHandler(this.printInvoiceButton_Click);
            this.exportButton.Location = new System.Drawing.Point(12, 202); this.exportButton.Size = new System.Drawing.Size(196, 30); this.exportButton.Text = "Export to Excel";
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            this.attachButton.Location = new System.Drawing.Point(12, 238); this.attachButton.Size = new System.Drawing.Size(196, 30); this.attachButton.Text = "Attach file…";
            this.attachButton.Click += new System.EventHandler(this.attachButton_Click);
            //
            // statusStrip
            //
            this.statusStrip.Panels.AddRange(new Wisej.Web.StatusBarPanel[] { this.statusLabel });
            this.statusStrip.Dock = Wisej.Web.DockStyle.Bottom;
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.ShowPanels = true;
            this.statusLabel.AutoSize = Wisej.Web.StatusBarPanelAutoSize.Spring;
            this.statusLabel.Text = "Ready";
            //
            // OrdersForm
            //
            this.ClientSize = new System.Drawing.Size(716, 372);
            this.Controls.Add(this.ordersGrid);
            this.Controls.Add(this.detailGroup);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MinimumSize = new System.Drawing.Size(640, 360);
            this.Name = "OrdersForm";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "Orders";
            this.Load += new System.EventHandler(this.OrdersForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ordersGrid)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.detailGroup.ResumeLayout(false);
            this.detailGroup.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.MenuBar menuStrip;
        private Wisej.Web.MenuItem fileMenu;
        private Wisej.Web.MenuItem settingsMenuItem;
        private Wisej.Web.MenuItem exitMenuItem;
        private Wisej.Web.MenuItem viewMenu;
        private Wisej.Web.MenuItem filterOpenMenuItem;
        private Wisej.Web.MenuItem filterAllMenuItem;
        private Wisej.Web.MenuItem reportsMenu;
        private Wisej.Web.MenuItem printInvoiceMenuItem;
        private Wisej.Web.MenuItem helpMenu;
        private Wisej.Web.MenuItem aboutMenuItem;
        private Wisej.Web.DataGridView ordersGrid;
        private Wisej.Web.DataGridViewTextBoxColumn orderColumn;
        private Wisej.Web.DataGridViewTextBoxColumn customerColumn;
        private Wisej.Web.DataGridViewTextBoxColumn totalColumn;
        private Wisej.Web.DataGridViewTextBoxColumn statusColumn;
        private Wisej.Web.GroupBox detailGroup;
        private Wisej.Web.Label customerLabel;
        private Wisej.Web.Label customerValue;
        private Wisej.Web.Label poLabel;
        private Wisej.Web.Label poValue;
        private Wisej.Web.Label linesLabel;
        private Wisej.Web.Label linesValue;
        private Wisej.Web.Button newOrderButton;
        private Wisej.Web.Button printInvoiceButton;
        private Wisej.Web.Button exportButton;
        private Wisej.Web.Button attachButton;
        private Wisej.Web.StatusBar statusStrip;
        private Wisej.Web.StatusBarPanel statusLabel;
    }
}
