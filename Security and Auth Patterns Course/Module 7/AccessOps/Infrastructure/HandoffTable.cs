using System;
using System.Security.Cryptography;
using System.Text;
using AccessOps.Security;
using Microsoft.AspNetCore.WebUtilities;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// Single-use sign-in hand-off handles in the shared store (table <c>signin_handoffs</c>),
    /// Module 7. The browser holds a random 256-bit handle; the table holds only its SHA-256 hash,
    /// the kind ("external", "cookie"), the payload and the expiry. Redeeming is one
    /// <c>DELETE … RETURNING</c> statement, so of two nodes - or a node and its restarted self -
    /// presenting the same handle at the same time, exactly one gets the payload. Because the row
    /// lives in the store and not in process memory, a callback that lands on node B after node A
    /// parked it resumes once, and a repeat of it fails safely.
    /// </summary>
    public sealed class HandoffTable
    {
        private readonly AccessOpsDatabase _db;
        private readonly IClock _clock;

        /// <summary>Creates the table access over the AccessOps database.</summary>
        public HandoffTable(AccessOpsDatabase db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }

        /// <summary>
        /// LAB FAULT INJECTION (Debug builds, Development only, see Startup.cs): when true, a redeem
        /// reads the row without deleting it - the defect "a replayed callback is accepted as a fresh
        /// sign-in" that the release review's stop condition must catch. Never true in production.
        /// </summary>
        public static bool ReplayableForLab { get; set; }

        /// <summary>Stores <paramref name="payload"/> under a new random handle and returns the handle.</summary>
        public string Park(string kind, string payload, TimeSpan lifetime)
        {
            var handle = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
            var now = _clock.Now;
            using var db = _db.Open();
            using (var purge = db.CreateCommand())
            {
                purge.CommandText = "DELETE FROM signin_handoffs WHERE expires_at <= $now";
                purge.Parameters.AddWithValue("$now", AccessOpsDatabase.Stamp(now));
                purge.ExecuteNonQuery();
            }
            using var cmd = db.CreateCommand();
            cmd.CommandText = "INSERT INTO signin_handoffs (handle_hash, kind, payload, created_at, expires_at) VALUES ($hash, $kind, $payload, $created, $expires)";
            cmd.Parameters.AddWithValue("$hash", Hash(handle));
            cmd.Parameters.AddWithValue("$kind", kind);
            cmd.Parameters.AddWithValue("$payload", payload);
            cmd.Parameters.AddWithValue("$created", AccessOpsDatabase.Stamp(now));
            cmd.Parameters.AddWithValue("$expires", AccessOpsDatabase.Stamp(now + lifetime));
            cmd.ExecuteNonQuery();
            return handle;
        }

        /// <summary>
        /// Redeems <paramref name="handle"/> once: deletes the row and returns its payload. False for
        /// an unknown, already redeemed, expired or other-kind handle.
        /// </summary>
        public bool TryRedeem(string kind, string handle, out string payload)
        {
            payload = null;
            if (string.IsNullOrEmpty(handle))
                return false;

            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = ReplayableForLab
                ? "SELECT payload, expires_at FROM signin_handoffs WHERE handle_hash = $hash AND kind = $kind"
                : "DELETE FROM signin_handoffs WHERE handle_hash = $hash AND kind = $kind RETURNING payload, expires_at";
            cmd.Parameters.AddWithValue("$hash", Hash(handle));
            cmd.Parameters.AddWithValue("$kind", kind);
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return false;                                            // unknown or already redeemed
            if (AccessOpsDatabase.ParseStamp(r.GetString(1)) <= _clock.Now)
                return false;                                            // expired (the row is gone too)
            payload = r.GetString(0);
            return true;
        }

        private static string Hash(string handle)
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(handle))).ToLowerInvariant();
    }
}
