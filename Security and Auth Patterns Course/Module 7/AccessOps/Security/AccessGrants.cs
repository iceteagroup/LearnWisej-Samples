using System;
using AccessOps.Identity;
using AccessOps.Infrastructure;

namespace AccessOps.Security
{
    /// <summary>
    /// The intranet route's grant lookup. The Windows account (its SID) is evidence of who signed
    /// in; the AccessOps grant issued for the account mapped to that SID is the permission, and it
    /// lives in the same durable <see cref="GrantStore"/> as every partner grant. Windows groups, the
    /// user name and the fact that the host authenticated the request decide nothing: an unmapped or
    /// disabled account gets no grant, and an account without the Reader role gets a grant without
    /// the read right - <see cref="RecordPolicy"/> denies both before the repository runs.
    /// </summary>
    public static class AccessGrants
    {
        /// <summary>
        /// The grant for this intranet sign-in of the account mapped to <paramref name="accountSid"/>
        /// (issued now, same shape and lifetime as a partner grant), or null when the Windows account
        /// is not mapped or its account is disabled.
        /// </summary>
        public static Grant FindForAccount(string accountSid)
        {
            var account = AppHost.Get<WindowsAccountMap>().FindAccount(accountSid);
            if (account == null || account.Disabled || !account.TenantApproved)
                return null;
            return AppHost.Get<GrantStore>().Issue(account.Id, account.Tenant, account.Role,
                canRead: string.Equals(account.Role, "Reader", StringComparison.Ordinal));
        }

        /// <summary>
        /// The verified intranet sign-in for <see cref="ApplicationPrincipal.Establish"/>: the mapped
        /// account, its tenant and the grant from <see cref="FindForAccount"/>. An unmapped Windows
        /// account becomes an authenticated-but-unknown subject (a hash of the SID, never the SID)
        /// with no grant, which every decision denies.
        /// </summary>
        public static VerifiedSignIn SignInFor(string accountSid, Grant grant, string maskedName, DateTimeOffset now)
        {
            var account = AppHost.Get<WindowsAccountMap>().FindAccount(accountSid);
            return new VerifiedSignIn
            {
                AccountId = account?.Id ?? Subject.UnmappedWindowsAccountId(accountSid),
                TenantId = account?.Tenant,
                GrantId = grant?.Id,
                Roles = account == null ? Array.Empty<string>() : new[] { account.Role },
                DisplayName = account?.DisplayName ?? maskedName,
                Method = "windows",
                VerifiedAt = now
            };
        }
    }
}
