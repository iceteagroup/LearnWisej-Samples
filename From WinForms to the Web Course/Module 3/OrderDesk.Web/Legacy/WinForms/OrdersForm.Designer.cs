// ✕ LegacyOrderDesk "before" source, copied unchanged for side-by-side reading. NOT compiled (see OrderDesk.Web.csproj):
// ✕ System.Windows.Forms does not exist in the web project; the Wisej.NET port of this file lives in Shell/, Screens/ or Dialogs/.

namespace LegacyOrderDesk
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();      // ✕ MenuStrip → Wisej.Web.MenuBar (Shell/AppShell.Designer.cs)
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.filterOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.printInvoiceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ordersGrid = new System.Windows.Forms.DataGridView();
            this.orderColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.customerColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.detailGroup = new System.Windows.Forms.GroupBox();
            this.customerLabel = new System.Windows.Forms.Label();
            this.customerValue = new System.Windows.Forms.Label();
            this.poLabel = new System.Windows.Forms.Label();
            this.poValue = new System.Windows.Forms.Label();
            this.linesLabel = new System.Windows.Forms.Label();
            this.linesValue = new System.Windows.Forms.Label();
            this.newOrderButton = new System.Windows.Forms.Button();
            this.printInvoiceButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.attachButton = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();  // ✕ StatusStrip → Wisej.Web.StatusBar
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ordersGrid)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.detailGroup.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip  (File · View · Reports · Help)
            //
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.settingsMenuItem, this.exitMenuItem });
            this.fileMenu.Text = "&File";
            this.settingsMenuItem.Text = "&Settings…";
            this.settingsMenuItem.Click += new System.EventHandler(this.settingsMenuItem_Click);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.filterOpenMenuItem, this.filterAllMenuItem });
            this.viewMenu.Text = "&View";
            this.filterOpenMenuItem.Text = "Open orders only";
            this.filterOpenMenuItem.Click += new System.EventHandler(this.filterOpenMenuItem_Click);
            this.filterAllMenuItem.Text = "All orders";
            this.filterAllMenuItem.Click += new System.EventHandler(this.filterAllMenuItem_Click);
            this.reportsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.printInvoiceMenuItem });
            this.reportsMenu.Text = "&Reports";
            this.printInvoiceMenuItem.Text = "Print &Invoice";
            this.printInvoiceMenuItem.Click += new System.EventHandler(this.printInvoiceButton_Click);
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.aboutMenuItem });
            this.helpMenu.Text = "&Help";
            this.aboutMenuItem.Text = "&About";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.fileMenu, this.viewMenu, this.reportsMenu, this.helpMenu });
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            //
            // ordersGrid
            //
            this.ordersGrid.AllowUserToAddRows = false;
            this.ordersGrid.AllowUserToDeleteRows = false;
            this.ordersGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.ordersGrid.AutoGenerateColumns = false;
            this.ordersGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ordersGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.orderColumn, this.customerColumn, this.totalColumn, this.statusColumn });
            this.ordersGrid.Location = new System.Drawing.Point(12, 36);
            this.ordersGrid.MultiSelect = false;
            this.ordersGrid.Name = "ordersGrid";
            this.ordersGrid.ReadOnly = true;
            this.ordersGrid.RowHeadersVisible = false;
            this.ordersGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ordersGrid.Size = new System.Drawing.Size(460, 300);
            this.ordersGrid.TabIndex = 0;
            this.ordersGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ordersGrid_CellDoubleClick);
            this.ordersGrid.SelectionChanged += new System.EventHandler(this.ordersGrid_SelectionChanged);
            this.orderColumn.DataPropertyName = "Id";
            this.orderColumn.HeaderText = "Order";
            this.orderColumn.Name = "orderColumn";
            this.orderColumn.Width = 70;
            this.customerColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.customerColumn.DataPropertyName = "CustomerName";
            this.customerColumn.HeaderText = "Customer";
            this.customerColumn.Name = "customerColumn";
            this.totalColumn.DataPropertyName = "Total";
            this.totalColumn.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.totalColumn.DefaultCellStyle.Format = "C2";
            this.totalColumn.HeaderText = "Total";
            this.totalColumn.Name = "totalColumn";
            this.totalColumn.Width = 100;
            this.statusColumn.DataPropertyName = "Status";
            this.statusColumn.HeaderText = "Status";
            this.statusColumn.Name = "statusColumn";
            this.statusColumn.Width = 90;
            //
            // detailGroup  (right-hand detail panel)
            //
            this.detailGroup.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
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
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.statusLabel });
            this.statusStrip.Name = "statusStrip";
            this.statusLabel.Text = "Ready";
            //
            // OrdersForm
            //
            this.ClientSize = new System.Drawing.Size(716, 372);   // ✕ fixed 716×372 designed for a 1024×768 desktop; the browser viewport is whatever the user has
            this.Controls.Add(this.ordersGrid);
            this.Controls.Add(this.detailGroup);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(640, 360);
            this.Name = "OrdersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;   // ✕ there is no screen to centre on: the main form becomes a Page filling the browser tab
            this.Text = "LegacyOrderDesk — Orders";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OrdersForm_FormClosing);
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

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem settingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem filterOpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportsMenu;
        private System.Windows.Forms.ToolStripMenuItem printInvoiceMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.DataGridView ordersGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn orderColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn customerColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusColumn;
        private System.Windows.Forms.GroupBox detailGroup;
        private System.Windows.Forms.Label customerLabel;
        private System.Windows.Forms.Label customerValue;
        private System.Windows.Forms.Label poLabel;
        private System.Windows.Forms.Label poValue;
        private System.Windows.Forms.Label linesLabel;
        private System.Windows.Forms.Label linesValue;
        private System.Windows.Forms.Button newOrderButton;
        private System.Windows.Forms.Button printInvoiceButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button attachButton;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
    }
}
