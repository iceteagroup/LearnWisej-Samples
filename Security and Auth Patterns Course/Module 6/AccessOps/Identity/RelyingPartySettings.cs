using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AccessOps.Identity
{
    /// <summary>
    /// The WebAuthn relying party, written once in configuration (<c>WebAuthn</c> section): the RP
    /// ID (the sign-in host; <c>localhost</c> for the lab), the exact allowed origins (scheme, host
    /// and port - a list, not a pattern) and SHA-256 of the RP ID for the <c>RPIDHash</c> check.
    /// <c>WebAuthn.GetAsync</c> sends no RP ID to the browser, so registration and sign-in must run
    /// on this same host.
    /// </summary>
    public sealed class RelyingPartySettings
    {
        /// <summary>Creates the settings.</summary>
        public RelyingPartySettings(string rpId, string rpName, IEnumerable<string> allowedOrigins, bool requireUserVerification = true, int recentSignInMinutes = 10)
        {
            this.RpId = rpId ?? throw new ArgumentNullException(nameof(rpId));
            this.RpName = rpName ?? "AccessOps";
            this.AllowedOrigins = new HashSet<string>((allowedOrigins ?? Array.Empty<string>()).Select(o => o.TrimEnd('/')), StringComparer.Ordinal);
            this.RpIdHash = SHA256.HashData(Encoding.UTF8.GetBytes(rpId));
            this.RequireUserVerification = requireUserVerification;
            this.RecentSignIn = TimeSpan.FromMinutes(recentSignInMinutes);
        }

        /// <summary>The relying party ID (a domain, no scheme, no port).</summary>
        public string RpId { get; }

        /// <summary>The name the authenticator shows.</summary>
        public string RpName { get; }

        /// <summary>Exact origins accepted in <c>ClientData.Origin</c>.</summary>
        public IReadOnlySet<string> AllowedOrigins { get; }

        /// <summary>SHA-256 of <see cref="RpId"/>, compared with <c>AuthenticatorData.RPIDHash</c>.</summary>
        public byte[] RpIdHash { get; }

        /// <summary>Whether the <c>UserVerified</c> flag must be set (GetAsync sends no userVerification option).</summary>
        public bool RequireUserVerification { get; }

        /// <summary>How recent a sign-in must be before a passkey may be added.</summary>
        public TimeSpan RecentSignIn { get; }

        /// <summary>Reads the <c>WebAuthn</c> section.</summary>
        public static RelyingPartySettings FromConfiguration(IConfiguration configuration)
        {
            var s = configuration.GetSection("WebAuthn");
            return new RelyingPartySettings(
                s["RpId"] ?? "localhost",
                s["RpName"] ?? "AccessOps",
                s.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>(),
                s.GetValue("RequireUserVerification", true),
                s.GetValue("RecentSignInMinutes", 10));
        }
    }
}
