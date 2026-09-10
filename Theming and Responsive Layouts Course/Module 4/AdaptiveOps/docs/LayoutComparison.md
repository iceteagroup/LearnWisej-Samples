# Layout comparison — resize code vs containers

Lab deliverable for Module 4. The console keeps its "before" alive on purpose: the second tab of the
workspace (**Resize-code twin (before)**) hosts `Lab/ResizeCodeTwin`, a miniature shell whose five
panels have no Dock and no Anchor and are positioned by a `Resize` handler. The real shell around it
is the "after". Both are resized by the same drag of the browser edge, so the difference is seen,
not read about. The full container tree of the after is in [`ShellComposition.md`](ShellComposition.md).

## Before — `MainPage_Resize` sets Bounds

This is what the twin does on every resize (`Lab/ResizeCodeTwin.cs`), and what the course's starting
shell did for its five real regions:

```csharp
// ✕ six Bounds lines, on the server, after every client resize
private void ResizeCodeTwin_Resize(object sender, EventArgs e)
{
    int w = this.ClientSize.Width, h = this.ClientSize.Height;
    int top = ToolbarHeight + Gap;
    int middle = h - ToolbarHeight - StatusHeight - 2 * Gap;

    this.twinToolbar.Bounds   = new Rectangle(0, 0, w, ToolbarHeight);
    this.twinStatus.Bounds    = new Rectangle(0, h - StatusHeight, w, StatusHeight);
    this.twinRail.Bounds      = new Rectangle(0, top, RailWidth, middle);
    this.twinWorkspace.Bounds = new Rectangle(RailWidth + Gap, top, WorkspaceWidth, middle);   // fixed width!
    this.twinDetails.Bounds   = new Rectangle(w - DetailsWidth, top, DetailsWidth, middle);    // "anchored right" by hand
    // btnTwinSave keeps its designer Location (12, 180): nothing moves it when the panel gets shorter.
}
```

It is correct at the width the developer tested (about 720 px inside the tab). Narrower, the
details panel — pinned to the right edge over a workspace of fixed width — slides over the ticket
grid; shorter, the Save button at a fixed `y` drops below the panel and is clipped. Every fix is one
more coordinate, and the handler runs on the server after the browser has already painted the old
layout, so each resize is a round trip and a visible jump.

## After — five Dock lines, no handler

```csharp
// MainPage.Designer.cs — the console shell, one Dock per region, child order = docking priority
this.toolbarPanel.Dock    = DockStyle.Top;     // 56 px
this.statusPanel.Dock     = DockStyle.Bottom;  // 28 px
this.navigationPanel.Dock = DockStyle.Left;    // 220 px
this.detailsPanel.Dock    = DockStyle.Right;   // 340 px, MinimumSize 260, MaximumSize 480
this.workspacePanel.Dock  = DockStyle.Fill;
this.workspacePanel.MinimumSize = new System.Drawing.Size(320, 240);

// gaps between docked regions come from the container, not from Margin
this.toolbarPanel.Padding    = new Padding(8, 8, 8, 4);
this.navigationPanel.Padding = new Padding(8, 4, 0, 4);
this.workspacePanel.Padding  = new Padding(8, 4, 8, 4);
this.detailsPanel.Padding    = new Padding(0, 4, 8, 4);
this.statusPanel.Padding     = new Padding(8, 0, 8, 4);

this.Controls.Add(this.workspacePanel);   // docked last — Fill takes what is left
this.Controls.Add(this.detailsPanel);
this.Controls.Add(this.navigationPanel);
this.Controls.Add(this.statusPanel);
this.Controls.Add(this.toolbarPanel);     // docked first
```

Then the Resize handler was deleted, not commented out. `MainPage_Resize` still exists in Module 4,
but it contains a single call, `ReportLayout("Page.Resize")`, which writes the region sizes into the
trace and never assigns a size.

## Side by side

| | Resize code (twin, "before") | Containers (shell, "after") |
|---|---|---|
| Code that runs on a browser resize | 6 `Bounds` assignments + arithmetic, on the server | none (the reporting lines only log) |
| Round trips per resize | one, after the client painted the old layout → lag and a visible jump | none for layout; Dock is applied by the client layout engine |
| At 1366 px | correct | correct |
| At 1100 px | details panel slides over the grid (`details overlaps the grid by N px` in the trace) | workspace shrinks by exactly the width lost, nothing overlaps |
| Short window | Save button clipped below the details panel | details fields scroll (AutoScroll, hidden bars); the Bottom-anchored buttons stay in the command bar |
| At 576 px | rail + fixed workspace + details = 724 px wide → workspace and details stacked over each other | rail 220 + details 340 + workspace min 320 > 576 → the page scrolls horizontally on purpose (`Page.AutoScroll`); Module 6 moves regions instead |
| Padding | unknown to the handler; every gap is a hard-coded `+ Gap` | `Padding` on each region panel reduces the `DisplayRectangle` the children dock into |
| Composability | when the details panel becomes a UserControl the handler has to learn its internals | the region docks the UserControl Fill; the UserControl lays itself out |
| MinimumSize / MaximumSize | not honoured unless coded | honoured by the engine (`Animate details` shows 240 → 260 and 560 → 480) |
| Where absolute coordinates remain legitimate | — | inside fixed-height bars (toolbar row, command bar) and inside the scrolling fields panel, always with an Anchor |

## When absolute positions are still the right tool

The lesson does not ban coordinates; it bans them as the mechanism that keeps a *screen* together.
Inside the toolbar card (a 56 px bar), the command bar (48 px) and the scrolling fields panel, the
controls have a `Location` and an `Anchor` because their container has a fixed height or scrolls —
the designer's precision is useful there, and the region around them is what resizes. Module 5
replaces the anchored field grid with a `TableLayoutPanel` and the toolbar row with a
`FlowLayoutPanel`, which express the same intent without twelve anchor settings.

## Evidence

- **Twin tab, browser dragged from 1366 to 1100 px.** Each resize adds `✕ twin Resize #k: 692×288 → 6 Bounds assignments on the server after the client painted · details overlaps the grid by 32 px` to the trace and the red details panel visibly covers the right part of the grid panel; the twin's toolbar label counts the runs. Dragged shorter than about 660 px of browser height the line gains `· Save button clipped` and the Save button disappears below the panel's bottom edge. The resize count keeps climbing while the shell around the twin produced zero layout code.
- **Tickets tab, same drag.** The `← client resize (…)` blocks show `workspace Dock=Fill 522×596` shrinking in step with the browser while `rail … 220×596` and `details … 340×596` keep their widths; the grid keeps its columns, nothing overlaps, and below about 600 px of height the details line turns into `… → scrolls by N px (AutoScroll, ScrollBars=Hidden)`.
- **Timing.** In the twin the panels jump to their new Bounds one round trip after the browser finished resizing (visible as a short lag on every drag step); in the shell the regions move with the browser edge because Dock is evaluated on the client. *(verified pattern from the other course samples: Dock/Anchor are applied client-side, server handlers run after the client event.)*
- **Screenshots** (taken by the learner): `screenshots/module4-before-1100.png` (twin tab, overlap), `screenshots/module4-after-1100.png`, `screenshots/module4-after-576.png` (page scrollbar, nothing overlapping).
