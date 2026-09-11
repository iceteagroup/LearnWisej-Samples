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
            this.btnTheme = new Wisej.Web.Button();
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
            this.slaPanel = new Wisej.Web.Panel();
            this.slaCard = new Wisej.Web.Panel();
            this.lblSlaCaption = new Wisej.Web.Label();
            this.barTrack = new Wisej.Web.Panel();
            this.barFill = new Wisej.Web.Panel();
            this.lblSlaValue = new Wisej.Web.Label();
            this.gridCard = new Wisej.Web.Panel();
            this.lblWorkspaceTitle = new Wisej.Web.Label();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.detailsPanel = new Wisej.Web.Panel();
            this.detailsCard = new Wisej.Web.Panel();
            this.lblDetailsTitle = new Wisej.Web.Label();
            this.lblDetailsSubtitle = new Wisej.Web.Label();
            this.lblPriorityBadge = new Wisej.Web.Label();
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
            this.slaPanel.SuspendLayout();
            this.slaCard.SuspendLayout();
            this.barTrack.SuspendLayout();
            this.gridCard.SuspendLayout();
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
            this.toolbarCard.AppearanceKey = "card";
            this.toolbarCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.toolbarCard.Controls.Add(this.lblAppTitle);
            this.toolbarCard.Controls.Add(this.btnTheme);
            this.toolbarCard.Dock = Wisej.Web.DockStyle.Fill;
            this.toolbarCard.Name = "toolbarCard";
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.Location = new System.Drawing.Point(12, 7);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(250, 30);
            this.lblAppTitle.Text = "Adaptive Operations Console";
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnTheme
            //
            this.btnTheme.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnTheme.Location = new System.Drawing.Point(1198, 7);
            this.btnTheme.Name = "btnTheme";
            this.btnTheme.Size = new System.Drawing.Size(120, 30);
            this.btnTheme.Text = "Dark theme";
            this.btnTheme.Click += new System.EventHandler(this.btnTheme_Click);
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
            this.navigationCard.AppearanceKey = "card";
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
            this.lblNavTitle.AppearanceKey = "muted-label";
            this.lblNavTitle.AutoSize = false;
            this.lblNavTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
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
            this.workspacePanel.Controls.Add(this.gridCard);
            this.workspacePanel.Controls.Add(this.slaPanel);
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
            this.cardOpen.CssClass = "metric-card";
            this.cardOpen.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOpen.Name = "cardOpen";
            //
            // stripOpen
            //
            this.stripOpen.CssClass = "metric-strip strip-open";
            this.stripOpen.Dock = Wisej.Web.DockStyle.Top;
            this.stripOpen.Name = "stripOpen";
            this.stripOpen.Size = new System.Drawing.Size(182, 4);
            //
            // lblOpenTitle
            //
            this.lblOpenTitle.AppearanceKey = "metric-title";
            this.lblOpenTitle.AutoSize = false;
            this.lblOpenTitle.CssClass = "metric-title";
            this.lblOpenTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOpenTitle.Name = "lblOpenTitle";
            this.lblOpenTitle.Size = new System.Drawing.Size(160, 16);
            this.lblOpenTitle.Text = "Open";
            //
            // lblOpenValue
            //
            this.lblOpenValue.AppearanceKey = "metric-value";
            this.lblOpenValue.AutoSize = false;
            this.lblOpenValue.Location = new System.Drawing.Point(12, 30);
            this.lblOpenValue.Name = "lblOpenValue";
            this.lblOpenValue.Size = new System.Drawing.Size(160, 36);
            this.lblOpenValue.Text = "–";
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
            this.cardOverdue.CssClass = "metric-card";
            this.cardOverdue.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOverdue.Name = "cardOverdue";
            //
            // stripOverdue
            //
            this.stripOverdue.CssClass = "metric-strip strip-overdue";
            this.stripOverdue.Dock = Wisej.Web.DockStyle.Top;
            this.stripOverdue.Name = "stripOverdue";
            this.stripOverdue.Size = new System.Drawing.Size(182, 4);
            //
            // lblOverdueTitle
            //
            this.lblOverdueTitle.AppearanceKey = "metric-title";
            this.lblOverdueTitle.AutoSize = false;
            this.lblOverdueTitle.CssClass = "metric-title";
            this.lblOverdueTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOverdueTitle.Name = "lblOverdueTitle";
            this.lblOverdueTitle.Size = new System.Drawing.Size(160, 16);
            this.lblOverdueTitle.Text = "Overdue";
            //
            // lblOverdueValue
            //
            this.lblOverdueValue.AppearanceKey = "metric-value";
            this.lblOverdueValue.AutoSize = false;
            this.lblOverdueValue.Location = new System.Drawing.Point(12, 30);
            this.lblOverdueValue.Name = "lblOverdueValue";
            this.lblOverdueValue.Size = new System.Drawing.Size(160, 36);
            this.lblOverdueValue.Text = "–";
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
            this.cardMine.CssClass = "metric-card";
            this.cardMine.Dock = Wisej.Web.DockStyle.Fill;
            this.cardMine.Name = "cardMine";
            //
            // stripMine
            //
            this.stripMine.CssClass = "metric-strip strip-mine";
            this.stripMine.Dock = Wisej.Web.DockStyle.Top;
            this.stripMine.Name = "stripMine";
            this.stripMine.Size = new System.Drawing.Size(182, 4);
            //
            // lblMineTitle
            //
            this.lblMineTitle.AppearanceKey = "metric-title";
            this.lblMineTitle.AutoSize = false;
            this.lblMineTitle.CssClass = "metric-title";
            this.lblMineTitle.Location = new System.Drawing.Point(12, 12);
            this.lblMineTitle.Name = "lblMineTitle";
            this.lblMineTitle.Size = new System.Drawing.Size(160, 16);
            this.lblMineTitle.Text = "Assigned to me";
            //
            // lblMineValue
            //
            this.lblMineValue.AppearanceKey = "metric-value";
            this.lblMineValue.AutoSize = false;
            this.lblMineValue.Location = new System.Drawing.Point(12, 30);
            this.lblMineValue.Name = "lblMineValue";
            this.lblMineValue.Size = new System.Drawing.Size(160, 36);
            this.lblMineValue.Text = "–";
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
            this.cardClosed.CssClass = "metric-card";
            this.cardClosed.Dock = Wisej.Web.DockStyle.Fill;
            this.cardClosed.Name = "cardClosed";
            //
            // stripClosed
            //
            this.stripClosed.CssClass = "metric-strip strip-closed";
            this.stripClosed.Dock = Wisej.Web.DockStyle.Top;
            this.stripClosed.Name = "stripClosed";
            this.stripClosed.Size = new System.Drawing.Size(182, 4);
            //
            // lblClosedTitle
            //
            this.lblClosedTitle.AppearanceKey = "metric-title";
            this.lblClosedTitle.AutoSize = false;
            this.lblClosedTitle.CssClass = "metric-title";
            this.lblClosedTitle.Location = new System.Drawing.Point(12, 12);
            this.lblClosedTitle.Name = "lblClosedTitle";
            this.lblClosedTitle.Size = new System.Drawing.Size(160, 16);
            this.lblClosedTitle.Text = "Closed this week";
            //
            // lblClosedValue
            //
            this.lblClosedValue.AppearanceKey = "metric-value";
            this.lblClosedValue.AutoSize = false;
            this.lblClosedValue.Location = new System.Drawing.Point(12, 30);
            this.lblClosedValue.Name = "lblClosedValue";
            this.lblClosedValue.Size = new System.Drawing.Size(160, 36);
            this.lblClosedValue.Text = "–";
            //
            // slaPanel
            //
            this.slaPanel.Controls.Add(this.slaCard);
            this.slaPanel.Dock = Wisej.Web.DockStyle.Top;
            this.slaPanel.Name = "slaPanel";
            this.slaPanel.Padding = new Wisej.Web.Padding(0, 0, 0, 8);
            this.slaPanel.Size = new System.Drawing.Size(772, 48);
            //
            // slaCard
            //
            this.slaCard.AppearanceKey = "card";
            this.slaCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.slaCard.Controls.Add(this.barTrack);
            this.slaCard.Controls.Add(this.lblSlaValue);
            this.slaCard.Controls.Add(this.lblSlaCaption);
            this.slaCard.Dock = Wisej.Web.DockStyle.Fill;
            this.slaCard.Name = "slaCard";
            this.slaCard.Padding = new Wisej.Web.Padding(12, 10, 12, 10);
            //
            // lblSlaCaption
            //
            this.lblSlaCaption.AppearanceKey = "muted-label";
            this.lblSlaCaption.AutoSize = false;
            this.lblSlaCaption.Dock = Wisej.Web.DockStyle.Left;
            this.lblSlaCaption.Name = "lblSlaCaption";
            this.lblSlaCaption.Size = new System.Drawing.Size(100, 18);
            this.lblSlaCaption.Text = "SLA today";
            this.lblSlaCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // barTrack
            //
            this.barTrack.BorderStyle = Wisej.Web.BorderStyle.None;
            this.barTrack.Controls.Add(this.barFill);
            this.barTrack.CssClass = "progress-track";
            this.barTrack.Dock = Wisej.Web.DockStyle.Fill;
            this.barTrack.Name = "barTrack";
            //
            // barFill
            //
            this.barFill.BorderStyle = Wisej.Web.BorderStyle.None;
            this.barFill.CssClass = "progress-fill";
            this.barFill.Dock = Wisej.Web.DockStyle.Fill;
            this.barFill.Name = "barFill";
            //
            // lblSlaValue
            //
            this.lblSlaValue.AppearanceKey = "metric-title";
            this.lblSlaValue.AutoSize = false;
            this.lblSlaValue.Dock = Wisej.Web.DockStyle.Right;
            this.lblSlaValue.Name = "lblSlaValue";
            this.lblSlaValue.Size = new System.Drawing.Size(72, 18);
            this.lblSlaValue.Text = "– %";
            this.lblSlaValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridCard
            //
            this.gridCard.AppearanceKey = "card";
            this.gridCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.gridCard.Controls.Add(this.gridTickets);
            this.gridCard.Controls.Add(this.lblWorkspaceTitle);
            this.gridCard.Dock = Wisej.Web.DockStyle.Fill;
            this.gridCard.Name = "gridCard";
            this.gridCard.Padding = new Wisej.Web.Padding(8);
            //
            // lblWorkspaceTitle
            //
            this.lblWorkspaceTitle.AutoSize = false;
            this.lblWorkspaceTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblWorkspaceTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblWorkspaceTitle.Name = "lblWorkspaceTitle";
            this.lblWorkspaceTitle.Size = new System.Drawing.Size(754, 26);
            this.lblWorkspaceTitle.Text = "Tickets";
            this.lblWorkspaceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.detailsCard.AppearanceKey = "card";
            this.detailsCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.detailsCard.Controls.Add(this.lblDetailsTitle);
            this.detailsCard.Controls.Add(this.lblDetailsSubtitle);
            this.detailsCard.Controls.Add(this.lblPriorityBadge);
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
            this.detailsCard.Size = new System.Drawing.Size(332, 588);
            //
            // lblDetailsTitle
            //
            this.lblDetailsTitle.AutoSize = false;
            this.lblDetailsTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblDetailsTitle.Location = new System.Drawing.Point(12, 12);
            this.lblDetailsTitle.Name = "lblDetailsTitle";
            this.lblDetailsTitle.Size = new System.Drawing.Size(306, 24);
            this.lblDetailsTitle.Text = "Ticket details";
            //
            // lblDetailsSubtitle
            //
            this.lblDetailsSubtitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblDetailsSubtitle.AppearanceKey = "muted-label";
            this.lblDetailsSubtitle.AutoEllipsis = true;
            this.lblDetailsSubtitle.AutoSize = false;
            this.lblDetailsSubtitle.Location = new System.Drawing.Point(12, 38);
            this.lblDetailsSubtitle.Name = "lblDetailsSubtitle";
            this.lblDetailsSubtitle.Size = new System.Drawing.Size(220, 20);
            this.lblDetailsSubtitle.Text = "Select a ticket in the grid";
            this.lblDetailsSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblPriorityBadge
            //
            this.lblPriorityBadge.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblPriorityBadge.AutoSize = false;
            this.lblPriorityBadge.CssClass = "compact-badge";
            this.lblPriorityBadge.Location = new System.Drawing.Point(238, 38);
            this.lblPriorityBadge.Name = "lblPriorityBadge";
            this.lblPriorityBadge.Size = new System.Drawing.Size(80, 20);
            this.lblPriorityBadge.Text = "";
            this.lblPriorityBadge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPriorityBadge.ToolTipText = "Priority of the selected ticket";
            //
            // lblTitleCaption
            //
            this.lblTitleCaption.AutoSize = false;
            this.lblTitleCaption.Location = new System.Drawing.Point(12, 66);
            this.lblTitleCaption.Name = "lblTitleCaption";
            this.lblTitleCaption.Size = new System.Drawing.Size(140, 16);
            this.lblTitleCaption.Text = "Title";
            //
            // txtTitle
            //
            this.txtTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtTitle.Location = new System.Drawing.Point(12, 84);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(306, 28);
            this.txtTitle.Watermark = "Short summary (required, max 80 characters)";
            //
            // lblPriorityCaption
            //
            this.lblPriorityCaption.AutoSize = false;
            this.lblPriorityCaption.Location = new System.Drawing.Point(12, 122);
            this.lblPriorityCaption.Name = "lblPriorityCaption";
            this.lblPriorityCaption.Size = new System.Drawing.Size(140, 16);
            this.lblPriorityCaption.Text = "Priority";
            //
            // cboPriority
            //
            this.cboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPriority.Location = new System.Drawing.Point(12, 140);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Size = new System.Drawing.Size(146, 28);
            //
            // lblStatusCaption
            //
            this.lblStatusCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.Location = new System.Drawing.Point(172, 122);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(140, 16);
            this.lblStatusCaption.Text = "Status";
            //
            // cboStatus
            //
            this.cboStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(172, 140);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(146, 28);
            //
            // lblOwnerCaption
            //
            this.lblOwnerCaption.AutoSize = false;
            this.lblOwnerCaption.Location = new System.Drawing.Point(12, 178);
            this.lblOwnerCaption.Name = "lblOwnerCaption";
            this.lblOwnerCaption.Size = new System.Drawing.Size(140, 16);
            this.lblOwnerCaption.Text = "Owner";
            //
            // txtOwner
            //
            this.txtOwner.Location = new System.Drawing.Point(12, 196);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.Size = new System.Drawing.Size(146, 28);
            this.txtOwner.Watermark = "Agent name (required)";
            //
            // lblDueCaption
            //
            this.lblDueCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblDueCaption.AutoSize = false;
            this.lblDueCaption.Location = new System.Drawing.Point(172, 178);
            this.lblDueCaption.Name = "lblDueCaption";
            this.lblDueCaption.Size = new System.Drawing.Size(140, 16);
            this.lblDueCaption.Text = "Due";
            //
            // dtpDue
            //
            this.dtpDue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.dtpDue.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpDue.Location = new System.Drawing.Point(172, 196);
            this.dtpDue.Name = "dtpDue";
            this.dtpDue.Size = new System.Drawing.Size(146, 28);
            //
            // lblNotesCaption
            //
            this.lblNotesCaption.AutoSize = false;
            this.lblNotesCaption.Location = new System.Drawing.Point(12, 234);
            this.lblNotesCaption.Name = "lblNotesCaption";
            this.lblNotesCaption.Size = new System.Drawing.Size(140, 16);
            this.lblNotesCaption.Text = "Notes";
            //
            // txtNotes
            //
            this.txtNotes.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtNotes.Location = new System.Drawing.Point(12, 252);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(306, 278);
            this.txtNotes.Watermark = "Anything the next agent needs";
            //
            // btnSave
            //
            this.btnSave.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnSave.AppearanceKey = "action-button";
            this.btnSave.Location = new System.Drawing.Point(12, 542);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 32);
            this.btnSave.Text = "Save";
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
            this.statusCard.AppearanceKey = "card";
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
            this.widthLabel.AutoEllipsis = true;
            this.widthLabel.AutoSize = false;
            this.widthLabel.Dock = Wisej.Web.DockStyle.Fill;
            this.widthLabel.Font = new System.Drawing.Font("monospace", 9F);
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
            this.slaPanel.ResumeLayout(false);
            this.slaCard.ResumeLayout(false);
            this.barTrack.ResumeLayout(false);
            this.gridCard.ResumeLayout(false);
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
        private Wisej.Web.Button btnTheme;
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
        private Wisej.Web.Panel slaPanel;
        private Wisej.Web.Panel slaCard;
        private Wisej.Web.Label lblSlaCaption;
        private Wisej.Web.Panel barTrack;
        private Wisej.Web.Panel barFill;
        private Wisej.Web.Label lblSlaValue;
        private Wisej.Web.Panel gridCard;
        private Wisej.Web.Label lblWorkspaceTitle;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colOwner;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.Panel detailsCard;
        private Wisej.Web.Label lblDetailsTitle;
        private Wisej.Web.Label lblDetailsSubtitle;
        private Wisej.Web.Label lblPriorityBadge;
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
