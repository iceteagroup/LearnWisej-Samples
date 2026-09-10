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
`dotnet build -nologo -v q` passes with no warnings for both targets (`net10.0-windows`, `net10.0`).

Unlike the other modules this screen is a **`Page`** (`Application.MainPage`) rather than a 1348×680
`Form`: the page fills the browser, so the responsive behaviour can be watched by resizing the real
window. The frame keeps the course layout — screen card left, **Activity trace** right, path buttons
below — and re-docks itself per profile too (trace card right → bottom → hidden).

## What to click in the Ticket Workspace

The strip above the workspace shows the live profile: `Active profile: Desktop · browser 1400×760 px ·
device Desktop · Phone ≤ 600 · Tablet 601–1024 · Desktop ≥ 1025`. The right card is the **Activity
trace · CLIENT → UI → Service → Data**: browser and profile events are `[CLIENT]`, layout toggles are
`[UI]`, decisions are `[SVC]` / `[DOMAIN]`, persistence is `[DATA]`, the profile file is `[INFRA]`.

| Action | Path | What you should see |
|---|---|---|
| **Resize the browser** below 1024 px, then below 600 px, then back | the real thing | `[CLIENT] TicketWorkspace.ResponsiveProfileChanged — Desktop → Tablet · browser 900×760 px`, `[UI] TicketWorkspace.MovePanelsToTabs — pnlDetail → tabPageDetails, pnlActivity → tabPageActivity (re-parented, widget state intact)`, `[UI] TicketWorkspace.ApplyResponsiveProfile — Tablet → nav compact (56 px) · Assignee column hidden · … · state kept (moved, not rebuilt): selected …`, `[UI] MainPage.ApplyFrameProfile — Tablet → trace card bottom, button bar 108 px`; the rail shrinks to icons, Assignee disappears, Details/Activity tabs appear, the trace card moves under the workspace. Below 600 px: header **Tickets**, no rail/toolbar; tap a row → **← Back  Ticket #1003**; the trace card hides. Status bar: `Active profile: …` |
| **Preview as desktop / tablet / phone** (combo) | success — on demand | `[UI] MainPage.Preview — preview Phone (browser really Desktop) → workspace.PreviewProfile("Phone") in a 400 px host`; the workspace becomes a 400 px column with the phone arrangement; banner *Previewing the phone layout — the browser is really on the Desktop profile*; strip adds `preview pinned: Phone`; real profile changes are logged but ignored until **Live — follow the browser** |
| **▶ Tour all profiles** | progress | a `Timer` selects desktop → tablet → phone → live every 1.6 s; progress bar 0→4, status **● tour n/4**, one `ApplyResponsiveProfile` block per step, ends **● live** |
| **Select a row · edit Title · click a chip (Open, High, Mine, Today, Unassigned, Escalated)** | success (search) | `[UI] TicketWorkspace.chip_Click — chip "High" on`, `[SVC] TicketService.SearchAsync — validate {text:"", chip:High}`, `[DATA] … rows`, `[SVC] 2 of 7 tickets match`; chips wrap when the list is narrow |
| **SearchBar #1** (toolbar) / **SearchBar #2** (activity card) | success (reuse) | `[UI] TicketWorkspace.searchTickets_SearchRequested — SearchBar #1 raised SearchRequested("safari")` → `SearchAsync`; the activity bar: `SearchBar #2 raised SearchRequested("patel") → ITicketService.GetActivityAsync` — one UserControl, two hosts |
| **Save** / **Close** in the detail card | success + domain rule | `[SVC] validate {…} valid → UpsertAsync(#1003)`, `[DATA] #1003 written`, `[DATA] event #7 … written`, the feed gains *S. Patel updated #1003*; Close on a ticket without hours: `[DOMAIN] ⚠ Ticket.CanClose — #1002 rejected: Log hours before closing.` |
| **Search 1 character** | failure 1 (validation) | `[SVC] ⚠ TicketService.SearchAsync — rejected: query "x" is shorter than 2 characters` — no `[DATA]` line; orange banner **Type at least 2 characters to search.**; status **● not applied** |
| **Apply profile "Kiosk"** | failure 2 (rule + fallback) | `[INFRA] ClientProfileCatalog.Contains — "Kiosk" defined in ClientProfiles.json? no`, `[UI] ⚠ TicketWorkspace.ApplyResponsiveProfile — "Kiosk" is not Desktop / Tablet / Phone … → desktop layout as the safe fallback`; orange banner **Profile "Kiosk" is not defined in ClientProfiles.json — the desktop layout was applied.**; status bar `Active profile: Kiosk (fallback: desktop layout)`; choose **Live** to return |
| **Simulate data outage** | error path | `[DATA] ✖ outage: SELECT * FROM Tickets failed — timeout connecting to sql01:1433 (TicketOps.dbo.Tickets)` and `[UI] ✖ TicketWorkspace.Refresh — caught DataOutageException — user sees the safe message` stay in the trace; the user sees only the red banner **The action could not be completed. Check the log for details.** and a toast; status **● failed** |
| **Recover the data store** (same button) | recovery | `[UI] outage OFF → refresh (recovery)`, `[DATA] 7 rows`, `[DATA] 6 events`; grid and feed reload; status **● ready** |
| **Clear trace** | — | empties the right-hand card |

