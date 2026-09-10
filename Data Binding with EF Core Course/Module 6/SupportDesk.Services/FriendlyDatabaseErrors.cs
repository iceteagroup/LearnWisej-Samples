namespace SupportDesk.Services;

/// <summary>
/// Module 5's final safety-layer messages: what the operator reads when the database itself rejects a
/// change that TicketValidator could not see coming (a duplicate ticket number is the lab's example — only
/// the database can guarantee uniqueness). UI-free and testable alone, on purpose: <c>TicketEditorForm</c>'s
/// <c>DbUpdateException</c> handler shows this text instead of composing its own, so a test can assert
/// directly — without a form, a session or a database — that nothing about SQL, a constraint name or a
/// connection string ever reaches the sentence a customer might see over an agent's shoulder.
/// </summary>
public static class FriendlyDatabaseErrors
{
    /// <summary>Shown when <c>SaveChangesAsync</c> throws <c>DbUpdateException</c> while saving a ticket (the duplicate-number lab prop, or any other constraint the database refuses).</summary>
    public const string TicketSaveRejected =
        "The ticket could not be saved because the database rejected the change. Please verify required fields and lookup values.";

    /// <summary>Shown when <c>SaveChangesAsync</c> throws <c>DbUpdateException</c> while deleting a ticket.</summary>
    public const string TicketDeleteRejected =
        "The ticket could not be deleted because the database rejected the change.";
}
