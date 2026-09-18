namespace ValidationClinic
{
    partial class CustomerIntakeForm
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
            this.errorProvider = new Wisej.Web.ErrorProvider(this.components);
            this.validation = new Wisej.Web.Validation(this.components);
            this.contactBindingSource = new Wisej.Web.BindingSource(this.components);
            this.lblTitle = new Wisej.Web.Label();
            this.lblHint = new Wisej.Web.Label();
            this.lblNameCaption = new Wisej.Web.Label();
            this.txtName = new Wisej.Web.TextBox();
            this.lblEmailCaption = new Wisej.Web.Label();
            this.txtEmail = new Wisej.Web.TextBox();
            this.lblPhoneCaption = new Wisej.Web.Label();
            this.txtPhone = new Wisej.Web.TextBox();
            this.lblCustomerCodeCaption = new Wisej.Web.Label();
            this.txtCustomerCode = new Wisej.Web.TextBox();
            this.lblAgeCaption = new Wisej.Web.Label();
            this.txtAge = new Wisej.Web.TextBox();
            this.lblCreditLimitCaption = new Wisej.Web.Label();
            this.txtCreditLimit = new Wisej.Web.TextBox();
            this.lblBirthDateCaption = new Wisej.Web.Label();
            this.dtpBirthDate = new Wisej.Web.DateTimePicker();
            this.lblStartCaption = new Wisej.Web.Label();
            this.dtpStart = new Wisej.Web.DateTimePicker();
            this.lblEndCaption = new Wisej.Web.Label();
            this.dtpEnd = new Wisej.Web.DateTimePicker();
            this.validationSummaryLabel = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.btnSave = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.btnValid = new Wisej.Web.Button();
            this.btnInvalid = new Wisej.Web.Button();
            this.btnFailure = new Wisej.Web.Button();
            this.panelSummary = new Wisej.Web.Panel();
            this.panelSummary.SuspendLayout();
            this.SuspendLayout();
            //
            // errorProvider
            //
            this.errorProvider.BlinkStyle = Wisej.Web.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            //
            // validation
            //
            this.validation.ContainerControl = this;
            //
            // contactBindingSource
            //
            this.contactBindingSource.DataSource = typeof(ValidationClinic.Models.ContactEditModel);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(860, 30);
            this.lblTitle.Text = "Customer intake - Module 6";
            //
            // lblHint
            //
            this.lblHint.AutoSize = false;
            this.lblHint.Location = new System.Drawing.Point(20, 47);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(860, 25);
            this.lblHint.Text = "Correct the fields, then Save. Cancel always leaves without writing.";
            //
            // lblNameCaption
            //
            this.lblNameCaption.AutoSize = false;
            this.lblNameCaption.Location = new System.Drawing.Point(20, 82);
            this.lblNameCaption.Name = "lblNameCaption";
            this.lblNameCaption.Size = new System.Drawing.Size(150, 28);
            this.lblNameCaption.Text = "Name";
            //
            // txtName
            //
            this.txtName.AccessibleName = "Name";
            this.txtName.Location = new System.Drawing.Point(172, 82);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(250, 30);
            this.txtName.TabIndex = 0;
            this.txtName.Text = "";
            this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
            //
            // lblEmailCaption
            //
            this.lblEmailCaption.AutoSize = false;
            this.lblEmailCaption.Location = new System.Drawing.Point(460, 82);
            this.lblEmailCaption.Name = "lblEmailCaption";
            this.lblEmailCaption.Size = new System.Drawing.Size(150, 28);
            this.lblEmailCaption.Text = "Email";
            //
            // txtEmail
            //
            this.txtEmail.AccessibleName = "Email";
            this.txtEmail.Location = new System.Drawing.Point(612, 82);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(250, 30);
            this.txtEmail.TabIndex = 1;
            this.txtEmail.Text = "";
            //
            // lblPhoneCaption
            //
            this.lblPhoneCaption.AutoSize = false;
            this.lblPhoneCaption.Location = new System.Drawing.Point(20, 131);
            this.lblPhoneCaption.Name = "lblPhoneCaption";
            this.lblPhoneCaption.Size = new System.Drawing.Size(150, 28);
            this.lblPhoneCaption.Text = "Phone (10 digits)";
            //
            // txtPhone
            //
            this.txtPhone.AccessibleName = "Phone (10 digits)";
            this.txtPhone.Location = new System.Drawing.Point(172, 131);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(250, 30);
            this.txtPhone.TabIndex = 2;
            this.txtPhone.Text = "";
            //
            // lblCustomerCodeCaption
            //
            this.lblCustomerCodeCaption.AutoSize = false;
            this.lblCustomerCodeCaption.Location = new System.Drawing.Point(460, 131);
            this.lblCustomerCodeCaption.Name = "lblCustomerCodeCaption";
            this.lblCustomerCodeCaption.Size = new System.Drawing.Size(150, 28);
            this.lblCustomerCodeCaption.Text = "Customer code";
            //
            // txtCustomerCode
            //
            this.txtCustomerCode.AccessibleName = "Customer code";
            this.txtCustomerCode.Location = new System.Drawing.Point(612, 131);
            this.txtCustomerCode.Name = "txtCustomerCode";
            this.txtCustomerCode.Size = new System.Drawing.Size(250, 30);
            this.txtCustomerCode.TabIndex = 3;
            this.txtCustomerCode.Text = "";
            //
            // lblAgeCaption
            //
            this.lblAgeCaption.AutoSize = false;
            this.lblAgeCaption.Location = new System.Drawing.Point(20, 180);
            this.lblAgeCaption.Name = "lblAgeCaption";
            this.lblAgeCaption.Size = new System.Drawing.Size(150, 28);
            this.lblAgeCaption.Text = "Age";
            //
            // txtAge
            //
            this.txtAge.AccessibleName = "Age";
            this.txtAge.Location = new System.Drawing.Point(172, 180);
            this.txtAge.Name = "txtAge";
            this.txtAge.Size = new System.Drawing.Size(250, 30);
            this.txtAge.TabIndex = 4;
            this.txtAge.Text = "";
            //
            // lblCreditLimitCaption
            //
            this.lblCreditLimitCaption.AutoSize = false;
            this.lblCreditLimitCaption.Location = new System.Drawing.Point(460, 180);
            this.lblCreditLimitCaption.Name = "lblCreditLimitCaption";
            this.lblCreditLimitCaption.Size = new System.Drawing.Size(150, 28);
            this.lblCreditLimitCaption.Text = "Credit limit (optional)";
            //
            // txtCreditLimit
            //
            this.txtCreditLimit.AccessibleName = "Credit limit (optional)";
            this.txtCreditLimit.Location = new System.Drawing.Point(612, 180);
            this.txtCreditLimit.Name = "txtCreditLimit";
            this.txtCreditLimit.Size = new System.Drawing.Size(250, 30);
            this.txtCreditLimit.TabIndex = 5;
            this.txtCreditLimit.Text = "";
            //
            // lblBirthDateCaption
            //
            this.lblBirthDateCaption.AutoSize = false;
            this.lblBirthDateCaption.Location = new System.Drawing.Point(20, 229);
            this.lblBirthDateCaption.Name = "lblBirthDateCaption";
            this.lblBirthDateCaption.Size = new System.Drawing.Size(150, 28);
            this.lblBirthDateCaption.Text = "Birth date";
            //
            // dtpBirthDate
            //
            this.dtpBirthDate.AccessibleName = "Birth date";
            this.dtpBirthDate.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpBirthDate.Location = new System.Drawing.Point(172, 229);
            this.dtpBirthDate.Name = "dtpBirthDate";
            this.dtpBirthDate.Size = new System.Drawing.Size(250, 30);
            this.dtpBirthDate.TabIndex = 6;
            //
            // lblStartCaption
            //
            this.lblStartCaption.AutoSize = false;
            this.lblStartCaption.Location = new System.Drawing.Point(460, 229);
            this.lblStartCaption.Name = "lblStartCaption";
            this.lblStartCaption.Size = new System.Drawing.Size(150, 28);
            this.lblStartCaption.Text = "Start date";
            //
            // dtpStart
            //
            this.dtpStart.AccessibleName = "Start date";
            this.dtpStart.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpStart.Location = new System.Drawing.Point(612, 229);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.Size = new System.Drawing.Size(250, 30);
            this.dtpStart.TabIndex = 7;
            this.dtpStart.ValueChanged += new System.EventHandler(this.Dates_ValueChanged);
            //
            // lblEndCaption
            //
            this.lblEndCaption.AutoSize = false;
            this.lblEndCaption.Location = new System.Drawing.Point(20, 278);
            this.lblEndCaption.Name = "lblEndCaption";
            this.lblEndCaption.Size = new System.Drawing.Size(150, 28);
            this.lblEndCaption.Text = "End date";
            //
            // dtpEnd
            //
            this.dtpEnd.AccessibleName = "End date";
            this.dtpEnd.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpEnd.Location = new System.Drawing.Point(172, 278);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(250, 30);
            this.dtpEnd.TabIndex = 8;
            this.dtpEnd.ValueChanged += new System.EventHandler(this.Dates_ValueChanged);
            //
            // validationSummaryLabel
            //
            this.validationSummaryLabel.AutoSize = true;
            this.validationSummaryLabel.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.validationSummaryLabel.Location = new System.Drawing.Point(0, 0);
            this.validationSummaryLabel.MaximumSize = new System.Drawing.Size(825, 0);
            this.validationSummaryLabel.Name = "validationSummaryLabel";
            this.validationSummaryLabel.Size = new System.Drawing.Size(860, 115);
            this.validationSummaryLabel.Text = "";
            this.validationSummaryLabel.Visible = false;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Location = new System.Drawing.Point(20, 455);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(860, 38);
            this.lblStatus.Text = "Nothing saved yet.";
            //
            // btnSave
            //
            this.btnSave.CausesValidation = true;
            this.btnSave.Location = new System.Drawing.Point(20, 510);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 36);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new System.Drawing.Point(130, 510);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 36);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnValid
            //
            this.btnValid.CausesValidation = false;
            this.btnValid.Location = new System.Drawing.Point(245, 510);
            this.btnValid.Name = "btnValid";
            this.btnValid.Size = new System.Drawing.Size(160, 36);
            this.btnValid.TabIndex = 11;
            this.btnValid.Text = "Load valid values";
            this.btnValid.Click += new System.EventHandler(this.btnValid_Click);
            //
            // btnInvalid
            //
            this.btnInvalid.CausesValidation = false;
            this.btnInvalid.Location = new System.Drawing.Point(420, 510);
            this.btnInvalid.Name = "btnInvalid";
            this.btnInvalid.Size = new System.Drawing.Size(165, 36);
            this.btnInvalid.TabIndex = 12;
            this.btnInvalid.Text = "Load invalid values";
            this.btnInvalid.Click += new System.EventHandler(this.btnInvalid_Click);
            //
            // btnFailure
            //
            this.btnFailure.CausesValidation = false;
            this.btnFailure.Location = new System.Drawing.Point(600, 510);
            this.btnFailure.Name = "btnFailure";
            this.btnFailure.Size = new System.Drawing.Size(150, 36);
            this.btnFailure.TabIndex = 13;
            this.btnFailure.Text = "Fail next write";
            this.btnFailure.Click += new System.EventHandler(this.btnFailure_Click);
            //
            // panelSummary
            //
            this.panelSummary.AutoScroll = true;
            this.panelSummary.Location = new System.Drawing.Point(20, 335);
            this.panelSummary.Name = "panelSummary";
            this.panelSummary.Size = new System.Drawing.Size(860, 115);
            this.panelSummary.Controls.Add(this.validationSummaryLabel);
            //
            // CustomerIntakeForm
            //
            this.AcceptButton = this.btnSave;
            this.AutoScroll = true;
            this.AutoValidate = Wisej.Web.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.CancelButton = this.btnCancel;
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.Fixed;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblHint);
            this.Controls.Add(this.lblNameCaption);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblEmailCaption);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblPhoneCaption);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.lblCustomerCodeCaption);
            this.Controls.Add(this.txtCustomerCode);
            this.Controls.Add(this.lblAgeCaption);
            this.Controls.Add(this.txtAge);
            this.Controls.Add(this.lblCreditLimitCaption);
            this.Controls.Add(this.txtCreditLimit);
            this.Controls.Add(this.lblBirthDateCaption);
            this.Controls.Add(this.dtpBirthDate);
            this.Controls.Add(this.lblStartCaption);
            this.Controls.Add(this.dtpStart);
            this.Controls.Add(this.lblEndCaption);
            this.Controls.Add(this.dtpEnd);
            this.Controls.Add(this.panelSummary);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnValid);
            this.Controls.Add(this.btnInvalid);
            this.Controls.Add(this.btnFailure);
            this.Name = "CustomerIntakeForm";
            this.ShowInTaskbar = false;
            this.Size = new System.Drawing.Size(920, 650);
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "ValidationClinic - customer intake";
            this.panelSummary.ResumeLayout(false);
            this.panelSummary.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.ErrorProvider errorProvider;
        private Wisej.Web.Validation validation;
        private Wisej.Web.BindingSource contactBindingSource;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblHint;
        private Wisej.Web.Label lblNameCaption;
        private Wisej.Web.TextBox txtName;
        private Wisej.Web.Label lblEmailCaption;
        private Wisej.Web.TextBox txtEmail;
        private Wisej.Web.Label lblPhoneCaption;
        private Wisej.Web.TextBox txtPhone;
        private Wisej.Web.Label lblCustomerCodeCaption;
        private Wisej.Web.TextBox txtCustomerCode;
        private Wisej.Web.Label lblAgeCaption;
        private Wisej.Web.TextBox txtAge;
        private Wisej.Web.Label lblCreditLimitCaption;
        private Wisej.Web.TextBox txtCreditLimit;
        private Wisej.Web.Label lblBirthDateCaption;
        private Wisej.Web.DateTimePicker dtpBirthDate;
        private Wisej.Web.Label lblStartCaption;
        private Wisej.Web.DateTimePicker dtpStart;
        private Wisej.Web.Label lblEndCaption;
        private Wisej.Web.DateTimePicker dtpEnd;
        private Wisej.Web.Label validationSummaryLabel;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Button btnValid;
        private Wisej.Web.Button btnInvalid;
        private Wisej.Web.Button btnFailure;
        private Wisej.Web.Panel panelSummary;
    }
}
