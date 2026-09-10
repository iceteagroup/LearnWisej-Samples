# ExplorerNotes.md — Operations Console, Module 4

The control decisions behind the **document explorer** (`Sections/ListsTreesPage`), the identity rule it
enforces, the large-data strategy it uses, and what breaks first at ten times the volume.

This is the third page of the team's *control selection guide* (after `ControlSelection.md` from Module 1 and
`LayoutNotes.md` from Module 3). Same rule as everywhere: **choose the simplest native control that expresses
the user's task**.

## 1. Data shape → control

The first design question is not "which control looks nice", it is **what shape is the data**. Each shape
names its control, and the explorer holds three shapes at once.

| Part of the screen | Shape of the data | Control chosen | Why | What was considered and rejected |
|---|---|---|---|---|
| `categoryTree` | **Hierarchical** — customers own folders, folders may own folders | `TreeView` with lazy loading | Expansion, parent/child context and "where am I" are the task. The user's map. | **`ComboBox`** — flattens the hierarchy into a drop-down and loses the parent context; a scrollbar is not search. **`ListBox`** — same, without even the compact footprint. |
| `documentList` | **Flat collection, presentation matters** — name, type, size, modified | `ListView` (`View = Details`) in virtual mode | File-like content: the same collection can switch to `Tile` / `LargeIcon` without rebuilding anything, and virtual mode caps the cost of 312 rows at the rows on screen. | **`DataRepeater`** — a card per document would be prettier and would create a set of server controls plus browser widgets *per row*; at 312 rows that is thousands of widgets for four short columns. **`DataGridView`** — the right answer once the rows are editable, sortable and bindable (Module 5 does exactly that); here nothing is edited. |
| `documentDetail` | **One object, a set of properties** | `DocumentDetailControl : UserControl` with one `Show(DocumentModel)` | The detail is a designed screen with business labels and one public method; it opens in the designer and can be reused by the capstone. | **`PropertyGrid`** — generic inspection, zero layout work, but it exposes property names and implementation details to an end user and gives no control over wording or grouping. Right for an admin or developer tool, wrong here. |

The rules from the reading, in the order the explorer applies them: **ComboBox** for compact selection,
**ListView** when presentation matters, **TreeView** for hierarchy, **DataRepeater** for cards,
**PropertyGrid** for inspection — and **virtual mode, lazy loading and stable IDs before anything gets slow**.

## 2. The identity rule

> The same label appears under many branches. Display text is for people; identity is for code.

`DocumentService` deliberately ships three folders called **Contracts** — category **7** under Contoso,
**10** under Fabrikam, **13** under Northwind — and two empty categories, and one top-level branch
(`Archive 2019-2023`) that expands to nothing.

- Every `TreeNode` gets `Tag = categoryId` and `Name = "cat-7"` (`CreateCategoryNode`).
- Every `ListViewItem` gets `Tag = documentId` and `Name = "DOC-000312"` (`documentList_RetrieveVirtualItem`).
- The placeholder node (`Name = "placeholder"`) and the error node (`Name = "load-error"`) carry **no `Tag`**,
  so `categoryTree_AfterSelect` can tell "the user picked a category" from "the user picked scaffolding" by
  a type check (`node.Tag is int`) instead of by comparing text.
- No handler in `ListsTreesPage.cs` reads `Node.Text` or `Item.Text` to decide anything. Text is used only to
  write log lines and status messages, where it is addressed at a human.

The detail card prints the category path **and** the category ID (`Contoso ▸ Contracts (category 7)`), so the
difference between the three "Contracts" folders is visible on screen, not just in the code.

## 3. The large-data strategy

Two different strategies, one per control, because they solve different problems.

**Lazy loading (the tree).** `LoadTopCategories()` fetches only the six top-level categories. Each node gets
one child named `placeholder` with the text "Loading…", which is what makes the expand glyph appear before any
children exist. `categoryTree_AfterExpand` → `LoadChildren(node)` finds the placeholder, clears the branch and
calls `DocumentService.GetChildren(Tag)`. A branch is loaded at most once (`NeedsLoading` returns false
afterwards), and a failed expand puts the placeholder **back** next to a visible
`Could not load — expand again to retry` node, so collapsing and expanding again is the retry.

**Virtual mode (the list).** `documentList.VirtualMode = true`. On `AfterSelect` the page asks the service for
a `DocumentPage` — the category's rows plus `Total` — keeps it in the `_page` field, and sets
`documentList.VirtualListSize = page.Total`. Nothing else happens. The browser then asks for the rows it can
actually paint, `documentList_RetrieveVirtualItem` builds one `ListViewItem` per request out of `_page.Items`
(setting `Tag` and `ImageKey` at the same moment) and increments a counter. Selecting Contoso ▸ Contracts
therefore reports something like **`312 rows · 8 items created`** in the page header: 312 is what the control
believes, 8 is what it cost. `documentList_CacheVirtualItems` logs the range the control is about to show —
against a real database that handler is where the next block would be fetched.

