using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// The authentication gate — a **simulation** of the OIDC/SSO boundary.
    ///
    /// There is no identity provider, no token signature and, deliberately, no password field: this screen picks
    /// which identity the provider should assert and shows exactly what crosses the boundary — a list of claims.
    /// Everything to the right of that list is the application's own responsibility, which is the whole subject
    /// of the module.
    ///
    /// The screen owns no security decision. It calls <see cref="SignInService.SignInAsync"/>, which maps the
    /// claims, fills the session and registers the membership, and it closes with <c>DialogResult.OK</c> only
    /// when that service says a verified identity exists.
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

        /// <summary>The identity the gate signed in, for the caller's banner. Null unless the result was OK.</summary>
        public MappedIdentity Identity { get; private set; }

        /// <summary>Everything the signed-in identity may do — the caller shows it once, then asks the services.</summary>
        public IReadOnlyList<Permission> Permissions { get; private set; } = Array.Empty<Permission>();

        #region Event handlers — thin, one service call each

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

            lblMapped.Text = "Press Sign in to run ClaimsMapper over these claims.";
        }

        /// <summary>The gate itself: one service call, one result, no security logic in the handler.</summary>
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

        #region Showing results — UI state only

        private void ShowResult(SignInResult result)
        {
            if (!result.Succeeded)
            {
                ShowError(result.FirstError);
                return;
            }

            Identity = result.Identity;
            Permissions = result.Permissions;

            // "Authenticated, entitled to nothing" is a successful sign-in: the gate closes, and the screen
            // behind it shows the warning and refuses every action. Authentication is not authorization.
            lblMapped.Text = Describe(result);
            DialogResult = DialogResult.OK;
            Close();
        }

        private static string Describe(SignInResult result)
            => $"user   {result.Identity.UserId}\n"
             + $"tenant {result.Identity.TenantId}\n"
             + $"roles  {result.Identity.RoleList}\n"
             + $"grants {(result.Permissions.Count == 0 ? "(none)" : string.Join(", ", result.Permissions))}";

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
