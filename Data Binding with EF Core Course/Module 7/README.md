# SupportDesk · Data Binding with EF Core · Module 7

Local lab build for **Module 7 · Concurrency, transactions, deployment, diagnostics and the capstone**. It
follows the walkthrough video ("Two users, one ticket: resolving the conflict"): Dana closes ticket T-1042
at 10:43, Priya raises its priority at 10:44 — without a token, whichever save lands second silently
overwrites the first and nobody is told. `TicketEditModel` now carries the `RowVersion` it loaded as hidden
state; `TicketCommandService.SaveAsync` restores it as the tracked entity's `OriginalValue` **before**
mapping any other field, so a stale save throws `DbUpdateConcurrencyException` instead of a silent
overwrite. `TicketCommandService.SaveAsync`'s own catch builds the field-by-field conflict list (while its
`DbContext` is still open — see `docs/ConcurrencyResolution.md` for why that matters) and attaches it to the
exception; the editor's own `catch (DbUpdateConcurrencyException ex)`, already sitting above `catch
(DbUpdateException ex)` since Module 4/5, now opens a new modal `ConflictDialog` instead of just showing a
friendly sentence. Reload is always offered; Overwrite only to a Supervisor (`cboRole` on the page, checked
by `ConflictResolution.CanOverwrite`) — never a silent automatic retry. `TicketCommandService.CloseTicketWithCommentAsync`
adds a second, independent lab: an explicit transaction across two `SaveChangesAsync` calls, with a lab
prop (`TransactionFailureSwitch`) that fails the unit right after the first write so the rollback is visible.
Deployment gets the same honesty the rest of the course insists on: `appsettings.Production.json`'s
connection string is a placeholder naming an environment variable, production logging categories are
configured, `EnableSensitiveDataLogging` stays behind the Development check it always was (now with a
startup line reporting the decision), and application startup never calls `Migrate()` outside Development.
Everything Module 1–5 could do still works, further down the bottom bar.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 solution on this machine with a local SQLite
file. **This module's lab is also the capstone submission** — see [Capstone](#capstone) below.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 7/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5407
```

Then open <http://localhost:5407>. (Visual Studio: open `SupportDesk.slnx`, press F5 — the port and the
Development environment are in `SupportDesk.Web/Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package, EF Core 10.0.12
(`Microsoft.EntityFrameworkCore.Sqlite` + `Design`) and the global `dotnet-ef` 10.0.12 tool. The web project
multi-targets `net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

In Development the host **migrates and seeds** `SupportDesk.Web/App_Data/supportdesk.db` before the first
session, exactly as in Modules 2–5 — the entity model did not change in Module 7 (`TicketEditModel.RowVersion`
is a UI-model field, not an entity property; `Ticket.RowVersion` has carried the real concurrency token since
Module 2), so `InitialCreate` is still the only migration:

```
[SupportDesk] environment: Development · sensitive-data logging: ON (Development)
[SupportDesk] MigrateAsync: 0 pending migration(s) applied, 1 applied in total (20260910150534_InitialCreate)
[SupportDesk] Development seed: seeded 5 customers, 3 agents, 6 categories, 312 tickets, 99 comments in ... ms
```

> **Coming from Module 5?** Its `App_Data/supportdesk.db` already has data. Module 7 has its own `App_Data`
> folder, so nothing is shared — but if you ever copy one over, delete it or press **Reset & reseed**.

Delete `SupportDesk.Web/App_Data/supportdesk.db` (and its `-wal`/`-shm` companions) to reset the module
completely; **Reset & reseed** on the page does the same thing without a restart.

Tests: `dotnet test SupportDesk.Tests` — **82 tests** against SQLite in memory (68 carried over from
Modules 1–5 unchanged, 10 new in `ConcurrencyAndTransactionsTests.cs`, 4 new in `MigrationScriptTests.cs`).

Migration commands, from the `Module 7` folder (`--framework` is required because the web project
multi-targets):

```bash
dotnet ef migrations list   --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0
dotnet ef migrations script --project SupportDesk.Data --startup-project SupportDesk.Web \
    --framework net10.0 --output artifacts/sql/supportdesk_migrations.sql
```

