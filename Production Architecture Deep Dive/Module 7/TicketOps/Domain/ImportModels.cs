using System.Collections.Generic;

namespace TicketOps.Domain
{
    /// <summary>
    /// A CSV file the import service has opened and validated: the header is known, the data lines are in
    /// memory, so the total is known before the first row is written (the progress bar needs it).
    /// </summary>
    public sealed class ImportFile
    {
        public ImportFile(string path, string fileName, IReadOnlyDictionary<string, int> columns, IReadOnlyList<string> rows)
        {
            Path = path;
            FileName = fileName;
            Columns = columns;
            Rows = rows;
        }

        public string Path { get; }
        public string FileName { get; }
        public IReadOnlyDictionary<string, int> Columns { get; }

        /// <summary>The data lines (header excluded). Row numbers are 1-based indexes into this list.</summary>
        public IReadOnlyList<string> Rows { get; }

        public int TotalRows => Rows.Count;

        public override string ToString() => $"{FileName} · {TotalRows} rows";
    }

    /// <summary>How an import run ended. Every outcome leaves the repository consistent (no half-written row).</summary>
    public enum ImportOutcome
    {
        /// <summary>Every row was processed (imported or skipped with a reason).</summary>
        Completed,

        /// <summary>The token was signalled; the run stopped between two rows. NextRow says where to resume.</summary>
        Cancelled,

        /// <summary>The data store failed mid-run; the run stopped cleanly before the failing row. NextRow says where to resume.</summary>
        Faulted
    }

    public enum ImportProgressKind
    {
        Started,
        RowImported,
        RowSkipped
    }

    /// <summary>
    /// One progress report from the import service to whoever is watching (the screen). Immutable: it is
    /// created on the worker thread and read on the session context, so nothing in it may change after
    /// construction.
    /// </summary>
    public sealed class ImportProgress
    {
        public ImportProgress(ImportProgressKind kind, int rowNumber, int totalRows, int imported, int skipped, bool isMilestone, string message)
        {
            Kind = kind;
            RowNumber = rowNumber;
            TotalRows = totalRows;
            Imported = imported;
            Skipped = skipped;
            IsMilestone = isMilestone;
            Message = message;
        }

        public ImportProgressKind Kind { get; }

        /// <summary>The row just processed (1-based). For <see cref="ImportProgressKind.Started"/>: the rows already done before this run (0 for a fresh run).</summary>
        public int RowNumber { get; }
        public int TotalRows { get; }
        public int Imported { get; }
        public int Skipped { get; }

        /// <summary>True every 10 % — the service marks the rows worth a log line so the screen does not have to count.</summary>
        public bool IsMilestone { get; }

        /// <summary>A safe one-line description (e.g. the reason a row was skipped).</summary>
        public string Message { get; }

        public int Percent => TotalRows == 0 ? 100 : RowNumber * 100 / TotalRows;
    }

    /// <summary>A row the import skipped and why. Safe to show: the reason names the rule, never an internal.</summary>
    public sealed class ImportRowError
    {
        public ImportRowError(int rowNumber, string reason)
        {
            RowNumber = rowNumber;
            Reason = reason;
        }

        public int RowNumber { get; }
        public string Reason { get; }

        public override string ToString() => $"Row {RowNumber}: {Reason}";
    }

    /// <summary>
    /// The final state of one import run, returned by the service. It is complete for every outcome — the
    /// counts, the per-row errors and the row to resume from — so the screen can report cancellation and
    /// data-store failure as calmly as success.
    /// </summary>
    public sealed class ImportResult
    {
        public ImportResult(ImportOutcome outcome, string fileName, int firstRow, int lastRow, int totalRows,
            int imported, int skipped, IReadOnlyList<ImportRowError> rowErrors, int nextRow, int ticketsInStore, string message)
        {
            Outcome = outcome;
            FileName = fileName;
            FirstRow = firstRow;
            LastRow = lastRow;
            TotalRows = totalRows;
            Imported = imported;
            Skipped = skipped;
            RowErrors = rowErrors ?? new ImportRowError[0];
            NextRow = nextRow;
            TicketsInStore = ticketsInStore;
            Message = message;
        }

        public ImportOutcome Outcome { get; }
        public string FileName { get; }

        /// <summary>The first row this run processed (1 for a fresh run, higher for a resume).</summary>
        public int FirstRow { get; }

        /// <summary>The last row this run processed (imported or skipped).</summary>
        public int LastRow { get; }
        public int TotalRows { get; }
        public int Imported { get; }
        public int Skipped { get; }
        public IReadOnlyList<ImportRowError> RowErrors { get; }

        /// <summary>Where a resume starts: TotalRows + 1 when complete, otherwise the first row not yet written.</summary>
        public int NextRow { get; }

        /// <summary>How many tickets the repository holds after the run (the screen shows it without another call).</summary>
        public int TicketsInStore { get; }

        /// <summary>A safe one-line summary the screen may show verbatim.</summary>
        public string Message { get; }

        public bool IsComplete => Outcome == ImportOutcome.Completed;

        public override string ToString() => $"{Outcome} · {Message}";
    }
}
