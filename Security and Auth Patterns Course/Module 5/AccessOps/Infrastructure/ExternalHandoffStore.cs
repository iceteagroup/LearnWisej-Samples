using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using AccessOps.Identity;
using AccessOps.Security;
using Microsoft.AspNetCore.WebUtilities;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// Where <c>OnTokenValidated</c> parks a validated <see cref="ExternalIdentity"/> until the
    /// Wisej.NET page load picks it up. The browser only holds a random 256-bit handle in the
    /// HttpOnly <see cref="CookieName"/> cookie; the handle redeems once and for two minutes. In
    /// process memory - one server only (Module 7 moves it to the shared store).
    /// </summary>
    public sealed class ExternalHandoffStore
    {
        /// <summary>The hand-off cookie.</summary>
        public const string CookieName = "AccessOps.Handoff";

        /// <summary>How long a parked identity waits for its page load.</summary>
        public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(2);

        private readonly ConcurrentDictionary<string, Entry> _entries = new ConcurrentDictionary<string, Entry>(StringComparer.Ordinal);
        private readonly IClock _clock;

        /// <summary>Creates the store on <paramref name="clock"/>.</summary>
        public ExternalHandoffStore(IClock clock)
        {
            _clock = clock;
        }

        /// <summary>Parks <paramref name="identity"/> and returns its new handle.</summary>
        public string Park(ExternalIdentity identity)
        {
            var handle = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
            _entries[handle] = new Entry(identity, _clock.Now + Lifetime);
            return handle;
        }

        /// <summary>Redeems a handle once. False for an unknown, expired or already redeemed handle.</summary>
        public bool TryRedeem(string handle, out ExternalIdentity identity)
        {
            identity = null;
            if (string.IsNullOrEmpty(handle) || !_entries.TryRemove(handle, out var entry))
                return false;
            if (entry.ExpiresAt <= _clock.Now)
                return false;
            identity = entry.Identity;
            return true;
        }

        private sealed record Entry(ExternalIdentity Identity, DateTimeOffset ExpiresAt);
    }
}
