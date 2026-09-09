using System.Collections.Generic;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// What the identity provider asserts about a signed-in person: the verified claims a
    /// <see cref="SessionContext"/> is built from. Nothing in here comes from a UI control.
    /// </summary>
    public sealed class VerifiedIdentity
    {
        public string UserId { get; }
        public string DisplayName { get; }
        public IReadOnlyList<string> Roles { get; }
        public IReadOnlyList<string> Permissions { get; }
        /// <summary>The tenants this person may work in. The session's current tenant must be one of them.</summary>
        public IReadOnlyList<string> EntitledTenants { get; }
        public string DefaultTenantId { get; }
        public string Culture { get; }

        public VerifiedIdentity(string userId, string displayName, IReadOnlyList<string> roles,
            IReadOnlyList<string> permissions, IReadOnlyList<string> entitledTenants, string defaultTenantId, string culture)
        {
            UserId = userId;
            DisplayName = displayName;
            Roles = roles;
            Permissions = permissions;
            EntitledTenants = entitledTenants;
            DefaultTenantId = defaultTenantId;
            Culture = culture;
        }
    }
}
