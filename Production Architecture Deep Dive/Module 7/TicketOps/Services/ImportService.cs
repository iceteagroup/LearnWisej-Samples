using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// The import logic, with no UI in it. It is called from the screen's background task, so everything
    /// here runs on a worker thread; the only thing it shares with the request thread is the repository,
    /// which guards itself. All the run's own state (counters, the error list) is local to
    /// <see cref="ImportAsync"/> — the best lock is the one you never need.
    ///
    /// Three kinds of outcome, kept apart on purpose:
    /// <list type="bullet">
    ///   <item>a row breaks a rule → skipped and reported, the run continues (per-row error);</item>
    ///   <item>the token is signalled → the run stops between two rows (cancelled), state stays consistent;</item>
    ///   <item>the repository throws → the run stops before the failing row (faulted), logs the detail, reports a resume point.</item>
    /// </list>
    /// </summary>
    public sealed class ImportService : IImportService
    {
        /// <summary>The simulated cost of parsing, validating and writing one row (a real driver call would be awaited here).</summary>
        public const int RowWorkMilliseconds = 10;

        private readonly ITicketRepository _repository;
        private readonly ILog _log;

        public ImportService(ITicketRepository repository, ILog log)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task<OperationResult<ImportFile>> OpenAsync(string path)
        {
            string fileName = Path.GetFileName(path ?? "");
            _log.Info(LogLayer.Service, "ImportService.OpenAsync", $"validate {fileName}");

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                _log.Warn(LogLayer.Service, "ImportService.OpenAsync", $"rejected: file not found ({path})");
                return Task.FromResult(OperationResult<ImportFile>.Fail("The import file was not found."));
            }

            string[] lines = File.ReadAllLines(path);
            if (lines.Length == 0)
            {
                _log.Warn(LogLayer.Service, "ImportService.OpenAsync", "rejected: empty file");
                return Task.FromResult(OperationResult<ImportFile>.Fail("The import file is empty."));
            }

            var columns = TicketImportRules.ParseHeader(lines[0]);
            var missing = TicketImportRules.MissingColumns(columns);
            if (missing.Count > 0)
            {
                // Expected outcome: the header is wrong. The user reads which columns are missing; nothing is thrown.
                _log.Warn(LogLayer.Service, "ImportService.OpenAsync", $"rejected: header \"{lines[0]}\" lacks {string.Join(", ", missing)}");
                return Task.FromResult(OperationResult<ImportFile>.Fail(
                    $"The file is missing required columns: {string.Join(", ", missing)}.", missing.ToArray()));
            }

            var rows = lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
            var file = new ImportFile(path, fileName, columns, rows);
            _log.Info(LogLayer.Service, "ImportService.OpenAsync", $"valid: {file} · columns {string.Join(",", TicketImportRules.RequiredColumns)}");
            return Task.FromResult(OperationResult<ImportFile>.Ok(file, $"{fileName} · {rows.Count} rows"));
        }

        public async Task<ImportResult> ImportAsync(ImportFile file, int startRow, Action<ImportProgress> onProgress, CancellationToken token)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (onProgress == null) throw new ArgumentNullException(nameof(onProgress));

            int total = file.TotalRows;
            int first = Math.Max(1, startRow);
            int milestoneEvery = Math.Max(1, total / 10);

            // Everything the run owns stays local: no field, no static, nothing another thread can see.
            int imported = 0;
            int skipped = 0;
            int lastRow = first - 1;
            var rowErrors = new List<ImportRowError>();

            _log.Info(LogLayer.Service, "ImportService.ImportAsync",
                $"start {file.FileName} rows {first}–{total} on thread {Thread.CurrentThread.ManagedThreadId} · ~{RowWorkMilliseconds} ms/row · token checked before every row");
            // RowNumber = rows already done, so a resume keeps the bar where the previous run left it.
            onProgress(new ImportProgress(ImportProgressKind.Started, first - 1, total, 0, 0, false,
                first == 1 ? $"Importing {file.FileName} ({total} rows)…" : $"Resuming {file.FileName} at row {first} of {total}…"));

            for (int row = first; row <= total; row++)
            {
                // Cooperative cancellation: checked between rows, never in the middle of one.
                if (token.IsCancellationRequested)
                {
                    _log.Warn(LogLayer.Service, "ImportService.ImportAsync", $"cancelled before row {row} — {imported} imported, {skipped} skipped, resume at {row}");
                    return await FinishAsync(ImportOutcome.Cancelled, file, first, lastRow, imported, skipped, rowErrors, row,
                        $"Import cancelled by user — {imported} of {total - first + 1} rows imported, resume at row {row}.");
                }

                Thread.Sleep(RowWorkMilliseconds);          // the simulated per-row cost

                // Rule violation → per-row error. The row is reported and the run keeps going.
                if (!TicketImportRules.TryParseRow(file.Rows[row - 1], file.Columns, out Ticket ticket, out string reason))
                {
                    skipped++;
                    lastRow = row;
                    rowErrors.Add(new ImportRowError(row, reason));
                    _log.Warn(LogLayer.Service, "ImportService.ImportAsync", $"row {row} skipped — {reason}");
                    onProgress(new ImportProgress(ImportProgressKind.RowSkipped, row, total, imported, skipped, false, $"Row {row} skipped — {reason}"));
                    continue;
                }

                bool written;
                try
                {
                    written = await _repository.InsertAsync(ticket);
                }
                catch (Exception ex)
                {
                    // Not a row problem: the store itself failed. Stop cleanly BEFORE this row is counted, keep the
                    // detail in the log, and tell the caller where a resume starts. The user never sees ex.Message.
                    _log.Error(LogLayer.Service, "ImportService.ImportAsync", ex,
                        $"stopped at row {row}: repository unavailable — {imported} imported, {skipped} skipped, resume at {row}");
                    return await FinishAsync(ImportOutcome.Faulted, file, first, lastRow, imported, skipped, rowErrors, row,
                        $"Import stopped at row {row} — the data store is unavailable. {imported} rows were imported; resume from row {row} after recovery.");
                }

                lastRow = row;
                if (written)
                {
                    imported++;
                    bool milestone = row % milestoneEvery == 0 || row == total;
                    onProgress(new ImportProgress(ImportProgressKind.RowImported, row, total, imported, skipped, milestone,
                        milestone ? $"Row {row} imported — {row * 100 / total}%" : null));
                    if (milestone)
                        _log.Info(LogLayer.Service, "ImportService.ImportAsync", $"row {row}/{total} — {row * 100 / total}% · {imported} imported · {skipped} skipped");
                }
                else
                {
                    skipped++;
                    string duplicate = $"duplicate ticket id #{ticket.Id}";
                    rowErrors.Add(new ImportRowError(row, duplicate));
                    _log.Warn(LogLayer.Service, "ImportService.ImportAsync", $"row {row} skipped — {duplicate}");
                    onProgress(new ImportProgress(ImportProgressKind.RowSkipped, row, total, imported, skipped, false, $"Row {row} skipped — {duplicate}"));
                }
            }

            _log.Info(LogLayer.Service, "ImportService.ImportAsync", $"complete — {imported} imported · {skipped} skipped ({rowErrors.Count} row errors)");
            return await FinishAsync(ImportOutcome.Completed, file, first, lastRow, imported, skipped, rowErrors, total + 1,
                $"Import complete — {imported} imported · {skipped} skipped.");
        }

        private async Task<ImportResult> FinishAsync(ImportOutcome outcome, ImportFile file, int first, int lastRow,
            int imported, int skipped, List<ImportRowError> rowErrors, int nextRow, string message)
        {
            // Faulted means the store is down: do not ask it again (the count is unknown, the result is still complete).
            int inStore = -1;
            if (outcome != ImportOutcome.Faulted)
            {
                try
                {
                    inStore = await _repository.CountAsync();
                }
                catch (Exception ex)
                {
                    _log.Warn(LogLayer.Service, "ImportService.FinishAsync", $"count unavailable after the run ({ex.GetType().Name}) — reported as unknown");
                }
            }

            return new ImportResult(outcome, file.FileName, first, lastRow, file.TotalRows, imported, skipped,
                rowErrors.ToArray(), nextRow, inStore, message);
        }
    }
}
