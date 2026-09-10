namespace EnterpriseOps.UI
{
    partial class WorkOrdersPage
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
            this.lblTenant = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.lblCorrelation = new Wisej.Web.Label();
            this.pnlWorkArea = new Wisej.Web.Panel();
            this.btnOpen = new Wisej.Web.Button();
            this.btnInProgress = new Wisej.Web.Button();
            this.btnDone = new Wisej.Web.Button();
            this.btnNewWorkOrder = new Wisej.Web.Button();
            this.lblStatus = new Wisej.Web.Label();
            this.dgvWorkOrders = new Wisej.Web.DataGridView();
            this.colWoId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colWoTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colWoPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colWoState = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colWoDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.lblTraceFooter = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnApprove = new Wisej.Web.Button();
            this.btnApproveStale = new Wisej.Web.Button();
            this.btnNewBlank = new Wisej.Web.Button();
            this.btnBackToDossier = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlWorkArea.SuspendLayout();
            this.pnlTrace.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (the app bar of the video's screen; its colour comes from the theme map, not from here)
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblTenant);
            this.pnlHeader.Controls.Add(this.lblUser);
            this.pnlHeader.Controls.Add(this.lblCorrelation);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1348, 44);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(24, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(560, 44);
            this.lblTitle.Text = "TicketOps — Work Orders";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTenant
            //
            this.lblTenant.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblTenant.AutoSize = false;
            this.lblTenant.Font = new System.Drawing.Font("default", 9F);
            this.lblTenant.ForeColor = System.Drawing.Color.FromArgb(230, 238, 248);
            this.lblTenant.Location = new System.Drawing.Point(700, 0);
            this.lblTenant.Name = "lblTenant";
            this.lblTenant.Size = new System.Drawing.Size(160, 44);
            this.lblTenant.Text = "tenant: contoso";
            this.lblTenant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUser
            //
            this.lblUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(870, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(280, 44);
            this.lblUser.Text = "Signed in: ana.ops · Manager";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCorrelation
            //
            this.lblCorrelation.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCorrelation.AutoSize = false;
            this.lblCorrelation.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCorrelation.ForeColor = System.Drawing.Color.FromArgb(230, 238, 248);
            this.lblCorrelation.Location = new System.Drawing.Point(1160, 0);
            this.lblCorrelation.Name = "lblCorrelation";
            this.lblCorrelation.Size = new System.Drawing.Size(164, 44);
            this.lblCorrelation.Text = "corr —";
            this.lblCorrelation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlWorkArea  (the migrated screen: the three tab buttons, + New work order, the grid, banner, footer)
            //
            this.pnlWorkArea.BackColor = System.Drawing.Color.White;
            this.pnlWorkArea.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlWorkArea.Controls.Add(this.btnOpen);
            this.pnlWorkArea.Controls.Add(this.btnInProgress);
            this.pnlWorkArea.Controls.Add(this.btnDone);
            this.pnlWorkArea.Controls.Add(this.btnNewWorkOrder);
            this.pnlWorkArea.Controls.Add(this.lblStatus);
            this.pnlWorkArea.Controls.Add(this.dgvWorkOrders);
            this.pnlWorkArea.Controls.Add(this.lblBanner);
            this.pnlWorkArea.Controls.Add(this.lblStatusBar);
            this.pnlWorkArea.Location = new System.Drawing.Point(24, 64);
            this.pnlWorkArea.Name = "pnlWorkArea";
            this.pnlWorkArea.Size = new System.Drawing.Size(860, 512);
            //
            // btnOpen / btnInProgress / btnDone  (the video's three tabs — colour and radius come from the theme map)
            //
            this.btnOpen.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnOpen.Location = new System.Drawing.Point(20, 14);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(120, 36);
            this.btnOpen.Text = "Open";
            this.btnOpen.ToolTipText = "btnOpen_Click → LoadBucket(QueueBucket.Open). The filter runs on the server, the same place it ran on Wisej.NET 3.5 (flow 2).";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            this.btnInProgress.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnInProgress.Location = new System.Drawing.Point(148, 14);
            this.btnInProgress.Name = "btnInProgress";
            this.btnInProgress.Size = new System.Drawing.Size(140, 36);
            this.btnInProgress.Text = "In progress";
            this.btnInProgress.ToolTipText = "btnInProgress_Click → LoadBucket(QueueBucket.InProgress) — flow 3 proves the filter is server-side.";
            this.btnInProgress.Click += new System.EventHandler(this.btnInProgress_Click);
            this.btnDone.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnDone.Location = new System.Drawing.Point(296, 14);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(110, 36);
            this.btnDone.Text = "Done";
            this.btnDone.ToolTipText = "btnDone_Click → LoadBucket(QueueBucket.Done).";
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            //
            // btnNewWorkOrder  (success path: a valid work order goes through WorkOrderService.Save)
            //
            this.btnNewWorkOrder.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnNewWorkOrder.Location = new System.Drawing.Point(660, 14);
            this.btnNewWorkOrder.Name = "btnNewWorkOrder";
            this.btnNewWorkOrder.Size = new System.Drawing.Size(180, 36);
            this.btnNewWorkOrder.Text = "+ New work order";
            this.btnNewWorkOrder.ToolTipText = "btnNewWorkOrder_Click → _workOrders.Save(new SaveWorkOrderCommand { … }). Same validation rules as on 3.5.";
            this.btnNewWorkOrder.Click += new System.EventHandler(this.btnNewWorkOrder_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(20, 58);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(820, 26);
            this.lblStatus.Text = "● loading…";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // dgvWorkOrders  (bound to WorkQueueRow — the projection, never the entity)
            //
            this.dgvWorkOrders.AllowUserToAddRows = false;
            this.dgvWorkOrders.AllowUserToDeleteRows = false;
            this.dgvWorkOrders.AutoGenerateColumns = false;
            this.dgvWorkOrders.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvWorkOrders.BackColor = System.Drawing.Color.White;
            this.dgvWorkOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colWoId,
            this.colWoTitle,
            this.colWoPriority,
            this.colWoState,
            this.colWoDue});
            this.dgvWorkOrders.Location = new System.Drawing.Point(20, 90);
            this.dgvWorkOrders.MultiSelect = false;
            this.dgvWorkOrders.Name = "dgvWorkOrders";
            this.dgvWorkOrders.ReadOnly = true;
            this.dgvWorkOrders.RowHeadersVisible = false;
            this.dgvWorkOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWorkOrders.Size = new System.Drawing.Size(820, 318);
            //
            // colWoId / colWoTitle / colWoPriority / colWoState / colWoDue
            //
            this.colWoId.DataPropertyName = "Id";
            this.colWoId.FillWeight = 10F;
            this.colWoId.HeaderText = "Id";
            this.colWoId.Name = "colWoId";
            this.colWoId.ReadOnly = true;
            this.colWoTitle.DataPropertyName = "Title";
            this.colWoTitle.FillWeight = 46F;
            this.colWoTitle.HeaderText = "Title";
            this.colWoTitle.Name = "colWoTitle";
            this.colWoTitle.ReadOnly = true;
            this.colWoPriority.DataPropertyName = "PriorityText";
            this.colWoPriority.FillWeight = 14F;
            this.colWoPriority.HeaderText = "Priority";
            this.colWoPriority.Name = "colWoPriority";
            this.colWoPriority.ReadOnly = true;
            this.colWoState.DataPropertyName = "State";
            this.colWoState.FillWeight = 16F;
            this.colWoState.HeaderText = "State";
            this.colWoState.Name = "colWoState";
            this.colWoState.ReadOnly = true;
            this.colWoDue.DataPropertyName = "Due";
            this.colWoDue.FillWeight = 14F;
            this.colWoDue.HeaderText = "Due";
            this.colWoDue.Name = "colWoDue";
            this.colWoDue.ReadOnly = true;
            //
            // lblBanner  (the visual-diff banner: the three theme failures the harness reports)
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(20, 416);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(820, 36);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatusBar  (the video's dark footer: "2 open work orders — Wisej.NET 4, mapped theme mixin")
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(20, 458);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(820, 36);
            this.lblStatusBar.Text = "Loading…";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlTrace  (Server · live activity trace — the same buffer the dossier page was showing)
            //
            this.pnlTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlTrace.BackColor = System.Drawing.Color.White;
            this.pnlTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTrace.Controls.Add(this.lblTraceTitle);
            this.pnlTrace.Controls.Add(this.lstTrace);
            this.pnlTrace.Controls.Add(this.lblTraceFooter);
            this.pnlTrace.Location = new System.Drawing.Point(900, 64);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(424, 512);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(392, 28);
            this.lblTraceTitle.Text = "Server · live activity trace";
            //
            // lstTrace
            //
            this.lstTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTrace.Location = new System.Drawing.Point(16, 46);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(392, 422);
            //
            // lblTraceFooter
            //
            this.lblTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceFooter.AutoSize = false;
            this.lblTraceFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTraceFooter.Location = new System.Drawing.Point(16, 474);
            this.lblTraceFooter.Name = "lblTraceFooter";
            this.lblTraceFooter.Size = new System.Drawing.Size(392, 28);
            this.lblTraceFooter.Text = "the buffer came across from MigrationDossierPage — same SessionContext (flow 9)";
            //
            // pnlActions  (bottom bar: success, the two behaviour failure paths, navigation back, clear)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.Controls.Add(this.btnApprove);
            this.pnlActions.Controls.Add(this.btnApproveStale);
            this.pnlActions.Controls.Add(this.btnNewBlank);
            this.pnlActions.Controls.Add(this.btnBackToDossier);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Location = new System.Drawing.Point(24, 592);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1300, 44);
            //
            // btnApprove  (success path: current Version accepted, New → Assigned)
            //
            this.btnApprove.Location = new System.Drawing.Point(0, 4);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(210, 36);
            this.btnApprove.Text = "Approve selected";
            this.btnApprove.ToolTipText = "btnApprove_Click → _workOrders.Approve(new ApproveWorkOrderCommand { … Version = current }). Managers only.";
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            //
            // btnApproveStale  (failure path: optimistic concurrency, exactly what flow 5 checks)
            //
            this.btnApproveStale.Location = new System.Drawing.Point(218, 4);
            this.btnApproveStale.Name = "btnApproveStale";
            this.btnApproveStale.Size = new System.Drawing.Size(250, 36);
            this.btnApproveStale.Text = "Fail: approve with stale version";
            this.btnApproveStale.ToolTipText = "Sends Version - 1. The service refuses with a typed error, nothing is written — the behaviour the migration must keep.";
            this.btnApproveStale.Click += new System.EventHandler(this.btnApproveStale_Click);
            //
            // btnNewBlank  (failure path: validation, exactly what flow 4 checks)
            //
            this.btnNewBlank.Location = new System.Drawing.Point(476, 4);
            this.btnNewBlank.Name = "btnNewBlank";
            this.btnNewBlank.Size = new System.Drawing.Size(250, 36);
            this.btnNewBlank.Text = "Fail: new work order, blank title";
            this.btnNewBlank.ToolTipText = "SaveWorkOrderCommand with a blank Title: rejected with a message, the store is unchanged.";
            this.btnNewBlank.Click += new System.EventHandler(this.btnNewBlank_Click);
            //
            // btnBackToDossier
            //
            this.btnBackToDossier.Location = new System.Drawing.Point(734, 4);
            this.btnBackToDossier.Name = "btnBackToDossier";
            this.btnBackToDossier.Size = new System.Drawing.Size(250, 36);
            this.btnBackToDossier.Text = "← Back to the migration dossier";
            this.btnBackToDossier.ToolTipText = "Application.MainPage = MigrationDossierPage — the same registry, the same session, the same trace buffer.";
            this.btnBackToDossier.Click += new System.EventHandler(this.btnBackToDossier_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearTrace.Location = new System.Drawing.Point(1190, 4);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(110, 36);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // WorkOrdersPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlWorkArea);
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlActions);
            this.Name = "WorkOrdersPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "Work Orders";
            this.Load += new System.EventHandler(this.WorkOrdersPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlWorkArea.ResumeLayout(false);
            this.pnlTrace.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblTenant;
        private Wisej.Web.Label lblUser;
        private Wisej.Web.Label lblCorrelation;
        private Wisej.Web.Panel pnlWorkArea;
        private Wisej.Web.Button btnOpen;
        private Wisej.Web.Button btnInProgress;
        private Wisej.Web.Button btnDone;
        private Wisej.Web.Button btnNewWorkOrder;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.DataGridView dgvWorkOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colWoId;
        private Wisej.Web.DataGridViewTextBoxColumn colWoTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colWoPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colWoState;
        private Wisej.Web.DataGridViewTextBoxColumn colWoDue;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatusBar;
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Label lblTraceFooter;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnApprove;
        private Wisej.Web.Button btnApproveStale;
        private Wisej.Web.Button btnNewBlank;
        private Wisej.Web.Button btnBackToDossier;
        private Wisej.Web.Button btnClearTrace;
    }
}
