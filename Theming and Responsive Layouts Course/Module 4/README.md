# AdaptiveOps · Theming & Responsive Layouts Course · Module 4

Local lab build for **Module 4 · Layout Fundamentals: Docking, Anchoring, AutoSize, AutoScroll, and
Shell Composition**. It follows the walkthrough video ("From resize code to a nested container
shell"): the Adaptive Operations Console shell is composed from five docked regions in the right
child order — toolbar Top, status Bottom, navigation Left, details Right, workspace Fill — with
`Padding` for the gaps, `MinimumSize` on the workspace and the grid, and no `Resize` handler that
sets `Bounds`. Four regions are UserControls with their own local layout (`Shell/NavigationRail`,
`Shell/DetailsEditor`, `Shell/Workspace`, `Shell/StatusBar`); the details editor scrolls through
`AutoScroll` with hidden scrollbars and anchors its fields and its Save / Cancel buttons. The
"before" — a shell held together by a `Resize` handler — stays alive in a second workspace tab
(`Lab/ResizeCodeTwin`) so the overlap, the clipped Save button and the round-trip lag can be seen
next to the docked shell while the same browser edge is dragged.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine, built from the
Module 1 shell of this course.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 4/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5504
```

Then open <http://localhost:5504>. (Visual Studio: open `AdaptiveOps.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Load the page | – | The trace opens with `• server shell built from nested containers: toolbar Dock=Top 1348×56 · rail Dock=Left 220×596 · workspace Dock=Fill 788×596 min 320×240 · details Dock=Right 340×596 min 260 max 480 · status Dock=Bottom 1348×28`, `• server composition verified …`, the Margin proof (`navigationPanel.Margin=12 but Left=0 … the 8 px gap is Padding`) and the first `← client resize (page load)` block with the rail and details scroll state |
| **Compose shell** | success (default) | `• server composition applied (Compose shell): child order [workspace, details, rail, status, toolbar] → docked Top, Bottom, Left, Right, Fill · …` then `• server composition verified …`; status `● ready` |
| Drag the browser edge (any width, any height) | success | One `← client resize (Application.BrowserSizeChanged)` and one `(Page.Resize)` block per resize listing the five region sizes; only the workspace width changes. Below ≈ 600 px of height the details fields scroll by wheel/touch with no scrollbar (`… → scrolls by N px (AutoScroll, ScrollBars=Hidden)`); below ≈ 480 px the rail scrolls too |
| **Animate details** | progress | A `Wisej.Web.Timer` steps the details region through 16 widths every 350 ms: `→ render step 5/16: details.Width requested 240 → actual 260 (clamped by MinimumSize) · workspace 868×596 · …`; 520 and 560 come back as 480 `(clamped by MaximumSize)`; the workspace follows by Dock, the rail and bars never move |
| **Swap dock order** | failure 1 | The workspace is moved to the end of the child order and docks FIRST: `✕ wrong dock order: workspace moved to child index 4 (docked FIRST) → workspace 1348×680 = the whole page; toolbar covers the metric cards, status bar covers the last grid row …`, `✕ server composition BROKEN: … child order wrong`; on screen the status bar hides the last grid row |
| **Dock+Anchor fight** | failure 2 | `Anchor = Top\|Bottom\|Right` is set on the docked details region; the trace logs `Dock=… Anchor=…` as the framework reports them afterwards and explains the consequence (Dock dropped → the region floats and the workspace runs under it on the next resize; Dock kept → the Anchor is stored but never applied) |
| **Squeeze workspace** | failure 3 | The workspace `MinimumSize` is raised above the width left for it: `✕ MinimumSize violated: … it cannot shrink, so it runs under the details region and the page (AutoScroll) shows a horizontal scrollbar`, plus the browser width at which the real minimum (320) does the same |
| **Restore** | recovery | Child order, docks, widths and MinimumSize restored, seed tickets reloaded, banner cleared: `• server composition applied (Restore) …`, `→ render Restore: 12 tickets …`, `● ready` |
| Edit a ticket, **Save** | success | `• server saved T-1042 …`, `✓ saved HH:mm:ss` next to the buttons, grid and metric cards re-rendered |
| Blank the title, **Save** | failure (validation) | `• server rejected T-1042: Title is required.`, the red banner appears above the tabs (a docked panel became visible — nothing moved by Bounds), hint `✖ Title is required.` |
| Owner = `fault`, **Save** | failure (exception) | `• server threw InvalidOperationException while saving T-1042: Simulated storage fault … — caught around DetailsEditor.Saved`; banner + hint show it, the layout is untouched |
| **Cancel** in the editor | – | The editor restores the ticket from its own copy: `← client cancel: DetailsEditor restored ticket T-1042 …` |
| Tab **Resize-code twin (before)**, then drag the browser | the anti-pattern | Every resize adds `✕ twin Resize #k: 692×288 → 6 Bounds assignments on the server after the client painted · details overlaps the grid by 32 px`; below ≈ 1320 px of browser width the red details panel covers the grid panel, in a short window `· Save button clipped` and the Save button falls below the panel; the docked shell around the tab does neither |
| Click a rail section | – | `← client navigation: NavigationRail.SelectedSection = Reports (the rail's buttons are private to the UserControl)`; the workspace title follows |
| Clear trace | – | Empties the trace list |

The status bar shows `Browser W × H px · Desktop · <composition state>` (composed / dock order
swapped / Dock+Anchor fight / MinimumSize violated), refreshed from `Application.BrowserSizeChanged`
and `Page.Resize`.

## Where things live

```
Module 4/
├─ AdaptiveOps.slnx
└─ AdaptiveOps/
   ├─ MainPage.cs                the shell's behaviour: composition, reporting on resize, lab paths, save round trip (no Bounds)
   ├─ MainPage.Designer.cs       the five regions: Dock, child order, Padding, MinimumSize/MaximumSize, Page.AutoScroll
   ├─ Shell/
   │  ├─ NavigationRail.cs / .Designer.cs   Dock=Left region: 7 buttons Anchor T|L|R in an AutoScroll rail (hidden bars); SelectedSection + SectionChanged
   │  ├─ DetailsEditor.cs / .Designer.cs    Dock=Right region: header (Top) · scrolling fields (Fill, AutoScroll, hidden bars, AutoScrollMargin) · command bar (Bottom, Save/Cancel B|R); Ticket + Saved + Cancelled
   │  ├─ Workspace.cs / .Designer.cs        Dock=Fill region: metric cards, banner, tabbed grid (MinimumSize 300×160), the live trace; hosts the twin
   │  └─ StatusBar.cs / .Designer.cs        Dock=Bottom region: three docked labels; ShowStatus, BrowserText, ThemeText
   ├─ Lab/
   │  └─ ResizeCodeTwin.cs / .Designer.cs   the "before": five panels, no Dock, Bounds set in a Resize handler (deliberately kept)
   ├─ Models/Ticket.cs, TicketRepository.cs the same 12 tickets as every module; owner "fault" is the lab prop that makes Save throw
   ├─ docs/
   │  ├─ ShellComposition.md     the container tree, every Dock/Anchor/AutoSize/AutoScroll/Padding/MinimumSize decision and why, evidence
   │  └─ LayoutComparison.md     resize code vs containers: before/after code, side-by-side table, evidence
   ├─ Program.cs / Startup.cs    Wisej.NET session entry point / Kestrel host
   └─ Default.html / Default.json / Web.config   Bootstrap-4 base theme selected, viewport meta tag
```

## Deliverables

1. **Shell rebuilt from nested containers** with toolbar, navigation, workspace, details and status regions — [`AdaptiveOps/MainPage.Designer.cs`](AdaptiveOps/MainPage.Designer.cs), documented in [`AdaptiveOps/docs/ShellComposition.md`](AdaptiveOps/docs/ShellComposition.md)
2. **All manual Resize code that set Bounds removed** — `MainPage_Resize` only reports; the before/after is in [`AdaptiveOps/docs/LayoutComparison.md`](AdaptiveOps/docs/LayoutComparison.md) (the twin that keeps the smell alive for comparison: [`AdaptiveOps/Lab/ResizeCodeTwin.cs`](AdaptiveOps/Lab/ResizeCodeTwin.cs))
3. **Docking order, Padding and MinimumSize set intentionally on every region** — decision table in `ShellComposition.md`, verified at runtime by `VerifyComposition()`
4. **Details editor scrolling through AutoScroll with hidden scrollbars** — [`AdaptiveOps/Shell/DetailsEditor.Designer.cs`](AdaptiveOps/Shell/DetailsEditor.Designer.cs) (`fieldsPanel`)
5. **Navigation and details regions extracted into UserControls** — [`AdaptiveOps/Shell/NavigationRail.cs`](AdaptiveOps/Shell/NavigationRail.cs), [`AdaptiveOps/Shell/DetailsEditor.cs`](AdaptiveOps/Shell/DetailsEditor.cs) (plus `Workspace` and `StatusBar`)

## Lab step → code map

| Lab step | Where |
|---|---|
| 1. Run the Module 3 shell, drag below 1100 px, note what clips or overlaps | The **Resize-code twin (before)** tab in the workspace reproduces it: `Lab/ResizeCodeTwin.cs` (`ResizeCodeTwin_Resize`), trace lines `✕ twin Resize #k …` |
| 2. Lab goal: nested containers, no Bounds, dock order, Padding, MinimumSize, AutoScroll | `MainPage.Designer.cs` (regions), `Shell/*.Designer.cs` (local layouts), `docs/ShellComposition.md` |
| 3. Find every Resize/Layout/load-time method that sets Bounds, list the five regions and their edges, delete the code | Section 1 of `docs/ShellComposition.md`; `MainPage.cs` has no such method — `MainPage_Resize` calls `ReportLayout` only |
| 4. Dock the five panels, fix the child order with BringToFront / SendToBack | `MainPage.Designer.cs` — the five `Controls.Add` lines at the end of `InitializeComponent()`; `ComposeShell()` re-applies the order with `SetChildIndex`; **Swap dock order** shows the wrong order |
| 5. Padding for the gaps (no Margin), MinimumSize on the workspace and the grid | `Padding` on every region panel; `navigationPanel.Margin = 12` as the proof it is ignored (logged in `MainPage_Load`); `workspacePanel.MinimumSize = 320×240`, `gridTickets.MinimumSize = 300×160` (`Shell/Workspace.Designer.cs`) |
| 6. Anchor labels T\|L, editors L\|R, notes T\|L\|R, buttons B\|R; AutoScroll, ScrollBars.Hidden, AutoScrollMargin | `Shell/DetailsEditor.Designer.cs` — `fieldsPanel` (AutoScroll, `ScrollBars.Hidden`, `AutoScrollMargin (0,24)`), the anchored captions/editors, `commandBar` with `btnSave` / `btnCancel` Bottom\|Right |
| 7. Extract NavigationRail and DetailsEditor as UserControls with a Ticket property, a Saved event and a SelectedSection; keep children private | `Shell/NavigationRail.cs` (`SelectedSection`, `SectionChanged`), `Shell/DetailsEditor.cs` (`Ticket`, `Saved`, `Cancelled`); `MainPage.cs` references no child control of either |
| 8. Wire btnSave_Click: validate, raise Saved, report the outcome, try/catch around the save | `DetailsEditor.btnSave_Click` raises `Saved`; `MainPage.detailsEditor_Saved` saves in try/catch (`TicketValidationException` and `Exception`), sets `e.Error`; the editor shows `✓ saved` / `✖ …`; owner `fault` triggers the throw |
| 9. Show every path: resize 1366 → 576 and back, scroll the editor, save valid / invalid / throwing | The **What to try** table above; `ReportLayout` logs the region sizes and scroll state after every resize |
| 10. Review against the smell list, before/after screenshots, note on docking order, Padding and MinimumSize | Section 6 (smell list) and 3 (decisions) of `docs/ShellComposition.md`; screenshots named in its Evidence section |

In Visual Studio the child order is what **BringToFront** / **SendToBack** change in the designer,
and the UserControls open in the Wisej designer because their layout lives in `InitializeComponent()`.

## Self-check answers (lab guide)

- **If the workspace panel were docked before the status panel in the child order, what would the user see at the bottom of the grid, and which designer command fixes it without changing a single size?**
  The workspace would be docked first and, being `DockStyle.Fill`, would take the whole page; the
  status panel, docked afterwards, would be laid out over the workspace's bottom strip, so the user
  would see the status bar covering the last row of the ticket grid (and the toolbar covering the top
  of the metric cards). The fix is a child-order change, not a size change: **SendToBack** on the
  workspace (or **BringToFront** on the status panel and the other edge regions) so the edges are
  claimed first and Fill takes what is left. The **Swap dock order** button reproduces the wrong
  order at runtime and `ComposeShell()` fixes it with `Controls.SetChildIndex(workspacePanel, 0)`.
- **You set a 12-pixel Margin on the navigation rail and the gap did not appear. Why not, and which two mechanisms do produce a gap between docked regions?**
  Docked children are placed inside the parent's `DisplayRectangle`, and the default layout engine
  does not use `Margin` for docked controls — Margin is honoured by the Flow, Table and Flex engines,
  not by docking. `navigationPanel.Margin` is 12 in this sample and the rail still starts at
  `Left = 0`; `MainPage_Load` logs it. The two mechanisms that do produce a gap are **Padding on the
  container** (the parent's Padding shrinks the DisplayRectangle the children dock into; every region
  panel here carries its gap as Padding) and **a thin docked spacer control** (a Panel docked on the
  same edge before the region, whose only job is to occupy the gap).
- **The details editor is a UserControl with AutoScroll. Which of its members must the shell know about to move it under the grid on a tablet, and which must it never need?**
  It must know the members the region exposes: the `Ticket` property to load and read the ticket, the
  `Saved` event (and `Cancelled`) to react to the user, and the control-level layout properties every
  control has — `Dock`, `Parent`/`Controls.Add`, `Visible`, `Height` or `MinimumSize` — to dock it
  Bottom under the grid instead of Right, or to place it in a Form on a phone. It must never need a
  child control: not `txtTitle`, `txtNotes`, the captions, the scrolling `fieldsPanel`, its
  `AutoScroll` / `ScrollBars` settings or the Save / Cancel buttons. Those stay private, which is why
  a rename inside the editor never breaks the page and why the anchors and the scrolling keep working
  wherever the region is docked.
- **Knowledge check — what affects docking priority in a parent container?**
  The order of the controls in the parent's `Controls` collection (the child index / z-order): the
  control added last is docked first, and BringToFront / SendToBack change it.
- **Knowledge check — what is a warning sign when using AutoSize?**
  A container that auto-sizes based on children that are docked or anchored back to that same
  container: the parent asks the children for its size while the children take their size from the
  parent, and the engine has no stable answer. Keep AutoSize on leaf controls (captions, badges) and
  on containers whose children have fixed or content-driven sizes; none of the containers in this
  shell has AutoSize.
