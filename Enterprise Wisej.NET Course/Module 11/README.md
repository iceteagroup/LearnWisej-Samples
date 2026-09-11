# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 11

Local lab build for **Advanced Module 11 · Observability, Diagnostics, Performance & Session Profiling**,
following the walkthrough video *"Instrument EnterpriseOps end to end"*. The `DiagnosticsPage` binds a
`DiagnosticSnapshot` (version · environment · node · theme · flags) behind a role check, shows session &
health, the performance budget table (`PerfBudgetPanel`) and the structured log. `OperationTimer` times
every service operation and writes one structured JSON line per operation, and one correlation id ties a
click to the log line, the budget row and the status bar. The slow query from the video (`pageSize` 5,000 →
2,340 ms against a 400 ms budget) is diagnosed from the log field and then fixed.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine, with in-memory data.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Enterprise Wisej.NET Course\Module 11\EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5211
```

Then open <http://localhost:5211>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

## What to try

| Control | What you should see |
|---|---|
| **Role** / **secrets redacted (5)** chips | `Role: Manager ✓`; hover the amber chip for the five redaction notes |
| **VERSION · ENVIRONMENT · NODE · THEME** cards | `2.4.1+sha.9e21b0`, the environment, `app-node-B`, the active theme |
| **Session & health** card | session count, uptime, managed heap / working set, feature flags, and the health chip (hover it for all six checks); refreshed every second by `timerLive` |
| **Run query · 50** | `Query — SearchWorkOrders  ≤ 400 ms  162 ms  OK ✓`, one `information` JSON line with `"pageSize":50`, and the status bar `… · within budget` |
| **Slow query · 5000** | the row goes `2,340 ms  OVER ✕` on pink, the JSON line says `"elapsedMs":2340,"pageSize":5000,"budget":"over"`, the status bar reads `OperationTimer — SearchWorkOrders 2,340 ms · correlation … · OVER BUDGET` |
| **Fix page size** | back to `pageSize` 50 → ≈ 162 ms, the row turns green, a new correlation id in the status bar |
| **Structured log** list | one JSON line per operation, correlation id last; an error entry is shown redacted |
| status bar | the last result and its correlation id |

## Lab tasks → where in the code

| Lab task | Where |
|---|---|
| Open the project, run it locally once | `EnterpriseOps.slnx`, `Properties/launchSettings.json` (port 5211), `Startup.cs` (`app.UseWisej()`) |
| Structured logging | `Diagnostics/StructuredLog.cs` — `LogEntry.ToJsonLine()`, bounded 200-entry buffer, `SafeErrorMessage` |
| Correlation IDs | `Services/SessionContext.NewCommand()` → `CommandContext` parameter on every service and data method |
| Diagnostics page | `UI/DiagnosticsPage.cs` / `.Designer.cs`, `Diagnostics/DiagnosticsService.CaptureSnapshot()`, `Security/DiagnosticsAccessPolicy.cs` |
| Health status | `Diagnostics/HealthCheck.cs`, probes registered in `DiagnosticsPage.RegisterHealthProbes()` |
| Performance timing panel | `Diagnostics/PerformanceBudget.cs`, `OperationTimer`, `UI/PerfBudgetPanel.cs` |
| Session-memory review checklist | `Diagnostics/SessionMemoryAudit.cs`, `DiagnosticsPage.RegisterRetainedState()`, `DiagnosticsPage_Disposed` |
| Simulate a slow query and diagnose it | `btnSlowQuery_Click` → `RunQueryAsync` → `WorkOrderService.SearchWorkOrdersAsync` → `InMemoryWorkOrderStore.SimulatedLatencyMs` (`140 + 0.44 × pageSize`) |
| Show every path without leaking internals | `DiagnosticsPage.ReportFailure`, `SafeErrorMessage.For`, `DiagnosticsPage.ForDisplay` (errors redacted on screen) |
| Deliverables + production-readiness note | `EnterpriseOps/docs/` — `DiagnosticsPage.md`, `StructuredLogExample.md`, `CorrelationIdPropagation.md` (+ `correlation-propagation.svg`), `PerformanceBudget.md`, `SessionMemoryAudit.md` |

## Student review questions, answered against the sample

**Can support identify the exact version and tenant?**
Yes. The **VERSION** card shows `2.4.1+sha.9e21b0` — the `AssemblyInformationalVersion` read through
reflection (`DiagnosticsService.Version`), so it is the semantic version *and* the commit. **NODE** and
**ENVIRONMENT** say which machine and deployment answered, the status bar names the tenant, and the
correlation id in the user's error message points at a log line that carries tenant and user as fields.

**Does diagnostics expose secrets?**
No, by type. `DiagnosticSnapshot` has six fields and none can hold a connection string, an API key, a stack
trace or customer data. `DeploymentConfig` does hold those values, and `CaptureSnapshot()` records what it
did with each one in `RedactionNotes` (excluded ×2, masked ×1, shortened ×1, plus "exception messages never
reach `SafeRecentEvents`"). An error entry is shown as its `SafeSummary` only. The page is behind
`DiagnosticsAccessPolicy` (Manager, Admin) and opening it is audited with the correlation id.

**Which objects are retained for the whole session?**
Three registered holders, each with a reason, a bound and a lifetime in `SessionMemoryAudit`:
`InMemoryWorkOrderStore._rows` (150 rows), `StructuredLog._entries` (ring buffer of 200) and the on-screen
log list (trimmed at 60 lines). `DiagnosticsPage_Disposed` stops `timerLive`, unsubscribes the
`EntryWritten` handler and disposes the `CancellationTokenSource`.

## Instructor acceptance criteria, answered against the sample

- **Course architecture baseline.** One project, folder-per-layer (`UI`, `Services`, `Domain`, `Data`,
  `Security`, `Diagnostics`), typed commands and results, the `WorkOrder` entity projected into
  `WorkQueueRow`, per-session services created in the page constructor — no static user or tenant state.
- **Thin handlers.** The query handlers set the page size and call `RunQueryAsync`: build a context, await
  the service, display the result, and on failure log with full detail and show a safe message.
  `PerformanceBudget`, `HealthCheck`, `DiagnosticsAccessPolicy` and `SessionMemoryAudit` decide.
- **Service-level logic reviewable without the designer.** Every rule lives in `Diagnostics/*.cs`,
  `Services/*.cs` and `Security/*.cs`.
- **A failure path.** The over-budget slow query, diagnosed from the structured log and recovered by
  **Fix page size**; exceptions go through `ReportFailure` (full detail server-side, reference to the user).
- **State, security, production.** Per-session `SessionContext`, registered retained holders, one timer
  with a named stop site; role-protected, audited page with secrets excluded or masked; budgets measured in
  the running session, a bounded log, `Application.Update(this)` after awaits.

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
   │  └─ InMemoryWorkOrderStore.cs   150 rows; latency = 140 ms + 0.44 ms × pageSize
   ├─ Services/
   │  ├─ SessionContext.cs           per-session user/tenant + correlation-id factory + startup stopwatch
   │  ├─ CommandContext.cs           tenant + user + correlation id, a parameter on every call
   │  ├─ Contracts.cs                WorkQueueQuery, WorkQueueRow, PagedResult<T>, CommandResult, SearchResult
   │  └─ WorkOrderService.cs         the OperationTimer + budget verdict + structured entry per operation
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
   │  ├─ DiagnosticsPage.cs/.Designer.cs   the screen: snapshot cards, health, budgets, structured log, status bar
   │  └─ PerfBudgetPanel.cs/.Designer.cs   dgvBudgets — Operation · Budget · Measured · Status, red when over
   └─ docs/                          the five deliverables + the correlation SVG
```

Service, data and diagnostics layers also write a plain server-side trace (`Service:` · `Data:` ·
`Diagnostics:` lines with the correlation id) to `System.Diagnostics.Trace`; it never reaches the page.
