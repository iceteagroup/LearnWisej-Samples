# TicketOps · Production Architecture Deep Dive · Module 6

Local lab build for **Module 6 · Modal Workflows, Dialog Result Objects & Transactional UI**. It follows
the walkthrough video: the Work Orders screen of the TicketOps Console opens an **`ApprovalDialog`** for
the selected work order (WO-2002 · *Repair loading dock pump*), awaits it with `ShowDialogAsync`, and
reads one typed **`ApprovalDialogResult`** — never the dialog's controls. Only a confirmed result reaches
**`ApprovalService.ApplyAsync`**, which re-checks the rules and commits status, audit row and notification
as **one transaction**. Cancel, ✕ and a rejection without comments leave the work order exactly as it
was; a store outage during the commit rolls everything back and the user reads a safe message.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 6\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5106
```

Then open <http://localhost:5106>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with no warnings for both targets (`net10.0-windows`, `net10.0`).

## What to click in the Work Orders window

The left card is the queue and the selected work order; the right card is the **Activity trace ·
UI → Dialog → Service → Data (tx)**: every click is logged as it crosses a boundary
(`[UI]` → `[SVC]` → `[DOMAIN]` → `[DATA]` → `[UI]`), so you can see that the handler never read a control
of the dialog, never checked a rule and never wrote a row. The dark strip under the buttons is the
workflow status the video shows (*Approval dialog open — work order 2002 untouched.* …).

| Button | Path | What you should see |
|---|---|---|
| **✓ Approve / Reject…** with WO-2002 selected → keep **Approve**, type a comment or not, **Confirm** | success | `[UI] buttonReview_Click → new ApprovalDialog(WO-2002) · await ShowDialogAsync()`; a modal dialog *Approve Work Order 2002*; on Confirm: `[UI] ApprovalDialog.buttonConfirm_Click — Confirm → Result {confirmed:true, action:Approve, …} · DialogResult.OK`, `[UI] … DialogResult.OK · Result … → IApprovalService.ApplyAsync(#2002, result)`, `[SVC] ApprovalDialogResult → ApprovalCommand {…, by:approver@ticketops}`, `[DATA] … #2002 found`, `[DOMAIN] WorkOrder.Decide — WO-2002 status → Approved (in memory only until the commit)`, `[DATA] tx#1 begin`, `UPDATE … 1/3`, `INSERT INTO ApprovalAudit … 2/3`, `INSERT INTO Outbox … 3/3`, `commit — 3 of 3 statements applied atomically`, `[UI] OK · Work order WO-2002 approved.`; the grid refreshes, the selected card reads **Status: Approved by approver@ticketops …**; status **● Work order WO-2002 approved.** |
| **✓ Approve / Reject…** → **Cancel** (or the title-bar **✕**) | cancel = unchanged | `[UI] ApprovalDialog.buttonCancel_Click — Cancel → Result {confirmed:false} · DialogResult.Cancel` (✕: `[UI] ApprovalDialog.FormClosed — closed with ✕ (UserClosing) → Result {confirmed:false}`), then `[UI] … DialogResult.Cancel · Result {confirmed:false} → return (no service call, no mutation)`; no `[SVC]` line; strip **Approval canceled — work order 2002 unchanged.**; status **● unchanged** |
| **✓ Approve / Reject…** → choose **Reject**, leave comments empty, **Confirm** | validation inside the dialog | the hint *required when rejecting* appears next to COMMENTS; on Confirm the dialog **stays open** with the red line **Comments are required when rejecting.** and `⚠ [UI] ApprovalDialog.buttonConfirm_Click — validation failed … dialog stays open, nothing sent to the service`. Type a comment and Confirm → the success path with `action:Reject`, or Cancel |
| **▶ Run result-handling tests** | progress | six unit-style cases run one per tick on their own fixtures (the session queue is untouched): `[UI] Test RH-1 — Approve, confirmed — expect: …`, the fixture's `[DATA] seeded 6 work orders…`, the `[SVC]`/`[DOMAIN]`/`[DATA]` lines, `[UI] Test RH-1 — PASS · …` … RH-6 shows the outage + rollback + recovery; ends with **● tests 6/6 passed** and `6/6 passed — see docs/TestCases.md` |
| **Reject without comments** (WO-2002 selected) | failure 1 (rule re-checked in the service) | a forged confirmed rejection bypasses the dialog: `[UI] … forged result {confirmed:true, action:Reject, comments:""} (bypassing the dialog) → ApplyAsync`, `⚠ [SVC] ApprovalService.ExecuteAsync — #2002 rejected: comments are required when rejecting — nothing written`; no `[DATA]` write; orange banner **Comments are required when rejecting.**; status **● not applied**; strip **Validation failed — nothing was sent to the service.** |
| **Decide WO-2001 again** | failure 2 (domain rule) | WO-2001 is already approved: `⚠ [DOMAIN] WorkOrder.CanDecide — WO-2001 rejected: Only a pending work order can be approved or rejected (WO-2001 is already approved).`; orange banner with that sentence; the handler never knew the rule |
| **Simulate data outage**, then **✓ Approve / Reject…** → **Confirm** on a pending order | error path | `[DATA] tx#n begin`, `UPDATE WorkOrders … 1/3 on the working copy`, `✖ [DATA] InMemoryWorkOrderRepository — outage: INSERT INTO ApprovalAudit (WO-2003) in tx#n failed — timeout connecting to sql01:1433 …` (trace only), `⚠ [DATA] tx#n rolled back — 1 of 3 statements discarded, live store unchanged`, `✖ [UI] buttonReview_Click — caught DataOutageException — user sees the safe message`; red banner + toast **The action could not be completed. Check the log for details.**; the grid still shows the order **Pending** — nothing partially applied |
| **Recover the data store** (same button), then confirm the same decision again | recovery | the store answers again: `tx#n+1 … commit — 3 of 3 statements applied atomically`; status **● Work order WO-2003 approved.** |
| **↻ Refresh** | — | reloads the queue through `IApprovalService.GetQueueAsync()` |
| **Clear trace** | — | empties the right-hand card |

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | `ApprovalDialog` form — one decision (Approve / Reject), comments, Confirm / Cancel / ✕; comments required for Reject, validated inside the dialog before it can close with OK | `TicketOps/Dialogs/ApprovalDialog.cs` + `.Designer.cs` |
| 2 | `ApprovalDialogResult` class — typed: `Confirmed`, `Action`, `Comments`; never a bare `DialogResult` | `TicketOps/Dialogs/ApprovalDialogResult.cs` |
| 3 | `ApprovalService` method — `IApprovalService.ApplyAsync(workOrderId, result)` re-checks the result and `WorkOrder.CanDecide`, translates to an `ApprovalCommand`, commits status + audit + notification as one transaction (`ExecuteAsync(command)` for callers without a dialog) | `TicketOps/Services/IApprovalService.cs`, `ApprovalService.cs`, `Domain/ApprovalCommand.cs`, `Data/IWorkOrderRepository.cs` (`IWorkOrderTransaction`), `Data/InMemoryWorkOrderRepository.cs` |
| 4 | Unit-style test cases for result handling — approve, reject with comments, reject without comments refused, cancel = unchanged, already decided, outage → rollback → recovery | [`docs/TestCases.md`](TicketOps/docs/TestCases.md) · runnable: `TicketOps/Diagnostics/ResultHandlingTests.cs` + **▶ Run result-handling tests** |
| 5 | Workflow diagram | [`docs/WorkflowDiagram.md`](TicketOps/docs/WorkflowDiagram.md) + [`docs/ApprovalWorkflow.svg`](TicketOps/docs/ApprovalWorkflow.svg) |
| 6 | Every path visible without leaking internals | `WorkOrderQueue.ShowResult` / `ReportFailure`, `Resources/Strings.cs`, the trace panel |
| 7 | Production-readiness note | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |
| — | Designer & code review checklist, applied | [`docs/ReviewChecklist.md`](TicketOps/docs/ReviewChecklist.md) |

## Where things live

```
TicketOps/
├─ Views/
│  ├─ WorkOrderQueue.cs           the screen: buttonReview_Click opens the dialog, awaits it, gates on DialogResult.OK + Result.Confirmed
│  └─ WorkOrderQueue.Designer.cs  GENERATED-style layout (opens in the Wisej Designer) — no logic here
├─ Dialogs/
│  ├─ ApprovalDialog.cs           the modal checkpoint: private controls, validation in Confirm, one public Result
│  ├─ ApprovalDialog.Designer.cs  layout only
│  └─ ApprovalDialogResult.cs     the typed contract (Confirmed / Action / Comments) — plain data
├─ Controls/StatusBanner          reusable "● state" + banner UserControl (display only)
├─ Services/
│  ├─ IApprovalService.cs         GetQueueAsync · ApplyAsync(id, result) · ExecuteAsync(command)
│  └─ ApprovalService.cs          three gates, ApprovalDialogResult → ApprovalCommand, one transaction (no UI types)
├─ Domain/
│  ├─ WorkOrder.cs                record + its own rule (CanDecide / Decide); compiles without Wisej.NET
│  ├─ ApprovalCommand.cs          the confirmed intent as data (+ ApprovalRecord, the audit row)
│  └─ OperationResult.cs          success / safe explanation handed back to the screen
├─ Data/
│  ├─ IWorkOrderRepository.cs     persistence contract + IWorkOrderTransaction (Update / RecordAudit / QueueNotification / CommitAsync)
│  └─ InMemoryWorkOrderRepository fake store, seeded; buffered writes, working-copy commit, SimulateOutage fails the audit INSERT
├─ Infrastructure/
│  ├─ ILog.cs / ActivityLog.cs    cross-cutting logging (details stay here)
│  └─ AppComposition.cs           who gets what: one object graph per session, the trusted CurrentUser, no statics
├─ Resources/Strings.cs           safe user-facing messages (CommentsRequiredToReject, DecisionNotConfirmed, …)
├─ Diagnostics/
│  ├─ ActivityTracePanel          the live trace card
│  └─ ResultHandlingTests.cs      the six unit-style cases, each on its own fixture
├─ docs/                          the deliverables
├─ Program.cs                     Wisej.NET session entry point → AppComposition
└─ Startup.cs                     Kestrel host (app.UseWisej())
```

## Self-check answers (lesson guide)

- **What code runs if the dialog is closed with X?**
  None of ours except `ApprovalDialog_FormClosed`, which only logs. `Result` is still the default
  `ApprovalDialogResult.NotConfirmed()`, `ShowDialogAsync` returns `DialogResult.Cancel`, and the caller's
  gate returns before any service call — the trace shows *closed with ✕ … → return (no service call, no mutation)*.
- **Does anything change before you see `DialogResult.OK`?**
  No. The dialog reads the `WorkOrder` into a label and never writes to it; the radio buttons and the
  comments live in private controls until Confirm builds the result. Cancel, ✕ and a failed validation
  leave the store untouched (RH-4 proves the service refuses an unconfirmed result even if it were called).
- **Can you read the outcome without touching a single control?**
  Yes: `dialog.Result` (`Confirmed`, `Action`, `Comments`). `grep "dialog\." Views/WorkOrderQueue.cs`
  finds only `dialog.ShowDialogAsync()` and `dialog.Result`.
- **Can the approval service be called without the dialog?**
  Yes — that is what the six test cases and the two failure buttons do: they build an
  `ApprovalDialogResult` (or an `ApprovalCommand`) and call `ApplyAsync` / `ExecuteAsync` directly. No
  Wisej.NET type appears in `Services/`, `Domain/` or `Data/`.
- **Where is role authorization checked?**
  Nowhere yet — deliberately listed as the first item of the production-readiness note. It belongs in
  `ApprovalService.ExecuteAsync` as Gate 0, using the trusted identity `AppComposition` hands the service
  (Module 11 replaces the constant with `Application.User`), never in the dialog.
