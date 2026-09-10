namespace OperationsConsole.Editors
{
    partial class CustomerEditor
    {
        /// <summary>Holds the three extender components (ErrorProvider, ToolTip, HelpTip).</summary>
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
            this.helpTip = new Wisej.Web.HelpTip(this.components);
            this.pnlCard = new Wisej.Web.Panel();
            this.lblCardTitle = new Wisej.Web.Label();
            this.lblCardHint = new Wisej.Web.Label();
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
            this.lblRecordId = new Wisej.Web.Label();
            this.lblSavedStamp = new Wisej.Web.Label();
            this.btnValidate = new Wisej.Web.Button();
            this.btnReset = new Wisej.Web.Button();
            this.btnSave = new Wisej.Web.Button();
            this.lblBusy = new Wisej.Web.Label();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            //
            // errorProvider  (ONE per editor: the field-error channel, never used for guidance)
            //
            this.errorProvider.ContainerControl = this;
            this.errorProvider.BlinkStyle = Wisej.Web.ErrorBlinkStyle.NeverBlink;
            //
            // toolTip  (guidance channel: what the field expects — a separate extender from errorProvider)
            //
            this.toolTip.InitialDelay = 300;
            //
            // helpTip  (the persistent "?" style hint on the two commands that change state)
            //
            this.helpTip.InitialDelay = 300;
            //
            // pnlCard  (white card on the grey page)
            //
            this.pnlCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right | Wisej.Web.AnchorStyles.Bottom;
            this.pnlCard.BackColor = System.Drawing.Color.White;
            this.pnlCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCard.Controls.Add(this.lblCardTitle);
            this.pnlCard.Controls.Add(this.lblCardHint);
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
            this.pnlCard.Controls.Add(this.lblRecordId);
            this.pnlCard.Controls.Add(this.lblSavedStamp);
            this.pnlCard.Controls.Add(this.btnValidate);
            this.pnlCard.Controls.Add(this.btnReset);
            this.pnlCard.Controls.Add(this.btnSave);
            this.pnlCard.Controls.Add(this.lblBusy);
            this.pnlCard.Location = new System.Drawing.Point(24, 12);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(692, 452);
            //
            // lblCardTitle
            //
            this.lblCardTitle.AutoSize = false;
            this.lblCardTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblCardTitle.Location = new System.Drawing.Point(20, 12);
            this.lblCardTitle.Name = "lblCardTitle";
            this.lblCardTitle.Size = new System.Drawing.Size(420, 26);
            this.lblCardTitle.Text = "Customer editor";
            this.lblCardTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblCardHint
            //
            this.lblCardHint.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblCardHint.AutoSize = false;
            this.lblCardHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCardHint.Location = new System.Drawing.Point(20, 38);
            this.lblCardHint.Name = "lblCardHint";
            this.lblCardHint.Size = new System.Drawing.Size(650, 36);
            this.lblCardHint.Text = "Each value gets the editor that already excludes impossible input. Hover a field for guidance (ToolTip); a red mark beside a field is a validation error (ErrorProvider).";
            this.lblCardHint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // lblName  (every editable field has a visible label — design rule 1)
            //
            this.lblName.AutoSize = false;
            this.lblName.Location = new System.Drawing.Point(20, 80);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(140, 28);
            this.lblName.Text = "Customer name";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtName  (ordinary text → TextBox; MaxLength stops an impossible length at the source)
            //
            this.txtName.AccessibleName = "Customer name";
            this.txtName.Location = new System.Drawing.Point(170, 80);
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
            this.lblEmail.Location = new System.Drawing.Point(20, 118);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(140, 28);
            this.lblEmail.Text = "Email";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtEmail  (CharacterCasing.Lower normalises the value while the user types)
            //
            this.txtEmail.AccessibleName = "Email address";
            this.txtEmail.CharacterCasing = Wisej.Web.CharacterCasing.Lower;
            this.txtEmail.Location = new System.Drawing.Point(170, 118);
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
            this.lblStatus.Location = new System.Drawing.Point(20, 156);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(140, 28);
            this.lblStatus.Text = "Status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cboStatus  (list value → ComboBox; DropDownList means the user cannot type a value that is not in the list.
            //             DisplayMember/ValueMember keep "Active" (text) apart from "ACT" (stored key).)
            //
            this.cboStatus.AccessibleName = "Customer status";
            this.cboStatus.DisplayMember = "Text";
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(170, 156);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(220, 28);
            this.cboStatus.TabIndex = 3;
            this.cboStatus.ValueMember = "Key";
            this.cboStatus.SelectedIndexChanged += new System.EventHandler(this.cboStatus_SelectedIndexChanged);
            //
            // lblCustomerType
            //
            this.lblCustomerType.AutoSize = false;
            this.lblCustomerType.Location = new System.Drawing.Point(20, 194);
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
            this.cboCustomerType.Location = new System.Drawing.Point(170, 194);
            this.cboCustomerType.Name = "cboCustomerType";
            this.cboCustomerType.Size = new System.Drawing.Size(220, 28);
            this.cboCustomerType.TabIndex = 4;
            this.cboCustomerType.ValueMember = "Key";
            //
            // lblStartDate
            //
            this.lblStartDate.AutoSize = false;
            this.lblStartDate.Location = new System.Drawing.Point(20, 232);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(140, 28);
            this.lblStartDate.Text = "Start date";
            this.lblStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // dtpStartDate  (a date → DateTimePicker: "03/04/2026" is never parsed from text.
            //                MinDate/MaxDate make an out-of-era date impossible; the "not in the future
            //                for an Active customer" rule still needs a validator — MaxDate allows +1 year.)
            //
            this.dtpStartDate.AccessibleName = "Start date";
            this.dtpStartDate.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new System.Drawing.Point(170, 232);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(180, 28);
            this.dtpStartDate.TabIndex = 5;
            this.dtpStartDate.Validating += new System.ComponentModel.CancelEventHandler(this.dtpStartDate_Validating);
            //
            // lblCreditLimit
            //
            this.lblCreditLimit.AutoSize = false;
            this.lblCreditLimit.Location = new System.Drawing.Point(20, 270);
            this.lblCreditLimit.Name = "lblCreditLimit";
            this.lblCreditLimit.Size = new System.Drawing.Size(140, 28);
            this.lblCreditLimit.Text = "Credit limit (EUR)";
            this.lblCreditLimit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // numCreditLimit  (a bounded number → NumericUpDown: "1.500" is never parsed from text,
            //                  and Minimum/Maximum/Increment express the range in the control itself)
            //
            this.numCreditLimit.AccessibleName = "Credit limit in euros";
            this.numCreditLimit.DecimalPlaces = 0;
            this.numCreditLimit.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numCreditLimit.Location = new System.Drawing.Point(170, 270);
            this.numCreditLimit.Maximum = new decimal(new int[] { 250000, 0, 0, 0 });
            this.numCreditLimit.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numCreditLimit.Name = "numCreditLimit";
            this.numCreditLimit.Size = new System.Drawing.Size(180, 28);
            this.numCreditLimit.TabIndex = 6;
            this.numCreditLimit.Validating += new System.ComponentModel.CancelEventHandler(this.numCreditLimit_Validating);
            //
            // lblRecordId  (state refresh after a save: the id the service assigned)
            //
            this.lblRecordId.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblRecordId.AutoSize = false;
            this.lblRecordId.Font = new System.Drawing.Font("monospace", 9F);
            this.lblRecordId.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblRecordId.Location = new System.Drawing.Point(20, 314);
            this.lblRecordId.Name = "lblRecordId";
            this.lblRecordId.Size = new System.Drawing.Size(650, 22);
            this.lblRecordId.Text = "Record: — (new customer, not saved yet)";
            this.lblRecordId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSavedStamp
            //
            this.lblSavedStamp.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblSavedStamp.AutoSize = false;
            this.lblSavedStamp.Font = new System.Drawing.Font("monospace", 9F);
            this.lblSavedStamp.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSavedStamp.Location = new System.Drawing.Point(20, 336);
            this.lblSavedStamp.Name = "lblSavedStamp";
            this.lblSavedStamp.Size = new System.Drawing.Size(650, 22);
            this.lblSavedStamp.Text = "Last saved: —";
            this.lblSavedStamp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnValidate  (runs ValidateContent() only — no service call)
            //
            this.btnValidate.AccessibleName = "Validate the customer form";
            this.btnValidate.Location = new System.Drawing.Point(20, 372);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(110, 32);
            this.btnValidate.TabIndex = 7;
            this.btnValidate.Text = "Validate";
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            //
            // btnReset  (CausesValidation = false: leaving a half-typed field to press Reset must not
            //            re-validate that field on the way out)
            //
            this.btnReset.AccessibleName = "Reset the form to the last saved values";
            this.btnReset.CausesValidation = false;
            this.btnReset.Location = new System.Drawing.Point(140, 372);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(110, 32);
            this.btnReset.TabIndex = 8;
            this.btnReset.Text = "Reset";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            //
            // btnSave  (the primary command: validate → persist → refresh state → notify)
            //
            this.btnSave.AccessibleName = "Save the customer";
            this.btnSave.Location = new System.Drawing.Point(260, 372);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(130, 32);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // lblBusy  (plain-text mirror of the busy state, next to the disabled buttons)
            //
            this.lblBusy.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblBusy.AutoSize = false;
            this.lblBusy.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblBusy.Location = new System.Drawing.Point(404, 372);
            this.lblBusy.Name = "lblBusy";
            this.lblBusy.Size = new System.Drawing.Size(266, 32);
            this.lblBusy.Text = "Idle — the three commands are enabled.";
            this.lblBusy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ToolTip / HelpTip: the guidance channel. One sentence per editor saying what it expects —
            // never an error message; errors belong to errorProvider.
            //
            this.toolTip.SetToolTip(this.txtName, "The legal or trading name, up to 80 characters. Required.");
            this.toolTip.SetToolTip(this.txtEmail, "One address for order confirmations, e.g. name@company.com. Stored in lower case.");
            this.toolTip.SetToolTip(this.cboStatus, "Prospect, Active, On hold or Closed. Active customers must have a credit limit of at least 1,000 and a start date that is not in the future.");
            this.toolTip.SetToolTip(this.cboCustomerType, "Direct, Reseller, OEM partner or Government — it drives the price list, not the credit rules.");
            this.toolTip.SetToolTip(this.dtpStartDate, "The day the contract starts. Pick a date between 01/01/2000 and one year from today.");
            this.toolTip.SetToolTip(this.numCreditLimit, "0 to 250,000 EUR, in steps of 1,000. A prospect may not exceed 5,000.");
            this.helpTip.SetHelpTip(this.btnSave, "Validates the form, calls the customer service, refreshes the record state and reports the outcome without blocking you.");
            this.helpTip.SetHelpTip(this.btnReset, "Puts the last saved values back and clears the error marks. Asks first when there are unsaved changes.");
            //
            // CustomerEditor
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlCard);
            this.Name = "CustomerEditor";
            this.Size = new System.Drawing.Size(740, 480);
            this.pnlCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // extenders (components — no visual surface of their own)
        private Wisej.Web.ErrorProvider errorProvider;
        private Wisej.Web.ToolTip toolTip;
        private Wisej.Web.HelpTip helpTip;

        // the card and its content — all private: the host page uses the public surface, never the children
        private Wisej.Web.Panel pnlCard;
        private Wisej.Web.Label lblCardTitle;
        private Wisej.Web.Label lblCardHint;
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
        private Wisej.Web.Label lblRecordId;
        private Wisej.Web.Label lblSavedStamp;
        private Wisej.Web.Button btnValidate;
        private Wisej.Web.Button btnReset;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Label lblBusy;
    }
}
