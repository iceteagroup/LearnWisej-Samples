using System;
using System.Threading;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Ticket Import (CSV): file line, Start / Cancel / Refresh list, progress bar with
    /// percent and row counts, and the import log.
    ///
    /// Threading model (docs/BackgroundTaskNotes.md has the long version):
    /// <list type="number">
    ///   <item>The click handler takes the lock, refuses a second run, creates the CancellationTokenSource,
    ///         sets the UI for this response, and calls <c>Application.StartTask</c>. It returns at once.</item>
    ///   <item>The task runs <see cref="IImportService.ImportAsync"/>. The service never sees a control; it
    ///         reports rows through a callback on the worker thread.</item>
    ///   <item>The callback copies what it needs, decides whether the change is worth a push (every
    ///         <see cref="PushEveryRows"/> rows, every skipped row, every 10 % milestone, the final state) and
    ///         applies the control changes inside <c>Application.Update(this, () => …)</c> — session context
    ///         restored, one flush. Every callback checks <c>IsDisposed</c> first.</item>
    ///   <item>Shared fields (<c>_isRunning</c>, <c>_importCancel</c>, <c>_resumeAtRow</c>) are read and written
    ///         only under <c>_sync</c>; the lock is never held while a control is touched or a push is made.</item>
    /// </list>
    /// </summary>
    public partial class ImportPage : Form
    {
        /// <summary>Progress is pushed every N rows — plus every 10 % milestone, every skipped row and the final state.</summary>
        private const int PushEveryRows = 25;
        private const int MaxImportLogLines = 400;

        private const string MarkInfo = "·";
        private const string MarkWarn = "⚠";
        private const string MarkOk = "✔";
        private const string MarkStop = "⏹";
        private const string MarkError = "✖";

        private readonly IImportService _importService;
        private readonly ITicketService _tickets;
        private readonly ImportFileLocator _files;
        private readonly ILog _log;

        // Shared between the request thread and the import task. Every read and every write takes _sync.
        private readonly object _sync = new object();
        private bool _isRunning;
        private CancellationTokenSource _importCancel;
        private int _resumeAtRow = 1;

        // Request-thread state (set in Load / handlers; read inside Application.Update callbacks, which run in context).
        private ImportFile _file;
        private int _pushers;
        private bool _polling;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public ImportPage() : this(null, null, null, new ActivityLog())
        {
        }

        public ImportPage(IImportService importService, ITicketService tickets, ImportFileLocator files, ILog log)
        {
            InitializeComponent();

            _importService = importService;
            _tickets = tickets;
            _files = files;
            _log = log;

            // Closing the tab is a cancellation too: the task sees the token before its next row, and every
            // callback checks IsDisposed before touching a control.
            this.Disposed += ImportPage_Disposed;
        }

        #region Screen lifecycle

        private async void ImportPage_Load(object sender, EventArgs e)
        {
            try
            {
                this.statusBanner.SetStatus("opening the import file", StatusKind.Busy);

                string path = _files.Resolve(SampleFiles.Tickets);
                var opened = await _importService.OpenAsync(path);
                if (!opened.Succeeded)
                {
                    ShowResult(opened);
                    this.buttonStartImport.Enabled = false;
                    return;
                }

                _file = opened.Value;
                this.labelFile.Text = $"{_file.FileName} · {_file.TotalRows} rows";
                this.labelRowsDone.Text = $"0 of {_file.TotalRows} rows";
                await RefreshCountAsync();
                this.statusBanner.SetStatus($"Ready to import {_file.FileName}.", StatusKind.Success);
            }
            catch (Exception ex)
            {
                ReportFailure("ImportPage.Load", ex);
            }
        }

        private void ImportPage_Disposed(object sender, EventArgs e)
        {
            CancellationTokenSource cts;
            lock (_sync) cts = _importCancel;
            try
            {
                cts?.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // the run finished between the read and the call
            }
        }

        #endregion

        #region Background task launcher — ▶ Start import

        /// <summary>
        /// The launcher: guard, token, UI state for THIS response, StartTask. The import itself runs in
        /// <see cref="RunImportAsync"/> on a worker thread bound to this session; progress comes back through
        /// <see cref="OnImportProgress"/>.
        /// </summary>
        private void buttonStartImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TryBeginRun(out ImportFile file, out int startRow, out CancellationToken token))
                    return;                                                  // refused under the lock: a run is already active

                ShowRunStarted(file, startRow);                              // UI state for this response (request thread, in context)
                BeginPush();                                                 // polling fallback when there is no WebSocket

                // StartTask: a worker thread that still belongs to THIS session. The handler returns immediately.
                Application.StartTask(() => RunImportAsync(file, startRow, token));
            }
            catch (Exception ex)
            {
                ReportFailure("ImportPage.buttonStartImport_Click", ex);
            }
        }

        /// <summary>
        /// The double-start guard: one lock, one check, one new token source. A second click while a run is
        /// active — even one that arrives before the disabled button reaches the browser — is refused here.
        /// </summary>
        private bool TryBeginRun(out ImportFile file, out int startRow, out CancellationToken token)
        {
            file = _file;
            startRow = 0;
            token = CancellationToken.None;

            if (file == null)
            {
                ShowResult(OperationResult<ImportFile>.Fail("Open a valid import file first."));
                return false;
            }

            bool refused;
            lock (_sync)
            {
                refused = _isRunning;
                if (!refused)
                {
                    _isRunning = true;
                    _importCancel = new CancellationTokenSource();      // a fresh source every run: a cancelled one is never reused
                    token = _importCancel.Token;
                    startRow = _resumeAtRow;
                }
            }

            if (refused)
            {
                this.statusBanner.ShowBanner("⚠ " + Strings.ImportAlreadyRunning, StatusKind.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Runs on the worker thread started by StartTask. The service does the work; this method only
        /// decides how the run ends and restores the shared state and the UI — in a finally block, so the
        /// screen is never left with a disabled Start button.
        /// </summary>
        private async Task RunImportAsync(ImportFile file, int startRow, CancellationToken token)
        {
            ImportResult result = null;
            Exception failure = null;
            try
            {
                result = await _importService.ImportAsync(file, startRow, OnImportProgress, token);
            }
            catch (Exception ex)
            {
                // Anything the service did not classify: caught INSIDE the task — it never escapes to the
                // framework — logged with its details, shown as a safe message.
                failure = ex;
                _log.Error(LogLayer.UI, "ImportPage.RunImportAsync", ex);
            }
            finally
            {
                CancellationTokenSource cts;
                lock (_sync)
                {
                    _isRunning = false;
                    cts = _importCancel;
                    _importCancel = null;
                    if (result != null)
                        _resumeAtRow = result.IsComplete ? 1 : result.NextRow;
                }
                cts?.Dispose();

                PushFinalState(result, failure);
            }
        }

        #endregion

        #region Progress UI — worker thread → session context → browser

        /// <summary>
        /// Called by the service on the worker thread once per row. Decides whether the report is worth a push,
        /// then marshals it into the session context and flushes once.
        /// </summary>
        private void OnImportProgress(ImportProgress progress)
        {
            if (!ShouldPush(progress) || this.IsDisposed)
                return;

            try
            {
                Application.Update(this, () =>
                {
                    if (this.IsDisposed)
                        return;

                    ApplyProgress(progress);
                });
            }
            catch (ObjectDisposedException)
            {
                // the page went away between the check and the push
            }
        }

        /// <summary>Bounded update rate: every N rows, every milestone (10 %), every skipped row, the last row.</summary>
        private static bool ShouldPush(ImportProgress progress)
        {
            return progress.Kind != ImportProgressKind.RowImported
                || progress.IsMilestone
                || progress.RowNumber % PushEveryRows == 0
                || progress.RowNumber == progress.TotalRows;
        }

        /// <summary>Runs inside Application.Update(this, …): the only place progress touches controls.</summary>
        private void ApplyProgress(ImportProgress progress)
        {
            this.progressImport.Value = progress.Percent;
            this.labelPercent.Text = $"{progress.Percent}%";
            this.labelRowsDone.Text = $"{progress.RowNumber} of {progress.TotalRows} rows · {progress.Imported} imported · {progress.Skipped} skipped";
            this.statusBanner.SetStatus($"Importing… {progress.RowNumber} of {progress.TotalRows} rows — UI remains responsive.", StatusKind.Busy);

            switch (progress.Kind)
            {
                case ImportProgressKind.Started:
                    AddImportLog(progress.Message, MarkInfo);
                    break;
                case ImportProgressKind.RowSkipped:
                    AddImportLog(progress.Message, MarkWarn);          // per-row error: reported, the run continues
                    break;
                case ImportProgressKind.RowImported:
                    if (progress.IsMilestone)
                        AddImportLog(progress.Message, MarkInfo);
                    break;
            }
        }

        /// <summary>The final push, from the task's finally block: buttons back, outcome shown, polling released.</summary>
        private void PushFinalState(ImportResult result, Exception failure)
        {
            if (this.IsDisposed)
                return;

            try
            {
                Application.Update(this, () =>
                {
                    if (this.IsDisposed)
                        return;

                    ShowRunFinished(result, failure);
                    EndPush();
                });
            }
            catch (ObjectDisposedException)
            {
                // the page went away between the check and the push
            }
        }

        private void ShowRunStarted(ImportFile file, int startRow)
        {
            this.buttonStartImport.Enabled = false;
            this.buttonCancelImport.Enabled = true;
            this.statusBanner.HideBanner();

            if (startRow <= 1)
            {
                this.progressImport.Value = 0;
                this.labelPercent.Text = "0%";
                this.labelRowsDone.Text = $"0 of {file.TotalRows} rows";
                this.statusBanner.SetStatus("import started", StatusKind.Busy);
            }
            else
            {
                this.labelRowsDone.Text = $"resuming at row {startRow} of {file.TotalRows}";
                this.statusBanner.SetStatus($"resuming at row {startRow}", StatusKind.Busy);
            }
        }

        private void ShowRunFinished(ImportResult result, Exception failure)
        {
            this.buttonStartImport.Enabled = true;
            this.buttonCancelImport.Enabled = false;

            if (failure != null || result == null)
            {
                this.buttonStartImport.Text = "▶ Start import";
                AddImportLog("Import failed — the details are in the log", MarkError);
                this.statusBanner.ShowBanner("✖ " + Strings.ImportFailed, StatusKind.Error);
                this.statusBanner.SetStatus("failed", StatusKind.Error);
                return;
            }

            if (result.TicketsInStore >= 0)
                this.labelTicketCount.Text = $"{result.TicketsInStore} tickets in the repository";

            switch (result.Outcome)
            {
                case ImportOutcome.Completed:
                    this.progressImport.Value = 100;
                    this.labelPercent.Text = "100%";
                    this.buttonStartImport.Text = "▶ Start import";
                    AddImportLog(result.Message, MarkOk);
                    this.statusBanner.HideBanner();
                    this.statusBanner.SetStatus($"Import complete — {result.Imported} imported · {result.Skipped} skipped.", StatusKind.Success);
                    break;

                case ImportOutcome.Cancelled:
                    this.buttonStartImport.Text = $"▶ Resume import (row {result.NextRow})";
                    AddImportLog(result.Message, MarkStop);
                    this.statusBanner.ShowBanner("⏹ " + Strings.ImportCancelled, StatusKind.Warning);
                    this.statusBanner.SetStatus($"Canceled — {result.Imported} rows kept.", StatusKind.Warning);
                    break;

                case ImportOutcome.Faulted:
                    this.buttonStartImport.Text = $"▶ Resume import (row {result.NextRow})";
                    AddImportLog($"Import stopped at row {result.NextRow} — the data store is unavailable ({result.Imported} rows imported in this run)", MarkError);
                    this.statusBanner.ShowBanner("✖ " + Strings.ImportInterrupted, StatusKind.Error);
                    this.statusBanner.SetStatus("stopped — data store unavailable", StatusKind.Error);
                    break;
            }
        }

        #endregion

        #region Cancellation — ⏹ Cancel (cooperative)

        private void buttonCancelImport_Click(object sender, EventArgs e)
        {
            try
            {
                CancellationTokenSource cts;
                lock (_sync) cts = _importCancel;
                if (cts == null)
                    return;

                cts.Cancel();

                this.buttonCancelImport.Enabled = false;
                this.statusBanner.SetStatus("cancelling — finishing the current row", StatusKind.Warning);
                AddImportLog("Cancel requested — the task stops before its next row", MarkStop);
            }
            catch (ObjectDisposedException)
            {
                // the run finished (and disposed its source) between the read and the call: nothing left to cancel
            }
            catch (Exception ex)
            {
                ReportFailure("ImportPage.buttonCancelImport_Click", ex);
            }
        }

        #endregion

        #region ↻ Refresh list — the request thread reads the store while the task writes

        private async void buttonRefreshList_Click(object sender, EventArgs e)
        {
            try
            {
                int count = await _tickets.CountAsync();
                this.labelTicketCount.Text = $"{count} tickets in the repository";
                AddImportLog($"List refreshed — {count} tickets", MarkOk);
            }
            catch (Exception ex)
            {
                ReportFailure("ImportPage.buttonRefreshList_Click", ex);
            }
        }

        private async Task RefreshCountAsync()
        {
            int count = await _tickets.CountAsync();
            this.labelTicketCount.Text = $"{count} tickets in the repository";
        }

        #endregion

        #region Delivery: WebSocket push, or the polling fallback while a task needs it

        /// <summary>
        /// Called where out-of-bound work starts. With a WebSocket the pushes flow by themselves. Without one,
        /// ask the browser to poll every second for the duration of the work — never in Load, where
        /// IsWebSocket is still false because the socket opens after the first response.
        /// </summary>
        private void BeginPush()
        {
            _pushers++;
            if (Application.IsWebSocket || _polling)
                return;

            Application.StartPolling(1000);
            _polling = true;
        }

        private void EndPush()
        {
            if (_pushers > 0)
                _pushers--;
            if (_pushers > 0 || !_polling)
                return;

            Application.EndPolling();
            _polling = false;
        }

        #endregion

        #region data → UI helpers

        private void AddImportLog(string text, string mark)
        {
            this.listImportLog.Items.Add($"{DateTime.Now:HH:mm:ss} {mark} {text}");
            while (this.listImportLog.Items.Count > MaxImportLogLines)
                this.listImportLog.Items.RemoveAt(0);
            this.listImportLog.SelectedIndex = this.listImportLog.Items.Count - 1;
        }

        private void ShowResult<T>(OperationResult<T> result)
        {
            if (result.Succeeded)
            {
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(result.Message, StatusKind.Success);
            }
            else
            {
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("rejected", StatusKind.Warning);
            }
        }

        /// <summary>Unexpected failure on the request thread: the details go to the log, the user sees one safe sentence.</summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex);
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion
    }
}
