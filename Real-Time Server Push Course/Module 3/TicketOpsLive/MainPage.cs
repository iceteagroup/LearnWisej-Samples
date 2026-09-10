using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using TicketOpsLive.Services;
using Wisej.Web;

namespace TicketOpsLive
{
    public enum TraceDirection { Push, Request, Server }

    /// <summary>
    /// TicketOps Live — Module 3 · Progress Without Refresh: Background Import Monitor.
    ///
    /// Left card:    the Import Monitor — progress bar, status and record-count labels, the import log
    ///               (newest first, every line stamped with the JobId), the fail-at-87 switch, job id and
    ///               elapsed time. The import runs on a task started from the click and is pushed to the
    ///               browser every 10 records; the browser never refreshes.
    /// Right card:   every push the server makes, every request the browser sends, every server decision.
    /// Bottom bar:   success/progress path (Start import), cancellation (Cancel), failure (the checkbox
    ///               throws at record 87), recovery (Start again), the anti-pattern (Blocking import).
    ///
    /// The six-step pattern of the lesson is marked "Step 1 … Step 6" in startImportButton_Click and RunImport.
    /// </summary>
    public partial class MainPage : Page
    {
        // Per-session state lives in INSTANCE fields: every browser tab has its own MainPage, its own
        // running job and its own CancellationTokenSource. A static _cts would let one user cancel
        // another user's import.
        private CancellationTokenSource _cts;
        private volatile bool _importRunning;
        private ImportJob _currentJob;
        private readonly List<ImportJob> _finishedJobs = new List<ImportJob>();
        private int _pushes;
        private int _completed;
        private int _cancelled;
        private int _failed;
        private int _pushers;               // tasks that currently need out-of-bound delivery
        private bool _polling;              // the fallback is active (no WebSocket when a task started)

        private const int TotalRecords = 200;
        private const int RecordMilliseconds = 25;
        private const int PushEvery = 10;
        private const int LogEvery = 50;

        public MainPage()
        {
            InitializeComponent();

            // The page going away is a cancellation too: the lesson says "if the background work is only
            // for that page, cancel it on disposal". The loop sees the token at its next record and the
            // finally block skips the UI work because IsDisposed is true.
            this.Disposed += (s, e) => RequestCancel("page disposed");
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            Application.Title = "TicketOps Live";
            AddTrace(TraceDirection.Request, "MainPage_Load", $"first request: the page is rendered and returned with the response · IsWebSocket={Low(Application.IsWebSocket)} (the socket opens after this response)");
            SetStatus("idle · click ▶ Start import", StatusKind.Normal);
            UpdateState();
        }

        #region Success / progress path — Start import (the six-step pattern, steps 1-3)

        private void startImportButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "startImportButton_Click", "the browser sent the click");

            if (_importRunning)
            {
                // Acceptance criterion: starting twice must not create two competing imports.
                AddTrace(TraceDirection.Server, "startImportButton_Click", $"refused — job {_currentJob?.JobId} is still running (_importRunning == true)");
                ShowBanner($"⚠ Import {_currentJob?.JobId} is still running — a second click must not start a competing job.", BannerKind.Warn);
                return;
            }

            bool failAt87 = failAt87CheckBox.Checked;
            var job = new ImportJob(TotalRecords, "background");
            _currentJob = job;
            _importRunning = true;

            // Step 1 — adjust the UI before the work starts (these changes travel back with THIS response).
            startImportButton.Enabled = false;
            cancelImportButton.Enabled = true;
            blockingImportButton.Enabled = false;
            failAt87CheckBox.Enabled = false;
            importProgressBar.Value = 0;
            recordsImportedLabel.Text = "0";
            jobIdLabel.Text = "Job " + job.JobId + (failAt87 ? "  ·  will throw at record 87" : "");
            elapsedLabel.Text = "elapsed 0.00 s";
            importStatusLabel.Text = "Starting import…";
            HideBanner();
            SetStatus($"import {job.JobId} running", StatusKind.Normal);
            Log(job, failAt87 ? "Started (simulated failure armed at record 87)" : "Started");

            // Step 2 — create the cancellation state the user can trigger. Cooperative: nobody aborts a thread.
            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            AddTrace(TraceDirection.Server, "Application.StartTask", $"job {job.JobId}: {TotalRecords} records × {RecordMilliseconds} ms, push every {PushEvery} records, log every {LogEvery} · the handler returns now");
            UpdateState();
            BeginPush();

