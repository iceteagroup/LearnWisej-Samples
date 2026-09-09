namespace WisejTrainingApp.Views
{
    partial class SettingsView
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
            this.lblViewTitle = new Wisej.Web.Label();
            this.pnlSettings = new Wisej.Web.Panel();
            this.lblSettingsCaption = new Wisej.Web.Label();
            this.lblCompanyName = new Wisej.Web.Label();
            this.txtCompanyName = new Wisej.Web.TextBox();
            this.lblDefaultPriority = new Wisej.Web.Label();
            this.cboDefaultPriority = new Wisej.Web.ComboBox();
            this.chkEmailNotifications = new Wisej.Web.CheckBox();
            this.btnSaveSettings = new Wisej.Web.Button();
            this.btnTryServerSave = new Wisej.Web.Button();
            this.lblPermissionNote = new Wisej.Web.Label();
            this.pnlRole = new Wisej.Web.Panel();
            this.lblRoleCaption = new Wisej.Web.Label();
            this.lblRoleValue = new Wisej.Web.Label();
            this.lblMatrixCaption = new Wisej.Web.Label();
            this.lstPermissions = new Wisej.Web.ListBox();
            this.lblRoleHint = new Wisej.Web.Label();
            this.pnlSettings.SuspendLayout();
            this.pnlRole.SuspendLayout();
            this.SuspendLayout();
            //
            // lblViewTitle
            //
            this.lblViewTitle.AutoSize = false;
            this.lblViewTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblViewTitle.Location = new System.Drawing.Point(0, 0);
            this.lblViewTitle.Name = "lblViewTitle";
            this.lblViewTitle.Size = new System.Drawing.Size(400, 30);
            this.lblViewTitle.Text = "Settings  ·  SettingsView (UserControl)";
            //
            // pnlSettings  (the settings card)
            //
            this.pnlSettings.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.pnlSettings.BackColor = System.Drawing.Color.White;
            this.pnlSettings.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlSettings.Controls.Add(this.lblSettingsCaption);
            this.pnlSettings.Controls.Add(this.lblCompanyName);
            this.pnlSettings.Controls.Add(this.txtCompanyName);
            this.pnlSettings.Controls.Add(this.lblDefaultPriority);
            this.pnlSettings.Controls.Add(this.cboDefaultPriority);
            this.pnlSettings.Controls.Add(this.chkEmailNotifications);
            this.pnlSettings.Controls.Add(this.btnSaveSettings);
            this.pnlSettings.Controls.Add(this.btnTryServerSave);
            this.pnlSettings.Controls.Add(this.lblPermissionNote);
            this.pnlSettings.Location = new System.Drawing.Point(0, 44);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(640, 492);
            //
            // lblSettingsCaption
            //
            this.lblSettingsCaption.AutoSize = false;
            this.lblSettingsCaption.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblSettingsCaption.Location = new System.Drawing.Point(24, 14);
            this.lblSettingsCaption.Name = "lblSettingsCaption";
            this.lblSettingsCaption.Size = new System.Drawing.Size(592, 28);
            this.lblSettingsCaption.Text = "Application settings";
            //
            // lblCompanyName
            //
            this.lblCompanyName.AutoSize = false;
            this.lblCompanyName.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCompanyName.Location = new System.Drawing.Point(24, 60);
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Size = new System.Drawing.Size(400, 22);
            this.lblCompanyName.Text = "Company name";
            //
            // txtCompanyName
            //
            this.txtCompanyName.Location = new System.Drawing.Point(24, 84);
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.Size = new System.Drawing.Size(400, 34);
            this.txtCompanyName.Text = "ServiceDesk Inc.";
            //
            // lblDefaultPriority
            //
            this.lblDefaultPriority.AutoSize = false;
            this.lblDefaultPriority.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblDefaultPriority.Location = new System.Drawing.Point(24, 134);
            this.lblDefaultPriority.Name = "lblDefaultPriority";
            this.lblDefaultPriority.Size = new System.Drawing.Size(400, 22);
            this.lblDefaultPriority.Text = "Default priority for new tickets";
            //
            // cboDefaultPriority
            //
            this.cboDefaultPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboDefaultPriority.Items.AddRange(new object[] { "Low", "Medium", "High" });
            this.cboDefaultPriority.Location = new System.Drawing.Point(24, 158);
            this.cboDefaultPriority.Name = "cboDefaultPriority";
            this.cboDefaultPriority.SelectedIndex = 1;
            this.cboDefaultPriority.Size = new System.Drawing.Size(200, 34);
            //
            // chkEmailNotifications
            //
            this.chkEmailNotifications.Checked = true;
            this.chkEmailNotifications.Location = new System.Drawing.Point(24, 210);
            this.chkEmailNotifications.Name = "chkEmailNotifications";
            this.chkEmailNotifications.Size = new System.Drawing.Size(400, 26);
            this.chkEmailNotifications.Text = "Send an email notification when a ticket is closed";
            //
            // btnSaveSettings  (disabled for a Support Agent — the first permission)
            //
            this.btnSaveSettings.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSaveSettings.Location = new System.Drawing.Point(24, 262);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(160, 36);
            this.btnSaveSettings.Text = "Save Settings";
            this.btnSaveSettings.ToolTipText = "Enabled only when PermissionService.CanSaveSettings(role) is true.";
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            //
            // btnTryServerSave  (never disabled: proves the server check, not the button, protects the action)
            //
            this.btnTryServerSave.Location = new System.Drawing.Point(196, 262);
            this.btnTryServerSave.Name = "btnTryServerSave";
            this.btnTryServerSave.Size = new System.Drawing.Size(260, 36);
            this.btnTryServerSave.Text = "Try to save anyway (server check)";
            this.btnTryServerSave.ToolTipText = "Runs the same SaveSettings() without the disabled button in the way — the server refuses it for a Support Agent.";
            this.btnTryServerSave.Click += new System.EventHandler(this.btnTryServerSave_Click);
            //
            // lblPermissionNote
            //
            this.lblPermissionNote.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblPermissionNote.AutoSize = false;
            this.lblPermissionNote.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblPermissionNote.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.lblPermissionNote.Location = new System.Drawing.Point(24, 312);
            this.lblPermissionNote.Name = "lblPermissionNote";
            this.lblPermissionNote.Size = new System.Drawing.Size(592, 60);
            this.lblPermissionNote.Text = "Settings are view-only for this role.";
            this.lblPermissionNote.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // pnlRole  (who you are and what the matrix allows)
            //
            this.pnlRole.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlRole.BackColor = System.Drawing.Color.White;
            this.pnlRole.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlRole.Controls.Add(this.lblRoleCaption);
            this.pnlRole.Controls.Add(this.lblRoleValue);
            this.pnlRole.Controls.Add(this.lblMatrixCaption);
            this.pnlRole.Controls.Add(this.lstPermissions);
            this.pnlRole.Controls.Add(this.lblRoleHint);
            this.pnlRole.Location = new System.Drawing.Point(664, 44);
            this.pnlRole.Name = "pnlRole";
            this.pnlRole.Size = new System.Drawing.Size(416, 492);
            //
            // lblRoleCaption
            //
            this.lblRoleCaption.AutoSize = false;
            this.lblRoleCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblRoleCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblRoleCaption.Location = new System.Drawing.Point(20, 14);
            this.lblRoleCaption.Name = "lblRoleCaption";
            this.lblRoleCaption.Size = new System.Drawing.Size(376, 22);
            this.lblRoleCaption.Text = "CURRENT ROLE";
            //
            // lblRoleValue
            //
            this.lblRoleValue.AutoSize = false;
            this.lblRoleValue.Font = new System.Drawing.Font("default", 20F, System.Drawing.FontStyle.Bold);
            this.lblRoleValue.Location = new System.Drawing.Point(20, 40);
            this.lblRoleValue.Name = "lblRoleValue";
            this.lblRoleValue.Size = new System.Drawing.Size(376, 40);
            this.lblRoleValue.Text = "Support Agent";
            //
            // lblMatrixCaption
            //
            this.lblMatrixCaption.AutoSize = false;
            this.lblMatrixCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblMatrixCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblMatrixCaption.Location = new System.Drawing.Point(20, 100);
            this.lblMatrixCaption.Name = "lblMatrixCaption";
            this.lblMatrixCaption.Size = new System.Drawing.Size(376, 22);
            this.lblMatrixCaption.Text = "WHAT THIS ROLE MAY DO  (PermissionService)";
            //
            // lstPermissions
            //
            this.lstPermissions.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstPermissions.Font = new System.Drawing.Font("monospace", 9F);
            this.lstPermissions.Location = new System.Drawing.Point(20, 126);
            this.lstPermissions.Name = "lstPermissions";
            this.lstPermissions.Size = new System.Drawing.Size(376, 130);
            //
            // lblRoleHint
            //
            this.lblRoleHint.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblRoleHint.AutoSize = false;
            this.lblRoleHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblRoleHint.Location = new System.Drawing.Point(20, 270);
            this.lblRoleHint.Name = "lblRoleHint";
            this.lblRoleHint.Size = new System.Drawing.Size(376, 120);
            this.lblRoleHint.Text = "No login system yet — the role is a design concept. Switch the role in the header to Manager: the shell re-applies permissions and rebuilds this view with Save Settings enabled.\n\nLater this grows into real authentication and authorization; the PermissionService calls stay where they are.";
            this.lblRoleHint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // SettingsView
            //
            this.Controls.Add(this.lblViewTitle);
            this.Controls.Add(this.pnlSettings);
            this.Controls.Add(this.pnlRole);
            this.Name = "SettingsView";
            this.Size = new System.Drawing.Size(1080, 536);
            this.Load += new System.EventHandler(this.SettingsView_Load);
            this.pnlSettings.ResumeLayout(false);
            this.pnlRole.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblViewTitle;
        private Wisej.Web.Panel pnlSettings;
        private Wisej.Web.Label lblSettingsCaption;
        private Wisej.Web.Label lblCompanyName;
        private Wisej.Web.TextBox txtCompanyName;
        private Wisej.Web.Label lblDefaultPriority;
        private Wisej.Web.ComboBox cboDefaultPriority;
        private Wisej.Web.CheckBox chkEmailNotifications;
        private Wisej.Web.Button btnSaveSettings;
        private Wisej.Web.Button btnTryServerSave;
        private Wisej.Web.Label lblPermissionNote;
        private Wisej.Web.Panel pnlRole;
        private Wisej.Web.Label lblRoleCaption;
        private Wisej.Web.Label lblRoleValue;
        private Wisej.Web.Label lblMatrixCaption;
        private Wisej.Web.ListBox lstPermissions;
        private Wisej.Web.Label lblRoleHint;
    }
}
