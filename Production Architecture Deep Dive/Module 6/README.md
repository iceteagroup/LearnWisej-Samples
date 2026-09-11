# TicketOps · Production Architecture Deep Dive · Module 6

Local lab build for **Module 6 · Modal Workflows, Dialog Result Objects & Transactional UI**. It follows
the walkthrough video: the Work Orders screen of the TicketOps Console opens an **`ApprovalDialog`** for
the selected work order (WO-2002 · *Repair loading dock pump*), awaits it with `ShowDialogAsync`, and
reads one typed **`ApprovalDialogResult`** — never the dialog's controls. Only a confirmed result reaches
**`ApprovalService.ApplyAsync`**, which re-checks the rules and commits status, audit row and notification
as **one transaction**. Cancel, ✕ and a rejection without comments leave the work order exactly as it was.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 6\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5106
```

Then open <http://localhost:5106>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try in the Work Orders window

The dark strip under the buttons is the workflow status the video shows (*Work order 2002 selected.*, *Approval dialog open — work order 2002 untouched.* …).

| Action | Path | What you should see |
|---|---|---|
| WO-2002 selected → **✓ Approve / Reject…** → keep **Approve**, **Confirm** | success | the modal dialog *Approve Work Order 2002*; on Confirm the grid refreshes, the card reads **Status: Approved by approver@ticketops …**; status **● Work order WO-2002 approved.** |
| **✓ Approve / Reject…** → **Cancel** (or the title-bar **✕**) | cancel = unchanged | strip **Approval canceled — work order 2002 unchanged.**; status **● unchanged** |
| **✓ Approve / Reject…** → **Reject**, leave comments empty, **Confirm** | validation inside the dialog | the hint *required when rejecting* appears; the dialog **stays open** with **Comments are required when rejecting.** Type a comment and Confirm → the order is rejected |
| Select an order that is already decided | domain rule | **✓ Approve / Reject…** is disabled; the card shows who decided and when |
| **↻ Refresh** | — | reloads the queue |

If the commit fails, nothing is applied (status, audit and notification roll back together), the details go to the
log and the user sees **The action could not be completed. Check the log for details.**

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | `ApprovalDialog` form — one decision (Approve / Reject), comments, Confirm / Cancel / ✕; comments required for Reject, validated inside the dialog before it can close with OK | `TicketOps/Dialogs/ApprovalDialog.cs` + `.Designer.cs` |
| 2 | `ApprovalDialogResult` class — typed: `Confirmed`, `Action`, `Comments`; never a bare `DialogResult` | `TicketOps/Dialogs/ApprovalDialogResult.cs` |
| 3 | `ApprovalService` method — `IApprovalService.ApplyAsync(workOrderId, result)` re-checks the result and `WorkOrder.CanDecide`, translates to an `ApprovalCommand`, commits status + audit + notification as one transaction (`ExecuteAsync(command)` for callers without a dialog) | `TicketOps/Services/IApprovalService.cs`, `ApprovalService.cs`, `Domain/ApprovalCommand.cs`, `Data/IWorkOrderRepository.cs` (`IWorkOrderTransaction`), `Data/InMemoryWorkOrderRepository.cs` |
| 4 | Unit-style test cases for result handling — approve, reject with comments, reject without comments refused, cancel = unchanged, already decided, failed commit → rollback → recovery | [`docs/TestCases.md`](TicketOps/docs/TestCases.md) · code: `TicketOps/Diagnostics/ResultHandlingTests.cs` |
| 5 | Workflow diagram | [`docs/WorkflowDiagram.md`](TicketOps/docs/WorkflowDiagram.md) + [`docs/ApprovalWorkflow.svg`](TicketOps/docs/ApprovalWorkflow.svg) |
| 6 | Every path visible without leaking internals | `WorkOrderQueue.ShowResult` / `ReportFailure`, `Resources/Strings.cs` |
| 7 | Production-readiness note | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |
| — | Designer & code review checklist, applied | [`docs/ReviewChecklist.md`](TicketOps/docs/ReviewChecklist.md) |

## Where things live

```
TicketOps/
├─ Views/WorkOrderQueue           the screen: buttonReview_Click opens the dialog, awaits it, gates on DialogResult.OK + Result.Confirmed
├─ Dialogs/
│  ├─ ApprovalDialog              the modal checkpoint: private controls, validation in Confirm, one public Result
│  └─ ApprovalDialogResult.cs     the typed contract (Confirmed / Action / Comments) — plain data
├─ Controls/StatusBanner          reusable "● state" + banner UserControl (display only)
├─ Services/                      IApprovalService / ApprovalService: three gates, result → ApprovalCommand, one transaction
├─ Domain/                        WorkOrder (CanDecide / Decide), ApprovalCommand (+ ApprovalRecord), OperationResult
├─ Data/                          IWorkOrderRepository + IWorkOrderTransaction, InMemoryWorkOrderRepository (working-copy commit)
├─ Infrastructure/                ILog / ActivityLog (server console), AppComposition (per session, the trusted CurrentUser)
├─ Resources/Strings.cs           safe user-facing messages
├─ Diagnostics/ResultHandlingTests.cs  the six unit-style cases, each on its own fixture
├─ docs/                          the deliverables
├─ Program.cs                     Wisej.NET session entry point → AppComposition
└─ Startup.cs                     Kestrel host (app.UseWisej())
```

## Self-check answers (lesson guide)

- **What code runs if the dialog is closed with X?**
  None of ours. `Result` is still the default `ApprovalDialogResult.NotConfirmed()`, `ShowDialogAsync` returns
  `DialogResult.Cancel`, and the caller's gate returns before any service call.
- **Does anything change before you see `DialogResult.OK`?**
  No. The dialog reads the `WorkOrder` into a label and never writes to it; the radio buttons and the
  comments live in private controls until Confirm builds the result. Cancel, ✕ and a failed validation
  leave the store untouched (RH-4 proves the service refuses an unconfirmed result even if it were called).
- **Can you read the outcome without touching a single control?**
  Yes: `dialog.Result` (`Confirmed`, `Action`, `Comments`). `grep "dialog\." Views/WorkOrderQueue.cs`
  finds only `dialog.ShowDialogAsync()` and `dialog.Result`.
- **Can the approval service be called without the dialog?**
  Yes — that is what the six test cases do: they build an `ApprovalDialogResult` (or an `ApprovalCommand`)
  and call `ApplyAsync` / `ExecuteAsync` directly. No Wisej.NET type appears in `Services/`, `Domain/` or `Data/`.
- **Where is role authorization checked?**
  Nowhere yet — deliberately listed as the first item of the production-readiness note. It belongs in
  `ApprovalService.ExecuteAsync` as Gate 0, using the trusted identity `AppComposition` hands the service
  (Module 11 replaces the constant with `Application.User`), never in the dialog.
