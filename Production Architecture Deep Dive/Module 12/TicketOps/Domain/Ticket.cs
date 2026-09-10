using System;

namespace TicketOps.Domain
{
    public enum TicketPriority
    {
        Low,
        Medium,
        High
    }

    public enum TicketStatus
    {
        Open,
        InProgress,
        Closed
    }

    /// <summary>
    /// The ticket record and its own rules. Plain C# with no UI dependency: it would compile in a class
    /// library that never references Wisej.NET, which is the test of a good Domain type.
    /// </summary>
    public sealed class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public double HoursLogged { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ClosedAt { get; set; }

        /// <summary>
        /// The rule the junior app hid inside btnClose_Click: only an open ticket with logged hours may close.
        /// Living here it can be unit-tested without a browser and reused by a bulk-close action.
        /// </summary>
        public bool CanClose(out string reason)
        {
            if (Status == TicketStatus.Closed)
            {
                reason = "The ticket is already closed.";
                return false;
            }

            if (HoursLogged <= 0)
            {
                reason = "Log hours before closing.";
                return false;
            }

            reason = null;
            return true;
        }

        public void Close()
        {
            if (!CanClose(out string reason))
                throw new InvalidOperationException(reason);

            Status = TicketStatus.Closed;
            ClosedAt = DateTime.Now;
        }
    }
}
