using System;
using System.Diagnostics;

namespace WisejPerfLab.Health
{
    /// <summary>
    /// The two process readings the health gate needs: memory in use and CPU load, as percentages.
    /// </summary>
    /// <remarks>
    /// Wisej.NET computes the same two numbers internally for <c>healthcheck.wx</c> (its own accessors
    /// are not public), so this is the sample's equivalent — and it has the advantage of being readable:
    /// memory is the process working set against the memory the runtime reports as available, and CPU is
    /// processor time used since the previous reading divided by wall-clock time and cores.
    /// <para>
    /// Both are cheap and both are sampled: a health check is asked every few seconds by every instance,
    /// and one that costs anything makes the instance less healthy each time it is asked.
    /// </para>
    /// </remarks>
    public static class ServerLoad
    {
        private static readonly object Gate = new object();

        private static DateTime _lastSampleAt = DateTime.UtcNow;
        private static TimeSpan _lastProcessorTime = TimeSpan.Zero;
        private static int _lastCpuPercent;

        /// <summary>Working set as a percentage of the memory the runtime reports as available.</summary>
        public static int MemoryPercent()
        {
            try
            {
                var workingSet = Process.GetCurrentProcess().WorkingSet64;
                var available = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

                if (available <= 0)
                    return 0;

                return (int)Math.Min(100, workingSet * 100 / available);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Processor time used since the previous call, as a percentage of one wall-clock second across
        /// all cores. The first call after start returns 0: a rate needs two samples.
        /// </summary>
        public static int CpuPercent()
        {
            try
            {
                lock (Gate)
                {
                    var now = DateTime.UtcNow;
                    var processorTime = Process.GetCurrentProcess().TotalProcessorTime;
                    var elapsed = (now - _lastSampleAt).TotalMilliseconds;

                    if (elapsed < 200)
                        return _lastCpuPercent;      // too soon to be a rate

                    var used = (processorTime - _lastProcessorTime).TotalMilliseconds;
                    _lastSampleAt = now;
                    _lastProcessorTime = processorTime;
                    _lastCpuPercent = (int)Math.Min(100, Math.Max(0, used * 100 / (elapsed * Environment.ProcessorCount)));

                    return _lastCpuPercent;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
