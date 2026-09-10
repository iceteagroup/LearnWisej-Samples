using System.Collections.Generic;
using Wisej.Web;

namespace OperationsConsole.Models
{
    /// <summary>One field error: which control is wrong and what the user has to fix.</summary>
    public sealed class ValidationError
    {
        public ValidationError(string controlName, string message)
        {
            ControlName = controlName;
            Message = message;
        }

        /// <summary>The <c>Name</c> of the offending control ("txtEmail") — what the Event log prints.</summary>
        public string ControlName { get; }

        /// <summary>The actionable message the ErrorProvider shows on that control.</summary>
        public string Message { get; }

        public override string ToString() => ControlName + ": " + Message;
    }

    /// <summary>
    /// What <c>CustomerEditor.ValidateContent()</c> returns. The hosting page asks "is this valid?" and
    /// "what is wrong?" — it never learns how a single rule is implemented. The result also remembers the
    /// first invalid control so the editor can put the caret back where the user has work to do
    /// (step 5 of the six-step flow: keep the user on the screen, focus the first invalid control).
    /// </summary>
    public sealed class ValidationResult
    {
        private readonly List<ValidationError> _errors = new List<ValidationError>();

        /// <summary>No field error was found.</summary>
        public bool IsValid => _errors.Count == 0;

        /// <summary>The (ControlName, Message) pairs, in the order the validators ran.</summary>
        public IReadOnlyList<ValidationError> Errors => _errors;

        /// <summary>The control the editor focuses when validation fails.</summary>
        public Control FirstInvalid { get; private set; }

        /// <summary>"3 fields need attention" / "everything is valid" — used in the status area and the Toast.</summary>
        public string Summary =>
            IsValid
                ? "all six fields are valid"
                : _errors.Count + (_errors.Count == 1 ? " field needs attention" : " fields need attention");

        public void Add(Control control, string message)
        {
            _errors.Add(new ValidationError(control?.Name ?? "(unknown)", message));
            FirstInvalid ??= control;
        }

        /// <summary>Called by <c>ValidateContent()</c> after every validator has run.</summary>
        public void FocusFirstInvalid() => FirstInvalid?.Focus();
    }
}
