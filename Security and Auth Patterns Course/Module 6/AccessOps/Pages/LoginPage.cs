using System;
using System.Drawing;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using AccessOps.Security;
using AccessOps.UI;
using Wisej.Ext.WebAuthn;
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
    /// The partner sign-in page of Module 6 ("AccessOps · Partner sign-in"): <b>Sign in with a
    /// passkey</b> (<see cref="btnPasskeySignIn_Click"/>: <c>WebAuthn.GetAsync</c> and the server-side
    /// checks of <see cref="PasskeyService"/>), <b>Sign in with your organization</b> (Module 5) and
    /// <b>Use your AccessOps password</b> (the Module 4 form, service accounts only), the
    /// "No passkey yet?" panel, the banner and the status strip with a reference.
    /// </summary>
    public class LoginPage : Page
    {
        private readonly LoginService _login = AppHost.Get<LoginService>();
        private readonly IExternalSignIn _external = AppHost.Get<IExternalSignIn>();
        private readonly PasskeyService _passkeys = AppHost.Get<PasskeyService>();

        private readonly Button btnPasskeySignIn = new Button();
        private readonly Button btnOrganization = new Button();
        private readonly Button btnPassword = new Button();
        private readonly Panel pnlPassword = new Panel();
        private readonly TextBox txtUserName = new TextBox();
        private readonly TextBox txtPassword = new TextBox();
        private readonly Label lblDemo = new Label();
        private readonly Button btnSignIn = new Button();
        private readonly Banner _banner = new Banner();
        private readonly StatusStrip _status = new StatusStrip();

        /// <summary>Creates the sign-in page with an optional notice.</summary>
        public LoginPage(LoginNotice notice = LoginNotice.None)
        {
            this.Name = "LoginPage";
            this.Text = "AccessOps";
            this.BackColor = Color.White;

            var appBar = new AppBar { Screen = "Partner sign-in" };

            // right: "No passkey yet?"
            var side = new Panel
            {
                Name = "pnlNoPasskey",
                Dock = DockStyle.Right,
                Width = 300,
                BackColor = Theme.Strip,
                Padding = new Padding(26, 30, 26, 24),
                CssStyle = "border-left:1px solid #e4eaf1;"
            };
            side.Controls.Add(new Label
            {
                Name = "lblLostDevice",
                Text = "Lost a device? Your other sign-in methods still work, and support can review your passkeys with you.",
                Dock = DockStyle.Bottom,
                Height = 60,
                Font = Theme.Px(12f),
                ForeColor = ColorTranslator.FromHtml("#7d8ea0")
            });
            side.Controls.Add(new Label
            {
                Name = "lblNoPasskeyText",
                Text = "Sign in with your organization or your password, then add a passkey from your account page.",
                Dock = DockStyle.Top,
                Height = 64,
                Font = Theme.Px(13.5f),
                ForeColor = ColorTranslator.FromHtml("#3a4d63")
            });
            side.Controls.Add(new Label
            {
                Name = "lblNoPasskey",
                Text = "NO PASSKEY YET?",
                Dock = DockStyle.Top,
                Height = 26,
                Font = Theme.Px(12f, FontStyle.Bold),
                ForeColor = Theme.Muted,
                CssStyle = "letter-spacing:.04em;"
            });

            // left: the three ways in
            var main = new Panel { Name = "pnlMain", Dock = DockStyle.Fill, Padding = new Padding(36, 30, 36, 16) };
            var column = new Panel { Name = "pnlColumn", Dock = DockStyle.Left, Width = 560 };

            var heading = new Label { Name = "lblHeading", Text = "Sign in to AccessOps", Dock = DockStyle.Top, Height = 32, Font = Theme.Px(22f, FontStyle.Bold), ForeColor = Theme.Ink };
            var subtitle = new Label { Name = "lblSubtitle", Text = "Partner route · approved partner tenants only", Dock = DockStyle.Top, Height = 24, Font = Theme.Px(13.5f), ForeColor = Theme.Muted };

            StyleWide(btnPasskeySignIn, "btnPasskeySignIn", "Sign in with a passkey", primary: true);
            btnPasskeySignIn.Click += btnPasskeySignIn_Click;
            StyleWide(btnOrganization, "btnOrganization", "Sign in with your organization", primary: false);
            btnOrganization.Click += (s, e) =>
            {
                _status.Show("Redirecting to " + _external.ProviderName, StatusTone.Idle, "partner route");
                _external.Begin();
            };
            StyleWide(btnPassword, "btnPassword", "Use your AccessOps password", primary: false);
            btnPassword.Click += (s, e) => { pnlPassword.Visible = !pnlPassword.Visible; };

            // the Module 4 form, kept for named service accounts only
            pnlPassword.Name = "pnlPassword";
            pnlPassword.Dock = DockStyle.Top;
            pnlPassword.Height = 66;
            pnlPassword.Visible = false;
            var caption = new Label { Text = "SERVICE ACCOUNTS ONLY", Dock = DockStyle.Top, Height = 22, Font = Theme.Px(11.5f, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#9aa9b8"), TextAlign = ContentAlignment.BottomLeft, CssStyle = "letter-spacing:.05em;" };
            // 4 px of padding keeps the focused input's glow inside the row (its pane clips overflow).
            var row = new Panel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(0, 4, 0, 4) };
            txtUserName.Name = "txtUserName";
            txtUserName.Watermark = "account name";
            Theme.StyleInput(txtUserName);
            txtUserName.Font = Theme.Px(12.5f);
            txtUserName.Height = 34;
            txtUserName.Dock = DockStyle.Left;
            txtUserName.Width = 210;
            txtPassword.Name = "txtPassword";
            txtPassword.Watermark = "password";
            txtPassword.InputType.Type = TextBoxType.Password;
            Theme.StyleInput(txtPassword);
            txtPassword.Font = Theme.Px(12.5f);
            txtPassword.Height = 34;
            txtPassword.Dock = DockStyle.Left;
            txtPassword.Width = 210;
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
            pnlPassword.Controls.Add(row);
            pnlPassword.Controls.Add(caption);

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

            _banner.Dock = DockStyle.Bottom;

            column.Controls.Add(lblDemo);
            column.Controls.Add(Gap(8));
            column.Controls.Add(pnlPassword);
            column.Controls.Add(Gap(10));
            column.Controls.Add(btnPassword);
            column.Controls.Add(Gap(12));
            column.Controls.Add(btnOrganization);
            column.Controls.Add(Gap(12));
            column.Controls.Add(btnPasskeySignIn);
            column.Controls.Add(Gap(20));
            column.Controls.Add(subtitle);
            column.Controls.Add(heading);
            column.Controls.Add(_banner);

            main.Controls.Add(column);

            this.Controls.Add(main);
            this.Controls.Add(side);
            this.Controls.Add(_status);
            this.Controls.Add(appBar);

            switch (notice)
            {
                case LoginNotice.ExternalFailed:
                    _banner.Show(OpenIdConnectSetup.FailedSentence, ok: false);
                    _status.Show("Sign-in did not complete", StatusTone.Bad, "no grant created");
                    break;
                case LoginNotice.SignedOut:
                    _status.Show("You have signed out. Sign in to continue.", StatusTone.Idle, "partner route");
                    break;
                default:
                    _status.Show("Ready", StatusTone.Idle, "partner route");
                    break;
            }
        }

        // Gaps are transparent so the focused input's 3 px glow is not painted over by a white panel.
        private static Panel Gap(int height) => new Panel { Dock = DockStyle.Top, Height = height, BackColor = Color.Transparent };
#if DEBUG
        /// <summary>The demo sign-ins a Debug build seeds (README "Demo accounts"); null in Release.</summary>
        private const string DemoHint = "Demo sign-ins (Debug build): organization account dana.ellis@contoso.example / Demo-IdP-2026, then enroll a passkey · AccessOps password svc.reports / Reports-demo-31";
#else
        private const string DemoHint = null;
#endif


        private static void StyleWide(Button button, string name, string text, bool primary)
        {
            button.Name = name;
            button.Text = text;
            button.Dock = DockStyle.Top;
            button.Height = primary ? 46 : 44;
            Theme.StyleButton(button, primary, size: primary ? 15f : 14f);
            button.CssStyle = primary
                ? "border-radius:8px; border:none; box-shadow:0 4px 12px rgba(21,101,216,.3);"
                : "border-radius:8px; border:1px solid #cdd9e6;";
        }

        // Passkey sign-in: a one-time challenge, the browser ceremony, then every check on the
        // server. Only an Updated commit creates a grant and runs the Module 4 hand-off.
        private async void btnPasskeySignIn_Click(object sender, EventArgs e)
        {
            _banner.Clear();
            _status.Show("Waiting for your authenticator…", StatusTone.Idle, "partner route");

            var challenge = _passkeys.BeginSignIn();
            PasskeyResult result;
            try
            {
                var response = await WebAuthn.GetAsync(challenge.Base64, null, 60000);
                result = _passkeys.CompleteSignIn(challenge.Id, response);
            }
            catch (Exception)
            {
                // cancelled prompt, timeout, no credential, insecure context: the browser's message
                // is not shown - every refusal gets the same fixed sentence
                result = _passkeys.BrowserFailed(challenge, PasskeyService.SignInType);
            }

            if (!result.Succeeded)
            {
                _banner.Show(PasskeyResult.SignInRefused, ok: false);
                _status.Show("Passkey sign-in refused · no grant", StatusTone.Bad, "reference " + result.Reference);
                AlertBox.Show(PasskeyResult.SignInRefused, MessageBoxIcon.Warning, allowHtml: false);
                Application.Update(this);
                return;
            }

            _status.Show("Passkey verified · grant issued", StatusTone.Ok, "reference " + result.Reference);
            Application.Session["PasskeyReference"] = result.Reference;
            Application.Update(this);
            SignInHandoff.Park(result.SignIn);   // reload → RefreshSessionId() → Application.User
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
