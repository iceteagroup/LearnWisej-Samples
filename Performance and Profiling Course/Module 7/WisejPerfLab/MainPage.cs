using System;
using System.Collections.Generic;
using System.Drawing;
using WisejPerfLab.Data;
using WisejPerfLab.Diagnostics;
using WisejPerfLab.Forms;
using WisejPerfLab.Pages;
using WisejPerfLab.Shell;
using Wisej.Services;
using Wisej.Web;

namespace WisejPerfLab
{
    /// <summary>
    /// The shell: the three screens that own the three measured scenarios, the PERF log of this session,
    /// and the lab controls that make a measurement repeatable — a discarded warm-up run, three runs to
    /// take a median from, and a database outage to show what a failing scenario measures.
    /// </summary>
    public partial class MainPage : Page, IPerfLabShell
    {
        // Resolved through Microsoft DI: Startup.cs registered app.Services with Wisej.NET.
        [Inject]
        private OutageSwitch Outage { get; set; }

        [Inject]
        private ScenarioProbe Probe { get; set; }

        private readonly List<IScenarioPage> _scenarioPages = new List<IScenarioPage>();

        private DashboardPage _dashboardPage;
        private TicketGridPage _ticketPage;
        private CustomerTreePage _customerPage;
        private CapacityPage _capacityPage;

        private PerfLogBuffer _perfLog;

        // Module 4: snapshot A of the open/close comparison.
        private MemorySnapshot _snapshotA;

        public MainPage()
        {
            InitializeComponent();
        }

        #region Load

        private void MainPage_Load(object sender, EventArgs e)
        {
            lblDataset.Text =
                $"{PerfLabDatabase.Dataset.Tickets:N0} tickets · {PerfLabDatabase.Dataset.Customers:N0} customer nodes\r\n" +
                $"build: {BuildConfiguration}   session: {Application.SessionId}";

            // One PERF buffer per session. The probe writes into it from whichever thread is running.
            _perfLog = SessionPerfLog.Current;
            if (_perfLog != null)
                _perfLog.RecordWritten += OnPerfRecordWritten;

            this.Disposed += MainPage_Disposed;

            _dashboardPage = new DashboardPage(this) { Dock = DockStyle.Fill };
            _ticketPage = new TicketGridPage(this) { Dock = DockStyle.Fill };
            _customerPage = new CustomerTreePage(this) { Dock = DockStyle.Fill };
            _capacityPage = new CapacityPage(this) { Dock = DockStyle.Fill };

            tabDashboard.Controls.Add(_dashboardPage);
            tabTickets.Controls.Add(_ticketPage);
            tabCustomers.Controls.Add(_customerPage);
            tabCapacity.Controls.Add(_capacityPage);

            _scenarioPages.Add(_dashboardPage);
            _scenarioPages.Add(_ticketPage);
            _scenarioPages.Add(_customerPage);
            _scenarioPages.Add(_capacityPage);

            SetStatus(ShellState.Idle, "idle — run a scenario, or warm the app up first");
            RenderPerfLog();
        }

        private static string BuildConfiguration
        {
            get
            {
#if DEBUG
                return "Debug (measure in Release)";
#else
                return "Release";
#endif
            }
        }

        #endregion

        #region The PERF log card

        private void OnPerfRecordWritten(object sender, PerfRecord record)
        {
            // A background task writes from its own thread; the list is rebuilt on the next update either way.
            RenderPerfLog();
        }

        private void RenderPerfLog()
        {
            if (_perfLog == null)
                return;

            listPerf.BeginUpdate();
            try
            {
                listPerf.Items.Clear();
                foreach (var record in _perfLog.Records)
                    listPerf.Items.Add(record.Timestamp.ToString("HH:mm:ss.fff") + "  " + record);

                if (listPerf.Items.Count > 0)
                    listPerf.SelectedIndex = listPerf.Items.Count - 1;
            }
            finally
            {
                listPerf.EndUpdate();
            }
        }

        /// <summary>A line in the PERF log that is not a measurement: a warm-up marker, a median, a note.</summary>
        private void Note(string text)
        {
            _perfLog?.Write(new PerfRecord { Timestamp = DateTime.Now, Phase = "note", Scenario = text });
        }

        #endregion

        #region The lab controls

        /// <summary>
        /// The run nobody records: it pays for JIT, the first query plan and the first control layout.
        /// Every baseline in this course is taken after one of these.
        /// </summary>
        private void btnWarmUp_Click(object sender, EventArgs e)
        {
            ClearBanner();
            Note("warm-up run — the three records below are discarded");

            foreach (var page in _scenarioPages)
                RunOne(page);

            Note("warm-up complete — start measuring from here");
            SetStatus(ShellState.Ok, "warm-up done — the app is warm, the next runs are the ones to keep");
        }

