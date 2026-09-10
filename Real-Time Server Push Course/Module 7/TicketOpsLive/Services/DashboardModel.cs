using System;

namespace TicketOpsLive.Services
{
    /// <summary>
    /// A read-only copy of the dashboard numbers, taken under the model lock so the three values
    /// always belong to the same instant. The UI renders snapshots, never the live fields.
    /// </summary>
    public readonly struct DashboardSnapshot
    {
        public DashboardSnapshot(int openTickets, int queueDepth, double avgWaitMinutes, long version)
        {
            OpenTickets = openTickets;
            QueueDepth = queueDepth;
            AvgWaitMinutes = avgWaitMinutes;
            Version = version;
        }

        public int OpenTickets { get; }
        public int QueueDepth { get; }
        public double AvgWaitMinutes { get; }

        /// <summary>How many model changes produced this state (grows on every Step, not on every UI refresh).</summary>
        public long Version { get; }
    }

    /// <summary>
    /// The simulated TicketOps dashboard model: open tickets, queue depth, average wait — a random walk.
    ///
    /// Thread-safe: <see cref="Step"/> is called by the simulator task (every 50 ms), <see cref="Snapshot"/>
    /// by the timer tick (a browser request). Both run under one lock, so a tick never reads a half-changed
    /// model. The model knows nothing about pages, controls or pushes: it only changes numbers.
    /// </summary>
    public sealed class DashboardModel
    {
        private readonly object _sync = new object();
        private readonly Random _random;
        private int _openTickets = 42;
        private int _queueDepth = 7;
        private double _avgWaitMinutes = 3.4;
        private long _version;

        public DashboardModel(int seed)
        {
            _random = new Random(seed);
        }

        /// <summary>One model change: every value moves a little, the version grows by one.</summary>
        public void Step()
        {
            lock (_sync)
            {
                _openTickets = Clamp(_openTickets + _random.Next(-2, 3), 0, 200);
                _queueDepth = Clamp(_queueDepth + _random.Next(-1, 2), 0, 40);
                _avgWaitMinutes = Math.Round(Math.Clamp(_avgWaitMinutes + (_random.NextDouble() - 0.5) * 0.4, 0.5, 30.0), 1);
                _version++;
            }
        }

        /// <summary>A consistent copy of the three values, taken under the lock.</summary>
        public DashboardSnapshot Snapshot()
        {
            lock (_sync)
            {
                return new DashboardSnapshot(_openTickets, _queueDepth, _avgWaitMinutes, _version);
            }
        }

        private static int Clamp(int value, int min, int max) => value < min ? min : (value > max ? max : value);
    }
}
