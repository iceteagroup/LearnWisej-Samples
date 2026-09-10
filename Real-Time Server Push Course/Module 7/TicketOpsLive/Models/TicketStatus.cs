namespace TicketOpsLive.Models
{
    /// <summary>The ticket lifecycle used by the whole course (lesson: "The Ticket model").</summary>
    public enum TicketStatus
    {
        New,
        Assigned,
        Waiting,
        Resolved,
        Escalated
    }
}
