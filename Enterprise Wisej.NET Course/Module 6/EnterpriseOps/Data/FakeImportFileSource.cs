using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;
using EnterpriseOps.Services.Jobs;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The "file system" the imports read. There is no disk and no network in this sample: every file is
    /// generated deterministically, so the README can promise exactly which rows retry and which rows fail.
    ///
    /// Two files in the drop folder:
    ///   contoso_q2.csv      1,000 rows · 10 batches · 12 transient rows · 2 terminal rows   (the video's job)
    ///   contoso_pilot.csv     300 rows ·  3 batches · clean
    /// </summary>
    public sealed class FakeImportFileSource : IImportFileSource
    {
        public const string PrimaryFile = "contoso_q2.csv";
        public const string PilotFile = "contoso_pilot.csv";

        /// <summary>Lines the flaky downstream writer times out on before it succeeds (transient → retried).</summary>
        private static readonly int[] TransientLines =
            { 73, 137, 199, 244, 318, 401, 466, 512, 605, 688, 741, 902 };

        /// <summary>Lines carrying an asset code that does not exist (terminal → recorded, never retried).</summary>
        private static readonly int[] TerminalLines = { 412, 806 };

        private static readonly string[] Customers =
            { "Northwind Foods", "Contoso Retail", "Adventure Works", "Fabrikam Labs", "Tailwind Traders", "Wide World Importers" };

        private static readonly string[] Sites =
            { "Milan DC", "Turin Plant", "Genoa Depot", "Bologna Hub", "Verona Store", "Padua Workshop" };

        public IReadOnlyList<ImportFileInfo> ListFiles() => new[]
        {
            new ImportFileInfo
            {
                FileName = PrimaryFile,
                Description = "Q2 work orders — 1,000 rows, 10 batches (12 flaky rows, 2 bad asset codes)",
                RowCount = 1000, ExpectedTransientRows = TransientLines.Length, ExpectedTerminalRows = TerminalLines.Length
            },
            new ImportFileInfo
            {
                FileName = PilotFile,
                Description = "Pilot batch — 300 clean rows, 3 batches",
                RowCount = 300
            },
        };

        /// <summary>
        /// Reading the file is itself I/O, so it is awaited and cancelable. A file that cannot be parsed is a
        /// TERMINAL error for the whole job — there is nothing to retry, and the job says so instead of looping.
        /// </summary>
        public async Task<ImportFile> OpenAsync(string fileName, CancellationToken cancellationToken)
        {
            await Task.Delay(400, cancellationToken);

            switch (fileName)
            {
                case PrimaryFile:
                    return Build(fileName, "WO-EXT", 1000, batchSize: 100, batchLatencyMs: 600, withFailures: true);

                case PilotFile:
                    return Build(fileName, "WO-PIL", 300, batchSize: 100, batchLatencyMs: 600, withFailures: false);

                default:
                    throw new MalformedFileException($"'{fileName}' does not exist in the import drop folder.");
            }
        }

        private static ImportFile Build(string fileName, string refPrefix, int rowCount, int batchSize, int batchLatencyMs, bool withFailures)
        {
            var rows = new List<ImportRow>(rowCount);
            for (int line = 1; line <= rowCount; line++)
            {
                var row = new ImportRow
                {
                    LineNumber = line,
                    ExternalRef = $"{refPrefix}-{line:00000}",
                    Title = $"Preventive maintenance · unit {line:0000}",
                    Customer = Customers[line % Customers.Length],
                    Site = Sites[(line / 3) % Sites.Length],
                    AssetCode = $"AC-{1000 + (line % 900):0000}",
                    Priority = (Priority)(line % 4),
                };

                if (withFailures)
                {
                    if (Array.IndexOf(TerminalLines, line) >= 0)
                        row.AssetCode = "ZZ-0000";                                  // unknown asset → terminal
                    else if (Array.IndexOf(TransientLines, line) >= 0)
                        row.TransientFailuresBeforeSuccess = line % 2 == 0 ? 2 : 1;  // times out, then succeeds
                }

                rows.Add(row);
            }

            return new ImportFile
            {
                FileName = fileName,
                Rows = rows,
                BatchSize = batchSize,
                BatchLatencyMs = batchLatencyMs,
            };
        }
    }
}
