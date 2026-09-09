using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Services.WorkQueues
{
    /// <summary>One selected row as the batch carries it: the key plus the version the user saw.</summary>
    public sealed record BatchItem(int WorkOrderId, string Number, int ExpectedVersion);

    /// <summary>
    /// The batch command: the selected keys, the target technician, the context. One user action, N business
    /// operations — the workflow processes them one at a time and reports per row.
    /// </summary>
    public sealed class ReassignBatchCommand
    {
        public IReadOnlyList<BatchItem> Items { get; }
        public string TargetTechnician { get; }
        public CommandContext Context { get; }
        public bool IsRetry { get; }

        public ReassignBatchCommand(IReadOnlyList<BatchItem> items, string targetTechnician, CommandContext context, bool isRetry = false)
        {
            Items = items;
            TargetTechnician = targetTechnician;
            Context = context;
            IsRetry = isRetry;
        }
    }

    public enum BatchRowOutcome { Succeeded, Failed, Skipped }

    /// <summary>Failure codes the UI can act on (retryable ones get the "Retry after approval" button).</summary>
    public static class BatchFailureCodes
    {
        public const string PermissionDenied = "permission-denied";
        public const string NotFound = "not-found";
        public const string Closed = "closed";
        public const string ApprovalLock = "approval-lock";
        public const string StaleVersion = "stale-version";
        public const string Certification = "certification";
        public const string NoChange = "no-change";

        public static bool IsRetryable(string code) => code == ApprovalLock || code == StaleVersion;
    }

    public sealed record BatchRowResult(int WorkOrderId, string Number, BatchRowOutcome Outcome, string Message, string Code)
    {
        public string Glyph => Outcome == BatchRowOutcome.Succeeded ? "✓" : Outcome == BatchRowOutcome.Failed ? "✕" : "–";
        public string OutcomeText => Outcome.ToString();
    }

    /// <summary>The per-row report: every row, its outcome and the reason. Partial failure is a normal result, not an exception.</summary>
    public sealed class BatchResult
    {
        public string CorrelationId { get; }
        public string TargetTechnician { get; }
        public IReadOnlyList<BatchRowResult> Rows { get; }
        public long ElapsedMs { get; }

        public BatchResult(string correlationId, string targetTechnician, IReadOnlyList<BatchRowResult> rows, long elapsedMs)
        {
            CorrelationId = correlationId;
            TargetTechnician = targetTechnician;
            Rows = rows;
            ElapsedMs = elapsedMs;
        }

        public int Succeeded => Rows.Count(r => r.Outcome == BatchRowOutcome.Succeeded);
        public int Failed => Rows.Count(r => r.Outcome == BatchRowOutcome.Failed);
        public int Skipped => Rows.Count(r => r.Outcome == BatchRowOutcome.Skipped);
        public bool IsPartialFailure => Failed > 0 && Succeeded > 0;

        public IReadOnlyList<BatchRowResult> FailedRows => Rows.Where(r => r.Outcome == BatchRowOutcome.Failed).ToList();
        public bool HasRetryableFailures => FailedRows.Any(r => BatchFailureCodes.IsRetryable(r.Code));

        public string Summary => $"{Succeeded} succeeded, {Failed} failed" + (Skipped > 0 ? $", {Skipped} skipped" : "");
    }

    /// <summary>Progress as the workflow reports it after every row.</summary>
    public sealed record BatchProgress(int Done, int Total, string CurrentNumber, BatchRowOutcome LastOutcome);
}
