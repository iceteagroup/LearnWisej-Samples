# Deliverable 2 — Structured log example

**Code:** `Diagnostics/StructuredLog.cs` (`LogEntry`, `StructuredLog`, `SafeErrorMessage`) ·
`Diagnostics/DiagnosticsPerformancePatterns.cs` (`OperationTimer`) ·
`Services/WorkOrderService.RecordTiming`

## Fields, not prose

A log line written as a sentence can only be read. A log line written as fields can be *queried*.

```
Bad   SearchWorkOrders was slow for a fabrikam user just now
Good  {"ts":"2026-09-10T09:14:21.418Z","level":"warning","op":"SearchWorkOrders","elapsedMs":2340,
       "tenant":"fabrikam","user":"ana.ops","page":1,"pageSize":5000,"budgetMs":400,"budget":"over",
       "correlation":"5e8a13f7"}
```

The second one answers "every `SearchWorkOrders` over 400 ms in tenant `fabrikam` yesterday" with a filter.
The first one answers nothing.

`LogEntry.ToJsonLine()` always emits `ts`, `level` and `op` first and `correlation` last, with the
operation's own fields in between, so a shipper can index a stable envelope regardless of the operation.

## Every entry is written by an `OperationTimer`

Timing is a `using` block, so it cannot be forgotten and it survives an exception:

```csharp
using (new OperationTimer(SearchOperation, ctx.CorrelationId,
           (operation, elapsed, correlationId) => verdict = RecordTiming(operation, elapsed, correlationId, query, ctx, failed)))
{
    page = await _store.SearchAsync(query, ctx, ct);
}
```

`OperationTimer` knows only three things — the operation name, the correlation id, and where to send the
elapsed time. It has no opinion about budgets or log levels; `WorkOrderService.RecordTiming` asks
`PerformanceBudget` for the verdict and picks `Information` or `Warning` from it. The UI is not involved.

## The entries the lab produces

| Button | Level | Line (abridged) |
|---|---|---|
| **Run query · 50** | `information` | `{"ts":…,"level":"information","op":"SearchWorkOrders","elapsedMs":162,"tenant":"fabrikam","user":"ana.ops","page":1,"pageSize":50,"budgetMs":400,"budget":"ok","correlation":"9c44d2a1"}` |
| **Slow query · 5000** | `warning` | `{"ts":…,"level":"warning","op":"SearchWorkOrders","elapsedMs":2340,"tenant":"fabrikam","user":"ana.ops","page":1,"pageSize":5000,"budgetMs":400,"budget":"over","correlation":"5e8a13f7"}` |
| **Fix page size** | `information` | the same shape as the first line, `"pageSize":50`, `"budget":"ok"`, a new correlation id |

`elapsedMs` for a 50-row page is `140 + 50 × 0.44 ≈ 162 ms` and for 5,000 rows
`140 + 5000 × 0.44 = 2,340 ms` — the store's simulated latency is a function of the requested page size
(`InMemoryWorkOrderStore.SimulatedLatencyMs`), so the number is **measured**, not printed.

## The error entry, and the different thing the user is told

`StructuredLog.Error` writes the full detail server-side:

```csharp
all["exceptionType"] = ex.GetType().Name;
all["message"]       = ex.Message;
all["userMessage"]   = SafeErrorMessage.For(correlationId);
```

so an entry for a failed `SearchWorkOrders` (for example a dropped database connection) contains:

```json
{"ts":"2026-09-10T09:16:02.771Z","level":"error","op":"SearchWorkOrders","tenant":"fabrikam",
 "user":"ana.ops","pageSize":50,"exceptionType":"InvalidOperationException",
 "message":"Connection reset by peer while reading the result set (host sql-prod-02:1433, pool WorkOrders, spid 71).",
 "userMessage":"The operation could not be completed. Reference 7b3d1e04 — try again, or contact support and quote the reference.",
 "correlation":"7b3d1e04"}
```

The user is shown only the `userMessage`: what happened in plain words, the reference to quote, what to do
next. The host name, the pool, the spid and the exception type help an attacker more than they help the
user, so they stay on the server. On the diagnostics page itself the error entry is rendered as
`09:16:02 SearchWorkOrders · error · 7b3d1e04   (error detail is in the server log)` — the page shows what
happened without showing what it was.

## The log must not become the leak it exists to catch

`StructuredLog` is a bounded ring buffer (`Capacity = 200`; the oldest entry is dropped). It registers
itself with the session-memory audit as a holder with a reason and a lifetime, like every other collection
this session keeps. The on-screen list keeps only the last 60 lines. The live refresh timer writes **no**
entry per tick — a once-per-second log line would fill any sink and teach nothing.

## Evidence — what the running app shows

- The **Structured log** card fills from the bottom as you click; each line is one complete JSON object
  with the correlation id last.
- Click **Run query · 50** and then **Slow query · 5000**: two lines with the same `op`, different
  `elapsedMs`, different `pageSize`, different `budget` — and *different* correlation ids, because they are
  two user actions.
