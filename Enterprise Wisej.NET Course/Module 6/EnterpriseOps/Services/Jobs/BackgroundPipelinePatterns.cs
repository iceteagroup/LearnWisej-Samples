// BackgroundPipelinePatterns.cs — the job model from the Module 6 lesson resource, as the video types it,
// plus the lab's real ImportWorkOrdersJob: batches, validation, retryable rows, per-row errors and
// cancellation honoured between batches.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services.Jobs
{
    /// <summary>The job state machine. Progress is a separate number: Running at 10% and at 90% are the same status.</summary>
    public enum JobStatus
    {
        Queued, Running, Completed,
        CompletedWithErrors, Failed, Canceled
    }

    /// <summary>
    /// A milestone. The job publishes these through a sink and has no idea who is watching, or whether anyone is.
    /// <see cref="Result"/> travels only with the final milestone.
    /// </summary>
    public sealed record JobProgress(
        Guid JobId, JobStatus Status, int Percent, string Message)
    {
        public JobResultSummary Result { get; init; }
    }

    public interface IJobProgressSink
    {
        Task PublishAsync(JobProgress progress,
            CancellationToken cancellationToken);
    }

    public interface IBackgroundJob
    {
        Guid JobId { get; }
        Task RunAsync(IJobProgressSink progress,
            CancellationToken cancellationToken);
    }

    /// <summary>What the UI asks for: which file to import.</summary>
    public sealed class ImportJobDefinition
    {
        public string FileName { get; set; }
    }

    /// <summary>Reads an import file (a fake one in this sample). Terminal file errors throw <see cref="MalformedFileException"/>.</summary>
    public interface IImportFileSource
    {
        IReadOnlyList<ImportFileInfo> ListFiles();
        Task<ImportFile> OpenAsync(string fileName, CancellationToken cancellationToken);
    }

    public sealed class ImportFileInfo
    {
        public string FileName { get; set; }
        public string Description { get; set; }
        public int RowCount { get; set; }
        public int ExpectedTransientRows { get; set; }
        public int ExpectedTerminalRows { get; set; }
    }

    public sealed class ImportFile
    {
        public string FileName { get; set; }
        public IReadOnlyList<ImportRow> Rows { get; set; }
        public int BatchSize { get; set; } = 100;
        public int BatchLatencyMs { get; set; } = 600;   // simulated I/O per batch (bulk write + commit)
    }

    /// <summary>Writes one row. Throws <see cref="TransientRowException"/> (retry) or <see cref="TerminalRowException"/> (record, move on).</summary>
    public interface IWorkOrderImportWriter
    {
        /// <returns>true when a new work order was created, false when an existing one was updated (idempotent re-import).</returns>
        bool Upsert(string tenantId, ImportRow row);
    }

    /// <summary>
    /// The lab's job. Same shape as the video: publish "started", loop the batches with
    /// <c>ThrowIfCancellationRequested</c> at the top, publish a milestone per batch, publish the final state.
    /// What the lab adds: a validation milestone, per-row processing under the <see cref="RetryPolicy"/>,
    /// a result summary with per-row errors, and cancellation that finishes the current batch first.
    /// </summary>
    public sealed class ImportWorkOrdersJob : IBackgroundJob
    {
        private readonly ImportJobDefinition _definition;
        private readonly string _tenantId;
        private readonly IImportFileSource _files;
        private readonly IWorkOrderImportWriter _writer;
        private readonly RetryPolicy _retry;

        public ImportWorkOrdersJob(ImportJobDefinition definition, string tenantId,
            IImportFileSource files, IWorkOrderImportWriter writer, RetryPolicy retry)
        {
            _definition = definition;
            _tenantId = tenantId;
            _files = files;
            _writer = writer;
            _retry = retry;
        }

        public Guid JobId { get; } = Guid.NewGuid();

        public async Task RunAsync(IJobProgressSink progress,
            CancellationToken cancellationToken)
        {
            var summary = new JobResultSummary();

            await progress.PublishAsync(
                new(JobId, JobStatus.Running, 0, "Import started."),
                cancellationToken);

            // 1. Validate. A malformed file is TERMINAL for the whole job: fail now, retry nothing.
            ImportFile file;
            try
            {
                file = await _files.OpenAsync(_definition.FileName, cancellationToken);
            }
            catch (MalformedFileException ex)
            {
                summary.TerminalError = ex.Message;
                await progress.PublishAsync(
                    new(JobId, JobStatus.Failed, 0, $"Failed: {ex.Message} (terminal — not retried).") { Result = summary },
                    CancellationToken.None);
                return;
            }

            summary.TotalRows = file.Rows.Count;
            int batchCount = (file.Rows.Count + file.BatchSize - 1) / file.BatchSize;
            await progress.PublishAsync(
                new(JobId, JobStatus.Running, 5, $"Validated {file.Rows.Count:n0} rows · {batchCount} batches of {file.BatchSize}."),
                cancellationToken);

            // 2. Batches. Cancellation is honoured HERE, between batches — never in the middle of a row.
            try
            {
                for (int i = 1; i <= batchCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int from = (i - 1) * file.BatchSize;
                    int count = Math.Min(file.BatchSize, file.Rows.Count - from);
                    var batch = new BatchCounters();

                    for (int r = 0; r < count; r++)
                    {
                        // Rows inside a batch run to completion: CancellationToken.None on purpose
                        // (CancellationPolicy.FinishCurrentBatch).
                        var row = file.Rows[from + r];
                        await ProcessRowAsync(row, summary, batch);
                    }

                    // Simulated bulk write + commit for the batch.
                    await Task.Delay(file.BatchLatencyMs, CancellationToken.None);

                    summary.BatchesCompleted = i;
                    int percent = 5 + (int)Math.Round(90.0 * i / batchCount);
                    await progress.PublishAsync(
                        new(JobId, JobStatus.Running, percent,
                            $"Processed batch {i} of {batchCount} · {batch.Created} created, {batch.Updated} updated" +
                            (batch.Retried > 0 ? $", {batch.Retried} retried" : "") +
                            (batch.Terminal > 0 ? $", {batch.Terminal} terminal" : "") + "."),
                        cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // The flag was set by the queue. Record what was done; nothing is half-written.
                await progress.PublishAsync(
                    new(JobId, JobStatus.Canceled, 5 + (int)Math.Round(90.0 * summary.BatchesCompleted / batchCount),
                        $"Canceled after batch {summary.BatchesCompleted} of {batchCount} · {summary.Imported:n0} rows imported, none half-written.") { Result = summary },
                    CancellationToken.None);
                return;
            }

            // 3. Final state: Completed, or CompletedWithErrors with every bad row named. Rows that failed
            //    transiently and were then retried into success are NOT errors; rows that gave up after the
            //    last attempt are counted apart from the rows that were terminal from the start.
            bool hasErrors = summary.RowErrors.Count > 0;
            int terminalRows = summary.RowErrors.Count(e => !e.Retryable);
            int exhaustedRows = summary.RowErrors.Count - terminalRows;
            await progress.PublishAsync(
                new(JobId, hasErrors ? JobStatus.CompletedWithErrors : JobStatus.Completed, 100,
                    hasErrors
                        ? $"Completed with errors: {summary.Imported:n0} imported · {summary.Retried} retried ✓ · {terminalRows} terminal" +
                          (exhaustedRows > 0 ? $" · {exhaustedRows} gave up after {_retry.MaxAttempts} attempts" : "") + "."
                        : $"Import completed: {summary.Imported:n0} rows imported ({summary.Created:n0} created, {summary.Updated:n0} updated).")
                { Result = summary },
                CancellationToken.None);
        }

        /// <summary>
        /// One row under the retry policy: transient failures are retried with backoff, a terminal failure
        /// is recorded and the job moves on. Writing is idempotent (upsert by ExternalRef) so a retry after a
        /// timeout can never create a duplicate.
        /// </summary>
        private async Task ProcessRowAsync(ImportRow row, JobResultSummary summary, BatchCounters batch)
        {
            for (int attempt = 1; ; attempt++)
            {
                try
                {
                    bool created = _writer.Upsert(_tenantId, row);
                    summary.Imported++;
                    if (created) { summary.Created++; batch.Created++; } else { summary.Updated++; batch.Updated++; }
                    if (attempt > 1) { summary.Retried++; batch.Retried++; }
                    return;
                }
                catch (Exception ex) when (_retry.IsTransient(ex) && attempt < _retry.MaxAttempts)
                {
                    summary.RetryAttempts++;
                    await Task.Delay(_retry.DelayFor(attempt), CancellationToken.None);
                }
                catch (Exception ex) when (_retry.IsTransient(ex))
                {
                    summary.RowErrors.Add(new RowError(row.LineNumber, row.ExternalRef, $"transient, {_retry.MaxAttempts} attempts exhausted: {ex.Message}", retryable: true));
                    batch.Terminal++;
                    return;
                }
                catch (TerminalRowException ex)
                {
                    summary.RowErrors.Add(new RowError(row.LineNumber, row.ExternalRef, ex.Message, retryable: false));
                    batch.Terminal++;
                    return;
                }
            }
        }

        private sealed class BatchCounters
        {
            public int Created, Updated, Retried, Terminal;
        }
    }
}
