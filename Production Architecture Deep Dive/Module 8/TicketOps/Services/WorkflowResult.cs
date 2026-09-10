using TicketOps.Domain;

namespace TicketOps.Services
{
    public enum WorkflowOutcome
    {
        Ok,
        Invalid,
        Denied,
        NotFound
    }

    /// <summary>
    /// What the presenter hands back to the screen: an outcome the screen can render without knowing
    /// how it was decided, and a message the user may read. Expected "no" answers (invalid input,
    /// permission denied, gone) are outcomes — only unexpected failures are exceptions.
    /// </summary>
    public sealed class WorkflowResult
    {
        public WorkflowOutcome Outcome { get; }
        public string Message { get; }
        public Ticket Ticket { get; }

        public bool Succeeded => Outcome == WorkflowOutcome.Ok;

        private WorkflowResult(WorkflowOutcome outcome, string message, Ticket ticket)
        {
            Outcome = outcome;
            Message = message;
            Ticket = ticket;
        }

        public static WorkflowResult Ok(Ticket ticket, string message) => new WorkflowResult(WorkflowOutcome.Ok, message, ticket);
        public static WorkflowResult Invalid(string message) => new WorkflowResult(WorkflowOutcome.Invalid, message, null);
        public static WorkflowResult Denied(string message) => new WorkflowResult(WorkflowOutcome.Denied, message, null);
        public static WorkflowResult NotFound(string message) => new WorkflowResult(WorkflowOutcome.NotFound, message, null);

        public override string ToString() => $"{Outcome} · {Message}";
    }
}
