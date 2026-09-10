namespace OperationsConsole.Sections
{
    partial class DataGridViewPage
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

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ordersSource = new Wisej.Web.BindingSource(this.components);
            this.dueDateCalendar = new Wisej.Web.MonthCalendar();
            this.pnlCommandRow = new Wisej.Web.Panel();
            this.flowCommands = new Wisej.Web.FlowLayoutPanel();
            this.btnLoadOrders = new Wisej.Web.Button();
            this.btnOpenSelected = new Wisej.Web.Button();
            this.btnPushDueDate = new Wisej.Web.Button();
            this.btnPastDueDate = new Wisej.Web.Button();
            this.chkCalendarEditor = new Wisej.Web.CheckBox();
            this.btnInjectUnsafeStatus = new Wisej.Web.Button();
            this.chkSimulateFailure = new Wisej.Web.CheckBox();
            this.lblSubtitle = new Wisej.Web.Label();
            this.lblTitle = new Wisej.Web.Label();
            this.pnlGridCard = new Wisej.Web.Panel();
            this.ordersGrid = new Wisej.Web.DataGridView();
            this.colNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDueDate = new Wisej.Web.DataGridViewDateTimePickerColumn();
            this.colTotal = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOpen = new Wisej.Web.DataGridViewButtonColumn();
            this.pnlGridStatus = new Wisej.Web.Panel();
            this.flowGridStatus = new Wisej.Web.FlowLayoutPanel();
            this.lblRowCount = new Wisej.Web.Label();
            this.lblSelectedOrder = new Wisej.Web.Label();
            this.lblCacheState = new Wisej.Web.Label();
            this.lblLastRefresh = new Wisej.Web.Label();
            this.pnlFilterStrip = new Wisej.Web.Panel();
            this.txtSearch = new Wisej.Web.TextBox();
            this.pnlFilterActions = new Wisej.Web.FlowLayoutPanel();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.btnApply = new Wisej.Web.Button();
            this.btnClearFilter = new Wisej.Web.Button();
            this.chkVirtualMode = new Wisej.Web.CheckBox();
            this.pnlCommandRow.SuspendLayout();
            this.flowCommands.SuspendLayout();
            this.pnlGridCard.SuspendLayout();
            this.pnlGridStatus.SuspendLayout();
            this.flowGridStatus.SuspendLayout();
            this.pnlFilterStrip.SuspendLayout();
            this.pnlFilterActions.SuspendLayout();
            this.SuspendLayout();
            //
            // dueDateCalendar  (the custom editor — a control, not a column type; assigned to colDueDate.Editor in code)
            //
            this.dueDateCalendar.Name = "dueDateCalendar";
            this.dueDateCalendar.Size = new System.Drawing.Size(260, 230);
            this.dueDateCalendar.ShowToday = true;
            //
            // ============================================================================================
            // pnlCommandRow — the command row: every path of this lab has a control here
            // ============================================================================================
            //
            this.pnlCommandRow.BackColor = System.Drawing.Color.White;
            this.pnlCommandRow.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCommandRow.Controls.Add(this.flowCommands);
            this.pnlCommandRow.Controls.Add(this.lblSubtitle);
            this.pnlCommandRow.Controls.Add(this.lblTitle);
            this.pnlCommandRow.Dock = Wisej.Web.DockStyle.Top;
            this.pnlCommandRow.Name = "pnlCommandRow";
            this.pnlCommandRow.Padding = new Wisej.Web.Padding(16, 12, 16, 8);
            this.pnlCommandRow.Size = new System.Drawing.Size(980, 150);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(948, 28);
            this.lblTitle.Text = "Orders";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(948, 34);
            this.lblSubtitle.Text = "Explicit columns on a BindingSource, an HTML status badge built in CellFormatting, a MonthCalendar " +
                "editor on the due date, and a virtual-mode path backed by OrderCache. Every button below drives one path.";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // flowCommands
            //
            this.flowCommands.AutoScroll = true;
            this.flowCommands.Controls.Add(this.btnLoadOrders);
            this.flowCommands.Controls.Add(this.btnOpenSelected);
            this.flowCommands.Controls.Add(this.btnPushDueDate);
            this.flowCommands.Controls.Add(this.btnPastDueDate);
            this.flowCommands.Controls.Add(this.chkCalendarEditor);
            this.flowCommands.Controls.Add(this.btnInjectUnsafeStatus);
            this.flowCommands.Controls.Add(this.chkSimulateFailure);
            this.flowCommands.Dock = Wisej.Web.DockStyle.Fill;
            this.flowCommands.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.flowCommands.Name = "flowCommands";
            this.flowCommands.Size = new System.Drawing.Size(948, 68);
            this.flowCommands.WrapContents = true;
            //
            // btnLoadOrders
            //
            this.btnLoadOrders.AccessibleName = "Load the orders that match the filter";
            this.btnLoadOrders.Margin = new Wisej.Web.Padding(0, 4, 8, 4);
            this.btnLoadOrders.Name = "btnLoadOrders";
            this.btnLoadOrders.Size = new System.Drawing.Size(120, 30);
            this.btnLoadOrders.TabIndex = 1;
            this.btnLoadOrders.Text = "Load orders";
            this.btnLoadOrders.Click += this.btnLoadOrders_Click;
            //
            // btnOpenSelected
            //
            this.btnOpenSelected.AccessibleName = "Open the selected order through the service";
            this.btnOpenSelected.Margin = new Wisej.Web.Padding(0, 4, 8, 4);
            this.btnOpenSelected.Name = "btnOpenSelected";
            this.btnOpenSelected.Size = new System.Drawing.Size(130, 30);
            this.btnOpenSelected.TabIndex = 2;
            this.btnOpenSelected.Text = "Open selected";
            this.btnOpenSelected.Click += this.btnOpenSelected_Click;
            //
            // btnPushDueDate
            //
            this.btnPushDueDate.AccessibleName = "Move the selected order's due date one week later";
            this.btnPushDueDate.Margin = new Wisej.Web.Padding(0, 4, 8, 4);
            this.btnPushDueDate.Name = "btnPushDueDate";
            this.btnPushDueDate.Size = new System.Drawing.Size(150, 30);
            this.btnPushDueDate.TabIndex = 3;
            this.btnPushDueDate.Text = "Due date + 7 days";
            this.btnPushDueDate.Click += this.btnPushDueDate_Click;
            //
            // btnPastDueDate
            //
            this.btnPastDueDate.AccessibleName = "Try to set a due date in the past — the service rejects it";
            this.btnPastDueDate.Margin = new Wisej.Web.Padding(0, 4, 8, 4);
            this.btnPastDueDate.Name = "btnPastDueDate";
            this.btnPastDueDate.Size = new System.Drawing.Size(190, 30);
            this.btnPastDueDate.TabIndex = 4;
            this.btnPastDueDate.Text = "Due date last week (rejected)";
            this.btnPastDueDate.Click += this.btnPastDueDate_Click;
            //
            // chkCalendarEditor
            //
            this.chkCalendarEditor.AccessibleName = "Use a MonthCalendar as the due-date cell editor";
            this.chkCalendarEditor.Margin = new Wisej.Web.Padding(0, 4, 8, 4);
            this.chkCalendarEditor.Name = "chkCalendarEditor";
            this.chkCalendarEditor.Size = new System.Drawing.Size(180, 30);
            this.chkCalendarEditor.TabIndex = 5;
            this.chkCalendarEditor.Text = "MonthCalendar editor";
            this.chkCalendarEditor.CheckedChanged += this.chkCalendarEditor_CheckedChanged;
            //
            // btnInjectUnsafeStatus
            //
            this.btnInjectUnsafeStatus.AccessibleName = "Store a status that contains markup, to prove the badge encodes it";
            this.btnInjectUnsafeStatus.Margin = new Wisej.Web.Padding(0, 4, 8, 4);
            this.btnInjectUnsafeStatus.Name = "btnInjectUnsafeStatus";
            this.btnInjectUnsafeStatus.Size = new System.Drawing.Size(170, 30);
            this.btnInjectUnsafeStatus.TabIndex = 6;
            this.btnInjectUnsafeStatus.Text = "Inject unsafe status";
            this.btnInjectUnsafeStatus.Click += this.btnInjectUnsafeStatus_Click;
            //
            // chkSimulateFailure
            //
            this.chkSimulateFailure.AccessibleName = "Make the orders service fail on the next call";
            this.chkSimulateFailure.Margin = new Wisej.Web.Padding(0, 4, 8, 4);
            this.chkSimulateFailure.Name = "chkSimulateFailure";
            this.chkSimulateFailure.Size = new System.Drawing.Size(190, 30);
            this.chkSimulateFailure.TabIndex = 7;
            this.chkSimulateFailure.Text = "Simulate service failure";
            this.chkSimulateFailure.CheckedChanged += this.chkSimulateFailure_CheckedChanged;
            //
            // ============================================================================================
            // pnlGridCard — the composed grid: filter strip (Top), status strip (Bottom), ordersGrid (Fill)
            // Dock order: the Fill child is added FIRST, the edge strips LAST (docking is applied from the
            // last child to the first), and ComposeGrid() re-asserts it with SendToBack / BringToFront.
            // ============================================================================================
            //
            this.pnlGridCard.BackColor = System.Drawing.Color.White;
            this.pnlGridCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlGridCard.Controls.Add(this.ordersGrid);
            this.pnlGridCard.Controls.Add(this.pnlGridStatus);
            this.pnlGridCard.Controls.Add(this.pnlFilterStrip);
            this.pnlGridCard.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlGridCard.Name = "pnlGridCard";
            this.pnlGridCard.Size = new System.Drawing.Size(980, 554);
            //
            // ordersGrid
            //
            this.ordersGrid.AllowUserToAddRows = false;
            this.ordersGrid.AllowUserToDeleteRows = false;
            this.ordersGrid.AutoGenerateColumns = false;
            this.ordersGrid.BorderStyle = Wisej.Web.BorderStyle.None;
            this.ordersGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.colNumber,
                this.colCustomer,
                this.colDueDate,
                this.colTotal,
                this.colStatus,
                this.colOpen});
            this.ordersGrid.Dock = Wisej.Web.DockStyle.Fill;
            this.ordersGrid.MultiSelect = false;
            this.ordersGrid.Name = "ordersGrid";
            this.ordersGrid.RowHeadersVisible = false;
            this.ordersGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.ordersGrid.ShowCellToolTips = true;
            this.ordersGrid.Size = new System.Drawing.Size(978, 472);
            this.ordersGrid.TabIndex = 20;
            this.ordersGrid.CellBeginEdit += this.ordersGrid_CellBeginEdit;
            this.ordersGrid.CellEndEdit += this.ordersGrid_CellEndEdit;
            this.ordersGrid.CellFormatting += this.ordersGrid_CellFormatting;
            this.ordersGrid.CellValueNeeded += this.ordersGrid_CellValueNeeded;
            this.ordersGrid.DataRead += this.ordersGrid_DataRead;
            this.ordersGrid.SelectionChanged += this.ordersGrid_SelectionChanged;
            // Wisej.NET 4.1 raises CellClick ("fired when any part of a cell is clicked") — it has no separate
            // CellContentClick like WinForms, so the lab's command-column handler is wired here.
            this.ordersGrid.CellClick += this.ordersGrid_CellContentClick;
            //
            // colNumber — the stable ID: narrow, read-only, sortable
            //
            this.colNumber.DataPropertyName = "Number";
            this.colNumber.HeaderText = "Order";
            this.colNumber.Name = "colNumber";
            this.colNumber.ReadOnly = true;
            this.colNumber.SortMode = Wisej.Web.DataGridViewColumnSortMode.Automatic;
            this.colNumber.Width = 120;
            //
            // colCustomer — the only column allowed to grow; plain text, never HTML
            //
            this.colCustomer.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colCustomer.DataPropertyName = "Customer";
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.MinimumWidth = 160;
            this.colCustomer.Name = "colCustomer";
            this.colCustomer.ReadOnly = true;
            this.colCustomer.SortMode = Wisej.Web.DataGridViewColumnSortMode.Automatic;
            this.colCustomer.Width = 280;
            //
            // colDueDate — the one editable column: a typed date column, optionally edited by dueDateCalendar
            //
            this.colDueDate.DataPropertyName = "DueDate";
            this.colDueDate.DefaultCellStyle = new Wisej.Web.DataGridViewCellStyle();
            this.colDueDate.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            this.colDueDate.DefaultCellStyle.Format = "d";
            this.colDueDate.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.colDueDate.HeaderText = "Due date";
            this.colDueDate.Name = "colDueDate";
            this.colDueDate.ReadOnly = false;
            this.colDueDate.SortMode = Wisej.Web.DataGridViewColumnSortMode.Automatic;
            this.colDueDate.Width = 140;
            //
            // colTotal — money: right aligned, "C2", read-only
            //
            this.colTotal.DataPropertyName = "Total";
            this.colTotal.DefaultCellStyle = new Wisej.Web.DataGridViewCellStyle();
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colTotal.DefaultCellStyle.Format = "C2";
            this.colTotal.HeaderText = "Total";
            this.colTotal.Name = "colTotal";
            this.colTotal.ReadOnly = true;
            this.colTotal.SortMode = Wisej.Web.DataGridViewColumnSortMode.Automatic;
            this.colTotal.Width = 120;
            //
            // colStatus — the HTML cell: AllowHtml here, the markup is produced in CellFormatting
            //
            this.colStatus.AllowHtml = true;
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.SortMode = Wisej.Web.DataGridViewColumnSortMode.Automatic;
            this.colStatus.Width = 150;
            //
            // colOpen — the command column: one button per row, no business data in the cell
            //
            this.colOpen.HeaderText = "";
            this.colOpen.Name = "colOpen";
            this.colOpen.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            this.colOpen.Text = "Open";
            this.colOpen.UseColumnTextForButtonValue = true;
            this.colOpen.Width = 90;
            //
            // pnlFilterStrip — docked Top inside the grid card
            //
            this.pnlFilterStrip.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.pnlFilterStrip.Controls.Add(this.txtSearch);
            this.pnlFilterStrip.Controls.Add(this.pnlFilterActions);
            this.pnlFilterStrip.Dock = Wisej.Web.DockStyle.Top;
            this.pnlFilterStrip.Name = "pnlFilterStrip";
            this.pnlFilterStrip.Padding = new Wisej.Web.Padding(10, 8, 10, 8);
            this.pnlFilterStrip.Size = new System.Drawing.Size(978, 48);
            //
            // txtSearch
            //
            this.txtSearch.AccessibleName = "Search orders by number or customer";
            this.txtSearch.Dock = Wisej.Web.DockStyle.Fill;
            this.txtSearch.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.txtSearch.MaxLength = 60;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(500, 32);
            this.txtSearch.TabIndex = 10;
            this.txtSearch.Watermark = "Search orders — number or customer";
            //
            // pnlFilterActions
            //
            this.pnlFilterActions.Controls.Add(this.cboStatus);
            this.pnlFilterActions.Controls.Add(this.btnApply);
            this.pnlFilterActions.Controls.Add(this.btnClearFilter);
            this.pnlFilterActions.Controls.Add(this.chkVirtualMode);
            this.pnlFilterActions.Dock = Wisej.Web.DockStyle.Right;
            this.pnlFilterActions.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlFilterActions.Name = "pnlFilterActions";
            this.pnlFilterActions.Size = new System.Drawing.Size(452, 32);
            this.pnlFilterActions.WrapContents = false;
            //
            // cboStatus
            //
            this.cboStatus.AccessibleName = "Filter the orders by status";
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Margin = new Wisej.Web.Padding(0, 0, 8, 0);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(150, 32);
            this.cboStatus.TabIndex = 11;
            //
            // btnApply
            //
            this.btnApply.AccessibleName = "Apply the filter";
            this.btnApply.Margin = new Wisej.Web.Padding(0, 0, 8, 0);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(80, 32);
            this.btnApply.TabIndex = 12;
            this.btnApply.Text = "Apply";
            this.btnApply.Click += this.btnApply_Click;
            //
            // btnClearFilter
            //
            this.btnClearFilter.AccessibleName = "Clear the filter";
            this.btnClearFilter.Margin = new Wisej.Web.Padding(0, 0, 8, 0);
            this.btnClearFilter.Name = "btnClearFilter";
            this.btnClearFilter.Size = new System.Drawing.Size(70, 32);
            this.btnClearFilter.TabIndex = 13;
            this.btnClearFilter.Text = "Clear";
            this.btnClearFilter.Click += this.btnClearFilter_Click;
            //
            // chkVirtualMode
            //
            this.chkVirtualMode.AccessibleName = "Switch the grid between the bound path and the virtual path";
            this.chkVirtualMode.Margin = new Wisej.Web.Padding(0, 0, 0, 0);
            this.chkVirtualMode.Name = "chkVirtualMode";
            this.chkVirtualMode.Size = new System.Drawing.Size(126, 32);
            this.chkVirtualMode.TabIndex = 14;
            this.chkVirtualMode.Text = "Virtual mode";
            this.chkVirtualMode.CheckedChanged += this.chkVirtualMode_CheckedChanged;
            //
            // pnlGridStatus — docked Bottom inside the grid card
            //
            this.pnlGridStatus.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.pnlGridStatus.Controls.Add(this.flowGridStatus);
            this.pnlGridStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlGridStatus.Name = "pnlGridStatus";
            this.pnlGridStatus.Padding = new Wisej.Web.Padding(10, 4, 10, 4);
            this.pnlGridStatus.Size = new System.Drawing.Size(978, 34);
            //
            // flowGridStatus
            //
            this.flowGridStatus.Controls.Add(this.lblRowCount);
            this.flowGridStatus.Controls.Add(this.lblSelectedOrder);
            this.flowGridStatus.Controls.Add(this.lblCacheState);
            this.flowGridStatus.Controls.Add(this.lblLastRefresh);
            this.flowGridStatus.Dock = Wisej.Web.DockStyle.Fill;
            this.flowGridStatus.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.flowGridStatus.Name = "flowGridStatus";
            this.flowGridStatus.Size = new System.Drawing.Size(958, 26);
            this.flowGridStatus.WrapContents = false;
            //
            // lblRowCount
            //
            this.lblRowCount.AutoSize = false;
            this.lblRowCount.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblRowCount.Margin = new Wisej.Web.Padding(0, 0, 18, 0);
            this.lblRowCount.Name = "lblRowCount";
            this.lblRowCount.Size = new System.Drawing.Size(240, 24);
            this.lblRowCount.Text = "Rows: —";
            this.lblRowCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSelectedOrder
            //
            this.lblSelectedOrder.AutoSize = false;
            this.lblSelectedOrder.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblSelectedOrder.Margin = new Wisej.Web.Padding(0, 0, 18, 0);
            this.lblSelectedOrder.Name = "lblSelectedOrder";
            this.lblSelectedOrder.Size = new System.Drawing.Size(230, 24);
            this.lblSelectedOrder.Text = "Selected: —";
            this.lblSelectedOrder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblCacheState
            //
            this.lblCacheState.AutoSize = false;
            this.lblCacheState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCacheState.Margin = new Wisej.Web.Padding(0, 0, 18, 0);
            this.lblCacheState.Name = "lblCacheState";
            this.lblCacheState.Size = new System.Drawing.Size(300, 24);
            this.lblCacheState.Text = "Bound path — no cache in use";
            this.lblCacheState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblLastRefresh
            //
            this.lblLastRefresh.AutoSize = false;
            this.lblLastRefresh.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblLastRefresh.Margin = new Wisej.Web.Padding(0, 0, 0, 0);
            this.lblLastRefresh.Name = "lblLastRefresh";
            this.lblLastRefresh.Size = new System.Drawing.Size(170, 24);
            this.lblLastRefresh.Text = "Last refresh: —";
            this.lblLastRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // DataGridViewPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlGridCard);
            this.Controls.Add(this.pnlCommandRow);
            this.Name = "DataGridViewPage";
            this.Padding = new Wisej.Web.Padding(16, 16, 16, 16);
            this.Size = new System.Drawing.Size(1012, 736);
            this.pnlFilterActions.ResumeLayout(false);
            this.pnlFilterStrip.ResumeLayout(false);
            this.flowGridStatus.ResumeLayout(false);
            this.pnlGridStatus.ResumeLayout(false);
            this.pnlGridCard.ResumeLayout(false);
            this.flowCommands.ResumeLayout(false);
            this.pnlCommandRow.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.BindingSource ordersSource;
        private Wisej.Web.MonthCalendar dueDateCalendar;

        private Wisej.Web.Panel pnlCommandRow;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblSubtitle;
        private Wisej.Web.FlowLayoutPanel flowCommands;
        private Wisej.Web.Button btnLoadOrders;
        private Wisej.Web.Button btnOpenSelected;
        private Wisej.Web.Button btnPushDueDate;
        private Wisej.Web.Button btnPastDueDate;
        private Wisej.Web.CheckBox chkCalendarEditor;
        private Wisej.Web.Button btnInjectUnsafeStatus;
        private Wisej.Web.CheckBox chkSimulateFailure;

        private Wisej.Web.Panel pnlGridCard;
        private Wisej.Web.DataGridView ordersGrid;
        private Wisej.Web.DataGridViewTextBoxColumn colNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewDateTimePickerColumn colDueDate;
        private Wisej.Web.DataGridViewTextBoxColumn colTotal;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewButtonColumn colOpen;

        private Wisej.Web.Panel pnlFilterStrip;
        private Wisej.Web.TextBox txtSearch;
        private Wisej.Web.FlowLayoutPanel pnlFilterActions;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.Button btnApply;
        private Wisej.Web.Button btnClearFilter;
        private Wisej.Web.CheckBox chkVirtualMode;

        private Wisej.Web.Panel pnlGridStatus;
        private Wisej.Web.FlowLayoutPanel flowGridStatus;
        private Wisej.Web.Label lblRowCount;
        private Wisej.Web.Label lblSelectedOrder;
        private Wisej.Web.Label lblCacheState;
        private Wisej.Web.Label lblLastRefresh;
    }
}
