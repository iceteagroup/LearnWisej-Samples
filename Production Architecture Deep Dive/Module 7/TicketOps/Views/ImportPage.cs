using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Ticket Import (CSV) — the Module 7 screen (the video calls it "Work Order Import").
    ///
    /// Left card:   the import screen — file line, ▶ Start import / ⏹ Cancel / ↻ Refresh list, progress bar
    ///              with percent and row counts, the import log, and a SERVER STATE line printing the fields
    ///              the two threads share.
    /// Right card:  the Diagnostics activity trace. Lines written by the worker thread ([SVC], [DATA]) are
    ///              queued and flushed with each push, so the trace itself is never touched off-context.
    /// Bottom bar:  validation failure (a file with the wrong header), the double-start guard, the error
    ///              path with recovery (simulated data outage → the run stops cleanly → resume), reset, Clear trace.
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
    ///   <item>Shared fields (<c>_isRunning</c>, <c>_importCancel</c>, <c>_resumeAtRow</c>, <c>_pushes</c>,
    ///         <c>_lastProgress</c>) are read and written only under <c>_sync</c>; the lock is never held
    ///         while a control is touched or a push is made.</item>
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
        private readonly InMemoryTicketRepository _repository;   // only for the lab's outage and reset switches
        private readonly ImportFileLocator _files;
        private readonly ILog _log;

        // ── Shared between the request thread and the import task. Every read and every write takes _sync. ──
        private readonly object _sync = new object();
        private bool _isRunning;
        private CancellationTokenSource _importCancel;
        private int _resumeAtRow = 1;
        private int _pushes;
        private ImportProgress _lastProgress;

        // ── Request-thread state (set in Load / handlers; read inside Application.Update callbacks, which run in context). ──
        private ImportFile _file;
        private int _pushers;
        private bool _polling;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public ImportPage() : this(null, null, null, null, new ActivityLog())
        {
        }

        public ImportPage(IImportService importService, ITicketService tickets, InMemoryTicketRepository repository, ImportFileLocator files, ILog log)
        {
            InitializeComponent();

            _importService = importService;
            _tickets = tickets;
            _repository = repository;
            _files = files;
            _log = log;

            if (log is ActivityLog activityLog)
                this.tracePanel.Attach(activityLog);

            // Closing the tab is a cancellation too: the task sees the token before its next row, and every
            // callback checks IsDisposed before touching a control.
            this.Disposed += ImportPage_Disposed;
        }

        #region Screen lifecycle

        private async void ImportPage_Load(object sender, EventArgs e)
        {
            try
            {
                _log.Info(LogLayer.Session, "ImportPage.Load",
                    $"first request · IsWebSocket={Low(Application.IsWebSocket)} (the socket opens after this response) — no StartPolling here; BeginPush decides when a task starts");
                this.statusBanner.SetStatus("opening the sample file", StatusKind.Busy);

                string path = _files.Resolve(SampleFiles.Tickets);
                var opened = await _importService.OpenAsync(path);
                if (!opened.Succeeded)
                {
                    ShowResult(opened);
                    this.buttonStartImport.Enabled = false;
                    return;
                }

                _file = opened.Value;
                this.labelFile.Text = $"{_file.FileName} · {_file.TotalRows} rows · {string.Join(", ", TicketImportRules.RequiredColumns)}";
                this.labelRowsDone.Text = $"0 of {_file.TotalRows} rows";
                await RefreshCountAsync();
                this.statusBanner.SetStatus("ready", StatusKind.Success);
                UpdateStateLabel();
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

        #region Background task launcher — ▶ Start import (success + progress path)

        /// <summary>
        /// The launcher. Everything the round-trip needs happens here in a few milliseconds: guard, token,
        /// UI state for THIS response, StartTask. The import itself runs in <see cref="RunImportAsync"/> on a
        /// worker thread bound to this session; progress comes back through <see cref="OnImportProgress"/>.
        /// </summary>
        private void buttonStartImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TryBeginRun("buttonStartImport_Click", out ImportFile file, out int startRow, out CancellationToken token))
                    return;                                                  // refused under the lock: a run is already active

                ShowRunStarted(file, startRow);                              // UI state for this response (request thread, in context)
                BeginPush();                                                 // polling fallback when there is no WebSocket
                _log.Info(LogLayer.UI, "ImportPage.buttonStartImport_Click",
                    $"→ Application.StartTask(IImportService.ImportAsync {file.FileName} from row {startRow}) — the round-trip closes now; progress arrives by Application.Update every {PushEveryRows} rows");
                this.tracePanel.BeginDeferred();                             // from here on, worker-thread trace lines are queued until the next push

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
        private bool TryBeginRun(string who, out ImportFile file, out int startRow, out CancellationToken token)
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
                    _pushes = 0;
                    _lastProgress = null;
                }
            }

            if (refused)
            {
                // Outside the lock: a lock is for the data, never for logging or controls.
                _log.Warn(LogLayer.UI, "ImportPage.TryBeginRun", $"{who}: refused — _isRunning is true (checked under the lock); no second task, no second token");
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
                // Anything the service did not classify (a bug, not a row and not the store): caught INSIDE the
                // task — it never escapes to the framework — logged with its details, shown as a safe message.
                failure = ex;
                _log.Error(LogLayer.UI, "ImportPage.RunImportAsync", ex, "unexpected failure inside the task — caught in the task, UI restored below");
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
        /// Called by the service on the worker thread once per row. Copies the report under the lock, decides
        /// whether it is worth a push, then marshals the copy into the session context and flushes once. The
        /// lock is released before Application.Update: a lock is for data, never around a network flush.
        /// </summary>
        private void OnImportProgress(ImportProgress progress)
        {
            bool push;
            lock (_sync)
            {
                _lastProgress = progress;
                push = ShouldPush(progress);
                if (push)
                    _pushes++;
            }

            if (!push || this.IsDisposed)
                return;

            try
            {
                Application.Update(this, () =>
                {
                    if (this.IsDisposed)
                        return;

                    ApplyProgress(progress);
                    UpdateStateLabel();
                    this.tracePanel.FlushPending();      // the [SVC]/[DATA] lines the worker wrote since the last push
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
            this.statusBanner.SetStatus($"importing {progress.RowNumber}/{progress.TotalRows} — UI stays responsive", StatusKind.Busy);

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

            int pushes;
            lock (_sync) pushes = _pushes;
            _log.Info(LogLayer.UI, "ImportPage.OnImportProgress",
                $"push #{pushes} — row {progress.RowNumber}/{progress.TotalRows} ({progress.Percent}%) → Application.Update(this, …) from thread {Thread.CurrentThread.ManagedThreadId}");
        }

        /// <summary>The final push, from the task's finally block: buttons back, outcome shown, polling released.</summary>
        private void PushFinalState(ImportResult result, Exception failure)
        {
            if (this.IsDisposed)
            {
                _log.Info(LogLayer.Session, "ImportPage.PushFinalState", "page disposed during the import — nothing to push, the task just ends");
                return;
            }

            try
            {
                Application.Update(this, () =>
                {
                    if (this.IsDisposed)
                        return;

                    ShowRunFinished(result, failure);
                    EndPush();
                    lock (_sync) _pushes++;
                    UpdateStateLabel();
                    this.tracePanel.EndDeferred();       // drains the queue and returns the trace to direct mode
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
            this.buttonResetData.Enabled = false;
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
            this.buttonResetData.Enabled = true;

            if (failure != null || result == null)
            {
                this.buttonStartImport.Text = "▶ Start import";
                AddImportLog("Import failed — the details are in the activity trace", MarkError);
                this.statusBanner.ShowBanner("✖ " + Strings.ImportFailed, StatusKind.Error);
                this.statusBanner.SetStatus("failed — UI restored in finally", StatusKind.Error);
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
                    this.statusBanner.SetStatus($"{result.Imported} imported · {result.Skipped} skipped", StatusKind.Success);
                    _log.Info(LogLayer.UI, "ImportPage.ShowRunFinished", $"OK · {result.Message} · {result.RowErrors.Count} row errors listed, none fatal (final push)");
                    break;

                case ImportOutcome.Cancelled:
                    this.buttonStartImport.Text = $"▶ Resume import (row {result.NextRow})";
                    AddImportLog(result.Message, MarkStop);
                    this.statusBanner.ShowBanner("⏹ " + Strings.ImportCancelled, StatusKind.Warning);
                    this.statusBanner.SetStatus($"cancelled before row {result.NextRow} — {result.Imported} rows kept", StatusKind.Warning);
                    _log.Warn(LogLayer.UI, "ImportPage.ShowRunFinished", $"cancelled · {result.Message} · state consistent, Start now resumes at row {result.NextRow} (final push)");
                    break;

                case ImportOutcome.Faulted:
                    this.buttonStartImport.Text = $"▶ Resume import (row {result.NextRow})";
                    AddImportLog($"Import stopped at row {result.NextRow} — the data store is unavailable ({result.Imported} rows imported in this run)", MarkError);
                    this.statusBanner.ShowBanner("✖ " + Strings.ImportInterrupted, StatusKind.Error);
                    this.statusBanner.SetStatus("stopped — data store unavailable", StatusKind.Error);
                    _log.Warn(LogLayer.UI, "ImportPage.ShowRunFinished", $"faulted · user sees Strings.ImportInterrupted; the driver message stays in the ✖ [DATA]/[SVC] lines · resume at row {result.NextRow} after recovery (final push)");
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

                _log.Info(LogLayer.UI, "ImportPage.buttonCancelImport_Click",
                    $"_importCancel.Cancel() → the task sees the token before its next row (≤ {ImportService.RowWorkMilliseconds} ms); rows already written stay");
                cts.Cancel();

                this.buttonCancelImport.Enabled = false;
                this.statusBanner.SetStatus("cancelling — finishing the current row", StatusKind.Warning);
                AddImportLog("Cancel requested — the task stops before its next row", MarkStop);
                this.tracePanel.FlushPending();
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
                bool running;
                lock (_sync) running = _isRunning;

                _log.Info(LogLayer.UI, "ImportPage.buttonRefreshList_Click",
                    $"→ ITicketService.CountAsync() on request thread {Thread.CurrentThread.ManagedThreadId}{(running ? " — while the import task is writing (the repository lock keeps both safe)" : "")}");
                int count = await _tickets.CountAsync();

                this.labelTicketCount.Text = $"{count} tickets in the repository";
                AddImportLog($"List refreshed — {count} tickets{(running ? " (request thread; the import kept running)" : "")}", MarkOk);
                UpdateStateLabel();
                this.tracePanel.FlushPending();      // this request's own lines travel back with its response
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

        #region Bottom bar: validation failure, double-start guard, outage + recovery, reset, clear

        /// <summary>Failure path (validation): the header lacks the required columns. A result, not an exception; no task starts.</summary>
        private async void buttonWrongHeader_Click(object sender, EventArgs e)
        {
            try
            {
                string path = _files.Resolve(SampleFiles.WrongHeader);
                _log.Info(LogLayer.UI, "ImportPage.buttonWrongHeader_Click", $"→ IImportService.OpenAsync({Path.GetFileName(path)}) — validation happens before any task starts");
                var opened = await _importService.OpenAsync(path);
                ShowResult(opened);                                          // expected: FAIL · missing columns — nothing written
                this.tracePanel.FlushPending();
            }
            catch (Exception ex)
            {
                ReportFailure("ImportPage.buttonWrongHeader_Click", ex);
            }
        }

        /// <summary>Synchronization: two starts in one request. The second is refused by the lock-guarded flag, not by a disabled button.</summary>
        private void buttonStartTwice_Click(object sender, EventArgs e)
        {
            try
            {
                _log.Info(LogLayer.UI, "ImportPage.buttonStartTwice_Click", "two starts in one request → expect one StartTask and one ⚠ refusal from TryBeginRun");
                buttonStartImport_Click(this.buttonStartImport, EventArgs.Empty);    // starts (or is refused if a run is already active)
                buttonStartImport_Click(this.buttonStartImport, EventArgs.Empty);    // refused under the lock
                this.tracePanel.FlushPending();                                      // show the refusal with this response
            }
            catch (Exception ex)
            {
                ReportFailure("ImportPage.buttonStartTwice_Click", ex);
            }
        }

        /// <summary>Error path + recovery: toggle the store outage. Mid-import, the task's next write fails and the run stops cleanly with a resume row.</summary>
        private void buttonOutage_Click(object sender, EventArgs e)
        {
            if (_repository == null)
                return;

            try
            {
                _repository.SimulateOutage = !_repository.SimulateOutage;
                bool outage = _repository.SimulateOutage;
                bool running;
                lock (_sync) running = _isRunning;

                this.buttonOutage.Text = outage ? "Recover the data store" : "Simulate data outage";
                _log.Info(LogLayer.UI, "ImportPage.buttonOutage_Click", outage
                    ? (running
                        ? "outage ON while the task runs → its next InsertAsync throws: expect ✖ in DATA and SVC, the run stops before that row and reports where to resume"
                        : "outage ON → the next import (or ↻ Refresh list) hits ✖ in DATA; the user sees only the safe message")
                    : "outage OFF → recovered; click ▶ Resume import to continue from the row that failed");

                this.statusBanner.SetStatus(outage ? "data store outage (simulated)" : "data store recovered", outage ? StatusKind.Error : StatusKind.Success);
                if (!outage)
                    this.statusBanner.HideBanner();
                UpdateStateLabel();
                this.tracePanel.FlushPending();
            }
            catch (Exception ex)
            {
                ReportFailure("ImportPage.buttonOutage_Click", ex);
            }
        }

        /// <summary>Re-seeds the store so a full run can be repeated. Refused while a run is active (same guard, same lock).</summary>
        private async void buttonResetData_Click(object sender, EventArgs e)
        {
            if (_repository == null)
                return;

            try
            {
                bool running;
                lock (_sync) running = _isRunning;
                if (running)
                {
                    _log.Warn(LogLayer.UI, "ImportPage.buttonResetData_Click", "refused — an import is running (checked under the lock)");
                    this.statusBanner.ShowBanner("⚠ " + Strings.ImportAlreadyRunning, StatusKind.Warning);
                    this.tracePanel.FlushPending();
                    return;
                }

                _log.Info(LogLayer.UI, "ImportPage.buttonResetData_Click", "→ ITicketRepository.ResetAsync() — re-seed the store, resume row back to 1");
                await _repository.ResetAsync();

                lock (_sync)
                {
                    _resumeAtRow = 1;
                    _lastProgress = null;
                    _pushes = 0;
                }

                this.buttonStartImport.Text = "▶ Start import";
                this.progressImport.Value = 0;
                this.labelPercent.Text = "0%";
                this.labelRowsDone.Text = $"0 of {(_file == null ? 0 : _file.TotalRows)} rows";
                this.listImportLog.Items.Clear();
                await RefreshCountAsync();
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus("ready", StatusKind.Success);
                UpdateStateLabel();
            }
            catch (Exception ex)
            {
                ReportFailure("ImportPage.buttonResetData_Click", ex);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.tracePanel.ClearTrace();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("ready", StatusKind.Normal);
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
            _log.Info(LogLayer.Session, "ImportPage.BeginPush", "no WebSocket when the task started → Application.StartPolling(1000) until the task ends");
        }

        private void EndPush()
        {
            if (_pushers > 0)
                _pushers--;
            if (_pushers > 0 || !_polling)
                return;

            Application.EndPolling();
            _polling = false;
            _log.Info(LogLayer.Session, "ImportPage.EndPush", "last task ended → Application.EndPolling()");
        }

        #endregion

        #region data → UI helpers

        /// <summary>Copies the shared fields under the lock, then prints them — the lock is gone before the label changes.</summary>
        private void UpdateStateLabel()
        {
            bool running;
            int pushes;
            int resumeAt;
            ImportProgress last;
            lock (_sync)
            {
                running = _isRunning;
                pushes = _pushes;
                resumeAt = _resumeAtRow;
                last = _lastProgress;
            }

            int total = _file == null ? 0 : _file.TotalRows;
            string rows = last == null ? $"0/{total}" : $"{last.RowNumber}/{last.TotalRows}";
            string counts = last == null ? "imported 0 · skipped 0" : $"imported {last.Imported} · skipped {last.Skipped}";
            bool outage = _repository != null && _repository.SimulateOutage;

            this.labelState.Text =
                "SERVER STATE (this session only · ImportPage instance fields guarded by _sync · nothing static)\n" +
                $"isRunning={Low(running)} · rows {rows} · {counts} · resumeAt={(resumeAt > 1 ? resumeAt.ToString() : "-")} · outage={Low(outage)}\n" +
                $"pushes={pushes} (every {PushEveryRows} rows + 10% milestones + skipped rows + final) · IsWebSocket={Low(Application.IsWebSocket)} · polling={Low(_polling)}\n" +
                $"this line was written on thread {Thread.CurrentThread.ManagedThreadId} inside {(running ? "Application.Update(this, …)" : "a request")}";
        }

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
                _log.Info(LogLayer.UI, "ImportPage.ShowResult", $"OK · {result.Message}");
            }
            else
            {
                // Expected outcome: the service explained it in words the user may read.
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("rejected", StatusKind.Warning);
                _log.Warn(LogLayer.UI, "ImportPage.ShowResult", $"FAIL · {result.Message}");
            }
        }

        /// <summary>
        /// Unexpected failure on the request thread: details go to the log (with the exception type and
        /// message), the user sees one generic sentence. Nothing internal leaks through the banner.
        /// </summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message");
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            this.tracePanel.FlushPending();
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private static string Low(bool value) => value ? "true" : "false";

        #endregion
    }
}
