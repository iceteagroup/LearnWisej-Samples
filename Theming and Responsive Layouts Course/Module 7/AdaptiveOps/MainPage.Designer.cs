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
                // Session-level events: unsubscribe with the page (lab step "unsubscribe in Dispose").
                Wisej.Web.Application.BrowserSizeChanged -= this.Application_BrowserSizeChanged;
                Wisej.Web.Application.ResponsiveProfileChanged -= this.Application_ResponsiveProfileChanged;

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
            this.lblAppTitle = new Wisej.Web.Label();
            this.btnReview = new Wisej.Web.Button();
            this.btnReviewProfiles = new Wisej.Web.Button();
            this.btnInjectViolation = new Wisej.Web.Button();
            this.btnRecover = new Wisej.Web.Button();
            this.btnBulkFill = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.lblProgress = new Wisej.Web.Label();
            this.navigationPanel = new Wisej.Web.Panel();
            this.navRail = new AdaptiveOps.Shell.NavigationRail();
            this.workspacePanel = new Wisej.Web.Panel();
            this.metricsFlow = new Wisej.Web.FlowLayoutPanel();
            this.cardOpen = new AdaptiveOps.Shell.MetricCard();
            this.cardOverdue = new AdaptiveOps.Shell.MetricCard();
            this.cardMine = new AdaptiveOps.Shell.MetricCard();
            this.cardClosed = new AdaptiveOps.Shell.MetricCard();
            this.cardCost = new AdaptiveOps.Shell.LayoutCostCard();
            this.bannerPanel = new Wisej.Web.Panel();
            this.lblBanner = new Wisej.Web.Label();
            this.contentFlex = new Wisej.Web.FlexLayoutPanel();
            this.gridCard = new Wisej.Web.Panel();
            this.gridHeader = new Wisej.Web.Panel();
            this.lblWorkspaceTitle = new Wisej.Web.Label();
            this.btnOpenDetails = new Wisej.Web.Button();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.traceCard = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.detailsPanel = new Wisej.Web.Panel();
            this.detailsEditor = new AdaptiveOps.Shell.DetailsEditor();
            this.statusPanel = new Wisej.Web.Panel();
            this.statusCard = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.lblBrowserWidth = new Wisej.Web.Label();
            this.lblTheme = new Wisej.Web.Label();
            this.lblProfile = new Wisej.Web.Label();
            this.timerReview = new Wisej.Web.Timer(this.components);
            this.toolbarPanel.SuspendLayout();
            this.toolbarCard.SuspendLayout();
            this.toolbarFlow.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            this.workspacePanel.SuspendLayout();
            this.metricsFlow.SuspendLayout();
            this.bannerPanel.SuspendLayout();
            this.contentFlex.SuspendLayout();
            this.gridCard.SuspendLayout();
            this.gridHeader.SuspendLayout();
            this.traceCard.SuspendLayout();
            this.detailsPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.statusCard.SuspendLayout();
            this.SuspendLayout();
            //
            // toolbarPanel  (region · Dock = Top · 56 px)
            //
            // The five region panels are transparent Dock containers: their Padding is the gap between
            // neighbouring regions (Dock ignores Margin) and the themed card inside fills them.
            //
            this.toolbarPanel.Controls.Add(this.toolbarCard);
            this.toolbarPanel.Dock = Wisej.Web.DockStyle.Top;
            this.toolbarPanel.Name = "toolbarPanel";
            this.toolbarPanel.Padding = new Wisej.Web.Padding(8, 8, 8, 4);
            this.toolbarPanel.Size = new System.Drawing.Size(1348, 56);
            this.toolbarPanel.TabIndex = 0;
            this.toolbarPanel.TabStop = false;
            //
            // toolbarCard  (theme appearance surface-card)
            //
            this.toolbarCard.AppearanceKey = "surface-card";
            this.toolbarCard.Controls.Add(this.toolbarFlow);
            this.toolbarCard.Dock = Wisej.Web.DockStyle.Fill;
            this.toolbarCard.Name = "toolbarCard";
            this.toolbarCard.Padding = new Wisej.Web.Padding(6, 5, 6, 5);
            this.toolbarCard.TabStop = false;
            //
            // toolbarFlow  (FlowLayoutPanel · commands in a row · lblProgress takes the remaining width by FillWeight)
            //
            this.toolbarFlow.Controls.Add(this.lblAppTitle);
            this.toolbarFlow.Controls.Add(this.btnReview);
            this.toolbarFlow.Controls.Add(this.btnReviewProfiles);
            this.toolbarFlow.Controls.Add(this.btnInjectViolation);
            this.toolbarFlow.Controls.Add(this.btnRecover);
            this.toolbarFlow.Controls.Add(this.btnBulkFill);
            this.toolbarFlow.Controls.Add(this.btnClearTrace);
            this.toolbarFlow.Controls.Add(this.lblProgress);
            this.toolbarFlow.Dock = Wisej.Web.DockStyle.Fill;
            this.toolbarFlow.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.toolbarFlow.Name = "toolbarFlow";
            this.toolbarFlow.TabStop = false;
            this.toolbarFlow.WrapContents = false;
            this.toolbarFlow.SetFillWeight(this.lblProgress, 1);
            //
            // lblAppTitle  (theme appearance heading-label)
            //
            this.lblAppTitle.AppearanceKey = "heading-label";
            this.lblAppTitle.AutoEllipsis = true;
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Margin = new Wisej.Web.Padding(6, 0, 12, 0);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(236, 34);
            this.lblAppTitle.TabStop = false;
            this.lblAppTitle.Text = "Adaptive Operations Console";
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // Lab buttons: success · progress · failure · recovery (+ performance demo, clear)
            //
            InitToolbarButton(this.btnReview, "Governance review", "icon-check?color=white", "action-button", 1,
                "Success path: run the production checklist against the running app (all rules green).", "Run the governance review");
            InitToolbarButton(this.btnReviewProfiles, "Review all profiles", "icon-refresh", null, 2,
                "Progress path: a server Timer applies Phone, Tablet, Small Desktop and Desktop in turn and logs the checklist per profile.", "Review across every profile");
            InitToolbarButton(this.btnInjectViolation, "Inject violation", "icon-warning?color=white", "destructive-button", 3,
                "Failure path: hard-code a BackColor on one card, a CssStyle and an unknown CssClass on others, swap the theme, then re-run the review.", "Inject governance violations");
            InitToolbarButton(this.btnRecover, "Recover", "icon-undo", null, 4,
                "Recovery: clear the injected violations, reload the AdaptiveOps theme, re-run the review.", "Recover from the injected violations");
            InitToolbarButton(this.btnBulkFill, "Bulk fill", "icon-columns", null, 5,
                "Performance: fill the grid with 240 rows twice — without and with SuspendLayout/ResumeLayout — and time both on the server.", "Bulk-fill timing demo");
            InitToolbarButton(this.btnClearTrace, "Clear trace", "icon-close", null, 6,
                "Empty the live trace list.", "Clear the trace");
            this.btnReview.Click += new System.EventHandler(this.btnReview_Click);
            this.btnReviewProfiles.Click += new System.EventHandler(this.btnReviewProfiles_Click);
            this.btnInjectViolation.Click += new System.EventHandler(this.btnInjectViolation_Click);
            this.btnRecover.Click += new System.EventHandler(this.btnRecover_Click);
            this.btnBulkFill.Click += new System.EventHandler(this.btnBulkFill_Click);
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // lblProgress  (theme appearance muted-label · FillWeight 1 in the flow)
            //
            this.lblProgress.AppearanceKey = "muted-label";
            this.lblProgress.AutoEllipsis = true;
            this.lblProgress.AutoSize = false;
            this.lblProgress.Margin = new Wisej.Web.Padding(8, 0, 0, 0);
            this.lblProgress.MinimumSize = new System.Drawing.Size(80, 34);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(240, 34);
            this.lblProgress.TabStop = false;
            this.lblProgress.Text = "";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // navigationPanel  (region · Dock = Left · 220 px desktop, 60–68 px icon-only on narrow profiles)
            //
            this.navigationPanel.Controls.Add(this.navRail);
            this.navigationPanel.Dock = Wisej.Web.DockStyle.Left;
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Padding = new Wisej.Web.Padding(8, 4, 0, 4);
            this.navigationPanel.Size = new System.Drawing.Size(220, 596);
            this.navigationPanel.TabIndex = 1;
            this.navigationPanel.TabStop = false;
            //
            // navRail  (UserControl · Dock = Fill)
            //
            this.navRail.Dock = Wisej.Web.DockStyle.Fill;
            this.navRail.Name = "navRail";
            this.navRail.TabIndex = 1;
            this.navRail.Navigated += new System.EventHandler<string>(this.navRail_Navigated);
            //
            // workspacePanel  (region · Dock = Fill · MinimumSize 320×240)
            //
            // Child order: contentFlex first (docked LAST, Fill), then bannerPanel and metricsFlow (Top).
            //
            this.workspacePanel.Controls.Add(this.contentFlex);
            this.workspacePanel.Controls.Add(this.bannerPanel);
            this.workspacePanel.Controls.Add(this.metricsFlow);
            this.workspacePanel.Dock = Wisej.Web.DockStyle.Fill;
            this.workspacePanel.MinimumSize = new System.Drawing.Size(320, 240);
            this.workspacePanel.Name = "workspacePanel";
            this.workspacePanel.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.workspacePanel.Size = new System.Drawing.Size(788, 596);
            this.workspacePanel.TabIndex = 2;
            this.workspacePanel.TabStop = false;
            //
            // metricsFlow  (FlowLayoutPanel · Dock = Top · wraps the five cards; the profile sets the row budget as Height)
            //
            this.metricsFlow.Controls.Add(this.cardOpen);
            this.metricsFlow.Controls.Add(this.cardOverdue);
            this.metricsFlow.Controls.Add(this.cardMine);
            this.metricsFlow.Controls.Add(this.cardClosed);
            this.metricsFlow.Controls.Add(this.cardCost);
            this.metricsFlow.Dock = Wisej.Web.DockStyle.Top;
            this.metricsFlow.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.metricsFlow.Name = "metricsFlow";
            this.metricsFlow.Size = new System.Drawing.Size(772, 92);
            this.metricsFlow.TabStop = false;
            this.metricsFlow.WrapContents = true;
            //
            // metric cards  (UserControls · Margin is honoured by the flow engine)
            //
            InitCard(this.cardOpen, "cardOpen", "OPEN");
            InitCard(this.cardOverdue, "cardOverdue", "OVERDUE");
            InitCard(this.cardMine, "cardMine", "ASSIGNED TO ME");
            InitCard(this.cardClosed, "cardClosed", "CLOSED THIS WEEK");
            this.cardCost.Margin = new Wisej.Web.Padding(0, 0, 8, 8);
            this.cardCost.Name = "cardCost";
            this.cardCost.Size = new System.Drawing.Size(140, 84);
            //
            // bannerPanel  (theme appearance banner-danger · Dock = Top · hidden until a rejection)
            //
            this.bannerPanel.AppearanceKey = "banner-danger";
            this.bannerPanel.Controls.Add(this.lblBanner);
            this.bannerPanel.Dock = Wisej.Web.DockStyle.Top;
            this.bannerPanel.Name = "bannerPanel";
            this.bannerPanel.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.bannerPanel.Size = new System.Drawing.Size(772, 36);
            this.bannerPanel.TabStop = false;
            this.bannerPanel.Visible = false;
            this.lblBanner.AccessibleName = "Rejection banner";
            this.lblBanner.AutoEllipsis = true;
            this.lblBanner.AutoSize = false;
            this.lblBanner.Dock = Wisej.Web.DockStyle.Fill;
            this.lblBanner.ImageSource = "icon-error";
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.TabStop = false;
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // contentFlex  (FlexLayoutPanel · Dock = Fill · grid card weight 3, trace card weight 2 · Horizontal on desktop, Vertical on phone)
            //
            this.contentFlex.Controls.Add(this.gridCard);
            this.contentFlex.Controls.Add(this.traceCard);
            this.contentFlex.Dock = Wisej.Web.DockStyle.Fill;
            this.contentFlex.LayoutStyle = Wisej.Web.FlexLayoutStyle.Horizontal;
            this.contentFlex.Name = "contentFlex";
            this.contentFlex.Spacing = 8;
            this.contentFlex.TabStop = false;
            this.contentFlex.SetFillWeight(this.gridCard, 3);
            this.contentFlex.SetFillWeight(this.traceCard, 2);
            //
            // gridCard  (theme appearance surface-card · MinimumSize keeps the fill-weighted region usable)
            //
            this.gridCard.AppearanceKey = "surface-card";
            this.gridCard.Controls.Add(this.gridTickets);
            this.gridCard.Controls.Add(this.gridHeader);
            this.gridCard.MinimumSize = new System.Drawing.Size(280, 200);
            this.gridCard.Name = "gridCard";
            this.gridCard.Padding = new Wisej.Web.Padding(8);
            this.gridCard.Size = new System.Drawing.Size(460, 400);
            this.gridCard.TabStop = false;
            //
            // gridHeader  (Dock = Top · title + the phone-only "Open details" command)
            //
            this.gridHeader.Controls.Add(this.lblWorkspaceTitle);
            this.gridHeader.Controls.Add(this.btnOpenDetails);
            this.gridHeader.Dock = Wisej.Web.DockStyle.Top;
            this.gridHeader.Name = "gridHeader";
            this.gridHeader.Padding = new Wisej.Web.Padding(0, 0, 0, 6);
            this.gridHeader.Size = new System.Drawing.Size(444, 38);
            this.gridHeader.TabStop = false;
            this.lblWorkspaceTitle.AppearanceKey = "subheading-label";
            this.lblWorkspaceTitle.AutoEllipsis = true;
            this.lblWorkspaceTitle.AutoSize = false;
            this.lblWorkspaceTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblWorkspaceTitle.Name = "lblWorkspaceTitle";
            this.lblWorkspaceTitle.TabStop = false;
            this.lblWorkspaceTitle.Text = "Tickets";
            this.lblWorkspaceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenDetails.AccessibleName = "Open ticket details";
            this.btnOpenDetails.AppearanceKey = "action-button";
            this.btnOpenDetails.Dock = Wisej.Web.DockStyle.Right;
            this.btnOpenDetails.ImageSource = "icon-open?color=white";
            this.btnOpenDetails.Name = "btnOpenDetails";
            this.btnOpenDetails.Size = new System.Drawing.Size(128, 32);
            this.btnOpenDetails.TabIndex = 1;
            this.btnOpenDetails.Text = "Open details";
            this.btnOpenDetails.ToolTipText = "Open the selected ticket in the editor dialog (phone profiles)";
            this.btnOpenDetails.Visible = false;
            this.btnOpenDetails.Click += new System.EventHandler(this.btnOpenDetails_Click);
            //
            // gridTickets
            //
            this.gridTickets.AccessibleName = "Ticket grid";
            this.gridTickets.AccessibleDescription = "Operations tickets: id, title, priority, status, owner and due date. Select a row to edit it.";
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
            this.gridTickets.MinimumSize = new System.Drawing.Size(240, 160);
            this.gridTickets.MultiSelect = false;
            this.gridTickets.Name = "gridTickets";
            this.gridTickets.ReadOnly = true;
            this.gridTickets.RowHeadersVisible = false;
            this.gridTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridTickets.TabIndex = 2;
            this.gridTickets.SelectionChanged += new System.EventHandler(this.gridTickets_SelectionChanged);
            //
            // grid columns  (Visible per profile: phone shows Id · Title · Status)
            //
            InitColumn(this.colId, "colId", "Id", 60F, 64);
            InitColumn(this.colTitle, "colTitle", "Title", 240F, 160);
            InitColumn(this.colPriority, "colPriority", "Priority", 70F, 64);
            InitColumn(this.colStatus, "colStatus", "Status", 70F, 64);
            InitColumn(this.colOwner, "colOwner", "Owner", 70F, 64);
            InitColumn(this.colDue, "colDue", "Due", 90F, 90);
            //
            // traceCard  ("Layout & theme · live trace" · theme appearance surface-card)
            //
            this.traceCard.AppearanceKey = "surface-card";
            this.traceCard.Controls.Add(this.listTrace);
            this.traceCard.Controls.Add(this.lblTraceTitle);
            this.traceCard.MinimumSize = new System.Drawing.Size(200, 120);
            this.traceCard.Name = "traceCard";
            this.traceCard.Padding = new Wisej.Web.Padding(8);
            this.traceCard.Size = new System.Drawing.Size(304, 400);
            this.traceCard.TabStop = false;
            this.lblTraceTitle.AppearanceKey = "subheading-label";
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Padding = new Wisej.Web.Padding(0, 0, 0, 6);
            this.lblTraceTitle.Size = new System.Drawing.Size(288, 30);
            this.lblTraceTitle.TabStop = false;
            this.lblTraceTitle.Text = "Layout & theme · live trace";
            this.lblTraceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // listTrace  (theme appearance trace-list: mono font · CSS class trace-list)
            //
            this.listTrace.AccessibleName = "Layout and theme live trace";
            this.listTrace.AppearanceKey = "trace-list";
            this.listTrace.CssClass = "trace-list";
            this.listTrace.Dock = Wisej.Web.DockStyle.Fill;
            this.listTrace.Name = "listTrace";
            this.listTrace.TabIndex = 3;
            //
            // detailsPanel  (region · Dock = Right · 340 px · MinimumSize / MaximumSize set per profile in ApplyProfile)
            //
            this.detailsPanel.Controls.Add(this.detailsEditor);
            this.detailsPanel.Dock = Wisej.Web.DockStyle.Right;
            this.detailsPanel.MaximumSize = new System.Drawing.Size(420, 0);
            this.detailsPanel.MinimumSize = new System.Drawing.Size(280, 0);
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Padding = new Wisej.Web.Padding(0, 4, 8, 4);
            this.detailsPanel.Size = new System.Drawing.Size(340, 596);
            this.detailsPanel.TabIndex = 3;
            this.detailsPanel.TabStop = false;
            //
            // detailsEditor  (UserControl · TableLayoutPanel form · Dock = Fill)
            //
            this.detailsEditor.Dock = Wisej.Web.DockStyle.Fill;
            this.detailsEditor.Name = "detailsEditor";
            this.detailsEditor.TabIndex = 1;
            this.detailsEditor.SaveRequested += new System.EventHandler<AdaptiveOps.Models.Ticket>(this.detailsEditor_SaveRequested);
            //
            // statusPanel  (region · Dock = Bottom · 28 px)
            //
            this.statusPanel.Controls.Add(this.statusCard);
            this.statusPanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Padding = new Wisej.Web.Padding(8, 0, 8, 4);
            this.statusPanel.Size = new System.Drawing.Size(1348, 28);
            this.statusPanel.TabIndex = 4;
            this.statusPanel.TabStop = false;
            //
            // statusCard  (labels docked Left · Fill · Right · Right; the last added docks first)
            //
            this.statusCard.AppearanceKey = "surface-card";
            this.statusCard.Controls.Add(this.lblBrowserWidth);
            this.statusCard.Controls.Add(this.lblTheme);
            this.statusCard.Controls.Add(this.lblProfile);
            this.statusCard.Controls.Add(this.lblStatus);
            this.statusCard.Dock = Wisej.Web.DockStyle.Fill;
            this.statusCard.Name = "statusCard";
            this.statusCard.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.statusCard.TabStop = false;
            //
            // lblStatus  (● ready / warn / error via theme states ok · warn · error on status-label)
            //
            this.lblStatus.AccessibleName = "Application status";
            this.lblStatus.AppearanceKey = "status-label";
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Left;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(240, 22);
            this.lblStatus.TabStop = false;
            this.lblStatus.Text = "● starting";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblBrowserWidth  ("Browser 1348 × 680 px · Desktop")
            //
            this.lblBrowserWidth.AppearanceKey = "mono-label";
            this.lblBrowserWidth.AutoEllipsis = true;
            this.lblBrowserWidth.AutoSize = false;
            this.lblBrowserWidth.Dock = Wisej.Web.DockStyle.Fill;
            this.lblBrowserWidth.Name = "lblBrowserWidth";
            this.lblBrowserWidth.TabStop = false;
            this.lblBrowserWidth.Text = "Browser size: not read yet";
            this.lblBrowserWidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTheme / lblProfile  (lblProfile is the test-mode profile indicator: Visible = TestMode)
            //
            this.lblTheme.AppearanceKey = "muted-label";
            this.lblTheme.AutoEllipsis = true;
            this.lblTheme.AutoSize = false;
            this.lblTheme.Dock = Wisej.Web.DockStyle.Right;
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(180, 22);
            this.lblTheme.TabStop = false;
            this.lblTheme.Text = "theme: –";
            this.lblTheme.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblProfile.AccessibleName = "Active client profile (test mode)";
            this.lblProfile.AppearanceKey = "mono-label";
            this.lblProfile.AutoEllipsis = true;
            this.lblProfile.AutoSize = false;
            this.lblProfile.Dock = Wisej.Web.DockStyle.Right;
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(200, 22);
            this.lblProfile.TabStop = false;
            this.lblProfile.Text = "profile: –";
            this.lblProfile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblProfile.ToolTipText = "Application.ActiveProfile · shown only while ADAPTIVEOPS_TESTMODE is on";
            //
            // timerReview  (progress path: steps the review through the profiles)
            //
            this.timerReview.Interval = 900;
            this.timerReview.Tick += new System.EventHandler(this.timerReview_Tick);
            //
            // MainPage
            //
            // Docking priority follows the child order: the control added LAST is docked FIRST against
            // the page edges — workspace (Fill) first, toolbar last. The page background is the theme's
            // page appearance (surfaceAlt), not a BackColor.
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
            this.toolbarFlow.ResumeLayout(false);
            this.navigationPanel.ResumeLayout(false);
            this.workspacePanel.ResumeLayout(false);
            this.metricsFlow.ResumeLayout(false);
            this.bannerPanel.ResumeLayout(false);
            this.contentFlex.ResumeLayout(false);
            this.gridCard.ResumeLayout(false);
            this.gridHeader.ResumeLayout(false);
            this.traceCard.ResumeLayout(false);
            this.detailsPanel.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.statusCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private static void InitToolbarButton(Wisej.Web.Button button, string text, string icon, string appearanceKey, int tabIndex, string tooltip, string accessibleName)
        {
            button.AccessibleName = accessibleName;
            if (!string.IsNullOrEmpty(appearanceKey))
                button.AppearanceKey = appearanceKey;
            button.Display = Wisej.Web.Display.Both;
            button.ImageSource = icon;
            button.Margin = new Wisej.Web.Padding(0, 0, 8, 0);
            button.Name = "btn" + text.Replace(" ", string.Empty);
            button.Size = new System.Drawing.Size(150, 34);
            button.TabIndex = tabIndex;
            button.Text = text;
            button.ToolTipText = tooltip;
        }

        private static void InitCard(AdaptiveOps.Shell.MetricCard card, string name, string title)
        {
            card.Margin = new Wisej.Web.Padding(0, 0, 8, 8);
            card.Name = name;
            card.Size = new System.Drawing.Size(140, 84);
            card.Title = title;
        }

        private static void InitColumn(Wisej.Web.DataGridViewTextBoxColumn column, string name, string header, float fillWeight, int minimumWidth)
        {
            column.FillWeight = fillWeight;
            column.HeaderText = header;
            column.MinimumWidth = minimumWidth;
            column.Name = name;
            column.ReadOnly = true;
        }

        #endregion

        // Shell regions (the names every module of the course uses)
        private Wisej.Web.Panel toolbarPanel;
        private Wisej.Web.Panel navigationPanel;
        private Wisej.Web.Panel workspacePanel;
        private Wisej.Web.Panel detailsPanel;
        private Wisej.Web.Panel statusPanel;

        // Toolbar
        private Wisej.Web.Panel toolbarCard;
        private Wisej.Web.FlowLayoutPanel toolbarFlow;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Button btnReview;
        private Wisej.Web.Button btnReviewProfiles;
        private Wisej.Web.Button btnInjectViolation;
        private Wisej.Web.Button btnRecover;
        private Wisej.Web.Button btnBulkFill;
        private Wisej.Web.Button btnClearTrace;
        private Wisej.Web.Label lblProgress;

        // Navigation rail
        private AdaptiveOps.Shell.NavigationRail navRail;

        // Workspace
        private Wisej.Web.FlowLayoutPanel metricsFlow;
        private AdaptiveOps.Shell.MetricCard cardOpen;
        private AdaptiveOps.Shell.MetricCard cardOverdue;
        private AdaptiveOps.Shell.MetricCard cardMine;
        private AdaptiveOps.Shell.MetricCard cardClosed;
        private AdaptiveOps.Shell.LayoutCostCard cardCost;
        private Wisej.Web.Panel bannerPanel;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.FlexLayoutPanel contentFlex;
        private Wisej.Web.Panel gridCard;
        private Wisej.Web.Panel gridHeader;
        private Wisej.Web.Label lblWorkspaceTitle;
        private Wisej.Web.Button btnOpenDetails;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colOwner;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.Panel traceCard;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox listTrace;

        // Details editor
        private AdaptiveOps.Shell.DetailsEditor detailsEditor;

        // Status bar
        private Wisej.Web.Panel statusCard;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblBrowserWidth;
        private Wisej.Web.Label lblTheme;
        private Wisej.Web.Label lblProfile;

        // Components
        private Wisej.Web.Timer timerReview;
    }
}
