using System;
using TicketOps.Controls;
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
    /// The password is read from the TextBox and handed to the service — it is never logged, never
    /// audited and never compared here.
    /// </summary>
    public partial class LoginView : Form
    {
        private readonly IAuthenticationService _auth;
        private readonly ILog _log;
        private readonly Func<Form> _openWorkspace;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public LoginView() : this(null, new ActivityLog(), null)
        {
        }

        public LoginView(IAuthenticationService auth, ILog log, Func<Form> openWorkspace)
        {
            InitializeComponent();

            _auth = auth;
            _log = log;
            _openWorkspace = openWorkspace;
        }

        private void LoginView_Load(object sender, EventArgs e)
        {
            this.statusBanner.SetStatus("signed out", StatusKind.Normal);
            this.textUser.Focus();
        }

        private void ShowSignInFailure(string message)
        {
            // Expected outcome: the service explained it in words the user may read (and nothing more).
            this.statusBanner.ShowBanner(message, StatusKind.Warning);
            this.statusBanner.SetStatus("not signed in", StatusKind.Warning);
            this.textPassword.Text = string.Empty;
            this.textPassword.Focus();
        }

        /// <summary>
        /// Unexpected failure (the directory is down): details go to the log, the user sees one calm sentence.
        /// </summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex);
            this.statusBanner.ShowBanner("✖ " + Strings.SignInUnavailable, StatusKind.Error);
            this.statusBanner.SetStatus("unavailable", StatusKind.Error);
            AlertBox.Show(Strings.SignInUnavailable, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>Read the form, ask the service, open the workspace when the identity is bound.</summary>
        private async void buttonSignIn_Click(object sender, EventArgs e)
        {
            try
            {
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus("checking", StatusKind.Busy);

                var result = await _auth.SignInAsync(this.textUser.Text, this.textPassword.Text);   // the decision lives in the service
                if (!result.Succeeded)
                {
                    ShowSignInFailure(result.Message);
                    return;
                }

                var workspace = _openWorkspace();
                workspace.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ReportFailure("LoginView.buttonSignIn_Click", ex);
            }
        }
    }
}
