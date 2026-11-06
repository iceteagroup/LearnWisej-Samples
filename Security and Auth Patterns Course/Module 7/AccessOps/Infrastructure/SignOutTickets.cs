using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// Module 7 review finding: <c>GET /account/signout</c> ended the identity provider's session for
    /// anybody who could make the browser request it - an image tag on another site was enough
    /// (logout CSRF). The Wisej.NET session now issues a one-time ticket when the user signs out, the
    /// start page carries it to the endpoint, and the endpoint ends the provider session only for a
    /// ticket it issued in the last two minutes.
    /// </summary>
    public sealed class SignOutTickets
    {
        public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(2);

        private readonly ConcurrentDictionary<string, DateTime> _tickets = new ConcurrentDictionary<string, DateTime>(StringComparer.Ordinal);
        private readonly Func<DateTime> _clock;

        public SignOutTickets() : this(() => DateTime.UtcNow) { }

        public SignOutTickets(Func<DateTime> clock) => _clock = clock;

        public string Issue()
        {
            var now = _clock();
            foreach (var old in _tickets)
            {
                if (old.Value < now)
                    _tickets.TryRemove(old.Key, out _);
            }

            var ticket = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
            _tickets[ticket] = now.Add(Lifetime);
            return ticket;
        }

        /// <summary>True once for a ticket this server issued and that has not expired.</summary>
        public bool Redeem(string ticket)
            => !string.IsNullOrEmpty(ticket) && _tickets.TryRemove(ticket, out var expires) && expires >= _clock();
    }
}
