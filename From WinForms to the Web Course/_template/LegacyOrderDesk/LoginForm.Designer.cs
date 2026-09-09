namespace LegacyOrderDesk
{
    partial class LoginForm
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
            this.userLabel = new System.Windows.Forms.Label();
            this.userTextBox = new System.Windows.Forms.TextBox();
            this.companyLabel = new System.Windows.Forms.Label();
            this.companyComboBox = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // userLabel
            //
            this.userLabel.AutoSize = true;
            this.userLabel.Location = new System.Drawing.Point(20, 24);
            this.userLabel.Name = "userLabel";
            this.userLabel.Text = "User name";
            //
            // userTextBox
            //
            this.userTextBox.Location = new System.Drawing.Point(120, 20);
            this.userTextBox.Name = "userTextBox";
            this.userTextBox.Size = new System.Drawing.Size(200, 23);
            this.userTextBox.TabIndex = 0;
            this.userTextBox.Text = "kelly";
            //
            // companyLabel
            //
            this.companyLabel.AutoSize = true;
            this.companyLabel.Location = new System.Drawing.Point(20, 58);
            this.companyLabel.Name = "companyLabel";
            this.companyLabel.Text = "Company";
            //
            // companyComboBox
            //
            this.companyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.companyComboBox.Items.AddRange(new object[] { "Acme", "Globex" });
            this.companyComboBox.Location = new System.Drawing.Point(120, 54);
            this.companyComboBox.Name = "companyComboBox";
            this.companyComboBox.SelectedIndex = 0;
            this.companyComboBox.Size = new System.Drawing.Size(200, 23);
            this.companyComboBox.TabIndex = 1;
            //
            // okButton
            //
            this.okButton.Location = new System.Drawing.Point(164, 100);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 28);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "Sign in";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            //
            // cancelButton
            //
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(245, 100);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 28);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            //
            // LoginForm
            //
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(344, 146);
            this.Controls.Add(this.userLabel);
            this.Controls.Add(this.userTextBox);
            this.Controls.Add(this.companyLabel);
            this.Controls.Add(this.companyComboBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LegacyOrderDesk — Sign in";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label userLabel;
        private System.Windows.Forms.TextBox userTextBox;
        private System.Windows.Forms.Label companyLabel;
        private System.Windows.Forms.ComboBox companyComboBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}
