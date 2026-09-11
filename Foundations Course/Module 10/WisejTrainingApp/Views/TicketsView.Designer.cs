namespace WisejTrainingApp.Views
{
    partial class TicketsView
    {
        private System.ComponentModel.IContainer components = null;

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
            this.lblPageTitle = new Wisej.Web.Label();
            this.btnCreateTicket = new Wisej.Web.Button();
            this.btnEditTicket = new Wisej.Web.Button();
            this.btnDeleteTicket = new Wisej.Web.Button();
            this.dgvTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssignedTo = new Wisej.Web.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTickets)).BeginInit();
            this.SuspendLayout();
            //
            // lblPageTitle
            //
            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Text = "Tickets";
            //
            // btnCreateTicket
            //
            this.btnCreateTicket.Location = new System.Drawing.Point(0, 44);
            this.btnCreateTicket.Name = "btnCreateTicket";
            this.btnCreateTicket.Size = new System.Drawing.Size(110, 34);
            this.btnCreateTicket.Text = "Create";
            this.btnCreateTicket.Click += new System.EventHandler(this.btnCreateTicket_Click);
            //
            // btnEditTicket
            //
            this.btnEditTicket.Location = new System.Drawing.Point(118, 44);
            this.btnEditTicket.Name = "btnEditTicket";
            this.btnEditTicket.Size = new System.Drawing.Size(110, 34);
            this.btnEditTicket.Text = "Edit";
            this.btnEditTicket.Click += new System.EventHandler(this.btnEditTicket_Click);
            //
            // btnDeleteTicket
            //
            this.btnDeleteTicket.Location = new System.Drawing.Point(236, 44);
            this.btnDeleteTicket.Name = "btnDeleteTicket";
            this.btnDeleteTicket.Size = new System.Drawing.Size(110, 34);
            this.btnDeleteTicket.Text = "Delete";
            this.btnDeleteTicket.Click += new System.EventHandler(this.btnDeleteTicket_Click);
            //
            // dgvTickets
            //
            this.dgvTickets.Anchor = ((Wisej.Web.AnchorStyles)((((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom)
            | Wisej.Web.AnchorStyles.Left)
            | Wisej.Web.AnchorStyles.Right)));
            this.dgvTickets.AutoGenerateColumns = false;
            this.dgvTickets.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTickets.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colId,
            this.colTitle,
            this.colCustomer,
            this.colStatus,
            this.colPriority,
            this.colAssignedTo});
            this.dgvTickets.Location = new System.Drawing.Point(0, 90);
            this.dgvTickets.MultiSelect = false;
            this.dgvTickets.Name = "dgvTickets";
            this.dgvTickets.ReadOnly = true;
            this.dgvTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTickets.Size = new System.Drawing.Size(800, 410);
            //
            // colId
            //
            this.colId.DataPropertyName = "Id";
            this.colId.HeaderText = "Id";
            this.colId.Name = "colId";
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            //
            // colCustomer
            //
            this.colCustomer.DataPropertyName = "Customer";
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.Name = "colCustomer";
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            //
            // colPriority
            //
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.HeaderText = "Priority";
            this.colPriority.Name = "colPriority";
            //
            // colAssignedTo
            //
            this.colAssignedTo.DataPropertyName = "AssignedTo";
            this.colAssignedTo.HeaderText = "Assigned To";
            this.colAssignedTo.Name = "colAssignedTo";
            //
            // TicketsView
            //
            this.Controls.Add(this.dgvTickets);
            this.Controls.Add(this.btnDeleteTicket);
            this.Controls.Add(this.btnEditTicket);
            this.Controls.Add(this.btnCreateTicket);
            this.Controls.Add(this.lblPageTitle);
            this.Name = "TicketsView";
            this.Size = new System.Drawing.Size(800, 500);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTickets)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.Button btnCreateTicket;
        private Wisej.Web.Button btnEditTicket;
        private Wisej.Web.Button btnDeleteTicket;
        private Wisej.Web.DataGridView dgvTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colAssignedTo;
    }
}
