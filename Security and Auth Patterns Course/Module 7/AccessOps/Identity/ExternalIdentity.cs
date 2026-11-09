namespace AccessOps.Identity
{
    /// <summary>
    /// What AccessOps keeps from a validated ID token: the issuer and the subject - the only
    /// stable external account key. No e-mail, no name, no token. The tenant and roles come from
    /// AccessOps data (<see cref="ExternalIdentityMapper"/>), never from a claim.
    /// </summary>
    public sealed record ExternalIdentity(string Issuer, string Subject)
    {
        /// <summary>The subject shortened for log lines ("…dana-ellis" style tail, 6 characters).</summary>
        public string RedactedSubject => "sub …" + Redact.Tail(this.Subject, 6);
    }

    /// <summary>The mapper's answer.</summary>
    public sealed class ExternalMapResult
    {
        /// <summary>The local account, when the mapping passed every check.</summary>
        public Account Account { get; init; }

        /// <summary>The allowlisted refusal reason ("unknown_login", "disabled", "unapproved_tenant"), or null.</summary>
        public string Reason { get; init; }

        /// <summary>True when <see cref="Account"/> may sign in.</summary>
        public bool Succeeded => this.Account != null;
    }
}
