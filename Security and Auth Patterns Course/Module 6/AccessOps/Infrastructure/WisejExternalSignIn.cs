using System;
using AccessOps.Identity;
using AccessOps.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wisej.Web;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// The Wisej.NET half of partner federation. <see cref="Begin"/> navigates the tab to the
    /// ASP.NET Core endpoint that issues the OpenID Connect challenge. After the provider and the
    /// handler are done, the page load runs <see cref="TryCompleteOnPageLoad"/>: read the hand-off
    /// handle with <c>Application.Cookies</c>, expire the cookie, redeem the handle once, map
    /// issuer + subject, create exactly one grant, then the Module 4 hand-off
    /// (<see cref="SignInHandoff.EstablishOnPageLoad"/>: new session ID, <c>Application.User</c>).
    /// </summary>
    public sealed class WisejExternalSignIn : IExternalSignIn
    {
        private readonly ExternalHandoffStore _handoffs;
        private readonly ExternalIdentityMapper _mapper;
        private readonly GrantStore _grants;
        private readonly LoginAudit _audit;
        private readonly IClock _clock;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WisejExternalSignIn> _log;

        /// <summary>Creates the seam.</summary>
        public WisejExternalSignIn(ExternalHandoffStore handoffs, ExternalIdentityMapper mapper, GrantStore grants,
            LoginAudit audit, IClock clock, IConfiguration configuration, ILogger<WisejExternalSignIn> log)
        {
            _handoffs = handoffs;
            _mapper = mapper;
            _grants = grants;
            _audit = audit;
            _clock = clock;
            _configuration = configuration;
            _log = log;
        }

        /// <inheritdoc />
        public string ProviderName => _configuration["Oidc:ProviderName"] ?? "your organisation";

        /// <inheritdoc />
        public void Begin() => Application.Navigate(OpenIdConnectSetup.SignInPath.TrimStart('/'), "_self");

        /// <inheritdoc />
        public ExternalCompletion TryCompleteOnPageLoad()
        {
            var handle = Application.Cookies[ExternalHandoffStore.CookieName];
            if (string.IsNullOrEmpty(handle))
                return ExternalCompletion.None;

            // The handle has done its job (and redeems only once anyway): expire the HttpOnly cookie.
            // Allowed here because this is a page load (Program.Main / ApplicationRefresh), not a WebSocket event.
            Application.Cookies.Add(ExternalHandoffStore.CookieName, "", null, DateTime.UtcNow.AddDays(-1),
                true, true, Wisej.Base.Cookie.SameSiteMode.Strict);

            var correlationId = AuditLog.NewCorrelationId();
            var address = Subject.OpaqueValue("a-", Application.UserHostAddress ?? "unknown");

            if (!_handoffs.TryRedeem(handle, out var identity))
                return Refuse("replayed_handoff", "handoff", address, correlationId);

            var mapped = _mapper.Map(identity);
            var subjectKey = Subject.OpaqueValue("x-", identity.Issuer + "|" + identity.Subject);
            if (!mapped.Succeeded)
                return Refuse(mapped.Reason, subjectKey, address, correlationId);

            // exactly one login grant for this sign-in
            var account = mapped.Account;
            var grant = _grants.Issue(account.Id, account.Tenant, account.Role);
            _audit.Write(subjectKey, address, LoginOutcome.Succeeded, correlationId, grant.Display + " oidc");

            var signIn = new VerifiedSignIn
            {
                AccountId = account.Id,
                TenantId = account.Tenant,
                GrantId = grant.Id,
                Roles = new[] { account.Role },
                DisplayName = account.DisplayName,
                Method = "oidc",
                VerifiedAt = _clock.Now
            };

            if (!SignInHandoff.EstablishOnPageLoad(signIn))
                return Refuse("grant_inactive", subjectKey, address, correlationId);

            _log.LogInformation("external sign-in completed · {Subject} → {Account} · tenant {Tenant} (AccessOps data) · grants total={Grants}",
                identity.RedactedSubject, "acct …" + Redact.Tail(account.Id, 4), account.Tenant, _grants.CountAll());
            return ExternalCompletion.SignedIn;
        }

        private ExternalCompletion Refuse(string reason, string subjectKey, string address, string correlationId)
        {
            _audit.Write(subjectKey, address, LoginOutcome.ExternalRefused, correlationId, reason);
            _log.LogWarning("external sign-in refused · reason={Reason} · grants total={Grants} · correlation={CorrelationId}",
                reason, _grants.CountAll(), correlationId);
            return ExternalCompletion.Rejected;
        }
    }
}
