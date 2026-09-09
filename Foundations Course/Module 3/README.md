# WisejTrainingApp · Wisej.NET Foundations · Module 3

Local lab build for **Module 3 · Application shell and navigation / Reusable navigation and permissions**.
It is the lab's ServiceDesk shell: a `Page` with a header (title, "Signed in as", breadcrumb, role picker),
a left navigation panel with four buttons, one content panel that swaps between four `UserControl` views
(`DashboardView`, `TicketsView`, `CustomersView`, `SettingsView`) through one shared `NavigateTo()`, a
status bar that logs every navigation — and the first permission: Settings is view-only for a Support Agent,
enforced by the disabled button *and* by a server-side check you can watch refuse the action.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 3/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5083
```

Then open <http://localhost:5083>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. The project multi-targets
`net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Load the app | shell | header *ServiceDesk Application · Home / Dashboard · Signed in as: Support Agent*, four nav buttons with **Dashboard** highlighted, the Dashboard cards, status `hh:mm:ss PM - Opened Dashboard page.` |
| Click **Tickets**, **Customers**, **Settings**, **Dashboard** | success (navigation) | only the content area changes; breadcrumb becomes `Home / Tickets` etc., the clicked button turns blue, the status line says `Opened … page.`; back on Dashboard the *Recent activity* card lists every line |
| Resize the browser | layout | header stretches, nav stays 220 px, content and the cards inside it grow, status bar stays at the bottom (Dock + Anchor) |
| Dashboard → **Run Health Check** | success / warning | *System Status* shows *All systems operational* (green); every third run *Degraded: email gateway slow* (amber) and an amber status line |
| Tickets → **New Ticket** | success | a row `#7 New ticket #7 · Tailspin · Open` is added and selected, summary `7 tickets · 6 open`, status `Ticket #7 created for Tailspin.`; go to Dashboard and back — it is still there (the service is per session, the view is not) |
| Tickets → **Close Selected** with nothing selected | failure | toast *Select a ticket first.* and an amber status `Close Selected: no ticket selected.` |
| Tickets → select a row → **Close Selected** | success | the row's Status becomes *Closed*, the Open count drops, status `Ticket #1 closed (Northwind).`; closing it again logs `Ticket #1 is already closed.` |
| Customers → **View Selected** | success / failure | with a row selected: a toast with name, contact, city and open tickets; with none: *Select a customer first.* and an amber status |
| Customers → **Add Customer** as Support Agent | permission | button disabled, note *Customers are view-only for this role (Support Agent).* |
| Settings as Support Agent | **failure path** | `Save Settings` disabled, note *Settings are view-only for this role.*, the role card shows the matrix row `Settings  View only` |
| Settings → **Try to save anyway (server check)** as Support Agent | failure (server) | toast + red status `Permission denied: Support Agent cannot save settings. (via Try to save anyway)` — the server refused, not the button |
| Header `cboRole` → **Manager** | recovery | status `Role changed to Manager - permissions re-applied.`, header *Signed in as: Manager*, the open page is rebuilt: `Save Settings` enabled, note *Manager can save settings.*; `Add Customer` enabled on Customers |
| Settings → **Save Settings** as Manager | success | green status `Settings saved by Manager. (company = "…", default priority = Medium, email notifications = on)` and a toast |
| Back to **Support Agent** | permission again | Save disabled again; the activity list on the Dashboard shows the whole story in order |

## Where things live

