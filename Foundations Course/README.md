# Wisej.NET Foundations · lab samples

One runnable Wisej.NET 4 application per module of the **Wisej.NET Foundations** course, built from each
module's lesson guide, lab guide and walkthrough video. Every sample follows the same layout: the screen the
lab asks for, built with the control names the lab guide uses, plus an **event log** card that shows what
the server-side C# did for every click, a status label, and buttons that exercise the success path, a
progress path where the module has one, at least one failure path and the recovery. Each module folder has
its own `README.md` (what to click, lab steps → code map, self-check answers) and a `docs/` folder with the
lab deliverables.

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package. Nothing is deployed anywhere.

| Module | Folder | What it builds | Run |
|---|---|---|---|
| 1 · Getting started | `Module 1` | `Window1`: prompt label, `txtName`, `btnGreet`, `lblResult` — the design → name → handle event → run cycle, plus "Inspect files" | `dotnet run -f net10.0 --urls http://localhost:5081` |
| 2 · Designer, properties & events | `Module 2` | `DashboardWindow`: title/status labels, three service indicators, Start / Stop / Reset / Refresh, `lstEventLog` + `AddLog` | `http://localhost:5082` |
| 3 · Application shell & navigation | `Module 3` | `MainPage : Page` shell: docked header / nav / content / status, four `UserControl` views, one `NavigateTo`, view-only Settings for a Support Agent | `http://localhost:5083` |
| 4 · Data binding & layout | `Module 4` | `TicketsWindow`: `Ticket` + `TicketService`, `SplitContainer`, `dgvTickets` bound through a `BindingSource`, detail controls, `btnSaveTicket` | `http://localhost:5084` |
| 5 · Dialogs & validation | `Module 5` | the ticket screen + one `TicketDialog` for New and Edit, `ValidateForm`, `DialogResult.OK`, delete confirmation, refresh only after a successful save | `http://localhost:5085` |
| 6 · State, background work & errors | `Module 6` | `JobsWindow`: Start Import / Export, Cancel, progress bar, job log, `async`/`await` + `try/catch/finally`, safe messages, session vs static state | `http://localhost:5086` |
| 7 · Theming & UI modernization | `Module 7` | `MainWindow`: theme selector (`Application.LoadTheme`), active nav state, metric cards, grouped commands, recent activity — ticket logic untouched | `http://localhost:5087` |
| 8 · JavaScript widget | `Module 8` | `StatusPage`: `widStatus` (`Wisej.Web.Widget`) fed by a C# `StatusService`, `Widgets/statusGauge.js` + `.css`, data card, Refresh, `gaugeClick` event | `http://localhost:5088` |
| 9 · Configuration, security & deployment | `Module 9` | `ReleaseReviewWindow`: required/optional release checklist, environment & target, role-gated review backed by a server check, secrets from secure config, troubleshooting log | `http://localhost:5089` |
| 10 · Capstone | `Module 10` | `Window1` mini helpdesk: nav shell, ticket CRUD with `TicketDialog` + `TicketValidator` + `TicketService`, dashboard cards, jobs, architecture / code-review / deployment / next-steps screens | `http://localhost:5090` |

Run any module from its `WisejTrainingApp` project folder. The projects multi-target `net10.0-windows` and
`net10.0`, so `dotnet run` needs a framework (`-f net10.0`, or `-f net10.0-windows`), e.g.

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 4/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5084
```

or open the `WisejTrainingApp.slnx` in the module folder with Visual Studio and press F5.

## `_template`

The scaffold every module was built from, plus `COOKBOOK.md`: the conventions and the Wisej.NET facts verified
while building and running these samples (dialogs are awaited with `ShowDialogAsync`, data binding through
`BindingSource`, async handlers push progress with `Application.Update`, `Application.LoadTheme`, the Widget
members `Packages` / `InitScript` / `Options`, and the gotchas found along the way). Read it before writing a
new sample.

## The project name

The course's "Getting started" lesson suggests `WisejTrainingApp` as the practice project name, so every module
uses it as project and namespace. Ticket-based modules (4, 5, 7, 10) share one `Ticket` model shape and one
`TicketService` API so the samples line up with each other and with the lesson code.
