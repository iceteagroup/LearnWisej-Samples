using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Services.Workflow
{
    /// <summary>How the workflow ended. Callers switch on this — never on a message string or an exception type.</summary>
    public enum WorkflowOutcome
    {
        Created,                    // every step succeeded
        CreatedWithCompensation,    // the escalation exists; an external step failed and was compensated
        ValidationFailed,           // nothing persisted; FieldErrors says which step and field
        Unauthorized,               // nothing persisted; the user or tenant may not escalate
        Failed,                     // an unexpected error; the workflow reverted what it could
    }

    /// <summary>One validation problem, addressed to a wizard step and a field so the UI can jump to it.</summary>
    public sealed record FieldError(WizardStep Step, string Field, string Message);

    /// <summary>What happened at one orchestration step (the strip under the wizard in the video).</summary>
    public sealed record StepOutcome(string Name, StepStatus Status, string Detail);

    public enum StepStatus { Pending, Running, Succeeded, Failed, Skipped, Compensated }

    /// <summary>
    /// The typed result: whether the escalation was created, which validations failed and where,
    /// whether the external step succeeded, and what the user should do next.
    /// Shape from the video (Success, Message, CompensationAction) plus the fields the lesson asks for.
    /// </summary>
    public sealed record WorkflowResult(
        bool Success,
        string Message,
        string CompensationAction = null)
    {
        public WorkflowOutcome Outcome { get; init; } = WorkflowOutcome.Created;
        public int? EscalationId { get; init; }
        public string CorrelationId { get; init; }
        public IReadOnlyList<FieldError> FieldErrors { get; init; } = new List<FieldError>();
        public IReadOnlyList<StepOutcome> Steps { get; init; } = new List<StepOutcome>();
        public string NextAction { get; init; }

        public bool HasCompensation => CompensationAction != null;

        /// <summary>The first failing step, for the wizard to jump to (null when there are no field errors).</summary>
        public WizardStep? FirstFailingStep => FieldErrors.Count == 0 ? (WizardStep?)null : FieldErrors.Min(f => f.Step);

        /// <summary>One line for the trace / result label — the shape the video shows in the status strip.</summary>
        public override string ToString()
        {
            string comp = CompensationAction == null ? "none" : CompensationAction;
            string esc = EscalationId.HasValue ? $"ESC-{EscalationId}" : "—";
            return $"WorkflowResult — Outcome: {Outcome} · Success: {Success} · {esc} · CompensationAction: {comp} · correlation {CorrelationId}";
        }
    }
}
