namespace WisejTrainingApp
{
    partial class TicketsWindow
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
            this.panelHeader = new Wisej.Web.Panel();
            this.lblPageTitle = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.splitContainer1 = new Wisej.Web.SplitContainer();
            this.dgvTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssignedTo = new Wisej.Web.DataGridViewTextBoxColumn();
            this.panelState = new Wisej.Web.Panel();
            this.panelStateCard = new Wisej.Web.Panel();
            this.labelStateCard = new Wisej.Web.Label();
            this.lblState = new Wisej.Web.Label();
            this.btnAddSample = new Wisej.Web.Button();
            this.grpTicketDetails = new Wisej.Web.GroupBox();
            this.lblTitleCaption = new Wisej.Web.Label();
            this.txtTitle = new Wisej.Web.TextBox();
            this.lblCustomerCaption = new Wisej.Web.Label();
            this.txtCustomer = new Wisej.Web.TextBox();
            this.lblAssignedToCaption = new Wisej.Web.Label();
            this.txtAssignedTo = new Wisej.Web.TextBox();
            this.lblStatusCaption = new Wisej.Web.Label();
            this.cmbStatus = new Wisej.Web.ComboBox();
            this.lblPriorityCaption = new Wisej.Web.Label();
            this.cmbPriority = new Wisej.Web.ComboBox();
            this.lblCreatedCaption = new Wisej.Web.Label();
            this.dtpCreated = new Wisej.Web.DateTimePicker();
            this.lblDescriptionCaption = new Wisej.Web.Label();
            this.txtDescription = new Wisej.Web.TextBox();
            this.btnSaveTicket = new Wisej.Web.Button();
            this.btnRefresh = new Wisej.Web.Button();
            this.btnSimulateBadSave = new Wisej.Web.Button();
            this.panelLog = new Wisej.Web.Panel();
            this.panelLogCard = new Wisej.Web.Panel();
            this.labelLogCard = new Wisej.Web.Label();
            this.btnClearLog = new Wisej.Web.Button();
            this.lstEventLog = new Wisej.Web.ListBox();
            this.labelLogFooter = new Wisej.Web.Label();
            this.panelHeader.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelState.SuspendLayout();
            this.panelStateCard.SuspendLayout();
            this.grpTicketDetails.SuspendLayout();
            this.panelLog.SuspendLayout();
            this.panelLogCard.SuspendLayout();
            this.SuspendLayout();
            //
            // panelHeader  (Dock = Top: the page title on the left, the status label on the right)
            //
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelHeader.Controls.Add(this.lblPageTitle);
            this.panelHeader.Controls.Add(this.lblStatus);
            this.panelHeader.Dock = Wisej.Web.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1348, 56);
            //
            // lblPageTitle
            //
            this.lblPageTitle.AutoSize = false;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(24, 10);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(560, 36);
            this.lblPageTitle.Text = "Ticket Management";
            this.lblPageTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStatus  (right-aligned; text and colour change with every action)
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(624, 14);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(700, 28);
            this.lblStatus.Text = "● ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // splitContainer1  (Dock = Fill, vertical: grid on the left, details + log on the right)
            //
            this.splitContainer1.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.splitContainer1.Dock = Wisej.Web.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 56);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = Wisej.Web.Orientation.Vertical;
            //
            // splitContainer1.Panel1  (the ticket list — the large side)
            //
            this.splitContainer1.Panel1.Controls.Add(this.dgvTickets);
            this.splitContainer1.Panel1.Controls.Add(this.panelState);
            this.splitContainer1.Panel1.Padding = new Wisej.Web.Padding(24, 16, 12, 24);
            //
            // splitContainer1.Panel2  (the detail editor and the event log)
            //
            this.splitContainer1.Panel2.Controls.Add(this.panelLog);
            this.splitContainer1.Panel2.Controls.Add(this.grpTicketDetails);
            this.splitContainer1.Panel2.Padding = new Wisej.Web.Padding(12, 16, 24, 24);
            this.splitContainer1.Size = new System.Drawing.Size(1348, 624);
            this.splitContainer1.SplitterDistance = 760;
            //
            // dgvTickets  (Dock = Fill inside Panel1; explicit business columns, whole-row selection, read-only)
            //
            this.dgvTickets.AllowUserToAddRows = false;
            this.dgvTickets.AllowUserToDeleteRows = false;
            this.dgvTickets.AutoGenerateColumns = false;
            this.dgvTickets.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTickets.BackColor = System.Drawing.Color.White;
            this.dgvTickets.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colId,
            this.colTitle,
            this.colCustomer,
            this.colStatus,
            this.colPriority,
            this.colAssignedTo});
            this.dgvTickets.Dock = Wisej.Web.DockStyle.Fill;
            this.dgvTickets.Location = new System.Drawing.Point(24, 16);
            this.dgvTickets.MultiSelect = false;
            this.dgvTickets.Name = "dgvTickets";
            this.dgvTickets.ReadOnly = true;
            this.dgvTickets.RowHeadersVisible = false;
            this.dgvTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTickets.Size = new System.Drawing.Size(724, 480);
            //
            // colId
            //
            this.colId.DataPropertyName = "Id";
            this.colId.FillWeight = 8F;
            this.colId.HeaderText = "Id";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.FillWeight = 34F;
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            //
            // colCustomer
            //
            this.colCustomer.DataPropertyName = "Customer";
            this.colCustomer.FillWeight = 18F;
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.Name = "colCustomer";
            this.colCustomer.ReadOnly = true;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.FillWeight = 13F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            //
            // colPriority
            //
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.FillWeight = 10F;
            this.colPriority.HeaderText = "Priority";
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            //
            // colAssignedTo
            //
            this.colAssignedTo.DataPropertyName = "AssignedTo";
            this.colAssignedTo.FillWeight = 17F;
            this.colAssignedTo.HeaderText = "Assigned To";
            this.colAssignedTo.Name = "colAssignedTo";
            this.colAssignedTo.ReadOnly = true;
            //
            // panelState  (Dock = Bottom under the grid: the three state types, lesson s16 §7)
            //
            this.panelState.Controls.Add(this.panelStateCard);
            this.panelState.Dock = Wisej.Web.DockStyle.Bottom;
            this.panelState.Location = new System.Drawing.Point(24, 496);
            this.panelState.Name = "panelState";
            this.panelState.Size = new System.Drawing.Size(724, 104);
            //
            // panelStateCard
            //
            this.panelStateCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelStateCard.BackColor = System.Drawing.Color.White;
            this.panelStateCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelStateCard.Controls.Add(this.labelStateCard);
            this.panelStateCard.Controls.Add(this.btnAddSample);
            this.panelStateCard.Controls.Add(this.lblState);
            this.panelStateCard.Location = new System.Drawing.Point(0, 12);
            this.panelStateCard.Name = "panelStateCard";
            this.panelStateCard.Size = new System.Drawing.Size(724, 92);
            //
            // labelStateCard
            //
            this.labelStateCard.AutoSize = false;
            this.labelStateCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelStateCard.Location = new System.Drawing.Point(20, 8);
            this.labelStateCard.Name = "labelStateCard";
            this.labelStateCard.Size = new System.Drawing.Size(420, 26);
            this.labelStateCard.Text = "State  ·  UI state vs business state vs persisted data";
            //
            // btnAddSample  (a change the service makes that the screen cannot know about until Refresh)
            //
            this.btnAddSample.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnAddSample.Location = new System.Drawing.Point(474, 6);
            this.btnAddSample.Name = "btnAddSample";
            this.btnAddSample.Size = new System.Drawing.Size(230, 30);
            this.btnAddSample.Text = "Add a ticket through the service";
            this.btnAddSample.ToolTipText = "ticketService.AddTicket(...) only — the grid keeps showing its bound list until you click Refresh.";
            this.btnAddSample.Click += new System.EventHandler(this.btnAddSample_Click);
            //
            // lblState  (monospace, three lines, updated after every action)
            //
            this.lblState.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblState.AutoSize = false;
            this.lblState.Font = new System.Drawing.Font("monospace", 9F);
            this.lblState.ForeColor = System.Drawing.Color.FromArgb(52, 64, 78);
            this.lblState.Location = new System.Drawing.Point(20, 36);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(684, 50);
            this.lblState.Text = "UI state       : …\nBusiness state : …\nPersisted data : …";
            this.lblState.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // grpTicketDetails  (Dock = Top inside Panel2: the detail editor — the lab's bound controls)
            //
            this.grpTicketDetails.BackColor = System.Drawing.Color.White;
            this.grpTicketDetails.Controls.Add(this.lblTitleCaption);
            this.grpTicketDetails.Controls.Add(this.txtTitle);
            this.grpTicketDetails.Controls.Add(this.lblCustomerCaption);
            this.grpTicketDetails.Controls.Add(this.txtCustomer);
            this.grpTicketDetails.Controls.Add(this.lblAssignedToCaption);
            this.grpTicketDetails.Controls.Add(this.txtAssignedTo);
            this.grpTicketDetails.Controls.Add(this.lblStatusCaption);
            this.grpTicketDetails.Controls.Add(this.cmbStatus);
            this.grpTicketDetails.Controls.Add(this.lblPriorityCaption);
            this.grpTicketDetails.Controls.Add(this.cmbPriority);
            this.grpTicketDetails.Controls.Add(this.lblCreatedCaption);
            this.grpTicketDetails.Controls.Add(this.dtpCreated);
            this.grpTicketDetails.Controls.Add(this.lblDescriptionCaption);
            this.grpTicketDetails.Controls.Add(this.txtDescription);
            this.grpTicketDetails.Controls.Add(this.btnSaveTicket);
            this.grpTicketDetails.Controls.Add(this.btnRefresh);
            this.grpTicketDetails.Controls.Add(this.btnSimulateBadSave);
            this.grpTicketDetails.Dock = Wisej.Web.DockStyle.Top;
            this.grpTicketDetails.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.grpTicketDetails.Location = new System.Drawing.Point(12, 16);
            this.grpTicketDetails.Name = "grpTicketDetails";
            this.grpTicketDetails.Size = new System.Drawing.Size(548, 352);
            this.grpTicketDetails.Text = "Ticket details  ·  bound to ticketsBindingSource";
            //
            // lblTitleCaption
            //
            this.lblTitleCaption.AutoSize = true;
            this.lblTitleCaption.Font = new System.Drawing.Font("default", 9F);
            this.lblTitleCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTitleCaption.Location = new System.Drawing.Point(16, 30);
            this.lblTitleCaption.Name = "lblTitleCaption";
            this.lblTitleCaption.Size = new System.Drawing.Size(40, 18);
            this.lblTitleCaption.Text = "Title";
            //
            // txtTitle
            //
            this.txtTitle.Font = new System.Drawing.Font("default", 10F);
            this.txtTitle.Location = new System.Drawing.Point(16, 50);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(516, 32);
            this.txtTitle.Watermark = "Required";
            //
            // lblCustomerCaption
            //
            this.lblCustomerCaption.AutoSize = true;
            this.lblCustomerCaption.Font = new System.Drawing.Font("default", 9F);
            this.lblCustomerCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCustomerCaption.Location = new System.Drawing.Point(16, 92);
            this.lblCustomerCaption.Name = "lblCustomerCaption";
            this.lblCustomerCaption.Size = new System.Drawing.Size(70, 18);
            this.lblCustomerCaption.Text = "Customer";
            //
            // txtCustomer
            //
            this.txtCustomer.Font = new System.Drawing.Font("default", 10F);
            this.txtCustomer.Location = new System.Drawing.Point(16, 112);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.Size = new System.Drawing.Size(250, 32);
            //
            // lblAssignedToCaption
            //
            this.lblAssignedToCaption.AutoSize = true;
            this.lblAssignedToCaption.Font = new System.Drawing.Font("default", 9F);
            this.lblAssignedToCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblAssignedToCaption.Location = new System.Drawing.Point(282, 92);
            this.lblAssignedToCaption.Name = "lblAssignedToCaption";
            this.lblAssignedToCaption.Size = new System.Drawing.Size(80, 18);
            this.lblAssignedToCaption.Text = "Assigned to";
            //
            // txtAssignedTo
            //
            this.txtAssignedTo.Font = new System.Drawing.Font("default", 10F);
            this.txtAssignedTo.Location = new System.Drawing.Point(282, 112);
            this.txtAssignedTo.Name = "txtAssignedTo";
            this.txtAssignedTo.Size = new System.Drawing.Size(250, 32);
            //
            // lblStatusCaption
            //
            this.lblStatusCaption.AutoSize = true;
            this.lblStatusCaption.Font = new System.Drawing.Font("default", 9F);
            this.lblStatusCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStatusCaption.Location = new System.Drawing.Point(16, 154);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(50, 18);
            this.lblStatusCaption.Text = "Status";
            //
            // cmbStatus  (DropDownList: only Open / In Progress / Closed can be chosen)
            //
            this.cmbStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbStatus.Font = new System.Drawing.Font("default", 10F);
            this.cmbStatus.Items.AddRange(new object[] {
            "Open",
            "In Progress",
            "Closed"});
            this.cmbStatus.Location = new System.Drawing.Point(16, 174);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(164, 32);
            //
            // lblPriorityCaption
            //
            this.lblPriorityCaption.AutoSize = true;
            this.lblPriorityCaption.Font = new System.Drawing.Font("default", 9F);
            this.lblPriorityCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblPriorityCaption.Location = new System.Drawing.Point(192, 154);
            this.lblPriorityCaption.Name = "lblPriorityCaption";
            this.lblPriorityCaption.Size = new System.Drawing.Size(50, 18);
            this.lblPriorityCaption.Text = "Priority";
            //
            // cmbPriority
            //
            this.cmbPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbPriority.Font = new System.Drawing.Font("default", 10F);
            this.cmbPriority.Items.AddRange(new object[] {
            "Low",
            "Medium",
            "High"});
            this.cmbPriority.Location = new System.Drawing.Point(192, 174);
            this.cmbPriority.Name = "cmbPriority";
            this.cmbPriority.Size = new System.Drawing.Size(164, 32);
            //
            // lblCreatedCaption
            //
            this.lblCreatedCaption.AutoSize = true;
            this.lblCreatedCaption.Font = new System.Drawing.Font("default", 9F);
            this.lblCreatedCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCreatedCaption.Location = new System.Drawing.Point(368, 154);
            this.lblCreatedCaption.Name = "lblCreatedCaption";
            this.lblCreatedCaption.Size = new System.Drawing.Size(60, 18);
            this.lblCreatedCaption.Text = "Created";
            //
            // dtpCreated
            //
            this.dtpCreated.Font = new System.Drawing.Font("default", 10F);
            this.dtpCreated.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpCreated.Location = new System.Drawing.Point(368, 174);
            this.dtpCreated.Name = "dtpCreated";
            this.dtpCreated.Size = new System.Drawing.Size(164, 32);
            //
            // lblDescriptionCaption
            //
            this.lblDescriptionCaption.AutoSize = true;
            this.lblDescriptionCaption.Font = new System.Drawing.Font("default", 9F);
            this.lblDescriptionCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblDescriptionCaption.Location = new System.Drawing.Point(16, 216);
            this.lblDescriptionCaption.Name = "lblDescriptionCaption";
            this.lblDescriptionCaption.Size = new System.Drawing.Size(80, 18);
            this.lblDescriptionCaption.Text = "Description";
            //
            // txtDescription
            //
            this.txtDescription.Font = new System.Drawing.Font("default", 10F);
            this.txtDescription.Location = new System.Drawing.Point(16, 236);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(516, 56);
            //
            // btnSaveTicket  (the lab's Save handler: service → ResetBindings → status)
            //
            this.btnSaveTicket.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSaveTicket.Location = new System.Drawing.Point(16, 304);
            this.btnSaveTicket.Name = "btnSaveTicket";
            this.btnSaveTicket.Size = new System.Drawing.Size(120, 34);
            this.btnSaveTicket.Text = "Save Ticket";
            this.btnSaveTicket.ToolTipText = "ticketService.SaveTicket(ticketsBindingSource.Current) then ResetBindings(false).";
            this.btnSaveTicket.Click += new System.EventHandler(this.btnSaveTicket_Click);
            //
            // btnRefresh  (reload from the service on purpose — discards unsaved edits)
            //
            this.btnRefresh.Font = new System.Drawing.Font("default", 10F);
            this.btnRefresh.Location = new System.Drawing.Point(144, 304);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 34);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.ToolTipText = "LoadTickets() again: ticketsBindingSource.DataSource = ticketService.GetTickets().";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnSimulateBadSave  (failure path: blank the Title, save through the same handler)
            //
            this.btnSimulateBadSave.Font = new System.Drawing.Font("default", 10F);
            this.btnSimulateBadSave.Location = new System.Drawing.Point(252, 304);
            this.btnSimulateBadSave.Name = "btnSimulateBadSave";
            this.btnSimulateBadSave.Size = new System.Drawing.Size(280, 34);
            this.btnSimulateBadSave.Text = "Save with blank title (validation)";
            this.btnSimulateBadSave.ToolTipText = "Clears txtTitle and calls btnSaveTicket_Click — TicketService rejects it with ArgumentException.";
            this.btnSimulateBadSave.Click += new System.EventHandler(this.btnSimulateBadSave_Click);
            //
            // panelLog  (Dock = Fill under the details: the course's event-log card)
            //
            this.panelLog.Controls.Add(this.panelLogCard);
            this.panelLog.Dock = Wisej.Web.DockStyle.Fill;
            this.panelLog.Location = new System.Drawing.Point(12, 368);
            this.panelLog.Name = "panelLog";
            this.panelLog.Size = new System.Drawing.Size(548, 232);
            //
            // panelLogCard
            //
            this.panelLogCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelLogCard.BackColor = System.Drawing.Color.White;
            this.panelLogCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelLogCard.Controls.Add(this.labelLogCard);
            this.panelLogCard.Controls.Add(this.btnClearLog);
            this.panelLogCard.Controls.Add(this.lstEventLog);
            this.panelLogCard.Controls.Add(this.labelLogFooter);
            this.panelLogCard.Location = new System.Drawing.Point(0, 12);
            this.panelLogCard.Name = "panelLogCard";
            this.panelLogCard.Size = new System.Drawing.Size(548, 220);
            //
            // labelLogCard
            //
            this.labelLogCard.AutoSize = false;
            this.labelLogCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelLogCard.Location = new System.Drawing.Point(20, 8);
            this.labelLogCard.Name = "labelLogCard";
            this.labelLogCard.Size = new System.Drawing.Size(400, 26);
            this.labelLogCard.Text = "Event log  ·  what the server-side code did";
            //
            // btnClearLog
            //
            this.btnClearLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearLog.Location = new System.Drawing.Point(438, 6);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(90, 30);
            this.btnClearLog.Text = "Clear log";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            // lstEventLog
            //
            this.lstEventLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstEventLog.Font = new System.Drawing.Font("monospace", 9F);
            this.lstEventLog.Location = new System.Drawing.Point(20, 42);
            this.lstEventLog.Name = "lstEventLog";
            this.lstEventLog.Size = new System.Drawing.Size(508, 146);
            //
            // labelLogFooter
            //
            this.labelLogFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLogFooter.AutoSize = false;
            this.labelLogFooter.Font = new System.Drawing.Font("default", 9F);
            this.labelLogFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLogFooter.Location = new System.Drawing.Point(20, 192);
            this.labelLogFooter.Name = "labelLogFooter";
            this.labelLogFooter.Size = new System.Drawing.Size(508, 22);
            this.labelLogFooter.Text = "grid ↔ BindingSource ↔ detail controls  ·  Save → TicketService → ResetBindings";
            //
            // TicketsWindow
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panelHeader);
            this.Name = "TicketsWindow";
            this.Text = "WisejTrainingApp — Ticket Management (Module 4)";
            this.Load += new System.EventHandler(this.TicketsWindow_Load);
            this.panelHeader.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panelState.ResumeLayout(false);
            this.panelStateCard.ResumeLayout(false);
            this.grpTicketDetails.ResumeLayout(false);
            this.panelLog.ResumeLayout(false);
            this.panelLogCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelHeader;
        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.SplitContainer splitContainer1;
        private Wisej.Web.DataGridView dgvTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colAssignedTo;
        private Wisej.Web.Panel panelState;
        private Wisej.Web.Panel panelStateCard;
        private Wisej.Web.Label labelStateCard;
        private Wisej.Web.Label lblState;
        private Wisej.Web.Button btnAddSample;
        private Wisej.Web.GroupBox grpTicketDetails;
        private Wisej.Web.Label lblTitleCaption;
        private Wisej.Web.TextBox txtTitle;
        private Wisej.Web.Label lblCustomerCaption;
        private Wisej.Web.TextBox txtCustomer;
        private Wisej.Web.Label lblAssignedToCaption;
        private Wisej.Web.TextBox txtAssignedTo;
        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.ComboBox cmbStatus;
        private Wisej.Web.Label lblPriorityCaption;
        private Wisej.Web.ComboBox cmbPriority;
        private Wisej.Web.Label lblCreatedCaption;
        private Wisej.Web.DateTimePicker dtpCreated;
        private Wisej.Web.Label lblDescriptionCaption;
        private Wisej.Web.TextBox txtDescription;
        private Wisej.Web.Button btnSaveTicket;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.Button btnSimulateBadSave;
        private Wisej.Web.Panel panelLog;
        private Wisej.Web.Panel panelLogCard;
        private Wisej.Web.Label labelLogCard;
        private Wisej.Web.Button btnClearLog;
        private Wisej.Web.ListBox lstEventLog;
        private Wisej.Web.Label labelLogFooter;
    }
}
