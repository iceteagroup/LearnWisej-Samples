using System;
using System.Diagnostics;
using System.Threading;
using Wisej.Web;

namespace EnterpriseOps.Services.Jobs
{
    /// <summary>
    /// The progress observer: the ONE place in the sample that pushes to the browser without a request.
    ///
    /// The halves never share an object. The queue worker changes the job store on a plain thread pool
    /// thread that has NO Wisej session — it cannot touch a control and does not try to. All it does here is
    /// set a flag (<see cref="_dirty"/>). The observer belongs to a page, runs inside
    /// <c>Application.StartTask</c> (which keeps the session context), and every
    /// <see cref="MinPushIntervalMs"/> it asks "did anything change?", refreshes the screen and flushes with
    /// <c>Application.Update</c>.
    ///
    /// That is the answer to the lab's second review question, "how often does the UI update?": at most
    /// 1000 / <see cref="MinPushIntervalMs"/> times per second, no matter how many events the job raises —
    /// a thousand row events coalesce into a handful of pushes, because the flag is a flag and not a queue.
    /// Set <see cref="Throttled"/> to false and the same loop becomes the anti-pattern the lesson warns about:
    /// one push per event. The page's "Anti-pattern: push every row" button does exactly that, on purpose,
    /// and prints both counters afterwards.
    /// </summary>
    public sealed class JobProgressObserver : IDisposable
    {
        /// <summary>The bound. The cookbook's floor is 250 ms; 400 ms is calm and still feels live.</summary>
        public const int MinPushIntervalMs = 400;

        /// <summary>Unthrottled = the anti-pattern: the loop pushes as fast as events arrive.</summary>
        private const int FloodIntervalMs = 5;

        /// <summary>The loop stops after this long with nothing to observe; any user action restarts it.</summary>
        private static readonly TimeSpan IdleTimeout = TimeSpan.FromSeconds(20);

        private readonly IJobStatusStore _store;
        private readonly INotificationService _notifications;
        private readonly Control _target;
        private readonly Action<JobProgressObserver> _refresh;

        private volatile bool _dirty = true;
        private volatile bool _disposed;
        private int _running;
        private long _events;
        private long _pushes;

        public JobProgressObserver(IJobStatusStore store, INotificationService notifications,
            Control target, Action<JobProgressObserver> refresh)
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

        /// <summary>Milestones only (true) or every event (false, the anti-pattern).</summary>
        public bool Throttled { get; set; } = true;

        /// <summary>Change events received from the job store and the notification service.</summary>
        public long Events => Interlocked.Read(ref _events);

        /// <summary>Screen refreshes actually pushed to the browser. The ratio is the whole point.</summary>
        public long Pushes => Interlocked.Read(ref _pushes);

        public bool IsRunning => Volatile.Read(ref _running) == 1;

        public DateTime? LastPushUtc { get; private set; }

        public string PolicyDescription =>
            Throttled
                ? $"throttled · at most one push every {MinPushIntervalMs} ms ({1000 / MinPushIntervalMs} per second)"
                : $"UNTHROTTLED (anti-pattern) · one push per event, every {FloodIntervalMs} ms";

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

        public void ResetCounters()
        {
            Interlocked.Exchange(ref _events, 0);
            Interlocked.Exchange(ref _pushes, 0);
        }

        private void Store_Changed(object sender, JobChangedEventArgs e)
        {
            Interlocked.Increment(ref _events);
            _dirty = true;
        }

        private void Notifications_Published(object sender, Notification e)
        {
            Interlocked.Increment(ref _events);
            _dirty = true;
        }

        /// <summary>
        /// Runs on a background thread inside the session context. Everything the UI does happens here:
        /// change controls, then <c>Application.Update</c> to flush them over the WebSocket.
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

                        Interlocked.Increment(ref _pushes);
                        _refresh(this);
                        Application.Update(_target);
                        LastPushUtc = DateTime.UtcNow;
                    }
                    else if (idle.Elapsed > IdleTimeout)
                    {
                        break;      // nothing to observe: stop pushing until something happens again
                    }

                    Thread.Sleep(Throttled ? MinPushIntervalMs : FloodIntervalMs);
                }
            }
            catch (ObjectDisposedException)
            {
                // The page was reopened or the session ended while we were mid-push. Expected; the job is
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
