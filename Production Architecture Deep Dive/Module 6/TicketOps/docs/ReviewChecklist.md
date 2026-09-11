# Designer & code review checklist — applied to Module 6

| Check | Result | Evidence |
|---|---|---|
| The screen remains usable in the Visual Studio / Wisej.NET Designer | ✔ | `Views/WorkOrderQueue.Designer.cs` and `Dialogs/ApprovalDialog.Designer.cs` hold the whole layout in `InitializeComponent()`; both forms keep a parameterless constructor; no logic in the generated files |
| Controls are named clearly enough for a teammate to follow the event code | ✔ | `gridWorkOrders`, `buttonReview`, `radioApprove`, `radioReject`, `textComments`, `buttonConfirm`, `buttonCancel`, `buttonRefresh`; handlers are `<control>_<event>` |
| Business logic is not trapped in visual event handlers | ✔ | the comments rule and the confirmed gate are re-checked in `ApprovalService`; the status rule is `WorkOrder.CanDecide`; the transaction is `InMemoryWorkOrderRepository.Commit`. `WorkOrderQueue.cs` reads `wo.IsPending` / `wo.Status` only to colour the labels and enable the button (display), never to decide anything |
| The dialog exposes a typed result; callers never read its controls | ✔ | `ApprovalDialog.Result` (`ApprovalDialogResult`: `Confirmed`, `Action`, `Comments`); `grep -n "dialog\." Views/WorkOrderQueue.cs` finds only `dialog.ShowDialogAsync()` and `dialog.Result` |
| Changes are applied only when `DialogResult.OK` is returned | ✔ | `if (outcome != DialogResult.OK \|\| !result.Confirmed) return;` precedes the only `ApplyAsync` call in the success path; the dialog never touches the `WorkOrder` it was given |
| Failure paths are visible, logged, and explained without leaking internals | ✔ | expected refusals → `ShowResult` (orange banner); a failed commit → `ReportFailure` (red banner + toast with `Strings.ActionFailed`; exception details in `ILog` only) |
| No per-user state is held in static fields | ✔ | `AppComposition` per session in `Program.Main`; statics are constants and pure helpers only |
| The deliverable can be reviewed without running the whole course | ✔ | `README.md` + this `docs/` folder; the app runs standalone on port 5106 |

## Common mistakes checked

| Mistake | Present? |
|---|---|
| Reading the dialog's controls after it closes (`dlg.txtReason.Text`) | no — `dialog.Result` only |
| Applying changes when the user cancelled or closed with ✕ | no — gate on `DialogResult.OK` and `Result.Confirmed`; RH-4 proves the service refuses an unconfirmed result too |
| Mutating the selected object inside the dialog as the user types | no — the dialog reads `WorkOrder` into a label and never writes to it |
| Using a non-modal window where a decision is required | no — `await dialog.ShowDialogAsync()` |
| Validating only in the caller, after the dialog closed | no — `buttonConfirm_Click` validates and keeps the dialog open with `DialogResult.None` |
| Putting the business decision (authorization, persistence) in the dialog | no — `ApprovalService` decides and commits; the dialog gathers intent |
