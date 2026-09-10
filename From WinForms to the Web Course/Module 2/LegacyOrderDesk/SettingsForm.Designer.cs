namespace LegacyOrderDesk
{
    partial class SettingsForm
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
            this.densityLabel = new System.Windows.Forms.Label();
            this.densityComboBox = new System.Windows.Forms.ComboBox();
            this.exportFolderLabel = new System.Windows.Forms.Label();
            this.exportFolderTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.densityLabel.AutoSize = true; this.densityLabel.Location = new System.Drawing.Point(20, 22); this.densityLabel.Text = "Grid density";
            this.densityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.densityComboBox.Items.AddRange(new object[] { "Comfortable", "Compact" });
            this.densityComboBox.Location = new System.Drawing.Point(120, 18); this.densityComboBox.Name = "densityComboBox"; this.densityComboBox.Size = new System.Drawing.Size(240, 23); this.densityComboBox.TabIndex = 0;
            this.exportFolderLabel.AutoSize = true; this.exportFolderLabel.Location = new System.Drawing.Point(20, 56); this.exportFolderLabel.Text = "Export folder";
            this.exportFolderTextBox.Location = new System.Drawing.Point(120, 52); this.exportFolderTextBox.Name = "exportFolderTextBox"; this.exportFolderTextBox.Size = new System.Drawing.Size(240, 23); this.exportFolderTextBox.TabIndex = 1;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(204, 100); this.cancelButton.Name = "cancelButton"; this.cancelButton.Size = new System.Drawing.Size(75, 28); this.cancelButton.TabIndex = 3; this.cancelButton.Text = "Cancel";
            this.saveButton.Location = new System.Drawing.Point(285, 100); this.saveButton.Name = "saveButton"; this.saveButton.Size = new System.Drawing.Size(75, 28); this.saveButton.TabIndex = 2; this.saveButton.Text = "Save";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            this.AcceptButton = this.saveButton;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(382, 146);
            this.Controls.Add(this.densityLabel); this.Controls.Add(this.densityComboBox);
            this.Controls.Add(this.exportFolderLabel); this.Controls.Add(this.exportFolderTextBox);
            this.Controls.Add(this.cancelButton); this.Controls.Add(this.saveButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label densityLabel;
        private System.Windows.Forms.ComboBox densityComboBox;
        private System.Windows.Forms.Label exportFolderLabel;
        private System.Windows.Forms.TextBox exportFolderTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
    }
}
