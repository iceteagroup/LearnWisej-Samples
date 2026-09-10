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
            this.panelWorkbook = new Wisej.Web.Panel();
            this.labelWorkbookTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.buttonFilterAll = new Wisej.Web.Button();
            this.buttonFilterDirect = new Wisej.Web.Button();
            this.buttonFilterAdapt = new Wisej.Web.Button();
            this.buttonFilterRedesign = new Wisej.Web.Button();
            this.buttonFilterDefer = new Wisej.Web.Button();
            this.labelWorkbookCount = new Wisej.Web.Label();
            this.gridWorkbook = new Wisej.Web.DataGridView();
            this.colFeature = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDependency = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colRisk = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colVerdict = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colEffort = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colSlice = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelWorkbookDetail = new Wisej.Web.Label();
            this.panelSlice = new Wisej.Web.Panel();
            this.labelSliceTitle = new Wisej.Web.Label();
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
            this.buttonAttach = new Wisej.Web.Button();
            this.labelBanner = new Wisej.Web.Label();
            this.trace = new OrderDesk.Views.TracePanel();
            this.panelSession = new Wisej.Web.Panel();
            this.labelSessionTitle = new Wisej.Web.Label();
            this.labelSessionValues = new Wisej.Web.Label();
            this.buttonSignInKelly = new Wisej.Web.Button();
            this.buttonSignInSam = new Wisej.Web.Button();
            this.buttonReread = new Wisej.Web.Button();
            this.buttonSecondSession = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.panelWorkbook.SuspendLayout();
            this.panelSlice.SuspendLayout();
            this.panelSession.SuspendLayout();
            this.SuspendLayout();
            //
            // panelWorkbook  (deliverables 1 + 2: the assessment workbook, classified)
            //
            this.panelWorkbook.BackColor = System.Drawing.Color.White;
            this.panelWorkbook.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelWorkbook.Controls.Add(this.labelWorkbookTitle);
            this.panelWorkbook.Controls.Add(this.labelStatus);
            this.panelWorkbook.Controls.Add(this.buttonFilterAll);
            this.panelWorkbook.Controls.Add(this.buttonFilterDirect);
            this.panelWorkbook.Controls.Add(this.buttonFilterAdapt);
            this.panelWorkbook.Controls.Add(this.buttonFilterRedesign);
            this.panelWorkbook.Controls.Add(this.buttonFilterDefer);
            this.panelWorkbook.Controls.Add(this.labelWorkbookCount);
            this.panelWorkbook.Controls.Add(this.gridWorkbook);
            this.panelWorkbook.Controls.Add(this.labelWorkbookDetail);
            this.panelWorkbook.Location = new System.Drawing.Point(30, 30);
            this.panelWorkbook.Name = "panelWorkbook";
            this.panelWorkbook.Size = new System.Drawing.Size(640, 318);
            //
            // labelWorkbookTitle
            //
            this.labelWorkbookTitle.AutoSize = false;
            this.labelWorkbookTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelWorkbookTitle.Location = new System.Drawing.Point(20, 14);
            this.labelWorkbookTitle.Name = "labelWorkbookTitle";
            this.labelWorkbookTitle.Size = new System.Drawing.Size(400, 30);
            this.labelWorkbookTitle.Text = "Assessment workbook · LegacyOrderDesk";
            //
            // labelStatus
            //
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(420, 18);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(202, 24);
            this.labelStatus.Text = "● loading";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // filter buttons: the four verdicts
            //
            this.buttonFilterAll.Location = new System.Drawing.Point(20, 50);
            this.buttonFilterAll.Name = "buttonFilterAll";
            this.buttonFilterAll.Size = new System.Drawing.Size(60, 28);
            this.buttonFilterAll.Text = "All";
            this.buttonFilterAll.Click += new System.EventHandler(this.buttonFilterAll_Click);
            this.buttonFilterDirect.Location = new System.Drawing.Point(86, 50);
            this.buttonFilterDirect.Name = "buttonFilterDirect";
            this.buttonFilterDirect.Size = new System.Drawing.Size(100, 28);
            this.buttonFilterDirect.Text = "Direct-port";
            this.buttonFilterDirect.Click += new System.EventHandler(this.buttonFilterDirect_Click);
            this.buttonFilterAdapt.Location = new System.Drawing.Point(192, 50);
            this.buttonFilterAdapt.Name = "buttonFilterAdapt";
            this.buttonFilterAdapt.Size = new System.Drawing.Size(80, 28);
            this.buttonFilterAdapt.Text = "Adapt";
            this.buttonFilterAdapt.Click += new System.EventHandler(this.buttonFilterAdapt_Click);
            this.buttonFilterRedesign.Location = new System.Drawing.Point(278, 50);
            this.buttonFilterRedesign.Name = "buttonFilterRedesign";
            this.buttonFilterRedesign.Size = new System.Drawing.Size(90, 28);
            this.buttonFilterRedesign.Text = "Redesign";
            this.buttonFilterRedesign.Click += new System.EventHandler(this.buttonFilterRedesign_Click);
            this.buttonFilterDefer.Location = new System.Drawing.Point(374, 50);
            this.buttonFilterDefer.Name = "buttonFilterDefer";
            this.buttonFilterDefer.Size = new System.Drawing.Size(70, 28);
            this.buttonFilterDefer.Text = "Defer";
            this.buttonFilterDefer.Click += new System.EventHandler(this.buttonFilterDefer_Click);
            //
            // labelWorkbookCount
            //
            this.labelWorkbookCount.AutoSize = false;
            this.labelWorkbookCount.Font = new System.Drawing.Font("default", 9F);
            this.labelWorkbookCount.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelWorkbookCount.Location = new System.Drawing.Point(450, 52);
            this.labelWorkbookCount.Name = "labelWorkbookCount";
            this.labelWorkbookCount.Size = new System.Drawing.Size(172, 24);
            this.labelWorkbookCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridWorkbook
            //
            this.gridWorkbook.AllowUserToAddRows = false;
            this.gridWorkbook.AllowUserToDeleteRows = false;
            this.gridWorkbook.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colFeature, this.colDependency, this.colRisk, this.colVerdict, this.colEffort, this.colSlice });
            this.gridWorkbook.Location = new System.Drawing.Point(20, 86);
            this.gridWorkbook.MultiSelect = false;
            this.gridWorkbook.Name = "gridWorkbook";
            this.gridWorkbook.ReadOnly = true;
            this.gridWorkbook.RowHeadersVisible = false;
            this.gridWorkbook.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridWorkbook.Size = new System.Drawing.Size(602, 160);
            this.gridWorkbook.SelectionChanged += new System.EventHandler(this.gridWorkbook_SelectionChanged);
            this.colFeature.HeaderText = "Form / feature"; this.colFeature.Name = "colFeature"; this.colFeature.Width = 190; this.colFeature.ReadOnly = true;
            this.colDependency.HeaderText = "Dependency"; this.colDependency.Name = "colDependency"; this.colDependency.Width = 160; this.colDependency.ReadOnly = true;
            this.colRisk.HeaderText = "Risk tag"; this.colRisk.Name = "colRisk"; this.colRisk.Width = 88; this.colRisk.ReadOnly = true;
            this.colVerdict.HeaderText = "Verdict"; this.colVerdict.Name = "colVerdict"; this.colVerdict.Width = 100; this.colVerdict.ReadOnly = true;
            this.colEffort.HeaderText = "Effort"; this.colEffort.Name = "colEffort"; this.colEffort.Width = 30; this.colEffort.ReadOnly = true;
            this.colSlice.HeaderText = "Slice"; this.colSlice.Name = "colSlice"; this.colSlice.Width = 32; this.colSlice.ReadOnly = true;
            //
            // labelWorkbookDetail  (why + replacement of the selected row)
            //
            this.labelWorkbookDetail.AutoSize = false;
            this.labelWorkbookDetail.Font = new System.Drawing.Font("monospace", 9F);
            this.labelWorkbookDetail.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelWorkbookDetail.Location = new System.Drawing.Point(20, 250);
            this.labelWorkbookDetail.Name = "labelWorkbookDetail";
            this.labelWorkbookDetail.Size = new System.Drawing.Size(602, 60);
            this.labelWorkbookDetail.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelSlice  (deliverable 3: the first vertical slice, running)
            //
            this.panelSlice.BackColor = System.Drawing.Color.White;
            this.panelSlice.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSlice.Controls.Add(this.labelSliceTitle);
            this.panelSlice.Controls.Add(this.gridOrders);
            this.panelSlice.Controls.Add(this.labelOrderTitle);
            this.panelSlice.Controls.Add(this.labelOrderDetail);
            this.panelSlice.Controls.Add(this.buttonNewOrder);
            this.panelSlice.Controls.Add(this.buttonPrintInvoice);
            this.panelSlice.Controls.Add(this.buttonExport);
            this.panelSlice.Controls.Add(this.buttonAttach);
            this.panelSlice.Controls.Add(this.labelBanner);
            this.panelSlice.Location = new System.Drawing.Point(30, 362);
            this.panelSlice.Name = "panelSlice";
            this.panelSlice.Size = new System.Drawing.Size(640, 292);
            //
            // labelSliceTitle
            //
            this.labelSliceTitle.AutoSize = false;
            this.labelSliceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelSliceTitle.Location = new System.Drawing.Point(20, 12);
            this.labelSliceTitle.Name = "labelSliceTitle";
            this.labelSliceTitle.Size = new System.Drawing.Size(602, 28);
            this.labelSliceTitle.Text = "First slice · OrderDesk — Orders (same workflow, in the browser)";
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
            this.gridOrders.Size = new System.Drawing.Size(380, 170);
            this.gridOrders.SelectionChanged += new System.EventHandler(this.gridOrders_SelectionChanged);
            this.colOrder.HeaderText = "Order"; this.colOrder.Name = "colOrder"; this.colOrder.Width = 60; this.colOrder.ReadOnly = true;
            this.colCustomer.HeaderText = "Customer"; this.colCustomer.Name = "colCustomer"; this.colCustomer.Width = 150; this.colCustomer.ReadOnly = true;
            this.colTotal.HeaderText = "Total"; this.colTotal.Name = "colTotal"; this.colTotal.Width = 90; this.colTotal.ReadOnly = true;
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colTotal.DefaultCellStyle.Format = "N2";
            this.colStatus.HeaderText = "Status"; this.colStatus.Name = "colStatus"; this.colStatus.Width = 78; this.colStatus.ReadOnly = true;
            //
            // labelOrderTitle / labelOrderDetail  (the detail panel)
            //
            this.labelOrderTitle.AutoSize = false;
            this.labelOrderTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelOrderTitle.Location = new System.Drawing.Point(416, 46);
            this.labelOrderTitle.Name = "labelOrderTitle";
            this.labelOrderTitle.Size = new System.Drawing.Size(206, 24);
            this.labelOrderTitle.Text = "Order";
            this.labelOrderDetail.AutoSize = false;
            this.labelOrderDetail.Font = new System.Drawing.Font("monospace", 9F);
            this.labelOrderDetail.Location = new System.Drawing.Point(416, 72);
            this.labelOrderDetail.Name = "labelOrderDetail";
            this.labelOrderDetail.Size = new System.Drawing.Size(206, 84);
            this.labelOrderDetail.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // the four actions of the slice
            //
            this.buttonNewOrder.Location = new System.Drawing.Point(416, 160);
            this.buttonNewOrder.Name = "buttonNewOrder";
            this.buttonNewOrder.Size = new System.Drawing.Size(100, 28);
            this.buttonNewOrder.Text = "New Order";
            this.buttonNewOrder.ToolTipText = "OrderService.Save → CalculateOrderTotal: the reused business logic.";
            this.buttonNewOrder.Click += new System.EventHandler(this.buttonNewOrder_Click);
            this.buttonPrintInvoice.Location = new System.Drawing.Point(522, 160);
            this.buttonPrintInvoice.Name = "buttonPrintInvoice";
            this.buttonPrintInvoice.Size = new System.Drawing.Size(100, 28);
            this.buttonPrintInvoice.Text = "Print (PDF)";
            this.buttonPrintInvoice.ToolTipText = "PrintDocument → server-generated PDF in a PdfViewer.";
            this.buttonPrintInvoice.Click += new System.EventHandler(this.buttonPrintInvoice_Click);
            this.buttonExport.Location = new System.Drawing.Point(416, 194);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(100, 28);
            this.buttonExport.Text = "Export ⬇";
            this.buttonExport.ToolTipText = "Excel Interop + C:\\Orders → bytes in memory → Application.Download.";
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            this.buttonAttach.Location = new System.Drawing.Point(522, 194);
            this.buttonAttach.Name = "buttonAttach";
            this.buttonAttach.Size = new System.Drawing.Size(100, 28);
            this.buttonAttach.Text = "Attach file…";
            this.buttonAttach.ToolTipText = "The boundary the first slice logs but does not cross yet.";
            this.buttonAttach.Click += new System.EventHandler(this.buttonAttach_Click);
            //
            // labelBanner
            //
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(20, 226);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(602, 54);
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // trace  (the migration log)
            //
            this.trace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.trace.Location = new System.Drawing.Point(690, 30);
            this.trace.Name = "trace";
            this.trace.Size = new System.Drawing.Size(628, 440);
            //
            // panelSession  (one server, many sessions)
            //
            this.panelSession.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelSession.BackColor = System.Drawing.Color.White;
            this.panelSession.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSession.Controls.Add(this.labelSessionTitle);
            this.panelSession.Controls.Add(this.labelSessionValues);
            this.panelSession.Controls.Add(this.buttonSignInKelly);
            this.panelSession.Controls.Add(this.buttonSignInSam);
            this.panelSession.Controls.Add(this.buttonReread);
            this.panelSession.Controls.Add(this.buttonSecondSession);
            this.panelSession.Controls.Add(this.buttonClear);
            this.panelSession.Location = new System.Drawing.Point(690, 484);
            this.panelSession.Name = "panelSession";
            this.panelSession.Size = new System.Drawing.Size(628, 170);
            //
            // labelSessionTitle
            //
            this.labelSessionTitle.AutoSize = false;
            this.labelSessionTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelSessionTitle.Location = new System.Drawing.Point(20, 12);
            this.labelSessionTitle.Name = "labelSessionTitle";
            this.labelSessionTitle.Size = new System.Drawing.Size(590, 28);
            this.labelSessionTitle.Text = "Session check · one server, many sessions";
            //
            // labelSessionValues
            //
            this.labelSessionValues.AutoSize = false;
            this.labelSessionValues.Font = new System.Drawing.Font("monospace", 9F);
            this.labelSessionValues.Location = new System.Drawing.Point(20, 44);
            this.labelSessionValues.Name = "labelSessionValues";
            this.labelSessionValues.Size = new System.Drawing.Size(590, 60);
            this.labelSessionValues.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // session buttons
            //
            this.buttonSignInKelly.Location = new System.Drawing.Point(20, 116);
            this.buttonSignInKelly.Name = "buttonSignInKelly";
            this.buttonSignInKelly.Size = new System.Drawing.Size(120, 32);
            this.buttonSignInKelly.Text = "Sign in as kelly";
            this.buttonSignInKelly.Click += new System.EventHandler(this.buttonSignInKelly_Click);
            this.buttonSignInSam.Location = new System.Drawing.Point(148, 116);
            this.buttonSignInSam.Name = "buttonSignInSam";
            this.buttonSignInSam.Size = new System.Drawing.Size(120, 32);
            this.buttonSignInSam.Text = "Sign in as sam";
            this.buttonSignInSam.Click += new System.EventHandler(this.buttonSignInSam_Click);
            this.buttonReread.Location = new System.Drawing.Point(276, 116);
            this.buttonReread.Name = "buttonReread";
            this.buttonReread.Size = new System.Drawing.Size(110, 32);
            this.buttonReread.Text = "Re-read state";
            this.buttonReread.Click += new System.EventHandler(this.buttonReread_Click);
            this.buttonSecondSession.Location = new System.Drawing.Point(394, 116);
            this.buttonSecondSession.Name = "buttonSecondSession";
            this.buttonSecondSession.Size = new System.Drawing.Size(150, 32);
            this.buttonSecondSession.Text = "Open second session ↗";
            this.buttonSecondSession.Click += new System.EventHandler(this.buttonSecondSession_Click);
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(552, 116);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(58, 32);
            this.buttonClear.Text = "Clear";
            this.buttonClear.ToolTipText = "Clear the trace.";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelWorkbook);
            this.Controls.Add(this.panelSlice);
            this.Controls.Add(this.trace);
            this.Controls.Add(this.panelSession);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 684);
            this.Text = "OrderDesk — Migration Discovery";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelWorkbook.ResumeLayout(false);
            this.panelSlice.ResumeLayout(false);
            this.panelSession.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelWorkbook;
        private Wisej.Web.Label labelWorkbookTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Button buttonFilterAll;
        private Wisej.Web.Button buttonFilterDirect;
        private Wisej.Web.Button buttonFilterAdapt;
        private Wisej.Web.Button buttonFilterRedesign;
        private Wisej.Web.Button buttonFilterDefer;
        private Wisej.Web.Label labelWorkbookCount;
        private Wisej.Web.DataGridView gridWorkbook;
        private Wisej.Web.DataGridViewTextBoxColumn colFeature;
        private Wisej.Web.DataGridViewTextBoxColumn colDependency;
        private Wisej.Web.DataGridViewTextBoxColumn colRisk;
        private Wisej.Web.DataGridViewTextBoxColumn colVerdict;
        private Wisej.Web.DataGridViewTextBoxColumn colEffort;
        private Wisej.Web.DataGridViewTextBoxColumn colSlice;
        private Wisej.Web.Label labelWorkbookDetail;
        private Wisej.Web.Panel panelSlice;
        private Wisej.Web.Label labelSliceTitle;
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
        private Wisej.Web.Button buttonAttach;
        private Wisej.Web.Label labelBanner;
        private OrderDesk.Views.TracePanel trace;
        private Wisej.Web.Panel panelSession;
        private Wisej.Web.Label labelSessionTitle;
        private Wisej.Web.Label labelSessionValues;
        private Wisej.Web.Button buttonSignInKelly;
        private Wisej.Web.Button buttonSignInSam;
        private Wisej.Web.Button buttonReread;
        private Wisej.Web.Button buttonSecondSession;
        private Wisej.Web.Button buttonClear;
    }
}
