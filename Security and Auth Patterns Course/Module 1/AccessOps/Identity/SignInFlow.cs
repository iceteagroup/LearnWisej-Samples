using Wisej.Web;

namespace AccessOps.Identity
{
    /// <summary>
    /// The two halves of an AccessOps sign-in. The click (a WebSocket event) only verifies and
    /// parks the account; the page load that <c>Application.Reload()</c> causes rotates the
    /// session ID and establishes the principal. The parked account is not an identity yet:
    /// nothing reads the pending key except <see cref="CompletePending"/>.
    /// </summary>
    internal static class SignInFlow
    {
        /// <summary>The <c>Application.Session</c> key of the verified, not yet established account.</summary>
        public const string PendingKey = "AccessOps.PendingAccount";

        /// <summary>
        /// Runs on the sign-in click. Returns false when the verifier refused (or there is no
        /// verifier in this build); otherwise parks the account and reloads the page.
        /// </summary>
        public static bool Start(IAccountVerifier verifier, string userName, string secret)
        {
            var account = verifier?.Verify(userName, secret);
            if (account == null)
                return false;

            Application.Session[PendingKey] = account;
            Application.Reload();   // no RefreshSessionId here: a WebSocket click cannot rotate the ID
            return true;
        }

        /// <summary>
        /// Runs in <c>Application.ApplicationRefresh</c> (a page load). Takes the parked account,
        /// clears the key, rotates the session ID and sets <c>Application.User</c>.
        /// </summary>
        public static bool CompletePending()
        {
            if (Application.Session[PendingKey] is not Account account)
                return false;

            Application.Session[PendingKey] = null;
            ApplicationPrincipal.Establish(account.Id, account.Tenant, account.GrantId, account.Roles);
            return true;
        }
    }
}
