using System;
using System.Collections.Generic;
using System.Globalization;
using AccessOps.Infrastructure;
using Microsoft.Extensions.Logging;

namespace AccessOps.Security
{
    /// <summary>
    /// One audit event: opaque subject, action, resource, classification, decision, the reason
    /// (for the auditor, never shown to the user) and a correlation ID. It has no field for a
    /// session ID, cookie, token, name or account ID - there is nowhere to put them.
    /// </summary>
    public sealed record AuditEvent(
        string Subject,
        string Action,
        string Resource,
        string Classification,
        string Decision,
        string Reason,
        string CorrelationId);

    /// <summary>
    /// The append-only audit trail: the <c>audit</c> table (UPDATE and DELETE are refused by
    /// triggers) plus one structured log line per event. Both routes write to the same table in
    /// the shared store. <see cref="Deny"/> is the one call <c>RecordPolicy</c> makes before it
    /// returns a deny - always before the repository could run.
    /// </summary>
    public sealed class AuditService
    {
        private readonly AccessOpsDatabase _db;
        private readonly ILogger<AuditService> _logger;

        public AuditService(AccessOpsDatabase db, ILogger<AuditService> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// The correlation ID of the current click or review case (<see cref="Correlation.Current"/>),
        /// or a new one (12 hex characters) outside such a scope.
        /// </summary>
        public static string NewCorrelationId() => Correlation.Current ?? Guid.NewGuid().ToString("N")[..12];

        /// <summary>
        /// Records a deny at <paramref name="boundary"/> ("policy", "ingress", "callback", …) for
        /// tenant / record. The grant appears only shortened in the log line, never in the table;
        /// <paramref name="reason"/> is an allowlisted word for the auditor ("tenant", "revoked", …).
        /// </summary>
        public void Deny(string boundary, string grantId, string tenantId, int recordId, string reason = null,
            string subject = null, string action = "record.read", string correlationId = null)
        {
            correlationId ??= NewCorrelationId();
            _logger.LogInformation("deny boundary={Boundary} grant={Grant} reason={Reason} correlation={CorrelationId}",
                boundary, Grant.Shorten(grantId), reason ?? "-", correlationId);
            Write(new AuditEvent(subject ?? "anonymous", action, $"{tenantId} / {recordId}", null, "denied",
                string.IsNullOrEmpty(reason) ? boundary : boundary + ":" + reason, correlationId));
        }

        /// <summary>Appends the event.</summary>
        public void Write(AuditEvent e)
        {
            using (var db = _db.Open())
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO audit (at, subject, action, resource, classification, decision, reason, correlation_id)
VALUES ($at, $subject, $action, $resource, $classification, $decision, $reason, $correlation)";
                cmd.Parameters.AddWithValue("$at", DateTimeOffset.UtcNow.ToString("o", CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("$subject", e.Subject);
                cmd.Parameters.AddWithValue("$action", e.Action);
                cmd.Parameters.AddWithValue("$resource", e.Resource);
                cmd.Parameters.AddWithValue("$classification", e.Classification ?? "-");
                cmd.Parameters.AddWithValue("$decision", e.Decision);
                cmd.Parameters.AddWithValue("$reason", (object)e.Reason ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$correlation", e.CorrelationId);
                cmd.ExecuteNonQuery();
            }

            _logger.LogInformation(
                "audit subject={Subject} action={Action} resource={Resource} classification={Classification} decision={Decision} reason={Reason} correlation={CorrelationId}",
                e.Subject, e.Action, e.Resource, e.Classification ?? "-", e.Decision, e.Reason ?? "-", e.CorrelationId);
        }

        /// <summary>The newest event with this action, or null (the release review reads sign-in evidence with it).</summary>
        public AuditRow Latest(string action, DateTimeOffset since)
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = @"
SELECT at, subject, action, resource, decision, IFNULL(reason, '-'), correlation_id
FROM audit WHERE action = $action AND at >= $since ORDER BY seq DESC LIMIT 1";
            cmd.Parameters.AddWithValue("$action", action);
            cmd.Parameters.AddWithValue("$since", since.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture));
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return null;
            return new AuditRow
            {
                Time = DateTimeOffset.Parse(r.GetString(0), CultureInfo.InvariantCulture).ToLocalTime().ToString("HH:mm:ss"),
                Subject = r.GetString(1),
                Action = r.GetString(2),
                Resource = r.GetString(3),
                Decision = r.GetString(4),
                Reason = r.GetString(5),
                Correlation = r.GetString(6)
            };
        }

        /// <summary>Every audit row as one line of text (the review's "no credential material" scan).</summary>
        public IEnumerable<string> AllText()
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT subject || '|' || action || '|' || resource || '|' || classification || '|' || decision || '|' || IFNULL(reason, '') || '|' || correlation_id FROM audit";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                yield return r.GetString(0);
        }

        /// <summary>The newest events first, for the admin console.</summary>
        public IReadOnlyList<AuditRow> Recent(int count)
        {
            var rows = new List<AuditRow>();
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = @"
SELECT at, subject, action, resource, decision, IFNULL(reason, '-'), correlation_id
FROM audit ORDER BY seq DESC LIMIT $count";
            cmd.Parameters.AddWithValue("$count", count);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                rows.Add(new AuditRow
                {
                    Time = DateTimeOffset.Parse(r.GetString(0), CultureInfo.InvariantCulture).ToLocalTime().ToString("HH:mm:ss"),
                    Subject = r.GetString(1),
                    Action = r.GetString(2),
                    Resource = r.GetString(3),
                    Decision = r.GetString(4),
                    Reason = r.GetString(5),
                    Correlation = r.GetString(6)
                });
            }
            return rows;
        }
    }

    /// <summary>One row of the admin console's audit list.</summary>
    public sealed class AuditRow
    {
        public string Time { get; init; }
        public string Subject { get; init; }
        public string Action { get; init; }
        public string Resource { get; init; }
        public string Decision { get; init; }
        public string Reason { get; init; }
        public string Correlation { get; init; }
    }
}
