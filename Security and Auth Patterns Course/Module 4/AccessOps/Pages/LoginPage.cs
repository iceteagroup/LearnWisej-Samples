using System;
using System.Drawing;
using AccessOps.Identity;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The partner login view on the anonymous public route ("AccessOps · Partner sign-in"):
    /// User name, Password, Sign in, the status banner and the SERVER line of the walkthrough
    /// mock. <see cref="btnSignIn_Click"/> reads the two boxes, clears the password box and calls
    /// <see cref="LoginService.SignIn"/>; everything else happens on the server and on the page
    /// load that follows (<see cref="SignInHandoff"/>).
    /// </summary>
    public class LoginPage : Page
    {
        /// <summary>The sentence shown after a sign-out ended the previous session.</summary>
        public const string SignedOutNotice = "You have signed out. Sign in to continue.";

        private readonly LoginService _login = AppHost.Get<LoginService>();

        private readonly TextBox txtUserName = new TextBox();
        private readonly TextBox txtPassword = new TextBox();
        private readonly Button btnSignIn = new Button();
        private readonly Label lblStatus = new Label();
        private readonly Label lblServer = new Label();

        /// <summary>Creates the login view; <paramref name="signedOut"/> shows the signed-out notice.</summary>
        public LoginPage(bool signedOut = false)
        {
            this.Name = "LoginPage";
            this.Text = "AccessOps";
            this.BackColor = Color.White;

            var appBar = new AppBar { Screen = "Partner sign-in" };

            // the form column
            var form = new Panel
            {
                Name = "pnlLogin",
                Dock = DockStyle.Left,
                Width = 680,
                Padding = new Padding(40, 30, 40, 0)
            };

            var heading = new Label
            {
                Name = "lblHeading",
                Text = "Sign in to AccessOps",
                Dock = DockStyle.Top,
                Height = 38,
                Font = Theme.Px(26f, FontStyle.Bold),
                ForeColor = Theme.Ink
            };
            var subtitle = new Label
            {
                Name = "lblSubtitle",
                Text = "Partner route · documents and approval requests",
                Dock = DockStyle.Top,
                Height = 24,
                Font = Theme.Px(14f),
                ForeColor = Theme.Muted
            };

            var userLabel = Theme.FieldLabel("User name");
            userLabel.Dock = DockStyle.Top;
            txtUserName.Name = "txtUserName";
            Theme.StyleInput(txtUserName);
            txtUserName.Dock = DockStyle.Top;
            txtUserName.MaxLength = 128;

            var passwordLabel = Theme.FieldLabel("Password");
            passwordLabel.Dock = DockStyle.Top;
            txtPassword.Name = "txtPassword";
            txtPassword.InputType.Type = TextBoxType.Password;
            Theme.StyleInput(txtPassword);
            txtPassword.Dock = DockStyle.Top;
            txtPassword.MaxLength = 256;

            btnSignIn.Name = "btnSignIn";
            btnSignIn.Text = "Sign in";
            Theme.StyleButton(btnSignIn, primary: true);
            btnSignIn.Dock = DockStyle.Left;
            btnSignIn.Width = 104;
            btnSignIn.Click += btnSignIn_Click;
            var buttonRow = new Panel { Name = "pnlButtons", Dock = DockStyle.Top, Height = 38 };
            buttonRow.Controls.Add(btnSignIn);

            lblStatus.Name = "lblStatus";
            lblStatus.Dock = DockStyle.Top;
            lblStatus.Height = 40;
            lblStatus.Padding = new Padding(13, 0, 13, 0);
            lblStatus.Font = Theme.Px(14f, FontStyle.Bold);
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus.Visible = false;

            // Dock order is the reverse of Controls.Add: add bottom-up.
            form.Controls.Add(lblStatus);
            form.Controls.Add(Spacer(14));
            form.Controls.Add(buttonRow);
            form.Controls.Add(Spacer(18));
            form.Controls.Add(txtPassword);
            form.Controls.Add(Spacer(6));
            form.Controls.Add(passwordLabel);
            form.Controls.Add(Spacer(14));
            form.Controls.Add(txtUserName);
            form.Controls.Add(Spacer(6));
            form.Controls.Add(userLabel);
            form.Controls.Add(Spacer(14));
            form.Controls.Add(subtitle);
            form.Controls.Add(heading);

            // the SERVER line at the bottom of the window
            var footer = new Panel
            {
                Name = "pnlServer",
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(40, 0, 40, 0),
                CssStyle = "border-top:1px solid #e4eaf1;"
            };
            lblServer.Name = "lblServer";
            lblServer.Dock = DockStyle.Fill;
            lblServer.Font = Theme.Mono(12f);
            lblServer.ForeColor = Theme.Faint;
            lblServer.TextAlign = ContentAlignment.MiddleLeft;
            var serverTag = new Label
            {
                Name = "lblServerTag",
                Text = "SERVER",
                Dock = DockStyle.Left,
                Width = 62,
                Font = Theme.Px(12f, FontStyle.Bold),
                ForeColor = Theme.Faint,
                TextAlign = ContentAlignment.MiddleLeft,
                CssStyle = "letter-spacing:.05em;"
            };
            footer.Controls.Add(lblServer);
            footer.Controls.Add(serverTag);

            this.Controls.Add(form);
            this.Controls.Add(footer);
            this.Controls.Add(appBar);

            if (signedOut)
            {
                Theme.ShowBanner(lblStatus, SignedOutNotice, BannerTone.Info);
                lblServer.Text = "grant revoked · context cleared · Application.Exit()";
            }
            else
            {
                lblServer.Text = ServerLine(0);
            }
        }

        private static string ServerLine(int failures) => $"LoginThrottle · failures {failures} of {LoginThrottle.AccountLimit}";

        private static Panel Spacer(int height) => new Panel { Dock = DockStyle.Top, Height = height };

        // The click verifies and parks; it never rotates the session ID or sets Application.User.
        private void btnSignIn_Click(object sender, EventArgs e)
        {
            var userName = txtUserName.Text;
            var password = txtPassword.Text;
            txtPassword.Text = "";            // the password box is cleared on every attempt

            var result = _login.SignIn(userName, password, Application.UserHostAddress);
            password = null;

            if (!result.Succeeded)
            {
                Theme.ShowBanner(lblStatus, result.Message, BannerTone.Bad);
                lblServer.Text = ServerLine(result.Failures);
                return;
            }

            Theme.ShowBanner(lblStatus, "Signing in…", BannerTone.Ok);
            lblServer.Text = ServerLine(0);
            SignInHandoff.Park(result.SignIn);   // Application.Reload(): the load finishes the sign-in
        }
    }
}
