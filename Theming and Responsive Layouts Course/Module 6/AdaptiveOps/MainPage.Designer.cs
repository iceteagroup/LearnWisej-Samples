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
            this.components = new System.ComponentModel.Container();
            this.toolbarPanel = new Wisej.Web.Panel();
            this.toolbarCard = new Wisej.Web.Panel();
            this.toolbarFlow = new Wisej.Web.FlowLayoutPanel();
            this.btnMenu = new Wisej.Web.Button();
            this.lblAppTitle = new Wisej.Web.Label();
            this.btnRefresh = new Wisej.Web.Button();
            this.btnProfilePhone = new Wisej.Web.Button();
            this.btnProfileTablet = new Wisej.Web.Button();
            this.btnProfileDesktop = new Wisej.Web.Button();
            this.btnStepProfiles = new Wisej.Web.Button();
            this.btnUnknownProfile = new Wisej.Web.Button();
            this.btnFaultyRegion = new Wisej.Web.Button();
            this.btnReapply = new Wisej.Web.Button();
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
            this.bannerPanel = new Wisej.Web.Panel();
            this.lblBanner = new Wisej.Web.Label();
            this.gridCard = new Wisej.Web.Panel();
            this.lblWorkspaceTitle = new Wisej.Web.Label();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.tracePanel = new Wisej.Web.Panel();
            this.traceCard = new Wisej.Web.Panel();
            this.traceHeader = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.btnClearTrace = new Wisej.Web.Button();
            this.listTrace = new Wisej.Web.ListBox();
            this.detailsPanel = new Wisej.Web.Panel();
            this.detailsCard = new Wisej.Web.Panel();
            this.ticketEditor = new AdaptiveOps.Shell.TicketEditor();
            this.statusPanel = new Wisej.Web.Panel();
            this.statusCard = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.lblProfile = new Wisej.Web.Label();
            this.lblTheme = new Wisej.Web.Label();
            this.timerProfiles = new Wisej.Web.Timer(this.components);
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
            this.bannerPanel.SuspendLayout();
            this.gridCard.SuspendLayout();
            this.tracePanel.SuspendLayout();
            this.traceCard.SuspendLayout();
            this.traceHeader.SuspendLayout();
            this.detailsPanel.SuspendLayout();
            this.detailsCard.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.statusCard.SuspendLayout();
            this.SuspendLayout();
            //
            // toolbarPanel  (region · Dock = Top · 56 px)
            //
            // The five region panels are transparent Dock containers: their Padding is the gap
            // between neighbouring regions (Dock ignores Margin), and the white card inside fills them.
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
            // toolbarFlow  (the command bar: a horizontal FlowLayoutPanel, so a button that changes width or
            // visibility per profile re-flows its neighbours — no Location arithmetic in ApplyProfile)
            //
            this.toolbarFlow.Controls.Add(this.btnMenu);
            this.toolbarFlow.Controls.Add(this.lblAppTitle);
            this.toolbarFlow.Controls.Add(this.btnRefresh);
            this.toolbarFlow.Controls.Add(this.btnProfilePhone);
            this.toolbarFlow.Controls.Add(this.btnProfileTablet);
            this.toolbarFlow.Controls.Add(this.btnProfileDesktop);
            this.toolbarFlow.Controls.Add(this.btnStepProfiles);
            this.toolbarFlow.Controls.Add(this.btnUnknownProfile);
            this.toolbarFlow.Controls.Add(this.btnFaultyRegion);
            this.toolbarFlow.Controls.Add(this.btnReapply);
            this.toolbarFlow.Dock = Wisej.Web.DockStyle.Fill;
            this.toolbarFlow.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.toolbarFlow.Name = "toolbarFlow";
            this.toolbarFlow.Padding = new Wisej.Web.Padding(6, 6, 6, 0);
            this.toolbarFlow.WrapContents = false;
            //
            // btnMenu  (Phone only: the rail is hidden, its five sections come back as a menu)
            //
            this.btnMenu.Display = Wisej.Web.Display.Icon;
            this.btnMenu.ImageSource = "icon-justify-fill";
            this.btnMenu.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(36, 30);
            this.btnMenu.Text = "Menu";
            this.btnMenu.ToolTipText = "Navigation menu (the rail is hidden on the Phone profiles)";
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
            // btnRefresh  (reload from the repository — kept from Module 1)
            //
            this.btnRefresh.ImageSource = "icon-refresh";
            this.btnRefresh.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(92, 30);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.ToolTipText = "Reload the tickets from the repository and recompute the metrics.";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnProfilePhone / btnProfileTablet / btnProfileDesktop  (success path: simulate a profile)
            //
            this.btnProfilePhone.ImageSource = "icon-first";
            this.btnProfilePhone.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.btnProfilePhone.Name = "btnProfilePhone";
            this.btnProfilePhone.Size = new System.Drawing.Size(84, 30);
            this.btnProfilePhone.Tag = "Phone";
            this.btnProfilePhone.Text = "Phone";
            this.btnProfilePhone.ToolTipText = "Simulate profile: Phone — ApplyProfile with the framework's \"Phone\" ClientProfile object.";
            this.btnProfilePhone.Click += new System.EventHandler(this.btnSimulateProfile_Click);
            this.btnProfileTablet.ImageSource = "icon-justify-center";
            this.btnProfileTablet.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.btnProfileTablet.Name = "btnProfileTablet";
            this.btnProfileTablet.Size = new System.Drawing.Size(84, 30);
            this.btnProfileTablet.Tag = "Tablet";
            this.btnProfileTablet.Text = "Tablet";
            this.btnProfileTablet.ToolTipText = "Simulate profile: Tablet — rail icon-only, details docked under the grid.";
            this.btnProfileTablet.Click += new System.EventHandler(this.btnSimulateProfile_Click);
            this.btnProfileDesktop.ImageSource = "icon-last";
            this.btnProfileDesktop.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.btnProfileDesktop.Name = "btnProfileDesktop";
            this.btnProfileDesktop.Size = new System.Drawing.Size(92, 30);
            this.btnProfileDesktop.Tag = "Desktop";
            this.btnProfileDesktop.Text = "Desktop";
            this.btnProfileDesktop.ToolTipText = "Simulate profile: Desktop — everything visible.";
            this.btnProfileDesktop.Click += new System.EventHandler(this.btnSimulateProfile_Click);
            //
            // btnStepProfiles  (progress path: a Wisej.Web.Timer walks Desktop → Small Desktop → Tablet → Phone and back)
            //
            this.btnStepProfiles.ImageSource = "icon-redo";
            this.btnStepProfiles.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.btnStepProfiles.Name = "btnStepProfiles";
            this.btnStepProfiles.Size = new System.Drawing.Size(114, 30);
            this.btnStepProfiles.Text = "Step profiles";
            this.btnStepProfiles.ToolTipText = "A server Timer applies Desktop → Small Desktop → Tablet → Phone and back, one profile per tick.";
            this.btnStepProfiles.Click += new System.EventHandler(this.btnStepProfiles_Click);
            //
            // btnUnknownProfile  (failure path 1: a profile name that is not in ClientProfiles.json)
            //
            this.btnUnknownProfile.ImageSource = "icon-question";
            this.btnUnknownProfile.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.btnUnknownProfile.Name = "btnUnknownProfile";
            this.btnUnknownProfile.Size = new System.Drawing.Size(104, 30);
            this.btnUnknownProfile.Text = "Unknown";
            this.btnUnknownProfile.ToolTipText = "Ask for the profile \"Kiosk\": it is not in ClientProfiles.json, so the current profile is kept and the miss is logged.";
            this.btnUnknownProfile.Click += new System.EventHandler(this.btnUnknownProfile_Click);
            //
            // btnFaultyRegion  (failure path 2: one region throws inside ApplyProfile; the others still apply)
            //
            this.btnFaultyRegion.ImageSource = "icon-warning";
            this.btnFaultyRegion.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.btnFaultyRegion.Name = "btnFaultyRegion";
            this.btnFaultyRegion.Size = new System.Drawing.Size(114, 30);
            this.btnFaultyRegion.Text = "Faulty region";
            this.btnFaultyRegion.ToolTipText = "Re-apply the current profile with a fault injected into the metrics region: that region fails, the other five apply.";
            this.btnFaultyRegion.Click += new System.EventHandler(this.btnFaultyRegion_Click);
            //
            // btnReapply  (recovery: re-read Application.ActiveProfile and apply it again)
            //
            this.btnReapply.ImageSource = "icon-undo";
            this.btnReapply.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.btnReapply.Name = "btnReapply";
            this.btnReapply.Size = new System.Drawing.Size(96, 30);
            this.btnReapply.Text = "Re-apply";
            this.btnReapply.ToolTipText = "Recovery: read Application.ActiveProfile again and apply it — ends any simulation and clears the banner.";
            this.btnReapply.Click += new System.EventHandler(this.btnReapply_Click);
            //
            // navigationPanel  (region · Dock = Left · 220 px full, 64 px icon-only, hidden on Phone)
            //
            this.navigationPanel.Controls.Add(this.navigationRail);
            this.navigationPanel.Dock = Wisej.Web.DockStyle.Left;
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Padding = new Wisej.Web.Padding(8, 4, 0, 4);
            this.navigationPanel.Size = new System.Drawing.Size(220, 596);
            //
            // navigationRail  (Shell/NavigationRail — the UserControl the lab hides on Phone)
            //
            this.navigationRail.Dock = Wisej.Web.DockStyle.Fill;
            this.navigationRail.Name = "navigationRail";
            this.navigationRail.SectionSelected += new System.EventHandler<AdaptiveOps.Shell.SectionEventArgs>(this.navigationRail_SectionSelected);
            //
            // workspacePanel  (region · Dock = Fill · MinimumSize 320×240)
            //
            // Child order matters here too: gridCard is added first so it is docked LAST (Fill takes
            // what is left); metricsTable is added last so it is docked FIRST against the top edge.
            //
            this.workspacePanel.Controls.Add(this.gridCard);
            this.workspacePanel.Controls.Add(this.tracePanel);
            this.workspacePanel.Controls.Add(this.bannerPanel);
            this.workspacePanel.Controls.Add(this.metricsTable);
            this.workspacePanel.Dock = Wisej.Web.DockStyle.Fill;
            this.workspacePanel.MinimumSize = new System.Drawing.Size(320, 240);
            this.workspacePanel.Name = "workspacePanel";
            this.workspacePanel.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.workspacePanel.Size = new System.Drawing.Size(788, 596);
            //
            // metricsTable  (Dock = Top · a TableLayoutPanel: 4×1 on Desktop, 2×2 on Small Desktop, 1×4 on Phone —
            // ApplyProfile changes ColumnCount/RowCount and the cell positions, never a card's Bounds)
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
            // Metric card: Open
            //
            this.cardOpen.BackColor = System.Drawing.Color.White;
            this.cardOpen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardOpen.Controls.Add(this.lblOpenTitle);
            this.cardOpen.Controls.Add(this.lblOpenValue);
            this.cardOpen.Controls.Add(this.stripOpen);
            this.cardOpen.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOpen.Margin = new Wisej.Web.Padding(0, 0, 8, 8);
            this.cardOpen.Name = "cardOpen";
            this.stripOpen.BackColor = System.Drawing.Color.FromArgb(36, 84, 166);
            this.stripOpen.Dock = Wisej.Web.DockStyle.Top;
            this.stripOpen.Name = "stripOpen";
            this.stripOpen.Size = new System.Drawing.Size(182, 4);
            this.lblOpenTitle.AutoSize = false;
            this.lblOpenTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblOpenTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblOpenTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOpenTitle.Name = "lblOpenTitle";
            this.lblOpenTitle.Size = new System.Drawing.Size(140, 16);
            this.lblOpenTitle.Text = "OPEN";
            this.lblOpenValue.AutoSize = false;
            this.lblOpenValue.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.lblOpenValue.Location = new System.Drawing.Point(12, 30);
            this.lblOpenValue.Name = "lblOpenValue";
            this.lblOpenValue.Size = new System.Drawing.Size(140, 36);
            this.lblOpenValue.Text = "–";
            //
            // Metric card: Overdue
            //
            this.cardOverdue.BackColor = System.Drawing.Color.White;
            this.cardOverdue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardOverdue.Controls.Add(this.lblOverdueTitle);
            this.cardOverdue.Controls.Add(this.lblOverdueValue);
            this.cardOverdue.Controls.Add(this.stripOverdue);
            this.cardOverdue.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOverdue.Margin = new Wisej.Web.Padding(0, 0, 8, 8);
            this.cardOverdue.Name = "cardOverdue";
            this.stripOverdue.BackColor = System.Drawing.Color.FromArgb(180, 35, 24);
            this.stripOverdue.Dock = Wisej.Web.DockStyle.Top;
            this.stripOverdue.Name = "stripOverdue";
            this.stripOverdue.Size = new System.Drawing.Size(182, 4);
            this.lblOverdueTitle.AutoSize = false;
            this.lblOverdueTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblOverdueTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblOverdueTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOverdueTitle.Name = "lblOverdueTitle";
            this.lblOverdueTitle.Size = new System.Drawing.Size(140, 16);
            this.lblOverdueTitle.Text = "OVERDUE";
            this.lblOverdueValue.AutoSize = false;
            this.lblOverdueValue.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.lblOverdueValue.Location = new System.Drawing.Point(12, 30);
            this.lblOverdueValue.Name = "lblOverdueValue";
            this.lblOverdueValue.Size = new System.Drawing.Size(140, 36);
            this.lblOverdueValue.Text = "–";
            //
            // Metric card: Assigned to me
            //
            this.cardMine.BackColor = System.Drawing.Color.White;
            this.cardMine.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardMine.Controls.Add(this.lblMineTitle);
            this.cardMine.Controls.Add(this.lblMineValue);
            this.cardMine.Controls.Add(this.stripMine);
            this.cardMine.Dock = Wisej.Web.DockStyle.Fill;
            this.cardMine.Margin = new Wisej.Web.Padding(0, 0, 8, 8);
            this.cardMine.Name = "cardMine";
            this.stripMine.BackColor = System.Drawing.Color.FromArgb(181, 71, 8);
            this.stripMine.Dock = Wisej.Web.DockStyle.Top;
            this.stripMine.Name = "stripMine";
            this.stripMine.Size = new System.Drawing.Size(182, 4);
            this.lblMineTitle.AutoSize = false;
            this.lblMineTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblMineTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblMineTitle.Location = new System.Drawing.Point(12, 12);
            this.lblMineTitle.Name = "lblMineTitle";
            this.lblMineTitle.Size = new System.Drawing.Size(140, 16);
            this.lblMineTitle.Text = "ASSIGNED TO ME";
            this.lblMineValue.AutoSize = false;
            this.lblMineValue.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.lblMineValue.Location = new System.Drawing.Point(12, 30);
            this.lblMineValue.Name = "lblMineValue";
            this.lblMineValue.Size = new System.Drawing.Size(140, 36);
            this.lblMineValue.Text = "–";
            //
            // Metric card: Closed this week
            //
            this.cardClosed.BackColor = System.Drawing.Color.White;
            this.cardClosed.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardClosed.Controls.Add(this.lblClosedTitle);
            this.cardClosed.Controls.Add(this.lblClosedValue);
            this.cardClosed.Controls.Add(this.stripClosed);
            this.cardClosed.Dock = Wisej.Web.DockStyle.Fill;
            this.cardClosed.Margin = new Wisej.Web.Padding(0, 0, 8, 8);
            this.cardClosed.Name = "cardClosed";
            this.stripClosed.BackColor = System.Drawing.Color.FromArgb(2, 122, 72);
            this.stripClosed.Dock = Wisej.Web.DockStyle.Top;
            this.stripClosed.Name = "stripClosed";
            this.stripClosed.Size = new System.Drawing.Size(182, 4);
            this.lblClosedTitle.AutoSize = false;
            this.lblClosedTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblClosedTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblClosedTitle.Location = new System.Drawing.Point(12, 12);
            this.lblClosedTitle.Name = "lblClosedTitle";
            this.lblClosedTitle.Size = new System.Drawing.Size(140, 16);
            this.lblClosedTitle.Text = "CLOSED THIS WEEK";
            this.lblClosedValue.AutoSize = false;
            this.lblClosedValue.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.lblClosedValue.Location = new System.Drawing.Point(12, 30);
            this.lblClosedValue.Name = "lblClosedValue";
            this.lblClosedValue.Size = new System.Drawing.Size(140, 36);
            this.lblClosedValue.Text = "–";
            //
            // bannerPanel  (Dock = Top · hidden until something goes wrong; a hidden docked control takes no space)
            //
            this.bannerPanel.Controls.Add(this.lblBanner);
            this.bannerPanel.Dock = Wisej.Web.DockStyle.Top;
            this.bannerPanel.Name = "bannerPanel";
            this.bannerPanel.Padding = new Wisej.Web.Padding(0, 0, 0, 8);
            this.bannerPanel.Size = new System.Drawing.Size(772, 38);
            this.bannerPanel.Visible = false;
            //
            // lblBanner
            //
            this.lblBanner.AutoEllipsis = true;
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Dock = Wisej.Web.DockStyle.Fill;
            this.lblBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // gridCard  (Dock = Fill · the ticket grid)
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
            // gridTickets  (MinimumSize keeps it from collapsing to zero on the phone layout)
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
            // grid columns  (Owner and Due are the "secondary" columns the Phone profile hides)
            //
            this.colId.FillWeight = 60F;
            this.colId.HeaderText = "Id";
            this.colId.MinimumWidth = 64;
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            this.colTitle.FillWeight = 240F;
            this.colTitle.HeaderText = "Title";
            this.colTitle.MinimumWidth = 120;
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            this.colPriority.FillWeight = 70F;
            this.colPriority.HeaderText = "Priority";
            this.colPriority.MinimumWidth = 64;
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            this.colStatus.FillWeight = 70F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.MinimumWidth = 64;
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colOwner.FillWeight = 70F;
            this.colOwner.HeaderText = "Owner";
            this.colOwner.MinimumWidth = 64;
            this.colOwner.Name = "colOwner";
            this.colOwner.ReadOnly = true;
            this.colDue.FillWeight = 90F;
            this.colDue.HeaderText = "Due";
            this.colDue.MinimumWidth = 90;
            this.colDue.Name = "colDue";
            this.colDue.ReadOnly = true;
            //
            // tracePanel  (Dock = Bottom · the gap above it is its Padding)
            //
            this.tracePanel.Controls.Add(this.traceCard);
            this.tracePanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.tracePanel.Name = "tracePanel";
            this.tracePanel.Padding = new Wisej.Web.Padding(0, 8, 0, 0);
            this.tracePanel.Size = new System.Drawing.Size(772, 176);
            //
            // traceCard  ("Layout & theme · live trace")
            //
            this.traceCard.BackColor = System.Drawing.Color.White;
            this.traceCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.traceCard.Controls.Add(this.listTrace);
            this.traceCard.Controls.Add(this.traceHeader);
            this.traceCard.Dock = Wisej.Web.DockStyle.Fill;
            this.traceCard.Name = "traceCard";
            this.traceCard.Padding = new Wisej.Web.Padding(8);
            //
            // traceHeader  (title Fill + clear button Right)
            //
            this.traceHeader.Controls.Add(this.lblTraceTitle);
            this.traceHeader.Controls.Add(this.btnClearTrace);
            this.traceHeader.Dock = Wisej.Web.DockStyle.Top;
            this.traceHeader.Name = "traceHeader";
            this.traceHeader.Size = new System.Drawing.Size(754, 24);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Text = "Layout & theme · live trace";
            this.lblTraceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnClearTrace
            //
            this.btnClearTrace.Display = Wisej.Web.Display.Icon;
            this.btnClearTrace.Dock = Wisej.Web.DockStyle.Right;
            this.btnClearTrace.ImageSource = "icon-close";
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(28, 24);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.ToolTipText = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // listTrace
            //
            this.listTrace.Dock = Wisej.Web.DockStyle.Fill;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Name = "listTrace";
            //
            // detailsPanel  (region · Dock = Right · 340 px on Desktop; 300 px on Small Desktop / Tablet (Landscape);
            // Dock = Bottom · 320 px on Tablet; hidden on Phone, where the editor lives in TicketEditorForm)
            //
            this.detailsPanel.Controls.Add(this.detailsCard);
            this.detailsPanel.Dock = Wisej.Web.DockStyle.Right;
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Padding = new Wisej.Web.Padding(0, 4, 8, 4);
            this.detailsPanel.Size = new System.Drawing.Size(340, 596);
            //
            // detailsCard  (AutoScroll: when the region is docked under the grid the editor keeps its MinimumSize and scrolls)
            //
            this.detailsCard.AutoScroll = true;
            this.detailsCard.BackColor = System.Drawing.Color.White;
            this.detailsCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.detailsCard.Controls.Add(this.ticketEditor);
            this.detailsCard.Dock = Wisej.Web.DockStyle.Fill;
            this.detailsCard.Name = "detailsCard";
            //
            // ticketEditor  (Shell/TicketEditor — moved into Dialogs/TicketEditorForm on the Phone profiles)
            //
            this.ticketEditor.Dock = Wisej.Web.DockStyle.Fill;
            this.ticketEditor.Name = "ticketEditor";
            this.ticketEditor.SaveRequested += new System.EventHandler(this.ticketEditor_SaveRequested);
            this.ticketEditor.CloseRequested += new System.EventHandler(this.ticketEditor_CloseRequested);
            //
            // statusPanel  (region · Dock = Bottom · 28 px)
            //
            this.statusPanel.Controls.Add(this.statusCard);
            this.statusPanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Padding = new Wisej.Web.Padding(8, 0, 8, 4);
            this.statusPanel.Size = new System.Drawing.Size(1348, 28);
            //
            // statusCard  (three labels, all docked: Left · Fill · Right)
            //
            this.statusCard.BackColor = System.Drawing.Color.White;
            this.statusCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.statusCard.Controls.Add(this.lblProfile);
            this.statusCard.Controls.Add(this.lblTheme);
            this.statusCard.Controls.Add(this.lblStatus);
            this.statusCard.Dock = Wisej.Web.DockStyle.Fill;
            this.statusCard.Name = "statusCard";
            this.statusCard.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            //
            // lblStatus  (● ready / warn / error)
            //
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Left;
            this.lblStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(200, 22);
            this.lblStatus.Text = "● starting";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblProfile  (the lab's profile label: "profile: Phone · browser 390 × 844 px · Mobile")
            //
            this.lblProfile.AutoEllipsis = true;
            this.lblProfile.AutoSize = false;
            this.lblProfile.Dock = Wisej.Web.DockStyle.Fill;
            this.lblProfile.Font = new System.Drawing.Font("monospace", 9F);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Text = "profile: not read yet";
            this.lblProfile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTheme  (hidden on the Phone profiles: the status bar keeps profile + size only)
            //
            this.lblTheme.AutoEllipsis = true;
            this.lblTheme.AutoSize = false;
            this.lblTheme.Dock = Wisej.Web.DockStyle.Right;
            this.lblTheme.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(220, 22);
            this.lblTheme.Text = "theme: –";
            this.lblTheme.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // timerProfiles  (progress path: one profile per tick)
            //
            this.timerProfiles.Interval = 1500;
            this.timerProfiles.Tick += new System.EventHandler(this.timerProfiles_Tick);
            //
            // MainPage
            //
            // Docking priority follows the child order: the control added LAST is docked FIRST
            // against the page edges. So workspacePanel (Fill) goes in first and toolbarPanel last,
            // which gives the lab's order: toolbar Top, status Bottom, navigation Left, details Right,
            // workspace fills what is left. Reordering these five lines changes the layout.
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
            this.Resize += new System.EventHandler(this.MainPage_Resize);
            this.ResponsiveProfileChanged += new Wisej.Web.ResponsiveProfileChangedEventHandler(this.MainPage_ResponsiveProfileChanged);
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
            this.bannerPanel.ResumeLayout(false);
            this.gridCard.ResumeLayout(false);
            this.tracePanel.ResumeLayout(false);
            this.traceCard.ResumeLayout(false);
            this.traceHeader.ResumeLayout(false);
            this.detailsPanel.ResumeLayout(false);
            this.detailsCard.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.statusCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // Shell regions (the names every module reuses)
        private Wisej.Web.Panel toolbarPanel;
        private Wisej.Web.Panel navigationPanel;
        private Wisej.Web.Panel workspacePanel;
        private Wisej.Web.Panel detailsPanel;
        private Wisej.Web.Panel statusPanel;

        // Toolbar
        private Wisej.Web.Panel toolbarCard;
        private Wisej.Web.FlowLayoutPanel toolbarFlow;
        private Wisej.Web.Button btnMenu;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.Button btnProfilePhone;
        private Wisej.Web.Button btnProfileTablet;
        private Wisej.Web.Button btnProfileDesktop;
        private Wisej.Web.Button btnStepProfiles;
        private Wisej.Web.Button btnUnknownProfile;
        private Wisej.Web.Button btnFaultyRegion;
        private Wisej.Web.Button btnReapply;

        // Navigation rail
        private AdaptiveOps.Shell.NavigationRail navigationRail;

        // Workspace: metrics
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

        // Workspace: banner, grid, trace
        private Wisej.Web.Panel bannerPanel;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Panel gridCard;
        private Wisej.Web.Label lblWorkspaceTitle;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colOwner;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.Panel tracePanel;
        private Wisej.Web.Panel traceCard;
        private Wisej.Web.Panel traceHeader;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.Button btnClearTrace;
        private Wisej.Web.ListBox listTrace;

        // Details editor
        private Wisej.Web.Panel detailsCard;
        private AdaptiveOps.Shell.TicketEditor ticketEditor;

        // Status bar
        private Wisej.Web.Panel statusCard;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblProfile;
        private Wisej.Web.Label lblTheme;

        // Components
        private Wisej.Web.Timer timerProfiles;
    }
}
