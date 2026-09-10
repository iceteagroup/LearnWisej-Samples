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
            this.filterBar = new AdaptiveOps.Layout.FilterBar();
            this.navigationPanel = new Wisej.Web.Panel();
            this.navigationRail = new Wisej.Web.FlowLayoutPanel();
            this.lblNavTitle = new Wisej.Web.Label();
            this.btnNavDashboard = new Wisej.Web.Button();
            this.btnNavTickets = new Wisej.Web.Button();
            this.btnNavReports = new Wisej.Web.Button();
            this.btnNavSettings = new Wisej.Web.Button();
            this.btnNavHelp = new Wisej.Web.Button();
            this.workspacePanel = new Wisej.Web.Panel();
            this.metricsTabs = new Wisej.Web.TabControl();
            this.tabFlow = new Wisej.Web.TabPage();
            this.flowHost = new Wisej.Web.FlowLayoutPanel();
            this.tabTable = new Wisej.Web.TabPage();
            this.tableHost = new Wisej.Web.TableLayoutPanel();
            this.tabFlex = new Wisej.Web.TabPage();
            this.flexHost = new Wisej.Web.FlexLayoutPanel();
            this.cardOpen = new AdaptiveOps.Shell.MetricCard();
            this.cardOverdue = new AdaptiveOps.Shell.MetricCard();
            this.cardMine = new AdaptiveOps.Shell.MetricCard();
            this.cardClosed = new AdaptiveOps.Shell.MetricCard();
            this.bannerPanel = new Wisej.Web.Panel();
            this.lblBanner = new Wisej.Web.Label();
            this.dashboard = new AdaptiveOps.Layout.DashboardWorkspace();
            this.tracePanel = new Wisej.Web.Panel();
            this.traceCard = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.statusPanel = new Wisej.Web.Panel();
            this.statusCard = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.lblBrowserWidth = new Wisej.Web.Label();
            this.lblEngine = new Wisej.Web.Label();
            this.timerAddCards = new Wisej.Web.Timer(this.components);
            this.timerSizes = new Wisej.Web.Timer(this.components);
            this.toolbarPanel.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            this.navigationRail.SuspendLayout();
            this.workspacePanel.SuspendLayout();
            this.metricsTabs.SuspendLayout();
            this.tabFlow.SuspendLayout();
            this.flowHost.SuspendLayout();
            this.tabTable.SuspendLayout();
            this.tableHost.SuspendLayout();
            this.tabFlex.SuspendLayout();
            this.flexHost.SuspendLayout();
            this.bannerPanel.SuspendLayout();
            this.tracePanel.SuspendLayout();
            this.traceCard.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.statusCard.SuspendLayout();
            this.SuspendLayout();
            //
            // toolbarPanel  (region · Dock = Top · 100 px: the FilterBar wraps onto two rows because of the FlowBreak on Apply)
            //
            // The five region panels are transparent Dock containers: their Padding is the gap between
            // neighbouring regions (the default layout engine ignores Margin when docking), and the white
            // card inside fills them.
            //
            this.toolbarPanel.Controls.Add(this.filterBar);
            this.toolbarPanel.Dock = Wisej.Web.DockStyle.Top;
            this.toolbarPanel.Name = "toolbarPanel";
            this.toolbarPanel.Padding = new Wisej.Web.Padding(8, 8, 8, 4);
            this.toolbarPanel.Size = new System.Drawing.Size(1348, 100);
            //
            // filterBar  (Layout/FilterBar : FlowLayoutPanel — its own Designer file holds the children and the extended properties)
            //
            this.filterBar.Dock = Wisej.Web.DockStyle.Fill;
            this.filterBar.Name = "filterBar";
            this.filterBar.Command += new System.EventHandler<AdaptiveOps.Layout.FilterBarCommandEventArgs>(this.filterBar_Command);
            //
            // navigationPanel  (region · Dock = Left · 220 px)
            //
            this.navigationPanel.Controls.Add(this.navigationRail);
            this.navigationPanel.Dock = Wisej.Web.DockStyle.Left;
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Padding = new Wisej.Web.Padding(8, 4, 0, 4);
            this.navigationPanel.Size = new System.Drawing.Size(220, 460);
            //
            // navigationRail  (FlowLayoutPanel · FlowDirection = TopDown · WrapContents = false: a single column of buttons)
            //
            // The rail buttons carry no Anchor any more — the flow engine ignores it — and the 8-px gap
            // between them is each button's Margin, which the flow engine honours.
            //
            this.navigationRail.AutoScroll = true;
            this.navigationRail.BackColor = System.Drawing.Color.White;
            this.navigationRail.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.navigationRail.Controls.Add(this.lblNavTitle);
            this.navigationRail.Controls.Add(this.btnNavDashboard);
            this.navigationRail.Controls.Add(this.btnNavTickets);
            this.navigationRail.Controls.Add(this.btnNavReports);
            this.navigationRail.Controls.Add(this.btnNavSettings);
            this.navigationRail.Controls.Add(this.btnNavHelp);
            this.navigationRail.Dock = Wisej.Web.DockStyle.Fill;
            this.navigationRail.FlowDirection = Wisej.Web.FlowDirection.TopDown;
            this.navigationRail.Name = "navigationRail";
            this.navigationRail.Padding = new Wisej.Web.Padding(12);
            this.navigationRail.WrapContents = false;
            //
            // lblNavTitle
            //
            this.lblNavTitle.AutoSize = false;
            this.lblNavTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblNavTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblNavTitle.Margin = new Wisej.Web.Padding(0, 0, 0, 10);
            this.lblNavTitle.Name = "lblNavTitle";
            this.lblNavTitle.Size = new System.Drawing.Size(186, 18);
            this.lblNavTitle.Text = "NAVIGATION";
            //
            // btnNavDashboard … btnNavHelp  (the rail: five plain buttons, one shared handler)
            //
            this.btnNavDashboard.Margin = new Wisej.Web.Padding(0, 0, 0, 8);
            this.btnNavDashboard.Name = "btnNavDashboard";
            this.btnNavDashboard.Size = new System.Drawing.Size(186, 36);
            this.btnNavDashboard.Text = "Dashboard";
            this.btnNavDashboard.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavTickets.Margin = new Wisej.Web.Padding(0, 0, 0, 8);
            this.btnNavTickets.Name = "btnNavTickets";
            this.btnNavTickets.Size = new System.Drawing.Size(186, 36);
            this.btnNavTickets.Text = "Tickets";
            this.btnNavTickets.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavReports.Margin = new Wisej.Web.Padding(0, 0, 0, 8);
            this.btnNavReports.Name = "btnNavReports";
            this.btnNavReports.Size = new System.Drawing.Size(186, 36);
            this.btnNavReports.Text = "Reports";
            this.btnNavReports.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavSettings.Margin = new Wisej.Web.Padding(0, 0, 0, 8);
            this.btnNavSettings.Name = "btnNavSettings";
            this.btnNavSettings.Size = new System.Drawing.Size(186, 36);
            this.btnNavSettings.Text = "Settings";
            this.btnNavSettings.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavHelp.Margin = new Wisej.Web.Padding(0, 0, 0, 8);
            this.btnNavHelp.Name = "btnNavHelp";
            this.btnNavHelp.Size = new System.Drawing.Size(186, 36);
            this.btnNavHelp.Text = "Help";
            this.btnNavHelp.Click += new System.EventHandler(this.btnNav_Click);
            //
            // workspacePanel  (region · Dock = Fill · MinimumSize 320×240)
            //
            // Child order matters: dashboard is added first so it is docked LAST (Fill takes what is left);
            // metricsTabs is added last so it is docked FIRST against the top edge; the banner sits between.
            //
            this.workspacePanel.Controls.Add(this.dashboard);
            this.workspacePanel.Controls.Add(this.bannerPanel);
            this.workspacePanel.Controls.Add(this.metricsTabs);
            this.workspacePanel.Dock = Wisej.Web.DockStyle.Fill;
            this.workspacePanel.MinimumSize = new System.Drawing.Size(320, 240);
            this.workspacePanel.Name = "workspacePanel";
            this.workspacePanel.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.workspacePanel.Size = new System.Drawing.Size(1128, 460);
            //
            // metricsTabs  ("Same region, three ways" · Dock = Top · 200 px)
            //
            // The same four MetricCards are re-parented into whichever host is selected; only one host is
            // visible at a time. The hosts are declared here with the Designer's Set* notation; the cards'
            // per-host extended properties (FillWeight, cell) are set by MainPage.SwitchEngine() when they move.
            //
            this.metricsTabs.Controls.Add(this.tabFlow);
            this.metricsTabs.Controls.Add(this.tabTable);
            this.metricsTabs.Controls.Add(this.tabFlex);
            this.metricsTabs.Dock = Wisej.Web.DockStyle.Top;
            this.metricsTabs.Name = "metricsTabs";
            this.metricsTabs.SelectedIndex = 0;
            this.metricsTabs.Size = new System.Drawing.Size(1112, 200);
            this.metricsTabs.SelectedIndexChanged += new System.EventHandler(this.metricsTabs_SelectedIndexChanged);
            //
            // tabFlow / flowHost  (FlowLayoutPanel · LeftToRight · WrapContents = true · AutoScroll = true)
            //
            this.tabFlow.Controls.Add(this.flowHost);
            this.tabFlow.Name = "tabFlow";
            this.tabFlow.Text = "Flow · FlowLayoutPanel";
            this.flowHost.AutoScroll = true;
            this.flowHost.Controls.Add(this.cardOpen);
            this.flowHost.Controls.Add(this.cardOverdue);
            this.flowHost.Controls.Add(this.cardMine);
            this.flowHost.Controls.Add(this.cardClosed);
            this.flowHost.Dock = Wisej.Web.DockStyle.Fill;
            this.flowHost.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.flowHost.Name = "flowHost";
            this.flowHost.Padding = new Wisej.Web.Padding(4);
            this.flowHost.WrapContents = true;
            //
            // tabTable / tableHost  (TableLayoutPanel · 4 columns Percent 25 · 1 row Absolute 84 · GrowStyle = AddRows)
            //
            this.tabTable.Controls.Add(this.tableHost);
            this.tabTable.Name = "tabTable";
            this.tabTable.Text = "Table · TableLayoutPanel";
            this.tableHost.AutoScroll = true;
            this.tableHost.ColumnCount = 4;
            this.tableHost.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            this.tableHost.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            this.tableHost.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            this.tableHost.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            this.tableHost.Dock = Wisej.Web.DockStyle.Fill;
            this.tableHost.GrowStyle = Wisej.Web.TableLayoutPanelGrowStyle.AddRows;
            this.tableHost.Name = "tableHost";
            this.tableHost.Padding = new Wisej.Web.Padding(4);
            this.tableHost.RowCount = 1;
            this.tableHost.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 84F));
            //
            // tabFlex / flexHost  (FlexLayoutPanel · Horizontal · Spacing 8)
            //
            this.tabFlex.Controls.Add(this.flexHost);
            this.tabFlex.Name = "tabFlex";
            this.tabFlex.Text = "Flex · FlexLayoutPanel";
            this.flexHost.Dock = Wisej.Web.DockStyle.Fill;
            this.flexHost.LayoutStyle = Wisej.Web.FlexLayoutStyle.Horizontal;
            this.flexHost.Name = "flexHost";
            this.flexHost.Padding = new Wisej.Web.Padding(4);
            this.flexHost.Spacing = 8;
            //
            // The four metric cards  (Shell/MetricCard · 192×76 · Margin 4 · MinimumSize 140×76 · MaximumSize 0×76)
            //
            this.cardOpen.Accent = System.Drawing.Color.FromArgb(36, 84, 166);
            this.cardOpen.Name = "cardOpen";
            this.cardOpen.Title = "Open";
            this.cardOverdue.Accent = System.Drawing.Color.FromArgb(180, 35, 24);
            this.cardOverdue.Name = "cardOverdue";
            this.cardOverdue.Title = "Overdue";
            this.cardMine.Accent = System.Drawing.Color.FromArgb(181, 71, 8);
            this.cardMine.Name = "cardMine";
            this.cardMine.Title = "Assigned to me";
            this.cardClosed.Accent = System.Drawing.Color.FromArgb(2, 122, 72);
            this.cardClosed.Name = "cardClosed";
            this.cardClosed.Title = "Closed this week";
            //
            // bannerPanel  (Dock = Top · hidden until a failure; a hidden docked control takes no space)
            //
            this.bannerPanel.Controls.Add(this.lblBanner);
            this.bannerPanel.Dock = Wisej.Web.DockStyle.Top;
            this.bannerPanel.Name = "bannerPanel";
            this.bannerPanel.Padding = new Wisej.Web.Padding(0, 8, 0, 0);
            this.bannerPanel.Size = new System.Drawing.Size(1112, 38);
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
            // dashboard  (Layout/DashboardWorkspace : FlexLayoutPanel — built in code with Wisej.Web.Markup; Dock = Fill is set there)
            //
            this.dashboard.Name = "dashboard";
            //
            // tracePanel  (region · Dock = Bottom · 150 px)
            //
            this.tracePanel.Controls.Add(this.traceCard);
            this.tracePanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.tracePanel.Name = "tracePanel";
            this.tracePanel.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.tracePanel.Size = new System.Drawing.Size(1348, 150);
            //
            // traceCard  ("Layout & theme · live trace")
            //
            this.traceCard.BackColor = System.Drawing.Color.White;
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
            this.lblTraceTitle.Size = new System.Drawing.Size(1314, 22);
            this.lblTraceTitle.Text = "Layout & theme · live trace";
            this.lblTraceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // listTrace
            //
            this.listTrace.Dock = Wisej.Web.DockStyle.Fill;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Name = "listTrace";
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
            this.statusCard.Controls.Add(this.lblBrowserWidth);
            this.statusCard.Controls.Add(this.lblEngine);
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
            this.lblStatus.Size = new System.Drawing.Size(260, 22);
            this.lblStatus.Text = "● starting";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblBrowserWidth  ("Browser 1348 × 738 px · Desktop")
            //
            this.lblBrowserWidth.AutoEllipsis = true;
            this.lblBrowserWidth.AutoSize = false;
            this.lblBrowserWidth.Dock = Wisej.Web.DockStyle.Fill;
            this.lblBrowserWidth.Font = new System.Drawing.Font("monospace", 9F);
            this.lblBrowserWidth.Name = "lblBrowserWidth";
            this.lblBrowserWidth.Text = "Browser size: not read yet";
            this.lblBrowserWidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblEngine  ("engine: Flow · 4 cards · 1 row(s)")
            //
            this.lblEngine.AutoEllipsis = true;
            this.lblEngine.AutoSize = false;
            this.lblEngine.Dock = Wisej.Web.DockStyle.Right;
            this.lblEngine.Font = new System.Drawing.Font("monospace", 9F);
            this.lblEngine.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblEngine.Name = "lblEngine";
            this.lblEngine.Size = new System.Drawing.Size(420, 22);
            this.lblEngine.Text = "engine: –";
            this.lblEngine.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // timerAddCards  (progress path: one metric card per tick)
            //
            this.timerAddCards.Interval = 700;
            this.timerAddCards.Tick += new System.EventHandler(this.timerAddCards_Tick);
            //
            // timerSizes  (debounces the "← client card bounds" trace after the engines have laid the cards out)
            //
            this.timerSizes.Interval = 300;
            this.timerSizes.Tick += new System.EventHandler(this.timerSizes_Tick);
            //
            // MainPage
            //
            // Docking priority follows the child order: the control added LAST is docked FIRST against the
            // page edges. workspacePanel (Fill) goes in first and toolbarPanel last, which gives: toolbar
            // Top, status Bottom, trace Bottom (above the status bar), navigation Left, workspace fills the rest.
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.workspacePanel);
            this.Controls.Add(this.navigationPanel);
            this.Controls.Add(this.tracePanel);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.toolbarPanel);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 738);
            this.Text = "Adaptive Operations Console";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.Resize += new System.EventHandler(this.MainPage_Resize);
            this.toolbarPanel.ResumeLayout(false);
            this.navigationPanel.ResumeLayout(false);
            this.navigationRail.ResumeLayout(false);
            this.workspacePanel.ResumeLayout(false);
            this.metricsTabs.ResumeLayout(false);
            this.tabFlow.ResumeLayout(false);
            this.flowHost.ResumeLayout(false);
            this.tabTable.ResumeLayout(false);
            this.tableHost.ResumeLayout(false);
            this.tabFlex.ResumeLayout(false);
            this.flexHost.ResumeLayout(false);
            this.bannerPanel.ResumeLayout(false);
            this.tracePanel.ResumeLayout(false);
            this.traceCard.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.statusCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // Shell regions (the names every module reuses)
        private Wisej.Web.Panel toolbarPanel;
        private Wisej.Web.Panel navigationPanel;
        private Wisej.Web.Panel workspacePanel;
        private Wisej.Web.Panel tracePanel;
        private Wisej.Web.Panel statusPanel;

        // Toolbar: the FlowLayoutPanel filter bar (Layout/FilterBar)
        private AdaptiveOps.Layout.FilterBar filterBar;

        // Navigation rail (FlowLayoutPanel, TopDown)
        private Wisej.Web.FlowLayoutPanel navigationRail;
        private Wisej.Web.Label lblNavTitle;
        private Wisej.Web.Button btnNavDashboard;
        private Wisej.Web.Button btnNavTickets;
        private Wisej.Web.Button btnNavReports;
        private Wisej.Web.Button btnNavSettings;
        private Wisej.Web.Button btnNavHelp;

        // Workspace: "Same region, three ways"
        private Wisej.Web.TabControl metricsTabs;
        private Wisej.Web.TabPage tabFlow;
        private Wisej.Web.FlowLayoutPanel flowHost;
        private Wisej.Web.TabPage tabTable;
        private Wisej.Web.TableLayoutPanel tableHost;
        private Wisej.Web.TabPage tabFlex;
        private Wisej.Web.FlexLayoutPanel flexHost;
        private AdaptiveOps.Shell.MetricCard cardOpen;
        private AdaptiveOps.Shell.MetricCard cardOverdue;
        private AdaptiveOps.Shell.MetricCard cardMine;
        private AdaptiveOps.Shell.MetricCard cardClosed;

        // Workspace: banner and the flex list/details split (Layout/DashboardWorkspace)
        private Wisej.Web.Panel bannerPanel;
        private Wisej.Web.Label lblBanner;
        private AdaptiveOps.Layout.DashboardWorkspace dashboard;

        // Trace
        private Wisej.Web.Panel traceCard;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox listTrace;

        // Status bar
        private Wisej.Web.Panel statusCard;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblBrowserWidth;
        private Wisej.Web.Label lblEngine;

        // Components
        private Wisej.Web.Timer timerAddCards;
        private Wisej.Web.Timer timerSizes;
    }
}
