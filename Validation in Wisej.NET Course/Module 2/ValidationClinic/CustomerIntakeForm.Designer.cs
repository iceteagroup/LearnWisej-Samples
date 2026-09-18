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
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(860, 30);
            this.lblTitle.Text = "Customer intake - Module 2";
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
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
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
            this.txtEmail.Validating += new System.ComponentModel.CancelEventHandler(this.txtEmail_Validating);
            //
            // lblPhoneCaption
            //
            this.lblPhoneCaption.AutoSize = false;
            this.lblPhoneCaption.Location = new System.Drawing.Point(20, 131);
            this.lblPhoneCaption.Name = "lblPhoneCaption";
            this.lblPhoneCaption.Size = new System.Drawing.Size(150, 28);
            this.lblPhoneCaption.Text = "Phone (at least 7 digits)";
            //
            // txtPhone
            //
            this.txtPhone.AccessibleName = "Phone (at least 7 digits)";
            this.txtPhone.Location = new System.Drawing.Point(172, 131);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(250, 30);
            this.txtPhone.TabIndex = 2;
            this.txtPhone.Text = "";
            this.txtPhone.Validating += new System.ComponentModel.CancelEventHandler(this.txtPhone_Validating);
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
            this.txtCustomerCode.Validating += new System.ComponentModel.CancelEventHandler(this.txtCustomerCode_Validating);
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
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new System.Drawing.Point(130, 510);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 36);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnValid
            //
            this.btnValid.CausesValidation = false;
            this.btnValid.Location = new System.Drawing.Point(245, 510);
            this.btnValid.Name = "btnValid";
            this.btnValid.Size = new System.Drawing.Size(160, 36);
            this.btnValid.TabIndex = 6;
            this.btnValid.Text = "Load valid values";
            this.btnValid.Click += new System.EventHandler(this.btnValid_Click);
            //
            // btnInvalid
            //
            this.btnInvalid.CausesValidation = false;
            this.btnInvalid.Location = new System.Drawing.Point(420, 510);
            this.btnInvalid.Name = "btnInvalid";
            this.btnInvalid.Size = new System.Drawing.Size(165, 36);
            this.btnInvalid.TabIndex = 7;
            this.btnInvalid.Text = "Load invalid values";
            this.btnInvalid.Click += new System.EventHandler(this.btnInvalid_Click);
            //
            // btnFailure
            //
            this.btnFailure.CausesValidation = false;
            this.btnFailure.Location = new System.Drawing.Point(600, 510);
            this.btnFailure.Name = "btnFailure";
            this.btnFailure.Size = new System.Drawing.Size(150, 36);
            this.btnFailure.TabIndex = 8;
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
