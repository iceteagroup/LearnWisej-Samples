namespace TicketOps.Views
{
    partial class TicketWorkflow
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
            this.labelSignedIn = new Wisej.Web.Label();
            this.comboOperator = new Wisej.Web.ComboBox();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.columnId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnHours = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnAssignee = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelSelected = new Wisej.Web.Label();
            this.textReason = new Wisej.Web.TextBox();
            this.buttonClose = new Wisej.Web.Button();
            this.buttonAssign = new Wisej.Web.Button();
            this.buttonRefresh = new Wisej.Web.Button();
            this.panelScreen.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.labelSignedIn);
            this.panelScreen.Controls.Add(this.comboOperator);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.gridTickets);
            this.panelScreen.Controls.Add(this.labelSelected);
            this.panelScreen.Controls.Add(this.textReason);
            this.panelScreen.Controls.Add(this.buttonClose);
            this.panelScreen.Controls.Add(this.buttonAssign);
            this.panelScreen.Controls.Add(this.buttonRefresh);
            this.panelScreen.Location = new System.Drawing.Point(30, 30);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(760, 388);
            //
            // labelScreenTitle
            //
            this.labelScreenTitle.AutoSize = false;
            this.labelScreenTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelScreenTitle.Location = new System.Drawing.Point(24, 18);
            this.labelScreenTitle.Name = "labelScreenTitle";
            this.labelScreenTitle.Size = new System.Drawing.Size(300, 30);
            this.labelScreenTitle.Text = "Ticket Workflow";
            //
            // statusBanner
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // labelSignedIn
            //
            this.labelSignedIn.AutoSize = false;
            this.labelSignedIn.Font = new System.Drawing.Font("default", 9F);
            this.labelSignedIn.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelSignedIn.Location = new System.Drawing.Point(24, 60);
            this.labelSignedIn.Name = "labelSignedIn";
            this.labelSignedIn.Size = new System.Drawing.Size(84, 20);
            this.labelSignedIn.Text = "Signed in as";
            //
            // comboOperator  (added before the StatusBanner so the banner's rectangle never covers it)
            //
            this.comboOperator.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboOperator.Location = new System.Drawing.Point(108, 54);
            this.comboOperator.Name = "comboOperator";
            this.comboOperator.Size = new System.Drawing.Size(232, 30);
            this.comboOperator.SelectedIndexChanged += new System.EventHandler(this.comboOperator_SelectedIndexChanged);
            //
            // gridTickets
            //
            this.gridTickets.AllowUserToAddRows = false;
            this.gridTickets.AllowUserToDeleteRows = false;
            this.gridTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridTickets.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.columnId,
                this.columnTitle,
                this.columnPriority,
                this.columnStatus,
                this.columnHours,
                this.columnAssignee});
            this.gridTickets.Location = new System.Drawing.Point(24, 94);
            this.gridTickets.MultiSelect = false;
            this.gridTickets.Name = "gridTickets";
            this.gridTickets.ReadOnly = true;
            this.gridTickets.RowHeadersVisible = false;
            this.gridTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridTickets.Size = new System.Drawing.Size(712, 208);
            this.gridTickets.SelectionChanged += new System.EventHandler(this.gridTickets_SelectionChanged);
            //
            // columns
            //
            this.columnId.HeaderText = "Id";
            this.columnId.Name = "columnId";
            this.columnId.ReadOnly = true;
            this.columnId.Width = 60;
            this.columnTitle.HeaderText = "Title";
            this.columnTitle.Name = "columnTitle";
            this.columnTitle.ReadOnly = true;
            this.columnTitle.Width = 290;
            this.columnPriority.HeaderText = "Priority";
            this.columnPriority.Name = "columnPriority";
            this.columnPriority.ReadOnly = true;
            this.columnPriority.Width = 80;
            this.columnStatus.HeaderText = "Status";
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.ReadOnly = true;
            this.columnStatus.Width = 100;
            this.columnHours.HeaderText = "Hours";
            this.columnHours.Name = "columnHours";
            this.columnHours.ReadOnly = true;
            this.columnHours.Width = 60;
            this.columnAssignee.HeaderText = "Assignee";
            this.columnAssignee.Name = "columnAssignee";
            this.columnAssignee.ReadOnly = true;
            this.columnAssignee.Width = 120;
            //
            // labelSelected
            //
            this.labelSelected.AutoSize = false;
            this.labelSelected.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelSelected.Location = new System.Drawing.Point(24, 310);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(712, 20);
            this.labelSelected.Text = "Select a ticket";
            //
            // textReason
            //
            this.textReason.Location = new System.Drawing.Point(24, 334);
            this.textReason.Name = "textReason";
            this.textReason.Size = new System.Drawing.Size(300, 32);
            this.textReason.Watermark = "Close reason (required)";
            //
            // buttonClose
            //
            this.buttonClose.Location = new System.Drawing.Point(334, 334);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(130, 32);
            this.buttonClose.Text = "Close ticket";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            //
            // buttonAssign
            //
            this.buttonAssign.Location = new System.Drawing.Point(474, 334);
            this.buttonAssign.Name = "buttonAssign";
            this.buttonAssign.Size = new System.Drawing.Size(130, 32);
            this.buttonAssign.Text = "Assign to me";
            this.buttonAssign.Click += new System.EventHandler(this.buttonAssign_Click);
            //
            // buttonRefresh
            //
            this.buttonRefresh.Location = new System.Drawing.Point(614, 334);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(122, 32);
            this.buttonRefresh.Text = "↻ Refresh";
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            //
            // TicketWorkflow
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(820, 448);
            this.Controls.Add(this.panelScreen);
            this.Name = "TicketWorkflow";
            this.Text = "TicketOps Console";
            this.Load += new System.EventHandler(this.TicketWorkflow_Load);
            this.panelScreen.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label labelSignedIn;
        private Wisej.Web.ComboBox comboOperator;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn columnId;
        private Wisej.Web.DataGridViewTextBoxColumn columnTitle;
        private Wisej.Web.DataGridViewTextBoxColumn columnPriority;
        private Wisej.Web.DataGridViewTextBoxColumn columnStatus;
        private Wisej.Web.DataGridViewTextBoxColumn columnHours;
        private Wisej.Web.DataGridViewTextBoxColumn columnAssignee;
        private Wisej.Web.Label labelSelected;
        private Wisej.Web.TextBox textReason;
        private Wisej.Web.Button buttonClose;
        private Wisej.Web.Button buttonAssign;
        private Wisej.Web.Button buttonRefresh;
    }
}
