namespace OrderDesk.Dialogs            // ✓ was: namespace LegacyOrderDesk
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

        #region Wisej.NET Designer generated code   (was: Windows Form Designer generated code)

        // "System.Windows.Forms." → "Wisej.Web." everywhere; the only line the compiler rejected is tagged ✓ was.
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
            this.customerLabel.AutoSize = true; this.customerLabel.Location = new System.Drawing.Point(20, 22); this.customerLabel.Text = "Customer";
            this.customerComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.customerComboBox.Location = new System.Drawing.Point(110, 18); this.customerComboBox.Name = "customerComboBox"; this.customerComboBox.Size = new System.Drawing.Size(260, 23); this.customerComboBox.TabIndex = 0;
            this.ownerLabel.AutoSize = true; this.ownerLabel.Location = new System.Drawing.Point(20, 56); this.ownerLabel.Text = "Owner";
            this.ownerTextBox.Location = new System.Drawing.Point(110, 52); this.ownerTextBox.Name = "ownerTextBox"; this.ownerTextBox.Size = new System.Drawing.Size(260, 23); this.ownerTextBox.TabIndex = 1;
            this.statusLabel.AutoSize = true; this.statusLabel.Location = new System.Drawing.Point(20, 90); this.statusLabel.Text = "Status";
            this.statusComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.statusComboBox.Location = new System.Drawing.Point(110, 86); this.statusComboBox.Name = "statusComboBox"; this.statusComboBox.Size = new System.Drawing.Size(260, 23); this.statusComboBox.TabIndex = 2;
            this.poLabel.AutoSize = true; this.poLabel.Location = new System.Drawing.Point(20, 124); this.poLabel.Text = "PO #";
            this.poTextBox.Location = new System.Drawing.Point(110, 120); this.poTextBox.Name = "poTextBox"; this.poTextBox.Size = new System.Drawing.Size(260, 23); this.poTextBox.TabIndex = 3;
            this.cancelButton.DialogResult = Wisej.Web.DialogResult.Cancel;
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
            this.FormBorderStyle = Wisej.Web.FormBorderStyle.Fixed;         // ✓ was: FormBorderStyle.FixedDialog (CS0117 — Wisej has Fixed / FixedToolWindow)
            this.MaximizeBox = false; this.MinimizeBox = false;
            this.Name = "EditOrderDialog";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
            this.Text = "Edit Order";
            this.ResumeLayout(false);
            this.PerformLayout();
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
