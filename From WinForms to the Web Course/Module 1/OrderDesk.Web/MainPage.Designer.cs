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
            this.panelOrders = new Wisej.Web.Panel();
            this.labelOrdersTitle = new Wisej.Web.Label();
            this.gridOrders = new Wisej.Web.DataGridView();
            this.colOrder = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTotal = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelOrderTitle = new Wisej.Web.Label();
            this.labelOrderDetail = new Wisej.Web.Label();
            this.buttonNewOrder = new Wisej.Web.Button();
            this.buttonPrintInvoice = new Wisej.Web.Button();
            this.buttonExport = new Wisej.Web.Button();
            this.panelSession = new Wisej.Web.Panel();
            this.labelSessionTitle = new Wisej.Web.Label();
            this.labelSessionValues = new Wisej.Web.Label();
            this.buttonSignInKelly = new Wisej.Web.Button();
            this.buttonSignInSam = new Wisej.Web.Button();
            this.buttonReread = new Wisej.Web.Button();
            this.buttonSecondSession = new Wisej.Web.Button();
            this.panelOrders.SuspendLayout();
            this.panelSession.SuspendLayout();
            this.SuspendLayout();
            //
            // panelOrders
            //
            this.panelOrders.BackColor = System.Drawing.Color.White;
            this.panelOrders.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelOrders.Controls.Add(this.labelOrdersTitle);
            this.panelOrders.Controls.Add(this.gridOrders);
            this.panelOrders.Controls.Add(this.labelOrderTitle);
            this.panelOrders.Controls.Add(this.labelOrderDetail);
            this.panelOrders.Controls.Add(this.buttonNewOrder);
            this.panelOrders.Controls.Add(this.buttonPrintInvoice);
            this.panelOrders.Controls.Add(this.buttonExport);
            this.panelOrders.Location = new System.Drawing.Point(20, 20);
            this.panelOrders.Name = "panelOrders";
            this.panelOrders.Size = new System.Drawing.Size(660, 306);
            //
            // labelOrdersTitle
            //
            this.labelOrdersTitle.AutoSize = false;
            this.labelOrdersTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelOrdersTitle.Location = new System.Drawing.Point(20, 12);
            this.labelOrdersTitle.Name = "labelOrdersTitle";
            this.labelOrdersTitle.Size = new System.Drawing.Size(300, 28);
            this.labelOrdersTitle.Text = "Orders";
            //
            // gridOrders
            //
            this.gridOrders.AllowUserToAddRows = false;
            this.gridOrders.AllowUserToDeleteRows = false;
            this.gridOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colOrder, this.colCustomer, this.colTotal, this.colStatus });
            this.gridOrders.Location = new System.Drawing.Point(20, 46);
            this.gridOrders.MultiSelect = false;
            this.gridOrders.Name = "gridOrders";
            this.gridOrders.ReadOnly = true;
            this.gridOrders.RowHeadersVisible = false;
            this.gridOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOrders.Size = new System.Drawing.Size(400, 200);
            this.gridOrders.SelectionChanged += new System.EventHandler(this.gridOrders_SelectionChanged);
            this.colOrder.HeaderText = "Order"; this.colOrder.Name = "colOrder"; this.colOrder.Width = 60; this.colOrder.ReadOnly = true;
            this.colCustomer.HeaderText = "Customer"; this.colCustomer.Name = "colCustomer"; this.colCustomer.Width = 160; this.colCustomer.ReadOnly = true;
            this.colTotal.HeaderText = "Total"; this.colTotal.Name = "colTotal"; this.colTotal.Width = 90; this.colTotal.ReadOnly = true;
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colTotal.DefaultCellStyle.Format = "N2";
            this.colStatus.HeaderText = "Status"; this.colStatus.Name = "colStatus"; this.colStatus.Width = 88; this.colStatus.ReadOnly = true;
            //
            // labelOrderTitle
            //
            this.labelOrderTitle.AutoSize = false;
            this.labelOrderTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelOrderTitle.Location = new System.Drawing.Point(436, 46);
            this.labelOrderTitle.Name = "labelOrderTitle";
            this.labelOrderTitle.Size = new System.Drawing.Size(204, 24);
            this.labelOrderTitle.Text = "Order";
            //
            // labelOrderDetail
            //
            this.labelOrderDetail.AutoSize = false;
            this.labelOrderDetail.Font = new System.Drawing.Font("monospace", 9F);
            this.labelOrderDetail.Location = new System.Drawing.Point(436, 72);
            this.labelOrderDetail.Name = "labelOrderDetail";
            this.labelOrderDetail.Size = new System.Drawing.Size(204, 100);
            this.labelOrderDetail.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // buttonNewOrder
            //
            this.buttonNewOrder.Location = new System.Drawing.Point(20, 258);
            this.buttonNewOrder.Name = "buttonNewOrder";
            this.buttonNewOrder.Size = new System.Drawing.Size(110, 30);
            this.buttonNewOrder.Text = "New Order";
            this.buttonNewOrder.Click += new System.EventHandler(this.buttonNewOrder_Click);
            //
            // buttonPrintInvoice
            //
            this.buttonPrintInvoice.Location = new System.Drawing.Point(138, 258);
            this.buttonPrintInvoice.Name = "buttonPrintInvoice";
            this.buttonPrintInvoice.Size = new System.Drawing.Size(150, 30);
            this.buttonPrintInvoice.Text = "Print Invoice (PDF)";
            this.buttonPrintInvoice.Click += new System.EventHandler(this.buttonPrintInvoice_Click);
            //
            // buttonExport
            //
            this.buttonExport.Location = new System.Drawing.Point(296, 258);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(140, 30);
            this.buttonExport.Text = "Export (Download)";
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            //
            // panelSession
            //
            this.panelSession.BackColor = System.Drawing.Color.White;
            this.panelSession.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSession.Controls.Add(this.labelSessionTitle);
            this.panelSession.Controls.Add(this.labelSessionValues);
            this.panelSession.Controls.Add(this.buttonSignInKelly);
            this.panelSession.Controls.Add(this.buttonSignInSam);
            this.panelSession.Controls.Add(this.buttonReread);
            this.panelSession.Controls.Add(this.buttonSecondSession);
            this.panelSession.Location = new System.Drawing.Point(20, 340);
            this.panelSession.Name = "panelSession";
            this.panelSession.Size = new System.Drawing.Size(660, 156);
            //
            // labelSessionTitle
            //
            this.labelSessionTitle.AutoSize = false;
            this.labelSessionTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelSessionTitle.Location = new System.Drawing.Point(20, 12);
            this.labelSessionTitle.Name = "labelSessionTitle";
            this.labelSessionTitle.Size = new System.Drawing.Size(300, 28);
            this.labelSessionTitle.Text = "Session";
            //
            // labelSessionValues
            //
            this.labelSessionValues.AutoSize = false;
            this.labelSessionValues.Font = new System.Drawing.Font("monospace", 9F);
            this.labelSessionValues.Location = new System.Drawing.Point(20, 44);
            this.labelSessionValues.Name = "labelSessionValues";
            this.labelSessionValues.Size = new System.Drawing.Size(620, 56);
            this.labelSessionValues.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // buttonSignInKelly
            //
            this.buttonSignInKelly.Location = new System.Drawing.Point(20, 108);
            this.buttonSignInKelly.Name = "buttonSignInKelly";
            this.buttonSignInKelly.Size = new System.Drawing.Size(120, 30);
            this.buttonSignInKelly.Text = "Sign in as kelly";
            this.buttonSignInKelly.Click += new System.EventHandler(this.buttonSignInKelly_Click);
            //
            // buttonSignInSam
            //
            this.buttonSignInSam.Location = new System.Drawing.Point(148, 108);
            this.buttonSignInSam.Name = "buttonSignInSam";
            this.buttonSignInSam.Size = new System.Drawing.Size(120, 30);
            this.buttonSignInSam.Text = "Sign in as sam";
            this.buttonSignInSam.Click += new System.EventHandler(this.buttonSignInSam_Click);
            //
            // buttonReread
            //
            this.buttonReread.Location = new System.Drawing.Point(276, 108);
            this.buttonReread.Name = "buttonReread";
            this.buttonReread.Size = new System.Drawing.Size(110, 30);
            this.buttonReread.Text = "Re-read state";
            this.buttonReread.Click += new System.EventHandler(this.buttonReread_Click);
            //
            // buttonSecondSession
            //
            this.buttonSecondSession.Location = new System.Drawing.Point(394, 108);
            this.buttonSecondSession.Name = "buttonSecondSession";
            this.buttonSecondSession.Size = new System.Drawing.Size(160, 30);
            this.buttonSecondSession.Text = "Open second session ↗";
            this.buttonSecondSession.Click += new System.EventHandler(this.buttonSecondSession_Click);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelOrders);
            this.Controls.Add(this.panelSession);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(700, 516);
            this.Text = "OrderDesk — Orders";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelOrders.ResumeLayout(false);
            this.panelSession.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelOrders;
        private Wisej.Web.Label labelOrdersTitle;
        private Wisej.Web.DataGridView gridOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colOrder;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colTotal;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.Label labelOrderTitle;
        private Wisej.Web.Label labelOrderDetail;
        private Wisej.Web.Button buttonNewOrder;
        private Wisej.Web.Button buttonPrintInvoice;
        private Wisej.Web.Button buttonExport;
        private Wisej.Web.Panel panelSession;
        private Wisej.Web.Label labelSessionTitle;
        private Wisej.Web.Label labelSessionValues;
        private Wisej.Web.Button buttonSignInKelly;
        private Wisej.Web.Button buttonSignInSam;
        private Wisej.Web.Button buttonReread;
        private Wisej.Web.Button buttonSecondSession;
    }
}
