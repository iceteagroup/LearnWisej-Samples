namespace WisejTrainingApp.Views
{
    partial class TicketsView
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
            this.lblViewTitle = new Wisej.Web.Label();
            this.lblTicketsSummary = new Wisej.Web.Label();
            this.pnlTickets = new Wisej.Web.Panel();
            this.dgvTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssignedTo = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCreated = new Wisej.Web.DataGridViewTextBoxColumn();
            this.btnNewTicket = new Wisej.Web.Button();
            this.btnCloseSelected = new Wisej.Web.Button();
            this.lblTicketsHint = new Wisej.Web.Label();
            this.pnlTickets.SuspendLayout();
            this.SuspendLayout();
            //
            // lblViewTitle
            //
            this.lblViewTitle.AutoSize = false;
            this.lblViewTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblViewTitle.Location = new System.Drawing.Point(0, 0);
            this.lblViewTitle.Name = "lblViewTitle";
            this.lblViewTitle.Size = new System.Drawing.Size(400, 30);
            this.lblViewTitle.Text = "Tickets  ·  TicketsView (UserControl)";
            //
            // lblTicketsSummary
            //
            this.lblTicketsSummary.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblTicketsSummary.AutoSize = false;
            this.lblTicketsSummary.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTicketsSummary.Location = new System.Drawing.Point(780, 0);
            this.lblTicketsSummary.Name = "lblTicketsSummary";
            this.lblTicketsSummary.Size = new System.Drawing.Size(300, 30);
            this.lblTicketsSummary.Text = "";
            this.lblTicketsSummary.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlTickets  (the list card)
            //
            this.pnlTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlTickets.BackColor = System.Drawing.Color.White;
            this.pnlTickets.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTickets.Controls.Add(this.dgvTickets);
            this.pnlTickets.Controls.Add(this.btnNewTicket);
            this.pnlTickets.Controls.Add(this.btnCloseSelected);
            this.pnlTickets.Controls.Add(this.lblTicketsHint);
            this.pnlTickets.Location = new System.Drawing.Point(0, 44);
            this.pnlTickets.Name = "pnlTickets";
            this.pnlTickets.Size = new System.Drawing.Size(1080, 492);
            //
            // dgvTickets
            //
            this.dgvTickets.AllowUserToAddRows = false;
            this.dgvTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvTickets.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colId,
            this.colTitle,
            this.colCustomer,
            this.colStatus,
            this.colPriority,
            this.colAssignedTo,
            this.colCreated});
            this.dgvTickets.Location = new System.Drawing.Point(20, 20);
            this.dgvTickets.MultiSelect = false;
            this.dgvTickets.Name = "dgvTickets";
            this.dgvTickets.ReadOnly = true;
            this.dgvTickets.RowHeadersVisible = false;
            this.dgvTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTickets.Size = new System.Drawing.Size(1040, 396);
            //
            // colId
            //
            this.colId.HeaderText = "#";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            this.colId.Width = 50;
            //
            // colTitle
            //
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            this.colTitle.Width = 360;
            //
            // colCustomer
            //
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.Name = "colCustomer";
            this.colCustomer.ReadOnly = true;
            this.colCustomer.Width = 190;
            //
            // colStatus
            //
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 110;
            //
            // colPriority
            //
            this.colPriority.HeaderText = "Priority";
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            this.colPriority.Width = 90;
            //
            // colAssignedTo
            //
            this.colAssignedTo.HeaderText = "Assigned to";
            this.colAssignedTo.Name = "colAssignedTo";
            this.colAssignedTo.ReadOnly = true;
            this.colAssignedTo.Width = 110;
            //
            // colCreated
            //
            this.colCreated.HeaderText = "Created";
            this.colCreated.Name = "colCreated";
            this.colCreated.ReadOnly = true;
            this.colCreated.Width = 110;
            //
            // btnNewTicket
            //
            this.btnNewTicket.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnNewTicket.Location = new System.Drawing.Point(20, 436);
            this.btnNewTicket.Name = "btnNewTicket";
            this.btnNewTicket.Size = new System.Drawing.Size(150, 36);
            this.btnNewTicket.Text = "New Ticket";
            this.btnNewTicket.ToolTipText = "Adds a ticket through TicketService and selects it in the grid.";
            this.btnNewTicket.Click += new System.EventHandler(this.btnNewTicket_Click);
            //
            // btnCloseSelected
            //
            this.btnCloseSelected.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnCloseSelected.Location = new System.Drawing.Point(180, 436);
            this.btnCloseSelected.Name = "btnCloseSelected";
            this.btnCloseSelected.Size = new System.Drawing.Size(150, 36);
            this.btnCloseSelected.Text = "Close Selected";
            this.btnCloseSelected.ToolTipText = "Marks the selected ticket Closed; with no selection the shell logs a warning.";
            this.btnCloseSelected.Click += new System.EventHandler(this.btnCloseSelected_Click);
            //
            // lblTicketsHint
            //
            this.lblTicketsHint.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.lblTicketsHint.AutoSize = false;
            this.lblTicketsHint.Font = new System.Drawing.Font("monospace", 8F);
            this.lblTicketsHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTicketsHint.Location = new System.Drawing.Point(500, 436);
            this.lblTicketsHint.Name = "lblTicketsHint";
            this.lblTicketsHint.Size = new System.Drawing.Size(560, 36);
            this.lblTicketsHint.Text = "Both roles may create and close tickets · the list lives in TicketService (per session)";
            this.lblTicketsHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // TicketsView
            //
            this.Controls.Add(this.lblViewTitle);
            this.Controls.Add(this.lblTicketsSummary);
            this.Controls.Add(this.pnlTickets);
            this.Name = "TicketsView";
            this.Size = new System.Drawing.Size(1080, 536);
            this.Load += new System.EventHandler(this.TicketsView_Load);
            this.pnlTickets.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblViewTitle;
        private Wisej.Web.Label lblTicketsSummary;
        private Wisej.Web.Panel pnlTickets;
        private Wisej.Web.DataGridView dgvTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colAssignedTo;
        private Wisej.Web.DataGridViewTextBoxColumn colCreated;
        private Wisej.Web.Button btnNewTicket;
        private Wisej.Web.Button btnCloseSelected;
        private Wisej.Web.Label lblTicketsHint;
    }
}
