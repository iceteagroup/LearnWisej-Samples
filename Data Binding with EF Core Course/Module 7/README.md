# SupportDesk · Data Binding with EF Core · Module 7

Local lab build for **Module 7 · Concurrency, Deployment, Diagnostics and the Capstone**. `TicketEditModel`
carries the `RowVersion` it was loaded with as hidden state; `TicketCommandService.SaveAsync` restores it as
`OriginalValue`, so a stale save matches zero rows and throws `DbUpdateConcurrencyException`. The conflict
list is built with `GetDatabaseValuesAsync` and shown in `ConflictDialog` (field, your value, database value,
original) with Reload, and Overwrite for a Supervisor only. Deployment: the migration script in
`artifacts/sql`, environment-specific configuration, sensitive-data logging only in Development.

Built from Module 5 (the Module 6 performance work is not part of this capstone solution).

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 7/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5407
```

The console prints the environment and whether sensitive-data logging is on; outside Development it says
migrations are not applied at startup. Tests: `dotnet test SupportDesk.Tests`.

## What to try

The browser adds a **Role** selector (`cboRole`: Agent or Supervisor). There is no sign-in in this sample,
so it stands in for the user's role.

- **Normal save**: edit a ticket and Save; the editor closes and the grid refreshes.
- **Conflict**: open the same ticket in two browser tabs. Change the status in tab 1 and Save. Change the
  priority in tab 2 and Save: the conflict dialog lists the differing fields.
- **Reload**: the editor takes the database values and the new `RowVersion`, and stays open.
- **Overwrite**: hidden for an Agent; with Role = Supervisor it saves your values with the database's current
  token as `OriginalValue`, and the grid refreshes.
- **Deleted meanwhile**: delete the ticket in one tab, Save it in the other: reported clearly, the editor
  closes and the grid refreshes.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | `RowVersion` in the edit model, set as `OriginalValue` before saving | [`docs/ConcurrencyResolution.md`](docs/ConcurrencyResolution.md) · `TicketEditModel.RowVersion`, `TicketCommandService.SaveAsync` |
| 2 | Reproducible conflict caught as `DbUpdateConcurrencyException` | [`docs/ConcurrencyResolution.md`](docs/ConcurrencyResolution.md) · two browser sessions; `ConcurrencyAndTransactionsTests` |
| 3 | Conflict dialog with Reload and Overwrite | `ConflictDialog(.Designer).cs`, `ConflictResolution.BuildConflictListAsync` / `CanOverwrite`, `TicketEditorForm.ShowConflictDialogAsync` / `OverwriteAsync` |
| 4 | Migration script, deployment notes, environment-specific connection strings | [`docs/DeploymentNotes.md`](docs/DeploymentNotes.md) · `artifacts/sql/supportdesk_migrations.sql`, `appsettings.Production.json`, `MigrationScriptTests.cs` |
| 5 | Sensitive-data logging off outside development; capstone review | `AddSupportDeskData(connectionString, isDevelopment)`; [`docs/CapstoneReview.md`](docs/CapstoneReview.md), [`docs/Checklists.md`](docs/Checklists.md) |

Transactions and retries: [`docs/TransactionsAndRetries.md`](docs/TransactionsAndRetries.md).

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 3 · `byte[] RowVersion` on `TicketEditModel`, filled on load, bound to no control | `TicketEditModel.cs`, `TicketCommandService.ToModel` |
| 4 · `OriginalValue = model.RowVersion` before mapping | `TicketCommandService.SaveAsync` |
| 5 · Reproduce the conflict in two sessions | see **What to try** |
| 6 · Catch the exception and build the `ConflictField` list | `TicketCommandService.SaveAsync` catch → `ConflictResolution.BuildConflictListAsync` (built while the context is still open) |
| 7 · `ConflictDialog` with Reload and policy-gated Overwrite; grid refreshed afterwards | `ConflictDialog`, `TicketEditorForm.ShowConflictDialogAsync`, `TicketBrowserPage.OpenEditorAsync` |
| 8 · Migration script, deployment notes, sensitive-data logging gate, production logging | `artifacts/sql/`, [`docs/DeploymentNotes.md`](docs/DeploymentNotes.md), `appsettings.Production.json` |
| 10 · Checklists and capstone review | [`docs/Checklists.md`](docs/Checklists.md), [`docs/CapstoneReview.md`](docs/CapstoneReview.md), [`docs/AiGroundingNote.md`](docs/AiGroundingNote.md) |

The walkthrough's conflict dialog also shows a *Merge…* button; the lab asks only for Reload and Overwrite,
so this sample has those two plus Cancel.

## Self-check answers

- **If you left out the `OriginalValue` line, what would the two-session test show?**
  Both saves succeed. `SaveAsync`'s fresh read always sees the current token, so the `WHERE` always matches
  and the second save silently overwrites the first operator's change.
  `SaveAsync_with_a_stale_RowVersion_throws_DbUpdateConcurrencyException` would go red.
- **Which resolution paths does an Agent get, and a Supervisor, and what happens to the grid and `RowVersion` after each?**
  Agent: Reload and Cancel. Supervisor: also Overwrite (`ConflictResolution.CanOverwrite`). Reload loads the
  database values and the current token into the editor, which stays open; nothing was written, so the grid
  refreshes when the editor later closes with OK. Overwrite saves with the database's token as
  `OriginalValue`, a new token is stamped, the editor closes with OK and the grid re-runs its search.
- **Three production instances start after a deployment. Which one applies the migration?**
  None. The reviewed script is applied once by the release pipeline before traffic switches; `MigrateAsync`
  runs only inside the Development branch of `Startup.cs`. Migrating from every instance's startup would race
  the same DDL at the same moment and fail (or half-apply) unattended, mid-deployment.
