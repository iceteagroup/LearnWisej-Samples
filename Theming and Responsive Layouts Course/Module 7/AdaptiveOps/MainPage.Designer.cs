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
                // Session-level events: unsubscribe with the page.
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
            this.toolbarPanel = new Wisej.Web.Panel();
            this.toolbarCard = new Wisej.Web.Panel();
            this.toolbarFlow = new Wisej.Web.FlowLayoutPanel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.btnRefresh = new Wisej.Web.Button();
            this.navigationPanel = new Wisej.Web.Panel();
            this.navRail = new AdaptiveOps.Shell.NavigationRail();
            this.workspacePanel = new Wisej.Web.Panel();
            this.metricsFlow = new Wisej.Web.FlowLayoutPanel();
            this.cardOpen = new AdaptiveOps.Shell.MetricCard();
            this.cardOverdue = new AdaptiveOps.Shell.MetricCard();
            this.cardMine = new AdaptiveOps.Shell.MetricCard();
            this.cardClosed = new AdaptiveOps.Shell.MetricCard();
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
            this.detailsPanel = new Wisej.Web.Panel();
            this.detailsEditor = new AdaptiveOps.Shell.DetailsEditor();
            this.statusPanel = new Wisej.Web.Panel();
            this.statusCard = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.widthLabel = new Wisej.Web.Label();
            this.lblProfile = new Wisej.Web.Label();
            this.toolbarPanel.SuspendLayout();
            this.toolbarCard.SuspendLayout();
            this.toolbarFlow.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            this.workspacePanel.SuspendLayout();
            this.metricsFlow.SuspendLayout();
            this.contentFlex.SuspendLayout();
            this.gridCard.SuspendLayout();
            this.gridHeader.SuspendLayout();
            this.detailsPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.statusCard.SuspendLayout();
            this.SuspendLayout();
            //
            // toolbarPanel  (Dock = Top · 56 px)
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
            // toolbarFlow  (FlowLayoutPanel · title and commands in a row)
            //
            this.toolbarFlow.Controls.Add(this.lblAppTitle);
            this.toolbarFlow.Controls.Add(this.btnRefresh);
            this.toolbarFlow.Dock = Wisej.Web.DockStyle.Fill;
            this.toolbarFlow.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.toolbarFlow.Name = "toolbarFlow";
            this.toolbarFlow.TabStop = false;
            this.toolbarFlow.WrapContents = false;
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
            // btnRefresh  (icon-only on phone and portrait tablet; ToolTipText and AccessibleName name it)
            //
            InitToolbarButton(this.btnRefresh, "Refresh", "icon-refresh", 1, "Refresh", "Refresh tickets");
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // navigationPanel  (Dock = Left · 220 px desktop, 60–68 px icon-only on narrow profiles)
            //
            this.navigationPanel.Controls.Add(this.navRail);
            this.navigationPanel.Dock = Wisej.Web.DockStyle.Left;
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Padding = new Wisej.Web.Padding(8, 4, 0, 4);
            this.navigationPanel.Size = new System.Drawing.Size(220, 596);
            this.navigationPanel.TabIndex = 1;
            this.navigationPanel.TabStop = false;
            //
            // navRail
            //
            this.navRail.Dock = Wisej.Web.DockStyle.Fill;
            this.navRail.Name = "navRail";
            this.navRail.TabIndex = 1;
            this.navRail.Navigated += new System.EventHandler<string>(this.navRail_Navigated);
            //
            // workspacePanel  (Dock = Fill · MinimumSize 320×240)
            //
            this.workspacePanel.Controls.Add(this.contentFlex);
            this.workspacePanel.Controls.Add(this.metricsFlow);
            this.workspacePanel.Dock = Wisej.Web.DockStyle.Fill;
            this.workspacePanel.MinimumSize = new System.Drawing.Size(320, 240);
            this.workspacePanel.Name = "workspacePanel";
            this.workspacePanel.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.workspacePanel.Size = new System.Drawing.Size(788, 596);
            this.workspacePanel.TabIndex = 2;
            this.workspacePanel.TabStop = false;
            //
            // metricsFlow  (FlowLayoutPanel · Dock = Top · wraps the cards; the profile sets the row budget as Height)
            //
            this.metricsFlow.Controls.Add(this.cardOpen);
            this.metricsFlow.Controls.Add(this.cardOverdue);
            this.metricsFlow.Controls.Add(this.cardMine);
            this.metricsFlow.Controls.Add(this.cardClosed);
            this.metricsFlow.Dock = Wisej.Web.DockStyle.Top;
            this.metricsFlow.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.metricsFlow.Name = "metricsFlow";
            this.metricsFlow.Size = new System.Drawing.Size(772, 92);
            this.metricsFlow.TabStop = false;
            this.metricsFlow.WrapContents = true;
            //
            // metric cards
            //
            InitCard(this.cardOpen, "cardOpen", "OPEN");
            InitCard(this.cardOverdue, "cardOverdue", "OVERDUE");
            InitCard(this.cardMine, "cardMine", "ASSIGNED TO ME");
            InitCard(this.cardClosed, "cardClosed", "CLOSED THIS WEEK");
            //
            // contentFlex  (FlexLayoutPanel · Dock = Fill · Horizontal, Vertical on a portrait phone)
            //
            this.contentFlex.Controls.Add(this.gridCard);
            this.contentFlex.Dock = Wisej.Web.DockStyle.Fill;
            this.contentFlex.LayoutStyle = Wisej.Web.FlexLayoutStyle.Horizontal;
            this.contentFlex.Name = "contentFlex";
            this.contentFlex.Spacing = 8;
            this.contentFlex.TabStop = false;
            this.contentFlex.SetFillWeight(this.gridCard, 1);
            //
            // gridCard  (theme appearance surface-card · MinimumSize keeps the fill-weighted region usable)
            //
            this.gridCard.AppearanceKey = "surface-card";
            this.gridCard.Controls.Add(this.gridTickets);
            this.gridCard.Controls.Add(this.gridHeader);
            this.gridCard.MinimumSize = new System.Drawing.Size(280, 200);
            this.gridCard.Name = "gridCard";
            this.gridCard.Padding = new Wisej.Web.Padding(8);
            this.gridCard.Size = new System.Drawing.Size(772, 400);
            this.gridCard.TabStop = false;
            //
            // gridHeader  (Dock = Top · title and the phone-only "Open details" command)
            //
            this.gridHeader.Controls.Add(this.lblWorkspaceTitle);
            this.gridHeader.Controls.Add(this.btnOpenDetails);
            this.gridHeader.Dock = Wisej.Web.DockStyle.Top;
            this.gridHeader.Name = "gridHeader";
            this.gridHeader.Padding = new Wisej.Web.Padding(0, 0, 0, 6);
            this.gridHeader.Size = new System.Drawing.Size(756, 38);
            this.gridHeader.TabStop = false;
            //
            // lblWorkspaceTitle
            //
            this.lblWorkspaceTitle.AppearanceKey = "subheading-label";
            this.lblWorkspaceTitle.AutoEllipsis = true;
            this.lblWorkspaceTitle.AutoSize = false;
            this.lblWorkspaceTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblWorkspaceTitle.Name = "lblWorkspaceTitle";
            this.lblWorkspaceTitle.TabStop = false;
            this.lblWorkspaceTitle.Text = "Tickets";
            this.lblWorkspaceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnOpenDetails
            //
            this.btnOpenDetails.AccessibleName = "Open ticket details";
            this.btnOpenDetails.AppearanceKey = "action-button";
            this.btnOpenDetails.Dock = Wisej.Web.DockStyle.Right;
            this.btnOpenDetails.ImageSource = "icon-open?color=white";
            this.btnOpenDetails.Name = "btnOpenDetails";
            this.btnOpenDetails.Size = new System.Drawing.Size(128, 32);
            this.btnOpenDetails.TabIndex = 1;
            this.btnOpenDetails.Text = "Open details";
            this.btnOpenDetails.ToolTipText = "Open details";
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
            // detailsPanel  (Dock = Right · 340 px · MinimumSize / MaximumSize set per profile in ApplyProfile)
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
            // detailsEditor  (UserControl · TableLayoutPanel form)
            //
            this.detailsEditor.Dock = Wisej.Web.DockStyle.Fill;
            this.detailsEditor.Name = "detailsEditor";
            this.detailsEditor.TabIndex = 1;
            this.detailsEditor.SaveRequested += new System.EventHandler<AdaptiveOps.Models.Ticket>(this.detailsEditor_SaveRequested);
            //
            // statusPanel  (Dock = Bottom · 28 px)
            //
            this.statusPanel.Controls.Add(this.statusCard);
            this.statusPanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Padding = new Wisej.Web.Padding(8, 0, 8, 4);
            this.statusPanel.Size = new System.Drawing.Size(1348, 28);
            this.statusPanel.TabIndex = 4;
            this.statusPanel.TabStop = false;
            //
            // statusCard  (lblStatus Left · lblProfile Right · widthLabel Fill; the last added docks first)
            //
            this.statusCard.AppearanceKey = "surface-card";
            this.statusCard.Controls.Add(this.widthLabel);
            this.statusCard.Controls.Add(this.lblProfile);
            this.statusCard.Controls.Add(this.lblStatus);
            this.statusCard.Dock = Wisej.Web.DockStyle.Fill;
            this.statusCard.Name = "statusCard";
            this.statusCard.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.statusCard.TabStop = false;
            //
            // lblStatus
            //
            this.lblStatus.AccessibleName = "Application status";
            this.lblStatus.AppearanceKey = "status-label";
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Left;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(360, 22);
            this.lblStatus.TabStop = false;
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
            this.widthLabel.TabStop = false;
            this.widthLabel.Text = "Width: –";
            this.widthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblProfile  (the test-mode profile indicator: Visible = TestMode)
            //
            this.lblProfile.AccessibleName = "Active client profile";
            this.lblProfile.AppearanceKey = "mono-label";
            this.lblProfile.AutoEllipsis = true;
            this.lblProfile.AutoSize = false;
            this.lblProfile.Dock = Wisej.Web.DockStyle.Right;
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(200, 22);
            this.lblProfile.TabStop = false;
            this.lblProfile.Text = "Profile: –";
            this.lblProfile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // MainPage
            //
            // Docking priority follows the child order: the control added last is docked first against the
            // page edges: workspace (Fill) first, toolbar last.
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
            this.toolbarPanel.ResumeLayout(false);
            this.toolbarCard.ResumeLayout(false);
            this.toolbarFlow.ResumeLayout(false);
            this.navigationPanel.ResumeLayout(false);
            this.workspacePanel.ResumeLayout(false);
            this.metricsFlow.ResumeLayout(false);
            this.contentFlex.ResumeLayout(false);
            this.gridCard.ResumeLayout(false);
            this.gridHeader.ResumeLayout(false);
            this.detailsPanel.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.statusCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private static void InitToolbarButton(Wisej.Web.Button button, string text, string icon, int tabIndex, string tooltip, string accessibleName)
        {
            button.AccessibleName = accessibleName;
            button.Display = Wisej.Web.Display.Both;
            button.ImageSource = icon;
            button.Margin = new Wisej.Web.Padding(0, 0, 8, 0);
            button.Name = "btn" + text.Replace(" ", string.Empty);
            button.Size = new System.Drawing.Size(110, 34);
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

        private Wisej.Web.Panel toolbarPanel;
        private Wisej.Web.Panel navigationPanel;
        private Wisej.Web.Panel workspacePanel;
        private Wisej.Web.Panel detailsPanel;
        private Wisej.Web.Panel statusPanel;
        private Wisej.Web.Panel toolbarCard;
        private Wisej.Web.FlowLayoutPanel toolbarFlow;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Button btnRefresh;
        private AdaptiveOps.Shell.NavigationRail navRail;
        private Wisej.Web.FlowLayoutPanel metricsFlow;
        private AdaptiveOps.Shell.MetricCard cardOpen;
        private AdaptiveOps.Shell.MetricCard cardOverdue;
        private AdaptiveOps.Shell.MetricCard cardMine;
        private AdaptiveOps.Shell.MetricCard cardClosed;
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
        private AdaptiveOps.Shell.DetailsEditor detailsEditor;
        private Wisej.Web.Panel statusCard;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label widthLabel;
        private Wisej.Web.Label lblProfile;
    }
}
