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
            this.btnApplyStyles = new Wisej.Web.Button();
            this.btnAnimateSla = new Wisej.Web.Button();
            this.btnStyleMiss = new Wisej.Web.Button();
            this.btnReset = new Wisej.Web.Button();
            this.btnThemeGlobal = new Wisej.Web.Button();
            this.btnThemeSession = new Wisej.Web.Button();
            this.btnThemeBack = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
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
            this.lblTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
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
            this.lblDetailsHint = new Wisej.Web.Label();
            this.statusPanel = new Wisej.Web.Panel();
            this.statusCard = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.lblBrowserWidth = new Wisej.Web.Label();
            this.lblTheme = new Wisej.Web.Label();
            this.timerSla = new Wisej.Web.Timer(this.components);
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
            this.bannerPanel.SuspendLayout();
            this.gridCard.SuspendLayout();
            this.tracePanel.SuspendLayout();
            this.traceCard.SuspendLayout();
            this.detailsPanel.SuspendLayout();
            this.detailsCard.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.statusCard.SuspendLayout();
            this.SuspendLayout();
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
            // toolbarCard  (AppearanceKey "card": surface colour, border and radius come from the theme)
            //
            this.toolbarCard.AppearanceKey = "card";
            this.toolbarCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.toolbarCard.Controls.Add(this.lblAppTitle);
            this.toolbarCard.Controls.Add(this.btnApplyStyles);
            this.toolbarCard.Controls.Add(this.btnAnimateSla);
            this.toolbarCard.Controls.Add(this.btnStyleMiss);
            this.toolbarCard.Controls.Add(this.btnReset);
            this.toolbarCard.Controls.Add(this.btnThemeGlobal);
            this.toolbarCard.Controls.Add(this.btnThemeSession);
            this.toolbarCard.Controls.Add(this.btnThemeBack);
            this.toolbarCard.Controls.Add(this.btnClearTrace);
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
            // btnApplyStyles  (success path: theme state + CssClass + the one CssStyle)
            //
            this.btnApplyStyles.Location = new System.Drawing.Point(272, 7);
            this.btnApplyStyles.Name = "btnApplyStyles";
            this.btnApplyStyles.Size = new System.Drawing.Size(100, 30);
            this.btnApplyStyles.Text = "Apply styles";
            this.btnApplyStyles.ToolTipText = "Re-derive the stale state from the data, toggle the elevated class on the Overdue card and set the SLA fill (the one CssStyle).";
            this.btnApplyStyles.Click += new System.EventHandler(this.btnApplyStyles_Click);
            //
            // btnAnimateSla  (progress path: Wisej.Web.Timer drives the CssStyle 0 → 100 %)
            //
            this.btnAnimateSla.Location = new System.Drawing.Point(380, 7);
            this.btnAnimateSla.Name = "btnAnimateSla";
            this.btnAnimateSla.Size = new System.Drawing.Size(104, 30);
            this.btnAnimateSla.Text = "Animate SLA";
            this.btnAnimateSla.ToolTipText = "A server Timer moves the SLA fill from 0 to 100 % through the single CssStyle, then settles on the real value.";
            this.btnAnimateSla.Click += new System.EventHandler(this.btnAnimateSla_Click);
            //
            // btnStyleMiss  (failure path: undefined class, invalid inline style, unknown theme)
            //
            this.btnStyleMiss.Location = new System.Drawing.Point(492, 7);
            this.btnStyleMiss.Name = "btnStyleMiss";
            this.btnStyleMiss.Size = new System.Drawing.Size(90, 30);
            this.btnStyleMiss.Text = "Style miss";
            this.btnStyleMiss.ToolTipText = "Applies a class the stylesheet does not define, an invalid CssStyle string and LoadTheme(\"Nope\"): nothing breaks, nothing renders.";
            this.btnStyleMiss.Click += new System.EventHandler(this.btnStyleMiss_Click);
            //
            // btnReset  (recovery)
            //
            this.btnReset.Location = new System.Drawing.Point(590, 7);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(70, 30);
            this.btnReset.Text = "Reset";
            this.btnReset.ToolTipText = "Remove the lab states and classes, reload the AdaptiveOps theme and re-read everything from the repository.";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            //
            // btnThemeGlobal  (Application.LoadTheme — every session)
            //
            this.btnThemeGlobal.Location = new System.Drawing.Point(676, 7);
            this.btnThemeGlobal.Name = "btnThemeGlobal";
            this.btnThemeGlobal.Size = new System.Drawing.Size(170, 30);
            this.btnThemeGlobal.Text = "Global: MaterialDark-4";
            this.btnThemeGlobal.ToolTipText = "Application.LoadTheme(\"MaterialDark-4\"): swaps the shared theme for EVERY session. Open a second tab to see it follow.";
            this.btnThemeGlobal.Click += new System.EventHandler(this.btnThemeGlobal_Click);
            //
            // btnThemeSession  (Application.Theme = copy — this session only)
            //
            this.btnThemeSession.Location = new System.Drawing.Point(854, 7);
            this.btnThemeSession.Name = "btnThemeSession";
            this.btnThemeSession.Size = new System.Drawing.Size(140, 30);
            this.btnThemeSession.Text = "Session-only dark";
            this.btnThemeSession.ToolTipText = "new ClientTheme(\"AdaptiveOps-Dark\", Application.Theme) with dark colour tokens, assigned to Application.Theme: THIS session only. A second tab stays light.";
            this.btnThemeSession.Click += new System.EventHandler(this.btnThemeSession_Click);
            //
            // btnThemeBack  (Application.LoadTheme("AdaptiveOps"))
            //
            this.btnThemeBack.Location = new System.Drawing.Point(1002, 7);
            this.btnThemeBack.Name = "btnThemeBack";
            this.btnThemeBack.Size = new System.Drawing.Size(150, 30);
            this.btnThemeBack.Text = "Back to AdaptiveOps";
            this.btnThemeBack.ToolTipText = "Application.LoadTheme(\"AdaptiveOps\") and forget the session choice.";
            this.btnThemeBack.Click += new System.EventHandler(this.btnThemeBack_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Location = new System.Drawing.Point(1160, 7);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(96, 30);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
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
            // lblNavTitle  (AppearanceKey "muted-label": the muted text colour is a theme token, not a ForeColor)
            //
            this.lblNavTitle.AppearanceKey = "muted-label";
            this.lblNavTitle.AutoSize = false;
            this.lblNavTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblNavTitle.Location = new System.Drawing.Point(12, 12);
            this.lblNavTitle.Name = "lblNavTitle";
            this.lblNavTitle.Size = new System.Drawing.Size(186, 18);
            this.lblNavTitle.Text = "NAVIGATION";
            //
            // btnNavDashboard … btnNavHelp  (the rail: five plain buttons, one shared handler)
            //
            this.btnNavDashboard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavDashboard.Location = new System.Drawing.Point(12, 40);
            this.btnNavDashboard.Name = "btnNavDashboard";
            this.btnNavDashboard.Size = new System.Drawing.Size(186, 36);
            this.btnNavDashboard.Text = "Dashboard";
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
            // Child order matters here too: gridCard is added first so it is docked LAST (Fill takes
            // what is left); metricsPanel is added last so it is docked FIRST against the top edge.
            //
            this.workspacePanel.Controls.Add(this.gridCard);
            this.workspacePanel.Controls.Add(this.tracePanel);
            this.workspacePanel.Controls.Add(this.bannerPanel);
            this.workspacePanel.Controls.Add(this.slaPanel);
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
            // Each card is styled by three layers, each owning one decision:
            //   AppearanceKey "metric-card"  theme: surface, border, radius and the custom "stale" state
            //   CssClass "metric-card"       Styles/AdaptiveOps.css: transition, ".elevated" shadow
            //   (no CssStyle)                the cards carry no inline style
            // No BackColor / ForeColor / Font is set on the card, the strip, the title or the value.
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
            this.cardOpen.CssClass = "metric-card";
            this.cardOpen.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOpen.Name = "cardOpen";
            this.stripOpen.CssClass = "metric-strip strip-open";
            this.stripOpen.Dock = Wisej.Web.DockStyle.Top;
            this.stripOpen.Name = "stripOpen";
            this.stripOpen.Size = new System.Drawing.Size(182, 4);
            this.lblOpenTitle.AppearanceKey = "metric-title";
            this.lblOpenTitle.AutoSize = false;
            this.lblOpenTitle.CssClass = "metric-title";
            this.lblOpenTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOpenTitle.Name = "lblOpenTitle";
            this.lblOpenTitle.Size = new System.Drawing.Size(160, 16);
            this.lblOpenTitle.Text = "Open";
            this.lblOpenValue.AppearanceKey = "metric-value";
            this.lblOpenValue.AutoSize = false;
            this.lblOpenValue.Location = new System.Drawing.Point(12, 30);
            this.lblOpenValue.Name = "lblOpenValue";
            this.lblOpenValue.Size = new System.Drawing.Size(160, 36);
            this.lblOpenValue.Text = "–";
            //
            // Metric card: Overdue  (the card the lab marks "stale" and "elevated")
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
            this.cardOverdue.CssClass = "metric-card";
            this.cardOverdue.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOverdue.Name = "cardOverdue";
            this.stripOverdue.CssClass = "metric-strip strip-overdue";
            this.stripOverdue.Dock = Wisej.Web.DockStyle.Top;
            this.stripOverdue.Name = "stripOverdue";
            this.stripOverdue.Size = new System.Drawing.Size(182, 4);
            this.lblOverdueTitle.AppearanceKey = "metric-title";
            this.lblOverdueTitle.AutoSize = false;
            this.lblOverdueTitle.CssClass = "metric-title";
            this.lblOverdueTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOverdueTitle.Name = "lblOverdueTitle";
            this.lblOverdueTitle.Size = new System.Drawing.Size(160, 16);
            this.lblOverdueTitle.Text = "Overdue";
            this.lblOverdueValue.AppearanceKey = "metric-value";
            this.lblOverdueValue.AutoSize = false;
            this.lblOverdueValue.Location = new System.Drawing.Point(12, 30);
            this.lblOverdueValue.Name = "lblOverdueValue";
            this.lblOverdueValue.Size = new System.Drawing.Size(160, 36);
            this.lblOverdueValue.Text = "–";
            //
            // Metric card: Assigned to me
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
            this.cardMine.CssClass = "metric-card";
            this.cardMine.Dock = Wisej.Web.DockStyle.Fill;
            this.cardMine.Name = "cardMine";
            this.stripMine.CssClass = "metric-strip strip-mine";
            this.stripMine.Dock = Wisej.Web.DockStyle.Top;
            this.stripMine.Name = "stripMine";
            this.stripMine.Size = new System.Drawing.Size(182, 4);
            this.lblMineTitle.AppearanceKey = "metric-title";
            this.lblMineTitle.AutoSize = false;
            this.lblMineTitle.CssClass = "metric-title";
            this.lblMineTitle.Location = new System.Drawing.Point(12, 12);
            this.lblMineTitle.Name = "lblMineTitle";
            this.lblMineTitle.Size = new System.Drawing.Size(160, 16);
            this.lblMineTitle.Text = "Assigned to me";
            this.lblMineValue.AppearanceKey = "metric-value";
            this.lblMineValue.AutoSize = false;
            this.lblMineValue.Location = new System.Drawing.Point(12, 30);
            this.lblMineValue.Name = "lblMineValue";
            this.lblMineValue.Size = new System.Drawing.Size(160, 36);
            this.lblMineValue.Text = "–";
            //
            // Metric card: Closed this week
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
            this.cardClosed.CssClass = "metric-card";
            this.cardClosed.Dock = Wisej.Web.DockStyle.Fill;
            this.cardClosed.Name = "cardClosed";
            this.stripClosed.CssClass = "metric-strip strip-closed";
            this.stripClosed.Dock = Wisej.Web.DockStyle.Top;
            this.stripClosed.Name = "stripClosed";
            this.stripClosed.Size = new System.Drawing.Size(182, 4);
            this.lblClosedTitle.AppearanceKey = "metric-title";
            this.lblClosedTitle.AutoSize = false;
            this.lblClosedTitle.CssClass = "metric-title";
            this.lblClosedTitle.Location = new System.Drawing.Point(12, 12);
            this.lblClosedTitle.Name = "lblClosedTitle";
            this.lblClosedTitle.Size = new System.Drawing.Size(160, 16);
            this.lblClosedTitle.Text = "Closed this week";
            this.lblClosedValue.AppearanceKey = "metric-value";
            this.lblClosedValue.AutoSize = false;
            this.lblClosedValue.Location = new System.Drawing.Point(12, 30);
            this.lblClosedValue.Name = "lblClosedValue";
            this.lblClosedValue.Size = new System.Drawing.Size(160, 36);
            this.lblClosedValue.Text = "–";
            //
            // slaPanel  (Dock = Top · the SLA row: caption · track/fill · value)
            //
            this.slaPanel.Controls.Add(this.slaCard);
            this.slaPanel.Dock = Wisej.Web.DockStyle.Top;
            this.slaPanel.Name = "slaPanel";
            this.slaPanel.Padding = new Wisej.Web.Padding(0, 0, 0, 8);
            this.slaPanel.Size = new System.Drawing.Size(772, 48);
            //
            // slaCard  (children: barTrack added first so it is docked LAST and fills the middle)
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
            this.lblSlaCaption.Size = new System.Drawing.Size(250, 18);
            this.lblSlaCaption.Text = "SLA · open tickets still within their due date";
            this.lblSlaCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // barTrack / barFill  (two plain Panels; the stylesheet owns their look through .progress-track / .progress-fill;
            // MainPage.SetSlaProgress owns how much of the fill is revealed — the single CssStyle of the module)
            //
            this.barTrack.BorderStyle = Wisej.Web.BorderStyle.None;
            this.barTrack.Controls.Add(this.barFill);
            this.barTrack.CssClass = "progress-track";
            this.barTrack.Dock = Wisej.Web.DockStyle.Fill;
            this.barTrack.Name = "barTrack";
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
            // bannerPanel  (Dock = Top · hidden until a validation error; a hidden docked control takes no space)
            //
            this.bannerPanel.Controls.Add(this.lblBanner);
            this.bannerPanel.Dock = Wisej.Web.DockStyle.Top;
            this.bannerPanel.Name = "bannerPanel";
            this.bannerPanel.Padding = new Wisej.Web.Padding(0, 0, 0, 8);
            this.bannerPanel.Size = new System.Drawing.Size(772, 38);
            this.bannerPanel.Visible = false;
            //
            // lblBanner  (AppearanceKey "banner-label" = red bold text from the theme · CssClass "banner-danger" = tinted surface from the stylesheet)
            //
            this.lblBanner.AppearanceKey = "banner-label";
            this.lblBanner.AutoEllipsis = true;
            this.lblBanner.AutoSize = false;
            this.lblBanner.CssClass = "banner-danger";
            this.lblBanner.Dock = Wisej.Web.DockStyle.Fill;
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 10, 0);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // gridCard  (Dock = Fill · the ticket grid)
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
            // grid columns
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
            this.traceCard.AppearanceKey = "card";
            this.traceCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.traceCard.Controls.Add(this.listTrace);
            this.traceCard.Controls.Add(this.lblTraceTitle);
            this.traceCard.Dock = Wisej.Web.DockStyle.Fill;
            this.traceCard.Name = "traceCard";
            this.traceCard.Padding = new Wisej.Web.Padding(8);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(754, 22);
            this.lblTraceTitle.Text = "Layout & theme · live trace";
            this.lblTraceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // listTrace
            //
            this.listTrace.Dock = Wisej.Web.DockStyle.Fill;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
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
            // detailsCard  ("Ticket details" editor; Location + Anchor inside the card until Module 5 replaces it with a TableLayoutPanel)
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
            this.detailsCard.Controls.Add(this.lblDetailsHint);
            this.detailsCard.Dock = Wisej.Web.DockStyle.Fill;
            this.detailsCard.Name = "detailsCard";
            this.detailsCard.Size = new System.Drawing.Size(332, 588);
            //
            // lblDetailsTitle / lblDetailsSubtitle / lblPriorityBadge
            //
            this.lblDetailsTitle.AutoSize = false;
            this.lblDetailsTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblDetailsTitle.Location = new System.Drawing.Point(12, 12);
            this.lblDetailsTitle.Name = "lblDetailsTitle";
            this.lblDetailsTitle.Size = new System.Drawing.Size(306, 24);
            this.lblDetailsTitle.Text = "Ticket details";
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
            // lblPriorityBadge  (CssClass "compact-badge priority-<level>": the pill is app-owned CSS, the text stays in the theme font)
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
            // Title
            //
            this.lblTitleCaption.AutoSize = false;
            this.lblTitleCaption.Location = new System.Drawing.Point(12, 66);
            this.lblTitleCaption.Name = "lblTitleCaption";
            this.lblTitleCaption.Size = new System.Drawing.Size(140, 16);
            this.lblTitleCaption.Text = "Title";
            this.txtTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtTitle.Location = new System.Drawing.Point(12, 84);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(306, 28);
            this.txtTitle.Watermark = "Short summary (required, max 80 characters)";
            //
            // Priority / Status
            //
            this.lblPriorityCaption.AutoSize = false;
            this.lblPriorityCaption.Location = new System.Drawing.Point(12, 122);
            this.lblPriorityCaption.Name = "lblPriorityCaption";
            this.lblPriorityCaption.Size = new System.Drawing.Size(140, 16);
            this.lblPriorityCaption.Text = "Priority";
            this.cboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPriority.Location = new System.Drawing.Point(12, 140);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Size = new System.Drawing.Size(146, 28);
            this.lblStatusCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.Location = new System.Drawing.Point(172, 122);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(140, 16);
            this.lblStatusCaption.Text = "Status";
            this.cboStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(172, 140);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(146, 28);
            //
            // Owner / Due
            //
            this.lblOwnerCaption.AutoSize = false;
            this.lblOwnerCaption.Location = new System.Drawing.Point(12, 178);
            this.lblOwnerCaption.Name = "lblOwnerCaption";
            this.lblOwnerCaption.Size = new System.Drawing.Size(140, 16);
            this.lblOwnerCaption.Text = "Owner";
            this.txtOwner.Location = new System.Drawing.Point(12, 196);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.Size = new System.Drawing.Size(146, 28);
            this.txtOwner.Watermark = "Agent name (required)";
            this.lblDueCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblDueCaption.AutoSize = false;
            this.lblDueCaption.Location = new System.Drawing.Point(172, 178);
            this.lblDueCaption.Name = "lblDueCaption";
            this.lblDueCaption.Size = new System.Drawing.Size(140, 16);
            this.lblDueCaption.Text = "Due";
            this.dtpDue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.dtpDue.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpDue.Location = new System.Drawing.Point(172, 196);
            this.dtpDue.Name = "dtpDue";
            this.dtpDue.Size = new System.Drawing.Size(146, 28);
            //
            // Notes
            //
            this.lblNotesCaption.AutoSize = false;
            this.lblNotesCaption.Location = new System.Drawing.Point(12, 234);
            this.lblNotesCaption.Name = "lblNotesCaption";
            this.lblNotesCaption.Size = new System.Drawing.Size(140, 16);
            this.lblNotesCaption.Text = "Notes";
            this.txtNotes.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtNotes.Location = new System.Drawing.Point(12, 252);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(306, 278);
            this.txtNotes.Watermark = "Anything the next agent needs";
            //
            // btnSave  (the primary command: AppearanceKey "action-button" — default, hovered, pressed and
            // disabled all come from Themes/AdaptiveOps.theme; no BackColor, no ForeColor, no hover code)
            //
            this.btnSave.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnSave.AppearanceKey = "action-button";
            this.btnSave.Location = new System.Drawing.Point(12, 542);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 32);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // lblDetailsHint
            //
            this.lblDetailsHint.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblDetailsHint.AppearanceKey = "muted-label";
            this.lblDetailsHint.AutoEllipsis = true;
            this.lblDetailsHint.AutoSize = false;
            this.lblDetailsHint.Location = new System.Drawing.Point(120, 542);
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
            this.statusCard.AppearanceKey = "card";
            this.statusCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.statusCard.Controls.Add(this.lblBrowserWidth);
            this.statusCard.Controls.Add(this.lblTheme);
            this.statusCard.Controls.Add(this.lblStatus);
            this.statusCard.Dock = Wisej.Web.DockStyle.Fill;
            this.statusCard.Name = "statusCard";
            this.statusCard.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            //
            // lblStatus  (● ready / warn / error — AppearanceKey "status-label"; the colour follows the custom states "warn" and "error")
            //
            this.lblStatus.AppearanceKey = "status-label";
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Left;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(220, 22);
            this.lblStatus.Text = "● starting";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblBrowserWidth  ("Browser 1348 × 680 px · Desktop")
            //
            this.lblBrowserWidth.AutoEllipsis = true;
            this.lblBrowserWidth.AutoSize = false;
            this.lblBrowserWidth.Dock = Wisej.Web.DockStyle.Fill;
            this.lblBrowserWidth.Font = new System.Drawing.Font("monospace", 9F);
            this.lblBrowserWidth.Name = "lblBrowserWidth";
            this.lblBrowserWidth.Text = "Browser size: not read yet";
            this.lblBrowserWidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTheme  (theme: AdaptiveOps · shared / AdaptiveOps-Dark · this session)
            //
            this.lblTheme.AppearanceKey = "muted-label";
            this.lblTheme.AutoEllipsis = true;
            this.lblTheme.AutoSize = false;
            this.lblTheme.Dock = Wisej.Web.DockStyle.Right;
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(340, 22);
            this.lblTheme.Text = "theme: –";
            this.lblTheme.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // timerSla  (progress path: a Component with no visual surface)
            //
            this.timerSla.Interval = 120;
            this.timerSla.Tick += new System.EventHandler(this.timerSla_Tick);
            //
            // MainPage
            //
            // Docking priority follows the child order: the control added LAST is docked FIRST
            // against the page edges. So workspacePanel (Fill) goes in first and toolbarPanel last,
            // which gives the lab's order: toolbar Top, status Bottom, navigation Left, details Right,
            // workspace fills what is left. The page background is the theme's "page" appearance
            // (surfaceAlt token): no BackColor here any more.
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
            this.bannerPanel.ResumeLayout(false);
            this.gridCard.ResumeLayout(false);
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
        private Wisej.Web.Button btnApplyStyles;
        private Wisej.Web.Button btnAnimateSla;
        private Wisej.Web.Button btnStyleMiss;
        private Wisej.Web.Button btnReset;
        private Wisej.Web.Button btnThemeGlobal;
        private Wisej.Web.Button btnThemeSession;
        private Wisej.Web.Button btnThemeBack;
        private Wisej.Web.Button btnClearTrace;

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

        // Workspace: SLA row, banner, grid, trace
        private Wisej.Web.Panel slaPanel;
        private Wisej.Web.Panel slaCard;
        private Wisej.Web.Label lblSlaCaption;
        private Wisej.Web.Panel barTrack;
        private Wisej.Web.Panel barFill;
        private Wisej.Web.Label lblSlaValue;
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
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox listTrace;

        // Details editor
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
        private Wisej.Web.Label lblDetailsHint;

        // Status bar
        private Wisej.Web.Panel statusCard;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblBrowserWidth;
        private Wisej.Web.Label lblTheme;

        // Components
        private Wisej.Web.Timer timerSla;
    }
}
