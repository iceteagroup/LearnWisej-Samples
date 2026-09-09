namespace LegacyOrderDesk
{
    partial class OrdersForm
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.reportsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ordersGrid = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCustomer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.detailPanel = new System.Windows.Forms.Panel();
            this.detailTitle = new System.Windows.Forms.Label();
            this.labelCustomer = new System.Windows.Forms.Label();
            this.detailCustomer = new System.Windows.Forms.TextBox();
            this.labelPo = new System.Windows.Forms.Label();
            this.detailPo = new System.Windows.Forms.TextBox();
            this.labelLines = new System.Windows.Forms.Label();
            this.detailLines = new System.Windows.Forms.TextBox();
            this.newOrderButton = new System.Windows.Forms.Button();
            this.printInvoiceButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ordersGrid)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.detailPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip
            //
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.fileMenu, this.editMenu, this.viewMenu, this.reportsMenu, this.helpMenu });
            this.menuStrip.Name = "menuStrip";
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.exitMenuItem });
            this.fileMenu.Text = "File";
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            this.editMenu.Text = "Edit";
            this.viewMenu.Text = "View";
            this.reportsMenu.Text = "Reports";
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.aboutMenuItem });
            this.helpMenu.Text = "Help";
            this.aboutMenuItem.Text = "About";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            //
            // ordersGrid
            //
            this.ordersGrid.AllowUserToAddRows = false;
            this.ordersGrid.AllowUserToDeleteRows = false;
            this.ordersGrid.AutoGenerateColumns = false;
            this.ordersGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.ordersGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.colId, this.colCustomer, this.colTotal, this.colStatus });
            this.ordersGrid.Location = new System.Drawing.Point(0, 27);
            this.ordersGrid.MultiSelect = false;
            this.ordersGrid.Name = "ordersGrid";
            this.ordersGrid.ReadOnly = true;
            this.ordersGrid.RowHeadersVisible = false;
            this.ordersGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ordersGrid.Size = new System.Drawing.Size(496, 292);
            this.ordersGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ordersGrid_CellDoubleClick);
            this.ordersGrid.SelectionChanged += new System.EventHandler(this.ordersGrid_SelectionChanged);
            this.colId.DataPropertyName = "Id";
            this.colId.HeaderText = "Order";
            this.colId.Width = 70;
            this.colCustomer.DataPropertyName = "CustomerName";
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTotal.DataPropertyName = "Total";
            this.colTotal.HeaderText = "Total";
            this.colTotal.Width = 90;
            this.colTotal.DefaultCellStyle.Format = "C2";
            this.colTotal.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Width = 80;
            //
            // detailPanel
            //
            this.detailPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.detailPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
            this.detailPanel.Location = new System.Drawing.Point(500, 27);
            this.detailPanel.Name = "detailPanel";
            this.detailPanel.Size = new System.Drawing.Size(220, 292);
            this.detailTitle.AutoSize = true;
            this.detailTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.detailTitle.Location = new System.Drawing.Point(12, 12);
            this.detailTitle.Text = "Order 1042";
            this.labelCustomer.AutoSize = true;
            this.labelCustomer.Location = new System.Drawing.Point(12, 40);
            this.labelCustomer.Text = "Customer";
            this.detailCustomer.Location = new System.Drawing.Point(12, 58);
            this.detailCustomer.ReadOnly = true;
            this.detailCustomer.Width = 192;
            this.labelPo.AutoSize = true;
            this.labelPo.Location = new System.Drawing.Point(12, 88);
            this.labelPo.Text = "PO #";
            this.detailPo.Location = new System.Drawing.Point(12, 106);
            this.detailPo.ReadOnly = true;
            this.detailPo.Width = 192;
            this.labelLines.AutoSize = true;
            this.labelLines.Location = new System.Drawing.Point(12, 136);
            this.labelLines.Text = "Lines";
            this.detailLines.Location = new System.Drawing.Point(12, 154);
            this.detailLines.ReadOnly = true;
            this.detailLines.Width = 192;
            this.newOrderButton.Location = new System.Drawing.Point(12, 196);
            this.newOrderButton.Size = new System.Drawing.Size(192, 26);
            this.newOrderButton.Text = "New Order";
            this.newOrderButton.Click += new System.EventHandler(this.newOrderButton_Click);
            this.printInvoiceButton.Location = new System.Drawing.Point(12, 226);
            this.printInvoiceButton.Size = new System.Drawing.Size(192, 26);
            this.printInvoiceButton.Text = "Print Invoice";
            this.printInvoiceButton.Click += new System.EventHandler(this.printInvoiceButton_Click);
            this.exportButton.Location = new System.Drawing.Point(12, 256);
            this.exportButton.Size = new System.Drawing.Size(192, 26);
            this.exportButton.Text = "Export to Excel";
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.statusLabel });
            this.statusStrip.Name = "statusStrip";
            this.statusLabel.Text = "Ready · single-user desktop";
            //
            // OrdersForm
            //
            this.ClientSize = new System.Drawing.Size(720, 341);
            this.Controls.Add(this.ordersGrid);
            this.Controls.Add(this.detailPanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(640, 300);
            this.Name = "OrdersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LegacyOrderDesk — Orders";
            this.Load += new System.EventHandler(this.OrdersForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ordersGrid)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.detailPanel.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMenu;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem reportsMenu;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.DataGridView ordersGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCustomer;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.Panel detailPanel;
        private System.Windows.Forms.Label detailTitle;
        private System.Windows.Forms.Label labelCustomer;
        private System.Windows.Forms.TextBox detailCustomer;
        private System.Windows.Forms.Label labelPo;
        private System.Windows.Forms.TextBox detailPo;
        private System.Windows.Forms.Label labelLines;
        private System.Windows.Forms.TextBox detailLines;
        private System.Windows.Forms.Button newOrderButton;
        private System.Windows.Forms.Button printInvoiceButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
    }
}
