using System;
using System.Collections.Generic;
using System.Globalization;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using Microsoft.Data.Sqlite;

namespace AccessOps.Security
{
    /// <summary>
    /// The grants, in the AccessOps database. <see cref="Find"/> always reads the row as it is now,
    /// so a revocation made in another session is seen by the very next decision - including a
    /// click on a screen that was loaded before the revocation, and a queued job.
    /// </summary>
    public sealed class GrantStore
    {
        private readonly AccessOpsDatabase _db;
        private readonly AuditLog _audit;
        private readonly IClock _clock;

        public GrantStore(AccessOpsDatabase db, AuditLog audit, IClock clock)
        {
            _db = db;
            _audit = audit;
            _clock = clock;
        }

        /// <summary>The grant's current state, or null when no grant has this ID.</summary>
        public Grant Find(string grantId)
        {
            if (string.IsNullOrEmpty(grantId))
                return null;

            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT id, account_id, tenant, role, can_read, can_admin, revoked, expires_at FROM grants WHERE id = $id";
            cmd.Parameters.AddWithValue("$id", grantId);
            using var r = cmd.ExecuteReader();
            return r.Read() ? Map(r) : null;
        }

        /// <summary>True when the subject's current grant is valid and allows administration.</summary>
        public bool CanAdminister(Subject subject)
        {
            var grant = Find(subject?.GrantId);
            return grant != null
                && !grant.Revoked
                && grant.ExpiresAt > _clock.Now
                && grant.CanAdminister
                && StringComparer.Ordinal.Equals(grant.AccountId, subject.AccountId);
        }

        /// <summary>Every grant with its account's sign-in name, for the admin console.</summary>
        public IReadOnlyList<GrantRow> List(Subject administrator)
        {
            if (!CanAdminister(administrator))
                return null;

            var rows = new List<GrantRow>();
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = @"
SELECT g.id, g.account_id, g.tenant, g.role, g.can_read, g.can_admin, g.revoked, g.expires_at, a.user_name
FROM grants g JOIN accounts a ON a.id = g.account_id
ORDER BY g.tenant, a.user_name";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var grant = Map(r);
                rows.Add(new GrantRow
                {
                    GrantId = grant.Id,
                    Grant = "…" + Redact.Tail(grant.Id, 2),
                    Account = r.GetString(8),
                    Tenant = grant.Tenant,
                    Role = grant.Role,
                    State = grant.Revoked ? "Revoked" : grant.ExpiresAt <= _clock.Now ? "Expired" : "Active"
                });
            }
            return rows;
        }

        /// <summary>
        /// Revokes (or restores) a grant. Only a subject whose own current grant allows
        /// administration may do it; both outcomes are audited.
        /// </summary>
        public bool SetRevoked(Subject administrator, string grantId, bool revoked)
        {
            var action = revoked ? "grant.revoke" : "grant.restore";
            var correlationId = AuditLog.NewCorrelationId();
            if (!CanAdminister(administrator))
            {
                _audit.Write(new AuditEvent(administrator.Opaque, action, "grant …" + Redact.Tail(grantId, 2), "-", "denied", "administration", correlationId));
                return false;
            }

            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "UPDATE grants SET revoked = $revoked WHERE id = $id";
            cmd.Parameters.AddWithValue("$revoked", revoked ? 1 : 0);
            cmd.Parameters.AddWithValue("$id", grantId);
            var changed = cmd.ExecuteNonQuery() == 1;

            _audit.Write(new AuditEvent(administrator.Opaque, action, "grant …" + Redact.Tail(grantId, 2), "-",
                changed ? "allowed" : "denied", changed ? null : "no such grant", correlationId));
            return changed;
        }

        private static Grant Map(SqliteDataReader r) => new Grant
        {
            Id = r.GetString(0),
            AccountId = r.GetString(1),
            Tenant = r.GetString(2),
            Role = r.GetString(3),
            CanRead = r.GetInt64(4) == 1,
            CanAdminister = r.GetInt64(5) == 1,
            Revoked = r.GetInt64(6) == 1,
            ExpiresAt = DateTimeOffset.Parse(r.GetString(7), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
        };
    }

    /// <summary>One row of the admin console's grant list.</summary>
    public sealed class GrantRow
    {
        /// <summary>Full grant ID (kept on the server: the grid has no column for it).</summary>
        public string GrantId { get; init; }

        /// <summary>The grant ID cut to its last two characters, as the console shows it.</summary>
        public string Grant { get; init; }

        public string Account { get; init; }

        public string Tenant { get; init; }

        public string Role { get; init; }

        public string State { get; init; }
    }
}
