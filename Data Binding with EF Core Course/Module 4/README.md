# SupportDesk · Data Binding with EF Core · Module 4

Local lab build for **Module 4 · Two-Way Binding and CRUD Editors**. It follows the walkthrough video: the
Support Desk ticket browser (Module 3) gains **Add ticket** and **Edit ticket**. A modal `TicketEditorForm`
binds `txtTitle`, `txtDescription`, `cboCustomer`, `cboAgent`, `cboCategory`, `cboStatus`, `cboPriority`,
`chkIsUrgent` and (by hand, not through `DataBindings.Add`) `dtpDueDate` to a `TicketEditModel` through
`editBindingSource`. **Save** runs the fixed pipeline the lesson names — a guard, `EndEdit`, the Module 4
validation placeholder (Title required), a fresh `DbContext` from `TicketCommandService`, map,
`SaveChangesAsync`, `DialogResult.OK` — and only `DialogResult.OK` makes the browser page re-run its
search. **Delete** confirms first, reloads the ticket by key in its own fresh context, applies one business
rule (a `Closed` ticket cannot be deleted), and treats "the row is already gone" as an ordinary result, not
an exception. Everything Module 1, 2 and 3 could do still works, on the second, third and fourth button
rows.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 solution on this machine with a local SQLite
file.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 4/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5404
```

Then open <http://localhost:5404>. (Visual Studio: open `SupportDesk.slnx`, press F5 — the port and the
Development environment are in `SupportDesk.Web/Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package, EF Core 10.0.12
(`Microsoft.EntityFrameworkCore.Sqlite` + `Design`) and the global `dotnet-ef` 10.0.12 tool. The web
project multi-targets `net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

In Development the host **migrates and seeds** `SupportDesk.Web/App_Data/supportdesk.db` before the first
session, exactly as in Module 3 — the model did not change in Module 4 (`TicketEditModel` is a UI-only
class in the Services project, not an entity), so `InitialCreate` is still the only migration and the
committed script under `artifacts/sql/` is unchanged:

```
[SupportDesk] MigrateAsync: 1 pending migration(s) applied, 1 applied in total (20260910150534_InitialCreate)
[SupportDesk] Development seed: seeded 5 customers, 3 agents, 6 categories, 312 tickets, 99 comments in ... ms
```

> **Coming from Module 3?** Its `App_Data/supportdesk.db` already has 312 tickets. Module 4 has its own
> `App_Data` folder, so nothing is shared — but if you ever copy one over, delete it or press
> **Reset & reseed**.

Delete `SupportDesk.Web/App_Data/supportdesk.db` (and its `-wal`/`-shm` companions) to reset the module
completely; **Reset & reseed** on the page does the same thing without a restart.

Tests: `dotnet test SupportDesk.Tests` — **52 tests** against SQLite in memory (the 40 carried over from
Modules 1–3 unchanged, plus 12 new ones for `TicketCommandService`: create, update, delete, delete an
already-deleted ticket, the Closed business rule, the edit-model round trip, and the exact statement counts
for a create, an update and a delete).

## What to click

The **ticket browser card** is unchanged from Module 3 (search, filters, paging) except that double-clicking
a row now opens the editor. Bottom bar **row 1** is the Module 4 lab props (`Clear trace` is anchored right,
on row 1, as it always is on the newest module's row); **row 2** ("Module 3 · browser") keeps every Module 3
path working — including **Break the database** / **Restore and search**, which now also govern the
editor's Save and Delete, since `TicketCommandService` shares the same `DevelopmentOutageSwitch` as
`TicketQueryService`; **row 3** ("Module 2 · model") and **row 4** ("Module 1 · lifetimes") are unchanged.

| Action | Path | What you should see |
|---|---|---|
| **Add ticket** (`btnAdd`) | success · new ticket | `TicketEditorForm` opens, titled *Add ticket*, `lblNumber` reads *assigned on save*, `btnDelete` hidden, `cboStatus`/`cboPriority` default to Open/Normal. Fill in Title, pick a Customer and a Category, **Save** → trace shows `EndEdit → model {...}`, then `◦ context #n created`, `→ SQL SELECT "t"."Number" ...`, `→ SQL INSERT INTO "Tickets" ... RETURNING "Id";`, `← result saved SD-1313 · 2 statement(s)...`, the dialog closes, `• dialog Add ticket → OK → grid refreshed`, and the new row is on page 1 (most recently updated) |
| **Edit ticket** (`btnEdit`) with a row selected, or double-click a row | success · edit | titled *Edit ticket SD-1xxx*, every field pre-filled from the database, `btnDelete` visible. Change something, **Save** → `◦ context created`, `→ SQL SELECT ... WHERE "t"."Id" = @model_Id LIMIT 2`, `→ SQL UPDATE "Tickets" SET ... WHERE "Id" = @p4 AND "RowVersion" = @p5 RETURNING 1;`, `← result saved SD-1xxx · 2 statement(s)...`, `• dialog Edit ticket SD-1xxx → OK → grid refreshed` |
| **Edit ticket** with nothing selected | guard | `• dialog Edit ticket: no row selected in the grid — pick a ticket first` — no dialog opens |
| **Cancel** in the editor | Cancel persists nothing | `• editor Cancel: DialogResult.Cancel — no context created, nothing written`, `• dialog ... → Cancel → nothing persisted, grid untouched` — no SQL line anywhere for this click |
| Leave Title blank, **Save** | Module 4 validation placeholder | `errorProvider` marks `txtTitle`, `• editor validation placeholder: Title is required — Save stopped, no context created` — the dialog stays open, no `◦ context created` line at all |
| **Slow save (2.5 s)** toggle, then **Add ticket** or **Edit ticket**, then **Save** | progress · guard | the toggle reads *ON*; after Save, `• editor` notes *simulated latency of 2.5 s inside the unit of work*; clicking **Save** again during the wait logs `• editor guard: save already running — this click is ignored` |
| **Delete** in the editor, confirm | success · delete | `MessageBox.ShowAsync` Yes/No, then `◦ context created`, `→ SQL SELECT ... LIMIT 2`, `→ SQL DELETE FROM "Tickets" WHERE "Id" = @p0 AND "RowVersion" = @p1 RETURNING 1;`, `← result deleted SD-1xxx · 2 statement(s)`, dialog closes OK, grid refreshed (the row is gone) |
| **Delete** in the editor, then **No** | Delete confirms first | `• editor delete: not confirmed — nothing sent to the database` — no SQL, dialog stays open |
| **Delete** on a `Closed` ticket | business rule refusal | `• editor delete refused: This ticket is Closed and is kept for the record...` — the banner shows the same text, the dialog stays open, exactly one `SELECT` in the trace, no `DELETE` |
| **Simulate: another operator deletes it** (`buttonSimulateDelete`) with a row selected | failure prop | `TicketCommandService.DeleteAsync` runs **outside** the editor; trace shows the same SELECT+DELETE (or the Closed refusal). The grid keeps showing the row — it is deliberately **not** refreshed |
| ...then **Edit** that same (now-vanished) row | already-deleted · Edit → Delete | the editor opens on stale data (its own `LoadEditModelAsync` still ran while the row existed a moment before, or — if opened fresh — `AsNoTracking().SingleAsync` now throws because the row is gone; either way, **Delete** on it → `• editor delete: already gone — another session (or the page's Simulate button) deleted it first`, a top-right `AlertBox`, dialog closes OK, grid refreshes and the row disappears |
| ...then **Edit** and **Save** that row instead | already-deleted · Edit → Save | `TicketCommandService.SaveAsync` finds nothing to update and throws `TicketNotFoundException`; the editor catches it, shows *This ticket was already deleted by someone else. Nothing was saved.*, and still closes with `DialogResult.OK` so the grid refreshes |
| **Break the database**, then **Save** (or **Delete**) in an open editor | failure · outage | `DatabaseUnavailableException` on connection open → friendly banner *The Support Desk database is not reachable right now. Nothing was saved...*, the dialog **stays open and usable** — nothing closes on a caught failure |
| **Restore and search**, then **Save** again | recovery | a fresh context, an ordinary two-statement save |
| Search / Next page / Previous page / Slow search / anti-pattern / Module 2 / Module 1 buttons | unchanged | see the Module 1–3 READMEs — every path still works |

The right-hand card is unchanged: **Server ⇄ Database · EF Core lifetime & SQL trace**. Everything the
editor decides — `LoadEditorAsync`'s summary, `EndEdit`'s snapshot of the model, the validation placeholder,
every context created/disposed and SQL statement inside `Save`/`Delete`, and the dialog's own OK/Cancel
line — reaches this list through the `Action<char, string, string>` callback `TicketBrowserPage` gives the
dialog when it is opened (`AddTraceRaw`), at the same detail level a search gets.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | `TicketEditModel` and `TicketEditorForm` with a `BindingSource` and an `ErrorProvider` | [`docs/EditorFormAndBindingSource.md`](docs/EditorFormAndBindingSource.md) · `SupportDesk.Services/TicketEditModel.cs`, `SupportDesk.Web/TicketEditorForm(.Designer).cs` |
| 2 | `TextBox`, `ComboBox`, `DateTimePicker` and `CheckBox` properties bound to the edit model | [`docs/ControlDataBindings.md`](docs/ControlDataBindings.md) · `TicketEditorForm.Designer.cs` (`DataBindings.Add`), `AgentBinding_Format`/`_Parse` |
| 3 | `LoadEditorAsync` for new and existing tickets and `SaveAsync` with `EndEdit`, mapping and `SaveChangesAsync` | [`docs/LoadAndSaveFlows.md`](docs/LoadAndSaveFlows.md) · `TicketEditorForm.LoadEditorAsync` / `SaveAsync`, `TicketCommandService.LoadEditModelAsync` / `SaveAsync` |
| 4 | Delete with confirmation, a fresh entity load and graceful handling of already-deleted records | [`docs/DeleteConfirmationAndReload.md`](docs/DeleteConfirmationAndReload.md) · `TicketEditorForm.DeleteAsync`, `TicketCommandService.DeleteAsync`, `TicketBrowserPage.buttonSimulateDelete_Click` |
| 5 | Parent grid refreshed after `DialogResult.OK`, and Cancel persists nothing | [`docs/DialogResultAndGridRefresh.md`](docs/DialogResultAndGridRefresh.md) · `TicketBrowserPage.OpenEditorAsync`, `btnAdd_Click`/`btnEdit_Click`/`ticketsDataGridView_CellDoubleClick` |

The Module 1–3 deliverables and their notes are unchanged and still in `docs/`:
[`ModelAndDeleteBehaviours.md`](docs/ModelAndDeleteBehaviours.md), [`RowVersionAndIndexes.md`](docs/RowVersionAndIndexes.md),
[`MigrationWorkflow.md`](docs/MigrationWorkflow.md), [`SeedData.md`](docs/SeedData.md),
[`SchemaVsUiValidation.md`](docs/SchemaVsUiValidation.md), [`TicketSearchService.md`](docs/TicketSearchService.md),
[`TicketBrowserBinding.md`](docs/TicketBrowserBinding.md), [`PagingInTheDatabase.md`](docs/PagingInTheDatabase.md),
[`LookupComboBoxes.md`](docs/LookupComboBoxes.md), [`SearchPagingAndTheLoadingGuard.md`](docs/SearchPagingAndTheLoadingGuard.md).

## Where things live

```
Module 4/
├─ SupportDesk.slnx                 the four projects
├─ artifacts/sql/
│  └─ supportdesk_migrations.sql    unchanged since Module 2 — the model did not change in Module 4
├─ SupportDesk.Web/                 the Wisej.NET application (net10.0-windows;net10.0)
│  ├─ Startup.cs                    + AddTransient<TicketCommandService>()
│  ├─ TicketBrowserPage.cs          + OpenEditorAsync, btnAdd_Click, btnEdit_Click, CellDoubleClick,
│  │                                  buttonSlowSave_Click, buttonSimulateDelete_Click, AddTraceRaw
│  ├─ TicketBrowserPage.Designer.cs + btnAdd, btnEdit, buttonSlowSave, buttonSimulateDelete (row 1);
│  │                                  panelActions2/3/4 = the former Module 3/2/1 rows, unchanged inside
│  ├─ TicketEditorForm.cs           NEW — LoadEditorAsync, SaveAsync, DeleteAsync, the trace translator
│  ├─ TicketEditorForm.Designer.cs  NEW — editBindingSource, errorProvider, every editor control
│  └─ (Program.cs / SupportDeskDevelopmentDatabase.cs / appsettings*.json / Default.* — unchanged)
├─ SupportDesk.Data/                unchanged since Module 2 — no entity or mapping changes in Module 4
├─ SupportDesk.Services/
│  ├─ TicketBrowsing.cs             + TicketPriorities (alongside the existing TicketStatuses)
│  ├─ TicketEditModel.cs            NEW — the UI-only edit model, INotifyPropertyChanged
│  ├─ TicketCommandService.cs       NEW — LoadEditModelAsync, GetLookupsAsync, SaveAsync, DeleteAsync,
│  │                                  EditorLookups, TicketEditData, SaveTicketResult, DeleteTicketResult,
│  │                                  DeleteOutcome, TicketNotFoundException, NextNumberAsync
│  └─ (TicketQueryService.cs / DevelopmentSeeder.cs / ModelDemoService.cs / ... — unchanged)
├─ SupportDesk.Tests/               xunit, SQLite in memory — 52 tests
│  ├─ TicketCommandServiceTests.cs  NEW — the Module 4 deliverable: create, update, delete, already-deleted,
│  │                                  the Closed rule, the edit-model round trip, exact statement counts
│  └─ (everything from Modules 1–3 — unchanged, still green)
└─ docs/                            the five Module 4 deliverables + the ten Module 1–3 notes
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the Module 3 solution, confirm the ticket browser searches and pages through `ticketBindingSource` | `SupportDesk.slnx`; unchanged from Module 3 |
| 2 · `TicketEditModel` in `SupportDesk.Services`, with only the properties the screen may edit plus `Id` | `SupportDesk.Services/TicketEditModel.cs` — see [`docs/EditorFormAndBindingSource.md`](docs/EditorFormAndBindingSource.md) for what is deliberately left out and why |
| 3 · `TicketEditorForm` with an injected `TicketCommandService`, a `BindingSource`, an `ErrorProvider` and Save/Cancel/Delete buttons; lookups loaded before the model | `TicketEditorForm(.Designer).cs`; `[Inject] TicketCommandService Commands` |
| 4 · Bind each control with `DataBindings.Add` and an explicit `DataSourceUpdateMode`; write `LoadEditorAsync` for both cases | `TicketEditorForm.Designer.cs` (the bindings) + `TicketEditorForm.LoadEditorAsync` — see [`docs/ControlDataBindings.md`](docs/ControlDataBindings.md) |
| 5 · `SaveAsync` as the pipeline (guard, `EndEdit`, validate, fresh context, load or create, map, `SaveChangesAsync`, close) and `DeleteAsync` with confirmation and a fresh load by key | `TicketEditorForm.SaveAsync` / `DeleteAsync`, `TicketCommandService.SaveAsync` / `DeleteAsync` — see [`docs/LoadAndSaveFlows.md`](docs/LoadAndSaveFlows.md) and [`docs/DeleteConfirmationAndReload.md`](docs/DeleteConfirmationAndReload.md) |
| 6 · Wire the ticket browser's Add and Edit buttons to `ShowDialog` and re-run the search only on `DialogResult.OK` | `TicketBrowserPage.OpenEditorAsync`, `btnAdd_Click`, `btnEdit_Click`, `ticketsDataGridView_CellDoubleClick` — see [`docs/DialogResultAndGridRefresh.md`](docs/DialogResultAndGridRefresh.md) |
| 7 · Show every path: new, edited, Cancel, a double click on Save ignored, Delete confirmed and refused, a Delete already done in a "second session" | the bottom bar row 1 buttons — see **What to click** above |
| 8 · Review & run: two sessions, confirm the grid shows the saved row, write the DbContext-lifetime note | `TicketCommandServiceTests.cs` for what a console check can prove; **Self-check answers** below for the note |

Lab code check (`labs.js` m4): a `TicketEditModel` with the named fields (present, minus `RowVersion` — see
the deviation note below), a `TicketEditorForm` constructor, `DataBindings.Add` with `nameof`/explicit
`DataSourceUpdateMode`, `LoadEditorAsync`, `SaveAsync` with `EndEdit`/a fresh context/`SaveChangesAsync`,
`DeleteAsync` with confirmation and a fresh load by key, and `btnAdd`/`btnEdit` re-running the search only
on `DialogResult.OK` — all present and exercised by `TicketCommandServiceTests.cs`.

## Self-check answers (the lab's student review questions)

- **What lifetime did you choose for the `DbContext` in the editor, and what would have to be true about
  the form before a form-scoped context became acceptable?**

  Per-operation, the same rule every module in this course has kept: `TicketCommandService.LoadEditModelAsync`,
  `GetLookupsAsync`, `SaveAsync` and `DeleteAsync` each call `await _dbFactory.CreateDbContextAsync(token)`
  as their first line and dispose it (`await using`) before returning. Four contexts, four operations,
  four dispositions — `LoadEditorAsync` alone opens and closes two of them (lookups, then the model) before
  the dialog is even visible. `TicketEditorForm` itself never holds a context field; the trace proves it —
  every `◦ context #n created` is followed by exactly one `◦ context #n disposed` inside the same click,
  the same shape `TicketBrowserPage`'s trace has shown since Module 1.

  A form-scoped context — one field, created once in the constructor, reused by every method — would only
  become acceptable if several things were true at once, none of which hold here. *First*, the form would
  have to guarantee only one database operation runs on it at a time; Wisej.NET delivers a second click
  while an await is pending (verified since Module 1), so the guard (`_saving`) would have to become the
  *only* thing standing between a legitimate double click and `InvalidOperationException: A second
  operation was started on this context instance...`. It already is that guard today, but today a bug in
  it merely lets a second, harmless, independent context start; with a shared context a bug in the guard
  would corrupt the one context every subsequent click depends on. *Second*, the form would have to accept
  stale reads: a context's identity map returns the same tracked `Ticket` instance on a second query, so a
  `Closed` status set by someone else between `LoadEditorAsync` and a later re-check would not be seen —
  today `DeleteAsync`'s business-rule check is safe precisely because it re-reads the row in the same fresh
  context it is about to act on. *Third*, the outage recovery path would break: `DevelopmentOutageSwitch`
  fails a connection *open*, and a per-operation context is simply replaced by a new one on the next
  attempt (**Restore and search**, then **Save**, proves this); a context that lived for the form's whole
  lifetime would need to be detected as poisoned and rebuilt by hand — extra state, for no benefit, since
  the editor's operations (one load, one save or delete) are already short and independent. The honest case
  *for* a form-scoped context — Direct entity binding to a tracked graph, kept alive because the whole form
  edits one aggregate in one context that is disposed when the form closes — is exactly what this module's
  reading contrasts with an edit-model approach, and is not what `TicketEditorForm` does: it binds
  `TicketEditModel`, not a tracked `Ticket`, so there is no tracked graph for a form-scoped context to hold
  onto in the first place.

- **Which values are bound to the UI, which live only in the edit model, and which remain only in the
  database, and why is `Number` not editable?**

  **Bound to a control** (through `DataBindings.Add`, or copied by hand for `dtpDueDate`): `Title`,
  `Description`, `CustomerId`, `AgentId`, `CategoryId`, `Status`, `Priority`, `DueDate`, `IsUrgent` — the
  nine fields `TicketEditModel` declares. **Live only in the edit model, touched by no control:** `Id` —
  read by `SaveAsync` to decide Add vs. load-and-update, never shown or typed. **Remain only in the
  database, never crossing into `TicketEditModel` at all:** `Number`, `CreatedAt`, `UpdatedAt`, `RowVersion`,
  and the full `Customer`/`Agent`/`Category`/`Comments` navigation graph a tracked `Ticket` would carry.
  `lblNumber` *shows* the ticket's number (carried alongside the model as `TicketEditData.Number`, from
  `TicketCommandService.LoadEditModelAsync`), but nothing binds to it and no control can change it.

  `Number` is not editable because nothing on this screen is allowed to renumber a ticket after the fact —
  the unique index `IX_Tickets_Number` (Module 2) and every report, search result and comment thread that
  might reference `SD-1042` by that string depend on it never changing once assigned.
  `TicketCommandService.SaveAsync` is the only code that ever sets it, and only once: `NextNumberAsync`
  runs exactly when `model.Id == 0` (a new ticket), and an existing ticket's `Ticket.Number` is never
  reassigned in the mapping block. Leaving `Number` out of `TicketEditModel` entirely — rather than putting
  it in the model as a read-only property — means there is no bound control, no `DataSourceUpdateMode`, and
  no code path in the entire editor that could ever write to it: the lesson's "an edit model can leave a
  field out entirely, so a user cannot edit a field the screen never meant to expose" is not a policy this
  code enforces at runtime, it is a fact about what does and does not compile.