**Why `_page` and not a query per row.** `RetrieveVirtualItem` runs inside rendering. Anything expensive there
multiplies by the number of visible rows and runs again on every scroll. The service is called **once per
category**, into a server-side page; the handler is an array index.

**Clearing.** In virtual mode `Items` is not the store, so the list is emptied with `VirtualListSize = 0`
(`ClearList`) — and that happens **before** every service call, so a failure cannot leave half-built rows
behind. An empty category sets `VirtualListSize = 0` too and shows the amber
`No documents in this category` band.

**Icons.** One `ImageList` (`imageList`, 16×16) is shared by `categoryTree.ImageList` and
`documentList.SmallImageList`, with the keys `folder`, `contract`, `invoice`, `drawing`, `warning`. The five
SVG files live in `wwwroot/icons/` and are registered with the two-string overload
`imageList.Images.Add(key, "wwwroot/icons/<key>.svg")`, whose second argument is a theme icon **name or a
URL** — static files are served from the project folder, so the browser fetches `/wwwroot/icons/folder.svg`.
The alternative is to pass built-in theme icon names (`"icon-folder"`, `"icon-warning"`, …): they follow the
active theme for free, but you get whatever the theme ships. Own files were chosen because these five keys are
business meanings, not UI affordances. A flagged document (`IsWarning`) shows the `warning` key **instead of**
its type key, so the tree and the list report trouble the same way.

## 4. Selection is an event, not a query

```
control  →  page (ListsTreesPage)  →  service (DocumentService)  →  detail (DocumentDetailControl)
```

Both selection handlers do the same three things and nothing else:

1. resolve the stable ID — `(int)e.Node.Tag` for the tree, `_page.Items[documentList.SelectedIndices[0]].Id`
   for the list (in virtual mode the ID comes from the cached page, never from `Items`);
2. call `DocumentService` inside `try` / `catch`, with a loading state (`documentList.ShowLoader = true`,
   `documentDetail.ShowLoader = true`) that is visible because the service costs ~300 ms;
3. hand the finished model to `documentDetail.Show(model)` and record it with `ConsoleLog.Record("DOC-000312")`.

`DocumentDetailControl` has exactly two public methods — `Show(DocumentModel)` and `ShowMessage(string)` — and
no reference to the tree, the list, the page or the service. Controls display view state, services retrieve
business data, the screen coordinates the workflow.

**What is unit-testable without a browser** (the exam question): the ID resolution (`node.Tag is int` →
`(int)node.Tag`, `page.Items[index].Id`), everything in `DocumentService`, and the mapping from
`DocumentModel` to display strings (`DocumentId`, `SizeText`, `ModifiedText`, `StatusText`) — those are plain
properties on the model. What is **not** testable that way, and belongs in the UserControl, is which Label
receives which string and what colour `lblStatus` turns.

## 5. What breaks first at 10× — and which strategy fixes it

Today: 6 top-level categories, 12 sub-categories, 557 documents, the largest folder 312 rows. At 10× that is
~180 categories and ~5 600 documents, largest folder ~3 100.

| Order | What breaks | Why | The fix, in the order you should reach for it |
|---|---|---|---|
| 1 | **The `DocumentPage` itself** — `GetDocumentPage(7)` returns all 3 120 `DocumentSummary` objects for one category | Virtual mode caps what the *browser* renders, not what the *server* materialises. The page is the largest allocation on the screen and it is rebuilt on every category click. | Turn the page into a real window: `Skip/Take` plus an index-keyed cache, filled from `CacheVirtualItems(e.StartIndex, e.EndIndex)`. `VirtualListSize` still comes from a `COUNT`. This is the one change that actually matters. |
| 2 | **Scrolling feels steppy** — every new window of rows is a server round trip while the user drags the scrollbar | `RetrieveVirtualItem` is cheap, but a cache miss is not. | `documentList.PrefetchItems` (currently 0): pre-render rows outside the visible area so the next few scroll steps are already there. Cosmetic-but-real; it does not reduce the data, it hides the latency. |
| 3 | **An expanded branch with ~180 sibling categories** renders slowly in the browser | Lazy loading already stops the *fetch*; what remains is the DOM cost of the nodes of one open branch. | `categoryTree.VirtualScroll = true` (plus `TreeView.PrefetchItems`): only the visible nodes are rendered. Note the constraint — with `VirtualScroll` on, all nodes must be the same height, so the richer HTML node rendering is off. |
| 4 | **Nothing, for a while** — startup and the first expand stay flat | Lazy loading means the top level costs six rows whatever the catalogue holds. | Nothing to do. This is the strategy that already paid. |

