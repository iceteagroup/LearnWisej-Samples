using System;
using System.Security.Cryptography;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using Microsoft.Data.Sqlite;

namespace AccessOps.Security
{
    /// <summary>
    /// The login grants, in the AccessOps database. IDs are opaque random values from
    /// <see cref="RandomNumberGenerator"/> (never a counter, a name or a session ID).
    /// <see cref="Find"/> always reads the row as it is now, so a revocation made in another
    /// session or another browser profile is seen by the very next decision - including a click
    /// on a screen that was loaded before the revocation.
    /// </summary>
    public sealed class GrantStore
    {
        /// <summary>How long a login grant lives when nobody revokes it.</summary>
        public static readonly TimeSpan DefaultLifetime = TimeSpan.FromHours(8);

        private readonly AccessOpsDatabase _db;
        private readonly IClock _clock;

        /// <summary>Creates the store over the AccessOps database.</summary>
        public GrantStore(AccessOpsDatabase db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }

        /// <summary>A new opaque grant ID: "G-" and 128 random bits as lower-case hex.</summary>
        public static string NewId() => "G-" + Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();

        /// <summary>
        /// Creates one login grant for a verified account and returns it. <paramref name="canRead"/>
        /// is false for an account whose role carries no read right (the intranet "Visitor").
        /// </summary>
        public Grant Issue(string accountId, string tenant, string role, TimeSpan? lifetime = null, bool canRead = true)
        {
            var now = _clock.Now;
            var grant = new Grant
            {
                Id = NewId(),
                AccountId = accountId,
                Tenant = tenant,
                Role = role,
                CanRead = canRead,
                CreatedAt = now,
                ExpiresAt = now + (lifetime ?? DefaultLifetime)
            };

            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = @"
INSERT INTO grants (id, account_id, tenant, role, can_read, created_at, expires_at)
VALUES ($id, $account, $tenant, $role, $read, $created, $expires)";
            cmd.Parameters.AddWithValue("$id", grant.Id);
            cmd.Parameters.AddWithValue("$account", accountId);
            cmd.Parameters.AddWithValue("$tenant", tenant);
            cmd.Parameters.AddWithValue("$role", role);
            cmd.Parameters.AddWithValue("$read", canRead ? 1 : 0);
            cmd.Parameters.AddWithValue("$created", AccessOpsDatabase.Stamp(grant.CreatedAt));
            cmd.Parameters.AddWithValue("$expires", AccessOpsDatabase.Stamp(grant.ExpiresAt));
            cmd.ExecuteNonQuery();
            return grant;
        }

        /// <summary>The grant's current state, or null when no grant has this ID.</summary>
        public Grant Find(string grantId)
        {
            if (string.IsNullOrEmpty(grantId))
                return null;

            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT id, account_id, tenant, role, can_read, created_at, expires_at, revoked FROM grants WHERE id = $id";
            cmd.Parameters.AddWithValue("$id", grantId);
            using var r = cmd.ExecuteReader();
            return r.Read() ? Map(r) : null;
        }

        /// <summary>
        /// Why <paramref name="subject"/> may not act right now, or null when its current grant is
        /// valid: "grant" (missing, revoked, expired or another account's), "role" (no read right).
        /// The tenant and the record are the caller's next checks.
        /// </summary>
        public string Check(Subject subject, out Grant grant)
        {
            grant = Find(subject?.GrantId);
            if (grant is null || grant.Revoked || grant.ExpiresAt <= _clock.Now
                || !StringComparer.Ordinal.Equals(grant.AccountId, subject.AccountId))
                return "grant";
            if (!grant.CanRead)
                return "role";
            return null;
        }

        /// <summary>True when the grant exists, is not revoked and has not expired.</summary>
        public bool IsActive(string grantId)
        {
            var grant = Find(grantId);
            return grant != null && !grant.Revoked && grant.ExpiresAt > _clock.Now;
        }

        /// <summary>Revokes one grant. Returns false when it was unknown or already revoked.</summary>
        public bool Revoke(string grantId)
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "UPDATE grants SET revoked = 1, revoked_at = $at WHERE id = $id AND revoked = 0";
            cmd.Parameters.AddWithValue("$at", AccessOpsDatabase.Stamp(_clock.Now));
            cmd.Parameters.AddWithValue("$id", grantId ?? "");
            return cmd.ExecuteNonQuery() == 1;
        }

        /// <summary>
        /// Revokes every active login grant of the account - sign-out ends the account's sign-in
        /// in every tab and every browser profile. Returns how many grants it revoked.
        /// </summary>
        public int RevokeAllForAccount(string accountId)
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "UPDATE grants SET revoked = 1, revoked_at = $at WHERE account_id = $account AND revoked = 0";
            cmd.Parameters.AddWithValue("$at", AccessOpsDatabase.Stamp(_clock.Now));
            cmd.Parameters.AddWithValue("$account", accountId ?? "");
            return cmd.ExecuteNonQuery();
        }

        /// <summary>How many grants exist in total (issued, any state) - the lab's grant count.</summary>
        public long CountAll()
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM grants";
            return Convert.ToInt64(cmd.ExecuteScalar());
        }

        /// <summary>How many grants are revoked (the rollback check: revocations must survive it).</summary>
        public long CountRevoked()
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM grants WHERE revoked = 1";
            return Convert.ToInt64(cmd.ExecuteScalar());
        }

        /// <summary>How many grants of the account are active now.</summary>
        public long CountActive(string accountId)
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM grants WHERE account_id = $account AND revoked = 0 AND expires_at > $now";
            cmd.Parameters.AddWithValue("$account", accountId ?? "");
            cmd.Parameters.AddWithValue("$now", AccessOpsDatabase.Stamp(_clock.Now));
            return Convert.ToInt64(cmd.ExecuteScalar());
        }

        private static Grant Map(SqliteDataReader r) => new Grant
        {
            Id = r.GetString(0),
            AccountId = r.GetString(1),
            Tenant = r.GetString(2),
            Role = r.GetString(3),
            CanRead = r.GetInt64(4) == 1,
            CreatedAt = AccessOpsDatabase.ParseStamp(r.GetString(5)),
            ExpiresAt = AccessOpsDatabase.ParseStamp(r.GetString(6)),
            Revoked = r.GetInt64(7) == 1
        };
    }
}
