using System;
using System.Diagnostics;
using System.Globalization;

namespace TicketOpsLive.Services
{
    /// <summary>How an import job ended. <see cref="Running"/> until the finally block records the result.</summary>
    public enum ImportOutcome { Running, Completed, Cancelled, Failed }

    /// <summary>
    /// Bookkeeping for one background import (the lab's extension challenge: a JobId on every import).
    /// The page keeps the running job in an instance field and the finished ones in a per-session list; the
    /// JobId appears in every log line, every trace line and every server-console entry so a support engineer
    /// can connect what the user saw with what the server logged.
    /// </summary>
    public sealed class ImportJob
    {
        private readonly Stopwatch _watch = Stopwatch.StartNew();

        public ImportJob(int totalRecords, string kind)
        {
            JobId = NewId();
            TotalRecords = totalRecords;
            Kind = kind;
            StartedAt = DateTime.Now;
        }

        /// <summary>Short, human-readable id: "J-3F9A2C". Unique enough for a log; not a database key.</summary>
        public string JobId { get; }

        /// <summary>"background" (Application.StartTask) or "blocking" (the anti-pattern that runs inside the click).</summary>
        public string Kind { get; }

        public int TotalRecords { get; }

        public DateTime StartedAt { get; }

        public DateTime? FinishedAt { get; private set; }

        /// <summary>The last record that was imported completely; the failing record is never counted.</summary>
        public int RecordsImported { get; set; }

        /// <summary>How many Application.Update calls this job made (throttled: every 10 records + the final one).</summary>
        public int Pushes { get; set; }

        public ImportOutcome Outcome { get; private set; } = ImportOutcome.Running;

        /// <summary>The exception message of a failed job — for the server log, never for the UI.</summary>
        public string FailureDetail { get; private set; }

        public bool IsRunning => Outcome == ImportOutcome.Running;

        /// <summary>Wall time of the job: still counting while it runs, frozen when it finishes.</summary>
        public TimeSpan Elapsed => _watch.Elapsed;

        public string ElapsedText => Elapsed.TotalSeconds.ToString("0.00", CultureInfo.InvariantCulture) + " s";

        public void Complete() => Finish(ImportOutcome.Completed, null);

        public void Cancel() => Finish(ImportOutcome.Cancelled, null);

        public void Fail(Exception ex) => Finish(ImportOutcome.Failed, ex?.ToString());

        private void Finish(ImportOutcome outcome, string detail)
        {
            if (!IsRunning)
                return;                         // the first outcome wins; finally must not overwrite it
            _watch.Stop();
            FinishedAt = DateTime.Now;
            Outcome = outcome;
            FailureDetail = detail;
        }

        /// <summary>The final summary line the lab asks for: "Job J-3F9A2C: 200 records in 5.31 s — completed".</summary>
        public string Summary =>
            $"Job {JobId}: {RecordsImported} records in {ElapsedText} — {OutcomeText}";

        public string OutcomeText => Outcome switch
        {
            ImportOutcome.Completed => "completed",
            ImportOutcome.Cancelled => "cancelled",
            ImportOutcome.Failed => "failed",
            _ => "running",
        };

        /// <summary>Compact form for the SERVER STATE label: "J-3F9A2C completed 200 in 5.31 s".</summary>
        public string Short => $"{JobId} {OutcomeText} {RecordsImported} in {ElapsedText}";

        private static string NewId() =>
            "J-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();
    }
}
