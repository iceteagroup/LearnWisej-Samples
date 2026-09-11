namespace OrderDesk
{
    partial class MainPage
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
            this.panelGrid = new Wisej.Web.Panel();
            this.labelGridTitle = new Wisej.Web.Label();
            this.textSearch = new Wisej.Web.TextBox();
            this.comboStatus = new Wisej.Web.ComboBox();
            this.comboSort = new Wisej.Web.ComboBox();
            this.buttonApply = new Wisej.Web.Button();
            this.gridOrders = new Wisej.Web.DataGridView();
            this.colOrder = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTotal = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDate = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelFooter = new Wisej.Web.Label();
            this.buttonNewOrder = new Wisej.Web.Button();
            this.buttonEdit = new Wisej.Web.Button();
            this.panelPerf = new Wisej.Web.Panel();
            this.labelPerfTitle = new Wisej.Web.Label();
            this.labelPerfCounts = new Wisej.Web.Label();
            this.panelGrid.SuspendLayout();
            this.panelPerf.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGrid
            //
            this.panelGrid.BackColor = System.Drawing.Color.White;
            this.panelGrid.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGrid.Controls.Add(this.labelGridTitle);
            this.panelGrid.Controls.Add(this.textSearch);
            this.panelGrid.Controls.Add(this.comboStatus);
            this.panelGrid.Controls.Add(this.comboSort);
            this.panelGrid.Controls.Add(this.buttonApply);
            this.panelGrid.Controls.Add(this.gridOrders);
            this.panelGrid.Controls.Add(this.labelFooter);
            this.panelGrid.Controls.Add(this.buttonNewOrder);
            this.panelGrid.Controls.Add(this.buttonEdit);
            this.panelGrid.Location = new System.Drawing.Point(20, 20);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Size = new System.Drawing.Size(700, 584);
            //
            // labelGridTitle
            //
            this.labelGridTitle.AutoSize = false;
            this.labelGridTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelGridTitle.Location = new System.Drawing.Point(20, 12);
            this.labelGridTitle.Name = "labelGridTitle";
            this.labelGridTitle.Size = new System.Drawing.Size(300, 28);
            this.labelGridTitle.Text = "Orders";
            //
            // textSearch
            //
            this.textSearch.Location = new System.Drawing.Point(20, 48);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(220, 28);
            this.textSearch.Watermark = "Search customer, order, PO…";
            //
            // comboStatus
            //
            this.comboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboStatus.Items.AddRange(new object[] { "All", "Open", "InProgress", "Shipped", "Invoiced", "Hold" });
            this.comboStatus.Location = new System.Drawing.Point(246, 48);
            this.comboStatus.Name = "comboStatus";
            this.comboStatus.Size = new System.Drawing.Size(110, 28);
            //
            // comboSort
            //
            this.comboSort.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboSort.Items.AddRange(new object[] { "Date ↓ (newest)", "Total ↓", "Customer ↑", "Status ↑", "Order # ↓" });
            this.comboSort.Location = new System.Drawing.Point(362, 48);
            this.comboSort.Name = "comboSort";
            this.comboSort.Size = new System.Drawing.Size(150, 28);
            //
            // buttonApply
            //
            this.buttonApply.Location = new System.Drawing.Point(518, 48);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(90, 28);
            this.buttonApply.Text = "Apply";
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            //
            // gridOrders  (VirtualMode: RowCount from a count, cells from CellValueNeeded)
            //
            this.gridOrders.AllowUserToAddRows = false;
            this.gridOrders.AllowUserToDeleteRows = false;
            this.gridOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colOrder, this.colCustomer, this.colTotal, this.colStatus, this.colDate });
            this.gridOrders.Location = new System.Drawing.Point(20, 86);
            this.gridOrders.MultiSelect = false;
            this.gridOrders.Name = "gridOrders";
            this.gridOrders.ReadOnly = true;
            this.gridOrders.RowHeadersVisible = false;
            this.gridOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOrders.Size = new System.Drawing.Size(660, 420);
            this.gridOrders.VirtualMode = true;
            this.gridOrders.CellValueNeeded += new Wisej.Web.DataGridViewCellValueEventHandler(this.gridOrders_CellValueNeeded);
            this.gridOrders.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.gridOrders_CellDoubleClick);
            this.colOrder.HeaderText = "Order"; this.colOrder.Name = "colOrder"; this.colOrder.Width = 70; this.colOrder.ReadOnly = true; this.colOrder.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            this.colCustomer.HeaderText = "Customer"; this.colCustomer.Name = "colCustomer"; this.colCustomer.Width = 240; this.colCustomer.ReadOnly = true; this.colCustomer.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            this.colTotal.HeaderText = "Total"; this.colTotal.Name = "colTotal"; this.colTotal.Width = 110; this.colTotal.ReadOnly = true; this.colTotal.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            this.colTotal.DefaultCellStyle.Format = "N2";
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colStatus.HeaderText = "Status"; this.colStatus.Name = "colStatus"; this.colStatus.Width = 100; this.colStatus.ReadOnly = true; this.colStatus.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            this.colDate.HeaderText = "Date"; this.colDate.Name = "colDate"; this.colDate.Width = 110; this.colDate.ReadOnly = true; this.colDate.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // labelFooter  (the Σ row: count + total of everything the filter matches)
            //
            this.labelFooter.AutoSize = false;
            this.labelFooter.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelFooter.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.labelFooter.Location = new System.Drawing.Point(20, 510);
            this.labelFooter.Name = "labelFooter";
            this.labelFooter.Size = new System.Drawing.Size(660, 24);
            this.labelFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonNewOrder
            //
            this.buttonNewOrder.Location = new System.Drawing.Point(20, 540);
            this.buttonNewOrder.Name = "buttonNewOrder";
            this.buttonNewOrder.Size = new System.Drawing.Size(110, 30);
            this.buttonNewOrder.Text = "New order";
            this.buttonNewOrder.Click += new System.EventHandler(this.buttonNewOrder_Click);
            //
            // buttonEdit
            //
            this.buttonEdit.Location = new System.Drawing.Point(136, 540);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(110, 30);
            this.buttonEdit.Text = "Edit…";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            //
            // panelPerf
            //
            this.panelPerf.BackColor = System.Drawing.Color.White;
            this.panelPerf.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelPerf.Controls.Add(this.labelPerfTitle);
            this.panelPerf.Controls.Add(this.labelPerfCounts);
            this.panelPerf.Location = new System.Drawing.Point(740, 20);
            this.panelPerf.Name = "panelPerf";
            this.panelPerf.Size = new System.Drawing.Size(240, 190);
            //
            // labelPerfTitle
            //
            this.labelPerfTitle.AutoSize = false;
            this.labelPerfTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelPerfTitle.Location = new System.Drawing.Point(16, 12);
            this.labelPerfTitle.Name = "labelPerfTitle";
            this.labelPerfTitle.Size = new System.Drawing.Size(208, 28);
            this.labelPerfTitle.Text = "Performance";
            //
            // labelPerfCounts
            //
            this.labelPerfCounts.AutoSize = false;
            this.labelPerfCounts.Font = new System.Drawing.Font("monospace", 9F);
            this.labelPerfCounts.Location = new System.Drawing.Point(16, 44);
            this.labelPerfCounts.Name = "labelPerfCounts";
            this.labelPerfCounts.Size = new System.Drawing.Size(208, 130);
            this.labelPerfCounts.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelPerf);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1000, 624);
            this.Text = "OrderDesk — Orders";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelGrid.ResumeLayout(false);
            this.panelPerf.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGrid;
        private Wisej.Web.Label labelGridTitle;
        private Wisej.Web.TextBox textSearch;
        private Wisej.Web.ComboBox comboStatus;
        private Wisej.Web.ComboBox comboSort;
        private Wisej.Web.Button buttonApply;
        private Wisej.Web.DataGridView gridOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colOrder;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colTotal;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colDate;
        private Wisej.Web.Label labelFooter;
        private Wisej.Web.Button buttonNewOrder;
        private Wisej.Web.Button buttonEdit;
        private Wisej.Web.Panel panelPerf;
        private Wisej.Web.Label labelPerfTitle;
        private Wisej.Web.Label labelPerfCounts;
    }
}
