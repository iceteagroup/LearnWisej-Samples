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
    /// triggers) plus one structured log line per event.
    /// </summary>
    public sealed class AuditLog
    {
        private readonly AccessOpsDatabase _db;
        private readonly ILogger<AuditLog> _logger;

        public AuditLog(AccessOpsDatabase db, ILogger<AuditLog> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>A new correlation ID (12 hex characters).</summary>
        public static string NewCorrelationId() => Guid.NewGuid().ToString("N")[..12];

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
