# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 5

Local lab build for **Advanced Module 5 · High-volume data UX, server filtering & batch operations** — the
walkthrough video's *Enterprise Work Queue*, built as a runnable Wisej.NET 4 app.

One screen (`UI/WorkQueuePage`) over **6,000 seeded work orders**: server-side filters, saved views, a pager that
loads exactly one page, sort kept in server-side grid state, and a batch reassignment with progress, a per-row
result report (`UI/BatchResultDialog`) and a retry for the failures only. Plus the anti-pattern the video shows —
`grid.DataSource = db.WorkOrders.ToList()` — kept on purpose and **measured** next to one page.

Nothing is deployed anywhere and nothing talks to a network: in-memory stores only.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Enterprise Wisej.NET Course/Module 5/EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5205
```

Then open <http://localhost:5205>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — `launchSettings.json`
already points at port 5205.)

Requirements already on this machine: the .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package. The project
multi-targets `net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

## What to click

Signed in as **ana.ops (Manager)** in tenant **contoso**. The right-hand card is the live activity trace: every
line is tagged with the layer that took the decision (`UI →`, `Session:`, `Security:`, `Service:`, `Data:`, `Job:`).

| Action | Path | What you should see |
|---|---|---|
| *(the app opens)* | success | 50 rows, footer `50 of 2,319 matching · sorted by Priority ↓ · N ms server · 13.7 KB page`, pager `Page 1 of 47`, badge ★ My critical queue |
| Type `pump`, press **Enter** (or **Search**) | success | ~110 matching rows, page back to 1, trace `Service: SearchAsync page 1 size 50 … search "pump"` |
| Click **▶ / ⏭ / ◀ / ⏮** | success | one more `SearchAsync` with `skip N, take 50` — the previous page is not kept anywhere |
| Click the **Priority** or **Due** header | success | the arrow moves, the sort is stored in `GridState`, the page resets to 1 — the grid never sorted anything itself |
| Choose **Overdue HVAC** → **Apply view** | success | trace prints the stored JSON definition; filters, sort and the ★ badge follow it |
| Edit any filter | success | the badge falls back to *custom filters* — the screen is no longer the saved view |
| **★ Save current as view** | success | a new view appears in the drop-down; the trace prints the JSON that would go in the database column |
| Ctrl-click 3 rows (or **Select page**) | success | `3 selected across pages · 3 the server would allow`; the batch button reads *Reassign 3 selected…* |
| Go to page 2, come back | success | the same 3 rows are still selected — the selection lives in `GridState`, not in the grid |
| Pick a technician, **Reassign 3 selected…** → Yes | progress | the bar and `Reassigning to s.patel — 0 of 3…` appear **before** row 1 is touched, then one step per row; the button becomes **■ Cancel batch** |
| *(batch ends, all good)* | success | green banner `Batch … — 3 succeeded, 0 failed`, dialog lists all three rows, the grid reloads with the new assignee and bumped `v` |
| **Fail: approval lock on a row**, then reassign | **failure** | that row comes back `locked by an open approval (APR-1042) — not changed`; the others are reassigned. Amber banner *2 succeeded, 1 failed*, dialog shows ✓ ✓ ✕ |
| **Retry failed rows** (in the dialog or the bottom bar) | recovery attempt | only the failed row is retried, with its version re-read — it fails again while the approval is open, which is the correct answer |
| **Recover: complete the approval**, then **Retry failed rows** | recovery | the row goes through; the successes are never repeated |
| **Fail: concurrent edit**, then reassign | **failure** | `changed by another user (v2 → v3) — refresh and retry` → `stale-version`. **Retry failed rows** succeeds, because the retry re-reads the version |
| **Run as ben.tech (Technician)** | **failure** | the next page comes back with `0 the server would allow`; running the batch anyway denies **every** row with `permission-denied` and traces `Security: ben.tech (Technician) may not reassign WO-…` |
| **Run as ben.tech** again (button reads *Back to ana.ops*) | recovery | the flags come back and the batch works again |
| **Anti-pattern: load everything** | **failure (measured)** | 2,977 rows bound at once, banner `≈59× the payload of one page`, footer `2,977 rows bound · 806 KB · materialize N ms + project N ms — one page was 13.7 KB`. Click **Search** to go back to paging |
| **⟳ Simulate refresh (F5)** | success | the page object is thrown away and rebuilt; the same page number, sort, filters, selection and trace come back from `SessionContext` |
| Reassign to **j.kim** a row that needs a certification he lacks | **failure** | `j.kim lacks the electrical certification` → `certification` |
| **■ Cancel batch** mid-run | **failure / honesty** | amber banner *the rows already committed were not rolled back — the audit log lists them* |
| **Clear trace** | – | empties the trace card |

