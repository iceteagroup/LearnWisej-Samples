# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 7

Local lab build for **Module 7 · Advanced Workflow UX: Wizards, Modal Orchestration & Compensation**. It follows
the walkthrough video: the **Escalation Wizard** — six designable steps (Reason · Attachments · Approver ·
Due date · Notifications · Review) opened modally with `ShowDialogAsync` — fills one typed `EscalationCommand`,
and **`EscalationWorkflow : IEscalationWorkflow`** executes the whole transition
(`validate → authorize → persist → notify → audit`) and returns one typed `WorkflowResult`.

The lab's failure path is the video's: the **notification fails after the persist succeeded**. The escalation is
kept, a compensation is recorded in the manual-review queue, the audit entry explains the gap, and the queued
entry can be retried later. An anti-pattern button writes the same flow inside the screen and deletes a valid
escalation when the e-mail bounces, so the two can be compared side by side.

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

## What to click

### Work queue (left card)

| Button | Path | What you should see |
|---|---|---|
| **Escalate work order…** | success | select a row (e.g. **WO-100232**, Critical) → the modal wizard opens on step 1. Complete the six steps → green banner *"Escalation created and approver notified."*, the row turns `Escalated` (version bumped), the dark strip prints the whole `WorkflowResult` |
| **Escalate…** with an empty reason | validation | Next stays on step 1: *"Reason: a reason is required"*, trace `Service: ValidateStep(Reason) → invalid`, nothing persisted |
| **Escalate…** → `ben.tech` as approver | validation (security) | *"ben.tech is a Technician — only Supervisors and Managers approve"* — the directory lists him, `PermissionService` refuses him |
| **Escalate…** → `ana.ops` as approver → Finish | validation at the command | every step passes, but `EscalateAsync` returns `ValidationFailed` (*"you cannot approve your own escalation"*) and the wizard **jumps back to step 3** because `result.FirstFailingStep` said so |
| **Cancel → Yes** then **Escalate…** again | resume | the wizard reopens on the same step with every answer, the rail says `resumed`, `draft <id>` and the saved progress |
| **Cancel → No** | cancel | draft removed and `Data: staging cleaned up — n upload(s) removed`; nothing was persisted |
| **Retry notification** | recovery | select the open `NotificationOutstanding` entry → it is re-sent, resolved, `escalation.notify.retried` audited under a new correlation id |
| **Resolve manually** | recovery | the only way to clear an `AuditGap` — a sent notification cannot be unsent |

### Bottom bar

| Button | Path | What you should see |
|---|---|---|
| **Fail: notification (SMTP)** | failure + compensation | arms the gateway. The next Finish shows `✓ validate → ✓ authorize → ✓ persist → ✕ notify + compensation → ✓ audit`, the amber banner *"Escalation created — approver NOT notified."*, `CompensationAction: manual-review queued (#1)`, and a new row in the manual-review queue. **The escalation is kept.** |
| **Fail: audit after notify** | failure + compensation | the notification goes out, the audit write throws → `AuditGap` recorded and alerted; the escalation and the notification both stand |
| **Fail: directory timeout** | timeout | the Approver step's lookup outlasts the wizard's 5 s `CancellationTokenSource`: *"The approver directory did not answer within 5 seconds. Your draft is saved"* — nothing half-applied |
| **Run workflow without the wizard** | success, no UI | builds an `EscalationCommand` in code and calls the same `EscalateAsync` — the answer to review question 1, visible in the trace |
| **Anti-pattern: logic in the page** | counter-example | the same flow inlined in the handler: re-invented rules, no correlation id, no audit — and the `catch` **deletes** the valid escalation. Red banner + two `UI ←` trace lines say exactly what was lost |
| **Clear trace** | — | empties the right-hand card |

