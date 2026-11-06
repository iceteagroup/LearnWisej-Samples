using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AccessOps.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wisej.Web;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// <see cref="IExternalSignIn"/> over the ASP.NET Core OpenID Connect handler (see
    /// <see cref="OpenIdConnectSetup"/>). The Wisej.NET side of the round trip: start it with a
    /// top-level navigation, and on the page load that follows the callback redeem the one-time
    /// hand-off, map it, and establish the SessionContext (page load: the session ID is renewed
    /// first). Tokens are kept in Application.Session - server memory - and never rendered.
    /// </summary>
    public sealed class WisejExternalSignIn : IExternalSignIn
    {
        private const string InfoKey = "ExternalSessionInfo";
        private const string TokensKey = "ExternalTokens";

        private readonly ExternalHandoffStore _handoffs;
        private readonly ExternalIdentityMapper _mapper;
        private readonly LoginService _login;
        private readonly IAccessOpsSession _session;
        private readonly IOptionsMonitor<OpenIdConnectOptions> _oidc;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _http;
        private readonly ILogger<WisejExternalSignIn> _log;

        public WisejExternalSignIn(ExternalHandoffStore handoffs, ExternalIdentityMapper mapper, LoginService login,
            IAccessOpsSession session, IOptionsMonitor<OpenIdConnectOptions> oidc, IConfiguration configuration,
            IHttpClientFactory http, ILogger<WisejExternalSignIn> log)
        {
            _handoffs = handoffs;
            _mapper = mapper;
            _login = login;
            _session = session;
            _oidc = oidc;
            _configuration = configuration;
            _http = http;
            _log = log;
        }

        public string ProviderName => _configuration["Oidc:ProviderName"] ?? "your organisation";

        public ExternalSessionInfo Current => Application.Session[InfoKey] as ExternalSessionInfo;

        public void Begin() => Application.Navigate(OpenIdConnectSetup.SignInPath.TrimStart('/'), "_self");

        public ExternalCompletion TryCompleteOnPageLoad()
        {
            var handle = Application.Cookies[ExternalHandoffStore.CookieName];
            if (string.IsNullOrEmpty(handle))
                return ExternalCompletion.None;

            // the handle has done its job (and can only be redeemed once anyway): expire the cookie
            Application.Cookies.Add(ExternalHandoffStore.CookieName, "", null, DateTime.UtcNow.AddDays(-1),
                Application.IsSecure, true, Wisej.Base.Cookie.SameSiteMode.Strict);

            if (!_handoffs.TryRedeem(handle, out var identity, out var tokens))
            {
                // replayed or expired hand-off: nothing is redeemed twice
                _log.LogWarning("External sign-in hand-off not redeemable (replayed or expired)");
                return ExternalCompletion.Rejected;
            }

            // the sign-in is a command too: one correlation ID for its log lines and its audit row
            var command = CommandContext.Anonymous("oidc:" + identity.Subject, _session.ClientAddress, _session.UserAgent);
            using var scope = _log.BeginScope(command.Scope());
            var mapped = _mapper.Map(identity, command);
            if (mapped.Outcome == ExternalMapOutcome.Rejected)
            {
                _log.LogWarning("External sign-in rejected for subject {Subject}: account disabled", identity.Subject);
                return ExternalCompletion.Rejected;
            }

            var result = _login.SignInExternal(mapped.EmployeeId, "OpenID Connect · " + identity.Provider);
            if (!result.Succeeded)
                return ExternalCompletion.Rejected;

            Application.Session[InfoKey] = new ExternalSessionInfo(identity.Provider, identity.Issuer, identity.Subject,
                mapped.Outcome == ExternalMapOutcome.Provisioned);
            Application.Session[TokensKey] = tokens;
            _log.LogInformation("Employee {EmployeeId} signed in via {Provider} ({Outcome})", mapped.EmployeeId, identity.Provider, mapped.Outcome);
            return ExternalCompletion.SignedIn;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (Application.Session[TokensKey] is not ExternalTokenSet tokens || tokens.AccessToken == null)
                return null;

            if (tokens.AccessTokenExpiresUtc - DateTime.UtcNow > TimeSpan.FromSeconds(60))
                return tokens.AccessToken;

            if (tokens.RefreshToken != null && await RefreshAsync(tokens))
                return tokens.AccessToken;

            Application.Session[TokensKey] = null;
            return null;
        }

        private async Task<bool> RefreshAsync(ExternalTokenSet tokens)
        {
            try
            {
                var options = _oidc.Get(OpenIdConnectDefaults.AuthenticationScheme);
                var metadata = await options.ConfigurationManager.GetConfigurationAsync(CancellationToken.None);
                using var response = await _http.CreateClient().PostAsync(metadata.TokenEndpoint, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["refresh_token"] = tokens.RefreshToken,
                    ["client_id"] = options.ClientId,
                    ["client_secret"] = options.ClientSecret
                }));
                if (!response.IsSuccessStatusCode)
                {
                    _log.LogWarning("Token refresh refused by the provider ({Status})", (int)response.StatusCode);
                    return false;
                }

                using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var root = json.RootElement;
                tokens.AccessToken = root.GetProperty("access_token").GetString();
                tokens.RefreshToken = root.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : tokens.RefreshToken;
                tokens.AccessTokenExpiresUtc = DateTime.UtcNow.AddSeconds(root.TryGetProperty("expires_in", out var e) ? e.GetInt32() : 300);
                _log.LogInformation("Access token refreshed in the back channel");
                return true;
            }
            catch (HttpRequestException ex)
            {
                _log.LogWarning("Token refresh failed: provider unavailable ({Message})", ex.Message);
                return false;
            }
        }
    }
}
