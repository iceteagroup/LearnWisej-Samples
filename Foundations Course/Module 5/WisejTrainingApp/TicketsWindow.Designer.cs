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
            this.pnlCommands = new Wisej.Web.Panel();
            this.btnNew = new Wisej.Web.Button();
            this.btnEdit = new Wisej.Web.Button();
            this.dgvTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssignedTo = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlCommands.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTickets)).BeginInit();
            this.SuspendLayout();
            //
            // pnlCommands
            //
            this.pnlCommands.Controls.Add(this.btnNew);
            this.pnlCommands.Controls.Add(this.btnEdit);
            this.pnlCommands.Dock = Wisej.Web.DockStyle.Top;
            this.pnlCommands.Name = "pnlCommands";
            this.pnlCommands.Size = new System.Drawing.Size(880, 52);
            //
            // btnNew
            //
            this.btnNew.Location = new System.Drawing.Point(8, 9);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(120, 34);
            this.btnNew.Text = "New Ticket";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            //
            // btnEdit
            //
            this.btnEdit.Location = new System.Drawing.Point(136, 9);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(120, 34);
            this.btnEdit.Text = "Edit Ticket";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            //
            // dgvTickets
            //
            this.dgvTickets.AutoGenerateColumns = false;
            this.dgvTickets.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTickets.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colId,
            this.colTitle,
            this.colCustomer,
            this.colStatus,
            this.colPriority,
            this.colAssignedTo});
            this.dgvTickets.Dock = Wisej.Web.DockStyle.Fill;
            this.dgvTickets.MultiSelect = false;
            this.dgvTickets.Name = "dgvTickets";
            this.dgvTickets.ReadOnly = true;
            this.dgvTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
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
            // TicketsWindow
            //
            this.ClientSize = new System.Drawing.Size(880, 480);
            this.Controls.Add(this.dgvTickets);
            this.Controls.Add(this.pnlCommands);
            this.Name = "TicketsWindow";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "Tickets";
            this.pnlCommands.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTickets)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlCommands;
        private Wisej.Web.Button btnNew;
        private Wisej.Web.Button btnEdit;
        private Wisej.Web.DataGridView dgvTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colAssignedTo;
    }
}
