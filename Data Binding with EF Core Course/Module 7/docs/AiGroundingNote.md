# AI grounding note

What an AI assistant helping on this codebase should answer from, and why — the fifth capstone piece the lab
guide asks for. This is a short, honest note about the patterns actually implemented here, not a general
essay on EF Core or Wisej.NET.

## The patterns to answer from

1. **Short-lived contexts, created from a factory.** Every read and every write in this solution creates its
   own `SupportDeskContext` from `IDbContextFactory<SupportDeskContext>`, uses it for one operation, and
   disposes it (`await using`) before the method returns. There is no `Scoped` `DbContext` registration
   anywhere, and no `DbContext` field on any `Page` or `Form`. An assistant suggesting a `Scoped`
   registration, or a `DbContext` held across two handlers, is suggesting something this codebase does not
   do and — on Wisej.NET, whose `[Inject]` resolves through the root DI provider (see
   `docs/ArchitectureNote.md`) — something that would throw at page construction time.
2. **`BindingSource` as the UI bridge.** `TicketBrowserPage`'s grid and `TicketEditorForm`'s fields never
   bind directly to an EF Core entity or a live query. The browser binds a materialised `List<TicketListItem>`
   (never an `IQueryable`: bound after its context is disposed, it fails with `ObjectDisposedException` when
   the grid enumerates, see `docs/TicketBrowserBinding.md`); the editor binds a `TicketEditModel`, a plain UI-only class
   `TicketCommandService` maps onto a tracked entity only inside `SaveAsync`. An assistant should not suggest
   binding a grid or form directly to `DbSet<T>` or an `IQueryable`.
3. **Explicit validation, not implicit.** `TicketValidator.Validate` is the one place
   `Validator.TryValidateObject` actually runs, called by `TicketEditorForm.SaveAsync` before any `DbContext`
   is created. EF Core does not run `DataAnnotations` before `SaveChangesAsync`, and Wisej.NET's
   `BindingSource` does not either — an assistant suggesting "just add `[Required]` and it will validate on
   save" is describing ASP.NET Core MVC model binding, not this stack.
4. **Async loading guards.** Every page and form handler follows the same shape: a boolean guard field
   (`_loading` / `_saving`) checked first, set busy, one awaited service call, a friendly message in `catch`,
   the UI restored in `finally` with `Application.Update(this)`. Wisej.NET *does* deliver a second click
   while an awaited handler is still pending — the guard, not the framework, is what drops it (verified in
   the browser in Module 1). An assistant should not assume disabling a button is enough by
   itself, or that Wisej.NET serialises clicks for you.
5. **`RowVersion` concurrency, restored as `OriginalValue` before mapping.** The one new pattern this module
   adds: `TicketEditModel.RowVersion` is hidden state carried from `LoadEditModelAsync` to `SaveAsync`, and
   `db.Entry(ticket).Property(t => t.RowVersion).OriginalValue = model.RowVersion` runs **before** any other
   field is mapped, on a **freshly loaded** tracked entity (never the editor's own no-tracking copy). An
   assistant suggesting "just catch `DbUpdateConcurrencyException` and save again" without restoring
   `OriginalValue` first is describing a save that can never detect a conflict at all — see
   `docs/ConcurrencyResolution.md`'s first Student review question.

## Where the assistant should say "I'm not sure" instead of inventing

- **SQL Server-specific behaviour.** This module runs entirely on SQLite. Anything about server-generated
  `rowversion` columns, `SqlServerRetryingExecutionStrategy`, idempotent migration scripts with per-migration
  `IF NOT EXISTS` guards, or `OFFSET…FETCH` paging is a different provider's behaviour, documented here only
  as a contrast (see `docs/ConcurrencyResolution.md`'s "SQLite needs the two extra lines" note inherited from
  Module 2, and `docs/DeploymentNotes.md`'s `--idempotent` deviation) — an assistant should say so rather than
  presenting SQL Server behaviour as what this application does.
- **Wisej.NET binding quirks not yet hit in this course.** The cookbook (`_template/COOKBOOK.md`) records
  facts *verified in the browser* on specific modules (the `BindingSource.DataSource = typeof(...)` trick,
  `SelectedValue` needing a `ValueMember`, the ComboBox binding-order rule). Anything about Wisej.NET not
  covered there is unverified for this codebase specifically, even if it is true of WinForms in general.
- **Whether a specific browser path in this module actually works.** This report's own "Verified /
  unverified" section in the README is the source of truth for what has and has not been exercised in the
  Browser pane — an assistant should defer to it rather than assume a code path that compiles and passes its
  unit test also renders correctly.

## Evidence

- The five patterns above are each visible in the running app (search and paging, Add Ticket / Edit Ticket,
  validation in the editor, the two-tab conflict; see the README) and covered by at least one test in
  `SupportDesk.Tests` (`TicketQueryServiceTests`, `TicketCommandServiceTests`, `TicketValidatorTests`,
  `ConcurrencyAndTransactionsTests`).
- `docs/ArchitectureNote.md` — the service boundary and DI bridge these patterns depend on.
- `_template/COOKBOOK.md` — the verified-vs-unverified distinction this note follows for Wisej.NET facts.
