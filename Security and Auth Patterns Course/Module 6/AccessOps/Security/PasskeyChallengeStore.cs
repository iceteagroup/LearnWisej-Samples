using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace AccessOps.Security
{
    /// <summary>A one-time WebAuthn challenge: an opaque ticket ID for the server, standard base64 bytes for the browser.</summary>
    public sealed class PasskeyChallenge
    {
        /// <summary>The server-side handle of this ceremony (never sent as the challenge).</summary>
        public string Id { get; init; }

        /// <summary>The 32 random challenge bytes as standard base64 (what CreateAsync / GetAsync take).</summary>
        public string Base64 { get; init; }

        /// <summary>"webauthn.create" or "webauthn.get".</summary>
        public string Ceremony { get; init; }

        /// <summary>The account an enrollment is bound to (null for sign-in).</summary>
        public string AccountId { get; init; }

        /// <summary>The challenge stops being accepted at this instant.</summary>
        public DateTimeOffset ExpiresAt { get; init; }

        internal byte[] Bytes { get; init; }
    }

    /// <summary>
    /// Issues 32 random bytes per ceremony, bound to the ceremony type, the account (for
    /// enrollment) and a two-minute expiry. <see cref="TryConsume"/> removes the challenge whatever
    /// the outcome - a failed attempt burns it too - and accepts it only once, only for the same
    /// ceremony and account, only before expiry and only when the bytes equal
    /// <c>ClientData.Challenge</c>. In process memory: one server only (Module 7 moves it to the
    /// shared store).
    /// </summary>
    public sealed class PasskeyChallengeStore
    {
        /// <summary>How long a challenge is valid.</summary>
        public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(2);

        private readonly ConcurrentDictionary<string, PasskeyChallenge> _open = new ConcurrentDictionary<string, PasskeyChallenge>(StringComparer.Ordinal);
        private readonly IClock _clock;

        /// <summary>Creates the store on <paramref name="clock"/>.</summary>
        public PasskeyChallengeStore(IClock clock)
        {
            _clock = clock;
        }

        /// <summary>Issues a new challenge for <paramref name="ceremony"/>.</summary>
        public PasskeyChallenge Issue(string ceremony, string accountId = null)
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            var challenge = new PasskeyChallenge
            {
                Id = Convert.ToHexString(RandomNumberGenerator.GetBytes(12)).ToLowerInvariant(),
                Base64 = Convert.ToBase64String(bytes),
                Ceremony = ceremony,
                AccountId = accountId,
                ExpiresAt = _clock.Now + Lifetime,
                Bytes = bytes
            };
            _open[challenge.Id] = challenge;
            return challenge;
        }

        /// <summary>
        /// Consumes the challenge <paramref name="ticketId"/>: true only the first time, for the
        /// right ceremony and account, before expiry, and when <paramref name="clientDataChallenge"/>
        /// has the same bytes.
        /// </summary>
        public bool TryConsume(string ticketId, string ceremony, byte[] clientDataChallenge, string accountId = null)
        {
            if (string.IsNullOrEmpty(ticketId) || !_open.TryRemove(ticketId, out var challenge))
                return false;                                   // unknown or already used
            if (challenge.ExpiresAt <= _clock.Now)
                return false;
            if (!string.Equals(challenge.Ceremony, ceremony, StringComparison.Ordinal))
                return false;
            if (!string.Equals(challenge.AccountId, accountId, StringComparison.Ordinal))
                return false;
            return clientDataChallenge != null
                && CryptographicOperations.FixedTimeEquals(challenge.Bytes, clientDataChallenge);
        }

        /// <summary>Throws the challenge away (a cancelled or failed prompt).</summary>
        public void Discard(string ticketId)
        {
            if (!string.IsNullOrEmpty(ticketId))
                _open.TryRemove(ticketId, out _);
        }
    }
}
