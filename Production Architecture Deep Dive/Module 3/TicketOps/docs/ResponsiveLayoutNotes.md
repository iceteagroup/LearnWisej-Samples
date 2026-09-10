# Responsive layout notes — the Ticket Workspace

*Module 3 deliverable · TicketOps Console · layout decisions, one engine per container*

The Ticket Workspace (`Controls/TicketWorkspace`) is one UserControl with four regions — navigation,
ticket list, ticket detail, activity feed — that must work on desktop, tablet and phone. Every
container in it runs **exactly one** layout engine, chosen by the intent of its content, and the
active Client Profile decides how the same panels are arranged. Nothing is positioned by coordinate
and nothing is rebuilt when the profile changes.

The composition is drawn in [`WorkspaceComposition.svg`](WorkspaceComposition.svg).

## Engine per container (intent → engine)

| Container | Engine | Intent | Where |
|---|---|---|---|
| `TicketWorkspace` (outer shell) | **Dock** | Regions pinned to edges, the body fills the rest: header `Top`, toolbar `Top`, navigation `Left`, status `Bottom`, body `Fill` | `TicketWorkspace.Designer.cs` — the `Controls.Add` block at the end |
| `flexBody` | **FlexLayoutPanel** (horizontal) | Ticket list and side pane share the width **by ratio** — 3 : 2 on desktop, 1 : 1 on tablet — instead of a fixed split | `flexBody.SetFillWeight(pnlList, 3)` / `(pnlSide, 2)` |
| `flexSide` | **FlexLayoutPanel** (vertical) | Detail over activity, 3 : 2, on desktop | `flexSide.SetFillWeight(pnlDetail, 3)` / `(pnlActivity, 2)` |
| `tabActivity` | **TabControl** | On tablet and phone the *same* detail and activity panels sit behind two tabs to reclaim width | `MovePanelsToTabs()` re-parents `pnlDetail` / `pnlActivity` into the pages |
| `tableDetail` | **TableLayoutPanel** | Captions and fields stay aligned as the pane resizes: 35 % / 65 % columns, fixed-height rows, the Notes row takes the remaining height (`Percent 100`) | `tableDetail.ColumnStyles`, `RowStyles`, `Controls.Add(control, column, row)` |
| `flowChips` | **FlowLayoutPanel** | Six filter chips that wrap to a second line when the list card is narrow | `WrapContents = true` |
| `flowDetailButtons` | **FlowLayoutPanel** (right-to-left) | Save hugs the right edge, Close follows it — no anchored coordinates | `FlowDirection = RightToLeft` |
| `SearchBar` (inside) | **TableLayoutPanel** | One 100 % text column + one 84 px button column; the UserControl stays designer-friendly | `SearchBar.Designer.cs` |
| Frame (`Views/MainPage`) | **Dock** + **FlowLayoutPanel** | Workspace card `Fill`, trace card `Right` (desktop) / `Bottom` (tablet) / hidden (phone); the bottom bar is a Flow panel so the path buttons wrap on narrow browsers | `MainPage.ApplyFrameProfile` |

Every pixel value that remains is a *design-time* size (`Size = 712×456`) or a strip thickness
(`Height = 36`); no control is placed with `Location` inside a resizable region. The only `Location`s
are inside the fixed 52 px toolbar strip (`searchTickets` anchored left+right, `btnNewTicket` anchored right).

## Dock order — the gotcha, made explicit

The control added **last** docks **first**. The shell therefore adds `flexBody` (Fill) first, then
`pnlNavigation` (Left), `pnlToolbar` (Top), `pnlHeader` (Top) and `lblStatus` (Bottom):

```csharp
this.Controls.Add(this.flexBody);        // Fill — added first, laid out last: takes what is left
this.Controls.Add(this.pnlNavigation);   // Left
this.Controls.Add(this.pnlToolbar);      // Top (below the header)
this.Controls.Add(this.pnlHeader);       // Top (docks before the toolbar → sits above it)
this.Controls.Add(this.lblStatus);       // Bottom — added last, docks first
```

The same rule orders the four navigation items (`lblNavSettings` is added first so `lblNavDashboard`,
added last, ends up on top) and the frame (`pnlActionsHost` Bottom docks first, then `pnlTraceHost`
Right, then the workspace card fills).

