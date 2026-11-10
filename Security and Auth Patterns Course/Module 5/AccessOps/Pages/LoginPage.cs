using System;
using System.Drawing;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>Which notice the sign-in view opens with.</summary>
    public enum LoginNotice
    {
        /// <summary>No notice.</summary>
        None,
        /// <summary>The previous session ended with a sign-out.</summary>
        SignedOut,
        /// <summary>An external sign-in failed or was refused (one fixed sentence for every reason).</summary>
        ExternalFailed
    }

    /// <summary>
    /// The partner sign-in view of Module 5 ("AccessOps · Partner sign-in"): federation first -
    /// <b>Sign in with your work account</b> calls <see cref="IExternalSignIn.Begin"/> - and the
    /// Module 4 password row kept underneath for named service accounts only. Every refusal shows
    /// one fixed sentence; the reason goes to the log and the login audit.
    /// </summary>
    public class LoginPage : Page
    {
        private readonly LoginService _login = AppHost.Get<LoginService>();
        private readonly IExternalSignIn _external = AppHost.Get<IExternalSignIn>();

        private readonly Button btnWorkAccount = new Button();
        private readonly Label lblNotice = new Label();
        private readonly TextBox txtUserName = new TextBox();
        private readonly TextBox txtPassword = new TextBox();
        private readonly Label lblDemo = new Label();
        private readonly Button btnSignIn = new Button();
        private readonly StatusStrip _status = new StatusStrip();

        /// <summary>Creates the sign-in view with an optional notice.</summary>
        public LoginPage(LoginNotice notice = LoginNotice.None)
        {
            this.Name = "LoginPage";
            this.Text = "AccessOps";
            this.BackColor = Color.White;

            var appBar = new AppBar { Screen = "Partner sign-in" };

            var form = new Panel { Name = "pnlSignIn", Dock = DockStyle.Left, Width = 640, Padding = new Padding(24, 20, 24, 0) };

            var heading = new Label { Name = "lblHeading", Text = "Sign in to AccessOps", Dock = DockStyle.Top, Height = 30, Font = Theme.Px(20f, FontStyle.Bold), ForeColor = Theme.Ink };
            var subtitle = new Label { Name = "lblSubtitle", Text = "Partner route · documents, approvals and records for your tenant", Dock = DockStyle.Top, Height = 22, Font = Theme.Px(13f), ForeColor = Theme.Muted };

            lblNotice.Name = "lblNotice";
            lblNotice.Dock = DockStyle.Top;
            lblNotice.Height = 36;
            lblNotice.Padding = new Padding(13, 0, 13, 0);
            lblNotice.Font = Theme.Px(12.5f, FontStyle.Bold);
            lblNotice.ForeColor = ColorTranslator.FromHtml("#8a4a12");
            lblNotice.BackColor = ColorTranslator.FromHtml("#fdf1e3");
            lblNotice.CssStyle = "border:1px solid #eccaa4; border-radius:7px;";
            lblNotice.TextAlign = ContentAlignment.MiddleLeft;
            lblNotice.AllowHtml = false;
            lblNotice.Visible = false;
            var noticeGap = new Panel { Dock = DockStyle.Top, Height = 12, Visible = false, BackColor = Color.Transparent };

            btnWorkAccount.Name = "btnWorkAccount";
            btnWorkAccount.Dock = DockStyle.Top;
            btnWorkAccount.Height = 46;
            btnWorkAccount.AllowHtml = true;   // fixed markup built here, never input
            btnWorkAccount.Text =
                "<span style=\"display:flex;align-items:center;gap:12px;width:100%\">" +
                "<span style=\"width:20px;height:20px;border-radius:5px;background:rgba(255,255,255,.9);color:#1565d8;font-size:13px;font-weight:900;display:inline-flex;align-items:center;justify-content:center\">C</span>" +
                "<span>Sign in with your work account</span><span style=\"margin-left:auto;font-size:17px\">→</span></span>";
            btnWorkAccount.TextAlign = ContentAlignment.MiddleLeft;
            btnWorkAccount.Padding = new Padding(18, 0, 18, 0);
            Theme.StyleButton(btnWorkAccount, primary: true, size: 15f);
            btnWorkAccount.CssStyle = "border-radius:9px; border:none;";
            btnWorkAccount.Click += btnWorkAccount_Click;

            var caption = new Label { Name = "lblCaption", Text = "You sign in at your organisation. AccessOps never sees the password.", Dock = DockStyle.Top, Height = 24, Font = Theme.Px(12f), ForeColor = ColorTranslator.FromHtml("#7d8ea0"), TextAlign = ContentAlignment.BottomLeft };

            var divider = new Label
            {
                Name = "lblServiceAccounts",
                Dock = DockStyle.Top,
                Height = 44,
                AllowHtml = true,   // fixed markup
                Text = "<div style=\"display:flex;align-items:center;gap:12px;height:44px\"><span style=\"flex:1;height:1px;background:#e4eaf1\"></span>" +
                       "<span style=\"font-size:11.5px;font-weight:800;color:#9aa9b8;letter-spacing:.05em\">SERVICE ACCOUNTS ONLY</span>" +
                       "<span style=\"flex:1;height:1px;background:#e4eaf1\"></span></div>"
            };

            // 4 px of padding keeps the focused input's glow inside the row (its pane clips overflow).
            var row = new Panel { Name = "pnlServiceAccount", Dock = DockStyle.Top, Height = 42, Padding = new Padding(0, 4, 0, 4) };
            txtUserName.Name = "txtUserName";
            txtUserName.Watermark = "account name";
            Theme.StyleInput(txtUserName);
            txtUserName.Font = Theme.Px(12.5f);
            txtUserName.Height = 34;
            txtUserName.Dock = DockStyle.Left;
            txtUserName.Width = 250;
            txtUserName.MaxLength = 128;
            txtPassword.Name = "txtPassword";
            txtPassword.Watermark = "password";
            txtPassword.InputType.Type = TextBoxType.Password;
            Theme.StyleInput(txtPassword);
            txtPassword.Font = Theme.Px(12.5f);
            txtPassword.Height = 34;
            txtPassword.Dock = DockStyle.Left;
            txtPassword.Width = 250;
            txtPassword.MaxLength = 256;

            // Debug builds name the seeded demo sign-ins under the form; a Release build has no
            // password hashes and no hint (the literal is compiled out with the constant).
            lblDemo.Name = "lblDemo";
            lblDemo.Dock = DockStyle.Top;
            lblDemo.Height = 40;
            lblDemo.Font = Theme.Px(12f);
            lblDemo.ForeColor = Theme.Faint;
            lblDemo.AllowHtml = false;
            lblDemo.Text = DemoHint ?? "";
            lblDemo.Visible = DemoHint != null;
            btnSignIn.Name = "btnSignIn";
            btnSignIn.Text = "Sign in";
            Theme.StyleButton(btnSignIn, primary: false, size: 12.5f);
            btnSignIn.Dock = DockStyle.Right;
            btnSignIn.Width = 78;
            btnSignIn.Click += btnSignIn_Click;
            row.Controls.Add(txtPassword);
            row.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 10, BackColor = Color.Transparent });
            row.Controls.Add(txtUserName);
            row.Controls.Add(btnSignIn);

            // Dock order is the reverse of Controls.Add: add bottom-up.
            form.Controls.Add(lblDemo);
            form.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 8, BackColor = Color.Transparent });
            form.Controls.Add(row);
            form.Controls.Add(divider);
            form.Controls.Add(caption);
            form.Controls.Add(btnWorkAccount);
            form.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 18, BackColor = Color.Transparent });
            form.Controls.Add(noticeGap);
            form.Controls.Add(lblNotice);
            form.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 6, BackColor = Color.Transparent });
            form.Controls.Add(subtitle);
            form.Controls.Add(heading);

            this.Controls.Add(form);
            this.Controls.Add(_status);
            this.Controls.Add(appBar);

            switch (notice)
            {
                case LoginNotice.ExternalFailed:
                    lblNotice.Text = OpenIdConnectSetup.FailedSentence;
                    lblNotice.Visible = noticeGap.Visible = true;
                    _status.Show("Sign-in did not complete", StatusTone.Bad, "no grant created");
                    break;
                case LoginNotice.SignedOut:
                    _status.Show("You have signed out. Sign in to continue.", StatusTone.Idle, "grant revoked · provider session ended");
                    break;
                default:
                    _status.Show("Ready", StatusTone.Idle, "partner route · https");
                    break;
            }
        }


