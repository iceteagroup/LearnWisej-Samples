# Modernization checklist — visual hierarchy without rewriting logic (Module 7)

## 1. Visual-hierarchy rules and where each one is applied

| Rule | What it means | Where it is applied in this app |
|---|---|---|
| Make the page title obvious | The user immediately knows which screen they are on | `lblPageTitle` — 18 pt bold at (32, 24) on Dashboard, Tickets, Customers, Reports and Settings; a grey one-line `lblPageDescription` under it at (32, 60); the window title and `lblAppTitle` "ServiceDesk" in the header |
| Group related content | Panels / cards keep related records and commands together | Dashboard: four metric cards, a Commands card and a Recent activity card; Tickets: the queue and its buttons in one "Ticket queue" card; Settings: "UI settings" and "What the visual layer owns" cards. Every card is a white `Panel` with `BorderStyle.Solid`, title at (20, 14), content from y = 52 |
| Keep commands predictable | Buttons sit near the content they affect | `btnNew / btnEdit / btnDelete` are anchored to the bottom of the ticket-queue card, under the grid; `btnCreateTicket / btnEditTicket / btnCloseTicket` sit together in the Commands card; Save / Cancel are bottom-right in the dialog; every command button is 36 px high, 12 px apart |
| Use consistent spacing | Screens feel like the same application | `Views/LayoutRules.cs`: 32 px page padding, first card row at y = 100, 24 px gap between cards, 250 px metric cards, 524 px wide cards. The Customers, Reports and Settings cards are the same 524 px and start at the same (32, 100) as the Dashboard commands card |
| Show active navigation state | The selected navigation button looks selected | `MainWindow.SetActiveButton(Button)`: green `(31,157,87)` background, white text, `BorderStyle.Solid`, bold; the other four buttons get `ResetBackColor()` / `ResetForeColor()`, `BorderStyle.None`, regular font. Called from `NavigateTo`, so it can never disagree with the page on screen |

## 2. Polish without rewriting logic — checklist

- [x] **Rename controls clearly, but do not break existing handlers.** The Module 4/5 names stayed:
      `dgvTickets`, `ticketsBindingSource`, `btnNew_Click`, `btnEdit_Click`, `btnDelete_Click`,
      `TicketDialog`, `ValidateForm`, `TicketResult`, `btnSave_Click`, `btnCancel_Click`. The new names
      (`cboTheme`, `lblCurrentTheme`, `btnDashboard … btnSettings`, `lstActivity`, `cardQueue`) are new controls.
- [x] **Move related controls into panels or cards without changing the service layer.** The grid and its
      buttons moved into `cardQueue`; `Services/TicketService.cs` and `Models/Ticket.cs` are byte-for-byte the
      Module 5 versions plus a `Clone()` helper the edit dialog already needed.
- [x] **Use Dock, Anchor and AutoSize intentionally.** Header `Dock.Top`, navigation `Dock.Left`, status
      `Dock.Bottom`, content `Dock.Fill`, each view `Dock.Fill`. Inside the views: the ticket-queue card, the
      grid and the activity card are anchored on all four sides; the buttons under the grid are anchored
      `Bottom | Left` (and `Bottom | Right` for the validation button); header controls are anchored
      `Top | Right`. Metric cards stay fixed. All labels are `AutoSize = false` with explicit sizes.
- [x] **Keep the same data source and binding logic.** `ticketsBindingSource.DataSource =
      shell.Tickets.GetTickets()`, `AutoGenerateColumns = false`, the same `DataPropertyName` columns,
      `ResetBindings(false)` after every save — `TicketsView.RefreshGrid()`.
- [x] **Test each navigation button and workflow after the polish.** Every nav button goes through
      `NavigateTo`; New / Edit / Delete / Cancel / blank-title validation / theme switch are all logged in the
      Recent activity list (see Evidence) so the reviewer can tick them one by one.
- [x] **Do not hide broken logic behind a nicer theme.** The failure paths are still visible and safe: an
      unknown theme name is reported in red and the previous theme kept; a blank title is blocked with a
      message; an unknown page name in `NavigateTo` is reported instead of throwing.

## 3. Module 7 checkpoint — answers

- **Can you explain what the theme changes?** Only how the app looks: colours, fonts, paddings, borders
  and control styling for every control on every page, applied by `Application.LoadTheme(name)` from
  `cboTheme_SelectedIndexChanged`. The ticket list, the metric numbers, the validation messages and the
  navigation are identical before and after a switch — the activity log proves it line by line.
- **Can you point to the business logic that should not change?** `Services/TicketService.cs`
  (`GetTickets`, `GetTicket`, `AddTicket`, `UpdateTicket`, `DeleteTicket`, `SaveTicket`), `Models/Ticket.cs`,
  `TicketDialog.ValidateForm()` and the `DialogResult.OK` / `Cancel` flow, `MainWindow.NavigateTo(string)`.
  None of them reads `Application.Theme`, `cboTheme` or `lblCurrentTheme`.
- **Can you identify the current page from the navigation state?** Yes — `SetActiveButton` paints exactly
  one nav button green with a border and bold text, and it is always the button of the view in
  `panelContent`, because both are set in the same `NavigateTo` call. The page title repeats the name.
- **Can you explain why cards, labels and spacing make the app easier to use?** Cards tell the eye which
  records and commands belong together (metrics, commands, activity, the queue); a consistent title /
  description / card pattern means a user who learned the Dashboard already knows how to read Reports or
  Settings; equal padding and gaps stop the screens from looking like five different apps; large numbers
  in the metric cards answer "how are we doing?" before the user reads anything else.

## Evidence

| Action | Recent activity line | Screen |
|---|---|---|
| Click **Tickets**, **Customers**, **Reports**, **Settings**, **Dashboard** | `NavigateTo("Tickets") → TicketsView shown, btnTickets active` (one line per page) | the clicked nav button is green with a border; the page title matches; status green "● Tickets page" |
| **Create ticket** (Dashboard) → fill in a title and customer → Save | `Ticket #7 created — "…" (TicketService.AddTicket, metric cards refreshed)` | the Open card counts one more; on the Tickets page the new row is selected |
| **Close ticket** (Dashboard) → Yes | `Ticket #1 closed (TicketService.UpdateTicket, Status = Closed) — metric cards refreshed` | Open −1, Closed +1 |
| **Edit ticket** (Dashboard) | `Dashboard → btnEditTicket_Click → NavigateTo("Tickets") to pick a ticket in the queue` | Tickets page, status amber "select a ticket in the queue, then click Edit" |
| Tickets → select a row → **Edit** → change the status → Save | `Ticket #2 saved — "…", Closed (TicketService.UpdateTicket)` | the row updates, stays selected |
| Tickets → **Edit** → Cancel | `Edit ticket #2 → Cancel — the stored ticket is untouched (the dialog worked on a copy)` | nothing changed |
| Tickets → **Delete** → Yes / No | `Ticket #3 deleted (MessageBox.ShowAsync → Yes → TicketService.DeleteTicket)` or `Delete ticket #3 → No — nothing changed` | row gone / row still there |
| Tickets → **Try a blank title (validation)** | `btnTryBlankTitle_Click → ValidateForm() = False → "Title is required." — theme Bootstrap-4 changed nothing about the rule` | dialog opens with the red message already showing; type a title and Save → `Recovery: title filled in → ValidateForm() = true → ticket #8 created` |
| Switch to **BootstrapDark-4** and repeat the blank-title step | the same lines with `theme BootstrapDark-4` | same behaviour, dark look |
