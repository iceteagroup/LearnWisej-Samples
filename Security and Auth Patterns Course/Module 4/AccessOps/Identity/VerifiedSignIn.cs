using System;
using System.Collections.Generic;

namespace AccessOps.Identity
{
    /// <summary>
    /// The result of a sign-in transaction that passed every check: the account, its tenant and
    /// roles from AccessOps data, and the login grant created for it. It is parked under
    /// <see cref="SignInHandoff.PendingKey"/> between the click and the page load. It has no
    /// field for a password, a session ID or a token - there is nowhere to put one.
    /// </summary>
    public sealed class VerifiedSignIn
    {
        /// <summary>Immutable account ID.</summary>
        public string AccountId { get; init; }

        /// <summary>The account's tenant (from AccessOps data).</summary>
        public string TenantId { get; init; }

        /// <summary>The login grant created for this sign-in.</summary>
        public string GrantId { get; init; }

        /// <summary>Role names from AccessOps data.</summary>
        public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();

        /// <summary>The name the console shows.</summary>
        public string DisplayName { get; init; }

        /// <summary>How the account proved itself ("password").</summary>
        public string Method { get; init; } = "password";

        /// <summary>When the credential was verified.</summary>
        public DateTimeOffset VerifiedAt { get; init; }
    }
}
