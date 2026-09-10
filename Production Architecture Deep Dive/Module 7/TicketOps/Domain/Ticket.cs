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
    /// The ticket record. Plain C# with no UI dependency: it would compile in a class library that never
    /// references Wisej.NET. Module 7 adds the two columns the CSV import carries (assignee, due date).
    /// </summary>
    public sealed class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public string Assignee { get; set; }
        public DateTime? DueDate { get; set; }
        public double HoursLogged { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ClosedAt { get; set; }

        public override string ToString()
            => $"#{Id} \"{Title}\" {Priority} → {Assignee ?? "-"} due {(DueDate.HasValue ? DueDate.Value.ToString("yyyy-MM-dd") : "-")}";
    }
}
