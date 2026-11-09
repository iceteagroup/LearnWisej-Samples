using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using AccessOps.Infrastructure;
using Microsoft.Data.Sqlite;
using Wisej.Ext.WebAuthn;

namespace AccessOps.Security
{
    /// <summary>One stored passkey: public data only (no private key, no biometric sample).</summary>
    public sealed class PasskeyCredential
    {
        /// <summary>Row identity: a new value for every enrollment, so a stale update can never land on a re-enrolled credential.</summary>
        public string RowId { get; init; }

        /// <summary>The relying party ID the credential is scoped to.</summary>
        public string RpId { get; init; }

        /// <summary>The credential ID (base64, as the extension returns it).</summary>
        public string CredentialId { get; init; }

        /// <summary>The owning AccessOps account.</summary>
        public string AccountId { get; init; }

        /// <summary>The account's opaque ASCII user handle.</summary>
        public string UserHandle { get; init; }

        /// <summary>The public key as <c>JSON.Stringify(publicKey)</c> wrote it.</summary>
        public string PublicKeyJson { get; init; }

        /// <summary>The COSE algorithm (-7 ES256, -257 RS256).</summary>
        public int Algorithm { get; init; }

        /// <summary>The last accepted signature counter.</summary>
        public long SignCount { get; init; }

        /// <summary>The authenticator's BackupEligibility flag as last seen (recorded; no decision depends on it).</summary>
        public bool BackupEligible { get; init; }

        /// <summary>The owner's name for it ("Work laptop · Windows Hello"). User input: always shown as text.</summary>
        public string Label { get; init; }

        /// <summary>When it was enrolled.</summary>
        public DateTimeOffset CreatedAt { get; init; }

        /// <summary>When it last signed in, or null.</summary>
        public DateTimeOffset? LastUsedAt { get; init; }

        /// <summary>The optimistic-concurrency version the commit compares.</summary>
        public long Version { get; init; }

        /// <summary>The public key restored for <c>WebAuthn.Validate</c> (<c>new PublicKey(json)</c>).</summary>
        public PublicKey Key => new PublicKey(this.PublicKeyJson);
    }

    /// <summary>What <see cref="PasskeyCredentialStore.CommitUse"/> did.</summary>
    public enum CommitOutcome
    {
        /// <summary>One row matched what the verifier read and was updated.</summary>
        Updated,
        /// <summary>The row still exists but changed since it was read (another sign-in committed first).</summary>
        Conflict,
        /// <summary>The row read for verification is gone (deleted, or re-enrolled under a new row identity).</summary>
        NotFound
    }

    /// <summary>
    /// AccessOps' own passkey store: one table keyed by RP ID + credential ID (unique, so a
    /// credential cannot be enrolled twice or under a second account), with the owner, the opaque
    /// user handle, the public key JSON, the algorithm, the last counter, BackupEligibility, a label,
    /// a row identity and a version. <see cref="CommitUse"/> is one conditional UPDATE; deletes are
    /// final. Every statement is parameterized.
    /// </summary>
    public sealed class PasskeyCredentialStore
    {
        private readonly AccessOpsDatabase _db;
        private readonly IClock _clock;

        /// <summary>Creates the store over the AccessOps database.</summary>
        public PasskeyCredentialStore(AccessOpsDatabase db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }

        /// <summary>
        /// The account's opaque user handle: 24 random bytes as lower-case hex (ASCII - the client
        /// sends the id one byte per character and decodes UserHandle as UTF-8). Created once,
        /// never an e-mail address.
        /// </summary>
        public string UserHandleFor(string accountId)
        {
            using var db = _db.Open();
            using (var find = db.CreateCommand())
            {
                find.CommandText = "SELECT user_handle FROM passkey_user_handles WHERE account_id = $account";
                find.Parameters.AddWithValue("$account", accountId);
                if (find.ExecuteScalar() is string existing)
                    return existing;
            }

            var handle = Convert.ToHexString(RandomNumberGenerator.GetBytes(24)).ToLowerInvariant();
            using var insert = db.CreateCommand();
            insert.CommandText = "INSERT OR IGNORE INTO passkey_user_handles (account_id, user_handle) VALUES ($account, $handle)";
            insert.Parameters.AddWithValue("$account", accountId);
            insert.Parameters.AddWithValue("$handle", handle);
            insert.ExecuteNonQuery();
            return UserHandleFor(accountId);
        }

        /// <summary>The account a user handle belongs to, or null.</summary>
        public string AccountForUserHandle(string userHandle)
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT account_id FROM passkey_user_handles WHERE user_handle = $handle";
            cmd.Parameters.AddWithValue("$handle", userHandle ?? "");
            return cmd.ExecuteScalar() as string;
        }

