using System;
using System.Windows.Forms;

namespace LegacyOrderDesk
{
    /// <summary>Login — sets the static current user (see AppState).</summary>
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var user = userTextBox.Text.Trim();
            if (user.Length == 0)
            {
                MessageBox.Show("Enter a user name.", "LegacyOrderDesk", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AppState.CurrentUser = user;           // ✕ static "current user" — shared across every session on a server
            AppState.CurrentCompany = companyComboBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
