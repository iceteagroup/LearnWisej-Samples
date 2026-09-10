namespace SupportDesk.Data;

/// <summary>
/// The centre of the Support Desk model. A ticket belongs to a <see cref="Customer"/> and a
/// <see cref="Category"/> (required), may be assigned to an <see cref="Agent"/> (nullable, so a new
/// ticket exists before anyone owns it), and carries a detail collection of <see cref="TicketComment"/>.
/// <see cref="RowVersion"/> is the optimistic-concurrency token (see SupportDeskContext for how it is
/// filled on SQLite). No mapping attributes here: every schema rule lives in
/// <c>SupportDeskContext.OnModelCreating</c>.
/// </summary>
public class Ticket
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Open";
    public string Priority { get; set; } = "Normal";
    public bool IsUrgent { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int? AgentId { get; set; }
    public Agent? Agent { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public List<TicketComment> Comments { get; set; } = new();

    /// <summary>Concurrency token. Written by <c>SupportDeskContext.SaveChanges</c> on every insert and update.</summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
