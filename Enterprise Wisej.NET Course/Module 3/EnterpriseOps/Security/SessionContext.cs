using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// Session-scoped state: one object created at sign-in from verified claims and stored in
    /// <c>Application.Session</c>. It owns who the user is, which tenant they are working in, their culture
    /// and theme. Screens read it to adapt what they show; services read it (through the
    /// <see cref="CommandContext"/> it issues) to decide what they allow. It is never static: the server
    /// process hosts every session in the same memory, so a static "current user" is the classic leak.
    /// </summary>
    public sealed class SessionContext
    {
        public string SessionId { get; init; } = Guid.NewGuid().ToString("N");
        public string UserId { get; init; } = "anonymous";
        public string DisplayName { get; init; } = "Anonymous";
        public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> Permissions { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> EntitledTenants { get; init; } = Array.Empty<string>();
        public string Culture { get; init; } = "en-US";
        public string Theme { get; init; } = "Bootstrap-4";
        public DateTimeOffset SignedInAt { get; init; } = DateTimeOffset.UtcNow;

        /// <summary>The correlation id of the sign-in itself — every command gets its own.</summary>
        public string CorrelationId { get; init; } = NewCorrelationId();

        /// <summary>The current tenant. Changed only through <see cref="SwitchTenant"/>, which checks entitlement.</summary>
        public string TenantId { get; private set; } = "default";

        /// <summary>Builds the session context from verified claims — the only constructor path the app uses.</summary>
        public static SessionContext Create(VerifiedIdentity identity, string sessionId)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));

            return new SessionContext
            {
                SessionId = sessionId ?? Guid.NewGuid().ToString("N"),
                UserId = identity.UserId,
                DisplayName = identity.DisplayName,
                Roles = identity.Roles,
                Permissions = identity.Permissions,
                EntitledTenants = identity.EntitledTenants,
                Culture = identity.Culture,
                TenantId = identity.DefaultTenantId,
            };
        }

        public bool IsEntitledTo(string tenantId)
            => tenantId != null && EntitledTenants.Contains(tenantId, StringComparer.Ordinal);

        public bool HasRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The user picked a tenant. The dropdown is a request, not a decision: the switch happens only if the
        /// verified claims entitle the user to that tenant. A tampered dropdown value ends here.
        /// </summary>
        public TenantSwitchResult SwitchTenant(string tenantId)
        {
            if (string.Equals(tenantId, TenantId, StringComparison.Ordinal))
                return TenantSwitchResult.Unchanged(TenantId);

            if (!IsEntitledTo(tenantId))
                return TenantSwitchResult.Rejected(TenantId, tenantId,
                    $"{UserId} is not entitled to tenant '{tenantId}' (entitled: {string.Join(", ", EntitledTenants)})");

            string previous = TenantId;
            TenantId = tenantId;
            return TenantSwitchResult.Switched(previous, tenantId);
        }

        /// <summary>
        /// Starts a command: a fresh correlation id, a timestamp and the command name, plus the user and
        /// tenant this session owns. Handlers call this once per user action and pass the result down.
        /// </summary>
        public CommandContext BeginCommand(string commandName)
            => new CommandContext(UserId, TenantId, NewCorrelationId(), DateTimeOffset.UtcNow, commandName);

        /// <summary>Eight hex characters: short enough for a status bar, unique enough for a log search.</summary>
        public static string NewCorrelationId() => Guid.NewGuid().ToString("N").Substring(0, 8);

        public override string ToString() => $"{UserId}@{TenantId} · session {ShortId(SessionId)}";

        public static string ShortId(string id) => string.IsNullOrEmpty(id) ? "?" : (id.Length > 8 ? id.Substring(0, 8) : id);
    }

    /// <summary>Outcome of <see cref="SessionContext.SwitchTenant"/> — the UI shows it, never decides it.</summary>
    public sealed class TenantSwitchResult
    {
        public bool Succeeded { get; private init; }
        public bool Changed { get; private init; }
        public string PreviousTenantId { get; private init; }
        public string TenantId { get; private init; }
        public string Reason { get; private init; }

        public static TenantSwitchResult Switched(string previous, string current)
            => new TenantSwitchResult { Succeeded = true, Changed = true, PreviousTenantId = previous, TenantId = current, Reason = "entitled" };

        public static TenantSwitchResult Unchanged(string current)
            => new TenantSwitchResult { Succeeded = true, Changed = false, PreviousTenantId = current, TenantId = current, Reason = "already current" };

        public static TenantSwitchResult Rejected(string current, string requested, string reason)
            => new TenantSwitchResult { Succeeded = false, Changed = false, PreviousTenantId = current, TenantId = current, Reason = reason + $" — requested '{requested}'" };
    }
}
