# Capstone review

The Module 7 lab is also the capstone submission. This walks the rubric
(`assets/courses/ef-core-binding/labs/m7.json` / `ef7s2.html`) against this solution, with concrete
file/method evidence for each area.

| Area | Weight |
|---|---|
| Architecture | 20% |
| Binding | 20% |
| EF Core correctness | 20% |
| Validation UX | 15% |
| Performance | 15% |
| Deployment and diagnostics | 10% |

## Architecture — 20%

**Excellent looks like:** *clear service boundary, factory usage, no shared mutable state.*

- Service boundary: `SupportDesk.Web` (Wisej.NET forms/pages, `Nullable`/`ImplicitUsings` disabled) →
  `SupportDesk.Services` (stateless, UI-free, `TicketQueryService`/`TicketCommandService`/`TicketValidator`/
  `ConflictResolution`) → `SupportDesk.Data` (the only project referencing the SQLite provider). See
  `docs/ArchitectureNote.md` for the full diagram.
- Factory usage: every service holds only `IDbContextFactory<SupportDeskContext>` (or, for a handful of lab
  props, a small singleton like `DevelopmentOutageSwitch`/`TransactionFailureSwitch`/`StartupDiagnostics`,
  none of them mutable *application* state). No `DbContext` field exists anywhere outside a method body.
- No shared mutable state: `TicketBrowserPage`'s fields (`_pageIndex`, `_totalCount`, `_loading`, …) are
  session state by design — they belong to one browser tab, not shared across sessions. The one place shared
  mutable state deliberately exists is the lab props themselves (`DevelopmentOutageSwitch.IsDown`,
  `TransactionFailureSwitch.FailAfterFirstWrite`) — singletons on purpose, so one reviewer's click can be seen
  by that same reviewer's next click, and both are documented as lab instruments, not production patterns.
  `SharedContextAntiPattern` (Module 1) is the one place this codebase *shows* what sharing a `DbContext`
  actually breaks, as a deliberately-wrong demo, never a pattern to keep.

## Binding — 20%

**Excellent looks like:** *BindingSource, DTO or edit model, EndEdit and grid refresh all correct.*

- `ticketBindingSource.DataSource = result.Items.ToList()` — a materialised `List<TicketListItem>`, never a
  query, never an entity. `TicketListItem` is a positional record; the grid only ever reads it.
  (`TicketBrowserPage.LoadTicketsAsync`)
- `editBindingSource.DataSource = model` (a `TicketEditModel`, not a `Ticket`); every text/combo/checkbox
  control binds through `DataBindings.Add`, with the two verified Wisej.NET rules from the cookbook
  respected: the `BindingSource`'s `DataSource` is a `Type` before the instance exists
  (`editBindingSource.DataSource = typeof(TicketEditModel)` in the Designer), and lookup `SelectedValue`
  bindings are added in `BindLookupControls()` — after the lookup `DataSource`s and the model both exist, not
  in the Designer. (`TicketEditorForm.Designer.cs`, `TicketEditorForm.LoadEditorAsync`/`BindLookupControls`)
- `EndEdit`: `TicketEditorForm.SaveAsync` calls `this.editBindingSource.EndEdit()` before reading the model,
  so an in-progress edit in the currently focused control is flushed before validation or save.
- Grid refresh: `TicketBrowserPage.OpenEditorAsync` re-searches (`_pageIndex = 0; await
  LoadTicketsAsync(...)`) only when the dialog closes with `DialogResult.OK` — which both the ordinary save
  path and a successful Overwrite reach; Cancel and an unresolved conflict (Reload, still-open) correctly do
  not trigger a refresh until the editor itself eventually closes with OK.

## EF Core correctness — 20%

**Excellent looks like:** *async, tracking versus no-tracking, migrations and concurrency handled.*

- Async everywhere: every database call in `SupportDesk.Services` and `SupportDesk.Data` is the `Async`
  overload, awaited; there is no `.Result`/`.Wait()`/blocking call anywhere in the solution.
- Tracking vs. no-tracking: reads for display (`SearchTicketsAsync`, `LoadEditModelAsync`, `GetLookupsAsync`,
  `GetCustomersAsync`) are `.AsNoTracking()`; reads for a write (`SaveAsync`'s existing-ticket branch,
  `DeleteAsync`, `CloseTicketWithCommentAsync`) are tracked, loaded fresh, inside the very method that is
  about to change them — the two never share a context or an entity instance.
