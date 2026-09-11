# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 7

Local lab build for **Module 7 · Advanced Workflow UX: Wizards, Modal Orchestration & Compensation**. It follows
the walkthrough video: the **Escalation Wizard** — six steps (Reason · Attachments · Approver · Due date ·
Notifications · Review) opened modally with `ShowDialogAsync` — fills one typed `EscalationCommand`, and
**`EscalationWorkflow : IEscalationWorkflow`** executes the whole transition
(`validate → authorize → persist → notify → audit`) and returns one typed `WorkflowResult`.

The lab's failure path is the video's: the **notification fails after the persist succeeded**. The escalation is
kept, a compensation is recorded in the manual-review queue, the audit entry explains the gap, and the queued
entry can be retried later. The simulated failure lives in `FakeNotificationGateway`: its SMTP relay times out on
the **first e-mail of the session** and delivers every send after that.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine, with in-memory data only.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Enterprise Wisej.NET Course\Module 7\EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5207
```

Then open <http://localhost:5207>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes for both target frameworks with 0 warnings and 0 errors.

## What's on screen

- **Work queue** (`WorkQueuePage`): header title, the queue grid (`dgvWorkQueue`), **Escalate work order…**
  (`btnEscalate`), a message banner (`lblBanner`), and the **manual-review queue** (`lstCompensation`) with
  **Retry notification** (`btnRetryNotification`).
- **Escalation Wizard** (`EscalationWizard`, modal): the steps rail (`stepsRail`), one panel per step,
  `lblValidation`, **Cancel / Back / Next** (Next becomes **Finish** on Review, **Close** after it), and the dark
  status strip (`lblWizardStatus`) the video shows under the wizard.

## What to click

| Action | Path | What you should see |
|---|---|---|
| Select **WO-100232** (Critical) → **Escalate work order…** → six steps → **Finish** (first e-mail of the session) | failure + compensation | amber banner *"Escalation created — approver NOT notified."*, `persist ✓ · notify ✕ · CompensationAction: manual-review queued (#1)`, the `WorkflowResult` line in the status strip; after **Close** the row is `Escalated` and the manual-review queue has an open entry. **The escalation is kept.** |
| Select the open entry → **Retry notification** | recovery | the notification is re-sent, the entry is resolved, `escalation.notify.retried` is audited under a new correlation id |
| Escalate another work order → **Finish** | success | green banner *"Escalation created and approver notified."* |
| **Next** with an empty reason | validation | stays on step 1: *"Reason: a reason is required"*, nothing persisted |
| Critical row, **Next** on step 2 with no file | validation | *"a Critical escalation needs at least one attachment"* |
| `ben.tech` as approver → **Next** | validation (security) | *"ben.tech is a Technician — only Supervisors and Managers approve"* |
| `ana.ops` as approver → **Finish** | validation at the command | `EscalateAsync` returns `ValidationFailed` (*"you cannot approve your own escalation"*) and the wizard **jumps back to step 3** because `result.FirstFailingStep` said so |
| **Cancel → Yes**, then **Escalate…** again | resume | the wizard reopens on the same step with every answer; the status strip says *Draft resumed* |
| **Cancel → No** | cancel | draft removed, staged uploads cleaned up; nothing was persisted |

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | Wizard screen flow | [`EnterpriseOps/docs/WizardScreenFlow.md`](EnterpriseOps/docs/WizardScreenFlow.md) + [`wizard-screen-flow.svg`](EnterpriseOps/docs/wizard-screen-flow.svg) · code: `UI/EscalationWizard.cs` / `.Designer.cs` |
| 2 | Workflow service | [`EnterpriseOps/docs/WorkflowService.md`](EnterpriseOps/docs/WorkflowService.md) · code: `Services/Workflow/EscalationWorkflow.cs`, `IEscalationWorkflow.cs` |
| 3 | Typed command/result objects | [`EnterpriseOps/docs/TypedCommandAndResult.md`](EnterpriseOps/docs/TypedCommandAndResult.md) · code: `Services/Workflow/EscalationCommand.cs`, `WorkflowResult.cs`, `EscalationWizardState.cs`, `StepValidation.cs` |
| 4 | Failure path matrix | [`EnterpriseOps/docs/FailurePathMatrix.md`](EnterpriseOps/docs/FailurePathMatrix.md) |
| 5 | Compensation example | [`EnterpriseOps/docs/CompensationExample.md`](EnterpriseOps/docs/CompensationExample.md) · code: `EscalationWorkflow` steps 4–5, `CompensationLog.cs` |
| — | Production-readiness note | [`EnterpriseOps/docs/ProductionReadinessNote.md`](EnterpriseOps/docs/ProductionReadinessNote.md) |

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Open the project and run it locally | `EnterpriseOps.slnx`, `Properties/launchSettings.json` (port 5207), `Startup.cs`, `Program.cs` |
| Orchestration in a service, not in the wizard pages | `Services/Workflow/` — `IEscalationWorkflow`, `EscalationWorkflow`, `EscalationCommand`, `WorkflowResult`, `EscalationWizardState`, `WizardStep`, `StepValidation`, `WorkflowStateStore`, `CompensationLog` |
| Escalation Wizard (reason, attachments, approver, due date, notifications) | `UI/EscalationWizard.*` (`stepsRail`, six panels, `btnBack`/`btnNext`/`btnCancel`), opened from `UI/WorkQueuePage.btnEscalate_Click` |
| Compensation for a simulated notification failure | `FakeNotificationGateway` (first e-mail times out), `EscalationWorkflow` `catch (NotificationFailedException)`, `CompensationLog.Record`, `RetryNotificationAsync` |
| Show every path | `ShowResult(WorkflowResult)` in both screens: switch on `Outcome`, generic message + correlation id on `Failed` |
| Review & run: production-readiness note | `docs/ProductionReadinessNote.md` |

## Student review questions, answered against this sample

- **Can the workflow be tested without the wizard?**
  Yes. `IEscalationWorkflow.EscalateAsync(EscalationCommand)` takes a plain immutable record and returns a plain
  record; the workflow's constructor takes interfaces and stores, never a control. A test builds the command in
  code (see `docs/WorkflowService.md`) and gets the same outcomes and compensations as a wizard run.

- **What state survives browser refresh?**
  Everything in `EscalationWizardState`: the reason, the staged attachments, the approver, the due date, the
  channels, the current step and the set of completed steps — because the wizard calls
  `WorkflowStateStore.Save(state)` after every completed step, and that store lives on the server, not in the
  page. What does **not** survive is the half-typed content of the step you are on (it is copied into the state on
  Next/Back/Cancel) and anything that only ever existed in a control.

- **What happens after notification succeeds but audit logging fails?**
  The escalation stands and the notification stays sent — a message that has left cannot be unsent, so there is
  nothing to roll back. The workflow records an `AuditGap` compensation (kind, escalation, cause, correlation id),
  alerts ops through the same queue, and still returns `Success = true` with
  `Outcome = CreatedWithCompensation`. **Retry notification** refuses that entry; an operator closes it.

## Where things live

```
Module 7/
├─ EnterpriseOps.slnx
└─ EnterpriseOps/
   ├─ UI/
   │  ├─ WorkQueuePage.cs / .Designer.cs      the queue + the manual-review queue
   │  └─ EscalationWizard.cs / .Designer.cs   the modal wizard: stepsRail + six panels + Cancel/Back/Next
   ├─ Services/
   │  ├─ SessionServices.cs                   per-session composition root (page + wizard share it)
   │  ├─ SessionContext.cs  ActivityTrace.cs (server log)  WorkQueueRow.cs
   │  └─ Workflow/                            IEscalationWorkflow, EscalationWorkflow, EscalationCommand,
   │                                          WorkflowResult, EscalationWizardState, WorkflowStateStore,
   │                                          CompensationLog, WizardStep, StepValidation, WorkflowProgressObserver
   ├─ Domain/        WorkOrder.cs, Escalation.cs (+ Attachment, Approver, NotificationChannels)
   ├─ Data/          InMemoryWorkOrderStore, InMemoryEscalationStore, AttachmentStaging, SeedData (48 rows)
   ├─ Security/      UserRole.cs, PermissionService.cs, AuditLog.cs
   ├─ Integrations/  INotificationGateway + Fake (SMTP timeout on the first e-mail), IApproverDirectory + Fake
   ├─ docs/          the five deliverables + the SVG + the production-readiness note
   ├─ Program.cs     Application.MainPage = new UI.WorkQueuePage()
   └─ Startup.cs     Kestrel host (app.UseWisej())
```

## Verified / unverified

Verified in earlier course samples on the same framework build: `ShowDialogAsync()` in an `async void` handler,
`MessageBox.ShowAsync(..., MessageBoxButtons.YesNo, ...)`, `AlertBox.Show(..., alignment: TopRight, autoCloseDelay: 4000)`,
`Application.Update(this)` after an `await`, an instance-field `CancellationTokenSource`, `DataGridView` with
`AutoGenerateColumns = false` + `BindingSource`, `ComboBox` `DataSource` + `DisplayMember` over auto-properties.

**(unverified)** — compile and follow the Wisej.NET XML docs, confirm at runtime: `FlowLayoutPanel` with
`FlowDirection.TopDown` / `WrapContents = false` for `stepsRail`; `DataGridView.ColumnHeadersVisible = false`;
`DateTimePicker` with a custom format and `ShowUpDown`; `Form.StartPosition = CenterParent` and `FormClosing`
firing when the modal is closed with the X; the ✓ / ✕ glyphs in labels.
