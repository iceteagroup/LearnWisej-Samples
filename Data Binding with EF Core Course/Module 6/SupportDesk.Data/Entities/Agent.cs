namespace SupportDesk.Data;

/// <summary>
/// Lookup entity: a support agent who can own tickets. Deleting an agent unassigns their tickets
/// (DeleteBehavior.SetNull on <see cref="Ticket.AgentId"/>) — a departing agent must never take
/// the queue with them.
/// </summary>
public class Agent
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }

    public List<Ticket> Tickets { get; set; } = new();
}
