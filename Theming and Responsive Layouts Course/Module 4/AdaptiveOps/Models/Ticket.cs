using System;

namespace AdaptiveOps.Models
{
    public enum TicketPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum TicketStatus
    {
        Open,
        Assigned,
        Waiting,
        Resolved,
        Closed
    }

    /// <summary>
    /// One operations ticket. The same data model is used by every module of the course;
    /// the shell shows it in the ticket grid and edits it in the details panel.
    /// </summary>
    public sealed class Ticket
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public string Owner { get; set; }
        public DateTime DueDate { get; set; }
        public string Notes { get; set; }

        /// <summary>Open tickets are everything that is not Resolved or Closed.</summary>
        public bool IsOpen => Status != TicketStatus.Resolved && Status != TicketStatus.Closed;

        /// <summary>An open ticket whose due date is already in the past.</summary>
        public bool IsOverdue(DateTime today) => IsOpen && DueDate.Date < today.Date;

        public Ticket Clone()
        {
            return new Ticket
            {
                Id = Id,
                Title = Title,
                Priority = Priority,
                Status = Status,
                Owner = Owner,
                DueDate = DueDate,
                Notes = Notes
            };
        }
    }
}
