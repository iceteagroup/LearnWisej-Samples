using System;

namespace TicketOpsLive.Models
{
    /// <summary>
    /// The domain event of the lesson. It is a value message, not a UI instruction: it says WHAT happened and
    /// carries enough metadata for a session to decide whether the event concerns it.
    ///
    ///   Ticket       a snapshot (clone) of the changed record — never the instance the hub stores;
    ///   ChangeType   "Added", "Updated" or "Escalated" — the sessions filter on this too (only "Escalated" pops a toast);
    ///   Message      the human line the notifications list shows;
    ///   EventId      a short id so the same event can be followed across sessions in the traces;
    ///   PublishedBy  the Application.ClientId of the session that published it ("who fired this?");
    ///   TenantId     the tenant metadata — sessions of another tenant drop the event (targeted update).
    /// </summary>
    public class TicketChangedEventArgs : EventArgs
    {
        public Ticket Ticket { get; set; }
        public string ChangeType { get; set; }
        public string Message { get; set; }
        public string EventId { get; set; }
        public string PublishedBy { get; set; }
        public string TenantId { get; set; }

        /// <summary>A short, readable event id (the first 8 characters of a Guid).</summary>
        public static string NewEventId()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }
    }
}
