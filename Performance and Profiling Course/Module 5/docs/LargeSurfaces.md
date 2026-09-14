# Two large surfaces, before and after

Before = the app in `Module 4`. After = the app in this folder. Same dataset, same build, same warm-up,
same clicks.

## The ticket grid

| | Before (bound grid) | After (virtual grid) |
|---|---:|---:|
| `Tickets/Search`, 5,000 rows | **990 ms** | **125 ms** |
| SQL statements for the click | 5,001 | **3** |
| SQL statements to fill the first screen | — | 201 (one page of 200 rows) |
| Row objects held in the session | 5,000 `TicketRow` + 5,000 entities | **200** `TicketGridRow` (up to 4 pages = 800) |
| Long text columns loaded | `Description` + `Notes` on every row | still fetched by the page query — Module 6 |
| `Tickets/Redraw` | rebinds a 5,000-row list | repaints the visible rows |
| Budget (300 ms) | over | within |

The scenario's **statement count dropped because the page fetched less, not because the query improved**.
`TicketQueryService.GetPage` still loads entities and still asks for the customer name one row at a
time — 201 statements per page instead of 5,001 for the result. That N+1 is Module 6.

## The customer tree

| | Before (everything up front) | After (a level at a time) |
|---|---:|---:|
| `Customers/LoadTree` | **510 ms** | **62 ms** |
| Statements at load | 3,201 | **3** |
| Nodes built at load | 3,200 | **8** |
| `Customers/ExpandNode` | 158 ms, 3,201 statements | **9 ms, 3 statements** |
| Nodes built per expand | 0 (all of them already existed) | the children of that branch only |
| Budget (150 ms / 100 ms) | over / over | within / within |

Collapsed branches are not empty of information: each one carries a **count placeholder** —
`Benelux region  (20 below)` — and a leaf carries its ticket count. Both numbers come from the two
grouped queries that loaded the level, so the label costs nothing extra.

## What the browser actually receives

The lab asks for the network evidence, and this is where the sample has to be precise about what it can
and cannot show.

Two things were measured in the browser:

1. **Update size.** Wisej.NET sends updates over the **WebSocket** as soon as one is available, so the
   network panel shows a single long-lived connection rather than one response per click, and
   `performance.getEntriesByType('resource')` reports nothing for those updates. To compare bytes
   yourself, open the browser's Network panel, select the `wisej.wx` WebSocket and read the frames for
   one click — or disable the WebSocket to force long-polling and read the POST responses.
2. **What the controls create in the browser.** Measured directly from the client widget registry:
   after loading the 3,200-node tree, the browser held **9** `TreeNode` widgets, not 3,200; and the
   bound grid asked the server for rows as it scrolled rather than rendering 5,000 at once.

That second reading is the important one, and it is not the one people expect:

> **Wisej.NET's `TreeView` and `DataGridView` already virtualise their rendering. They do not
> virtualise your server.**

The browser was never going to draw 3,200 nodes, so the tree did not feel catastrophic. What the 3,200
nodes cost was on the server: 3,201 statements, 3,200 `TreeNode` objects held for the life of the
session, and a model of all of them serialised to the client. That is what this module removes — and it
is why the evidence for these two scenarios is *server time, statement count and objects held*, with the
payload as a supporting argument rather than the headline.

And the point the lab asks to be stated explicitly: **compression reduces bytes; it does not make an
unnecessary update free.** The 3,200-node model compresses well. It still had to be built, counted,
serialised, sent, parsed and held — for a tree the user opens two branches of.

## Rules the virtual grid has to keep

All three are in `TicketPageCache`, and each of them is a way a virtual grid can end up slower or wrong
than the bound one it replaced:

- **No database call per cell.** `CellValueNeeded` fires once per visible cell — six times per row. It
  reads from a page already in memory, and only a row outside every held page triggers a fetch.
- **No formatting in the handler.** The rows arrive already formatted from `TicketQueryService`; the
  handler picks a field by column index and returns it.
- **No lock around the fetch.** The browser can ask for several blocks of rows at once while scrolling.
  A lock would serialise those into a visible stutter.

And one correctness rule: **the order must be stable.** `GetPage` orders by `UpdatedAt` descending and
then by `Id`, so row index 4,201 is the same ticket on every fetch. A virtual grid whose query returns
rows in a different order between pages shows values from the wrong row, and it does it silently.

## Failure paths

- A filter that matches nothing sets `RowCount = 0`: the grid is empty, not showing the previous page.
  `TicketPageCache.Reset` throws every held page away whenever the filter changes, which is what makes
  that true.
- A page that cannot be fetched (**Break the database**) leaves its cells empty and reports the error in
  the status line and the banner. `CellValueNeeded` never throws — an exception there takes the grid
  down mid-render.
- A branch whose children fail to load keeps its placeholder and says *(children unavailable)* on the
  node, so the next attempt tries again instead of showing an empty branch as if it were empty.

## Is prefetching ahead of the viewport worth it?

The cache holds four pages of 200 and fetches on `DataRead`, which is the block of rows the client is
about to read. Fetching *further* ahead — say the next page as soon as one is served — was measured and
not kept: one page fetch is 201 statements and about 40 ms on this machine, and a user who scrolls two
pages and stops would pay for a third page nobody looks at. The decision belongs to the measurement,
and the measurement changes once Module 6 makes a page one statement instead of 201.
