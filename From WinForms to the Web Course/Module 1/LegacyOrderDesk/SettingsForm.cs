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
            RegistrySettings.Save(settings);      // ✕ HKCU on the SERVER is the wrong machine once this runs on the web
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
