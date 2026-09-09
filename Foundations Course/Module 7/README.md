# WisejTrainingApp · Wisej.NET Foundations · Module 7

Local lab build for **Module 7 · Theming and UI modernization**. It is the ticket app of Modules 4 and 5
wrapped in a modern shell: a header with a working theme selector, a left navigation with an active-page
state, a Dashboard made of metric cards + grouped commands + a recent-activity log, and consistent
spacing on every page — while `TicketService`, the grid binding, the ticket dialog and its validation are
exactly what the earlier modules built. The sample is self-contained: it carries its own copy of the ticket
pieces and does not reference the other module folders.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 7/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5087
```

Then open <http://localhost:5087>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. The project multi-targets
`net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Open the app | start | Dashboard page; `btnDashboard` green with a border; header "Current theme: Bootstrap-4"; four metric cards (3 Open, 2 In progress, 1 Closed, 6 Customers); the Recent activity card already lists `Program.Main → …` and `NavigateTo("Dashboard") → …` |
| Pick **BootstrapDark-4** (or Blue-1, Classic-2, Material-3, FluentLight-5) in `cboTheme` | success | the whole app restyles live; `lblCurrentTheme` updates; status green "theme … applied — tickets, binding and validation untouched"; activity line `Theme changed to … (Application.LoadTheme) — lblCurrentTheme updated; TicketService not involved`; the numbers and the grid are unchanged |
| **Try an unknown theme** | failure | status red "● Theme 'Foo-9' not found — kept …"; the look does not change; activity line `Theme 'Foo-9' not found (…) — kept …; pick a real theme in cboTheme to recover` |
| Pick any real theme afterwards | recovery | the theme loads, label and status update, activity line `Theme changed to …` |
| Click **Tickets**, **Customers**, **Reports**, **Settings**, **Dashboard** | navigation | the clicked button turns green with a border, the others go flat; the page title matches; one activity line per page: `NavigateTo("…") → …View shown, btn… active` |
| Dashboard → **Create ticket** → fill Title + Customer → Save | success | Open card +1; activity line `Ticket #7 created — "…" (TicketService.AddTicket, metric cards refreshed)` |
| Dashboard → **Close ticket** → Yes | success | the oldest non-closed ticket becomes Closed; Open −1, Closed +1; activity line `Ticket #1 closed (TicketService.UpdateTicket, Status = Closed) …` |
| Dashboard → **Edit ticket** | navigation | goes to the Tickets page; status amber "select a ticket in the queue, then click Edit" |
| Tickets → select a row → **Edit** → change fields → Save | success | the row updates and stays selected; activity line `Ticket #n saved — …` |
| Tickets → **Edit** → Cancel | – | nothing changes: `Edit ticket #n → Cancel — the stored ticket is untouched (the dialog worked on a copy)` |
| Tickets → **Delete** → Yes / No | success / – | `await MessageBox.ShowAsync(… YesNo …)`; the row is removed on Yes, nothing happens on No — both logged |
| Tickets → **New** → Save with an empty title | failure | the dialog shows "Title is required." in red and stays open; status amber "validation: Title is required."; activity line `TicketDialog.ValidateForm() → false: Title is required. (current theme: …)` |
| Tickets → **Try a blank title (validation)** — best after a theme switch | failure → recovery | the dialog opens with the red message already showing (`ValidateForm()` was run first and logged with the current theme name); type a title and Save → `Recovery: title filled in → ValidateForm() = true → ticket #n created` |
| **Settings** | – | "UI settings" card shows the same current theme as the header and a note that theme settings stay out of business logic; the second card lists what the visual layer owns |
| Recent activity → **Clear** | – | empties the activity list |

The "Recent activity" card on the Dashboard is the course's event log for this module: every navigation,
theme change and ticket action is written there through one `AddActivity(string)` in `MainWindow`, with a
timestamp. The views are created once and swapped, so the list keeps growing while you are on other pages.

## Where things live

