using System;
using System.Threading;
using Wisej.Web;

namespace TicketOpsLive.Services
{
    /// <summary>
    /// The "machine producing telemetry" of the lesson: a session-owned background task that changes the
    /// <see cref="DashboardModel"/> every 50 ms — faster than any UI cadence the user can select.
    ///
    /// It is deliberately dumb about the UI: after every change it only invokes <c>onModelChanged</c>
    /// (the page's <c>SimulatedModelChanged()</c>, which counts the event and sets the dirty flag). It never
    /// touches a control and never calls <c>Application.Update</c>. Applying the model to the screen is the
    /// job of the page's <c>refreshTimer</c>, at the UI cadence — that is the coalescing pattern.
    ///
    /// Ownership: one simulator per page (instance field, never static). It stops when <see cref="Stop"/> is
    /// called (live mode off) or when <c>stopRequested()</c> says the page is disposed. A generation counter
    /// makes a Stop/Start pair safe: a loop from an older generation exits even if it was still sleeping.
    /// </summary>
    public sealed class DashboardSimulator
    {
        public const int IntervalMs = 50;

        private readonly DashboardModel _model;
        private readonly Action _onModelChanged;
        private readonly Func<bool> _stopRequested;
        private readonly Action<Exception> _onError;
        private volatile bool _running;
        private int _generation;

        public DashboardSimulator(DashboardModel model, Action onModelChanged, Func<bool> stopRequested, Action<Exception> onError)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _onModelChanged = onModelChanged ?? throw new ArgumentNullException(nameof(onModelChanged));
            _stopRequested = stopRequested ?? (() => false);
            _onError = onError ?? (ex => { });
        }

        public bool IsRunning => _running;

        /// <summary>Starts the task once. A second call while running is refused (returns false).</summary>
        public bool Start()
        {
            if (_running)
                return false;

            _running = true;
            int generation = Interlocked.Increment(ref _generation);

            // Application.StartTask keeps the session context on the new thread, so the callback can read
            // Application.ClientId and the page can count the event in its own instance fields.
            Application.StartTask(() => Loop(generation));
            return true;
        }

        /// <summary>Cooperative stop: the loop sees the flag at its next check (within 50 ms).</summary>
        public void Stop()
        {
            _running = false;
        }

        private void Loop(int generation)
        {
            try
            {
                while (_running && generation == _generation && !_stopRequested())
                {
                    _model.Step();          // the model changed…
                    _onModelChanged();      // …tell the page (count + dirty flag). No push here, ever.
                    Thread.Sleep(IntervalMs);
                }
            }
            catch (Exception ex)
            {
                // A background task must never fail silently and must never take the session down.
                _onError(ex);
            }
            finally
            {
                if (generation == _generation)
                    _running = false;
            }
        }
    }
}
