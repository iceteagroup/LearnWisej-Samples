# Deliverable 2 — Workflow service

**Contract:** `Services/Workflow/IEscalationWorkflow.cs`
**Implementation:** `Services/Workflow/EscalationWorkflow.cs` (386+ lines, no `Wisej.Web` reference anywhere)

```csharp
public interface IEscalationWorkflow
{
    Task<WorkflowResult> EscalateAsync(EscalationCommand command);
    Task<WorkflowResult> EscalateAsync(EscalationCommand command, IProgress<WorkflowProgress> progress);

    StepValidation ValidateStep(EscalationWizardState state, WizardStep step);
    EscalationCommand BuildCommand(EscalationWizardState state, SessionContext session);
    Task<IReadOnlyList<Approver>> LookupApproversAsync(string tenantId, CancellationToken cancellationToken);

    Task<WorkflowResult> RetryNotificationAsync(CompensationEntry entry, SessionContext session);
}
```

`EscalateAsync(command)` is the shape the walkthrough shows. The other members exist so the **wizard can stay
thin**: it never invents a rule, never builds the command itself, and never holds a reference to a repository,
a gateway or the audit log.

## The transition it owns

| # | Step | What it does | Failure handling |
|---|---|---|---|
| 1 | `validate` | every step's rules + the cross-field ones (`ApproverId == RequestedBy`, unknown work order) | `ValidationFailed` with `FieldError(Step, Field, Message)`; nothing persisted |
| 2 | `authorize` | `PermissionService.CanEscalate(user, sessionTenant, workOrderTenant)`; already escalated / completed / cancelled rows are rejected | `Unauthorized` (or `ValidationFailed` for a wrong status); nothing persisted |
| 3 | `persist` | `InMemoryEscalationStore.AddAsync` + `MarkEscalated(id, expectedVersion)` + `AttachmentStaging.Commit` | **can** be rolled back (nothing external happened yet): escalation removed, work order reverted, `RevertedPersist` compensation recorded, `Failed` returned |
| 4 | `notify` | `INotificationGateway.SendApproverNotificationAsync` — the external step | **cannot** be rolled back: the escalation is kept, `NotificationOutstanding` compensation queued → `CreatedWithCompensation` |
| 5 | `audit` | `AuditLog.WriteAsync(action, subject, user, correlationId, detail)` | a sent notification cannot be unsent: `AuditGap` recorded and alerted → `CreatedWithCompensation` |

Every step reports through `IProgress<WorkflowProgress>` (`Services/Workflow/WorkflowProgressObserver.cs`, a
synchronous `IProgress` so the callback runs where the step happened) and lands in `WorkflowResult.Steps` as
`StepOutcome(Name, Status, Detail)` — the strip the wizard and the video draw.

## Why the boundary is drawn here

- **One owner for the transition.** The wizard, a batch job and a unit test all reach the same five steps in
  the same order. The lesson's failure mode — "approvals, assignments, validations, uploads and external checks
  scattered across page events" — is impossible when the page has nothing to scatter.
- **Security is server-side and service-side.** `PermissionService` is asked inside `authorize`, not in a
  `Form`. A wizard that checked roles would have hidden a security decision in the UI layer.
- **The integrations are the workflow's, not the screen's.** Even the Approver step's directory lookup goes
  through `LookupApproversAsync`, so `EscalationWizard` has no `using EnterpriseOps.Integrations`.
- **State ownership.** `EscalationWizardState` belongs to the workflow's namespace and is persisted by
  `WorkflowStateStore`; the screen holds a reference, not the truth.

## Testable without any UI

`WorkQueuePage.btnRunHeadless_Click` is the proof, and it is deliberately written the way a test would be:

```csharp
var command = new EscalationCommand(
    WorkOrderId: target.Id, WorkOrderVersion: target.Version, TenantId: "contoso",
    Reason: "Batch escalation: vendor SLA breached …", Attachments: new List<Attachment>(),
    ApproverId: "m.weber", DueAt: DateTimeOffset.Now.AddHours(8),
    Channels: NotificationChannels.Email | NotificationChannels.InApp,
    RequestedBy: "ana.ops", CorrelationId: session.NextCorrelation());

var result = await _services.Workflow.EscalateAsync(command);   // no wizard, no dialog, no controls
```

`SessionServices` (the per-session composition root) builds the workflow out of interfaces and fakes, so a test
project would substitute its own `INotificationGateway` / `IApproverDirectory` without touching the workflow.

## Evidence in the running app

- Click **Run workflow without the wizard**: the trace prints the same
  `Service: EscalateAsync […] → Service: ValidateStep… → Security: … → Data: persisted ESC-… → Integrations: sent …`
  sequence as a wizard run, and the dark strip shows the same `WorkflowResult` line.
- Every trace line is tagged with its layer (`UI →`, `Service:`, `Security:`, `Data:`, `Integrations:`) — the
  handler lines are always two or three, the decisions are always `Service:` or `Security:`.