        /// <summary>
        /// Stores a new credential. False when the RP ID + credential ID pair already exists (the
        /// unique constraint), whoever owns it.
        /// </summary>
        public bool Add(PasskeyCredential credential)
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = @"
INSERT INTO passkey_credentials (row_id, rp_id, credential_id, account_id, user_handle, public_key_json, algorithm, sign_count, backup_eligible, label, created_at, last_used_at, version)
VALUES ($row, $rp, $cred, $account, $handle, $key, $alg, $count, $backup, $label, $created, NULL, 1)";
            cmd.Parameters.AddWithValue("$row", credential.RowId ?? Guid.NewGuid().ToString("N"));
            cmd.Parameters.AddWithValue("$rp", credential.RpId);
            cmd.Parameters.AddWithValue("$cred", credential.CredentialId);
            cmd.Parameters.AddWithValue("$account", credential.AccountId);
            cmd.Parameters.AddWithValue("$handle", credential.UserHandle);
            cmd.Parameters.AddWithValue("$key", credential.PublicKeyJson);
            cmd.Parameters.AddWithValue("$alg", credential.Algorithm);
            cmd.Parameters.AddWithValue("$count", credential.SignCount);
            cmd.Parameters.AddWithValue("$backup", credential.BackupEligible ? 1 : 0);
            cmd.Parameters.AddWithValue("$label", credential.Label ?? "Passkey");
            cmd.Parameters.AddWithValue("$created", AccessOpsDatabase.Stamp(_clock.Now));
            try
            {
                return cmd.ExecuteNonQuery() == 1;
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)   // UNIQUE (rp_id, credential_id)
            {
                return false;
            }
        }

        /// <summary>The credentials stored for a user handle under one RP (the keys a sign-in may try).</summary>
        public IReadOnlyList<PasskeyCredential> ForUserHandle(string rpId, string userHandle)
            => Query("WHERE rp_id = $a AND user_handle = $b", rpId, userHandle ?? "");

        /// <summary>The account's own credentials, for the review page.</summary>
        public IReadOnlyList<PasskeyCredential> ForAccount(string accountId)
            => Query("WHERE account_id = $a ORDER BY created_at, label", accountId ?? "", null);

        /// <summary>One row by RP ID and credential ID, or null.</summary>
        public PasskeyCredential Find(string rpId, string credentialId)
        {
            var rows = Query("WHERE rp_id = $a AND credential_id = $b", rpId, credentialId ?? "");
            return rows.Count == 1 ? rows[0] : null;
        }

        /// <summary>
        /// Applies a verified sign-in in one conditional statement: counter and last-used time are
        /// written only where the RP, the credential ID, the owner, the row identity and the version
        /// all still match what the verifier read. Zero rows is classified, never retried:
        /// <see cref="CommitOutcome.NotFound"/> if that row is gone, otherwise <see cref="CommitOutcome.Conflict"/>.
        /// </summary>
        public CommitOutcome CommitUse(PasskeyCredential read, long newSignCount, bool backupEligible)
        {
            using var db = _db.Open();
            using (var update = db.CreateCommand())
            {
                update.CommandText = @"
UPDATE passkey_credentials
SET sign_count = $count, backup_eligible = $backup, last_used_at = $now, version = version + 1
WHERE rp_id = $rp AND credential_id = $cred AND account_id = $account AND row_id = $row AND version = $version";
                update.Parameters.AddWithValue("$count", newSignCount);
                update.Parameters.AddWithValue("$backup", backupEligible ? 1 : 0);
                update.Parameters.AddWithValue("$now", AccessOpsDatabase.Stamp(_clock.Now));
                update.Parameters.AddWithValue("$rp", read.RpId);
                update.Parameters.AddWithValue("$cred", read.CredentialId);
                update.Parameters.AddWithValue("$account", read.AccountId);
                update.Parameters.AddWithValue("$row", read.RowId);
                update.Parameters.AddWithValue("$version", read.Version);
                if (update.ExecuteNonQuery() == 1)
                    return CommitOutcome.Updated;
            }

            using var exists = db.CreateCommand();
            exists.CommandText = "SELECT COUNT(*) FROM passkey_credentials WHERE row_id = $row";
            exists.Parameters.AddWithValue("$row", read.RowId);
            return Convert.ToInt64(exists.ExecuteScalar()) == 0 ? CommitOutcome.NotFound : CommitOutcome.Conflict;
        }

        /// <summary>Renames one of the owner's credentials. The owner comes from the session principal.</summary>
        public bool Rename(string ownerAccountId, string rowId, string label)
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "UPDATE passkey_credentials SET label = $label, version = version + 1 WHERE row_id = $row AND account_id = $owner";
            cmd.Parameters.AddWithValue("$label", label);
            cmd.Parameters.AddWithValue("$row", rowId ?? "");
            cmd.Parameters.AddWithValue("$owner", ownerAccountId ?? "");
            return cmd.ExecuteNonQuery() == 1;
        }

        /// <summary>Deletes one of the owner's credentials - final: the row is gone, a stale commit gets NotFound.</summary>
        public bool Delete(string ownerAccountId, string rowId)
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "DELETE FROM passkey_credentials WHERE row_id = $row AND account_id = $owner";
            cmd.Parameters.AddWithValue("$row", rowId ?? "");
            cmd.Parameters.AddWithValue("$owner", ownerAccountId ?? "");
            return cmd.ExecuteNonQuery() == 1;
        }

        private IReadOnlyList<PasskeyCredential> Query(string where, string a, string b)
        {
            var rows = new List<PasskeyCredential>();
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            // "where" is one of the constant clauses above; the values are parameters
            cmd.CommandText = @"
SELECT row_id, rp_id, credential_id, account_id, user_handle, public_key_json, algorithm, sign_count, backup_eligible, label, created_at, last_used_at, version
FROM passkey_credentials " + where;
            cmd.Parameters.AddWithValue("$a", a ?? "");
            if (b != null)
                cmd.Parameters.AddWithValue("$b", b);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                rows.Add(new PasskeyCredential
                {
                    RowId = r.GetString(0),
                    RpId = r.GetString(1),
                    CredentialId = r.GetString(2),
                    AccountId = r.GetString(3),
                    UserHandle = r.GetString(4),
                    PublicKeyJson = r.GetString(5),
                    Algorithm = r.GetInt32(6),
                    SignCount = r.GetInt64(7),
                    BackupEligible = r.GetInt64(8) == 1,
                    Label = r.GetString(9),
                    CreatedAt = AccessOpsDatabase.ParseStamp(r.GetString(10)),
                    LastUsedAt = r.IsDBNull(11) ? null : AccessOpsDatabase.ParseStamp(r.GetString(11)),
                    Version = r.GetInt64(12)
                });
            }
            return rows;
        }
    }
}
