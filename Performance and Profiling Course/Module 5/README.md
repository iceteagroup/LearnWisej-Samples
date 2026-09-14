# WisejPerfLab · Performance & Profiling · Module 5

Lab build for **Module 5 · Data Controls, Browser Payloads and Large UI Surfaces**. The two biggest
surfaces in the app are gone:

- **The ticket grid is virtual.** `Tickets/Search` **990 ms → 125 ms**; the click issues **3**
  statements instead of 5,001, and the session holds a page of rows instead of the result.
- **The customer tree loads a level at a time.** `Customers/LoadTree` **510 ms → 62 ms** (3,201
  statements → 3), `Customers/ExpandNode` **158 ms → 9 ms**.

Both are now inside budget. Evidence and the payload discussion: [`LargeSurfaces.md`](docs/LargeSurfaces.md).

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Performance and Profiling Course/Module 5/WisejPerfLab"
dotnet run -c Release -f net10.0 --urls http://localhost:5805
```

Module 4 on 5804 is the before.

## What to click

| Action | What you should see |
|---|---|
| Tickets → **Search tickets** | `5,000 rows … 125 ms — within the 300 ms budget`, and a moment later `1 page(s) held   1 fetch(es)   201 statements` as the grid asks for the rows it is showing |
| scroll the grid to the middle | another fetch, another page — never a query per cell |
| Tickets → **Redraw** | a repaint: no query, no row objects |
| Customers → **Load the customer tree** | `8 root nodes   3 SQL statements   62 ms — within the 150 ms budget`, each branch labelled `(20 below)` |
| Customers → **Expand the first branch** | `20 children   3 SQL statements   9 ms` — and expanding it again costs nothing, the children are already there |
| **Break the database** → search | empty grid, banner, `failed=DatabaseUnavailableException` |
| **Break the database** → expand a fresh branch | the node says *(children unavailable)* and keeps its placeholder |
| **Run the three scenarios ×3** | all three medians inside budget for the first time in the course |

## What changed since Module 4

```
Models/TicketGridRow.cs          new — the six displayed columns, formatted; no entity reaches the UI
Models/TicketRow.cs              deleted (replaced by TicketGridRow)
Models/TicketDisplayRow.cs       deleted — its last caller went with TicketFormatter.FormatRow
Models/CustomerNodeRow.cs        + ChildCount, and a label that carries the count placeholder
Services/TicketQueryService.cs   new — Count(filter) and GetPage(filter, first, count)
Services/TicketSearchService.cs  deleted
Services/TicketPageCache.cs      new — the page cache behind CellValueNeeded
Services/CustomerTreeService.cs  rewritten — GetRoots() / GetChildren(id), 3 grouped statements a level
Services/TicketFormatter.cs      FormatRow deleted; + FormatMoment
Pages/TicketGridPage.cs          virtual mode: RowCount from a count query, CellValueNeeded + DataRead
Pages/CustomerTreePage.cs        roots only, placeholder children, load on expand
Forms/TicketDetailForm.cs        takes a TicketGridRow
Startup.cs                       registers TicketQueryService
```

**`TicketFormatter.FormatRow` — the function at the top of the Module 2 hot path, called 50,000 times
per refresh — no longer exists.** Module 3 took the dashboard off it, Module 5 took the grid off it, and
the last caller went with them. That is the normal ending for a hot function: not making it faster, not
calling it.

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Collect the before evidence for both scenarios, including the browser network panel | [`LargeSurfaces.md`](docs/LargeSurfaces.md), which also says what the WebSocket makes it impossible to read from the Performance API |
| `TicketGridRow` with only the displayed columns and a precomputed `AgeText`; no entity reaches the UI | `Models/TicketGridRow.cs`, `TicketQueryService.BuildRow` |
| `VirtualMode = true`, `RowCount` from a count query, `CellValueNeeded` from a paged cache | `TicketGridPage` constructor and `RunSearch`; `TicketPageCache.GetValue` |
| No database call per cell, no formatting in the handler, no lock that blocks parallel requests | `TicketPageCache` — the three rules are its remarks |
| Children on expand with a count placeholder on collapsed branches | `CustomerTreePage.BuildNode` / `treeView1_BeforeExpand`; `CustomerNodeRow.Label` |
| Decide from a measurement whether prefetching ahead of the viewport earns its fetches | the last section of [`LargeSurfaces.md`](docs/LargeSurfaces.md) |
| A filter that matches nothing shows an empty grid, not a stale page | `TicketPageCache.Reset` on every search |
| A branch whose children fail to load reports it on the node | the `catch` in `treeView1_BeforeExpand` |
| Scrolling faster than the service can answer never shows values from the wrong row | `GetPage` orders by `UpdatedAt` then `Id`; the cache keys pages by first index |
| Server CPU, allocations and retained row objects before and after | [`LargeSurfaces.md`](docs/LargeSurfaces.md) |
| Browser evidence before and after, and compression vs unnecessary updates | [`LargeSurfaces.md`](docs/LargeSurfaces.md) |

## Self-check answers

**Why did the statement count drop if the query did not change?** Because the page fetches 200 rows
instead of 5,000. The N+1 is still in `GetPage` — 201 statements per page — and Module 6 removes it.
Fetching less is a real fix, and it is not the same fix as querying better.

**Does virtual mode save the browser or the server?** Here, mostly the server. Wisej.NET's grid and tree
already virtualise their **rendering** — the browser held nine `TreeNode` widgets for a 3,200-node tree.
What the large surface cost was server memory, server CPU and statements, and a model of everything
serialised to the client.

**What makes a virtual grid slower than the bound one it replaced?** A query per cell, formatting inside
`CellValueNeeded`, or a lock around the fetch. All three turn one scroll into a stutter.

**What makes it wrong?** An unstable sort. If two pages of the same result can come back in different
orders, row 4,201 is a different ticket on the second fetch and the user sees values from the wrong row.

## Known simplifications

- Update **byte** counts are not in the docs, because every update here travels over the WebSocket and
  the Performance API does not report its frames. [`LargeSurfaces.md`](docs/LargeSurfaces.md) says how to
  read them in the browser's Network panel, and what was measured instead.
- The export still blocks the request thread and still writes the file a line at a time. Module 6.
