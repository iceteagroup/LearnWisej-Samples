# Designer & code review checklist — applied to Module 1

| Check | Result | Evidence |
|---|---|---|
| The screen remains usable in the Visual Studio / Wisej.NET Designer | ✔ | `Views/TicketEditor.Designer.cs` holds the whole layout in `InitializeComponent()`; a parameterless constructor exists for the Designer; no logic in the generated file |
| Controls are named clearly enough for a teammate to follow the event code | ✔ | `gridTickets`, `textTitle`, `comboPriority`, `numericHours`, `buttonSave`, `buttonClose`, `buttonOutage`; handlers are `<control>_<event>` |
| Business logic is not trapped in visual event handlers | ✔ | validation in `TicketService.Validate`, the close rule in `Ticket.CanClose`; `TicketEditor.cs` contains no `if` about a ticket's state |
| No per-user state is held in static fields | ✔ | `grep -r "static" TicketOps/*.cs` finds only `SeedData.Tickets()`, `OperationResult.Ok/Fail`, `Strings` constants and `StatusBanner.ColorFor` — pure functions and constants; `AppComposition` is created per session in `Program.Main` |
| Failure paths are visible, logged, and explained without leaking internals | ✔ | `ShowResult` (expected) and `ReportFailure` (unexpected); the outage message with `sql01:1433` is in the trace only, the banner shows `Strings.ActionFailed` |
| The deliverable can be reviewed without running the whole course | ✔ | `README.md` + this `docs/` folder; the app runs standalone on port 5101 |

## Common mistakes checked

| Mistake | Present? |
|---|---|
| Reaching into the browser DOM to change a control | no — every change is a server property (`labelCount.Text`, `Rows.Add`) and Wisej.NET syncs |
| Per-user data in a static field | no (see above) |
| Heavy work synchronously in a handler | no — the 200-ticket load is paced by a `Timer`; Module 7 moves real work to `Application.StartTask` |
| Editing `.Designer.cs` by hand for logic | no — the generated file has layout only |
