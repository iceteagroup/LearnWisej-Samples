namespace SupportDesk.Data;

/// <summary>
/// Lookup entity: the kind of request (Hardware, Network, Accounts, …). Required on every ticket;
/// deleting a category that is still in use is refused (DeleteBehavior.Restrict).
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Ticket> Tickets { get; set; } = new();
}
