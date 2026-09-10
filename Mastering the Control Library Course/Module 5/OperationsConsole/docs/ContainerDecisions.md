# ContainerDecisions.md — why these containers and these UserControls

The lab's closing task: *"write a short note explaining the container and UserControl decisions"*. This is that
note. `docs/LayoutNotes.md` says **what** is docked and anchored; this file says **why**, and what was rejected.

---

## 1. Container decisions

| Region | Chosen | Rejected alternative | Why |
|---|---|---|---|
| The screen itself | `Page` | `Form`, `Desktop` | The Operations Console is a web-style application entry surface: it fills the browser and has no window chrome. A `Form` is for dialog-like windows and modal workflows; a `Desktop` only earns its keep when the application deliberately preserves a desktop metaphor with several windows. |
| Frequent commands | `ToolBar` (New, Refresh, Save) | `MenuBar`, `RibbonBar` | A ToolBar is exactly "the actions people use all day". A MenuBar suits a traditional, deep application menu; a RibbonBar groups commands by task area and is overkill for a console with a dozen commands. |
| Commands tied to the selection | `ContextMenu` on `navList` | more ToolBar buttons | Refresh / New / Save on the navigation list act on *that* section. A ContextMenu is the command surface for "this object", and the items call the very same command methods the ToolBar calls — one command model, three surfaces. |
| Status | `StatusBar` with six named panels | a docked `Panel` of `Label`s (Module 1) | The StatusBar is a real control with panels, sizing and theming, and it is not a dumping ground: it shows the active profile, the record count, the selected record, the control used last and the last refresh — the status the module's reading asks for. |
| Work area | `SplitContainer splitMain` | a `Panel` docked Left + a `Panel` docked Fill | The user, not the developer, should decide how much room the navigation gets. A SplitContainer gives a draggable splitter, `Panel1MinSize`, and `Panel1Collapsed` for the narrow profile — three behaviours that would otherwise be hand-written. |
| Navigation | `ListBox navList` in `Panel1` | six `Button`s (Module 1), `Accordion`, `TreeView` | The sections are a flat, ordered list of peers: a ListBox is the smallest native control that expresses that, gives keyboard navigation for free and hosts a `ContextMenu`. An Accordion would imply stacked, collapsible groups; a TreeView would imply a hierarchy that does not exist. |
| Detail surface | `TabControl tabDetail` in `Panel2` | one content panel + `Navigate()` (Module 1) | The six sections are **peer** views — Editors, Layouts, Lists and Trees, DataGridView, Dashboard, Widgets — not steps of a workflow, and each one keeps its state while the user looks at another. That is exactly what tab pages are for. No required field lives behind a tab. |
| Section content | `TableLayoutPanel tblDemo` | absolute positions | Two columns of demos that must survive any splitter position. A table gives percentage columns and rows; coordinates would need recomputing on every resize. |
| Aligned form | `TableLayoutPanel tblForm` | `Label` + `TextBox` at fixed points | Fixed label column, editor column at `Percent 100`: the labels stay aligned and the editors take the change. |
| Card collection | `FlowLayoutPanel flowChips` | a `Panel` with computed positions | The number of cards changes at runtime (**Add card** / **New**). A flow panel wraps them; a positioned panel would clip them. |
| Anchor demo | `Panel pnlAnchorDemo` with anchored children | another layout panel | Anchoring is the point of that box: each child says what its `Anchor` value does, and the splitter is the resize handle that proves it. |

Two things were deliberately **not** done:

- **No `AutoSize` on containers.** Auto-sizing a large container or a grid makes the layout engine measure
  everything on every resize and every profile change — the pitfall the module's reading calls out.
- **No second, narrow screen.** The workflow does not change on a phone; only how much room the navigation gets.
  A separate screen would have to be maintained twice and would drift.

## 2. UserControl decisions

The repeated UI in this console was the **record header** (title + record count + last refresh + a Refresh
button) and the **status strip** under it. Copied onto six section pages, they were six places to change and six
chances to drift — the video shows the copy that already says `lblHeader` instead of `lblTitle`.

Both now live in `Shell/` as UserControls with the **same, small public surface**:

```csharp
public string    Title          { get; set; }   // what the page is showing
public int       RecordCount    { get; set; }   // how many records it holds
public DateTime? LastRefresh    { get; set; }   // when it was last refreshed (null = never)
public event EventHandler RefreshRequested;     // the user asked for a refresh
```

- `RecordHeader` renders that as a white card with four positioned controls.
- `StatusStrip` renders the *same three values* as one composed monospace line with a small `↻` button.
- Neither exposes a child control. `lblTitle`, `lblCount`, `lblRefreshed`, `btnRefresh`, `lblStrip`,
  `btnStripRefresh` are all `private`.

What that buys, concretely:

- `Sections/LayoutsPage` sets three properties on each control (`UpdateReusableControls()`) and handles one event
  per control. It never asks which button was clicked, because there is no button in the surface.
- Every private child can be renamed, restyled or replaced tomorrow and no consuming page changes. The three
  properties and the event cannot — that is the contract, and that is the answer to the lab's review question
  *"which members could you rename without touching any consuming page?"*
- The two controls prove the boundary is real: identical public surface, completely different internals.

`Shell/ISectionRecords.cs` is the same idea one level up. `MainPage` needs a record count for the StatusBar and
something for **New** and **Save** to act on, but it must not know that `LayoutsPage` exists. So a section that
holds records implements a three-member interface; a section that does not is not broken — the shell answers
"— records" and "nothing to save", which is exactly what the Module 2 / 4–7 placeholders do until their own
module fills them in.

## 3. Evidence

| Decision | What the running app shows |
|---|---|
| SplitContainer over a docked Panel | Drag the splitter: the navigation resizes and stops at `Panel1MinSize` (180 px). **Force narrow** collapses it with one property. |
| TabControl for peer sections | All six tabs are reachable in any order; switching away and back keeps the Layouts selection, the card list and the narrow toggle. |
| ContextMenu = the same commands | Right-click the navigation list → Refresh / New / Save produce the same Event log lines and the same StatusBar text as the ToolBar buttons. |
| StatusBar shows real status | `Desktop profile · Desktop`, `6 records`, `Record: LAY-003`, `Control: btnSave`, `Refreshed: 09:41:12` — all of them change as you work. |
| FlowLayoutPanel wraps | Press **Add card** a few times, then drag the splitter left: the chips rewrap, none is clipped. |
| TableLayoutPanel keeps labels aligned | Same drag: the captions stay put, the read-only editors change width. |
| Anchoring inside a region | `Top | Left | Right` stretches, `Top | Right` keeps its width, `Bottom | Right` stays in the corner, the unanchored label does not move. |
| One UserControl, two consumers of the same event | Press **Refresh** in the card header *or* `↻` in the strip: the same `RefreshSection()` runs, and the log names which control asked. |
| No child control is exposed | The Layouts page only ever assigns `Title`, `RecordCount`, `LastRefresh` — grep `recordHeader.` in `Sections/LayoutsPage.cs`. |
