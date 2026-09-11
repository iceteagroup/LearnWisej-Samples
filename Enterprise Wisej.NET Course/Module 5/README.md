# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 5

Local lab build for **Advanced Module 5 · High-volume data UX, server filtering & batch operations** — the
walkthrough video's *Enterprise Work Queue*, built as a runnable Wisej.NET 4 app.

One screen (`UI/WorkQueuePage`) over **6,000 seeded work orders**: server-side filters, saved views, a pager that
loads exactly one page, sort kept in server-side grid state, and a batch reassignment with progress, a per-row
result report (`UI/BatchResultDialog`) and a retry for the failures only.

Nothing is deployed anywhere and nothing talks to a network: in-memory stores only.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Enterprise Wisej.NET Course/Module 5/EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5205
```

Then open <http://localhost:5205>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — `launchSettings.json`
already points at port 5205.) The project multi-targets `net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

## The screen

Header with the screen title; one card with the filter bar (`txtSearch`, `cboStatus`, `cboAssigned`, `cboSort`,
`btnSearch`), the saved views (`cboSavedView`, `btnApplyView`, `btnSaveView`, the ★ `lblViewBadge`), `dgvQueue`, the
pager (`btnFirst`/`btnPrev`/`lblPage`/`btnNext`/`btnLast`, `cboPageSize`, `lblSelection`), the batch bar
(`cboTechnician`, `btnBatchReassign`, `btnSelectPage`, `btnClearSelection`), the progress line (`lblProgress`,
`progressBatch`), an error banner (`lblBanner`) and the dark status bar (`lblStatusBar`).

## What to click

Signed in as **ana.ops (Manager)** in tenant **contoso**.