```
WisejTrainingApp/
├─ Program.cs                    Application.MainPage = new MainPage()   (a Page, not a Window)
├─ MainPage.cs                   the shell: NavigateTo, SetActiveButton, ApplyPermissions, Log (IShellHost), cboRole handler
├─ MainPage.Designer.cs          pnlHeader (Top) · pnlNav (Left) · pnlContent (Fill) · lblStatus (Bottom); nav buttons anchored
├─ Views/
│  ├─ DashboardView(.Designer).cs   cards Open Tickets / Customers / System Status, btnRunHealthCheck, lstEventLog "Recent activity"
│  ├─ TicketsView(.Designer).cs     dgvTickets + btnNewTicket / btnCloseSelected
│  ├─ CustomersView(.Designer).cs   lstCustomers + btnAddCustomer (Manager only) / btnViewSelected
│  └─ SettingsView(.Designer).cs    settings controls, btnSaveSettings (disabled for Support Agent), btnTryServerSave, lblPermissionNote, role card
├─ Services/
│  ├─ PermissionService.cs       the role matrix: CanSaveSettings, CanEditCustomers, CanManageTickets, DescribeRole
│  ├─ IShellHost.cs              what a view may ask of the shell: CurrentRole, Log(), Activity
│  ├─ TicketService.cs           the shared ticket store (6 seeded tickets, per session)
│  ├─ CustomerService.cs         the customer list (6 seeded customers, per session)
│  ├─ HealthCheckService.cs      the simulated health check behind the Dashboard quick action
│  └─ LogKind.cs                 Info / Warn / Error → green / amber / red status
├─ Models/                       Ticket (the course-wide shape), Customer, HealthReport
├─ Startup.cs · Default.json · Default.html · Web.config · Properties/launchSettings.json (port 5083)
└─ docs/
   ├─ ShellLayout.md             the four regions, Dock/Anchor table, Page vs Window, NavigateTo, evidence
   └─ PermissionsMatrix.md       role × page matrix, how SettingsView enforces it, why the server check matters, evidence
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the project | `WisejTrainingApp.csproj` (Wisej-4 4.1.0, `net10.0-windows;net10.0`), `dotnet run -f net10.0 --urls http://localhost:5083` |
| 2 · Build the shell (header Top, nav Left, content Fill, status Bottom) | `MainPage.Designer.cs` — `pnlHeader.Dock = Top`, `pnlNav.Dock = Left`, `pnlContent.Dock = Fill`, `lblStatus.Dock = Bottom` (and the `Controls.Add` order comment) |
| 3 · Header content (app name, current user, breadcrumb) | `lblAppTitle`, `lblUser`, `lblBreadcrumb` in `MainPage.Designer.cs`; `lblUser` is refreshed by `ApplyPermissions()` |
| 4 · Navigation buttons | `btnDashboard`, `btnTickets`, `btnCustomers`, `btnSettings` — `Anchor = Top + Left + Right` inside `pnlNav` |
| 5 · Reusable views as UserControls | `Views/DashboardView.cs`, `TicketsView.cs`, `CustomersView.cs`, `SettingsView.cs` (each with its `.Designer.cs`) |
| 6 · One navigation method | `MainPage.NavigateTo(string pageName)`; the four `btn…_Click` handlers each contain one line |
| 7 · Status & breadcrumb on each navigation | end of `NavigateTo`: `lblBreadcrumb.Text = "Home / " + pageName`, `Log("Opened " + pageName + " page.")` → `lblStatus` with `hh:mm:ss tt` |
| 8 · Permissions: Save disabled for a Support Agent | `SettingsView.ApplyPermissions()` → `btnSaveSettings.Enabled = false`, note *Settings are view-only for this role.*; rule in `Services/PermissionService.CanSaveSettings`; server-side re-check in `SettingsView.SaveSettings()` |
| 9 · Run & test each nav button | the "What to try" table above; `docs/ShellLayout.md` and `docs/PermissionsMatrix.md` record the evidence |

Lab code check (`labs.js` m3): the code has a shared `NavigateTo(`, the nav buttons route through `btn…_Click`
handlers, `pnlContent.Controls.Clear()` / `Controls.Add(view)`, the four `…View` classes, `lblStatus` and
`lblBreadcrumb`, and a permission (`currentRole`, `"Support Agent"`, `btnSaveSettings.Enabled = false`,
`PermissionService`). Paste `MainPage.cs` (or `MainPage.cs` + `SettingsView.cs`) into the lab checker.

