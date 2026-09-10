namespace SupportDesk.Data;

/// <summary>
/// Lookup entity: the company or person a ticket is raised for. Small, stable, and shaped for a
/// ComboBox (Id + Name). Deleting a customer that still has tickets is refused (DeleteBehavior.Restrict).
/// </summary>
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }

    public List<Ticket> Tickets { get; set; } = new();
}