| Action | What you should see |
|---|---|
| *(the app opens)* | 50 rows, status bar `Page 1 of 47 · 50 of 2,319 matching · sorted by Priority ↓ · N ms`, badge ★ Saved view: “My critical queue” |
| Type `pump`, press **Enter** (or **Search**) | ~110 matching rows, page back to 1 |
| Click **▶ / ⏭ / ◀ / ⏮** | the next page is fetched from the server — one page at a time |
| Click the **Priority** or **Due** header | the arrow moves, the sort is stored in `GridState`, the page resets to 1 |
| Choose **Overdue HVAC** → **Apply view** | filters, sort and the ★ badge follow the stored query |
| Edit a filter, **Search** | the badge falls back to *custom filters* |
| **★ Save current as view** | a new view appears in the drop-down |
| Ctrl-click rows (or **Select page**), change page, come back | the selection is still there — it lives in `GridState` |
| Search `WO-10023`, select **WO-100234 / 100235 / 100236**, target **s.patel**, **Reassign 3 selected…** → Yes | progress `Reassigning to s.patel — 0 of 3…` before the first row, the button becomes **■ Cancel batch**; then status bar `Batch complete — 2 succeeded · 1 failed · per-row report · audited` and the report: WO-100236 `locked by an open approval (APR-1042) — not changed` (the video's failure path) |
| **Retry 1 failed row(s)** in the report | only the failed row is retried, with its version re-read — it fails again while the approval is open |
| Reassign a row needing a certification to **j.kim** | `j.kim lacks the … certification` → `certification` |
| **■ Cancel batch** mid-run | amber banner: the rows already committed were not rolled back |
| Reload the browser (F5) | the same page, sort, filters and selection come back from `SessionContext` |

The services log every decision (`Service:`, `Data:`, `Security:`, `Job:`) to `System.Diagnostics.Trace` through
`Services/ActivityTrace.cs` — visible in the debugger's Output window.

## Where things live

```
EnterpriseOps/
├─ Program.cs                      Application.MainPage = new UI.WorkQueuePage()
├─ UI/  WorkQueuePage (.cs/.Designer.cs) · BatchResultDialog (.cs/.Designer.cs)
├─ Domain/  WorkOrder.cs · Technician.cs
├─ Services/
│  ├─ ActivityTrace.cs · CommandContext.cs · SessionContext.cs (grid state + saved views, per session)
│  └─ WorkQueues/  HighVolumeGridPatterns.cs · WorkQueueQueryService.cs · GridState.cs · SavedView.cs
│                  ReassignBatchCommand.cs · BatchReassignWorkflow.cs
├─ Data/  WorkOrderStore.cs (6,000 deterministic rows) · SavedViewStore.cs
├─ Security/  PermissionService.cs · AuditTrail.cs
└─ docs/  PagedQueryService.md · SearchProjectionModel.md · SavedViewDefinition.md
          BatchReassignmentWorkflow.md (+ .svg) · PerformanceNotes.md
```

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Open the project, run it once | `EnterpriseOps.slnx`, `dotnet run -f net10.0 --urls http://localhost:5205` |
| Server-side filters | `WorkQueueQueryService.ApplyFilters` ← `WorkQueueQuery` built in `WorkQueuePage.ReadFiltersIntoGridState` |
| Saved views | `SavedView.cs`, `Data/SavedViewStore.cs`, `btnApplyView_Click` / `btnSaveView_Click` |
| Paging | `WorkQueueQueryService.SearchAsync` (`Skip/Take`), `PagedResult<T>`, `GoToPageAsync` |
| Sort persistence | `GridState.Query.SortBy` / `Descending`, `dgvQueue_ColumnHeaderMouseClick`, restored by `WorkQueuePage_Load` |
| Batch reassignment + progress | `ReassignBatchCommand`, `BatchReassignWorkflow.RunAsync`, `btnBatchReassign_Click` → `RunBatchLoopAsync`, `OnBatchProgress` |
| Per-row report for partial failures | `BatchResult` / `BatchRowResult`, `UI/BatchResultDialog.cs` |
| **Deliverables** | `docs/PagedQueryService.md`, `docs/SearchProjectionModel.md`, `docs/SavedViewDefinition.md`, `docs/BatchReassignmentWorkflow.md`, `docs/PerformanceNotes.md` |
| Show every path | partial failure in the report; command rejection / cancel / unexpected error → `lblBanner` with the correlation id (`ReportFailure`) |

`btnSearch_Click` / `btnBatchReassign_Click` are `async` handlers that `await` a service inside `try` /
`catch (Exception ex)`:

```csharp
private async void btnSearch_Click(object sender, EventArgs e)
{
    try
    {
        await RunQueryAsync(ReadFiltersIntoGridState(1), "Searching…");
    }
    catch (Exception ex)
    {
        ReportFailure(ex);
    }
}
```

## Student review questions, answered against this sample

**1 · How many records are loaded for the first screen?** Exactly `PageSize` — 50 — plus a `TotalCount` for the
pager. `SearchAsync` ends in `Skip((Page-1)*PageSize).Take(PageSize).Select(Project)`, and `MaxPageSize` (200)
clamps anything larger. One page is ≈ 13.7 KB; the whole tenant would be ≈ 806 KB (see `PerformanceNotes.md`).

**2 · What happens when one batch row fails?** Nothing happens to the other rows. `BatchReassignWorkflow`
validates and commits each row on its own and records a `BatchRowResult` with an outcome and a reason for every
row, plus one audit entry per row. It returns a report, it does not throw. `BatchResultDialog` lists every row and
*Retry failed rows* re-runs the failed rows only, with re-read versions and a new correlation id.

**3 · Can the user repeat the search after refresh?** Yes. The query, sort, page number, selection and saved view
name live in `GridState` inside `SessionContext` (in `Application.Session`, per user). A saved view is a stored
query definition, so applying it tomorrow runs the same search against tomorrow's data.

## Instructor acceptance criteria, answered

* **Architecture baseline** — folder-per-layer, namespaces matching the folders, typed commands and results at the
  boundary (`WorkQueueQuery`, `PagedResult<T>`, `ReassignBatchCommand`, `BatchResult`).
* **Thin handlers** — every handler in `WorkQueuePage.cs` is one service call inside a `try` / `catch`.
* **Service logic reviewable without the designer** — `WorkQueueQueryService`, `BatchReassignWorkflow`,
  `PermissionService`, `SavedViewStore`, `GridState`.
* **A failure path** — the video's partial failure (approval lock on WO-100236), plus stale-version,
  certification and permission checks per row, command rejection and cancel.
* **State, security, production** — shared data in `WorkOrderStore` / `AuditTrail`, user state in
  `SessionContext`; tenant taken from the session, `PageSize` clamped, permissions re-checked before every write;
  one page per request, a deterministic tie-breaker, optimistic concurrency, progress before the first row.

## Verified / unverified

Builds with `dotnet build -nologo -v q` for both target frameworks. Not yet verified at runtime:
`DataGridView.ColumnHeaderMouseClick` as the sort trigger (columns `NotSortable`), `SelectionChanged` /
`ClearSelection()` / `Row.Selected` for the cross-page selection, `AutoSelectFirstRow = false`, runtime
`HeaderText` changes, `ProgressBar` driven from the progress callback, `TextBox.Watermark`, `DialogResult.Retry`
from `BatchResultDialog`, the grid state surviving a browser reload, and reading a not-yet-set member of
`Application.Session`.
