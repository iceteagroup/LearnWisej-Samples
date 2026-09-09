# Theming notes — theming vs business logic (Module 7)

The Module 7 lab continues from the ticket app of Modules 4 and 5. Everything that changed is in the
visual layer; everything that decides what the app *does* is the same code as before. This note says
which members are which, where theme files and style options fit, and what the modern screen looks like.

## 1. Theming vs business logic — this app's members

| Area | UI modernization (changed in Module 7) | Business logic (NOT changed) |
|---|---|---|
| Theme | `cboTheme`, `cboTheme_SelectedIndexChanged`, `ApplyTheme()` → `Application.LoadTheme(name)`, `lblCurrentTheme`, `btnTryUnknownTheme` — all in `MainWindow.cs` | `TicketService.AddTicket / UpdateTicket / DeleteTicket / SaveTicket`, `TicketDialog.ValidateForm()` |
| Spacing | `Views/LayoutRules.cs` (32 px page padding, 24 px card gap, 20 px card padding, 250 px metric cards, 524 px wide cards, 36 px buttons) applied in every `*View.Designer.cs` | `Models/Ticket.cs` (the shared shape), `TicketService.GetTickets()` |
| Grouping | `cardOpen … cardCustomers`, `cardCommands`, `cardActivity` (Dashboard), `cardQueue` (Tickets), `cardCustomers`, `cardByStatus / cardByPriority`, `cardUiSettings / cardShell` — white `Panel`s with `BorderStyle.Solid` | `NavigateTo(string)` — the one navigation method; the dialog's `DialogResult.OK` / `Cancel` flow |
| Labels | `lblPageTitle` (18 pt bold) and `lblPageDescription` on every page, card titles at 12 pt bold, captions in grey `(90,107,125)` | the metric numbers themselves — computed from `TicketService.GetTickets()` in `DashboardView.RefreshView()` |
| Commands | `btnCreateTicket / btnEditTicket / btnCloseTicket` grouped in the Commands card; `btnNew / btnEdit / btnDelete` under the ticket queue | what those handlers do: `AddTicket`, `UpdateTicket`, `DeleteTicket`, `await MessageBox.ShowAsync(... YesNo ...)`, `await dialog.ShowDialogAsync()` |
| Navigation state | `SetActiveButton(Button)` — green `(31,157,87)` background, white text, `BorderStyle.Solid`, bold font on the current page's button; the others reset | `views` dictionary + `NavigateTo` — which page is which |

The rule in one line: **the theme selector updates a label; the ticket dialog updates a ticket.** The
two never call each other. `TicketService` has no reference to `Application.Theme`, and `ApplyTheme()`
has no reference to `TicketService`.

## 2. Theme files, fonts, images and style options

- **Built-in themes.** Wisej.NET 4.1.0 embeds Blue-1/2/3, Bootstrap-4, BootstrapDark-4, Classic-2,
  Clear-1/2/3, FluentDark-5, FluentLight-5, Graphite-3, Material-3, Material-4, MaterialDark-4 and
  Vista-2. The app starts on Bootstrap-4 (`Default.json` → `"theme": "Bootstrap-4"`, mirrored in
  `Web.config` → `Wisej.DefaultTheme`) and offers six of them in `cboTheme`.
- **Switching at runtime.** `Application.LoadTheme("Material-3")` restyles the running app live; the
  browser re-renders every control with the new theme's colours, fonts and paddings. The lesson writes
  the shorthand `Application.Theme.Name = selectedTheme;` — `Application.Theme` is the current
  `ClientTheme` object, `LoadTheme(name)` is the call that loads an embedded theme by name.
- **Fonts.** The app uses two families only: `"default"` (the theme's font) in 9, 10, 12, 13, 16, 18 and
  26 pt, and `"monospace"` 8–9 pt for logs and code hints. No third family, no odd sizes.
- **Images and icons.** None — the course app needs no decoration; the four metric cards use colour
  (red / amber / green / theme default) to carry meaning instead.
- **Style options that improve clarity.** Active navigation state (`SetActiveButton`), one page-title
  pattern, one card pattern (white, solid border, 20 px padding, 12 pt bold title), one command-button
  size (36 px high), one status label (`lblStatus`, green / amber / red).
- **Theme Builder.** Not used. The built-in Bootstrap-4 is close enough for the lab; Theme Builder is
  the next step when a project needs its own primary colour, background or control spacing — start
  from a built-in theme and change a few values, do not hand-edit every screen.

## 3. The modern screen structure, as built

```
MainWindow  (Form, 1348 × 720, IAppShell)
├─ panelHeader   Dock Top, 64 px     lblAppTitle "ServiceDesk" · lblModule
│                                    btnTryUnknownTheme · lblCurrentTheme · cboTheme  (anchored right)
├─ lblStatus     Dock Bottom, 36 px  "● …" green / amber / red
├─ panelNav      Dock Left, 200 px   btnDashboard · btnTickets · btnCustomers · btnReports · btnSettings
│                                    (168 × 40, 48 px apart; SetActiveButton highlights one)
└─ panelContent  Dock Fill           one Views/*View (UserControl, Dock Fill) at a time — NavigateTo(page)
   ├─ DashboardView   title + description · 4 metric cards · Commands card | Recent activity card (lstActivity)
   ├─ TicketsView     title + description · Ticket queue card: dgvTickets + New / Edit / Delete + Try a blank title
   ├─ CustomersView   title + description · Customers card (tickets per customer)
   ├─ ReportsView     title + description · Tickets by status | Tickets by priority
   └─ SettingsView    title + description · UI settings card (current theme) | What the visual layer owns
```

The views are created once in the `MainWindow` constructor and swapped in and out of `panelContent`, so
the Dashboard's activity list keeps growing while the user is on other pages.

## Evidence

| Action | What the running app shows |
|---|---|
| Start | Dashboard page, `btnDashboard` green with a border, header "Current theme: Bootstrap-4", status green "● Dashboard page", activity list: `Program.Main → …`, `NavigateTo("Dashboard") → DashboardView shown, btnDashboard active` |
| Pick **BootstrapDark-4** in `cboTheme` | the whole app turns dark, `lblCurrentTheme` = "Current theme: BootstrapDark-4", status green "theme BootstrapDark-4 applied — tickets, binding and validation untouched", activity line `Theme changed to BootstrapDark-4 (Application.LoadTheme) — lblCurrentTheme updated; TicketService not involved`; the metric numbers and the ticket grid are the same as before |
| **Try an unknown theme** | status red "● Theme 'Foo-9' not found — kept BootstrapDark-4", the app keeps its look, activity line `Theme 'Foo-9' not found (…) — kept BootstrapDark-4; pick a real theme in cboTheme to recover` |
| Pick **Material-3** afterwards | recovery — the Material theme loads, the label and status update, the log shows `Theme changed to Material-3` |
| Settings page | "UI settings" card shows the same "Current theme: …" text as the header, plus the note that the theme never reaches the service |
| Tickets page after any theme switch → **New** → Save with a blank title | the dialog shows "Title is required." in red and stays open; status amber "validation: Title is required."; activity line `TicketDialog.ValidateForm() → false: Title is required. (current theme: Material-3)` |