        /// <summary>
        /// Three runs of each scenario, then the median of each. A single run is a sample, not a number:
        /// the difference between two single runs is often smaller than the noise between them.
        /// </summary>
        private void btnRunThree_Click(object sender, EventArgs e)
        {
            ClearBanner();
            Note("three runs per scenario — medians follow");

            for (var run = 1; run <= 3; run++)
                foreach (var page in _scenarioPages)
                    RunOne(page);

            var summary = new List<string>();
            foreach (var page in _scenarioPages)
            {
                var median = _perfLog?.MedianElapsed(page.Scenario, page.UserAction);
                if (median == null)
                    continue;

                Note($"median {page.Scenario}/{page.UserAction} = {PerfBudget.Describe(page.Scenario, page.UserAction, median.Value)}");

                // The noise floor. Without it, a 10 % "improvement" in the next module is just a rerun.
                var spread = _perfLog.SpreadPercent(page.Scenario, page.UserAction);
                if (spread != null)
                {
                    var samples = _perfLog.Samples(page.Scenario, page.UserAction);
                    Note($"      spread {samples[0]:N0}-{samples[samples.Count - 1]:N0} ms = {spread.Value} % of the median — a smaller difference is not a finding");
                }

                summary.Add($"{page.Scenario}/{page.UserAction} {median.Value:N0} ms");
            }

            SetStatus(ShellState.Warn, "medians of three runs — " + string.Join("   ", summary));
        }

        private void RunOne(IScenarioPage page)
        {
            try
            {
                page.RunScenario();
            }
            catch (Exception ex)
            {
                // A scenario that fails is still a measured scenario: the probe already wrote its end record.
                ShowBanner($"{page.Scenario}/{page.UserAction} failed: {ex.Message}");
            }
        }

        /// <summary>
        /// The note that belongs beside the trace file, built from what the app actually knows: the
        /// scenario, the build, the dataset, the tool the budget names, and the numbers of the last runs.
        /// </summary>
        private void btnTraceNote_Click(object sender, EventArgs e)
        {
            var last = LastCompletedRun();
            if (last == null)
            {
                SetStatus(ShellState.Idle, "run a scenario first — there is nothing to write a note about");
                return;
            }

            var samples = _perfLog.Samples(last.Scenario, last.UserAction);
            var median = _perfLog.MedianElapsed(last.Scenario, last.UserAction);
            var spread = _perfLog.SpreadPercent(last.Scenario, last.UserAction);
            var threshold = PerfBudget.ThresholdMs(last.Scenario, last.UserAction);
            var stamp = DateTime.Now.ToString("yyyyMMdd-HHmm");
            var fileName =
                $"PP-M02_{last.Scenario}{last.UserAction}_{PerfLabDatabase.Dataset.Tickets}_{BuildConfigurationShort}_{stamp}.diagsession";

            var note = string.Join(Environment.NewLine, new[]
            {
                "// Trace note — save beside the .diagsession under the same name",
                "// File:     " + fileName,
                "// Module:   02 — Visual Studio profiling workflow",
                "// Scenario: " + last.Scenario + "/" + last.UserAction + "   rows=" + (last.Rows?.ToString("N0") ?? "-"),
                "// Build:    " + BuildConfiguration + ", x64, no debugger, warm",
                "// Hosting:  Kestrel, local SQLite (App_Data/perflab.db)",
                "// Dataset:  " + PerfLabDatabase.Dataset.Tickets.ToString("N0") + " tickets, " +
                    PerfLabDatabase.Dataset.Customers.ToString("N0") + " customer nodes",
                "// Browser:  one tab, one session",
                "// Tool:     " + PerfBudget.Tool(last.Scenario, last.UserAction),
                "// Budget:   " + (threshold.HasValue ? "< " + threshold.Value + " ms" : "(none)"),
                "// Measured: last run " + last.ElapsedMs + " ms" +
                    (median.HasValue ? ", median of " + samples.Count + " = " + median.Value + " ms" : string.Empty) +
                    (spread.HasValue ? ", spread " + spread.Value + " %" : string.Empty),
                string.Empty,
                "// Collect it:",
                "//   1. Release, no debugger. Run the scenario once without recording (warm-up).",
                "//   2. Debug > Performance Profiler (Alt+F2), tick CPU Usage and nothing else, Start.",
                "//   3. Run the scenario exactly once. Stop as soon as the screen settles.",
                "//   4. Drag a selection on the summary timeline around the click itself.",
                "//   5. Call tree, Show Hot Path, scroll past the ASP.NET Core and Wisej.NET dispatch",
                "//      frames to the first function in the WisejPerfLab namespace. Record total and self CPU.",
                "//   6. Caller/callee on that function: who calls it, and how many times.",
                "//   7. Repeat with Instrumentation for call counts and wall-clock time, and compare:",
                "//      CPU time close to wall-clock = the code is working; far below = it is waiting."
            });

            Console.Error.WriteLine(note);
            Note("trace note written for " + last.Scenario + "/" + last.UserAction);

            using var dialog = new TraceNoteForm(note);
            dialog.ShowDialog();
        }

