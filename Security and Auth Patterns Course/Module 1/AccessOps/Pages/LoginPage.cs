using System;
using System.Linq;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The Module 1 placeholder login. It checks no credential - AccessOps has no credential store
    /// yet - and it does not pretend to: the status line says what happened, and the generic
    /// failure names neither the user nor the reason. Module 2 replaces the body of
    /// <see cref="btnSignIn_Click"/> with a call to LoginService.
    /// </summary>
    public partial class LoginPage : Page
    {
        // The one sentence every refused sign-in gets, here and in every later module.
        internal const string GenericFailure = "That user name and password combination is not valid.";

        public LoginPage()
        {
            InitializeComponent();
            this.lblFooterMeta.Text = "placeholder · no credential is checked yet";
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            var user = this.txtUserName.Text.Trim();
            var password = this.txtPassword.Text;

            // validation path: say what is missing, nothing more
            if (user.Length == 0 || password.Length == 0)
            {
                this.lblStatus.Show("Enter your user name and password.", StatusTone.Bad);
                return;
            }

            // failure path: an entry that cannot be an AccessOps account gets the generic
            // sentence - no "no such user", no "wrong password", no echo of what was typed
            if (!LooksLikeAccountName(user))
            {
                this.txtPassword.Text = "";
                this.lblStatus.Show(GenericFailure, StatusTone.Bad);
                return;
            }

            // accepted path: the entry is well formed. Nobody is authenticated; the shell opens
            // with no identity in the session.
            this.txtPassword.Text = "";
            this.lblStatus.Show("Entry accepted. No credential is checked yet, so nobody is signed in.", StatusTone.Ok);
            Application.MainPage = new MainPage();
            this.Dispose();
        }

        private static bool LooksLikeAccountName(string user)
            => user.Length <= 64 && user.All(c => char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '_' || c == '@');
    }
}