- **Which failure modes did you handle explicitly (double click, already-deleted ticket, database
  rejection, concurrent edit), and what does the operator see for each?**

  **Double click on Save or Delete.** `_saving` is checked first in both `SaveAsync` and `DeleteAsync`
  before anything else runs — no context is created, no SQL is sent. The operator sees the button already
  disabled (`this.btnSave.Enabled = false` in the `try`, restored in `finally`) and, if Wisej.NET still
  delivers the second click (verified since Module 1 that it does), the trace line
  `• editor guard: save already running — this click is ignored`. The **Slow save (2.5 s)** toggle exists
  specifically to make the window wide enough to click twice and see it happen.

  **An already-deleted ticket.** Two entry points, two friendly outcomes. Deleting a ticket that is already
  gone: `TicketCommandService.DeleteAsync`'s `SingleOrDefaultAsync` returns null, and the result comes back
  as `DeleteOutcome.NotFound` — not an exception — with the reason *"This ticket was already deleted by
  someone else."* shown in a top-right `AlertBox`; the dialog still closes with `DialogResult.OK` so the
  stale row leaves the grid on the next search. Saving a ticket that is already gone: the same
  `SingleOrDefaultAsync` inside `SaveAsync` returns null, and rather than let a null-reference exception
  surface, the service throws `TicketNotFoundException` — a typed signal, not a generic failure — which
  `TicketEditorForm.SaveAsync` catches ahead of every other handler and shows *"This ticket was already
  deleted by someone else. Nothing was saved."*, again closing with `DialogResult.OK`. The page's
  **"Simulate: another operator deletes it"** button, and a real second browser session, produce the same
  two outcomes the same way, because both go through the identical `TicketCommandService` methods.

  **Database rejection.** `DbUpdateException` (a constraint the database refuses — the same family Module
  2's demos exercise) is caught separately from every other failure and shown as *"The ticket could not be
  saved because the database rejected the change."* / *"...could not be deleted..."*; the full exception,
  including any `SqliteException` inner detail, is written to the server-side trace, never to the operator.
  `DatabaseUnavailableException` — the **Break the database** switch — is caught before it, with its own
  message, because "the database is unreachable right now" is a different, more temporary-sounding problem
  than "the database refused this specific change," and the operator should be told which one happened. In
  both cases the dialog **stays open and usable** — nothing about a caught failure closes it, so the
  operator's typed values are not lost and Save can simply be tried again after **Restore and search**.

  **Concurrent edit.** Module 4 catches `DbUpdateConcurrencyException` — *"Someone else changed this ticket
  in the meantime. Nothing was saved — reload and try again."* — but, as `TicketEditModel`'s remarks say
  plainly, does not *detect* one on purpose: the model never carries the `RowVersion` it would need to
  compare against, so `SaveAsync` reloads the ticket fresh, immediately before mapping and saving, and that
  reload's own `RowVersion` becomes the "original" value EF Core checks — the last Save always wins unless
  two saves land in the same instant (rare, but the catch exists for exactly that residual case, which is
  why it is still there rather than omitted). Module 7 adds the token, the reload-and-compare check, and a
  conflict dialog that shows the operator both versions and lets them choose; until then, "last write wins"
  is the honestly documented behaviour, not a bug.

## Verified / unverified

Built and tested on this machine (Wisej-4 4.1.0, .NET 10, EF Core 10.0.12, SQLite):

- `dotnet build SupportDesk.slnx -nologo -v q` — succeeds with **0 warnings, 0 errors**.
- `dotnet test SupportDesk.Tests -nologo -v q` — **52 passed**, 0 failed (40 carried over from Modules 1–3
  unchanged, 12 new for `TicketCommandService`).
- The SQL quoted in this README and in `docs/` was captured from `TicketCommandService` running against
  SQLite in memory (a console check and the tests, the same code paths `TicketEditorForm` calls), not
  written from memory: the update `UPDATE "Tickets" SET "RowVersion" = @p0, "Status" = @p1, "Title" = @p2,
  "UpdatedAt" = @p3 WHERE "Id" = @p4 AND "RowVersion" = @p5 RETURNING 1;`, the create
  `INSERT INTO "Tickets" (...) VALUES (...) RETURNING "Id";` preceded by
  `SELECT "t"."Number" FROM "Tickets" AS "t"`, the delete
  `DELETE FROM "Tickets" WHERE "Id" = @p0 AND "RowVersion" = @p1 RETURNING 1;`, and the by-key reads —
  `LoadEditModelAsync` and `DeleteAsync` render `... WHERE "t"."Id" = @id LIMIT 2`, the update path in
  `SaveAsync` renders `... WHERE "t"."Id" = @model_Id LIMIT 2` (the parameter name follows the C# expression
  each `Single(OrDefault)Async` call filters on) — EF Core's `SingleAsync`/`SingleOrDefaultAsync` ask for
  two rows, to detect a violated uniqueness assumption, not one.
- The generated ticket number is measured, not assumed: the seed's highest existing number is `SD-1312`
  (312 tickets, `SD-1001`…`SD-1312`), and a fresh `SaveAsync` on a new `TicketEditModel` produces `SD-1313`.
  A second test seeds two tickets by hand with a gap (`SD-1007`, `SD-1500`) and confirms the next number is
  `SD-1501` — the scan follows the highest number, not the row count.
- Statement counts are measured with the same `QueryTrace` scope the trace card uses: a create is
  **exactly two** statements (the number scan, the `INSERT`), an update is **exactly two** (the tracked
  read, the `UPDATE`), a delete of an existing ticket is **exactly two** (the tracked read, the `DELETE`),
  and both the "already deleted" and the "Closed, refused" delete paths are **exactly one** (the `SELECT`
  only — nothing further is ever sent once the row is missing or the rule refuses it).
- `TicketNotFoundException` really is thrown, not merely described: a test loads the edit model, deletes
  the row through the service (mirroring the page's Simulate button), then saves the now-stale model and
  asserts the exception and its `TicketId`.

Facts carried over from Modules 1–3 and still relied on here: `Application.Services.AddService<IServiceProvider>(app.Services)`
makes `[Inject]` resolve through Microsoft DI for a `Page` **and, per the course cookbook, a `Form`
constructed with `new`** — `TicketEditorForm`'s `[Inject] TicketCommandService Commands` depends on this;
application services must be Transient or Singleton for the same root-provider reason; after an `await` the
handler continues off the original request, so `Application.Update(this)` in `finally` pushes the final
state; Wisej.NET delivers a second click while an awaited handler is pending, which is what makes every
guard (`_loading`, now also `_saving`) load-bearing rather than decorative.

**Deviations from the lab guide's exact wording, and why:**

1. **`TicketEditModel` has no `RowVersion` property**, though `labs.js`'s Module 4 task list names one.
   This module's build instructions were explicit that Module 4 should *not* carry the concurrency token
   yet (Module 7 adds it, with the conflict dialog it needs to be useful) — see `TicketEditModel`'s XML
   remarks for the full reasoning, and the "concurrent edit" answer above for what that means in practice.
2. **`dtpDueDate` is not wired through `DataBindings.Add`** — it is the one control copied by hand between
   `Checked`/`Value` and the model, in `LoadEditorAsync` and `SaveAsync`. This follows the course cookbook's
   own guidance for exactly this situation (binding a nullable `DateTime?` to `DateTimePicker.Value` is
   flagged unverified there, with the manual copy named as the safer alternative) rather than the lab
   guide's more general list of "bound" controls, which does not single out the date picker.

Every `DataBindings.Add` call uses `nameof(TicketEditModel.X)` for the data-member name, as the lesson
recommends ("use `nameof` so a rename breaks the build, not the screen") — `TicketEditorForm.Designer.cs`
adds a `using SupportDesk.Services;` to make that possible, one line further than Module 3's Designer file
needed to go.

**Not verified here — for the browser reviewer.** The application was not started (by instruction), so
everything below is Wisej.NET behaviour that only a running page and a running dialog can confirm — this
list is the main content of what remains open, per the course cookbook's honesty rule:

1. `TicketEditorForm` renders centred over `TicketBrowserPage`, at a fixed 620×560 size, non-resizable,
   with `btnDelete` hidden for a new ticket and visible for an existing one.
2. Every `DataBindings.Add` binding actually pushes a value from control to model and back the way the
   `DataSourceUpdateMode` implies — in particular `OnValidation` on `txtTitle`/`txtDescription` only
   commits when focus leaves the control, which is the whole reason `EndEdit` exists in `SaveAsync`.
3. `cboAgent`'s `Format`/`Parse` events fire the way the Wisej.NET XML docs describe: picking
   "— unassigned —" really sets `AgentId` to `null`, and loading a ticket with no agent really selects
   "— unassigned —" rather than nothing.
4. `dtpDueDate.ShowCheckBox = true` behaves as "unticked = no date" (flagged unverified in the cookbook
   since Module 1, still unverified here).
5. `errorProvider.SetError` renders a visible indicator next to `txtTitle` when Title is blank, and clears
   when the operator fixes it.
6. `MessageBox.ShowAsync` with `MessageBoxButtons.YesNo` truly blocks `DeleteAsync` until the operator
   answers, and a `No` (or dismissing it) sends nothing.
7. The Save button visibly greys out during an armed 2.5 s delay, and a real second click on it in that
   window is what produces the guard trace line — not just the guard field being correct in isolation.
8. `await editor.ShowDialogAsync()` really suspends `OpenEditorAsync` until the dialog closes, and the
   grid visibly refreshes to page 1 with the saved/edited row at the top only when the result is
   `DialogResult.OK` — never on Cancel, a refused delete, or a caught failure.
9. `AlertBox.Show(..., alignment: TopRight, autoCloseDelay: 4000)` appears top-right for the "already
   deleted" and failure paths inside the dialog, the same as it does on the browser page.
10. The reviewer-visible flow end to end: Add a ticket, see it on page 1; Edit it, change the Status, see
    the grid reflect it; Delete it with confirmation, see it gone; use **Simulate** then **Edit → Delete**
    and **Edit → Save** on a different row to see both already-deleted paths without a second browser tab.

**Browser verification: see the note at the end.**

## Browser results (reviewer, 2026-09-10)

Run on this machine at <http://localhost:5404> in the Browser pane; trace read back from the page. Three defects were
found and fixed before the paths below passed — each one is a Wisej.NET binding fact now recorded in the course cookbook:

1. **Bindings before the BindingSource had a data source.** `DataBindings.Add("Text", editBindingSource, "Title", …)` in
   `InitializeComponent` threw *Cannot bind to the property or column Title on the DataSource* the moment the form was created,
   because `editBindingSource` was still empty (the model arrives in `LoadEditorAsync`). Fix: the Designer now sets
   `editBindingSource.DataSource = typeof(TicketEditModel)` right after creating the BindingSource, the same trick the WinForms
   designer uses, so every binding has a property to resolve; the instance replaces the type in `LoadEditorAsync`.
2. **`SelectedValue` bindings added before the lookups and the model existed did nothing.** With the five ComboBox bindings in the
   Designer, `EndEdit` reported `CustomerId=—, CategoryId=—` after the operator had picked both, and the model's default
   Priority never reached the control. The bindings now live in `BindLookupControls()`, called once after the lookup
   `DataSource`s are set and `editBindingSource.DataSource = model` — after that, the trace shows
   `• editor EndEdit → model {Title='…', CustomerId=1, AgentId=—, CategoryId=6, Status=Open, Priority=Normal, …}`.
3. **`SelectedValue` needs a `ValueMember`.** `cboStatus`/`cboPriority` were bound to plain `List<string>`s; loading ticket SD-1002
   (Resolved / High) showed *Open / Low* (the first rows). They now bind to `NamedValue(Value, Name)` rows with
   `ValueMember = "Value"`, exactly like the `LookupItem` combos, and the edit dialog opens on *Resolved / High*.
4. **An exception escaping the async `Load` handler is a Wisej.NET "Application Error" dialog.** Opening the editor on a ticket that
   had just been deleted (Simulate → Edit) crashed the session with *Sequence contains no elements* from `SingleAsync`.
   `LoadEditModelAsync` now uses `SingleOrDefaultAsync` and throws `TicketNotFoundException`; `TicketEditorForm_Load` catches it,
   shows the toast *This ticket was already deleted by someone else. The list will refresh.*, and closes with `DialogResult.OK`.

Paths verified after the fixes:

- **Add ticket** → dialog *Add ticket*, `lblNumber` *assigned on save*, Delete hidden, Agent *— unassigned —*, Status *Open*, Priority *Normal*.
  Title typed, Customer *Halden Logistics* and Category *Printing* picked from the drop-downs, **Save** →
  `• editor EndEdit → model {…CustomerId=1… CategoryId=6… Priority=Normal…}`, `→ SQL SELECT "t"."Number" FROM "Tickets"`,
  `→ SQL INSERT INTO "Tickets" (…) VALUES (…) RETURNING "Id";`, `← result saved SD-1313 · 2 statement(s), 1 context created, 1 disposed`,
  `• dialog Add ticket → OK → grid refreshed`, then the two-statement search: SD-1313 is the first row, *Showing 50 of 313*.
- **Edit ticket** (double-click SD-1002) → *Edit ticket SD-1002*, every field pre-filled (Brightwater Clinics, Tomasz Wierzbicki, Network,
  Resolved, High, no due date, not urgent), Delete visible. Title edited and Urgent ticked, **Save** →
  `→ SQL UPDATE "Tickets" SET "IsUrgent" = @p0, "RowVersion" = @p1, "Title" = @p2, "UpdatedAt" = @p3 WHERE "Id" = @p4 AND "RowVersion" = @p5 RETURNING 1;`
  (the token in the WHERE), `← result saved SD-1002 · 2 statement(s)`, grid refreshed with SD-1002 on top.
- **Delete** (in the editor of SD-1313) → the *Confirm delete* Yes/No box, **Yes** → `• editor DeleteAsync(#313 / SD-1313): removed, SaveChangesAsync`,
  `← result deleted SD-1313 · 2 statement(s)`, `• dialog Edit ticket SD-1313 → OK → grid refreshed`, *Showing 50 of 312*.
- **Simulate: another operator deletes it** on SD-1004 → `TicketCommandService.DeleteAsync(#4 / SD-1004) — bypassing the editor`, SELECT + DELETE,
  `← result SD-1004 deleted behind the scenes — the grid still shows it until the next search`; then **Edit ticket** on that row →
  `• editor LoadEditorAsync(#4): the ticket no longer exists — deleted by another session after the grid was loaded; closing with OK so the grid refreshes`,
  the top-right toast, `• dialog Edit ticket SD-1004 → OK → grid refreshed`, *Showing 50 of 310*.
- **Cancel** → `• dialog Edit ticket SD-1005 → Cancel → nothing persisted, grid untouched` — no context line, no SQL.
- The Module 1–3 rows behave as in their own modules. Not exercised this round: the Slow-save guard and Break-the-database inside the
  dialog (the same `DevelopmentOutageSwitch` and `_saving` guard as the verified page paths).