#if DEBUG
        /// <summary>The demo sign-ins a Debug build seeds (README "Demo accounts"); null in Release.</summary>
        private const string DemoHint = "Demo sign-ins (Debug build): work account dana.ellis@contoso.example / Demo-IdP-2026 · service account svc.reports / Reports-demo-31";
#else
        private const string DemoHint = null;
#endif

        // Federation first: a top-level navigation to /account/signin, where the challenge happens.
        private void btnWorkAccount_Click(object sender, EventArgs e)
        {
            _status.Show("Redirecting to " + _external.ProviderName, StatusTone.Idle, "correlation + nonce cookies set by the handler");
            _external.Begin();
        }

        // The Module 4 password form, for named service accounts only (LoginService decides).
        private void btnSignIn_Click(object sender, EventArgs e)
        {
            var userName = txtUserName.Text;
            var password = txtPassword.Text;
            txtPassword.Text = "";

            var result = _login.SignIn(userName, password, Application.UserHostAddress);
            password = null;

            if (!result.Succeeded)
            {
                _status.Show(result.Message, StatusTone.Bad, $"LoginThrottle · failures {result.Failures} of {LoginThrottle.AccountLimit}");
                return;
            }

            _status.Show("Signing in…", StatusTone.Ok, "service account");
            SignInHandoff.Park(result.SignIn);
        }
    }
}
