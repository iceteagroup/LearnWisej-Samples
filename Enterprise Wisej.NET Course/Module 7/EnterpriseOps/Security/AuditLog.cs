using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnterpriseOps.Services;

namespace EnterpriseOps.Security
{
    /// <summary>One audit entry — who did what, to which record, under which correlation id.</summary>
    public class AuditEntry
    {
        public DateTime Utc;
        public string Action;
        public string Subject;
        public string User;
        public string CorrelationId;
        public string Detail;

        public override string ToString() => $"{Utc:HH:mm:ss} {Action} {Subject} by {User} [{CorrelationId}] {Detail}";
    }

    /// <summary>Thrown by the audit store when the write fails.</summary>
    public class AuditWriteException : Exception
    {
        public AuditWriteException(string message) : base(message) { }
    }

    /// <summary>
    /// In-memory audit log. In production this is a separate store (append-only table, SIEM, …) — which is
    /// exactly why it can fail AFTER the notification was already sent.
    /// </summary>
    public class AuditLog
    {
        private readonly ActivityTrace _trace;
        private readonly List<AuditEntry> _entries = new List<AuditEntry>();

        public AuditLog(ActivityTrace trace)
        {
            _trace = trace;
        }

        public IReadOnlyList<AuditEntry> Entries => _entries;

        public async Task WriteAsync(string action, string subject, string user, string correlationId, string detail)
        {
            await Task.Delay(120);                  // a remote append-only store, not a local list

            _entries.Add(new AuditEntry
            {
                Utc = DateTime.UtcNow,
                Action = action,
                Subject = subject,
                User = user,
                CorrelationId = correlationId,
                Detail = detail,
            });
            _trace.Write($"Security: audit {action} {subject} by {user} [{correlationId}] {detail}");
        }
    }
}
