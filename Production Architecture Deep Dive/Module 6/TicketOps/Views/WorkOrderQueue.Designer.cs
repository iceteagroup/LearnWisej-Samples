namespace TicketOps.Views
{
    partial class WorkOrderQueue
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
            this.panelScreen = new Wisej.Web.Panel();
            this.labelScreenTitle = new Wisej.Web.Label();
            this.statusBanner = new TicketOps.Controls.StatusBanner();
            this.labelCount = new Wisej.Web.Label();
            this.gridWorkOrders = new Wisej.Web.DataGridView();
            this.columnNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnRequester = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnAmount = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelSelectedCaption = new Wisej.Web.Label();
            this.labelSelected = new Wisej.Web.Label();
            this.labelSelectedDetail = new Wisej.Web.Label();
            this.labelDecision = new Wisej.Web.Label();
            this.buttonReview = new Wisej.Web.Button();
            this.buttonRefresh = new Wisej.Web.Button();
            this.labelWorkflow = new Wisej.Web.Label();
            this.panelScreen.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.labelCount);
            this.panelScreen.Controls.Add(this.gridWorkOrders);
            this.panelScreen.Controls.Add(this.labelSelectedCaption);
            this.panelScreen.Controls.Add(this.labelSelected);
            this.panelScreen.Controls.Add(this.labelSelectedDetail);
            this.panelScreen.Controls.Add(this.labelDecision);
            this.panelScreen.Controls.Add(this.buttonReview);
            this.panelScreen.Controls.Add(this.buttonRefresh);
            this.panelScreen.Controls.Add(this.labelWorkflow);
            this.panelScreen.Location = new System.Drawing.Point(30, 30);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(760, 512);
            //
            // labelScreenTitle
            //
            this.labelScreenTitle.AutoSize = false;
            this.labelScreenTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelScreenTitle.Location = new System.Drawing.Point(24, 18);
            this.labelScreenTitle.Name = "labelScreenTitle";
            this.labelScreenTitle.Size = new System.Drawing.Size(300, 30);
            this.labelScreenTitle.Text = "Work Orders";
            //
            // statusBanner
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // labelCount
            //
            this.labelCount.AutoSize = false;
            this.labelCount.Font = new System.Drawing.Font("default", 9F);
            this.labelCount.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelCount.Location = new System.Drawing.Point(24, 48);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(300, 20);
            this.labelCount.Text = "loading…";
            //
            // gridWorkOrders
            //
            this.gridWorkOrders.AllowUserToAddRows = false;
            this.gridWorkOrders.AllowUserToDeleteRows = false;
            this.gridWorkOrders.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridWorkOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.columnNumber,
                this.columnTitle,
                this.columnRequester,
                this.columnAmount,
                this.columnStatus});
            this.gridWorkOrders.Location = new System.Drawing.Point(24, 92);
            this.gridWorkOrders.MultiSelect = false;
            this.gridWorkOrders.Name = "gridWorkOrders";
            this.gridWorkOrders.ReadOnly = true;
            this.gridWorkOrders.RowHeadersVisible = false;
            this.gridWorkOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridWorkOrders.Size = new System.Drawing.Size(712, 196);
            this.gridWorkOrders.SelectionChanged += new System.EventHandler(this.gridWorkOrders_SelectionChanged);
            //
            // columns
            //
            this.columnNumber.HeaderText = "Number";
            this.columnNumber.Name = "columnNumber";
            this.columnNumber.ReadOnly = true;
            this.columnNumber.Width = 90;
            this.columnTitle.HeaderText = "Title";
            this.columnTitle.Name = "columnTitle";
            this.columnTitle.ReadOnly = true;
            this.columnTitle.Width = 280;
            this.columnRequester.HeaderText = "Requester";
            this.columnRequester.Name = "columnRequester";
            this.columnRequester.ReadOnly = true;
            this.columnRequester.Width = 120;
            this.columnAmount.HeaderText = "Cost";
            this.columnAmount.Name = "columnAmount";
            this.columnAmount.ReadOnly = true;
            this.columnAmount.Width = 100;
            this.columnStatus.HeaderText = "Status";
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.ReadOnly = true;
            this.columnStatus.Width = 100;
            //
            // labelSelectedCaption
            //
            this.labelSelectedCaption.AutoSize = false;
            this.labelSelectedCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelSelectedCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelSelectedCaption.Location = new System.Drawing.Point(24, 300);
            this.labelSelectedCaption.Name = "labelSelectedCaption";
            this.labelSelectedCaption.Size = new System.Drawing.Size(300, 18);
            this.labelSelectedCaption.Text = "SELECTED WORK ORDER";
            //
            // labelSelected
            //
            this.labelSelected.AutoSize = false;
            this.labelSelected.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelSelected.Location = new System.Drawing.Point(24, 320);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(712, 26);
            this.labelSelected.Text = "No work order selected";
            //
            // labelSelectedDetail
            //
            this.labelSelectedDetail.AutoSize = false;
            this.labelSelectedDetail.Font = new System.Drawing.Font("default", 9F);
            this.labelSelectedDetail.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelSelectedDetail.Location = new System.Drawing.Point(24, 348);
            this.labelSelectedDetail.Name = "labelSelectedDetail";
            this.labelSelectedDetail.Size = new System.Drawing.Size(712, 20);
            this.labelSelectedDetail.Text = "";
            //
            // labelDecision
            //
            this.labelDecision.AutoSize = false;
            this.labelDecision.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelDecision.ForeColor = System.Drawing.Color.FromArgb(185, 119, 14);
            this.labelDecision.Location = new System.Drawing.Point(24, 370);
            this.labelDecision.Name = "labelDecision";
            this.labelDecision.Size = new System.Drawing.Size(712, 20);
            this.labelDecision.Text = "";
            //
            // buttonReview
            //
            this.buttonReview.Location = new System.Drawing.Point(24, 402);
            this.buttonReview.Name = "buttonReview";
            this.buttonReview.Size = new System.Drawing.Size(190, 40);
            this.buttonReview.Text = "✓ Approve / Reject…";
            this.buttonReview.Click += new System.EventHandler(this.buttonReview_Click);
            //
            // buttonRefresh
            //
            this.buttonRefresh.Location = new System.Drawing.Point(224, 402);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(120, 40);
            this.buttonRefresh.Text = "↻ Refresh";
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            //
            // labelWorkflow  (the workflow status strip)
            //
            this.labelWorkflow.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelWorkflow.AutoSize = false;
            this.labelWorkflow.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.labelWorkflow.Font = new System.Drawing.Font("monospace", 9F);
            this.labelWorkflow.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.labelWorkflow.Location = new System.Drawing.Point(24, 458);
            this.labelWorkflow.Name = "labelWorkflow";
            this.labelWorkflow.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.labelWorkflow.Size = new System.Drawing.Size(712, 30);
            this.labelWorkflow.Text = "";
            this.labelWorkflow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // WorkOrderQueue
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(820, 572);
            this.Controls.Add(this.panelScreen);
            this.Name = "WorkOrderQueue";
            this.Text = "TicketOps Console";
            this.Load += new System.EventHandler(this.WorkOrderQueue_Load);
            this.panelScreen.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label labelCount;
        private Wisej.Web.DataGridView gridWorkOrders;
        private Wisej.Web.DataGridViewTextBoxColumn columnNumber;
        private Wisej.Web.DataGridViewTextBoxColumn columnTitle;
        private Wisej.Web.DataGridViewTextBoxColumn columnRequester;
        private Wisej.Web.DataGridViewTextBoxColumn columnAmount;
        private Wisej.Web.DataGridViewTextBoxColumn columnStatus;
        private Wisej.Web.Label labelSelectedCaption;
        private Wisej.Web.Label labelSelected;
        private Wisej.Web.Label labelSelectedDetail;
        private Wisej.Web.Label labelDecision;
        private Wisej.Web.Button buttonReview;
        private Wisej.Web.Button buttonRefresh;
        private Wisej.Web.Label labelWorkflow;
    }
}