            // Step 3 — start the work in the session context. StartTask keeps THIS session's context on the
            // new thread, so RunImport can change the page's controls directly; nothing reaches the browser
            // until it calls Application.Update(this).
            Application.StartTask(() => RunImport(token, failAt87));
        }

        #endregion

        #region Cancellation path — Cancel (cooperative)

        private void cancelImportButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "cancelImportButton_Click", $"_cts.Cancel() → job {_currentJob?.JobId} sees the token at its next record (≤ {RecordMilliseconds} ms)");
            cancelImportButton.Enabled = false;
            importStatusLabel.Text = "Cancelling…";
            RequestCancel("user");
        }

        /// <summary>Signals the token. The loop decides when to stop: between two records, never in the middle of one.</summary>
        private void RequestCancel(string who)
        {
            var cts = _cts;
            if (cts == null)
                return;
            try
            {
                cts.Cancel();
                if (_currentJob != null && !this.IsDisposed)
                    Log(_currentJob, "Cancel requested by " + who);
            }
            catch (ObjectDisposedException)
            {
                // the job finished (and disposed its token source) between the check and the call
            }
        }

        #endregion

        #region The import loop — steps 4-6 (runs on the task thread, session context kept by StartTask)

        /// <summary>
        /// The lab's loop. 200 simulated records, 25 ms each. The controls change on EVERY record (the
        /// server-side model is always exact) but the browser is updated every 10 records — plus the final
        /// state, which is always pushed from finally. try/catch/finally guarantees the UI is never stuck.
        /// </summary>
        private void RunImport(CancellationToken token, bool failAt87)
        {
            ImportJob job = _currentJob;

            try
            {
                // Step 4 — run the work in small steps: one record per iteration, the token checked between records.
                for (int i = 1; i <= TotalRecords; i++)
                {
                    token.ThrowIfCancellationRequested();       // cooperative cancellation point

                    Thread.Sleep(RecordMilliseconds);           // the simulated record (parse, validate, insert…)

                    if (failAt87 && i == 87)
                        throw new InvalidOperationException($"Simulated malformed record #{i}: field 'priority' has value 'urgentish' (job {job.JobId}).");

                    // The model changes on every record…
                    job.RecordsImported = i;
                    recordsImportedLabel.Text = i.ToString(CultureInfo.InvariantCulture);
                    importProgressBar.Value = i / 2;
                    importStatusLabel.Text = $"Importing… {i}/{TotalRecords}";
                    elapsedLabel.Text = "elapsed " + job.ElapsedText;

                    if (i % LogEvery == 0)
                        Log(job, $"Imported {i} records");

                    // Step 5 — …but the browser is updated at a controlled interval: every 10th record.
                    if (i % PushEvery == 0)
                    {
                        _pushes++;
                        job.Pushes++;
                        AddTrace(TraceDirection.Push, "Application.Update(this)", $"job {job.JobId} · {i}/{TotalRecords} · {i / 2}% · {job.ElapsedText}");
                        UpdateState();
                        Application.Update(this);
                    }
                }

                job.Complete();
                importStatusLabel.Text = "Import completed successfully.";
                Log(job, "Completed");
                AddTrace(TraceDirection.Server, "import", $"job {job.JobId} completed — {job.RecordsImported} records, {job.Pushes} pushes so far");
            }
            catch (OperationCanceledException)
            {
                // The token was signalled: the loop stopped between two records, the state is consistent.
                job.Cancel();
                importStatusLabel.Text = "Import cancelled by user.";
                Log(job, "Cancelled at " + DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + $" after record {job.RecordsImported}");
                AddTrace(TraceDirection.Server, "import", $"job {job.JobId} cancelled after record {job.RecordsImported} (OperationCanceledException caught inside the task)");
            }
            catch (Exception ex)
            {
                // A background task must never fail silently: the detail goes to the server log with the
                // JobId; the user gets a safe message, never a stack trace.
                job.Fail(ex);
                LogError(job.JobId, "import", ex);
                importStatusLabel.Text = "Import failed. Review the server log.";
                Log(job, "Failed at " + DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + $" on record {job.RecordsImported + 1}");
                AddTrace(TraceDirection.Server, "import", $"job {job.JobId} FAILED on record {job.RecordsImported + 1}: {ex.GetType().Name} caught inside the task → server log, safe message");
            }
            finally
            {
                // Step 6 — re-enable the UI and push the final state, whatever happened above.
                var cts = _cts;
                _cts = null;
                cts?.Dispose();
                _importRunning = false;
                FinishJob(job);

                if (!this.IsDisposed)
                {
                    try
                    {
                        // The context-callback form: several control changes applied in the session context
                        // and pushed in ONE flush. Equivalent to the direct form used in the loop above.
                        Application.Update(this, () =>
                        {
                            startImportButton.Enabled = true;
                            cancelImportButton.Enabled = false;
                            blockingImportButton.Enabled = true;
                            failAt87CheckBox.Enabled = true;
                            elapsedLabel.Text = "elapsed " + job.ElapsedText;
                            jobIdLabel.Text = "Job " + job.JobId + " · " + job.OutcomeText;
                            Log(job, job.Summary);

                            switch (job.Outcome)
                            {
                                case ImportOutcome.Failed:
                                    ShowBanner($"✖ {job.Summary}. The detail is in the server log under job {job.JobId} (client {Application.ClientId}). Click ▶ Start import to recover.", BannerKind.Error);
                                    SetStatus("import failed — UI restored in finally", StatusKind.Error);
                                    break;
                                case ImportOutcome.Cancelled:
                                    ShowBanner($"ⓘ {job.Summary}. Records 1–{job.RecordsImported} are in; nothing was half-written.", BannerKind.Info);
                                    SetStatus("import cancelled — UI restored in finally", StatusKind.Warn);
                                    break;
                                default:
                                    SetStatus("import completed", StatusKind.Normal);
                                    break;
                            }

                            _pushes++;
                            job.Pushes++;
                            EndPush();
                            AddTrace(TraceDirection.Push, "Application.Update(this, …)", $"{job.Summary} · {job.Pushes} pushes (final state, finally block)");
                            UpdateState();
                        });
                    }
                    catch (ObjectDisposedException) { /* the page went away between the check and the push */ }
                }
            }
        }

        #endregion

        #region Anti-pattern — the import runs INSIDE the click handler

        private void blockingImportButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "blockingImportButton_Click", "the browser sent the click — and now waits for the response");

            if (_importRunning)
            {
                AddTrace(TraceDirection.Server, "blockingImportButton_Click", $"refused — job {_currentJob?.JobId} is still running");
                ShowBanner("⚠ A background import is running — wait for it or cancel it first.", BannerKind.Warn);
                return;
            }

            const int records = 60;
            var job = new ImportJob(records, "blocking");
            HideBanner();
            jobIdLabel.Text = "Job " + job.JobId + "  ·  blocking (anti-pattern)";
            SetStatus($"blocking import {job.JobId} — the request is open", StatusKind.Warn);
            Log(job, $"Started INSIDE the click handler — {records} records × {RecordMilliseconds} ms");
            AddTrace(TraceDirection.Server, "blocking import", $"job {job.JobId}: {records} records synchronously in the handler · no push is possible while the request is open — Application.Update would have nothing to deliver ahead of the response");

            var watch = Stopwatch.StartNew();
            try
            {
                for (int i = 1; i <= records; i++)
                {
                    Thread.Sleep(RecordMilliseconds);
                    job.RecordsImported = i;
                    importProgressBar.Value = i * 100 / records;
                    recordsImportedLabel.Text = i.ToString(CultureInfo.InvariantCulture);
                    importStatusLabel.Text = $"Blocking import… {i}/{records}";
                    // No Application.Update here on purpose: the click request is still open and the browser
                    // is waiting for its response. Every one of these 60 changes collapses into that single
                    // response — the user sees the bar jump from 0 to 100 after ~1.5 s of nothing.
                }
                job.Complete();
            }
            catch (Exception ex)
            {
                job.Fail(ex);
                LogError(job.JobId, "blocking import", ex);
            }
            finally
            {
                watch.Stop();
                FinishJob(job);
                elapsedLabel.Text = "elapsed " + job.ElapsedText;
                jobIdLabel.Text = "Job " + job.JobId + " · " + job.OutcomeText + " (blocking)";
                importStatusLabel.Text = $"Blocking import done — the browser saw nothing for {watch.ElapsedMilliseconds} ms, then everything at once.";
                Log(job, job.Summary);
                ShowBanner($"⚠ Anti-pattern: {job.Summary}. Progress, Cancel and the trace were all frozen for {watch.ElapsedMilliseconds} ms because the handler never returned.", BannerKind.Warn);
                SetStatus("blocking import finished — everything arrived with one response", StatusKind.Warn);
                AddTrace(TraceDirection.Server, "in-request update", $"job {job.JobId}: {records} model changes → 0 pushes, ONE response after {watch.ElapsedMilliseconds} ms (compare the timestamps: these lines were written {watch.ElapsedMilliseconds} ms apart but arrived together)");
                UpdateState();
            }
        }

        #endregion

        #region Delivery: WebSocket push, or the polling fallback while a task needs it

        /// <summary>
        /// Called where out-of-bound work starts. With a WebSocket the pushes flow by themselves. Without one,
        /// ask the browser to poll every second for the duration of the work — and only for that duration.
        /// </summary>
        private void BeginPush()
        {
            _pushers++;
            if (Application.IsWebSocket || _polling)
                return;

            Application.StartPolling(1000);
            _polling = true;
            AddTrace(TraceDirection.Server, "Application.StartPolling(1000)", "no WebSocket when the task started → fallback polling ON until the task ends");
        }

        private void EndPush()
        {
            if (_pushers > 0) _pushers--;
            if (_pushers > 0 || !_polling)
                return;

            Application.EndPolling();
            _polling = false;
            AddTrace(TraceDirection.Server, "Application.EndPolling()", "the last task ended → fallback polling OFF");
        }

        #endregion

        #region Job bookkeeping, log, trace and UI helpers

        private enum StatusKind { Normal, Warn, Error }
        private enum BannerKind { Info, Warn, Error }

        /// <summary>Moves a job to the per-session history and updates the outcome counters.</summary>
        private void FinishJob(ImportJob job)
        {
            if (job == null)
                return;
            switch (job.Outcome)
            {
                case ImportOutcome.Completed: _completed++; break;
                case ImportOutcome.Cancelled: _cancelled++; break;
                case ImportOutcome.Failed: _failed++; break;
            }
            _finishedJobs.Add(job);
            while (_finishedJobs.Count > 20)
                _finishedJobs.RemoveAt(0);
            if (ReferenceEquals(_currentJob, job))
                _currentJob = null;
        }

        /// <summary>The import log the lab asks for: newest first, every line stamped with the JobId.</summary>
        private void Log(ImportJob job, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            importLogListBox.Items.Insert(0, $"{time}  [{job.JobId}] {message}");
            while (importLogListBox.Items.Count > 200)
                importLogListBox.Items.RemoveAt(importLogListBox.Items.Count - 1);
        }

        private void clearLogButton_Click(object sender, EventArgs e)
        {
            importLogListBox.Items.Clear();
            AddTrace(TraceDirection.Request, "clearLogButton_Click", "import log cleared (in-request update)");
        }

        private void AddTrace(TraceDirection direction, string name, string payload)
        {
            string prefix = direction switch
            {
                TraceDirection.Push => "→ push    ",
                TraceDirection.Request => "← request ",
                _ => "• server  ",
            };
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            listTrace.Items.Add($"{time}  {prefix} {name,-30} {payload}");
            while (listTrace.Items.Count > 400)
                listTrace.Items.RemoveAt(0);
            listTrace.SelectedIndex = listTrace.Items.Count - 1;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            listTrace.Items.Clear();
        }

        private void SetStatus(string text, StatusKind kind)
        {
            labelStatus.Text = "● " + text;
            labelStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        private void ShowBanner(string text, BannerKind kind)
        {
            labelBanner.Text = text;
            switch (kind)
            {
                case BannerKind.Error:
                    labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
                    labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
                    break;
                case BannerKind.Warn:
                    labelBanner.BackColor = System.Drawing.Color.FromArgb(255, 244, 229);
                    labelBanner.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
                    break;
                default:
                    labelBanner.BackColor = System.Drawing.Color.FromArgb(230, 240, 251);
                    labelBanner.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
                    break;
            }
            labelBanner.Visible = true;
        }

        private void HideBanner()
        {
            labelBanner.Visible = false;
        }

        private void UpdateState()
        {
            string session = Application.SessionId ?? "";
            if (session.Length > 8) session = session.Substring(0, 8) + "…";

            ImportJob job = _currentJob;
            string jobText = job == null ? "—" : job.JobId;
            string records = job == null ? "0" : job.RecordsImported.ToString(CultureInfo.InvariantCulture);

            string history = "none yet";
            if (_finishedJobs.Count > 0)
            {
                var parts = new List<string>();
                for (int i = _finishedJobs.Count - 1; i >= 0 && parts.Count < 3; i--)
                    parts.Add(_finishedJobs[i].Short);
                history = string.Join("  ·  ", parts);
                if (_finishedJobs.Count > 3) history += $"  (+{_finishedJobs.Count - 3} more)";
            }

            labelState.Text =
                $"SERVER STATE (authoritative · this session only)\n" +
                $"job={jobText} running={Low(_importRunning)} records={records}/{TotalRecords} pushes={_pushes} completed={_completed} cancelled={_cancelled} failed={_failed} tasks={_pushers} polling={Low(_polling)}\n" +
                $"IsWebSocket={Low(Application.IsWebSocket)} ClientId={Application.ClientId} SessionId={session}\n" +
                $"finished jobs: {history}\n" +
                $"_cts, the running job and the job list are MainPage instance fields — one per browser tab, never static";
        }

        /// <summary>Server log: the technical detail (with the JobId) stays here; the UI only gets a safe message.</summary>
        private static void LogError(string jobId, string operation, Exception ex)
        {
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} job {jobId} {operation} failed for client {Application.ClientId} session {Application.SessionId}: {ex}");
        }

        private static string Low(bool value) => value ? "true" : "false";

        #endregion
    }
}
