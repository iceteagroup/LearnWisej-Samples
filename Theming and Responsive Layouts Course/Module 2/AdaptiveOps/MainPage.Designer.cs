namespace AdaptiveOps
{
    partial class MainPage
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
            this.toolbarPanel = new Wisej.Web.Panel();
            this.toolbarCard = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.btnApplyTheme = new Wisej.Web.Button();
            this.navigationPanel = new Wisej.Web.Panel();
            this.navigationCard = new Wisej.Web.Panel();
            this.lblNavTitle = new Wisej.Web.Label();
            this.btnNavDashboard = new Wisej.Web.Button();
            this.btnNavTickets = new Wisej.Web.Button();
            this.btnNavReports = new Wisej.Web.Button();
            this.btnNavSettings = new Wisej.Web.Button();
            this.btnNavHelp = new Wisej.Web.Button();
            this.workspacePanel = new Wisej.Web.Panel();
            this.metricsPanel = new Wisej.Web.Panel();
            this.slotOpen = new Wisej.Web.Panel();
            this.cardOpen = new Wisej.Web.Panel();
            this.stripOpen = new Wisej.Web.Panel();
            this.lblOpenTitle = new Wisej.Web.Label();
            this.lblOpenValue = new Wisej.Web.Label();
            this.slotOverdue = new Wisej.Web.Panel();
            this.cardOverdue = new Wisej.Web.Panel();
            this.stripOverdue = new Wisej.Web.Panel();
            this.lblOverdueTitle = new Wisej.Web.Label();
            this.lblOverdueValue = new Wisej.Web.Label();
            this.slotMine = new Wisej.Web.Panel();
            this.cardMine = new Wisej.Web.Panel();
            this.stripMine = new Wisej.Web.Panel();
            this.lblMineTitle = new Wisej.Web.Label();
            this.lblMineValue = new Wisej.Web.Label();
            this.slotClosed = new Wisej.Web.Panel();
            this.cardClosed = new Wisej.Web.Panel();
            this.stripClosed = new Wisej.Web.Panel();
            this.lblClosedTitle = new Wisej.Web.Label();
            this.lblClosedValue = new Wisej.Web.Label();
            this.tabWorkspace = new Wisej.Web.TabControl();
            this.pageTickets = new Wisej.Web.TabPage();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.detailsPanel = new Wisej.Web.Panel();
            this.detailsCard = new Wisej.Web.Panel();
            this.lblDetailsSubtitle = new Wisej.Web.Label();
            this.lblTitleCaption = new Wisej.Web.Label();
            this.txtTitle = new Wisej.Web.TextBox();
            this.lblPriorityCaption = new Wisej.Web.Label();
            this.cboPriority = new Wisej.Web.ComboBox();
            this.lblStatusCaption = new Wisej.Web.Label();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.lblOwnerCaption = new Wisej.Web.Label();
            this.txtOwner = new Wisej.Web.TextBox();
            this.lblDueCaption = new Wisej.Web.Label();
            this.dtpDue = new Wisej.Web.DateTimePicker();
            this.lblNotesCaption = new Wisej.Web.Label();
            this.txtNotes = new Wisej.Web.TextBox();
            this.btnSave = new Wisej.Web.Button();
            this.statusPanel = new Wisej.Web.Panel();
            this.statusCard = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.widthLabel = new Wisej.Web.Label();
            this.toolbarPanel.SuspendLayout();
            this.toolbarCard.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            this.navigationCard.SuspendLayout();
            this.workspacePanel.SuspendLayout();
            this.metricsPanel.SuspendLayout();
            this.slotOpen.SuspendLayout();
            this.cardOpen.SuspendLayout();
            this.slotOverdue.SuspendLayout();
            this.cardOverdue.SuspendLayout();
            this.slotMine.SuspendLayout();
            this.cardMine.SuspendLayout();
            this.slotClosed.SuspendLayout();
            this.cardClosed.SuspendLayout();
            this.tabWorkspace.SuspendLayout();
            this.pageTickets.SuspendLayout();
            this.detailsPanel.SuspendLayout();
            this.detailsCard.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.statusCard.SuspendLayout();
            this.SuspendLayout();
            //
            // toolbarPanel
            //
            this.toolbarPanel.Controls.Add(this.toolbarCard);
            this.toolbarPanel.Dock = Wisej.Web.DockStyle.Top;
            this.toolbarPanel.Name = "toolbarPanel";
            this.toolbarPanel.Padding = new Wisej.Web.Padding(8, 8, 8, 4);
            this.toolbarPanel.Size = new System.Drawing.Size(1348, 56);
            //
            // toolbarCard
            //
            this.toolbarCard.AppearanceKey = "metric-card";
            this.toolbarCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.toolbarCard.Controls.Add(this.lblAppTitle);
            this.toolbarCard.Controls.Add(this.btnApplyTheme);
            this.toolbarCard.Dock = Wisej.Web.DockStyle.Fill;
            this.toolbarCard.Name = "toolbarCard";
            //
            // lblAppTitle
            //
            this.lblAppTitle.AppearanceKey = "heading-label";
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Location = new System.Drawing.Point(12, 7);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(250, 30);
            this.lblAppTitle.Text = "Adaptive Operations Console";
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnApplyTheme
            //
            this.btnApplyTheme.Location = new System.Drawing.Point(272, 7);
            this.btnApplyTheme.Name = "btnApplyTheme";
            this.btnApplyTheme.Size = new System.Drawing.Size(104, 30);
            this.btnApplyTheme.Text = "Apply theme";
            this.btnApplyTheme.Click += new System.EventHandler(this.btnApplyTheme_Click);
            //
            // navigationPanel
            //
            this.navigationPanel.Controls.Add(this.navigationCard);
            this.navigationPanel.Dock = Wisej.Web.DockStyle.Left;
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Padding = new Wisej.Web.Padding(8, 4, 0, 4);
            this.navigationPanel.Size = new System.Drawing.Size(220, 596);
            //
            // navigationCard
            //
            this.navigationCard.AppearanceKey = "metric-card";
            this.navigationCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.navigationCard.Controls.Add(this.lblNavTitle);
            this.navigationCard.Controls.Add(this.btnNavDashboard);
            this.navigationCard.Controls.Add(this.btnNavTickets);
            this.navigationCard.Controls.Add(this.btnNavReports);
            this.navigationCard.Controls.Add(this.btnNavSettings);
            this.navigationCard.Controls.Add(this.btnNavHelp);
            this.navigationCard.Dock = Wisej.Web.DockStyle.Fill;
            this.navigationCard.Name = "navigationCard";
            //
            // lblNavTitle
            //
            this.lblNavTitle.AppearanceKey = "overline-label";
            this.lblNavTitle.AutoSize = false;
            this.lblNavTitle.Location = new System.Drawing.Point(12, 12);
            this.lblNavTitle.Name = "lblNavTitle";
            this.lblNavTitle.Size = new System.Drawing.Size(186, 18);
            this.lblNavTitle.Text = "NAVIGATION";
            //
            // btnNavDashboard
            //
            this.btnNavDashboard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavDashboard.Location = new System.Drawing.Point(12, 40);
            this.btnNavDashboard.Name = "btnNavDashboard";
            this.btnNavDashboard.Size = new System.Drawing.Size(186, 36);
            this.btnNavDashboard.Text = "Dashboard";
            this.btnNavDashboard.Click += new System.EventHandler(this.btnNav_Click);
            //
            // btnNavTickets
            //
            this.btnNavTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavTickets.Location = new System.Drawing.Point(12, 84);
            this.btnNavTickets.Name = "btnNavTickets";
            this.btnNavTickets.Size = new System.Drawing.Size(186, 36);
            this.btnNavTickets.Text = "Tickets";
            this.btnNavTickets.Click += new System.EventHandler(this.btnNav_Click);
            //
            // btnNavReports
            //
            this.btnNavReports.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavReports.Location = new System.Drawing.Point(12, 128);
            this.btnNavReports.Name = "btnNavReports";
            this.btnNavReports.Size = new System.Drawing.Size(186, 36);
            this.btnNavReports.Text = "Reports";
            this.btnNavReports.Click += new System.EventHandler(this.btnNav_Click);
            //
            // btnNavSettings
            //
            this.btnNavSettings.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavSettings.Location = new System.Drawing.Point(12, 172);
            this.btnNavSettings.Name = "btnNavSettings";
            this.btnNavSettings.Size = new System.Drawing.Size(186, 36);
            this.btnNavSettings.Text = "Settings";
            this.btnNavSettings.Click += new System.EventHandler(this.btnNav_Click);
            //
            // btnNavHelp
            //
            this.btnNavHelp.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavHelp.Location = new System.Drawing.Point(12, 216);
            this.btnNavHelp.Name = "btnNavHelp";
            this.btnNavHelp.Size = new System.Drawing.Size(186, 36);
            this.btnNavHelp.Text = "Help";
            this.btnNavHelp.Click += new System.EventHandler(this.btnNav_Click);
            //
            // workspacePanel
            //
            this.workspacePanel.Controls.Add(this.tabWorkspace);
            this.workspacePanel.Controls.Add(this.metricsPanel);
            this.workspacePanel.Dock = Wisej.Web.DockStyle.Fill;
            this.workspacePanel.MinimumSize = new System.Drawing.Size(320, 240);
            this.workspacePanel.Name = "workspacePanel";
            this.workspacePanel.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.workspacePanel.Size = new System.Drawing.Size(788, 596);
            //
            // metricsPanel
            //
            this.metricsPanel.Controls.Add(this.slotClosed);
            this.metricsPanel.Controls.Add(this.slotMine);
            this.metricsPanel.Controls.Add(this.slotOverdue);
            this.metricsPanel.Controls.Add(this.slotOpen);
            this.metricsPanel.Dock = Wisej.Web.DockStyle.Top;
            this.metricsPanel.Name = "metricsPanel";
            this.metricsPanel.Padding = new Wisej.Web.Padding(0, 0, 0, 8);
            this.metricsPanel.Size = new System.Drawing.Size(772, 84);
            //
            // slotOpen
            //
            this.slotOpen.Controls.Add(this.cardOpen);
            this.slotOpen.Dock = Wisej.Web.DockStyle.Left;
            this.slotOpen.Name = "slotOpen";
            this.slotOpen.Padding = new Wisej.Web.Padding(0, 0, 8, 0);
            this.slotOpen.Size = new System.Drawing.Size(192, 76);
            //
            // cardOpen
            //
            this.cardOpen.AppearanceKey = "metric-card";
            this.cardOpen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardOpen.Controls.Add(this.lblOpenTitle);
            this.cardOpen.Controls.Add(this.lblOpenValue);
            this.cardOpen.Controls.Add(this.stripOpen);
            this.cardOpen.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOpen.Name = "cardOpen";
            //
            // stripOpen
            //
            this.stripOpen.AppearanceKey = "metric-strip";
            this.stripOpen.Dock = Wisej.Web.DockStyle.Top;
            this.stripOpen.Name = "stripOpen";
            this.stripOpen.Size = new System.Drawing.Size(182, 4);
            //
            // lblOpenTitle
            //
            this.lblOpenTitle.AppearanceKey = "overline-label";
            this.lblOpenTitle.AutoSize = false;
            this.lblOpenTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOpenTitle.Name = "lblOpenTitle";
            this.lblOpenTitle.Size = new System.Drawing.Size(160, 16);
            this.lblOpenTitle.Text = "OPEN";
            //
            // lblOpenValue
            //
            this.lblOpenValue.AppearanceKey = "heading-label";
            this.lblOpenValue.AutoSize = false;
            this.lblOpenValue.Location = new System.Drawing.Point(12, 30);
            this.lblOpenValue.Name = "lblOpenValue";
            this.lblOpenValue.Size = new System.Drawing.Size(160, 36);
            this.lblOpenValue.Text = "–";
            this.lblOpenValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // slotOverdue
            //
            this.slotOverdue.Controls.Add(this.cardOverdue);
            this.slotOverdue.Dock = Wisej.Web.DockStyle.Left;
            this.slotOverdue.Name = "slotOverdue";
            this.slotOverdue.Padding = new Wisej.Web.Padding(0, 0, 8, 0);
            this.slotOverdue.Size = new System.Drawing.Size(192, 76);
            //
            // cardOverdue
            //
            this.cardOverdue.AppearanceKey = "metric-card";
            this.cardOverdue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardOverdue.Controls.Add(this.lblOverdueTitle);
            this.cardOverdue.Controls.Add(this.lblOverdueValue);
            this.cardOverdue.Controls.Add(this.stripOverdue);
            this.cardOverdue.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOverdue.Name = "cardOverdue";
            //
            // stripOverdue
            //
            this.stripOverdue.AppearanceKey = "metric-strip";
            this.stripOverdue.Dock = Wisej.Web.DockStyle.Top;
            this.stripOverdue.Name = "stripOverdue";
            this.stripOverdue.Size = new System.Drawing.Size(182, 4);
            this.stripOverdue.States = new string[] { "danger" };
            //
            // lblOverdueTitle
            //
            this.lblOverdueTitle.AppearanceKey = "overline-label";
            this.lblOverdueTitle.AutoSize = false;
            this.lblOverdueTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOverdueTitle.Name = "lblOverdueTitle";
            this.lblOverdueTitle.Size = new System.Drawing.Size(160, 16);
            this.lblOverdueTitle.Text = "OVERDUE";
            //
            // lblOverdueValue
            //
            this.lblOverdueValue.AppearanceKey = "heading-label";
            this.lblOverdueValue.AutoSize = false;
            this.lblOverdueValue.Location = new System.Drawing.Point(12, 30);
            this.lblOverdueValue.Name = "lblOverdueValue";
            this.lblOverdueValue.Size = new System.Drawing.Size(160, 36);
            this.lblOverdueValue.Text = "–";
            this.lblOverdueValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // slotMine
            //
            this.slotMine.Controls.Add(this.cardMine);
            this.slotMine.Dock = Wisej.Web.DockStyle.Left;
            this.slotMine.Name = "slotMine";
            this.slotMine.Padding = new Wisej.Web.Padding(0, 0, 8, 0);
            this.slotMine.Size = new System.Drawing.Size(192, 76);
            //
            // cardMine
            //
            this.cardMine.AppearanceKey = "metric-card";
            this.cardMine.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardMine.Controls.Add(this.lblMineTitle);
            this.cardMine.Controls.Add(this.lblMineValue);
            this.cardMine.Controls.Add(this.stripMine);
            this.cardMine.Dock = Wisej.Web.DockStyle.Fill;
            this.cardMine.Name = "cardMine";
            //
            // stripMine
            //
            this.stripMine.AppearanceKey = "metric-strip";
            this.stripMine.Dock = Wisej.Web.DockStyle.Top;
            this.stripMine.Name = "stripMine";
            this.stripMine.Size = new System.Drawing.Size(182, 4);
            this.stripMine.States = new string[] { "warning" };
            //
            // lblMineTitle
            //
            this.lblMineTitle.AppearanceKey = "overline-label";
            this.lblMineTitle.AutoSize = false;
            this.lblMineTitle.Location = new System.Drawing.Point(12, 12);
            this.lblMineTitle.Name = "lblMineTitle";
            this.lblMineTitle.Size = new System.Drawing.Size(160, 16);
            this.lblMineTitle.Text = "ASSIGNED TO ME";
            //
            // lblMineValue
            //
            this.lblMineValue.AppearanceKey = "heading-label";
            this.lblMineValue.AutoSize = false;
            this.lblMineValue.Location = new System.Drawing.Point(12, 30);
            this.lblMineValue.Name = "lblMineValue";
            this.lblMineValue.Size = new System.Drawing.Size(160, 36);
            this.lblMineValue.Text = "–";
            this.lblMineValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // slotClosed
            //
            this.slotClosed.Controls.Add(this.cardClosed);
            this.slotClosed.Dock = Wisej.Web.DockStyle.Left;
            this.slotClosed.Name = "slotClosed";
            this.slotClosed.Padding = new Wisej.Web.Padding(0, 0, 8, 0);
            this.slotClosed.Size = new System.Drawing.Size(192, 76);
            //
            // cardClosed
            //
            this.cardClosed.AppearanceKey = "metric-card";
            this.cardClosed.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardClosed.Controls.Add(this.lblClosedTitle);
            this.cardClosed.Controls.Add(this.lblClosedValue);
            this.cardClosed.Controls.Add(this.stripClosed);
            this.cardClosed.Dock = Wisej.Web.DockStyle.Fill;
            this.cardClosed.Name = "cardClosed";
            //
            // stripClosed
            //
            this.stripClosed.AppearanceKey = "metric-strip";
            this.stripClosed.Dock = Wisej.Web.DockStyle.Top;
            this.stripClosed.Name = "stripClosed";
            this.stripClosed.Size = new System.Drawing.Size(182, 4);
            this.stripClosed.States = new string[] { "success" };
            //
            // lblClosedTitle
            //
            this.lblClosedTitle.AppearanceKey = "overline-label";
            this.lblClosedTitle.AutoSize = false;
            this.lblClosedTitle.Location = new System.Drawing.Point(12, 12);
            this.lblClosedTitle.Name = "lblClosedTitle";
            this.lblClosedTitle.Size = new System.Drawing.Size(160, 16);
            this.lblClosedTitle.Text = "CLOSED THIS WEEK";
            //
            // lblClosedValue
            //
            this.lblClosedValue.AppearanceKey = "heading-label";
            this.lblClosedValue.AutoSize = false;
            this.lblClosedValue.Location = new System.Drawing.Point(12, 30);
            this.lblClosedValue.Name = "lblClosedValue";
            this.lblClosedValue.Size = new System.Drawing.Size(160, 36);
            this.lblClosedValue.Text = "–";
            this.lblClosedValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tabWorkspace
            //
            this.tabWorkspace.Dock = Wisej.Web.DockStyle.Fill;
            this.tabWorkspace.Name = "tabWorkspace";
            this.tabWorkspace.TabPages.AddRange(new Wisej.Web.TabPage[] {
            this.pageTickets});
            //
            // pageTickets
            //
            this.pageTickets.Controls.Add(this.gridTickets);
            this.pageTickets.Name = "pageTickets";
            this.pageTickets.Padding = new Wisej.Web.Padding(8);
            this.pageTickets.Text = "Tickets";
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
            this.colStatus,
            this.colOwner,
            this.colDue});
            this.gridTickets.Dock = Wisej.Web.DockStyle.Fill;
            this.gridTickets.MultiSelect = false;
            this.gridTickets.Name = "gridTickets";
            this.gridTickets.ReadOnly = true;
            this.gridTickets.RowHeadersVisible = false;
            this.gridTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridTickets.SelectionChanged += new System.EventHandler(this.gridTickets_SelectionChanged);
            //
            // colId
            //
            this.colId.FillWeight = 60F;
            this.colId.HeaderText = "Id";
            this.colId.MinimumWidth = 64;
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            //
            // colTitle
            //
            this.colTitle.FillWeight = 240F;
            this.colTitle.HeaderText = "Title";
            this.colTitle.MinimumWidth = 160;
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            //
            // colPriority
            //
            this.colPriority.FillWeight = 70F;
            this.colPriority.HeaderText = "Priority";
            this.colPriority.MinimumWidth = 64;
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            //
            // colStatus
            //
            this.colStatus.FillWeight = 70F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.MinimumWidth = 64;
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            //
            // colOwner
            //
            this.colOwner.FillWeight = 70F;
            this.colOwner.HeaderText = "Owner";
            this.colOwner.MinimumWidth = 64;
            this.colOwner.Name = "colOwner";
            this.colOwner.ReadOnly = true;
            //
            // colDue
            //
            this.colDue.FillWeight = 90F;
            this.colDue.HeaderText = "Due";
            this.colDue.MinimumWidth = 90;
            this.colDue.Name = "colDue";
            this.colDue.ReadOnly = true;
            //
            // detailsPanel
            //
            this.detailsPanel.Controls.Add(this.detailsCard);
            this.detailsPanel.Dock = Wisej.Web.DockStyle.Right;
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Padding = new Wisej.Web.Padding(0, 4, 8, 4);
            this.detailsPanel.Size = new System.Drawing.Size(340, 596);
            //
            // detailsCard
            //
            this.detailsCard.AppearanceKey = "metric-card";
            this.detailsCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.detailsCard.Controls.Add(this.lblDetailsSubtitle);
            this.detailsCard.Controls.Add(this.lblTitleCaption);
            this.detailsCard.Controls.Add(this.txtTitle);
            this.detailsCard.Controls.Add(this.lblPriorityCaption);
            this.detailsCard.Controls.Add(this.cboPriority);
            this.detailsCard.Controls.Add(this.lblStatusCaption);
            this.detailsCard.Controls.Add(this.cboStatus);
            this.detailsCard.Controls.Add(this.lblOwnerCaption);
            this.detailsCard.Controls.Add(this.txtOwner);
            this.detailsCard.Controls.Add(this.lblDueCaption);
            this.detailsCard.Controls.Add(this.dtpDue);
            this.detailsCard.Controls.Add(this.lblNotesCaption);
            this.detailsCard.Controls.Add(this.txtNotes);
            this.detailsCard.Controls.Add(this.btnSave);
            this.detailsCard.Dock = Wisej.Web.DockStyle.Fill;
            this.detailsCard.Name = "detailsCard";
            this.detailsCard.ShowHeader = true;
            this.detailsCard.Size = new System.Drawing.Size(332, 588);
            this.detailsCard.Text = "Ticket details";
            //
            // lblDetailsSubtitle
            //
            this.lblDetailsSubtitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblDetailsSubtitle.AppearanceKey = "muted-label";
            this.lblDetailsSubtitle.AutoEllipsis = true;
            this.lblDetailsSubtitle.AutoSize = false;
            this.lblDetailsSubtitle.Location = new System.Drawing.Point(12, 10);
            this.lblDetailsSubtitle.Name = "lblDetailsSubtitle";
            this.lblDetailsSubtitle.Size = new System.Drawing.Size(306, 18);
            this.lblDetailsSubtitle.Text = "Select a ticket in the grid";
            //
            // lblTitleCaption
            //
            this.lblTitleCaption.AutoSize = false;
            this.lblTitleCaption.Location = new System.Drawing.Point(12, 38);
            this.lblTitleCaption.Name = "lblTitleCaption";
            this.lblTitleCaption.Size = new System.Drawing.Size(140, 16);
            this.lblTitleCaption.Text = "Title";
            //
            // txtTitle
            //
            this.txtTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtTitle.Location = new System.Drawing.Point(12, 56);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(306, 28);
            this.txtTitle.Watermark = "Short summary (required, max 80 characters)";
            //
            // lblPriorityCaption
            //
            this.lblPriorityCaption.AutoSize = false;
            this.lblPriorityCaption.Location = new System.Drawing.Point(12, 94);
            this.lblPriorityCaption.Name = "lblPriorityCaption";
            this.lblPriorityCaption.Size = new System.Drawing.Size(140, 16);
            this.lblPriorityCaption.Text = "Priority";
            //
            // cboPriority
            //
            this.cboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPriority.Location = new System.Drawing.Point(12, 112);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Size = new System.Drawing.Size(146, 28);
            //
            // lblStatusCaption
            //
            this.lblStatusCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.Location = new System.Drawing.Point(172, 94);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(140, 16);
            this.lblStatusCaption.Text = "Status";
            //
            // cboStatus
            //
            this.cboStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(172, 112);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(146, 28);
            //
            // lblOwnerCaption
            //
            this.lblOwnerCaption.AutoSize = false;
            this.lblOwnerCaption.Location = new System.Drawing.Point(12, 150);
            this.lblOwnerCaption.Name = "lblOwnerCaption";
            this.lblOwnerCaption.Size = new System.Drawing.Size(140, 16);
            this.lblOwnerCaption.Text = "Owner";
            //
            // txtOwner
            //
            this.txtOwner.Location = new System.Drawing.Point(12, 168);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.Size = new System.Drawing.Size(146, 28);
            this.txtOwner.Watermark = "Agent name (required)";
            //
            // lblDueCaption
            //
            this.lblDueCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblDueCaption.AutoSize = false;
            this.lblDueCaption.Location = new System.Drawing.Point(172, 150);
            this.lblDueCaption.Name = "lblDueCaption";
            this.lblDueCaption.Size = new System.Drawing.Size(140, 16);
            this.lblDueCaption.Text = "Due";
            //
            // dtpDue
            //
            this.dtpDue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.dtpDue.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpDue.Location = new System.Drawing.Point(172, 168);
            this.dtpDue.Name = "dtpDue";
            this.dtpDue.Size = new System.Drawing.Size(146, 28);
            //
            // lblNotesCaption
            //
            this.lblNotesCaption.AutoSize = false;
            this.lblNotesCaption.Location = new System.Drawing.Point(12, 206);
            this.lblNotesCaption.Name = "lblNotesCaption";
            this.lblNotesCaption.Size = new System.Drawing.Size(140, 16);
            this.lblNotesCaption.Text = "Notes";
            //
            // txtNotes
            //
            this.txtNotes.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtNotes.Location = new System.Drawing.Point(12, 224);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(306, 278);
            this.txtNotes.Watermark = "Anything the next agent needs";
            //
            // btnSave
            //
            this.btnSave.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnSave.Location = new System.Drawing.Point(12, 514);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 32);
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "Save the ticket";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // statusPanel
            //
            this.statusPanel.Controls.Add(this.statusCard);
            this.statusPanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Padding = new Wisej.Web.Padding(8, 0, 8, 4);
            this.statusPanel.Size = new System.Drawing.Size(1348, 28);
            //
            // statusCard
            //
            this.statusCard.AppearanceKey = "metric-card";
            this.statusCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.statusCard.Controls.Add(this.widthLabel);
            this.statusCard.Controls.Add(this.lblStatus);
            this.statusCard.Dock = Wisej.Web.DockStyle.Fill;
            this.statusCard.Name = "statusCard";
            this.statusCard.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            //
            // lblStatus
            //
            this.lblStatus.AppearanceKey = "status-label";
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Left;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(420, 22);
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // widthLabel
            //
            this.widthLabel.AppearanceKey = "mono-label";
            this.widthLabel.AutoEllipsis = true;
            this.widthLabel.AutoSize = false;
            this.widthLabel.Dock = Wisej.Web.DockStyle.Fill;
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Text = "Width: –";
            this.widthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // MainPage
            //
            this.Controls.Add(this.workspacePanel);
            this.Controls.Add(this.detailsPanel);
            this.Controls.Add(this.navigationPanel);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.toolbarPanel);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "Adaptive Operations Console";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.Resize += new System.EventHandler(this.MainPage_Resize);
            this.toolbarPanel.ResumeLayout(false);
            this.toolbarCard.ResumeLayout(false);
            this.navigationPanel.ResumeLayout(false);
            this.navigationCard.ResumeLayout(false);
            this.workspacePanel.ResumeLayout(false);
            this.metricsPanel.ResumeLayout(false);
            this.slotOpen.ResumeLayout(false);
            this.cardOpen.ResumeLayout(false);
            this.slotOverdue.ResumeLayout(false);
            this.cardOverdue.ResumeLayout(false);
            this.slotMine.ResumeLayout(false);
            this.cardMine.ResumeLayout(false);
            this.slotClosed.ResumeLayout(false);
            this.cardClosed.ResumeLayout(false);
            this.tabWorkspace.ResumeLayout(false);
            this.pageTickets.ResumeLayout(false);
            this.detailsPanel.ResumeLayout(false);
            this.detailsCard.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.statusCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel toolbarPanel;
        private Wisej.Web.Panel navigationPanel;
        private Wisej.Web.Panel workspacePanel;
        private Wisej.Web.Panel detailsPanel;
        private Wisej.Web.Panel statusPanel;
        private Wisej.Web.Panel toolbarCard;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Button btnApplyTheme;
        private Wisej.Web.Panel navigationCard;
        private Wisej.Web.Label lblNavTitle;
        private Wisej.Web.Button btnNavDashboard;
        private Wisej.Web.Button btnNavTickets;
        private Wisej.Web.Button btnNavReports;
        private Wisej.Web.Button btnNavSettings;
        private Wisej.Web.Button btnNavHelp;
        private Wisej.Web.Panel metricsPanel;
        private Wisej.Web.Panel slotOpen;
        private Wisej.Web.Panel cardOpen;
        private Wisej.Web.Panel stripOpen;
        private Wisej.Web.Label lblOpenTitle;
        private Wisej.Web.Label lblOpenValue;
        private Wisej.Web.Panel slotOverdue;
        private Wisej.Web.Panel cardOverdue;
        private Wisej.Web.Panel stripOverdue;
        private Wisej.Web.Label lblOverdueTitle;
        private Wisej.Web.Label lblOverdueValue;
        private Wisej.Web.Panel slotMine;
        private Wisej.Web.Panel cardMine;
        private Wisej.Web.Panel stripMine;
        private Wisej.Web.Label lblMineTitle;
        private Wisej.Web.Label lblMineValue;
        private Wisej.Web.Panel slotClosed;
        private Wisej.Web.Panel cardClosed;
        private Wisej.Web.Panel stripClosed;
        private Wisej.Web.Label lblClosedTitle;
        private Wisej.Web.Label lblClosedValue;
        private Wisej.Web.TabControl tabWorkspace;
        private Wisej.Web.TabPage pageTickets;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colOwner;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.Panel detailsCard;
        private Wisej.Web.Label lblDetailsSubtitle;
        private Wisej.Web.Label lblTitleCaption;
        private Wisej.Web.TextBox txtTitle;
        private Wisej.Web.Label lblPriorityCaption;
        private Wisej.Web.ComboBox cboPriority;
        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.Label lblOwnerCaption;
        private Wisej.Web.TextBox txtOwner;
        private Wisej.Web.Label lblDueCaption;
        private Wisej.Web.DateTimePicker dtpDue;
        private Wisej.Web.Label lblNotesCaption;
        private Wisej.Web.TextBox txtNotes;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Panel statusCard;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label widthLabel;
    }
}
