using System;
using System.Drawing;
using AccessOps.Identity;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The sign-in screen. The click only verifies and parks the account
    /// (<see cref="SignInFlow.Start"/>); the principal is established on the page load that
    /// follows, never here.
    /// </summary>
    public class SignInView : UserControl
    {
        private readonly TextBox txtUser = new TextBox();
        private readonly TextBox txtSecret = new TextBox();
        private readonly Button btnSignIn = new Button();
        private readonly StatusLine lblStatus = new StatusLine();

        public SignInView()
        {
            this.Name = "SignInView";
            this.BackColor = Color.White;

            var card = new Panel
            {
                Name = "pnlSignIn",
                Size = new Size(340, 236),
                Location = new Point(40, 36),
                Padding = new Padding(20, 16, 20, 16),
                CssStyle = "border:1px solid #e0e7ef; border-radius:9px;"
            };

            var title = new Label
            {
                Text = "Sign in",
                Dock = DockStyle.Top,
                Height = 32,
                Font = Theme.Px(16f, FontStyle.Bold),
                ForeColor = Theme.Text
            };

            txtUser.Name = "txtUser";
            txtUser.Dock = DockStyle.Top;
            Theme.StyleInput(txtUser, 300);
            txtUser.Dock = DockStyle.Top;

            txtSecret.Name = "txtSecret";
            txtSecret.InputType.Type = TextBoxType.Password;
            Theme.StyleInput(txtSecret, 300);
            txtSecret.Dock = DockStyle.Top;

            btnSignIn.Name = "btnSignIn";
            btnSignIn.Text = "Sign in";
            Theme.StyleButton(btnSignIn, primary: true);
            btnSignIn.Dock = DockStyle.Top;
            btnSignIn.Click += btnSignIn_Click;

            var userLabel = Theme.FieldLabel("User name");
            userLabel.AutoSize = false;
            userLabel.Dock = DockStyle.Top;
            userLabel.Height = 22;
            var secretLabel = Theme.FieldLabel("Secret");
            secretLabel.AutoSize = false;
            secretLabel.Dock = DockStyle.Top;
            secretLabel.Height = 22;

            // Dock order is the reverse of Controls.Add.
            card.Controls.Add(btnSignIn);
            card.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 14 });
            card.Controls.Add(txtSecret);
            card.Controls.Add(secretLabel);
            card.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 8 });
            card.Controls.Add(txtUser);
            card.Controls.Add(userLabel);
            card.Controls.Add(title);

            lblStatus.Show("Sign in to open the Documents screen.", StatusTone.Idle);

            this.Controls.Add(card);
            this.Controls.Add(lblStatus);
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            var verifier = AppHost.Find<IAccountVerifier>();
            if (verifier == null)
            {
                lblStatus.Show("Sign-in is not available in this build.", StatusTone.Warn);
                return;
            }

            if (!SignInFlow.Start(verifier, txtUser.Text, txtSecret.Text))
            {
                txtSecret.Text = "";
                lblStatus.Show("Sign-in failed.", StatusTone.Deny);
            }
        }
    }
}
