using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;
using EnterpriseOps.Integrations;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using EnterpriseOps.Services.Workflow;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — the work queue that opens the Escalation Wizard.
    ///
    /// Left card:  the queue (WorkQueueRow projections), the "Escalate work order…" button that opens the wizard
    ///             modally, the manual-review queue the compensations land in, its retry / resolve buttons and the
    ///             dark strip showing the last WorkflowResult verbatim.
    /// Right card: the live activity trace — every layer tags its line (UI → / Service: / Security: / Data: /
    ///             Integrations:), which is how a reviewer sees that the handlers here decide nothing.
    /// Bottom bar: three armed failure paths (notification, audit, directory timeout), the same workflow run
    ///             WITHOUT the wizard, the anti-pattern that inlines the flow in this page, and Clear trace.
    /// </summary>
    public partial class WorkQueuePage : Page
    {
        // The per-session service graph: page and wizard share it, two browser sessions share nothing.
        private readonly SessionServices _services;

        private List<WorkQueueRow> _rows = new List<WorkQueueRow>();
        private List<CompensationEntry> _queue = new List<CompensationEntry>();

        public WorkQueuePage()
        {
            InitializeComponent();

            _services = new SessionServices("contoso", UserDirectory.AnaOps);
            _services.Trace.Written += trace_Written;
            _services.Compensation.Changed += compensation_Changed;
        }

        #region Event handlers — thin, one service (or one dialog) each

        private void WorkQueuePage_Load(object sender, EventArgs e)
        {
            lblTenant.Text = "tenant: " + _services.Session.TenantId;
            lblUser.Text = "user: " + _services.Session.User;
            _services.Trace.Write($"UI → WorkQueuePage_Load session {_services.Session.TenantId}/{_services.Session.User.UserName}");
            LoadQueue();
            RenderCompensationQueue();
        }

        /// <summary>The success path: open the wizard modally and show the typed result it brings back.</summary>
        private async void btnEscalate_Click(object sender, EventArgs e)
        {
            var row = SelectedRow();
            if (row == null)
            {
                ShowStatus("Select a work order in the queue first.", StatusKind.Warn);
                return;
            }

            try
            {
                _services.Trace.Write($"UI → btnEscalate_Click {row.Number} → new EscalationWizard(...) → await ShowDialogAsync()");
                var wizard = new EscalationWizard(_services, row.Id);
                DialogResult answer = await wizard.ShowDialogAsync();
                ShowWizardOutcome(answer, wizard);
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
            finally
            {
                LoadQueue();
                RenderCompensationQueue();
            }
        }

        /// <summary>The recovery: the queued compensation is retried, and the workflow finishes later.</summary>
        private async void btnRetryNotification_Click(object sender, EventArgs e)
        {
            var entry = SelectedCompensation();
            if (entry == null)
            {
                ShowStatus("Select an entry in the manual-review queue first.", StatusKind.Warn);
                return;
            }

            try
            {
                btnRetryNotification.Enabled = false;
                var result = await _services.Workflow.RetryNotificationAsync(entry, _services.Session);
                ShowResult(result);
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
            finally
            {
                btnRetryNotification.Enabled = true;
                RenderCompensationQueue();
                LoadQueue();
            }
        }

        /// <summary>An audit gap cannot be retried — a sent notification cannot be unsent. An operator closes it.</summary>
        private void btnResolveManually_Click(object sender, EventArgs e)
        {
            var entry = SelectedCompensation();
            if (entry == null)
            {
                ShowStatus("Select an entry in the manual-review queue first.", StatusKind.Warn);
                return;
            }

            _services.Compensation.Resolve(entry, $"closed manually by {_services.Session.User.UserName} after follow-up");
            ShowStatus($"Compensation #{entry.Id} resolved manually.", StatusKind.Ok);
            RenderCompensationQueue();
        }

        private void btnFailNotify_Click(object sender, EventArgs e)
        {
            _services.Notifications.FailNextSend("smtp timeout after 30s");
            ShowStatus("Armed: the next approver notification will fail AFTER the escalation is persisted.", StatusKind.Warn);
        }

        private void btnFailAudit_Click(object sender, EventArgs e)
        {
            _services.Audit.FailNextWrite("audit store unavailable (503)");
            ShowStatus("Armed: the next audit write will fail AFTER the notification was sent.", StatusKind.Warn);
        }

        private void btnFailDirectory_Click(object sender, EventArgs e)
        {
            _services.Directory.HangNextLookup();
            ShowStatus("Armed: the next approver lookup will outlast the wizard's 5 s timeout.", StatusKind.Warn);
        }

        /// <summary>
        /// Review question 1 — "can the workflow be tested without the wizard?". This handler is the proof:
        /// it builds an EscalationCommand in code and calls the same EscalateAsync the wizard calls.
        /// A unit test would be these same six lines without the labels.
        /// </summary>
        private async void btnRunHeadless_Click(object sender, EventArgs e)
        {
            try
            {
                var target = FirstEscalatable();
                if (target == null)
                {
                    ShowStatus("No escalatable work order left in this tenant.", StatusKind.Warn);
                    return;
                }

                var command = new EscalationCommand(
                    WorkOrderId: target.Id,
                    WorkOrderVersion: target.Version,
                    TenantId: _services.Session.TenantId,
                    Reason: "Batch escalation: vendor SLA breached and the site has been without cooling for two days.",
                    Attachments: new List<Attachment>(),
                    ApproverId: "m.weber",
                    DueAt: DateTimeOffset.Now.AddHours(8),
                    Channels: NotificationChannels.Email | NotificationChannels.InApp,
                    RequestedBy: _services.Session.User.UserName,
                    CorrelationId: _services.Session.NextCorrelation());

                _services.Trace.Write($"UI → btnRunHeadless_Click — no wizard, no dialog: EscalateAsync({target.Number}) with a hand-built command");
                var result = await _services.Workflow.EscalateAsync(command);
                ShowResult(result);
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
            finally
            {
                LoadQueue();
                RenderCompensationQueue();
            }
        }

        /// <summary>
        /// THE ANTI-PATTERN the video shows — the same escalation written inside the screen.
        ///
        /// Everything wrong with it is on display: the rules are duplicated here (and already differ from the
        /// workflow's), the page talks to the stores and the gateway directly, there is no correlation id and no
        /// audit entry, and when the external step fails it "cleans up" by DELETING the escalation — a false
        /// transaction that throws away a valid record and any trace that it ever existed.
        ///
        /// Compare with btnEscalate_Click: four lines, one dialog, one typed result.
        /// </summary>
        private async void btnAntiPattern_Click(object sender, EventArgs e)
        {
            var target = FirstEscalatable();
            if (target == null)
            {
                ShowStatus("No escalatable work order left in this tenant.", StatusKind.Warn);
                return;
            }

            _services.Trace.Write("UI → btnAntiPattern_Click — the flow inlined in the page (arming the same SMTP failure)");
            _services.Notifications.FailNextSend("smtp timeout after 30s");

            string reason = "Escalating from the page because the wizard was in a hurry.";
            if (reason.Length < 10)                                  // a rule invented here — the workflow says 20
            {
                ShowStatus("Reason too short.", StatusKind.Error);
                return;
            }

            Escalation escalation = null;
            var previousStatus = target.Status;
            var previousVersion = target.Version;
            try
            {
                escalation = await _services.Escalations.AddAsync(new Escalation
                {
                    WorkOrderId = target.Id,
                    TenantId = target.TenantId,
                    Reason = reason,
                    ApproverId = "m.weber",
                    DueAt = DateTimeOffset.Now.AddDays(30),          // 30 days out — no rule ever looked at it
                    Channels = NotificationChannels.Email,
                    RequestedBy = _services.Session.User.UserName,
                    CreatedUtc = DateTime.UtcNow,
                    CorrelationId = "(none)",
                });
                _services.WorkOrders.MarkEscalated(target.Id, target.Version);

                var approver = new Approver { Id = "m.weber", DisplayName = "Mara Weber", Role = "Supervisor" };
                await _services.Notifications.SendApproverNotificationAsync(escalation, approver);

                ShowStatus($"Anti-pattern run created {escalation.Number} — and nothing was audited.", StatusKind.Warn);
            }
            catch (NotificationFailedException ex)
            {
                // The "rollback" nobody asked for: a valid escalation is deleted because an e-mail did not go out.
                _services.Escalations.Remove(escalation.Id);
                _services.WorkOrders.Revert(target.Id, previousStatus, previousVersion);
                _services.Trace.Write($"UI ← anti-pattern: notify failed ({ex.Message}) → the page DELETED {escalation.Number} and reverted {target.Number}");
                _services.Trace.Write("UI ← nothing compensated, nothing audited, nothing queued — the escalation simply never happened");

                ShowBanner($"Anti-pattern: the notification failed, so the page deleted a valid escalation for {target.Number}. No compensation, no audit entry, no retry.");
                ShowStatus("Anti-pattern: a false transaction threw the record away.", StatusKind.Error);
                lblStatusBar.Text = "No WorkflowResult — the page swallowed the outcome in a catch block.";
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
            finally
            {
                LoadQueue();
                RenderCompensationQueue();
            }
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            lstTrace.Items.Clear();
            lblBanner.Visible = false;
        }

        private void trace_Written(string line)
        {
            lstTrace.Items.Add(line);
            lstTrace.SelectedIndex = lstTrace.Items.Count - 1;
        }

        private void compensation_Changed()
        {
            RenderCompensationQueue();
        }

        #endregion

        #region Rendering — UI state only

        private void LoadQueue()
        {
            _rows = _services.WorkQueue.Load(_services.Session);
            dgvWorkQueue.DataSource = new BindingSource { DataSource = _rows };
            lblCorrelation.Text = "corr " + _services.Session.CorrelationId;
        }

        private void RenderCompensationQueue()
        {
            _queue = _services.Compensation.Entries.OrderBy(c => c.Status).ThenBy(c => c.Id).ToList();
            lstCompensation.Items.Clear();
            foreach (var entry in _queue)
                lstCompensation.Items.Add(entry.ToString());

            int open = _services.Compensation.ManualReviewQueue.Count;
            lblCompensationTitle.Text = $"Manual-review queue — open compensations: {open}";
        }

        /// <summary>What the wizard brought back: a typed result, or a cancel that says what happened to the draft.</summary>
        private void ShowWizardOutcome(DialogResult answer, EscalationWizard wizard)
        {
            if (answer == DialogResult.OK && wizard.Result != null)
            {
                ShowResult(wizard.Result);
                return;
            }

            lblBanner.Visible = false;
            ShowStatus(wizard.DraftKept
                    ? "Wizard cancelled — the draft is kept on the server; \"Escalate work order…\" resumes it."
                    : "Wizard cancelled — draft discarded and the staged uploads cleaned up. Nothing was persisted.",
                StatusKind.Warn);
            lblStatusBar.Text = "No WorkflowResult — the workflow was never called.";
        }

        /// <summary>One typed result in, one screen state out. Switch on the Outcome, never on the message.</summary>
        private void ShowResult(WorkflowResult result)
        {
            lblStatusBar.Text = result.ToString();
            lblCorrelation.Text = "corr " + result.CorrelationId;

            switch (result.Outcome)
            {
                case WorkflowOutcome.Created:
                    lblBanner.Visible = false;
                    ShowStatus(result.Message, StatusKind.Ok);
                    AlertBox.Show(result.Message, MessageBoxIcon.Information,
                        alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                    break;

                case WorkflowOutcome.CreatedWithCompensation:
                    ShowBanner($"{result.Message}  ·  {result.CompensationAction}  ·  next: {result.NextAction}");
                    ShowStatus("Escalation kept, compensation recorded — see the manual-review queue.", StatusKind.Warn);
                    AlertBox.Show(result.Message, MessageBoxIcon.Warning,
                        alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                    break;

                case WorkflowOutcome.ValidationFailed:
                    ShowBanner(result.Message + "  ·  " + string.Join(" · ", result.FieldErrors.Select(f => $"{WizardSteps.Title(f.Step)}/{f.Field}: {f.Message}")));
                    ShowStatus("Validation failed — nothing was persisted.", StatusKind.Error);
                    break;

                default:
                    ShowBanner($"{result.Message}  ·  next: {result.NextAction}");
                    ShowStatus($"{result.Outcome} — nothing was persisted.", StatusKind.Error);
                    break;
            }
        }

        private void ShowBanner(string text)
        {
            lblBanner.Text = text;
            lblBanner.Visible = true;
        }

        private enum StatusKind { Ok, Warn, Error }

        private void ShowStatus(string text, StatusKind kind)
        {
            lblStatus.Text = "● " + text;
            lblStatus.ForeColor = kind switch
            {
                StatusKind.Ok => System.Drawing.Color.FromArgb(31, 157, 87),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(224, 86, 59),
            };
        }

        private WorkQueueRow SelectedRow()
        {
            int index = dgvWorkQueue.CurrentRow?.Index ?? -1;
            return index >= 0 && index < _rows.Count ? _rows[index] : null;
        }

        private CompensationEntry SelectedCompensation()
        {
            int index = lstCompensation.SelectedIndex;
            return index >= 0 && index < _queue.Count ? _queue[index] : null;
        }

        /// <summary>A work order this tenant can still escalate — used by the two "no wizard" buttons.</summary>
        private WorkOrder FirstEscalatable()
        {
            return _services.WorkOrders.ForTenant(_services.Session.TenantId)
                .FirstOrDefault(w => w.Status != WorkOrderStatus.Completed
                                  && w.Status != WorkOrderStatus.Cancelled
                                  && w.Status != WorkOrderStatus.Escalated
                                  && !_services.Escalations.ExistsForWorkOrder(w.Id));
        }

        private void ReportUnexpected(Exception ex)
        {
            _services.Trace.Write($"UI ← unexpected {ex.GetType().Name}: {ex.Message}");
            ShowStatus("The action could not be completed. Check the trace for details.", StatusKind.Error);
            AlertBox.Show("The action could not be completed. Check the log for details.",
                MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion
    }
}
