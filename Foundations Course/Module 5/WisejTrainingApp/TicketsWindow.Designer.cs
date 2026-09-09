namespace WisejTrainingApp
{
    partial class TicketsWindow
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
            this.ticketsBindingSource = new Wisej.Web.BindingSource(this.components);
            this.panelTickets = new Wisej.Web.Panel();
            this.dgvTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssignedTo = new Wisej.Web.DataGridViewTextBoxColumn();
            this.panelHeader = new Wisej.Web.Panel();
            this.lblPageTitle = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.panelCommands = new Wisej.Web.Panel();
            this.btnNew = new Wisej.Web.Button();
            this.btnEdit = new Wisej.Web.Button();
            this.btnDelete = new Wisej.Web.Button();
            this.btnRefresh = new Wisej.Web.Button();
            this.lblSelection = new Wisej.Web.Label();
            this.panelLog = new Wisej.Web.Panel();
            this.labelWorkflowCard = new Wisej.Web.Label();
            this.lblWorkflow = new Wisej.Web.Label();
            this.labelLogCard = new Wisej.Web.Label();
            this.lstEventLog = new Wisej.Web.ListBox();
            this.labelLogFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.btnClearSelection = new Wisej.Web.Button();
            this.btnResetData = new Wisej.Web.Button();
            this.btnClearLog = new Wisej.Web.Button();
            this.panelTickets.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelCommands.SuspendLayout();
            this.panelLog.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelTickets  (the main card — the Module 4 ticket screen, now with dialogs)
            //
            this.panelTickets.BackColor = System.Drawing.Color.White;
            this.panelTickets.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTickets.Controls.Add(this.dgvTickets);
            this.panelTickets.Controls.Add(this.panelCommands);
            this.panelTickets.Controls.Add(this.panelHeader);
            this.panelTickets.Location = new System.Drawing.Point(30, 30);
            this.panelTickets.Name = "panelTickets";
            this.panelTickets.Padding = new Wisej.Web.Padding(20, 12, 20, 14);
            this.panelTickets.Size = new System.Drawing.Size(880, 580);
            //
            // panelHeader  (header row: page title on the left, status on the right)
            //
            this.panelHeader.Controls.Add(this.lblPageTitle);
            this.panelHeader.Controls.Add(this.lblStatus);
            this.panelHeader.Dock = Wisej.Web.DockStyle.Top;
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(838, 56);
            //
            // lblPageTitle
            //
            this.lblPageTitle.AutoSize = false;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(0, 6);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(300, 36);
            this.lblPageTitle.Text = "Tickets";
            this.lblPageTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(308, 12);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(530, 26);
            this.lblStatus.Text = "● ready — 6 tickets loaded";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dgvTickets  (Dock Fill between the header and the command row)
            //
            this.dgvTickets.AllowUserToAddRows = false;
            this.dgvTickets.AllowUserToDeleteRows = false;
            this.dgvTickets.AutoGenerateColumns = false;
            this.dgvTickets.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTickets.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colId,
            this.colTitle,
            this.colCustomer,
            this.colStatus,
            this.colPriority,
            this.colAssignedTo});
            this.dgvTickets.DataSource = this.ticketsBindingSource;
            this.dgvTickets.Dock = Wisej.Web.DockStyle.Fill;
            this.dgvTickets.MultiSelect = false;
            this.dgvTickets.Name = "dgvTickets";
            this.dgvTickets.ReadOnly = true;
            this.dgvTickets.RowHeadersVisible = false;
            this.dgvTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTickets.ToolTipText = "Double-click a row to edit it (same handler as the Edit Ticket button).";
            this.dgvTickets.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.dgvTickets_CellDoubleClick);
            this.dgvTickets.SelectionChanged += new System.EventHandler(this.dgvTickets_SelectionChanged);
            //
            // colId
            //
            this.colId.DataPropertyName = "Id";
            this.colId.FillWeight = 8F;
            this.colId.HeaderText = "Id";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.FillWeight = 34F;
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            //
            // colCustomer
            //
            this.colCustomer.DataPropertyName = "Customer";
            this.colCustomer.FillWeight = 20F;
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
            this.colAssignedTo.FillWeight = 14F;
            this.colAssignedTo.HeaderText = "Assigned To";
            this.colAssignedTo.Name = "colAssignedTo";
            this.colAssignedTo.ReadOnly = true;
            //
            // panelCommands  (command row under the grid)
            //
            this.panelCommands.Controls.Add(this.btnNew);
            this.panelCommands.Controls.Add(this.btnEdit);
            this.panelCommands.Controls.Add(this.btnDelete);
            this.panelCommands.Controls.Add(this.btnRefresh);
            this.panelCommands.Controls.Add(this.lblSelection);
            this.panelCommands.Dock = Wisej.Web.DockStyle.Bottom;
            this.panelCommands.Name = "panelCommands";
            this.panelCommands.Size = new System.Drawing.Size(838, 52);
            //
            // btnNew  (step 1 of the workflow: Button click → New)
            //
            this.btnNew.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnNew.Location = new System.Drawing.Point(0, 14);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(130, 36);
            this.btnNew.Text = "New Ticket";
            this.btnNew.ToolTipText = "btnNew_Click: new TicketDialog() → await ShowDialogAsync() → OK ? AddTicket + refresh : nothing";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            //
            // btnEdit  (step 1 of the workflow: Button click → Edit)
            //
            this.btnEdit.Location = new System.Drawing.Point(138, 14);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(130, 36);
            this.btnEdit.Text = "Edit Ticket";
            this.btnEdit.ToolTipText = "btnEdit_Click: guard (a row must be selected) → new TicketDialog(selected) → OK ? UpdateTicket + refresh";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            //
            // btnDelete  (confirmation through MessageBox.ShowAsync)
            //
            this.btnDelete.Location = new System.Drawing.Point(276, 14);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(130, 36);
            this.btnDelete.Text = "Delete Ticket";
            this.btnDelete.ToolTipText = "btnDelete_Click: guard → await MessageBox.ShowAsync(YesNo) → Yes ? DeleteTicket + refresh : log 'cancelled'";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(414, 14);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(110, 36);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.ToolTipText = "RefreshTicketGrid(): re-read TicketService.GetTickets() and ResetBindings(false)";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // lblSelection
            //
            this.lblSelection.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblSelection.AutoEllipsis = true;
            this.lblSelection.AutoSize = false;
            this.lblSelection.Font = new System.Drawing.Font("monospace", 9F);
            this.lblSelection.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSelection.Location = new System.Drawing.Point(534, 14);
            this.lblSelection.Name = "lblSelection";
            this.lblSelection.Size = new System.Drawing.Size(304, 36);
            this.lblSelection.Text = "selected: none";
            this.lblSelection.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // panelLog  (right-hand card: the workflow steps + the event log)
            //
            this.panelLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelLog.BackColor = System.Drawing.Color.White;
            this.panelLog.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelLog.Controls.Add(this.labelWorkflowCard);
            this.panelLog.Controls.Add(this.lblWorkflow);
            this.panelLog.Controls.Add(this.labelLogCard);
            this.panelLog.Controls.Add(this.lstEventLog);
            this.panelLog.Controls.Add(this.labelLogFooter);
            this.panelLog.Location = new System.Drawing.Point(938, 30);
            this.panelLog.Name = "panelLog";
            this.panelLog.Size = new System.Drawing.Size(380, 580);
            //
            // labelWorkflowCard
            //
            this.labelWorkflowCard.AutoSize = false;
            this.labelWorkflowCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelWorkflowCard.Location = new System.Drawing.Point(20, 14);
            this.labelWorkflowCard.Name = "labelWorkflowCard";
            this.labelWorkflowCard.Size = new System.Drawing.Size(340, 30);
            this.labelWorkflowCard.Text = "Workflow  ·  the five-step modal pattern";
            //
            // lblWorkflow  (monospace; ▶ marks the step that is running right now)
            //
            this.lblWorkflow.AutoSize = false;
            this.lblWorkflow.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblWorkflow.Font = new System.Drawing.Font("monospace", 9F);
            this.lblWorkflow.Location = new System.Drawing.Point(20, 48);
            this.lblWorkflow.Name = "lblWorkflow";
            this.lblWorkflow.Padding = new Wisej.Web.Padding(10, 8, 10, 8);
            this.lblWorkflow.Size = new System.Drawing.Size(340, 118);
            this.lblWorkflow.Text = "";
            this.lblWorkflow.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelLogCard
            //
            this.labelLogCard.AutoSize = false;
            this.labelLogCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelLogCard.Location = new System.Drawing.Point(20, 180);
            this.labelLogCard.Name = "labelLogCard";
            this.labelLogCard.Size = new System.Drawing.Size(340, 30);
            this.labelLogCard.Text = "Event log  ·  what the server-side code did";
            //
            // lstEventLog
            //
            this.lstEventLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstEventLog.Font = new System.Drawing.Font("monospace", 9F);
            this.lstEventLog.Location = new System.Drawing.Point(20, 214);
            this.lstEventLog.Name = "lstEventLog";
            this.lstEventLog.Size = new System.Drawing.Size(340, 314);
            //
            // labelLogFooter
            //
            this.labelLogFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLogFooter.AutoSize = false;
            this.labelLogFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLogFooter.Location = new System.Drawing.Point(20, 538);
            this.labelLogFooter.Name = "labelLogFooter";
            this.labelLogFooter.Size = new System.Drawing.Size(340, 26);
            this.labelLogFooter.Text = "click → dialog → ValidateForm → service → RefreshTicketGrid";
            //
            // panelActions  (bottom bar: helpers for the failure paths + Clear log)
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.btnClearSelection);
            this.panelActions.Controls.Add(this.btnResetData);
            this.panelActions.Controls.Add(this.btnClearLog);
            this.panelActions.Location = new System.Drawing.Point(30, 626);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // btnClearSelection  (sets up the Edit / Delete guard: no row selected)
            //
            this.btnClearSelection.Location = new System.Drawing.Point(0, 4);
            this.btnClearSelection.Name = "btnClearSelection";
            this.btnClearSelection.Size = new System.Drawing.Size(260, 36);
            this.btnClearSelection.Text = "Clear selection (then try Edit → guard)";
            this.btnClearSelection.ToolTipText = "dgvTickets.ClearSelection() — Edit Ticket and Delete Ticket now stop at their guard.";
            this.btnClearSelection.Click += new System.EventHandler(this.btnClearSelection_Click);
            //
            // btnResetData  (recovery: a fresh TicketService with the 6 seeded tickets)
            //
            this.btnResetData.Location = new System.Drawing.Point(268, 4);
            this.btnResetData.Name = "btnResetData";
            this.btnResetData.Size = new System.Drawing.Size(200, 36);
            this.btnResetData.Text = "Reset sample data";
            this.btnResetData.ToolTipText = "new TicketService() — back to the six seeded tickets.";
            this.btnResetData.Click += new System.EventHandler(this.btnResetData_Click);
            //
            // btnClearLog
            //
            this.btnClearLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearLog.Location = new System.Drawing.Point(1178, 4);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(110, 36);
            this.btnClearLog.Text = "Clear log";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            // TicketsWindow
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelTickets);
            this.Controls.Add(this.panelLog);
            this.Controls.Add(this.panelActions);
            this.Name = "TicketsWindow";
            this.Text = "WisejTrainingApp — Tickets with dialogs (Module 5)";
            this.Load += new System.EventHandler(this.TicketsWindow_Load);
            this.panelTickets.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelCommands.ResumeLayout(false);
            this.panelLog.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.BindingSource ticketsBindingSource;
        private Wisej.Web.Panel panelTickets;
        private Wisej.Web.DataGridView dgvTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colAssignedTo;
        private Wisej.Web.Panel panelHeader;
        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel panelCommands;
        private Wisej.Web.Button btnNew;
        private Wisej.Web.Button btnEdit;
        private Wisej.Web.Button btnDelete;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.Label lblSelection;
        private Wisej.Web.Panel panelLog;
        private Wisej.Web.Label labelWorkflowCard;
        private Wisej.Web.Label lblWorkflow;
        private Wisej.Web.Label labelLogCard;
        private Wisej.Web.ListBox lstEventLog;
        private Wisej.Web.Label labelLogFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button btnClearSelection;
        private Wisej.Web.Button btnResetData;
        private Wisej.Web.Button btnClearLog;
    }
}
