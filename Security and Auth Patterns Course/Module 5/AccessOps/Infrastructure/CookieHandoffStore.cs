using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using AccessOps.Identity;
using AccessOps.Security;
using Microsoft.AspNetCore.WebUtilities;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// Single-use, short-lived handles the Wisej.NET session creates so the browser can ask the
    /// HTTPS endpoint <c>/auth/cookie</c> for the host authentication cookie. A handle names a
    /// verified sign-in on the server; it is not a credential the browser could reuse: it works
    /// once and for 60 seconds. In process memory - one server only.
    /// </summary>
    public sealed class CookieHandoffStore
    {
        /// <summary>How long a handle can be redeemed.</summary>
        public static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(60);

        private readonly ConcurrentDictionary<string, Entry> _entries = new ConcurrentDictionary<string, Entry>(StringComparer.Ordinal);
        private readonly IClock _clock;

        /// <summary>Creates the store on <paramref name="clock"/>.</summary>
        public CookieHandoffStore(IClock clock)
        {
            _clock = clock;
        }

        /// <summary>Parks <paramref name="signIn"/> and returns a new random handle (256 bits).</summary>
        public string Create(VerifiedSignIn signIn)
        {
            var handle = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
            _entries[handle] = new Entry(signIn, _clock.Now + Lifetime);
            return handle;
        }

        /// <summary>Redeems a handle once. False for an unknown, expired or already redeemed handle.</summary>
        public bool TryRedeem(string handle, out VerifiedSignIn signIn)
        {
            signIn = null;
            if (string.IsNullOrEmpty(handle) || !_entries.TryRemove(handle, out var entry))
                return false;
            if (entry.ExpiresAt <= _clock.Now)
                return false;
            signIn = entry.SignIn;
            return true;
        }

        private sealed record Entry(VerifiedSignIn SignIn, DateTimeOffset ExpiresAt);
    }
}
