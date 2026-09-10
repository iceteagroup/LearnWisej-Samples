using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EnterpriseOps.Data;
using EnterpriseOps.Services;
using EnterpriseOps.Services.Jobs;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// The Import Center — the observing half of the pipeline.
    ///
    /// This page starts jobs and displays whatever the job store says about them. It does not import
    /// anything, does not own a thread, and holds no state a job depends on: close it (the "Reopen" button
    /// disposes it and builds a new one) and the import carries on, because the queue, the job store, the
    /// notification service and the work-order table live in <see cref="JobInfrastructure"/> — the process,
    /// not the session.
    ///
    /// Layout:
    ///   header      screen name · tenant · user · session correlation id · the notification bell
    ///   pnlQueue    dgvJobs (this tenant's jobs) + the progress observer's bar, percent and status
    ///   pnlJobDetail  the JobDetailPanel: status history and the per-row result
    ///   pnlNotifications  notification records with read / unread state
    ///   pnlTrace    Server · live activity trace (UI → · Service: · Queue: · Job: · Data: · Security: · Observer:)
    ///   pnlActions  success · progress · failure · anti-pattern · recovery · clear
    ///
    /// Every handler is thin: build a command, call <see cref="ImportService"/>, show the typed result.
    /// The only place that touches the browser without a request is <see cref="JobProgressObserver"/>.
    /// </summary>
    public partial class ImportCenterPage : Page
    {
        private static readonly Color Ok = Color.FromArgb(31, 157, 87);
        private static readonly Color Warn = Color.FromArgb(232, 161, 60);
        private static readonly Color Bad = Color.FromArgb(224, 86, 59);

        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;
        private readonly ImportService _service;
        private readonly CommandContext _observerContext;
        private JobProgressObserver _observer;

        /// <summary>How much of each job's history has already been copied into the trace.</summary>
        private readonly Dictionary<Guid, int> _tracedHistory = new Dictionary<Guid, int>();

        private Guid _selectedJobId;
        private bool _suspendSelection;
        private int _lastUnread = -1;

        /// <summary>The job the observer counters are currently measuring (events vs pushes).</summary>
        private Guid _measuredJobId;
        private DateTime _measuredStartUtc;
        private bool _measuredIsFlood;

        public ImportCenterPage()
        {
            InitializeComponent();

            // Session state lives in Application.Session (per session, never static), so it survives the page
            // being disposed and recreated by the "Reopen" button.
            _session = (SessionContext)Application.Session.Context;
            _trace = (ActivityTrace)Application.Session.Trace;
            _observerContext = _session.NewCommand();
            _service = new ImportService(_trace);

            _trace.LineAdded += Trace_LineAdded;

            // The observer: the only pusher. It refreshes this page from the store and flushes with
            // Application.Update, at most once every JobProgressObserver.MinPushIntervalMs.
            _observer = new JobProgressObserver(JobInfrastructure.Store, JobInfrastructure.Notifications,
                this, RefreshFromObserver);
        }

        #region Load / teardown

        private void ImportCenterPage_Load(object sender, EventArgs e)
        {
            try
            {
                this.lblContext.Text =
                    $"tenant {_session.TenantId} ({_session.TenantName}) · {_session.User} ({_session.Role}) · session {_session.SessionCorrelationId}";
                this.lblQueueTitle.Text = $"Job queue · tenant {_session.TenantId}";
                this.pnlJobDetail.ShowPolicy(_service.RetryPolicy);

                foreach (var file in _service.ListFiles())
                    this.cboFile.Items.Add(new FileItem(file));
                if (this.cboFile.Items.Count > 0)
                    this.cboFile.SelectedIndex = 0;

                ReplayTrace();
                _trace.Add($"UI → Import Center opened by {_session.User} · observer {_observer.PolicyDescription}.");

                // Nothing of the job's history is replayed into the trace on open: mark what already exists
                // as "seen", then log only what happens from now on.
                foreach (var job in _service.ListJobs(_observerContext, trace: true))
                    _tracedHistory[job.JobId] = job.History.Count;

                var active = _service.ActiveJob(_observerContext);
                if (active != null)
                {
                    // The recovery path: this page was just created and there is already a job running.
                    _selectedJobId = active.JobId;
                    _trace.Add($"UI → re-attached to {active.Number} ({ImportService.Describe(active.Status)} · {active.Percent}%) — " +
                               $"started by {active.StartedBy} {(DateTime.UtcNow - active.CreatedUtc).TotalSeconds:n0} s ago, in a page that no longer exists.");
                    foreach (var entry in active.History.Skip(Math.Max(0, active.History.Count - 3)))
                        _trace.Add($"Job: {active.Number} (while you were away) {entry.Message}");
                }

                RefreshAll();
                _observer.Start();
            }
            catch (Exception ex)
            {
                ReportFailure("The Import Center could not be opened.", ex);
            }
        }

        /// <summary>Reloads the session's trace into the list box — it outlives the page, so a reopen keeps it.</summary>
        private void ReplayTrace()
        {
            this.lstTrace.BeginUpdate();
            try
            {
                this.lstTrace.Items.Clear();
                foreach (string line in _trace.Snapshot())
                    this.lstTrace.Items.Add(line);
                SelectLastTraceLine();
            }
            finally
            {
                this.lstTrace.EndUpdate();
            }
        }

        /// <summary>Called from the designer's Dispose: the page detaches, the job carries on.</summary>
        private void DetachFromSession()
        {
            if (_trace != null)
                _trace.LineAdded -= Trace_LineAdded;
            if (_observer != null)
            {
                _observer.Dispose();
                _observer = null;
            }
        }

        #endregion

        #region Handlers (thin: build a command, call the service, show the result)

        private void btnStartImport_Click(object sender, EventArgs e)
        {
            var file = this.cboFile.SelectedItem as FileItem;
            StartImport(file?.Info.FileName, publishEveryRow: false, unthrottled: false, "UI → start import");
        }

        private void btnReimport_Click(object sender, EventArgs e)
        {
            // Same file again: every row updates its existing work order, none is duplicated, because the
            // writer upserts by ExternalRef. That is what makes retries safe.
            var file = this.cboFile.SelectedItem as FileItem;
            StartImport(file?.Info.FileName, publishEveryRow: false, unthrottled: false,
                "UI → re-import the same file (idempotency check: expect 0 created, all updated)");
        }

        private void btnMalformedFile_Click(object sender, EventArgs e)
        {
            // Failure path: the file cannot be parsed. That is TERMINAL — the job fails at once instead of
            // retrying something that can never work.
            StartImport(FakeImportFileSource.CorruptFile, publishEveryRow: false, unthrottled: false,
                "UI → start the corrupt file (expect a terminal failure, no retries)");
        }

        private void btnAntiPattern_Click(object sender, EventArgs e)
        {
            // The lesson's anti-pattern, on purpose and measured: the job publishes an event per row AND the
            // observer stops throttling, so every event becomes a push. Compare the counters afterwards.
            StartImport(FakeImportFileSource.FloodFile, publishEveryRow: true, unthrottled: true,
                "UI → ANTI-PATTERN: publish every row and push every event");
        }

        private void StartImport(string fileName, bool publishEveryRow, bool unthrottled, string traceLine)
        {
            try
            {
                if (fileName == null)
                {
                    ShowBanner("Choose a file first.", Warn);
                    return;
                }

                _trace.Add(traceLine + $" · \"{fileName}\"");
                var ctx = _session.NewCommand();
                var result = _service.StartImport(new StartImportCommand { FileName = fileName, PublishEveryRow = publishEveryRow }, ctx);

                if (!result.Succeeded)
                {
                    ShowBanner(string.Join(" ", result.Errors), Bad);
                    SetStatus("rejected", Bad);
                    return;
                }

                var job = result.Value;
                _selectedJobId = job.JobId;
                _tracedHistory[job.JobId] = 0;
                BeginMeasuring(job.JobId, unthrottled);

                HideBanner();
                SetStatus($"{job.Number} queued", Ok);
                AlertBox.Show($"{job.Number} queued — {job.Description}", MessageBoxIcon.Information,
                    alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);

                RefreshAll();
                _observer.Invalidate();     // the loop wakes up and starts pushing progress
            }
            catch (Exception ex)
            {
                ReportFailure("The import could not be started.", ex);
            }
        }

        private void btnCancelJob_Click(object sender, EventArgs e)
        {
            try
            {
                var job = SelectedJob() ?? _service.ActiveJob(_observerContext);
                if (job == null)
                {
                    ShowBanner("Select a job to cancel.", Warn);
                    return;
                }

                _trace.Add($"UI → cancel {job.Number} (sets a flag; the job decides when it is safe to stop)");
                var result = _service.CancelJob(new CancelJobCommand { JobId = job.JobId }, _session.NewCommand());

                if (!result.Succeeded)
                {
                    ShowBanner(string.Join(" ", result.Errors), Warn);
                    return;
                }

                SetStatus($"{job.Number} cancelling — finishing the current batch", Warn);
                _observer.Invalidate();
            }
            catch (Exception ex)
            {
                ReportFailure("The job could not be cancelled.", ex);
            }
        }

        /// <summary>
        /// The recovery path. Disposing this page is as close as a button can get to closing the tab: the
        /// observer stops, the controls go away, the trace and the session context stay in Application.Session,
        /// and the job — owned by the queue — never notices. The new page finds it in the store and re-attaches.
        /// </summary>
        private void btnReopenPage_Click(object sender, EventArgs e)
        {
            try
            {
                var active = _service.ActiveJob(_observerContext);
                _trace.Add(active == null
                    ? "UI → closing the Import Center page (no job is running)."
                    : $"UI → closing the Import Center page while {active.Number} is at {active.Percent}% — the queue keeps it running.");

                var fresh = new ImportCenterPage();
                Application.MainPage = fresh;

                if (!this.IsDisposed)
                    this.Dispose();
            }
            catch (Exception ex)
            {
                ReportFailure("The page could not be reopened.", ex);
            }
        }

        private void btnBell_Click(object sender, EventArgs e)
        {
            try
            {
                _trace.Add("UI → bell clicked");
                var notifications = _service.ListNotifications(_observerContext);
                if (notifications.Count == 0)
                {
                    AlertBox.Show("No notifications yet — start an import.", MessageBoxIcon.Information,
                        alignment: ContentAlignment.TopRight, autoCloseDelay: 3000);
                    return;
                }

                var newest = notifications[0];
                AlertBox.Show($"🔔 {newest.Title}\n{newest.Message}", MessageBoxIcon.Information,
                    alignment: ContentAlignment.TopRight, autoCloseDelay: 6000);
                _observer.Invalidate();
            }
            catch (Exception ex)
            {
                ReportFailure("The notifications could not be read.", ex);
            }
        }

        private void btnMarkRead_Click(object sender, EventArgs e)
        {
            try
            {
                int count = _service.MarkNotificationsRead(_session.NewCommand());
                _lastUnread = -1;
                RefreshNotifications();
                SetStatus(count == 0 ? "nothing unread" : $"{count} notification(s) marked read", Ok);
            }
            catch (Exception ex)
            {
                ReportFailure("The notifications could not be updated.", ex);
            }
        }

        private void dgvJobs_SelectionChanged(object sender, EventArgs e)
        {
            // The grid also raises this while it is being rebound and while the page is being disposed
            // (the "Reopen" path). Neither is a user selecting a job.
            if (_suspendSelection || _observer == null || this.IsDisposed)
                return;

            try
            {
                var row = this.dgvJobs.CurrentRow?.DataBoundItem as JobQueueRow;
                if (row == null || row.JobId == _selectedJobId)
                    return;

                _selectedJobId = row.JobId;
                _trace.Add($"UI → selected {row.Number} — the detail panel reads the store, it does not ask the job.");
                RefreshDetail();
                _observer?.Start();
            }
            catch (Exception ex)
            {
                ReportFailure("The job detail could not be shown.", ex);
            }
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            try
            {
                _trace.Clear();
                this.lstTrace.Items.Clear();
                HideBanner();
            }
            catch (Exception ex)
            {
                ReportFailure("The trace could not be cleared.", ex);
            }
        }

        #endregion

        #region The observer's view of the world

        /// <summary>
        /// Called by <see cref="JobProgressObserver"/> from inside Application.StartTask — a background thread
        /// that carries this session's context. Everything here changes controls; the observer flushes them
        /// with Application.Update immediately afterwards.
        /// </summary>
        private void RefreshFromObserver(JobProgressObserver observer)
        {
            RefreshAll();
            ReportMeasurementIfFinished(observer);
        }

        private void RefreshAll()
        {
            DrainJobHistoryIntoTrace();
            RefreshQueue();
            RefreshDetail();
            RefreshNotifications();
            RefreshStatus();
        }

        /// <summary>
        /// The job store's history is the job's own account of itself. Copying only the NEW entries into the
        /// trace is what keeps the trace readable: a thousand rows produce a dozen lines.
        /// </summary>
        private void DrainJobHistoryIntoTrace()
        {
            foreach (var job in _service.ListJobs(_observerContext))
            {
                int already = _tracedHistory.TryGetValue(job.JobId, out int seen) ? seen : job.History.Count;
                for (int i = already; i < job.History.Count; i++)
                {
                    var entry = job.History[i];
                    _trace.Add($"{entry.Layer} {job.Number} {entry.Message}");
                }
                _tracedHistory[job.JobId] = job.History.Count;
            }
        }

        private void RefreshQueue()
        {
            var rows = _service.ListJobRows(_observerContext).ToList();

            _suspendSelection = true;
            try
            {
                // A fresh list each time, so the grid always rebinds. There are a handful of jobs, not a
                // page of ten thousand rows — Module 5 is the one about paging.
                this.dgvJobs.DataSource = rows;

                int index = rows.FindIndex(r => r.JobId == _selectedJobId);
                if (index < 0 && rows.Count > 0)
                {
                    index = 0;
                    _selectedJobId = rows[0].JobId;
                }
                if (index >= 0 && index < this.dgvJobs.Rows.Count)
                    this.dgvJobs.Rows[index].Selected = true;
            }
            finally
            {
                _suspendSelection = false;
            }
        }

        private void RefreshDetail()
        {
            var job = SelectedJob();
            this.pnlJobDetail.Show(job);
        }

        private void RefreshNotifications()
        {
            var notifications = _service.ListNotifications(_observerContext);
            int unread = notifications.Count(n => !n.IsRead);

            this.btnBell.Text = unread > 0 ? $"🔔 Notifications ({unread})" : "🔔 Notifications";

            this.lstNotifications.BeginUpdate();
            try
            {
                this.lstNotifications.Items.Clear();
                if (notifications.Count == 0)
                    this.lstNotifications.Items.Add("(no notifications — start an import)");
                foreach (var n in notifications)
                    this.lstNotifications.Items.Add(n.ToString());
            }
            finally
            {
                this.lstNotifications.EndUpdate();
            }

            // A brand new unread notification is news: toast it once. This is the "while you were away"
            // delivery — the notification was stored by the queue worker, with no session in sight.
            if (_lastUnread >= 0 && unread > _lastUnread && notifications.Count > 0)
            {
                var newest = notifications[0];
                try
                {
                    AlertBox.Show($"🔔 {newest.Title}\n{newest.Message}", MessageBoxIcon.Information,
                        alignment: ContentAlignment.TopRight, autoCloseDelay: 6000);
                }
                catch (Exception)
                {
                    // A toast is never worth breaking the observer over.
                }
            }
            _lastUnread = unread;
        }

        private void RefreshStatus()
        {
            var job = _service.ActiveJob(_observerContext) ?? SelectedJob();
            if (job == null)
            {
                this.prgJob.Value = 0;
                this.lblPercent.Text = "0%";
                SetStatus("idle", Ok);
                return;
            }

            this.prgJob.Value = Math.Max(0, Math.Min(100, job.Percent));
            this.lblPercent.Text = job.Percent + "%";

            switch (job.Status)
            {
                case JobStatus.Queued:
                    SetStatus($"{job.Number} queued — waiting for the worker", Warn);
                    break;
                case JobStatus.Running:
                    SetStatus($"{job.Number} running · {job.Message}", Ok);
                    break;
                case JobStatus.Completed:
                    SetStatus($"{job.Number} completed", Ok);
                    HideBanner();
                    break;
                case JobStatus.CompletedWithErrors:
                    SetStatus($"{job.Number} completed with errors", Warn);
                    ShowBanner($"{job.Number}: {job.Message} Per-row detail is in the job detail panel — fix those rows and re-import only them.", Warn);
                    break;
                case JobStatus.Canceled:
                    SetStatus($"{job.Number} cancelled", Warn);
                    ShowBanner($"{job.Number}: {job.Message}", Warn);
                    break;
                case JobStatus.Failed:
                    SetStatus($"{job.Number} failed", Bad);
                    ShowBanner($"{job.Number}: {job.Message}", Bad);
                    break;
            }
        }

        #endregion

        #region Measuring the push rate (the lesson's "how often does the UI update?")

        private void BeginMeasuring(Guid jobId, bool unthrottled)
        {
            _measuredJobId = jobId;
            _measuredStartUtc = DateTime.UtcNow;
            _measuredIsFlood = unthrottled;
            _observer.ResetCounters();
            _observer.Throttled = !unthrottled;
            _trace.Add($"Observer: counters reset · {_observer.PolicyDescription}.");
        }

        private void ReportMeasurementIfFinished(JobProgressObserver observer)
        {
            if (_measuredJobId == Guid.Empty)
                return;

            var job = _service.GetJob(_measuredJobId, _observerContext);
            if (job == null || !job.IsFinished)
                return;

            double seconds = Math.Max(0.1, (DateTime.UtcNow - _measuredStartUtc).TotalSeconds);
            _trace.Add($"Observer: {job.Number} finished · {observer.Events} change event(s) → {observer.Pushes} push(es) " +
                       $"in {seconds:n1} s ({observer.Pushes / seconds:n1} pushes/s) · {observer.PolicyDescription}.");

            if (_measuredIsFlood)
            {
                observer.Throttled = true;
                _trace.Add($"Observer: throttling restored — {observer.PolicyDescription}. That ratio is the difference " +
                           "between a job the browser can watch and a job that floods it.");
            }

            _measuredJobId = Guid.Empty;
        }

        #endregion

        #region Small helpers

        private JobRecord SelectedJob() =>
            _selectedJobId == Guid.Empty ? null : _service.GetJob(_selectedJobId, _observerContext);

        private void SetStatus(string text, Color color)
        {
            this.lblStatus.Text = text;
            this.lblStatus.ForeColor = color;
        }

        private void ShowBanner(string text, Color color)
        {
            this.lblBanner.Text = text;
            this.lblBanner.ForeColor = color;
            this.lblBanner.BackColor = color == Bad ? Color.FromArgb(253, 240, 236) : Color.FromArgb(253, 247, 235);
            this.lblBanner.Visible = true;
        }

        private void HideBanner()
        {
            this.lblBanner.Visible = false;
            this.lblBanner.Text = "";
        }

        private void ReportFailure(string userMessage, Exception ex)
        {
            _trace.Error(ex);
            ShowBanner($"{userMessage} ({ex.GetType().Name})", Bad);
            SetStatus("error", Bad);
            AlertBox.Show(userMessage + " Check the activity trace for details.", MessageBoxIcon.Error,
                alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
        }

        /// <summary>The trace is session state; the list box is page state. This is the bridge.</summary>
        private void Trace_LineAdded(string line)
        {
            try
            {
                if (this.IsDisposed || this.lstTrace.IsDisposed)
                    return;

                this.lstTrace.Items.Add(line);
                if (this.lstTrace.Items.Count > 500)
                    this.lstTrace.Items.RemoveAt(0);
                SelectLastTraceLine();
            }
            catch (ObjectDisposedException)
            {
                // The page went away between the check and the add — the trace does not care.
            }
        }

        private void SelectLastTraceLine()
        {
            if (this.lstTrace.Items.Count > 0)
                this.lstTrace.SelectedIndex = this.lstTrace.Items.Count - 1;
        }

        /// <summary>What the file combo shows. A tiny view model, not an entity.</summary>
        private sealed class FileItem
        {
            public FileItem(ImportFileInfo info) { Info = info; }
            public ImportFileInfo Info { get; }
            public override string ToString() => $"{Info.FileName} — {Info.Description}";
        }

        #endregion
    }
}