- Migrations: `InitialCreate` (Module 2) is still the only migration — Module 7 added no entity properties,
  so no new migration was needed (`TicketEditModel.RowVersion` is a UI-model field, not an entity change).
  The release script is regenerated and committed (`artifacts/sql/supportdesk_migrations.sql`,
  `docs/DeploymentNotes.md`).
- Concurrency: the full `RowVersion` round trip, `DbUpdateConcurrencyException`, the conflict list, and
  Reload/Overwrite — see `docs/ConcurrencyResolution.md` and `ConcurrencyAndTransactionsTests.cs`.

## Validation UX — 15%

**Excellent looks like:** *field and summary errors clear and reusable.*

Unchanged and still working from Module 5: `TicketValidator.Validate` (DataAnnotations + the closed/future-
due-date cross-field rule) is the one place validation actually runs; `TicketEditorForm.ShowValidation` maps
every message onto `errorProvider.SetError` for the matching control and joins everything into
`validationSummaryLabel`; `btnSave.Enabled` tracks the model's live validity; a duplicate ticket number still
reaches the database and becomes one friendly sentence via `DbUpdateException`. Module 7 adds one more layer
below it in the same shape: a genuine `DbUpdateConcurrencyException` becomes the `ConflictDialog`'s field-by-
field list — the same "translate the exception into business language, never show raw SQL" discipline
`FailDbUpdateAsync` already used. `TicketValidatorTests` (13 tests, Module 5) and `DuplicateNumberTests`
(3 tests) are unchanged and still pass.

## Performance — 15%

**Excellent looks like:** *server-side filtering, paging and projection with measured notes.*

The plan names a "measured optimisation" as Module 6's deliverable. Module 7 was built directly from Module 5
(not Module 6 — the two were built in parallel from the same Module 5 base) because the optimised shape
Module 6 measures was already present in Module 5's `TicketQueryService.SearchTicketsAsync`, and it carried
into Module 7 unchanged: filters, ordering and paging are
all composed on the `IQueryable` before either `await` runs, so the database does the filtering
(`WHERE`), the counting (`COUNT(*)`) and the paging (`LIMIT @p OFFSET @p`) — nothing is pulled into memory
and filtered in .NET. The whole search is **exactly two statements** regardless of how many rows exist
(`TicketQueryServiceTests`/`TicketSearchTests` assert the statement count directly via `QueryTrace`), and
every list the UI binds is a `Select`-projected DTO (`TicketListItem`, `LookupItem`), never a full tracked
`Ticket` graph. The anti-pattern this correctness is contrasted against — binding an unexecuted
`IQueryable` to the grid after its context has disposed — is kept as a live, working demo
(`buttonBindQuery_Click` / `BoundIQueryableAntiPattern`) specifically so the difference is visible, not just
asserted.

## Deployment and diagnostics — 10%

**Excellent looks like:** *safe configuration, logging and a migration plan.*

- Safe configuration: `appsettings.Production.json`'s connection string is a placeholder naming the
  `ConnectionStrings__SupportDesk` environment variable — never a real value in source.
- Logging: production categories set `Microsoft.EntityFrameworkCore.Database.Command` to `Warning` and
  `SupportDesk` to `Information`; `EnableSensitiveDataLogging` stays behind the `Development` check it always
  was, now with a startup line reporting the decision.
- Migration plan: reviewed script → applied by the release pipeline as one step → traffic switches after;
  never `Migrate()` from application startup outside Development. Full detail, including the
  `--idempotent`/SQLite deviation, in `docs/DeploymentNotes.md`.
- Diagnostics: the page's **Environment & diagnostics** panel (`StartupDiagnostics`, read once at host start,
  rendered by `TicketBrowserPage.RefreshEnvironmentPanel`) shows the environment name, whether migrations ran
  at startup, whether sensitive-data logging is on, the applied migrations, and the script path — the same
  facts the server console prints, never guessed twice.

## What this review does not claim

This document, `docs/ArchitectureNote.md`, `docs/AiGroundingNote.md` and `docs/Checklists.md` are all built
and reasoned about from source, `dotnet build`/`dotnet test`/`dotnet ef` output and console checks — never
from running the application in a browser, which this build was explicitly asked not to do. The README's
"Verified / unverified" table is the honest boundary: what a test or a build/console check actually proved,
versus what the reviewer still needs to click through in the Browser pane (the two-browser-session
concurrency walkthrough chief among them).
