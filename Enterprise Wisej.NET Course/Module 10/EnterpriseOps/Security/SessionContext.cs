using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Diagnostics;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// The bridge between identity and the UI: what the screens read, and what every <see cref="CommandContext"/>
    /// is built from.
    ///
    /// One instance per browser session, created in <c>Program.Main</c> and parked in <c>Application.Session</c>
    /// — never in a static field, because every session of every user runs in the same process.
    ///
    /// It is deliberately small: identity, tenant, roles, and the command that is running now. It is built once,
    /// at sign-in, from **verified claims**; screens read it cheaply, and services still enforce the rules.
    /// <see cref="RefreshPermissions"/> exists because roles change while sessions are open: a long-running
    /// session must be able to reload its permissions without forcing a new login.
    /// </summary>
    public sealed class SessionContext
    {
        private readonly ActivityTrace _trace;

        public SessionContext(ActivityTrace trace)
        {
            _trace = trace;
            SessionStartedUtc = DateTime.UtcNow;
        }

        /// <summary>Null until the sign-in gate completes. Nothing else in the app may run before it is set.</summary>
        public MappedIdentity Identity { get; private set; }

        public bool IsAuthenticated => Identity != null;

        public DateTime SessionStartedUtc { get; }
        public DateTime? SignedInUtc { get; private set; }
        public CommandContext CurrentCommand { get; private set; }

        /// <summary>Bumped by <see cref="RefreshPermissions"/> so the trace can show that a reload happened.</summary>
        public int PermissionGeneration { get; private set; }

        public string TenantId => Identity?.TenantId;
        public string UserId => Identity?.UserId;
        public IReadOnlyList<Role> Roles => Identity?.Roles ?? (IReadOnlyList<Role>)Array.Empty<Role>();

        /// <summary>
        /// The only way an identity enters the application. It takes an already-mapped identity, so the session
        /// never parses a claim itself, and it replaces the whole identity rather than patching it.
        /// </summary>
        public void SignIn(MappedIdentity identity)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            SignedInUtc = DateTime.UtcNow;
            PermissionGeneration = 1;
            CurrentCommand = null;
            _trace?.Identity($"SessionContext — signed in {identity.UserId}@{identity.TenantId} roles={identity.RoleList} amr={identity.AuthMethod}");
        }

        public void SignOut()
        {
            _trace?.Identity($"SessionContext — signed out {Identity?.UserId ?? "(nobody)"}; every screen must go back through the gate");
            Identity = null;
            SignedInUtc = null;
            CurrentCommand = null;
            PermissionGeneration = 0;
        }

        /// <summary>
        /// Reloads the roles for the signed-in user without a new login — the "roles changed while you were
        /// working" path. The claims are re-fetched from the provider and re-mapped; the session keeps its id.
        /// </summary>
        public void RefreshPermissions(ISsoIdentityProvider provider, IClaimsMapper mapper)
        {
            if (!IsAuthenticated) throw new InvalidOperationException("Nobody is signed in.");

            var claims = provider.Authenticate(Identity.UserId);
            if (claims == null) throw new ClaimsMappingException($"The provider no longer knows '{Identity.UserId}'.");

            Identity = mapper.Map(claims);
            PermissionGeneration++;
            _trace?.Identity($"SessionContext — permissions reloaded (generation {PermissionGeneration}) roles={Identity.RoleList}; no new login was needed");
        }

        /// <summary>Starts a new unit of work: same identity, new correlation id. Every click gets one.</summary>
        public CommandContext BeginCommand()
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("No verified identity in this session — the sign-in gate has not run.");

            CurrentCommand = new CommandContext(
                Identity.UserId,
                Identity.DisplayName,
                Identity.TenantId,
                Identity.Roles.ToList(),
                NewCorrelationId());

            return CurrentCommand;
        }

        private static string NewCorrelationId() => Guid.NewGuid().ToString("N").Substring(0, 8);
    }
}
