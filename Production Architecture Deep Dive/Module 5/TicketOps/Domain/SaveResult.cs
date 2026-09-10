using TicketOps.Validation;

namespace TicketOps.Domain
{
    public enum SaveOutcome
    {
        /// <summary>The transaction committed; the screen may say "Saved".</summary>
        Saved,
        /// <summary>The validator or a rule said no: field errors and/or summary errors, nothing persisted.</summary>
        Invalid
    }

    /// <summary>
    /// What the save pipeline hands back to a screen. Expected outcomes (invalid input, a rule saying no)
    /// are results, never exceptions; an unexpected failure (repository outage) is an exception the
    /// handler catches. <see cref="Message"/> is shown verbatim, so it never contains internals.
    /// </summary>
    public sealed class SaveResult
    {
        public SaveOutcome Outcome { get; }
        public bool Succeeded => Outcome == SaveOutcome.Saved;
        public string Message { get; }
        public WorkOrder WorkOrder { get; }
        public ValidationResult Errors { get; }
        /// <summary>Which pipeline step rejected the command ("validate" or "rules"); for the trace only.</summary>
        public string Stage { get; }

        private SaveResult(SaveOutcome outcome, string message, WorkOrder workOrder, ValidationResult errors, string stage)
        {
            Outcome = outcome;
            Message = message;
            WorkOrder = workOrder;
            Errors = errors ?? new ValidationResult();
            Stage = stage;
        }

        public static SaveResult Ok(WorkOrder saved, string message)
            => new SaveResult(SaveOutcome.Saved, message, saved, null, "confirm");

        public static SaveResult Invalid(string stage, ValidationResult errors)
            => new SaveResult(SaveOutcome.Invalid, errors.Count == 1 ? "1 problem found — nothing was saved." : $"{errors.Count} problems found — nothing was saved.", null, errors, stage);

        public override string ToString() => Succeeded ? $"OK · {Message}" : $"INVALID ({Stage}) · {Errors}";
    }
}
