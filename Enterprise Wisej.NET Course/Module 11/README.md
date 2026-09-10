# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 11

Local lab build for **Advanced Module 11 · Observability, Diagnostics, Performance & Session Profiling**.
It follows the walkthrough video *"Instrument EnterpriseOps end to end"*: the `DiagnosticsPage` binds a
`DiagnosticSnapshot` (version · environment · node · theme · flags) behind a role check, `OperationTimer`
times every service operation as a `using` block and writes one **structured JSON line** per operation, the
`PerfBudgetPanel` shows measured against allowed, and one **correlation id** ties a click to the log line,
the budget row, the audit entry and the message the user is shown.

The failure path the video walks through is here as three buttons: a **slow query** (`pageSize` 5,000 →
2,340 ms against a 400 ms budget, diagnosed from the log field, then fixed), an **operation that throws**
(full detail server-side, safe message to the user, same correlation id), and a **session memory leak**
(a 50,000-row report cached in a session field, caught by the audit and released).

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine, with in-memory data.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Enterprise Wisej.NET Course\Module 11\EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5211
```

Then open <http://localhost:5211>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. Both target
frameworks (`net10.0-windows`, `net10.0`) build; `dotnet build -nologo -v q` reports 0 warnings, 0 errors.

## What to click

Header bar — screen · tenant · **user combo** · the live correlation id.

| Control | Path | What you should see |
|---|---|---|
| **user combo → `ben.tech`** | failure (permission) | `Security: DiagnosticsAccessPolicy → DENIED — Technician is not an operator role (allowed: Manager, Admin)`, the attempt is audited, the role chip goes red, the snapshot cards fall back to `—`, and the budgets / log / health cards are hidden — the values are never rendered |
| **user combo → `ana.ops`** | recovery | a fresh snapshot with a new correlation id; everything returns |
| hover **`secrets redacted (5)`** | — | the five redaction notes: two excluded by type, one masked (`st•••••prod`), exception messages server-only, session id shortened |

Bottom bar:

| Button | Path | What you should see |
|---|---|---|
| **Run query · 50** | success | `Query — SearchWorkOrders  ≤ 400 ms  162 ms  OK ✓`, a green status line, and one `information` JSON line with `"pageSize":50,"budget":"ok"` |
| **Slow query · 5000** | failure 1 | the same row goes **`2,340 ms  OVER ✕`** on pink, the status reads `OperationTimer — SearchWorkOrders 2,340 ms · correlation … · OVER BUDGET`, a red banner names the field, and the JSON line says `"elapsedMs":2340,"pageSize":5000,"budget":"over"` |
| **Fix page size** | recovery 1 | back to `pageSize` 50 → ≈ 162 ms, the row turns green, a green banner quotes the new correlation id |
| **Store failure** | failure 2 | the store throws on the next call; the trace shows `Data: connection lost (simulated) … → throwing to the service`, `Service: … rethrown to the handler`, `Log: InvalidOperationException written with full detail · correlation …`; the page shows the entry **redacted** (`← error detail is server-log only`) and the user is told only *"The operation could not be completed. Reference … "* |
| **Leak: cache 50k rows** | failure 3 | the managed-heap figure jumps ≈ 11.4 MB, a `RetainReport` warning line appears with `heapBeforeBytes` / `growthBytes`, and the audit prints `[OVER BUDGET] ReportCacheService._cached ≈11.4 MB · whole session — never cleared` |
| **Release cache** | recovery 3 | `Service: ReportCacheService.Release() → 50,000 rows dropped`, the audit passes, the heap falls back, green banner |
| **▶ Live refresh** | progress | `timerLive` (1 s) refreshes session count, heap, working set, uptime and the health chip; each tick is measured as `Background job tick`. Click again to stop |
| **Health check** | — | the chip turns `HEALTHY` / `DEGRADED`; the trace lists all six checks (work-order store, live refresh job, deployment config, performance budgets, session memory, structured log) with their detail |
| **Clear trace** | — | empties the right-hand card; the structured log is untouched, because it is the record |

The right-hand card is the **Server · live activity trace**: every line is `HH:mm:ss.fff` plus a layer
prefix — `UI →`, `Service:`, `Data:`, `Diagnostics:`, `Security:`, `Log:` — and every line for one click
carries that click's correlation id. This is how the reviewer proves the handler is thin and the service
(not the screen) decided.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | Diagnostics page | [`EnterpriseOps/docs/DiagnosticsPage.md`](EnterpriseOps/docs/DiagnosticsPage.md) · code: `UI/DiagnosticsPage.cs`, `Diagnostics/DiagnosticsService.cs`, `DiagnosticSnapshot`, `Security/DiagnosticsAccessPolicy.cs` |
| 2 | Structured log example | [`EnterpriseOps/docs/StructuredLogExample.md`](EnterpriseOps/docs/StructuredLogExample.md) · code: `Diagnostics/StructuredLog.cs`, `WorkOrderService.RecordTiming` |
| 3 | Correlation ID propagation | [`EnterpriseOps/docs/CorrelationIdPropagation.md`](EnterpriseOps/docs/CorrelationIdPropagation.md) + [`correlation-propagation.svg`](EnterpriseOps/docs/correlation-propagation.svg) · code: `Services/SessionContext.cs`, `Services/CommandContext.cs` |
| 4 | Performance budget table | [`EnterpriseOps/docs/PerformanceBudget.md`](EnterpriseOps/docs/PerformanceBudget.md) · code: `Diagnostics/PerformanceBudget.cs`, `UI/PerfBudgetPanel.cs` |
| 5 | Memory/session audit notes | [`EnterpriseOps/docs/SessionMemoryAudit.md`](EnterpriseOps/docs/SessionMemoryAudit.md) · code: `Diagnostics/SessionMemoryAudit.cs`, `Services/ReportCacheService.cs`, `DiagnosticsPage_Disposed` |

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Open the project, run it locally once | `EnterpriseOps.slnx`, `Properties/launchSettings.json` (port 5211), `Startup.cs` (`app.UseWisej()`) |
| Structured logging | `Diagnostics/StructuredLog.cs` — `LogEntry.ToJsonLine()`, bounded 200-entry buffer, `SafeErrorMessage` |
| Correlation IDs | `Services/SessionContext.NewCommand()` → `CommandContext` parameter on every service and data method |
| Diagnostics page | `UI/DiagnosticsPage.cs` / `.Designer.cs`, `Diagnostics/DiagnosticsService.CaptureSnapshot()` |
| Health status | `Diagnostics/HealthCheck.cs`, probes registered in `DiagnosticsPage.RegisterHealthProbes()` |
| Performance timing panel | `Diagnostics/PerformanceBudget.cs`, `OperationTimer`, `UI/PerfBudgetPanel.cs` |
| Session-memory review checklist | `Diagnostics/SessionMemoryAudit.cs`, `DiagnosticsPage.RegisterRetainedState()` |
| Simulate a slow query and diagnose it | `btnSlowQuery_Click` → `WorkOrderService.SearchWorkOrdersAsync` → `InMemoryWorkOrderStore.SimulatedLatencyMs` (`140 + 0.44 × pageSize`) |
| Show every path without leaking internals | `ReportFailure`, `SafeErrorMessage.For`, `DiagnosticsPage.ForDisplay` (errors redacted on screen) |
| Production-readiness note | the **Evidence** section of each `docs/*.md`, plus the tables above |

## Student review questions, answered against the sample

**Can support identify the exact version and tenant?**
Yes, without asking the user anything. The **VERSION** card shows `2.4.1+sha.9e21b0` — the
`AssemblyInformationalVersion` read back through reflection (`DiagnosticsService.Version`), so it is the
semantic version *and* the commit, and it cannot drift from a hand-typed string. The **NODE** and
**ENVIRONMENT** cards say which machine and which deployment answered, the header says which tenant and
user, and the correlation id in the user's error message points at a log line that repeats all four as
fields (`"tenant"`, `"user"`, `"node"`, `"environment"`).

**Does diagnostics expose secrets?**
No, and not by discipline — by type. `DiagnosticSnapshot` has six fields and none of them can hold a
connection string, an API key, a stack trace or customer data, so the page that binds to it has nothing to
leak. `DeploymentConfig` deliberately *does* hold those values, and `CaptureSnapshot()` records what it did
with each one in `RedactionNotes` (excluded by type ×2, masked ×1, shortened ×1, plus "exception messages
never reach `SafeRecentEvents`"). On screen, an error entry is rendered as its `SafeSummary` with
`← error detail is server-log only`. The page is behind `DiagnosticsAccessPolicy` (Manager, Admin), and
opening it — allowed or denied — is written to the audit trail with the correlation id. Every value on the
screen would survive being shown to a customer on a projector.

**Which objects are retained for the whole session?**
Four, and each one had to declare a reason, a bound and a lifetime to `SessionMemoryAudit`:
`InMemoryWorkOrderStore._rows` (150 rows, ≈ 0.05 MB, bounded), `StructuredLog._entries` (ring buffer of
200, ≤ 0.09 MB), the trace `ListBox` items (trimmed at 400 lines) and — the mistake —
`ReportCacheService._cached`, an unbounded "cache the big report" field that reaches ≈ 11.4 MB and is
flagged `OVER BUDGET`. Multiply that by concurrent sessions and it is the server. It is released by the
recovery button and by `DiagnosticsPage_Disposed`, which also stops `timerLive`, unsubscribes the
`EntryWritten` handler and disposes the `CancellationTokenSource`.

## Instructor acceptance criteria, answered against the sample

- **Follows the course architecture baseline.** One project, folder-per-layer (`UI`, `Services`, `Domain`,
  `Data`, `Security`, `Diagnostics`), namespaces matching the folders, typed commands and results
  (`WorkQueueQuery`, `SearchResult`, `CommandResult`, `PagedResult<T>`), the `WorkOrder` entity projected
  into `WorkQueueRow` before it reaches the UI, and per-session services created in the page constructor —
  no static user or tenant state.
- **UI event handlers remain thin and explainable.** `btnRunQuery_Click` is a `try` that builds a context,
  awaits `_workOrders.SearchWorkOrdersAsync` and displays the result, and a `catch` that logs and shows a
  safe message. No handler measures anything, compares to a budget, decides a health status or evaluates a
  role — `PerformanceBudget`, `HealthCheck`, `DiagnosticsAccessPolicy` and `SessionMemoryAudit` do.
- **Service-level logic can be reviewed without opening the designer.** Every rule lives in
  `Diagnostics/*.cs`, `Services/*.cs` and `Security/*.cs`; the designer file contains layout and event
  wiring only.
- **At least one failure path is demonstrated.** Four: over-budget slow query (recovered by **Fix page
  size**), a store exception (recovered by re-running the query), a session memory leak (recovered by
  **Release cache**) and a denied role (recovered by switching back to an operator).
- **State ownership, security implications and production behaviour can be explained.** State: per-session
  `SessionContext` in `Application.Session`, four registered retained holders, one timer with a named stop
  site. Security: role-protected page, audited access attempts, secrets excluded by type or masked, user
  messages that carry a reference and nothing else. Production: budgets measured in the running session,
  a bounded log, `Application.Update(this)` after awaits, and a `Disposed` handler that stops everything the
  page started.

## Where things live

```
Module 11/
├─ EnterpriseOps.slnx
└─ EnterpriseOps/
   ├─ Program.cs                     session entry point: SessionContext.Start → Application.MainPage
   ├─ Startup.cs                     Kestrel host (app.UseWisej())
   ├─ Properties/AssemblyInfo.cs     AssemblyInformationalVersion "2.4.1+sha.9e21b0" — the VERSION card
   ├─ Domain/WorkOrder.cs            WorkOrder, Tenant, WorkOrderStatus, Priority
   ├─ Data/
   │  ├─ DeploymentConfig.cs         environment, node, feature flags — and the secrets kept out of the snapshot
   │  └─ InMemoryWorkOrderStore.cs   150 rows; latency = 140 ms + 0.44 ms × pageSize; one-shot failure switch
   ├─ Services/
   │  ├─ SessionContext.cs           per-session user/tenant + correlation-id factory + startup stopwatch
   │  ├─ CommandContext.cs           tenant + user + correlation id, a parameter on every call
   │  ├─ Contracts.cs                WorkQueueQuery, WorkQueueRow, PagedResult<T>, CommandResult, SearchResult
   │  ├─ WorkOrderService.cs         the OperationTimer + budget verdict + structured entry per operation
   │  └─ ReportCacheService.cs       DELIBERATELY LEAKY: 50,000 rows in a session field, and its Release()
   ├─ Diagnostics/
   │  ├─ DiagnosticsPerformancePatterns.cs   OperationTimer + DiagnosticSnapshot (verbatim from the lesson)
   │  ├─ StructuredLog.cs            LogEntry.ToJsonLine(), bounded buffer, SafeErrorMessage
   │  ├─ PerformanceBudget.cs        the five budget rows; Record() decides OK / OVER
   │  ├─ HealthCheck.cs              probes + budgets + session memory + log → one HealthReport
   │  ├─ SessionMemoryAudit.cs       registered holders, timer probes, verdicts and advice
   │  └─ DiagnosticsService.cs       CaptureSnapshot() (safe by type, with redaction notes), CaptureStats()
   ├─ Security/
   │  ├─ AppUser.cs                  ana.ops (Manager), ben.tech (Technician), cara.admin (Admin)
   │  ├─ AuditTrail.cs               who opened diagnostics, allowed or denied, with the correlation id
   │  └─ DiagnosticsAccessPolicy.cs  operator roles only, decided on the server
   ├─ UI/
   │  ├─ DiagnosticsPage.cs/.Designer.cs   the screen: snapshot cards, health, budgets, JSON log, trace, buttons
   │  └─ PerfBudgetPanel.cs/.Designer.cs   dgvBudgets — Operation · Budget · Measured · Status, red when over
   └─ docs/                          the five deliverables + the correlation SVG
```

## Verified / unverified

Used from the course cookbook and verified by the build (both target frameworks, 0 warnings):

- `Wisej.Web.Timer(components)` with `Interval` / `Tick` / `Start()` / `Stop()` / `Enabled` — `timerLive`.
- `async void` handlers with `await`, `try/catch/finally`, buttons re-enabled in `finally`,
  `Application.Update(this)` after the await, and a `CancellationTokenSource` in an instance field.
- `AlertBox.Show(text, MessageBoxIcon.…, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000)`.
- `DataGridView` with `AutoGenerateColumns = false` and `DataGridViewTextBoxColumn`s added in the designer;
  rows added with `Rows.Add(params object[])`; per-row `DefaultCellStyle.BackColor` and per-cell
  `Style.ForeColor` / `Style.Font` (all four members exist in `Wisej.Framework` 4.1.0).
- Fonts `new Font("default", 12F, FontStyle.Bold)` and `new Font("monospace", 9F)`;
  `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`; `Label.TextAlign` with `System.Drawing.ContentAlignment`.
- `Application.MainPage = new UI.DiagnosticsPage();` for a `Page`; `Application.Session.<name>` as a dynamic
  per-session bag; `Application.SessionId`, `Application.SessionCount`, `Application.Theme` (a
  `Wisej.Core.ClientTheme` with a `Name`) — all four exist on `Wisej.Web.Application` in 4.1.0.
- `Control.Disposed` for the disposal review.

**Unverified at runtime** (compiles, but nobody ran the app — the reviewer should check these):

- `Application.SessionCount` returning a sensible live count under Kestrel, and `Application.Theme.Name`
  returning `Bootstrap-4` rather than `(default)`. Both are wrapped in `try/catch` in
  `DiagnosticsService` and degrade to `-1` / `(unavailable)`.
- `Application.Session.Context` round-tripping a typed object between `Program.Main` and the page
  constructor (the cookbook marks the dynamic session bag as verified for value types; a class reference is
  the same mechanism). `DiagnosticsPage.ReadSession()` falls back to a fresh `SessionContext` if it fails.
- The exact millisecond figures. The 2,340 ms / 162 ms numbers follow from
  `140 + 0.44 × pageSize` plus `Task.Delay` accuracy, so expect a few milliseconds of drift; the budget
  verdicts (400 ms) have enough margin that the OVER / OK outcome is stable.
- `Process.WorkingSet64` and `Process.StartTime` under the `net10.0` (non-Windows) target — both are wrapped
  in `try/catch` and fall back.
- Whether `timerLive`'s one-second tick is comfortably inside the 250 ms `Background job tick` budget on a
  loaded machine; the tick does no I/O, so it should be single-digit milliseconds.
