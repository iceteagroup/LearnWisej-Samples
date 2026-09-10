using System;
using System.Collections.Generic;
using System.Linq;
using TicketOps.Controls;
using TicketOps.Data;
using TicketOps.Diagnostics;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Ticket Workflow — the Module 8 screen.
    ///
    /// The Form is created by Program.Main with "new" and never sees a constructor argument: its five
    /// services (plus the log, the outage switch and the active profile) arrive through [Inject] properties
    /// filled from Application.Services. It hands them to a plain TicketWorkflowPresenter and becomes a
    /// translator: read the controls, call the presenter, render the WorkflowResult. Nothing in this file
    /// decides whether a ticket may close — that is the presenter's job, which is why the bottom bar can
    /// run the presenter's tests against fakes while the screen keeps its injected services.
    ///
    /// Left card:   signed-in operator, the open tickets, close / assign, and the "Injected services" grid
    ///              (contract → implementation → lifetime → instance id → what a second resolve returns).
    /// Right card:  the activity trace — UI → Presenter → Services → Data and back.
    /// Bottom bar:  progress path (run the presenter tests), two failure paths (permission, domain rule),
    ///              a DI failure path (resolving a service nobody registered), the error path with recovery
    ///              (simulated data outage) and Clear trace.
    /// </summary>
    public partial class TicketWorkflow : Form
    {
        // ---- property injection: the framework creates the Form, so it cannot take these by constructor ----

        [Inject(Required = true)]
        public ITicketService Tickets { get; set; }

        [Inject(Required = true)]
        public IUserService Users { get; set; }

        [Inject(Required = true)]
        public IPermissionService Permissions { get; set; }

        [Inject(Required = true)]
        public INotificationService Notifications { get; set; }

        [Inject(Required = true)]
        public IAuditLogService Audit { get; set; }

        [Inject]
        public ILog Log { get; set; }

        [Inject]
        public DataStoreHealth DataStore { get; set; }

        [Inject]
        public ActiveProfile Profile { get; set; }

        private ILog _log;                                   // the session log the trace panel is attached to
        private TicketWorkflowPresenter _presenter;
        private PresenterTestRunner _tests;
        private IReadOnlyList<Ticket> _rows = new List<Ticket>();
        private IReadOnlyList<Operator> _operators = new List<Operator>();
        private int _nextTest;
        private int _testsFailed;
        private bool _syncingOperator;

        /// <summary>The Designer (and the container) use the parameterless constructor; there is no other one.</summary>
        public TicketWorkflow()
        {
            InitializeComponent();
        }

        #region Screen lifecycle

        private async void TicketWorkflow_Load(object sender, EventArgs e)
        {
            try
            {
                EnsureInjected();
                _log.Info(LogLayer.UI, "TicketWorkflow.Load", "screen shown — the Form holds interfaces, not classes");

                BuildPresenter();
                _tests = new PresenterTestRunner(_log);

                FillOperators();
                ShowProfile();
                RefreshServicesGrid();
                await RefreshGridAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.Load", ex);
            }
        }

        /// <summary>
        /// Wisej.NET injects top-level containers (Form, Page, Desktop) automatically. If that has not happened
        /// by Load for a Form created with "new", ask the container explicitly — and say so in the trace, so
        /// the reviewer knows which path ran.
        /// </summary>
        private void EnsureInjected()
        {
            bool alreadyInjected = Tickets != null && Users != null && Permissions != null
                                   && Notifications != null && Audit != null && Log != null;
            if (!alreadyInjected)
                Application.Services.Inject(this);

            _log = Log ?? new ActivityLog();
            if (_log is ActivityLog activityLog)
                this.tracePanel.Attach(activityLog);

            if (alreadyInjected)
            {
                _log.Info(LogLayer.Infrastructure, "TicketWorkflow.EnsureInjected",
                    "[Inject] properties were filled automatically (top-level Form) — Application.Services.Inject(this) not needed");
            }
            else
            {
                _log.Warn(LogLayer.Infrastructure, "TicketWorkflow.EnsureInjected",
                    "auto-injection had not run by Load — called Application.Services.Inject(this) explicitly");
            }

            if (Tickets == null || Users == null || Permissions == null || Notifications == null || Audit == null)
                throw new InvalidOperationException("A required service is not registered: check ServiceRegistration.");

            _log.Info(LogLayer.Infrastructure, "TicketWorkflow.EnsureInjected",
                $"ITicketService → {Tickets.GetType().Name} {ServiceProbe.IdOf(Tickets)} · IUserService → {Users.GetType().Name} · IPermissionService → {Permissions.GetType().Name} · INotificationService → {Notifications.GetType().Name} {ServiceProbe.IdOf(Notifications)} · IAuditLogService → {Audit.GetType().Name} {ServiceProbe.IdOf(Audit)}");
        }

        private void BuildPresenter()
        {
            _presenter = new TicketWorkflowPresenter(Tickets, Users, Permissions, Notifications, Audit, _log);
            _log.Info(LogLayer.UI, "TicketWorkflow.BuildPresenter", "new TicketWorkflowPresenter(Tickets, Users, Permissions, Notifications, Audit, log) — the screen's decisions live there");
        }

        /// <summary>data → UI: the only place that fills the ticket grid.</summary>
        private async System.Threading.Tasks.Task RefreshGridAsync()
        {
            try
            {
                this.statusBanner.SetStatus("loading", StatusKind.Busy);
                _rows = await _presenter.LoadOpenTicketsAsync();

                this.gridTickets.Rows.Clear();
                foreach (var t in _rows)
                    this.gridTickets.Rows.Add(t.Id, t.Title, t.Priority.ToString(), t.Status.ToString(), t.HoursLogged, AssigneeName(t));

                this.labelSelected.Text = "Select a ticket";
                ShowAuditCount();
                this.statusBanner.SetStatus("ready", StatusKind.Success);
                _log.Info(LogLayer.UI, "TicketWorkflow.RefreshGrid", $"{_rows.Count} rows shown");
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.RefreshGrid", ex);
            }
        }

        private void FillOperators()
        {
            _operators = Users.AllOperators();
            _syncingOperator = true;
            this.comboOperator.Items.Clear();
            foreach (var o in _operators)
                this.comboOperator.Items.Add(o.ToString());
            this.comboOperator.SelectedIndex = IndexOfOperator(Users.Current.Id);
            _syncingOperator = false;
        }

        private void ShowProfile()
        {
            string name = Profile != null ? Profile.DisplayName : "unknown";
            this.labelProfile.Text = $"Profile: {name} · {Application.SessionCount} session(s)";
            this.buttonSwitchProfile.Text = Profile != null && Profile.Profile == ServiceProfile.Fake
                ? "Switch to production"
                : "Switch to fake";
            this.Text = $"TicketOps Console — Module 8 · Services & DI · profile: {name}";
        }

        private void ShowAuditCount()
        {
            this.labelAudit.Text = $"Audit trail: {Audit.Count} entries (Shared — same in every tab)";
        }

        /// <summary>Diagnostics: what the container hands out for each contract, and whether a second resolve is the same object.</summary>
        private void RefreshServicesGrid()
        {
            this.gridServices.Rows.Clear();
            foreach (var row in ServiceProbe.Probe(Profile))
                this.gridServices.Rows.Add(row.Service, row.Implementation, row.Lifetime, row.Instance, row.SecondResolve);
            this.gridServices.ClearSelection();
            _log.Info(LogLayer.Infrastructure, "ServiceProbe", "resolved every contract twice — Session/Shared return the same instance, Transient a new one");
        }

        #endregion

        #region UI → data and data → UI

        private int? SelectedTicketId()
        {
            var row = this.gridTickets.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _rows.Count)
                return null;
            return _rows[row.Index].Id;
        }

        private string AssigneeName(Ticket t)
        {
            if (!t.IsAssigned) return "—";
            var op = _operators.FirstOrDefault(o => o.Id == t.AssigneeId);
            return op != null ? op.Name : $"operator {t.AssigneeId}";
        }

        private int IndexOfOperator(int operatorId)
        {
            for (int i = 0; i < _operators.Count; i++)
                if (_operators[i].Id == operatorId)
                    return i;
            return -1;
        }

        private void ShowResult(WorkflowResult result)
        {
            if (result.Succeeded)
            {
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(result.Message, StatusKind.Success);
                _log.Info(LogLayer.UI, "TicketWorkflow.ShowResult", $"OK · {result.Message}");
                return;
            }

            // An expected "no": the presenter explained it in words the user may read.
            var kind = result.Outcome == WorkflowOutcome.Denied ? StatusKind.Error : StatusKind.Warning;
            this.statusBanner.ShowBanner(result.Message, kind);
            this.statusBanner.SetStatus(result.Outcome.ToString().ToLowerInvariant(), StatusKind.Warning);
            _log.Warn(LogLayer.UI, "TicketWorkflow.ShowResult", $"{result.Outcome} · {result.Message}");
        }

        /// <summary>Unexpected failure: details to the log, one safe sentence to the user.</summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log?.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message");
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Thin handlers → presenter

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            int? id = SelectedTicketId();
            if (id == null)
            {
                this.labelSelected.Text = "Select a ticket";
                return;
            }

            var t = _rows[this.gridTickets.CurrentRow.Index];
            this.labelSelected.Text = $"#{t.Id} · {t.Title} · {t.Status} · {AssigneeName(t)}";
        }

        private void comboOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncingOperator || this.comboOperator.SelectedIndex < 0)
                return;

            try
            {
                var op = _operators[this.comboOperator.SelectedIndex];
                _log.Info(LogLayer.UI, "TicketWorkflow.comboOperator_SelectedIndexChanged", $"→ IUserService.SignInAs({op.Id})");
                Users.SignInAs(op.Id);
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus($"signed in as {op.Name}", StatusKind.Normal);
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.comboOperator_SelectedIndexChanged", ex);
            }
        }

        /// <summary>Success path: read the screen, ask the presenter, show the result.</summary>
        private async void buttonClose_Click(object sender, EventArgs e)
        {
            try
            {
                int? id = SelectedTicketId();
                if (id == null)
                {
                    ShowResult(WorkflowResult.Invalid(Strings.SelectTicket));
                    return;
                }

                string reason = this.textReason.Text;                                          // UI → data
                _log.Info(LogLayer.UI, "TicketWorkflow.buttonClose_Click",
                    $"→ presenter.CloseAsync(#{id}, \"{reason}\") over the [Inject]ed ITicketService / IPermissionService / IAuditLogService / INotificationService");
                var result = await _presenter.CloseAsync(id.Value, reason);                   // the decision lives in the presenter, not in this handler
                ShowResult(result);                                                            // data → UI
                if (result.Succeeded)
                {
                    this.textReason.Text = "";
                    await RefreshGridAsync();
                }
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.buttonClose_Click", ex);
            }
        }

        private async void buttonAssign_Click(object sender, EventArgs e)
        {
            try
            {
                int? id = SelectedTicketId();
                if (id == null)
                {
                    ShowResult(WorkflowResult.Invalid(Strings.SelectTicket));
                    return;
                }

                _log.Info(LogLayer.UI, "TicketWorkflow.buttonAssign_Click", $"→ presenter.AssignToMeAsync(#{id})");
                var result = await _presenter.AssignToMeAsync(id.Value);
                ShowResult(result);
                if (result.Succeeded)
                    await RefreshGridAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.buttonAssign_Click", ex);
            }
        }

        private async void buttonRefresh_Click(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "TicketWorkflow.buttonRefresh_Click", "→ presenter.LoadOpenTicketsAsync()");
            await RefreshGridAsync();
        }

        /// <summary>
        /// Re-registers the other profile with AddOrReplaceService (application-wide), then asks the container to
        /// inject this Form again so its properties point at the new implementations, and rebuilds the presenter.
        /// The screen code that follows is exactly the same code that ran before the switch.
        /// </summary>
        private async void buttonSwitchProfile_Click(object sender, EventArgs e)
        {
            try
            {
                var target = Profile != null && Profile.Profile == ServiceProfile.Fake ? ServiceProfile.Production : ServiceProfile.Fake;
                _log.Info(LogLayer.UI, "TicketWorkflow.buttonSwitchProfile_Click", $"→ ServiceRegistration.Apply({target}) — AddOrReplaceService for every contract (application-wide)");

                Profile = ServiceRegistration.Apply(target, out bool changed);
                foreach (var entry in Profile.Entries)
                    _log.Info(LogLayer.Infrastructure, "ServiceRegistration", $"{entry.ServiceName} → {entry.ImplementationName} · {entry.Lifetime}{(changed ? "" : " (unchanged)")}");

                Application.Services.Inject(this);                                             // the properties still held the old profile's instances
                BuildPresenter();
                _log.Info(LogLayer.Infrastructure, "TicketWorkflow.buttonSwitchProfile_Click",
                    $"re-injected: ITicketService → {Tickets.GetType().Name} {ServiceProbe.IdOf(Tickets)} · IAuditLogService → {Audit.GetType().Name} {ServiceProbe.IdOf(Audit)} · ILog {(ReferenceEquals(Log, _log) ? "unchanged" : "CHANGED (trace stays on the original)")}");

                FillOperators();
                ShowProfile();
                RefreshServicesGrid();
                await RefreshGridAsync();
                this.statusBanner.SetStatus($"profile → {Profile.DisplayName}", StatusKind.Success);
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.buttonSwitchProfile_Click", ex);
            }
        }

        #endregion

        #region Bottom bar: progress, failures, DI failure, outage and recovery

        /// <summary>Progress path: one presenter test per tick, each against fresh fakes — no container, no browser in the loop.</summary>
        private void buttonRunTests_Click(object sender, EventArgs e)
        {
            if (this.timerTests.Enabled)
                return;

            _nextTest = 0;
            _testsFailed = 0;
            this.progressTests.Maximum = _tests.Count;
            this.progressTests.Value = 0;
            this.progressTests.Visible = true;
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus($"running presenter tests 0/{_tests.Count}", StatusKind.Busy);
            _log.Info(LogLayer.UI, "TicketWorkflow.buttonRunTests_Click", $"{_tests.Count} tests: new TicketWorkflowPresenter(fakes…) per test — the Form's injected services are not involved");
            this.timerTests.Start();
        }

        private async void timerTests_Tick(object sender, EventArgs e)
        {
            try
            {
                var result = await _tests.RunAsync(_nextTest);
                if (!result.Passed)
                    _testsFailed++;

                _nextTest++;
                this.progressTests.Value = _nextTest;
                this.statusBanner.SetStatus($"running presenter tests {_nextTest}/{_tests.Count}", StatusKind.Busy);

                if (_nextTest >= _tests.Count)
                {
                    this.timerTests.Stop();
                    this.progressTests.Visible = false;
                    if (_testsFailed == 0)
                    {
                        this.statusBanner.SetStatus($"{_tests.Count}/{_tests.Count} tests passed", StatusKind.Success);
                        this.statusBanner.ShowBanner(Strings.TestsPassed, StatusKind.Success);
                    }
                    else
                    {
                        this.statusBanner.SetStatus($"{_testsFailed} of {_tests.Count} tests failed", StatusKind.Error);
                        this.statusBanner.ShowBanner(Strings.TestsFailed, StatusKind.Error);
                    }
                    _log.Info(LogLayer.UI, "TicketWorkflow.timerTests_Tick", $"done: {_tests.Count - _testsFailed} passed, {_testsFailed} failed");
                }
            }
            catch (Exception ex)
            {
                this.timerTests.Stop();
                this.progressTests.Visible = false;
                ReportFailure("TicketWorkflow.timerTests_Tick", ex);
            }
        }

        /// <summary>Failure path 1: permission. Sign in as the Viewer, then ask to close #1041 — the presenter says no before any data changes.</summary>
        private async void buttonCloseAsViewer_Click(object sender, EventArgs e)
        {
            try
            {
                var viewer = _operators.First(o => o.Role == OperatorRole.Viewer);
                _log.Info(LogLayer.UI, "TicketWorkflow.buttonCloseAsViewer_Click", $"→ IUserService.SignInAs({viewer.Id}) then presenter.CloseAsync(#1041)");
                Users.SignInAs(viewer.Id);
                _syncingOperator = true;
                this.comboOperator.SelectedIndex = IndexOfOperator(viewer.Id);
                _syncingOperator = false;

                ShowResult(await _presenter.CloseAsync(1041, "Trying anyway"));
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.buttonCloseAsViewer_Click", ex);
            }
        }

        /// <summary>Failure path 2: the domain rule. #1042 belongs to Dana but has no hours logged, so Ticket.CanClose says no.</summary>
        private async void buttonCloseNoHours_Click(object sender, EventArgs e)
        {
            try
            {
                var dana = _operators.First(o => o.Id == SeedData.DefaultOperatorId);
                Users.SignInAs(dana.Id);
                _syncingOperator = true;
                this.comboOperator.SelectedIndex = IndexOfOperator(dana.Id);
                _syncingOperator = false;

                _log.Info(LogLayer.UI, "TicketWorkflow.buttonCloseNoHours_Click", "→ presenter.CloseAsync(#1042, \"Closing early\") as Dana");
                ShowResult(await _presenter.CloseAsync(1042, "Closing early"));
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.buttonCloseNoHours_Click", ex);
            }
        }

        /// <summary>
        /// Failure path 3: dependency injection. Nobody registered an IExportService; GetService returns null and
        /// HasService says false. The screen degrades with a clear message instead of a NullReferenceException.
        /// </summary>
        private void buttonMissingService_Click(object sender, EventArgs e)
        {
            try
            {
                _log.Info(LogLayer.UI, "TicketWorkflow.buttonMissingService_Click", "→ Application.Services.GetService<IExportService>()");
                bool registered = Application.Services.HasService<IExportService>();
                var export = Application.Services.GetService<IExportService>();
                if (!registered || export == null)
                {
                    _log.Warn(LogLayer.Infrastructure, "Application.Services",
                        $"IExportService is not registered (HasService = {registered}, GetService = null) — an [Inject(Required = true)] property of this type would fail at injection time; an optional [Inject] stays null");
                    this.statusBanner.ShowBanner(Strings.ServiceUnavailable, StatusKind.Warning);
                    this.statusBanner.SetStatus("feature unavailable", StatusKind.Warning);
                    return;
                }

                export.Export(_rows);
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.buttonMissingService_Click", ex);
            }
        }

        /// <summary>Error path + recovery: trip this session's outage switch, then refresh through the same presenter.</summary>
        private async void buttonOutage_Click(object sender, EventArgs e)
        {
            if (DataStore == null)
                return;

            DataStore.SimulateOutage = !DataStore.SimulateOutage;
            this.buttonOutage.Text = DataStore.SimulateOutage ? "Recover the data store" : "Simulate data outage";
            _log.Info(LogLayer.UI, "TicketWorkflow.buttonOutage_Click",
                DataStore.SimulateOutage ? "outage ON (this session's DataStoreHealth) → refresh (expect ✖ in DATA, safe message in UI)" : "outage OFF → refresh (recovery)");
            await RefreshGridAsync();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.tracePanel.ClearTrace();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("ready", StatusKind.Normal);
        }

        #endregion
    }

    /// <summary>
    /// A contract no profile registers — the DI failure path. Resolving it returns null; the screen must cope.
    /// </summary>
    public interface IExportService
    {
        void Export(IReadOnlyList<Ticket> tickets);
    }
}
