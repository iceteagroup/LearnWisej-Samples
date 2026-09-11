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
            this.dashboard = new AdaptiveOps.Layout.DashboardWorkspace();
            this.statusPanel = new Wisej.Web.Panel();
            this.statusCard = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.widthLabel = new Wisej.Web.Label();
            this.toolbarPanel.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            this.navigationRail.SuspendLayout();
            this.workspacePanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.statusCard.SuspendLayout();
            this.SuspendLayout();
            //
            // toolbarPanel  (Dock = Top: the FilterBar flow, search row + metric-card row)
            //
            this.toolbarPanel.Controls.Add(this.filterBar);
            this.toolbarPanel.Dock = Wisej.Web.DockStyle.Top;
            this.toolbarPanel.Name = "toolbarPanel";
            this.toolbarPanel.Padding = new Wisej.Web.Padding(8, 8, 8, 4);
            this.toolbarPanel.Size = new System.Drawing.Size(1348, 142);
            //
            // filterBar
            //
            this.filterBar.Dock = Wisej.Web.DockStyle.Fill;
            this.filterBar.Name = "filterBar";
            this.filterBar.Apply += new System.EventHandler(this.filterBar_Apply);
            //
            // navigationPanel
            //
            this.navigationPanel.Controls.Add(this.navigationRail);
            this.navigationPanel.Dock = Wisej.Web.DockStyle.Left;
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Padding = new Wisej.Web.Padding(8, 4, 0, 4);
            this.navigationPanel.Size = new System.Drawing.Size(220, 568);
            //
            // navigationRail  (FlowLayoutPanel TopDown: the gap between the buttons is their Margin)
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
            // btnNavDashboard
            //
            this.btnNavDashboard.Margin = new Wisej.Web.Padding(0, 0, 0, 8);
            this.btnNavDashboard.Name = "btnNavDashboard";
            this.btnNavDashboard.Size = new System.Drawing.Size(186, 36);
            this.btnNavDashboard.Text = "Dashboard";
            this.btnNavDashboard.Click += new System.EventHandler(this.btnNav_Click);
            //
            // btnNavTickets
            //
            this.btnNavTickets.Margin = new Wisej.Web.Padding(0, 0, 0, 8);
            this.btnNavTickets.Name = "btnNavTickets";
            this.btnNavTickets.Size = new System.Drawing.Size(186, 36);
            this.btnNavTickets.Text = "Tickets";
            this.btnNavTickets.Click += new System.EventHandler(this.btnNav_Click);
            //
            // btnNavReports
            //
            this.btnNavReports.Margin = new Wisej.Web.Padding(0, 0, 0, 8);
            this.btnNavReports.Name = "btnNavReports";
            this.btnNavReports.Size = new System.Drawing.Size(186, 36);
            this.btnNavReports.Text = "Reports";
            this.btnNavReports.Click += new System.EventHandler(this.btnNav_Click);
            //
            // btnNavSettings
            //
            this.btnNavSettings.Margin = new Wisej.Web.Padding(0, 0, 0, 8);
            this.btnNavSettings.Name = "btnNavSettings";
            this.btnNavSettings.Size = new System.Drawing.Size(186, 36);
            this.btnNavSettings.Text = "Settings";
            this.btnNavSettings.Click += new System.EventHandler(this.btnNav_Click);
            //
            // btnNavHelp
            //
            this.btnNavHelp.Margin = new Wisej.Web.Padding(0, 0, 0, 8);
            this.btnNavHelp.Name = "btnNavHelp";
            this.btnNavHelp.Size = new System.Drawing.Size(186, 36);
            this.btnNavHelp.Text = "Help";
            this.btnNavHelp.Click += new System.EventHandler(this.btnNav_Click);
            //
            // workspacePanel
            //
            this.workspacePanel.Controls.Add(this.dashboard);
            this.workspacePanel.Dock = Wisej.Web.DockStyle.Fill;
            this.workspacePanel.MinimumSize = new System.Drawing.Size(320, 240);
            this.workspacePanel.Name = "workspacePanel";
            this.workspacePanel.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.workspacePanel.Size = new System.Drawing.Size(1128, 568);
            //
            // dashboard  (Layout/DashboardWorkspace: built in code with Wisej.Web.Markup; Dock = Fill is set there)
            //
            this.dashboard.Name = "dashboard";
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
            this.statusCard.Controls.Add(this.widthLabel);
            this.statusCard.Controls.Add(this.lblStatus);
            this.statusCard.Dock = Wisej.Web.DockStyle.Fill;
            this.statusCard.Name = "statusCard";
            this.statusCard.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            //
            // lblStatus
            //
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Left;
            this.lblStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(480, 22);
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
            // Docking priority follows the child order: the control added last is docked first.
            // workspacePanel (Fill) goes in first and takes what the edges leave.
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.workspacePanel);
            this.Controls.Add(this.navigationPanel);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.toolbarPanel);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 738);
            this.Text = "Adaptive Operations Console";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.toolbarPanel.ResumeLayout(false);
            this.navigationPanel.ResumeLayout(false);
            this.navigationRail.ResumeLayout(false);
            this.workspacePanel.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.statusCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel toolbarPanel;
        private Wisej.Web.Panel navigationPanel;
        private Wisej.Web.Panel workspacePanel;
        private Wisej.Web.Panel statusPanel;
        private AdaptiveOps.Layout.FilterBar filterBar;
        private Wisej.Web.FlowLayoutPanel navigationRail;
        private Wisej.Web.Label lblNavTitle;
        private Wisej.Web.Button btnNavDashboard;
        private Wisej.Web.Button btnNavTickets;
        private Wisej.Web.Button btnNavReports;
        private Wisej.Web.Button btnNavSettings;
        private Wisej.Web.Button btnNavHelp;
        private AdaptiveOps.Layout.DashboardWorkspace dashboard;
        private Wisej.Web.Panel statusCard;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label widthLabel;
    }
}
