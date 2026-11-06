using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using AccessOps.Infrastructure;
using Microsoft.Data.Sqlite;

namespace AccessOps.Services
{
    public sealed class AuditRow
    {
        public long Seq { get; init; }
        public string AtUtc { get; init; }
        public string Actor { get; init; }
        public string Action { get; init; }
        public string Target { get; init; }
        public string Outcome { get; init; }
        public string ClientAddress { get; init; }
        public string CorrelationId { get; init; }
        public string Hash { get; init; }
    }

    /// <summary>
    /// The only writer of audit_entry. One row answers who (a stable id), did what (the action), to
    /// what (target type and id), when (UTC, server clock), from where (address and agent), under
    /// which correlation ID, and how it ended - denials included. Rows are append-only (triggers
    /// refuse UPDATE and DELETE), numbered by a gapless sequence and hash-chained: each row's hash
    /// covers its own fields and the previous row's hash, so <see cref="VerifyChain"/> names the first
    /// row that no longer matches.
    /// </summary>
    public sealed class AuditService
    {
        private readonly AccessOpsDatabase _db;

        public AuditService(AccessOpsDatabase db) => _db = db;

        /// <summary>Records in its own transaction (a denial, a sign-in attempt: no business change to share).</summary>
        public long Record(CommandContext ctx, string action, string targetType, string targetId, string outcome)
        {
            using var connection = _db.Open();
            using var tx = connection.BeginTransaction();
            var seq = Record(connection, tx, ctx, action, targetType, targetId, outcome);
            tx.Commit();
            return seq;
        }

        /// <summary>Records inside the caller's transaction: the business change and its audit row commit together or not at all.</summary>
        public long Record(SqliteConnection connection, SqliteTransaction tx, CommandContext ctx,
            string action, string targetType, string targetId, string outcome)
        {
            long previousSeq = 0;
            string previousHash = "";
            using (var last = connection.CreateCommand())
            {
                last.Transaction = tx;
                last.CommandText = "SELECT seq, hash FROM audit_entry ORDER BY seq DESC LIMIT 1";
                using var r = last.ExecuteReader();
                if (r.Read())
                {
                    previousSeq = r.GetInt64(0);
                    previousHash = r.GetString(1);
                }
            }

            var row = new AuditRow
            {
                Seq = previousSeq + 1,                                   // gapless: the next number, inside the write transaction
                AtUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                Actor = ctx.Actor,
                Action = action,
                Target = targetType + ":" + targetId,
                Outcome = outcome,
                ClientAddress = ctx.ClientAddress ?? "unknown",
                CorrelationId = ctx.CorrelationId
            };
            var hash = Hash(row, Truncate(ctx.UserAgent ?? "", 200), previousHash);

            using var insert = connection.CreateCommand();
            insert.Transaction = tx;
            insert.CommandText = @"
INSERT INTO audit_entry (seq, at_utc, actor, action, target_type, target_id, outcome, client_address, user_agent, correlation_id, prev_hash, hash)
VALUES ($seq, $at, $actor, $action, $type, $id, $outcome, $address, $agent, $correlation, $prev, $hash);";
            insert.Parameters.AddWithValue("$seq", row.Seq);
            insert.Parameters.AddWithValue("$at", row.AtUtc);
            insert.Parameters.AddWithValue("$actor", row.Actor);
            insert.Parameters.AddWithValue("$action", action);
            insert.Parameters.AddWithValue("$type", targetType);
            insert.Parameters.AddWithValue("$id", targetId);
            insert.Parameters.AddWithValue("$outcome", outcome);
            insert.Parameters.AddWithValue("$address", row.ClientAddress);
            insert.Parameters.AddWithValue("$agent", Truncate(ctx.UserAgent ?? "", 200));
            insert.Parameters.AddWithValue("$correlation", row.CorrelationId);
            insert.Parameters.AddWithValue("$prev", previousHash);
            insert.Parameters.AddWithValue("$hash", hash);
            insert.ExecuteNonQuery();
            return row.Seq;
        }

        public IReadOnlyList<AuditRow> Latest(int count)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT seq, at_utc, actor, action, target_type || ':' || target_id, outcome, client_address, correlation_id, hash
FROM audit_entry ORDER BY seq DESC LIMIT $n";
            cmd.Parameters.AddWithValue("$n", count);
            var rows = new List<AuditRow>();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                rows.Add(new AuditRow
                {
                    Seq = r.GetInt64(0), AtUtc = r.GetString(1), Actor = r.GetString(2), Action = r.GetString(3),
                    Target = r.GetString(4), Outcome = r.GetString(5), ClientAddress = r.GetString(6),
                    CorrelationId = r.GetString(7), Hash = r.GetString(8)
                });
            }
            return rows;
        }

        public int CountToday()
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM audit_entry WHERE substr(at_utc, 1, 10) = $today";
            cmd.Parameters.AddWithValue("$today", DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        /// <summary>
        /// Recomputes the chain from the first row. Returns null when every row matches, otherwise the
        /// sequence number of the first row that was altered, removed or inserted out of order.
        /// </summary>
        public long? VerifyChain()
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT seq, at_utc, actor, action, target_type || ':' || target_id, outcome, client_address, user_agent, correlation_id, prev_hash, hash
FROM audit_entry ORDER BY seq";
            using var r = cmd.ExecuteReader();
            long expectedSeq = 1;
            var previousHash = "";
            while (r.Read())
            {
                var row = new AuditRow
                {
                    Seq = r.GetInt64(0), AtUtc = r.GetString(1), Actor = r.GetString(2), Action = r.GetString(3),
                    Target = r.GetString(4), Outcome = r.GetString(5), ClientAddress = r.GetString(6), CorrelationId = r.GetString(8)
                };
                if (row.Seq != expectedSeq || r.GetString(9) != previousHash || Hash(row, r.GetString(7), previousHash) != r.GetString(10))
                    return row.Seq;

                previousHash = r.GetString(10);
                expectedSeq++;
            }
            return null;
        }

        private static string Hash(AuditRow row, string userAgent, string previousHash)
        {
            var text = string.Join("\u001f", row.Seq.ToString(CultureInfo.InvariantCulture), row.AtUtc, row.Actor, row.Action,
                row.Target, row.Outcome, row.ClientAddress, userAgent, row.CorrelationId, previousHash);
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();
        }

        private static string Truncate(string s, int max) => s.Length <= max ? s : s.Substring(0, max);
    }
}
