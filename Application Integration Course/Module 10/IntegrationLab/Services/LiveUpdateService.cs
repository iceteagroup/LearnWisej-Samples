using System;
using System.Threading;
using IntegrationLab.Data;
using Wisej.Web;

namespace IntegrationLab.Services
{
    /// <summary>
    /// Background updates done safely (lesson: "Application.StartTask → change options / Call → Application.Update").
    ///
    /// The three parts and the three cares:
    ///   1. <see cref="Application.StartTask(Action)"/> runs the loop on a background thread while keeping the
    ///      session context, so the loop can still reach the page and its widgets.
    ///   2. Inside the loop the page changes options / issues Call(...) exactly like a button handler would
    ///      (the <see cref="Updated"/> handler does that).
    ///   3. <see cref="Application.Update(Wisej.Core.IWisejComponent, Action)"/> pushes the pending changes to the
    ///      browser: without a request in flight nothing else would send them.
    ///
    ///   - Frequency is bounded: one push every <see cref="IntervalMs"/>, at most <see cref="MaxIterations"/> pushes.
    ///   - The session may end while the task runs: the loop checks <see cref="Control.IsDisposed"/> and catches
    ///     <see cref="ObjectDisposedException"/> instead of throwing on a disposed widget.
    ///   - The page stops the task when it closes (OperationsPage wires Disposed → Dispose()).
    /// </summary>
    public sealed class LiveUpdateService : IDisposable
    {
        private readonly Control _context;
        private readonly Func<int, LiveReading> _next;
        private readonly ManualResetEventSlim _stopSignal = new ManualResetEventSlim(false);
        private volatile bool _running;
        private int _iteration;

        /// <param name="context">The page (or any component) whose session context the task keeps and updates.</param>
        /// <param name="next">Produces the reading for iteration n (1-based). Runs on the background thread.</param>
        public LiveUpdateService(Control context, Func<int, LiveReading> next, int intervalMs = 1500, int maxIterations = 40)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _next = next ?? throw new ArgumentNullException(nameof(next));
            if (intervalMs < 250) throw new ArgumentOutOfRangeException(nameof(intervalMs), intervalMs, "Bound the frequency: at least 250 ms between pushes.");
            if (maxIterations < 1 || maxIterations > 1000) throw new ArgumentOutOfRangeException(nameof(maxIterations), maxIterations, "Bound the task: 1..1000 pushes.");
            this.IntervalMs = intervalMs;
            this.MaxIterations = maxIterations;
        }

        public int IntervalMs { get; }
        public int MaxIterations { get; }
        public bool IsRunning => _running;
        public int Iteration => _iteration;

        /// <summary>Raised inside Application.Update(context): the handler may touch controls and issue Call(...).</summary>
        public event EventHandler<LiveUpdateEventArgs> Updated;

        /// <summary>Raised (inside Application.Update) once the loop has ended, with the reason.</summary>
        public event EventHandler<LiveUpdateStoppedEventArgs> Stopped;

        public void Start()
        {
            if (_running) return;
            if (_context.IsDisposed) throw new ObjectDisposedException(nameof(LiveUpdateService), "The page is already disposed.");

            _stopSignal.Reset();
            _iteration = 0;
            _running = true;

            // (unverified beyond the docs) Application.StartTask keeps the session context on the new thread.
            Application.StartTask(RunLoop);
        }

        /// <summary>Requests a stop; the loop ends within one interval and raises <see cref="Stopped"/>.</summary>
        public void Stop() => _stopSignal.Set();

        private void RunLoop()
        {
            string reason = "completed";
            try
            {
                while (_iteration < this.MaxIterations)
                {
                    // Bounded frequency: wait one interval, or leave early when Stop() was called.
                    if (_stopSignal.Wait(this.IntervalMs)) { reason = "stopped by operator"; break; }
                    if (_context.IsDisposed) { reason = "page disposed"; break; }

                    var reading = _next(_iteration + 1);      // the "work": compute the next batch off the request thread
                    _iteration++;

                    try
                    {
                        // Everything the handler changes (Options, Call, labels) is batched and pushed once.
                        Application.Update(_context, () =>
                            Updated?.Invoke(this, new LiveUpdateEventArgs(reading, _iteration, this.MaxIterations)));
                    }
                    catch (ObjectDisposedException)
                    {
                        reason = "widget disposed";
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                reason = "fault: " + ex.Message;
            }
            finally
            {
                _running = false;
                if (!_context.IsDisposed)
                {
                    try
                    {
                        Application.Update(_context, () =>
                            Stopped?.Invoke(this, new LiveUpdateStoppedEventArgs(reason, _iteration)));
                    }
                    catch (ObjectDisposedException) { /* the page went away between the check and the push */ }
                }
            }
        }

        /// <summary>Called from the page's Disposed handler: only signals the loop, which then ends within one interval.</summary>
        public void Dispose()
        {
            Stop();
        }
    }

    public class LiveUpdateEventArgs : EventArgs
    {
        public LiveUpdateEventArgs(LiveReading reading, int iteration, int maxIterations)
        {
            this.Reading = reading;
            this.Iteration = iteration;
            this.MaxIterations = maxIterations;
        }

        public LiveReading Reading { get; }
        public int Iteration { get; }
        public int MaxIterations { get; }
    }

    public class LiveUpdateStoppedEventArgs : EventArgs
    {
        public LiveUpdateStoppedEventArgs(string reason, int iterations)
        {
            this.Reason = reason;
            this.Iterations = iterations;
        }

        public string Reason { get; }
        public int Iterations { get; }
    }
}
