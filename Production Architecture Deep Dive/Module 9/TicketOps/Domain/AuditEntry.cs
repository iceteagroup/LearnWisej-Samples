using System;

namespace TicketOps.Domain
{
    /// <summary>
    /// One line of the server-side audit log ("Ticket link copied — 2002 (L. Romero)" in the video).
    /// Written by <c>IAuditLogService</c> only after the browser confirmed the copy: the record belongs
    /// to the server, the browser only reports.
    /// </summary>
    public sealed class AuditEntry
    {
        public DateTime Time { get; init; }
        public string Action { get; init; }
        public int WorkOrderId { get; init; }
        public string UserName { get; init; }

        public override string ToString() => $"{Time:HH:mm:ss}  {Action} — #{WorkOrderId} ({UserName})";
    }
}
