# Deliverable 5 — Memory / session audit notes

**Code:** `Diagnostics/SessionMemoryAudit.cs` · `Services/ReportCacheService.cs` ·
`UI/DiagnosticsPage.RegisterRetainedState` · `UI/DiagnosticsPage_Disposed`

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
| `DiagnosticsPage.lstTrace.Items` | the on-screen activity trace | yes — trimmed to 400 lines | ≤ 0.05 MB | whole session | `ok` |
| `ReportCacheService._cached` | *"cache the big report so the second open is instant"* | **no** | **≈ 11.4 MB** when filled | **whole session — never cleared** | `REVIEW` empty · `OVER BUDGET` filled |

The last row is the deliberate mistake, and it is the shape almost every real leak takes: a well-meant
cache, a collection in a field, filled once, cleared never.

```csharp
// Services/ReportCacheService.cs — DELIBERATELY LEAKY
private List<WorkQueueRow> _cached;   // 50,000 rows ≈ 11.4 MB, for the whole session
```

`ReportCacheService.Retain` measures the damage rather than estimating it: `GC.GetTotalMemory(true)` before
and after, and the audit reports the larger of the model estimate and the measured growth.

The fix is one line — `_cached = null;` in `Release()` — called both by the **Release cache** button and by
the page's `Disposed` handler.

## Question 2 — which timers, subscriptions and tasks does this form start, and where are they stopped?

| Started | Where | Stopped |
|---|---|---|
| `timerLive` (1 s `Wisej.Web.Timer`) | `btnLive_Click` | `btnLive_Click` (toggle) **and** `DiagnosticsPage_Disposed` |
| `StructuredLog.EntryWritten` handler | `DiagnosticsPage` constructor | `DiagnosticsPage_Disposed` (`-=`) |
| `CancellationTokenSource _cts` | each `RunSearchAsync` | disposed on the next search and in `DiagnosticsPage_Disposed` |
| `ReportCacheService._cached` | `btnLeakSession_Click` | `btnReleaseLeak_Click` **and** `DiagnosticsPage_Disposed` |

```csharp
private void DiagnosticsPage_Disposed(object sender, EventArgs e)
{
    this.timerLive.Stop();                    // a running timer keeps the page alive
    _log.EntryWritten -= Log_EntryWritten;    // a live handler holds a reference to a disposed control
    _cts?.Dispose();
    _cts = null;
    _reportCache.Release();                   // drop the retained report
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
| trace `ListBox` items | yes | it is the screen; trimmed at 400 lines |
| `ReportCacheService._cached` | **no** | a convenience cache; it should be scoped to the report screen, capped, or not held at all |

The honest answer for the fourth row is that it should not be a session field. If the report really must be
cached, cache the *page* the user is looking at (50 rows), give it an expiry, and clear it when the screen
that needed it closes.

## Evidence — what the running app shows

1. Click **Leak: cache 50k rows**. The trace prints the audit line by line:
   `Diagnostics:   [OVER BUDGET] ReportCacheService._cached ≈11.4 MB · whole session — never cleared → unbounded and large — release it after use, or do not retain it at all`,
   the `managed heap` figure on the Session & health card jumps by roughly the same amount, a red banner
   appears, and the structured log gains a `RetainReport` warning entry with `heapBeforeBytes`,
   `heapAfterBytes` and `growthBytes`.
2. Click **Health check** while it is retained → the health chip goes `DEGRADED`, with
   `session memory — 1 over budget: ReportCacheService._cached ≈11.4 MB` in its tooltip.
3. Click **Release cache** → the trace shows `Service: ReportCacheService.Release() → 50,000 rows dropped;
   the GC can reclaim them`, the audit re-runs and passes (`ReportCacheService._cached` drops to `REVIEW`,
   because the *design* is still unbounded), the heap figure falls back, and the banner turns green.
4. Click **▶ Live refresh**, then run the audit again → the timer probe reports
   `timer timerLive (1 s) · RUNNING · stopped in btnLive_Click (toggle) and DiagnosticsPage_Disposed`.
   Stop it and the same line reads `stopped`.
