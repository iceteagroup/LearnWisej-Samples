namespace WisejTrainingApp.Dialogs
{
    partial class TicketDialog
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
            this.lblTitleField = new Wisej.Web.Label();
            this.txtTitle = new Wisej.Web.TextBox();
            this.lblCustomerField = new Wisej.Web.Label();
            this.txtCustomer = new Wisej.Web.TextBox();
            this.lblStatusField = new Wisej.Web.Label();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.lblPriorityField = new Wisej.Web.Label();
            this.cboPriority = new Wisej.Web.ComboBox();
            this.lblAssignedToField = new Wisej.Web.Label();
            this.txtAssignedTo = new Wisej.Web.TextBox();
            this.lblDescriptionField = new Wisej.Web.Label();
            this.txtDescription = new Wisej.Web.TextBox();
            this.btnSave = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // lblTitleField
            //
            this.lblTitleField.AutoSize = true;
            this.lblTitleField.Location = new System.Drawing.Point(20, 16);
            this.lblTitleField.Name = "lblTitleField";
            this.lblTitleField.Text = "Title *";
            //
            // txtTitle
            //
            this.txtTitle.Location = new System.Drawing.Point(20, 38);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(360, 30);
            this.txtTitle.TabIndex = 0;
            //
            // lblCustomerField
            //
            this.lblCustomerField.AutoSize = true;
            this.lblCustomerField.Location = new System.Drawing.Point(20, 76);
            this.lblCustomerField.Name = "lblCustomerField";
            this.lblCustomerField.Text = "Customer *";
            //
            // txtCustomer
            //
            this.txtCustomer.Location = new System.Drawing.Point(20, 98);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.Size = new System.Drawing.Size(360, 30);
            this.txtCustomer.TabIndex = 1;
            //
            // lblStatusField
            //
            this.lblStatusField.AutoSize = true;
            this.lblStatusField.Location = new System.Drawing.Point(20, 136);
            this.lblStatusField.Name = "lblStatusField";
            this.lblStatusField.Text = "Status *";
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Items.AddRange(new object[] {
            "Open",
            "In Progress",
            "Closed"});
            this.cboStatus.Location = new System.Drawing.Point(20, 158);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(172, 30);
            this.cboStatus.TabIndex = 2;
            //
            // lblPriorityField
            //
            this.lblPriorityField.AutoSize = true;
            this.lblPriorityField.Location = new System.Drawing.Point(208, 136);
            this.lblPriorityField.Name = "lblPriorityField";
            this.lblPriorityField.Text = "Priority";
            //
            // cboPriority
            //
            this.cboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPriority.Items.AddRange(new object[] {
            "Low",
            "Medium",
            "High"});
            this.cboPriority.Location = new System.Drawing.Point(208, 158);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Size = new System.Drawing.Size(172, 30);
            this.cboPriority.TabIndex = 3;
            //
            // lblAssignedToField
            //
            this.lblAssignedToField.AutoSize = true;
            this.lblAssignedToField.Location = new System.Drawing.Point(20, 196);
            this.lblAssignedToField.Name = "lblAssignedToField";
            this.lblAssignedToField.Text = "Assigned To";
            //
            // txtAssignedTo
            //
            this.txtAssignedTo.Location = new System.Drawing.Point(20, 218);
            this.txtAssignedTo.Name = "txtAssignedTo";
            this.txtAssignedTo.Size = new System.Drawing.Size(360, 30);
            this.txtAssignedTo.TabIndex = 4;
            //
            // lblDescriptionField
            //
            this.lblDescriptionField.AutoSize = true;
            this.lblDescriptionField.Location = new System.Drawing.Point(20, 256);
            this.lblDescriptionField.Name = "lblDescriptionField";
            this.lblDescriptionField.Text = "Description";
            //
            // txtDescription
            //
            this.txtDescription.Location = new System.Drawing.Point(20, 278);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(360, 90);
            this.txtDescription.TabIndex = 5;
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(180, 388);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(96, 34);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(284, 388);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 34);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // TicketDialog
            //
            this.AcceptButton = this.btnSave;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(400, 440);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescriptionField);
            this.Controls.Add(this.txtAssignedTo);
            this.Controls.Add(this.lblAssignedToField);
            this.Controls.Add(this.cboPriority);
            this.Controls.Add(this.lblPriorityField);
            this.Controls.Add(this.cboStatus);
            this.Controls.Add(this.lblStatusField);
            this.Controls.Add(this.txtCustomer);
            this.Controls.Add(this.lblCustomerField);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblTitleField);
            this.Name = "TicketDialog";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Create Ticket";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblTitleField;
        private Wisej.Web.TextBox txtTitle;
        private Wisej.Web.Label lblCustomerField;
        private Wisej.Web.TextBox txtCustomer;
        private Wisej.Web.Label lblStatusField;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.Label lblPriorityField;
        private Wisej.Web.ComboBox cboPriority;
        private Wisej.Web.Label lblAssignedToField;
        private Wisej.Web.TextBox txtAssignedTo;
        private Wisej.Web.Label lblDescriptionField;
        private Wisej.Web.TextBox txtDescription;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnCancel;
    }
}
