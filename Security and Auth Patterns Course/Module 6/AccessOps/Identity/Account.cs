namespace AccessOps.Identity
{
    /// <summary>
    /// A local AccessOps account as the account store returns it. <see cref="Id"/> is the
    /// immutable key every decision uses; <see cref="UserName"/> is only what the person types.
    /// The password hash never leaves the account store and <see cref="LoginService"/>.
    /// </summary>
    public sealed class Account
    {
        /// <summary>Immutable account ID (the subject of every claim and audit event).</summary>
        public string Id { get; init; }

        /// <summary>The sign-in name (stored lower case). Display only, never a key.</summary>
        public string UserName { get; init; }

        /// <summary>The name the console shows.</summary>
        public string DisplayName { get; init; }

        /// <summary>The tenant the account belongs to.</summary>
        public string Tenant { get; init; }

        /// <summary>True when AccessOps has approved the account's tenant.</summary>
        public bool TenantApproved { get; init; }

        /// <summary>The role a login grant gets ("Reader"). A label, not a permission.</summary>
        public string Role { get; init; }

        /// <summary>"interactive" (a person) or "service" (a named service account such as svc.reports).</summary>
        public string Kind { get; init; } = "interactive";

        /// <summary>True for a named service account.</summary>
        public bool IsService => this.Kind == "service";

        /// <summary>A disabled account can never sign in, whatever password is typed.</summary>
        public bool Disabled { get; init; }

        /// <summary>The PBKDF2 hash, or null when the account has no password.</summary>
        internal string PasswordHash { get; init; }
    }
}
