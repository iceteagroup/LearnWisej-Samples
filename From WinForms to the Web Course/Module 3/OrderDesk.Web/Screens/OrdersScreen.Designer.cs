namespace OrderDesk.Screens
{
    partial class OrdersScreen
    {
        // Layout: labelHeading Dock = Top · panelDetail Dock = Right (220) · gridOrders Dock = Fill.
        // Tab order: grid (0) → Edit (1) → New Order (2) → Print Invoice (3).
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
            this.labelHeading = new Wisej.Web.Label();
            this.gridOrders = new Wisej.Web.DataGridView();
            this.colOrder = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTotal = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.panelDetail = new Wisej.Web.Panel();
            this.labelDetailTitle = new Wisej.Web.Label();
            this.labelDetail = new Wisej.Web.Label();
            this.buttonScreenEdit = new Wisej.Web.Button();
            this.buttonScreenNewOrder = new Wisej.Web.Button();
            this.buttonScreenPrint = new Wisej.Web.Button();
            this.panelDetail.SuspendLayout();
            this.SuspendLayout();
            //
            // labelHeading
            //
            this.labelHeading.AutoSize = false;
            this.labelHeading.Dock = Wisej.Web.DockStyle.Top;
            this.labelHeading.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelHeading.Height = 30;
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelHeading.Text = "Orders · all";
            this.labelHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // gridOrders
            //
            this.gridOrders.AllowUserToAddRows = false;
            this.gridOrders.AllowUserToDeleteRows = false;
            this.gridOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colOrder, this.colCustomer, this.colTotal, this.colStatus });
            this.gridOrders.Dock = Wisej.Web.DockStyle.Fill;
            this.gridOrders.MultiSelect = false;
            this.gridOrders.Name = "gridOrders";
            this.gridOrders.ReadOnly = true;
            this.gridOrders.RowHeadersVisible = false;
            this.gridOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOrders.TabIndex = 0;
            this.gridOrders.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.gridOrders_CellDoubleClick);
            this.gridOrders.SelectionChanged += new System.EventHandler(this.gridOrders_SelectionChanged);
            this.colOrder.HeaderText = "Order"; this.colOrder.Name = "colOrder"; this.colOrder.Width = 60; this.colOrder.ReadOnly = true;
            this.colCustomer.HeaderText = "Customer"; this.colCustomer.Name = "colCustomer"; this.colCustomer.Width = 150; this.colCustomer.ReadOnly = true;
            this.colTotal.HeaderText = "Total"; this.colTotal.Name = "colTotal"; this.colTotal.Width = 90; this.colTotal.ReadOnly = true;
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colTotal.DefaultCellStyle.Format = "N2";
            this.colStatus.HeaderText = "Status"; this.colStatus.Name = "colStatus"; this.colStatus.Width = 80; this.colStatus.ReadOnly = true;
            //
            // panelDetail
            //
            this.panelDetail.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.panelDetail.Controls.Add(this.labelDetailTitle);
            this.panelDetail.Controls.Add(this.labelDetail);
            this.panelDetail.Controls.Add(this.buttonScreenEdit);
            this.panelDetail.Controls.Add(this.buttonScreenNewOrder);
            this.panelDetail.Controls.Add(this.buttonScreenPrint);
            this.panelDetail.Dock = Wisej.Web.DockStyle.Right;
            this.panelDetail.Name = "panelDetail";
            this.panelDetail.Padding = new Wisej.Web.Padding(12);
            this.panelDetail.Width = 220;
            //
            // labelDetailTitle
            //
            this.labelDetailTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelDetailTitle.AutoSize = false;
            this.labelDetailTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelDetailTitle.Location = new System.Drawing.Point(12, 8);
            this.labelDetailTitle.Name = "labelDetailTitle";
            this.labelDetailTitle.Size = new System.Drawing.Size(196, 24);
            this.labelDetailTitle.Text = "Order";
            //
            // labelDetail
            //
            this.labelDetail.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelDetail.AutoSize = false;
            this.labelDetail.Font = new System.Drawing.Font("monospace", 9F);
            this.labelDetail.Location = new System.Drawing.Point(12, 34);
            this.labelDetail.Name = "labelDetail";
            this.labelDetail.Size = new System.Drawing.Size(196, 84);
            this.labelDetail.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // buttonScreenEdit
            //
            this.buttonScreenEdit.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.buttonScreenEdit.Location = new System.Drawing.Point(12, 124);
            this.buttonScreenEdit.Name = "buttonScreenEdit";
            this.buttonScreenEdit.Size = new System.Drawing.Size(196, 30);
            this.buttonScreenEdit.TabIndex = 1;
            this.buttonScreenEdit.Text = "Edit…";
            this.buttonScreenEdit.Click += new System.EventHandler(this.buttonScreenEdit_Click);
            //
            // buttonScreenNewOrder
            //
            this.buttonScreenNewOrder.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.buttonScreenNewOrder.Location = new System.Drawing.Point(12, 160);
            this.buttonScreenNewOrder.Name = "buttonScreenNewOrder";
            this.buttonScreenNewOrder.Size = new System.Drawing.Size(196, 30);
            this.buttonScreenNewOrder.TabIndex = 2;
            this.buttonScreenNewOrder.Text = "New Order";
            this.buttonScreenNewOrder.Click += new System.EventHandler(this.buttonScreenNewOrder_Click);
            //
            // buttonScreenPrint
            //
            this.buttonScreenPrint.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.buttonScreenPrint.Location = new System.Drawing.Point(12, 196);
            this.buttonScreenPrint.Name = "buttonScreenPrint";
            this.buttonScreenPrint.Size = new System.Drawing.Size(196, 30);
            this.buttonScreenPrint.TabIndex = 3;
            this.buttonScreenPrint.Text = "Print Invoice";
            this.buttonScreenPrint.Click += new System.EventHandler(this.buttonScreenPrint_Click);
            //
            // OrdersScreen  (Fill first, then the docked edges)
            //
            this.Controls.Add(this.gridOrders);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.panelDetail);
            this.Name = "Orders";
            this.Size = new System.Drawing.Size(616, 300);
            this.panelDetail.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label labelHeading;
        private Wisej.Web.DataGridView gridOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colOrder;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colTotal;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.Panel panelDetail;
        private Wisej.Web.Label labelDetailTitle;
        private Wisej.Web.Label labelDetail;
        private Wisej.Web.Button buttonScreenEdit;
        private Wisej.Web.Button buttonScreenNewOrder;
        private Wisej.Web.Button buttonScreenPrint;
    }
}
