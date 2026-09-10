using System;
using System.Collections.Generic;
using System.Linq;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// Production profile: the audit trail lives in dbo.AuditLog. This stand-in keeps the rows in memory
    /// and exposes the INSERT it would run through <see cref="LastStatement"/> (a Shared service has no
    /// session log to write to — the presenter, which is session-scoped, logs on its behalf).
    /// </summary>
    public sealed class SqlAuditLogService : IAuditLogService
    {
        private readonly object _gate = new object();
        private readonly List<AuditEntry> _rows = new List<AuditEntry>();

        public int Count
        {
            get { lock (_gate) return _rows.Count; }
        }

        /// <summary>The statement the last <see cref="Record"/> call would have executed.</summary>
        public string LastStatement { get; private set; }

        public AuditEntry Record(string action, int ticketId, int operatorId, string detail)
        {
            if (string.IsNullOrWhiteSpace(action)) throw new ArgumentException("An audit action is required.", nameof(action));

            lock (_gate)
            {
                var entry = new AuditEntry
                {
                    Sequence = _rows.Count + 1,
                    At = DateTime.Now,
                    Action = action,
                    TicketId = ticketId,
                    OperatorId = operatorId,
                    Detail = detail
                };
                _rows.Add(entry);
                LastStatement = $"INSERT INTO dbo.AuditLog (Action, TicketId, OperatorId, Detail, At) VALUES ('{action}', {ticketId}, {operatorId}, @detail, SYSDATETIME())";
                return entry;
            }
        }

        public IReadOnlyList<AuditEntry> Recent(int count)
        {
            lock (_gate) return _rows.Skip(Math.Max(0, _rows.Count - count)).ToList();
        }
    }
}
