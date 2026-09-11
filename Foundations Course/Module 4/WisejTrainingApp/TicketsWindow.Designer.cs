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
            this.splitContainer1 = new Wisej.Web.SplitContainer();
            this.dgvTickets = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.grpTicketDetails = new Wisej.Web.GroupBox();
            this.lblTitleCaption = new Wisej.Web.Label();
            this.txtTitle = new Wisej.Web.TextBox();
            this.lblStatusCaption = new Wisej.Web.Label();
            this.cmbStatus = new Wisej.Web.ComboBox();
            this.lblPriorityCaption = new Wisej.Web.Label();
            this.cmbPriority = new Wisej.Web.ComboBox();
            this.lblCreatedCaption = new Wisej.Web.Label();
            this.dtpCreated = new Wisej.Web.DateTimePicker();
            this.btnSaveTicket = new Wisej.Web.Button();
            this.lblStatus = new Wisej.Web.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTickets)).BeginInit();
            this.grpTicketDetails.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainer1
            //
            this.splitContainer1.Dock = Wisej.Web.DockStyle.Fill;
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = Wisej.Web.Orientation.Vertical;
            //
            // splitContainer1.Panel1
            //
            this.splitContainer1.Panel1.Controls.Add(this.dgvTickets);
            //
            // splitContainer1.Panel2
            //
            this.splitContainer1.Panel2.Controls.Add(this.grpTicketDetails);
            this.splitContainer1.Size = new System.Drawing.Size(960, 452);
            this.splitContainer1.SplitterDistance = 600;
            //
            // dgvTickets
            //
            this.dgvTickets.AutoGenerateColumns = false;
            this.dgvTickets.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTickets.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colId,
            this.colTitle,
            this.colStatus,
            this.colPriority});
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
            // grpTicketDetails
            //
            this.grpTicketDetails.Controls.Add(this.lblTitleCaption);
            this.grpTicketDetails.Controls.Add(this.txtTitle);
            this.grpTicketDetails.Controls.Add(this.lblStatusCaption);
            this.grpTicketDetails.Controls.Add(this.cmbStatus);
            this.grpTicketDetails.Controls.Add(this.lblPriorityCaption);
            this.grpTicketDetails.Controls.Add(this.cmbPriority);
            this.grpTicketDetails.Controls.Add(this.lblCreatedCaption);
            this.grpTicketDetails.Controls.Add(this.dtpCreated);
            this.grpTicketDetails.Controls.Add(this.btnSaveTicket);
            this.grpTicketDetails.Dock = Wisej.Web.DockStyle.Fill;
            this.grpTicketDetails.Name = "grpTicketDetails";
            this.grpTicketDetails.Text = "Ticket Details";
            //
            // lblTitleCaption
            //
            this.lblTitleCaption.AutoSize = true;
            this.lblTitleCaption.Location = new System.Drawing.Point(16, 30);
            this.lblTitleCaption.Name = "lblTitleCaption";
            this.lblTitleCaption.Text = "Title";
            //
            // txtTitle
            //
            this.txtTitle.Location = new System.Drawing.Point(16, 52);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(300, 30);
            //
            // lblStatusCaption
            //
            this.lblStatusCaption.AutoSize = true;
            this.lblStatusCaption.Location = new System.Drawing.Point(16, 92);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Text = "Status";
            //
            // cmbStatus
            //
            this.cmbStatus.Items.AddRange(new object[] {
            "Open",
            "In Progress",
            "Closed"});
            this.cmbStatus.Location = new System.Drawing.Point(16, 114);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(300, 30);
            //
            // lblPriorityCaption
            //
            this.lblPriorityCaption.AutoSize = true;
            this.lblPriorityCaption.Location = new System.Drawing.Point(16, 154);
            this.lblPriorityCaption.Name = "lblPriorityCaption";
            this.lblPriorityCaption.Text = "Priority";
            //
            // cmbPriority
            //
            this.cmbPriority.Items.AddRange(new object[] {
            "Low",
            "Medium",
            "High"});
            this.cmbPriority.Location = new System.Drawing.Point(16, 176);
            this.cmbPriority.Name = "cmbPriority";
            this.cmbPriority.Size = new System.Drawing.Size(300, 30);
            //
            // lblCreatedCaption
            //
            this.lblCreatedCaption.AutoSize = true;
            this.lblCreatedCaption.Location = new System.Drawing.Point(16, 216);
            this.lblCreatedCaption.Name = "lblCreatedCaption";
            this.lblCreatedCaption.Text = "Created";
            //
            // dtpCreated
            //
            this.dtpCreated.Location = new System.Drawing.Point(16, 238);
            this.dtpCreated.Name = "dtpCreated";
            this.dtpCreated.Size = new System.Drawing.Size(300, 30);
            //
            // btnSaveTicket
            //
            this.btnSaveTicket.Location = new System.Drawing.Point(16, 288);
            this.btnSaveTicket.Name = "btnSaveTicket";
            this.btnSaveTicket.Size = new System.Drawing.Size(120, 34);
            this.btnSaveTicket.Text = "Save Ticket";
            this.btnSaveTicket.Click += new System.EventHandler(this.btnSaveTicket_Click);
            //
            // lblStatus
            //
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStatus.Size = new System.Drawing.Size(960, 28);
            this.lblStatus.Text = "Ready.";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // TicketsWindow
            //
            this.ClientSize = new System.Drawing.Size(960, 480);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.lblStatus);
            this.Name = "TicketsWindow";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "Tickets";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTickets)).EndInit();
            this.grpTicketDetails.ResumeLayout(false);
            this.grpTicketDetails.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.SplitContainer splitContainer1;
        private Wisej.Web.DataGridView dgvTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.GroupBox grpTicketDetails;
        private Wisej.Web.Label lblTitleCaption;
        private Wisej.Web.TextBox txtTitle;
        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.ComboBox cmbStatus;
        private Wisej.Web.Label lblPriorityCaption;
        private Wisej.Web.ComboBox cmbPriority;
        private Wisej.Web.Label lblCreatedCaption;
        private Wisej.Web.DateTimePicker dtpCreated;
        private Wisej.Web.Button btnSaveTicket;
        private Wisej.Web.Label lblStatus;
    }
}
