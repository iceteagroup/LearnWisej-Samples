using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Services.Jobs
{
    /// <summary>
    /// The persisted job record — the contract between the executing half and the observing half.
    /// It holds an identifier, the type of work, who started it and in which tenant, its status, a progress
    /// value, a message, timestamps, a result summary with per-row errors, and a history of transitions.
    /// The store hands out copies (<see cref="Clone"/>); the page never holds the live object.
    /// </summary>
    public sealed class JobRecord
    {
        public Guid JobId { get; set; }
        public string Number { get; set; }            // "IMP-3041" — what people say out loud
        public string Type { get; set; }              // "ImportWorkOrders"
        public string Description { get; set; }       // "Work order import — fabrikam_q2.csv"
        public string Input { get; set; }             // the file name
        public string TenantId { get; set; }
        public string StartedBy { get; set; }
        public string CorrelationId { get; set; }
        public JobStatus Status { get; set; }
        public int Percent { get; set; }
        public string Message { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? StartedUtc { get; set; }
        public DateTime? FinishedUtc { get; set; }
        public JobResultSummary Result { get; set; }
        public List<JobHistoryEntry> History { get; set; } = new List<JobHistoryEntry>();

        public bool IsActive => Status == JobStatus.Queued || Status == JobStatus.Running;
        public bool IsFinished => !IsActive;

        public JobRecord Clone()
        {
            var copy = (JobRecord)MemberwiseClone();
            copy.History = History.ToList();
            copy.Result = Result?.Clone();
            return copy;
        }
    }

    /// <summary>One transition or milestone, so the job detail screen can show when it happened and what it said.</summary>
    public sealed class JobHistoryEntry
    {
        public DateTime AtUtc { get; set; }
        public string Layer { get; set; }     // "Job:", "Queue:", "Data:", "Notify:"
        public JobStatus Status { get; set; }
        public int Percent { get; set; }
        public string Message { get; set; }

        public override string ToString() => $"{AtUtc.ToLocalTime():HH:mm:ss} {Status,-20} {(Percent >= 0 ? Percent + "%" : "  ")}  {Message}";
    }

    /// <summary>The result: counts plus every bad row by line number and reference. Partial failure is reported per row.</summary>
    public sealed class JobResultSummary
    {
        public int TotalRows { get; set; }
        public int BatchesCompleted { get; set; }
        public int Imported { get; set; }
        public int Created { get; set; }
        public int Updated { get; set; }
        public int Retried { get; set; }        // rows that failed transiently and then succeeded
        public int RetryAttempts { get; set; }  // total retry attempts made
        public string TerminalError { get; set; }
        public List<RowError> RowErrors { get; set; } = new List<RowError>();

        public JobResultSummary Clone()
        {
            var copy = (JobResultSummary)MemberwiseClone();
            copy.RowErrors = RowErrors.ToList();
            return copy;
        }
    }

    public sealed class RowError
    {
        public RowError(int lineNumber, string externalRef, string message, bool retryable)
        {
            LineNumber = lineNumber;
            ExternalRef = externalRef;
            Message = message;
            Retryable = retryable;
        }

        public int LineNumber { get; }
        public string ExternalRef { get; }
        public string Message { get; }
        public bool Retryable { get; }

        public override string ToString() => $"line {LineNumber,5}  {ExternalRef,-12}  {(Retryable ? "transient" : "terminal ")}  {Message}";
    }

    /// <summary>The commands the Import Center screen sends. Typed, small, no entities.</summary>
    public sealed class StartImportCommand
    {
        public string FileName { get; set; }
        public bool PublishEveryRow { get; set; }
    }

    public sealed class CancelJobCommand
    {
        public Guid JobId { get; set; }
    }
}
