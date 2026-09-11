# ExplorerNotes.md — Operations Console, Module 4

The control decisions behind the **document explorer** (`Sections/ListsTreesPage`), the identity rule it enforces,
the large-data strategy it uses, and what breaks first at ten times the volume.

## 1. Data shape → control

| Part of the screen | Shape of the data | Control chosen | Why | Considered and rejected |
|---|---|---|---|---|
| `categoryTree` | **Hierarchical** — customers own folders | `TreeView` with lazy loading | Expansion, parent/child context and "where am I" are the task. | **`ComboBox`** — flattens the hierarchy and loses the parent context. |
| `documentList` | **Flat collection, presentation matters** — name, type, size, modified | `ListView` (`View = Details`) in virtual mode | File-like content; virtual mode caps the cost of 312 rows at the rows on screen. | **`DataRepeater`** — a set of controls per row. **`DataGridView`** — right once rows are editable (Module 5). |
| `documentDetail` | **One object, a set of properties** | `DocumentDetailControl : UserControl` with `Show(DocumentModel)` | A designed screen with business labels and one public method. | **`PropertyGrid`** — exposes property names and implementation details to an end user. |

## 2. The identity rule

> The same label appears under many branches. Display text is for people; identity is for code.

`DocumentService` ships three folders called **Contracts** — category **7** under Contoso, **10** under Fabrikam,
**13** under Northwind — two empty categories, and one top-level branch (`Archive 2019-2023`) with no children.

- Every `TreeNode` gets `Tag = categoryId` and `Name = "cat-7"` (`CreateCategoryNode`).
- Every `ListViewItem` gets `Tag = documentId` and `Name = "DOC-000312"` (`documentList_RetrieveVirtualItem`).
- The placeholder and error nodes carry **no `Tag`**, so `categoryTree_AfterSelect` tells a category from
  scaffolding with a type check (`node.Tag is int`) instead of comparing text.
- No handler reads `Node.Text` or `Item.Text` to decide anything; text is used only in status messages.

## 3. The large-data strategy

**Lazy loading (the tree).** `LoadTopCategories()` fetches only the top level. Each node gets one child named
`placeholder`, which makes the expand glyph appear. `categoryTree_AfterExpand` → `LoadChildren(node)` finds the
placeholder, clears the branch and calls `DocumentService.GetChildren(Tag)`. A failed expand puts the placeholder
back next to a `Could not load — expand again to retry` node, so expanding again is the retry.

**Virtual mode (the list).** `documentList.VirtualMode = true`. On `AfterSelect` the page asks the service for a
`DocumentPage`, keeps it in `_page` and sets `documentList.VirtualListSize = page.Total`. The browser then asks for
the rows it can paint, and `documentList_RetrieveVirtualItem` builds one `ListViewItem` per request from
`_page.Items`, setting `Tag` and `ImageKey` at the same moment. The footer under the list reads
**`8 of 312 items created`**: 312 is what the control believes, 8 is what it cost.

**Clearing.** In virtual mode `Items` is not the store, so the list is emptied with `VirtualListSize = 0`
(`ClearList`) — before every service call, so a failure cannot leave half-built rows behind.

**Icons.** One `ImageList` (16×16) is shared by `categoryTree.ImageList` and `documentList.SmallImageList`, with the
keys `folder`, `contract`, `invoice`, `drawing`, `warning`, registered from `wwwroot/icons/<key>.svg`. A flagged
document shows `warning` instead of its type key.

## 4. Selection is an event, not a query

```
control  →  page (ListsTreesPage)  →  service (DocumentService)  →  detail (DocumentDetailControl)
```

Both selection handlers resolve the stable ID, call `DocumentService` inside `try` / `catch` with a loading state
(`ShowLoader`), and hand the finished model to `documentDetail.Show(model)`. `DocumentDetailControl` has two public
methods — `Show(DocumentModel)` and `ShowMessage(string)` — and no reference to the tree, the list or the service.

## 5. What breaks first at 10× — and which strategy fixes it

| Order | What breaks | Why | The fix |
|---|---|---|---|
| 1 | **The `DocumentPage` itself** — one category's page holds every row | Virtual mode caps what the browser renders, not what the server materialises. | Make the page a window: `Skip/Take` plus an index-keyed cache filled from `CacheVirtualItems`; `VirtualListSize` from a `COUNT`. |
| 2 | **Scrolling feels steppy** | A cache miss is a round trip. | `documentList.PrefetchItems` — pre-render rows outside the visible area. |
| 3 | **An expanded branch with hundreds of siblings** | Lazy loading stops the fetch, not the DOM cost of one open branch. | `categoryTree.VirtualScroll = true` (all nodes then share one height). |
| 4 | **Nothing, for a while** — startup stays flat | Lazy loading means the top level costs six rows whatever the catalogue holds. | Nothing to do. |

## 6. The paths

| Path | Trigger | What the user gets |
|---|---|---|
| success | expanding a node, selecting a category, selecting a document | green status, the list fills, the detail card fills |
| loading | every ~300 ms service call | `ShowLoader` on the list and on the detail card |
| empty | a category with no documents, or a branch with no children | `VirtualListSize = 0`, the amber **No documents in this category** band, amber status |
| failure | **Simulate service failure** ticked, then expand / select / pick a document | red status, an `AlertBox` in plain words, a friendly sentence in the detail card, a `warning` node in the tree |
| recovery | untick the switch and expand or select again | the call succeeds, status green |

## Evidence (what the running app shows)

- **Startup** — six top-level folder nodes, all collapsed.
- **Expand Contoso** — only Contoso's children load; Fabrikam and Northwind stay unfetched.
- **Select Contoso ▸ Contracts** — the list shows the loader, then 312 documents; the footer reads `8 of 312 items created`.
- **Select Fabrikam ▸ Contracts** — same folder text, 36 different documents: the handler keys on `Tag`.
- **Pick a document** — the detail card fills through the service and the diagnostics show `Record: DOC-000312`.
- **Select Shared templates ▸ Contract templates** — the amber **No documents in this category** band.
- **Simulate service failure + expand** — the branch shows `Could not load — expand again to retry` and collapses;
  red status and an `AlertBox`, no half-built items in the list.
