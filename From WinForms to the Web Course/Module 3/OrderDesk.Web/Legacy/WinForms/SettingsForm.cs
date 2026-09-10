// ✕ LegacyOrderDesk "before" source, copied unchanged for side-by-side reading. NOT compiled (see OrderDesk.Web.csproj):
// ✕ System.Windows.Forms does not exist in the web project; the Wisej.NET port of this file lives in Shell/, Screens/ or Dialogs/.

using System;
using System.Windows.Forms;
using LegacyOrderDesk.Settings;

namespace LegacyOrderDesk
{
    /// <summary>User preferences, stored in HKEY_CURRENT_USER — the Module 4 "registry" item.</summary>
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            var settings = RegistrySettings.Load();
            densityComboBox.SelectedItem = settings.GridDensity;
            exportFolderTextBox.Text = settings.ExportFolder;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var settings = RegistrySettings.Load();
            settings.GridDensity = (string)densityComboBox.SelectedItem ?? "Comfortable";
            settings.ExportFolder = exportFolderTextBox.Text.Trim();
            RegistrySettings.Save(settings);      // ✕ HKCU on the SERVER is the service account’s registry, shared by every visitor → Application.Session now, profile store in Module 4
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
