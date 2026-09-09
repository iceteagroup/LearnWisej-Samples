using System;
using System.Collections.Generic;
using System.Globalization;

namespace EnterpriseOps.Security
{
    public sealed class AuditEntry
    {
        public DateTime TimestampUtc { get; init; }
        public string UserName { get; init; }
        public string Action { get; init; }
        public string Outcome { get; init; }
        public string CorrelationId { get; init; }

        public override string ToString() =>
            $"{TimestampUtc.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} {UserName} {Action} → {Outcome} · correlation {CorrelationId}";
    }

    /// <summary>
    /// Who did what, with the correlation id of the action. Opening the diagnostics page is itself audited —
    /// allowed or denied — so a support engineer can prove who looked at what.
    /// </summary>
    public sealed class AuditTrail
    {
        private readonly List<AuditEntry> _entries = new List<AuditEntry>();

        public IReadOnlyList<AuditEntry> Entries => _entries;

        public AuditEntry Record(string userName, string action, string outcome, string correlationId)
        {
            var entry = new AuditEntry
            {
                TimestampUtc = DateTime.UtcNow,
                UserName = userName,
                Action = action,
                Outcome = outcome,
                CorrelationId = correlationId,
            };
            _entries.Add(entry);
            return entry;
        }
    }
}
