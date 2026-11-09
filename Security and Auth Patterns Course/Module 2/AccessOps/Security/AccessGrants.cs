using AccessOps.Identity;
using AccessOps.Infrastructure;

namespace AccessOps.Security
{
    /// <summary>
    /// The intranet route's grant lookup. The Windows account (its SID) is evidence of who signed
    /// in; the AccessOps grant mapped to that account is the permission, and it is read from the
    /// grant store - the Windows groups, the user name and the fact that the host authenticated the
    /// request decide nothing.
    /// </summary>
    public static class AccessGrants
    {
        /// <summary>
        /// The current grant of the AccessOps account mapped to <paramref name="accountSid"/>, or
        /// null when the Windows account is not mapped or its account has no grant.
        /// <c>DocumentService</c> denies a null grant before the repository runs.
        /// </summary>
        public static Grant FindForAccount(string accountSid)
        {
            var account = AppHost.Get<WindowsAccountMap>().FindAccount(accountSid);
            return account?.GrantId == null ? null : AppHost.Get<GrantStore>().Find(account.GrantId);
        }

        /// <summary>
        /// The AccessOps subject for the Windows account: the mapped account ID, its tenant and
        /// grant ID; an unmapped account gets an authenticated-but-unknown subject that every
        /// decision denies (opaque in the audit, never the SID).
        /// </summary>
        public static Subject SubjectForAccount(string accountSid)
        {
            var account = AppHost.Get<WindowsAccountMap>().FindAccount(accountSid);
            return account == null
                ? Subject.UnmappedWindowsAccount(accountSid)
                : new Subject(account.Id, account.Tenant, account.GrantId, account.Roles);
        }
    }
}