Round-trip proof: select **#1003**, type into Title, switch profiles (resize or Tour) and come back —
the text, the selection, the chip and the feed are all still there; every switch logs
`state kept (moved, not rebuilt): selected #1003, draft title "…"`.

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | `TicketWorkspace` UserControl hosting navigation, grid, detail and activity, composed from reusable pieces | `Controls/TicketWorkspace.cs` + `.Designer.cs` (hosted by `Views/MainPage`) |
| 2 | Engines picked by intent: Dock/Anchor shell, `TableLayoutPanel` form grid, `FlowLayoutPanel` chips/buttons, `FlexLayoutPanel` proportional regions | `TicketWorkspace.Designer.cs`; table in [`docs/ResponsiveLayoutNotes.md`](TicketOps/docs/ResponsiveLayoutNotes.md); diagram [`docs/WorkspaceComposition.svg`](TicketOps/docs/WorkspaceComposition.svg) |
| 3 | Desktop profile: all panels at once | `TicketWorkspace.ShowAllPanels()` |
| 4 | Tablet profile: activity collapsed into a tab | `TicketWorkspace.CollapseActivityToTab()` + `MovePanelsToTabs()` |
| 5 | Phone profile: single task view with a back button | `TicketWorkspace.ShowSingleTaskView()`, `ShowPhonePage()`, `btnBack_Click` |
| 6 | `ClientProfiles.json` + `ResponsiveProfileChanged` handler that adapts server-side properties | `TicketOps/ClientProfiles.json` (copied to `/bin` by the csproj), `TicketWorkspace.Application_ResponsiveProfileChanged` → `OnResponsiveProfileChanged` → `ApplyResponsiveProfile(Application.ActiveProfile.Name)`; `Infrastructure/ClientProfileCatalog.cs` reads the file back |
| 7 | Reusable `SearchBar` UserControl used across screens | `Controls/SearchBar.cs` + `.Designer.cs`; instances `searchTickets` (toolbar) and `searchActivity` (activity card) |
| 8 | Profile screenshots or notes | [`docs/ProfileNotes.md`](TicketOps/docs/ProfileNotes.md) |
| 9 | Production-readiness note | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |
| — | Active profile + browser size shown live; preview without resizing | `Views/MainPage.cs` (`UpdateProfileStrip`, `comboPreview`, `buttonTour`) |

## Where things live

