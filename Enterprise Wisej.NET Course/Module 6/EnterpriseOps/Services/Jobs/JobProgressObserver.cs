using System;
using System.Diagnostics;
using System.Threading;
using Wisej.Web;

namespace EnterpriseOps.Services.Jobs
{
    /// <summary>
    /// The progress observer: the one place in the app that pushes to the browser without a request.
    ///
    /// The halves never share an object. The queue worker changes the job store on a plain thread pool
    /// thread that has NO Wisej session — it cannot touch a control and does not try to. All it does here is
    /// set a flag (<see cref="_dirty"/>). The observer belongs to a page, runs inside
    /// <c>Application.StartTask</c> (which keeps the session context), and every
    /// <see cref="MinPushIntervalMs"/> it checks whether anything changed, refreshes the screen and flushes
    /// with <c>Application.Update</c>.
    ///
    /// So the UI updates at most 1000 / <see cref="MinPushIntervalMs"/> times per second, no matter how many
    /// events the job raises: the flag is a flag, not a queue.
    /// </summary>
    public sealed class JobProgressObserver : IDisposable
    {
        /// <summary>The bound between two pushes (the cookbook's floor is 250 ms).</summary>
        public const int MinPushIntervalMs = 400;

        /// <summary>The loop stops after this long with nothing to observe; any user action restarts it.</summary>
        private static readonly TimeSpan IdleTimeout = TimeSpan.FromSeconds(20);

        private readonly IJobStatusStore _store;
        private readonly INotificationService _notifications;
        private readonly Control _target;
        private readonly Action _refresh;

        private volatile bool _dirty = true;
        private volatile bool _disposed;
        private int _running;

        public JobProgressObserver(IJobStatusStore store, INotificationService notifications,
            Control target, Action refresh)
        {
            _store = store;
            _notifications = notifications;
            _target = target;
            _refresh = refresh;

            // Both events are raised by the queue worker — a thread with no session. Handlers must be cheap
            // and must never touch a control.
            _store.Changed += Store_Changed;
            _notifications.Published += Notifications_Published;
        }

        /// <summary>
        /// Starts the push loop if it is not already running. Called from a request thread (page load,
        /// button click, grid selection) — <c>Application.StartTask</c> inherits that session context and
        /// keeps it for the life of the task.
        /// </summary>
        public void Start()
        {
            if (_disposed || _target.IsDisposed)
                return;
            if (Interlocked.Exchange(ref _running, 1) == 1)
                return;

            Application.StartTask(PushLoop);
        }

        /// <summary>Called by the page when it knows the screen is stale (it just started a job, for instance).</summary>
        public void Invalidate()
        {
            _dirty = true;
            Start();
        }

        private void Store_Changed(object sender, JobChangedEventArgs e) => _dirty = true;

        private void Notifications_Published(object sender, Notification e) => _dirty = true;

        /// <summary>
        /// Runs on a background thread inside the session context: change controls, then
        /// <c>Application.Update</c> to flush them over the WebSocket.
        /// </summary>
        private void PushLoop()
        {
            var idle = Stopwatch.StartNew();
            try
            {
                while (!_disposed && !_target.IsDisposed)
                {
                    if (_dirty)
                    {
                        _dirty = false;
                        idle.Restart();

                        _refresh();
                        Application.Update(_target);
                    }
                    else if (idle.Elapsed > IdleTimeout)
                    {
                        break;      // nothing to observe: stop pushing until something happens again
                    }

                    Thread.Sleep(MinPushIntervalMs);
                }
            }
            catch (ObjectDisposedException)
            {
                // The page was closed or the session ended while we were mid-push. Expected; the job is
                // unaffected — it belongs to the queue, not to this loop.
            }
            catch (Exception)
            {
                // An observer must never take the session down with it.
            }
            finally
            {
                Interlocked.Exchange(ref _running, 0);
            }
        }

        public void Dispose()
        {
            _disposed = true;
            _store.Changed -= Store_Changed;
            _notifications.Published -= Notifications_Published;
        }
    }
}
