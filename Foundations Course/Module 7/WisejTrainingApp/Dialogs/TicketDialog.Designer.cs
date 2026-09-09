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
            this.lblDialogHeading = new Wisej.Web.Label();
            this.lblTitleCaption = new Wisej.Web.Label();
            this.txtTitle = new Wisej.Web.TextBox();
            this.lblCustomerCaption = new Wisej.Web.Label();
            this.txtCustomer = new Wisej.Web.TextBox();
            this.lblStatusCaption = new Wisej.Web.Label();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.lblPriorityCaption = new Wisej.Web.Label();
            this.cboPriority = new Wisej.Web.ComboBox();
            this.lblAssignedToCaption = new Wisej.Web.Label();
            this.txtAssignedTo = new Wisej.Web.TextBox();
            this.lblDescriptionCaption = new Wisej.Web.Label();
            this.txtDescription = new Wisej.Web.TextBox();
            this.lblValidation = new Wisej.Web.Label();
            this.btnSave = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // lblDialogHeading
            //
            this.lblDialogHeading.AutoSize = false;
            this.lblDialogHeading.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblDialogHeading.Location = new System.Drawing.Point(24, 16);
            this.lblDialogHeading.Name = "lblDialogHeading";
            this.lblDialogHeading.Size = new System.Drawing.Size(460, 30);
            this.lblDialogHeading.Text = "New ticket";
            //
            // lblTitleCaption
            //
            this.lblTitleCaption.AutoSize = false;
            this.lblTitleCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTitleCaption.Location = new System.Drawing.Point(24, 56);
            this.lblTitleCaption.Name = "lblTitleCaption";
            this.lblTitleCaption.Size = new System.Drawing.Size(460, 20);
            this.lblTitleCaption.Text = "Title *";
            //
            // txtTitle
            //
            this.txtTitle.Location = new System.Drawing.Point(24, 78);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(460, 34);
            this.txtTitle.Watermark = "What is the problem?";
            //
            // lblCustomerCaption
            //
            this.lblCustomerCaption.AutoSize = false;
            this.lblCustomerCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCustomerCaption.Location = new System.Drawing.Point(24, 122);
            this.lblCustomerCaption.Name = "lblCustomerCaption";
            this.lblCustomerCaption.Size = new System.Drawing.Size(224, 20);
            this.lblCustomerCaption.Text = "Customer *";
            //
            // txtCustomer
            //
            this.txtCustomer.Location = new System.Drawing.Point(24, 144);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.Size = new System.Drawing.Size(224, 34);
            //
            // lblAssignedToCaption
            //
            this.lblAssignedToCaption.AutoSize = false;
            this.lblAssignedToCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblAssignedToCaption.Location = new System.Drawing.Point(260, 122);
            this.lblAssignedToCaption.Name = "lblAssignedToCaption";
            this.lblAssignedToCaption.Size = new System.Drawing.Size(224, 20);
            this.lblAssignedToCaption.Text = "Assigned to";
            //
            // txtAssignedTo
            //
            this.txtAssignedTo.Location = new System.Drawing.Point(260, 144);
            this.txtAssignedTo.Name = "txtAssignedTo";
            this.txtAssignedTo.Size = new System.Drawing.Size(224, 34);
            //
            // lblStatusCaption
            //
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStatusCaption.Location = new System.Drawing.Point(24, 188);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(224, 20);
            this.lblStatusCaption.Text = "Status *";
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Items.AddRange(new object[] {
            "Open",
            "In Progress",
            "Closed"});
            this.cboStatus.Location = new System.Drawing.Point(24, 210);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(224, 34);
            //
            // lblPriorityCaption
            //
            this.lblPriorityCaption.AutoSize = false;
            this.lblPriorityCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblPriorityCaption.Location = new System.Drawing.Point(260, 188);
            this.lblPriorityCaption.Name = "lblPriorityCaption";
            this.lblPriorityCaption.Size = new System.Drawing.Size(224, 20);
            this.lblPriorityCaption.Text = "Priority *";
            //
            // cboPriority
            //
            this.cboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPriority.Items.AddRange(new object[] {
            "Low",
            "Medium",
            "High"});
            this.cboPriority.Location = new System.Drawing.Point(260, 210);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Size = new System.Drawing.Size(224, 34);
            //
            // lblDescriptionCaption
            //
            this.lblDescriptionCaption.AutoSize = false;
            this.lblDescriptionCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblDescriptionCaption.Location = new System.Drawing.Point(24, 254);
            this.lblDescriptionCaption.Name = "lblDescriptionCaption";
            this.lblDescriptionCaption.Size = new System.Drawing.Size(460, 20);
            this.lblDescriptionCaption.Text = "Description";
            //
            // txtDescription
            //
            this.txtDescription.Location = new System.Drawing.Point(24, 276);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(460, 90);
            //
            // lblValidation  (ValidateForm writes the reason here)
            //
            this.lblValidation.AutoSize = false;
            this.lblValidation.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblValidation.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblValidation.Location = new System.Drawing.Point(24, 378);
            this.lblValidation.Name = "lblValidation";
            this.lblValidation.Size = new System.Drawing.Size(460, 24);
            this.lblValidation.Text = "";
            //
            // btnSave
            //
            this.btnSave.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(258, 414);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 36);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(374, 414);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 36);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // TicketDialog
            //
            this.AcceptButton = this.btnSave;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(508, 470);
            this.Controls.Add(this.lblDialogHeading);
            this.Controls.Add(this.lblTitleCaption);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblCustomerCaption);
            this.Controls.Add(this.txtCustomer);
            this.Controls.Add(this.lblAssignedToCaption);
            this.Controls.Add(this.txtAssignedTo);
            this.Controls.Add(this.lblStatusCaption);
            this.Controls.Add(this.cboStatus);
            this.Controls.Add(this.lblPriorityCaption);
            this.Controls.Add(this.cboPriority);
            this.Controls.Add(this.lblDescriptionCaption);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblValidation);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TicketDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Ticket";
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblDialogHeading;
        private Wisej.Web.Label lblTitleCaption;
        private Wisej.Web.TextBox txtTitle;
        private Wisej.Web.Label lblCustomerCaption;
        private Wisej.Web.TextBox txtCustomer;
        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.Label lblPriorityCaption;
        private Wisej.Web.ComboBox cboPriority;
        private Wisej.Web.Label lblAssignedToCaption;
        private Wisej.Web.TextBox txtAssignedTo;
        private Wisej.Web.Label lblDescriptionCaption;
        private Wisej.Web.TextBox txtDescription;
        private Wisej.Web.Label lblValidation;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnCancel;
    }
}
