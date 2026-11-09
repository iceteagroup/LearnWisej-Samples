using AccessOps.Infrastructure;

namespace AccessOps.Identity
{
    /// <summary>
    /// Maps a validated issuer + subject to a local AccessOps account through
    /// <c>external_logins</c> - never by e-mail address, which a provider can reassign. It refuses
    /// an unknown pair, a disabled account and an account whose tenant AccessOps has not approved
    /// (fixture F08). Tenant and roles are read from AccessOps data; nothing in the token becomes a
    /// permission. It creates nothing: the caller issues the one grant when it succeeds.
    /// </summary>
    public sealed class ExternalIdentityMapper
    {
        private readonly AccountStore _accounts;

        /// <summary>Creates the mapper over the account store.</summary>
        public ExternalIdentityMapper(AccountStore accounts)
        {
            _accounts = accounts;
        }

        /// <summary>Maps <paramref name="identity"/> or says why not.</summary>
        public ExternalMapResult Map(ExternalIdentity identity)
        {
            var account = _accounts.FindByExternalLogin(identity?.Issuer, identity?.Subject);
            if (account == null)
                return new ExternalMapResult { Reason = "unknown_login" };
            if (account.Disabled)
                return new ExternalMapResult { Reason = "disabled" };
            if (!account.TenantApproved)
                return new ExternalMapResult { Reason = "unapproved_tenant" };
            if (account.IsService)
                return new ExternalMapResult { Reason = "service_account" };
            return new ExternalMapResult { Account = account };
        }
    }
}
