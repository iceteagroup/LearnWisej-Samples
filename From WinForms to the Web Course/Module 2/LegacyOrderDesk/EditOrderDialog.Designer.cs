namespace LegacyOrderDesk
{
    partial class EditOrderDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.customerLabel = new System.Windows.Forms.Label();
            this.customerComboBox = new System.Windows.Forms.ComboBox();
            this.ownerLabel = new System.Windows.Forms.Label();
            this.ownerTextBox = new System.Windows.Forms.TextBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.statusComboBox = new System.Windows.Forms.ComboBox();
            this.poLabel = new System.Windows.Forms.Label();
            this.poTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.customerLabel.AutoSize = true; this.customerLabel.Location = new System.Drawing.Point(20, 22); this.customerLabel.Text = "Customer";
            this.customerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.customerComboBox.Location = new System.Drawing.Point(110, 18); this.customerComboBox.Name = "customerComboBox"; this.customerComboBox.Size = new System.Drawing.Size(260, 23); this.customerComboBox.TabIndex = 0;
            this.ownerLabel.AutoSize = true; this.ownerLabel.Location = new System.Drawing.Point(20, 56); this.ownerLabel.Text = "Owner";
            this.ownerTextBox.Location = new System.Drawing.Point(110, 52); this.ownerTextBox.Name = "ownerTextBox"; this.ownerTextBox.Size = new System.Drawing.Size(260, 23); this.ownerTextBox.TabIndex = 1;
            this.statusLabel.AutoSize = true; this.statusLabel.Location = new System.Drawing.Point(20, 90); this.statusLabel.Text = "Status";
            this.statusComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.statusComboBox.Location = new System.Drawing.Point(110, 86); this.statusComboBox.Name = "statusComboBox"; this.statusComboBox.Size = new System.Drawing.Size(260, 23); this.statusComboBox.TabIndex = 2;
            this.poLabel.AutoSize = true; this.poLabel.Location = new System.Drawing.Point(20, 124); this.poLabel.Text = "PO #";
            this.poTextBox.Location = new System.Drawing.Point(110, 120); this.poTextBox.Name = "poTextBox"; this.poTextBox.Size = new System.Drawing.Size(260, 23); this.poTextBox.TabIndex = 3;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(214, 170); this.cancelButton.Name = "cancelButton"; this.cancelButton.Size = new System.Drawing.Size(75, 28); this.cancelButton.TabIndex = 5; this.cancelButton.Text = "Cancel";
            this.saveButton.Location = new System.Drawing.Point(295, 170); this.saveButton.Name = "saveButton"; this.saveButton.Size = new System.Drawing.Size(75, 28); this.saveButton.TabIndex = 4; this.saveButton.Text = "Save";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            this.AcceptButton = this.saveButton;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(392, 216);
            this.Controls.Add(this.customerLabel); this.Controls.Add(this.customerComboBox);
            this.Controls.Add(this.ownerLabel); this.Controls.Add(this.ownerTextBox);
            this.Controls.Add(this.statusLabel); this.Controls.Add(this.statusComboBox);
            this.Controls.Add(this.poLabel); this.Controls.Add(this.poTextBox);
            this.Controls.Add(this.cancelButton); this.Controls.Add(this.saveButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.MinimizeBox = false;
            this.Name = "EditOrderDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Order";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label customerLabel;
        private System.Windows.Forms.ComboBox customerComboBox;
        private System.Windows.Forms.Label ownerLabel;
        private System.Windows.Forms.TextBox ownerTextBox;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ComboBox statusComboBox;
        private System.Windows.Forms.Label poLabel;
        private System.Windows.Forms.TextBox poTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
    }
}
