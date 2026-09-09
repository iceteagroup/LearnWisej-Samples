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
            this.components = new System.ComponentModel.Container();
            this.lblDialogHint = new Wisej.Web.Label();
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
            this.lblValidation = new Wisej.Web.Label();
            this.panelButtons = new Wisej.Web.Panel();
            this.btnSave = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // lblDialogHint
            //
            this.lblDialogHint.AutoSize = false;
            this.lblDialogHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblDialogHint.Location = new System.Drawing.Point(24, 16);
            this.lblDialogHint.Name = "lblDialogHint";
            this.lblDialogHint.Size = new System.Drawing.Size(512, 22);
            this.lblDialogHint.Text = "Fill the ticket and Save.";
            //
            // lblTitleField
            //
            this.lblTitleField.AutoSize = false;
            this.lblTitleField.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblTitleField.Location = new System.Drawing.Point(24, 48);
            this.lblTitleField.Name = "lblTitleField";
            this.lblTitleField.Size = new System.Drawing.Size(512, 20);
            this.lblTitleField.Text = "Title *";
            //
            // txtTitle
            //
            this.txtTitle.Location = new System.Drawing.Point(24, 70);
            this.txtTitle.MaxLength = 120;
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(512, 34);
            this.txtTitle.Watermark = "Short summary of the problem (required, max 80 characters)";
            //
            // lblCustomerField
            //
            this.lblCustomerField.AutoSize = false;
            this.lblCustomerField.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblCustomerField.Location = new System.Drawing.Point(24, 114);
            this.lblCustomerField.Name = "lblCustomerField";
            this.lblCustomerField.Size = new System.Drawing.Size(512, 20);
            this.lblCustomerField.Text = "Customer *";
            //
            // txtCustomer
            //
            this.txtCustomer.Location = new System.Drawing.Point(24, 136);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.Size = new System.Drawing.Size(512, 34);
            this.txtCustomer.Watermark = "Company that reported it (required) — e.g. Northwind, Contoso";
            //
            // lblStatusField
            //
            this.lblStatusField.AutoSize = false;
            this.lblStatusField.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusField.Location = new System.Drawing.Point(24, 180);
            this.lblStatusField.Name = "lblStatusField";
            this.lblStatusField.Size = new System.Drawing.Size(248, 20);
            this.lblStatusField.Text = "Status *";
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Items.AddRange(new object[] {
            "Open",
            "In Progress",
            "Closed"});
            this.cboStatus.Location = new System.Drawing.Point(24, 202);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(248, 34);
            //
            // lblPriorityField
            //
            this.lblPriorityField.AutoSize = false;
            this.lblPriorityField.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblPriorityField.Location = new System.Drawing.Point(288, 180);
            this.lblPriorityField.Name = "lblPriorityField";
            this.lblPriorityField.Size = new System.Drawing.Size(248, 20);
            this.lblPriorityField.Text = "Priority *";
            //
            // cboPriority
            //
            this.cboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPriority.Items.AddRange(new object[] {
            "Low",
            "Medium",
            "High"});
            this.cboPriority.Location = new System.Drawing.Point(288, 202);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Size = new System.Drawing.Size(248, 34);
            //
            // lblAssignedToField
            //
            this.lblAssignedToField.AutoSize = false;
            this.lblAssignedToField.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblAssignedToField.Location = new System.Drawing.Point(24, 246);
            this.lblAssignedToField.Name = "lblAssignedToField";
            this.lblAssignedToField.Size = new System.Drawing.Size(512, 20);
            this.lblAssignedToField.Text = "Assigned to  (required when Status is Closed)";
            //
            // txtAssignedTo
            //
            this.txtAssignedTo.Location = new System.Drawing.Point(24, 268);
            this.txtAssignedTo.Name = "txtAssignedTo";
            this.txtAssignedTo.Size = new System.Drawing.Size(512, 34);
            this.txtAssignedTo.Watermark = "Agent name — Ada, Grace, Linus";
            //
            // lblDescriptionField
            //
            this.lblDescriptionField.AutoSize = false;
            this.lblDescriptionField.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblDescriptionField.Location = new System.Drawing.Point(24, 312);
            this.lblDescriptionField.Name = "lblDescriptionField";
            this.lblDescriptionField.Size = new System.Drawing.Size(512, 20);
            this.lblDescriptionField.Text = "Description";
            //
            // txtDescription
            //
            this.txtDescription.Location = new System.Drawing.Point(24, 334);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(512, 80);
            this.txtDescription.Watermark = "What the customer sees, steps to reproduce, anything the next agent needs";
            //
            // lblValidation  (TicketValidator's messages land here — one per line)
            //
            this.lblValidation.AutoSize = false;
            this.lblValidation.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblValidation.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblValidation.Location = new System.Drawing.Point(24, 422);
            this.lblValidation.Name = "lblValidation";
            this.lblValidation.Size = new System.Drawing.Size(512, 60);
            this.lblValidation.Text = "";
            this.lblValidation.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelButtons  (commands bottom-right: primary first, Cancel last — the same placement everywhere)
            //
            this.panelButtons.Controls.Add(this.btnSave);
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Location = new System.Drawing.Point(24, 492);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(512, 40);
            //
            // btnSave
            //
            this.btnSave.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(272, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(116, 36);
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "ValidateForm() → TicketValidator.Validate(ticket) → DialogResult.OK when valid.";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(396, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(116, 36);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // TicketDialog
            //
            this.AcceptButton = this.btnSave;
            this.ClientSize = new System.Drawing.Size(560, 548);
            this.Controls.Add(this.lblDialogHint);
            this.Controls.Add(this.lblTitleField);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblCustomerField);
            this.Controls.Add(this.txtCustomer);
            this.Controls.Add(this.lblStatusField);
            this.Controls.Add(this.cboStatus);
            this.Controls.Add(this.lblPriorityField);
            this.Controls.Add(this.cboPriority);
            this.Controls.Add(this.lblAssignedToField);
            this.Controls.Add(this.txtAssignedTo);
            this.Controls.Add(this.lblDescriptionField);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblValidation);
            this.Controls.Add(this.panelButtons);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TicketDialog";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "Ticket";
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblDialogHint;
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
        private Wisej.Web.Label lblValidation;
        private Wisej.Web.Panel panelButtons;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnCancel;
    }
}
