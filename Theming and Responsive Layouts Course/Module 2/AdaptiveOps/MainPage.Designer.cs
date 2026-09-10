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
                // Application.BrowserSizeChanged is a session-level event: unsubscribe with the page.
                Wisej.Web.Application.BrowserSizeChanged -= this.Application_BrowserSizeChanged;

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
            this.lblAppTitle = new Wisej.Web.Label();
            this.btnApplyTheme = new Wisej.Web.Button();
            this.btnWalkStates = new Wisej.Web.Button();
            this.btnMissingTheme = new Wisej.Web.Button();
            this.btnCompareTheme = new Wisej.Web.Button();
            this.btnInvalidTicket = new Wisej.Web.Button();
            this.btnReset = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.lblProgress = new Wisej.Web.Label();
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
            this.bannerPanel = new Wisej.Web.Panel();
            this.lblBanner = new Wisej.Web.Label();
            this.tabWorkspace = new Wisej.Web.TabControl();
            this.pageTickets = new Wisej.Web.TabPage();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pageTokens = new Wisej.Web.TabPage();
            this.gridTokens = new Wisej.Web.DataGridView();
            this.colTokenName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTokenKind = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTokenDefined = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTokenResolved = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTokenUsedBy = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblTokensHint = new Wisej.Web.Label();
            this.tracePanel = new Wisej.Web.Panel();
            this.traceCard = new Wisej.Web.Panel();
            this.listTrace = new Wisej.Web.ListBox();
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
            this.lblDetailsHint = new Wisej.Web.Label();
            this.statusPanel = new Wisej.Web.Panel();
            this.statusCard = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.lblBrowserWidth = new Wisej.Web.Label();
            this.lblTheme = new Wisej.Web.Label();
            this.timerWalk = new Wisej.Web.Timer(this.components);
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
            this.bannerPanel.SuspendLayout();
            this.tabWorkspace.SuspendLayout();
            this.pageTickets.SuspendLayout();
            this.pageTokens.SuspendLayout();
            this.tracePanel.SuspendLayout();
            this.traceCard.SuspendLayout();
            this.detailsPanel.SuspendLayout();
            this.detailsCard.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.statusCard.SuspendLayout();
            this.SuspendLayout();
            //
            // Module 2 rule: NO BackColor, ForeColor or Font in this file. Every look comes from
            // Themes/AdaptiveOps.theme; a control only says WHICH appearance it wants (AppearanceKey)
            // and, for the metric strips and the status label, which custom theme state is active (States).
            // Docking, Location, Size, Anchor, Padding stay here: layout is the container's job, not the theme's.
            //
            // toolbarPanel  (region · Dock = Top · 56 px)
            //
            // The five region panels are transparent Dock containers: their Padding is the gap
            // between neighbouring regions (Dock ignores Margin), and the card inside fills them.
            //
            this.toolbarPanel.Controls.Add(this.toolbarCard);
            this.toolbarPanel.Dock = Wisej.Web.DockStyle.Top;
            this.toolbarPanel.Name = "toolbarPanel";
            this.toolbarPanel.Padding = new Wisej.Web.Padding(8, 8, 8, 4);
            this.toolbarPanel.Size = new System.Drawing.Size(1348, 56);
            //
            // toolbarCard  (appearance "metric-card": surface background, windowFrame border, $borderRadius — from the theme)
            //
            this.toolbarCard.AppearanceKey = "metric-card";
            this.toolbarCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.toolbarCard.Controls.Add(this.lblAppTitle);
            this.toolbarCard.Controls.Add(this.btnApplyTheme);
            this.toolbarCard.Controls.Add(this.btnWalkStates);
            this.toolbarCard.Controls.Add(this.btnMissingTheme);
            this.toolbarCard.Controls.Add(this.btnCompareTheme);
            this.toolbarCard.Controls.Add(this.btnInvalidTicket);
            this.toolbarCard.Controls.Add(this.btnReset);
            this.toolbarCard.Controls.Add(this.btnClearTrace);
            this.toolbarCard.Controls.Add(this.lblProgress);
            this.toolbarCard.Dock = Wisej.Web.DockStyle.Fill;
            this.toolbarCard.Name = "toolbarCard";
            //
            // lblAppTitle  (appearance "heading-label": font "heading" + textMain from the theme)
            //
            this.lblAppTitle.AppearanceKey = "heading-label";
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Location = new System.Drawing.Point(12, 7);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(250, 30);
            this.lblAppTitle.Text = "Adaptive Operations Console";
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnApplyTheme  (success path: AppearanceKey = "action-button" + token inspector)
            //
            this.btnApplyTheme.Location = new System.Drawing.Point(272, 7);
            this.btnApplyTheme.Name = "btnApplyTheme";
            this.btnApplyTheme.Size = new System.Drawing.Size(104, 30);
            this.btnApplyTheme.Text = "Apply theme";
            this.btnApplyTheme.ToolTipText = "Reads Application.Theme.Name, gives the primary commands AppearanceKey = \"action-button\" and fills the token inspector with every resolved token.";
            this.btnApplyTheme.Click += new System.EventHandler(this.btnApplyTheme_Click);
            //
            // btnWalkStates  (progress path: a Wisej.Web.Timer steps through appearance/state queries)
            //
            this.btnWalkStates.Location = new System.Drawing.Point(384, 7);
            this.btnWalkStates.Name = "btnWalkStates";
            this.btnWalkStates.Size = new System.Drawing.Size(104, 30);
            this.btnWalkStates.Text = "Walk states";
            this.btnWalkStates.ToolTipText = "A server Timer resolves one appearance path per tick (button, action-button, textbox invalid, grid header, tooltip…) and logs the value the theme gives each state.";
            this.btnWalkStates.Click += new System.EventHandler(this.btnWalkStates_Click);
            //
            // btnMissingTheme  (failure path: Application.LoadTheme("Missing-Theme") + a wrong token name)
            //
            this.btnMissingTheme.Location = new System.Drawing.Point(496, 7);
            this.btnMissingTheme.Name = "btnMissingTheme";
            this.btnMissingTheme.Size = new System.Drawing.Size(116, 30);
            this.btnMissingTheme.Text = "Missing theme";
            this.btnMissingTheme.ToolTipText = "Tries Application.LoadTheme(\"Missing-Theme\") inside try/catch and asks the theme for a misspelt token; the app stays alive and the trace shows what each returns.";
            this.btnMissingTheme.Click += new System.EventHandler(this.btnMissingTheme_Click);
            //
            // btnCompareTheme  (comparison: Bootstrap-4 ⇄ AdaptiveOps, live, for every session)
            //
            this.btnCompareTheme.Location = new System.Drawing.Point(620, 7);
            this.btnCompareTheme.Name = "btnCompareTheme";
            this.btnCompareTheme.Size = new System.Drawing.Size(116, 30);
            this.btnCompareTheme.Text = "Base theme ⇄";
            this.btnCompareTheme.ToolTipText = "Switches the running app to the base theme Bootstrap-4 and back, so you can see which look comes from the base and which from Themes/AdaptiveOps.theme.";
            this.btnCompareTheme.Click += new System.EventHandler(this.btnCompareTheme_Click);
            //
            // btnInvalidTicket  (failure path 2: server-side validation → textbox "invalid" state → danger token)
            //
            this.btnInvalidTicket.Location = new System.Drawing.Point(744, 7);
            this.btnInvalidTicket.Name = "btnInvalidTicket";
            this.btnInvalidTicket.Size = new System.Drawing.Size(108, 30);
            this.btnInvalidTicket.Text = "Invalid ticket";
            this.btnInvalidTicket.ToolTipText = "Saves the selected ticket with an empty title; the repository rejects it and the Title editor enters the theme's invalid state (danger).";
            this.btnInvalidTicket.Click += new System.EventHandler(this.btnInvalidTicket_Click);
            //
            // btnReset  (recovery: Application.LoadTheme("AdaptiveOps"), action-button re-applied, repository reset)
            //
            this.btnReset.Location = new System.Drawing.Point(860, 7);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(72, 30);
            this.btnReset.Text = "Reset";
            this.btnReset.ToolTipText = "Reloads the AdaptiveOps theme, re-applies action-button, clears the invalid editor and the banner, restores the seed tickets.";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Location = new System.Drawing.Point(940, 7);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(96, 30);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.ToolTipText = "Empties the live trace.";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // lblProgress  (appearance "muted-label": textMuted from the theme)
            //
            this.lblProgress.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblProgress.AppearanceKey = "muted-label";
            this.lblProgress.AutoEllipsis = true;
            this.lblProgress.AutoSize = false;
            this.lblProgress.Location = new System.Drawing.Point(1048, 7);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(272, 30);
            this.lblProgress.Text = "";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // navigationPanel  (region · Dock = Left · 220 px)
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
            // lblNavTitle  (appearance "overline-label": defaultBold + textMuted)
            //
            this.lblNavTitle.AppearanceKey = "overline-label";
            this.lblNavTitle.AutoSize = false;
            this.lblNavTitle.Location = new System.Drawing.Point(12, 12);
            this.lblNavTitle.Name = "lblNavTitle";
            this.lblNavTitle.Size = new System.Drawing.Size(186, 18);
            this.lblNavTitle.Text = "NAVIGATION";
            //
            // btnNavDashboard … btnNavHelp  (the rail: five plain "button" appearances, one shared handler)
            //
            this.btnNavDashboard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavDashboard.Location = new System.Drawing.Point(12, 40);
            this.btnNavDashboard.Name = "btnNavDashboard";
            this.btnNavDashboard.Size = new System.Drawing.Size(186, 36);
            this.btnNavDashboard.Text = "Dashboard";
            this.btnNavDashboard.ToolTipText = "Navigation buttons use the base \"button\" appearance: surfaceAlt face, surfaceHover on hover, focusShadow ring.";
            this.btnNavDashboard.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavTickets.Location = new System.Drawing.Point(12, 84);
            this.btnNavTickets.Name = "btnNavTickets";
            this.btnNavTickets.Size = new System.Drawing.Size(186, 36);
            this.btnNavTickets.Text = "Tickets";
            this.btnNavTickets.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavReports.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavReports.Location = new System.Drawing.Point(12, 128);
            this.btnNavReports.Name = "btnNavReports";
            this.btnNavReports.Size = new System.Drawing.Size(186, 36);
            this.btnNavReports.Text = "Reports";
            this.btnNavReports.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavSettings.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavSettings.Location = new System.Drawing.Point(12, 172);
            this.btnNavSettings.Name = "btnNavSettings";
            this.btnNavSettings.Size = new System.Drawing.Size(186, 36);
            this.btnNavSettings.Text = "Settings";
            this.btnNavSettings.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavHelp.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavHelp.Location = new System.Drawing.Point(12, 216);
            this.btnNavHelp.Name = "btnNavHelp";
            this.btnNavHelp.Size = new System.Drawing.Size(186, 36);
            this.btnNavHelp.Text = "Help";
            this.btnNavHelp.Click += new System.EventHandler(this.btnNav_Click);
            //
            // workspacePanel  (region · Dock = Fill · MinimumSize 320×240)
            //
            // Child order matters here too: tabWorkspace is added first so it is docked LAST (Fill takes
            // what is left); metricsPanel is added last so it is docked FIRST against the top edge.
            //
            this.workspacePanel.Controls.Add(this.tabWorkspace);
            this.workspacePanel.Controls.Add(this.tracePanel);
            this.workspacePanel.Controls.Add(this.bannerPanel);
            this.workspacePanel.Controls.Add(this.metricsPanel);
            this.workspacePanel.Dock = Wisej.Web.DockStyle.Fill;
            this.workspacePanel.MinimumSize = new System.Drawing.Size(320, 240);
            this.workspacePanel.Name = "workspacePanel";
            this.workspacePanel.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.workspacePanel.Size = new System.Drawing.Size(788, 596);
            //
            // metricsPanel  (Dock = Top · four fixed-width slots docked Left)
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
            // Metric card: Open
            //
            // Each card is a Panel with AppearanceKey "metric-card"; the 4 px strip on top is a Panel with
            // AppearanceKey "metric-strip" whose colour comes from a custom theme STATE (default = brandPrimary,
            // "danger", "warning", "success"), so the four accent colours are tokens, not BackColor values.
            //
            this.slotOpen.Controls.Add(this.cardOpen);
            this.slotOpen.Dock = Wisej.Web.DockStyle.Left;
            this.slotOpen.Name = "slotOpen";
            this.slotOpen.Padding = new Wisej.Web.Padding(0, 0, 8, 0);
            this.slotOpen.Size = new System.Drawing.Size(192, 76);
            this.cardOpen.AppearanceKey = "metric-card";
            this.cardOpen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardOpen.Controls.Add(this.lblOpenTitle);
            this.cardOpen.Controls.Add(this.lblOpenValue);
            this.cardOpen.Controls.Add(this.stripOpen);
            this.cardOpen.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOpen.Name = "cardOpen";
            this.stripOpen.AppearanceKey = "metric-strip";
            this.stripOpen.Dock = Wisej.Web.DockStyle.Top;
            this.stripOpen.Name = "stripOpen";
            this.stripOpen.Size = new System.Drawing.Size(182, 4);
            this.lblOpenTitle.AppearanceKey = "overline-label";
            this.lblOpenTitle.AutoSize = false;
            this.lblOpenTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOpenTitle.Name = "lblOpenTitle";
            this.lblOpenTitle.Size = new System.Drawing.Size(160, 16);
            this.lblOpenTitle.Text = "OPEN";
            this.lblOpenValue.AppearanceKey = "heading-label";
            this.lblOpenValue.AutoSize = false;
            this.lblOpenValue.Location = new System.Drawing.Point(12, 30);
            this.lblOpenValue.Name = "lblOpenValue";
            this.lblOpenValue.Size = new System.Drawing.Size(160, 36);
            this.lblOpenValue.Text = "–";
            this.lblOpenValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // Metric card: Overdue  (strip state "danger")
            //
            this.slotOverdue.Controls.Add(this.cardOverdue);
            this.slotOverdue.Dock = Wisej.Web.DockStyle.Left;
            this.slotOverdue.Name = "slotOverdue";
            this.slotOverdue.Padding = new Wisej.Web.Padding(0, 0, 8, 0);
            this.slotOverdue.Size = new System.Drawing.Size(192, 76);
            this.cardOverdue.AppearanceKey = "metric-card";
            this.cardOverdue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardOverdue.Controls.Add(this.lblOverdueTitle);
            this.cardOverdue.Controls.Add(this.lblOverdueValue);
            this.cardOverdue.Controls.Add(this.stripOverdue);
            this.cardOverdue.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOverdue.Name = "cardOverdue";
            this.stripOverdue.AppearanceKey = "metric-strip";
            this.stripOverdue.Dock = Wisej.Web.DockStyle.Top;
            this.stripOverdue.Name = "stripOverdue";
            this.stripOverdue.Size = new System.Drawing.Size(182, 4);
            this.stripOverdue.States = new string[] { "danger" };
            this.lblOverdueTitle.AppearanceKey = "overline-label";
            this.lblOverdueTitle.AutoSize = false;
            this.lblOverdueTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOverdueTitle.Name = "lblOverdueTitle";
            this.lblOverdueTitle.Size = new System.Drawing.Size(160, 16);
            this.lblOverdueTitle.Text = "OVERDUE";
            this.lblOverdueValue.AppearanceKey = "heading-label";
            this.lblOverdueValue.AutoSize = false;
            this.lblOverdueValue.Location = new System.Drawing.Point(12, 30);
            this.lblOverdueValue.Name = "lblOverdueValue";
            this.lblOverdueValue.Size = new System.Drawing.Size(160, 36);
            this.lblOverdueValue.Text = "–";
            this.lblOverdueValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // Metric card: Assigned to me  (strip state "warning")
            //
            this.slotMine.Controls.Add(this.cardMine);
            this.slotMine.Dock = Wisej.Web.DockStyle.Left;
            this.slotMine.Name = "slotMine";
            this.slotMine.Padding = new Wisej.Web.Padding(0, 0, 8, 0);
            this.slotMine.Size = new System.Drawing.Size(192, 76);
            this.cardMine.AppearanceKey = "metric-card";
            this.cardMine.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardMine.Controls.Add(this.lblMineTitle);
            this.cardMine.Controls.Add(this.lblMineValue);
            this.cardMine.Controls.Add(this.stripMine);
            this.cardMine.Dock = Wisej.Web.DockStyle.Fill;
            this.cardMine.Name = "cardMine";
            this.stripMine.AppearanceKey = "metric-strip";
            this.stripMine.Dock = Wisej.Web.DockStyle.Top;
            this.stripMine.Name = "stripMine";
            this.stripMine.Size = new System.Drawing.Size(182, 4);
            this.stripMine.States = new string[] { "warning" };
            this.lblMineTitle.AppearanceKey = "overline-label";
            this.lblMineTitle.AutoSize = false;
            this.lblMineTitle.Location = new System.Drawing.Point(12, 12);
            this.lblMineTitle.Name = "lblMineTitle";
            this.lblMineTitle.Size = new System.Drawing.Size(160, 16);
            this.lblMineTitle.Text = "ASSIGNED TO ME";
            this.lblMineValue.AppearanceKey = "heading-label";
            this.lblMineValue.AutoSize = false;
            this.lblMineValue.Location = new System.Drawing.Point(12, 30);
            this.lblMineValue.Name = "lblMineValue";
            this.lblMineValue.Size = new System.Drawing.Size(160, 36);
            this.lblMineValue.Text = "–";
            this.lblMineValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // Metric card: Closed this week  (strip state "success")
            //
            this.slotClosed.Controls.Add(this.cardClosed);
            this.slotClosed.Dock = Wisej.Web.DockStyle.Left;
            this.slotClosed.Name = "slotClosed";
            this.slotClosed.Padding = new Wisej.Web.Padding(0, 0, 8, 0);
            this.slotClosed.Size = new System.Drawing.Size(192, 76);
            this.cardClosed.AppearanceKey = "metric-card";
            this.cardClosed.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardClosed.Controls.Add(this.lblClosedTitle);
            this.cardClosed.Controls.Add(this.lblClosedValue);
            this.cardClosed.Controls.Add(this.stripClosed);
            this.cardClosed.Dock = Wisej.Web.DockStyle.Fill;
            this.cardClosed.Name = "cardClosed";
            this.stripClosed.AppearanceKey = "metric-strip";
            this.stripClosed.Dock = Wisej.Web.DockStyle.Top;
            this.stripClosed.Name = "stripClosed";
            this.stripClosed.Size = new System.Drawing.Size(182, 4);
            this.stripClosed.States = new string[] { "success" };
            this.lblClosedTitle.AppearanceKey = "overline-label";
            this.lblClosedTitle.AutoSize = false;
            this.lblClosedTitle.Location = new System.Drawing.Point(12, 12);
            this.lblClosedTitle.Name = "lblClosedTitle";
            this.lblClosedTitle.Size = new System.Drawing.Size(160, 16);
            this.lblClosedTitle.Text = "CLOSED THIS WEEK";
            this.lblClosedValue.AppearanceKey = "heading-label";
            this.lblClosedValue.AutoSize = false;
            this.lblClosedValue.Location = new System.Drawing.Point(12, 30);
            this.lblClosedValue.Name = "lblClosedValue";
            this.lblClosedValue.Size = new System.Drawing.Size(160, 36);
            this.lblClosedValue.Text = "–";
            this.lblClosedValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // bannerPanel  (Dock = Top · hidden until a failure; a hidden docked control takes no space)
            //
            this.bannerPanel.Controls.Add(this.lblBanner);
            this.bannerPanel.Dock = Wisej.Web.DockStyle.Top;
            this.bannerPanel.Name = "bannerPanel";
            this.bannerPanel.Padding = new Wisej.Web.Padding(0, 0, 0, 8);
            this.bannerPanel.Size = new System.Drawing.Size(772, 38);
            this.bannerPanel.Visible = false;
            //
            // lblBanner  (appearance "banner-label": danger background, white bold text — from the theme)
            //
            this.lblBanner.AppearanceKey = "banner-label";
            this.lblBanner.AutoEllipsis = true;
            this.lblBanner.AutoSize = false;
            this.lblBanner.Dock = Wisej.Web.DockStyle.Fill;
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tabWorkspace  (Dock = Fill · "tabview" appearance: tab buttons and page from the theme)
            //
            this.tabWorkspace.Dock = Wisej.Web.DockStyle.Fill;
            this.tabWorkspace.Name = "tabWorkspace";
            this.tabWorkspace.TabPages.AddRange(new Wisej.Web.TabPage[] {
            this.pageTickets,
            this.pageTokens});
            //
            // pageTickets  (the ticket grid; "table" + "table-header-cell" appearances)
            //
            this.pageTickets.Controls.Add(this.gridTickets);
            this.pageTickets.Name = "pageTickets";
            this.pageTickets.Padding = new Wisej.Web.Padding(8);
            this.pageTickets.Text = "Tickets";
            this.pageTickets.ToolTipText = "The grid header cells use the table-header-cell appearance: surfaceAlt background, textMuted text, surfaceHover on hover.";
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
            // ticket grid columns
            //
            this.colId.FillWeight = 60F;
            this.colId.HeaderText = "Id";
            this.colId.MinimumWidth = 64;
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            this.colTitle.FillWeight = 240F;
            this.colTitle.HeaderText = "Title";
            this.colTitle.MinimumWidth = 160;
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
            // pageTokens  (the token inspector: every named color / font of Application.Theme with its resolved value)
            //
            this.pageTokens.Controls.Add(this.gridTokens);
            this.pageTokens.Controls.Add(this.lblTokensHint);
            this.pageTokens.Name = "pageTokens";
            this.pageTokens.Padding = new Wisej.Web.Padding(8);
            this.pageTokens.Text = "Token inspector";
            this.pageTokens.ToolTipText = "Application.Theme.GetColor(token) / GetFont(token) for every AdaptiveOps token — what the server resolves, next to what the file says.";
            //
            // lblTokensHint
            //
            this.lblTokensHint.AppearanceKey = "muted-label";
            this.lblTokensHint.AutoEllipsis = true;
            this.lblTokensHint.AutoSize = false;
            this.lblTokensHint.Dock = Wisej.Web.DockStyle.Top;
            this.lblTokensHint.Name = "lblTokensHint";
            this.lblTokensHint.Size = new System.Drawing.Size(754, 24);
            this.lblTokensHint.Text = "Click \"Apply theme\" to fill this table from Application.Theme. \"Defined as\" is the raw value in the theme file; \"Resolved\" is what ClientTheme.GetColor / GetFont return on the server.";
            this.lblTokensHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // gridTokens
            //
            this.gridTokens.AllowUserToAddRows = false;
            this.gridTokens.AllowUserToDeleteRows = false;
            this.gridTokens.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridTokens.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colTokenName,
            this.colTokenKind,
            this.colTokenDefined,
            this.colTokenResolved,
            this.colTokenUsedBy});
            this.gridTokens.Dock = Wisej.Web.DockStyle.Fill;
            this.gridTokens.MultiSelect = false;
            this.gridTokens.Name = "gridTokens";
            this.gridTokens.ReadOnly = true;
            this.gridTokens.RowHeadersVisible = false;
            this.gridTokens.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            //
            // token grid columns
            //
            this.colTokenName.FillWeight = 90F;
            this.colTokenName.HeaderText = "Token";
            this.colTokenName.MinimumWidth = 100;
            this.colTokenName.Name = "colTokenName";
            this.colTokenName.ReadOnly = true;
            this.colTokenKind.FillWeight = 50F;
            this.colTokenKind.HeaderText = "Kind";
            this.colTokenKind.MinimumWidth = 56;
            this.colTokenKind.Name = "colTokenKind";
            this.colTokenKind.ReadOnly = true;
            this.colTokenDefined.FillWeight = 110F;
            this.colTokenDefined.HeaderText = "Defined as (theme file)";
            this.colTokenDefined.MinimumWidth = 120;
            this.colTokenDefined.Name = "colTokenDefined";
            this.colTokenDefined.ReadOnly = true;
            this.colTokenResolved.FillWeight = 110F;
            this.colTokenResolved.HeaderText = "Resolved (server)";
            this.colTokenResolved.MinimumWidth = 120;
            this.colTokenResolved.Name = "colTokenResolved";
            this.colTokenResolved.ReadOnly = true;
            this.colTokenUsedBy.FillWeight = 240F;
            this.colTokenUsedBy.HeaderText = "Used by";
            this.colTokenUsedBy.MinimumWidth = 160;
            this.colTokenUsedBy.Name = "colTokenUsedBy";
            this.colTokenUsedBy.ReadOnly = true;
            //
            // tracePanel  (Dock = Bottom · the gap above it is its Padding)
            //
            this.tracePanel.Controls.Add(this.traceCard);
            this.tracePanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.tracePanel.Name = "tracePanel";
            this.tracePanel.Padding = new Wisej.Web.Padding(0, 8, 0, 0);
            this.tracePanel.Size = new System.Drawing.Size(772, 176);
            //
            // traceCard  ("Layout & theme · live trace" — a Panel WITH a caption bar: panel/captionbar comes from the theme)
            //
            this.traceCard.AppearanceKey = "metric-card";
            this.traceCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.traceCard.Controls.Add(this.listTrace);
            this.traceCard.Dock = Wisej.Web.DockStyle.Fill;
            this.traceCard.Name = "traceCard";
            this.traceCard.Padding = new Wisej.Web.Padding(8);
            this.traceCard.ShowHeader = true;
            this.traceCard.Text = "Layout & theme · live trace";
            //
            // listTrace  (appearance "trace-list": ListBox variant with the "mono" theme font)
            //
            this.listTrace.AppearanceKey = "trace-list";
            this.listTrace.Dock = Wisej.Web.DockStyle.Fill;
            this.listTrace.Name = "listTrace";
            //
            // detailsPanel  (region · Dock = Right · 340 px)
            //
            this.detailsPanel.Controls.Add(this.detailsCard);
            this.detailsPanel.Dock = Wisej.Web.DockStyle.Right;
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Padding = new Wisej.Web.Padding(0, 4, 8, 4);
            this.detailsPanel.Size = new System.Drawing.Size(340, 596);
            //
            // detailsCard  ("Ticket details" editor with a caption bar; Location + Anchor inside the card until Module 5 replaces it with a TableLayoutPanel)
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
            this.detailsCard.Controls.Add(this.lblDetailsHint);
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
            // Title  (the editor whose "invalid" state the theme paints with the danger token)
            //
            this.lblTitleCaption.AutoSize = false;
            this.lblTitleCaption.Location = new System.Drawing.Point(12, 38);
            this.lblTitleCaption.Name = "lblTitleCaption";
            this.lblTitleCaption.Size = new System.Drawing.Size(140, 16);
            this.lblTitleCaption.Text = "Title";
            this.txtTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtTitle.Location = new System.Drawing.Point(12, 56);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(306, 28);
            this.txtTitle.ToolTipText = "TextBox: textbox appearance. Focused = focusFrame border; Invalid = danger border (theme state, not code).";
            this.txtTitle.Watermark = "Short summary (required, max 80 characters)";
            //
            // Priority / Status
            //
            this.lblPriorityCaption.AutoSize = false;
            this.lblPriorityCaption.Location = new System.Drawing.Point(12, 94);
            this.lblPriorityCaption.Name = "lblPriorityCaption";
            this.lblPriorityCaption.Size = new System.Drawing.Size(140, 16);
            this.lblPriorityCaption.Text = "Priority";
            this.cboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPriority.Location = new System.Drawing.Point(12, 112);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Size = new System.Drawing.Size(146, 28);
            this.lblStatusCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.Location = new System.Drawing.Point(172, 94);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(140, 16);
            this.lblStatusCaption.Text = "Status";
            this.cboStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(172, 112);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(146, 28);
            //
            // Owner / Due
            //
            this.lblOwnerCaption.AutoSize = false;
            this.lblOwnerCaption.Location = new System.Drawing.Point(12, 150);
            this.lblOwnerCaption.Name = "lblOwnerCaption";
            this.lblOwnerCaption.Size = new System.Drawing.Size(140, 16);
            this.lblOwnerCaption.Text = "Owner";
            this.txtOwner.Location = new System.Drawing.Point(12, 168);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.Size = new System.Drawing.Size(146, 28);
            this.txtOwner.Watermark = "Agent name (required)";
            this.lblDueCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblDueCaption.AutoSize = false;
            this.lblDueCaption.Location = new System.Drawing.Point(172, 150);
            this.lblDueCaption.Name = "lblDueCaption";
            this.lblDueCaption.Size = new System.Drawing.Size(140, 16);
            this.lblDueCaption.Text = "Due";
            this.dtpDue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.dtpDue.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpDue.Location = new System.Drawing.Point(172, 168);
            this.dtpDue.Name = "dtpDue";
            this.dtpDue.Size = new System.Drawing.Size(146, 28);
            //
            // Notes
            //
            this.lblNotesCaption.AutoSize = false;
            this.lblNotesCaption.Location = new System.Drawing.Point(12, 206);
            this.lblNotesCaption.Name = "lblNotesCaption";
            this.lblNotesCaption.Size = new System.Drawing.Size(140, 16);
            this.lblNotesCaption.Text = "Notes";
            this.txtNotes.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtNotes.Location = new System.Drawing.Point(12, 224);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(306, 278);
            this.txtNotes.Watermark = "Anything the next agent needs";
            //
            // btnSave  (the primary command: "Apply theme" gives it AppearanceKey = "action-button"; no BackColor anywhere)
            //
            this.btnSave.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnSave.Location = new System.Drawing.Point(12, 514);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 32);
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "Primary command. After \"Apply theme\" it uses the action-button appearance: brandPrimary, brandPrimaryHover, brandPrimaryPressed; focused and disabled still come from button.";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // lblDetailsHint
            //
            this.lblDetailsHint.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblDetailsHint.AppearanceKey = "muted-label";
            this.lblDetailsHint.AutoEllipsis = true;
            this.lblDetailsHint.AutoSize = false;
            this.lblDetailsHint.Location = new System.Drawing.Point(120, 514);
            this.lblDetailsHint.Name = "lblDetailsHint";
            this.lblDetailsHint.Size = new System.Drawing.Size(198, 32);
            this.lblDetailsHint.Text = "Validation runs on the server.";
            this.lblDetailsHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.statusCard.AppearanceKey = "metric-card";
            this.statusCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.statusCard.Controls.Add(this.lblBrowserWidth);
            this.statusCard.Controls.Add(this.lblTheme);
            this.statusCard.Controls.Add(this.lblStatus);
            this.statusCard.Dock = Wisej.Web.DockStyle.Fill;
            this.statusCard.Name = "statusCard";
            this.statusCard.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            //
            // lblStatus  (appearance "status-label": the colour comes from the custom states ok / warn / error → success / warning / danger tokens)
            //
            this.lblStatus.AppearanceKey = "status-label";
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Left;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(240, 22);
            this.lblStatus.Text = "● starting";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblBrowserWidth  (appearance "mono-label": the "mono" theme font; "Browser 1348 × 680 px · Desktop")
            //
            this.lblBrowserWidth.AppearanceKey = "mono-label";
            this.lblBrowserWidth.AutoEllipsis = true;
            this.lblBrowserWidth.AutoSize = false;
            this.lblBrowserWidth.Dock = Wisej.Web.DockStyle.Fill;
            this.lblBrowserWidth.Name = "lblBrowserWidth";
            this.lblBrowserWidth.Text = "Browser size: not read yet";
            this.lblBrowserWidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTheme  (the lab's status report: Application.Theme.Name)
            //
            this.lblTheme.AppearanceKey = "muted-label";
            this.lblTheme.AutoEllipsis = true;
            this.lblTheme.AutoSize = false;
            this.lblTheme.Dock = Wisej.Web.DockStyle.Right;
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(360, 22);
            this.lblTheme.Text = "theme: –";
            this.lblTheme.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // timerWalk  (progress path: a Component with no visual surface)
            //
            this.timerWalk.Interval = 700;
            this.timerWalk.Tick += new System.EventHandler(this.timerWalk_Tick);
            //
            // MainPage
            //
            // Docking priority follows the child order: the control added LAST is docked FIRST
            // against the page edges. So workspacePanel (Fill) goes in first and toolbarPanel last,
            // which gives the lab's order: toolbar Top, status Bottom, navigation Left, details Right,
            // workspace fills what is left. Reordering these five lines changes the layout.
            // The page background is the "page" appearance (surfaceAlt) — no BackColor here any more.
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
            this.bannerPanel.ResumeLayout(false);
            this.tabWorkspace.ResumeLayout(false);
            this.pageTickets.ResumeLayout(false);
            this.pageTokens.ResumeLayout(false);
            this.tracePanel.ResumeLayout(false);
            this.traceCard.ResumeLayout(false);
            this.detailsPanel.ResumeLayout(false);
            this.detailsCard.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.statusCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // Shell regions (the names every later module reuses)
        private Wisej.Web.Panel toolbarPanel;
        private Wisej.Web.Panel navigationPanel;
        private Wisej.Web.Panel workspacePanel;
        private Wisej.Web.Panel detailsPanel;
        private Wisej.Web.Panel statusPanel;

        // Toolbar
        private Wisej.Web.Panel toolbarCard;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Button btnApplyTheme;
        private Wisej.Web.Button btnWalkStates;
        private Wisej.Web.Button btnMissingTheme;
        private Wisej.Web.Button btnCompareTheme;
        private Wisej.Web.Button btnInvalidTicket;
        private Wisej.Web.Button btnReset;
        private Wisej.Web.Button btnClearTrace;
        private Wisej.Web.Label lblProgress;

        // Navigation rail
        private Wisej.Web.Panel navigationCard;
        private Wisej.Web.Label lblNavTitle;
        private Wisej.Web.Button btnNavDashboard;
        private Wisej.Web.Button btnNavTickets;
        private Wisej.Web.Button btnNavReports;
        private Wisej.Web.Button btnNavSettings;
        private Wisej.Web.Button btnNavHelp;

        // Workspace: metrics
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

        // Workspace: banner, tabs (ticket grid + token inspector), trace
        private Wisej.Web.Panel bannerPanel;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.TabControl tabWorkspace;
        private Wisej.Web.TabPage pageTickets;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colOwner;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.TabPage pageTokens;
        private Wisej.Web.DataGridView gridTokens;
        private Wisej.Web.DataGridViewTextBoxColumn colTokenName;
        private Wisej.Web.DataGridViewTextBoxColumn colTokenKind;
        private Wisej.Web.DataGridViewTextBoxColumn colTokenDefined;
        private Wisej.Web.DataGridViewTextBoxColumn colTokenResolved;
        private Wisej.Web.DataGridViewTextBoxColumn colTokenUsedBy;
        private Wisej.Web.Label lblTokensHint;
        private Wisej.Web.Panel tracePanel;
        private Wisej.Web.Panel traceCard;
        private Wisej.Web.ListBox listTrace;

        // Details editor
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
        private Wisej.Web.Label lblDetailsHint;

        // Status bar
        private Wisej.Web.Panel statusCard;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblBrowserWidth;
        private Wisej.Web.Label lblTheme;

        // Components
        private Wisej.Web.Timer timerWalk;
    }
}
