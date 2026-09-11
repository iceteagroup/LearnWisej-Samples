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
            this.lblStatusCaption = new Wisej.Web.Label();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.lblPriorityCaption = new Wisej.Web.Label();
            this.cboPriority = new Wisej.Web.ComboBox();
            this.lblDueCaption = new Wisej.Web.Label();
            this.dtpDueDate = new Wisej.Web.DateTimePicker();
            this.lblAgentCaption = new Wisej.Web.Label();
            this.cboAgent = new Wisej.Web.ComboBox();
            this.lblDescriptionCaption = new Wisej.Web.Label();
            this.txtDescription = new Wisej.Web.TextBox();
            this.chkIsUrgent = new Wisej.Web.CheckBox();
            this.validationSummaryLabel = new Wisej.Web.Label();
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
            this.txtTitle.Size = new System.Drawing.Size(560, 30);
            this.txtTitle.DataBindings.Add("Text", this.editBindingSource, nameof(TicketEditModel.Title), true, Wisej.Web.DataSourceUpdateMode.OnValidation);
            this.txtTitle.Validated += new System.EventHandler(this.Field_Changed);
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
            this.cboCustomer.Size = new System.Drawing.Size(272, 30);
            this.cboCustomer.SelectedValueChanged += new System.EventHandler(this.Field_Changed);
            //
            // lblCategoryCaption
            //
            this.lblCategoryCaption.AutoSize = false;
            this.lblCategoryCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCategoryCaption.Location = new System.Drawing.Point(312, 80);
            this.lblCategoryCaption.Name = "lblCategoryCaption";
            this.lblCategoryCaption.Size = new System.Drawing.Size(200, 18);
            this.lblCategoryCaption.Text = "Category";
            //
            // cboCategory
            //
            this.cboCategory.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboCategory.Location = new System.Drawing.Point(312, 100);
            this.cboCategory.Name = "cboCategory";
            this.cboCategory.Size = new System.Drawing.Size(272, 30);
            this.cboCategory.SelectedValueChanged += new System.EventHandler(this.Field_Changed);
            //
            // lblStatusCaption
            //
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStatusCaption.Location = new System.Drawing.Point(24, 140);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(200, 18);
            this.lblStatusCaption.Text = "Status";
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(24, 160);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(272, 30);
            this.cboStatus.SelectedValueChanged += new System.EventHandler(this.Field_Changed);
            //
            // lblPriorityCaption
            //
            this.lblPriorityCaption.AutoSize = false;
            this.lblPriorityCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblPriorityCaption.Location = new System.Drawing.Point(312, 140);
            this.lblPriorityCaption.Name = "lblPriorityCaption";
            this.lblPriorityCaption.Size = new System.Drawing.Size(200, 18);
            this.lblPriorityCaption.Text = "Priority";
            //
            // cboPriority
            //
            this.cboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPriority.Location = new System.Drawing.Point(312, 160);
            this.cboPriority.Name = "cboPriority";
            this.cboPriority.Size = new System.Drawing.Size(272, 30);
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
            this.dtpDueDate.ValueChanged += new System.EventHandler(this.DueDate_Changed);
            this.dtpDueDate.Validated += new System.EventHandler(this.DueDate_Changed);
            //
            // lblAgentCaption
            //
            this.lblAgentCaption.AutoSize = false;
            this.lblAgentCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblAgentCaption.Location = new System.Drawing.Point(312, 200);
            this.lblAgentCaption.Name = "lblAgentCaption";
            this.lblAgentCaption.Size = new System.Drawing.Size(200, 18);
            this.lblAgentCaption.Text = "Agent";
            //
            // cboAgent
            //
            this.cboAgent.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboAgent.Location = new System.Drawing.Point(312, 220);
            this.cboAgent.Name = "cboAgent";
            this.cboAgent.Size = new System.Drawing.Size(272, 30);
            //
            // lblDescriptionCaption
            //
            this.lblDescriptionCaption.AutoSize = false;
            this.lblDescriptionCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblDescriptionCaption.Location = new System.Drawing.Point(24, 260);
            this.lblDescriptionCaption.Name = "lblDescriptionCaption";
            this.lblDescriptionCaption.Size = new System.Drawing.Size(200, 18);
            this.lblDescriptionCaption.Text = "Description";
            //
            // txtDescription
            //
            this.txtDescription.Location = new System.Drawing.Point(24, 280);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(560, 56);
            this.txtDescription.DataBindings.Add("Text", this.editBindingSource, nameof(TicketEditModel.Description), true, Wisej.Web.DataSourceUpdateMode.OnValidation);
            this.txtDescription.Validated += new System.EventHandler(this.Field_Changed);
            //
            // chkIsUrgent
            //
            this.chkIsUrgent.Location = new System.Drawing.Point(24, 346);
            this.chkIsUrgent.Name = "chkIsUrgent";
            this.chkIsUrgent.Size = new System.Drawing.Size(300, 26);
            this.chkIsUrgent.Text = "Escalate as urgent";
            this.chkIsUrgent.DataBindings.Add("Checked", this.editBindingSource, nameof(TicketEditModel.IsUrgent), true, Wisej.Web.DataSourceUpdateMode.OnPropertyChanged);
            //
            // validationSummaryLabel
            //
            this.validationSummaryLabel.AutoSize = false;
            this.validationSummaryLabel.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.validationSummaryLabel.Location = new System.Drawing.Point(24, 380);
            this.validationSummaryLabel.Name = "validationSummaryLabel";
            this.validationSummaryLabel.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.validationSummaryLabel.Size = new System.Drawing.Size(560, 44);
            this.validationSummaryLabel.Text = "";
            this.validationSummaryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.validationSummaryLabel.Visible = false;
            //
            // btnDelete
            //
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.btnDelete.Location = new System.Drawing.Point(24, 436);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 36);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(374, 436);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 36);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnSave
            //
            this.btnSave.Enabled = false;
            this.btnSave.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(484, 436);
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
            this.Controls.Add(this.lblStatusCaption);
            this.Controls.Add(this.cboStatus);
            this.Controls.Add(this.lblPriorityCaption);
            this.Controls.Add(this.cboPriority);
            this.Controls.Add(this.lblDueCaption);
            this.Controls.Add(this.dtpDueDate);
            this.Controls.Add(this.lblAgentCaption);
            this.Controls.Add(this.cboAgent);
            this.Controls.Add(this.lblDescriptionCaption);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.chkIsUrgent);
            this.Controls.Add(this.validationSummaryLabel);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.Fixed;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TicketEditorForm";
            this.ShowInTaskbar = false;
            this.Size = new System.Drawing.Size(624, 530);
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
        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.Label lblPriorityCaption;
        private Wisej.Web.ComboBox cboPriority;
        private Wisej.Web.Label lblDueCaption;
        private Wisej.Web.DateTimePicker dtpDueDate;
        private Wisej.Web.Label lblAgentCaption;
        private Wisej.Web.ComboBox cboAgent;
        private Wisej.Web.Label lblDescriptionCaption;
        private Wisej.Web.TextBox txtDescription;
        private Wisej.Web.CheckBox chkIsUrgent;
        private Wisej.Web.Label validationSummaryLabel;
        private Wisej.Web.Button btnDelete;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Button btnSave;
    }
}
