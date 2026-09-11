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
                Wisej.Web.Application.BrowserSizeChanged -= this.Application_BrowserSizeChanged;

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
            this.lblAppTitle = new Wisej.Web.Label();
            this.navigationPanel = new Wisej.Web.Panel();
            this.navigationRail = new AdaptiveOps.Shell.NavigationRail();
            this.workspacePanel = new Wisej.Web.Panel();
            this.workspace = new AdaptiveOps.Shell.Workspace();
            this.detailsPanel = new Wisej.Web.Panel();
            this.detailsEditor = new AdaptiveOps.Shell.DetailsEditor();
            this.statusPanel = new Wisej.Web.Panel();
            this.statusBar = new AdaptiveOps.Shell.StatusBar();
            this.toolbarPanel.SuspendLayout();
            this.toolbarCard.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            this.workspacePanel.SuspendLayout();
            this.detailsPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
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
            this.toolbarCard.Controls.Add(this.lblAppTitle);
            this.toolbarCard.Dock = Wisej.Web.DockStyle.Fill;
            this.toolbarCard.Name = "toolbarCard";
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.Location = new System.Drawing.Point(12, 7);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(260, 30);
            this.lblAppTitle.Text = "Adaptive Operations Console";
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // navigationPanel
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
            this.navigationRail.SectionChanged += new System.EventHandler(this.navigationRail_SectionChanged);
            //
            // workspacePanel
            //
            this.workspacePanel.Controls.Add(this.workspace);
            this.workspacePanel.Dock = Wisej.Web.DockStyle.Fill;
            this.workspacePanel.MinimumSize = new System.Drawing.Size(320, 240);
            this.workspacePanel.Name = "workspacePanel";
            this.workspacePanel.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.workspacePanel.Size = new System.Drawing.Size(788, 596);
            //
            // workspace
            //
            this.workspace.Dock = Wisej.Web.DockStyle.Fill;
            this.workspace.Name = "workspace";
            this.workspace.SelectionChanged += new System.EventHandler(this.workspace_SelectionChanged);
            //
            // detailsPanel
            //
            this.detailsPanel.Controls.Add(this.detailsEditor);
            this.detailsPanel.Dock = Wisej.Web.DockStyle.Right;
            this.detailsPanel.MaximumSize = new System.Drawing.Size(480, 0);
            this.detailsPanel.MinimumSize = new System.Drawing.Size(260, 0);
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Padding = new Wisej.Web.Padding(0, 4, 8, 4);
            this.detailsPanel.Size = new System.Drawing.Size(340, 596);
            //
            // detailsEditor
            //
            this.detailsEditor.Dock = Wisej.Web.DockStyle.Fill;
            this.detailsEditor.Name = "detailsEditor";
            this.detailsEditor.Saved += new System.EventHandler<AdaptiveOps.Shell.TicketSaveEventArgs>(this.detailsEditor_Saved);
            //
            // statusPanel
            //
            this.statusPanel.Controls.Add(this.statusBar);
            this.statusPanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Padding = new Wisej.Web.Padding(8, 0, 8, 4);
            this.statusPanel.Size = new System.Drawing.Size(1348, 28);
            //
            // statusBar
            //
            this.statusBar.Dock = Wisej.Web.DockStyle.Fill;
            this.statusBar.Name = "statusBar";
            //
            // MainPage
            //
            // Docking priority follows the child order: the control added last is docked first.
            // workspacePanel (Fill) goes in first, so it takes what the four edges leave. AutoScroll
            // lets the page scroll when the regions' minimum sizes exceed the browser viewport.
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
            this.toolbarPanel.ResumeLayout(false);
            this.toolbarCard.ResumeLayout(false);
            this.navigationPanel.ResumeLayout(false);
            this.workspacePanel.ResumeLayout(false);
            this.detailsPanel.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
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
        private AdaptiveOps.Shell.NavigationRail navigationRail;
        private AdaptiveOps.Shell.Workspace workspace;
        private AdaptiveOps.Shell.DetailsEditor detailsEditor;
        private AdaptiveOps.Shell.StatusBar statusBar;
    }
}
