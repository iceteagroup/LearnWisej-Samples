using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using EnterpriseOps.Integrations;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services.Workflow
{
    /// <summary>
    /// The escalation workflow: validate → authorize → persist → notify → audit — ONE owner for the whole
    /// transition, not five page events. Every rule that decides anything lives here, so the same
    /// transition can run from the wizard, from a batch job, or from a unit test with a hand-built command.
    ///
    /// Compensation: a database transaction covers persist and audit, not the notification. When the
    /// notification fails after the persist, the escalation is KEPT (it is valid) and a compensating entry
    /// records that the notification is outstanding; when the audit fails after the notification, the audit
    /// gap is recorded and alerted, because a sent notification cannot be unsent.
    /// </summary>
    public sealed class EscalationWorkflow : IEscalationWorkflow
    {
        private readonly InMemoryWorkOrderStore _workOrders;
        private readonly InMemoryEscalationStore _escalations;
        private readonly AttachmentStaging _staging;
        private readonly IApproverDirectory _directory;
        private readonly INotificationGateway _notifications;
        private readonly AuditLog _audit;
        private readonly PermissionService _permissions;
        private readonly CompensationLog _compensation;
        private readonly ActivityTrace _trace;

        public EscalationWorkflow(
            InMemoryWorkOrderStore workOrders,
            InMemoryEscalationStore escalations,
            AttachmentStaging staging,
            IApproverDirectory directory,
            INotificationGateway notifications,
            AuditLog audit,
            PermissionService permissions,
            CompensationLog compensation,
            ActivityTrace trace)
        {
            _workOrders = workOrders;
            _escalations = escalations;
            _staging = staging;
            _directory = directory;
            _notifications = notifications;
            _audit = audit;
            _permissions = permissions;
            _compensation = compensation;
            _trace = trace;
        }

        #region Step validation and command building (what keeps the wizard pages thin)

        /// <summary>
        /// The business rules for one step. The wizard calls this from btnNext_Click and shows the
        /// errors; it does not know that a Critical work order needs an attachment, or that a due date
        /// must fall within 48 hours for Critical — only this class does.
        /// </summary>
        public StepValidation ValidateStep(EscalationWizardState state, WizardStep step)
        {
            var errors = new List<FieldError>();
            var workOrder = _workOrders.Find(state.WorkOrderId);

            switch (step)
            {
                case WizardStep.Reason:
                    string reason = (state.Reason ?? "").Trim();
                    if (reason.Length == 0)
                        errors.Add(new FieldError(step, "Reason", "a reason is required"));
                    else if (reason.Length < 20)
                        errors.Add(new FieldError(step, "Reason", $"say why in at least 20 characters ({reason.Length} so far)"));
                    else if (reason.Length > 500)
                        errors.Add(new FieldError(step, "Reason", "keep the reason under 500 characters"));
                    break;

                case WizardStep.Attachments:
                    if (workOrder != null && workOrder.Priority == Priority.Critical && state.Attachments.Count == 0)
                        errors.Add(new FieldError(step, "Attachments", "a Critical escalation needs at least one attachment (quote, photo or report)"));
                    break;

                case WizardStep.Approver:
                    if (string.IsNullOrEmpty(state.ApproverId))
                        errors.Add(new FieldError(step, "Approver", "choose an approver"));
                    else
                    {
                        var approver = _directory.Find(state.TenantId, state.ApproverId);
                        if (approver == null)
                            errors.Add(new FieldError(step, "Approver", $"'{state.ApproverId}' is not in the directory"));
                        else if (!_permissions.CanApprove(approver))
                            errors.Add(new FieldError(step, "Approver", $"{approver.Id} is a {approver.Role} — only Supervisors and Managers approve"));
                    }
                    break;

                case WizardStep.DueDate:
                    var due = new DateTimeOffset(state.DueAtLocal);
                    var now = DateTimeOffset.Now;
                    if (due < now.AddHours(1))
                        errors.Add(new FieldError(step, "DueDate", "the due date must be at least one hour from now"));
                    else if (due > now.AddDays(14))
                        errors.Add(new FieldError(step, "DueDate", "escalations are due within 14 days"));
                    else if (workOrder != null && workOrder.Priority == Priority.Critical && due > now.AddHours(48))
                        errors.Add(new FieldError(step, "DueDate", "a Critical work order must be resolved within 48 hours"));
                    break;

                case WizardStep.Notifications:
                    if (state.Channels == NotificationChannels.None)
                        errors.Add(new FieldError(step, "Notifications", "pick at least one channel so the approver actually hears about it"));
                    break;

                case WizardStep.Review:
                    foreach (var earlier in Enum.GetValues(typeof(WizardStep)).Cast<WizardStep>().Where(s => s < WizardStep.Review))
                        errors.AddRange(ValidateStep(state, earlier).Errors);
                    break;
            }

            var result = errors.Count == 0 ? StepValidation.Valid : new StepValidation(errors);
            _trace.Write(result.IsValid
                ? $"Service: ValidateStep({step}) → valid"
                : $"Service: ValidateStep({step}) → invalid: {result.Summary}");
            return result;
        }

        /// <summary>
        /// The Approver step's list, fetched through the workflow so the wizard never holds an integration.
        /// The list is NOT filtered down to people who may approve: a directory is a directory, and who may
        /// approve is a rule — ValidateStep applies it, which is how the lab shows a "listed but not allowed" user.
        /// </summary>
        public async Task<IReadOnlyList<Approver>> LookupApproversAsync(string tenantId, System.Threading.CancellationToken cancellationToken)
        {
            _trace.Write($"Service: LookupApproversAsync tenant '{tenantId}' (external directory — the caller owns the timeout)");
            var approvers = await _directory.LookupAsync(tenantId, cancellationToken);
            _trace.Write($"Service: → {approvers.Count} directory entries; eligibility is decided at ValidateStep(Approver)");
            return approvers;
        }

        /// <summary>State → command. The only place the two shapes meet; the wizard never builds the command.</summary>
        public EscalationCommand BuildCommand(EscalationWizardState state, SessionContext session)
        {
            var command = new EscalationCommand(
                WorkOrderId: state.WorkOrderId,
                WorkOrderVersion: state.WorkOrderVersion,
                TenantId: session.TenantId,
                Reason: (state.Reason ?? "").Trim(),
                Attachments: state.Attachments.ToList(),
                ApproverId: state.ApproverId,
                DueAt: new DateTimeOffset(state.DueAtLocal),
                Channels: state.Channels,
                RequestedBy: session.User.UserName,
                CorrelationId: session.NextCorrelation());

            _trace.Write($"Service: BuildCommand → EscalationCommand(WO-{command.WorkOrderId} v{command.WorkOrderVersion}, approver {command.ApproverId}, due {command.DueAt:MMM d HH:mm}, {command.Channels}, {command.Attachments.Count} file(s)) [{command.CorrelationId}]");
            return command;
        }

        #endregion

        #region EscalateAsync — the orchestration

        public Task<WorkflowResult> EscalateAsync(EscalationCommand command) => EscalateAsync(command, null);

        public async Task<WorkflowResult> EscalateAsync(EscalationCommand command, IProgress<WorkflowProgress> progress)
        {
            var steps = new List<StepOutcome>();
            void Report(string step, StepStatus status, string detail)
            {
                steps.RemoveAll(s => s.Name == step);
                steps.Add(new StepOutcome(step, status, detail));
                progress?.Report(new WorkflowProgress(step, status, detail));
            }

            _trace.Write($"Service: EscalateAsync [{command.CorrelationId}] WO-{command.WorkOrderId} by {command.RequestedBy}");

            // ── 1. validate ────────────────────────────────────────────────────────────────────────────
            Report("validate", StepStatus.Running, "");
            var errors = ValidateCommand(command);
            if (errors.Count > 0)
            {
                Report("validate", StepStatus.Failed, $"{errors.Count} error(s)");
                _trace.Write($"Service: → ValidationFailed: {string.Join(" · ", errors.Select(e => $"{e.Step}.{e.Field}: {e.Message}"))}");
                return new WorkflowResult(false, "The escalation was not created — fix the highlighted fields.")
                {
                    Outcome = WorkflowOutcome.ValidationFailed,
                    FieldErrors = errors,
                    Steps = steps,
                    CorrelationId = command.CorrelationId,
                    NextAction = $"return to the {WizardSteps.Title(errors.Min(e => e.Step))} step",
                };
            }
            Report("validate", StepStatus.Succeeded, "command complete and consistent");

            // ── 2. authorize ───────────────────────────────────────────────────────────────────────────
            Report("authorize", StepStatus.Running, "");
            var workOrder = _workOrders.Find(command.WorkOrderId);
            var user = UserDirectory.Find(command.RequestedBy);
            if (user == null || !_permissions.CanEscalate(user, command.TenantId, workOrder.TenantId))
            {
                Report("authorize", StepStatus.Failed, "denied");
                return new WorkflowResult(false, $"{command.RequestedBy} may not escalate work orders in tenant '{command.TenantId}'.")
                {
                    Outcome = WorkflowOutcome.Unauthorized,
                    Steps = steps,
                    CorrelationId = command.CorrelationId,
                    NextAction = "ask a Manager to escalate, or request the permission",
                };
            }
            if (workOrder.Status == WorkOrderStatus.Completed || workOrder.Status == WorkOrderStatus.Cancelled || workOrder.Status == WorkOrderStatus.Escalated || _escalations.ExistsForWorkOrder(workOrder.Id))
            {
                Report("authorize", StepStatus.Failed, $"status {workOrder.Status}");
                _trace.Write($"Service: → rejected — WO-{workOrder.Id} is {workOrder.Status} and cannot be escalated (again)");
                return new WorkflowResult(false, $"{workOrder.Number} is {workOrder.Status} — it cannot be escalated.")
                {
                    Outcome = WorkflowOutcome.ValidationFailed,
                    FieldErrors = new[] { new FieldError(WizardStep.Reason, "WorkOrder", $"{workOrder.Number} is {workOrder.Status}") },
                    Steps = steps,
                    CorrelationId = command.CorrelationId,
                    NextAction = "pick a different work order",
                };
            }
            Report("authorize", StepStatus.Succeeded, $"{user} in '{command.TenantId}'");

            // ── 3. persist ─────────────────────────────────────────────────────────────────────────────
            Report("persist", StepStatus.Running, "");
            var previousStatus = workOrder.Status;
            var previousVersion = workOrder.Version;
            Escalation escalation = null;
            try
            {
                escalation = await _escalations.AddAsync(new Escalation
                {
                    WorkOrderId = command.WorkOrderId,
                    TenantId = command.TenantId,
                    Reason = command.Reason,
                    ApproverId = command.ApproverId,
                    DueAt = command.DueAt,
                    Channels = command.Channels,
                    Attachments = command.Attachments.ToList(),
                    RequestedBy = command.RequestedBy,
                    CreatedUtc = DateTime.UtcNow,
                    CorrelationId = command.CorrelationId,
                });
                _workOrders.MarkEscalated(command.WorkOrderId, command.WorkOrderVersion);
                _staging.Commit(command.Attachments);
                Report("persist", StepStatus.Succeeded, escalation.Number);
            }
            catch (Exception ex)
            {
                // Nothing external has happened yet, so this one CAN be rolled back: revert and report Failed.
                if (escalation != null) _escalations.Remove(escalation.Id);
                _workOrders.Revert(command.WorkOrderId, previousStatus, previousVersion);
                Report("persist", StepStatus.Failed, ex.Message);
                _compensation.Record(CompensationKind.RevertedPersist, command.WorkOrderId, null, "persist", ex.Message,
                    "work order reverted; nothing half-applied", command.CorrelationId);
                _trace.Write($"Service: → Failed at persist — {ex.GetType().Name}: {ex.Message}; reverted");
                return new WorkflowResult(false, $"The escalation could not be saved: {ex.Message}", "reverted")
                {
                    Outcome = WorkflowOutcome.Failed,
                    Steps = steps,
                    CorrelationId = command.CorrelationId,
                    NextAction = "reload the work order and try again",
                };
            }

            // ── 4. notify (external — outside any transaction) ─────────────────────────────────────────
            Report("notify", StepStatus.Running, "");
            string compensationAction = null;
            var approver = _directory.Find(command.TenantId, command.ApproverId);
            try
            {
                var receipt = await _notifications.SendApproverNotificationAsync(escalation, approver);
                _escalations.SetNotificationStatus(escalation.Id, NotificationStatus.Sent);
                Report("notify", StepStatus.Succeeded, receipt.MessageId);
            }
            catch (NotificationFailedException ex)
            {
                // COMPENSATION: the escalation is valid and stays. Record that the notification is outstanding
                // and queue it for manual review / retry — do not pretend the whole thing never happened.
                _escalations.SetNotificationStatus(escalation.Id, NotificationStatus.ManualReview);
                var entry = _compensation.Record(CompensationKind.NotificationOutstanding, command.WorkOrderId, escalation.Id,
                    "notify", ex.Message, $"manual-review queued: retry approver notification for {escalation.Number}", command.CorrelationId);
                compensationAction = $"manual-review queued (#{entry.Id})";
                Report("notify", StepStatus.Compensated, $"{ex.Message} → {compensationAction}");
            }

            // ── 5. audit ───────────────────────────────────────────────────────────────────────────────
            Report("audit", StepStatus.Running, "");
            try
            {
                await _audit.WriteAsync("escalation.created", escalation.Number, command.RequestedBy, command.CorrelationId,
                    compensationAction == null ? "approver notified" : $"approver NOT notified — {compensationAction}");
                Report("audit", StepStatus.Succeeded, "written");
            }
            catch (AuditWriteException ex)
            {
                // The notification has already gone out and cannot be unsent. Record the audit gap explicitly and alert.
                var entry = _compensation.Record(CompensationKind.AuditGap, command.WorkOrderId, escalation.Id,
                    "audit", ex.Message, $"audit gap recorded for {escalation.Number}; ops alerted; notification stays sent", command.CorrelationId);
                compensationAction = compensationAction == null
                    ? $"audit-gap recorded, ops alerted (#{entry.Id})"
                    : $"{compensationAction}; audit-gap recorded (#{entry.Id})";
                Report("audit", StepStatus.Compensated, $"{ex.Message} → audit gap #{entry.Id}");
            }

            // ── result ─────────────────────────────────────────────────────────────────────────────────
            var result = compensationAction == null
                ? new WorkflowResult(true, "Escalation created and approver notified.")
                {
                    Outcome = WorkflowOutcome.Created,
                    EscalationId = escalation.Id,
                    Steps = steps,
                    CorrelationId = command.CorrelationId,
                    NextAction = "nothing — the approver has been notified",
                }
                : new WorkflowResult(true, "Escalation created — approver NOT notified.", compensationAction)
                {
                    Outcome = WorkflowOutcome.CreatedWithCompensation,
                    EscalationId = escalation.Id,
                    Steps = steps,
                    CorrelationId = command.CorrelationId,
                    NextAction = "the manual-review queue retries the notification; nothing to redo",
                };

            _trace.Write($"Service: → {result}");
            return result;
        }

        /// <summary>The full command check — every step's rules plus the cross-field ones only the command can answer.</summary>
        private List<FieldError> ValidateCommand(EscalationCommand command)
        {
            var state = new EscalationWizardState
            {
                WorkOrderId = command.WorkOrderId,
                WorkOrderVersion = command.WorkOrderVersion,
                TenantId = command.TenantId,
                Reason = command.Reason,
                ApproverId = command.ApproverId,
                DueAtLocal = command.DueAt.LocalDateTime,
                NotifyEmail = command.Channels.HasFlag(NotificationChannels.Email),
                NotifyInApp = command.Channels.HasFlag(NotificationChannels.InApp),
                NotifySms = command.Channels.HasFlag(NotificationChannels.Sms),
            };
            state.Attachments.AddRange(command.Attachments);

            var errors = ValidateStep(state, WizardStep.Review).Errors.ToList();

            if (_workOrders.Find(command.WorkOrderId) == null)
                errors.Add(new FieldError(WizardStep.Reason, "WorkOrder", $"WO-{command.WorkOrderId} does not exist"));
            if (!string.IsNullOrEmpty(command.ApproverId) && command.ApproverId == command.RequestedBy)
                errors.Add(new FieldError(WizardStep.Approver, "Approver", "you cannot approve your own escalation"));

            return errors;
        }

        #endregion

        #region Resumable: the manual-review queue retries the notification later

        public async Task<WorkflowResult> RetryNotificationAsync(CompensationEntry entry, SessionContext session)
        {
            string correlationId = session.NextCorrelation();
            _trace.Write($"Service: RetryNotificationAsync [{correlationId}] compensation #{entry.Id} ({entry.Kind})");

            if (entry.Kind != CompensationKind.NotificationOutstanding || !entry.EscalationId.HasValue)
            {
                return new WorkflowResult(false, $"#{entry.Id} is a {entry.Kind} entry — it is resolved by an operator, not by a retry.")
                {
                    Outcome = WorkflowOutcome.Failed, CorrelationId = correlationId, NextAction = "review the audit gap manually",
                };
            }

            var escalation = _escalations.Find(entry.EscalationId.Value);
            var approver = _directory.Find(escalation.TenantId, escalation.ApproverId);
            try
            {
                var receipt = await _notifications.SendApproverNotificationAsync(escalation, approver);
                _escalations.SetNotificationStatus(escalation.Id, NotificationStatus.Sent);
                _compensation.Resolve(entry, $"notification retried and sent ({receipt.MessageId})");
                await _audit.WriteAsync("escalation.notify.retried", escalation.Number, session.User.UserName, correlationId, receipt.MessageId);
                return new WorkflowResult(true, $"{escalation.Number}: approver notified on retry — the workflow finished later, audit trail intact.")
                {
                    Outcome = WorkflowOutcome.Created, EscalationId = escalation.Id, CorrelationId = correlationId,
                };
            }
            catch (NotificationFailedException ex)
            {
                _trace.Write($"Service: retry failed — {ex.Message}; compensation #{entry.Id} stays open");
                return new WorkflowResult(true, $"{escalation.Number}: retry failed ({ex.Message}) — still queued.", $"manual-review still open (#{entry.Id})")
                {
                    Outcome = WorkflowOutcome.CreatedWithCompensation, EscalationId = escalation.Id, CorrelationId = correlationId,
                };
            }
        }

        #endregion
    }
}