```
WisejTrainingApp/
├─ Program.cs                  Wisej.NET session entry point: new MainWindow().Show()
├─ MainWindow.cs               the shell: NavigateTo, SetActiveButton, cboTheme_SelectedIndexChanged, ApplyTheme,
│                              AddActivity, SetStatus, the "Try an unknown theme" failure path (IAppShell)
├─ MainWindow.Designer.cs      header (Dock Top), navigation (Dock Left), status (Dock Bottom), content (Dock Fill)
├─ IAppShell.cs                what a page may ask the shell for (Tickets, CurrentTheme, NavigateTo, AddActivity, SetStatus)
│                              + IAppView.RefreshView() + StatusKind
├─ Models/Ticket.cs            the shared ticket shape (Modules 4, 5, 7, 10) — unchanged
├─ Services/TicketService.cs   GetTickets / GetTicket / AddTicket / UpdateTicket / DeleteTicket / SaveTicket, 6 seeds — unchanged
├─ Views/
│  ├─ LayoutRules.cs           the spacing numbers every page follows (32 / 24 / 20 / 250 / 524 / 36)
│  ├─ DashboardView(.Designer).cs   metric cards, Commands card (btnCreateTicket/btnEditTicket/btnCloseTicket), Recent activity (lstActivity)
│  ├─ TicketsView(.Designer).cs     the Module 4/5 screen in a card: dgvTickets + BindingSource, btnNew/btnEdit/btnDelete, btnTryBlankTitle
│  ├─ CustomersView(.Designer).cs   tickets per customer
│  ├─ ReportsView(.Designer).cs     tickets by status | by priority
│  └─ SettingsView(.Designer).cs    UI settings card (current theme) | what the visual layer owns
├─ Dialogs/TicketDialog(.Designer).cs   ValidateForm, TicketResult, DialogResult.OK/Cancel, ValidationFailed event
├─ Startup.cs                  Kestrel host (app.UseWisej(), static files from the project folder)
├─ Default.json / Default.html / Web.config / Properties/launchSettings.json   (theme Bootstrap-4, port 5087)
└─ docs/
   ├─ ThemingNotes.md           theming vs business logic (this app's members), theme files/fonts/style options,
   │                            Theme Builder, the modern screen structure as built, evidence
   └─ ModernizationChecklist.md visual-hierarchy rules → where applied, "polish without rewriting logic" ticked,
                                the Module 7 checkpoint answered, evidence
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the project | `WisejTrainingApp.csproj`, `Program.cs` → `new MainWindow().Show()`; run with `dotnet run -f net10.0 --urls http://localhost:5087` |
| 2 · Apply a built-in theme | `Default.json` → `"theme": "Bootstrap-4"` (and `Web.config` → `Wisej.DefaultTheme`); at runtime `MainWindow.ApplyTheme()` → `Application.LoadTheme(name)` |
| 3 · Add a theme selector | `cboTheme` (DropDownList: Bootstrap-4, BootstrapDark-4, Blue-1, Classic-2, Material-3, FluentLight-5) and `lblCurrentTheme` in `MainWindow.Designer.cs`; `cboTheme_SelectedIndexChanged` in `MainWindow.cs` |
| 4 · Modern header & nav | `panelHeader` (`lblAppTitle` "ServiceDesk", `lblModule`), `panelNav` (`btnDashboard`, `btnTickets`, `btnCustomers`, `btnReports`, `btnSettings`) in `MainWindow.Designer.cs`; `NavigateTo(string)` in `MainWindow.cs` |
| 5 · Show active nav state | `MainWindow.SetActiveButton(Button)` — green background, white text, solid border, bold font; called from `NavigateTo` |
| 6 · Group into cards | `Views/DashboardView.Designer.cs` — `cardOpen … cardCustomers`, `cardCommands`, `cardActivity`; `Views/TicketsView.Designer.cs` — `cardQueue` |
| 7 · Consistent spacing | `Views/LayoutRules.cs` (the numbers) applied as literals in every `*View.Designer.cs`; `Dock` in the shell, `Anchor` on the cards, grid, buttons and header controls |
| 8 · Keep logic intact | `Services/TicketService.cs`, `Models/Ticket.cs`, the `ticketsBindingSource → dgvTickets` binding, `TicketDialog.ValidateForm()`, `btnNew/btnEdit/btnDelete_Click` — see the "NOT changed" comment block at the top of `Views/TicketsView.cs` |
| 9 · Add recent activity | `MainWindow.AddActivity(string)` → `DashboardView.AppendActivity` → `lstActivity`; every nav click, theme change and ticket action calls it |
| 10 · Run & test | the "What to try" table above; `docs/ModernizationChecklist.md` → Evidence |

