using System;
using System.Threading;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Lets the failure-path buttons make the next command slow or the timeout short, without a test hook
    /// in the command shape. Per session, like everything else. Production builds do not need this class;
    /// it exists so the DB_TIMEOUT mapping can be seen on screen.
    /// </summary>
    public sealed class FaultInjector
    {
        private int _nextDelayMs;
        private int _nextTimeoutMs;

        /// <summary>Make the next command sleep inside its transaction (a simulated slow query).</summary>
        public void SlowNextCommand(int delayMs, int timeoutMs)
        {
            _nextDelayMs = delayMs;
            _nextTimeoutMs = timeoutMs;
        }

        /// <summary>Consumed by the service once; returns 0 when nothing was injected.</summary>
        public int TakeDelayMs() => Interlocked.Exchange(ref _nextDelayMs, 0);

        public TimeSpan? TakeTimeout()
        {
            int ms = Interlocked.Exchange(ref _nextTimeoutMs, 0);
            return ms > 0 ? TimeSpan.FromMilliseconds(ms) : (TimeSpan?)null;
        }
    }
}
