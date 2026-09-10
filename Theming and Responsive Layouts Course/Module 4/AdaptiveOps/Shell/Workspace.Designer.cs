namespace AdaptiveOps.Shell
{
    partial class Workspace
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
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
            this.tabs = new Wisej.Web.TabControl();
            this.tabTickets = new Wisej.Web.TabPage();
            this.lblWorkspaceTitle = new Wisej.Web.Label();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.tabTwin = new Wisej.Web.TabPage();
            this.twin = new AdaptiveOps.Lab.ResizeCodeTwin();
            this.tracePanel = new Wisej.Web.Panel();
            this.traceCard = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
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
            this.tabs.SuspendLayout();
            this.tabTickets.SuspendLayout();
            this.tabTwin.SuspendLayout();
            this.tracePanel.SuspendLayout();
            this.traceCard.SuspendLayout();
            this.SuspendLayout();
            //
            // metricsPanel  (Dock = Top · four fixed-width slots docked Left; the slot Padding is the gap)
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
            // Metric card: Open  (title label AutoSize = true; value label fixed 160×36)
            //
            this.slotOpen.Controls.Add(this.cardOpen);
            this.slotOpen.Dock = Wisej.Web.DockStyle.Left;
            this.slotOpen.Name = "slotOpen";
            this.slotOpen.Padding = new Wisej.Web.Padding(0, 0, 8, 0);
            this.slotOpen.Size = new System.Drawing.Size(192, 76);
            this.cardOpen.BackColor = System.Drawing.Color.White;
            this.cardOpen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardOpen.Controls.Add(this.lblOpenTitle);
            this.cardOpen.Controls.Add(this.lblOpenValue);
            this.cardOpen.Controls.Add(this.stripOpen);
            this.cardOpen.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOpen.Name = "cardOpen";
            this.stripOpen.BackColor = System.Drawing.Color.FromArgb(36, 84, 166);
            this.stripOpen.Dock = Wisej.Web.DockStyle.Top;
            this.stripOpen.Name = "stripOpen";
            this.stripOpen.Size = new System.Drawing.Size(182, 4);
            this.lblOpenTitle.AutoSize = true;
            this.lblOpenTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblOpenTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblOpenTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOpenTitle.Name = "lblOpenTitle";
            this.lblOpenTitle.Text = "OPEN";
            this.lblOpenValue.AutoSize = false;
            this.lblOpenValue.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.lblOpenValue.Location = new System.Drawing.Point(12, 30);
            this.lblOpenValue.Name = "lblOpenValue";
            this.lblOpenValue.Size = new System.Drawing.Size(160, 36);
            this.lblOpenValue.Text = "–";
            //
            // Metric card: Overdue
            //
            this.slotOverdue.Controls.Add(this.cardOverdue);
            this.slotOverdue.Dock = Wisej.Web.DockStyle.Left;
            this.slotOverdue.Name = "slotOverdue";
            this.slotOverdue.Padding = new Wisej.Web.Padding(0, 0, 8, 0);
            this.slotOverdue.Size = new System.Drawing.Size(192, 76);
            this.cardOverdue.BackColor = System.Drawing.Color.White;
            this.cardOverdue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardOverdue.Controls.Add(this.lblOverdueTitle);
            this.cardOverdue.Controls.Add(this.lblOverdueValue);
            this.cardOverdue.Controls.Add(this.stripOverdue);
            this.cardOverdue.Dock = Wisej.Web.DockStyle.Fill;
            this.cardOverdue.Name = "cardOverdue";
            this.stripOverdue.BackColor = System.Drawing.Color.FromArgb(180, 35, 24);
            this.stripOverdue.Dock = Wisej.Web.DockStyle.Top;
            this.stripOverdue.Name = "stripOverdue";
            this.stripOverdue.Size = new System.Drawing.Size(182, 4);
            this.lblOverdueTitle.AutoSize = true;
            this.lblOverdueTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblOverdueTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblOverdueTitle.Location = new System.Drawing.Point(12, 12);
            this.lblOverdueTitle.Name = "lblOverdueTitle";
            this.lblOverdueTitle.Text = "OVERDUE";
            this.lblOverdueValue.AutoSize = false;
            this.lblOverdueValue.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
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
            this.cardMine.BackColor = System.Drawing.Color.White;
            this.cardMine.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardMine.Controls.Add(this.lblMineTitle);
            this.cardMine.Controls.Add(this.lblMineValue);
            this.cardMine.Controls.Add(this.stripMine);
            this.cardMine.Dock = Wisej.Web.DockStyle.Fill;
            this.cardMine.Name = "cardMine";
            this.stripMine.BackColor = System.Drawing.Color.FromArgb(181, 71, 8);
            this.stripMine.Dock = Wisej.Web.DockStyle.Top;
            this.stripMine.Name = "stripMine";
            this.stripMine.Size = new System.Drawing.Size(182, 4);
            this.lblMineTitle.AutoSize = true;
            this.lblMineTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblMineTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblMineTitle.Location = new System.Drawing.Point(12, 12);
            this.lblMineTitle.Name = "lblMineTitle";
            this.lblMineTitle.Text = "ASSIGNED TO ME";
            this.lblMineValue.AutoSize = false;
            this.lblMineValue.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
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
            this.cardClosed.BackColor = System.Drawing.Color.White;
            this.cardClosed.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardClosed.Controls.Add(this.lblClosedTitle);
            this.cardClosed.Controls.Add(this.lblClosedValue);
            this.cardClosed.Controls.Add(this.stripClosed);
            this.cardClosed.Dock = Wisej.Web.DockStyle.Fill;
            this.cardClosed.Name = "cardClosed";
            this.stripClosed.BackColor = System.Drawing.Color.FromArgb(2, 122, 72);
            this.stripClosed.Dock = Wisej.Web.DockStyle.Top;
            this.stripClosed.Name = "stripClosed";
            this.stripClosed.Size = new System.Drawing.Size(182, 4);
            this.lblClosedTitle.AutoSize = true;
            this.lblClosedTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblClosedTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblClosedTitle.Location = new System.Drawing.Point(12, 12);
            this.lblClosedTitle.Name = "lblClosedTitle";
            this.lblClosedTitle.Text = "CLOSED THIS WEEK";
            this.lblClosedValue.AutoSize = false;
            this.lblClosedValue.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.lblClosedValue.Location = new System.Drawing.Point(12, 30);
            this.lblClosedValue.Name = "lblClosedValue";
            this.lblClosedValue.Size = new System.Drawing.Size(160, 36);
            this.lblClosedValue.Text = "–";
            //
            // bannerPanel  (Dock = Top · hidden until an error; a hidden docked control takes no space)
            //
            this.bannerPanel.Controls.Add(this.lblBanner);
            this.bannerPanel.Dock = Wisej.Web.DockStyle.Top;
            this.bannerPanel.Name = "bannerPanel";
            this.bannerPanel.Padding = new Wisej.Web.Padding(0, 0, 0, 8);
            this.bannerPanel.Size = new System.Drawing.Size(772, 38);
            this.bannerPanel.Visible = false;
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
            // tabs  (Dock = Fill · "Tickets" = the docked grid, "Resize-code twin" = the before)
            //
            this.tabs.Controls.Add(this.tabTickets);
            this.tabs.Controls.Add(this.tabTwin);
            this.tabs.Dock = Wisej.Web.DockStyle.Fill;
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
            //
            // tabTickets
            //
            this.tabTickets.Controls.Add(this.gridTickets);
            this.tabTickets.Controls.Add(this.lblWorkspaceTitle);
            this.tabTickets.Name = "tabTickets";
            this.tabTickets.Padding = new Wisej.Web.Padding(8);
            this.tabTickets.Text = "Tickets · docked shell (after)";
            //
            // lblWorkspaceTitle  (Dock = Top)
            //
            this.lblWorkspaceTitle.AutoSize = false;
            this.lblWorkspaceTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblWorkspaceTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblWorkspaceTitle.Name = "lblWorkspaceTitle";
            this.lblWorkspaceTitle.Size = new System.Drawing.Size(754, 26);
            this.lblWorkspaceTitle.Text = "Tickets";
            this.lblWorkspaceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // gridTickets  (Dock = Fill · MinimumSize 300×160: the grid never collapses into a sliver)
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
            this.gridTickets.MinimumSize = new System.Drawing.Size(300, 160);
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
            // tabTwin  (the "before": five panels positioned by a Resize handler)
            //
            this.tabTwin.Controls.Add(this.twin);
            this.tabTwin.Name = "tabTwin";
            this.tabTwin.Padding = new Wisej.Web.Padding(4);
            this.tabTwin.Text = "Resize-code twin (before)";
            //
            // twin
            //
            this.twin.Dock = Wisej.Web.DockStyle.Fill;
            this.twin.Name = "twin";
            this.twin.Traced += new System.EventHandler<AdaptiveOps.Lab.TraceEventArgs>(this.twin_Traced);
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
            this.traceCard.Controls.Add(this.lblTraceTitle);
            this.traceCard.Dock = Wisej.Web.DockStyle.Fill;
            this.traceCard.Name = "traceCard";
            this.traceCard.Padding = new Wisej.Web.Padding(8);
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(754, 22);
            this.lblTraceTitle.Text = "Layout & theme · live trace";
            this.lblTraceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.listTrace.Dock = Wisej.Web.DockStyle.Fill;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Name = "listTrace";
            //
            // Workspace
            //
            // Child order = dock priority: tabs (Fill) first so it is docked LAST; metricsPanel last so
            // it is docked FIRST against the top edge, then bannerPanel below it, tracePanel at the bottom.
            // No AutoSize on this container: its children dock back to it (the circular-layout smell).
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.BorderStyle = Wisej.Web.BorderStyle.None;
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.tracePanel);
            this.Controls.Add(this.bannerPanel);
            this.Controls.Add(this.metricsPanel);
            this.Name = "Workspace";
            this.Size = new System.Drawing.Size(772, 588);
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
            this.tabs.ResumeLayout(false);
            this.tabTickets.ResumeLayout(false);
            this.tabTwin.ResumeLayout(false);
            this.tracePanel.ResumeLayout(false);
            this.traceCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // Metrics
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

        // Banner, tabs, grid, twin, trace
        private Wisej.Web.Panel bannerPanel;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.TabControl tabs;
        private Wisej.Web.TabPage tabTickets;
        private Wisej.Web.Label lblWorkspaceTitle;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colOwner;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.TabPage tabTwin;
        private AdaptiveOps.Lab.ResizeCodeTwin twin;
        private Wisej.Web.Panel tracePanel;
        private Wisej.Web.Panel traceCard;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox listTrace;
    }
}
