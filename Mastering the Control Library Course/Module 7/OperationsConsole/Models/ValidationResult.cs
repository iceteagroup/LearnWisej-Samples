using System.Collections.Generic;
using Wisej.Web;

namespace OperationsConsole.Models
{
    /// <summary>One field error: which control is wrong and what the user has to fix.</summary>
    public sealed class ValidationError
    {
        public ValidationError(Control control, string message)
        {
            Control = control;
            Message = message;
        }

        /// <summary>The offending control.</summary>
        public Control Control { get; }

        /// <summary>The actionable message the ErrorProvider shows on that control.</summary>
        public string Message { get; }
    }

    /// <summary>
    /// What <c>CustomerEditor.ValidateContent()</c> returns: whether the form is valid, the invalid controls with
    /// their messages, and the first invalid control so the editor can put the caret back where the work is.
    /// </summary>
    public sealed class ValidationResult
    {
        private readonly List<ValidationError> _errors = new List<ValidationError>();

        /// <summary>No field error was found.</summary>
        public bool IsValid => _errors.Count == 0;

        /// <summary>The errors, in the order the validators ran.</summary>
        public IReadOnlyList<ValidationError> Errors => _errors;

        /// <summary>The control the editor focuses when validation fails.</summary>
        public Control FirstInvalid { get; private set; }

        /// <summary>"3 fields need attention" / "all six fields are valid" — used in the status area and the feedback.</summary>
        public string Summary =>
            IsValid
                ? "all six fields are valid"
                : _errors.Count + (_errors.Count == 1 ? " field needs attention" : " fields need attention");

        public void Add(Control control, string message)
        {
            _errors.Add(new ValidationError(control, message));
            FirstInvalid ??= control;
        }

        public void FocusFirstInvalid() => FirstInvalid?.Focus();
    }
}