The right-hand card is the live **Server · live activity trace**: every action and every decision with a
timestamp and its layer (`UI →`, `Service:`, `Security:`, `Data:`, `Integrations:`). It is how a reviewer sees
that the handlers decide nothing.

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
| Identify the architecture risk before touching the UI | `docs/WorkflowService.md` § *Why the boundary is drawn here*; the counter-example is `WorkQueuePage.btnAntiPattern_Click` |
| Create the service / command / model boundary **first** | `Services/Workflow/` — `IEscalationWorkflow`, `EscalationWorkflow`, `EscalationCommand`, `WorkflowResult`, `EscalationWizardState`, `WizardStep`, `StepValidation`, `WorkflowStateStore`, `CompensationLog` |
| Build the screen with the project standards | `UI/WorkQueuePage.*` (header bar, card, trace, bottom bar) and `UI/EscalationWizard.*` (`stepsRail`, six panels, `btnBack`/`btnNext`/`btnCancel`) |
| Deliverable · Wizard screen flow | `UI/EscalationWizard.ShowStep` / `RenderRail` + `docs/wizard-screen-flow.svg` |
| Deliverable · Workflow service | `EscalationWorkflow.EscalateAsync` (5 steps, `IProgress`) |
| Deliverable · Typed command/result | `BuildCommand(state, session)` → `EscalationCommand`; `WorkflowResult` with `Outcome`, `FieldErrors`, `Steps`, `NextAction` |
| Deliverable · Failure path matrix | `docs/FailurePathMatrix.md`; each cell has a button in the app |
| Deliverable · Compensation example | `EscalationWorkflow` `catch (NotificationFailedException)` / `catch (AuditWriteException)`, `CompensationLog.Record`, `RetryNotificationAsync` |
| Add a failure-path demonstration, not only the happy path | the three **Fail:** buttons, the wizard's validation and timeout paths, the anti-pattern button |
| Show every path without leaking internals | `ShowResult(WorkflowResult)` in both screens: switch on `Outcome`, generic message + correlation id on `Failed` |
| Review & run: production-readiness note | `docs/ProductionReadinessNote.md` |

## Student review questions, answered against this sample

- **Can the workflow be tested without the wizard?**
  Yes. `IEscalationWorkflow.EscalateAsync(EscalationCommand)` takes a plain immutable record and returns a plain
  record; the workflow's constructor takes interfaces and stores, never a control. **Run workflow without the
  wizard** is that test written as a button: it builds the command in code and produces the same trace, the same
  outcomes and the same compensations as a wizard run. The wizard adds three things on top — `ValidateStep` per
  step, `BuildCommand`, and an `IProgress` for the strip — and each of them is a service call the test can make too.

- **What state survives browser refresh?**
  Everything in `EscalationWizardState`: the reason, the staged attachments, the approver, the due date, the
  channels, the current step and the set of completed steps — because the wizard calls
  `WorkflowStateStore.Save(state)` after every completed step, and that store lives on the server, not in the
  page. What does **not** survive is the half-typed content of the step you are on (it is copied into the state on
  Next/Back/Cancel) and anything that only ever existed in a control. In production the store is a table keyed by
  user + work order, so the draft would also survive a new session on another device.

- **What happens after notification succeeds but audit logging fails?**
  The escalation stands and the notification stays sent — a message that has left cannot be unsent, so there is
  nothing to roll back. The workflow records an `AuditGap` compensation (kind, escalation, cause, correlation id),
  alerts ops through the same queue, and still returns `Success = true` with
  `Outcome = CreatedWithCompensation` so the caller knows the audit trail has a known hole. That entry cannot be
  cleared by **Retry notification** — only by **Resolve manually**, with who closed it and why. Arm
  **Fail: audit after notify** and finish a wizard run to watch it.

## Instructor acceptance criteria, answered

- **Follows the course architecture baseline** — folder-per-layer (`UI/`, `Domain/`, `Services/`, `Data/`,
  `Security/`, `Integrations/`, `docs/`) with namespaces to match, per-session services built in
  `Services/SessionServices.cs` and handed to the screens, no statics holding user or tenant state,
  `WorkQueueRow` projections instead of entities in the UI.
- **UI event handlers remain thin and explainable** — the longest handler on the happy path is
  `btnEscalate_Click`: select a row, open the wizard, `await ShowDialogAsync()`, show the typed result, refresh.
  The wizard's `btnNext_Click` is a `try`/`catch` around one of two service calls. The one deliberately fat
  handler is `btnAntiPattern_Click`, and its comment says why it is there.
- **Service-level logic can be reviewed without opening the designer** — every rule, permission check, external
  call and compensation is in `Services/Workflow/` and `Security/`; neither file references `Wisej.Web`.
- **At least one failure path is demonstrated** — five: step validation, command-level validation, directory
  timeout, notification failure with compensation, audit failure with compensation; plus the two recoveries and
  the anti-pattern counter-example.
