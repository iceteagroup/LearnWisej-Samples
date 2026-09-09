using System;
using System.Collections.Generic;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services.Workflow
{
    /// <summary>
    /// Everything the wizard collected, typed and immutable. The workflow accepts nothing else, so the
    /// same command can be built in a unit test or a batch job without any UI (review question 1).
    /// </summary>
    public sealed record EscalationCommand(
        int WorkOrderId,
        int WorkOrderVersion,
        string TenantId,
        string Reason,
        IReadOnlyList<Attachment> Attachments,
        string ApproverId,
        DateTimeOffset DueAt,
        NotificationChannels Channels,
        string RequestedBy,
        string CorrelationId);
}
