# Unit-style test cases — result handling

*Module 6 deliverable · TicketOps Console*

The cases below are implemented in `Diagnostics/ResultHandlingTests.cs` and run inside the app from the
bottom-bar button **▶ Run result-handling tests** (one case per Timer tick, so the trace shows each one).
Every case builds its own fixture — a freshly seeded `InMemoryWorkOrderRepository` and an
`ApprovalService` — and hands the service an `ApprovalDialogResult` exactly as the dialog would. No
dialog, no control, no browser is involved, which is the point: the file would move to an xUnit project
unchanged (`[Fact]` per method, `Assert.True(outcome.Passed)`).

Fixture facts: the seed holds 6 work orders (WO-2001 **Approved**, WO-2006 **Rejected**, four pending) and
2 audit rows. `PendingOrder` = 2002 (*Repair loading dock pump*), `DecidedOrder` = 2001.

| Id | Case | Arrange | Act | Assert |
|---|---|---|---|---|
| **RH-1** | Approve, confirmed | fresh fixture | `ApplyAsync(2002, Confirm(Approve, "Quote checked…"))` | `Succeeded`; WO-2002 `Status == Approved`, `DecidedBy == "test-approver"`; `AuditCount == 3`; `NotificationCount == 1` |
| **RH-2** | Reject with comments, confirmed | fresh fixture | `ApplyAsync(2002, Confirm(Reject, "Pump model mismatch — needs a re-quote first."))` | `Succeeded`; `Status == Rejected`; `DecisionComments` equals the comments; `AuditCount == 3` |
| **RH-3** | Reject without comments (forged past the dialog) | fresh fixture | `ApplyAsync(2002, Confirm(Reject, "   "))` | `!Succeeded`; `Message == Strings.CommentsRequiredToReject`; `Status == Pending`; `AuditCount == 2`; `NotificationCount == 0` |
| **RH-4** | Not confirmed (Cancel / ✕) | fresh fixture | `ApplyAsync(2002, NotConfirmed())` | `!Succeeded`; `Message == Strings.DecisionNotConfirmed`; `Status == Pending`; `DecidedBy == null`; `AuditCount == 2` |
| **RH-5** | Decide an already-decided order | fresh fixture, snapshot WO-2001 | `ApplyAsync(2001, Confirm(Reject, "Trying to flip a closed decision."))` | `!Succeeded`; message contains *already approved*; status, `DecidedBy`, `DecisionComments` unchanged; `AuditCount == 2` |
| **RH-6** | Repository outage during commit, then recovery | fresh fixture, `SimulateOutage = true` | `ApplyAsync(2002, Confirm(Approve, …))` → catch; `SimulateOutage = false`; same call again | first call throws `DataOutageException`; after it `Status == Pending`, `DecidedBy == null`, `AuditCount == 2`, `NotificationCount == 0` (nothing partially applied); retry `Succeeded`, `Status == Approved`, `AuditCount == 3` |

## What each case proves

- **RH-1 / RH-2** — a confirmed result becomes one committed unit: status, audit row and notification change together.
- **RH-3** — the comments rule holds on the server side of the boundary, not only in the dialog's Confirm handler.
- **RH-4** — Cancel and ✕ are indistinguishable to the service and both are refused; the handler's gate is the first line, this is the second.
- **RH-5** — the domain rule (`WorkOrder.CanDecide`) is enforced by the service; the dialog never knew it and the screen never checked it.
- **RH-6** — the transaction boundary is real: the UPDATE that "ran" before the failing INSERT is discarded with the working copy, and the same confirmed result applies cleanly once the store is back.

## Not covered here (needs the browser)

The dialog's own behaviour — *Confirm with Reject and empty comments keeps the dialog open*, *Cancel/✕ leave
`Result.Confirmed == false`*, *the caller never reads a control* — is exercised by hand; see the
**Evidence** below and the README's *What to click* table. Those paths are UI behaviour and are reviewed
in the browser, exactly as the lab guide's step 9 asks.

## Evidence

Click **▶ Run result-handling tests**. The progress bar advances one step per case and the trace shows, per case,
`[UI] Test RH-n — <name> — expect: <expectation>`, the fixture's own `[DATA] InMemoryWorkOrderRepository — seeded 6 work orders, 2 audit rows`,
the `[SVC]` / `[DOMAIN]` / `[DATA]` lines the service produced, and finally `[UI] Test RH-n — PASS · <detail>`.
RH-6 shows the outage in the middle: `✖ [DATA] … outage: INSERT INTO ApprovalAudit (WO-2002) in tx#1 failed — timeout connecting to sql01:1433 …`,
`⚠ [DATA] tx#1 rolled back — 1 of 3 statements discarded, live store unchanged`, then `tx#2 … commit — 3 of 3 statements applied atomically`.
The status shows **● tests 6/6 passed** and the line under the progress bar reads `6/6 passed — see docs/TestCases.md`.
The session's own queue is never touched by the tests (each case used its own repository): WO-2002 is still pending afterwards.
