namespace EnterpriseOps.UI
{
    partial class WorkOrderHistoryPage
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblTitle = new Wisej.Web.Label();
            this.pnlHistory = new Wisej.Web.Panel();
            this.lblCardTitle = new Wisej.Web.Label();
            this.statusTimeline = new EnterpriseOps.Controls.StatusTimeline();
            this.chartWorkOrders = new EnterpriseOps.Widgets.WorkOrderChartWidget();
            this.lblBanner = new Wisej.Web.Label();
            this.dgvSegment = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssignedTo = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlHistory.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(948, 44);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(24, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(460, 44);
            this.lblTitle.Text = "EnterpriseOps — Work order history";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlHistory
            //
            this.pnlHistory.BackColor = System.Drawing.Color.White;
            this.pnlHistory.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlHistory.Controls.Add(this.lblCardTitle);
            this.pnlHistory.Controls.Add(this.statusTimeline);
            this.pnlHistory.Controls.Add(this.chartWorkOrders);
            this.pnlHistory.Controls.Add(this.lblBanner);
            this.pnlHistory.Controls.Add(this.dgvSegment);
            this.pnlHistory.Controls.Add(this.lblStatusBar);
            this.pnlHistory.Location = new System.Drawing.Point(24, 64);
            this.pnlHistory.Name = "pnlHistory";
            this.pnlHistory.Size = new System.Drawing.Size(900, 502);
            //
            // lblCardTitle
            //
            this.lblCardTitle.AutoSize = false;
            this.lblCardTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblCardTitle.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblCardTitle.Location = new System.Drawing.Point(20, 12);
            this.lblCardTitle.Name = "lblCardTitle";
            this.lblCardTitle.Size = new System.Drawing.Size(860, 24);
            this.lblCardTitle.Text = "Work order 2002 — history";
            this.lblCardTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // statusTimeline
            //
            this.statusTimeline.Caption = "STATUS TIMELINE";
            this.statusTimeline.EmptyText = "No history to show.";
            this.statusTimeline.Location = new System.Drawing.Point(20, 48);
            this.statusTimeline.Name = "statusTimeline";
            this.statusTimeline.Size = new System.Drawing.Size(420, 210);
            this.statusTimeline.TimeFormat = "MMM dd, HH:mm";
            this.statusTimeline.ItemSelected += new System.EventHandler<EnterpriseOps.Controls.TimelineItemEventArgs>(this.statusTimeline_ItemSelected);
            //
            // chartWorkOrders
            //
            this.chartWorkOrders.Caption = "WORK-ORDER HISTORY";
            this.chartWorkOrders.Location = new System.Drawing.Point(460, 48);
            this.chartWorkOrders.Name = "chartWorkOrders";
            this.chartWorkOrders.ShowLegend = true;
            this.chartWorkOrders.Size = new System.Drawing.Size(420, 210);
            this.chartWorkOrders.SegmentClicked += new System.EventHandler<EnterpriseOps.Widgets.ChartSegmentEventArgs>(this.chartWorkOrders_SegmentClicked);
            this.chartWorkOrders.WidgetError += new System.EventHandler<EnterpriseOps.Widgets.WidgetErrorEventArgs>(this.chartWorkOrders_WidgetError);
            //
            // lblBanner
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 236);
            this.lblBanner.Font = new System.Drawing.Font("monospace", 9F);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(124, 42, 42);
            this.lblBanner.Location = new System.Drawing.Point(20, 266);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(860, 28);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // dgvSegment
            //
            this.dgvSegment.AllowUserToAddRows = false;
            this.dgvSegment.AllowUserToDeleteRows = false;
            this.dgvSegment.AutoGenerateColumns = false;
            this.dgvSegment.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSegment.BackColor = System.Drawing.Color.White;
            this.dgvSegment.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colId,
            this.colTitle,
            this.colCustomer,
            this.colStatus,
            this.colPriority,
            this.colAssignedTo,
            this.colDue});
            this.dgvSegment.Location = new System.Drawing.Point(20, 302);
            this.dgvSegment.MultiSelect = false;
            this.dgvSegment.Name = "dgvSegment";
            this.dgvSegment.ReadOnly = true;
            this.dgvSegment.RowHeadersVisible = false;
            this.dgvSegment.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSegment.Size = new System.Drawing.Size(860, 150);
            //
            // colId
            //
            this.colId.DataPropertyName = "Id";
            this.colId.FillWeight = 9F;
            this.colId.HeaderText = "Id";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.FillWeight = 26F;
            this.colTitle.HeaderText = "Work order";
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            //
            // colCustomer
            //
            this.colCustomer.DataPropertyName = "Customer";
            this.colCustomer.FillWeight = 19F;
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.Name = "colCustomer";
            this.colCustomer.ReadOnly = true;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.FillWeight = 13F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            //
            // colPriority
            //
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.FillWeight = 11F;
            this.colPriority.HeaderText = "Priority";
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            //
            // colAssignedTo
            //
            this.colAssignedTo.DataPropertyName = "AssignedTo";
            this.colAssignedTo.FillWeight = 12F;
            this.colAssignedTo.HeaderText = "Assigned to";
            this.colAssignedTo.Name = "colAssignedTo";
            this.colAssignedTo.ReadOnly = true;
            //
            // colDue
            //
            this.colDue.DataPropertyName = "Due";
            this.colDue.FillWeight = 10F;
            this.colDue.HeaderText = "Due";
            this.colDue.Name = "colDue";
            this.colDue.ReadOnly = true;
            //
            // lblStatusBar
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(20, 462);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(860, 28);
            this.lblStatusBar.Text = "Loading…";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // WorkOrderHistoryPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlHistory);
            this.Name = "WorkOrderHistoryPage";
            this.Size = new System.Drawing.Size(948, 590);
            this.Text = "EnterpriseOps — Work order history";
            this.Load += new System.EventHandler(this.WorkOrderHistoryPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHistory.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Panel pnlHistory;
        private Wisej.Web.Label lblCardTitle;
        private EnterpriseOps.Controls.StatusTimeline statusTimeline;
        private EnterpriseOps.Widgets.WorkOrderChartWidget chartWorkOrders;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.DataGridView dgvSegment;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colAssignedTo;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.Label lblStatusBar;
    }
}
