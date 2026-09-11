# Deliverable 3 — Typed command / result objects

**Files:** `Services/Workflow/EscalationCommand.cs`, `Services/Workflow/WorkflowResult.cs`,
`Services/Workflow/EscalationWizardState.cs`, `Services/Workflow/StepValidation.cs`, `Services/Workflow/WizardStep.cs`

## In: `EscalationCommand`

```csharp
public sealed record EscalationCommand(
    int WorkOrderId,
    int WorkOrderVersion,               // the optimistic-concurrency token travels with the intent
    string TenantId,
    string Reason,
    IReadOnlyList<Attachment> Attachments,
    string ApproverId,
    DateTimeOffset DueAt,
    NotificationChannels Channels,      // [Flags] Email | InApp | Sms
    string RequestedBy,
    string CorrelationId);
```

An immutable record with every field typed. It carries **intent**, not entities: the work order is referenced
by id + version, the approver by id, the channels by a flags enum. It can be built by the wizard
(`BuildCommand(state, session)`), by a batch job, or by a test — the workflow cannot tell the difference.

`EscalationWizardState` is the *mutable* half of the pair: the accumulated answers plus `CompletedSteps`, one
object, persisted server-side. `BuildCommand` is the only place the two shapes meet.

## Out: `WorkflowResult`

```csharp
public sealed record WorkflowResult(bool Success, string Message, string CompensationAction = null)
{
    public WorkflowOutcome Outcome { get; init; }          // Created · CreatedWithCompensation ·
                                                           // ValidationFailed · Unauthorized · Failed
    public int? EscalationId { get; init; }
    public string CorrelationId { get; init; }
    public IReadOnlyList<FieldError> FieldErrors { get; init; }   // (WizardStep Step, string Field, string Message)
    public IReadOnlyList<StepOutcome> Steps { get; init; }        // (string Name, StepStatus Status, string Detail)
    public string NextAction { get; init; }
    public WizardStep? FirstFailingStep => …;                     // where the wizard should jump
}
```

The three-field shape (`Success`, `Message`, `CompensationAction`) is exactly what the walkthrough shows; the
`init` members add what the lesson asks a typed result to answer:

| The lesson says the result must say… | Member |
|---|---|
| whether the escalation was created | `Outcome`, `EscalationId` |
| which validations failed and on which field | `FieldErrors` (step **and** field, so the UI can jump and highlight) |
| whether an external step succeeded | `Steps` (`notify` → `Succeeded` / `Compensated`) and `CompensationAction` |
| what the user should do next | `NextAction` |

`StepValidation` is the same idea for one step: the typed answer to *"may the user leave this step?"*, with the
errors already addressed to a field.

## How callers use it

The wizard and the page both **switch on `Outcome`** — never on the message, never on an exception type:

```csharp
switch (result.Outcome)
{
    case WorkflowOutcome.Created:                  // green banner, close
    case WorkflowOutcome.CreatedWithCompensation:  // amber banner + CompensationAction + manual-review queue
    case WorkflowOutcome.ValidationFailed:         // jump to result.FirstFailingStep, highlight the field
    default:                                       // Unauthorized · Failed → safe message + correlation id
}
```

A batch caller logs `result.ToString()` — one line with the outcome, the escalation number, the compensation
action and the correlation id — and needs no UI at all. That is why adding a new outcome is a compile-time
event for every caller instead of a silently unhandled string.

## Evidence in the running app

- The wizard's dark status strip prints the result verbatim:
  `WorkflowResult — Outcome: CreatedWithCompensation · Success: True · ESC-1041 · CompensationAction: manual-review queued (#1) · correlation 9d22e6c1`.
- Finish the first e-mail escalation of the session (the simulated SMTP relay times out): `Success` stays
  **true** — the escalation is valid — while `CompensationAction` is filled in. A boolean alone could not express that.
- Force a validation failure **at Finish**: pick **ana.ops** as the approver. Every step accepts her (she is a
  Manager, so `ValidateStep(Approver)` is happy), but the command-level rule *"you cannot approve your own
  escalation"* only exists where the whole command is visible. Finish returns `ValidationFailed`, and the wizard
  jumps back to step 3 because `result.FirstFailingStep` said so — no exception, no string parsing.
