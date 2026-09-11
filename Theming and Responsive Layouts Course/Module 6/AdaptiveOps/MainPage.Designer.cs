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
            if (disposing)
            {
                // Application.* events are session-level and outlive the page: unsubscribe with the page.
                Wisej.Web.Application.BrowserSizeChanged -= this.Application_BrowserSizeChanged;
                Wisej.Web.Application.ResponsiveProfileChanged -= this.Application_ResponsiveProfileChanged;

                // The reusable phone dialog is not in Controls, so it is not disposed with the page automatically.
                if (this._editorForm != null)
                {
                    this._editorForm.Dispose();
                    this._editorForm = null;
                }

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.toolbarPanel = new Wisej.Web.Panel();
            this.toolbarCard = new Wisej.Web.Panel();
            this.toolbarFlow = new Wisej.Web.FlowLayoutPanel();
            this.btnMenu = new Wisej.Web.Button();
            this.lblAppTitle = new Wisej.Web.Label();
            this.btnRefresh = new Wisej.Web.Button();
            this.btnReapply = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.navigationPanel = new Wisej.Web.Panel();
            this.navigationRail = new AdaptiveOps.Shell.NavigationRail();
            this.workspacePanel = new Wisej.Web.Panel();
            this.metricsTable = new Wisej.Web.TableLayoutPanel();
            this.cardOpen = new Wisej.Web.Panel();
            this.stripOpen = new Wisej.Web.Panel();
            this.lblOpenTitle = new Wisej.Web.Label();
            this.lblOpenValue = new Wisej.Web.Label();
            this.cardOverdue = new Wisej.Web.Panel();
            this.stripOverdue = new Wisej.Web.Panel();
            this.lblOverdueTitle = new Wisej.Web.Label();
            this.lblOverdueValue = new Wisej.Web.Label();
            this.cardMine = new Wisej.Web.Panel();
            this.stripMine = new Wisej.Web.Panel();
            this.lblMineTitle = new Wisej.Web.Label();
            this.lblMineValue = new Wisej.Web.Label();
            this.cardClosed = new Wisej.Web.Panel();
            this.stripClosed = new Wisej.Web.Panel();
            this.lblClosedTitle = new Wisej.Web.Label();
            this.lblClosedValue = new Wisej.Web.Label();
            this.gridCard = new Wisej.Web.Panel();
            this.lblWorkspaceTitle = new Wisej.Web.Label();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.logPanel = new Wisej.Web.Panel();
            this.logCard = new Wisej.Web.Panel();
            this.lblLogTitle = new Wisej.Web.Label();
            this.lstProfileLog = new Wisej.Web.ListBox();
            this.detailsPanel = new Wisej.Web.Panel();
            this.detailsCard = new Wisej.Web.Panel();
            this.ticketEditor = new AdaptiveOps.Shell.TicketEditor();
            this.statusPanel = new Wisej.Web.Panel();
            this.statusCard = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.lblProfile = new Wisej.Web.Label();
            this.toolbarPanel.SuspendLayout();
            this.toolbarCard.SuspendLayout();
            this.toolbarFlow.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            this.workspacePanel.SuspendLayout();
            this.metricsTable.SuspendLayout();
            this.cardOpen.SuspendLayout();
            this.cardOverdue.SuspendLayout();
            this.cardMine.SuspendLayout();
            this.cardClosed.SuspendLayout();
            this.gridCard.SuspendLayout();
            this.logPanel.SuspendLayout();
            this.logCard.SuspendLayout();
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
            this.toolbarCard.BackColor = System.Drawing.Color.White;
            this.toolbarCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.toolbarCard.Controls.Add(this.toolbarFlow);
            this.toolbarCard.Dock = Wisej.Web.DockStyle.Fill;
            this.toolbarCard.Name = "toolbarCard";
            //
            // toolbarFlow  (a FlowLayoutPanel, so a button that changes width or visibility per profile re-flows its neighbours)
            //
            this.toolbarFlow.Controls.Add(this.btnMenu);
            this.toolbarFlow.Controls.Add(this.lblAppTitle);
            this.toolbarFlow.Controls.Add(this.btnRefresh);
            this.toolbarFlow.Controls.Add(this.btnReapply);
            this.toolbarFlow.Controls.Add(this.btnClearTrace);
            this.toolbarFlow.Dock = Wisej.Web.DockStyle.Fill;
            this.toolbarFlow.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.toolbarFlow.Name = "toolbarFlow";
            this.toolbarFlow.Padding = new Wisej.Web.Padding(6, 6, 6, 0);
            this.toolbarFlow.WrapContents = false;
            //
            // btnMenu  (Phone only: the rail is hidden, its sections come back as a menu)
            //
            this.btnMenu.Display = Wisej.Web.Display.Icon;
            this.btnMenu.ImageSource = "icon-justify-fill";
            this.btnMenu.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(36, 30);
            this.btnMenu.Text = "Menu";
            this.btnMenu.ToolTipText = "Menu";
            this.btnMenu.Visible = false;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.Margin = new Wisej.Web.Padding(4, 0, 12, 0);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(250, 30);
            this.lblAppTitle.Text = "Adaptive Operations Console";
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnRefresh
            //
            this.btnRefresh.ImageSource = "icon-refresh";
            this.btnRefresh.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(92, 30);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.ToolTipText = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnReapply
            //
            this.btnReapply.ImageSource = "icon-undo";
            this.btnReapply.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.btnReapply.Name = "btnReapply";
            this.btnReapply.Size = new System.Drawing.Size(96, 30);
            this.btnReapply.Text = "Re-apply";
            this.btnReapply.ToolTipText = "Re-apply profile";
            this.btnReapply.Click += new System.EventHandler(this.btnReapply_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.ImageSource = "icon-close";
            this.btnClearTrace.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(96, 30);
            this.btnClearTrace.Text = "Clear log";
            this.btnClearTrace.ToolTipText = "Clear log";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // navigationPanel  (220 px full, 64 px icon-only, hidden on Phone)
            //
            this.navigationPanel.Controls.Add(this.navigationRail);
            this.navigationPanel.Dock = Wisej.Web.DockStyle.Left;
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Padding = new Wisej.Web.Padding(8, 4, 0, 4);
            this.navigationPanel.Size = new System.Drawing.Size(220, 596);
            //
            // navigationRail
            //
            this.navigationRail.Dock = Wisej.Web.DockStyle.Fill;
            this.navigationRail.Name = "navigationRail";
            this.navigationRail.SectionSelected += new System.EventHandler<AdaptiveOps.Shell.SectionEventArgs>(this.navigationRail_SectionSelected);
            //
            // workspacePanel
            //
            this.workspacePanel.Controls.Add(this.gridCard);
            this.workspacePanel.Controls.Add(this.logPanel);
            this.workspacePanel.Controls.Add(this.metricsTable);
            this.workspacePanel.Dock = Wisej.Web.DockStyle.Fill;
            this.workspacePanel.MinimumSize = new System.Drawing.Size(320, 240);
            this.workspacePanel.Name = "workspacePanel";
            this.workspacePanel.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.workspacePanel.Size = new System.Drawing.Size(788, 596);
            //
            // metricsTable  (4×1, or 2×2 on Phone: ApplyProfile changes the grid shape, never a card's Bounds)
            //
            this.metricsTable.ColumnCount = 4;
            this.metricsTable.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            this.metricsTable.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            this.metricsTable.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            this.metricsTable.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            this.metricsTable.Controls.Add(this.cardOpen, 0, 0);
            this.metricsTable.Controls.Add(this.cardOverdue, 1, 0);
            this.metricsTable.Controls.Add(this.cardMine, 2, 0);
            this.metricsTable.Controls.Add(this.cardClosed, 3, 0);
            this.metricsTable.Dock = Wisej.Web.DockStyle.Top;
            this.metricsTable.Name = "metricsTable";
            this.metricsTable.RowCount = 1;
            this.metricsTable.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
            this.metricsTable.Size = new System.Drawing.Size(772, 84);
            //
            // cardOpen
            //
            this.cardOpen.BackColor = System.Drawing.Color.White;
            this.cardOpen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardOpen.Controls.Add(this.lblOpenTitle);
            this.cardOpen.Controls.Add(this.lblOpenValue);
            this.cardOpen.Controls.Add(this.stripOpen);
            this.cardOpen.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOpen.Margin = new Wisej.Web.Padding(0, 0, 8, 8);
            this.cardOpen.Name = "cardOpen";
            //
            // stripOpen
            //
            this.stripOpen.BackColor = System.Drawing.Color.FromArgb(36, 84, 166);
            this.stripOpen.Dock = Wisej.Web.DockStyle.Top;
            this.stripOpen.Name = "stripOpen";
            this.stripOpen.Size = new System.Drawing.Size(182, 4);
            //
            // lblOpenTitle
            //
            this.lblOpenTitle.AutoSize = false;
            this.lblOpenTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblOpenTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblOpenTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOpenTitle.Name = "lblOpenTitle";
            this.lblOpenTitle.Size = new System.Drawing.Size(140, 16);
            this.lblOpenTitle.Text = "OPEN";
            //
            // lblOpenValue
            //
            this.lblOpenValue.AutoSize = false;
            this.lblOpenValue.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.lblOpenValue.Location = new System.Drawing.Point(12, 30);
            this.lblOpenValue.Name = "lblOpenValue";
            this.lblOpenValue.Size = new System.Drawing.Size(140, 36);
            this.lblOpenValue.Text = "–";
            //
            // cardOverdue
            //
            this.cardOverdue.BackColor = System.Drawing.Color.White;
            this.cardOverdue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardOverdue.Controls.Add(this.lblOverdueTitle);
            this.cardOverdue.Controls.Add(this.lblOverdueValue);
            this.cardOverdue.Controls.Add(this.stripOverdue);
            this.cardOverdue.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOverdue.Margin = new Wisej.Web.Padding(0, 0, 8, 8);
            this.cardOverdue.Name = "cardOverdue";
            //
            // stripOverdue
            //
            this.stripOverdue.BackColor = System.Drawing.Color.FromArgb(180, 35, 24);
            this.stripOverdue.Dock = Wisej.Web.DockStyle.Top;
            this.stripOverdue.Name = "stripOverdue";
            this.stripOverdue.Size = new System.Drawing.Size(182, 4);
            //
            // lblOverdueTitle
            //
            this.lblOverdueTitle.AutoSize = false;
            this.lblOverdueTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblOverdueTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblOverdueTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOverdueTitle.Name = "lblOverdueTitle";
            this.lblOverdueTitle.Size = new System.Drawing.Size(140, 16);
            this.lblOverdueTitle.Text = "OVERDUE";
            //
            // lblOverdueValue
            //
            this.lblOverdueValue.AutoSize = false;
            this.lblOverdueValue.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.lblOverdueValue.Location = new System.Drawing.Point(12, 30);
            this.lblOverdueValue.Name = "lblOverdueValue";
            this.lblOverdueValue.Size = new System.Drawing.Size(140, 36);
            this.lblOverdueValue.Text = "–";
            //
            // cardMine
            //
            this.cardMine.BackColor = System.Drawing.Color.White;
            this.cardMine.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardMine.Controls.Add(this.lblMineTitle);
            this.cardMine.Controls.Add(this.lblMineValue);
            this.cardMine.Controls.Add(this.stripMine);
            this.cardMine.Dock = Wisej.Web.DockStyle.Fill;
            this.cardMine.Margin = new Wisej.Web.Padding(0, 0, 8, 8);
            this.cardMine.Name = "cardMine";
            //
            // stripMine
            //
            this.stripMine.BackColor = System.Drawing.Color.FromArgb(181, 71, 8);
            this.stripMine.Dock = Wisej.Web.DockStyle.Top;
            this.stripMine.Name = "stripMine";
            this.stripMine.Size = new System.Drawing.Size(182, 4);
            //
            // lblMineTitle
            //
            this.lblMineTitle.AutoSize = false;
            this.lblMineTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblMineTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblMineTitle.Location = new System.Drawing.Point(12, 12);
            this.lblMineTitle.Name = "lblMineTitle";
            this.lblMineTitle.Size = new System.Drawing.Size(140, 16);
            this.lblMineTitle.Text = "ASSIGNED TO ME";
            //
            // lblMineValue
            //
            this.lblMineValue.AutoSize = false;
            this.lblMineValue.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.lblMineValue.Location = new System.Drawing.Point(12, 30);
            this.lblMineValue.Name = "lblMineValue";
            this.lblMineValue.Size = new System.Drawing.Size(140, 36);
            this.lblMineValue.Text = "–";
            //
            // cardClosed
            //
            this.cardClosed.BackColor = System.Drawing.Color.White;
            this.cardClosed.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardClosed.Controls.Add(this.lblClosedTitle);
            this.cardClosed.Controls.Add(this.lblClosedValue);
            this.cardClosed.Controls.Add(this.stripClosed);
            this.cardClosed.Dock = Wisej.Web.DockStyle.Fill;
            this.cardClosed.Margin = new Wisej.Web.Padding(0, 0, 8, 8);
            this.cardClosed.Name = "cardClosed";
            //
            // stripClosed
            //
            this.stripClosed.BackColor = System.Drawing.Color.FromArgb(2, 122, 72);
            this.stripClosed.Dock = Wisej.Web.DockStyle.Top;
            this.stripClosed.Name = "stripClosed";
            this.stripClosed.Size = new System.Drawing.Size(182, 4);
            //
            // lblClosedTitle
            //
            this.lblClosedTitle.AutoSize = false;
            this.lblClosedTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblClosedTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblClosedTitle.Location = new System.Drawing.Point(12, 12);
            this.lblClosedTitle.Name = "lblClosedTitle";
            this.lblClosedTitle.Size = new System.Drawing.Size(140, 16);
            this.lblClosedTitle.Text = "CLOSED THIS WEEK";
            //
            // lblClosedValue
            //
            this.lblClosedValue.AutoSize = false;
            this.lblClosedValue.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.lblClosedValue.Location = new System.Drawing.Point(12, 30);
            this.lblClosedValue.Name = "lblClosedValue";
            this.lblClosedValue.Size = new System.Drawing.Size(140, 36);
            this.lblClosedValue.Text = "–";
            //
            // gridCard
            //
            this.gridCard.BackColor = System.Drawing.Color.White;
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
            // gridTickets  (MinimumSize keeps it from collapsing on the phone layout)
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
            this.gridTickets.MinimumSize = new System.Drawing.Size(280, 120);
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
            this.colTitle.MinimumWidth = 120;
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
            // logPanel  (Dock = Bottom: the profile log, one line per profile change)
            //
            this.logPanel.Controls.Add(this.logCard);
            this.logPanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.logPanel.Name = "logPanel";
            this.logPanel.Padding = new Wisej.Web.Padding(0, 8, 0, 0);
            this.logPanel.Size = new System.Drawing.Size(772, 132);
            //
            // logCard
            //
            this.logCard.BackColor = System.Drawing.Color.White;
            this.logCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.logCard.Controls.Add(this.lstProfileLog);
            this.logCard.Controls.Add(this.lblLogTitle);
            this.logCard.Dock = Wisej.Web.DockStyle.Fill;
            this.logCard.Name = "logCard";
            this.logCard.Padding = new Wisej.Web.Padding(8);
            //
            // lblLogTitle
            //
            this.lblLogTitle.AutoSize = false;
            this.lblLogTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblLogTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblLogTitle.Name = "lblLogTitle";
            this.lblLogTitle.Size = new System.Drawing.Size(754, 22);
            this.lblLogTitle.Text = "Profile log";
            this.lblLogTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lstProfileLog
            //
            this.lstProfileLog.Dock = Wisej.Web.DockStyle.Fill;
            this.lstProfileLog.Font = new System.Drawing.Font("monospace", 9F);
            this.lstProfileLog.Name = "lstProfileLog";
            //
            // detailsPanel  (Right 340 px on Desktop, 300 px on Small Desktop; Bottom 320 px on Tablet; hidden on Phone)
            //
            this.detailsPanel.Controls.Add(this.detailsCard);
            this.detailsPanel.Dock = Wisej.Web.DockStyle.Right;
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Padding = new Wisej.Web.Padding(0, 4, 8, 4);
            this.detailsPanel.Size = new System.Drawing.Size(340, 596);
            //
            // detailsCard  (AutoScroll: docked under the grid, the editor keeps its MinimumSize and scrolls)
            //
            this.detailsCard.AutoScroll = true;
            this.detailsCard.BackColor = System.Drawing.Color.White;
            this.detailsCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.detailsCard.Controls.Add(this.ticketEditor);
            this.detailsCard.Dock = Wisej.Web.DockStyle.Fill;
            this.detailsCard.Name = "detailsCard";
            //
            // ticketEditor  (moved into Dialogs/TicketEditorForm on the Phone profiles)
            //
            this.ticketEditor.Dock = Wisej.Web.DockStyle.Fill;
            this.ticketEditor.Name = "ticketEditor";
            this.ticketEditor.SaveRequested += new System.EventHandler(this.ticketEditor_SaveRequested);
            this.ticketEditor.CloseRequested += new System.EventHandler(this.ticketEditor_CloseRequested);
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
            this.statusCard.BackColor = System.Drawing.Color.White;
            this.statusCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.statusCard.Controls.Add(this.lblStatus);
            this.statusCard.Controls.Add(this.lblProfile);
            this.statusCard.Dock = Wisej.Web.DockStyle.Fill;
            this.statusCard.Name = "statusCard";
            this.statusCard.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            //
            // lblProfile  (Dock = Left: "Profile: Tablet · 980 px")
            //
            this.lblProfile.AutoEllipsis = true;
            this.lblProfile.AutoSize = false;
            this.lblProfile.Dock = Wisej.Web.DockStyle.Left;
            this.lblProfile.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(260, 22);
            this.lblProfile.Text = "Profile: –";
            this.lblProfile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStatus
            //
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Fill;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // MainPage
            //
            // Docking priority follows the child order: the control added last is docked first.
            // toolbar Top, status Bottom, navigation Left, details Right, workspace fills what is left.
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.workspacePanel);
            this.Controls.Add(this.detailsPanel);
            this.Controls.Add(this.navigationPanel);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.toolbarPanel);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "Adaptive Operations Console";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.toolbarPanel.ResumeLayout(false);
            this.toolbarCard.ResumeLayout(false);
            this.toolbarFlow.ResumeLayout(false);
            this.navigationPanel.ResumeLayout(false);
            this.workspacePanel.ResumeLayout(false);
            this.metricsTable.ResumeLayout(false);
            this.cardOpen.ResumeLayout(false);
            this.cardOverdue.ResumeLayout(false);
            this.cardMine.ResumeLayout(false);
            this.cardClosed.ResumeLayout(false);
            this.gridCard.ResumeLayout(false);
            this.logPanel.ResumeLayout(false);
            this.logCard.ResumeLayout(false);
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
        private Wisej.Web.FlowLayoutPanel toolbarFlow;
        private Wisej.Web.Button btnMenu;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.Button btnReapply;
        private Wisej.Web.Button btnClearTrace;
        private AdaptiveOps.Shell.NavigationRail navigationRail;
        private Wisej.Web.TableLayoutPanel metricsTable;
        private Wisej.Web.Panel cardOpen;
        private Wisej.Web.Panel stripOpen;
        private Wisej.Web.Label lblOpenTitle;
        private Wisej.Web.Label lblOpenValue;
        private Wisej.Web.Panel cardOverdue;
        private Wisej.Web.Panel stripOverdue;
        private Wisej.Web.Label lblOverdueTitle;
        private Wisej.Web.Label lblOverdueValue;
        private Wisej.Web.Panel cardMine;
        private Wisej.Web.Panel stripMine;
        private Wisej.Web.Label lblMineTitle;
        private Wisej.Web.Label lblMineValue;
        private Wisej.Web.Panel cardClosed;
        private Wisej.Web.Panel stripClosed;
        private Wisej.Web.Label lblClosedTitle;
        private Wisej.Web.Label lblClosedValue;
        private Wisej.Web.Panel gridCard;
        private Wisej.Web.Label lblWorkspaceTitle;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colOwner;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.Panel logPanel;
        private Wisej.Web.Panel logCard;
        private Wisej.Web.Label lblLogTitle;
        private Wisej.Web.ListBox lstProfileLog;
        private Wisej.Web.Panel detailsCard;
        private AdaptiveOps.Shell.TicketEditor ticketEditor;
        private Wisej.Web.Panel statusCard;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblProfile;
    }
}
