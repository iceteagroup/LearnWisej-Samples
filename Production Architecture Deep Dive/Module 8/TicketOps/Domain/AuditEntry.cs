using System;

namespace TicketOps.Domain
{
    /// <summary>
    /// Who did what, when. Recorded by IAuditLogService, which is Shared (one instance for the whole
    /// server), so the entry carries the operator id explicitly: a shared service must never
    /// "remember" the current user — the caller passes it in.
    /// </summary>
    public sealed class AuditEntry
    {
        public int Sequence { get; init; }
        public DateTime At { get; init; }
        public string Action { get; init; }
        public int TicketId { get; init; }
        public int OperatorId { get; init; }
        public string Detail { get; init; }

        public override string ToString() => $"#{Sequence} {Action} ticket {TicketId} by operator {OperatorId}";
    }
}
