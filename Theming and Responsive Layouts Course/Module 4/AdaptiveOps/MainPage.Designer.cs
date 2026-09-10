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
            this.btnCompose = new Wisej.Web.Button();
            this.btnAnimate = new Wisej.Web.Button();
            this.btnSwapOrder = new Wisej.Web.Button();
            this.btnAnchorFight = new Wisej.Web.Button();
            this.btnSqueeze = new Wisej.Web.Button();
            this.btnRestore = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.lblProgress = new Wisej.Web.Label();
            this.navigationPanel = new Wisej.Web.Panel();
            this.navigationRail = new AdaptiveOps.Shell.NavigationRail();
            this.workspacePanel = new Wisej.Web.Panel();
            this.workspace = new AdaptiveOps.Shell.Workspace();
            this.detailsPanel = new Wisej.Web.Panel();
            this.detailsEditor = new AdaptiveOps.Shell.DetailsEditor();
            this.statusPanel = new Wisej.Web.Panel();
            this.statusBar = new AdaptiveOps.Shell.StatusBar();
            this.timerAnimate = new Wisej.Web.Timer(this.components);
            this.toolbarPanel.SuspendLayout();
            this.toolbarCard.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            this.workspacePanel.SuspendLayout();
            this.detailsPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // toolbarPanel  (region · Dock = Top · 56 px)
            //
            // The five region panels are transparent Dock containers: their Padding is the gap
            // between neighbouring regions (Dock ignores Margin), and the card or UserControl inside
            // fills them. The toolbar stays a Panel; the other four regions host a UserControl.
            //
            this.toolbarPanel.Controls.Add(this.toolbarCard);
            this.toolbarPanel.Dock = Wisej.Web.DockStyle.Top;
            this.toolbarPanel.Name = "toolbarPanel";
            this.toolbarPanel.Padding = new Wisej.Web.Padding(8, 8, 8, 4);
            this.toolbarPanel.Size = new System.Drawing.Size(1348, 56);
            //
            // toolbarCard  (AutoScroll + hidden bars: at narrow widths the button row scrolls instead of clipping)
            //
            this.toolbarCard.AutoScroll = true;
            this.toolbarCard.BackColor = System.Drawing.Color.White;
            this.toolbarCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.toolbarCard.Controls.Add(this.lblAppTitle);
            this.toolbarCard.Controls.Add(this.btnCompose);
            this.toolbarCard.Controls.Add(this.btnAnimate);
            this.toolbarCard.Controls.Add(this.btnSwapOrder);
            this.toolbarCard.Controls.Add(this.btnAnchorFight);
            this.toolbarCard.Controls.Add(this.btnSqueeze);
            this.toolbarCard.Controls.Add(this.btnRestore);
            this.toolbarCard.Controls.Add(this.btnClearTrace);
            this.toolbarCard.Controls.Add(this.lblProgress);
            this.toolbarCard.Dock = Wisej.Web.DockStyle.Fill;
            this.toolbarCard.Name = "toolbarCard";
            this.toolbarCard.ScrollBars = Wisej.Web.ScrollBars.Hidden;
            //
            // lblAppTitle  (fixed size: the toolbar row is absolute-positioned on purpose - a fixed-height bar)
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.Location = new System.Drawing.Point(12, 7);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(220, 30);
            this.lblAppTitle.Text = "Adaptive Operations Console";
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnCompose  (success path)
            //
            this.btnCompose.Location = new System.Drawing.Point(240, 7);
            this.btnCompose.Name = "btnCompose";
            this.btnCompose.Size = new System.Drawing.Size(110, 30);
            this.btnCompose.Text = "Compose shell";
            this.btnCompose.ToolTipText = "Re-apply the composition: five regions by Dock and child order only, then log every region's size.";
            this.btnCompose.Click += new System.EventHandler(this.btnCompose_Click);
            //
            // btnAnimate  (progress path: a Wisej.Web.Timer steps the details region's width)
            //
            this.btnAnimate.Location = new System.Drawing.Point(358, 7);
            this.btnAnimate.Name = "btnAnimate";
            this.btnAnimate.Size = new System.Drawing.Size(118, 30);
            this.btnAnimate.Text = "Animate details";
            this.btnAnimate.ToolTipText = "A server Timer changes the details region's width step by step; the docked workspace follows, MinimumSize/MaximumSize clamp.";
            this.btnAnimate.Click += new System.EventHandler(this.btnAnimate_Click);
            //
            // btnSwapOrder  (failure path 1: wrong dock order)
            //
            this.btnSwapOrder.Location = new System.Drawing.Point(484, 7);
            this.btnSwapOrder.Name = "btnSwapOrder";
            this.btnSwapOrder.Size = new System.Drawing.Size(124, 30);
            this.btnSwapOrder.Text = "Swap dock order";
            this.btnSwapOrder.ToolTipText = "Move the workspace to the end of the child order so it docks FIRST: it takes the whole page and the other regions cover it.";
            this.btnSwapOrder.Click += new System.EventHandler(this.btnSwapOrder_Click);
            //
            // btnAnchorFight  (failure path 2: Dock and Anchor on the same control)
            //
            this.btnAnchorFight.Location = new System.Drawing.Point(616, 7);
            this.btnAnchorFight.Name = "btnAnchorFight";
            this.btnAnchorFight.Size = new System.Drawing.Size(132, 30);
            this.btnAnchorFight.Text = "Dock+Anchor fight";
            this.btnAnchorFight.ToolTipText = "Set an Anchor on the docked details region and log what the framework did with Dock and Anchor.";
            this.btnAnchorFight.Click += new System.EventHandler(this.btnAnchorFight_Click);
            //
            // btnSqueeze  (failure path 3: MinimumSize larger than the space that is left)
            //
            this.btnSqueeze.Location = new System.Drawing.Point(756, 7);
            this.btnSqueeze.Name = "btnSqueeze";
            this.btnSqueeze.Size = new System.Drawing.Size(136, 30);
            this.btnSqueeze.Text = "Squeeze workspace";
            this.btnSqueeze.ToolTipText = "Raise the workspace MinimumSize above the remaining width: the guard rail holds, the page grows scrollbars.";
            this.btnSqueeze.Click += new System.EventHandler(this.btnSqueeze_Click);
            //
            // btnRestore  (recovery)
            //
            this.btnRestore.Location = new System.Drawing.Point(900, 7);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(80, 30);
            this.btnRestore.Text = "Restore";
            this.btnRestore.ToolTipText = "Restore the composition (order, docks, sizes) and the seed tickets.";
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Location = new System.Drawing.Point(988, 7);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(96, 30);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // lblProgress
            //
            this.lblProgress.AutoEllipsis = true;
            this.lblProgress.AutoSize = false;
            this.lblProgress.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblProgress.Location = new System.Drawing.Point(1096, 7);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(240, 30);
            this.lblProgress.Text = "";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // navigationPanel  (region · Dock = Left · 220 px)
            //
            // Margin = 12 on purpose: the lab asks you to confirm that the default engine ignores
            // Margin on docked children. The gap beside the rail comes from Padding, and MainPage_Load
            // logs Left/Top of this panel to prove it.
            //
            this.navigationPanel.Controls.Add(this.navigationRail);
            this.navigationPanel.Dock = Wisej.Web.DockStyle.Left;
            this.navigationPanel.Margin = new Wisej.Web.Padding(12);
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Padding = new Wisej.Web.Padding(8, 4, 0, 4);
            this.navigationPanel.Size = new System.Drawing.Size(220, 596);
            //
            // navigationRail  (UserControl · Dock = Fill inside its region)
            //
            this.navigationRail.Dock = Wisej.Web.DockStyle.Fill;
            this.navigationRail.Name = "navigationRail";
            this.navigationRail.SectionChanged += new System.EventHandler(this.navigationRail_SectionChanged);
            //
            // workspacePanel  (region · Dock = Fill · MinimumSize 320×240)
            //
            this.workspacePanel.Controls.Add(this.workspace);
            this.workspacePanel.Dock = Wisej.Web.DockStyle.Fill;
            this.workspacePanel.MinimumSize = new System.Drawing.Size(320, 240);
            this.workspacePanel.Name = "workspacePanel";
            this.workspacePanel.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.workspacePanel.Size = new System.Drawing.Size(788, 596);
            //
            // workspace  (UserControl · Dock = Fill inside its region)
            //
            this.workspace.Dock = Wisej.Web.DockStyle.Fill;
            this.workspace.Name = "workspace";
            this.workspace.SelectionChanged += new System.EventHandler(this.workspace_SelectionChanged);
            //
            // detailsPanel  (region · Dock = Right · 340 px · MinimumSize 260 · MaximumSize 480)
            //
            // MinimumSize keeps the editor usable, MaximumSize stops the region from swallowing an
            // ultra-wide monitor; "Animate details" runs into both.
            //
            this.detailsPanel.Controls.Add(this.detailsEditor);
            this.detailsPanel.Dock = Wisej.Web.DockStyle.Right;
            this.detailsPanel.MaximumSize = new System.Drawing.Size(480, 0);
            this.detailsPanel.MinimumSize = new System.Drawing.Size(260, 0);
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Padding = new Wisej.Web.Padding(0, 4, 8, 4);
            this.detailsPanel.Size = new System.Drawing.Size(340, 596);
            //
            // detailsEditor  (UserControl · Dock = Fill inside its region)
            //
            this.detailsEditor.Dock = Wisej.Web.DockStyle.Fill;
            this.detailsEditor.Name = "detailsEditor";
            this.detailsEditor.Saved += new System.EventHandler<AdaptiveOps.Shell.TicketSaveEventArgs>(this.detailsEditor_Saved);
            this.detailsEditor.Cancelled += new System.EventHandler(this.detailsEditor_Cancelled);
            //
            // statusPanel  (region · Dock = Bottom · 28 px)
            //
            this.statusPanel.Controls.Add(this.statusBar);
            this.statusPanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Padding = new Wisej.Web.Padding(8, 0, 8, 4);
            this.statusPanel.Size = new System.Drawing.Size(1348, 28);
            //
            // statusBar  (UserControl · Dock = Fill inside its region)
            //
            this.statusBar.Dock = Wisej.Web.DockStyle.Fill;
            this.statusBar.Name = "statusBar";
            //
            // timerAnimate  (progress path: a Component with no visual surface)
            //
            this.timerAnimate.Interval = 350;
            this.timerAnimate.Tick += new System.EventHandler(this.timerAnimate_Tick);
            //
            // MainPage
            //
            // Docking priority follows the child order: the control added LAST is docked FIRST
            // against the page edges. So workspacePanel (Fill) goes in first and toolbarPanel last,
            // which gives the lab's order: toolbar Top, status Bottom, navigation Left, details Right,
            // workspace fills what is left. "Swap dock order" moves workspacePanel to the end of this
            // list at runtime so you can see what happens; "Restore" puts it back.
            //
            // AutoScroll on the page: when the regions' minimum sizes add up to more than the browser
            // viewport (a phone, or "Squeeze workspace"), the page scrolls instead of overlapping.
            //
            this.AutoScroll = true;
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
            this.toolbarPanel.ResumeLayout(false);
            this.toolbarCard.ResumeLayout(false);
            this.navigationPanel.ResumeLayout(false);
            this.workspacePanel.ResumeLayout(false);
            this.detailsPanel.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // Shell regions (the names every later module reuses)
        private Wisej.Web.Panel toolbarPanel;
        private Wisej.Web.Panel navigationPanel;
        private Wisej.Web.Panel workspacePanel;
        private Wisej.Web.Panel detailsPanel;
        private Wisej.Web.Panel statusPanel;

        // The UserControls inside the regions
        private AdaptiveOps.Shell.NavigationRail navigationRail;
        private AdaptiveOps.Shell.Workspace workspace;
        private AdaptiveOps.Shell.DetailsEditor detailsEditor;
        private AdaptiveOps.Shell.StatusBar statusBar;

        // Toolbar
        private Wisej.Web.Panel toolbarCard;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Button btnCompose;
        private Wisej.Web.Button btnAnimate;
        private Wisej.Web.Button btnSwapOrder;
        private Wisej.Web.Button btnAnchorFight;
        private Wisej.Web.Button btnSqueeze;
        private Wisej.Web.Button btnRestore;
        private Wisej.Web.Button btnClearTrace;
        private Wisej.Web.Label lblProgress;

        // Components
        private Wisej.Web.Timer timerAnimate;
    }
}
