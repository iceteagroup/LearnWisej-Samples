using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The answer to a save. Three shapes, all of them data — none of them an exception, because a stale
    /// version is an expected outcome of an editing screen, not a bug:
    ///
    ///  • succeeded            — <see cref="Saved"/> and <see cref="NewVersion"/> are set;
    ///  • stale version        — <see cref="Conflict"/> is set and the screen opens the conflict dialog;
    ///  • validation rejected  — <see cref="CommandResult.Errors"/> explains what to fix.
    /// </summary>
    public sealed class SaveWorkOrderResult : CommandResult
    {
        /// <summary>The record as it stands after a successful save.</summary>
        public WorkOrderEditModel Saved { get; private init; }

        /// <summary>The version the record now carries (the token the screen must use for its next save).</summary>
        public ConcurrencyToken NewVersion { get; private init; }

        /// <summary>Set when another session saved first. Null on every other outcome.</summary>
        public ConflictInfo Conflict { get; private init; }

        public bool IsConflict => Conflict != null;

        public static SaveWorkOrderResult Applied(string correlationId, WorkOrderEditModel saved)
            => new SaveWorkOrderResult { Succeeded = true, CorrelationId = correlationId, Saved = saved, NewVersion = saved.Token };

        public static SaveWorkOrderResult Stale(string correlationId, ConflictInfo conflict)
        {
            var result = new SaveWorkOrderResult { Succeeded = false, CorrelationId = correlationId, Conflict = conflict };
            result.Errors.Add($"This work order changed while you were editing — expected {conflict.Expected.ToDisplayText()}, " +
                              $"found {conflict.Found.ToDisplayText()} (saved by {conflict.SavedByOther}). Nothing was overwritten.");
            return result;
        }

        public static SaveWorkOrderResult Rejected(string correlationId, params string[] errors)
        {
            var result = new SaveWorkOrderResult { Succeeded = false, CorrelationId = correlationId };
            result.Errors.AddRange(errors);
            return result;
        }
    }
}
