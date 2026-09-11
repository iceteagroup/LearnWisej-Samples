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
            this.escalatedOnlyCheckBox = new Wisej.Web.CheckBox();
            this.ticketsGrid = new Wisej.Web.DataGridView();
            this.colMarker = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colUpdatedAt = new Wisej.Web.DataGridViewTextBoxColumn();
            this.newTicketsButton = new Wisej.Web.Button();
            this.changeStatusButton = new Wisej.Web.Button();
            this.stopButton = new Wisej.Web.Button();
            this.ticketChangeLabel = new Wisej.Web.Label();
            this.dismissButton = new Wisej.Web.Button();
            this.panelBoard.SuspendLayout();
            this.SuspendLayout();
            //
            // markerTimer
            //
            this.markerTimer.Interval = 1000;
            this.markerTimer.Tick += new System.EventHandler(this.markerTimer_Tick);
            //
            // panelBoard
            //
            this.panelBoard.BackColor = System.Drawing.Color.White;
            this.panelBoard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelBoard.Controls.Add(this.labelTitle);
            this.panelBoard.Controls.Add(this.escalatedOnlyCheckBox);
            this.panelBoard.Controls.Add(this.ticketsGrid);
            this.panelBoard.Controls.Add(this.newTicketsButton);
            this.panelBoard.Controls.Add(this.changeStatusButton);
            this.panelBoard.Controls.Add(this.stopButton);
            this.panelBoard.Controls.Add(this.ticketChangeLabel);
            this.panelBoard.Controls.Add(this.dismissButton);
            this.panelBoard.Location = new System.Drawing.Point(20, 18);
            this.panelBoard.Name = "panelBoard";
            this.panelBoard.Size = new System.Drawing.Size(800, 470);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 16);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(400, 30);
            this.labelTitle.Text = "Live Ticket Board";
            //
            // escalatedOnlyCheckBox
            //
            this.escalatedOnlyCheckBox.Location = new System.Drawing.Point(620, 20);
            this.escalatedOnlyCheckBox.Name = "escalatedOnlyCheckBox";
            this.escalatedOnlyCheckBox.Size = new System.Drawing.Size(156, 24);
            this.escalatedOnlyCheckBox.Text = "Escalated only";
            this.escalatedOnlyCheckBox.CheckedChanged += new System.EventHandler(this.escalatedOnlyCheckBox_CheckedChanged);
            //
            // ticketsGrid
            //
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
            this.ticketsGrid.Location = new System.Drawing.Point(24, 56);
            this.ticketsGrid.MultiSelect = false;
            this.ticketsGrid.Name = "ticketsGrid";
            this.ticketsGrid.ReadOnly = true;
            this.ticketsGrid.RowHeadersVisible = false;
            this.ticketsGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.ticketsGrid.ShowFocusCell = false;
            this.ticketsGrid.Size = new System.Drawing.Size(752, 300);
            this.ticketsGrid.SelectionChanged += new System.EventHandler(this.ticketsGrid_SelectionChanged);
            //
            // colMarker
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
            // colUpdatedAt
            //
            this.colUpdatedAt.DataPropertyName = "UpdatedAt";
            this.colUpdatedAt.HeaderText = "Updated";
            this.colUpdatedAt.Name = "colUpdatedAt";
            this.colUpdatedAt.ReadOnly = true;
            this.colUpdatedAt.Width = 82;
            this.colUpdatedAt.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colUpdatedAt.DefaultCellStyle.Format = "HH:mm:ss";
            //
            // newTicketsButton
            //
            this.newTicketsButton.Location = new System.Drawing.Point(24, 368);
            this.newTicketsButton.Name = "newTicketsButton";
            this.newTicketsButton.Size = new System.Drawing.Size(230, 36);
            this.newTicketsButton.Text = "New ticket every second (20 s)";
            this.newTicketsButton.Click += new System.EventHandler(this.newTicketsButton_Click);
            //
            // changeStatusButton
            //
            this.changeStatusButton.Location = new System.Drawing.Point(262, 368);
            this.changeStatusButton.Name = "changeStatusButton";
            this.changeStatusButton.Size = new System.Drawing.Size(170, 36);
            this.changeStatusButton.Text = "Randomize statuses";
            this.changeStatusButton.Click += new System.EventHandler(this.changeStatusButton_Click);
            //
            // stopButton
            //
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(440, 368);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(110, 36);
            this.stopButton.Text = "Stop feed";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            //
            // ticketChangeLabel
            //
            this.ticketChangeLabel.AutoSize = false;
            this.ticketChangeLabel.BackColor = System.Drawing.Color.FromArgb(255, 244, 229);
            this.ticketChangeLabel.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.ticketChangeLabel.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.ticketChangeLabel.Location = new System.Drawing.Point(24, 414);
            this.ticketChangeLabel.Name = "ticketChangeLabel";
            this.ticketChangeLabel.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.ticketChangeLabel.Size = new System.Drawing.Size(636, 40);
            this.ticketChangeLabel.Text = "";
            this.ticketChangeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ticketChangeLabel.Visible = false;
            //
            // dismissButton
            //
            this.dismissButton.Location = new System.Drawing.Point(670, 416);
            this.dismissButton.Name = "dismissButton";
            this.dismissButton.Size = new System.Drawing.Size(106, 36);
            this.dismissButton.Text = "Dismiss";
            this.dismissButton.Visible = false;
            this.dismissButton.Click += new System.EventHandler(this.dismissButton_Click);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelBoard);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(840, 508);
            this.Text = "TicketOps Live — Live Ticket Board";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelBoard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.BindingSource ticketsBindingSource;
        private Wisej.Web.Timer markerTimer;
        private Wisej.Web.Panel panelBoard;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.CheckBox escalatedOnlyCheckBox;
        private Wisej.Web.DataGridView ticketsGrid;
        private Wisej.Web.DataGridViewTextBoxColumn colMarker;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colOwner;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colUpdatedAt;
        private Wisej.Web.Button newTicketsButton;
        private Wisej.Web.Button changeStatusButton;
        private Wisej.Web.Button stopButton;
        private Wisej.Web.Label ticketChangeLabel;
        private Wisej.Web.Button dismissButton;
    }
}
