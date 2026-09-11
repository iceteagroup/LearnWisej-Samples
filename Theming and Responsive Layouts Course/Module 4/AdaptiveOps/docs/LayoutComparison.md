# Layout comparison · resize code vs containers

The Module 3 shell kept its five regions together with a `Resize` handler that recomputed `Bounds`. Module 4
replaces it with docked containers. The full container tree of the result is in
[`ShellComposition.md`](ShellComposition.md).

## Before · a Resize handler sets Bounds

```csharp
// ✕ Bounds lines on the server, after every client resize
private void MainPage_Resize(object sender, EventArgs e)
{
    int w = this.ClientSize.Width, h = this.ClientSize.Height;
    int top = ToolbarHeight + Gap;
    int middle = h - ToolbarHeight - StatusHeight - 2 * Gap;

    toolbarPanel.Bounds    = new Rectangle(0, 0, w, ToolbarHeight);
    statusPanel.Bounds     = new Rectangle(0, h - StatusHeight, w, StatusHeight);
    navigationPanel.Bounds = new Rectangle(0, top, RailWidth, middle);
    workspacePanel.Bounds  = new Rectangle(RailWidth + Gap, top, WorkspaceWidth, middle);   // fixed width
    detailsPanel.Bounds    = new Rectangle(w - DetailsWidth, top, DetailsWidth, middle);    // "anchored right" by hand
}
```

It is correct at the width the developer tested. Narrower, the details panel, pinned to the right edge over a
fixed-width workspace, slides over the ticket grid; shorter, a Save button at a fixed `y` drops out of view.
Every fix is one more coordinate, and the handler runs on the server after the browser has painted, so each
resize is a round trip and a visible jump.

## After · five Dock lines, no handler

```csharp
// MainPage.Designer.cs: one Dock per region, child order = docking priority
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

this.Controls.Add(this.workspacePanel);   // docked last: Fill takes what is left
this.Controls.Add(this.detailsPanel);
this.Controls.Add(this.navigationPanel);
this.Controls.Add(this.statusPanel);
this.Controls.Add(this.toolbarPanel);     // docked first
```

The Resize handler was deleted, not commented out. The only resize-related code left is the status bar's
width label, updated from `Application.BrowserSizeChanged`; it never assigns a size.

## Side by side

| | Resize code (before) | Containers (after) |
|---|---|---|
| Code that runs on a browser resize | `Bounds` assignments and arithmetic, on the server | none for layout |
| Round trips per resize | one, after the client painted the old layout | none for layout; Dock is applied by the client layout engine |
| At 1366 px | correct | correct |
| At 1100 px | details panel slides over the grid | the workspace shrinks by exactly the width lost, nothing overlaps |
| Short window | Save button clipped | details fields scroll (AutoScroll, hidden bars); the Bottom-anchored buttons stay in the command bar |
| At 576 px | panels stacked over each other | rail 220 + details 340 + workspace min 320 > 576 → the page scrolls horizontally on purpose; Module 6 moves regions instead |
| Padding | every gap is a hard-coded `+ Gap` | `Padding` on each region panel |
| Composability | the handler has to learn a UserControl's internals | the region docks the UserControl Fill; the UserControl lays itself out |
| MinimumSize / MaximumSize | not honoured unless coded | honoured by the engine |

## When absolute positions are still the right tool

Inside a fixed-height bar (the toolbar card, the editor's command bar) and inside the scrolling fields panel,
controls keep a `Location` and an `Anchor`: their container has a fixed height or scrolls, and the region around
them is what resizes. Module 5 replaces the anchored field grid with a `TableLayoutPanel`.

## Evidence

Screenshots taken by the learner: `screenshots/module4-before-1100.png` (the Module 3 shell, overlap),
`screenshots/module4-after-1100.png`, `screenshots/module4-after-576.png` (page scrollbar, nothing overlapping).
Dragging the browser from 1366 to 1100 px shrinks only the workspace; the rail and the details region keep
their widths and the grid keeps its columns.
