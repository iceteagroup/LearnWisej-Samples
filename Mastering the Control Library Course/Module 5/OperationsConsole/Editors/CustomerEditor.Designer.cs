namespace OperationsConsole.Editors
{
    partial class CustomerEditor
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
            this.components = new System.ComponentModel.Container();
            this.errorProvider = new Wisej.Web.ErrorProvider(this.components);
            this.toolTip = new Wisej.Web.ToolTip(this.components);
            this.pnlCard = new Wisej.Web.Panel();
            this.lblName = new Wisej.Web.Label();
            this.txtName = new Wisej.Web.TextBox();
            this.lblEmail = new Wisej.Web.Label();
            this.txtEmail = new Wisej.Web.TextBox();
            this.lblStatus = new Wisej.Web.Label();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.lblCustomerType = new Wisej.Web.Label();
            this.cboCustomerType = new Wisej.Web.ComboBox();
            this.lblStartDate = new Wisej.Web.Label();
            this.dtpStartDate = new Wisej.Web.DateTimePicker();
            this.lblCreditLimit = new Wisej.Web.Label();
            this.numCreditLimit = new Wisej.Web.NumericUpDown();
            this.btnSave = new Wisej.Web.Button();
            this.btnReset = new Wisej.Web.Button();
            this.btnValidate = new Wisej.Web.Button();
            this.lblBusy = new Wisej.Web.Label();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            //
            // errorProvider
            //
            this.errorProvider.ContainerControl = this;
            this.errorProvider.BlinkStyle = Wisej.Web.ErrorBlinkStyle.NeverBlink;
            //
            // toolTip
            //
            this.toolTip.InitialDelay = 300;
            //
            // pnlCard
            //
            this.pnlCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlCard.BackColor = System.Drawing.Color.White;
            this.pnlCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCard.Controls.Add(this.lblName);
            this.pnlCard.Controls.Add(this.txtName);
            this.pnlCard.Controls.Add(this.lblEmail);
            this.pnlCard.Controls.Add(this.txtEmail);
            this.pnlCard.Controls.Add(this.lblStatus);
            this.pnlCard.Controls.Add(this.cboStatus);
            this.pnlCard.Controls.Add(this.lblCustomerType);
            this.pnlCard.Controls.Add(this.cboCustomerType);
            this.pnlCard.Controls.Add(this.lblStartDate);
            this.pnlCard.Controls.Add(this.dtpStartDate);
            this.pnlCard.Controls.Add(this.lblCreditLimit);
            this.pnlCard.Controls.Add(this.numCreditLimit);
            this.pnlCard.Controls.Add(this.btnSave);
            this.pnlCard.Controls.Add(this.btnReset);
            this.pnlCard.Controls.Add(this.btnValidate);
            this.pnlCard.Controls.Add(this.lblBusy);
            this.pnlCard.Location = new System.Drawing.Point(24, 20);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(692, 312);
            //
            // lblName
            //
            this.lblName.AutoSize = false;
            this.lblName.Location = new System.Drawing.Point(20, 20);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(140, 28);
            this.lblName.Text = "Customer name";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtName
            //
            this.txtName.AccessibleName = "Customer name";
            this.txtName.Location = new System.Drawing.Point(170, 20);
            this.txtName.MaxLength = 80;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(380, 28);
            this.txtName.TabIndex = 1;
            this.txtName.Watermark = "Company or contact name";
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            //
            // lblEmail
            //
            this.lblEmail.AutoSize = false;
            this.lblEmail.Location = new System.Drawing.Point(20, 58);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(140, 28);
            this.lblEmail.Text = "Email";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtEmail
            //
            this.txtEmail.AccessibleName = "Email address";
            this.txtEmail.CharacterCasing = Wisej.Web.CharacterCasing.Lower;
            this.txtEmail.Location = new System.Drawing.Point(170, 58);
            this.txtEmail.MaxLength = 120;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(380, 28);
            this.txtEmail.TabIndex = 2;
            this.txtEmail.Watermark = "name@company.com";
            this.txtEmail.Validating += new System.ComponentModel.CancelEventHandler(this.txtEmail_Validating);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Location = new System.Drawing.Point(20, 96);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(140, 28);
            this.lblStatus.Text = "Status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cboStatus
            //
            this.cboStatus.AccessibleName = "Customer status";
            this.cboStatus.DisplayMember = "Text";
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(170, 96);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(220, 28);
            this.cboStatus.TabIndex = 3;
            this.cboStatus.ValueMember = "Key";
            this.cboStatus.SelectedIndexChanged += new System.EventHandler(this.cboStatus_SelectedIndexChanged);
            //
            // lblCustomerType
            //
            this.lblCustomerType.AutoSize = false;
            this.lblCustomerType.Location = new System.Drawing.Point(20, 134);
            this.lblCustomerType.Name = "lblCustomerType";
            this.lblCustomerType.Size = new System.Drawing.Size(140, 28);
            this.lblCustomerType.Text = "Customer type";
            this.lblCustomerType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cboCustomerType
            //
            this.cboCustomerType.AccessibleName = "Customer type";
            this.cboCustomerType.DisplayMember = "Text";
            this.cboCustomerType.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboCustomerType.Location = new System.Drawing.Point(170, 134);
            this.cboCustomerType.Name = "cboCustomerType";
            this.cboCustomerType.Size = new System.Drawing.Size(220, 28);
            this.cboCustomerType.TabIndex = 4;
            this.cboCustomerType.ValueMember = "Key";
            //
            // lblStartDate
            //
            this.lblStartDate.AutoSize = false;
            this.lblStartDate.Location = new System.Drawing.Point(20, 172);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(140, 28);
            this.lblStartDate.Text = "Start date";
            this.lblStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // dtpStartDate
            //
            this.dtpStartDate.AccessibleName = "Start date";
            this.dtpStartDate.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new System.Drawing.Point(170, 172);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(180, 28);
            this.dtpStartDate.TabIndex = 5;
            this.dtpStartDate.Validating += new System.ComponentModel.CancelEventHandler(this.dtpStartDate_Validating);
            //
            // lblCreditLimit
            //
            this.lblCreditLimit.AutoSize = false;
            this.lblCreditLimit.Location = new System.Drawing.Point(20, 210);
            this.lblCreditLimit.Name = "lblCreditLimit";
            this.lblCreditLimit.Size = new System.Drawing.Size(140, 28);
            this.lblCreditLimit.Text = "Credit limit (EUR)";
            this.lblCreditLimit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // numCreditLimit
            //
            this.numCreditLimit.AccessibleName = "Credit limit in euros";
            this.numCreditLimit.DecimalPlaces = 0;
            this.numCreditLimit.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numCreditLimit.Location = new System.Drawing.Point(170, 210);
            this.numCreditLimit.Maximum = new decimal(new int[] { 250000, 0, 0, 0 });
            this.numCreditLimit.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numCreditLimit.Name = "numCreditLimit";
            this.numCreditLimit.Size = new System.Drawing.Size(180, 28);
            this.numCreditLimit.TabIndex = 6;
            this.numCreditLimit.Validating += new System.ComponentModel.CancelEventHandler(this.numCreditLimit_Validating);
            //
            // btnSave
            //
            this.btnSave.AccessibleName = "Save the customer";
            this.btnSave.Location = new System.Drawing.Point(20, 258);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 32);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnReset
            //
            this.btnReset.AccessibleName = "Reset the form to the last saved values";
            this.btnReset.CausesValidation = false;
            this.btnReset.Location = new System.Drawing.Point(140, 258);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(110, 32);
            this.btnReset.TabIndex = 8;
            this.btnReset.Text = "Reset";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            //
            // btnValidate
            //
            this.btnValidate.AccessibleName = "Validate the customer form";
            this.btnValidate.Location = new System.Drawing.Point(260, 258);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(110, 32);
            this.btnValidate.TabIndex = 9;
            this.btnValidate.Text = "Validate";
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            //
            // lblBusy
            //
            this.lblBusy.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblBusy.AutoSize = false;
            this.lblBusy.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.lblBusy.Location = new System.Drawing.Point(384, 258);
            this.lblBusy.Name = "lblBusy";
            this.lblBusy.Size = new System.Drawing.Size(286, 32);
            this.lblBusy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // toolTip
            //
            this.toolTip.SetToolTip(this.txtName, "The legal or trading name, up to 80 characters. Required.");
            this.toolTip.SetToolTip(this.txtEmail, "One address for order confirmations, e.g. name@company.com. Stored in lower case.");
            this.toolTip.SetToolTip(this.cboStatus, "Prospect, Active, On hold or Closed. Active customers must have a credit limit of at least 1,000 and a start date that is not in the future.");
            this.toolTip.SetToolTip(this.cboCustomerType, "Direct, Reseller, OEM partner or Government.");
            this.toolTip.SetToolTip(this.dtpStartDate, "The day the contract starts, between 01/01/2000 and one year from today.");
            this.toolTip.SetToolTip(this.numCreditLimit, "0 to 250,000 EUR, in steps of 1,000. A prospect may not exceed 5,000.");
            //
            // CustomerEditor
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlCard);
            this.Name = "CustomerEditor";
            this.Size = new System.Drawing.Size(740, 352);
            this.pnlCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.ErrorProvider errorProvider;
        private Wisej.Web.ToolTip toolTip;
        private Wisej.Web.Panel pnlCard;
        private Wisej.Web.Label lblName;
        private Wisej.Web.TextBox txtName;
        private Wisej.Web.Label lblEmail;
        private Wisej.Web.TextBox txtEmail;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.Label lblCustomerType;
        private Wisej.Web.ComboBox cboCustomerType;
        private Wisej.Web.Label lblStartDate;
        private Wisej.Web.DateTimePicker dtpStartDate;
        private Wisej.Web.Label lblCreditLimit;
        private Wisej.Web.NumericUpDown numCreditLimit;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnReset;
        private Wisej.Web.Button btnValidate;
        private Wisej.Web.Label lblBusy;
    }
}