```
TicketOps/
├─ ClientProfiles.json            Phone ≤ 600 · Tablet 601–1024 · Desktop ≥ 1025 (browser width; copied to /bin for both targets)
├─ Views/
│  ├─ MainPage.cs                 the Page that fills the browser: profile strip, frame re-docking, the path buttons
│  └─ MainPage.Designer.cs        GENERATED-style layout (Dock frame + FlowLayoutPanel bar) — no logic here
├─ Controls/
│  ├─ TicketWorkspace.cs          ApplyResponsiveProfile (the switch), ShowAllPanels / CollapseActivityToTab / ShowSingleTaskView, thin handlers
│  ├─ TicketWorkspace.Designer.cs the composition: Dock shell, flexBody, flexSide, tableDetail, flowChips, tabActivity
│  ├─ SearchBar.cs                Query · Placeholder · ButtonText · SearchRequested · Clear() — the inner controls stay private
│  ├─ SearchBar.Designer.cs       one TableLayoutPanel (100 % box + 84 px button)
│  └─ StatusBanner                reusable "● state" + banner UserControl (from the template)
├─ Services/
│  ├─ ITicketService.cs           GetOpenTicketsAsync · SearchAsync(TicketFilter) · SaveAsync · CloseAsync · GetActivityAsync
│  └─ TicketService.cs            validation, chip meaning, close rule, persistence orchestration (no UI types, no profile names)
├─ Domain/
│  ├─ Ticket.cs                   record + CanClose / Close; Module 3 adds Assignee, Notes, IsEscalated
│  ├─ TicketDraft.cs              what the detail form collects (UI → data)
│  ├─ TicketFilter.cs             free text + one chip (the screen's vocabulary, decided in the service)
│  ├─ TicketEvent.cs              one line of the activity feed
│  └─ OperationResult.cs          success / safe explanation handed back to the screen
├─ Data/
│  ├─ ITicketRepository.cs        persistence contract (+ the activity feed)
│  └─ InMemoryTicketRepository.cs fake store seeded with the video's tickets and feed; SimulateOutage throws like a real driver
├─ Infrastructure/
│  ├─ ClientProfileCatalog.cs     reads ClientProfiles.json back: Summary, Contains(name)
│  ├─ ILog.cs / ActivityLog.cs    cross-cutting logging (details stay here)
│  └─ AppComposition.cs           who gets what: one object graph per session, constructor injection, no statics
├─ Resources/Strings.cs           safe user-facing messages (UnknownProfile, PreviewPinned, ActionFailed, …)
├─ Diagnostics/ActivityTracePanel the live trace card
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
  `TableLayoutPanel` 35 % / 65 %. `flowChips`, `flowDetailButtons`, the frame's bottom bar → *wrap or
  hug an edge* → `FlowLayoutPanel`. `tabActivity` → *same panels, less width* → `TabControl`. The full
  table is in [`docs/ResponsiveLayoutNotes.md`](TicketOps/docs/ResponsiveLayoutNotes.md).
- **Can the layout be explained without opening the designer?**
  Yes — `TicketWorkspace.Designer.cs` reads top-down as that table, and the tree at the bottom of
  [`docs/WorkspaceComposition.svg`](TicketOps/docs/WorkspaceComposition.svg) is the same structure. The
  one rule worth memorising is in the `Controls.Add` block: the Fill child first, then the edges (the
  control added last docks first).
- **What server-side properties change when the active profile changes — and do you toggle panels rather than rebuild them?**
  `pnlNavigation.Visible/Width`, `pnlToolbar.Visible`, `btnBack.Visible`, `lblTitle.Text`,
  `colAssignee.Visible`, `colStatus.Visible`, the `flexBody` fill weights and `Padding`,
  `pnlDetail.Parent` / `pnlActivity.Parent` (moved between `flexSide` and the tab pages), plus the frame's
  `pnlTraceHost.Dock/Visible` and `pnlActionsHost.Height`. Nothing is `new`-ed in the handler; the trace
  prints the preserved state before every switch.
- **Do hidden controls still hold sensitive state you need to clear?**
  They hold state — that is what makes the round trip work (the draft survives). None of it is
  sensitive here; if a panel held a secret, `ApplyResponsiveProfile` is where it would be cleared before
  the panel is hidden ([`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md), item 4).
- **Adapt or split?** Adapt: the agent does the same job on every device, so one workspace with three
  arrangements. A narrower phone-only job (approve, not edit) would justify a separate simpler view.
