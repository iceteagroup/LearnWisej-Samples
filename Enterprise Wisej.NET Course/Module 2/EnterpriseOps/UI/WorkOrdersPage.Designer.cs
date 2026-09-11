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
            this.btnBackToDossier = new Wisej.Web.Button();
            this.pnlWorkArea = new Wisej.Web.Panel();
            this.btnOpen = new Wisej.Web.Button();
            this.btnInProgress = new Wisej.Web.Button();
            this.btnDone = new Wisej.Web.Button();
            this.btnNewWorkOrder = new Wisej.Web.Button();
            this.dgvWorkOrders = new Wisej.Web.DataGridView();
            this.colWoId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colWoTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colWoPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colWoState = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblStatus = new Wisej.Web.Label();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlWorkArea.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (its colour comes from the theme map at run time)
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.btnBackToDossier);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(908, 44);
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
            // btnBackToDossier
            //
            this.btnBackToDossier.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnBackToDossier.Location = new System.Drawing.Point(720, 6);
            this.btnBackToDossier.Name = "btnBackToDossier";
            this.btnBackToDossier.Size = new System.Drawing.Size(164, 32);
            this.btnBackToDossier.Text = "← Migration dossier";
            this.btnBackToDossier.Click += new System.EventHandler(this.btnBackToDossier_Click);
            //
            // pnlWorkArea
            //
            this.pnlWorkArea.BackColor = System.Drawing.Color.White;
            this.pnlWorkArea.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlWorkArea.Controls.Add(this.btnOpen);
            this.pnlWorkArea.Controls.Add(this.btnInProgress);
            this.pnlWorkArea.Controls.Add(this.btnDone);
            this.pnlWorkArea.Controls.Add(this.btnNewWorkOrder);
            this.pnlWorkArea.Controls.Add(this.dgvWorkOrders);
            this.pnlWorkArea.Controls.Add(this.lblStatus);
            this.pnlWorkArea.Controls.Add(this.lblBanner);
            this.pnlWorkArea.Controls.Add(this.lblStatusBar);
            this.pnlWorkArea.Location = new System.Drawing.Point(24, 64);
            this.pnlWorkArea.Name = "pnlWorkArea";
            this.pnlWorkArea.Size = new System.Drawing.Size(860, 512);
            //
            // btnOpen / btnInProgress / btnDone  (colour and radius come from the theme map at run time)
            //
            this.btnOpen.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnOpen.Location = new System.Drawing.Point(20, 14);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(120, 36);
            this.btnOpen.Text = "Open";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            this.btnInProgress.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnInProgress.Location = new System.Drawing.Point(148, 14);
            this.btnInProgress.Name = "btnInProgress";
            this.btnInProgress.Size = new System.Drawing.Size(140, 36);
            this.btnInProgress.Text = "In progress";
            this.btnInProgress.Click += new System.EventHandler(this.btnInProgress_Click);
            this.btnDone.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnDone.Location = new System.Drawing.Point(296, 14);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(110, 36);
            this.btnDone.Text = "Done";
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            //
            // btnNewWorkOrder
            //
            this.btnNewWorkOrder.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnNewWorkOrder.Location = new System.Drawing.Point(660, 14);
            this.btnNewWorkOrder.Name = "btnNewWorkOrder";
            this.btnNewWorkOrder.Size = new System.Drawing.Size(180, 36);
            this.btnNewWorkOrder.Text = "+ New work order";
            this.btnNewWorkOrder.Click += new System.EventHandler(this.btnNewWorkOrder_Click);
            //
            // dgvWorkOrders
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
            this.colWoState});
            this.dgvWorkOrders.Location = new System.Drawing.Point(20, 62);
            this.dgvWorkOrders.MultiSelect = false;
            this.dgvWorkOrders.Name = "dgvWorkOrders";
            this.dgvWorkOrders.ReadOnly = true;
            this.dgvWorkOrders.RowHeadersVisible = false;
            this.dgvWorkOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWorkOrders.Size = new System.Drawing.Size(820, 316);
            //
            // colWoId / colWoTitle / colWoPriority / colWoState
            //
            this.colWoId.DataPropertyName = "Id";
            this.colWoId.FillWeight = 10F;
            this.colWoId.HeaderText = "Id";
            this.colWoId.Name = "colWoId";
            this.colWoId.ReadOnly = true;
            this.colWoTitle.DataPropertyName = "Title";
            this.colWoTitle.FillWeight = 52F;
            this.colWoTitle.HeaderText = "Title";
            this.colWoTitle.Name = "colWoTitle";
            this.colWoTitle.ReadOnly = true;
            this.colWoPriority.DataPropertyName = "PriorityText";
            this.colWoPriority.FillWeight = 18F;
            this.colWoPriority.HeaderText = "Priority";
            this.colWoPriority.Name = "colWoPriority";
            this.colWoPriority.ReadOnly = true;
            this.colWoState.DataPropertyName = "State";
            this.colWoState.FillWeight = 20F;
            this.colWoState.HeaderText = "State";
            this.colWoState.Name = "colWoState";
            this.colWoState.ReadOnly = true;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStatus.Location = new System.Drawing.Point(20, 384);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(820, 24);
            this.lblStatus.Text = "Loading…";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblBanner
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
            // lblStatusBar
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
            // WorkOrdersPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlWorkArea);
            this.Name = "WorkOrdersPage";
            this.Size = new System.Drawing.Size(908, 600);
            this.Text = "Work Orders";
            this.Load += new System.EventHandler(this.WorkOrdersPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlWorkArea.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Button btnBackToDossier;
        private Wisej.Web.Panel pnlWorkArea;
        private Wisej.Web.Button btnOpen;
        private Wisej.Web.Button btnInProgress;
        private Wisej.Web.Button btnDone;
        private Wisej.Web.Button btnNewWorkOrder;
        private Wisej.Web.DataGridView dgvWorkOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colWoId;
        private Wisej.Web.DataGridViewTextBoxColumn colWoTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colWoPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colWoState;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatusBar;
    }
}
