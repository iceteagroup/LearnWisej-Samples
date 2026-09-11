# Deliverable 5 — Memory / session audit notes

**Code:** `Diagnostics/SessionMemoryAudit.cs` · `UI/DiagnosticsPage.RegisterRetainedState` ·
`UI/DiagnosticsPage_Disposed`

## Why a session, not a request

A Wisej.NET server hosts many live sessions at once, and each one holds its state for as long as the user
has the tab open. A field that costs 12 MB is not "12 MB"; it is 12 MB **times the number of concurrent
users**. That is the whole difference between a per-request web app and a session-based one, and it is why
this review exists.

Budgets used here:

| Budget | Value | Meaning |
|---|---|---|
| `PerItemBudgetBytes` | 2 MB | one holder over this **fails** the audit |
| `PerSessionBudgetBytes` | 8 MB | the whole session's registered holders over this **fails** the audit |

An unbounded holder that is currently small is marked **REVIEW**, not failed: it is design debt to argue
about at the next review, not a red build. Only a measurement failure is a failure
(`AuditReport.Passed => Failing == 0 && TotalRetainedBytes <= PerSessionBudgetBytes`).

## Question 1 — which fields hold collections, and are they cleared?

Every holder declares itself to the audit with a reason, a lifetime and a live size probe, so the answer is
generated rather than remembered:

| Holder | Reason it is kept | Bounded | Typical size | Lifetime | Verdict |
|---|---|---|---|---|---|
| `InMemoryWorkOrderStore._rows` | the work-order table this session queries | yes — fixed at 150 rows | ≈ 0.05 MB | whole session | `ok` |
| `StructuredLog._entries` | the session's structured log, read by this page | yes — ring buffer, cap 200 | ≤ 0.09 MB | whole session, oldest dropped | `ok` |
| `DiagnosticsPage.lstStructuredLog.Items` | the log lines shown on the page | yes — trimmed to 60 lines | ≤ 0.02 MB | whole session | `ok` |

What the review challenges is the shape almost every real leak takes: a well-meant cache — "keep the big
report so the second open is instant" — as a collection in a field, filled once, cleared never. A
50,000-row report held that way is ≈ 11 MB per session; registered with the audit it would be flagged
`OVER BUDGET` with the advice *"unbounded and large — release it after use, or do not retain it at all"*.
If a report really must be cached, cache the *page* the user is looking at (50 rows), give it an expiry,
and clear it when the screen that needed it closes.

## Question 2 — which timers, subscriptions and tasks does this form start, and where are they stopped?

| Started | Where | Stopped |
|---|---|---|
| `timerLive` (1 s `Wisej.Web.Timer`) | `DiagnosticsPage_Load` | `DiagnosticsPage_Disposed` |
| `StructuredLog.EntryWritten` handler | `DiagnosticsPage` constructor | `DiagnosticsPage_Disposed` (`-=`) |
| `CancellationTokenSource _cts` | each `RunSearchAsync` | disposed on the next search and in `DiagnosticsPage_Disposed` |

```csharp
private void DiagnosticsPage_Disposed(object sender, EventArgs e)
{
    this.timerLive.Stop();                    // a running timer keeps the page alive
    _log.EntryWritten -= Log_EntryWritten;    // a live handler holds a reference to a disposed control
    _cts?.Dispose();
    _cts = null;
}
```

The audit reports timer state too (`SessionMemoryAudit.RegisterTimer`), so "is anything still ticking?" is
answered by the report rather than by reading the code.

The four classic disposal mistakes, and where each one is avoided here:

1. **A timer started in a form and never stopped** — keeps the form alive after it closes → stopped in
   `Disposed`.
2. **An event handler attached to a shared service** — holds a reference to a disposed control → the
   `EntryWritten` subscription is removed in `Disposed`.
3. **A `DbContext` kept in a field** — survives the operation it was created for → this sample keeps no
   long-lived context; `InMemoryWorkOrderStore` is the session's data and is bounded and registered.
4. **A background task capturing the form in a closure** — keeps everything the form references → the live
   refresh is a `Timer` tick, and `timerLive_Tick` checks `IsDisposed` and catches
   `ObjectDisposedException` before touching a control.

## Question 3 — which objects are retained for the whole session, and does each one need to be?

| Object | Needs the whole session? | Why |
|---|---|---|
| `SessionContext` (user, tenant, correlation factory) | **yes** | it *is* the session's identity; it holds no collections |
| `InMemoryWorkOrderStore` | yes, in this lab | it stands in for the database; bounded at 150 rows |
| `StructuredLog` | yes | the diagnostics page reads it; bounded at 200 entries |
| structured log `ListBox` items | yes | it is the screen; trimmed at 60 lines |

## Evidence — what the running app shows

- **Hover the health chip** → the `session memory` check reads
  `healthy — 3 holders, ≈0.1 MB retained (budget 8.0 MB)`, and `live refresh job` reads
  `running every 1000 ms`.
- The **Session & health** card shows the managed heap and working set, refreshed every second.
