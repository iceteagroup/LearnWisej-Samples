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
            this.labelStatus = new Wisej.Web.Label();
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
            this.labelSelected = new Wisej.Web.Label();
            this.numNaiveRows = new Wisej.Web.NumericUpDown();
            this.buttonNaive = new Wisej.Web.Button();
            this.buttonOptimized = new Wisej.Web.Button();
            this.buttonEdit = new Wisej.Web.Button();
            this.panelValidation = new Wisej.Web.Panel();
            this.labelValidationTitle = new Wisej.Web.Label();
            this.buttonNewOrder = new Wisej.Web.Button();
            this.buttonBadOrder = new Wisej.Web.Button();
            this.buttonDesktopRule = new Wisej.Web.Button();
            this.buttonBatch = new Wisej.Web.Button();
            this.labelValidationDetail = new Wisej.Web.Label();
            this.labelBanner = new Wisej.Web.Label();
            this.trace = new OrderDesk.Views.TracePanel();
            this.panelPerf = new Wisej.Web.Panel();
            this.labelPerfTitle = new Wisej.Web.Label();
            this.labelPerfCounts = new Wisej.Web.Label();
            this.gridPerf = new Wisej.Web.DataGridView();
            this.colScenario = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colRows = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colServerMs = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPayload = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colNote = new Wisej.Web.DataGridViewTextBoxColumn();
            this.buttonMeasure = new Wisej.Web.Button();
            this.buttonSecondSession = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.labelPerfNote = new Wisej.Web.Label();
            this.timerBatch = new Wisej.Web.Timer(this.components);
            this.timerMeasure = new Wisej.Web.Timer(this.components);
            this.panelGrid.SuspendLayout();
            this.panelValidation.SuspendLayout();
            this.panelPerf.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGrid  (card A: the ported orders grid over 200,000 orders)
            //
            this.panelGrid.BackColor = System.Drawing.Color.White;
            this.panelGrid.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGrid.Controls.Add(this.labelGridTitle);
            this.panelGrid.Controls.Add(this.labelStatus);
            this.panelGrid.Controls.Add(this.textSearch);
            this.panelGrid.Controls.Add(this.comboStatus);
            this.panelGrid.Controls.Add(this.comboSort);
            this.panelGrid.Controls.Add(this.buttonApply);
            this.panelGrid.Controls.Add(this.gridOrders);
            this.panelGrid.Controls.Add(this.labelFooter);
            this.panelGrid.Controls.Add(this.labelSelected);
            this.panelGrid.Controls.Add(this.numNaiveRows);
            this.panelGrid.Controls.Add(this.buttonNaive);
            this.panelGrid.Controls.Add(this.buttonOptimized);
            this.panelGrid.Controls.Add(this.buttonEdit);
            this.panelGrid.Location = new System.Drawing.Point(30, 30);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Size = new System.Drawing.Size(640, 384);
            //
            // labelGridTitle / labelStatus
            //
            this.labelGridTitle.AutoSize = false;
            this.labelGridTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelGridTitle.Location = new System.Drawing.Point(20, 14);
            this.labelGridTitle.Name = "labelGridTitle";
            this.labelGridTitle.Size = new System.Drawing.Size(400, 30);
            this.labelGridTitle.Text = "Orders · 200,000 rows · OrderDesk.Web";
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(400, 18);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(222, 24);
            this.labelStatus.Text = "● loading";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // toolbar: search · status filter · sort · apply  (the filter/sort model stays on the server)
            //
            this.textSearch.Location = new System.Drawing.Point(20, 50);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(200, 28);
            this.textSearch.Watermark = "Search customer, order, PO…";
            this.textSearch.ToolTipText = "OrderQuery.Text — matched on the server against customer, id and PO number.";
            this.comboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboStatus.Items.AddRange(new object[] { "All", "Open", "InProgress", "Shipped", "Invoiced", "Hold" });
            this.comboStatus.Location = new System.Drawing.Point(226, 50);
            this.comboStatus.Name = "comboStatus";
            this.comboStatus.Size = new System.Drawing.Size(110, 28);
            this.comboStatus.ToolTipText = "The default filter: Open — the rows a clerk actually works in.";
            this.comboSort.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboSort.Items.AddRange(new object[] { "Date ↓ (newest)", "Total ↓", "Customer ↑", "Status ↑", "Order # ↓" });
            this.comboSort.Location = new System.Drawing.Point(342, 50);
            this.comboSort.Name = "comboSort";
            this.comboSort.Size = new System.Drawing.Size(150, 28);
            this.comboSort.ToolTipText = "Sorted in the query, not in the browser.";
            this.buttonApply.Location = new System.Drawing.Point(498, 50);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(124, 28);
            this.buttonApply.Text = "Apply filter";
            this.buttonApply.ToolTipText = "Re-count on the server and re-fetch blocks on demand.";
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            //
            // gridOrders  (VirtualMode: RowCount from a count, cells from CellValueNeeded, 50 rows per block)
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
            this.gridOrders.Size = new System.Drawing.Size(602, 200);
            this.gridOrders.CellValueNeeded += new Wisej.Web.DataGridViewCellValueEventHandler(this.gridOrders_CellValueNeeded);
            this.gridOrders.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.gridOrders_CellDoubleClick);
            this.gridOrders.SelectionChanged += new System.EventHandler(this.gridOrders_SelectionChanged);
            this.colOrder.HeaderText = "Order"; this.colOrder.Name = "colOrder"; this.colOrder.Width = 70; this.colOrder.ReadOnly = true; this.colOrder.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            this.colCustomer.HeaderText = "Customer"; this.colCustomer.Name = "colCustomer"; this.colCustomer.Width = 200; this.colCustomer.ReadOnly = true; this.colCustomer.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            this.colTotal.HeaderText = "Total"; this.colTotal.Name = "colTotal"; this.colTotal.Width = 100; this.colTotal.ReadOnly = true; this.colTotal.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            this.colTotal.DefaultCellStyle.Format = "N2";
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colStatus.HeaderText = "Status"; this.colStatus.Name = "colStatus"; this.colStatus.Width = 90; this.colStatus.ReadOnly = true; this.colStatus.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            this.colDate.HeaderText = "Date"; this.colDate.Name = "colDate"; this.colDate.Width = 100; this.colDate.ReadOnly = true; this.colDate.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // labelFooter  (the Σ row: count + total computed where the data is)
            //
            this.labelFooter.AutoSize = false;
            this.labelFooter.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelFooter.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.labelFooter.Location = new System.Drawing.Point(20, 290);
            this.labelFooter.Name = "labelFooter";
            this.labelFooter.Size = new System.Drawing.Size(602, 22);
            this.labelFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelSelected  (the current row)
            //
            this.labelSelected.AutoSize = false;
            this.labelSelected.Font = new System.Drawing.Font("monospace", 9F);
            this.labelSelected.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelSelected.Location = new System.Drawing.Point(20, 312);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(602, 22);
            this.labelSelected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // button row: naive vs optimized, edit
            //
            this.numNaiveRows.Location = new System.Drawing.Point(20, 342);
            this.numNaiveRows.Maximum = new decimal(new int[] { 200000, 0, 0, 0 });
            this.numNaiveRows.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numNaiveRows.Increment = new decimal(new int[] { 5000, 0, 0, 0 });
            this.numNaiveRows.Name = "numNaiveRows";
            this.numNaiveRows.Size = new System.Drawing.Size(96, 28);
            this.numNaiveRows.ToolTipText = "How many of the 200,000 cloned rows the naive grid is allowed to hold. The desktop had no limit; 20,000 is enough to feel it.";
            this.numNaiveRows.Value = new decimal(new int[] { 20000, 0, 0, 0 });
            this.buttonNaive.Location = new System.Drawing.Point(122, 342);
            this.buttonNaive.Name = "buttonNaive";
            this.buttonNaive.Size = new System.Drawing.Size(180, 28);
            this.buttonNaive.Text = "Naive port: load all rows";
            this.buttonNaive.ToolTipText = "✕ OrdersForm.ReloadGrid: GetOrders() clones + sorts 200,000 rows and binds them (failure path).";
            this.buttonNaive.Click += new System.EventHandler(this.buttonNaive_Click);
            this.buttonOptimized.Location = new System.Drawing.Point(308, 342);
            this.buttonOptimized.Name = "buttonOptimized";
            this.buttonOptimized.Size = new System.Drawing.Size(210, 28);
            this.buttonOptimized.Text = "Optimized: filter + virtual rows";
            this.buttonOptimized.ToolTipText = "✓ Count + Σ on the server, RowCount to the browser, 50-row blocks on demand (recovery).";
            this.buttonOptimized.Click += new System.EventHandler(this.buttonOptimized_Click);
            this.buttonEdit.Location = new System.Drawing.Point(524, 342);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(98, 28);
            this.buttonEdit.Text = "Edit selected…";
            this.buttonEdit.ToolTipText = "The ported edit dialog (double-click a row works too).";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            //
            // panelValidation  (card B: the rule out of the form)
            //
            this.panelValidation.BackColor = System.Drawing.Color.White;
            this.panelValidation.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelValidation.Controls.Add(this.labelValidationTitle);
            this.panelValidation.Controls.Add(this.buttonNewOrder);
            this.panelValidation.Controls.Add(this.buttonBadOrder);
            this.panelValidation.Controls.Add(this.buttonDesktopRule);
            this.panelValidation.Controls.Add(this.buttonBatch);
            this.panelValidation.Controls.Add(this.labelValidationDetail);
            this.panelValidation.Location = new System.Drawing.Point(30, 428);
            this.panelValidation.Name = "panelValidation";
            this.panelValidation.Size = new System.Drawing.Size(640, 150);
            //
            // labelValidationTitle
            //
            this.labelValidationTitle.AutoSize = false;
            this.labelValidationTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelValidationTitle.Location = new System.Drawing.Point(20, 12);
            this.labelValidationTitle.Name = "labelValidationTitle";
            this.labelValidationTitle.Size = new System.Drawing.Size(602, 28);
            this.labelValidationTitle.Text = "Validation · OrderValidator runs on the server  (was MessageBox.Show(\"Select a customer.\") inside the form)";
            //
            // validation buttons
            //
            this.buttonNewOrder.Location = new System.Drawing.Point(20, 46);
            this.buttonNewOrder.Name = "buttonNewOrder";
            this.buttonNewOrder.Size = new System.Drawing.Size(140, 28);
            this.buttonNewOrder.Text = "New order (dialog)";
            this.buttonNewOrder.ToolTipText = "Empty order → press Save → ErrorProvider messages next to each field (success path of the edit workflow).";
            this.buttonNewOrder.Click += new System.EventHandler(this.buttonNewOrder_Click);
            this.buttonBadOrder.Location = new System.Drawing.Point(166, 46);
            this.buttonBadOrder.Name = "buttonBadOrder";
            this.buttonBadOrder.Size = new System.Drawing.Size(196, 28);
            this.buttonBadOrder.Text = "Validate a bad order (no form)";
            this.buttonBadOrder.ToolTipText = "OrderValidator.Validate called directly — what a batch import or a web API does.";
            this.buttonBadOrder.Click += new System.EventHandler(this.buttonBadOrder_Click);
            this.buttonDesktopRule.Location = new System.Drawing.Point(368, 46);
            this.buttonDesktopRule.Name = "buttonDesktopRule";
            this.buttonDesktopRule.Size = new System.Drawing.Size(160, 28);
            this.buttonDesktopRule.Text = "Desktop rule → MessageBox";
            this.buttonDesktopRule.ToolTipText = "✕ The one rule LegacyOrderDesk had, blocking (failure path).";
            this.buttonDesktopRule.Click += new System.EventHandler(this.buttonDesktopRule_Click);
            this.buttonBatch.Location = new System.Drawing.Point(534, 46);
            this.buttonBatch.Name = "buttonBatch";
            this.buttonBatch.Size = new System.Drawing.Size(88, 28);
            this.buttonBatch.Text = "Batch 2,000";
            this.buttonBatch.ToolTipText = "The same rule over 2,000 stored orders, 200 per Timer tick (progress path).";
            this.buttonBatch.Click += new System.EventHandler(this.buttonBatch_Click);
            //
            // labelValidationDetail
            //
            this.labelValidationDetail.AutoSize = false;
            this.labelValidationDetail.Font = new System.Drawing.Font("monospace", 9F);
            this.labelValidationDetail.Location = new System.Drawing.Point(20, 82);
            this.labelValidationDetail.Name = "labelValidationDetail";
            this.labelValidationDetail.Size = new System.Drawing.Size(602, 62);
            this.labelValidationDetail.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelBanner  (one paragraph that explains the last failure or recovery)
            //
            this.labelBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(30, 592);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(640, 62);
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // trace  (the migration log)
            //
            this.trace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.trace.Location = new System.Drawing.Point(690, 30);
            this.trace.Name = "trace";
            this.trace.Size = new System.Drawing.Size(628, 300);
            //
            // panelPerf  (card C: what crossed the wire)
            //
            this.panelPerf.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelPerf.BackColor = System.Drawing.Color.White;
            this.panelPerf.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelPerf.Controls.Add(this.labelPerfTitle);
            this.panelPerf.Controls.Add(this.labelPerfCounts);
            this.panelPerf.Controls.Add(this.gridPerf);
            this.panelPerf.Controls.Add(this.buttonMeasure);
            this.panelPerf.Controls.Add(this.buttonSecondSession);
            this.panelPerf.Controls.Add(this.buttonClear);
            this.panelPerf.Controls.Add(this.labelPerfNote);
            this.panelPerf.Location = new System.Drawing.Point(690, 344);
            this.panelPerf.Name = "panelPerf";
            this.panelPerf.Size = new System.Drawing.Size(628, 310);
            //
            // labelPerfTitle / labelPerfCounts
            //
            this.labelPerfTitle.AutoSize = false;
            this.labelPerfTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelPerfTitle.Location = new System.Drawing.Point(20, 12);
            this.labelPerfTitle.Name = "labelPerfTitle";
            this.labelPerfTitle.Size = new System.Drawing.Size(320, 28);
            this.labelPerfTitle.Text = "Performance · what crossed the wire";
            this.labelPerfCounts.AutoSize = false;
            this.labelPerfCounts.Font = new System.Drawing.Font("default", 9F);
            this.labelPerfCounts.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelPerfCounts.Location = new System.Drawing.Point(340, 14);
            this.labelPerfCounts.Name = "labelPerfCounts";
            this.labelPerfCounts.Size = new System.Drawing.Size(268, 24);
            this.labelPerfCounts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridPerf  (one row per fetch: scenario · rows · server ms · ≈ payload · note)
            //
            this.gridPerf.AllowUserToAddRows = false;
            this.gridPerf.AllowUserToDeleteRows = false;
            this.gridPerf.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colScenario, this.colRows, this.colServerMs, this.colPayload, this.colNote });
            this.gridPerf.Location = new System.Drawing.Point(20, 46);
            this.gridPerf.MultiSelect = false;
            this.gridPerf.Name = "gridPerf";
            this.gridPerf.ReadOnly = true;
            this.gridPerf.RowHeadersVisible = false;
            this.gridPerf.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridPerf.Size = new System.Drawing.Size(588, 160);
            this.colScenario.HeaderText = "Scenario"; this.colScenario.Name = "colScenario"; this.colScenario.Width = 196; this.colScenario.ReadOnly = true;
            this.colRows.HeaderText = "Rows"; this.colRows.Name = "colRows"; this.colRows.Width = 64; this.colRows.ReadOnly = true;
            this.colRows.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colServerMs.HeaderText = "Server ms"; this.colServerMs.Name = "colServerMs"; this.colServerMs.Width = 76; this.colServerMs.ReadOnly = true;
            this.colServerMs.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colPayload.HeaderText = "≈ Payload"; this.colPayload.Name = "colPayload"; this.colPayload.Width = 80; this.colPayload.ReadOnly = true;
            this.colPayload.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colNote.HeaderText = "Note"; this.colNote.Name = "colNote"; this.colNote.Width = 150; this.colNote.ReadOnly = true;
            //
            // perf buttons
            //
            this.buttonMeasure.Location = new System.Drawing.Point(20, 216);
            this.buttonMeasure.Name = "buttonMeasure";
            this.buttonMeasure.Size = new System.Drawing.Size(170, 28);
            this.buttonMeasure.Text = "Measure 10 interactions";
            this.buttonMeasure.ToolTipText = "Ten random viewport jumps against the current filter + sort, timed on the server (progress path).";
            this.buttonMeasure.Click += new System.EventHandler(this.buttonMeasure_Click);
            this.buttonSecondSession.Location = new System.Drawing.Point(196, 216);
            this.buttonSecondSession.Name = "buttonSecondSession";
            this.buttonSecondSession.Size = new System.Drawing.Size(150, 28);
            this.buttonSecondSession.Text = "Open second session ↗";
            this.buttonSecondSession.ToolTipText = "A second browser session: the same 200,000-row store, its own grid and blocks.";
            this.buttonSecondSession.Click += new System.EventHandler(this.buttonSecondSession_Click);
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(550, 216);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(58, 28);
            this.buttonClear.Text = "Clear";
            this.buttonClear.ToolTipText = "Clear the trace.";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // labelPerfNote
            //
            this.labelPerfNote.AutoSize = false;
            this.labelPerfNote.Font = new System.Drawing.Font("default", 9F);
            this.labelPerfNote.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelPerfNote.Location = new System.Drawing.Point(20, 252);
            this.labelPerfNote.Name = "labelPerfNote";
            this.labelPerfNote.Size = new System.Drawing.Size(588, 50);
            this.labelPerfNote.Text = "The walkthrough's numbers: naive port 200,000 rows · 8.4 s · 96 MB — filtered + virtual 50 rows · 0.2 s · 38 KB. Your machine's numbers land in the table above and in docs/GridPerformanceNotes.md. Payload is estimated from the cell text plus framing, not sniffed from the socket.";
            this.labelPerfNote.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // timers
            //
            this.timerBatch.Interval = 400;
            this.timerBatch.Tick += new System.EventHandler(this.timerBatch_Tick);
            this.timerMeasure.Interval = 500;
            this.timerMeasure.Tick += new System.EventHandler(this.timerMeasure_Tick);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelValidation);
            this.Controls.Add(this.labelBanner);
            this.Controls.Add(this.trace);
            this.Controls.Add(this.panelPerf);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 684);
            this.Text = "OrderDesk — DataGridView, Validation & Performance";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelGrid.ResumeLayout(false);
            this.panelValidation.ResumeLayout(false);
            this.panelPerf.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGrid;
        private Wisej.Web.Label labelGridTitle;
        private Wisej.Web.Label labelStatus;
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
        private Wisej.Web.Label labelSelected;
        private Wisej.Web.NumericUpDown numNaiveRows;
        private Wisej.Web.Button buttonNaive;
        private Wisej.Web.Button buttonOptimized;
        private Wisej.Web.Button buttonEdit;
        private Wisej.Web.Panel panelValidation;
        private Wisej.Web.Label labelValidationTitle;
        private Wisej.Web.Button buttonNewOrder;
        private Wisej.Web.Button buttonBadOrder;
        private Wisej.Web.Button buttonDesktopRule;
        private Wisej.Web.Button buttonBatch;
        private Wisej.Web.Label labelValidationDetail;
        private Wisej.Web.Label labelBanner;
        private OrderDesk.Views.TracePanel trace;
        private Wisej.Web.Panel panelPerf;
        private Wisej.Web.Label labelPerfTitle;
        private Wisej.Web.Label labelPerfCounts;
        private Wisej.Web.DataGridView gridPerf;
        private Wisej.Web.DataGridViewTextBoxColumn colScenario;
        private Wisej.Web.DataGridViewTextBoxColumn colRows;
        private Wisej.Web.DataGridViewTextBoxColumn colServerMs;
        private Wisej.Web.DataGridViewTextBoxColumn colPayload;
        private Wisej.Web.DataGridViewTextBoxColumn colNote;
        private Wisej.Web.Button buttonMeasure;
        private Wisej.Web.Button buttonSecondSession;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Label labelPerfNote;
        private Wisej.Web.Timer timerBatch;
        private Wisej.Web.Timer timerMeasure;
    }
}
