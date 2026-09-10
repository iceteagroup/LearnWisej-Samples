using System;

namespace TicketOps.Domain
{
    public enum TicketStatus
    {
        Open,
        InProgress,
        Closed
    }

    /// <summary>
    /// The work-order ticket and its own rules. Plain C# with no UI dependency.
    /// <see cref="Note"/> is user-provided text: the domain stores it verbatim (data, not markup) and the
    /// UI decides how to render it safely (see Security/HtmlPolicy). Who may close or delete a ticket is
    /// NOT a domain rule — it is authorization, and it lives in the service that performs the action.
    /// </summary>
    public sealed class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ClosedAt { get; set; }
        public string ClosedBy { get; set; }

        public bool CanClose(out string reason)
        {
            if (Status == TicketStatus.Closed)
            {
                reason = "The ticket is already closed.";
                return false;
            }

            reason = null;
            return true;
        }

        public void Close(string closedBy)
        {
            if (!CanClose(out string reason))
                throw new InvalidOperationException(reason);

            Status = TicketStatus.Closed;
            ClosedAt = DateTime.Now;
            ClosedBy = closedBy;
        }
    }
}