The order is the point: **lazy loading** stops you fetching what nobody asked for, **virtual mode** stops you
creating items nobody can see, **virtual scrolling** stops the browser rendering nodes nobody can see, and
**prefetching** only smooths what is left. Reaching for prefetching first buys nothing.

What would *not* fix it: switching the list to a `DataRepeater` (more widgets per row, not fewer), or moving
the categories into a `ComboBox` (fewer widgets, but the user loses the map and you still load everything).

## 6. Feedback and the four paths

| Path | Trigger | What the user gets |
|---|---|---|
| success | **Reload tree**, expanding a node, selecting a category, selecting a document | green status, an Event log line per service call, the detail card fills |
| progress | **Expand all top level** (`Wisej.Web.Timer`, one node per 500 ms tick), and `ShowLoader` during every ~300 ms service call | the branches open one after the other, each one lazy-loading; the list and the detail card are covered by the loader while the service runs |
| empty | a category with no documents (Shared templates ▸ Contract templates, Compliance ▸ Certificates), or a branch with no children (Archive 2019-2023) | `VirtualListSize = 0`, the amber **No documents in this category** band, amber status, `documentDetail.ShowMessage(...)` |
| failure | **Simulate failure** ticked, then expand / select / pick a document | red status, an `AlertBox` top-right in plain words, a friendly sentence in the detail card, a `warning` node in the tree — and the exception type and message in the **Event log only** |
| recovery | **Retry**, or expanding the failed node again | the last action runs again and succeeds (the simulation switch is one-shot and clears itself) |

Exception text is never shown to the user. `ReportFailure` is the single place that decides this.

## Evidence (what the running app shows)

- **Startup** — the Lists and Trees section opens with six top-level nodes, all collapsed, all with a folder
  icon. Event log: `imageList ← 5 keys (…)`, `categoryTree ← DocumentService.GetTopCategories() — top level only, children load on expand`,
  `DocumentService.GetTopCategories() → 6 categories (top level only)`. Status (green):
  `6 top-level categories · 557 documents in the store, none loaded yet.` Header: `— rows · 0 items created`.
- **Expand Contoso** — only Contoso's children load. Event log:
  `categoryTree.AfterExpand "Contoso" → placeholder found, DocumentService.GetChildren(1)` then
  `DocumentService.GetChildren(1) → 3 child categories`. Fabrikam and Northwind stay collapsed and unfetched.
- **Expand Archive 2019-2023** — the placeholder disappears, no children arrive, the node becomes a leaf;
  amber status `"Archive 2019-2023" has no sub-categories.`
- **Select Contoso ▸ Contracts** — the list shows the loader for ~300 ms, then
  `DocumentService.GetDocumentPage(7) → 312 rows`, `documentList.VirtualListSize = 312 — items are created only when the browser asks for them`,
  `documentList.CacheVirtualItems → rows 0..N`, and the header reads **`312 rows · 8 items created`** (the
  exact right-hand number is however many rows fit the viewport). Diagnostics: `Control: categoryTree`.
- **Select Fabrikam ▸ Contracts** — same folder text, different data: `DocumentService.GetDocumentPage(10) → 36 rows`.
  The proof that the handler keys on `Tag`.
- **Pick a document** — `documentList.SelectedIndexChanged → document id 312 (from the cached page + SelectedIndices[0], not from Items)`,
  then `DocumentService.GetDocument(312) → DOC-000312 (Contoso ▸ Contracts)`. The detail card fills through
  the service, the shell's `Record:` shows `DOC-000312`. A flagged document (every 37th) shows the red
  `Retention review overdue` status and the `warning` icon in the list.
- **Select Shared templates ▸ Contract templates** — `→ 0 rows`, `VirtualListSize = 0`, amber band
  **No documents in this category**, detail card: `No documents in this category.`
- **Simulate failure + expand** — the branch shows `Could not load — expand again to retry` with the warning
  icon and collapses; red status, an `AlertBox`, `✗ InvalidOperationException — Simulated failure in DocumentService.GetChildren(2) …`
  in the log only, and `DocumentService.SimulateFailure = false (one-shot) — Retry will now succeed`.
- **Simulate failure + select a category** — the list is already at `VirtualListSize = 0` when the call throws,
  so no half-built rows survive; the detail card reads `The documents of this category could not be loaded.`
- **Retry** — `btnRetry → running the last action again`, the same call succeeds, status green.
- **Expand all top level** — the six branches open one per ~500 ms, each with its own
  `AfterExpand … GetChildren(n)` pair in the log; the button re-enables when the queue empties.
