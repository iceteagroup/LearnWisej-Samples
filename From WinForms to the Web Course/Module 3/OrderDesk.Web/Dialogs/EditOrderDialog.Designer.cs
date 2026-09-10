namespace OrderDesk.Dialogs
{
    partial class EditOrderDialog
    {
        // Designer-owned file: only InitializeComponent, the component container and the field
        // declarations live here. Dispose(bool) moved to EditOrderDialog.cs because it carries
        // migration logic (DialogTracker) that the designer must never regenerate away.
        private System.ComponentModel.IContainer components = null;

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.customerLabel = new Wisej.Web.Label();
            this.customerComboBox = new Wisej.Web.ComboBox();
            this.ownerLabel = new Wisej.Web.Label();
            this.ownerTextBox = new Wisej.Web.TextBox();
            this.statusLabel = new Wisej.Web.Label();
            this.statusComboBox = new Wisej.Web.ComboBox();
            this.poLabel = new Wisej.Web.Label();
            this.poTextBox = new Wisej.Web.TextBox();
            this.cancelButton = new Wisej.Web.Button();
            this.saveButton = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // customerLabel / customerComboBox   (TabIndex 0 — the focus order is set explicitly, as on the desktop)
            //
            this.customerLabel.AutoSize = true;
            this.customerLabel.Location = new System.Drawing.Point(20, 22);
            this.customerLabel.Name = "customerLabel";
            this.customerLabel.Text = "Customer";
            this.customerComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.customerComboBox.Location = new System.Drawing.Point(110, 18);
            this.customerComboBox.Name = "customerComboBox";
            this.customerComboBox.Size = new System.Drawing.Size(260, 28);
            this.customerComboBox.TabIndex = 0;
            //
            // ownerLabel / ownerTextBox   (TabIndex 1)
            //
            this.ownerLabel.AutoSize = true;
            this.ownerLabel.Location = new System.Drawing.Point(20, 58);
            this.ownerLabel.Name = "ownerLabel";
            this.ownerLabel.Text = "Owner";
            this.ownerTextBox.Location = new System.Drawing.Point(110, 54);
            this.ownerTextBox.Name = "ownerTextBox";
            this.ownerTextBox.Size = new System.Drawing.Size(260, 28);
            this.ownerTextBox.TabIndex = 1;
            //
            // statusLabel / statusComboBox   (TabIndex 2)
            //
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(20, 94);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Text = "Status";
            this.statusComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.statusComboBox.Location = new System.Drawing.Point(110, 90);
            this.statusComboBox.Name = "statusComboBox";
            this.statusComboBox.Size = new System.Drawing.Size(260, 28);
            this.statusComboBox.TabIndex = 2;
            //
            // poLabel / poTextBox   (TabIndex 3)
            //
            this.poLabel.AutoSize = true;
            this.poLabel.Location = new System.Drawing.Point(20, 130);
            this.poLabel.Name = "poLabel";
            this.poLabel.Text = "PO #";
            this.poTextBox.Location = new System.Drawing.Point(110, 126);
            this.poTextBox.Name = "poTextBox";
            this.poTextBox.Size = new System.Drawing.Size(260, 28);
            this.poTextBox.TabIndex = 3;
            //
            // saveButton (TabIndex 4, AcceptButton) / cancelButton (TabIndex 5, CancelButton)
            //
            this.saveButton.Location = new System.Drawing.Point(295, 176);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 30);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            this.cancelButton.DialogResult = Wisej.Web.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(214, 176);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 30);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            //
            // EditOrderDialog
            //
            this.AcceptButton = this.saveButton;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(392, 224);
            this.Controls.Add(this.customerLabel);
            this.Controls.Add(this.customerComboBox);
            this.Controls.Add(this.ownerLabel);
            this.Controls.Add(this.ownerTextBox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.statusComboBox);
            this.Controls.Add(this.poLabel);
            this.Controls.Add(this.poTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.Fixed;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditOrderDialog";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Edit Order";
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label customerLabel;
        private Wisej.Web.ComboBox customerComboBox;
        private Wisej.Web.Label ownerLabel;
        private Wisej.Web.TextBox ownerTextBox;
        private Wisej.Web.Label statusLabel;
        private Wisej.Web.ComboBox statusComboBox;
        private Wisej.Web.Label poLabel;
        private Wisej.Web.TextBox poTextBox;
        private Wisej.Web.Button cancelButton;
        private Wisej.Web.Button saveButton;
    }
}
