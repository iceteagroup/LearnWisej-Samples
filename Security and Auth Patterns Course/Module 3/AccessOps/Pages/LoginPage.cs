using System;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The AccessOps sign-in page. The handler reads the two boxes and calls
    /// <see cref="LoginService"/> - nothing else. Every refusal, whatever its real reason, is the
    /// same sentence; the reason is in the login_audit table.
    /// </summary>
    public partial class LoginPage : Page
    {
        // The one sentence every refused sign-in gets.
        internal const string GenericFailure = "That user name and password combination is not valid.";
        private const string DefaultMeta = "PasswordService · PBKDF2-SHA256 · 210,000 iterations";

        private readonly LoginService _login = AppHost.Get<LoginService>();
        private readonly RememberedDeviceService _devices = AppHost.Get<RememberedDeviceService>();

        public LoginPage(string notice = null, string meta = null)
        {
            InitializeComponent();
            this.lblFooterMeta.Text = meta ?? DefaultMeta;
            if (notice != null)
                this.lblStatus.Show(notice, StatusTone.Bad);
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            var user = this.txtUserName.Text;
            var password = this.txtPassword.Text;

            // the password does not outlive this handler: not on the page, not in a field
            this.txtPassword.Text = "";

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrEmpty(password))
            {
                this.lblStatus.Show("Enter your user name and password.", StatusTone.Bad);
                return;
            }

            this.btnSignIn.Text = "Checking…";
            this.btnSignIn.Enabled = false;
            Application.Update(this);

            LoginResult result;
            try
            {
                result = _login.SignIn(user, password);
            }
            finally
            {
                this.btnSignIn.Text = "Sign in";
                this.btnSignIn.Enabled = true;
            }

            if (!result.Succeeded)
            {
                this.lblStatus.Show(GenericFailure, StatusTone.Bad);
                this.lblFooterMeta.Text = $"LoginThrottle · failures {Math.Min(_login.FailuresFor(user), LoginThrottle.MaxAccountFailures)} of {LoginThrottle.MaxAccountFailures}";
                return;
            }

            // The verified identity is parked and the page reloads: on that page load
            // (Program.OnPageLoad) the session gets a new ID and only then the SessionContext,
            // and the console replaces this page.
            this.lblStatus.Show("Signing in…", StatusTone.Ok);

            if (this.chkRemember.Checked)
            {
                // The device cookie is HttpOnly, and an HttpOnly cookie can only be written outside
                // a WebSocket event: it is written on the same page load.
                var token = _devices.Issue(result.EmployeeId, out var expires);
                Application.Session["PendingDeviceCookie"] = new PendingDeviceCookie(token, expires);
            }
        }
    }

    internal sealed record PendingDeviceCookie(string Token, DateTime ExpiresUtc);
}
