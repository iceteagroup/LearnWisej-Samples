using AccessOps.Security;

namespace AccessOps.Identity
{
    /// <summary>
    /// What the intranet start-up found for this session: the masked Windows account (display
    /// only), the AccessOps subject mapped from its SID and the grant as it was at start-up (display
    /// only). Nothing here is a permission: every read goes back to <c>RecordPolicy</c>, which
    /// reads the grant's current state.
    /// </summary>
    public sealed class IntranetUser
    {
        /// <summary>Creates the session's intranet user.</summary>
        public IntranetUser(string maskedAccount, Subject subject, Grant grantAtStart)
        {
            this.MaskedAccount = maskedAccount;
            this.Subject = subject;
            this.GrantText = grantAtStart == null
                ? "none"
                : $"{grantAtStart.Role} · tenant {grantAtStart.Tenant}";
        }

        /// <summary>The Windows account as the screen shows it ("CONTOSO\re•••1").</summary>
        public string MaskedAccount { get; }

        /// <summary>The AccessOps subject (account ID, tenant, grant ID) mapped from the SID.</summary>
        public Subject Subject { get; }

        /// <summary>"Reader · tenant A", or "none" when the account has no grant.</summary>
        public string GrantText { get; }
    }
}
