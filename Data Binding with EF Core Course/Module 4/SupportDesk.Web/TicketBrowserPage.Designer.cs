namespace SupportDesk.Web
{
    partial class TicketBrowserPage
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
            this.components = new System.ComponentModel.Container();
            this.ticketBindingSource = new Wisej.Web.BindingSource(this.components);
            this.panelBrowser = new Wisej.Web.Panel();
            this.labelTitle = new Wisej.Web.Label();
            this.labelState = new Wisej.Web.Label();
            this.labelLead = new Wisej.Web.Label();
            this.labelSearchCaption = new Wisej.Web.Label();
            this.labelStatusCaption = new Wisej.Web.Label();
            this.labelCustomerCaption = new Wisej.Web.Label();
            this.labelDueCaption = new Wisej.Web.Label();
            this.searchTextBox = new Wisej.Web.TextBox();
            this.statusComboBox = new Wisej.Web.ComboBox();
            this.customerComboBox = new Wisej.Web.ComboBox();
            this.dueFromDateTimePicker = new Wisej.Web.DateTimePicker();
            this.dueToDateTimePicker = new Wisej.Web.DateTimePicker();
            this.searchButton = new Wisej.Web.Button();
            this.ticketsDataGridView = new Wisej.Web.DataGridView();
            this.colNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomerName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAgentName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCategoryName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDueDate = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colUpdatedAt = new Wisej.Web.DataGridViewTextBoxColumn();
            this.statusLabel = new Wisej.Web.Label();
            this.prevPageButton = new Wisej.Web.Button();
            this.nextPageButton = new Wisej.Web.Button();
            this.labelBanner = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelModel = new Wisej.Web.Panel();
            this.labelModelTitle = new Wisej.Web.Label();
            this.labelModel = new Wisej.Web.Label();
            this.labelLifetimesTitle = new Wisej.Web.Label();
            this.labelLifetimes = new Wisej.Web.Label();
            this.labelRule = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.labelActions0 = new Wisej.Web.Label();
            this.btnAdd = new Wisej.Web.Button();
            this.btnEdit = new Wisej.Web.Button();
            this.buttonSlowSave = new Wisej.Web.Button();
            this.buttonSimulateDelete = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.panelActions2 = new Wisej.Web.Panel();
            this.labelActions1 = new Wisej.Web.Label();
            this.buttonSlowSearch = new Wisej.Web.Button();
            this.buttonBreak = new Wisej.Web.Button();
            this.buttonRestore = new Wisej.Web.Button();
            this.buttonBindQuery = new Wisej.Web.Button();
            this.panelActions3 = new Wisej.Web.Panel();
            this.labelActions2 = new Wisej.Web.Label();
            this.btnSeed = new Wisej.Web.Button();
            this.buttonReset = new Wisej.Web.Button();
            this.buttonOverlongTitle = new Wisej.Web.Button();
            this.buttonDeleteCustomer = new Wisej.Web.Button();
            this.buttonDeleteAgent = new Wisej.Web.Button();
            this.buttonDeleteTicket = new Wisej.Web.Button();
            this.panelActions4 = new Wisej.Web.Panel();
            this.labelActions3 = new Wisej.Web.Label();
            this.countButton = new Wisej.Web.Button();
            this.buttonSlowCount = new Wisej.Web.Button();
            this.buttonRapid = new Wisej.Web.Button();
            this.buttonAntiPattern = new Wisej.Web.Button();
            this.panelBrowser.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelModel.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.panelActions2.SuspendLayout();
            this.panelActions3.SuspendLayout();
            this.panelActions4.SuspendLayout();
            this.SuspendLayout();
            //
            // panelBrowser  (the ticket browser card — the Module 3 search, now with Module 4's editor)
            //
            this.panelBrowser.BackColor = System.Drawing.Color.White;
            this.panelBrowser.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelBrowser.Controls.Add(this.labelTitle);
            this.panelBrowser.Controls.Add(this.labelState);
            this.panelBrowser.Controls.Add(this.labelLead);
            this.panelBrowser.Controls.Add(this.labelSearchCaption);
            this.panelBrowser.Controls.Add(this.labelStatusCaption);
            this.panelBrowser.Controls.Add(this.labelCustomerCaption);
            this.panelBrowser.Controls.Add(this.labelDueCaption);
            this.panelBrowser.Controls.Add(this.searchTextBox);
            this.panelBrowser.Controls.Add(this.statusComboBox);
            this.panelBrowser.Controls.Add(this.customerComboBox);
            this.panelBrowser.Controls.Add(this.dueFromDateTimePicker);
            this.panelBrowser.Controls.Add(this.dueToDateTimePicker);
            this.panelBrowser.Controls.Add(this.searchButton);
            this.panelBrowser.Controls.Add(this.ticketsDataGridView);
            this.panelBrowser.Controls.Add(this.statusLabel);
            this.panelBrowser.Controls.Add(this.prevPageButton);
            this.panelBrowser.Controls.Add(this.nextPageButton);
            this.panelBrowser.Controls.Add(this.labelBanner);
            this.panelBrowser.Location = new System.Drawing.Point(30, 30);
            this.panelBrowser.Name = "panelBrowser";
            this.panelBrowser.Size = new System.Drawing.Size(940, 640);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 16);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(420, 30);
            this.labelTitle.Text = "Support Desk Data Console  ·  Tickets";
            //
            // labelState  (● idle / working / ok / fault)
            //
            this.labelState.AutoSize = false;
            this.labelState.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelState.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelState.Location = new System.Drawing.Point(700, 18);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(216, 26);
            this.labelState.Text = "● idle";
            this.labelState.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // labelLead
            //
            this.labelLead.AutoSize = false;
            this.labelLead.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLead.Location = new System.Drawing.Point(24, 50);
            this.labelLead.Name = "labelLead";
            this.labelLead.Size = new System.Drawing.Size(892, 46);
            this.labelLead.Text = "Module 4 · Add and Edit join the Module 3 search. Double-click a row (or Edit ticket) to open TicketEditorForm — a modal bound through editBindingSource; Save runs EndEdit, a fresh DbContext, SaveChangesAsync, DialogResult.OK, and only OK re-runs this search.";
            this.labelLead.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelSearchCaption
            //
            this.labelSearchCaption.AutoSize = false;
            this.labelSearchCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelSearchCaption.Location = new System.Drawing.Point(24, 100);
            this.labelSearchCaption.Name = "labelSearchCaption";
            this.labelSearchCaption.Size = new System.Drawing.Size(230, 18);
            this.labelSearchCaption.Text = "Number, title or customer";
            //
            // labelStatusCaption
            //
            this.labelStatusCaption.AutoSize = false;
            this.labelStatusCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelStatusCaption.Location = new System.Drawing.Point(262, 100);
            this.labelStatusCaption.Name = "labelStatusCaption";
            this.labelStatusCaption.Size = new System.Drawing.Size(130, 18);
            this.labelStatusCaption.Text = "Status";
            //
            // labelCustomerCaption
            //
            this.labelCustomerCaption.AutoSize = false;
            this.labelCustomerCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelCustomerCaption.Location = new System.Drawing.Point(400, 100);
            this.labelCustomerCaption.Name = "labelCustomerCaption";
            this.labelCustomerCaption.Size = new System.Drawing.Size(160, 18);
            this.labelCustomerCaption.Text = "Customer";
            //
            // labelDueCaption
            //
            this.labelDueCaption.AutoSize = false;
            this.labelDueCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelDueCaption.Location = new System.Drawing.Point(568, 100);
            this.labelDueCaption.Name = "labelDueCaption";
            this.labelDueCaption.Size = new System.Drawing.Size(228, 18);
            this.labelDueCaption.Text = "Due between  (tick a box to use it)";
            //
            // searchTextBox  (the lab control)
            //
            this.searchTextBox.Location = new System.Drawing.Point(24, 120);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(230, 30);
            this.searchTextBox.Watermark = "SD-10…, part of a title or a customer";
            //
            // statusComboBox  (lookup: DisplayMember = Name, ValueMember = Status)
            //
            this.statusComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.statusComboBox.Location = new System.Drawing.Point(262, 120);
            this.statusComboBox.Name = "statusComboBox";
            this.statusComboBox.Size = new System.Drawing.Size(130, 30);
            this.statusComboBox.ToolTipText = "Filled in the page Load handler from TicketQueryService.GetStatusesAsync, before the first search. \"All statuses\" carries an empty key and becomes a null criterion.";
            //
            // customerComboBox  (lookup: DisplayMember = Name, ValueMember = Id)
            //
            this.customerComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.customerComboBox.Location = new System.Drawing.Point(400, 120);
            this.customerComboBox.Name = "customerComboBox";
            this.customerComboBox.Size = new System.Drawing.Size(160, 30);
            this.customerComboBox.ToolTipText = "Filled from TicketQueryService.GetCustomersAsync (Id + Name, no tracking). SelectedValue is a boxed int — the criteria carry the key, never the display text.";
            //
            // dueFromDateTimePicker  (unticked = no lower bound)
            //
            this.dueFromDateTimePicker.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dueFromDateTimePicker.Location = new System.Drawing.Point(568, 120);
            this.dueFromDateTimePicker.Name = "dueFromDateTimePicker";
            this.dueFromDateTimePicker.ShowCheckBox = true;
            this.dueFromDateTimePicker.Checked = false;
            this.dueFromDateTimePicker.Size = new System.Drawing.Size(110, 30);
            this.dueFromDateTimePicker.ToolTipText = "Unticked = no lower bound. Ticking it adds one Where to the query.";
            //
            // dueToDateTimePicker  (unticked = no upper bound)
            //
            this.dueToDateTimePicker.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dueToDateTimePicker.Location = new System.Drawing.Point(686, 120);
            this.dueToDateTimePicker.Name = "dueToDateTimePicker";
            this.dueToDateTimePicker.ShowCheckBox = true;
            this.dueToDateTimePicker.Checked = false;
            this.dueToDateTimePicker.Size = new System.Drawing.Size(110, 30);
            this.dueToDateTimePicker.ToolTipText = "Unticked = no upper bound. Ticking it adds one Where to the query.";
            //
            // searchButton  (the lab control — resets the page index)
            //
            this.searchButton.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.searchButton.Location = new System.Drawing.Point(804, 120);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(112, 30);
            this.searchButton.Text = "Search";
            this.searchButton.ToolTipText = "Builds a TicketSearchCriteria from the controls, resets the page index and runs one search: two statements.";
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            //
            // ticketsDataGridView  (the lab control — bound through ticketBindingSource)
            //
            this.ticketsDataGridView.AllowUserToAddRows = false;
            this.ticketsDataGridView.AllowUserToDeleteRows = false;
            this.ticketsDataGridView.AutoGenerateColumns = false;
            this.ticketsDataGridView.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.ticketsDataGridView.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colNumber,
            this.colTitle,
            this.colCustomerName,
            this.colAgentName,
            this.colCategoryName,
            this.colStatus,
            this.colPriority,
            this.colDueDate,
            this.colUpdatedAt});
            this.ticketsDataGridView.Location = new System.Drawing.Point(24, 164);
            this.ticketsDataGridView.MultiSelect = false;
            this.ticketsDataGridView.Name = "ticketsDataGridView";
            this.ticketsDataGridView.ReadOnly = true;
            this.ticketsDataGridView.RowHeadersVisible = false;
            this.ticketsDataGridView.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.ticketsDataGridView.Size = new System.Drawing.Size(892, 360);
            // The DataSource is assigned AFTER the columns exist — the columns are matched to the bound
            // properties by DataPropertyName, so they must be there when the source arrives.
            this.ticketsDataGridView.DataSource = this.ticketBindingSource;
            // Module 4: double-click a row to edit it — opens the same TicketEditorForm as btnEdit.
            this.ticketsDataGridView.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.ticketsDataGridView_CellDoubleClick);
            //
            // colNumber
            //
            this.colNumber.DataPropertyName = "Number";
            this.colNumber.FillWeight = 70F;
            this.colNumber.HeaderText = "Number";
            this.colNumber.Name = "colNumber";
            this.colNumber.Width = 70;
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.FillWeight = 200F;
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            this.colTitle.Width = 200;
            //
            // colCustomerName
            //
            this.colCustomerName.DataPropertyName = "CustomerName";
            this.colCustomerName.FillWeight = 130F;
            this.colCustomerName.HeaderText = "Customer";
            this.colCustomerName.Name = "colCustomerName";
            this.colCustomerName.Width = 130;
            //
            // colAgentName  (null when the ticket is unassigned)
            //
            this.colAgentName.DataPropertyName = "AgentName";
            this.colAgentName.DefaultCellStyle.NullValue = "— unassigned —";
            this.colAgentName.FillWeight = 120F;
            this.colAgentName.HeaderText = "Agent";
            this.colAgentName.Name = "colAgentName";
            this.colAgentName.Width = 120;
            //
            // colCategoryName
            //
            this.colCategoryName.DataPropertyName = "CategoryName";
            this.colCategoryName.FillWeight = 120F;
            this.colCategoryName.HeaderText = "Category";
            this.colCategoryName.Name = "colCategoryName";
            this.colCategoryName.Width = 120;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.FillWeight = 80F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 80;
            //
            // colPriority
            //
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.FillWeight = 60F;
            this.colPriority.HeaderText = "Priority";
            this.colPriority.Name = "colPriority";
            this.colPriority.Width = 60;
            //
            // colDueDate
            //
            this.colDueDate.DataPropertyName = "DueDate";
            this.colDueDate.DefaultCellStyle.Format = "yyyy-MM-dd";
            this.colDueDate.DefaultCellStyle.NullValue = "";
            this.colDueDate.FillWeight = 80F;
            this.colDueDate.HeaderText = "Due";
            this.colDueDate.Name = "colDueDate";
            this.colDueDate.Width = 80;
            //
            // colUpdatedAt
            //
            this.colUpdatedAt.DataPropertyName = "UpdatedAt";
            this.colUpdatedAt.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            this.colUpdatedAt.FillWeight = 110F;
            this.colUpdatedAt.HeaderText = "Updated";
            this.colUpdatedAt.Name = "colUpdatedAt";
            this.colUpdatedAt.Width = 110;
            //
            // statusLabel  (the lab control: returned count, total count, page and page size)
            //
            this.statusLabel.AutoSize = false;
            this.statusLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.statusLabel.Location = new System.Drawing.Point(24, 532);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(600, 28);
            this.statusLabel.Text = "Loading the lookups…";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // prevPageButton  (the lab control — disabled on page 1)
            //
            this.prevPageButton.Location = new System.Drawing.Point(652, 530);
            this.prevPageButton.Name = "prevPageButton";
            this.prevPageButton.Size = new System.Drawing.Size(128, 32);
            this.prevPageButton.Text = "◀ Previous page";
            this.prevPageButton.ToolTipText = "One new query with a smaller OFFSET — never a cached copy of the whole result set.";
            this.prevPageButton.Click += new System.EventHandler(this.prevPageButton_Click);
            //
            // nextPageButton  (the lab control — disabled on the last page)
            //
            this.nextPageButton.Location = new System.Drawing.Point(788, 530);
            this.nextPageButton.Name = "nextPageButton";
            this.nextPageButton.Size = new System.Drawing.Size(128, 32);
            this.nextPageButton.Text = "Next page ▶";
            this.nextPageButton.ToolTipText = "One new query with a larger OFFSET — never a cached copy of the whole result set.";
            this.nextPageButton.Click += new System.EventHandler(this.nextPageButton_Click);
            //
            // labelBanner  (friendly error message)
            //
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(24, 568);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(892, 44);
            this.labelBanner.Text = "";
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // panelTrace  (Server ⇄ Database trace)
            //
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Controls.Add(this.labelTraceFooter);
            this.panelTrace.Location = new System.Drawing.Point(986, 30);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(684, 640);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(644, 30);
            this.labelTraceTitle.Text = "Server ⇄ Database  ·  EF Core lifetime & SQL trace";
            //
            // listTrace
            //
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(644, 536);
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 598);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(644, 26);
            this.labelTraceFooter.Text = "• server   ◦ context created / disposed   → SQL sent to the database (ms)   ← result back in the handler";
            //
            // panelModel  (the Module 2 card and the Module 1 lifetimes table, kept working under the browser)
            //
            this.panelModel.BackColor = System.Drawing.Color.White;
            this.panelModel.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelModel.Controls.Add(this.labelModelTitle);
            this.panelModel.Controls.Add(this.labelModel);
            this.panelModel.Controls.Add(this.labelLifetimesTitle);
            this.panelModel.Controls.Add(this.labelLifetimes);
            this.panelModel.Controls.Add(this.labelRule);
            this.panelModel.Location = new System.Drawing.Point(30, 686);
            this.panelModel.Name = "panelModel";
            this.panelModel.Size = new System.Drawing.Size(1640, 260);
            //
            // labelModelTitle
            //
            this.labelModelTitle.AutoSize = false;
            this.labelModelTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelModelTitle.Location = new System.Drawing.Point(24, 14);
            this.labelModelTitle.Name = "labelModelTitle";
            this.labelModelTitle.Size = new System.Drawing.Size(900, 24);
            this.labelModelTitle.Text = "Model & migration (Module 2) — read from the database and the model after every operation";
            //
            // labelModel  (monospace table: row counts, migrations, indexes, delete behaviours, checks)
            //
            this.labelModel.AllowHtml = true;
            this.labelModel.AutoSize = false;
            this.labelModel.Font = new System.Drawing.Font("monospace", 9F);
            this.labelModel.ForeColor = System.Drawing.Color.FromArgb(60, 72, 88);
            this.labelModel.Location = new System.Drawing.Point(24, 42);
            this.labelModel.Name = "labelModel";
            this.labelModel.Size = new System.Drawing.Size(900, 204);
            this.labelModel.Text = "Reading the schema…";
            this.labelModel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelLifetimesTitle
            //
            this.labelLifetimesTitle.AutoSize = false;
            this.labelLifetimesTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelLifetimesTitle.Location = new System.Drawing.Point(952, 14);
            this.labelLifetimesTitle.Name = "labelLifetimesTitle";
            this.labelLifetimesTitle.Size = new System.Drawing.Size(660, 24);
            this.labelLifetimesTitle.Text = "Four lifetimes on one server — what this session owns right now (Module 1)";
            //
            // labelLifetimes  (monospace table)
            //
            this.labelLifetimes.AllowHtml = true;
            this.labelLifetimes.AutoSize = false;
            this.labelLifetimes.Font = new System.Drawing.Font("monospace", 9F);
            this.labelLifetimes.ForeColor = System.Drawing.Color.FromArgb(60, 72, 88);
            this.labelLifetimes.Location = new System.Drawing.Point(952, 42);
            this.labelLifetimes.Name = "labelLifetimes";
            this.labelLifetimes.Size = new System.Drawing.Size(660, 100);
            this.labelLifetimes.Text = "";
            this.labelLifetimes.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelRule
            //
            this.labelRule.AutoSize = false;
            this.labelRule.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelRule.Location = new System.Drawing.Point(952, 150);
            this.labelRule.Name = "labelRule";
            this.labelRule.Size = new System.Drawing.Size(660, 96);
            this.labelRule.Text = "Rule: the page, the BindingSource and the selected row live for the session; a DbContext lives for one operation. The grid holds a list of TicketListItem records — not entities, not a query — so nothing shown here can be written back by accident.";
            this.labelRule.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelActions  (bottom bar, row 1: the Module 4 paths — the newest module leads)
            //
            this.panelActions.Controls.Add(this.labelActions0);
            this.panelActions.Controls.Add(this.btnAdd);
            this.panelActions.Controls.Add(this.btnEdit);
            this.panelActions.Controls.Add(this.buttonSlowSave);
            this.panelActions.Controls.Add(this.buttonSimulateDelete);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 962);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1640, 44);
            //
            // labelActions0
            //
            this.labelActions0.AutoSize = false;
            this.labelActions0.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelActions0.Location = new System.Drawing.Point(0, 4);
            this.labelActions0.Name = "labelActions0";
            this.labelActions0.Size = new System.Drawing.Size(130, 36);
            this.labelActions0.Text = "Module 4 · editor:";
            this.labelActions0.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnAdd  (the lab control — success path)
            //
            this.btnAdd.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.Location = new System.Drawing.Point(134, 4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(120, 36);
            this.btnAdd.Text = "Add ticket";
            this.btnAdd.ToolTipText = "Opens TicketEditorForm with ticketId = null: a blank TicketEditModel, Number assigned by SaveAsync.";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // btnEdit  (the lab control — success path)
            //
            this.btnEdit.Location = new System.Drawing.Point(262, 4);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(120, 36);
            this.btnEdit.Text = "Edit ticket";
            this.btnEdit.ToolTipText = "Opens TicketEditorForm on the selected grid row. Double-clicking a row does the same.";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            //
            // buttonSlowSave  (progress path: a toggle armed for the next Add/Edit dialog)
            //
            this.buttonSlowSave.Location = new System.Drawing.Point(390, 4);
            this.buttonSlowSave.Name = "buttonSlowSave";
            this.buttonSlowSave.Size = new System.Drawing.Size(190, 36);
            this.buttonSlowSave.Text = "Slow save (2.5 s): off";
            this.buttonSlowSave.ToolTipText = "Arms a 2.5 s delay inside TicketCommandService.SaveAsync's unit of work for the next dialog — click Save twice to see the guard.";
            this.buttonSlowSave.Click += new System.EventHandler(this.buttonSlowSave_Click);
            //
            // buttonSimulateDelete  (failure path)
            //
            this.buttonSimulateDelete.Location = new System.Drawing.Point(588, 4);
            this.buttonSimulateDelete.Name = "buttonSimulateDelete";
            this.buttonSimulateDelete.Size = new System.Drawing.Size(310, 36);
            this.buttonSimulateDelete.Text = "Simulate: another operator deletes it";
            this.buttonSimulateDelete.ToolTipText = "Deletes the selected grid row through TicketCommandService directly, bypassing the editor. Open Edit on the same row afterwards to see the already-deleted handling.";
            this.buttonSimulateDelete.Click += new System.EventHandler(this.buttonSimulateDelete_Click);
            //
            // buttonClear  (moved here — row 1 is always the newest module's row)
            //
            this.buttonClear.Location = new System.Drawing.Point(1530, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(110, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // panelActions2  (bottom bar, row 2: the Module 3 paths — buttonBreak/buttonRestore also
            // govern the editor's Save and Delete, since TicketCommandService shares the same
            // DevelopmentOutageSwitch as TicketQueryService)
            //
            this.panelActions2.Controls.Add(this.labelActions1);
            this.panelActions2.Controls.Add(this.buttonSlowSearch);
            this.panelActions2.Controls.Add(this.buttonBreak);
            this.panelActions2.Controls.Add(this.buttonRestore);
            this.panelActions2.Controls.Add(this.buttonBindQuery);
            this.panelActions2.Location = new System.Drawing.Point(30, 1012);
            this.panelActions2.Name = "panelActions2";
            this.panelActions2.Size = new System.Drawing.Size(1640, 44);
            //
            // labelActions1
            //
            this.labelActions1.AutoSize = false;
            this.labelActions1.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelActions1.Location = new System.Drawing.Point(0, 4);
            this.labelActions1.Name = "labelActions1";
            this.labelActions1.Size = new System.Drawing.Size(130, 36);
            this.labelActions1.Text = "Module 3 · browser:";
            this.labelActions1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonSlowSearch  (progress path: the loading guard)
            //
            this.buttonSlowSearch.Location = new System.Drawing.Point(134, 4);
            this.buttonSlowSearch.Name = "buttonSlowSearch";
            this.buttonSlowSearch.Size = new System.Drawing.Size(170, 36);
            this.buttonSlowSearch.Text = "Slow search (2.5 s)";
            this.buttonSlowSearch.ToolTipText = "SearchTicketsSlowlyAsync: click Search while it runs and watch the guard ignore the click.";
            this.buttonSlowSearch.Click += new System.EventHandler(this.buttonSlowSearch_Click);
            //
            // buttonBreak  (failure path — also breaks Save/Delete inside the editor)
            //
            this.buttonBreak.Location = new System.Drawing.Point(312, 4);
            this.buttonBreak.Name = "buttonBreak";
            this.buttonBreak.Size = new System.Drawing.Size(180, 36);
            this.buttonBreak.Text = "Break the database";
            this.buttonBreak.ToolTipText = "Turns the development outage switch on. Affects the search AND the editor's Save/Delete — try it, then open Edit and click Save.";
            this.buttonBreak.Click += new System.EventHandler(this.buttonBreak_Click);
            //
            // buttonRestore  (recovery)
            //
            this.buttonRestore.Location = new System.Drawing.Point(500, 4);
            this.buttonRestore.Name = "buttonRestore";
            this.buttonRestore.Size = new System.Drawing.Size(180, 36);
            this.buttonRestore.Text = "Restore and search";
            this.buttonRestore.ToolTipText = "Turns the switch off. The next search — or the next Save/Delete in the editor — gets a fresh context from the factory.";
            this.buttonRestore.Click += new System.EventHandler(this.buttonRestore_Click);
            //
            // buttonBindQuery  (anti-pattern: the query bound to the grid)
            //
            this.buttonBindQuery.Location = new System.Drawing.Point(688, 4);
            this.buttonBindQuery.Name = "buttonBindQuery";
            this.buttonBindQuery.Size = new System.Drawing.Size(260, 36);
            this.buttonBindQuery.Text = "Bind IQueryable (anti-pattern)";
            this.buttonBindQuery.ToolTipText = "Assigns the query instead of the list, disposes the context the way the handler would, then enumerates it the way the grid would.";
            this.buttonBindQuery.Click += new System.EventHandler(this.buttonBindQuery_Click);
            //
            // panelActions3  (bottom bar, row 3: the Module 2 paths)
            //
            this.panelActions3.Controls.Add(this.labelActions2);
            this.panelActions3.Controls.Add(this.btnSeed);
            this.panelActions3.Controls.Add(this.buttonReset);
            this.panelActions3.Controls.Add(this.buttonOverlongTitle);
            this.panelActions3.Controls.Add(this.buttonDeleteCustomer);
            this.panelActions3.Controls.Add(this.buttonDeleteAgent);
            this.panelActions3.Controls.Add(this.buttonDeleteTicket);
            this.panelActions3.Location = new System.Drawing.Point(30, 1062);
            this.panelActions3.Name = "panelActions3";
            this.panelActions3.Size = new System.Drawing.Size(1640, 44);
            //
            // labelActions2
            //
            this.labelActions2.AutoSize = false;
            this.labelActions2.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelActions2.Location = new System.Drawing.Point(0, 4);
            this.labelActions2.Name = "labelActions2";
            this.labelActions2.Size = new System.Drawing.Size(130, 36);
            this.labelActions2.Text = "Module 2 · model:";
            this.labelActions2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnSeed  (the Module 2 lab control)
            //
            this.btnSeed.Location = new System.Drawing.Point(134, 4);
            this.btnSeed.Name = "btnSeed";
            this.btnSeed.Size = new System.Drawing.Size(200, 36);
            this.btnSeed.Text = "Seed development data";
            this.btnSeed.ToolTipText = "DevelopmentSeeder.SeedDevelopmentDataAsync: one context, AnyAsync, AddRange, SaveChangesAsync. Click it twice — the second run seeds nothing.";
            this.btnSeed.Click += new System.EventHandler(this.btnSeed_Click);
            //
            // buttonReset  (lab prop: run the delete demos again)
            //
            this.buttonReset.Location = new System.Drawing.Point(342, 4);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(150, 36);
            this.buttonReset.Text = "Reset & reseed";
            this.buttonReset.ToolTipText = "Empties every table (children first) and seeds again, so the delete demos can be repeated. Development only.";
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            //
            // buttonOverlongTitle  (failure path: the CHECK constraint)
            //
            this.buttonOverlongTitle.Location = new System.Drawing.Point(500, 4);
            this.buttonOverlongTitle.Name = "buttonOverlongTitle";
            this.buttonOverlongTitle.Size = new System.Drawing.Size(220, 36);
            this.buttonOverlongTitle.Text = "Save a 200-char title (fails)";
            this.buttonOverlongTitle.ToolTipText = "Inserts a ticket with a 200-character title. SQLite ignores HasMaxLength(180); the CHECK constraint refuses it → DbUpdateException → friendly banner.";
            this.buttonOverlongTitle.Click += new System.EventHandler(this.buttonOverlongTitle_Click);
            //
            // buttonDeleteCustomer  (failure path: Restrict)
            //
            this.buttonDeleteCustomer.Location = new System.Drawing.Point(728, 4);
            this.buttonDeleteCustomer.Name = "buttonDeleteCustomer";
            this.buttonDeleteCustomer.Size = new System.Drawing.Size(280, 36);
            this.buttonDeleteCustomer.Text = "Delete a customer with tickets (refused)";
            this.buttonDeleteCustomer.ToolTipText = "Removes a customer that still has tickets. ON DELETE RESTRICT refuses the DELETE → DbUpdateException → friendly banner.";
            this.buttonDeleteCustomer.Click += new System.EventHandler(this.buttonDeleteCustomer_Click);
            //
            // buttonDeleteAgent  (SetNull)
            //
            this.buttonDeleteAgent.Location = new System.Drawing.Point(1016, 4);
            this.buttonDeleteAgent.Name = "buttonDeleteAgent";
            this.buttonDeleteAgent.Size = new System.Drawing.Size(210, 36);
            this.buttonDeleteAgent.Text = "Unassign an agent (SetNull)";
            this.buttonDeleteAgent.ToolTipText = "Deletes an agent that owns tickets. ON DELETE SET NULL clears AgentId on their tickets — the trace shows the unassigned count before and after.";
            this.buttonDeleteAgent.Click += new System.EventHandler(this.buttonDeleteAgent_Click);
            //
            // buttonDeleteTicket  (Cascade)
            //
            this.buttonDeleteTicket.Location = new System.Drawing.Point(1234, 4);
            this.buttonDeleteTicket.Name = "buttonDeleteTicket";
            this.buttonDeleteTicket.Size = new System.Drawing.Size(270, 36);
            this.buttonDeleteTicket.Text = "Delete a ticket with comments (Cascade)";
            this.buttonDeleteTicket.ToolTipText = "Deletes a ticket that has comments. ON DELETE CASCADE removes the comments — the trace shows the comment count before and after.";
            this.buttonDeleteTicket.Click += new System.EventHandler(this.buttonDeleteTicket_Click);
            //
            // panelActions4  (bottom bar, row 4: the Module 1 paths)
            //
            this.panelActions4.Controls.Add(this.labelActions3);
            this.panelActions4.Controls.Add(this.countButton);
            this.panelActions4.Controls.Add(this.buttonSlowCount);
            this.panelActions4.Controls.Add(this.buttonRapid);
            this.panelActions4.Controls.Add(this.buttonAntiPattern);
            this.panelActions4.Location = new System.Drawing.Point(30, 1112);
            this.panelActions4.Name = "panelActions4";
            this.panelActions4.Size = new System.Drawing.Size(1640, 44);
            //
            // labelActions3
            //
            this.labelActions3.AutoSize = false;
            this.labelActions3.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelActions3.Location = new System.Drawing.Point(0, 4);
            this.labelActions3.Name = "labelActions3";
            this.labelActions3.Size = new System.Drawing.Size(130, 36);
            this.labelActions3.Text = "Module 1 · lifetimes:";
            this.labelActions3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // countButton  (the Module 1 lab control)
            //
            this.countButton.Location = new System.Drawing.Point(134, 4);
            this.countButton.Name = "countButton";
            this.countButton.Size = new System.Drawing.Size(150, 36);
            this.countButton.Text = "Count tickets";
            this.countButton.ToolTipText = "TicketQueryService.CountTicketsAsync: one context, one COUNT, disposed before the handler returns.";
            this.countButton.Click += new System.EventHandler(this.countButton_Click);
            //
            // buttonSlowCount  (progress path: the loading guard)
            //
            this.buttonSlowCount.Location = new System.Drawing.Point(292, 4);
            this.buttonSlowCount.Name = "buttonSlowCount";
            this.buttonSlowCount.Size = new System.Drawing.Size(170, 36);
            this.buttonSlowCount.Text = "Slow count (2.5 s)";
            this.buttonSlowCount.ToolTipText = "CountTicketsSlowlyAsync: click Count tickets while it runs and watch the guard ignore the click.";
            this.buttonSlowCount.Click += new System.EventHandler(this.buttonSlowCount_Click);
            //
            // buttonRapid
            //
            this.buttonRapid.Location = new System.Drawing.Point(470, 4);
            this.buttonRapid.Name = "buttonRapid";
            this.buttonRapid.Size = new System.Drawing.Size(190, 36);
            this.buttonRapid.Text = "▶ Count ×3 rapid (guard)";
            this.buttonRapid.ToolTipText = "Three counts started at once: the first runs, the loading flag drops the other two.";
            this.buttonRapid.Click += new System.EventHandler(this.buttonRapid_Click);
            //
            // buttonAntiPattern  (what a shared context does)
            //
            this.buttonAntiPattern.Location = new System.Drawing.Point(668, 4);
            this.buttonAntiPattern.Name = "buttonAntiPattern";
            this.buttonAntiPattern.Size = new System.Drawing.Size(248, 36);
            this.buttonAntiPattern.Text = "Two ops, one context (anti-pattern)";
            this.buttonAntiPattern.ToolTipText = "Two concurrent queries on one DbContext instance — what a static/shared context does with two sessions.";
            this.buttonAntiPattern.Click += new System.EventHandler(this.buttonAntiPattern_Click);
            //
            // TicketBrowserPage
            //
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelBrowser);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelModel);
            this.Controls.Add(this.panelActions);
            this.Controls.Add(this.panelActions2);
            this.Controls.Add(this.panelActions3);
            this.Controls.Add(this.panelActions4);
            this.Name = "TicketBrowserPage";
            this.Size = new System.Drawing.Size(1700, 1190);
            this.Text = "Support Desk Data Console — Module 4";
            this.Load += new System.EventHandler(this.TicketBrowserPage_Load);
            this.panelBrowser.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelModel.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.panelActions2.ResumeLayout(false);
            this.panelActions3.ResumeLayout(false);
            this.panelActions4.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.BindingSource ticketBindingSource;
        private Wisej.Web.Panel panelBrowser;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Label labelLead;
        private Wisej.Web.Label labelSearchCaption;
        private Wisej.Web.Label labelStatusCaption;
        private Wisej.Web.Label labelCustomerCaption;
        private Wisej.Web.Label labelDueCaption;
        private Wisej.Web.TextBox searchTextBox;
        private Wisej.Web.ComboBox statusComboBox;
        private Wisej.Web.ComboBox customerComboBox;
        private Wisej.Web.DateTimePicker dueFromDateTimePicker;
        private Wisej.Web.DateTimePicker dueToDateTimePicker;
        private Wisej.Web.Button searchButton;
        private Wisej.Web.DataGridView ticketsDataGridView;
        private Wisej.Web.DataGridViewTextBoxColumn colNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomerName;
        private Wisej.Web.DataGridViewTextBoxColumn colAgentName;
        private Wisej.Web.DataGridViewTextBoxColumn colCategoryName;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colDueDate;
        private Wisej.Web.DataGridViewTextBoxColumn colUpdatedAt;
        private Wisej.Web.Label statusLabel;
        private Wisej.Web.Button prevPageButton;
        private Wisej.Web.Button nextPageButton;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelModel;
        private Wisej.Web.Label labelModelTitle;
        private Wisej.Web.Label labelModel;
        private Wisej.Web.Label labelLifetimesTitle;
        private Wisej.Web.Label labelLifetimes;
        private Wisej.Web.Label labelRule;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Label labelActions0;
        private Wisej.Web.Button btnAdd;
        private Wisej.Web.Button btnEdit;
        private Wisej.Web.Button buttonSlowSave;
        private Wisej.Web.Button buttonSimulateDelete;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Panel panelActions2;
        private Wisej.Web.Label labelActions1;
        private Wisej.Web.Button buttonSlowSearch;
        private Wisej.Web.Button buttonBreak;
        private Wisej.Web.Button buttonRestore;
        private Wisej.Web.Button buttonBindQuery;
        private Wisej.Web.Panel panelActions3;
        private Wisej.Web.Label labelActions2;
        private Wisej.Web.Button btnSeed;
        private Wisej.Web.Button buttonReset;
        private Wisej.Web.Button buttonOverlongTitle;
        private Wisej.Web.Button buttonDeleteCustomer;
        private Wisej.Web.Button buttonDeleteAgent;
        private Wisej.Web.Button buttonDeleteTicket;
        private Wisej.Web.Panel panelActions4;
        private Wisej.Web.Label labelActions3;
        private Wisej.Web.Button countButton;
        private Wisej.Web.Button buttonSlowCount;
        private Wisej.Web.Button buttonRapid;
        private Wisej.Web.Button buttonAntiPattern;
    }
}
