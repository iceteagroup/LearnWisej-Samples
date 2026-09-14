using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace WisejPerfLab.Diagnostics
{
    /// <summary>
    /// The Module 1 deliverable: a scope that turns a named user action into a comparable number.
    /// <c>using (_probe.Measure("Dashboard", "Refresh"))</c> writes a PERF start record, starts a
    /// <see cref="Stopwatch"/>, and on dispose writes a PERF end record with the elapsed milliseconds
    /// and the row count.
    /// </summary>
    /// <remarks>
    /// Two rules make the numbers worth keeping:
    /// <list type="bullet">
    /// <item>The scope goes around the <b>whole user action</b> — the query, the projection and the control
    /// mutation — not around the query alone. A scope around the query only would have said 90 ms for a
    /// refresh the user waits 1.8 seconds for.</item>
    /// <item>The fields are logged as structured values (<c>{Scenario}</c>, <c>{ElapsedMs}</c>), not
    /// concatenated into the message, so a log pipeline can aggregate them later.</item>
    /// </list>
    /// It is registered as a singleton: it holds no per-session state, and the records it writes go to the
    /// logger and to the PERF buffer of the session that is running (<see cref="SessionPerfLog"/>).
    /// </remarks>
    public sealed class ScenarioProbe
    {
        private readonly ILogger<ScenarioProbe> _logger;

        public ScenarioProbe(ILogger<ScenarioProbe> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Opens a measured scope. Dispose it (a <c>using</c> block) to write the PERF end record —
        /// a scenario that throws still reports its elapsed time, because the dispose runs anyway.
        /// </summary>
        /// <param name="scenario">The screen or feature: Dashboard, Tickets, Customers.</param>
        /// <param name="userAction">The action inside it: Refresh, Search, ExpandNode, Export.</param>
        /// <param name="rowCount">The row count when it is known up front; otherwise set <see cref="ScenarioScope.Rows"/> later.</param>
        public ScenarioScope Measure(string scenario, string userAction, int? rowCount = null)
            => new ScenarioScope(this, scenario, userAction, rowCount);

        private void Write(PerfRecord record)
        {
            if (record.Phase == "start")
            {
                _logger.LogInformation(
                    "PERF start {Scenario} {UserAction} rows={Rows}",
                    record.Scenario, record.UserAction, record.Rows);
            }
            else if (record.Failed)
            {
                _logger.LogWarning(
                    "PERF end {Scenario} {UserAction} elapsedMs={ElapsedMs} rows={Rows} failed={Error}",
                    record.Scenario, record.UserAction, record.ElapsedMs, record.Rows, record.Error);
            }
            else
            {
                _logger.LogInformation(
                    "PERF end {Scenario} {UserAction} elapsedMs={ElapsedMs} rows={Rows}",
                    record.Scenario, record.UserAction, record.ElapsedMs, record.Rows);
            }

            SessionPerfLog.Current?.Write(record);
        }

        /// <summary>One measured scenario. Disposing it closes the measurement.</summary>
        public sealed class ScenarioScope : IDisposable
        {
            private readonly ScenarioProbe _probe;
            private readonly Stopwatch _stopwatch;
            private readonly string _scenario;
            private readonly string _userAction;
            private bool _disposed;

            internal ScenarioScope(ScenarioProbe probe, string scenario, string userAction, int? rowCount)
            {
                _probe = probe;
                _scenario = scenario;
                _userAction = userAction;
                Rows = rowCount;

                _probe.Write(new PerfRecord
                {
                    Timestamp = DateTime.Now,
                    Phase = "start",
                    Scenario = scenario,
                    UserAction = userAction,
                    Rows = rowCount
                });

                _stopwatch = Stopwatch.StartNew();
            }

            /// <summary>The row count, set when the scenario knows it (usually after the query).</summary>
            public int? Rows { get; set; }

            /// <summary>Elapsed time so far, for a handler that wants to report it on screen.</summary>
            public long ElapsedMs => _stopwatch.ElapsedMilliseconds;

            /// <summary>Records that this run failed. The end record is still written, with the elapsed time.</summary>
            public void Fail(Exception exception)
            {
                Error = exception?.GetType().Name;
            }

            private string Error { get; set; }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _stopwatch.Stop();

                _probe.Write(new PerfRecord
                {
                    Timestamp = DateTime.Now,
                    Phase = "end",
                    Scenario = _scenario,
                    UserAction = _userAction,
                    ElapsedMs = _stopwatch.ElapsedMilliseconds,
                    Rows = Rows,
                    Error = Error
                });
            }
        }
    }
}
