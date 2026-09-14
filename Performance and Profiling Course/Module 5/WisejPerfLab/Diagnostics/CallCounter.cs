using System;
using System.Collections.Generic;
using Wisej.Web;

namespace WisejPerfLab.Diagnostics
{
    /// <summary>
    /// Counts how often a named function ran inside the current scenario — the one number a CPU Usage
    /// trace cannot give you and an Instrumentation trace can.
    /// </summary>
    /// <remarks>
    /// Module 2 is about reading a profiler, not replacing it. This counter exists so the app can state
    /// the call count that the Instrumentation trace and the caller/callee view will confirm: when the
    /// trace says <c>TicketFormatter.FormatRow</c> is the hot path, the question that matters is not how
    /// fast the function is but <b>who calls it 50,000 times</b>. Counting costs about 50 ns per call;
    /// it is a lab instrument, and the first thing you would remove before recording a sampling trace.
    /// </remarks>
    public static class CallCounter
    {
        private const string Key = "CallCounters";

        private static Dictionary<string, int> Current
        {
            get
            {
                try
                {
                    if (Application.Session[Key] is Dictionary<string, int> existing)
                        return existing;

                    var created = new Dictionary<string, int>();
                    Application.Session[Key] = created;
                    return created;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>Records one call of <paramref name="function"/>.</summary>
        public static void Count(string function)
        {
            var counters = Current;
            if (counters == null)
                return;

            lock (counters)
            {
                counters.TryGetValue(function, out var count);
                counters[function] = count + 1;
            }
        }

        public static void Reset()
        {
            var counters = Current;
            if (counters == null)
                return;

            lock (counters)
            {
                counters.Clear();
            }
        }

        /// <summary>The counts since the last reset, largest first.</summary>
        public static IReadOnlyList<KeyValuePair<string, int>> Snapshot()
        {
            var counters = Current;
            if (counters == null)
                return Array.Empty<KeyValuePair<string, int>>();

            lock (counters)
            {
                var list = new List<KeyValuePair<string, int>>(counters);
                list.Sort((a, b) => b.Value.CompareTo(a.Value));
                return list;
            }
        }
    }
}
