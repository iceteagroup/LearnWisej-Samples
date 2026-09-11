namespace TicketOps.Views
{
    partial class WorkOrdersPage
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

        private void InitializeComponent()
        {
            Wisej.Web.DataGridViewCellStyle cellStyleCost = new Wisej.Web.DataGridViewCellStyle();
            Wisej.Web.DataGridViewCellStyle cellStyleDue = new Wisej.Web.DataGridViewCellStyle();
            this.components = new System.ComponentModel.Container();
            this.workOrderSource = new Wisej.Web.BindingSource(this.components);
            this.panelScreen = new Wisej.Web.Panel();
            this.labelScreenTitle = new Wisej.Web.Label();
            this.statusBanner = new TicketOps.Controls.StatusBanner();
            this.textSearch = new Wisej.Web.TextBox();
            this.comboStatusFilter = new Wisej.Web.ComboBox();
            this.labelCount = new Wisej.Web.Label();
            this.dgvWorkOrders = new Wisej.Web.DataGridView();
            this.columnId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnAssignedTo = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnDueDate = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnCost = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelDetailTitle = new Wisej.Web.Label();
            this.labelDirty = new Wisej.Web.Label();
            this.buttonReload = new Wisej.Web.Button();
            this.panelDetail = new Wisej.Web.Panel();
            this.labelTitleCaption = new Wisej.Web.Label();
            this.textTitle = new Wisej.Web.TextBox();
            this.labelStatusCaption = new Wisej.Web.Label();
            this.comboStatus = new Wisej.Web.ComboBox();
            this.labelPriorityCaption = new Wisej.Web.Label();
            this.comboPriority = new Wisej.Web.ComboBox();
            this.labelAssignedCaption = new Wisej.Web.Label();
            this.textAssignedTo = new Wisej.Web.TextBox();
            this.labelDueCaption = new Wisej.Web.Label();
            this.dateDue = new Wisej.Web.DateTimePicker();
            this.labelCostCaption = new Wisej.Web.Label();
            this.numericCost = new Wisej.Web.NumericUpDown();
            this.buttonSave = new Wisej.Web.Button();
            this.buttonDiscard = new Wisej.Web.Button();
            this.panelScreen.SuspendLayout();
            this.panelDetail.SuspendLayout();
            this.SuspendLayout();
            //
            // workOrderSource
            //
            this.workOrderSource.CurrentChanged += new System.EventHandler(this.workOrderSource_CurrentChanged);
            //
            // panelScreen
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.textSearch);
            this.panelScreen.Controls.Add(this.comboStatusFilter);
            this.panelScreen.Controls.Add(this.labelCount);
            this.panelScreen.Controls.Add(this.dgvWorkOrders);
            this.panelScreen.Controls.Add(this.labelDetailTitle);
            this.panelScreen.Controls.Add(this.labelDirty);
            this.panelScreen.Controls.Add(this.buttonReload);
            this.panelScreen.Controls.Add(this.panelDetail);
            this.panelScreen.Location = new System.Drawing.Point(30, 30);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(760, 556);
            //
            // labelScreenTitle
            //
            this.labelScreenTitle.AutoSize = false;
            this.labelScreenTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelScreenTitle.Location = new System.Drawing.Point(24, 18);
            this.labelScreenTitle.Name = "labelScreenTitle";
            this.labelScreenTitle.Size = new System.Drawing.Size(300, 30);
            this.labelScreenTitle.Text = "Work Orders";
            //
            // statusBanner
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // textSearch
            //
            this.textSearch.Location = new System.Drawing.Point(24, 84);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(280, 30);
            this.textSearch.Watermark = "Search work orders…";
            this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
            //
            // comboStatusFilter
            //
            this.comboStatusFilter.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboStatusFilter.Location = new System.Drawing.Point(316, 84);
            this.comboStatusFilter.Name = "comboStatusFilter";
            this.comboStatusFilter.Size = new System.Drawing.Size(150, 30);
            this.comboStatusFilter.SelectedIndexChanged += new System.EventHandler(this.comboStatusFilter_SelectedIndexChanged);
            //
            // labelCount
            //
            this.labelCount.AutoSize = false;
            this.labelCount.Font = new System.Drawing.Font("default", 9F);
            this.labelCount.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelCount.Location = new System.Drawing.Point(476, 89);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(260, 20);
            this.labelCount.Text = "loading…";
            this.labelCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dgvWorkOrders
            //
            this.dgvWorkOrders.AllowUserToAddRows = false;
            this.dgvWorkOrders.AllowUserToDeleteRows = false;
            this.dgvWorkOrders.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvWorkOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.columnId,
                this.columnTitle,
                this.columnStatus,
                this.columnPriority,
                this.columnAssignedTo,
                this.columnDueDate,
                this.columnCost});
            this.dgvWorkOrders.Location = new System.Drawing.Point(24, 124);
            this.dgvWorkOrders.MultiSelect = false;
            this.dgvWorkOrders.Name = "dgvWorkOrders";
            this.dgvWorkOrders.ReadOnly = true;
            this.dgvWorkOrders.RowHeadersVisible = false;
            this.dgvWorkOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWorkOrders.Size = new System.Drawing.Size(712, 210);
            this.dgvWorkOrders.CellFormatting += new Wisej.Web.DataGridViewCellFormattingEventHandler(this.dgvWorkOrders_CellFormatting);
            //
            // columns  (DataPropertyName = WorkOrder property; formats live here, not in the model)
            //
            this.columnId.DataPropertyName = "Id";
            this.columnId.HeaderText = "Id";
            this.columnId.Name = "columnId";
            this.columnId.ReadOnly = true;
            this.columnId.Width = 56;
            this.columnTitle.DataPropertyName = "Title";
            this.columnTitle.HeaderText = "Title";
            this.columnTitle.Name = "columnTitle";
            this.columnTitle.ReadOnly = true;
            this.columnTitle.Width = 214;
            this.columnStatus.DataPropertyName = "Status";
            this.columnStatus.HeaderText = "Status";
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.ReadOnly = true;
            this.columnStatus.Width = 96;
            this.columnPriority.DataPropertyName = "Priority";
            this.columnPriority.HeaderText = "Priority";
            this.columnPriority.Name = "columnPriority";
            this.columnPriority.ReadOnly = true;
            this.columnPriority.Width = 76;
            this.columnAssignedTo.DataPropertyName = "AssignedTo";
            this.columnAssignedTo.HeaderText = "Assigned";
            this.columnAssignedTo.Name = "columnAssignedTo";
            this.columnAssignedTo.ReadOnly = true;
            this.columnAssignedTo.Width = 104;
            cellStyleDue.Format = "MMM d";
            this.columnDueDate.DataPropertyName = "DueDate";
            this.columnDueDate.DefaultCellStyle = cellStyleDue;
            this.columnDueDate.HeaderText = "Due";
            this.columnDueDate.Name = "columnDueDate";
            this.columnDueDate.ReadOnly = true;
            this.columnDueDate.Width = 66;
            cellStyleCost.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            cellStyleCost.Format = "C2";
            this.columnCost.DataPropertyName = "Cost";
            this.columnCost.DefaultCellStyle = cellStyleCost;
            this.columnCost.HeaderText = "Cost";
            this.columnCost.Name = "columnCost";
            this.columnCost.ReadOnly = true;
            this.columnCost.Width = 88;
            //
            // labelDetailTitle
            //
            this.labelDetailTitle.AutoSize = false;
            this.labelDetailTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelDetailTitle.Location = new System.Drawing.Point(24, 344);
            this.labelDetailTitle.Name = "labelDetailTitle";
            this.labelDetailTitle.Size = new System.Drawing.Size(280, 22);
            this.labelDetailTitle.Text = "Detail";
            //
            // labelDirty
            //
            this.labelDirty.AutoSize = false;
            this.labelDirty.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelDirty.ForeColor = System.Drawing.Color.FromArgb(214, 122, 0);
            this.labelDirty.Location = new System.Drawing.Point(312, 344);
            this.labelDirty.Name = "labelDirty";
            this.labelDirty.Size = new System.Drawing.Size(290, 22);
            this.labelDirty.Text = "";
            this.labelDirty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // buttonReload
            //
            this.buttonReload.Location = new System.Drawing.Point(616, 340);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(120, 30);
            this.buttonReload.Text = "↻ Reload";
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            //
            // panelDetail  (the master-detail editor; disabled when the BindingSource has no current item)
            //
            this.panelDetail.Controls.Add(this.labelTitleCaption);
            this.panelDetail.Controls.Add(this.textTitle);
            this.panelDetail.Controls.Add(this.labelStatusCaption);
            this.panelDetail.Controls.Add(this.comboStatus);
            this.panelDetail.Controls.Add(this.labelPriorityCaption);
            this.panelDetail.Controls.Add(this.comboPriority);
            this.panelDetail.Controls.Add(this.labelAssignedCaption);
            this.panelDetail.Controls.Add(this.textAssignedTo);
            this.panelDetail.Controls.Add(this.labelDueCaption);
            this.panelDetail.Controls.Add(this.dateDue);
            this.panelDetail.Controls.Add(this.labelCostCaption);
            this.panelDetail.Controls.Add(this.numericCost);
            this.panelDetail.Controls.Add(this.buttonSave);
            this.panelDetail.Controls.Add(this.buttonDiscard);
            this.panelDetail.Location = new System.Drawing.Point(24, 368);
            this.panelDetail.Name = "panelDetail";
            this.panelDetail.Size = new System.Drawing.Size(712, 166);
            //
            // labelTitleCaption
            //
            this.labelTitleCaption.AutoSize = false;
            this.labelTitleCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelTitleCaption.Location = new System.Drawing.Point(0, 4);
            this.labelTitleCaption.Name = "labelTitleCaption";
            this.labelTitleCaption.Size = new System.Drawing.Size(300, 18);
            this.labelTitleCaption.Text = "Title";
            //
            // textTitle
            //
            this.textTitle.Location = new System.Drawing.Point(0, 24);
            this.textTitle.Name = "textTitle";
            this.textTitle.Size = new System.Drawing.Size(300, 30);
            //
            // labelStatusCaption
            //
            this.labelStatusCaption.AutoSize = false;
            this.labelStatusCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelStatusCaption.Location = new System.Drawing.Point(312, 4);
            this.labelStatusCaption.Name = "labelStatusCaption";
            this.labelStatusCaption.Size = new System.Drawing.Size(120, 18);
            this.labelStatusCaption.Text = "Status";
            //
            // comboStatus
            //
            this.comboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboStatus.Location = new System.Drawing.Point(312, 24);
            this.comboStatus.Name = "comboStatus";
            this.comboStatus.Size = new System.Drawing.Size(120, 30);
            this.comboStatus.SelectedIndexChanged += new System.EventHandler(this.comboStatus_SelectedIndexChanged);
            //
            // labelPriorityCaption
            //
            this.labelPriorityCaption.AutoSize = false;
            this.labelPriorityCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelPriorityCaption.Location = new System.Drawing.Point(444, 4);
            this.labelPriorityCaption.Name = "labelPriorityCaption";
            this.labelPriorityCaption.Size = new System.Drawing.Size(110, 18);
            this.labelPriorityCaption.Text = "Priority";
            //
            // comboPriority
            //
            this.comboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboPriority.Location = new System.Drawing.Point(444, 24);
            this.comboPriority.Name = "comboPriority";
            this.comboPriority.Size = new System.Drawing.Size(110, 30);
            this.comboPriority.SelectedIndexChanged += new System.EventHandler(this.comboPriority_SelectedIndexChanged);
            //
            // labelAssignedCaption
            //
            this.labelAssignedCaption.AutoSize = false;
            this.labelAssignedCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelAssignedCaption.Location = new System.Drawing.Point(0, 64);
            this.labelAssignedCaption.Name = "labelAssignedCaption";
            this.labelAssignedCaption.Size = new System.Drawing.Size(200, 18);
            this.labelAssignedCaption.Text = "Assigned to";
            //
            // textAssignedTo
            //
            this.textAssignedTo.Location = new System.Drawing.Point(0, 84);
            this.textAssignedTo.Name = "textAssignedTo";
            this.textAssignedTo.Size = new System.Drawing.Size(200, 30);
            this.textAssignedTo.Watermark = "Unassigned";
            //
            // labelDueCaption
            //
            this.labelDueCaption.AutoSize = false;
            this.labelDueCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelDueCaption.Location = new System.Drawing.Point(212, 64);
            this.labelDueCaption.Name = "labelDueCaption";
            this.labelDueCaption.Size = new System.Drawing.Size(150, 18);
            this.labelDueCaption.Text = "Due date";
            //
            // dateDue
            //
            this.dateDue.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dateDue.Location = new System.Drawing.Point(212, 84);
            this.dateDue.Name = "dateDue";
            this.dateDue.Size = new System.Drawing.Size(150, 30);
            //
            // labelCostCaption
            //
            this.labelCostCaption.AutoSize = false;
            this.labelCostCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelCostCaption.Location = new System.Drawing.Point(374, 64);
            this.labelCostCaption.Name = "labelCostCaption";
            this.labelCostCaption.Size = new System.Drawing.Size(130, 18);
            this.labelCostCaption.Text = "Cost";
            //
            // numericCost
            //
            this.numericCost.DecimalPlaces = 2;
            this.numericCost.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            this.numericCost.Location = new System.Drawing.Point(374, 84);
            this.numericCost.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.numericCost.Name = "numericCost";
            this.numericCost.Size = new System.Drawing.Size(130, 30);
            //
            // buttonSave
            //
            this.buttonSave.Enabled = false;
            this.buttonSave.Location = new System.Drawing.Point(0, 126);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(120, 36);
            this.buttonSave.Text = "Save";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            //
            // buttonDiscard
            //
            this.buttonDiscard.Enabled = false;
            this.buttonDiscard.Location = new System.Drawing.Point(130, 126);
            this.buttonDiscard.Name = "buttonDiscard";
            this.buttonDiscard.Size = new System.Drawing.Size(120, 36);
            this.buttonDiscard.Text = "Discard";
            this.buttonDiscard.Click += new System.EventHandler(this.buttonDiscard_Click);
            //
            // WorkOrdersPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(820, 616);
            this.Controls.Add(this.panelScreen);
            this.Name = "WorkOrdersPage";
            this.Text = "TicketOps Console";
            this.Load += new System.EventHandler(this.WorkOrdersPage_Load);
            this.panelScreen.ResumeLayout(false);
            this.panelDetail.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.BindingSource workOrderSource;
        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.TextBox textSearch;
        private Wisej.Web.ComboBox comboStatusFilter;
        private Wisej.Web.Label labelCount;
        private Wisej.Web.DataGridView dgvWorkOrders;
        private Wisej.Web.DataGridViewTextBoxColumn columnId;
        private Wisej.Web.DataGridViewTextBoxColumn columnTitle;
        private Wisej.Web.DataGridViewTextBoxColumn columnStatus;
        private Wisej.Web.DataGridViewTextBoxColumn columnPriority;
        private Wisej.Web.DataGridViewTextBoxColumn columnAssignedTo;
        private Wisej.Web.DataGridViewTextBoxColumn columnDueDate;
        private Wisej.Web.DataGridViewTextBoxColumn columnCost;
        private Wisej.Web.Label labelDetailTitle;
        private Wisej.Web.Label labelDirty;
        private Wisej.Web.Button buttonReload;
        private Wisej.Web.Panel panelDetail;
        private Wisej.Web.Label labelTitleCaption;
        private Wisej.Web.TextBox textTitle;
        private Wisej.Web.Label labelStatusCaption;
        private Wisej.Web.ComboBox comboStatus;
        private Wisej.Web.Label labelPriorityCaption;
        private Wisej.Web.ComboBox comboPriority;
        private Wisej.Web.Label labelAssignedCaption;
        private Wisej.Web.TextBox textAssignedTo;
        private Wisej.Web.Label labelDueCaption;
        private Wisej.Web.DateTimePicker dateDue;
        private Wisej.Web.Label labelCostCaption;
        private Wisej.Web.NumericUpDown numericCost;
        private Wisej.Web.Button buttonSave;
        private Wisej.Web.Button buttonDiscard;
    }
}
