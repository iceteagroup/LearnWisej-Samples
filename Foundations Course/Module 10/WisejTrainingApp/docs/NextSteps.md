# Next steps — what a production-ready version still needs

Lesson s47 §3: "The Next Steps screen should describe what a production-ready version would still need."
The **Next Steps** screen (`Views/NextStepsView.cs`) lists seven items in `lstNextSteps`; selecting one shows
*Today* (what exists), *Production* (what replaces it) and *Touches* (which files change). Because the layers
each have one job (see `docs/ArchitectureNotes.md`), most items replace one file and leave the rest alone.

| # | Item | Today | Production | Touches |
|---|---|---|---|---|
| 1 | **Real database** | `TicketRepository` keeps a `List<Ticket>` per session; data is gone when the session ends and two agents never see the same tickets. | An EF Core `DbContext` (SQL Server / PostgreSQL) behind the same repository methods (`GetAll`, `Get`, `Add`, `Update`, `Delete`); migrations for the Ticket and Customer tables; async queries. | `Services/TicketRepository.cs` is replaced; `TicketService` and every screen stay as they are. `CustomerService` gets the same split (service + repository). |
| 2 | **Authentication** | `lblUser` says "Support Agent" and every command is available to everyone. | A sign-in page or an identity provider (Entra ID, Auth0); `Application.Session` holds the user and roles; Delete and the Deployment screen are gated **server-side** by role (s42 §1), not only hidden — `TicketService.DeleteTicket` checks the caller's role before the repository call. | `Program.cs` (login before `Window1`), `Window1.cs` (`lblUser`, role checks in `BuildScreens`), `TicketService` (per-command authorization). |
| 3 | **Logging destination** | `lstActivity` and `lstJobLog` show what happened in this browser tab only. | Structured logging (`ILogger` / Serilog) to a file, Seq or Application Insights with timestamp, session id and user; the s42 §2 rules stay: workflow start/finish, validation failures, exceptions with detail, never passwords, license keys or personal data. | `Window1.AddActivity` and `JobsView.LogError` forward to `ILogger`; a logging section in `appsettings.json`; `Default.json` `"debug": false`. |
| 4 | **Tests** | `TicketValidator` and `TicketService` are testable (no controls inside) but untested. | An xUnit project: validator rules (blank title, 81-character title, unknown status, Closed without assignee), service behaviour (`AddTicket` assigns the next Id, `UpdateTicket` rejects an unknown Id, `DeleteTicket` throws when missing, `GetSummary` counts match), `CustomerService.ValidateCustomer` duplicate detection. | A new `WisejTrainingApp.Tests` project referencing `Services/` and `Models/` — no UI needed. |
| 5 | **CI / CD** | Built and run by hand with `dotnet run`. | A pipeline that restores, builds, runs the tests, publishes with `-c Release -f net10.0`, and deploys to a staging slot; the nine Deployment checks become pipeline steps and the license key comes from a pipeline secret. | A workflow file (GitHub Actions / Azure Pipelines); `Default.json` `"debug": false` in the release artifact. |
| 6 | **Concurrency & paging** | The grid loads every ticket; two agents editing the same ticket overwrite each other silently. | Server-side paging and filtering in the repository; a `RowVersion` on `Ticket` so `UpdateTicket` can detect a stale edit and tell the second agent instead of overwriting. | `Models/Ticket.cs` (`RowVersion`), `TicketRepository` (paged queries), `TicketsView` (page controls, stale-edit message). |
| 7 | **Audit trail** | Only the activity log knows who changed what, and only for one session. | A `TicketHistory` table written by `TicketService` on every Add / Update / Delete (who, when, old → new status), shown as a tab in `TicketDialog`. | `Services/TicketService.cs`, `Dialogs/TicketDialog.cs`, a new `Models/TicketHistory.cs`. |

Smaller items the screen does not list:

- **Escape to cancel** — `TicketDialog` sets `AcceptButton = btnSave` but no `CancelButton`; wire `btnCancel`
  so Escape closes the dialog like Module 5's dialog does.
- **Customer picker** — `txtCustomer` is free text; `CustomerService.GetCompanyNames()` already exists so the
  dialog could offer a `ComboBox` of known companies and the validator could reject unknown ones.
- **Localisation and dates** — `CreatedDate` is shown as `yyyy-MM-dd`; a real app takes the user's culture from
  `Application.CurrentCulture`.
- **Real background work** — the digest job in `JobsView` simulates with `Task.Delay`; a real one sends mail
  through an injected `IMailSender` so the job can be tested without a relay.

## Evidence

- **Next Steps screen**: `lstNextSteps` shows the seven titles; selecting one fills `lblStepTitle`,
  `lblStepToday` ("Today: …"), `lblStepProduction` ("Production: …") and `lblStepTouches` ("Touches: …");
  `lblStatus` reads `● next step n of 7`. The first item is selected automatically when the screen opens
  (`ActivateScreen`).
- The gaps are visible in the running app: open the app in two browser tabs and create a ticket in one —
  the other tab's grid does not change (no real database); the header always says "Signed in as: Support Agent"
  (no authentication); refresh the browser and the activity log is empty (no logging destination).
