namespace OrderDesk.Pages
{
    partial class OrdersPage
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

        // Ported from LegacyOrderDesk/OrdersForm.Designer.cs:
        //   System.Windows.Forms.*        → Wisej.Web.*
        //   Form                          → Panel (OrdersPage, hosted by the MainPage shell)
        //   MenuStrip / StatusStrip       → moved to the shell as MenuBar / StatusBar (MainPage.Designer.cs)
        //   Anchor Top|Bottom|Left|Right  → Dock (grid Fill, detail panel Right 220, header Top 36)
        //   ClientSize / MinimumSize / StartPosition / MainMenuStrip → removed (designer-only on a Form)
        //   ISupportInitialize BeginInit/EndInit on the grid → removed (not needed by Wisej.Web.DataGridView)
        //   TabIndex values kept: grid 0 · detail panel 1 (Customer 0, PO 1, Lines 2, New Order 3, Print 4, Export 5)
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.headerLabel = new Wisej.Web.Label();
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
            this.detailPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // headerLabel  (the blue page header the walkthrough shows above the grid)
            //
            this.headerLabel.AutoSize = false;
            this.headerLabel.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.headerLabel.Dock = Wisej.Web.DockStyle.Top;
            this.headerLabel.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.headerLabel.ForeColor = System.Drawing.Color.White;
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Padding = new Wisej.Web.Padding(16, 0, 0, 0);
            this.headerLabel.Size = new System.Drawing.Size(680, 36);
            this.headerLabel.TabIndex = 2;
            this.headerLabel.TabStop = false;
            this.headerLabel.Text = "Orders";
            this.headerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ordersGrid
            //
            this.ordersGrid.AllowUserToAddRows = false;
            this.ordersGrid.AllowUserToDeleteRows = false;
            this.ordersGrid.AutoGenerateColumns = false;
            this.ordersGrid.BorderStyle = Wisej.Web.BorderStyle.None;
            this.ordersGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colId, this.colCustomer, this.colTotal, this.colStatus });
            this.ordersGrid.Dock = Wisej.Web.DockStyle.Fill;
            this.ordersGrid.MultiSelect = false;
            this.ordersGrid.Name = "ordersGrid";
            this.ordersGrid.ReadOnly = true;
            this.ordersGrid.RowHeadersVisible = false;
            this.ordersGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.ordersGrid.TabIndex = 0;
            this.ordersGrid.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.ordersGrid_CellDoubleClick);
            this.ordersGrid.SelectionChanged += new System.EventHandler(this.ordersGrid_SelectionChanged);
            //
            // columns (unchanged from the WinForms designer)
            //
            this.colId.DataPropertyName = "Id";
            this.colId.HeaderText = "Order";
            this.colId.Name = "colId";
            this.colId.Width = 70;
            this.colCustomer.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colCustomer.DataPropertyName = "CustomerName";
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.Name = "colCustomer";
            this.colTotal.DataPropertyName = "Total";
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colTotal.DefaultCellStyle.Format = "C2";
            this.colTotal.HeaderText = "Total";
            this.colTotal.Name = "colTotal";
            this.colTotal.Width = 100;
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 90;
            //
            // detailPanel  (was Anchor Top|Bottom|Right at x=500 — now Dock Right)
            //
            this.detailPanel.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.detailPanel.BorderStyle = Wisej.Web.BorderStyle.None;
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
            this.detailPanel.Dock = Wisej.Web.DockStyle.Right;
            this.detailPanel.Name = "detailPanel";
            this.detailPanel.Size = new System.Drawing.Size(220, 400);
            this.detailPanel.TabIndex = 1;
            //
            // detailTitle
            //
            this.detailTitle.AutoSize = false;
            this.detailTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.detailTitle.Location = new System.Drawing.Point(14, 12);
            this.detailTitle.Name = "detailTitle";
            this.detailTitle.Size = new System.Drawing.Size(192, 22);
            this.detailTitle.TabIndex = 6;
            this.detailTitle.TabStop = false;
            this.detailTitle.Text = "Order 1042";
            //
            // labelCustomer / detailCustomer
            //
            this.labelCustomer.AutoSize = false;
            this.labelCustomer.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelCustomer.Location = new System.Drawing.Point(14, 42);
            this.labelCustomer.Name = "labelCustomer";
            this.labelCustomer.Size = new System.Drawing.Size(192, 18);
            this.labelCustomer.TabIndex = 7;
            this.labelCustomer.TabStop = false;
            this.labelCustomer.Text = "Customer";
            this.detailCustomer.Location = new System.Drawing.Point(14, 60);
            this.detailCustomer.Name = "detailCustomer";
            this.detailCustomer.ReadOnly = true;
            this.detailCustomer.Size = new System.Drawing.Size(192, 30);
            this.detailCustomer.TabIndex = 0;
            //
            // labelPo / detailPo
            //
            this.labelPo.AutoSize = false;
            this.labelPo.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelPo.Location = new System.Drawing.Point(14, 98);
            this.labelPo.Name = "labelPo";
            this.labelPo.Size = new System.Drawing.Size(192, 18);
            this.labelPo.TabIndex = 8;
            this.labelPo.TabStop = false;
            this.labelPo.Text = "PO #";
            this.detailPo.Location = new System.Drawing.Point(14, 116);
            this.detailPo.Name = "detailPo";
            this.detailPo.ReadOnly = true;
            this.detailPo.Size = new System.Drawing.Size(192, 30);
            this.detailPo.TabIndex = 1;
            //
            // labelLines / detailLines
            //
            this.labelLines.AutoSize = false;
            this.labelLines.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLines.Location = new System.Drawing.Point(14, 154);
            this.labelLines.Name = "labelLines";
            this.labelLines.Size = new System.Drawing.Size(192, 18);
            this.labelLines.TabIndex = 9;
            this.labelLines.TabStop = false;
            this.labelLines.Text = "Lines";
            this.detailLines.Location = new System.Drawing.Point(14, 172);
            this.detailLines.Name = "detailLines";
            this.detailLines.ReadOnly = true;
            this.detailLines.Size = new System.Drawing.Size(192, 30);
            this.detailLines.TabIndex = 2;
            //
            // newOrderButton
            //
            this.newOrderButton.Location = new System.Drawing.Point(14, 222);
            this.newOrderButton.Name = "newOrderButton";
            this.newOrderButton.Size = new System.Drawing.Size(192, 30);
            this.newOrderButton.TabIndex = 3;
            this.newOrderButton.Text = "New Order";
            this.newOrderButton.Click += new System.EventHandler(this.newOrderButton_Click);
            //
            // printInvoiceButton  (✕ PrintDocument → Module 6; kept for parity, disabled)
            //
            this.printInvoiceButton.Enabled = false;
            this.printInvoiceButton.Location = new System.Drawing.Point(14, 258);
            this.printInvoiceButton.Name = "printInvoiceButton";
            this.printInvoiceButton.Size = new System.Drawing.Size(192, 30);
            this.printInvoiceButton.TabIndex = 4;
            this.printInvoiceButton.Text = "Print Invoice";
            this.printInvoiceButton.ToolTipText = "PrintDocument prints on the SERVER — replaced by a PDF in Module 6";
            //
            // exportButton  (✕ Excel Interop → Module 6; kept for parity, disabled)
            //
            this.exportButton.Enabled = false;
            this.exportButton.Location = new System.Drawing.Point(14, 294);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(192, 30);
            this.exportButton.TabIndex = 5;
            this.exportButton.Text = "Export to Excel";
            this.exportButton.ToolTipText = "Excel Interop needs Excel on the SERVER — replaced by a managed .xlsx download in Module 6";
            //
            // OrdersPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.ordersGrid);
            this.Controls.Add(this.detailPanel);
            this.Controls.Add(this.headerLabel);
            this.Name = "OrdersPage";
            this.Size = new System.Drawing.Size(680, 436);
            this.detailPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label headerLabel;
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
    }
}
