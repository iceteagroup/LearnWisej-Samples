using System.Collections.Generic;

namespace AccessOps.Identity
{
    /// <summary>
    /// An AccessOps account. <see cref="Id"/> is the immutable key every decision uses;
    /// <see cref="UserName"/> is a display name and never a key. On the intranet route a Windows
    /// account (SID) is mapped to an account (<c>WindowsAccountMap</c>).
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

}
