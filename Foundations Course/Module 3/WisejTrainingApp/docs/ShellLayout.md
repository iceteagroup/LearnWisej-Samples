# Shell layout — the four regions, Dock/Anchor, Page vs Window, NavigateTo

The lesson "Application shell and navigation" says a real app has a shared outer structure that stays put
while only the content changes. This is how the Module 3 shell (`MainPage`) implements it.

## 1. The four regions

| Shell part | Control | Dock | Size | What it holds |
|---|---|---|---|---|
| Header | `pnlHeader` (Panel) | `Top` | 64 px high | `lblAppTitle` "ServiceDesk Application", `lblBreadcrumb` "Home / Dashboard", `lblUser` "Signed in as: Support Agent", `cboRole` (Support Agent / Manager) |
| Left navigation | `pnlNav` (Panel) | `Left` | 220 px wide | `btnDashboard`, `btnTickets`, `btnCustomers`, `btnSettings` |
| Content area | `pnlContent` (Panel) | `Fill` | whatever is left | the current view — one `UserControl`, docked to fill |
| Status area | `lblStatus` (Label) | `Bottom` | 32 px high | `03:26:05 PM - Opened Tickets page.` |

The shell is the only thing in `MainPage.Designer.cs`. The pages (Dashboard, Tickets, Customers, Settings)
are separate `UserControl`s in `Views/`; the shell never knows what is inside them.

## 2. Dock / Anchor table (from the lesson, as applied)

| Control area | Layout used | Why |
|---|---|---|
| Header panel | `Dock = Top` | stretches across the top at any browser width |
| Left navigation panel | `Dock = Left` | stays on the left and keeps the full height under the header |
| Content panel | `Dock = Fill` | the current page uses whatever is left |
| Status label | `Dock = Bottom` | always visible at the bottom |
| Navigation buttons | `Anchor = Top + Left + Right` inside `pnlNav` | all four keep the same width if the nav panel is resized |
| `lblUser`, `cboRole` in the header | `Anchor = Top + Right` | stay glued to the right edge |
| Cards inside the views | `Anchor` on the sides that should follow the content panel | the ticket grid, the customer list and the activity log grow with the browser |

**Dock order.** Docking is resolved from the *last* control in `Controls` to the first, so the Designer adds the
`Fill` panel first and the `Top` panel last:

```csharp
this.Controls.Add(this.pnlContent);   // Fill  — laid out last, gets what is left
this.Controls.Add(this.lblStatus);    // Bottom
this.Controls.Add(this.pnlNav);       // Left  — below the header because the header is docked first
this.Controls.Add(this.pnlHeader);    // Top   — laid out first
```

If the nav panel ever overlaps the header, that order is what to check.

## 3. Page vs Window/Form — and the minimize-button habit

| Choice | What it feels like | This module |
|---|---|---|
| `Page` | fills the browser frame like a normal web app; no title bar, nothing to move or minimize | **chosen** — `MainPage : Wisej.Web.Page`, started with `Application.MainPage = new MainPage();` in `Program.cs` |
| `Window` / `Form` | a desktop-style window inside the browser that can move, minimize, maximize and close | Modules 1, 2, 4… use it for single screens and dialogs |

The lesson's habit: if the shell *is* a `Form`, remove or disable the minimize button (`MinimizeBox = false`)
unless you also build a taskbar or a restore area — a minimized shell looks like the app disappeared. A `Page`
sidesteps the problem, which is one reason it is the better fit for a shell.

## 4. NavigateTo — one place changes the page

Every nav button handler is one line:

```csharp
private void btnTickets_Click(object sender, EventArgs e)
{
    NavigateTo("Tickets");
}
```

and the one shared method does the work:

```csharp
private void NavigateTo(string pageName)
{
    pnlContent.Controls.Clear();

    UserControl view;
    switch (pageName)
    {
        case "Tickets":   view = new TicketsView(this, ticketService); break;
        case "Customers": view = new CustomersView(this, customerService, ticketService, permissions); break;
        case "Settings":  view = new SettingsView(currentRole, this, permissions); break;
        default:          pageName = "Dashboard"; view = new DashboardView(this, ticketService, customerService, healthCheck); break;
    }

    view.Dock = DockStyle.Fill;
    pnlContent.Controls.Add(view);

    currentPage = pageName;
    lblBreadcrumb.Text = "Home / " + pageName;
    SetActiveButton(pageName);
    Log("Opened " + pageName + " page.");        // → lblStatus: "hh:mm:ss tt - Opened Tickets page."
}
```

Clear → create → dock → add → breadcrumb → highlight → status. Nothing else in the app adds to
`pnlContent.Controls`; if a page ever shows up wrong, there is one method to read.

The views receive the services the shell owns (`TicketService`, `CustomerService`, …) so a ticket created on
the Tickets page is still there after going to the Dashboard and back — the *view* is recreated, the *data* is not.
They also receive the shell as `IShellHost`, which is how they write the status bar (`shell.Log(...)`) without
ever touching `lblStatus` themselves.

## 5. The status bar and the activity list are the same thing

`MainPage.Log(message, kind)` is the only writer of `lblStatus`. It formats
`DateTime.Now.ToString("hh:mm:ss tt") + " - " + message`, colours the label (green / amber / red), appends the
line to a per-session `List<string>` and, if the Dashboard is on screen, pushes the line into its
"Recent activity" list (`lstEventLog`) too. Navigating to the Dashboard replays the whole list, so nothing that
happened on the other pages is lost.

## Evidence

- On load: breadcrumb `Home / Dashboard`, **Dashboard** highlighted blue, status
  `hh:mm:ss PM - Opened Dashboard page.` (preceded in the activity list by `Application shell loaded.`).
- Click **Tickets**: the content area swaps to the ticket grid, breadcrumb `Home / Tickets`, status
  `… - Opened Tickets page.`, **Tickets** highlighted and **Dashboard** back to white.
- Click **Customers**, **Settings**: same pattern; the header, nav and status bar never move.
- Resize the browser: the header stretches, the nav keeps 220 px, the content panel (and the grid / list /
  activity card inside it) grows, the status bar stays at the bottom.
- Back on **Dashboard**: the Recent activity card lists every `Opened … page.` line in order.
