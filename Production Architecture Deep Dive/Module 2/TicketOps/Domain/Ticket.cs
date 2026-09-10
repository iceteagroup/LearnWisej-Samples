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
    /// The ticket record. Plain C# with no UI dependency — it would compile in a class library that
    /// never references Wisej.NET. Module 2 adds <see cref="Tenant"/> and <see cref="Author"/>: both are
    /// stamped by the service from the session's <c>SessionContext</c>, never from a static field.
    /// </summary>
    public sealed class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Tenant { get; set; }
        public string Author { get; set; }
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public override string ToString() => $"#{Id} \"{Title}\" ({Tenant} · {Author})";
    }
}
