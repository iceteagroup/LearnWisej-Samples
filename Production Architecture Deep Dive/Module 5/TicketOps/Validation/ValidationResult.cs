using System.Collections.Generic;
using System.Linq;

namespace TicketOps.Validation
{
    /// <summary>
    /// One problem. <see cref="Field"/> names the command property it belongs to (the screen maps it to
    /// a control); a null Field is a summary error — a rule that spans fields or describes the workflow.
    /// </summary>
    public sealed class ValidationError
    {
        public string Field { get; }
        public string Message { get; }

        public ValidationError(string field, string message)
        {
            Field = field;
            Message = message;
        }

        public bool IsFieldError => Field != null;

        public override string ToString() => IsFieldError ? $"{Field}: {Message}" : Message;
    }

    /// <summary>
    /// The outcome of running the rules against a command. Errors are COLLECTED, not thrown one at a time,
    /// so the user sees every problem at once. Two channels feed two display mechanisms: field errors go
    /// to the ErrorProvider glyphs, summary errors to the panel above the buttons.
    /// Plain C# — no UI type, so a test can assert on it directly.
    /// </summary>
    public sealed class ValidationResult
    {
        private readonly List<ValidationError> _errors = new List<ValidationError>();

        public IReadOnlyList<ValidationError> Errors => _errors;
        public IEnumerable<ValidationError> FieldErrors => _errors.Where(e => e.IsFieldError);
        public IEnumerable<string> SummaryErrors => _errors.Where(e => !e.IsFieldError).Select(e => e.Message);

        public bool IsValid => _errors.Count == 0;
        public int Count => _errors.Count;

        public void AddFieldError(string field, string message) => _errors.Add(new ValidationError(field, message));
        public void AddSummaryError(string message) => _errors.Add(new ValidationError(null, message));

        /// <summary>Appends another result's errors (the service merges validator + rules before answering).</summary>
        public ValidationResult Merge(ValidationResult other)
        {
            if (other != null)
                _errors.AddRange(other._errors);
            return this;
        }

        public bool HasFieldError(string field) => _errors.Any(e => e.Field == field);

        /// <summary>Short form for the trace: "3 errors (Title, DueDate, EstimatedCost)".</summary>
        public override string ToString()
        {
            if (IsValid)
                return "valid";
            var fields = FieldErrors.Select(e => e.Field).ToList();
            int summary = _errors.Count - fields.Count;
            var parts = new List<string>();
            if (fields.Count > 0) parts.Add(string.Join(", ", fields));
            if (summary > 0) parts.Add($"{summary} summary");
            return $"{Count} error{(Count == 1 ? "" : "s")} ({string.Join("; ", parts)})";
        }
    }
}
