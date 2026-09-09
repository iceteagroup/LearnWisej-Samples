namespace WisejTrainingApp.Views
{
    partial class CustomersView
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
            this.pnlCustomers = new Wisej.Web.Panel();
            this.labelCustomersCard = new Wisej.Web.Label();
            this.dgvCustomers = new Wisej.Web.DataGridView();
            this.lblCustomerTickets = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlAddCustomer = new Wisej.Web.Panel();
            this.labelAddCard = new Wisej.Web.Label();
            this.lblNameField = new Wisej.Web.Label();
            this.txtCustomerName = new Wisej.Web.TextBox();
            this.lblCompanyField = new Wisej.Web.Label();
            this.txtCompany = new Wisej.Web.TextBox();
            this.lblEmailField = new Wisej.Web.Label();
            this.txtEmail = new Wisej.Web.TextBox();
            this.btnAddCustomer = new Wisej.Web.Button();
            this.btnFillSampleCustomer = new Wisej.Web.Button();
            this.lblCustomerValidation = new Wisej.Web.Label();
            this.pnlCustomers.SuspendLayout();
            this.pnlAddCustomer.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlCustomers  (left card: the grid)
            //
            this.pnlCustomers.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlCustomers.BackColor = System.Drawing.Color.White;
            this.pnlCustomers.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCustomers.Controls.Add(this.labelCustomersCard);
            this.pnlCustomers.Controls.Add(this.dgvCustomers);
            this.pnlCustomers.Controls.Add(this.lblCustomerTickets);
            this.pnlCustomers.Controls.Add(this.lblStatus);
            this.pnlCustomers.Location = new System.Drawing.Point(0, 0);
            this.pnlCustomers.Name = "pnlCustomers";
            this.pnlCustomers.Size = new System.Drawing.Size(640, 532);
            //
            // labelCustomersCard
            //
            this.labelCustomersCard.AutoSize = false;
            this.labelCustomersCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelCustomersCard.Location = new System.Drawing.Point(24, 14);
            this.labelCustomersCard.Name = "labelCustomersCard";
            this.labelCustomersCard.Size = new System.Drawing.Size(592, 28);
            this.labelCustomersCard.Text = "Customers  ·  the companies the tickets name";
            //
            // dgvCustomers
            //
            this.dgvCustomers.AllowUserToAddRows = false;
            this.dgvCustomers.AllowUserToDeleteRows = false;
            this.dgvCustomers.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvCustomers.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCustomers.Location = new System.Drawing.Point(24, 56);
            this.dgvCustomers.MultiSelect = false;
            this.dgvCustomers.Name = "dgvCustomers";
            this.dgvCustomers.ReadOnly = true;
            this.dgvCustomers.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomers.Size = new System.Drawing.Size(592, 380);
            this.dgvCustomers.SelectionChanged += new System.EventHandler(this.dgvCustomers_SelectionChanged);
            //
            // lblCustomerTickets
            //
            this.lblCustomerTickets.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblCustomerTickets.AutoSize = false;
            this.lblCustomerTickets.AutoEllipsis = true;
            this.lblCustomerTickets.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCustomerTickets.Location = new System.Drawing.Point(24, 446);
            this.lblCustomerTickets.Name = "lblCustomerTickets";
            this.lblCustomerTickets.Size = new System.Drawing.Size(592, 22);
            this.lblCustomerTickets.Text = "Select a customer to see its tickets.";
            //
            // lblStatus
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(24, 476);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(592, 26);
            this.lblStatus.Text = "● ready";
            //
            // pnlAddCustomer  (right card: the inline form)
            //
            this.pnlAddCustomer.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.pnlAddCustomer.BackColor = System.Drawing.Color.White;
            this.pnlAddCustomer.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlAddCustomer.Controls.Add(this.labelAddCard);
            this.pnlAddCustomer.Controls.Add(this.lblNameField);
            this.pnlAddCustomer.Controls.Add(this.txtCustomerName);
            this.pnlAddCustomer.Controls.Add(this.lblCompanyField);
            this.pnlAddCustomer.Controls.Add(this.txtCompany);
            this.pnlAddCustomer.Controls.Add(this.lblEmailField);
            this.pnlAddCustomer.Controls.Add(this.txtEmail);
            this.pnlAddCustomer.Controls.Add(this.btnAddCustomer);
            this.pnlAddCustomer.Controls.Add(this.btnFillSampleCustomer);
            this.pnlAddCustomer.Controls.Add(this.lblCustomerValidation);
            this.pnlAddCustomer.Location = new System.Drawing.Point(656, 0);
            this.pnlAddCustomer.Name = "pnlAddCustomer";
            this.pnlAddCustomer.Size = new System.Drawing.Size(376, 532);
            //
            // labelAddCard
            //
            this.labelAddCard.AutoSize = false;
            this.labelAddCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelAddCard.Location = new System.Drawing.Point(24, 14);
            this.labelAddCard.Name = "labelAddCard";
            this.labelAddCard.Size = new System.Drawing.Size(328, 28);
            this.labelAddCard.Text = "Add customer";
            //
            // lblNameField
            //
            this.lblNameField.AutoSize = false;
            this.lblNameField.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblNameField.Location = new System.Drawing.Point(24, 56);
            this.lblNameField.Name = "lblNameField";
            this.lblNameField.Size = new System.Drawing.Size(328, 20);
            this.lblNameField.Text = "Contact name *";
            //
            // txtCustomerName
            //
            this.txtCustomerName.Location = new System.Drawing.Point(24, 78);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Size = new System.Drawing.Size(328, 34);
            this.txtCustomerName.Watermark = "Who we talk to";
            //
            // lblCompanyField
            //
            this.lblCompanyField.AutoSize = false;
            this.lblCompanyField.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblCompanyField.Location = new System.Drawing.Point(24, 122);
            this.lblCompanyField.Name = "lblCompanyField";
            this.lblCompanyField.Size = new System.Drawing.Size(328, 20);
            this.lblCompanyField.Text = "Company *  (the name tickets use)";
            //
            // txtCompany
            //
            this.txtCompany.Location = new System.Drawing.Point(24, 144);
            this.txtCompany.Name = "txtCompany";
            this.txtCompany.Size = new System.Drawing.Size(328, 34);
            this.txtCompany.Watermark = "Must be new — Northwind already exists";
            //
            // lblEmailField
            //
            this.lblEmailField.AutoSize = false;
            this.lblEmailField.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblEmailField.Location = new System.Drawing.Point(24, 188);
            this.lblEmailField.Name = "lblEmailField";
            this.lblEmailField.Size = new System.Drawing.Size(328, 20);
            this.lblEmailField.Text = "Email *";
            //
            // txtEmail
            //
            this.txtEmail.Location = new System.Drawing.Point(24, 210);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(328, 34);
            this.txtEmail.Watermark = "name@company.example";
            //
            // btnAddCustomer  (primary, left)
            //
            this.btnAddCustomer.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnAddCustomer.Location = new System.Drawing.Point(24, 260);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new System.Drawing.Size(150, 36);
            this.btnAddCustomer.Text = "Add customer";
            this.btnAddCustomer.ToolTipText = "CustomerService.ValidateCustomer → AddCustomer → grid refresh. Click with blank fields to see the failure path.";
            this.btnAddCustomer.Click += new System.EventHandler(this.btnAddCustomer_Click);
            //
            // btnFillSampleCustomer  (recovery)
            //
            this.btnFillSampleCustomer.Location = new System.Drawing.Point(182, 260);
            this.btnFillSampleCustomer.Name = "btnFillSampleCustomer";
            this.btnFillSampleCustomer.Size = new System.Drawing.Size(170, 36);
            this.btnFillSampleCustomer.Text = "Fill a valid sample";
            this.btnFillSampleCustomer.Click += new System.EventHandler(this.btnFillSampleCustomer_Click);
            //
            // lblCustomerValidation
            //
            this.lblCustomerValidation.AutoSize = false;
            this.lblCustomerValidation.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblCustomerValidation.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblCustomerValidation.Location = new System.Drawing.Point(24, 308);
            this.lblCustomerValidation.Name = "lblCustomerValidation";
            this.lblCustomerValidation.Size = new System.Drawing.Size(328, 90);
            this.lblCustomerValidation.Text = "";
            this.lblCustomerValidation.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // CustomersView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlCustomers);
            this.Controls.Add(this.pnlAddCustomer);
            this.Name = "CustomersView";
            this.Size = new System.Drawing.Size(1032, 532);
            this.pnlCustomers.ResumeLayout(false);
            this.pnlAddCustomer.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlCustomers;
        private Wisej.Web.Label labelCustomersCard;
        private Wisej.Web.DataGridView dgvCustomers;
        private Wisej.Web.Label lblCustomerTickets;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlAddCustomer;
        private Wisej.Web.Label labelAddCard;
        private Wisej.Web.Label lblNameField;
        private Wisej.Web.TextBox txtCustomerName;
        private Wisej.Web.Label lblCompanyField;
        private Wisej.Web.TextBox txtCompany;
        private Wisej.Web.Label lblEmailField;
        private Wisej.Web.TextBox txtEmail;
        private Wisej.Web.Button btnAddCustomer;
        private Wisej.Web.Button btnFillSampleCustomer;
        private Wisej.Web.Label lblCustomerValidation;
    }
}
