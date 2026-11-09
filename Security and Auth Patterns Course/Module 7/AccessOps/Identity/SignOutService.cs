using AccessOps.Security;

namespace AccessOps.Identity
{
    /// <summary>
    /// Sign-out is revocation of server-side grants, not a hidden button or a deleted cookie. It
    /// revokes every active login grant of the account - so a second tab or a second browser
    /// profile signed in as the same partner is denied on its next protected click - and audits
    /// it. The caller then clears the session identity, expires the host cookie through
    /// <c>/auth/signout</c> and ends the Wisej.NET session.
    /// </summary>
    public sealed class SignOutService
    {
        private readonly GrantStore _grants;
        private readonly LoginAudit _audit;

        /// <summary>Creates the service.</summary>
        public SignOutService(GrantStore grants, LoginAudit audit)
        {
            _grants = grants;
            _audit = audit;
        }

        /// <summary>Revokes the subject's grants and audits the sign-out. Returns how many grants were revoked.</summary>
        public int SignOut(Subject subject, string clientAddress)
        {
            if (subject == null || subject.IsAnonymous)
                return 0;

            var revoked = _grants.RevokeAllForAccount(subject.AccountId);
            _audit.Write(subject.Opaque, Subject.OpaqueValue("a-", clientAddress ?? "unknown"), LoginOutcome.SignedOut,
                AuditService.NewCorrelationId(), Grant.Shorten(subject.GrantId) + " +" + revoked + " revoked");
            return revoked;
        }
    }
}
