using System;
using System.Collections.Generic;
using System.Linq;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// The outcome of a validation: either valid, or a list of messages the dialog shows to the user.
    /// </summary>
    public class ValidationResult
    {
        public List<string> Errors { get; } = new List<string>();

        public bool IsValid => Errors.Count == 0;

        /// <summary>The messages joined for a label — one per line.</summary>
        public string Message => string.Join("\n", Errors);

        public void Add(string error)
        {
            Errors.Add(error);
        }
    }

    /// <summary>
    /// Reusable business rules for a ticket (s47 §1: "A TicketValidator or helper can be tested and reused by
    /// create/edit workflows"). The Create and Edit dialogs share it through TicketDialog.ValidateForm(), and
    /// TicketService calls it again before storing — the UI is the first line, never the only one.
    ///
    /// No controls in here: the validator sees a Ticket, not a TextBox, so it can be unit-tested.
    /// </summary>
    public class TicketValidator
    {
        public const int MaxTitleLength = 80;

        public static readonly string[] Statuses = { "Open", "In Progress", "Closed" };

        public static readonly string[] Priorities = { "Low", "Medium", "High" };

        public ValidationResult Validate(Ticket t)
        {
            var result = new ValidationResult();

            if (t == null)
            {
                result.Add("No ticket to validate.");
                return result;
            }

            string title = (t.Title ?? "").Trim();
            if (title.Length == 0)
                result.Add("Title is required.");
            else if (title.Length > MaxTitleLength)
                result.Add($"Title must be {MaxTitleLength} characters or fewer (it is {title.Length}).");

            if (string.IsNullOrWhiteSpace(t.Customer))
                result.Add("Customer is required.");

            if (!Statuses.Contains(t.Status, StringComparer.Ordinal))
                result.Add("Status must be Open, In Progress or Closed.");

            if (!Priorities.Contains(t.Priority, StringComparer.Ordinal))
                result.Add("Priority must be Low, Medium or High.");

            if (t.Status == "Closed" && string.IsNullOrWhiteSpace(t.AssignedTo))
                result.Add("A Closed ticket must have someone in Assigned to.");

            return result;
        }
    }
}