Lab code check (`labs.js` m7): the handler is `cboTheme_SelectedIndexChanged`, it reads `cboTheme.Text`,
applies the theme (`Application.LoadTheme` — the lesson's `Application.Theme.Name = …` shorthand is noted
in the comment above `ApplyTheme`), updates `lblCurrentTheme.Text`, contains no `SaveTicket` / `ValidateForm`
/ `AddTicket`, and lives in `MainWindow.cs`, not the Designer file.

## Self-check (Module 7 checkpoint)

- **What does the theme change?** Only the look — colours, fonts, paddings, borders of every control — via
  `Application.LoadTheme(name)`. The ticket list, the metric numbers, the validation messages and the
  navigation are the same before and after; the activity log shows the theme line and the unchanged ticket
  lines side by side.
- **Which business logic must not change?** `Services/TicketService.cs`, `Models/Ticket.cs`,
  `TicketDialog.ValidateForm()` and its `DialogResult.OK` / `Cancel` flow, `MainWindow.NavigateTo`. None of
  them mentions `Application.Theme`, `cboTheme` or `lblCurrentTheme`.
- **How do you know which page you are on?** Exactly one nav button is green with a border and bold text —
  `SetActiveButton` is called inside `NavigateTo`, so the highlight and the page can never disagree — and the
  page title repeats the name.
- **Why do cards, labels and spacing help?** Cards show what belongs together (metrics, commands, activity,
  the queue); one title / description / card pattern means the user reads Reports the way they read the
  Dashboard; equal padding and gaps make five pages feel like one app; the large numbers answer the first
  question before anything else is read.

## Verified / unverified

`dotnet build -nologo -v q` (both targets, full rebuild): 0 warnings, 0 errors. Not run in the browser by the
author — the reviewer runs it. Calls the cookbook marks as verified on this framework build:
`Application.LoadTheme(name)`, `Panel.BorderStyle`, fonts `"default"` / `"monospace"`, `Label.TextAlign`.

Unverified at runtime (compiled only — please check when running):

- `await dialog.ShowDialogAsync()` returning `DialogResult.OK` after `this.DialogResult = DialogResult.OK; Close();`
  in the dialog, and `await MessageBox.ShowAsync(… YesNo …)` — the `async void` handlers in `TicketsView` and
  `DashboardView` push their final state with `Application.Update(this)`.
- `BindingSource.DataSource = List<Ticket>` → `dgvTickets` with `AutoGenerateColumns = false`, `FillWeight`
  under `AutoSizeColumnsMode.Fill`, `DefaultCellStyle.Format = "d"` on the date column, `ResetBindings(false)`,
  `bindingSource.Position = index` to reselect a row, `bindingSource.Current as Ticket`.
- `cboTheme.Text` on a `DropDownList` combo returning the selected item (the handler falls back to
  `SelectedItem.ToString()` if it is empty); setting `cboTheme.SelectedIndex = 0` in the Designer before the
  `SelectedIndexChanged` subscription so the initial selection does not re-apply the theme.
- `Application.Theme?.Name` after `LoadTheme` matching the requested name — used to detect a silently ignored
  unknown theme name. If `LoadTheme("Foo-9")` throws, the `catch` path handles it; if it neither throws nor
  changes `Theme.Name`, the name check handles it; if `Theme.Name` is empty the app assumes success.
- `Button.BorderStyle`, `Button.TextAlign`, `Control.ResetBackColor()` / `ResetForeColor()` for the active nav
  state; whether the theme's own button styling wins over `BackColor` on some themes (Material) — if the green
  does not show, the border and bold font still mark the active button.
- `Form.AcceptButton` / `CancelButton`, `StartPosition = CenterParent`, `ShowInTaskbar = false` on the dialog.
- `Panel.Controls.Remove(view)` + `Controls.Add(view)` to swap cached `UserControl` views without disposing them.
