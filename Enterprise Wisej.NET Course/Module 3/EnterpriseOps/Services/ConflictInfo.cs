using System.Collections.Generic;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>The three honest paths out of a stale edit. The user picks; the audit records the choice.</summary>
    public enum ConflictResolution
    {
        /// <summary>Discard the local edits and show the current record.</summary>
        Reload,
        /// <summary>Show both versions field by field so the user can merge.</summary>
        Compare,
        /// <summary>Leave the screen as it is and decide later. Nothing is saved, nothing is lost.</summary>
        Cancel,
    }

    /// <summary>One row of the field-by-field comparison the Compare path shows.</summary>
    public sealed class FieldComparison
    {
        public string Field { get; init; }
        public string Yours { get; init; }
        public string Current { get; init; }
        public bool Differs { get; init; }

        /// <summary>Rendered as a column so the grid needs no custom painting.</summary>
        public string Verdict => Differs ? "differs" : "same";
    }

    /// <summary>
    /// Everything the conflict dialog needs to explain a rejected save: what the user was editing, what the
    /// record looks like now, which version each of them is, and the correlation id that ties the whole
    /// episode together in the audit log. Built by the service — the dialog only renders it.
    /// </summary>
    public sealed class ConflictInfo
    {
        public int WorkOrderId { get; init; }

        /// <summary>The edit the user attempted, still based on <see cref="Expected"/>.</summary>
        public WorkOrderEditModel Yours { get; init; }

        /// <summary>The record as the store holds it now, at <see cref="Found"/>.</summary>
        public WorkOrderEditModel Current { get; init; }

        public ConcurrencyToken Expected { get; init; }
        public ConcurrencyToken Found { get; init; }
        public string CorrelationId { get; init; }

        /// <summary>"correlation 8f3a21c4 — expected v7, found v8" — the footnote in the dialog.</summary>
        public string Footnote =>
            $"correlation {CorrelationId} — expected {Expected.ToDisplayText()}, found {Found.ToDisplayText()}";

        public string SavedByOther => Current?.ModifiedBy ?? "another session";

        public IReadOnlyList<FieldComparison> Compare()
        {
            return new List<FieldComparison>
            {
                Row("Title", Yours.Title, Current.Title),
                Row("Status", Yours.Status.ToString(), Current.Status.ToString()),
                Row("Assigned to", Yours.AssignedTo ?? "(unassigned)", Current.AssignedTo ?? "(unassigned)"),
                Row("Priority", Yours.Priority.ToString(), Current.Priority.ToString()),
                Row("Version", Expected.ToDisplayText(), Found.ToDisplayText()),
            };
        }

        private static FieldComparison Row(string field, string yours, string current) =>
            new FieldComparison { Field = field, Yours = yours, Current = current, Differs = yours != current };
    }
}
