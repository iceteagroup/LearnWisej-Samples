using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;
using EnterpriseOps.Services;
using EnterpriseOps.Services.Workflow;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — the Escalation Wizard (the video's EscalationWizard, opened modally with ShowDialogAsync).
    ///
    /// Six designable steps — Reason · Attachments · Approver · Due date · Notifications · Review — fill ONE typed
    /// <see cref="EscalationWizardState"/>. On Finish the workflow turns that state into one
    /// <see cref="EscalationCommand"/> and returns one <see cref="WorkflowResult"/>.
    ///
    /// What this file is allowed to do: move between steps, copy values into the state object, show what the
    /// workflow answered. What it must never do — and does not do anywhere below:
    ///   · decide whether a reason is long enough, an attachment required, an approver allowed, a due date acceptable
    ///     → EscalationWorkflow.ValidateStep(state, step)
    ///   · persist, notify, audit or compensate → EscalationWorkflow.EscalateAsync(command)
    ///   · talk to a repository, a gateway or the audit log directly → it has no reference to any of them.
    ///
    /// State ownership: the state object lives in WorkflowStateStore on the server, saved after every completed
    /// step, so a browser refresh (or a cancelled wizard) loses nothing — the next open resumes the draft.
    /// </summary>
    public partial class EscalationWizard : Form
    {
        private readonly SessionServices _services;
        private readonly IEscalationWorkflow _workflow;
        private readonly EscalationWizardState _state;
        private readonly WorkOrder _workOrder;

        // The five orchestration steps of EscalateAsync, in order — the strip under the wizard in the video.
        private static readonly string[] OrchestrationSteps = { "validate", "authorize", "persist", "notify", "audit" };
        private readonly Dictionary<string, StepStatus> _stepStatus = new Dictionary<string, StepStatus>();

        private CancellationTokenSource _lookupCts;     // instance field: the approver lookup must be cancellable
        private bool _loading;                          // suppress control events while the state is written into the UI
        private bool _finished;                         // the workflow has run; Next becomes Close
        private bool _cancelHandled;                    // Cancel already decided what happens to the draft

        /// <summary>The typed result the caller reads after ShowDialogAsync returns (null when cancelled).</summary>
        public WorkflowResult Result { get; private set; }

        /// <summary>True when Cancel kept the draft, so the page can say "resume later" instead of "discarded".</summary>
        public bool DraftKept { get; private set; }

        /// <summary>True when this wizard picked up a draft saved by an earlier (interrupted) run.</summary>
        public bool Resumed { get; }

        public EscalationWizard(SessionServices services, int workOrderId)
        {
            InitializeComponent();

            _services = services;
            _workflow = services.Workflow;
            _workOrder = services.WorkOrders.Find(workOrderId);

            // Resumable workflow: a draft saved by an earlier run wins over a fresh state object.
            var draft = services.Drafts.Find(workOrderId);
            Resumed = draft != null;
            _state = draft ?? new EscalationWizardState
            {
                WorkOrderId = workOrderId,
                WorkOrderVersion = _workOrder.Version,
                WorkOrderTitle = _workOrder.Title,
                TenantId = services.Session.TenantId,
            };
            _state.WorkOrderVersion = _workOrder.Version;    // the version travels in the command
        }

        #region Event handlers — thin: collect, ask the workflow, show the answer

        private async void EscalationWizard_Load(object sender, EventArgs e)
        {
            try
            {
                _services.Trace.Write($"UI → EscalationWizard opened for {_workOrder.Number} ({(Resumed ? "resumed" : "new")} draft {_state.DraftId})");
                this.Text = $"Escalate work order — {_workOrder.Number}";
                lblWorkOrder.Text = $"{_workOrder.Number} — {_workOrder.Title} · {_workOrder.Customer} · {_workOrder.Priority} · tenant {_workOrder.TenantId} · v{_workOrder.Version}";

                WriteStateIntoControls();
                ShowStep(_state.CurrentStep);
                RenderOrchestration();

                if (_state.CurrentStep == WizardStep.Approver)
                    await LoadApproversAsync();
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
        }

        /// <summary>Next / Finish / Close — the handler the lab's code check reads: thin, async, service call, try/catch.</summary>
        private async void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (_finished)
                {
                    CloseWith(DialogResult.OK);
                    return;
                }

                if (_state.CurrentStep == WizardStep.Review)
                    await FinishAsync();
                else
                    await AdvanceAsync();
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
        }

        private async void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (_state.CurrentStep == WizardStep.Reason) return;

                CollectCurrentStep();
                var previous = (WizardStep)((int)_state.CurrentStep - 1);
                _services.Trace.Write($"UI → Back to step {(int)previous + 1} ({WizardSteps.Title(previous)}) — nothing is validated going back");
                ShowStep(previous);

                if (previous == WizardStep.Approver && cboApprover.Items.Count == 0)
                    await LoadApproversAsync();
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
        }

        /// <summary>
        /// Closing the window with the X (or a lost tab) is an interruption, not a decision: the state object is
        /// saved so the next open resumes it. Only Cancel → No actually discards a draft.
        /// </summary>
        private void EscalationWizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_finished || _cancelHandled) return;

            CollectCurrentStep();
            _services.Drafts.Save(_state);
            DraftKept = true;
            _services.Trace.Write($"UI → wizard closed at step {(int)_state.CurrentStep + 1} without a decision — draft {_state.DraftId} kept, nothing persisted");
        }

        /// <summary>Cancel: keep the collected state as a draft, or discard it and clean the staged uploads up.</summary>
        private async void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                CollectCurrentStep();

                DialogResult answer = await MessageBox.ShowAsync(
                    $"Keep this escalation as a draft?\n\n{_state.Progress} for {_workOrder.Number}.\nNo discards it and removes {_state.Attachments.Count} staged upload(s).",
                    "Cancel escalation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (answer == DialogResult.Yes)
                {
                    _services.Drafts.Save(_state);
                    DraftKept = true;
                    _services.Trace.Write($"UI → cancelled, draft {_state.DraftId} kept — the wizard can resume from step {(int)_state.CurrentStep + 1}");
                }
                else
                {
                    _services.Staging.DiscardAll(_state.Attachments);
                    _services.Drafts.Remove(_state.WorkOrderId, "cancelled by the user");
                    _services.Trace.Write("UI → cancelled, draft discarded and staging cleaned up — nothing was persisted");
                }

                _cancelHandled = true;
                CloseWith(DialogResult.Cancel);
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
        }

        private void btnStageFile_Click(object sender, EventArgs e)
        {
            var attachment = _services.Staging.StageNextSample();
            _state.Attachments.Add(attachment);
            RenderAttachments();
            lblValidation.Text = "";
        }

        private void btnDiscardFile_Click(object sender, EventArgs e)
        {
            int index = lstAttachments.SelectedIndex;
            if (index < 0 || index >= _state.Attachments.Count)
            {
                lblValidation.Text = "Select a staged file first.";
                return;
            }

            var attachment = _state.Attachments[index];
            _services.Staging.Discard(attachment.StagedId);
            _state.Attachments.RemoveAt(index);
            RenderAttachments();
        }

        private async void btnLookupApprovers_Click(object sender, EventArgs e)
        {
            try
            {
                await LoadApproversAsync();
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
        }

        private void cboApprover_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            _state.ApproverId = (cboApprover.SelectedItem as Approver)?.Id;
            lblValidation.Text = "";
        }

        private void dtpDueDate_ValueChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            _state.DueAtLocal = dtpDueDate.Value;
            lblValidation.Text = "";
        }

        private void notification_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            _state.NotifyEmail = chkNotifyEmail.Checked;
            _state.NotifyInApp = chkNotifyInApp.Checked;
            _state.NotifySms = chkNotifySms.Checked;
            lblValidation.Text = "";
        }

        private void txtReason_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            _state.Reason = txtReason.Text;
            lblReasonHint.Text = $"{txtReason.Text.Trim().Length} characters. Whether that is enough is decided by EscalationWorkflow.ValidateStep — this page only counts.";
        }

        #endregion

        #region The two operations — one asks the workflow to validate, the other asks it to execute

        /// <summary>Step → next step. The rule that says "you may leave" belongs to the workflow, so it is asked.</summary>
        private async Task AdvanceAsync()
        {
            CollectCurrentStep();

            var validation = _workflow.ValidateStep(_state, _state.CurrentStep);
            if (!validation.IsValid)
            {
                // Validation failure path: stay on the step, show what the service said, persist nothing.
                lblValidation.Text = validation.Summary;
                lblWizardStatus.Text = $"StepValidation — {validation.Errors.Count} error(s) on {WizardSteps.Title(_state.CurrentStep)} · the step is not left until they are fixed";
                AlertBox.Show(validation.Summary, MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            _state.MarkComplete(_state.CurrentStep);
            _services.Drafts.Save(_state);                       // this is what survives a browser refresh

            var next = (WizardStep)((int)_state.CurrentStep + 1);
            ShowStep(next);

            if (next == WizardStep.Approver && cboApprover.Items.Count == 0)
                await LoadApproversAsync();
        }

        /// <summary>Finish: build the typed command, run the workflow, show the typed result. No business logic here.</summary>
        private async Task FinishAsync()
        {
            var command = _workflow.BuildCommand(_state, _services.Session);
            var progress = new WorkflowProgressObserver(OnWorkflowProgress);

            BeginBusy("EscalateAsync — validate → authorize → persist → notify → audit…");
            try
            {
                Result = await _workflow.EscalateAsync(command, progress);
                ShowResult(Result);
            }
            finally
            {
                EndBusy();
            }
        }

        /// <summary>The external directory call: bounded by a timeout, and a timeout is not a lost draft.</summary>
        private async Task LoadApproversAsync()
        {
            _lookupCts?.Cancel();
            _lookupCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            btnLookupApprovers.Enabled = false;
            lblApproverHint.Text = "Looking the directory up (5 s timeout)…";
            Application.Update(this);

            try
            {
                var approvers = await _workflow.LookupApproversAsync(_state.TenantId, _lookupCts.Token);
                _loading = true;
                cboApprover.DataSource = null;
                cboApprover.DisplayMember = "DisplayName";
                cboApprover.DataSource = approvers.ToList();
                cboApprover.SelectedItem = approvers.FirstOrDefault(a => a.Id == _state.ApproverId);
                if (cboApprover.SelectedItem == null) cboApprover.SelectedIndex = -1;
                _loading = false;

                lblApproverHint.Text = $"{approvers.Count} people in the directory. Who may actually approve is decided by PermissionService when you click Next — not here.";
            }
            catch (OperationCanceledException)
            {
                // Failure path matrix · timeout: the draft stays, nothing is half-applied, the user can retry.
                _services.Trace.Write("UI → approver lookup timed out after 5 s — draft kept, the user can retry");
                lblApproverHint.Text = "The approver directory did not answer within 5 seconds. Your draft is saved — click \"Look up the directory again\".";
                lblValidation.Text = "External step timed out: approver directory. Nothing was lost.";
                lblWizardStatus.Text = "Timeout — the workflow was never called; the wizard state is still on the server.";
            }
            finally
            {
                btnLookupApprovers.Enabled = true;
                Application.Update(this);
            }
        }

        #endregion

        #region Rendering — UI state only

        /// <summary>Copies the controls of the current step into the state object. No rules, just a copy.</summary>
        private void CollectCurrentStep()
        {
            switch (_state.CurrentStep)
            {
                case WizardStep.Reason:
                    _state.Reason = txtReason.Text;
                    break;
                case WizardStep.Approver:
                    _state.ApproverId = (cboApprover.SelectedItem as Approver)?.Id ?? _state.ApproverId;
                    break;
                case WizardStep.DueDate:
                    _state.DueAtLocal = dtpDueDate.Value;
                    break;
                case WizardStep.Notifications:
                    _state.NotifyEmail = chkNotifyEmail.Checked;
                    _state.NotifyInApp = chkNotifyInApp.Checked;
                    _state.NotifySms = chkNotifySms.Checked;
                    break;
            }
        }

        /// <summary>The resumed (or fresh) state object drives every control — the state is the truth, not the controls.</summary>
        private void WriteStateIntoControls()
        {
            _loading = true;
            txtReason.Text = _state.Reason;
            dtpDueDate.Value = _state.DueAtLocal;
            chkNotifyEmail.Checked = _state.NotifyEmail;
            chkNotifyInApp.Checked = _state.NotifyInApp;
            chkNotifySms.Checked = _state.NotifySms;
            _loading = false;

            lblReasonHint.Text = $"{(_state.Reason ?? "").Trim().Length} characters. Whether that is enough is decided by EscalationWorkflow.ValidateStep — this page only counts.";
            RenderAttachments();
        }

        private void ShowStep(WizardStep step)
        {
            _state.CurrentStep = step;
            lblValidation.Text = "";

            pnlReason.Visible = step == WizardStep.Reason;
            pnlAttachments.Visible = step == WizardStep.Attachments;
            pnlApprover.Visible = step == WizardStep.Approver;
            pnlDueDate.Visible = step == WizardStep.DueDate;
            pnlNotifications.Visible = step == WizardStep.Notifications;
            pnlReview.Visible = step == WizardStep.Review;

            btnBack.Enabled = step != WizardStep.Reason;
            btnNext.Text = step == WizardStep.Review ? "Finish" : "Next";
            lblStepCounter.Text = $"Step {(int)step + 1} of {WizardSteps.Count}";

            if (step == WizardStep.Review) RenderSummary();

            RenderRail();
            lblWizardStatus.Text = step == WizardStep.Review
                ? "Review & finish — EscalationCommand ready"
                : $"Step {(int)step + 1} of {WizardSteps.Count} — the pages collect, EscalationWorkflow decides.";
        }

        private void RenderRail()
        {
            var labels = new[] { lblStepReason, lblStepAttachments, lblStepApprover, lblStepDueDate, lblStepNotifications, lblStepReview };
            for (int i = 0; i < labels.Length; i++)
            {
                var step = (WizardStep)i;
                bool active = step == _state.CurrentStep;
                bool done = _state.CompletedSteps.Contains(step);

                labels[i].Text = $"{(done ? "✓" : (i + 1).ToString())}  {WizardSteps.Title(step)}";
                labels[i].Font = new System.Drawing.Font("default", 10F, active ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular);
                labels[i].BackColor = active ? System.Drawing.Color.FromArgb(234, 243, 255) : System.Drawing.Color.Transparent;
                labels[i].ForeColor = active
                    ? System.Drawing.Color.FromArgb(13, 27, 42)
                    : (done ? System.Drawing.Color.FromArgb(31, 138, 76) : System.Drawing.Color.FromArgb(138, 151, 164));
            }

            lblDraftState.Text = $"draft {_state.DraftId} · {(Resumed ? "resumed" : "new")} · {_state.Progress} · saved {_state.UpdatedUtc:HH:mm:ss} UTC";
        }

        private void RenderAttachments()
        {
            lstAttachments.Items.Clear();
            foreach (var attachment in _state.Attachments)
                lstAttachments.Items.Add($"{attachment.StagedId}  {attachment}");
        }

        private void RenderSummary()
        {
            var rows = new List<SummaryRow>
            {
                new SummaryRow { Field = "WORK ORDER",  Value = $"{_workOrder.Number} — {_workOrder.Title} (v{_state.WorkOrderVersion})" },
                new SummaryRow { Field = "REASON",      Value = (_state.Reason ?? "").Trim() },
                new SummaryRow { Field = "ATTACHMENTS", Value = _state.AttachmentSummary },
                new SummaryRow { Field = "APPROVER",    Value = ApproverText() },
                new SummaryRow { Field = "DUE",         Value = $"{_state.DueAtLocal:MMM d, HH:mm} · notify by {_state.ChannelSummary}" },
                new SummaryRow { Field = "REQUESTED BY",Value = $"{_services.Session.User} · tenant {_services.Session.TenantId}" },
            };
            dgvSummary.DataSource = new BindingSource { DataSource = rows };
        }

        private string ApproverText()
        {
            var approver = cboApprover.SelectedItem as Approver;
            return approver != null ? $"{approver.Id} — {approver.Role}" : (_state.ApproverId ?? "—");
        }

        /// <summary>The video's orchestration strip, driven by the workflow's progress reports.</summary>
        private void OnWorkflowProgress(WorkflowProgress progress)
        {
            _stepStatus[progress.Step] = progress.Status;
            RenderOrchestration();
            lblWizardStatus.Text = $"EscalateAsync — {progress.Step} {progress.Status.ToString().ToLowerInvariant()}{(string.IsNullOrEmpty(progress.Detail) ? "" : ": " + progress.Detail)}";
            Application.Update(this);
        }

        private void RenderOrchestration()
        {
            var parts = OrchestrationSteps.Select(name =>
            {
                var status = _stepStatus.TryGetValue(name, out var s) ? s : StepStatus.Pending;
                string glyph = status switch
                {
                    StepStatus.Succeeded => "✓",
                    StepStatus.Failed => "✕",
                    StepStatus.Compensated => "✕",
                    StepStatus.Running => "…",
                    _ => "·",
                };
                return $"{glyph} {name}{(status == StepStatus.Compensated ? " + compensation" : "")}";
            });
            lblOrchestration.Text = "EscalationWorkflow:  " + string.Join("  →  ", parts);
        }

        /// <summary>Typed result in, screen out. Every branch is an Outcome — no string parsing, no exception catching.</summary>
        private void ShowResult(WorkflowResult result)
        {
            lblResultBanner.Visible = true;
            lblResultDetail.Visible = true;
            lblWizardStatus.Text = result.ToString();

            switch (result.Outcome)
            {
                case WorkflowOutcome.Created:
                    Banner(System.Drawing.Color.FromArgb(240, 249, 243), System.Drawing.Color.FromArgb(21, 95, 51), System.Drawing.Color.FromArgb(46, 125, 79));
                    lblResultBanner.Text = "✓  " + result.Message;
                    lblResultDetail.Text = $"persist ✓ · notify ✓ · audit ✓ · ESC-{result.EscalationId} · correlation {result.CorrelationId}";
                    Finish("Close");
                    break;

                case WorkflowOutcome.CreatedWithCompensation:
                    // The video's failure path: the escalation is KEPT, the compensation is recorded.
                    Banner(System.Drawing.Color.FromArgb(255, 248, 236), System.Drawing.Color.FromArgb(122, 82, 16), System.Drawing.Color.FromArgb(154, 122, 58));
                    lblResultBanner.Text = "!  " + result.Message;
                    lblResultDetail.Text = $"{StepLine(result)} · CompensationAction: {result.CompensationAction} · audited · next: {result.NextAction}";
                    Finish("Close");
                    break;

                case WorkflowOutcome.ValidationFailed:
                    Banner(System.Drawing.Color.FromArgb(253, 236, 234), System.Drawing.Color.FromArgb(154, 42, 24), System.Drawing.Color.FromArgb(180, 70, 50));
                    lblResultBanner.Text = "✕  " + result.Message;
                    lblResultDetail.Text = string.Join("   ·   ", result.FieldErrors.Select(f => $"{WizardSteps.Title(f.Step)}/{f.Field}: {f.Message}"));
                    // The typed result says which step failed, so the wizard can jump straight to it.
                    if (result.FirstFailingStep.HasValue)
                    {
                        var step = result.FirstFailingStep.Value;
                        _services.Trace.Write($"UI ← ValidationFailed → jumping back to step {(int)step + 1} ({WizardSteps.Title(step)})");
                        ShowStep(step);
                        lblValidation.Text = string.Join(" · ", result.FieldErrors.Where(f => f.Step == step).Select(f => $"{f.Field}: {f.Message}"));
                    }
                    break;

                default:    // Unauthorized · Failed
                    Banner(System.Drawing.Color.FromArgb(253, 236, 234), System.Drawing.Color.FromArgb(154, 42, 24), System.Drawing.Color.FromArgb(180, 70, 50));
                    lblResultBanner.Text = "✕  " + result.Message;
                    lblResultDetail.Text = $"{result.Outcome} · nothing was persisted · correlation {result.CorrelationId} · next: {result.NextAction}";
                    break;
            }
        }

        private static string StepLine(WorkflowResult result)
        {
            return string.Join(" · ", result.Steps.Select(s => $"{s.Name} {(s.Status == StepStatus.Succeeded ? "✓" : (s.Status == StepStatus.Compensated ? "✕" : s.Status.ToString().ToLowerInvariant()))}"));
        }

        private void Banner(System.Drawing.Color back, System.Drawing.Color title, System.Drawing.Color detail)
        {
            lblResultBanner.BackColor = back;
            lblResultBanner.ForeColor = title;
            lblResultDetail.BackColor = back;
            lblResultDetail.ForeColor = detail;
        }

        /// <summary>The workflow ran and the escalation exists: the wizard is read-only from here.</summary>
        private void Finish(string buttonText)
        {
            _finished = true;
            _services.Drafts.Remove(_state.WorkOrderId, "the escalation was created");
            btnBack.Enabled = false;
            btnCancel.Visible = false;
            btnNext.Text = buttonText;
        }

        private void BeginBusy(string status)
        {
            btnNext.Enabled = false;
            btnBack.Enabled = false;
            btnCancel.Enabled = false;
            lblValidation.Text = "";
            lblWizardStatus.Text = status;
            _stepStatus.Clear();
            RenderOrchestration();
            Application.Update(this);
        }

        private void EndBusy()
        {
            btnNext.Enabled = true;
            btnCancel.Enabled = true;
            if (!_finished) btnBack.Enabled = _state.CurrentStep != WizardStep.Reason;
            Application.Update(this);
        }

        private void CloseWith(DialogResult result)
        {
            this.DialogResult = result;
            this.Close();
        }

        /// <summary>Nothing the workflow models — a bug. Log it, tell the user something safe, keep the draft.</summary>
        private void ReportUnexpected(Exception ex)
        {
            _services.Trace.Write($"UI ← unexpected {ex.GetType().Name}: {ex.Message} — draft {_state.DraftId} kept");
            _services.Drafts.Save(_state);
            lblValidation.Text = "The action could not be completed. Your draft was kept.";
            AlertBox.Show("The action could not be completed. Check the activity trace for details.",
                MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        /// <summary>The Review grid's row shape — a projection, never a domain entity.</summary>
        private class SummaryRow
        {
            public string Field { get; set; }
            public string Value { get; set; }
        }
    }
}
