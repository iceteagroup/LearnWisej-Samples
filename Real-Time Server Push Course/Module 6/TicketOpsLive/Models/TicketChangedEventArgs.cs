using System;

namespace TicketOpsLive.Models
{
    /// <summary>
    /// The domain event: a value message that says what happened and carries the metadata a session needs to
    /// decide whether the event concerns it.
    ///
    ///   Ticket       a snapshot (clone) of the changed record — never the instance the hub stores;
    ///   ChangeType   "Added", "Updated" or "Escalated" (only "Escalated" pops a toast);
    ///   Message      the line the notifications list shows;
    ///   TenantId     the tenant metadata — sessions of another tenant drop the event.
    /// </summary>
    public class TicketChangedEventArgs : EventArgs
    {
        public Ticket Ticket { get; set; }
        public string ChangeType { get; set; }
        public string Message { get; set; }
        public string TenantId { get; set; }
    }
}
