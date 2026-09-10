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
    /// The ticket record and its own rules. Plain C# with no UI and no Wisej.NET dependency: it would
    /// compile in a class library that never references the framework, which is the test of a Domain type.
    /// Both ticket services (the fake and the production-shaped one) apply the same rule through
    /// <see cref="CanClose"/> — the rule is written once, here, not once per implementation.
    /// </summary>
    public sealed class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public double HoursLogged { get; set; }

        /// <summary>Operator id of the assignee; 0 means unassigned.</summary>
        public int AssigneeId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ClosedAt { get; set; }
        public string CloseReason { get; set; }

        public bool IsAssigned => AssigneeId != 0;

        /// <summary>The domain rule: only an open ticket with logged hours may close.</summary>
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

        public void Close(string closeReason)
        {
            if (!CanClose(out string reason))
                throw new InvalidOperationException(reason);

            Status = TicketStatus.Closed;
            ClosedAt = DateTime.Now;
            CloseReason = closeReason;
        }

        public void AssignTo(int operatorId)
        {
            AssigneeId = operatorId;
            if (Status == TicketStatus.Open && HoursLogged > 0)
                Status = TicketStatus.InProgress;
        }

        public Ticket Clone() => new Ticket
        {
            Id = Id,
            Title = Title,
            Priority = Priority,
            Status = Status,
            HoursLogged = HoursLogged,
            AssigneeId = AssigneeId,
            CreatedAt = CreatedAt,
            ClosedAt = ClosedAt,
            CloseReason = CloseReason
        };

        public override string ToString() => $"#{Id} \"{Title}\" ({Status}, {HoursLogged}h)";
    }
}
