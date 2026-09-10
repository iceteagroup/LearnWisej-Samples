using System;
using System.Collections.Generic;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// In-memory audit log, one per session (created in AppComposition). In production this writes to a
    /// table or an append-only store; the contract and the rule stay the same.
    /// </summary>
    public sealed class AuditLogService : IAuditLogService
    {
        private readonly List<AuditEntry> _entries = new List<AuditEntry>();
        private readonly ILog _log;

        public AuditLogService(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public IReadOnlyList<AuditEntry> Entries => _entries.ToArray();

        public void Record(string action, int workOrderId, SessionUser user)
        {
            if (string.IsNullOrWhiteSpace(action)) throw new ArgumentException("An audit action is required.", nameof(action));
            if (user == null) throw new ArgumentNullException(nameof(user));

            var entry = new AuditEntry
            {
                Time = DateTime.Now,
                Action = action,
                WorkOrderId = workOrderId,
                UserName = user.Name
            };
            _entries.Add(entry);
            _log.Info(LogLayer.Service, "AuditLogService.Record", $"{action} — #{workOrderId} by {user.Name} (server owns the record)");
        }
    }
}
