using System;

namespace WisejPerfLab.Diagnostics
{
    /// <summary>One PERF line: the structured shape the probe logs and the log card renders.</summary>
    public sealed class PerfRecord
    {
        public DateTime Timestamp { get; set; }

        /// <summary>"start" or "end". Both are written so an unfinished scenario is visible as a missing end.</summary>
        public string Phase { get; set; }

        /// <summary>The screen or feature: Dashboard, Tickets, Customers.</summary>
        public string Scenario { get; set; }

        /// <summary>The user action inside it: Refresh, Search, ExpandNode, Export.</summary>
        public string UserAction { get; set; }

        public long ElapsedMs { get; set; }

        public int? Rows { get; set; }

        /// <summary>Set when the scenario ended in a failure. A failed run still reports its elapsed time.</summary>
        public string Error { get; set; }

        public bool Failed => !string.IsNullOrEmpty(Error);

        /// <summary>The line as it appears in the console and in the PERF log card.</summary>
        public override string ToString()
        {
            if (Phase == "note")
                return "--  " + Scenario;

            var text = Phase == "start"
                ? $"PERF start  {Scenario}/{UserAction}"
                : $"PERF end    {Scenario}/{UserAction} elapsedMs={ElapsedMs}";

            if (Rows.HasValue)
                text += $" rows={Rows.Value}";

            if (Failed)
                text += $" failed={Error}";

            return text;
        }
    }
}
