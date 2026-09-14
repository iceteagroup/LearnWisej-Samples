using System;
using System.Collections.Generic;
using System.Drawing;
using WisejPerfLab.Data;
using WisejPerfLab.Diagnostics;
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

        private readonly List<IScenarioPage> _scenarioPages = new List<IScenarioPage>();

        private DashboardPage _dashboardPage;
        private TicketGridPage _ticketPage;
        private CustomerTreePage _customerPage;

        private PerfLogBuffer _perfLog;

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

            tabDashboard.Controls.Add(_dashboardPage);
            tabTickets.Controls.Add(_ticketPage);
            tabCustomers.Controls.Add(_customerPage);

            _scenarioPages.Add(_dashboardPage);
            _scenarioPages.Add(_ticketPage);
            _scenarioPages.Add(_customerPage);

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
