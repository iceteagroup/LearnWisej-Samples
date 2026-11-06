using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;

namespace AccessOps.Services
{
    /// <summary>
    /// What a validated OpenID Connect sign-in hands to AccessOps: who (issuer + subject, the only
    /// stable key), what the provider says about them, and the tokens - which stay on the server.
    /// </summary>
    public sealed record ExternalIdentity(
        string Provider, string Issuer, string Subject, string Email, string DisplayName, string GivenName, string FamilyName);

    /// <summary>Server-side tokens of one external sign-in. Never rendered, never in a cookie.</summary>
    public sealed class ExternalTokenSet
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiresUtc { get; set; }
    }

    /// <summary>
    /// The hand-off between the ASP.NET Core OpenID Connect callback (no Wisej session) and the
    /// Wisej.NET page load that follows it. A validated identity is parked under a random handle for
    /// two minutes; the browser carries only the handle, in an HttpOnly cookie; the handle can be
    /// redeemed once.
    /// </summary>
    public sealed class ExternalHandoffStore
    {
        public const string CookieName = "AccessOps.Handoff";
        public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(2);

        private readonly ConcurrentDictionary<string, (ExternalIdentity Identity, ExternalTokenSet Tokens, DateTime ExpiresUtc)> _entries = new();

        public string Park(ExternalIdentity identity, ExternalTokenSet tokens)
        {
            foreach (var stale in _entries)
            {
                if (stale.Value.ExpiresUtc < DateTime.UtcNow)
                    _entries.TryRemove(stale.Key, out _);
            }

            var handle = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
            _entries[handle] = (identity, tokens, DateTime.UtcNow + Lifetime);
            return handle;
        }

        /// <summary>One time only: a second redemption of the same handle finds nothing.</summary>
        public bool TryRedeem(string handle, out ExternalIdentity identity, out ExternalTokenSet tokens)
        {
            identity = null;
            tokens = null;
            if (string.IsNullOrEmpty(handle) || !_entries.TryRemove(handle, out var entry) || entry.ExpiresUtc < DateTime.UtcNow)
                return false;

            identity = entry.Identity;
            tokens = entry.Tokens;
            return true;
        }
    }
}