## Profile → arrangement (the same controls, three shapes)

| | Desktop (≥ 1025 px) | Tablet (601–1024 px) | Phone (≤ 600 px) |
|---|---|---|---|
| Navigation rail | 150 px, icon + text | 56 px, icons only | hidden |
| Toolbar (SearchBar #1, New Ticket) | visible | visible | hidden |
| Ticket list | weight 3, all columns | weight 1, **Assignee column hidden** | the *list page*: full width, Assignee + Status hidden |
| Detail + activity | `flexSide` 3 : 2, both visible | `tabActivity` — Details / Activity tabs | the *task page*: `tabActivity` full width |
| Back button | — | — | in the header; returns to the list page |
| Frame: trace card | docked right (528 px) | docked bottom (220 px) | hidden |
| Frame: button bar | 60 px, one row | 108 px, wraps | 200 px, wraps |

The handler that does it (`TicketWorkspace.ApplyResponsiveProfile`) is a `switch` that delegates:

```csharp
switch (profileName)
{
    case "Desktop": ShowAllPanels();         break;
    case "Tablet":  CollapseActivityToTab(); break;
    case "Phone":   ShowSingleTaskView();    break;
    default:        /* warn, ShowAllPanels(), banner */ break;
}
```

`ShowAllPanels`, `CollapseActivityToTab` and `ShowSingleTaskView` only touch `Visible`, `Dock`,
`Width`, `Padding`, `SetFillWeight`, column `Visible` and `Parent`. The two `Move…` helpers re-parent
`pnlDetail` and `pnlActivity` between `flexSide` and the tab pages — the same instances, so the draft in
`txtTitle`, the selected row and the activity feed survive every switch (the trace prints the state
before each switch: `state kept (moved, not rebuilt): selected #1003, draft title "…"`).

## Server-side properties that change with the profile

The point of Client Profiles versus CSS media queries: these are **.NET properties on server objects**.

- `colAssignee.Visible` (tablet, phone) and `colStatus.Visible` (phone) — grid columns.
- `pnlNavigation.Width` + the four item texts (compact rail).
- `pnlToolbar.Visible`, `pnlNavigation.Visible`, `btnBack.Visible`, `lblTitle.Text`.
- `flexBody` fill weights and `Padding`.
- `pnlDetail.Parent` / `pnlActivity.Parent`.
- In the frame: `pnlTraceHost.Dock` / `Visible`, `pnlActionsHost.Height`.

Hidden is not cleared: a hidden editor still holds its draft (that is the feature). If a panel held a
secret (a token, a card number), `ApplyResponsiveProfile` would be the place to clear it before hiding.
Nothing in this workspace is sensitive, and the trace says so.

## Adapt or split?

The Ticket Workspace **adapts**: the agent does the same job — find a ticket, read it, update it, see
what happened — on every device, so one screen with three arrangements is right. The phone gets the
narrowest version (one pane at a time with a Back button) but not a different workflow. A genuinely
different task (an approver who only accepts/rejects from a phone) would get its own simpler view
routed by profile — that would be a **split**, and it is deliberately not done here.

## Evidence

Run the app and read the trace card:

- Load: `[INFRA] ClientProfileCatalog.Load — ClientProfiles.json: Phone ≤ 600 · Tablet 601–1024 · Desktop ≥ 1025`,
  `[CLIENT] MainPage.UpdateProfileStrip — Load: Application.ActiveProfile = Desktop · Application.Browser.Size = 1400×760`,
  `[UI] TicketWorkspace.ApplyResponsiveProfile — Desktop → nav 150 px · list : side = 3 : 2 · …`.
- Resize the browser below 1024 px: `[CLIENT] TicketWorkspace.ResponsiveProfileChanged — Desktop → Tablet …`,
  `[UI] TicketWorkspace.MovePanelsToTabs — pnlDetail → tabPageDetails, pnlActivity → tabPageActivity (re-parented, widget state intact)`,
  `[UI] MainPage.ApplyFrameProfile — Tablet → trace card bottom, button bar 108 px`.
- Below 600 px: `… Tablet → Phone`, the header shows **Tickets**, selecting a row shows **Ticket #1003** with **← Back**.
- Without resizing: **Preview as …** applies each arrangement in a device-width column; **▶ Tour all profiles** walks all three.
