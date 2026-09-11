namespace OrderDesk.Screens
{
    partial class ReportsScreen
    {
        // Layout: heading Dock = Top, actions Dock = Right (220), order list Dock = Fill.
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
            this.gridReportOrders = new Wisej.Web.DataGridView();
            this.colOrder = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTotal = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonReportPrint = new Wisej.Web.Button();
            this.buttonReportExport = new Wisej.Web.Button();
            this.panelActions.SuspendLayout();
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
            this.labelHeading.Text = "Reports · pick an order";
            this.labelHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // gridReportOrders
            //
            this.gridReportOrders.AllowUserToAddRows = false;
            this.gridReportOrders.AllowUserToDeleteRows = false;
            this.gridReportOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colOrder, this.colCustomer, this.colTotal, this.colStatus });
            this.gridReportOrders.Dock = Wisej.Web.DockStyle.Fill;
            this.gridReportOrders.MultiSelect = false;
            this.gridReportOrders.Name = "gridReportOrders";
            this.gridReportOrders.ReadOnly = true;
            this.gridReportOrders.RowHeadersVisible = false;
            this.gridReportOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridReportOrders.TabIndex = 0;
            this.colOrder.HeaderText = "Order"; this.colOrder.Name = "colOrder"; this.colOrder.Width = 60; this.colOrder.ReadOnly = true;
            this.colCustomer.HeaderText = "Customer"; this.colCustomer.Name = "colCustomer"; this.colCustomer.Width = 150; this.colCustomer.ReadOnly = true;
            this.colTotal.HeaderText = "Total"; this.colTotal.Name = "colTotal"; this.colTotal.Width = 90; this.colTotal.ReadOnly = true;
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colTotal.DefaultCellStyle.Format = "N2";
            this.colStatus.HeaderText = "Status"; this.colStatus.Name = "colStatus"; this.colStatus.Width = 80; this.colStatus.ReadOnly = true;
            //
            // panelActions
            //
            this.panelActions.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.panelActions.Controls.Add(this.buttonReportPrint);
            this.panelActions.Controls.Add(this.buttonReportExport);
            this.panelActions.Dock = Wisej.Web.DockStyle.Right;
            this.panelActions.Name = "panelActions";
            this.panelActions.Width = 220;
            //
            // buttonReportPrint
            //
            this.buttonReportPrint.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.buttonReportPrint.Location = new System.Drawing.Point(12, 12);
            this.buttonReportPrint.Name = "buttonReportPrint";
            this.buttonReportPrint.Size = new System.Drawing.Size(196, 30);
            this.buttonReportPrint.TabIndex = 1;
            this.buttonReportPrint.Text = "Print Invoice (PDF)";
            this.buttonReportPrint.Click += new System.EventHandler(this.buttonReportPrint_Click);
            //
            // buttonReportExport
            //
            this.buttonReportExport.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.buttonReportExport.Location = new System.Drawing.Point(12, 48);
            this.buttonReportExport.Name = "buttonReportExport";
            this.buttonReportExport.Size = new System.Drawing.Size(196, 30);
            this.buttonReportExport.TabIndex = 2;
            this.buttonReportExport.Text = "Export orders ⬇";
            this.buttonReportExport.Click += new System.EventHandler(this.buttonReportExport_Click);
            //
            // ReportsScreen
            //
            this.Controls.Add(this.gridReportOrders);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.panelActions);
            this.Name = "Reports";
            this.Size = new System.Drawing.Size(616, 300);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label labelHeading;
        private Wisej.Web.DataGridView gridReportOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colOrder;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colTotal;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonReportPrint;
        private Wisej.Web.Button buttonReportExport;
    }
}
