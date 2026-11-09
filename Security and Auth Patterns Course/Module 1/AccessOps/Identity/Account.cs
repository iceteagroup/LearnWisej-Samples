using System.Collections.Generic;

namespace AccessOps.Identity
{
    /// <summary>
    /// An account a trusted verifier returned. <see cref="Id"/> is the immutable key every
    /// decision uses; <see cref="UserName"/> is only what the person typed and never a key.
    /// </summary>
    public sealed class Account
    {
        /// <summary>Immutable account ID (the subject of every claim and audit event).</summary>
        public string Id { get; init; }

        /// <summary>The sign-in name. Display only.</summary>
        public string UserName { get; init; }

        /// <summary>The tenant the account belongs to.</summary>
        public string Tenant { get; init; }

        /// <summary>The account's grant ID, or null when the account has no grant.</summary>
        public string GrantId { get; init; }

        /// <summary>Role names at sign-in time. A role is a label, not a permission.</summary>
        public IReadOnlyList<string> Roles { get; init; } = new string[0];
    }

    /// <summary>
    /// Checks a sign-in name and secret and returns the account, or null. Module 1 ships only a
    /// training implementation (<c>TestVerifier</c>, Debug builds); Module 4 replaces it with
    /// AccessOps' own credential store.
    /// </summary>
    public interface IAccountVerifier
    {
        /// <summary>Returns the verified account, or null for any failure (no reason given).</summary>
        Account Verify(string userName, string secret);
    }
}
