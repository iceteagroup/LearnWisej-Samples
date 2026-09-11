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
            this.chkVirtualMode = new Wisej.Web.CheckBox();
            this.chkSimulateFailure = new Wisej.Web.CheckBox();
            this.pnlGridCard.SuspendLayout();
            this.pnlGridStatus.SuspendLayout();
            this.flowGridStatus.SuspendLayout();
            this.pnlFilterStrip.SuspendLayout();
            this.pnlFilterActions.SuspendLayout();
            this.SuspendLayout();
            //
            // dueDateCalendar
            //
            this.dueDateCalendar.Name = "dueDateCalendar";
            this.dueDateCalendar.Size = new System.Drawing.Size(260, 230);
            this.dueDateCalendar.ShowToday = true;
            //
            // pnlGridCard
            //
            this.pnlGridCard.BackColor = System.Drawing.Color.White;
            this.pnlGridCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlGridCard.Controls.Add(this.ordersGrid);
            this.pnlGridCard.Controls.Add(this.pnlGridStatus);
            this.pnlGridCard.Controls.Add(this.pnlFilterStrip);
            this.pnlGridCard.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlGridCard.Name = "pnlGridCard";
            this.pnlGridCard.Size = new System.Drawing.Size(980, 704);
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
            this.ordersGrid.Size = new System.Drawing.Size(978, 620);
            this.ordersGrid.TabIndex = 20;
            this.ordersGrid.CellBeginEdit += this.ordersGrid_CellBeginEdit;
            this.ordersGrid.CellEndEdit += this.ordersGrid_CellEndEdit;
            this.ordersGrid.CellFormatting += this.ordersGrid_CellFormatting;
            this.ordersGrid.CellValueNeeded += this.ordersGrid_CellValueNeeded;
            this.ordersGrid.DataRead += this.ordersGrid_DataRead;
            this.ordersGrid.SelectionChanged += this.ordersGrid_SelectionChanged;
            this.ordersGrid.CellClick += this.ordersGrid_CellContentClick;
            //
            // colNumber
            //
            this.colNumber.DataPropertyName = "Number";
            this.colNumber.HeaderText = "Order";
            this.colNumber.Name = "colNumber";
            this.colNumber.ReadOnly = true;
            this.colNumber.SortMode = Wisej.Web.DataGridViewColumnSortMode.Automatic;
            this.colNumber.Width = 120;
            //
            // colCustomer
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
            // colDueDate
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
            // colTotal
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
            // colStatus
            //
            this.colStatus.AllowHtml = true;
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.SortMode = Wisej.Web.DataGridViewColumnSortMode.Automatic;
            this.colStatus.Width = 150;
            //
            // colOpen
            //
            this.colOpen.HeaderText = "";
            this.colOpen.Name = "colOpen";
            this.colOpen.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            this.colOpen.Text = "Open";
            this.colOpen.UseColumnTextForButtonValue = true;
            this.colOpen.Width = 90;
            //
            // pnlFilterStrip
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
            this.txtSearch.Size = new System.Drawing.Size(380, 32);
            this.txtSearch.TabIndex = 10;
            this.txtSearch.Watermark = "Search orders — number or customer";
            //
            // pnlFilterActions
            //
            this.pnlFilterActions.Controls.Add(this.cboStatus);
            this.pnlFilterActions.Controls.Add(this.btnApply);
            this.pnlFilterActions.Controls.Add(this.chkVirtualMode);
            this.pnlFilterActions.Controls.Add(this.chkSimulateFailure);
            this.pnlFilterActions.Dock = Wisej.Web.DockStyle.Right;
            this.pnlFilterActions.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlFilterActions.Name = "pnlFilterActions";
            this.pnlFilterActions.Size = new System.Drawing.Size(578, 32);
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
            // chkVirtualMode
            //
            this.chkVirtualMode.AccessibleName = "Switch the grid between the bound path and the virtual path";
            this.chkVirtualMode.Margin = new Wisej.Web.Padding(0, 0, 8, 0);
            this.chkVirtualMode.Name = "chkVirtualMode";
            this.chkVirtualMode.Size = new System.Drawing.Size(126, 32);
            this.chkVirtualMode.TabIndex = 13;
            this.chkVirtualMode.Text = "Virtual mode";
            this.chkVirtualMode.CheckedChanged += this.chkVirtualMode_CheckedChanged;
            //
            // chkSimulateFailure
            //
            this.chkSimulateFailure.AccessibleName = "Make the orders service fail";
            this.chkSimulateFailure.Margin = new Wisej.Web.Padding(0, 0, 0, 0);
            this.chkSimulateFailure.Name = "chkSimulateFailure";
            this.chkSimulateFailure.Size = new System.Drawing.Size(190, 32);
            this.chkSimulateFailure.TabIndex = 14;
            this.chkSimulateFailure.Text = "Simulate service failure";
            this.chkSimulateFailure.CheckedChanged += this.chkSimulateFailure_CheckedChanged;
            //
            // pnlGridStatus
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
            this.lblRowCount.Size = new System.Drawing.Size(200, 24);
            this.lblRowCount.Text = "Rows: —";
            this.lblRowCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSelectedOrder
            //
            this.lblSelectedOrder.AutoSize = false;
            this.lblSelectedOrder.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblSelectedOrder.Margin = new Wisej.Web.Padding(0, 0, 18, 0);
            this.lblSelectedOrder.Name = "lblSelectedOrder";
            this.lblSelectedOrder.Size = new System.Drawing.Size(200, 24);
            this.lblSelectedOrder.Text = "Selected: —";
            this.lblSelectedOrder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblCacheState
            //
            this.lblCacheState.AutoSize = false;
            this.lblCacheState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCacheState.Margin = new Wisej.Web.Padding(0, 0, 18, 0);
            this.lblCacheState.Name = "lblCacheState";
            this.lblCacheState.Size = new System.Drawing.Size(320, 24);
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
            this.Name = "DataGridViewPage";
            this.Padding = new Wisej.Web.Padding(16, 16, 16, 16);
            this.Size = new System.Drawing.Size(1012, 736);
            this.pnlFilterActions.ResumeLayout(false);
            this.pnlFilterStrip.ResumeLayout(false);
            this.flowGridStatus.ResumeLayout(false);
            this.pnlGridStatus.ResumeLayout(false);
            this.pnlGridCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.BindingSource ordersSource;
        private Wisej.Web.MonthCalendar dueDateCalendar;

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
        private Wisej.Web.CheckBox chkVirtualMode;
        private Wisej.Web.CheckBox chkSimulateFailure;

        private Wisej.Web.Panel pnlGridStatus;
        private Wisej.Web.FlowLayoutPanel flowGridStatus;
        private Wisej.Web.Label lblRowCount;
        private Wisej.Web.Label lblSelectedOrder;
        private Wisej.Web.Label lblCacheState;
        private Wisej.Web.Label lblLastRefresh;
    }
}
