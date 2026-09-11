using System;
using System.Diagnostics;
using System.Globalization;

namespace TicketOpsLive.Services
{
    /// <summary>How an import job ended. <see cref="Running"/> until the job finishes.</summary>
    public enum ImportOutcome { Running, Completed, Cancelled, Failed }

    /// <summary>
    /// One background import: a JobId for the log and the server console, the records imported,
    /// the elapsed time and the final summary line.
    /// </summary>
    public sealed class ImportJob
    {
        private readonly Stopwatch _watch = Stopwatch.StartNew();

        public ImportJob(int totalRecords)
        {
            JobId = "J-" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();
            TotalRecords = totalRecords;
        }

        public string JobId { get; }

        public int TotalRecords { get; }

        /// <summary>The last record that was imported completely; the failing record is never counted.</summary>
        public int RecordsImported { get; set; }

        public ImportOutcome Outcome { get; private set; } = ImportOutcome.Running;

        public string ElapsedText => _watch.Elapsed.TotalSeconds.ToString("0.00", CultureInfo.InvariantCulture) + " s";

        public void Complete() => Finish(ImportOutcome.Completed);

        public void Cancel() => Finish(ImportOutcome.Cancelled);

        public void Fail() => Finish(ImportOutcome.Failed);

        private void Finish(ImportOutcome outcome)
        {
            if (Outcome != ImportOutcome.Running)
                return;
            _watch.Stop();
            Outcome = outcome;
        }

        /// <summary>"Job J-3F9A2C: 200 records in 5.31 s — completed".</summary>
        public string Summary => $"Job {JobId}: {RecordsImported} records in {ElapsedText} — {OutcomeText}";

        public string OutcomeText => Outcome switch
        {
            ImportOutcome.Completed => "completed",
            ImportOutcome.Cancelled => "cancelled",
            ImportOutcome.Failed => "failed",
            _ => "running",
        };
    }
}
