using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>What the sign-in gate shows after a successful sign-in, so the screen never re-reads the claims.</summary>
    public sealed class SignInResult : CommandResult
    {
        private SignInResult(bool succeeded, bool denied, string summary, string correlationId, IReadOnlyList<string> errors,
            MappedIdentity identity, IReadOnlyList<Permission> permissions)
            : base(succeeded, denied, summary, correlationId, errors)
        {
            Identity = identity;
            Permissions = permissions ?? new List<Permission>();
        }

        public MappedIdentity Identity { get; }

        /// <summary>Everything this identity may do in this tenant — for the gate's "you will be able to…" list.</summary>
        public IReadOnlyList<Permission> Permissions { get; }

        public static SignInResult SignedIn(MappedIdentity identity, IReadOnlyList<Permission> permissions, string correlationId)
            => new SignInResult(true, false, $"signed in as {identity.UserId}", correlationId, null, identity, permissions);

        public static SignInResult Rejected(string reason, string correlationId)
            => new SignInResult(false, true, "sign-in rejected", correlationId, new List<string> { reason }, null, null);
    }

    /// <summary>
    /// The authentication gate: the one door into the application.
    ///
    /// The order matters and is the whole lesson. The provider authenticates (here, a simulation — no password
    /// is ever collected). The claims are mapped, once, into the application's own vocabulary. The session is
    /// given the mapped identity. The role store is told what this user holds in this tenant. Only then does a
    /// <see cref="CommandContext"/> exist, and only with one can any service be called.
    ///
    /// Nothing downstream parses a claim, and nothing downstream can be reached without a context.
    /// </summary>
    public sealed class SignInService
    {
        private readonly ISsoIdentityProvider _provider;
        private readonly IClaimsMapper _mapper;
        private readonly SessionContext _session;
        private readonly RolePermissionStore _roles;
        private readonly IAuditLog _audit;
        private readonly ActivityTrace _trace;

        public SignInService(ISsoIdentityProvider provider, IClaimsMapper mapper, SessionContext session,
            RolePermissionStore roles, IAuditLog audit, ActivityTrace trace)
        {
            _provider = provider;
            _mapper = mapper;
            _session = session;
            _roles = roles;
            _audit = audit;
            _trace = trace;
        }

        public IReadOnlyList<SsoAccount> Directory => _provider.Directory;

        /// <summary>The claims the provider would assert for an account — shown on the gate before signing in.</summary>
        public IReadOnlyList<SsoClaim> PreviewClaims(string subject) => _provider.Authenticate(subject);

        /// <summary>
        /// Runs the whole gate. Returns a result rather than throwing for a rejected sign-in, because "your
        /// account is not entitled to this application" is an answer the screen must show calmly.
        /// </summary>
        public async Task<SignInResult> SignInAsync(string subject)
        {
            _trace?.Service($"SignInService.SignInAsync('{subject}') — provider → claims → mapper → session → role store");

            await Task.Delay(120);   // stands in for the redirect to the identity provider and back

            var claims = _provider.Authenticate(subject);
            if (claims == null)
                return SignInResult.Rejected("The identity provider does not know that account.", "—");

            MappedIdentity identity;
            try
            {
                identity = _mapper.Map(claims);
            }
            catch (ClaimsMappingException ex)
            {
                _trace?.Identity($"ClaimsMapper rejected the token — {ex.Message}");
                return SignInResult.Rejected(ex.Message, "—");
            }

            _session.SignIn(identity);
            _roles.SetMembership(identity.UserId, identity.TenantId, identity.Roles);

            CommandContext context = _session.BeginCommand();

            var permissions = new List<Permission>();
            foreach (Role role in identity.Roles)
                foreach (Permission permission in RolePermissionStore.PermissionsFor(role))
                    if (!permissions.Contains(permission)) permissions.Add(permission);

            if (permissions.Count == 0)
            {
                // Authenticated, entitled to nothing: a real state, and one the application must survive.
                _audit.Write(context, "SignIn", AuditResult.Denied, "",
                    $"claims mapped to no role (groups: {string.Join(", ", identity.UnmappedGroups)})");
                _trace?.Identity($"{identity.UserId} is authenticated but holds no role in '{identity.TenantId}' — every Demand will refuse");
                return SignInResult.SignedIn(identity, permissions, context.CorrelationId);
            }

            _audit.Write(context, "SignIn", AuditResult.Ok, "", $"claims mapped → {identity.RoleList}");
            return SignInResult.SignedIn(identity, permissions, context.CorrelationId);
        }

        /// <summary>Roles changed while the session was open: reload them without a new login.</summary>
        public void RefreshPermissions()
        {
            _session.RefreshPermissions(_provider, _mapper);
            _roles.SetMembership(_session.UserId, _session.TenantId, _session.Roles);
        }

        public void SignOut() => _session.SignOut();
    }
}
