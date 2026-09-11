# Deliverable 4 — Reference screen naming and service-boundary example

`CommandCenterDashboard` is the course's **reference screen**. Every screen in Modules 2–14 is built by copying
its shape. This page is the shape, spelled out, so a new developer can produce a correct screen without reading
the whole codebase.

## The naming pattern

| Piece | Pattern | This screen | Next screen (Module 2) |
|---|---|---|---|
| Screen | `<Noun><Kind>` | `UI/CommandCenterDashboard.cs` | `UI/WorkQueuePage.cs` |
| Designer file | `<Screen>.Designer.cs` | `UI/CommandCenterDashboard.Designer.cs` | `UI/WorkQueuePage.Designer.cs` |
| Workflow service | `Services/Workflow/<Screen minus kind>Workflow.cs` | `DashboardWorkflow.cs` | `WorkQueueWorkflow.cs` |
| Result | `Services/Workflow/<Screen minus kind>Result.cs` | `DashboardResult.cs` (+ `DashboardKpis`, `IncidentRow`) | `WorkQueueResult.cs` |
| Policy | `Security/<Screen minus kind>Policy.cs` | `DashboardPolicy.cs` | `WorkQueuePolicy.cs` |
| Controls | `Controls/<Noun>.cs` | `KpiTile.cs` | reuses `KpiTile` |

**The one-minute test.** "Where is the workflow for the Command Center dashboard?" →
`Services/Workflow/DashboardWorkflow.cs`. No search, no asking, no reading the Designer. That is the entire
purpose of the naming rule (N-4).

## The boundary, in one picture

```
  btnRefresh_Click                       ← UI/CommandCenterDashboard.cs      UI STATE
        │  one call, ≤ 20 lines
        ▼
  DashboardWorkflow.RefreshAsync(ctx)    ← Services/Workflow/                DECISIONS
        │  1. DashboardPolicy.CanViewCommandCenter(ctx)   → Security/
        │  2. OperationsFeed.PullChangesAsync(tenant)     → Integrations/
        │  3. IWorkOrderRepository.QueryAsync(tenant, …)  → Data/
        │  4. count KPIs, project WorkOrder → IncidentRow
        ▼
  DashboardResult { Succeeded, Kpis, Incidents, Summary, ElapsedMs, Errors, CorrelationId }
        │
        ▼
  ShowResult(result)                     ← UI again: text, colour, DataSource, banner. No decisions.
```

## The handler — copy this

```csharp
private async void btnRefresh_Click(object sender, EventArgs e)
{
    BeginBusy(UiText.Refreshing);                         // UI state: Refresh off, new correlation id
    try
    {
        var result = await _workflow.RefreshAsync(CurrentContext);   // the ONE service call
        ShowResult(result);                                          // UI state again
    }
    catch (Exception ex)
    {
        ReportFailure(ex);                                // details to the log, a generic message to the user
    }
    finally
    {
        EndBusy();                                        // Refresh back on, Application.Update(this)
    }
}
```

Seventeen lines, one service call, one `try`/`catch`, no rule, no query, no role test. It is also exactly the
shape the site's lab code check greps for.

## What the service decides — and what the screen would have decided if we let it

| Question | Decided in | Never in the screen because |
|---|---|---|
| May this user open the Command Center? | `Security/DashboardPolicy` (called first by the workflow) | S-4 — one place to audit; a screen that "forgot" is a breach |
| What counts as "SLA at risk"? | `DashboardWorkflow.SlaWarningWindow` (4 hours) | A-5 — the same number must hold in the work queue, the report and the alert |
| Which rows belong to this tenant? | `InMemoryWorkOrderRepository.QueryAsync(tenantId, …)` | D-2 — isolation cannot depend on a caller remembering |
| Is `Priority.Normal` shown as "Medium"? | `DashboardWorkflow.DisplayPriority` | A-4 — the projection is part of the contract with the grid, so it is versioned with the result |
| What changed in the field since last refresh? | `Integrations/OperationsFeed`, applied by the workflow | D-4 — the workflow never knows whether the feed is HTTP, a queue or a fake |
| Which colour is the banner? | **the screen** | that *is* UI state — S-2 draws the line here |

## The counter-example

[`Architecture/Samples/OrderEntryLegacy.cs.txt`](../Architecture/Samples/OrderEntryLegacy.cs.txt) is the same
feature written without the boundary: `btnSubmitOrder_Click`, **74 lines**, containing validation, pricing rules
(duplicated in two other screens), a permission check, a connection string, string-concatenated SQL, a file
append, an e-mail, and the UI update — and **not one call into a service**. The file is `.cs.txt` so it never
compiles; it exists to be fed to the review gate:

```
ReviewGate.CheckEventHandler("btnSubmitOrder_Click", 74, callsService: false) → 2 issues
  ✕ btnSubmitOrder_Click is too large; move decisions into a service.
  ✕ btnSubmitOrder_Click does not call an application service; verify business logic location.
```

The same rules over `UI/CommandCenterDashboard.cs`: every handler under 20 lines, every handler calling a
service → `PASS — may merge`.

## Checklist for the next screen

1. `UI/<Screen>.cs` + `.Designer.cs`, controls named per N-2, parameterless constructor for the Designer (S-6).
2. `Services/Workflow/<Name>Workflow.cs` taking `CommandContext`, returning a `CommandResult` subclass (A-1).
3. `Security/<Name>Policy.cs` if the screen has any permission at all — called first (A-3).
4. A projection type per grid/list; no entity in the UI (A-4).
5. One log line per layer (A-6), one correlation id per action (E-4).
6. Failure paths handled in the handler's `catch` and in the result's `Errors`, never leaking internals.
7. Run the review gate on the new file before opening the pull request.
