# OperationsConsole · Mastering the Control Library · Module 4

Local lab build for **Module 4 · Lists, Trees, Repeaters, and Hierarchical Data**. It replaces the placeholder
body of the **Lists and Trees** section with the **document explorer** the lab asks for: a `TreeView` of
categories that loads **only the top level** and fetches each branch in `AfterExpand`, a `ListView` in
**virtual mode** whose `VirtualListSize` comes from a server-side `DocumentPage` and whose items are built one
at a time in `RetrieveVirtualItem`, one `ImageList` shared by both controls, and a `DocumentDetailControl`
UserControl that receives a finished `DocumentModel` and knows nothing about the tree, the list or the
service.

The course is cumulative: the shell (`MainPage`, the four docked areas, the Event log card and the
`Shell/` contract — `IConsoleShell`, `ConsoleLog`, `ISection`) and the earlier sections come from the previous
modules and are untouched here. This module adds `Services/DocumentService.cs`, four models, the
`ListsTrees/` folder, `wwwroot/icons/` and `docs/ExplorerNotes.md`.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 4/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5704
```

Then open <http://localhost:5704> and click **Lists and Trees** in the navigation. (Visual Studio: open
`OperationsConsole.slnx` in this folder, press F5.)

Run from the **project folder** — the static file server serves that folder, which is how
`wwwroot/icons/*.svg` reaches the browser as `/wwwroot/icons/folder.svg`.

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## The data you are looking at

`DocumentService` generates **557 documents** in **6 top-level categories** and **12 sub-categories**, in
memory, with a fixed seed. It is deliberately awkward:

- **three folders are called "Contracts"** — category **7** (Contoso), **10** (Fabrikam), **13** (Northwind).
  Only the ID tells them apart, which is the point of the module.
- Contoso ▸ Contracts holds **312 documents** (the number in the walkthrough video).
- two categories are **empty** (Shared templates ▸ Contract templates, Compliance ▸ Certificates) and one
  top-level branch (**Archive 2019-2023**) expands to **nothing at all**.
- every 37th document is **flagged**, so it shows the `warning` icon instead of its type icon.
- every call costs ~300 ms (`Task.Delay` / `Thread.Sleep`) so the loading state is visible, and every call is
  written to the Event log (`DocumentService.GetDocumentPage(7) → 312 rows`).

## What to try

| Action | Path | What you should see |
|---|---|---|
| Open **Lists and Trees** | ready state | six collapsed folder nodes; header `— rows · 0 items created`; green status `6 top-level categories · 557 documents in the store, none loaded yet.`; log `imageList ← 5 keys (…)`, `categoryTree ← DocumentService.GetTopCategories() — top level only, children load on expand`, `DocumentService.GetTopCategories() → 6 categories (top level only)` |
| Expand **Contoso** | success · lazy loading | only Contoso's children load: log `categoryTree.AfterExpand "Contoso" → placeholder found, DocumentService.GetChildren(1)` then `DocumentService.GetChildren(1) → 3 child categories`. Fabrikam and Northwind are still unfetched |
| Expand **Archive 2019-2023** | empty branch | the placeholder disappears and no children arrive — the node becomes a leaf; amber status `"Archive 2019-2023" has no sub-categories.` |
| Select **Contoso ▸ Contracts** | success · virtual mode | the list shows the loader for ~300 ms, then `DocumentService.GetDocumentPage(7) → 312 rows`, `documentList.VirtualListSize = 312 — items are created only when the browser asks for them`, `documentList.CacheVirtualItems → rows 0..N`; the header reads **`312 rows · 8 items created`** and the **CREATED ITEMS** box shows the same number (it is however many rows fit your viewport) |
| Scroll the list | progress · virtual scroll | more `CacheVirtualItems` ranges in the log and the created-items counter creeps up — it never reaches 312 |
| Select **Fabrikam ▸ Contracts** | identity | same folder text, different data: `DocumentService.GetDocumentPage(10) → 36 rows`. Proof the handler keys on `Tag`, not on `Text` |
| Pick a document | success · detail | `documentList.SelectedIndexChanged → document id 312 (from the cached page + SelectedIndices[0], not from Items)`, then `DocumentService.GetDocument(312) → DOC-000312 (Contoso ▸ Contracts)`; the detail card fills, the shell's `Record:` shows `DOC-000312`. A flagged document turns the status red (`Retention review overdue`) and carries the warning icon |
| Select **Shared templates ▸ Contract templates** | empty category | `→ 0 rows`, `VirtualListSize = 0`, the amber band **No documents in this category**, amber status, and the detail card says the same in one sentence |
| **Reload tree** | success | the tree is rebuilt top level only; the list and the detail card go back to their empty state |
| **Expand all top level** | progress | a `Wisej.Web.Timer` expands one node per 500 ms; each expand runs `AfterExpand` and lazy-loads that branch (one `GetChildren(n)` pair per node in the log). The button re-enables when the queue empties |
| Tick **Simulate failure**, then expand a node | failure | the branch shows `Could not load — expand again to retry` with the warning icon and collapses; red status, an `AlertBox` top-right in plain words, `✗ InvalidOperationException — …` in the **Event log only**; the checkbox unticks itself (`SimulateFailure = false (one-shot)`) |
| Tick **Simulate failure**, then select a category | failure | the list was already cleared to `VirtualListSize = 0` before the call, so **no half-built items survive**; the detail card reads `The documents of this category could not be loaded.` |
| Expand the failed node again, or click **Retry** | recovery | the placeholder is still there, so `AfterExpand` fires again and the branch loads; **Retry** re-runs whatever failed last (reload tree / expand node / load page / load document) and the status turns green |
| **Refresh** in the shell command bar | command | `ListsTreesPage.RefreshSection()` reloads the top level; open branches are dropped and lazy-load again |

## Where things live

```
OperationsConsole/
├─ Sections/
│  ├─ ListsTreesPage.cs             ← THIS MODULE: lazy tree, virtual list, selection → service → detail
│  └─ ListsTreesPage.Designer.cs    ← THIS MODULE: categoryTree, documentList, documentDetail, imageList, expandTimer,
│                                     the command row and the header counters
├─ ListsTrees/                      ← NEW
│  ├─ DocumentDetailControl.cs      the detail UserControl: Show(DocumentModel) + ShowMessage(string), nothing else
│  └─ DocumentDetailControl.Designer.cs
├─ Models/                          ← NEW (next to Module 1's SectionKey / SectionInfo)
│  ├─ CategoryNode.cs               id + title + path + HasChildren; NodeName = "cat-7"
│  ├─ DocumentSummary.cs            one list row: id, name, kind, type, size, modified, IsWarning → ImageKey
│  ├─ DocumentPage.cs               the server-side page: Items + Total (this is what VirtualListSize comes from)
│  └─ DocumentModel.cs              one whole document, already formatted — the only type the detail control sees
├─ Services/DocumentService.cs      ← NEW  GetTopCategories / GetChildren / GetDocumentPage(Async) / GetDocument(Async),
│                                     557 generated documents, ~300 ms latency, SimulateFailure, ConsoleLog on every call
├─ wwwroot/icons/                   ← NEW  folder.svg, contract.svg, invoice.svg, drawing.svg, warning.svg (16×16)
├─ docs/
│  ├─ ControlSelection.md           Module 1 (untouched)
│  └─ ExplorerNotes.md              ← NEW: data shape → control, the identity rule, lazy + virtual, what breaks at 10×
│
│  ── unchanged, from the earlier modules ──
├─ MainPage.cs / .Designer.cs       the shell: four docked areas + Event log card, Navigate(), IConsoleShell
├─ Shell/                           IConsoleShell.cs, ConsoleLog.cs, ISection.cs
├─ Sections/                        EditorsPage, LayoutsPage, DataGridViewPage, DashboardPage, WidgetsPage
├─ Models/SectionKey.cs, SectionInfo.cs   ·   Services/SectionCatalog.cs
├─ ClientProfiles.json  Program.cs  Startup.cs
```

## Lab steps → where in the code

| Lab step (`m4.json`) | Where |
|---|---|
| 1 · Open the solution, go to the Lists and Trees tab that still shows the placeholder | this folder; the placeholder body of `Sections/ListsTreesPage` is what this module replaced |
| 2 · Lab goal: tree with stable IDs, list tied to the node, lazy/virtual, image keys, detail UserControl through a service | `Sections/ListsTreesPage.cs` (the whole file) + `ListsTrees/DocumentDetailControl.cs` + `Services/DocumentService.cs` |
| 3 · `DocumentService` with `GetTopCategories()`, `GetChildren(categoryId)`, `GetDocumentPage(categoryId)`, `GetDocument(documentId)` over a few hundred generated records | `Services/DocumentService.cs` — the four public methods (plus `…Async` twins used by the page), `static DocumentService()` builds the catalogue, `GenerateDocuments()` builds 557 `DocumentSummary` rows |
| 4 · `categoryTree` built lazily: top level only, ID in `Tag`/`Name`, one placeholder child, `AfterExpand` replaces it | `LoadTopCategories()`, `CreateCategoryNode(category, withPlaceholder)`, `CreatePlaceholder()`, `NeedsLoading(node)`, `categoryTree_AfterExpand` → `LoadChildren(node)` |
| 5 · `documentList` in Details view tied to the selected node: virtual mode, `VirtualListSize` from the service page, items built in `RetrieveVirtualItem`, document ID in `Tag` | designer: `documentList.VirtualMode = true`, `View = Details`, four `Columns.Add(text, width)`. Code: `LoadCategoryPage()` → `ApplyPage()` sets `VirtualListSize`; `documentList_RetrieveVirtualItem` builds the item and sets `Tag` + `ImageKey`; `documentList_CacheVirtualItems` logs the range |
| 6 · One `ImageList` shared by both controls, `ImageKey` per item type (folder, contract, invoice, drawing, warning) | designer: `imageList` (16×16), `categoryTree.ImageList`, `documentList.SmallImageList`. Code: `RegisterIcons()` — `imageList.Images.Add(key, "wwwroot/icons/<key>.svg")`. Keys per row come from `DocumentSummary.ImageKey` |
| 7 · `DocumentDetailControl`: a UserControl with one public `Show(DocumentModel)` and no reference to the tree, the list or the service | `ListsTrees/DocumentDetailControl.cs` — `Show(DocumentModel)` plus `ShowMessage(string)` for the empty/error states; it imports only `OperationsConsole.Models` |
| 8 · In `categoryTree_AfterSelect` and `documentList_SelectedIndexChanged`, resolve the ID from `Tag`, call the service, hand the result to the detail control | `categoryTree_AfterSelect` → `LoadCategoryPage(id, title)`; `documentList_SelectedIndexChanged` → `LoadDocument(id)` → `documentDetail.Show(model)` + `ConsoleLog.Record(model.DocumentId)` |
| 9 · Show every path: loading state, empty-category message, friendly message on failure, no half-built items | loading: `documentList.ShowLoader` / `documentDetail.ShowLoader` around every ~300 ms call. Empty: `ApplyPage()` → `lblEmptyList.Visible` + `VirtualListSize = 0`. Failure: `ReportFailure()` (red status, `AlertBox`, exception to the log only) and the `warning` node in `LoadChildren`'s catch. No half-built items: `ClearList()` sets `VirtualListSize = 0` **before** the call. Recovery: `btnRetry_Click` → `_lastAction` |
| 10 · Review & run: test with realistic data, note what breaks first at 10× and which strategy fixes it | `docs/ExplorerNotes.md` §5 (the ordered table) and §"Evidence" |
| Stretch (lab 4) · diagnostic panel with selected control, record ID, profile, last refresh | already in the shell from Module 1; this page feeds it with `ConsoleLog.Control(...)` and `ConsoleLog.Record("DOC-000312")` |

## Self-check answers (lab / exam guide)

- **Two categories under different customers are both called "Contracts". What exactly does your selection
  handler use to tell them apart, and where does that value live?**
  The integer in `TreeNode.Tag` — the category ID the service assigned: **7** for Contoso ▸ Contracts, **10**
  for Fabrikam ▸ Contracts, **13** for Northwind ▸ Contracts. `CreateCategoryNode` writes it in two places at
  once: `Tag = category.Id` (what code reads) and `Name = "cat-7"` (what makes the node findable by key).
  `categoryTree_AfterSelect` does `if (!(node.Tag is int)) return;` and then `LoadCategoryPage((int)node.Tag, …)`
  — the text is passed along only so the status line can name the folder to a human. The type check doubles as
  the guard against the placeholder and error nodes, which deliberately carry no `Tag`. Same rule one level
  down: `ListViewItem.Tag = document.Id`, `Name = "DOC-000312"`, and the selected ID is resolved from the
  cached page at `SelectedIndices[0]`, never from `Items` and never from a column value.

- **If the document count under one category grew from 300 to 300,000, which part of your explorer would break
  first, and which property or event would you change to fix it?**
  Not the ListView — virtual mode already caps it at the rows on screen. What breaks first is the **server-side
  page**: `GetDocumentPage(7)` materialises every `DocumentSummary` of the category into `DocumentPage.Items`,
  and `RetrieveVirtualItem` indexes straight into it. At 300 000 rows that is one enormous allocation, rebuilt
  on every category click. The fix is to make the page a real window instead of the whole category: keep
  `VirtualListSize` fed from a `COUNT`, and fill an index-keyed cache from
  **`documentList.CacheVirtualItems`** (`e.StartIndex` … `e.EndIndex`) with a `Skip/Take` query, so
  `RetrieveVirtualItem` reads the cache and misses trigger the next block. After that, the second-order fixes
  in order: **`ListView.PrefetchItems`** for scroll smoothness, and on the tree side
  **`TreeView.VirtualScroll`** (+ its `PrefetchItems`) once one open branch holds thousands of nodes.
  `docs/ExplorerNotes.md` §5 has the full table.

- **Which lines of your selection handler could be moved into a unit test without a browser, and which lines
  belong to the detail UserControl instead?**
  Testable without a browser: the **ID resolution** (`node.Tag is int` → `(int)node.Tag`;
  `page.Items[selectedIndex].Id`) and the **bounds checks** — plain logic over a `DocumentPage`, which is a
  POCO; everything in `DocumentService` (that three folders named "Contracts" return three different pages,
  that an empty category returns `Total == 0`, that `SimulateFailure` throws); and the display formatting on
  the models (`DocumentSummary.ImageKey` picking `warning` over the type key, `SizeText`, `ModifiedText`,
  `DocumentModel.StatusText`). Belonging to the UserControl: which `Label` receives which string, the colour
  `lblStatus` turns, and the swap between `pnlFields` and `lblMessage` — that is view state, exercised through
  `Show(model)` / `ShowMessage(text)` and checked in the browser. What belongs to **neither** is the wiring in
  between (`ShowLoader`, `try`/`catch`, `AlertBox`) — that is the page's job as coordinator, and it is the part
  you verify by clicking, which is exactly why the command row makes every path reachable.

## Known simplifications / unverified

- **`DocumentPage` holds the whole category** (`Items.Count == Total`), so `CacheVirtualItems` only logs the
  range instead of fetching a block. That is the shortcut `docs/ExplorerNotes.md` §5 names as the first thing
  to fix at 10×; the class shape would not change.
- **`ListView.FullRowSelect` does not exist in Wisej.NET 4.1.** The lab step names the WinForms property, but
  Wisej renders the Details view through a `DataGridView` that always selects the whole row. The equivalent
  here is `MultiSelect = false` + `SelectionMode = SelectionMode.One`, both set in the designer.
- **`PrefetchItems = 0`** on the list, on purpose, so the "items created" counter shows exactly what the
  visible rows cost (the walkthrough's *312 rows · 8 items created*). Raising it is the third strategy, not the
  first — see `ExplorerNotes.md` §5.
- **`TreeView.VirtualScroll` is left off.** With 6 + 12 categories there is nothing to virtualise, and turning
  it on forces every node to the same height. It is documented as the fix for the 10× tree, not used here.
- **The icons are five hand-written SVG files** registered with `imageList.Images.Add(key, url)`. Built-in
  theme icon names (`"icon-folder"`, `"icon-warning"`, …) are the alternative and would follow the active
  theme for free; own files were chosen because these five keys carry business meaning. The SVGs are served
  from the project folder, so **run from `Module 4/OperationsConsole`** (the `csproj` was not touched to copy
  them to `bin/`).
- **API members used from the XML docs and not yet executed in a browser** (the reviewer should check them at
  runtime): `ListView.VirtualMode`, `VirtualListSize`, `RetrieveVirtualItem` (+ `RetrieveVirtualItemEventArgs.ItemIndex` / `.Item`),
  `CacheVirtualItems` (+ `CacheVirtualItemsEventArgs.StartIndex` / `.EndIndex`), `ListView.SelectedIndices[0]`,
  `ListView.PrefetchItems`, `ListView.SmallImageList`, `ListView.Columns.Add(text, width)`,
  `ListView.SelectionMode`, `TreeView.AfterExpand` / `AfterSelect` (`TreeViewEventArgs.Node`),
  `TreeNode.ImageKey` / `Tag` / `Name` / `ToolTipText` / `Collapse()` / `Expand()`,
  `ImageList.Images.Add(string, string)` with a URL, and `Control.ShowLoader`.
- **`node.Expand()` firing `AfterExpand` server-side** is assumed but not verified, so
  `expandTimer_Tick` calls `LoadChildren(node)` right after `node.Expand()` as a safety net; it is a no-op once
  the placeholder is gone, so the branch is never loaded twice either way.
- The **other five sections are still the Module 1 placeholders** in this folder — modules 2, 3, 5, 6 and 7 are
  built in their own folders and layered afterwards.
