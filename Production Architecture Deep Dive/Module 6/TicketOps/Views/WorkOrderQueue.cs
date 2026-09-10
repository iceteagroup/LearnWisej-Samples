using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Data;
using TicketOps.Dialogs;
using TicketOps.Diagnostics;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Work Orders — the Module 6 screen.
    ///
    /// Left card:   the work-order queue and the selected order (display only), plus "Approve / Reject…",
    ///              which opens the modal <see cref="ApprovalDialog"/> and awaits it.
    /// Right card:  the Diagnostics activity trace: UI → dialog → SVC → DOMAIN → DATA (tx) and back.
    /// Bottom bar:  the progress path (run the six result-handling tests), two failure paths (a forged
    ///              rejection without comments, deciding an already-decided order), the error path
    ///              (simulated outage during the commit) with its recovery, and Clear trace.
    ///
    /// The handler that matters is <see cref="buttonReview_Click"/>: open the dialog, await it, and if — and
    /// only if — the user confirmed, hand the typed result to IApprovalService. It never reads a control of
    /// the dialog and never mutates the work order itself.
    /// </summary>
    public partial class WorkOrderQueue : Form
    {
        private static readonly CultureInfo Money = CultureInfo.GetCultureInfo("en-US");

        private readonly IApprovalService _approvals;
        private readonly InMemoryWorkOrderRepository _repository;   // only for the lab's outage switch
        private readonly ILog _log;

        private IReadOnlyList<WorkOrder> _rows = new List<WorkOrder>();
        private IReadOnlyList<ResultHandlingTestCase> _testCases = new List<ResultHandlingTestCase>();
        private int _testIndex;
        private int _testsPassed;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public WorkOrderQueue() : this(null, null, new ActivityLog())
        {
        }

        public WorkOrderQueue(IApprovalService approvals, InMemoryWorkOrderRepository repository, ILog log)
        {
            InitializeComponent();

            _approvals = approvals;
            _repository = repository;
            _log = log;

            if (log is ActivityLog activityLog)
                this.tracePanel.Attach(activityLog);

            this.tracePanel.Title = "Activity trace · UI → Dialog → Service → Data (tx)";
        }

        #region Screen lifecycle

        private async void WorkOrderQueue_Load(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "WorkOrderQueue.Load", "screen shown → IApprovalService.GetQueueAsync()");
            await RefreshGridAsync(2002);
        }

        /// <summary>data → UI: the only place that fills the grid. Re-selects <paramref name="selectId"/> when given.</summary>
        private async Task RefreshGridAsync(int? selectId)
        {
            try
            {
                this.statusBanner.SetStatus("loading", StatusKind.Busy);
                _rows = await _approvals.GetQueueAsync();

                this.gridWorkOrders.Rows.Clear();
                int pending = 0;
                int selectIndex = -1;
                for (int i = 0; i < _rows.Count; i++)
                {
                    var w = _rows[i];
                    if (w.IsPending) pending++;
                    if (selectId.HasValue && w.Id == selectId.Value) selectIndex = i;
                    this.gridWorkOrders.Rows.Add(w.Number, w.Title, w.Requester, w.Amount.ToString("C2", Money), w.Status.ToString());
                }

                this.labelCount.Text = $"{pending} pending of {_rows.Count} work orders";
                if (selectIndex >= 0)
                    this.gridWorkOrders.CurrentCell = this.gridWorkOrders.Rows[selectIndex].Cells[0];
                ShowSelected();

                this.statusBanner.SetStatus("ready", StatusKind.Success);
                _log.Info(LogLayer.UI, "WorkOrderQueue.RefreshGrid", $"{_rows.Count} rows shown ({pending} pending)");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrderQueue.RefreshGrid", ex);
            }
        }

        #endregion

        #region Selection (display only) and result → UI

        private WorkOrder SelectedWorkOrder()
        {
            var row = this.gridWorkOrders.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _rows.Count)
                return null;
            return _rows[row.Index];
        }

        private void gridWorkOrders_SelectionChanged(object sender, EventArgs e)
        {
            ShowSelected();
            var wo = SelectedWorkOrder();
            if (wo != null)
                SetWorkflowStatus($"Work order {wo.Id} selected.");
        }

        private void ShowSelected()
        {
            var wo = SelectedWorkOrder();
            if (wo == null)
            {
                this.labelSelected.Text = "No work order selected";
                this.labelSelectedDetail.Text = "";
                this.labelDecision.Text = "";
                this.buttonReview.Enabled = false;
                return;
            }

            this.labelSelected.Text = $"{wo.Number} · {wo.Title}";
            this.labelSelectedDetail.Text = $"Requester {wo.Requester} · Cost {wo.Amount.ToString("C2", Money)} · Requested {wo.RequestedAt:MMM d} · Approver you ({AppComposition.CurrentUser})";
            this.labelDecision.Text = wo.IsPending
                ? "Status: Pending approval"
                : $"Status: {wo.Status} by {wo.DecidedBy} on {wo.DecidedAtUtc:yyyy-MM-dd HH:mm} UTC · \"{wo.DecisionComments}\"";
            this.labelDecision.ForeColor = wo.IsPending
                ? System.Drawing.Color.FromArgb(185, 119, 14)
                : StatusBanner.ColorFor(wo.Status == WorkOrderStatus.Approved ? StatusKind.Success : StatusKind.Error);
            this.buttonReview.Enabled = wo.IsPending;
        }

        private void SetWorkflowStatus(string text)
        {
            this.labelWorkflow.Text = text;
        }

        private void ShowResult<T>(OperationResult<T> result)
        {
            if (result.Succeeded)
            {
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(result.Message, StatusKind.Success);
                _log.Info(LogLayer.UI, "WorkOrderQueue.ShowResult", $"OK · {result.Message}");
            }
            else
            {
                // Expected outcome: the service explained it in words the user may read.
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("not applied", StatusKind.Warning);
                _log.Warn(LogLayer.UI, "WorkOrderQueue.ShowResult", $"FAIL · {result.Message}");
            }
        }

        /// <summary>
        /// Unexpected failure: details go to the log (with the exception type and message),
        /// the user sees one generic sentence. Nothing internal leaks through the banner.
        /// </summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message");
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region The modal workflow: open the dialog, await it, apply only on a confirmed result

        /// <summary>
        /// Success path (and, depending on what the user does in the dialog, the cancel, ✕ and validation
        /// paths). Read top to bottom: open the dialog, wait, and if the user confirmed hand the confirmed
        /// intent to the service. No control of the dialog is read here — only DialogResult and Result.
        /// </summary>
        private async void buttonReview_Click(object sender, EventArgs e)
        {
            try
            {
                var workOrder = SelectedWorkOrder();
                if (workOrder == null)
                {
                    ShowResult(OperationResult<WorkOrder>.Fail(Strings.SelectWorkOrder));
                    return;
                }

                _log.Info(LogLayer.UI, "WorkOrderQueue.buttonReview_Click", $"→ new ApprovalDialog({workOrder.Number}) · await ShowDialogAsync() — the work order is not touched while the dialog is open");
                SetWorkflowStatus(string.Format(Strings.DialogOpen, workOrder.Id));
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus("deciding", StatusKind.Busy);

                using (var dialog = new ApprovalDialog(workOrder, _log))
                {
                    DialogResult outcome = await dialog.ShowDialogAsync();      // modal; the handler waits here
                    ApprovalDialogResult result = dialog.Result;                // typed result — never dialog.textComments.Text

                    // The one gate: did the user actually commit?
                    if (outcome != DialogResult.OK || !result.Confirmed)
                    {
                        _log.Info(LogLayer.UI, "WorkOrderQueue.buttonReview_Click", $"DialogResult.{outcome} · Result {result} → return (no service call, no mutation)");
                        SetWorkflowStatus(string.Format(Strings.DialogCancelled, workOrder.Id));
                        this.statusBanner.SetStatus("unchanged", StatusKind.Normal);
                        return;
                    }

                    // Dialog gathered intent; the SERVICE executes the transaction.
                    _log.Info(LogLayer.UI, "WorkOrderQueue.buttonReview_Click", $"DialogResult.OK · Result {result} → IApprovalService.ApplyAsync(#{workOrder.Id}, result)");
                    var applied = await _approvals.ApplyAsync(workOrder.Id, result);
                    ShowResult(applied);
                    SetWorkflowStatus(applied.Succeeded
                        ? $"{applied.Message} Status, audit and notification committed together."
                        : $"{applied.Message} Work order {workOrder.Id} unchanged.");
                }

                await RefreshGridAsync(workOrder.Id);
            }
            catch (Exception ex)
            {
                // The repository threw during the commit: nothing was applied; the user reads the safe message.
                ReportFailure("WorkOrderQueue.buttonReview_Click", ex);
                SetWorkflowStatus("Commit failed — nothing was applied. Recover the data store and confirm again.");
                await RefreshGridAsync(SelectedWorkOrder()?.Id);
            }
        }

        private async void buttonRefresh_Click(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "WorkOrderQueue.buttonRefresh_Click", "→ IApprovalService.GetQueueAsync()");
            await RefreshGridAsync(SelectedWorkOrder()?.Id);
        }

        #endregion

        #region Bottom bar: progress (tests), failures, outage and recovery

        /// <summary>Progress path: run the six result-handling tests, one per Timer tick, each on its own fixture.</summary>
        private void buttonTests_Click(object sender, EventArgs e)
        {
            if (this.timerTests.Enabled)
                return;

            _testCases = new ResultHandlingTests(_log).Cases;
            _testIndex = 0;
            _testsPassed = 0;
            this.progressTests.Maximum = _testCases.Count;
            this.progressTests.Value = 0;
            this.progressTests.Visible = true;
            this.labelTests.Text = "";
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus($"running tests 0/{_testCases.Count}", StatusKind.Busy);
            _log.Info(LogLayer.UI, "WorkOrderQueue.buttonTests_Click", $"{_testCases.Count} result-handling tests, one per tick, each on a fresh repository + ApprovalService (no dialog, no browser needed)");
            this.timerTests.Start();
        }

        private async void timerTests_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_testIndex >= _testCases.Count)
                {
                    this.timerTests.Stop();
                    this.progressTests.Visible = false;
                    bool allPassed = _testsPassed == _testCases.Count;
                    this.labelTests.Text = $"{_testsPassed}/{_testCases.Count} passed — see docs/TestCases.md";
                    this.statusBanner.SetStatus($"tests {_testsPassed}/{_testCases.Count} passed", allPassed ? StatusKind.Success : StatusKind.Warning);
                    _log.Info(LogLayer.UI, "WorkOrderQueue.timerTests_Tick", $"done — {_testsPassed}/{_testCases.Count} passed; the session queue was never touched");
                    return;
                }

                var testCase = _testCases[_testIndex];
                _log.Info(LogLayer.UI, $"Test {testCase.Id}", $"{testCase.Name} — expect: {testCase.Expectation}");
                var outcome = await testCase.Run();
                if (outcome.Passed)
                {
                    _testsPassed++;
                    _log.Info(LogLayer.UI, $"Test {testCase.Id}", $"PASS · {outcome.Detail}");
                }
                else
                {
                    _log.Warn(LogLayer.UI, $"Test {testCase.Id}", $"FAIL · {outcome.Detail}");
                }

                _testIndex++;
                this.progressTests.Value = _testIndex;
                this.labelTests.Text = $"{testCase.Id} {(outcome.Passed ? "PASS" : "FAIL")} · {testCase.Name}";
                this.statusBanner.SetStatus($"running tests {_testIndex}/{_testCases.Count}", StatusKind.Busy);
            }
            catch (Exception ex)
            {
                this.timerTests.Stop();
                this.progressTests.Visible = false;
                ReportFailure("WorkOrderQueue.timerTests_Tick", ex);
            }
        }

        /// <summary>
        /// Failure path 1: a "confirmed" rejection without comments that skipped the dialog (a bug, a script,
        /// a second screen). The service re-checks the rule and refuses; nothing is written.
        /// </summary>
        private async void buttonRejectNoComments_Click(object sender, EventArgs e)
        {
            try
            {
                var workOrder = SelectedWorkOrder();
                if (workOrder == null)
                {
                    ShowResult(OperationResult<WorkOrder>.Fail(Strings.SelectWorkOrder));
                    return;
                }

                var forged = ApprovalDialogResult.Confirm(ApprovalAction.Reject, "");
                _log.Info(LogLayer.UI, "WorkOrderQueue.buttonRejectNoComments_Click", $"forged result {forged} (bypassing the dialog) → IApprovalService.ApplyAsync(#{workOrder.Id}, result)");
                var applied = await _approvals.ApplyAsync(workOrder.Id, forged);
                ShowResult(applied);
                SetWorkflowStatus(Strings.ValidationBlocked);
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrderQueue.buttonRejectNoComments_Click", ex);
            }
        }

        /// <summary>Failure path 2: the domain rule. WO-2001 is already approved; WorkOrder.CanDecide says no.</summary>
        private async void buttonDecideAgain_Click(object sender, EventArgs e)
        {
            try
            {
                const int alreadyDecided = 2001;
                var result = ApprovalDialogResult.Confirm(ApprovalAction.Reject, "Trying to flip a closed decision.");
                _log.Info(LogLayer.UI, "WorkOrderQueue.buttonDecideAgain_Click", $"{result} → IApprovalService.ApplyAsync(#{alreadyDecided}, result) — the rule lives in the domain, not here");
                var applied = await _approvals.ApplyAsync(alreadyDecided, result);
                ShowResult(applied);
                SetWorkflowStatus($"Rule refused — work order {alreadyDecided} unchanged.");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrderQueue.buttonDecideAgain_Click", ex);
            }
        }

        /// <summary>Error path + recovery: toggle the outage; the next confirmed decision fails at the commit and rolls back.</summary>
        private void buttonOutage_Click(object sender, EventArgs e)
        {
            if (_repository == null)
                return;

            _repository.SimulateOutage = !_repository.SimulateOutage;
            this.buttonOutage.Text = _repository.SimulateOutage ? "Recover the data store" : "Simulate data outage";
            if (_repository.SimulateOutage)
            {
                _log.Warn(LogLayer.UI, "WorkOrderQueue.buttonOutage_Click", "outage ON → Approve / Reject… and Confirm: expect ✖ in DATA at the audit INSERT, a rollback, and the safe message in the UI");
                this.statusBanner.ShowBanner("Data store outage simulated — confirm a decision to see the rollback.", StatusKind.Warning);
                this.statusBanner.SetStatus("outage", StatusKind.Warning);
            }
            else
            {
                _log.Info(LogLayer.UI, "WorkOrderQueue.buttonOutage_Click", "outage OFF → the store answers again; confirm the same decision to see it commit (recovery)");
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus("ready", StatusKind.Success);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.tracePanel.ClearTrace();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("ready", StatusKind.Normal);
        }

        #endregion
    }
}
