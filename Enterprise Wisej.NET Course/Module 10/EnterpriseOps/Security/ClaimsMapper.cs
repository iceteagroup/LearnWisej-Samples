using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Diagnostics;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// The result of mapping a set of provider claims into the application's own vocabulary. This — not the raw
    /// claim list — is what the session and the services see.
    /// </summary>
    public sealed class MappedIdentity
    {
        public MappedIdentity(string userId, string displayName, string email, string tenantId,
            IReadOnlyList<Role> roles, IReadOnlyList<string> unmappedGroups, string authMethod, DateTime authTimeUtc)
        {
            UserId = userId;
            DisplayName = displayName;
            Email = email;
            TenantId = tenantId;
            Roles = roles;
            UnmappedGroups = unmappedGroups;
            AuthMethod = authMethod;
            AuthTimeUtc = authTimeUtc;
        }

        public string UserId { get; }
        public string DisplayName { get; }
        public string Email { get; }
        public string TenantId { get; }
        public IReadOnlyList<Role> Roles { get; }

        /// <summary>Groups the provider sent that this application has no rule for. They grant nothing, and they are visible.</summary>
        public IReadOnlyList<string> UnmappedGroups { get; }

        public string AuthMethod { get; }
        public DateTime AuthTimeUtc { get; }

        public string RoleList => Roles.Count == 0 ? "(none)" : string.Join("+", Roles);

        public override string ToString() => $"{UserId}@{TenantId} · {RoleList}";
    }

    /// <summary>Thrown when the claims cannot be mapped at all — a missing subject or a missing/unknown tenant.</summary>
    public sealed class ClaimsMappingException : Exception
    {
        public ClaimsMappingException(string message) : base(message)
        {
        }
    }

    public interface IClaimsMapper
    {
        MappedIdentity Map(IReadOnlyList<SsoClaim> claims);
    }

    /// <summary>
    /// Claims mapping — the whole point of the layer: provider-specific strings go in, application roles come out,
    /// and **no authorization code anywhere else in the application ever parses a claim**. Swap the identity
    /// provider and only this file changes.
    ///
    /// Three rules the lab makes visible:
    ///  1. Unknown group ⇒ nothing. It is never "close enough" to a role, and it is never a default role.
    ///  2. The tenant comes from the token, not from a dropdown, a query string or a hidden field.
    ///  3. What could not be mapped is reported, not swallowed — see <see cref="MappedIdentity.UnmappedGroups"/>.
    /// </summary>
    public sealed class ClaimsMapper : IClaimsMapper
    {
        /// <summary>
        /// The one table that couples this application to the identity provider's directory. In production it lives
        /// in configuration so a new group can be onboarded without a deployment; here it is a constant so the lab
        /// can read it.
        /// </summary>
        private static readonly Dictionary<string, Role> GroupToRole = new Dictionary<string, Role>(StringComparer.OrdinalIgnoreCase)
        {
            ["EnterpriseOps-Technicians"] = Role.Technician,
            ["EnterpriseOps-Managers"] = Role.Manager,
            ["EnterpriseOps-Admins"] = Role.Admin,
            ["EnterpriseOps-Auditors"] = Role.Auditor,
            ["EnterpriseOps-Integration"] = Role.ServiceAccount,
        };

        private readonly ActivityTrace _trace;

        public ClaimsMapper(ActivityTrace trace)
        {
            _trace = trace;
        }

        /// <summary>The mapping table, for the identity-mapping design document and the sign-in screen.</summary>
        public static IReadOnlyDictionary<string, Role> Mappings => GroupToRole;

        public MappedIdentity Map(IReadOnlyList<SsoClaim> claims)
        {
            if (claims == null || claims.Count == 0)
                throw new ClaimsMappingException("No claims were presented — the identity provider did not authenticate anyone.");

            string subject = First(claims, ClaimTypes.Subject);
            if (string.IsNullOrWhiteSpace(subject))
                throw new ClaimsMappingException("The token carries no 'sub' claim; the application cannot name the caller.");

            string tenantId = First(claims, ClaimTypes.TenantId);
            if (string.IsNullOrWhiteSpace(tenantId) || Domain.Tenants.Find(tenantId) == null)
                throw new ClaimsMappingException($"The 'tid' claim '{tenantId ?? "(missing)"}' is not a tenant of this application.");

            var groups = claims.Where(c => c.Type == ClaimTypes.Groups).Select(c => c.Value).ToList();
            var roles = new List<Role>();
            var unmapped = new List<string>();

            foreach (string group in groups)
            {
                if (GroupToRole.TryGetValue(group, out Role role))
                {
                    roles.Add(role);
                    _trace?.Identity($"ClaimsMapper — group '{group}' → Role.{role}");
                }
                else
                {
                    unmapped.Add(group);
                    _trace?.Identity($"ClaimsMapper — group '{group}' has no rule → grants NOTHING (recorded, not guessed)");
                }
            }

            DateTime.TryParse(First(claims, ClaimTypes.AuthTime), null,
                System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal,
                out DateTime authTime);

            var identity = new MappedIdentity(
                subject,
                First(claims, ClaimTypes.Name) ?? subject,
                First(claims, ClaimTypes.Email),
                tenantId,
                roles.Distinct().ToList(),
                unmapped,
                First(claims, ClaimTypes.AuthMethod) ?? "unknown",
                authTime == default ? DateTime.UtcNow : authTime);

            _trace?.Identity($"ClaimsMapper — {claims.Count} claims → {identity.UserId}@{identity.TenantId} roles={identity.RoleList}");
            return identity;
        }

        private static string First(IReadOnlyList<SsoClaim> claims, string type)
            => claims.FirstOrDefault(c => c.Type == type)?.Value;
    }
}
