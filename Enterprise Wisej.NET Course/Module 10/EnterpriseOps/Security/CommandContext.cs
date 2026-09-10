using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// Everything a service needs to know about *who* is asking: user id, display name, tenant, roles and the
    /// correlation id of this one command.
    ///
    /// It is built once, from verified claims, by <see cref="SessionContext.SignIn"/>, and passed explicitly to
    /// every service call. A service never reads identity from a control, a query string or a hidden field —
    /// all three can be edited in the browser — and never from a static "current user".
    ///
    /// Immutable: a command context describes one command. The next click gets a new one with a new correlation id.
    /// </summary>
    public sealed class CommandContext
    {
        public CommandContext(string userId, string displayName, string tenantId, IEnumerable<Role> roles, string correlationId)
        {
            UserId = userId;
            DisplayName = displayName;
            TenantId = tenantId;
            Roles = (roles ?? Enumerable.Empty<Role>()).Distinct().OrderBy(r => r.ToString(), StringComparer.Ordinal).ToList();
            CorrelationId = correlationId;
        }

        /// <summary>The stable subject id from the identity provider ("sub"), mapped to the application's user id.</summary>
        public string UserId { get; }

        public string DisplayName { get; }

        /// <summary>The tenant this command runs in. Every permission check is evaluated inside it.</summary>
        public string TenantId { get; }

        /// <summary>The roles this user holds **in this tenant** — membership is per tenant, never global.</summary>
        public IReadOnlyList<Role> Roles { get; }

        /// <summary>Links this command to the audit entry, the trace lines and (Module 11) the structured logs.</summary>
        public string CorrelationId { get; }

        public string RoleList => Roles.Count == 0 ? "(none)" : string.Join("+", Roles);

        public override string ToString() => $"user={UserId} tenant={TenantId} roles={RoleList} corr={CorrelationId}";
    }
}
