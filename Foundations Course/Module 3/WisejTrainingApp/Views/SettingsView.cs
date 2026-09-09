using System;
using System.Collections.Generic;
using Wisej.Web;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Settings page — where the first permission becomes visible (lesson s12 §3).
    ///
    /// The view is built for a role (the shell passes currentRole, exactly like the lesson's
    /// new SettingsView(currentRole)). For a Support Agent the Save Settings button is disabled and
    /// the note says "Settings are view-only for this role."; for a Manager it is enabled.
    ///
    /// The disabled button is only the UI half. SaveSettings() asks PermissionService AGAIN before it
    /// saves — the "Try to save anyway (server check)" button skips the disabled button on purpose so
    /// you can watch the server refuse the action.
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private readonly string currentRole;
        private readonly IShellHost shell;
        private readonly PermissionService permissions;

        public SettingsView(string currentRole, IShellHost shell, PermissionService permissions)
        {
            this.currentRole = currentRole;
            this.shell = shell;
            this.permissions = permissions;

            InitializeComponent();
        }

        private void SettingsView_Load(object sender, EventArgs e)
        {
            lblRoleValue.Text = currentRole;
            ShowRoleMatrix();
            ApplyPermissions();
        }

        /// <summary>The beginner permission: view the settings, but only a Manager may save.</summary>
        private void ApplyPermissions()
        {
            if (permissions.CanSaveSettings(currentRole))
            {
                btnSaveSettings.Enabled = true;
                lblPermissionNote.Text = currentRole + " can save settings.";
                lblPermissionNote.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            }
            else
            {
                // Support Agent: view-only.
                btnSaveSettings.Enabled = false;
                lblPermissionNote.Text = "Settings are view-only for this role.";
                lblPermissionNote.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            }
        }

        private void ShowRoleMatrix()
        {
            lstPermissions.Items.Clear();
            foreach (KeyValuePair<string, string> row in permissions.DescribeRole(currentRole))
                lstPermissions.Items.Add($"{row.Key,-12} {row.Value}");
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            SaveSettings("Save Settings");
        }

        private void btnTryServerSave_Click(object sender, EventArgs e)
        {
            // Same code path as the real button — but this one is never disabled, so the server check is what stops it.
            SaveSettings("Try to save anyway");
        }

        /// <summary>
        /// The protected action. The role is read from the shell (what the header says now), and the
        /// check is the same PermissionService call the shell used to disable the button.
        /// </summary>
        private void SaveSettings(string via)
        {
            string role = shell.CurrentRole;

            if (!permissions.CanSaveSettings(role))
            {
                // Failure path: refused on the server, whatever the button state was.
                shell.Log($"Permission denied: {role} cannot save settings. (via {via})", LogKind.Error);
                lblPermissionNote.Text = "Refused by the server: " + role + " cannot save settings.";
                lblPermissionNote.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
                AlertBox.Show("Permission denied: " + role + " cannot save settings.", MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            // Success path (Manager): nothing is persisted in this module — the decision is what matters.
            string summary = $"company = \"{txtCompanyName.Text.Trim()}\", default priority = {cboDefaultPriority.Text}, "
                + $"email notifications = {(chkEmailNotifications.Checked ? "on" : "off")}";

            shell.Log($"Settings saved by {role}. ({summary})");
            lblPermissionNote.Text = "Saved: " + summary;
            lblPermissionNote.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            AlertBox.Show("Settings saved by " + role + ".", MessageBoxIcon.Information,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }
    }
}