        private PerfRecord LastCompletedRun()
        {
            if (_perfLog == null)
                return null;

            var records = _perfLog.Records;
            for (var i = records.Count - 1; i >= 0; i--)
                if (records[i].Phase == "end")
                    return records[i];

            return null;
        }

        private static string BuildConfigurationShort
        {
            get
            {
#if DEBUG
                return "Debug";
#else
                return "Release";
#endif
            }
        }

        #region Memory snapshots (Module 4)

        /// <summary>
        /// Snapshot A: the reading the open/close comparison is measured against. Take it on a warm,
        /// idle app — the same rule as every other measurement in this course.
        /// </summary>
        private void btnSnapshotA_Click(object sender, EventArgs e)
        {
            _snapshotA = MemoryProbe.Take();
            Note("snapshot A — " + _snapshotA);
            lblMemory.Text = "A: " + _snapshotA;
            SetStatus(ShellState.Ok, "snapshot A taken — now open and close the detail form fifty times");
        }

        /// <summary>
        /// Opens and closes the ticket detail form fifty times, the way a user would over an afternoon.
        /// Before Module 4 this left fifty forms, fifty bus subscriptions and fifty running timers in the
        /// session. Now it leaves nothing: watch the subscriber count come back to where it started.
        /// </summary>
        private void btnOpenCloseFifty_Click(object sender, EventArgs e)
        {
            var row = _ticketPage.FirstRowOrPlaceholder();   // a TicketGridRow since Module 5

            using var scope = Probe.Measure("Session", "OpenCloseDetail", 50);
            for (var i = 0; i < 50; i++)
            {
                var form = new TicketDetailForm(row);
                form.Show();
                form.Close();
            }

            SetStatus(ShellState.Busy, "opened and closed the detail form 50 times in " + scope.ElapsedMs + " ms — take snapshot B");
        }

        /// <summary>
        /// Snapshot B and the comparison. The two numbers that matter are not the megabytes: they are
        /// the subscriber count and the outstanding form count, because those say whether the objects
        /// <b>can</b> be collected. Bytes move for a dozen reasons; a rooted object never goes away.
        /// </summary>
        private void btnSnapshotB_Click(object sender, EventArgs e)
        {
            var snapshotB = MemoryProbe.Take();
            Note("snapshot B — " + snapshotB);

            var comparison = MemoryProbe.Compare(_snapshotA, snapshotB);
            Note(comparison);
            lblMemory.Text = "B: " + snapshotB + "\r\n" + comparison;

            var leaked = _snapshotA != null && snapshotB.FormsOutstanding > _snapshotA.FormsOutstanding;
            SetStatus(leaked ? ShellState.Fault : ShellState.Ok, comparison);
        }

        #endregion

        private void btnBreakDatabase_Click(object sender, EventArgs e)
        {
            Outage.Break();
            SetStatus(ShellState.Fault, "the ticket database is unreachable — run a scenario to see what a failure measures");
            Note("lab: database connection broken");
        }

        private void btnRestoreDatabase_Click(object sender, EventArgs e)
        {
            Outage.Restore();
            ClearBanner();
            SetStatus(ShellState.Ok, "database restored");
            Note("lab: database connection restored");
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            _perfLog?.Clear();
            RenderPerfLog();
            SetStatus(ShellState.Idle, "PERF log cleared");
        }

        #endregion

        #region IPerfLabShell

        public void SetStatus(ShellState state, string text)
        {
            Color color;
            switch (state)
            {
                case ShellState.Ok: color = Color.FromArgb(31, 157, 87); break;
                case ShellState.Busy: color = Color.FromArgb(21, 101, 216); break;
                case ShellState.Warn: color = Color.FromArgb(232, 161, 60); break;
                case ShellState.Fault: color = Color.FromArgb(224, 86, 59); break;
                default: color = Color.FromArgb(90, 107, 125); break;
            }

            lblState.ForeColor = color;
            lblState.Text = "● " + text;
        }

        public void ShowBanner(string message)
        {
            lblBanner.Text = message;
            lblBanner.Visible = true;
        }

        public void ClearBanner()
        {
            lblBanner.Text = string.Empty;
            lblBanner.Visible = false;
        }

        #endregion

        #region Disposal

        /// <summary>
        /// The page unsubscribes from the buffer it subscribed to. Nothing leaks here — the buffer lives
        /// in this session and dies with it — but the discipline is the point of Module 4: whoever
        /// subscribes, unsubscribes, in the same class.
        /// </summary>
        private void MainPage_Disposed(object sender, EventArgs e)
        {
            if (_perfLog != null)
            {
                _perfLog.RecordWritten -= OnPerfRecordWritten;
                _perfLog = null;
            }
        }

        #endregion
    }
}
