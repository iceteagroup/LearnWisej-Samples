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
            this.components = new System.ComponentModel.Container();
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
            this.progressTests = new Wisej.Web.ProgressBar();
            this.labelTests = new Wisej.Web.Label();
            this.tracePanel = new TicketOps.Diagnostics.ActivityTracePanel();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonTests = new Wisej.Web.Button();
            this.buttonRejectNoComments = new Wisej.Web.Button();
            this.buttonDecideAgain = new Wisej.Web.Button();
            this.buttonOutage = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerTests = new Wisej.Web.Timer(this.components);
            this.panelScreen.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen  (Work Orders: queue + selected order — display only; the decision happens in the dialog)
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
            this.panelScreen.Controls.Add(this.progressTests);
            this.panelScreen.Controls.Add(this.labelTests);
            this.panelScreen.Location = new System.Drawing.Point(30, 30);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(760, 560);
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
            // statusBanner  (Controls/StatusBanner: "● state" + banner line)
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
            // selected work order (display only — the dialog reads it, the service changes it)
            //
            this.labelSelectedCaption.AutoSize = false;
            this.labelSelectedCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelSelectedCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelSelectedCaption.Location = new System.Drawing.Point(24, 300);
            this.labelSelectedCaption.Name = "labelSelectedCaption";
            this.labelSelectedCaption.Size = new System.Drawing.Size(300, 18);
            this.labelSelectedCaption.Text = "SELECTED WORK ORDER";
            this.labelSelected.AutoSize = false;
            this.labelSelected.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelSelected.Location = new System.Drawing.Point(24, 320);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(712, 26);
            this.labelSelected.Text = "No work order selected";
            this.labelSelectedDetail.AutoSize = false;
            this.labelSelectedDetail.Font = new System.Drawing.Font("default", 9F);
            this.labelSelectedDetail.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelSelectedDetail.Location = new System.Drawing.Point(24, 348);
            this.labelSelectedDetail.Name = "labelSelectedDetail";
            this.labelSelectedDetail.Size = new System.Drawing.Size(712, 20);
            this.labelSelectedDetail.Text = "";
            this.labelDecision.AutoSize = false;
            this.labelDecision.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelDecision.ForeColor = System.Drawing.Color.FromArgb(185, 119, 14);
            this.labelDecision.Location = new System.Drawing.Point(24, 370);
            this.labelDecision.Name = "labelDecision";
            this.labelDecision.Size = new System.Drawing.Size(712, 20);
            this.labelDecision.Text = "";
            //
            // screen buttons (the modal workflow lives behind "Approve / Reject…")
            //
            this.buttonReview.Location = new System.Drawing.Point(24, 402);
            this.buttonReview.Name = "buttonReview";
            this.buttonReview.Size = new System.Drawing.Size(190, 40);
            this.buttonReview.Text = "✓ Approve / Reject…";
            this.buttonReview.ToolTipText = "Success path: opens the modal ApprovalDialog (await ShowDialogAsync); the service is called only after Confirm. Cancel, ✕ and a rejection without comments are the other exits.";
            this.buttonReview.Click += new System.EventHandler(this.buttonReview_Click);
            this.buttonRefresh.Location = new System.Drawing.Point(224, 402);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(120, 40);
            this.buttonRefresh.Text = "↻ Refresh";
            this.buttonRefresh.ToolTipText = "Reloads the queue through IApprovalService.GetQueueAsync()";
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            //
            // labelWorkflow  (the workflow status strip the video shows at the bottom of the screen)
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
            this.labelWorkflow.Text = "Select a work order, then Approve / Reject…";
            this.labelWorkflow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // progressTests + labelTests  (progress path)
            //
            this.progressTests.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.progressTests.Location = new System.Drawing.Point(24, 500);
            this.progressTests.Maximum = 6;
            this.progressTests.Name = "progressTests";
            this.progressTests.Size = new System.Drawing.Size(712, 14);
            this.progressTests.Visible = false;
            this.labelTests.AutoSize = false;
            this.labelTests.Font = new System.Drawing.Font("monospace", 9F);
            this.labelTests.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelTests.Location = new System.Drawing.Point(24, 520);
            this.labelTests.Name = "labelTests";
            this.labelTests.Size = new System.Drawing.Size(712, 22);
            this.labelTests.Text = "";
            //
            // tracePanel  (Diagnostics: the live activity trace)
            //
            this.tracePanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.tracePanel.Location = new System.Drawing.Point(810, 30);
            this.tracePanel.Name = "tracePanel";
            this.tracePanel.Size = new System.Drawing.Size(508, 560);
            //
            // panelActions  (bottom bar: progress / failures / outage + recovery / clear)
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonTests);
            this.panelActions.Controls.Add(this.buttonRejectNoComments);
            this.panelActions.Controls.Add(this.buttonDecideAgain);
            this.panelActions.Controls.Add(this.buttonOutage);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 606);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // bottom bar buttons
            //
            this.buttonTests.Location = new System.Drawing.Point(0, 4);
            this.buttonTests.Name = "buttonTests";
            this.buttonTests.Size = new System.Drawing.Size(240, 36);
            this.buttonTests.Text = "▶ Run result-handling tests";
            this.buttonTests.ToolTipText = "Progress path: six unit-style cases (approve, reject with comments, reject without comments refused, cancel = unchanged, already decided, outage → rollback → recovery), one per tick, each on a fresh fixture";
            this.buttonTests.Click += new System.EventHandler(this.buttonTests_Click);
            this.buttonRejectNoComments.Location = new System.Drawing.Point(250, 4);
            this.buttonRejectNoComments.Name = "buttonRejectNoComments";
            this.buttonRejectNoComments.Size = new System.Drawing.Size(210, 36);
            this.buttonRejectNoComments.Text = "Reject without comments";
            this.buttonRejectNoComments.ToolTipText = "Failure path 1: a forged confirmed rejection with no comments bypasses the dialog — the service re-checks the rule and refuses; nothing is written";
            this.buttonRejectNoComments.Click += new System.EventHandler(this.buttonRejectNoComments_Click);
            this.buttonDecideAgain.Location = new System.Drawing.Point(470, 4);
            this.buttonDecideAgain.Name = "buttonDecideAgain";
            this.buttonDecideAgain.Size = new System.Drawing.Size(200, 36);
            this.buttonDecideAgain.Text = "Decide WO-2001 again";
            this.buttonDecideAgain.ToolTipText = "Failure path 2: WO-2001 is already approved — WorkOrder.CanDecide (domain rule) says no; the handler never knew the rule";
            this.buttonDecideAgain.Click += new System.EventHandler(this.buttonDecideAgain_Click);
            this.buttonOutage.Location = new System.Drawing.Point(680, 4);
            this.buttonOutage.Name = "buttonOutage";
            this.buttonOutage.Size = new System.Drawing.Size(200, 36);
            this.buttonOutage.Text = "Simulate data outage";
            this.buttonOutage.ToolTipText = "Error path: the next confirmed decision fails at the audit INSERT inside the commit and rolls back — nothing partially applied. Click again to recover, then confirm again.";
            this.buttonOutage.Click += new System.EventHandler(this.buttonOutage_Click);
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(1148, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(140, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // timerTests
            //
            this.timerTests.Interval = 450;
            this.timerTests.Tick += new System.EventHandler(this.timerTests_Tick);
            //
            // WorkOrderQueue
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelScreen);
            this.Controls.Add(this.tracePanel);
            this.Controls.Add(this.panelActions);
            this.Name = "WorkOrderQueue";
            this.Text = "TicketOps Console — Module 6 · Work Orders";
            this.Load += new System.EventHandler(this.WorkOrderQueue_Load);
            this.panelScreen.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
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
        private Wisej.Web.ProgressBar progressTests;
        private Wisej.Web.Label labelTests;
        private TicketOps.Diagnostics.ActivityTracePanel tracePanel;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonTests;
        private Wisej.Web.Button buttonRejectNoComments;
        private Wisej.Web.Button buttonDecideAgain;
        private Wisej.Web.Button buttonOutage;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerTests;
    }
}
