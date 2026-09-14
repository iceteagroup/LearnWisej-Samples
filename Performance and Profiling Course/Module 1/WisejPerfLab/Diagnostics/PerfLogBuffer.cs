using System;
using System.Collections.Generic;

namespace WisejPerfLab.Diagnostics
{
    /// <summary>
    /// The PERF records of one session, kept so the screen can show what the console log shows.
    /// One instance per session (see <see cref="SessionPerfLog"/>): a static list here would be a
    /// textbook retention root and would mix the scenarios of every user into one timeline.
    /// </summary>
    public sealed class PerfLogBuffer
    {
        private const int MaxRecords = 400;

        private readonly List<PerfRecord> _records = new List<PerfRecord>();

        /// <summary>Raised on the thread that wrote the record, request thread or background task.</summary>
        public event EventHandler<PerfRecord> RecordWritten;

        public IReadOnlyList<PerfRecord> Records => _records;

        public void Write(PerfRecord record)
        {
            lock (_records)
            {
                _records.Add(record);
                if (_records.Count > MaxRecords)
                    _records.RemoveRange(0, _records.Count - MaxRecords);
            }

            RecordWritten?.Invoke(this, record);
        }

        public void Clear()
        {
            lock (_records)
            {
                _records.Clear();
            }

            RecordWritten?.Invoke(this, null);
        }

        /// <summary>
        /// The median elapsed time of the last <paramref name="runs"/> completed runs of one scenario.
        /// The course never reports a single run: the median of three is what goes into the baseline,
        /// so a difference inside the noise floor is not mistaken for a finding.
        /// </summary>
        public long? MedianElapsed(string scenario, string userAction, int runs = 3)
        {
            var samples = new List<long>();
            lock (_records)
            {
                for (var i = _records.Count - 1; i >= 0 && samples.Count < runs; i--)
                {
                    var record = _records[i];
                    if (record.Phase == "end" && !record.Failed &&
                        record.Scenario == scenario && record.UserAction == userAction)
                        samples.Add(record.ElapsedMs);
                }
            }

            if (samples.Count == 0)
                return null;

            samples.Sort();
            return samples[samples.Count / 2];
        }
    }
}