## Where things live

```
Module 5/
├─ EnterpriseOps.slnx
└─ EnterpriseOps/
   ├─ Program.cs                   session entry point → Application.MainPage = new UI.WorkQueuePage()
   ├─ Startup.cs                   Kestrel host (app.UseWisej(), static files, never the .json config)
   ├─ Default.json / Default.html / Web.config / Properties/launchSettings.json   (port 5205)
   ├─ UI/
   │  ├─ WorkQueuePage.cs / .Designer.cs        the screen: filter bar, saved views, dgvQueue, pager, batch bar
   │  └─ BatchResultDialog.cs / .Designer.cs    the per-row result report + Retry failed rows
   ├─ Domain/
   │  ├─ WorkOrder.cs              entity + WorkOrderStatus / Priority / Tenant  (no display, no permissions)
   │  └─ Technician.cs             the certifications a technician holds
   ├─ Services/
   │  ├─ ActivityTrace.cs          the trace every layer writes to (owned by the session)
   │  ├─ CommandContext.cs         tenant + user + role + correlation id
   │  ├─ SessionContext.cs         per-session state: grid state, saved views, trace  (never a static)
   │  └─ WorkQueues/
   │     ├─ HighVolumeGridPatterns.cs   WorkQueueQuery · WorkQueueRow · PagedResult<T> · IWorkQueueQueryService
   │     ├─ WorkQueueQueryService.cs    deliverable 1 — filter, sort, skip/take, project
   │     ├─ GridState.cs                page + sort + filters + cross-page selection, server-side
   │     ├─ SavedView.cs                a named, owned WorkQueueQuery
   │     ├─ ReassignBatchCommand.cs     command, BatchRowResult, BatchResult, failure codes, progress
   │     ├─ BatchReassignWorkflow.cs    deliverable 4 — per-row validate → commit → audit → report
   │     └─ LoadEverythingAntiPattern.cs  the video's anti-pattern, measured
   ├─ Data/
   │  ├─ WorkOrderStore.cs         the fake table: 6,000 deterministic rows, per-row locked writes
   │  └─ SavedViewStore.cs         saved views per (tenant, owner) + JSON serialize/deserialize
   ├─ Security/
   │  ├─ PermissionService.cs      who may reassign / approve — asked twice: for the flag, and before the write
   │  └─ AuditTrail.cs             append-only log; one entry per batch row, success or not
   └─ docs/
      ├─ PagedQueryService.md            deliverable 1
      ├─ SearchProjectionModel.md        deliverable 2
      ├─ SavedViewDefinition.md          deliverable 3
      ├─ BatchReassignmentWorkflow.md    deliverable 4  (+ BatchReassignmentWorkflow.svg)
      └─ PerformanceNotes.md             deliverable 5
```

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Open the project, run it once | `EnterpriseOps.slnx`, `dotnet run -f net10.0 --urls http://localhost:5205` |
| Lab goal · server-side filters | `WorkQueueQueryService.ApplyFilters` ← `WorkQueueQuery` built in `WorkQueuePage.ReadFiltersIntoGridState` |
| Lab goal · saved views | `SavedView.cs`, `Data/SavedViewStore.cs`, `btnApplyView_Click` / `btnSaveView_Click` |
| Lab goal · paging | `WorkQueueQueryService.SearchAsync` (`Skip/Take`), `PagedResult<T>`, `GoToPageAsync`, `UpdatePagerButtons` |
| Lab goal · sort persistence | `GridState.Query.SortBy` / `Descending`, `dgvQueue_ColumnHeaderMouseClick`, restored by `WorkQueuePage_Load` |
| Lab goal · batch reassignment | `ReassignBatchCommand`, `BatchReassignWorkflow.RunAsync`, `btnBatchReassign_Click` → `RunBatchLoopAsync` |
| Lab goal · progress | `BatchProgress`, `WorkQueuePage.OnBatchProgress` → `ShowProgress` + `Application.Update(this)` |
| Lab goal · per-row report for partial failures | `BatchResult` / `BatchRowResult`, `UI/BatchResultDialog.cs` |
| **Deliverable · Paged query service** | `Services/WorkQueues/WorkQueueQueryService.cs` → `docs/PagedQueryService.md` |
| **Deliverable · Search projection model** | `WorkQueueRow` in `HighVolumeGridPatterns.cs` + `WorkQueueQueryService.Project` → `docs/SearchProjectionModel.md` |
| **Deliverable · Saved view definition** | `SavedView.cs`, `SavedViewStore.cs`, `GridState.cs` → `docs/SavedViewDefinition.md` |
| **Deliverable · Batch reassignment workflow** | `ReassignBatchCommand.cs`, `BatchReassignWorkflow.cs` → `docs/BatchReassignmentWorkflow.md` + `.svg` |
| **Deliverable · Performance notes** | `LoadEverythingAntiPattern.cs`, `LastElapsedMs` / `LastPayloadBytes` → `docs/PerformanceNotes.md` |
| Show every path | bottom bar: anti-pattern · approval lock · concurrent edit · retry · run as Technician; banner + status + toasts |
| Review & run | this README's *Instructor acceptance criteria* section |

