# OperationsConsole · Mastering the Control Library · Module 4

Lab build for **Module 4 · Lists, Trees, Repeaters, and Hierarchical Data**: the **Lists and Trees** section is the
document explorer — a `TreeView` that loads only the top level and fetches each branch in `AfterExpand`, a
`ListView` in virtual mode whose `VirtualListSize` comes from a server-side `DocumentPage`, one `ImageList` shared
by both, and a `DocumentDetailControl` UserControl that receives a finished `DocumentModel`.

The shell and the earlier sections come from Modules 1–3. This module adds `Services/DocumentService.cs`, four
models, `ListsTrees/`, `wwwroot/icons/` and `docs/ExplorerNotes.md`.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 4/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5704
```

Then open <http://localhost:5704> and select **Lists and Trees**. Run from the project folder: the static file
server serves it, which is how `wwwroot/icons/*.svg` reaches the browser.

## The data

`DocumentService` generates 557 documents in 6 top-level and 12 sub-categories, in memory, with a fixed seed.
Three folders are called "Contracts" (categories 7, 10, 13); Contoso ▸ Contracts holds 312 documents; two
categories are empty and **Archive 2019-2023** has no children; every 37th document is flagged. Every call takes
~300 ms so the loading state is visible.

## What to try

| Action | What you should see |
|---|---|
| Open **Lists and Trees** | six collapsed folder nodes |
| Expand **Contoso** | only Contoso's children load |
| Expand **Archive 2019-2023** | no children arrive; amber status `"Archive 2019-2023" has no sub-categories.` |
| Select **Contoso ▸ Contracts** | the list shows the loader, then 312 rows; the footer reads `8 of 312 items created` (however many rows fit) |
| Select **Fabrikam ▸ Contracts** | same folder text, 36 different documents |
| Pick a document | the detail card fills; the StatusBar shows `Record: DOC-000312`; a flagged document shows a red status and the warning icon |
| Select **Shared templates ▸ Contract templates** | the amber **No documents in this category** band |
| Tick **Simulate service failure**, then expand or select | red status, an `AlertBox`, a friendly message; the list is never half-built. Untick and try again to recover |
| **Refresh** on the ToolBar | the tree reloads the top level |

## Where things live

```
OperationsConsole/
├─ Sections/ListsTreesPage.cs / .Designer.cs   lazy tree, virtual list, selection → service → detail
├─ ListsTrees/DocumentDetailControl.cs         Show(DocumentModel) + ShowMessage(string)
├─ Models/CategoryNode, DocumentSummary, DocumentPage, DocumentModel
├─ Services/DocumentService.cs                 GetTopCategories / GetChildren / GetDocumentPage / GetDocument
├─ wwwroot/icons/                              folder, contract, invoice, drawing, warning (16×16 SVG)
└─ docs/ExplorerNotes.md                       data shape → control, identity rule, lazy + virtual, what breaks at 10×
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| `DocumentService` with `GetTopCategories()`, `GetChildren(categoryId)`, `GetDocumentPage(categoryId)`, `GetDocument(documentId)` | `Services/DocumentService.cs` (plus `…Async` twins the page uses) |
| `categoryTree` built lazily: top level only, ID in `Tag` / `Name`, one placeholder child, `AfterExpand` replaces it | `LoadTopCategories()`, `CreateCategoryNode()`, `categoryTree_AfterExpand` → `LoadChildren()` |
| `documentList` in Details view, virtual mode, `VirtualListSize` from the page, items built in `RetrieveVirtualItem` with the ID in `Tag` | designer (`VirtualMode`, `View = Details`); `ApplyPage()`, `documentList_RetrieveVirtualItem` |
| One shared `ImageList` with an `ImageKey` per item type | `imageList`, `RegisterIcons()`, `DocumentSummary.ImageKey` |
| `DocumentDetailControl` with one public `Show(DocumentModel)` | `ListsTrees/DocumentDetailControl.cs` |
| Selection handlers resolve the ID, call the service, hand the result to the detail control | `categoryTree_AfterSelect` → `LoadCategoryPage()`; `documentList_SelectedIndexChanged` → `LoadDocument()` |
| Loading state, empty-category message, friendly message on failure, no half-built items | `ShowLoader`; `lblEmptyList`; `ReportFailure()`; `ClearList()` before every call |
| Note what breaks first at 10× | `docs/ExplorerNotes.md` §5 |

## Self-check answers

- **Two categories are both called "Contracts". What does your handler use to tell them apart?**
  The integer in `TreeNode.Tag` — 7, 10 or 13 — written by `CreateCategoryNode` together with `Name = "cat-7"`.
  `categoryTree_AfterSelect` checks `node.Tag is int` and loads `(int)node.Tag`; the text is only used for the status line.
- **If one category grew from 300 to 300,000 documents, what breaks first?**
  Not the ListView — virtual mode caps it. The server-side page does: `GetDocumentPage` materialises every row.
  Make it a window filled from `CacheVirtualItems` with `Skip/Take`, then add `PrefetchItems`, and
  `TreeView.VirtualScroll` once a branch holds thousands of nodes.
- **Which lines of the selection handler could move into a unit test?**
  The ID resolution (`node.Tag is int`, `page.Items[index].Id`), everything in `DocumentService`, and the display
  formatting on the models. Which Label shows which string belongs to the UserControl.

## Known simplifications

- `DocumentPage` holds the whole category (`Items.Count == Total`); `ExplorerNotes.md` §5 names this as the first fix at 10×.
- `ListView.FullRowSelect` does not exist in Wisej.NET 4.1; `MultiSelect = false` + `SelectionMode.One` is the equivalent.
- `PrefetchItems = 0` on purpose, so the created-items counter shows exactly what the visible rows cost.
