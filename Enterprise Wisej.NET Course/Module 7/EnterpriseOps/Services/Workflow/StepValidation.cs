using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Services.Workflow
{
    /// <summary>
    /// The typed answer to "may the user leave this step?". The wizard shows the errors next to the
    /// fields and blocks Next; it never decides the rule itself.
    /// </summary>
    public sealed class StepValidation
    {
        public static readonly StepValidation Valid = new StepValidation(new List<FieldError>());

        public StepValidation(IReadOnlyList<FieldError> errors)
        {
            Errors = errors;
        }

        public IReadOnlyList<FieldError> Errors { get; }

        public bool IsValid => Errors.Count == 0;

        public string Summary => string.Join(" · ", Errors.Select(e => $"{e.Field}: {e.Message}"));
    }
}
