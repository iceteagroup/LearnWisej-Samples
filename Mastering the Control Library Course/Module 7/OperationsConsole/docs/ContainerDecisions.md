# ContainerDecisions.md — why these containers and these UserControls

The lab's closing note: *"a short note explaining the container and UserControl decisions"*. `docs/LayoutNotes.md`
says **what** is docked and anchored; this file says **why**, and what was rejected.

## 1. Container decisions

| Region | Chosen | Rejected alternative | Why |
|---|---|---|---|
| The screen itself | `Page` | `Form`, `Desktop` | The console fills the browser and has no window chrome; a `Form` is for dialog-like windows. |
| Frequent commands | `ToolBar` (New, Refresh, Save) | `MenuBar`, `RibbonBar` | A ToolBar is "the actions people use all day"; a RibbonBar is overkill for a dozen commands. |
| Commands tied to the selection | `ContextMenu` on `navList` | more ToolBar buttons | Refresh / New / Save on the navigation list call the very same command methods as the ToolBar — one command model, several surfaces. |
| Status | `StatusBar` with named panels | a docked `Panel` of `Label`s (Module 1) | A real control with panels and sizing: active profile, record count, selected record, control used last, last refresh. |
| Work area | `SplitContainer splitMain` | a `Panel` docked Left + a `Panel` docked Fill | The user decides how much room the navigation gets; `Panel1MinSize` and `Panel1Collapsed` come for free. |
| Navigation | `ListBox navList` in `Panel1` | six `Button`s (Module 1), `Accordion`, `TreeView` | The sections are a flat, ordered list of peers; a ListBox expresses that and hosts a `ContextMenu`. |
| Detail surface | `TabControl tabDetail` in `Panel2` | one content panel + `Navigate()` (Module 1) | The six sections are peer views, and each keeps its state while the user looks at another. |
| Section content | `TableLayoutPanel tblDemo` | absolute positions | Two columns that must survive any splitter position. |
| Aligned form | `TableLayoutPanel tblForm` | `Label` + `TextBox` at fixed points | Fixed label column, editor column at `Percent 100`: labels stay aligned, editors take the change. |
| Card collection | `FlowLayoutPanel flowChips` | a `Panel` with computed positions | The number of cards changes at runtime (**New**); a flow panel wraps them. |

Two things were deliberately **not** done: no `AutoSize` on containers, and no second, narrow screen.

## 2. UserControl decisions

The repeated UI was the **record header** (title + record count + last refresh + Refresh button) and the **status
strip** under it. Both now live in `Shell/` as UserControls with the same small public surface:

```csharp
public string    Title          { get; set; }
public int       RecordCount    { get; set; }
public DateTime? LastRefresh    { get; set; }
public event EventHandler RefreshRequested;
```

- `RecordHeader` renders it as a white card with four positioned controls.
- `StatusStrip` renders the same values as one line with a small `↻` button.
- Neither exposes a child control: `lblTitle`, `lblCount`, `lblRefreshed`, `btnRefresh`, `lblStrip`,
  `btnStripRefresh` are all `private`, so they can be renamed or replaced without any consuming page noticing.

`Shell/ISectionRecords.cs` is the same idea one level up: `MainPage` needs a record count and something for **New**
and **Save** to act on, but must not know that `LayoutsPage` exists.

## 3. Evidence

| Decision | What the running app shows |
|---|---|
| SplitContainer over a docked Panel | Drag the splitter: the navigation resizes and stops at 180 px. A narrow browser collapses it with one property. |
| TabControl for peer sections | All six tabs are reachable in any order; switching away and back keeps the Layouts selection. |
| ContextMenu = the same commands | Right-click the navigation list → Refresh / New / Save give the same StatusBar text as the ToolBar buttons. |
| StatusBar shows real status | `Desktop profile · Desktop`, `6 records`, `Record: LAY-003`, `Control: btnSave`, `Refreshed: 09:41:12`. |
| FlowLayoutPanel wraps | Press **New** a few times, then drag the splitter left: the chips rewrap, none is clipped. |
| One UserControl surface, two consumers | **Refresh** in the header or `↻` in the strip runs the same `RefreshSection()`. |
