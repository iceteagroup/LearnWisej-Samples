# TicketOps · Production Architecture Deep Dive · Module 1

Local lab build for **Module 1 · Production Wisej.NET Architecture & Project Structure**. It follows
the walkthrough video: the junior TicketOps app (SQL, rules and UI fused inside `btnSave_Click`)
becomes a production structure — `Views`, `Controls`, `Services`, `Domain`, `Data`, `Infrastructure`,
`Resources`, `Diagnostics` — with an `ITicketService` contract, a fake in-memory `TicketService`
behind it, thin event handlers that read the form / call the service / show the result, and every
path (success, validation, domain rule, data failure, recovery) visible to the user without leaking
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
`dotnet build -nologo -v q` passes with no warnings for both targets (`net10.0-windows`, `net10.0`).

## What to click in the Open Tickets window

The left card is the refactored screen; the right card is the **Activity trace · UI → Service → Data**:
every click is logged as it crosses a boundary (`[UI]` → `[SVC]` → `[DATA]` / `[DOMAIN]` → `[UI]`), so
you can see that the handler never touched a rule or a query.

| Button | Path | What you should see |
|---|---|---|
| **Save ticket** (edit the selected row or a new one) | success | `[UI] buttonSave_Click → ITicketService.SaveAsync {…}`, `[SVC] validate … valid → ITicketRepository.UpsertAsync`, `[DATA] #1041 written`, `[UI] OK · Ticket #1041 saved.`; the grid refreshes; status **● Ticket #1041 saved.** |
| **Close ticket** (select #1041, which has 1.5 h) | success + domain rule | `[DOMAIN] Ticket.Close — #1041 status → Closed`; the row leaves the open list |
| **New / ↻ Refresh** | — | clears the editor / reloads through `GetOpenTicketsAsync` |
| **▶ Load 200 tickets** | progress | a `Timer` saves 10 generated drafts per tick through the same `SaveAsync`; the progress bar and **● loading n/200** advance; the trace shows one `[SVC]`+`[DATA]` pair per save; ends with **205 open tickets** |
| **Save without a title** | failure 1 (validation) | `[SVC] ⚠ rejected: Title is required.` — no `[DATA]` line follows; orange banner **Title is required.**; status **● not saved** |
| **Close #1042 without hours** | failure 2 (domain rule) | `[DOMAIN] ⚠ Ticket.CanClose — #1042 rejected: Log hours before closing.`; the handler never knew the rule |
| **Simulate data outage** | error path | `[DATA] ✖ outage: SELECT * FROM Tickets failed — timeout connecting to sql01:1433 …` stays in the trace; the user sees only the red banner **The action could not be completed. Check the log for details.** and a toast; status **● failed** |
| **Recover the data store** (same button) | recovery | the repository answers again, the grid reloads, status **● ready** |
| **Clear trace** | — | empties the right-hand card |

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Production folder structure | `TicketOps/Views`, `Controls`, `Services`, `Domain`, `Data`, `Infrastructure`, `Resources`, `Diagnostics` — see [`docs/FolderStructure.md`](TicketOps/docs/FolderStructure.md) |
| 2 | Domain model with no UI dependency | `Domain/Ticket.cs` (record + `CanClose`/`Close` rule), `Domain/TicketDraft.cs`, `Domain/OperationResult.cs` |
| 3 | `ITicketService` contract | `Services/ITicketService.cs` |
| 4 | Fake `TicketService` implementation | `Services/TicketService.cs` over `Data/ITicketRepository.cs` + `Data/InMemoryTicketRepository.cs` |
| 5 | Thin handlers | `Views/TicketEditor.cs` (`buttonSave_Click`, `buttonClose_Click` — read the form, call the service, show the result) |
| 6 | Every path visible without leaking internals | `TicketEditor.ShowResult` / `ReportFailure`, `Resources/Strings.cs`, the trace panel — see [`docs/BeforeAfterRefactor.md`](TicketOps/docs/BeforeAfterRefactor.md) |
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
│  └─ InMemoryTicketRepository.cs fake store, seeded; SimulateOutage throws like a real driver would
├─ Infrastructure/
│  ├─ ILog.cs / ActivityLog.cs    cross-cutting logging (details stay here)
│  └─ AppComposition.cs           who gets what: one object graph per session, constructor injection, no statics
├─ Resources/Strings.cs           safe user-facing messages
├─ Diagnostics/ActivityTracePanel the live trace card
├─ docs/                          the deliverables
├─ Program.cs                     Wisej.NET session entry point → AppComposition
└─ Startup.cs                     Kestrel host (app.UseWisej())
```

## Self-check answers (lesson guide)

- **Can every event handler be explained in one sentence?**
  Yes: *"Save reads the form, asks `ITicketService` to save, shows the result."* `buttonClose_Click`
  is *"take the selected id, ask the service to close it, show the result."* The bottom-bar buttons
  are the same sentences with a fixed input. None of them mentions a rule, a query or a connection.
- **Can business logic run without a control?**
  Yes. `TicketService` and `Ticket` take no Wisej.NET type; `new TicketService(new InMemoryTicketRepository(log), log).SaveAsync(draft)`
  runs in a unit test, and `Ticket.CanClose` is a plain method. The trace proves it: the `[SVC]` and
  `[DOMAIN]` lines are produced by code that never reads `textTitle.Text`.
- **Can a new developer find the ticket model, service and editor quickly?**
  `Domain/Ticket.cs`, `Services/ITicketService.cs` + `TicketService.cs`, `Views/TicketEditor.cs` — the
  feature's pieces line up by name, and the folder tells the role. [`docs/ArchitectureNote.md`](TicketOps/docs/ArchitectureNote.md)
  says where the next piece goes.
