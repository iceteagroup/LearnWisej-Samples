namespace OrderDesk.Dialogs
{
    partial class EditOrderDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used. In WinForms the process exit hid forgotten dialogs;
        /// here Dispose is the moment the server releases the instance (see LiveInstances).
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
                OnDisposing();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelCustomer = new Wisej.Web.Label();
            this.comboCustomer = new Wisej.Web.ComboBox();
            this.labelOwner = new Wisej.Web.Label();
            this.comboOwner = new Wisej.Web.ComboBox();
            this.labelStatus = new Wisej.Web.Label();
            this.comboStatus = new Wisej.Web.ComboBox();
            this.btnCancel = new Wisej.Web.Button();
            this.btnSave = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // labelCustomer
            //
            this.labelCustomer.AutoSize = false;
            this.labelCustomer.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelCustomer.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelCustomer.Location = new System.Drawing.Point(20, 16);
            this.labelCustomer.Name = "labelCustomer";
            this.labelCustomer.Size = new System.Drawing.Size(340, 18);
            this.labelCustomer.Text = "Customer";
            //
            // comboCustomer
            //
            this.comboCustomer.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboCustomer.Location = new System.Drawing.Point(20, 36);
            this.comboCustomer.Name = "comboCustomer";
            this.comboCustomer.Size = new System.Drawing.Size(340, 30);
            this.comboCustomer.TabIndex = 0;
            //
            // labelOwner
            //
            this.labelOwner.AutoSize = false;
            this.labelOwner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelOwner.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelOwner.Location = new System.Drawing.Point(20, 86);
            this.labelOwner.Name = "labelOwner";
            this.labelOwner.Size = new System.Drawing.Size(340, 18);
            this.labelOwner.Text = "Owner";
            //
            // comboOwner
            //
            this.comboOwner.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboOwner.Location = new System.Drawing.Point(20, 106);
            this.comboOwner.Name = "comboOwner";
            this.comboOwner.Size = new System.Drawing.Size(340, 30);
            this.comboOwner.TabIndex = 1;
            //
            // labelStatus
            //
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelStatus.Location = new System.Drawing.Point(20, 156);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(340, 18);
            this.labelStatus.Text = "Status";
            //
            // comboStatus
            //
            this.comboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboStatus.Location = new System.Drawing.Point(20, 176);
            this.comboStatus.Name = "comboStatus";
            this.comboStatus.Size = new System.Drawing.Size(340, 30);
            this.comboStatus.TabIndex = 2;
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(160, 232);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 32);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnSave
            //
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(264, 232);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(96, 32);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // EditOrderDialog
            //
            this.AcceptButton = this.btnSave;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.labelCustomer);
            this.Controls.Add(this.comboCustomer);
            this.Controls.Add(this.labelOwner);
            this.Controls.Add(this.comboOwner);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.comboStatus);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.Fixed;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditOrderDialog";
            this.ShowInTaskbar = false;
            this.Size = new System.Drawing.Size(380, 316);
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Edit Order";
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label labelCustomer;
        private Wisej.Web.ComboBox comboCustomer;
        private Wisej.Web.Label labelOwner;
        private Wisej.Web.ComboBox comboOwner;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.ComboBox comboStatus;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Button btnSave;
    }
}