**No `--idempotent`** — see [Deviations](#deviations-from-the-lab-guides-exact-wording-and-why) below and
`docs/DeploymentNotes.md`: the flag throws `System.NotSupportedException` on the SQLite provider.

## What to click

Bottom bar **row 1** is now the Module 7 paths — Edit ticket (moved up here from Module 4's row, since
Module 7's paths start from it), the two concurrency/transaction demos, `cboRole`, and
Break/Restore/Clear trace (moved up from the Module 5 row, since they still govern every dialog this row
opens). **Row 2** ("Module 5 · validation") keeps the five negative-case demos and Slow save working;
**row 3** ("Module 4 · editor") keeps Add ticket and the delete-simulation; **row 4** ("Module 3 ·
browser"), **row 5** ("Module 2 · model") and **row 6** ("Module 1 · lifetimes") are unchanged in content,
renumbered down one row each. A new **Environment & diagnostics** panel sits under the Model & migration
card, above the bottom bar.

| Action | Path | What you should see |
|---|---|---|
| **Edit ticket** (`btnEdit`), then **Lab: other operator changes it** (`btnLabSimulateChange`, bottom-left inside the editor — the page-level `buttonSimulateChange` cannot be clicked while the modal editor is open, so it serves the two-tab walkthrough and the *Simulate → then Edit* variant), then **Save** in the editor | success → conflict | The Simulate click updates the same row through a separate context: `• simulate SimulateAnotherOperatorChangeAsync(#N / SD-xxxx): Status = Closed, Priority = High — a fresh RowVersion is now on the row`. Back in the editor, **Save**: `→ SQL UPDATE "Tickets" SET … WHERE "Id" = @p4 AND "RowVersion" = @p5 RETURNING 1;`, `• editor SaveAsync: caught DbUpdateConcurrencyException — 0 rows matched the WHERE clause`, one `• conflict Field: yours '…' · database '…' · original '…'` line per differing field, then the **Conflict** dialog opens listing the same fields in its grid |
| In the conflict dialog, **Reload** | Reload | `• dialog Conflict → Reload`, `• editor Reload complete — the editor now shows the database's values and the new RowVersion; still open` — the dialog closes, the **editor stays open**, now showing Closed / High and a fresh `lblNumber`-adjacent state; Save again now succeeds ordinarily |
| Set `cboRole` to **Supervisor**, repeat the conflict, click **Overwrite** | Overwrite | The dialog's **Overwrite** button is visible only for Supervisor (`labelPolicy` explains the policy either way). Click it: `• dialog Conflict → Overwrite (Supervisor)`, `← result overwrite saved SD-xxxx — the database's token was used as OriginalValue, your values won`, the editor **closes with OK**, the grid refreshes with your values on top |
| Set `cboRole` back to **Agent**, repeat the conflict | policy | The dialog's `labelPolicy` reads *"Role: Agent — Overwrite is hidden. Only a Supervisor may discard another operator's change."* and the Overwrite button is not shown — Reload and Cancel only |
| **Close with comment (transaction)** (`buttonCloseWithComment`), with a row selected | success · transaction | `• transaction CloseTicketWithCommentAsync(#N / SD-xxxx, "Closed by the transaction demo")`, `→ SQL BeginTransactionAsync` (via the Note line), one `UPDATE "Tickets" …` then one `INSERT INTO "TicketComments" …`, `• transaction committed — both writes are durable`, `← result SD-xxxx closed with comment #k · … statement(s)` |
| **Close with comment — fail after first write** (`buttonCloseWithCommentFail`) | failure · rollback | `• transaction TransactionFailureSwitch.FailAfterFirstWrite = true`, then the same call: one `UPDATE` lands, `• transaction: TransactionFailureSwitch armed — throwing before the comment is written`, `• transaction rolled back — the comment was not written and the status is unchanged`, red banner, `● fault`. Click the ordinary **Close with comment** button again on the same ticket afterward — the switch reset itself, and it succeeds |
| **Break the database**, then Edit → Save, or either transaction button | failure · outage | Unchanged mechanism from every earlier module: `DatabaseUnavailableException` → friendly banner, the editor/page stays usable |
| **Restore and search** | recovery | Unchanged |
| The Module 5 validation row, the Module 4 Add ticket / Simulate-delete row, the Module 3 browser row, the Module 2 model row and the Module 1 lifetimes row | — | Behave exactly as in their own module's README |

### Two-session concurrency walkthrough (for the Browser pane reviewer)

The one-session **Simulate: another operator changes it** button above reproduces the conflict without a
second browser tab; this is the two-tab alternative the lesson and the lab guide both name, for when the
reviewer wants to see two independent Wisej.NET *sessions* (not just two contexts) collide:

1. Open <http://localhost:5407> in one browser tab. Open a **second, independent tab** (or a private/
   incognito window — a different Wisej.NET session, not a second view of the same one) at the same URL.
2. In **both** tabs, click **Edit ticket** on the same row (pick the same ticket number in each).
3. In **tab A**, change a field (e.g. tick Urgent) and click **Save**. It succeeds ordinarily — `← result
   saved SD-xxxx`, the dialog closes, tab A's grid refreshes.
4. In **tab B**, change a *different* field (e.g. Priority) and click **Save**. Tab B's `RowVersion` is now
   stale — the same trace shape as the Simulate button above, ending in the **Conflict** dialog in tab B.
5. Resolve it in tab B with Reload or (as a Supervisor) Overwrite, exactly as above.

## Deliverables

The five "Required deliverables" from `assets/courses/ef-core-binding/labs/m7.json` / `ef7s2.html`:

| # | Deliverable | Evidence |
|---|---|---|
| 1 | `RowVersion` carried in the edit model and set as `OriginalValue` before saving | [`docs/ConcurrencyResolution.md`](docs/ConcurrencyResolution.md) · `TicketEditModel.RowVersion`, `TicketCommandService.SaveAsync`, `ConcurrencyAndTransactionsTests.SaveAsync_with_a_stale_RowVersion_throws_DbUpdateConcurrencyException` |
| 2 | Reproducible concurrency conflict caught as `DbUpdateConcurrencyException` | [`docs/ConcurrencyResolution.md`](docs/ConcurrencyResolution.md) · `TicketCommandService.SimulateAnotherOperatorChangeAsync` (one-session repro) + the two-session walkthrough above; `TicketEditorForm.SaveAsync`'s `catch (DbUpdateConcurrencyException ex)` |
| 3 | Conflict dialog listing proposed and database values with Reload and Overwrite paths | [`docs/ConcurrencyResolution.md`](docs/ConcurrencyResolution.md) · `SupportDesk.Web/ConflictDialog(.Designer).cs`, `ConflictResolution.BuildConflictListAsync`/`CanOverwrite`, `TicketEditorForm.ShowConflictDialogAsync`/`OverwriteAsync` |
| 4 | Idempotent SQL migration script with deployment notes and environment-specific connection strings | [`docs/DeploymentNotes.md`](docs/DeploymentNotes.md) · `artifacts/sql/supportdesk_migrations.sql`, `SupportDesk.Web/appsettings.Production.json`, `MigrationScriptTests.cs` |
| 5 | Sensitive-data logging disabled outside development and the capstone review completed | [`docs/DeploymentNotes.md`](docs/DeploymentNotes.md) · `SupportDeskDataServiceCollectionExtensions.AddSupportDeskData`, `Startup.cs`'s `StartupDiagnostics` reporting; [`docs/CapstoneReview.md`](docs/CapstoneReview.md) |

The transactions/execution-strategy lesson objective has its own note:
[`docs/TransactionsAndRetries.md`](docs/TransactionsAndRetries.md) (why one `SaveChangesAsync` is already a
transaction, `CloseTicketWithCommentAsync`'s explicit transaction and its rollback demo, and why SQLite has
no retrying execution strategy to wrap it in).

## Where things live

```
Module 7/
├─ SupportDesk.slnx                 the four projects
├─ artifacts/sql/
│  └─ supportdesk_migrations.sql    dotnet ef migrations script (no --idempotent — see docs/DeploymentNotes.md); content unchanged since Module 2 (no entity changes here)
├─ SupportDesk.Web/                 the Wisej.NET application (net10.0-windows;net10.0)
│  ├─ Startup.cs                    + StartupDiagnostics population, the "no Migrate() outside Development" comment, AddTransient<ConflictResolution>()
│  ├─ TicketBrowserPage.cs          + cboRole fill, RefreshEnvironmentPanel, buttonSimulateChange/CloseWithComment(Fail)_Click, ReadRole
│  ├─ TicketBrowserPage.Designer.cs + panelActionsM7 (row 1), labelEnvironment(Title) in panelModel; panelActions/2/3/4/5 shifted down, Edit ticket/Break/Restore/Clear moved into panelActionsM7
│  ├─ TicketEditorForm.cs           + UserRole ctor param, ShowConflictDialogAsync, OverwriteAsync; the DbUpdateConcurrencyException catch now builds and shows the conflict, not just a friendly sentence
│  ├─ ConflictDialog.cs             NEW — the conflict resolution modal
│  └─ ConflictDialog.Designer.cs    NEW — DataGridView (Field/Your/Database/Original) + Reload/Overwrite/Cancel
├─ SupportDesk.Data/
│  ├─ SupportDeskContext.cs         unchanged since Module 2 — RowVersion/StampTickets already there
│  └─ Diagnostics/
│     ├─ TransactionFailureSwitch.cs  NEW — the transaction-rollback lab prop + SimulatedTransactionFailureException
│     └─ StartupDiagnostics.cs        NEW — what Startup.cs decided, for the Environment & diagnostics panel
├─ SupportDesk.Services/
│  ├─ TicketEditModel.cs            + RowVersion (hidden state)
│  ├─ TicketCommandService.cs       + the OriginalValue round trip, the DbUpdateConcurrencyException catch that builds the ConflictSet, SimulateAnotherOperatorChangeAsync, CloseTicketWithCommentAsync
│  └─ ConflictResolution.cs         NEW — UserRole, ConflictField, ConflictSet, BuildConflictListAsync, CanOverwrite
├─ SupportDesk.Tests/               xunit, SQLite in memory — 82 tests
│  ├─ ConcurrencyAndTransactionsTests.cs  NEW — the Module 7 deliverable: 10 tests (RowVersion round trip, conflict list, Reload, Overwrite, transaction commit/rollback, the verbatim trace)
│  ├─ MigrationScriptTests.cs             NEW — 4 tests against the committed script's actual content
│  └─ (everything from Modules 1–5 — unchanged, still green)
└─ docs/                            the five Module 7 deliverables + the capstone note, architecture note,
                                      AI grounding note and checklists + the twenty notes carried from Modules 1–5
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the Module 1–6 solution, confirm the browser and editor still work | `SupportDesk.slnx`; unchanged until the edits below (built from Module 5, per this build's instructions — see the Capstone section) |
| 2 · `byte[] RowVersion` on `TicketEditModel`, filled in the load path, kept out of the validator | `SupportDesk.Services/TicketEditModel.cs`, `TicketCommandService.ToModel` — see [`docs/ConcurrencyResolution.md`](docs/ConcurrencyResolution.md) |
| 3 · `SaveAsync` restores it as `OriginalValue` before mapping the other fields | `TicketCommandService.SaveAsync` — see [`docs/ConcurrencyResolution.md`](docs/ConcurrencyResolution.md) |
| 4 · Reproduce the conflict: two browser sessions, confirm `DbUpdateConcurrencyException` | the two-session walkthrough above; `buttonSimulateChange_Click` for the one-session alternative; `ConcurrencyAndTransactionsTests.SaveAsync_with_a_stale_RowVersion_throws_DbUpdateConcurrencyException` |
| 5 · Catch the exception in `SaveAsync`, build `ConflictField`s from `ex.Entries`/`GetDatabaseValuesAsync`, report deleted rows | `ConflictResolution.BuildConflictListAsync`; `TicketCommandService.SaveAsync`'s catch (see its remarks for why the building has to happen there, not in the editor) — see [`docs/ConcurrencyResolution.md`](docs/ConcurrencyResolution.md) |
| 6 · `ConflictDialog` form: `DataGridView` of field/your/database/original, Reload and Overwrite (policy-gated), refresh the grid after | `SupportDesk.Web/ConflictDialog(.Designer).cs`, `TicketEditorForm.ShowConflictDialogAsync`/`OverwriteAsync` — see [`docs/ConcurrencyResolution.md`](docs/ConcurrencyResolution.md) |
| 7 · `dotnet ef migrations script --idempotent` into `artifacts/sql`, deployment notes with environment-specific connection strings | attempted verbatim, throws on SQLite; run without `--idempotent` instead — see [`docs/DeploymentNotes.md`](docs/DeploymentNotes.md) |
| 8 · `EnableSensitiveDataLogging` behind the Development check, production logging categories | `SupportDeskDataServiceCollectionExtensions.AddSupportDeskData` (unchanged gate), `appsettings.Production.json`, `Startup.cs`'s reporting — see [`docs/DeploymentNotes.md`](docs/DeploymentNotes.md) |
| 9 · Show every path: normal save, stale save → conflict dialog, Reload, Overwrite hidden for Agent, deleted ticket reported clearly | the bottom bar row 1 — see **What to click** above |
| 10 · Review & run: concurrency/deployment/AI-grounding checklists, capstone rubric, the two-session test recorded | [`docs/Checklists.md`](docs/Checklists.md), [`docs/CapstoneReview.md`](docs/CapstoneReview.md); the two-session walkthrough above is **unverified** here — see Verified/unverified |

Lab code check (`labs.js` m7, inferred from `ef7s2.html`'s required deliverables and acceptance criteria):
`TicketEditModel.RowVersion`, the `OriginalValue` line in `SaveAsync`, a caught `DbUpdateConcurrencyException`,
a conflict dialog with Reload/Overwrite, the migration script under `artifacts/sql`, and
`EnableSensitiveDataLogging` gated by environment — all present and exercised by
`ConcurrencyAndTransactionsTests.cs`/`MigrationScriptTests.cs`.

## Self-check answers (the lab's student review questions)

- **If you left out the `OriginalValue` line, what would the two-session test show, and why would it look
  like success while silently losing the first operator's change?**

  Without `db.Entry(ticket).Property(t => t.RowVersion).OriginalValue = model.RowVersion;`, the tracked
  entity's `OriginalValue` for `RowVersion` stays whatever `SaveAsync`'s own fresh read just fetched — which
  is, by definition, always the row's *current* token, because that read happened moments before
  `SaveChangesAsync` runs. EF Core's generated `UPDATE … WHERE "Id" = @id AND "RowVersion" = @original` would
  then compare the row against a value that is guaranteed to still match (nothing else has had a chance to
  change it between this read and this write), so the statement always affects exactly one row and
  `SaveChangesAsync` never throws — not because there was no conflict, but because the check that would have
  noticed one was never wired to anything meaningful. Both browser tabs in the two-session walkthrough above
  would show **"Save"** succeed every time, with no error, no dialog, nothing to notice. The two-session test
  would *look* like success on both sides: tab A saves, tab B saves a moment later, both get a green result.
  What actually happened is that tab B's `UPDATE` statement — mapping tab B's editor fields onto the row from
  scratch — silently overwrote every field tab A had just changed with whatever tab B's own (older, stale)
  edit model held for those same fields, because tab B's `SaveAsync` maps *every* field from its model, not
  just the ones the operator touched. Tab A's operator would find their change simply gone the next time they
  looked at the ticket, with no error message anywhere in either session to explain why — exactly the "last
  write wins, silently" failure mode optimistic concurrency exists to prevent, and exactly why
  `ConcurrencyAndTransactionsTests.SaveAsync_with_a_stale_RowVersion_throws_DbUpdateConcurrencyException`
  exists: it is the one test in this module that would go from green to red (no exception thrown at all) if
  that single `OriginalValue` line were ever deleted.

- **Which resolution paths would you offer an ordinary agent and which a supervisor, and what must happen to
  the grid and the `RowVersion` in the edit model after each path?**

  An ordinary **Agent** gets **Reload** and **Cancel** only — `ConflictResolution.CanOverwrite(UserRole.Agent)`
  returns `false`, and `ConflictDialog`'s constructor sets `btnOverwrite.Visible = false` accordingly (with
  `labelPolicy` explaining why, not just hiding the button silently). A **Supervisor** additionally gets
  **Overwrite**. This module's `cboRole` is a lab stand-in for a real role claim (there is no authentication
  behind it), but the policy check itself — one static method, `ConflictResolution.CanOverwrite` — is exactly
  where a real system would plug in an actual authorization check without touching the dialog or the command
  service at all.

  After **Reload**: `ShowConflictDialogAsync` re-runs `LoadEditorAsync()`, which re-fetches the lookups and
  calls `Commands.LoadEditModelAsync` again — the edit model's `RowVersion` becomes the database's **current**
  token (the one the conflict was built against), every other field becomes the database's current value, and
  `editBindingSource.DataSource` is reassigned to this new model so every bound control repaints. The
  **editor stays open** (this is a deliberate choice per this module's build instructions) — the grid behind
  it has not changed yet, and does not need to: nothing was written. The grid only refreshes when the editor
  itself eventually closes with `DialogResult.OK` (an ordinary Save after Reload, or a Cancel that simply
  closes without saving does not trigger a refresh either — only a successful write does, unchanged since
  Module 4).

  After **Overwrite**: `OverwriteAsync` calls `Commands.SaveAsync(model, latency, forceDuplicateNumber: false,
  conflicts.DatabaseRowVersion)` — the database's current token (not the stale one) becomes the retried
  save's `OriginalValue`, so this write is expected to succeed. `SupportDeskContext.SaveChanges`'s
  `StampTickets` then stamps yet another **new** `RowVersion` onto the row as part of that successful write
  (Overwrite still goes through the ordinary save path — it does not skip the concurrency-token stamping,
  it just wins the check). The **editor closes with `DialogResult.OK`** immediately after, and
  `TicketBrowserPage.OpenEditorAsync` sees that and re-searches, refreshing the grid with the overwritten
  values on top — the same "close with OK, caller refreshes" contract every other successful save in this
  course already follows.

- **Three production instances start after a deployment. In your design, which of them applies the
  migration and when, and what would go wrong if `Migrate()` ran in each instance's startup?**

  **None of them** — in this design, no application instance ever applies a migration. The reviewed,
  idempotent-as-SQLite-allows script under `artifacts/sql/supportdesk_migrations.sql` is applied **once, by
  the release pipeline, as its own step, before traffic is switched to the new application version** (see
  `docs/DeploymentNotes.md`'s "Release steps"). `Startup.cs`'s only call to
  `SupportDeskDevelopmentDatabase.EnsureReadyAsync` (which is the only call site of `MigrateAsync` in this
  entire solution) is guarded by `if (app.Environment.IsDevelopment())` — a production instance's
  `IsDevelopment()` is always `false`, so that branch never runs there at all; `StartupDiagnostics.
  MigrationsAppliedAtStartup` is explicitly set to `false` on that path, and a console line says so
  (`"migrations: NOT applied at startup outside Development"`), visible on the page's Environment &
  diagnostics panel too.

  If `Migrate()` *did* run from every instance's own startup instead: all three instances typically start
  within seconds of each other during a rolling deployment, and all three would race to run the same
  `ALTER`/`CREATE` statements against the same database at nearly the same moment. On SQLite specifically
  this usually means one instance's writer lock blocks the others outright (`SQLITE_BUSY`) — the losing
  instance(s) would crash at *startup*, before ever serving a single request, with an error that looks like a
  database problem rather than what it actually is (a race the deployment topology created). On a networked
  database the same race can instead produce a partially-applied schema if one instance's transaction commits
  some but not all of a multi-statement migration before a second instance's conflicting DDL interleaves with
  it, or simply three redundant, wasted attempts to do the same idempotent-on-SQL-Server-but-not-SQLite work.
  Either way, the failure happens at the worst possible moment — mid-deployment, unattended, with no human
  reviewing what actually ran — instead of the one controlled point in the process (the pipeline step, with a
  reviewed SQL diff already sitting in the pull request) where a human can catch a problem *before* it reaches
  a live database, and where a rollback plan (a new forward migration, or restoring from backup — see
  `docs/DeploymentNotes.md`'s "Rollback notes") is actually available.

## Verified / unverified

Built and tested on this machine (Wisej-4 4.1.0, .NET 10, EF Core 10.0.12, SQLite):

- `dotnet build SupportDesk.slnx -nologo -v q` — succeeds with **0 warnings, 0 errors**.
- `dotnet test SupportDesk.Tests -nologo -v q` — **82 passed**, 0 failed (68 carried over from Modules 1–5
  unchanged, 10 new in `ConcurrencyAndTransactionsTests.cs`, 4 new in `MigrationScriptTests.cs`).
- The `RowVersion` round trip really produces `DbUpdateConcurrencyException` **through
  `TicketCommandService`**, not raw EF Core alone —
  `SaveAsync_with_a_stale_RowVersion_throws_DbUpdateConcurrencyException` loads an edit model, changes the
  row through a second context (`SimulateAnotherOperatorChangeAsync`), then saves the stale model and asserts
  the exception.
- `ConflictResolution.BuildConflictListAsync`'s exact output was captured from a running test against SQLite
  in memory (`Prints_the_trace_for_one_reproduced_conflict`, the same code path `TicketCommandService.SaveAsync`
  and `TicketEditorForm` call), not written from memory:
  ```
  Command: UPDATE "Tickets" SET "Priority" = @p0, "RowVersion" = @p1, "Status" = @p2, "UpdatedAt" = @p3 WHERE "Id" = @p4 AND "RowVersion" = @p5 RETURNING 1; (0.1 ms)
  Note: SaveAsync: caught DbUpdateConcurrencyException — 0 rows matched the WHERE clause · 3 field(s) differ
  conflict Priority: yours 'Low' · database 'High' · original 'High'
  conflict RowVersion (concurrency token): yours '0x0DF6508CA8AF3A4A810F3E4E93221D78' · database '0xB028F781A942DC45B179D21F650BA89A' · original '0x234AC8FC3F77304FB56AA717719F48AD'
  conflict Status: yours 'Resolved' · database 'Closed' · original 'Closed'
  ```
- The deleted-row branch (`ConflictSet.DeletedByAnotherUser`) was captured against a genuine
  `DbUpdateConcurrencyException` whose `GetDatabaseValuesAsync` really returns `null` — a tracked read held
  open across a second context's delete, not a fabricated `ConflictSet`
  (`BuildConflictListAsync_reports_the_row_as_deleted_when_it_is_removed_between_the_tracked_read_and_SaveChangesAsync`).
- Reload (`LoadEditModelAsync_after_a_conflict_returns_the_current_database_values_…`) and Overwrite
  (`SaveAsync_overwrite_with_the_databases_RowVersion_wins_…`) were both proven against SQLite in memory: the
  first shows the database's values reachable through the same call the dialog's Reload uses, the second
  shows the operator's values landing with a **new** token, not the one Overwrite forced as `OriginalValue`.
- The transaction commits both writes together and rolls back correctly when the lab prop fires, captured
  from a running test's trace (`Prints_the_trace_for_the_transaction_rollback`), not written from memory:
  ```
  Note: CloseTicketWithCommentAsync(#1): BeginTransactionAsync — status update and comment insert share one transaction
  Command: UPDATE "Tickets" SET "RowVersion" = @p0, "Status" = @p1, "UpdatedAt" = @p2 WHERE "Id" = @p3 AND "RowVersion" = @p4 RETURNING 1;
  Note: CloseTicketWithCommentAsync(#1 / SD-1001): write 1 of 2 done — Status = Closed, SaveChangesAsync
  Note: CloseTicketWithCommentAsync: TransactionFailureSwitch armed — throwing before the comment is written
  Note: CloseTicketWithCommentAsync(#1): transaction rolled back — the comment was not written and the status is unchanged
  ```
  — note there is exactly one `UPDATE` and **no** `INSERT INTO "TicketComments"` at all; the second write was
  never attempted, and the re-read after the exception confirms the status reverted.
- `dotnet ef migrations script --idempotent --project SupportDesk.Data --startup-project SupportDesk.Web
  --framework net10.0` was run verbatim on this machine and reproduced
  `System.NotSupportedException: Generating idempotent scripts for migrations is not currently supported for
  SQLite.` exactly as reported. The same command **without** `--idempotent` succeeded; its output was diffed
  byte-for-byte identical against the (failed) idempotent attempt's partial behaviour, confirming SQLite emits
  its one available guard (`CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory"`) unconditionally. The
  committed `artifacts/sql/supportdesk_migrations.sql` is that output; `MigrationScriptTests.cs` asserts its
  real content directly (the history-table guard, the transaction wrapper, the history `INSERT`).
- `Startup.cs`'s `if (app.Environment.IsDevelopment())` guard was read directly to confirm
  `SupportDeskDevelopmentDatabase.EnsureReadyAsync` (and therefore `Migrate()`) has exactly one call site in
  this solution, and that it is gated.

Facts carried over from Modules 1–5 and still relied on here: `Application.Services.AddService<IServiceProvider>(app.Services)`
makes `[Inject]` resolve through Microsoft DI; application services must be Transient or Singleton for the
root-provider reason; a `BindingSource`'s `DataSource` must be a `Type` before any `DataBindings.Add`, and
lookup `SelectedValue` bindings must be added after the lookup `DataSource` and the model both exist
(`BindLookupControls`); `Application.Update(this)` in `finally` pushes the final state after an `await`;
Wisej.NET delivers a second click while an awaited handler is pending, which is what makes `_saving` load-
bearing; nested modals (a `MessageBox`/`Form` shown from inside an already-open `Form`'s handler) work, as
proven by the Module 4 delete-confirmation flow — `ConflictDialog` opened from inside `TicketEditorForm`
follows the identical pattern and is assumed, not separately reverified, to nest the same way.

**Deviations from the lab guide's exact wording, and why:**

1. **`dotnet ef migrations script` was run without `--idempotent`.** The flag throws
   `System.NotSupportedException` on the SQLite provider used throughout this course — see
   `docs/DeploymentNotes.md`'s "The `--idempotent` deviation" for the full reasoning and the byte-for-byte
   comparison that shows nothing was lost by omitting it.
2. **The conflict dialog offers Reload and Overwrite, not the lesson's third option, Merge.** The lesson
   prose (`ef7s1.html`) names Merge as a concept ("keeps some values from each side"); the lab guide's own
   required deliverables and instructor acceptance criteria (`ef7s2.html`) name only Reload and Overwrite as
   the dialog's paths, and this build's own task followed that narrower, authoritative list. `ConflictField`
   already carries every value (Your/Database/Original) a future Merge UI would need, without any change to
   `ConflictResolution` — see `docs/ConcurrencyResolution.md`.
3. **Module 7 was built from Module 5, not Module 6.** Module 6 was being built separately, in parallel, from
   the same Module 5 base, per this build's own instructions — the "measured optimisation" Module 6 adds was
   already the shape of `TicketQueryService.SearchTicketsAsync` inherited unchanged from Module 5 (see
   `docs/CapstoneReview.md`'s Performance section), so nothing Module 7 needed was lost by skipping Module 6
   as a base.
4. **Control names, the `RowVersion`/duplicate-number/validation deviations already documented in Module 4
   and Module 5's own READMEs are unchanged and still apply** — this module is a copy of Module 5 plus the
   Module 7 lab, and did not revisit those choices.

**Not verified here — for the browser reviewer.** The application was not started (by instruction), so
everything below is Wisej.NET behaviour that only a running page and running dialogs can confirm:

1. The two-session concurrency walkthrough above, end to end, in two real browser tabs/sessions — the
   one-session `buttonSimulateChange_Click` alternative and every server-side mechanism it exercises are
   covered by tests, but two independent Wisej.NET sessions genuinely colliding has not been driven through
   the Browser pane in this build.
2. `ConflictDialog` actually renders as a modal, `CenterParent`, over `TicketEditorForm` (itself already
   modal over `TicketBrowserPage`) — a dialog nested inside a dialog — and that its `DataGridView` paints the
   four columns and every row correctly.
3. `btnOverwrite.Visible` really toggles in the browser as `cboRole` changes between Edit-ticket clicks (the
   underlying `ConflictResolution.CanOverwrite` logic is unit-tested; the control's actual visibility in a
   live dialog is not).
4. The Environment & diagnostics panel's HTML table renders correctly (`labelEnvironment.AllowHtml = true`) —
   the underlying `StartupDiagnostics` values are read directly from `Startup.cs`'s console output shape, not
   from a rendered page.
5. The bottom bar's new row (`panelActionsM7`) lays out without overlap at 1980px width, and the page's grown
   height (1430, up from 1250) does not clip anything below the fold before `AutoScroll` kicks in.
6. `AlertBox.Show(..., alignment: TopRight, autoCloseDelay: 4000)` appears for every new Module 7 failure
   path (`DatabaseUnavailableException` during the transaction demos, a deleted-row Reload) the same way it
   does everywhere else in this course.
7. The exact pixel layout of `ConflictDialog`'s grid columns under `AutoSizeColumnsMode.Fill` at the dialog's
   fixed 600×420 size.

## Capstone

The pieces the capstone hand-in asks for, and where they are:

- **The Support Desk Data Console itself** — this module, run as above. It demonstrates the projected, paged
  browser (Module 3), lookup ComboBoxes (Module 4), the modal edit-model form with explicit validation
  (Module 4/5), async load/save/delete with loading guards (Module 1/4), `RowVersion` concurrency with the
  conflict dialog (Module 7), and this module's migration/configuration notes and grounding note.
- **Screens to capture** (for the reviewer/hand-in): the ticket grid, the editor with a validation error
  (any of the Module 5 lab buttons), and the concurrency conflict dialog (Simulate → Save, or the two-session
  walkthrough above).
- **Migration files and the generated script** — `SupportDesk.Data/Migrations/`,
  `artifacts/sql/supportdesk_migrations.sql`.
- **The architecture note** — [`docs/ArchitectureNote.md`](docs/ArchitectureNote.md).
- **The AI assistant grounding note** — [`docs/AiGroundingNote.md`](docs/AiGroundingNote.md).
- **The completed checklists** — [`docs/Checklists.md`](docs/Checklists.md).
- **The rubric walkthrough** — [`docs/CapstoneReview.md`](docs/CapstoneReview.md).

## Browser results (reviewer, 2026-09-10)

Run on this machine at <http://localhost:5407> in the Browser pane; trace read back from the page.

- **Found and fixed:** the walkthrough "Edit ticket → Simulate (page button) → Save" cannot happen in one session, because the
  modal editor blocks the page — the click on the page-level `buttonSimulateChange` was never delivered while the editor was
  open. The editor now carries its own lab button **Lab: other operator changes it** (`btnLabSimulateChange`, bottom-left, visible
  for existing tickets) that runs `TicketCommandService.SimulateAnotherOperatorChangeAsync` through a separate context:
  `• simulate another operator changed SD-1002 while this editor is open: Status = Closed, Priority = High, fresh RowVersion on the row — this editor still holds the token it loaded, so Save will conflict (2 statement(s), 1 context created, 1 disposed)`.
  The page-level button remains for the *Simulate → then Edit* variant and the two-tab walkthrough.

Paths verified (role **Agent**):

- **Edit ticket SD-1002** → **Lab: other operator changes it** → **Save**: `• editor SaveAsync: existing ticket SD-1002 (#2) — tracked read, fields mapped from the edit model · OriginalValue RowVersion = 0x80205E5D…`,
  `→ SQL UPDATE "Tickets" SET "RowVersion" = @p0, "Status" = @p1, "UpdatedAt" = @p2 WHERE "Id" = @p3 AND "RowVersion" = @p4 RETURNING 1;`,
  `• editor SaveAsync: caught DbUpdateConcurrencyException — 0 rows matched the WHERE clause · 2 field(s) differ`,
  `• conflict RowVersion (concurrency token): yours '0x4EA2…' · database '0x4B88…' · original '0x8020…'`, `• conflict Status: yours 'Resolved' · database 'Closed' · original 'Closed'`,
  `• conflict dialog opened · role Agent · 2 field(s) differ · Overwrite hidden by policy` — the **Conflict** dialog lists Field / Your value / Database value / Original value
  and shows *Role: Agent — Overwrite is hidden. Only a Supervisor may discard another operator's change.*
- **Reload** → `• dialog Conflict → Reload`, `• editor LoadEditorAsync(#2): 1 no-tracking read + lookups`, `• editor Reload complete — the editor now shows the database's values and the new RowVersion; still open`;
  the editor shows *Closed / High*. **Cancel** closes it without a write.
- **Close with comment — fail after first write** → `BeginTransactionAsync`, `write 1 of 2 done — Status = Closed`, `TransactionFailureSwitch armed — throwing before the comment is written`,
  `• service … transaction rolled back — the comment was not written and the status is unchanged`, `• caught SimulatedTransactionFailureException … → friendly message shown` (the toast *The ticket could not be closed. Please try again in a moment.*).
  (The row picked was already Closed from the earlier demo, so no UPDATE statement appears before the rollback — pick an open ticket to see it.)
- **Close with comment (transaction)** on SD-1007 → `→ SQL UPDATE "Tickets" SET "RowVersion" = @p0, "Status" = @p1, "UpdatedAt" = @p2 WHERE "Id" = @p3 AND "RowVersion" = @p4 RETURNING 1;`,
  `→ SQL INSERT INTO "TicketComments" (…) RETURNING "Id";`, `transaction committed — both writes are durable`, `← result SD-1007 closed with comment #100 — both writes committed together · 3 statement(s)`.

Paths verified (role **Supervisor**, picked in `cboRole`; the editor's trace line reads *opening TicketEditorForm (role Supervisor)*):

- **Edit ticket SD-1004** → **Lab: other operator changes it** → tick Urgent → **Save** → `• conflict dialog opened · role Supervisor · 4 field(s) differ · Overwrite offered`
  (IsUrgent, Priority, RowVersion, Status) and *Role: Supervisor — Overwrite is available.*
- **Overwrite** → `• dialog Conflict → Overwrite (Supervisor)`, `• editor SaveAsync: … OriginalValue RowVersion = 0x1C2E… (OVERWRITE: forced to the database's current token)`,
  `→ SQL UPDATE "Tickets" SET "IsUrgent" = @p0, "Priority" = @p1, "RowVersion" = @p2, "Status" = @p3, "UpdatedAt" = @p4 WHERE "Id" = @p5 AND "RowVersion" = @p6 RETURNING 1;`,
  `← result overwrite saved SD-1004 — the database's token was used as OriginalValue, your values won`, `• dialog Edit ticket SD-1004 → OK → grid refreshed`.
- The **Environment & diagnostics** panel renders: environment Development, migrations at startup *yes — Development only*, sensitive-data logging *ON (Development)*,
  the applied migration and the script artifact path. The Module 1–5 rows behave as in their own modules.

Not exercised this round: the two-browser-tab conflict (the in-editor lab button reproduces the same UPDATE/0-rows path), and the Break-the-database variant inside the editor.