**Lab code check (`labs.js` m5).** `btnSearch_Click` / `btnBatchReassign_Click` are `async` handlers that `await` a
service (`_queryService.SearchAsync`, `_batchWorkflow.RunAsync`), wrap the call in `try` / `catch (Exception ex)`,
and the code is full of `paging` / `projection` / `filter` / `batch` — the five greps pass on `UI/WorkQueuePage.cs`.

```csharp
private async void btnSearch_Click(object sender, EventArgs e)
{
    _trace.Write("UI → btnSearch_Click: filters read into the grid state, page reset to 1");
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

**1 · How many records are loaded for the first screen?**
Exactly `PageSize` — 50 — plus a `TotalCount` for the pager. `WorkQueueQueryService.SearchAsync` ends in
`Skip((Page-1)*PageSize).Take(PageSize).Select(Project)`, and `MaxPageSize` (200) clamps anything larger. The
footer says `50 of 2,319 matching … 13.7 KB page` and the trace says `skip 0, take 50`. The alternative is on the
bottom bar and costs **806 KB for 2,977 rows — 59×** the payload of one page.

**2 · What happens when one batch row fails?**
Nothing happens to the other rows. `BatchReassignWorkflow` validates and commits each row on its own
(permission → exists → open → not locked → version → certification → commit under the store's lock) and records a
`BatchRowResult` with an outcome and a reason for every one of them, plus exactly one audit entry per row. The
workflow **returns a report, it does not throw**: partial failure is the expected outcome. The screen shows an
amber banner with the counts and `BatchResultDialog` lists every row (✓ / ✕ / –) with its reason and the
correlation id. *Retry failed rows* re-reads the current version of the **failed rows only** and runs the batch
again under a new correlation id — the successes were already dropped from the selection, so they cannot be
repeated.

**3 · Can the user repeat the search after refresh?**
Yes. The page owns nothing that must survive: the query, the sort, the page number, the selection and the saved
view name live in `GridState` inside `SessionContext` (stored in `Application.Session`, per user, never a static).
**⟳ Simulate refresh (F5)** disposes the page and does `Application.MainPage = new WorkQueuePage()`; the new page's
`Load` handler reads the same grid state back, rewrites the filter controls, re-runs the same query, restores the
selection and replays the trace. A saved view goes one step further: it is a **stored query definition** (JSON),
so applying it tomorrow runs the same search against tomorrow's data.

## Instructor acceptance criteria, answered

* **Follows the course architecture baseline** — folder-per-layer (`UI` · `Domain` · `Services` · `Data` ·
  `Security`), namespaces matching the folders, typed commands and results at the boundary
  (`WorkQueueQuery`, `PagedResult<T>`, `ReassignBatchCommand`, `BatchResult`), never a raw entity in the UI.
* **UI event handlers remain thin and explainable** — every handler in `WorkQueuePage.cs` is a trace line, one
  service call and a `try` / `catch`. The longest is `btnBatchReassign_Click`, which reads the selection, asks for
  confirmation and calls the workflow. All decisions — filtering, sorting, paging, permissions, validation,
  commits, partial failure — are in services.
* **Service-level logic can be reviewed without opening the designer** — `WorkQueueQueryService.cs`,
  `BatchReassignWorkflow.cs`, `PermissionService.cs`, `SavedViewStore.cs` and `GridState.cs` contain the whole
  behaviour of the module; the `.Designer.cs` files are layout only.
* **At least one failure path is demonstrated** — five, each with its recovery: approval lock (→ complete the
  approval), concurrent edit (→ retry re-reads the version), permission denied (→ switch back to the manager),
  certification missing (→ pick a certified technician), and the measured anti-pattern (→ Search returns to
  paging). Cancelling a running batch is a sixth, deliberately honest about the rows already committed.
* **The student can explain state ownership, security implications and production behaviour** —
  *state:* shared data in `WorkOrderStore` / `AuditTrail` (they stand in for tables), user state in
  `SessionContext` (grid state, saved views, trace), UI state only in the page.
  *security:* the tenant is taken from the session and a cross-tenant query is rejected; `PageSize` is clamped;
  `CanReassign` on the projection is a UI hint and the workflow asks `PermissionService` again against the entity
  before every write; every row writes an audit entry with the correlation id.
  *production:* one page per request, a total-order tie-breaker so paging is stable, optimistic concurrency on
  every write, progress within the first frame, a per-row report, and a retry that repeats nothing.

## Verified / unverified

Built here with `dotnet build -nologo -v q` for **both** target frameworks (`net10.0` and `net10.0-windows`),
0 errors, 0 warnings. The app was **not run** — the reviewer runs it.

Used from the cookbook and **verified** on this framework build (Wisej-4 4.1.0, .NET 10) by earlier course samples:
`Page` screens with a `.Designer.cs`; `Application.Session` as the per-session bag; `Application.SessionId`;
`AlertBox.Show(..., alignment: ContentAlignment.TopRight, autoCloseDelay: 4000)`; `async void` handlers with
`await` + `Application.Update(this)`; `MessageBox.ShowAsync(...)` and `await dialog.ShowDialogAsync()` instead of a
blocking `ShowDialog()`; a `CancellationTokenSource` in an instance field with `try/catch/finally`;
`DataGridView` with `AutoGenerateColumns = false`, explicit `DataGridViewTextBoxColumn`s, `DataSource` bound to a
`List<T>`, `SelectionMode = FullRowSelect`, `MultiSelect`, `SelectedRows`; the fonts `"default"` / `"monospace"`
and `Panel.BorderStyle = BorderStyle.Solid`.

Used and **not yet verified at runtime** (they compile against `Wisej.Framework` 4.1.0; please confirm in the
browser):

* `DataGridView.ColumnHeaderMouseClick` (`DataGridViewCellMouseEventArgs.ColumnIndex`) as the sort trigger, with
  every column set to `SortMode = NotSortable` so the grid cannot sort client-side — cookbook lists
  `dgv.Sort(...)` / `SortCompare` as unverified.
* `DataGridView.SelectionChanged`, `DataGridView.ClearSelection()` and `DataGridViewRow.Selected = true` used to
  push and restore the cross-page selection, and `DataGridView.AutoSelectFirstRow = false` so binding a page does
  not select a row by itself.
* `DataGridViewColumn.HeaderText` changed at runtime to move the ▲ / ▼ sort arrow.
* `Wisej.Web.ProgressBar` (`Minimum`/`Maximum`/`Value`/`Visible`) driven from a progress callback between awaits.
* `TextBox.Watermark` for the search placeholder, and `Wisej.Web.Keys.Enter` in a `KeyDown` handler.
* `ComboBox.DropDownStyle = ComboBoxStyle.DropDownList` with plain objects in `Items` (a small `Choice` class whose
  `ToString()` is the label).
* `Form.FormBorderStyle = Fixed` + `StartPosition = CenterParent` for `BatchResultDialog`, and
  `DialogResult.Retry` as the retry signal.
* `Application.MainPage = new WorkQueuePage()` used **while the app is running** to replace the current page
  (the *Simulate refresh* button). The page unhooks its trace handler first.
* Reading a **not-yet-set** member of the dynamic session bag — `SessionContext.Current` does
  `object stored = Application.Session.EnterpriseOpsContext;` and expects `null` on the first call before writing
  it back. (Writing to `Application.Session.X` is the verified half.)

Fixed while finishing this module: `WorkOrderStore.Instance` was an eager static property initializer declared
*above* the static seed arrays, so `Seed()` ran while `Templates` / `Sites` / `Customers` were still `null` and the
app threw a `TypeInitializationException` on the first request. It is now a `Lazy<WorkOrderStore>`.
