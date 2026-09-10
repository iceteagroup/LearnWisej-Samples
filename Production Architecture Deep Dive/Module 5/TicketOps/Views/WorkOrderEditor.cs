using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using TicketOps.Validation;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Work Orders — the Module 5 screen.
    ///
    /// Left card:   the work order grid and the editor. Field errors are painted by an ErrorProvider next
    ///              to the offending control; the summary panel above the bottom bar lists every problem.
    /// Right card:  the Diagnostics activity trace: every save travels UI → SVC → DOMAIN → DATA and back.
    /// Bottom bar:  the progress path (run the documented test cases), three failure paths (empty title,
    ///              hours out of range, editing a closed order), the UI-bypass path (a crafted command that
    ///              never touched the form), the error path (write outage during persist) and its recovery.
    ///
    /// The Save handler reads like the video's: build the command, pre-check it with the same validator the
    /// server uses, hand it to the service, show the result. The words "transaction", "sql01" and "role"
    /// do not decide anything in this file — they are only shown.
    /// </summary>
    public partial class WorkOrderEditor : Form
    {
        private readonly IWorkOrderService _workOrders;
        private readonly WorkOrderValidator _validator;
        private readonly SessionContext _session;
        private readonly InMemoryWorkOrderRepository _repository;   // only for the lab's outage switch
        private readonly ILog _log;

        private IReadOnlyList<WorkOrder> _rows = new List<WorkOrder>();
        private WorkOrderStatus _fromStatus = WorkOrderStatus.New;   // the status the editor was opened with
        private bool _loadingGrid;

        private readonly Queue<ValidationTestCase> _testQueue = new Queue<ValidationTestCase>();
        private ValidationTestRunner _testRunner;
        private int _testsRun, _testsPassed, _testsTotal;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public WorkOrderEditor() : this(null, new WorkOrderValidator(), new SessionContext(), null, new ActivityLog())
        {
        }

        public WorkOrderEditor(IWorkOrderService workOrders, WorkOrderValidator validator, SessionContext session,
                               InMemoryWorkOrderRepository repository, ILog log)
        {
            InitializeComponent();

            _workOrders = workOrders;
            _validator = validator;
            _session = session;
            _repository = repository;
            _log = log;

            if (log is ActivityLog activityLog)
                this.tracePanel.Attach(activityLog);

            this.comboStatus.Items.AddRange(Enum.GetNames(typeof(WorkOrderStatus)));
            this.comboPriority.Items.AddRange(Enum.GetNames(typeof(WorkOrderPriority)));
            this.comboRole.Items.AddRange(Enum.GetNames(typeof(UserRole)));
            this.comboRole.SelectedIndex = (int)_session.Role;
            this.comboStatus.SelectedIndex = 0;
            this.comboPriority.SelectedIndex = 1;
            this.dateDue.Value = DateTime.Today.AddDays(7);
        }

        #region Screen lifecycle

        private async void WorkOrderEditor_Load(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "WorkOrderEditor.Load", "screen shown → IWorkOrderService.GetWorkOrdersAsync()");
            await RefreshGridAsync();
            SelectRow(2002);                                          // the order the video edits
        }

        /// <summary>data → UI: the only place that fills the grid. Keeps the current selection; never touches the editor.</summary>
        private async Task RefreshGridAsync()
        {
            try
            {
                int? keep = SelectedWorkOrderId();
                this.statusBanner.SetStatus("loading", StatusKind.Busy);
                _rows = await _workOrders.GetWorkOrdersAsync();

                _loadingGrid = true;
                this.gridOrders.Rows.Clear();
                foreach (var o in _rows)
                    this.gridOrders.Rows.Add(o.Id, o.Title, o.AssigneeId, o.Status.ToString(),
                        o.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        o.EstimatedCost.ToString("N2", CultureInfo.InvariantCulture),
                        o.EstimatedHours.ToString("0.#", CultureInfo.InvariantCulture),
                        "v" + o.RowVersion.ToString(CultureInfo.InvariantCulture));
                if (!keep.HasValue || !SelectRowCore(keep.Value))
                {
                    // No row was selected before (New work order): make sure the grid does not pick one for us.
                    this.gridOrders.ClearSelection();
                    this.gridOrders.CurrentCell = null;
                }
                _loadingGrid = false;

                this.labelCount.Text = $"{_rows.Count} work orders";
                this.statusBanner.SetStatus("ready", StatusKind.Success);
                _log.Info(LogLayer.UI, "WorkOrderEditor.RefreshGrid", $"{_rows.Count} rows shown");
            }
            catch (Exception ex)
            {
                _loadingGrid = false;
                ReportFailure("WorkOrderEditor.RefreshGrid", ex, Strings.ActionFailed);
            }
        }

        #endregion

        #region UI → command (read the editor) and result → UI (show it)

        /// <summary>Step 1–3 of the pipeline: collect + map. No control reference leaks past this method.</summary>
        private SaveWorkOrderCommand BuildSaveCommandFromEditor()
        {
            return new SaveWorkOrderCommand
            {
                Id = SelectedWorkOrderId(),
                Title = this.textTitle.Text,
                AssigneeId = this.textAssignee.Text,
                Priority = (WorkOrderPriority)Math.Max(0, this.comboPriority.SelectedIndex),
                DueDate = this.dateDue.Value,
                EstimatedCost = this.numericCost.Value,
                EstimatedHours = (double)this.numericHours.Value,
                FromStatus = _fromStatus,
                ToStatus = (WorkOrderStatus)Math.Max(0, this.comboStatus.SelectedIndex)
            };
        }

        private int? SelectedWorkOrderId()
        {
            var row = this.gridOrders.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _rows.Count)
                return null;
            return _rows[row.Index].Id;
        }

        private void FillEditor(WorkOrder o)
        {
            ClearErrors();
            this.textTitle.Text = o.Title;
            this.textAssignee.Text = o.AssigneeId;
            this.comboPriority.SelectedIndex = (int)o.Priority;
            this.dateDue.Value = o.DueDate;
            this.numericCost.Value = o.EstimatedCost;
            this.numericHours.Value = (decimal)o.EstimatedHours;
            this.comboStatus.SelectedIndex = (int)o.Status;
            _fromStatus = o.Status;
            this.labelSelected.Text = $"Editing #{o.Id} · {o.Status} · v{o.RowVersion}";
        }

        /// <summary>
        /// Translate a ValidationResult into on-screen feedback: a glyph + tooltip beside each failing
        /// control (ErrorProvider) and the summary panel listing every problem, so cross-field and
        /// workflow errors are never hidden behind a single field. Display only — nothing is decided here.
        /// </summary>
        private void ShowValidation(ValidationResult result)
        {
            ClearErrors();

            foreach (var error in result.FieldErrors)
            {
                var control = ControlForField(error.Field);
                if (control != null)
                    this.errorProvider.SetError(control, error.Message);
            }

            var lines = result.Errors
                .Select(e => e.IsFieldError ? $"• {CaptionForField(e.Field)} — {e.Message}" : $"• {e.Message}")
                .ToList();
            int shown = Math.Min(lines.Count, 3);
            string more = lines.Count > shown ? $"\n• … and {lines.Count - shown} more" : "";

            this.labelSummaryTitle.Text = result.Count == 1 ? "1 problem needs attention" : $"{result.Count} problems need attention";
            this.labelSummaryList.Text = string.Join("\n", lines.Take(shown)) + more;
            this.panelSummary.Visible = true;

            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus(result.Count == 1 ? "1 problem found — nothing was saved" : $"{result.Count} problems found — nothing was saved", StatusKind.Warning);
        }

        private void ClearErrors()
        {
            this.errorProvider.Clear();
            this.panelSummary.Visible = false;
        }

        private Control ControlForField(string field)
        {
            switch (field)
            {
                case "Title": return this.textTitle;
                case "AssigneeId": return this.textAssignee;
                case "DueDate": return this.dateDue;
                case "EstimatedCost": return this.numericCost;
                case "EstimatedHours": return this.numericHours;
                default: return null;
            }
        }

        private static string CaptionForField(string field)
        {
            switch (field)
            {
                case "AssigneeId": return "Assignee";
                case "DueDate": return "Due date";
                case "EstimatedCost": return "Cost";
                case "EstimatedHours": return "Hours";
                default: return field;
            }
        }

        private void ShowSaveResult(SaveResult result)
        {
            if (result.Succeeded)
            {
                ClearErrors();
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(result.Message, StatusKind.Success);
                _log.Info(LogLayer.UI, "WorkOrderEditor.ShowSaveResult", $"OK · {result.Message}");
            }
            else
            {
                // Expected outcome: the pipeline explained it in words the user may read.
                ShowValidation(result.Errors);
                _log.Warn(LogLayer.UI, "WorkOrderEditor.ShowSaveResult", $"INVALID at {result.Stage} · {result.Errors} → glyphs + summary panel");
            }
        }

        /// <summary>
        /// Unexpected failure: details are already in the log (DATA and SVC lines); here we add the handler's
        /// line and show ONE safe sentence. The editor keeps the user's edits — nothing is reset.
        /// </summary>
        private void ReportFailure(string source, Exception ex, string safeMessage)
        {
            _log.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message, edits stay on screen");
            this.statusBanner.ShowBanner("✖ " + safeMessage, StatusKind.Error);
            this.statusBanner.SetStatus(safeMessage == Strings.SaveFailed ? Strings.SaveFailedStatus : "failed", StatusKind.Error);
            AlertBox.Show(safeMessage, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>After a failed persist: re-read the store and prove the row did not change (no partial save).</summary>
        private async Task VerifyNothingPartialAsync(int? id)
        {
            var before = id.HasValue ? _rows.FirstOrDefault(o => o.Id == id.Value) : null;
            await RefreshGridAsync();
            var after = id.HasValue ? _rows.FirstOrDefault(o => o.Id == id.Value) : null;

            if (before == null || after == null)
            {
                _log.Info(LogLayer.UI, "WorkOrderEditor.VerifyNothingPartial", $"re-read: {_rows.Count} rows, no new row appeared — nothing partial");
                return;
            }

            bool unchanged = after.RowVersion == before.RowVersion && after.Title == before.Title
                          && after.Status == before.Status && after.EstimatedCost == before.EstimatedCost;
            _log.Info(LogLayer.UI, "WorkOrderEditor.VerifyNothingPartial",
                $"re-read #{after.Id}: v{after.RowVersion} \"{after.Title}\" · {after.Status} · ${after.EstimatedCost:N0} — {(unchanged ? "unchanged, nothing partial" : "CHANGED (this would be a bug)")}");
        }

        #endregion

        #region Thin handlers

        private void gridOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (_loadingGrid)
                return;

            int? id = SelectedWorkOrderId();
            if (id == null)
            {
                this.labelSelected.Text = "New work order";
                return;
            }

            FillEditor(_rows[this.gridOrders.CurrentRow.Index]);
        }

        private void comboRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboRole.SelectedIndex < 0)
                return;

            _session.Role = (UserRole)this.comboRole.SelectedIndex;
            _log.Info(LogLayer.Session, "SessionContext", $"role → {_session.Role} (server-side; the command never carries it)");
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            this.gridOrders.ClearSelection();
            this.gridOrders.CurrentCell = null;
            ClearErrors();
            this.textTitle.Text = "";
            this.textAssignee.Text = "";
            this.comboPriority.SelectedIndex = 1;
            this.dateDue.Value = DateTime.Today.AddDays(7);
            this.numericCost.Value = 0;
            this.numericHours.Value = 0;
            this.comboStatus.SelectedIndex = (int)WorkOrderStatus.New;
            _fromStatus = WorkOrderStatus.New;
            this.labelSelected.Text = "New work order";
            this.textTitle.Focus();
            _log.Info(LogLayer.UI, "WorkOrderEditor.buttonNew_Click", "editor cleared (display only, no service call)");
        }

        /// <summary>
        /// The video's handler. Collect → map → UX pre-check with the shared validator → the service
        /// (which re-validates, applies rules and role, persists atomically) → show the result.
        /// </summary>
        private async void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                var command = BuildSaveCommandFromEditor();                                  // UI → command; no control leaks past this line
                _log.Info(LogLayer.UI, "WorkOrderEditor.buttonSave_Click", $"command built {command}");

                var preCheck = _validator.Validate(command);                                // UX: same rules, instant feedback, decides nothing
                if (!preCheck.IsValid)
                {
                    ShowValidation(preCheck);                                                // ErrorProvider glyphs + summary panel
                    _log.Warn(LogLayer.UI, "WorkOrderEditor.buttonSave_Click", $"pre-check: {preCheck} — nothing sent, nothing saved");
                    return;
                }

                _log.Info(LogLayer.UI, "WorkOrderEditor.buttonSave_Click", "pre-check passed → IWorkOrderService.SaveAsync (the server re-validates; it is the gate)");
                var result = await _workOrders.SaveAsync(command, _session);                // validate → rules → persist → confirm
                ShowSaveResult(result);                                                      // result → UI
                if (result.Succeeded)
                {
                    await RefreshGridAsync();
                    SelectRow(result.WorkOrder.Id);                                          // the saved row, with its new version
                }
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrderEditor.buttonSave_Click", ex, Strings.SaveFailed); // real exception → log; safe message → user
                await VerifyNothingPartialAsync(SelectedWorkOrderId());
            }
        }

        /// <summary>Reload from the store and discard the editor's unsaved values (re-fills from the selected row).</summary>
        private async void buttonRefresh_Click(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "WorkOrderEditor.buttonRefresh_Click", "→ IWorkOrderService.GetWorkOrdersAsync() — editor reloads the stored values");
            await RefreshGridAsync();
            int? id = SelectedWorkOrderId();
            if (id.HasValue)
                SelectRow(id.Value);
            else
                ClearErrors();
        }

        #endregion

        #region Bottom bar: test cases, failures, bypass, outage and recovery

        /// <summary>
        /// Shared by the bottom-bar buttons: the command goes straight to the service, skipping the editor's
        /// pre-check the way an import job or a replayed request would — which is exactly why the server
        /// must run the same rules again.
        /// </summary>
        private async Task SubmitWithoutPreCheckAsync(SaveWorkOrderCommand command, string source)
        {
            try
            {
                _log.Info(LogLayer.UI, source, $"pre-check skipped (like an import or a replayed request) → IWorkOrderService.SaveAsync {command}");
                var result = await _workOrders.SaveAsync(command, _session);
                ShowSaveResult(result);
                if (result.Succeeded)
                {
                    await RefreshGridAsync();
                    SelectRow(result.WorkOrder.Id);                                          // the saved row, with its new version
                }
            }
            catch (Exception ex)
            {
                ReportFailure(source, ex, Strings.SaveFailed);
                await VerifyNothingPartialAsync(command.Id);
            }
        }

        /// <summary>Progress path: a Timer runs one documented test case per tick through the real validator and rules.</summary>
        private void buttonRunTests_Click(object sender, EventArgs e)
        {
            if (this.timerTests.Enabled)
                return;

            _testQueue.Clear();
            foreach (var tc in ValidationTestCases.All())
                _testQueue.Enqueue(tc);
            _testsTotal = _testQueue.Count;
            _testsRun = 0;
            _testsPassed = 0;
            _testRunner = new ValidationTestRunner(_log);

            ClearErrors();
            this.progressTests.Maximum = _testsTotal;
            this.progressTests.Value = 0;
            this.progressTests.Visible = true;
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus($"running test cases 0/{_testsTotal}", StatusKind.Busy);
            _log.Info(LogLayer.UI, "WorkOrderEditor.buttonRunTests_Click", $"{_testsTotal} cases from docs/TestCases.md — no form involved, the Timer only paces them");
            this.timerTests.Start();
        }

        private void timerTests_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_testQueue.Count > 0)
                {
                    var outcome = _testRunner.Run(_testQueue.Dequeue());
                    _testsRun++;
                    if (outcome.Passed)
                        _testsPassed++;

                    this.progressTests.Value = _testsRun;
                    this.statusBanner.SetStatus($"running {outcome.Case.Id} · {_testsPassed}/{_testsRun} passed", StatusKind.Busy);
                }

                if (_testQueue.Count == 0)
                {
                    this.timerTests.Stop();
                    this.progressTests.Visible = false;
                    bool allGreen = _testsPassed == _testsTotal;
                    this.statusBanner.SetStatus($"{_testsPassed}/{_testsTotal} test cases passed", allGreen ? StatusKind.Success : StatusKind.Warning);
                    _log.Info(LogLayer.UI, "WorkOrderEditor.timerTests_Tick", $"test run complete — {_testsPassed}/{_testsTotal} PASS");
                }
            }
            catch (Exception ex)
            {
                this.timerTests.Stop();
                this.progressTests.Visible = false;
                ReportFailure("WorkOrderEditor.timerTests_Tick", ex, Strings.ActionFailed);
            }
        }

        /// <summary>Failure path 1 (validation): #2002 with a blank title. The server's validator pins the error to the Title field.</summary>
        private async void buttonEmptyTitle_Click(object sender, EventArgs e)
        {
            try
            {
                SelectRow(2002);
                this.textTitle.Text = "   ";
                await SubmitWithoutPreCheckAsync(BuildSaveCommandFromEditor(), "WorkOrderEditor.buttonEmptyTitle_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrderEditor.buttonEmptyTitle_Click", ex, Strings.ActionFailed);
            }
        }

        /// <summary>Failure path 2 (range): 1,200 hours. The spin box happily accepts it; the rule does not.</summary>
        private async void buttonHoursRange_Click(object sender, EventArgs e)
        {
            try
            {
                SelectRow(2002);
                this.numericHours.Value = 1200;
                await SubmitWithoutPreCheckAsync(BuildSaveCommandFromEditor(), "WorkOrderEditor.buttonHoursRange_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrderEditor.buttonHoursRange_Click", ex, Strings.ActionFailed);
            }
        }

        /// <summary>Failure path 3 (business rule + role): edit closed #2006. Every field is valid; WorkOrderRules says no.</summary>
        private async void buttonEditClosed_Click(object sender, EventArgs e)
        {
            try
            {
                SelectRow(2006);
                this.textTitle.Text = "Replace lobby badge reader (edited after close)";
                await SubmitWithoutPreCheckAsync(BuildSaveCommandFromEditor(), "WorkOrderEditor.buttonEditClosed_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrderEditor.buttonEditClosed_Click", ex, Strings.ActionFailed);
            }
        }

        /// <summary>
        /// The UI bypass from the video: a command that never touched the editor, built the way a scripted
        /// request would — cost 9,500 on #2002. The range is fine, so the validator passes; the role rule
        /// on the server rejects it for a Technician (switch "Acting as" to Supervisor and it saves).
        /// </summary>
        private async void buttonBypass_Click(object sender, EventArgs e)
        {
            try
            {
                var stored = _rows.FirstOrDefault(o => o.Id == 2002);
                var crafted = new SaveWorkOrderCommand
                {
                    Id = 2002,
                    Title = stored != null ? stored.Title : "Repair loading dock pump",
                    AssigneeId = "T. Nguyen",
                    Priority = WorkOrderPriority.High,
                    DueDate = DateTime.Today.AddDays(10),
                    EstimatedCost = 9500m,
                    EstimatedHours = 6,
                    FromStatus = stored != null ? stored.Status : WorkOrderStatus.Assigned,
                    ToStatus = stored != null ? stored.Status : WorkOrderStatus.Assigned
                };
                _log.Warn(LogLayer.Client, "crafted request", "a disabled button is not authorization: this command skipped the editor entirely");
                await SubmitWithoutPreCheckAsync(crafted, "WorkOrderEditor.buttonBypass_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrderEditor.buttonBypass_Click", ex, Strings.ActionFailed);
            }
        }

        /// <summary>Error path + recovery: toggle the write outage, then push the editor through the pipeline as-is.</summary>
        private async void buttonOutage_Click(object sender, EventArgs e)
        {
            if (_repository == null)
                return;

            try
            {
                _repository.SimulateWriteOutage = !_repository.SimulateWriteOutage;
                bool on = _repository.SimulateWriteOutage;
                this.buttonOutage.Text = on ? "Recover the data store" : "Simulate write outage";
                _log.Info(LogLayer.UI, "WorkOrderEditor.buttonOutage_Click",
                    on ? "write outage ON → saving the editor as-is (expect: validation passes, COMMIT ✖, rollback, safe message, edits kept)"
                       : "outage OFF → retrying the same save (recovery)");

                var command = BuildSaveCommandFromEditor();
                var preCheck = _validator.Validate(command);
                if (!preCheck.IsValid)
                {
                    ShowValidation(preCheck);
                    _log.Warn(LogLayer.UI, "WorkOrderEditor.buttonOutage_Click", $"pre-check: {preCheck} — fix the editor first, then click again");
                    return;
                }

                var result = await _workOrders.SaveAsync(command, _session);
                ShowSaveResult(result);
                if (result.Succeeded)
                {
                    await RefreshGridAsync();
                    SelectRow(result.WorkOrder.Id);                                          // the saved row, with its new version
                }
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrderEditor.buttonOutage_Click", ex, Strings.SaveFailed);
                await VerifyNothingPartialAsync(SelectedWorkOrderId());
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.tracePanel.ClearTrace();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("ready", StatusKind.Normal);
        }

        #endregion

        #region Grid selection helpers (display only)

        private void SelectRow(int id)
        {
            _loadingGrid = true;
            bool found = SelectRowCore(id);
            _loadingGrid = false;
            if (found)
                FillEditor(_rows[this.gridOrders.CurrentRow.Index]);
        }

        private bool SelectRowCore(int id)
        {
            for (int i = 0; i < _rows.Count && i < this.gridOrders.Rows.Count; i++)
            {
                if (_rows[i].Id == id)
                {
                    this.gridOrders.CurrentCell = this.gridOrders.Rows[i].Cells[0];
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
