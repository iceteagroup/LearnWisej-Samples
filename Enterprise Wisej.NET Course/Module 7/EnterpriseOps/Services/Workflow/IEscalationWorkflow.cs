using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services.Workflow
{
    /// <summary>Progress callback: one orchestration step started or finished (the wizard's status strip).</summary>
    public sealed record WorkflowProgress(string Step, StepStatus Status, string Detail);

    /// <summary>
    /// The workflow service — the single owner of the escalation transition. The interface is what the
    /// video shows (EscalateAsync(command)); the extra members let the wizard stay thin:
    /// ValidateStep answers "may the user leave this step?", BuildCommand turns state into the command.
    /// </summary>
    public interface IEscalationWorkflow
    {
        Task<WorkflowResult> EscalateAsync(EscalationCommand command);

        Task<WorkflowResult> EscalateAsync(EscalationCommand command, IProgress<WorkflowProgress> progress);

        StepValidation ValidateStep(EscalationWizardState state, WizardStep step);

        EscalationCommand BuildCommand(EscalationWizardState state, SessionContext session);

        /// <summary>
        /// The Approver step's list. It goes through the workflow on purpose: a wizard page that held a
        /// reference to IApproverDirectory would be a Form talking to an integration.
        /// </summary>
        Task<IReadOnlyList<Approver>> LookupApproversAsync(string tenantId, CancellationToken cancellationToken);

        /// <summary>Resumable: the manual-review queue retries the notification later.</summary>
        Task<WorkflowResult> RetryNotificationAsync(CompensationEntry entry, SessionContext session);
    }
}
