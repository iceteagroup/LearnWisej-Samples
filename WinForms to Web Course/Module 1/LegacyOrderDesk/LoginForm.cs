using System;
using System.Drawing;
using System.Windows.Forms;
using OrderDesk.Domain;

namespace LegacyOrderDesk
{
    /// <summary>The desktop login: any of the known clerks, no password (a LAN app behind the office firewall).</summary>
    public class LoginForm : Form
    {
        private readonly ComboBox _users;
        private readonly Button _ok;

        public User User { get; private set; }

        public LoginForm()
        {
            Text = "LegacyOrderDesk — Sign in";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false; MinimizeBox = false;
            ClientSize = new Size(360, 150);

            Controls.Add(new Label { Text = "User", Location = new Point(20, 24), AutoSize = true });
            _users = new ComboBox { Location = new Point(20, 44), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
            _users.Items.AddRange(new object[] { "kelly", "sam", "dana", "priya" });
            _users.SelectedItem = UserPreferences.Get("LastUser", "kelly");
            Controls.Add(_users);

            _ok = new Button { Text = "Sign in", Location = new Point(240, 100), Size = new Size(100, 30), DialogResult = DialogResult.OK };
            _ok.Click += (s, e) =>
            {
                var name = (string)_users.SelectedItem;
                User = new User { Id = _users.SelectedIndex + 1, UserName = name, DisplayName = char.ToUpperInvariant(name[0]) + name.Substring(1), Company = name == "sam" ? "Globex" : "Acme" };
                UserPreferences.Set("LastUser", name);       // ✕ HKCU on the user's PC
            };
            Controls.Add(_ok);
            AcceptButton = _ok;
        }
    }
}
