using System;
using Wisej.Web;

namespace WisejTrainingApp.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView(string currentRole)
        {
            InitializeComponent();

            // A Support Agent can view Settings but not save them.
            if (currentRole == "Support Agent")
            {
                btnSave.Enabled = false;
                lblPermission.Text = "View only — a Support Agent cannot save settings.";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            lblPermission.Text = "Settings saved.";
        }
    }
}