- **The student can explain state ownership, security implications and production behaviour** —
  `docs/ProductionReadinessNote.md` §§ *State ownership*, *Security*, *What is deliberately simplified*.

## Where things live

```
Module 7/
├─ EnterpriseOps.slnx
└─ EnterpriseOps/
   ├─ UI/
   │  ├─ WorkQueuePage.cs / .Designer.cs      the queue, the manual-review queue, the failure-path bar
   │  └─ EscalationWizard.cs / .Designer.cs   the modal wizard: stepsRail + six panels + Back/Next/Cancel
   ├─ Services/
   │  ├─ SessionServices.cs                   per-session composition root (page + wizard share it)
   │  ├─ SessionContext.cs  ActivityTrace.cs  WorkQueueRow.cs
   │  └─ Workflow/
   │     ├─ IEscalationWorkflow.cs            the contract (EscalateAsync, ValidateStep, BuildCommand, …)
   │     ├─ EscalationWorkflow.cs             validate → authorize → persist → notify → audit + compensation
   │     ├─ EscalationCommand.cs              typed intent in
   │     ├─ WorkflowResult.cs                 typed outcome out (+ FieldError, StepOutcome, WorkflowOutcome)
   │     ├─ EscalationWizardState.cs          the one state object the six pages fill
   │     ├─ WorkflowStateStore.cs             server-side drafts — what survives a refresh
   │     ├─ CompensationLog.cs                the manual-review queue
   │     ├─ WizardStep.cs  StepValidation.cs  WorkflowProgressObserver.cs
   ├─ Domain/        WorkOrder.cs, Escalation.cs (+ Attachment, Approver, NotificationChannels)
   ├─ Data/          InMemoryWorkOrderStore, InMemoryEscalationStore, AttachmentStaging, SeedData (48 rows)
   ├─ Security/      UserRole.cs, PermissionService.cs, AuditLog.cs (FailNextWrite switch)
   ├─ Integrations/  INotificationGateway + Fake (FailNextSend), IApproverDirectory + Fake (HangNextLookup)
   ├─ docs/          the five deliverables + the SVG + the production-readiness note
   ├─ Program.cs     Application.MainPage = new UI.WorkQueuePage()
   └─ Startup.cs     Kestrel host (app.UseWisej())
```

## Verified / unverified

Used from the course cookbook and **verified** in earlier course samples on the same framework build:
`ShowDialogAsync()` in an `async void` handler, `MessageBox.ShowAsync(..., MessageBoxButtons.YesNo, ...)`,
`AlertBox.Show(..., alignment: ContentAlignment.TopRight, autoCloseDelay: 4000)`, `Application.Update(this)`
after an `await`, an instance-field `CancellationTokenSource`, `DataGridView` with `AutoGenerateColumns = false`
+ `BindingSource`, `Font("default"/"monospace", …)`, `Panel.BorderStyle = BorderStyle.Solid`, the page/card/trace
layout at 1348×680.

Used and marked **(unverified)** in the cookbook — these compile and are per the Wisej.NET XML docs, but were not
executed in a browser while building this sample; the reviewer should confirm them at runtime:

- **Wizard shape** — the cookbook's "a `Form`/`Page` with a `TabControl` (or Panel per step) driven by a workflow
  service, plus a `Label` 'Step 2 of 5'". This sample uses a `Panel` per step plus `lblStepCounter`.
- `Wisej.Web.FlowLayoutPanel` with `FlowDirection.TopDown` / `WrapContents = false` for `stepsRail`, and
  `Label.Padding` for the rail's indentation.
- `DataGridView.ColumnHeadersVisible = false` and `RowHeadersVisible = false` on the wizard's summary grid.
- `DateTimePicker` with `Format = DateTimePickerFormat.Custom`, `CustomFormat = "MMM d, yyyy  HH:mm"`,
  `ShowUpDown = true`.
- `Form.StartPosition = FormStartPosition.CenterParent` with `MaximizeBox`/`MinimizeBox` off for the modal, and
  `FormClosing` (`FormClosingEventHandler`) firing when the modal is closed with the X.
- `ComboBox` bound through `DataSource` + `DisplayMember` with `SelectedItem` set to a domain object.
- The ✓ / ✕ / → glyphs in labels and list items (font coverage in the browser).
