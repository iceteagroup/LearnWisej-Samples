namespace TicketOpsLive
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
            this.markerTimer = new Wisej.Web.Timer(this.components);
            this.panelBoard = new Wisej.Web.Panel();
            this.labelTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.escalatedOnlyCheckBox = new Wisej.Web.CheckBox();
            this.selectedLabel = new Wisej.Web.Label();
            this.ticketsGrid = new Wisej.Web.DataGridView();
            this.colMarker = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colUpdatedAt = new Wisej.Web.DataGridViewTextBoxColumn();
            this.ticketChangeLabel = new Wisej.Web.Label();
            this.dismissButton = new Wisej.Web.Button();
            this.labelBanner = new Wisej.Web.Label();
            this.labelState = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.newTicketsButton = new Wisej.Web.Button();
            this.changeStatusButton = new Wisej.Web.Button();
            this.stopButton = new Wisej.Web.Button();
            this.rebindButton = new Wisej.Web.Button();
            this.corruptButton = new Wisej.Web.Button();
            this.clearButton = new Wisej.Web.Button();
            this.panelBoard.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // markerTimer  (row cue: clears the "updated" marker 3 seconds after the change)
            //
            this.markerTimer.Interval = 1000;
            this.markerTimer.Tick += new System.EventHandler(this.markerTimer_Tick);
            //
            // panelBoard  (the live ticket board)
            //
            this.panelBoard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelBoard.BackColor = System.Drawing.Color.White;
            this.panelBoard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelBoard.Controls.Add(this.labelTitle);
            this.panelBoard.Controls.Add(this.labelStatus);
            this.panelBoard.Controls.Add(this.escalatedOnlyCheckBox);
            this.panelBoard.Controls.Add(this.selectedLabel);
            this.panelBoard.Controls.Add(this.ticketsGrid);
            this.panelBoard.Controls.Add(this.ticketChangeLabel);
            this.panelBoard.Controls.Add(this.dismissButton);
            this.panelBoard.Controls.Add(this.labelBanner);
            this.panelBoard.Controls.Add(this.labelState);
            this.panelBoard.Location = new System.Drawing.Point(30, 18);
            this.panelBoard.Name = "panelBoard";
            this.panelBoard.Size = new System.Drawing.Size(800, 580);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 16);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(430, 30);
            this.labelTitle.Text = "Live ticket board";
            //
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(460, 18);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(316, 26);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // escalatedOnlyCheckBox  (extension challenge: a filter that survives live updates)
            //
            this.escalatedOnlyCheckBox.Location = new System.Drawing.Point(24, 52);
            this.escalatedOnlyCheckBox.Name = "escalatedOnlyCheckBox";
            this.escalatedOnlyCheckBox.Size = new System.Drawing.Size(300, 24);
            this.escalatedOnlyCheckBox.Text = "Escalated only  (the feed keeps running)";
            this.escalatedOnlyCheckBox.ToolTipText = "Filters the bound list from the master list. New and updated tickets keep arriving while the filter is on.";
            this.escalatedOnlyCheckBox.CheckedChanged += new System.EventHandler(this.escalatedOnlyCheckBox_CheckedChanged);
            //
            // selectedLabel  (proves the selection survived the update)
            //
            this.selectedLabel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.selectedLabel.AutoSize = false;
            this.selectedLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.selectedLabel.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.selectedLabel.Location = new System.Drawing.Point(336, 52);
            this.selectedLabel.Name = "selectedLabel";
            this.selectedLabel.Size = new System.Drawing.Size(440, 24);
            this.selectedLabel.Text = "selected: none — click a row, then push updates";
            this.selectedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // ticketsGrid  (lab task 1: the live DataGridView)
            //
            this.ticketsGrid.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.ticketsGrid.AllowUserToAddRows = false;
            this.ticketsGrid.AllowUserToDeleteRows = false;
            this.ticketsGrid.AutoGenerateColumns = false;
            this.ticketsGrid.AutoSelectFirstRow = false;
            this.ticketsGrid.Columns.Add(this.colMarker);
            this.ticketsGrid.Columns.Add(this.colId);
            this.ticketsGrid.Columns.Add(this.colTitle);
            this.ticketsGrid.Columns.Add(this.colCustomer);
            this.ticketsGrid.Columns.Add(this.colOwner);
            this.ticketsGrid.Columns.Add(this.colStatus);
            this.ticketsGrid.Columns.Add(this.colUpdatedAt);
            this.ticketsGrid.DataSource = this.ticketsBindingSource;
            this.ticketsGrid.Location = new System.Drawing.Point(24, 84);
            this.ticketsGrid.MultiSelect = false;
            this.ticketsGrid.Name = "ticketsGrid";
            this.ticketsGrid.ReadOnly = true;
            this.ticketsGrid.RowHeadersVisible = false;
            this.ticketsGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.ticketsGrid.ShowFocusCell = false;
            this.ticketsGrid.Size = new System.Drawing.Size(752, 300);
            this.ticketsGrid.SelectionChanged += new System.EventHandler(this.ticketsGrid_SelectionChanged);
            //
            // colMarker  (the temporary "updated" row cue — bound to the derived Ticket.Marker)
            //
            this.colMarker.DataPropertyName = "Marker";
            this.colMarker.HeaderText = "●";
            this.colMarker.Name = "colMarker";
            this.colMarker.ReadOnly = true;
            this.colMarker.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            this.colMarker.Width = 36;
            this.colMarker.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
            this.colMarker.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            //
            // colId
            //
            this.colId.DataPropertyName = "Id";
            this.colId.HeaderText = "Id";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            this.colId.Width = 56;
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            this.colTitle.Width = 236;
            //
            // colCustomer
            //
            this.colCustomer.DataPropertyName = "Customer";
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.Name = "colCustomer";
            this.colCustomer.ReadOnly = true;
            this.colCustomer.Width = 148;
            //
            // colOwner
            //
            this.colOwner.DataPropertyName = "Owner";
            this.colOwner.HeaderText = "Owner";
            this.colOwner.Name = "colOwner";
            this.colOwner.ReadOnly = true;
            this.colOwner.Width = 106;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 88;
            //
            // colUpdatedAt  (lab task: show UpdatedAt in a readable format — the model keeps a real DateTime)
            //
            this.colUpdatedAt.DataPropertyName = "UpdatedAt";
            this.colUpdatedAt.HeaderText = "Updated";
            this.colUpdatedAt.Name = "colUpdatedAt";
            this.colUpdatedAt.ReadOnly = true;
            this.colUpdatedAt.Width = 82;
            this.colUpdatedAt.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colUpdatedAt.DefaultCellStyle.Format = "HH:mm:ss";
            //
            // ticketChangeLabel  (the conflict rule: warn, do not reload — hidden until it is needed)
            //
            this.ticketChangeLabel.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.ticketChangeLabel.AutoSize = false;
            this.ticketChangeLabel.BackColor = System.Drawing.Color.FromArgb(255, 244, 229);
            this.ticketChangeLabel.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.ticketChangeLabel.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.ticketChangeLabel.Location = new System.Drawing.Point(24, 394);
            this.ticketChangeLabel.Name = "ticketChangeLabel";
            this.ticketChangeLabel.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.ticketChangeLabel.Size = new System.Drawing.Size(636, 40);
            this.ticketChangeLabel.Text = "";
            this.ticketChangeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ticketChangeLabel.Visible = false;
            //
            // dismissButton
            //
            this.dismissButton.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.dismissButton.Location = new System.Drawing.Point(670, 396);
            this.dismissButton.Name = "dismissButton";
            this.dismissButton.Size = new System.Drawing.Size(106, 36);
            this.dismissButton.Text = "Dismiss";
            this.dismissButton.ToolTipText = "Closes the conflict warning. Nothing is reloaded — the user keeps the row they were reading.";
            this.dismissButton.Visible = false;
            this.dismissButton.Click += new System.EventHandler(this.dismissButton_Click);
            //
            // labelBanner
            //
            this.labelBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(24, 442);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(752, 40);
            this.labelBanner.Text = "";
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // labelState  (what the server owns right now)
            //
            this.labelState.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelState.AutoSize = false;
            this.labelState.Font = new System.Drawing.Font("monospace", 9F);
            this.labelState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelState.Location = new System.Drawing.Point(24, 488);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(752, 76);
            this.labelState.Text = "";
            this.labelState.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelTrace  (Server → Browser live push trace)
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Controls.Add(this.labelTraceFooter);
            this.panelTrace.Location = new System.Drawing.Point(846, 18);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(472, 580);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(432, 30);
            this.labelTraceTitle.Text = "Server → Browser  ·  live push trace";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(432, 478);
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 538);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(432, 26);
            this.labelTraceFooter.Text = "→ push = Application.Update from a task   ·   ← request = the browser asked   ·   • server = decision";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.newTicketsButton);
            this.panelActions.Controls.Add(this.changeStatusButton);
            this.panelActions.Controls.Add(this.stopButton);
            this.panelActions.Controls.Add(this.rebindButton);
            this.panelActions.Controls.Add(this.corruptButton);
            this.panelActions.Controls.Add(this.clearButton);
            this.panelActions.Location = new System.Drawing.Point(30, 614);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // newTicketsButton  (progress path — lab task 4)
            //
            this.newTicketsButton.Location = new System.Drawing.Point(0, 4);
            this.newTicketsButton.Name = "newTicketsButton";
            this.newTicketsButton.Size = new System.Drawing.Size(214, 36);
            this.newTicketsButton.Text = "New ticket every second ×20";
            this.newTicketsButton.ToolTipText = "Application.StartTask raises one Added event per second for 20 seconds; each one is applied inside Application.Update(this, …).";
            this.newTicketsButton.Click += new System.EventHandler(this.newTicketsButton_Click);
            //
            // changeStatusButton  (success path — lab task 5)
            //
            this.changeStatusButton.Location = new System.Drawing.Point(222, 4);
            this.changeStatusButton.Name = "changeStatusButton";
            this.changeStatusButton.Size = new System.Drawing.Size(164, 36);
            this.changeStatusButton.Text = "Randomize statuses";
            this.changeStatusButton.ToolTipText = "Changes three random tickets in place — no row is added, no row moves, the selection stays.";
            this.changeStatusButton.Click += new System.EventHandler(this.changeStatusButton_Click);
            //
            // stopButton  (cancellation path)
            //
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(394, 4);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(110, 36);
            this.stopButton.Text = "■ Stop feed";
            this.stopButton.ToolTipText = "Cooperative stop: the loop checks the flag before the next ticket and ends within one second.";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            //
            // rebindButton  (the anti-pattern of the lesson)
            //
            this.rebindButton.Location = new System.Drawing.Point(512, 4);
            this.rebindButton.Name = "rebindButton";
            this.rebindButton.Size = new System.Drawing.Size(238, 36);
            this.rebindButton.Text = "Rebind whole grid (anti-pattern)";
            this.rebindButton.ToolTipText = "DataSource = null, then reload: the same rows come back — and the selection is gone.";
            this.rebindButton.Click += new System.EventHandler(this.rebindButton_Click);
            //
            // corruptButton  (failure path)
            //
            this.corruptButton.Location = new System.Drawing.Point(758, 4);
            this.corruptButton.Name = "corruptButton";
            this.corruptButton.Size = new System.Drawing.Size(174, 36);
            this.corruptButton.Text = "Apply corrupt event";
            this.corruptButton.ToolTipText = "Raises an event with no Ticket (or with Id 0). ApplyTicketEvent rejects it and the grid stays intact.";
            this.corruptButton.Click += new System.EventHandler(this.corruptButton_Click);
            //
            // clearButton
            //
            this.clearButton.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.clearButton.Location = new System.Drawing.Point(1178, 4);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(110, 36);
            this.clearButton.Text = "Clear trace";
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelBoard);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 700);
            this.Text = "TicketOps Live — Live Ticket Board";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelBoard.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.BindingSource ticketsBindingSource;
        private Wisej.Web.Timer markerTimer;
        private Wisej.Web.Panel panelBoard;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.CheckBox escalatedOnlyCheckBox;
        private Wisej.Web.Label selectedLabel;
        private Wisej.Web.DataGridView ticketsGrid;
        private Wisej.Web.DataGridViewTextBoxColumn colMarker;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colOwner;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colUpdatedAt;
        private Wisej.Web.Label ticketChangeLabel;
        private Wisej.Web.Button dismissButton;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button newTicketsButton;
        private Wisej.Web.Button changeStatusButton;
        private Wisej.Web.Button stopButton;
        private Wisej.Web.Button rebindButton;
        private Wisej.Web.Button corruptButton;
        private Wisej.Web.Button clearButton;
    }
}
