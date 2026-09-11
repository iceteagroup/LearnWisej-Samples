using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// The authentication gate — a simulation of the OIDC/SSO sign-in.
    ///
    /// There is no identity provider, no token signature and no password field: the gate picks which identity
    /// the provider should assert and shows the claims that cross the boundary. It calls
    /// <see cref="SignInService.SignInAsync"/>, which maps the claims, fills the session and registers the
    /// membership, and it closes with <c>DialogResult.OK</c> only when that service says a verified identity exists.
    /// </summary>
    public partial class SignInGate : Form
    {
        private readonly SignInService _signIn;
        private List<SsoAccount> _directory;

        public SignInGate(SignInService signIn)
        {
            InitializeComponent();
            _signIn = signIn;
        }

        /// <summary>The identity the gate signed in. Null unless the result was OK.</summary>
        public MappedIdentity Identity { get; private set; }

        #region Event handlers

        private void SignInGate_Load(object sender, EventArgs e)
        {
            _directory = _signIn.Directory.ToList();

            foreach (SsoAccount account in _directory)
                lstIdentities.Items.Add($"{account.Subject,-12} {account.DisplayName}");

            lstIdentities.SelectedIndex = 0;
        }

        /// <summary>Selecting an account previews its claims — nothing is signed in and nothing is audited yet.</summary>
        private void lstIdentities_SelectedIndexChanged(object sender, EventArgs e)
        {
            SsoAccount account = Selected;
            if (account == null) return;

            lblAccountDescription.Text = account.Description;
            HideError();

            lstClaims.Items.Clear();
            foreach (SsoClaim claim in _signIn.PreviewClaims(account.Subject) ?? new List<SsoClaim>())
                lstClaims.Items.Add($"{claim.Type,-10} {claim.Value}");
        }

        private async void btnSignIn_Click(object sender, EventArgs e)
        {
            SsoAccount account = Selected;
            if (account == null) return;

            SetBusy(true);
            try
            {
                SignInResult result = await _signIn.SignInAsync(account.Subject);
                ShowResult(result);
            }
            catch (Exception ex)
            {
                ShowError("Sign-in could not be completed. " + ex.GetType().Name);
                AlertBox.Show("Sign-in could not be completed.", MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
            finally
            {
                SetBusy(false);
                Application.Update(this);
            }
        }

        /// <summary>Closing without an identity is a legitimate outcome: the screen behind stays locked.</summary>
        private void btnStaySignedOut_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

        #region Showing results

        private void ShowResult(SignInResult result)
        {
            if (!result.Succeeded)
            {
                ShowError(result.FirstError);
                return;
            }

            // "Authenticated, entitled to nothing" is a successful sign-in: the gate closes, and the screen
            // behind it shows the warning and every service refuses.
            Identity = result.Identity;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ShowError(string message)
        {
            lblGateError.Text = message;
            lblGateError.Visible = true;
        }

        private void HideError()
        {
            lblGateError.Visible = false;
        }

        private void SetBusy(bool busy)
        {
            btnSignIn.Enabled = !busy;
            lstIdentities.Enabled = !busy;
            btnSignIn.Text = busy ? "Signing in…" : "Sign in";
        }

        private SsoAccount Selected
            => lstIdentities.SelectedIndex >= 0 && lstIdentities.SelectedIndex < _directory.Count
                ? _directory[lstIdentities.SelectedIndex]
                : null;

        #endregion
    }
}
