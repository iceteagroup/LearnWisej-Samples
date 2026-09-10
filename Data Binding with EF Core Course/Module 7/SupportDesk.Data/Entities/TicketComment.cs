namespace SupportDesk.Data;

/// <summary>
/// Detail row under a ticket. Comments have no life of their own: deleting the ticket removes them
/// (DeleteBehavior.Cascade).
/// </summary>
public class TicketComment
{
    public int Id { get; set; }

    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public string Body { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
