namespace TicketOps.Controls
{
    partial class TicketWorkspace
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
            if (disposing)
            {
                // Application.ResponsiveProfileChanged is a session-level event: unsubscribe with the control.
                if (_subscribed)
                {
                    Wisej.Web.Application.ResponsiveProfileChanged -= this.Application_ResponsiveProfileChanged;
                    _subscribed = false;
                }

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.pnlHeader = new Wisej.Web.Panel();
            this.btnBack = new Wisej.Web.Button();
            this.lblTitle = new Wisej.Web.Label();
            this.pnlToolbar = new Wisej.Web.Panel();
            this.searchTickets = new TicketOps.Controls.SearchBar();
            this.btnNewTicket = new Wisej.Web.Button();
            this.pnlNavigation = new Wisej.Web.Panel();
            this.lblNavDashboard = new Wisej.Web.Label();
            this.lblNavTickets = new Wisej.Web.Label();
            this.lblNavReports = new Wisej.Web.Label();
            this.lblNavSettings = new Wisej.Web.Label();
            this.flexBody = new Wisej.Web.FlexLayoutPanel();
            this.pnlList = new Wisej.Web.Panel();
            this.flowChips = new Wisej.Web.FlowLayoutPanel();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssignee = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlSide = new Wisej.Web.Panel();
            this.flexSide = new Wisej.Web.FlexLayoutPanel();
            this.pnlDetail = new Wisej.Web.Panel();
            this.lblDetailHead = new Wisej.Web.Label();
            this.tableDetail = new Wisej.Web.TableLayoutPanel();
            this.lblDetailTitle = new Wisej.Web.Label();
            this.lblCapTitle = new Wisej.Web.Label();
            this.txtTitle = new Wisej.Web.TextBox();
            this.lblCapPriority = new Wisej.Web.Label();
            this.cmbPriority = new Wisej.Web.ComboBox();
            this.lblCapAssignee = new Wisej.Web.Label();
            this.cmbAssignee = new Wisej.Web.ComboBox();
            this.lblCapStatus = new Wisej.Web.Label();
            this.lblStatusValue = new Wisej.Web.Label();
            this.lblCapHours = new Wisej.Web.Label();
            this.numHours = new Wisej.Web.NumericUpDown();
            this.lblCapNotes = new Wisej.Web.Label();
            this.txtNotes = new Wisej.Web.TextBox();
            this.flowDetailButtons = new Wisej.Web.FlowLayoutPanel();
            this.btnSave = new Wisej.Web.Button();
            this.btnClose = new Wisej.Web.Button();
            this.pnlActivity = new Wisej.Web.Panel();
            this.lblActivityHead = new Wisej.Web.Label();
            this.pnlActivitySearch = new Wisej.Web.Panel();
            this.searchActivity = new TicketOps.Controls.SearchBar();
            this.listActivity = new Wisej.Web.ListBox();
            this.tabActivity = new Wisej.Web.TabControl();
            this.tabPageDetails = new Wisej.Web.TabPage();
            this.tabPageActivity = new Wisej.Web.TabPage();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlToolbar.SuspendLayout();
            this.pnlNavigation.SuspendLayout();
            this.flexBody.SuspendLayout();
            this.pnlList.SuspendLayout();
            this.pnlSide.SuspendLayout();
            this.flexSide.SuspendLayout();
            this.pnlDetail.SuspendLayout();
            this.tableDetail.SuspendLayout();
            this.flowDetailButtons.SuspendLayout();
            this.pnlActivity.SuspendLayout();
            this.pnlActivitySearch.SuspendLayout();
            this.tabActivity.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (Dock Top: title + the phone-only Back button)
            //
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(26, 134, 255);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.btnBack);
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(712, 36);
            //
            // btnBack  (visible on the Phone profile only)
            //
            this.btnBack.Dock = Wisej.Web.DockStyle.Left;
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(84, 36);
            this.btnBack.Text = "← Back";
            this.btnBack.Visible = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("default", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new Wisej.Web.Padding(12, 0, 0, 0);
            this.lblTitle.Text = "TicketOps — Ticket Workspace";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlToolbar  (Dock Top: SearchBar #1 + New Ticket; hidden on the Phone profile)
            //
            this.pnlToolbar.BackColor = System.Drawing.Color.White;
            this.pnlToolbar.Controls.Add(this.searchTickets);
            this.pnlToolbar.Controls.Add(this.btnNewTicket);
            this.pnlToolbar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Size = new System.Drawing.Size(712, 52);
            //
            // searchTickets  (reusable SearchBar — first use)
            //
            this.searchTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.searchTickets.Location = new System.Drawing.Point(12, 9);
            this.searchTickets.Name = "searchTickets";
            this.searchTickets.Size = new System.Drawing.Size(546, 34);
            this.searchTickets.SearchRequested += new System.EventHandler<TicketOps.Controls.SearchEventArgs>(this.searchTickets_SearchRequested);
            //
            // btnNewTicket
            //
            this.btnNewTicket.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnNewTicket.Location = new System.Drawing.Point(570, 9);
            this.btnNewTicket.Name = "btnNewTicket";
            this.btnNewTicket.Size = new System.Drawing.Size(130, 34);
            this.btnNewTicket.Text = "+ New Ticket";
            this.btnNewTicket.Click += new System.EventHandler(this.btnNewTicket_Click);
            //
            // pnlNavigation  (Dock Left: the navigation rail; compact on tablet, hidden on phone)
            // Dock Top items: the control added LAST docks FIRST, so add them bottom-up.
            //
            this.pnlNavigation.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.pnlNavigation.Controls.Add(this.lblNavSettings);
            this.pnlNavigation.Controls.Add(this.lblNavReports);
            this.pnlNavigation.Controls.Add(this.lblNavTickets);
            this.pnlNavigation.Controls.Add(this.lblNavDashboard);
            this.pnlNavigation.Dock = Wisej.Web.DockStyle.Left;
            this.pnlNavigation.Name = "pnlNavigation";
            this.pnlNavigation.Padding = new Wisej.Web.Padding(0, 10, 0, 0);
            this.pnlNavigation.Size = new System.Drawing.Size(150, 342);
            //
            // navigation items
            //
            this.lblNavDashboard.AutoSize = false;
            this.lblNavDashboard.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblNavDashboard.Cursor = Wisej.Web.Cursors.Hand;
            this.lblNavDashboard.Dock = Wisej.Web.DockStyle.Top;
            this.lblNavDashboard.ForeColor = System.Drawing.Color.FromArgb(143, 169, 201);
            this.lblNavDashboard.Name = "lblNavDashboard";
            this.lblNavDashboard.Padding = new Wisej.Web.Padding(16, 0, 0, 0);
            this.lblNavDashboard.Size = new System.Drawing.Size(150, 42);
            this.lblNavDashboard.Tag = "Dashboard";
            this.lblNavDashboard.Text = "▦   Dashboard";
            this.lblNavDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNavDashboard.Click += new System.EventHandler(this.lblNav_Click);
            this.lblNavTickets.AutoSize = false;
            this.lblNavTickets.BackColor = System.Drawing.Color.FromArgb(31, 66, 110);
            this.lblNavTickets.Cursor = Wisej.Web.Cursors.Hand;
            this.lblNavTickets.Dock = Wisej.Web.DockStyle.Top;
            this.lblNavTickets.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblNavTickets.ForeColor = System.Drawing.Color.White;
            this.lblNavTickets.Name = "lblNavTickets";
            this.lblNavTickets.Padding = new Wisej.Web.Padding(16, 0, 0, 0);
            this.lblNavTickets.Size = new System.Drawing.Size(150, 42);
            this.lblNavTickets.Tag = "Tickets";
            this.lblNavTickets.Text = "◳   Tickets";
            this.lblNavTickets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNavTickets.Click += new System.EventHandler(this.lblNav_Click);
            this.lblNavReports.AutoSize = false;
            this.lblNavReports.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblNavReports.Cursor = Wisej.Web.Cursors.Hand;
            this.lblNavReports.Dock = Wisej.Web.DockStyle.Top;
            this.lblNavReports.ForeColor = System.Drawing.Color.FromArgb(143, 169, 201);
            this.lblNavReports.Name = "lblNavReports";
            this.lblNavReports.Padding = new Wisej.Web.Padding(16, 0, 0, 0);
            this.lblNavReports.Size = new System.Drawing.Size(150, 42);
            this.lblNavReports.Tag = "Reports";
            this.lblNavReports.Text = "▤   Reports";
            this.lblNavReports.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNavReports.Click += new System.EventHandler(this.lblNav_Click);
            this.lblNavSettings.AutoSize = false;
            this.lblNavSettings.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblNavSettings.Cursor = Wisej.Web.Cursors.Hand;
            this.lblNavSettings.Dock = Wisej.Web.DockStyle.Top;
            this.lblNavSettings.ForeColor = System.Drawing.Color.FromArgb(143, 169, 201);
            this.lblNavSettings.Name = "lblNavSettings";
            this.lblNavSettings.Padding = new Wisej.Web.Padding(16, 0, 0, 0);
            this.lblNavSettings.Size = new System.Drawing.Size(150, 42);
            this.lblNavSettings.Tag = "Settings";
            this.lblNavSettings.Text = "⚙   Settings";
            this.lblNavSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNavSettings.Click += new System.EventHandler(this.lblNav_Click);
            //
            // flexBody  (Dock Fill: FlexLayoutPanel, horizontal — list : side pane by fill weight)
            //
            this.flexBody.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.flexBody.Controls.Add(this.pnlList);
            this.flexBody.Controls.Add(this.pnlSide);
            this.flexBody.Dock = Wisej.Web.DockStyle.Fill;
            this.flexBody.LayoutStyle = Wisej.Web.FlexLayoutStyle.Horizontal;
            this.flexBody.Name = "flexBody";
            this.flexBody.Padding = new Wisej.Web.Padding(12);
            this.flexBody.SetFillWeight(this.pnlList, 3);
            this.flexBody.SetFillWeight(this.pnlSide, 2);
            this.flexBody.Size = new System.Drawing.Size(562, 342);
            this.flexBody.Spacing = 12;
            //
            // pnlList  (the ticket grid card: chips Dock Top, grid Dock Fill)
            //
            this.pnlList.BackColor = System.Drawing.Color.White;
            this.pnlList.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlList.Controls.Add(this.gridTickets);
            this.pnlList.Controls.Add(this.flowChips);
            this.pnlList.MinimumSize = new System.Drawing.Size(180, 120);
            this.pnlList.Name = "pnlList";
            this.pnlList.Size = new System.Drawing.Size(320, 318);
            //
            // flowChips  (FlowLayoutPanel: filter chips that wrap when the card gets narrow)
            //
            this.flowChips.Dock = Wisej.Web.DockStyle.Top;
            this.flowChips.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.flowChips.Name = "flowChips";
            this.flowChips.Padding = new Wisej.Web.Padding(8, 8, 2, 2);
            this.flowChips.Size = new System.Drawing.Size(318, 78);
            this.flowChips.WrapContents = true;
            //
            // gridTickets
            //
            this.gridTickets.AllowUserToAddRows = false;
            this.gridTickets.AllowUserToDeleteRows = false;
            this.gridTickets.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridTickets.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.colId,
                this.colTitle,
                this.colPriority,
                this.colAssignee,
                this.colStatus});
            this.gridTickets.Dock = Wisej.Web.DockStyle.Fill;
            this.gridTickets.MultiSelect = false;
            this.gridTickets.Name = "gridTickets";
            this.gridTickets.ReadOnly = true;
            this.gridTickets.RowHeadersVisible = false;
            this.gridTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridTickets.SelectionChanged += new System.EventHandler(this.gridTickets_SelectionChanged);
            //
            // columns  (Assignee hides on tablet, Assignee + Status hide on phone — server-side properties)
            //
            this.colId.FillWeight = 50F;
            this.colId.HeaderText = "Id";
            this.colId.MinimumWidth = 48;
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            this.colTitle.FillWeight = 200F;
            this.colTitle.HeaderText = "Title";
            this.colTitle.MinimumWidth = 120;
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            this.colPriority.FillWeight = 70F;
            this.colPriority.HeaderText = "Priority";
            this.colPriority.MinimumWidth = 64;
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            this.colAssignee.FillWeight = 90F;
            this.colAssignee.HeaderText = "Assignee";
            this.colAssignee.MinimumWidth = 80;
            this.colAssignee.Name = "colAssignee";
            this.colAssignee.ReadOnly = true;
            this.colStatus.FillWeight = 90F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.MinimumWidth = 80;
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            //
            // pnlSide  (holds the vertical flex on desktop and the TabControl on tablet / phone)
            //
            this.pnlSide.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.pnlSide.Controls.Add(this.flexSide);
            this.pnlSide.Controls.Add(this.tabActivity);
            this.pnlSide.MinimumSize = new System.Drawing.Size(160, 120);
            this.pnlSide.Name = "pnlSide";
            this.pnlSide.Size = new System.Drawing.Size(206, 318);
            //
            // flexSide  (FlexLayoutPanel, vertical — detail : activity = 3 : 2)
            //
            this.flexSide.Controls.Add(this.pnlDetail);
            this.flexSide.Controls.Add(this.pnlActivity);
            this.flexSide.Dock = Wisej.Web.DockStyle.Fill;
            this.flexSide.LayoutStyle = Wisej.Web.FlexLayoutStyle.Vertical;
            this.flexSide.Name = "flexSide";
            this.flexSide.SetFillWeight(this.pnlDetail, 3);
            this.flexSide.SetFillWeight(this.pnlActivity, 2);
            this.flexSide.Spacing = 12;
            //
            // pnlDetail  (the detail card: head Dock Top, TableLayoutPanel Dock Fill)
            //
            this.pnlDetail.BackColor = System.Drawing.Color.White;
            this.pnlDetail.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlDetail.Controls.Add(this.tableDetail);
            this.pnlDetail.Controls.Add(this.lblDetailHead);
            this.pnlDetail.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlDetail.MinimumSize = new System.Drawing.Size(160, 120);
            this.pnlDetail.Name = "pnlDetail";
            this.pnlDetail.Size = new System.Drawing.Size(206, 186);
            //
            // lblDetailHead
            //
            this.lblDetailHead.AutoSize = false;
            this.lblDetailHead.BackColor = System.Drawing.Color.FromArgb(246, 249, 252);
            this.lblDetailHead.Dock = Wisej.Web.DockStyle.Top;
            this.lblDetailHead.Font = new System.Drawing.Font("default", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblDetailHead.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblDetailHead.Name = "lblDetailHead";
            this.lblDetailHead.Padding = new Wisej.Web.Padding(12, 0, 0, 0);
            this.lblDetailHead.Size = new System.Drawing.Size(204, 28);
            this.lblDetailHead.Text = "TICKET DETAIL";
            this.lblDetailHead.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tableDetail  (TableLayoutPanel: 35 % captions / 65 % fields; Notes takes the remaining height)
            //
            this.tableDetail.ColumnCount = 2;
            this.tableDetail.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 35F));
            this.tableDetail.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 65F));
            this.tableDetail.Controls.Add(this.lblDetailTitle, 0, 0);
            this.tableDetail.Controls.Add(this.lblCapTitle, 0, 1);
            this.tableDetail.Controls.Add(this.txtTitle, 1, 1);
            this.tableDetail.Controls.Add(this.lblCapPriority, 0, 2);
            this.tableDetail.Controls.Add(this.cmbPriority, 1, 2);
            this.tableDetail.Controls.Add(this.lblCapAssignee, 0, 3);
            this.tableDetail.Controls.Add(this.cmbAssignee, 1, 3);
            this.tableDetail.Controls.Add(this.lblCapStatus, 0, 4);
            this.tableDetail.Controls.Add(this.lblStatusValue, 1, 4);
            this.tableDetail.Controls.Add(this.lblCapHours, 0, 5);
            this.tableDetail.Controls.Add(this.numHours, 1, 5);
            this.tableDetail.Controls.Add(this.lblCapNotes, 0, 6);
            this.tableDetail.Controls.Add(this.txtNotes, 1, 6);
            this.tableDetail.Controls.Add(this.flowDetailButtons, 0, 7);
            this.tableDetail.Dock = Wisej.Web.DockStyle.Fill;
            this.tableDetail.Name = "tableDetail";
            this.tableDetail.Padding = new Wisej.Web.Padding(10, 6, 10, 6);
            this.tableDetail.RowCount = 8;
            this.tableDetail.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 28F));
            this.tableDetail.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 36F));
            this.tableDetail.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 36F));
            this.tableDetail.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 36F));
            this.tableDetail.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 30F));
            this.tableDetail.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 36F));
            this.tableDetail.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
            this.tableDetail.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 44F));
            this.tableDetail.SetColumnSpan(this.lblDetailTitle, 2);
            this.tableDetail.SetColumnSpan(this.flowDetailButtons, 2);
            //
            // detail fields  (Dock Fill inside their cells — relational, never a coordinate)
            //
            this.lblDetailTitle.AutoSize = false;
            this.lblDetailTitle.AutoEllipsis = true;
            this.lblDetailTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblDetailTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblDetailTitle.Margin = new Wisej.Web.Padding(0, 0, 0, 4);
            this.lblDetailTitle.Name = "lblDetailTitle";
            this.lblDetailTitle.Text = "New ticket";
            this.lblDetailTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCapTitle.AutoSize = false;
            this.lblCapTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblCapTitle.ForeColor = System.Drawing.Color.FromArgb(106, 125, 146);
            this.lblCapTitle.Margin = new Wisej.Web.Padding(0);
            this.lblCapTitle.Name = "lblCapTitle";
            this.lblCapTitle.Text = "Title";
            this.lblCapTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.txtTitle.Margin = new Wisej.Web.Padding(0, 2, 0, 2);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Watermark = "What is wrong?";
            this.lblCapPriority.AutoSize = false;
            this.lblCapPriority.Dock = Wisej.Web.DockStyle.Fill;
            this.lblCapPriority.ForeColor = System.Drawing.Color.FromArgb(106, 125, 146);
            this.lblCapPriority.Margin = new Wisej.Web.Padding(0);
            this.lblCapPriority.Name = "lblCapPriority";
            this.lblCapPriority.Text = "Priority";
            this.lblCapPriority.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmbPriority.Dock = Wisej.Web.DockStyle.Fill;
            this.cmbPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbPriority.Margin = new Wisej.Web.Padding(0, 2, 0, 2);
            this.cmbPriority.Name = "cmbPriority";
            this.lblCapAssignee.AutoSize = false;
            this.lblCapAssignee.Dock = Wisej.Web.DockStyle.Fill;
            this.lblCapAssignee.ForeColor = System.Drawing.Color.FromArgb(106, 125, 146);
            this.lblCapAssignee.Margin = new Wisej.Web.Padding(0);
            this.lblCapAssignee.Name = "lblCapAssignee";
            this.lblCapAssignee.Text = "Assignee";
            this.lblCapAssignee.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmbAssignee.Dock = Wisej.Web.DockStyle.Fill;
            this.cmbAssignee.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbAssignee.Margin = new Wisej.Web.Padding(0, 2, 0, 2);
            this.cmbAssignee.Name = "cmbAssignee";
            this.lblCapStatus.AutoSize = false;
            this.lblCapStatus.Dock = Wisej.Web.DockStyle.Fill;
            this.lblCapStatus.ForeColor = System.Drawing.Color.FromArgb(106, 125, 146);
            this.lblCapStatus.Margin = new Wisej.Web.Padding(0);
            this.lblCapStatus.Name = "lblCapStatus";
            this.lblCapStatus.Text = "Status";
            this.lblCapStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatusValue.AutoSize = false;
            this.lblStatusValue.Dock = Wisej.Web.DockStyle.Fill;
            this.lblStatusValue.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblStatusValue.Margin = new Wisej.Web.Padding(0);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Text = "Open";
            this.lblStatusValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCapHours.AutoSize = false;
            this.lblCapHours.Dock = Wisej.Web.DockStyle.Fill;
            this.lblCapHours.ForeColor = System.Drawing.Color.FromArgb(106, 125, 146);
            this.lblCapHours.Margin = new Wisej.Web.Padding(0);
            this.lblCapHours.Name = "lblCapHours";
            this.lblCapHours.Text = "Hours logged";
            this.lblCapHours.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.numHours.DecimalPlaces = 2;
            this.numHours.Dock = Wisej.Web.DockStyle.Fill;
            this.numHours.Increment = new decimal(new int[] { 25, 0, 0, 131072 });
            this.numHours.Margin = new Wisej.Web.Padding(0, 2, 0, 2);
            this.numHours.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            this.numHours.Name = "numHours";
            this.lblCapNotes.AutoSize = false;
            this.lblCapNotes.Dock = Wisej.Web.DockStyle.Fill;
            this.lblCapNotes.ForeColor = System.Drawing.Color.FromArgb(106, 125, 146);
            this.lblCapNotes.Margin = new Wisej.Web.Padding(0);
            this.lblCapNotes.Name = "lblCapNotes";
            this.lblCapNotes.Text = "Notes";
            this.lblCapNotes.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.lblCapNotes.Padding = new Wisej.Web.Padding(0, 8, 0, 0);
            this.txtNotes.Dock = Wisej.Web.DockStyle.Fill;
            this.txtNotes.Margin = new Wisej.Web.Padding(0, 2, 0, 2);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Watermark = "Notes for the next agent";
            //
            // flowDetailButtons  (FlowLayoutPanel, right-to-left: Save hugs the right edge, Close follows)
            //
            this.flowDetailButtons.Controls.Add(this.btnSave);
            this.flowDetailButtons.Controls.Add(this.btnClose);
            this.flowDetailButtons.Dock = Wisej.Web.DockStyle.Fill;
            this.flowDetailButtons.FlowDirection = Wisej.Web.FlowDirection.RightToLeft;
            this.flowDetailButtons.Margin = new Wisej.Web.Padding(0);
            this.flowDetailButtons.Name = "flowDetailButtons";
            this.flowDetailButtons.WrapContents = false;
            //
            // btnSave / btnClose  (thin handlers → ITicketService)
            //
            this.btnSave.Margin = new Wisej.Web.Padding(6, 6, 0, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(84, 32);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnClose.Margin = new Wisej.Web.Padding(6, 6, 0, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(84, 32);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // pnlActivity  (the activity card: head Dock Top, SearchBar #2 Dock Top, feed Dock Fill)
            //
            this.pnlActivity.BackColor = System.Drawing.Color.White;
            this.pnlActivity.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlActivity.Controls.Add(this.listActivity);
            this.pnlActivity.Controls.Add(this.pnlActivitySearch);
            this.pnlActivity.Controls.Add(this.lblActivityHead);
            this.pnlActivity.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlActivity.MinimumSize = new System.Drawing.Size(160, 100);
            this.pnlActivity.Name = "pnlActivity";
            this.pnlActivity.Size = new System.Drawing.Size(206, 120);
            //
            // lblActivityHead
            //
            this.lblActivityHead.AutoSize = false;
            this.lblActivityHead.BackColor = System.Drawing.Color.FromArgb(246, 249, 252);
            this.lblActivityHead.Dock = Wisej.Web.DockStyle.Top;
            this.lblActivityHead.Font = new System.Drawing.Font("default", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblActivityHead.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblActivityHead.Name = "lblActivityHead";
            this.lblActivityHead.Padding = new Wisej.Web.Padding(12, 0, 0, 0);
            this.lblActivityHead.Size = new System.Drawing.Size(204, 28);
            this.lblActivityHead.Text = "ACTIVITY";
            this.lblActivityHead.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlActivitySearch → searchActivity  (reusable SearchBar — second use)
            //
            this.pnlActivitySearch.Controls.Add(this.searchActivity);
            this.pnlActivitySearch.Dock = Wisej.Web.DockStyle.Top;
            this.pnlActivitySearch.Name = "pnlActivitySearch";
            this.pnlActivitySearch.Padding = new Wisej.Web.Padding(8, 6, 8, 6);
            this.pnlActivitySearch.Size = new System.Drawing.Size(204, 46);
            this.searchActivity.Dock = Wisej.Web.DockStyle.Fill;
            this.searchActivity.Name = "searchActivity";
            this.searchActivity.SearchRequested += new System.EventHandler<TicketOps.Controls.SearchEventArgs>(this.searchActivity_SearchRequested);
            //
            // listActivity
            //
            this.listActivity.Dock = Wisej.Web.DockStyle.Fill;
            this.listActivity.Font = new System.Drawing.Font("default", 9F);
            this.listActivity.Name = "listActivity";
            //
            // tabActivity  (TabControl used on tablet / phone: the SAME detail and activity panels move into its pages)
            //
            this.tabActivity.Controls.Add(this.tabPageDetails);
            this.tabActivity.Controls.Add(this.tabPageActivity);
            this.tabActivity.Dock = Wisej.Web.DockStyle.Fill;
            this.tabActivity.Name = "tabActivity";
            this.tabActivity.SelectedIndex = 0;
            this.tabActivity.Visible = false;
            this.tabPageDetails.Name = "tabPageDetails";
            this.tabPageDetails.Text = "Details";
            this.tabPageActivity.Name = "tabPageActivity";
            this.tabPageActivity.Text = "Activity";
            //
            // lblStatus  (Dock Bottom: "Active profile: …" — the video's status bar)
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("monospace", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(12, 0, 0, 0);
            this.lblStatus.Size = new System.Drawing.Size(712, 26);
            this.lblStatus.Text = "Active profile: —";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // TicketWorkspace  (outer shell = Dock. Add order matters: Fill first, then the edges carve their strips)
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.flexBody);
            this.Controls.Add(this.pnlNavigation);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.lblStatus);
            this.Name = "TicketWorkspace";
            this.Size = new System.Drawing.Size(712, 456);
            this.pnlHeader.ResumeLayout(false);
            this.pnlToolbar.ResumeLayout(false);
            this.pnlNavigation.ResumeLayout(false);
            this.flexBody.ResumeLayout(false);
            this.pnlList.ResumeLayout(false);
            this.pnlSide.ResumeLayout(false);
            this.flexSide.ResumeLayout(false);
            this.pnlDetail.ResumeLayout(false);
            this.tableDetail.ResumeLayout(false);
            this.flowDetailButtons.ResumeLayout(false);
            this.pnlActivity.ResumeLayout(false);
            this.pnlActivitySearch.ResumeLayout(false);
            this.tabActivity.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Button btnBack;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Panel pnlToolbar;
        private TicketOps.Controls.SearchBar searchTickets;
        private Wisej.Web.Button btnNewTicket;
        private Wisej.Web.Panel pnlNavigation;
        private Wisej.Web.Label lblNavDashboard;
        private Wisej.Web.Label lblNavTickets;
        private Wisej.Web.Label lblNavReports;
        private Wisej.Web.Label lblNavSettings;
        private Wisej.Web.FlexLayoutPanel flexBody;
        private Wisej.Web.Panel pnlList;
        private Wisej.Web.FlowLayoutPanel flowChips;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colAssignee;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.Panel pnlSide;
        private Wisej.Web.FlexLayoutPanel flexSide;
        private Wisej.Web.Panel pnlDetail;
        private Wisej.Web.Label lblDetailHead;
        private Wisej.Web.TableLayoutPanel tableDetail;
        private Wisej.Web.Label lblDetailTitle;
        private Wisej.Web.Label lblCapTitle;
        private Wisej.Web.TextBox txtTitle;
        private Wisej.Web.Label lblCapPriority;
        private Wisej.Web.ComboBox cmbPriority;
        private Wisej.Web.Label lblCapAssignee;
        private Wisej.Web.ComboBox cmbAssignee;
        private Wisej.Web.Label lblCapStatus;
        private Wisej.Web.Label lblStatusValue;
        private Wisej.Web.Label lblCapHours;
        private Wisej.Web.NumericUpDown numHours;
        private Wisej.Web.Label lblCapNotes;
        private Wisej.Web.TextBox txtNotes;
        private Wisej.Web.FlowLayoutPanel flowDetailButtons;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnClose;
        private Wisej.Web.Panel pnlActivity;
        private Wisej.Web.Label lblActivityHead;
        private Wisej.Web.Panel pnlActivitySearch;
        private TicketOps.Controls.SearchBar searchActivity;
        private Wisej.Web.ListBox listActivity;
        private Wisej.Web.TabControl tabActivity;
        private Wisej.Web.TabPage tabPageDetails;
        private Wisej.Web.TabPage tabPageActivity;
        private Wisej.Web.Label lblStatus;
    }
}
