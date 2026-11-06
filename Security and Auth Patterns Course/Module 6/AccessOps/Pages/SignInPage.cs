using System;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The AccessOps sign-in page from Module 4 on. The work account goes to the identity provider
    /// through the one seam, <see cref="IExternalSignIn"/>; the Module 2 password form stays below for
    /// named service accounts only, rate limited and audited by <see cref="LoginService"/>.
    /// </summary>
    public partial class SignInPage : Page
    {
        // The one sentence every refused password sign-in gets.
        internal const string GenericFailure = "That user name and password combination is not valid.";

        private readonly LoginService _login = AppHost.Get<LoginService>();
        private readonly IExternalSignIn _external = AppHost.Get<IExternalSignIn>();

        public SignInPage(string notice = null)
        {
            InitializeComponent();
            this.statusStrip.Report("Ready", StatusTone.Idle, "authorization code flow · PKCE");
            ShowNotice(notice);
        }

        internal void ShowNotice(string notice)
        {
            if (notice == null)
                return;
            this.lblStatus.Show(notice, StatusTone.Bad);
            this.statusStrip.Report("Ready", StatusTone.Idle, "authorization code flow · PKCE");
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            this.statusStrip.Report("Redirecting to " + _external.ProviderName + "…", StatusTone.Idle, "no password stored here");
            Application.Update(this);
            _external.Begin();
        }

        private void btnServiceSignIn_Click(object sender, EventArgs e)
        {
            var user = this.txtUserName.Text;
            var password = this.txtPassword.Text;
            this.txtPassword.Text = "";   // the password does not outlive this handler

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrEmpty(password))
            {
                this.lblStatus.Show("Enter the service account name and password.", StatusTone.Bad);
                return;
            }

            var result = _login.SignIn(user, password);
            if (!result.Succeeded)
            {
                this.lblStatus.Show(GenericFailure, StatusTone.Bad);
                this.statusStrip.Report("Refused", StatusTone.Bad,
                    $"LoginThrottle · failures {Math.Min(_login.FailuresFor(user), LoginThrottle.MaxAccountFailures)} of {LoginThrottle.MaxAccountFailures}");
                return;
            }

            // verified: the page reloads, the session ID is renewed, then the console replaces this page
            this.lblStatus.Show("Signing in…", StatusTone.Ok);
        }
    }
}
