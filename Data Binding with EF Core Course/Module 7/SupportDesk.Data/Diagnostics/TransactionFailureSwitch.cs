namespace SupportDesk.Data.Diagnostics;

/// <summary>
/// Lab prop for Module 7's transaction demo: while <see cref="FailAfterFirstWrite"/> is true, the next call
/// to <c>TicketCommandService.CloseTicketWithCommentAsync</c> throws immediately after its first write
/// (the ticket's status update) and before its second (the closing comment insert), so the reviewer can
/// see the whole unit roll back instead of leaving a half-finished change. It is a singleton, exactly like
/// <see cref="DevelopmentOutageSwitch"/>, and it resets itself after firing once — the next click without
/// re-arming it runs the ordinary, successful path.
/// </summary>
public sealed class TransactionFailureSwitch
{
    public bool FailAfterFirstWrite { get; set; }
}

/// <summary>Thrown by <c>TicketCommandService.CloseTicketWithCommentAsync</c> when <see cref="TransactionFailureSwitch.FailAfterFirstWrite"/> is armed — a deliberate failure to show the rollback, never a real database error.</summary>
public sealed class SimulatedTransactionFailureException : Exception
{
    public SimulatedTransactionFailureException()
        : base("Simulated failure after the first write (TransactionFailureSwitch.FailAfterFirstWrite = true) — the transaction rolls back, the comment is never written and the status is unchanged.")
    {
    }
}
