namespace ValidationClinic
{
    partial class MainPage
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
            this.gridSource = new Wisej.Web.BindingSource(this.components);
            this.lblTitle = new Wisej.Web.Label();
            this.lblHint = new Wisej.Web.Label();
            this.gridContacts = new Wisej.Web.DataGridView();
            this.lblTrace = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.btnIntake = new Wisej.Web.Button();
            this.btnReload = new Wisej.Web.Button();
            this.btnFailure = new Wisej.Web.Button();
            this.lblStatus = new Wisej.Web.Label();
            this.NameColumn = new Wisej.Web.DataGridViewTextBoxColumn();
            this.EmailColumn = new Wisej.Web.DataGridViewTextBoxColumn();
            this.AgeColumn = new Wisej.Web.DataGridViewTextBoxColumn();
            this.StatusColumn = new Wisej.Web.DataGridViewComboBoxColumn();
            this.ClosedDateColumn = new Wisej.Web.DataGridViewTextBoxColumn();
            this.SuspendLayout();
            //
            // gridSource
            //
            this.gridSource.DataSource = typeof(ValidationClinic.Models.ContactEditModel);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1280, 34);
            this.lblTitle.Text = "ValidationClinic | Module 4 - Custom Validation Methods, Services and Rules";
            //
            // lblHint
            //
            this.lblHint.AutoSize = false;
            this.lblHint.Location = new System.Drawing.Point(20, 55);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(1270, 30);
            this.lblHint.Text = "Edit working copies below. Repository writes occur only after Save. Open intake to follow the form validation layers.";
            //
            // gridContacts
            //
            this.gridContacts.AllowUserToAddRows = true;
            this.gridContacts.AutoGenerateColumns = false;
            this.gridContacts.DataSource = this.gridSource;
            this.gridContacts.Location = new System.Drawing.Point(20, 142);
            this.gridContacts.Name = "gridContacts";
            this.gridContacts.Size = new System.Drawing.Size(835, 430);
            this.gridContacts.TabIndex = 0;
            this.gridContacts.Columns.Add(this.NameColumn);
            this.gridContacts.Columns.Add(this.EmailColumn);
            this.gridContacts.Columns.Add(this.AgeColumn);
            this.gridContacts.Columns.Add(this.StatusColumn);
            this.gridContacts.Columns.Add(this.ClosedDateColumn);
            //
            // lblTrace
            //
            this.lblTrace.AutoSize = false;
            this.lblTrace.Location = new System.Drawing.Point(880, 96);
            this.lblTrace.Name = "lblTrace";
            this.lblTrace.Size = new System.Drawing.Size(440, 28);
            this.lblTrace.Text = "Validation pipeline - live server trace";
            //
            // listTrace
            //
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(880, 130);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(440, 480);
            this.listTrace.TabIndex = 1;
            //
            // btnIntake
            //
            this.btnIntake.CausesValidation = false;
            this.btnIntake.Location = new System.Drawing.Point(20, 96);
            this.btnIntake.Name = "btnIntake";
            this.btnIntake.Size = new System.Drawing.Size(190, 34);
            this.btnIntake.TabIndex = 2;
            this.btnIntake.Text = "Open customer intake";
            this.btnIntake.Click += new System.EventHandler(this.btnIntake_Click);
            //
            // btnReload
            //
            this.btnReload.CausesValidation = false;
            this.btnReload.Location = new System.Drawing.Point(225, 96);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(165, 34);
            this.btnReload.TabIndex = 3;
            this.btnReload.Text = "Discard grid edits";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            //
            // btnFailure
            //
            this.btnFailure.CausesValidation = false;
            this.btnFailure.Location = new System.Drawing.Point(405, 96);
            this.btnFailure.Name = "btnFailure";
            this.btnFailure.Size = new System.Drawing.Size(155, 34);
            this.btnFailure.TabIndex = 4;
            this.btnFailure.Text = "Fail next write";
            this.btnFailure.Click += new System.EventHandler(this.btnFailure_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Location = new System.Drawing.Point(20, 580);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(835, 55);
            this.lblStatus.Text = "Ready. Saved contacts: 2. Writes: 0.";
            //
            // NameColumn
            //
            this.NameColumn.DataPropertyName = "Name";
            this.NameColumn.HeaderText = "Name";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.Width = 165;
            //
            // EmailColumn
            //
            this.EmailColumn.DataPropertyName = "Email";
            this.EmailColumn.HeaderText = "Email";
            this.EmailColumn.Name = "EmailColumn";
            this.EmailColumn.Width = 210;
            //
            // AgeColumn
            //
            this.AgeColumn.DataPropertyName = "Age";
            this.AgeColumn.HeaderText = "Age";
            this.AgeColumn.Name = "AgeColumn";
            this.AgeColumn.Width = 65;
            //
            // StatusColumn
            //
            this.StatusColumn.DataPropertyName = "Status";
            this.StatusColumn.DataSource = new string[] { "Open", "Closed" };
            this.StatusColumn.HeaderText = "Status";
            this.StatusColumn.Name = "StatusColumn";
            this.StatusColumn.Width = 100;
            //
            // ClosedDateColumn
            //
            this.ClosedDateColumn.DataPropertyName = "ClosedDate";
            this.ClosedDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            this.ClosedDateColumn.HeaderText = "Closed date";
            this.ClosedDateColumn.Name = "ClosedDateColumn";
            this.ClosedDateColumn.Width = 140;
            //
            // MainPage
            //
            this.AutoScroll = true;
            this.AutoValidate = Wisej.Web.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblHint);
            this.Controls.Add(this.gridContacts);
            this.Controls.Add(this.lblTrace);
            this.Controls.Add(this.listTrace);
            this.Controls.Add(this.btnIntake);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnFailure);
            this.Controls.Add(this.lblStatus);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 700);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.BindingSource gridSource;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblHint;
        private Wisej.Web.DataGridView gridContacts;
        private Wisej.Web.Label lblTrace;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Button btnIntake;
        private Wisej.Web.Button btnReload;
        private Wisej.Web.Button btnFailure;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.DataGridViewTextBoxColumn NameColumn;
        private Wisej.Web.DataGridViewTextBoxColumn EmailColumn;
        private Wisej.Web.DataGridViewTextBoxColumn AgeColumn;
        private Wisej.Web.DataGridViewComboBoxColumn StatusColumn;
        private Wisej.Web.DataGridViewTextBoxColumn ClosedDateColumn;
    }
}
