namespace SupportDesk.Data;

/// <summary>
/// Module 1 keeps the model deliberately small: one entity so the first count query has
/// something to count. Customer, Agent, Category and TicketComment, the relationships,
/// the RowVersion token and the indexes all arrive in Module 2.
/// </summary>
public class Ticket
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
