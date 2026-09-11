using System;
using System.Drawing;
using System.Linq;
using EnterpriseOps.Services;
using EnterpriseOps.Services.Jobs;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// The Import Center — the observing half of the pipeline.
    ///
    /// This page starts jobs and displays whatever the job store says about them. It does not import
    /// anything, does not own a thread, and holds no state a job depends on: close the browser tab and the
    /// import carries on, because the queue, the job store, the notification service and the work-order table
    /// live in <see cref="JobInfrastructure"/> — the process, not the session. The next session finds the job
    /// in the store and shows it again.
    ///
    /// Every handler is thin: build a command, call <see cref="ImportService"/>, show the typed result.
    /// The only place that touches the browser without a request is <see cref="JobProgressObserver"/>.
    /// </summary>
    public partial class ImportCenterPage : Page
    {
        private static readonly Color Warn = Color.FromArgb(232, 161, 60);
        private static readonly Color Bad = Color.FromArgb(224, 86, 59);

        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;
        private readonly ImportService _service;
        private readonly CommandContext _observerContext;
        private JobProgressObserver _observer;

        private Guid _selectedJobId;
        private bool _suspendSelection;
        private int _lastUnread = -1;

        public ImportCenterPage()
        {
            InitializeComponent();

            _session = (SessionContext)Application.Session.Context;
            _trace = new ActivityTrace();
            _observerContext = _session.NewCommand();
            _service = new ImportService(_trace);

            // The observer refreshes this page from the store and flushes with Application.Update,
            // at most once every JobProgressObserver.MinPushIntervalMs.
            _observer = new JobProgressObserver(JobInfrastructure.Store, JobInfrastructure.Notifications,
                this, RefreshAll);
        }

        #region Load / teardown

        private void ImportCenterPage_Load(object sender, EventArgs e)
        {
            try
            {
                foreach (var file in _service.ListFiles())
                    this.cboFile.Items.Add(new FileItem(file));
                if (this.cboFile.Items.Count > 0)
                    this.cboFile.SelectedIndex = 0;

                // A job may already be running, started from a session that has since closed.
                var active = _service.ActiveJob(_observerContext);
                if (active != null)
                {
                    _selectedJobId = active.JobId;
                    AlertBox.Show($"🔔 While you were away: {active.Number} reached {active.Percent}%.", MessageBoxIcon.Information,
                        alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
                }

                RefreshAll();
                _observer.Start();
            }
            catch (Exception ex)
            {
                ReportFailure("The Import Center could not be opened.", ex);
            }
        }

        /// <summary>Called from the designer's Dispose: the page stops observing, the job carries on.</summary>
        private void StopObserver()
        {
            if (_observer != null)
            {
                _observer.Dispose();
                _observer = null;
            }
        }

        #endregion

        #region Handlers

        private void btnStartImport_Click(object sender, EventArgs e)
        {
            try
            {
                var file = this.cboFile.SelectedItem as FileItem;
                if (file == null)
                {
                    ShowBanner("Choose a file first.", Warn);
                    return;
                }

                var result = _service.StartImport(new StartImportCommand { FileName = file.Info.FileName }, _session.NewCommand());
                if (!result.Succeeded)
                {
                    ShowBanner($"{string.Join(" ", result.Errors)} Reference: {result.CorrelationId}.", Bad);
                    return;
                }

                var job = result.Value;
                _selectedJobId = job.JobId;

                HideBanner();
                AlertBox.Show($"{job.Number} queued — {job.Description}", MessageBoxIcon.Information,
                    alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);

                RefreshAll();
                _observer.Invalidate();
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

                var result = _service.CancelJob(new CancelJobCommand { JobId = job.JobId }, _session.NewCommand());
                if (!result.Succeeded)
                {
                    ShowBanner(string.Join(" ", result.Errors), Warn);
                    return;
                }

                SetStatus($"{job.Number} cancelling — finishing the current batch");
                _observer.Invalidate();
            }
            catch (Exception ex)
            {
                ReportFailure("The job could not be cancelled.", ex);
            }
        }

        private void btnBell_Click(object sender, EventArgs e)
        {
            try
            {
                var notifications = _service.ListNotifications(_observerContext);
                if (notifications.Count == 0)
                {
                    AlertBox.Show("No notifications yet.", MessageBoxIcon.Information,
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
                _service.MarkNotificationsRead(_session.NewCommand());
                _lastUnread = -1;
                RefreshNotifications();
            }
            catch (Exception ex)
            {
                ReportFailure("The notifications could not be updated.", ex);
            }
        }

        private void dgvJobs_SelectionChanged(object sender, EventArgs e)
        {
            // The grid also raises this while it is being rebound and while the page is being disposed.
            // Neither is a user selecting a job.
            if (_suspendSelection || _observer == null || this.IsDisposed)
                return;

            try
            {
                var row = this.dgvJobs.CurrentRow?.DataBoundItem as JobQueueRow;
                if (row == null || row.JobId == _selectedJobId)
                    return;

                _selectedJobId = row.JobId;
                RefreshDetail();
                _observer?.Start();
            }
            catch (Exception ex)
            {
                ReportFailure("The job detail could not be shown.", ex);
            }
        }

        #endregion

        #region Refresh (called on load, after a command, and by the observer's push loop)

        private void RefreshAll()
        {
            RefreshQueue();
            RefreshDetail();
            RefreshNotifications();
            RefreshStatus();
        }

        private void RefreshQueue()
        {
            var rows = _service.ListJobRows(_observerContext).ToList();

            _suspendSelection = true;
            try
            {
                // A fresh list each time, so the grid always rebinds. There are a handful of jobs, so this is cheap.
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
            this.pnlJobDetail.Show(SelectedJob());
        }

        private void RefreshNotifications()
        {
            var notifications = _service.ListNotifications(_observerContext);
            int unread = notifications.Count(n => !n.IsRead);

            this.btnBell.Text = unread > 0 ? $"🔔 {unread}" : "🔔";

            this.lstNotifications.BeginUpdate();
            try
            {
                this.lstNotifications.Items.Clear();
                if (notifications.Count == 0)
                    this.lstNotifications.Items.Add("(no notifications)");
                foreach (var n in notifications)
                    this.lstNotifications.Items.Add(n.ToString());
            }
            finally
            {
                this.lstNotifications.EndUpdate();
            }

            // A brand new unread notification is news: toast it once. It was stored by the queue worker,
            // with no session in sight.
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
                SetStatus("Ready");
                return;
            }

            this.prgJob.Value = Math.Max(0, Math.Min(100, job.Percent));
            this.lblPercent.Text = job.Percent + "%";

            switch (job.Status)
            {
                case JobStatus.Queued:
                    SetStatus($"{job.Number} queued — waiting for the worker");
                    break;
                case JobStatus.Running:
                    SetStatus($"{job.Number} running · {job.Message}");
                    break;
                case JobStatus.Completed:
                    SetStatus($"{job.Number} completed");
                    HideBanner();
                    break;
                case JobStatus.CompletedWithErrors:
                    SetStatus($"{job.Number} completed with errors · per-row report in job detail");
                    ShowBanner($"{job.Number}: {job.Message} Fix the rows listed in the job detail and re-import them.", Warn);
                    break;
                case JobStatus.Canceled:
                    SetStatus($"{job.Number} cancelled");
                    ShowBanner($"{job.Number}: {job.Message}", Warn);
                    break;
                case JobStatus.Failed:
                    SetStatus($"{job.Number} failed");
                    ShowBanner($"{job.Number}: {job.Message}", Bad);
                    break;
            }
        }

        #endregion

        #region Helpers

        private JobRecord SelectedJob() =>
            _selectedJobId == Guid.Empty ? null : _service.GetJob(_selectedJobId, _observerContext);

        private void SetStatus(string text)
        {
            this.lblStatus.Text = text;
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
            string reference = SessionContext.NewCorrelationId();
            _trace?.Error(ex, reference);
            ShowBanner($"{userMessage} Reference: {reference}.", Bad);
            SetStatus("Error");
            AlertBox.Show($"{userMessage} Reference: {reference}.", MessageBoxIcon.Error,
                alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
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
