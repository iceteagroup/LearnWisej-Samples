using System;
using TicketOps.Controls;
using TicketOps.Data;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Security;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Sign in — the login gate. The console starts closed: this is the first screen
    /// of every session and the Work Orders screen is created only after IAuthenticationService verified a
    /// credential on the server and bound an identity to the session.
    ///
    /// Left card:   user name / password, the demo accounts (lab only) and the server-side identity read-out.
    /// Right card:  the activity trace — watch the credential travel UI → SVC → DATA and the identity land
    ///              in [SESSION] and [AUDIT].
    /// Bottom bar:  the failure paths (wrong password, unknown user), the error path with recovery
    ///              (user store outage) and Clear trace. Success is the Sign in button itself.
    ///
    /// The password is read from the TextBox and handed to the service — it is never logged, never
    /// audited and never compared here.
    /// </summary>
    public partial class LoginView : Form
    {
        private readonly IAuthenticationService _auth;
        private readonly InMemoryUserStore _userStore;      // only for the lab's outage switch
        private readonly IUserSession _session;
        private readonly ILog _log;
        private readonly Func<Form> _openWorkspace;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public LoginView() : this(null, null, null, new ActivityLog(), null)
        {
        }

        public LoginView(IAuthenticationService auth, InMemoryUserStore userStore, IUserSession session, ILog log, Func<Form> openWorkspace)
        {
            InitializeComponent();

            _auth = auth;
            _userStore = userStore;
            _session = session;
            _log = log;
            _openWorkspace = openWorkspace;

            if (log is ActivityLog activityLog)
                this.tracePanel.Attach(activityLog);
        }

        #region Screen lifecycle

        private void LoginView_Load(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "LoginView.Load", "login gate shown — nothing else opens before an identity is bound to the session");
            _log.Info(LogLayer.Session, "LoginView.Load", WisejSessionBinding.Describe());
            ShowIdentity();
            this.statusBanner.SetStatus("signed out", StatusKind.Normal);
            this.textUser.Focus();
        }

        private void LoginView_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.tracePanel.Attach(null);       // stop following the session log once the screen is gone
        }

        #endregion

        #region data → UI

        /// <summary>The server-side view of "who is this": what Wisej.NET says and what the app's session says.</summary>
        private void ShowIdentity()
        {
            string appUser = _session == null || _session.User == null ? "—" : _session.User.ToString();
            this.labelIdentity.Text =
                $"IUserSession.User = {appUser}\n" +
                WisejSessionBinding.Describe().Replace(" · ", "\n");
        }

        private void ShowSignInFailure(string message)
        {
            // Expected outcome: the service explained it in words the user may read (and nothing more).
            this.statusBanner.ShowBanner(message, StatusKind.Warning);
            this.statusBanner.SetStatus("not signed in", StatusKind.Warning);
            this.textPassword.Text = string.Empty;
            this.textPassword.Focus();
            _log.Warn(LogLayer.UI, "LoginView.ShowResult", $"FAIL · {message}");
        }

        /// <summary>
        /// Unexpected failure (the directory is down): details go to the log with the exception type,
        /// the user sees one calm sentence. The LDAP host name never crosses into the banner.
        /// </summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message");
            this.statusBanner.ShowBanner("✖ " + Strings.SignInUnavailable, StatusKind.Error);
            this.statusBanner.SetStatus("unavailable", StatusKind.Error);
            AlertBox.Show(Strings.SignInUnavailable, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Thin handlers

        /// <summary>Success path: read the form, ask the service, open the workspace when the identity is bound.</summary>
        private async void buttonSignIn_Click(object sender, EventArgs e)
        {
            try
            {
                await SignInAsync(this.textUser.Text, this.textPassword.Text, "LoginView.buttonSignIn_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("LoginView.buttonSignIn_Click", ex);
            }
        }

        private async System.Threading.Tasks.Task SignInAsync(string userName, string password, string source)
        {
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("checking", StatusKind.Busy);
            _log.Info(LogLayer.UI, source, $"→ IAuthenticationService.SignInAsync(\"{HtmlPolicy.Encode(userName)}\", password: not logged)");

            var result = await _auth.SignInAsync(userName, password);           // the decision lives in the service
            ShowIdentity();

            if (!result.Succeeded)
            {
                ShowSignInFailure(result.Message);
                return;
            }

            this.statusBanner.SetStatus(result.Message, StatusKind.Success);
            _log.Info(LogLayer.UI, source, $"OK · {result.Message} → opening Work Orders");

            var workspace = _openWorkspace();
            workspace.Show();
            this.Close();
        }

        private void buttonUseTechnician_Click(object sender, EventArgs e) => FillDemoAccount("l.romero");
        private void buttonUseSupervisor_Click(object sender, EventArgs e) => FillDemoAccount("m.weber");
        private void buttonUseAdmin_Click(object sender, EventArgs e) => FillDemoAccount("s.okafor");

        /// <summary>Lab convenience only: fills the form with a demo account. The server still verifies it.</summary>
        private void FillDemoAccount(string userName)
        {
            this.textUser.Text = userName;
            this.textPassword.Text = InMemoryUserStore.DemoPassword;
            this.statusBanner.HideBanner();
            _log.Info(LogLayer.UI, "LoginView.FillDemoAccount", $"form filled with demo account \"{userName}\" (display only — Sign in still goes through the service)");
            this.buttonSignIn.Focus();
        }

        #endregion

        #region Bottom bar: failures, outage and recovery

        /// <summary>Failure path 1: a known user with the wrong password. The store finds the user; the service still says no.</summary>
        private async void buttonWrongPassword_Click(object sender, EventArgs e)
        {
            try
            {
                this.textUser.Text = "l.romero";
                this.textPassword.Text = "guess-1234";
                await SignInAsync(this.textUser.Text, this.textPassword.Text, "LoginView.buttonWrongPassword_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("LoginView.buttonWrongPassword_Click", ex);
            }
        }

        /// <summary>Failure path 2: an unknown user. Same neutral message as the wrong password — the UI must not tell them apart.</summary>
        private async void buttonUnknownUser_Click(object sender, EventArgs e)
        {
            try
            {
                this.textUser.Text = "j.doe";
                this.textPassword.Text = InMemoryUserStore.DemoPassword;
                await SignInAsync(this.textUser.Text, this.textPassword.Text, "LoginView.buttonUnknownUser_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("LoginView.buttonUnknownUser_Click", ex);
            }
        }

        /// <summary>Error path + recovery: toggle the user store outage, then try a valid sign-in through the service.</summary>
        private async void buttonStoreOutage_Click(object sender, EventArgs e)
        {
            if (_userStore == null)
                return;

            try
            {
                _userStore.SimulateOutage = !_userStore.SimulateOutage;
                this.buttonStoreOutage.Text = _userStore.SimulateOutage ? "Recover the user store" : "Simulate user store outage";
                _log.Info(LogLayer.UI, "LoginView.buttonStoreOutage_Click",
                    _userStore.SimulateOutage ? "outage ON → sign in (expect ✖ in DATA, safe message in UI)" : "outage OFF → sign in again (recovery)");

                this.textUser.Text = "l.romero";
                this.textPassword.Text = InMemoryUserStore.DemoPassword;
                await SignInAsync(this.textUser.Text, this.textPassword.Text, "LoginView.buttonStoreOutage_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("LoginView.buttonStoreOutage_Click", ex);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.tracePanel.ClearTrace();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("signed out", StatusKind.Normal);
        }

        #endregion
    }
}
