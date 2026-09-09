using System;
using System.Collections.Generic;

namespace EnterpriseOps.Security
{
    /// <summary>One server-side audit record. Offline work carries two times: when the device did it and when it synced.</summary>
    public class AuditEntry
    {
        public int Id;
        public string TenantId;
        public string User;
        public string Action;
        public string EntityId;
        public string Outcome;             // Applied · Rejected · Conflict · Resolved
        public string Detail;
        public DateTimeOffset? DeviceTimestamp;   // when the technician did it (device clock, offline)
        public DateTimeOffset ServerTimestamp;    // when the server recorded it (sync time for offline commands)
        public string CorrelationId;

        public override string ToString()
        {
            string device = DeviceTimestamp.HasValue ? $"device {DeviceTimestamp.Value:HH:mm:ss} · " : "";
            return $"#{Id} {Action} {EntityId} by {User} → {Outcome} · {device}server {ServerTimestamp:HH:mm:ss} · {Detail}";
        }
    }

    /// <summary>Append-only, server-side. Every offline command lands here whatever its outcome.</summary>
    public class AuditLog
    {
        private readonly object _gate = new object();
        private readonly List<AuditEntry> _entries = new List<AuditEntry>();

        public AuditEntry Append(string tenantId, string user, string action, string entityId, string outcome, string detail,
                                 string correlationId, DateTimeOffset? deviceTimestamp = null)
        {
            lock (_gate)
            {
                var entry = new AuditEntry
                {
                    Id = _entries.Count + 1,
                    TenantId = tenantId,
                    User = user,
                    Action = action,
                    EntityId = entityId,
                    Outcome = outcome,
                    Detail = detail,
                    DeviceTimestamp = deviceTimestamp,
                    ServerTimestamp = DateTimeOffset.Now,
                    CorrelationId = correlationId,
                };
                _entries.Add(entry);
                return entry;
            }
        }

        public IReadOnlyList<AuditEntry> All()
        {
            lock (_gate) return _entries.ToArray();
        }
    }
}
