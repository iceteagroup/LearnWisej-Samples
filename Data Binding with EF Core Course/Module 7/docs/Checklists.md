# Checklists

Ticked against this solution, each line with the file/test that proves it.

## Concurrency

- [x] `RowVersion` is a real concurrency token on `Ticket`, stamped by `SupportDeskContext.SaveChanges(Async)`
      because SQLite has no server-generated `rowversion` type. — `SupportDesk.Data/SupportDeskContext.cs`
      (`IsRowVersion()`, `StampTickets`), inherited from Module 2, unchanged.
- [x] `TicketEditModel` carries `RowVersion` as hidden state; no control binds to it; `TicketValidator`
      ignores it. — `SupportDesk.Services/TicketEditModel.cs`.
- [x] `LoadEditModelAsync` fills it from the no-tracking read. — `TicketCommandService.ToModel`.
- [x] `SaveAsync` restores it as `OriginalValue` on the freshly loaded tracked entity, **before** mapping any
      other field. — `TicketCommandService.SaveAsync`; the exact line is quoted in
      `docs/ConcurrencyResolution.md`.
- [x] A stale save throws `DbUpdateConcurrencyException` — proved through the command service, not just raw
      EF Core. — `ConcurrencyAndTransactionsTests.SaveAsync_with_a_stale_RowVersion_throws_DbUpdateConcurrencyException`.
- [x] The conflict is reproducible on demand: two browser sessions editing the same ticket (lab step 5),
      and in the tests a second context that changes the row between load and save
      (`ConcurrencyAndTransactionsTests`' `ChangeAsAnotherOperatorAsync` helper). —
      `docs/ConcurrencyResolution.md` "Reproducing the conflict"; the two-session path itself is
      **unverified** here — see the README's Verified/unverified table, the reviewer confirms it in the
      Browser pane.
- [x] `ConflictResolution.BuildConflictListAsync` lists every differing field (Your / Database / Original),
      skips `UpdatedAt` as noise, keeps `RowVersion` labelled and hex-formatted, and reports a deleted row
      (`GetDatabaseValuesAsync` returning `null`) as `ConflictSet.DeletedByAnotherUser`. —
      `ConcurrencyAndTransactionsTests.SaveAsync_conflict_attaches_a_ConflictSet_…` and
      `BuildConflictListAsync_reports_the_row_as_deleted_…`.
- [x] The conflict list is built **while the throwing context is still open** (inside
      `TicketCommandService.SaveAsync`'s own catch), not after the editor's `await using` would have already
      disposed it — the exact reason is documented, not just asserted. — `ConflictResolution.cs`'s class
      remarks; `docs/ConcurrencyResolution.md`.
- [x] `ConflictDialog` (`Form`, `CenterParent`) lists Field / Your value / Database value / Original value in
      a `DataGridView`, explains what happened, and offers Reload, Overwrite and Cancel. —
      `SupportDesk.Web/ConflictDialog.cs` + `.Designer.cs`.
- [x] Overwrite is visible only when the role permits it (`ConflictResolution.CanOverwrite`, Supervisor only)
      — never a silent automatic retry. — `ConflictDialog`'s constructor; `cboRole` on the page.
- [x] Reload takes the database's values into the model and the editor; the editor **stays open** with the
      fresh `RowVersion`. — `TicketEditorForm.ShowConflictDialogAsync`'s Reload branch;
      `ConcurrencyAndTransactionsTests.LoadEditModelAsync_after_a_conflict_returns_the_current_database_values_…`.
- [x] Overwrite saves the operator's values with the database's current `RowVersion` forced as
      `OriginalValue`; the editor **closes with OK** afterward and the token changes again on the write. —
      `TicketEditorForm.OverwriteAsync`; `ConcurrencyAndTransactionsTests.SaveAsync_overwrite_with_the_databases_RowVersion_wins_…`.
- [x] After either resolution, the parent grid refreshes. — `TicketBrowserPage.OpenEditorAsync` re-searches
      on `DialogResult.OK`, unchanged from Module 4/5; both Reload-then-later-Save-OK and Overwrite-then-OK
      reach it. *(The dialog itself does not refresh the grid — only a `DialogResult.OK` close of the editor
      does, by the existing, already-working mechanism.)*

## Deployment and migrations

- [x] `dotnet ef migrations script --idempotent …` attempted verbatim; the SQLite `NotSupportedException` it
      throws is documented, not hidden. — `docs/DeploymentNotes.md` "The `--idempotent` deviation".
- [x] The release script was regenerated (without `--idempotent`, for the reason above) and committed. —
      `artifacts/sql/supportdesk_migrations.sql`; `SupportDesk.Tests/MigrationScriptTests.cs` asserts its
      real content.
- [x] `appsettings.Production.json` names an environment-variable-sourced connection string, never a real
      value. — `SupportDesk.Web/appsettings.Production.json`.
- [x] Production logging categories are configured (`Microsoft.EntityFrameworkCore.Database.Command` =
      `Warning`, `SupportDesk` = `Information`). — same file; `docs/DeploymentNotes.md`.
- [x] No migration on startup outside Development. — `Startup.cs`'s `if (app.Environment.IsDevelopment())`
      guard is the only call site of `SupportDeskDevelopmentDatabase.EnsureReadyAsync`.
- [x] A startup log line names the environment, and outside Development a second line says migrations are
      not applied at startup. — `Startup.cs`'s two `Console.Error.WriteLine` calls.
- [x] `EnableSensitiveDataLogging` stays inside `if (isDevelopment)`, with a startup console line reporting
      ON/OFF. — `SupportDeskDataServiceCollectionExtensions.AddSupportDeskData`; `Startup.cs`.
- [x] Deployment notes name where each environment's connection string comes from, the release steps,
      rollback notes and the logging configuration. — `docs/DeploymentNotes.md`.

## AI grounding

- [x] A short note naming the patterns an AI assistant should answer from (short-lived contexts,
      `BindingSource` bridge, explicit validation, loading guards, `RowVersion` concurrency) and where it
      should say "I'm not sure" instead of inventing (SQL Server-specific behaviour, unverified Wisej.NET
      binding facts, unverified browser paths in this module). — `docs/AiGroundingNote.md`.
- [x] Every pattern the note names is visible in the running app and covered by at least one test — not
      aspirational. — cross-referenced in `docs/AiGroundingNote.md`'s Evidence section.
