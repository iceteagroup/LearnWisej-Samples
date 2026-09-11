using SupportDesk.Services;

namespace SupportDesk.Web
{
    partial class TicketEditorForm
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
            this.editBindingSource = new Wisej.Web.BindingSource(this.components);
            // A Type as DataSource gives the DataBindings below a property to bind to before the real model
            // exists; LoadEditorAsync replaces it with the instance.
            this.editBindingSource.DataSource = typeof(SupportDesk.Services.TicketEditModel);
            this.errorProvider = new Wisej.Web.ErrorProvider(this.components);
            this.lblTitleCaption = new Wisej.Web.Label();
            this.txtTitle = new Wisej.Web.TextBox();
            this.lblCustomerCaption = new Wisej.Web.Label();
            this.cboCustomer = new Wisej.Web.ComboBox();
            this.lblCategoryCaption = new Wisej.Web.Label();
            this.cboCategory = new Wisej.Web.ComboBox();
            this.lblDueCaption = new Wisej.Web.Label();
            this.dtpDueDate = new Wisej.Web.DateTimePicker();
            this.chkIsUrgent = new Wisej.Web.CheckBox();
            this.btnDelete = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.btnSave = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // lblTitleCaption
            //
            this.lblTitleCaption.AutoSize = false;
            this.lblTitleCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTitleCaption.Location = new System.Drawing.Point(24, 20);
            this.lblTitleCaption.Name = "lblTitleCaption";
            this.lblTitleCaption.Size = new System.Drawing.Size(200, 18);
            this.lblTitleCaption.Text = "Title";
            //
            // txtTitle
            //
            this.txtTitle.Location = new System.Drawing.Point(24, 40);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(400, 30);
            this.txtTitle.DataBindings.Add("Text", this.editBindingSource, nameof(TicketEditModel.Title), true, Wisej.Web.DataSourceUpdateMode.OnValidation);
            //
            // lblCustomerCaption
            //
            this.lblCustomerCaption.AutoSize = false;
            this.lblCustomerCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCustomerCaption.Location = new System.Drawing.Point(24, 80);
            this.lblCustomerCaption.Name = "lblCustomerCaption";
            this.lblCustomerCaption.Size = new System.Drawing.Size(200, 18);
            this.lblCustomerCaption.Text = "Customer";
            //
            // cboCustomer
            //
            this.cboCustomer.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboCustomer.Location = new System.Drawing.Point(24, 100);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Size = new System.Drawing.Size(400, 30);
            //
            // lblCategoryCaption
            //
            this.lblCategoryCaption.AutoSize = false;
            this.lblCategoryCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCategoryCaption.Location = new System.Drawing.Point(24, 140);
            this.lblCategoryCaption.Name = "lblCategoryCaption";
            this.lblCategoryCaption.Size = new System.Drawing.Size(200, 18);
            this.lblCategoryCaption.Text = "Category";
            //
            // cboCategory
            //
            this.cboCategory.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboCategory.Location = new System.Drawing.Point(24, 160);
            this.cboCategory.Name = "cboCategory";
            this.cboCategory.Size = new System.Drawing.Size(400, 30);
            //
            // lblDueCaption
            //
            this.lblDueCaption.AutoSize = false;
            this.lblDueCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblDueCaption.Location = new System.Drawing.Point(24, 200);
            this.lblDueCaption.Name = "lblDueCaption";
            this.lblDueCaption.Size = new System.Drawing.Size(200, 18);
            this.lblDueCaption.Text = "Due date";
            //
            // dtpDueDate
            //
            this.dtpDueDate.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dtpDueDate.Location = new System.Drawing.Point(24, 220);
            this.dtpDueDate.Name = "dtpDueDate";
            this.dtpDueDate.ShowCheckBox = true;
            this.dtpDueDate.Size = new System.Drawing.Size(180, 30);
            //
            // chkIsUrgent
            //
            this.chkIsUrgent.Location = new System.Drawing.Point(24, 262);
            this.chkIsUrgent.Name = "chkIsUrgent";
            this.chkIsUrgent.Size = new System.Drawing.Size(300, 26);
            this.chkIsUrgent.Text = "Escalate as urgent";
            this.chkIsUrgent.DataBindings.Add("Checked", this.editBindingSource, nameof(TicketEditModel.IsUrgent), true, Wisej.Web.DataSourceUpdateMode.OnPropertyChanged);
            //
            // btnDelete
            //
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.btnDelete.Location = new System.Drawing.Point(24, 308);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 36);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(214, 308);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 36);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnSave
            //
            this.btnSave.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(324, 308);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 36);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // TicketEditorForm
            //
            this.AcceptButton = this.btnSave;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.lblTitleCaption);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblCustomerCaption);
            this.Controls.Add(this.cboCustomer);
            this.Controls.Add(this.lblCategoryCaption);
            this.Controls.Add(this.cboCategory);
            this.Controls.Add(this.lblDueCaption);
            this.Controls.Add(this.dtpDueDate);
            this.Controls.Add(this.chkIsUrgent);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.Fixed;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TicketEditorForm";
            this.ShowInTaskbar = false;
            this.Size = new System.Drawing.Size(464, 410);
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Add Ticket";
            this.Load += new System.EventHandler(this.TicketEditorForm_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.BindingSource editBindingSource;
        private Wisej.Web.ErrorProvider errorProvider;
        private Wisej.Web.Label lblTitleCaption;
        private Wisej.Web.TextBox txtTitle;
        private Wisej.Web.Label lblCustomerCaption;
        private Wisej.Web.ComboBox cboCustomer;
        private Wisej.Web.Label lblCategoryCaption;
        private Wisej.Web.ComboBox cboCategory;
        private Wisej.Web.Label lblDueCaption;
        private Wisej.Web.DateTimePicker dtpDueDate;
        private Wisej.Web.CheckBox chkIsUrgent;
        private Wisej.Web.Button btnDelete;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Button btnSave;
    }
}