## Self-check

**Lesson s11 — Completion check**

- **Identify the header, navigation, content and status areas.** `pnlHeader` (Top), `pnlNav` (Left), `pnlContent`
  (Fill), `lblStatus` (Bottom) — the four controls added directly to `MainPage`. Everything else is inside one of them.
- **Use anchoring or docking instead of fixed positions.** The four regions are docked; the nav buttons are anchored
  Top + Left + Right; the header's user label and role picker are anchored Top + Right; the cards in the views anchor
  to the sides that should follow the browser. No control relies on the browser being 1348 px wide.
- **Explain why Page and Window/Form differ.** A `Page` fills the browser frame like a normal web app — nothing to
  move, nothing to minimize. A `Window`/`Form` is a desktop-style window inside the browser that can be dragged,
  minimized, maximized and closed. A shell is better as a `Page` (this module); a `Form` needs the minimize button
  removed unless there is a taskbar to restore it from.
- **Route every nav button through one shared navigation method.** Each `btn…_Click` is one line, `NavigateTo("…")`.
  The method clears `pnlContent`, creates the view, docks it, adds it, updates the breadcrumb, highlights the button
  and writes the status — the only place in the app that does any of that.

**Lesson s12 — Designer checklist**

- **Dock or Anchor on header, nav, content and status bar** — yes, see above and `docs/ShellLayout.md` §2.
- **Remove the minimize button on a Window/Form unless you build a taskbar** — not needed: the shell is a `Page`.
  (If it were a `Form`: `this.MinimizeBox = false;`.)
- **Prefer a Page when the shell should fill the browser** — `MainPage : Page`, `Application.MainPage = new MainPage()`.
- **Keep navigation logic centralized in the shell** — `NavigateTo` lives in `MainPage`; the views never add anything
  to `pnlContent` and never touch `lblBreadcrumb` or `lblStatus` (they only call `shell.Log(...)` through `IShellHost`).
- **Use UserControls for repeated content** — `DashboardView`, `TicketsView`, `CustomersView`, `SettingsView`.

## Verified / unverified

`dotnet build -nologo -v q` succeeds for both targets with 0 warnings, 0 errors. Not run in the browser by the
author — the reviewer runs it. Calls the cookbook marks unverified, or that are new in this module:

- `Application.MainPage = new MainPage()` with a `Wisej.Web.Page` as the shell (the cookbook names it, not executed there).
- Dock layout order on a `Page`: `Controls.Add(pnlContent)` first, `pnlHeader` last — WinForms order; check that the
  nav panel sits under the header and the content panel gets the remaining space.
- `UserControl.Load` firing after `pnlContent.Controls.Add(view)` — the views fill their lists in `…View_Load`.
- `Button.BackColor` / `ForeColor` / `Font` changes at runtime for the active nav button (`SetActiveButton`).
- `ComboBox.DropDownStyle = DropDownList`, `SelectedItem = "…"` in code, and `SelectedIndexChanged` (used for `cboRole`
  and `cboDefaultPriority`); `cboRole.SelectedItem as string` reading the role back.
- `DataGridView.Rows.Add(object[])` returning the row index, `Rows[i].Selected = true`, `CurrentRow` / `SelectedRows[0]`,
  `Cells[0].Value` on an unbound grid; `RowHeadersVisible = false`, `AllowUserToAddRows = false`.
- `Label.Padding` on the docked `lblStatus`, `Panel.Padding = 24` on `pnlContent` (the docked view should sit inside the padding).
- `AlertBox.Show(text, icon, alignment: TopRight, autoCloseDelay: …)` with `\n` line breaks in the text (Customers → View Selected).

Deliberately left out: a login form (the lesson says the role is a design concept — `cboRole` stands in for it),
persisting the settings anywhere (only the permission decision matters here), and the per-screen right-hand event-log
card — the course's event log is the Dashboard's *Recent activity* card, fed by the shell's `Log()`, so the status bar
and the log can never disagree.
