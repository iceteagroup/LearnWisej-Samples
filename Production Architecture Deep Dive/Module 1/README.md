# TicketOps · Production Architecture Deep Dive · Module 1

Local lab build for **Module 1 · Production Wisej.NET Architecture & Project Structure**. It follows
the walkthrough video: the junior TicketOps app (SQL, rules and UI fused inside `btnSave_Click`)
becomes a production structure — `Views`, `Controls`, `Services`, `Domain`, `Data`, `Infrastructure`,
`Resources`, `Diagnostics` — with an `ITicketService` contract, a fake in-memory `TicketService`
behind it, thin event handlers that read the form / call the service / show the result, and every
path (success, validation, domain rule, unexpected failure) visible to the user without leaking
internals.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 1\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5101
```

Then open <http://localhost:5101>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try in the Open Tickets window

| Action | Path | What you should see |
|---|---|---|
| **↻ Refresh** | success | the grid reloads; status **● Ticket list refreshed.** |
| Edit the selected row (or **New**), **Save ticket** | success | status **● Ticket #1041 saved.**; the grid refreshes |
| Clear the title, **Save ticket** | validation | orange banner **Title is required.**; status **● not saved**; nothing is persisted |
| Select #1042 (no hours logged), **Close ticket** | domain rule | orange banner **Log hours before closing.** — the rule lives in `Ticket.CanClose`, not in the handler |
| Select #1041 (1.5 h), **Close ticket** | success | the row leaves the open list |

An unexpected exception in any handler is caught there: its details go to `ILog` (written to the
server console by `Diagnostics/ActivityLog`), and the user sees only the red banner and toast
**The action could not be completed. Check the log for details.**

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Production folder structure | `TicketOps/Views`, `Controls`, `Services`, `Domain`, `Data`, `Infrastructure`, `Resources`, `Diagnostics` — see [`docs/FolderStructure.md`](TicketOps/docs/FolderStructure.md) |
| 2 | Domain model with no UI dependency | `Domain/Ticket.cs` (record + `CanClose`/`Close` rule), `Domain/TicketDraft.cs`, `Domain/OperationResult.cs` |
| 3 | `ITicketService` contract | `Services/ITicketService.cs` |
| 4 | Fake `TicketService` implementation | `Services/TicketService.cs` over `Data/ITicketRepository.cs` + `Data/InMemoryTicketRepository.cs` |
| 5 | Thin handlers | `Views/TicketEditor.cs` (`buttonSave_Click`, `buttonClose_Click` — read the form, call the service, show the result) |
| 6 | Every path visible without leaking internals | `TicketEditor.ShowResult` / `ReportFailure`, `Resources/Strings.cs` — see [`docs/BeforeAfterRefactor.md`](TicketOps/docs/BeforeAfterRefactor.md) |
| 7 | Architecture note | [`docs/ArchitectureNote.md`](TicketOps/docs/ArchitectureNote.md) — where new code goes |
| — | Designer & code review checklist, applied | [`docs/ReviewChecklist.md`](TicketOps/docs/ReviewChecklist.md) |

## Where things live

```
TicketOps/
├─ Views/
│  ├─ TicketEditor.cs             the screen: thin handlers, ReadDraftFromForm / ShowResult / ReportFailure
│  └─ TicketEditor.Designer.cs    GENERATED-style layout (opens in the Wisej Designer) — no logic here
├─ Controls/StatusBanner          reusable "● state" + banner UserControl (display only)
├─ Services/
│  ├─ ITicketService.cs           the contract the screen calls
│  └─ TicketService.cs            validation + close rule + persistence orchestration (no UI types)
├─ Domain/
│  ├─ Ticket.cs                   record + its own rule (CanClose / Close); compiles without Wisej.NET
│  ├─ TicketDraft.cs              what the form collects (UI → data)
│  └─ OperationResult.cs          success / safe explanation handed back to the screen
├─ Data/
│  ├─ ITicketRepository.cs        persistence contract
│  └─ InMemoryTicketRepository.cs fake store, seeded, one per session
├─ Infrastructure/
│  ├─ ILog.cs                     cross-cutting logging contract
│  └─ AppComposition.cs           who gets what: one object graph per session, constructor injection, no statics
├─ Resources/Strings.cs           safe user-facing messages
├─ Diagnostics/ActivityLog.cs     the ILog implementation (error details stay here)
├─ docs/                          the deliverables
├─ Program.cs                     Wisej.NET session entry point → AppComposition
└─ Startup.cs                     Kestrel host (app.UseWisej())
```

## Self-check answers (lesson guide)

- **Can every event handler be explained in one sentence?**
  Yes: *"Save reads the form, asks `ITicketService` to save, shows the result."* `buttonClose_Click`
  is *"take the selected id, ask the service to close it, show the result."* None of them mentions a
  rule, a query or a connection.
- **Can business logic run without a control?**
  Yes. `TicketService` and `Ticket` take no Wisej.NET type; `new TicketService(new InMemoryTicketRepository(log), log).SaveAsync(draft)`
  runs in a unit test, and `Ticket.CanClose` is a plain method.
- **Can a new developer find the ticket model, service and editor quickly?**
  `Domain/Ticket.cs`, `Services/ITicketService.cs` + `TicketService.cs`, `Views/TicketEditor.cs` — the
  feature's pieces line up by name, and the folder tells the role. [`docs/ArchitectureNote.md`](TicketOps/docs/ArchitectureNote.md)
  says where the next piece goes.
