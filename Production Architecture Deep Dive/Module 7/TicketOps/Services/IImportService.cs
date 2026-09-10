using System;
using System.Threading;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The CSV import as a service: parsing, rules, and the per-row writes live here, so the screen only
    /// starts the task and shows what comes back. No Wisej.NET type appears in this contract — the same
    /// service could run from a scheduled job or a unit test.
    /// </summary>
    public interface IImportService
    {
        /// <summary>
        /// Opens and validates a CSV file without importing anything. A missing file or a header without the
        /// required columns is an expected outcome and comes back as a failed result, never as an exception.
        /// </summary>
        Task<OperationResult<ImportFile>> OpenAsync(string path);

        /// <summary>
        /// Imports the rows of <paramref name="file"/> from <paramref name="startRow"/> (1-based) to the end.
        /// Reports every row through <paramref name="onProgress"/> — on the calling (worker) thread, so the
        /// callback must marshal to the UI itself. Checks <paramref name="token"/> before each row.
        ///
        /// A row that breaks a rule is skipped and reported; the run continues. A data-store failure stops
        /// the run cleanly before the failing row; the result says where to resume. Neither throws.
        /// </summary>
        Task<ImportResult> ImportAsync(ImportFile file, int startRow, Action<ImportProgress> onProgress, CancellationToken token);
    }
}
