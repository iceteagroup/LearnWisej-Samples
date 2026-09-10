using System;
using System.Collections.Generic;
using System.Linq;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// Fake profile: an in-memory audit trail. Registered Shared, so it is written from many sessions at
    /// once — hence the lock. Nothing per-user lives here; the operator id arrives with every call.
    /// </summary>
    public sealed class FakeAuditLogService : IAuditLogService
    {
        private readonly object _gate = new object();
        private readonly List<AuditEntry> _entries = new List<AuditEntry>();

        public int Count
        {
            get { lock (_gate) return _entries.Count; }
        }

        public AuditEntry Record(string action, int ticketId, int operatorId, string detail)
        {
            if (string.IsNullOrWhiteSpace(action)) throw new ArgumentException("An audit action is required.", nameof(action));

            lock (_gate)
            {
                var entry = new AuditEntry
                {
                    Sequence = _entries.Count + 1,
                    At = DateTime.Now,
                    Action = action,
                    TicketId = ticketId,
                    OperatorId = operatorId,
                    Detail = detail
                };
                _entries.Add(entry);
                return entry;
            }
        }

        public IReadOnlyList<AuditEntry> Recent(int count)
        {
            lock (_gate) return _entries.Skip(Math.Max(0, _entries.Count - count)).ToList();
        }
    }
}
