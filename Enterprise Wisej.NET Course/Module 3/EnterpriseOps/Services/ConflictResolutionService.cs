using System;
using System.Collections.Generic;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The decisions behind the conflict dialog, kept out of the dialog. The dialog renders what this returns
    /// and reports what the user picked; it decides nothing. That is what lets the same three paths be reviewed
    /// without opening the designer — and reused by an import or a batch save that hits the same conflict.
    /// </summary>
    public sealed class ConflictResolutionService
    {
        private readonly AuditTrail _audit;
        private readonly ActivityTrace _trace;

        public ConflictResolutionService(AuditTrail audit, ActivityTrace trace)
        {
            _audit = audit;
            _trace = trace;
        }

        /// <summary>The two rows the dialog shows at the top: "YOUR EDIT — stale" against "CURRENT".</summary>
        public IReadOnlyList<ConflictSummaryRow> Summarize(ConflictInfo conflict)
        {
            if (conflict == null) throw new ArgumentNullException(nameof(conflict));

            return new List<ConflictSummaryRow>
            {
                new ConflictSummaryRow
                {
                    Which = "YOUR EDIT",
                    Title = conflict.Yours.Title,
                    Status = conflict.Yours.Status.ToString(),
                    Version = conflict.Expected.ToDisplayText() + " (stale)",
                },
                new ConflictSummaryRow
                {
                    Which = "CURRENT — " + conflict.Found.ToDisplayText(),
                    Title = conflict.Current.Title,
                    Status = conflict.Current.Status.ToString(),
                    Version = conflict.Found.ToDisplayText() + " · " + conflict.SavedByOther,
                },
            };
        }

        /// <summary>The field-by-field comparison behind the Compare path.</summary>
        public IReadOnlyList<FieldComparison> Compare(ConflictInfo conflict)
        {
            if (conflict == null) throw new ArgumentNullException(nameof(conflict));

            IReadOnlyList<FieldComparison> rows = conflict.Compare();
            int differing = 0;
            foreach (FieldComparison row in rows)
                if (row.Differs) differing++;

            _trace.Service($"ConflictResolutionService.Compare(#{conflict.WorkOrderId}) → {rows.Count} fields, {differing} differ");
            return rows;
        }

        /// <summary>
        /// Records what the user chose. The lesson is explicit about this: the choice goes into the audit
        /// trail with the correlation id, so "who overwrote my edit" has an answer months later.
        /// </summary>
        public void RecordChoice(CommandContext context, ConflictInfo conflict, ConflictResolution choice)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (conflict == null) throw new ArgumentNullException(nameof(conflict));

            string detail = $"#{conflict.WorkOrderId} expected {conflict.Expected.ToDisplayText()}, " +
                            $"found {conflict.Found.ToDisplayText()} → {choice}";

            _audit.Record(context, "work-order.conflict." + choice.ToString().ToLowerInvariant(), detail);
            _trace.Audit($"conflict resolution recorded — {detail} (correlation {context.CorrelationId})");
        }

        /// <summary>What each path means, in the words the dialog shows under the buttons.</summary>
        public static string Explain(ConflictResolution choice) => choice switch
        {
            ConflictResolution.Reload => "Reload discards the local edits and shows the current record.",
            ConflictResolution.Compare => "Compare shows both versions field by field so the edit can be merged.",
            _ => "Cancel leaves the screen as it is — nothing saved, nothing lost, decide later.",
        };
    }

    /// <summary>One of the two rows at the top of the conflict dialog. A projection built for that grid alone.</summary>
    public sealed class ConflictSummaryRow
    {
        public string Which { get; init; }
        public string Title { get; init; }
        public string Status { get; init; }
        public string Version { get; init; }
    }
}
