using System;
using System.Security.Cryptography;
using AccessOps.Infrastructure;

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
    /// <c>ClientData.Challenge</c>. Module 7: the open challenges live in the shared store (table
    /// <c>passkey_challenges</c>, consumed with one <c>DELETE … RETURNING</c>), so a prompt started
    /// on node A can finish on node B and a restart cannot make a used challenge usable again.
    /// </summary>
    public sealed class PasskeyChallengeStore
    {
        /// <summary>How long a challenge is valid.</summary>
        public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(2);

        private readonly AccessOpsDatabase _db;
        private readonly IClock _clock;

        /// <summary>Creates the store over the shared database.</summary>
        public PasskeyChallengeStore(AccessOpsDatabase db, IClock clock)
        {
            _db = db;
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

            using var db = _db.Open();
            using (var purge = db.CreateCommand())
            {
                purge.CommandText = "DELETE FROM passkey_challenges WHERE expires_at <= $now";
                purge.Parameters.AddWithValue("$now", AccessOpsDatabase.Stamp(_clock.Now));
                purge.ExecuteNonQuery();
            }
            using var cmd = db.CreateCommand();
            cmd.CommandText = "INSERT INTO passkey_challenges (id, challenge, ceremony, account_id, expires_at) VALUES ($id, $challenge, $ceremony, $account, $expires)";
            cmd.Parameters.AddWithValue("$id", challenge.Id);
            cmd.Parameters.AddWithValue("$challenge", challenge.Base64);
            cmd.Parameters.AddWithValue("$ceremony", ceremony);
            cmd.Parameters.AddWithValue("$account", (object)accountId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$expires", AccessOpsDatabase.Stamp(challenge.ExpiresAt));
            cmd.ExecuteNonQuery();
            return challenge;
        }

        /// <summary>
        /// Consumes the challenge <paramref name="ticketId"/>: true only the first time, for the
        /// right ceremony and account, before expiry, and when <paramref name="clientDataChallenge"/>
        /// has the same bytes.
        /// </summary>
        public bool TryConsume(string ticketId, string ceremony, byte[] clientDataChallenge, string accountId = null)
        {
            if (string.IsNullOrEmpty(ticketId))
                return false;

            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "DELETE FROM passkey_challenges WHERE id = $id RETURNING challenge, ceremony, account_id, expires_at";
            cmd.Parameters.AddWithValue("$id", ticketId);
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return false;                                   // unknown or already used
            var bytes = Convert.FromBase64String(r.GetString(0));
            var storedCeremony = r.GetString(1);
            var storedAccount = r.IsDBNull(2) ? null : r.GetString(2);
            var expires = AccessOpsDatabase.ParseStamp(r.GetString(3));

            if (expires <= _clock.Now)
                return false;
            if (!string.Equals(storedCeremony, ceremony, StringComparison.Ordinal))
                return false;
            if (!string.Equals(storedAccount, accountId, StringComparison.Ordinal))
                return false;
            return clientDataChallenge != null
                && CryptographicOperations.FixedTimeEquals(bytes, clientDataChallenge);
        }

        /// <summary>Throws the challenge away (a cancelled or failed prompt).</summary>
        public void Discard(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
                return;
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "DELETE FROM passkey_challenges WHERE id = $id";
            cmd.Parameters.AddWithValue("$id", ticketId);
            cmd.ExecuteNonQuery();
        }
    }
}
