namespace ValidationClinic
{
    partial class DataBoundErrorsForm
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
            this.contactBindingSource = new Wisej.Web.BindingSource(this.components);
            this.errorProvider = new Wisej.Web.ErrorProvider(this.components);
            this.lblTitle = new Wisej.Web.Label();
            this.txtName = new Wisej.Web.TextBox();
            this.txtEmail = new Wisej.Web.TextBox();
            this.lblSummary = new Wisej.Web.Label();
            this.btnCheck = new Wisej.Web.Button();
            this.btnSwitch = new Wisej.Web.Button();
            this.btnClose = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // contactBindingSource
            //
            this.contactBindingSource.DataSource = typeof(ValidationClinic.Models.ContactErrorModel);
            //
            // errorProvider
            //
            this.errorProvider.BlinkStyle = Wisej.Web.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(850, 40);
            this.lblTitle.Text = "IDataErrorInfo - errors supplied by the bound model";
            //
            // txtName
            //
            this.txtName.Location = new System.Drawing.Point(20, 80);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(340, 35);
            this.txtName.TabIndex = 0;
            this.txtName.Text = "";
            //
            // txtEmail
            //
            this.txtEmail.Location = new System.Drawing.Point(390, 80);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(340, 35);
            this.txtEmail.TabIndex = 1;
            this.txtEmail.Text = "";
            //
            // lblSummary
            //
            this.lblSummary.AutoSize = false;
            this.lblSummary.Location = new System.Drawing.Point(20, 140);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(850, 100);
            this.lblSummary.Text = "";
            //
            // btnCheck
            //
            this.btnCheck.CausesValidation = false;
            this.btnCheck.Location = new System.Drawing.Point(20, 260);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(170, 35);
            this.btnCheck.TabIndex = 2;
            this.btnCheck.Text = "Check model";
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            //
            // btnSwitch
            //
            this.btnSwitch.CausesValidation = false;
            this.btnSwitch.Location = new System.Drawing.Point(210, 260);
            this.btnSwitch.Name = "btnSwitch";
            this.btnSwitch.Size = new System.Drawing.Size(170, 35);
            this.btnSwitch.TabIndex = 3;
            this.btnSwitch.Text = "Switch source";
            this.btnSwitch.Click += new System.EventHandler(this.btnSwitch_Click);
            //
            // btnClose
            //
            this.btnClose.CausesValidation = false;
            this.btnClose.Location = new System.Drawing.Point(400, 260);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(130, 35);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // DataBoundErrorsForm
            //
            this.AutoScroll = true;
            this.AutoValidate = Wisej.Web.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblSummary);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.btnSwitch);
            this.Controls.Add(this.btnClose);
            this.Name = "DataBoundErrorsForm";
            this.Size = new System.Drawing.Size(920, 650);
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "Data-bound ErrorProvider";
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.BindingSource contactBindingSource;
        private Wisej.Web.ErrorProvider errorProvider;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.TextBox txtName;
        private Wisej.Web.TextBox txtEmail;
        private Wisej.Web.Label lblSummary;
        private Wisej.Web.Button btnCheck;
        private Wisej.Web.Button btnSwitch;
        private Wisej.Web.Button btnClose;
    }
}
