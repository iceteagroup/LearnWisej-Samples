# TicketOps · Production Architecture Deep Dive · Module 3

Local lab build for **Module 3 · Responsive Layouts, Client Profiles & Reusable UI Composition**. It
follows the walkthrough video: one **`TicketWorkspace` UserControl** — navigation, ticket grid, ticket
detail, activity feed — composed from nested single-purpose containers (Dock shell, `FlexLayoutPanel`
regions, a `TableLayoutPanel` form, `FlowLayoutPanel` chips) that **re-arranges itself for desktop,
tablet and phone** when the active Client Profile changes (`ClientProfiles.json` +
`Application.ResponsiveProfileChanged`), and a reusable **`SearchBar` UserControl** used twice instead
of two copies of a textbox-and-button. The profile handler toggles panels that already exist; nothing is
rebuilt, so the selected ticket, the draft and the search text survive every switch.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 3\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5103
```

Then open <http://localhost:5103>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

Unlike the other modules this screen is a **`Page`** (`Application.MainPage`) rather than a fixed-size
`Form`: the page fills the browser, so the responsive behaviour can be watched by resizing the real window.

## What to try in the Ticket Workspace

| Action | What you should see |
|---|---|
| **Resize the browser** below 1024 px, then below 600 px, then back | Tablet: the rail shrinks to icons, Assignee disappears, Details / Activity tabs appear. Phone: header **Tickets**, no rail/toolbar; tap a row → **← Back  Ticket #1003**. The status bar shows `Active profile: …` |
| Select a row, edit it, **Save** | status **● Ticket #1003 saved.**; the activity feed gains *S. Patel updated #1003* |
| Clear the title, **Save** | orange banner **Title is required.** |
| Select #1002 (no hours logged), **Close** | orange banner **Log hours before closing.** |
| Click a chip (Open, High, Mine, Today, Unassigned, Escalated) | the grid filters; chips wrap when the list is narrow |
| Type in the ticket SearchBar / the activity SearchBar, press Enter | the grid / the feed filters — one UserControl, two hosts. One character only: orange banner **Type at least 2 characters to search.** |

Round trip: select **#1003**, type into Title, resize across the profiles and come back — the text, the
selection, the chip and the feed are all still there.

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | `TicketWorkspace` UserControl hosting navigation, grid, detail and activity, composed from reusable pieces | `Controls/TicketWorkspace.cs` + `.Designer.cs` (hosted by `Views/MainPage`) |
| 2 | Engines picked by intent: Dock/Anchor shell, `TableLayoutPanel` form grid, `FlowLayoutPanel` chips/buttons, `FlexLayoutPanel` proportional regions | `TicketWorkspace.Designer.cs`; table in [`docs/ResponsiveLayoutNotes.md`](TicketOps/docs/ResponsiveLayoutNotes.md); diagram [`docs/WorkspaceComposition.svg`](TicketOps/docs/WorkspaceComposition.svg) |
| 3 | Desktop profile: all panels at once | `TicketWorkspace.ShowAllPanels()` |
| 4 | Tablet profile: activity collapsed into a tab | `TicketWorkspace.CollapseActivityToTab()` + `MovePanelsToTabs()` |
| 5 | Phone profile: single task view with a back button | `TicketWorkspace.ShowSingleTaskView()`, `ShowPhonePage()`, `btnBack_Click` |
| 6 | `ClientProfiles.json` + `ResponsiveProfileChanged` handler that adapts server-side properties | `TicketOps/ClientProfiles.json` (copied to `/bin` by the csproj), `TicketWorkspace.Application_ResponsiveProfileChanged` → `OnResponsiveProfileChanged` → `ApplyResponsiveProfile(Application.ActiveProfile.Name)` |
| 7 | Reusable `SearchBar` UserControl used across screens | `Controls/SearchBar.cs` + `.Designer.cs`; instances `searchTickets` (toolbar) and `searchActivity` (activity card) |
| 8 | Profile screenshots or notes | [`docs/ProfileNotes.md`](TicketOps/docs/ProfileNotes.md) |
| 9 | Production-readiness note | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |

## Where things live

```
TicketOps/
├─ ClientProfiles.json            Phone ≤ 600 · Tablet 601–1024 · Desktop ≥ 1025 (browser width; copied to /bin for both targets)
├─ Views/MainPage                 the Page that fills the browser: status banner + the workspace
├─ Controls/
│  ├─ TicketWorkspace.cs          ApplyResponsiveProfile (the switch), ShowAllPanels / CollapseActivityToTab / ShowSingleTaskView, thin handlers
│  ├─ TicketWorkspace.Designer.cs the composition: Dock shell, flexBody, flexSide, tableDetail, flowChips, tabActivity
│  ├─ SearchBar.cs                Query · Placeholder · ButtonText · SearchRequested · Clear() — the inner controls stay private
│  ├─ SearchBar.Designer.cs       one TableLayoutPanel (100 % box + 84 px button)
│  └─ StatusBanner                reusable "● state" + banner UserControl
├─ Services/
│  ├─ ITicketService.cs           GetOpenTicketsAsync · SearchAsync(TicketFilter) · SaveAsync · CloseAsync · GetActivityAsync
│  └─ TicketService.cs            validation, chip meaning, close rule, persistence orchestration (no UI types, no profile names)
├─ Domain/                        Ticket (CanClose / Close), TicketDraft, TicketFilter, TicketEvent, OperationResult
├─ Data/                          ITicketRepository + InMemoryTicketRepository (seeded with the video's tickets and feed)
├─ Infrastructure/                ILog / ActivityLog (server console), AppComposition (one object graph per session)
├─ Resources/Strings.cs           safe user-facing messages
├─ docs/                          ResponsiveLayoutNotes.md · ProfileNotes.md · ProductionReadinessNote.md · WorkspaceComposition.svg
├─ Default.html                   + viewport meta tag so phones report their CSS width
├─ Program.cs                     session entry point → Application.MainPage = AppComposition.CreateMainView()
└─ Startup.cs                     Kestrel host (app.UseWisej()); *.json is never served
```

In Visual Studio the same per-profile values (`Visible`, `Width`, `Dock`) could be entered in the
Wisej Designer's profile drop-down (`Control.ResponsiveProfiles`); this sample writes them in
`ApplyResponsiveProfile` so the decision reads as a table in one place.

## Self-check answers (lesson guide)

- **Can you name the intent behind each container and the engine that serves it?**
  Shell → *pin regions to edges, body fills* → Dock. `flexBody` / `flexSide` → *share space by ratio* →
  `FlexLayoutPanel` with fill weights. `tableDetail` → *keep captions and fields aligned* →
  `TableLayoutPanel` 35 % / 65 %. `flowChips`, `flowDetailButtons` → *wrap or hug an edge* →
  `FlowLayoutPanel`. `tabActivity` → *same panels, less width* → `TabControl`. The full table is in
  [`docs/ResponsiveLayoutNotes.md`](TicketOps/docs/ResponsiveLayoutNotes.md).
- **Can the layout be explained without opening the designer?**
  Yes — `TicketWorkspace.Designer.cs` reads top-down as that table, and the tree at the bottom of
  [`docs/WorkspaceComposition.svg`](TicketOps/docs/WorkspaceComposition.svg) is the same structure. The
  one rule worth memorising is in the `Controls.Add` block: the Fill child first, then the edges (the
  control added last docks first).
- **What server-side properties change when the active profile changes — and do you toggle panels rather than rebuild them?**
  `pnlNavigation.Visible/Width`, `pnlToolbar.Visible`, `btnBack.Visible`, `lblTitle.Text`,
  `colAssignee.Visible`, `colStatus.Visible`, the `flexBody` fill weights and `Padding`, and
  `pnlDetail.Parent` / `pnlActivity.Parent` (moved between `flexSide` and the tab pages). Nothing is
  `new`-ed in the handler.
- **Do hidden controls still hold sensitive state you need to clear?**
  They hold state — that is what makes the round trip work (the draft survives). None of it is
  sensitive here; if a panel held a secret, `ApplyResponsiveProfile` is where it would be cleared before
  the panel is hidden ([`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md), item 4).
- **Adapt or split?** Adapt: the agent does the same job on every device, so one workspace with three
  arrangements. A narrower phone-only job (approve, not edit) would justify a separate simpler view.
