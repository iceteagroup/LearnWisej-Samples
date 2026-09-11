# Deliverable 4 — Performance budget table

**Code:** `Diagnostics/PerformanceBudget.cs` (`BudgetRow`, `BudgetStatus`) ·
`Diagnostics/DiagnosticsPerformancePatterns.cs` (`OperationTimer`) · `UI/PerfBudgetPanel.cs` ·
`Services/WorkOrderService.RecordTiming`

## The table

Budgets are measured at the points where a **user waits**, and each one is a number the team agreed to,
not a feeling.

| # | Operation | Budget | Measured where | What breaks it |
|---|---|---|---|---|
| 1 | **Startup** | ≤ 800 ms | `SessionContext` stopwatch: `Program.Main` → `DiagnosticsPage_Load` | work in the entry point, eager service construction, a synchronous config or schema call |
| 2 | **Screen load — Diagnostics** | ≤ 400 ms | stopwatch around the body of `DiagnosticsPage_Load` | building the whole screen before showing anything; querying in the constructor |
| 3 | **Query — SearchWorkOrders** | ≤ 400 ms | `OperationTimer` inside `WorkOrderService.SearchWorkOrdersAsync` | an unpaged query, a missing index, N+1 loading |
| 4 | **Binding refresh** | ≤ 150 ms | `PerfBudgetPanel.Bind` returns its own elapsed ms | rebinding a whole grid instead of updating changed rows |
| 5 | **Background job tick** | ≤ 250 ms | stopwatch around `timerLive_Tick` | doing real work on the UI timer; pushing more often than the browser can paint |

Rows 1, 2 and 4 are recorded during load and row 5 by the first live tick, so the table is already
populated the first time you look at it — a budget table full of "not measured" teaches nothing.

## The verdict is not the UI's

`PerformanceBudget.Record` is the only way a measurement gets in, and it is the only place OK / OVER is
decided:

```csharp
internal void Record(long measuredMs, string correlationId)
{
    MeasuredMs    = measuredMs;
    Status        = measuredMs <= BudgetMs ? BudgetStatus.Ok : BudgetStatus.Over;
    CorrelationId = correlationId;
    MeasuredUtc   = DateTime.UtcNow;
}
```

`PerfBudgetPanel` has no threshold of its own — it reads `BudgetRow.IsOver` and paints. That is why the
whole rule can be reviewed without opening a designer, and why the same verdict drives the health check
(`HealthCheck` reports `Degraded` while any row is over) as well as the colour of a cell.

Each row also keeps the **correlation id of the measurement**, so a red row is not an anonymous number: it
names the action that produced it, and that action's log line names the cause.

## The diagnosis walk-through — a slow query, not guessed

The store's latency is a function of the page size — `140 ms + 0.44 ms × pageSize`
(`InMemoryWorkOrderStore.SimulatedLatencyMs`) — so these numbers are measured, not printed:

| pageSize | measured | budget | status |
|---|---|---|---|
| 50 | ≈ 162 ms | ≤ 400 ms | `OK ✓` |
| 5,000 | ≈ 2,340 ms | ≤ 400 ms | `OVER ✕` |

The five steps the lesson asks the student to demonstrate, in order:

1. **Timer** — `OperationTimer` wraps the service call and, on `Dispose`, reports `2,340 ms`.
2. **Budget breach** — `PerformanceBudget.Record` compares it to 400 ms and sets `BudgetStatus.Over`; the
   `Query — SearchWorkOrders` row turns red on the table and the status bar reads
   `OperationTimer — SearchWorkOrders 2,340 ms · correlation 5e8a13f7 · OVER BUDGET`.
3. **Structured log line** — the entry that the same timer callback wrote:
   `{"op":"SearchWorkOrders","elapsedMs":2340,"tenant":"fabrikam","page":1,"pageSize":5000,"budgetMs":400,"budget":"over","correlation":"5e8a13f7"}`.
4. **Correlation id** — `5e8a13f7` on the row, on the log line, in the server trace and in the status bar.
   One action, one thread to pull.
5. **The offending operation** — `"pageSize": 5000`. Somebody bypassed the paged query. The field names the
   bug; nobody had to reproduce it locally or attach a debugger.

Then **Fix page size** runs the *same* operation with `pageSize` 50: ≈ 162 ms, the row turns green, and the
new log line proves the fix with a number rather than a claim.

## Rules the sample follows

- **Failures do not fake speed.** When the store throws, `RecordTiming` writes an entry with
  `"outcome":"failed"` and leaves the budget row untouched — a 3 ms exception is not a fast query.
- **Measure in production too.** Every budget here is computed from the running session, not from a
  build-time benchmark: data volume, concurrency and network distance change the numbers, and a timing
  panel that only works on a developer machine is a demo, not a diagnostic.
- **A budget is a contract, not a target.** ≤ 400 ms is the number the team defends in review; the table
  turns red the moment it is broken, which is what makes a regression a bug rather than an argument.

## Evidence — what the running app shows

- On load the table shows `Startup`, `Screen load — Diagnostics` and `Binding refresh` measured and green;
  `Background job tick` updates once a second, in the low single-digit milliseconds.
- **Run query · 50** → `Query — SearchWorkOrders  ≤ 400 ms  162 ms  OK ✓`.
- **Slow query · 5000** → the same row goes `2,340 ms  OVER ✕` on a pink background and the status bar
  reads `… · OVER BUDGET` with the correlation id.
- **Fix page size** → green again, and the status bar names the new correlation id.
