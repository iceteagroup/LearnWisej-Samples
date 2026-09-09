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
            this.lblPageTitle = new Wisej.Web.Label();
            this.lblPageDescription = new Wisej.Web.Label();
            this.cardQueue = new Wisej.Web.Panel();
            this.lblQueueTitle = new Wisej.Web.Label();
            this.dgvTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssignedTo = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCreatedDate = new Wisej.Web.DataGridViewTextBoxColumn();
            this.ticketsBindingSource = new Wisej.Web.BindingSource(this.components);
            this.btnNew = new Wisej.Web.Button();
            this.btnEdit = new Wisej.Web.Button();
            this.btnDelete = new Wisej.Web.Button();
            this.btnTryBlankTitle = new Wisej.Web.Button();
            this.lblQueueHint = new Wisej.Web.Label();
            this.cardQueue.SuspendLayout();
            this.SuspendLayout();
            //
            // lblPageTitle
            //
            this.lblPageTitle.AutoSize = false;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(32, 24);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(600, 34);
            this.lblPageTitle.Text = "Tickets";
            //
            // lblPageDescription
            //
            this.lblPageDescription.AutoSize = false;
            this.lblPageDescription.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblPageDescription.Location = new System.Drawing.Point(32, 60);
            this.lblPageDescription.Name = "lblPageDescription";
            this.lblPageDescription.Size = new System.Drawing.Size(1000, 22);
            this.lblPageDescription.Text = "The Module 4/5 ticket queue — same service, same binding, same dialog and validation. Only the card and the spacing are new.";
            //
            // cardQueue  (1084 wide at 32,100 · anchored on all four sides so the grid grows with the window)
            //
            this.cardQueue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.cardQueue.BackColor = System.Drawing.Color.White;
            this.cardQueue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardQueue.Controls.Add(this.lblQueueTitle);
            this.cardQueue.Controls.Add(this.dgvTickets);
            this.cardQueue.Controls.Add(this.btnNew);
            this.cardQueue.Controls.Add(this.btnEdit);
            this.cardQueue.Controls.Add(this.btnDelete);
            this.cardQueue.Controls.Add(this.btnTryBlankTitle);
            this.cardQueue.Controls.Add(this.lblQueueHint);
            this.cardQueue.Location = new System.Drawing.Point(32, 100);
            this.cardQueue.Name = "cardQueue";
            this.cardQueue.Size = new System.Drawing.Size(1084, 496);
            //
            // lblQueueTitle
            //
            this.lblQueueTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblQueueTitle.AutoSize = false;
            this.lblQueueTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblQueueTitle.Location = new System.Drawing.Point(20, 14);
            this.lblQueueTitle.Name = "lblQueueTitle";
            this.lblQueueTitle.Size = new System.Drawing.Size(1044, 28);
            this.lblQueueTitle.Text = "Ticket queue";
            //
            // dgvTickets  (Module 4 binding — unchanged: BindingSource → DataSource, explicit columns)
            //
            this.dgvTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvTickets.AutoGenerateColumns = false;
            this.dgvTickets.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTickets.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colId,
            this.colTitle,
            this.colCustomer,
            this.colStatus,
            this.colPriority,
            this.colAssignedTo,
            this.colCreatedDate});
            this.dgvTickets.DataSource = this.ticketsBindingSource;
            this.dgvTickets.Location = new System.Drawing.Point(20, 52);
            this.dgvTickets.MultiSelect = false;
            this.dgvTickets.Name = "dgvTickets";
            this.dgvTickets.ReadOnly = true;
            this.dgvTickets.RowHeadersVisible = false;
            this.dgvTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTickets.Size = new System.Drawing.Size(1044, 356);
            //
            // colId
            //
            this.colId.DataPropertyName = "Id";
            this.colId.FillWeight = 6F;
            this.colId.HeaderText = "#";
            this.colId.Name = "colId";
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.FillWeight = 34F;
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            //
            // colCustomer
            //
            this.colCustomer.DataPropertyName = "Customer";
            this.colCustomer.FillWeight = 18F;
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.Name = "colCustomer";
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.FillWeight = 11F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            //
            // colPriority
            //
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.FillWeight = 9F;
            this.colPriority.HeaderText = "Priority";
            this.colPriority.Name = "colPriority";
            //
            // colAssignedTo
            //
            this.colAssignedTo.DataPropertyName = "AssignedTo";
            this.colAssignedTo.FillWeight = 11F;
            this.colAssignedTo.HeaderText = "Assigned to";
            this.colAssignedTo.Name = "colAssignedTo";
            //
            // colCreatedDate
            //
            this.colCreatedDate.DataPropertyName = "CreatedDate";
            this.colCreatedDate.DefaultCellStyle.Format = "d";
            this.colCreatedDate.FillWeight = 11F;
            this.colCreatedDate.HeaderText = "Created";
            this.colCreatedDate.Name = "colCreatedDate";
            //
            // btnNew  (commands under the queue they act on: 130 × 36, 12 px apart, anchored to the card bottom)
            //
            this.btnNew.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnNew.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnNew.Location = new System.Drawing.Point(20, 432);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(130, 36);
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            //
            // btnEdit
            //
            this.btnEdit.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnEdit.Location = new System.Drawing.Point(162, 432);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(130, 36);
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            //
            // btnDelete
            //
            this.btnDelete.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnDelete.Location = new System.Drawing.Point(304, 432);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(130, 36);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            //
            // btnTryBlankTitle  (failure path: validation still blocks a blank title after a theme switch)
            //
            this.btnTryBlankTitle.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.btnTryBlankTitle.Location = new System.Drawing.Point(824, 432);
            this.btnTryBlankTitle.Name = "btnTryBlankTitle";
            this.btnTryBlankTitle.Size = new System.Drawing.Size(240, 36);
            this.btnTryBlankTitle.Text = "Try a blank title (validation)";
            this.btnTryBlankTitle.ToolTipText = "Opens the dialog with an empty title: ValidateForm() blocks Save whatever theme is loaded. Type a title to recover.";
            this.btnTryBlankTitle.Click += new System.EventHandler(this.btnTryBlankTitle_Click);
            //
            // lblQueueHint
            //
            this.lblQueueHint.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.lblQueueHint.AutoSize = false;
            this.lblQueueHint.Font = new System.Drawing.Font("monospace", 8F);
            this.lblQueueHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblQueueHint.Location = new System.Drawing.Point(452, 432);
            this.lblQueueHint.Name = "lblQueueHint";
            this.lblQueueHint.Size = new System.Drawing.Size(360, 36);
            this.lblQueueHint.Text = "ticketsBindingSource → dgvTickets · TicketDialog.ValidateForm\nawait ShowDialogAsync() · await MessageBox.ShowAsync(YesNo)";
            this.lblQueueHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // TicketsView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.lblPageTitle);
            this.Controls.Add(this.lblPageDescription);
            this.Controls.Add(this.cardQueue);
            this.Name = "TicketsView";
            this.Size = new System.Drawing.Size(1148, 620);
            this.cardQueue.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.Label lblPageDescription;
        private Wisej.Web.Panel cardQueue;
        private Wisej.Web.Label lblQueueTitle;
        private Wisej.Web.DataGridView dgvTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colAssignedTo;
        private Wisej.Web.DataGridViewTextBoxColumn colCreatedDate;
        private Wisej.Web.BindingSource ticketsBindingSource;
        private Wisej.Web.Button btnNew;
        private Wisej.Web.Button btnEdit;
        private Wisej.Web.Button btnDelete;
        private Wisej.Web.Button btnTryBlankTitle;
        private Wisej.Web.Label lblQueueHint;
    }
}
