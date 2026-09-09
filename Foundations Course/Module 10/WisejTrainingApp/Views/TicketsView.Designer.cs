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
            this.pnlTickets = new Wisej.Web.Panel();
            this.labelTicketsCard = new Wisej.Web.Label();
            this.btnCreateTicket = new Wisej.Web.Button();
            this.btnEditTicket = new Wisej.Web.Button();
            this.btnDeleteTicket = new Wisej.Web.Button();
            this.btnRefresh = new Wisej.Web.Button();
            this.dgvTickets = new Wisej.Web.DataGridView();
            this.lblSelection = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.lblHint = new Wisej.Web.Label();
            this.pnlTickets.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlTickets  (one card: title, command row top-right, grid, selection hint, status, hint)
            //
            this.pnlTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlTickets.BackColor = System.Drawing.Color.White;
            this.pnlTickets.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTickets.Controls.Add(this.labelTicketsCard);
            this.pnlTickets.Controls.Add(this.btnCreateTicket);
            this.pnlTickets.Controls.Add(this.btnEditTicket);
            this.pnlTickets.Controls.Add(this.btnDeleteTicket);
            this.pnlTickets.Controls.Add(this.btnRefresh);
            this.pnlTickets.Controls.Add(this.dgvTickets);
            this.pnlTickets.Controls.Add(this.lblSelection);
            this.pnlTickets.Controls.Add(this.lblStatus);
            this.pnlTickets.Controls.Add(this.lblHint);
            this.pnlTickets.Location = new System.Drawing.Point(0, 0);
            this.pnlTickets.Name = "pnlTickets";
            this.pnlTickets.Size = new System.Drawing.Size(1032, 532);
            //
            // labelTicketsCard
            //
            this.labelTicketsCard.AutoSize = false;
            this.labelTicketsCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTicketsCard.Location = new System.Drawing.Point(24, 14);
            this.labelTicketsCard.Name = "labelTicketsCard";
            this.labelTicketsCard.Size = new System.Drawing.Size(420, 28);
            this.labelTicketsCard.Text = "Tickets  ·  dgvTickets bound through a BindingSource";
            //
            // btnCreateTicket  (primary — first in the group)
            //
            this.btnCreateTicket.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnCreateTicket.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnCreateTicket.Location = new System.Drawing.Point(508, 10);
            this.btnCreateTicket.Name = "btnCreateTicket";
            this.btnCreateTicket.Size = new System.Drawing.Size(140, 36);
            this.btnCreateTicket.Text = "Create ticket";
            this.btnCreateTicket.ToolTipText = "new TicketDialog() → ShowDialogAsync → TicketService.AddTicket → RefreshTicketGrid.";
            this.btnCreateTicket.Click += new System.EventHandler(this.btnCreateTicket_Click);
            //
            // btnEditTicket
            //
            this.btnEditTicket.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnEditTicket.Location = new System.Drawing.Point(656, 10);
            this.btnEditTicket.Name = "btnEditTicket";
            this.btnEditTicket.Size = new System.Drawing.Size(110, 36);
            this.btnEditTicket.Text = "Edit";
            this.btnEditTicket.ToolTipText = "Same TicketDialog with the selected ticket → TicketService.UpdateTicket.";
            this.btnEditTicket.Click += new System.EventHandler(this.btnEditTicket_Click);
            //
            // btnDeleteTicket  (danger — last in the group, before the neutral Refresh)
            //
            this.btnDeleteTicket.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnDeleteTicket.Location = new System.Drawing.Point(774, 10);
            this.btnDeleteTicket.Name = "btnDeleteTicket";
            this.btnDeleteTicket.Size = new System.Drawing.Size(110, 36);
            this.btnDeleteTicket.Text = "Delete";
            this.btnDeleteTicket.ToolTipText = "MessageBox.ShowAsync Yes/No → TicketService.DeleteTicket.";
            this.btnDeleteTicket.Click += new System.EventHandler(this.btnDeleteTicket_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnRefresh.Location = new System.Drawing.Point(898, 10);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(110, 36);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // dgvTickets
            //
            this.dgvTickets.AllowUserToAddRows = false;
            this.dgvTickets.AllowUserToDeleteRows = false;
            this.dgvTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvTickets.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTickets.Location = new System.Drawing.Point(24, 58);
            this.dgvTickets.MultiSelect = false;
            this.dgvTickets.Name = "dgvTickets";
            this.dgvTickets.ReadOnly = true;
            this.dgvTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTickets.Size = new System.Drawing.Size(984, 380);
            this.dgvTickets.SelectionChanged += new System.EventHandler(this.dgvTickets_SelectionChanged);
            //
            // lblSelection
            //
            this.lblSelection.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblSelection.AutoSize = false;
            this.lblSelection.AutoEllipsis = true;
            this.lblSelection.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSelection.Location = new System.Drawing.Point(24, 446);
            this.lblSelection.Name = "lblSelection";
            this.lblSelection.Size = new System.Drawing.Size(984, 22);
            this.lblSelection.Text = "No ticket selected.";
            //
            // lblStatus
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(24, 472);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(984, 26);
            this.lblStatus.Text = "● ready — Create, or select a row and Edit / Delete";
            //
            // lblHint
            //
            this.lblHint.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblHint.AutoSize = false;
            this.lblHint.Font = new System.Drawing.Font("monospace", 8F);
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblHint.Location = new System.Drawing.Point(24, 500);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(984, 22);
            this.lblHint.Text = "flow: click → TicketDialog → TicketValidator → TicketService → RefreshTicketGrid() → lblStatus + Dashboard activity";
            //
            // TicketsView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlTickets);
            this.Name = "TicketsView";
            this.Size = new System.Drawing.Size(1032, 532);
            this.pnlTickets.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlTickets;
        private Wisej.Web.Label labelTicketsCard;
        private Wisej.Web.Button btnCreateTicket;
        private Wisej.Web.Button btnEditTicket;
        private Wisej.Web.Button btnDeleteTicket;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.DataGridView dgvTickets;
        private Wisej.Web.Label lblSelection;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblHint;
    }
}
