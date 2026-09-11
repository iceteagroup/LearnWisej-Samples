# SupportDesk · Data Binding with EF Core · Module 3

Local lab build for **Module 3 · Loading Data into BindingSource, DataGridView and Lookup Controls**: the
Support Desk ticket browser. `TicketQueryService.SearchTicketsAsync` composes `AsNoTracking`, one `Where`
per filter, `CountAsync`, `OrderByDescending`, `Skip`/`Take` and a `Select` into `TicketListItem`, and the
materialised list goes into `ticketBindingSource`, which the grid is bound to.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 3/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5403
```

In Development the host migrates and seeds the local SQLite file at start. The EF Core command log is on in
`appsettings.Development.json`, so every statement appears in the console. Tests: `dotnet test SupportDesk.Tests`.

## What to try

The page is the ticket browser: `searchTextBox`, `statusComboBox`, `customerComboBox`, `searchButton`,
`ticketsDataGridView` (through `ticketBindingSource`), `statusLabel`, `prevPageButton`, `nextPageButton`.

- **Open the page**: the lookups load first, then the first page: *Showing 50 of 312 tickets · page 1 of 7 · page size 50*.
- **Search** with text, a status and/or a customer: the page index resets; the console shows exactly two
  statements, a `COUNT` and a paged `SELECT … LIMIT @p OFFSET @p` (SQLite renders paging as LIMIT/OFFSET).
- **Next page / Previous page**: one new query each; Previous is disabled on page 1, Next on the last page.
- **No matches**: *No tickets match these filters*.
- **Database failure**: a friendly `AlertBox`, *Tickets could not be loaded*, and the buttons come back, so
  the next Search simply runs again.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | `TicketListItem` and `TicketSearchCriteria` records with `SearchTicketsAsync` | [`docs/TicketSearchService.md`](docs/TicketSearchService.md) · `SupportDesk.Services/TicketBrowsing.cs`, `TicketQueryService` |
| 2 | `DataGridView` bound to `TicketListItem` through a `BindingSource` | [`docs/TicketBrowserBinding.md`](docs/TicketBrowserBinding.md) · `TicketBrowserPage.Designer.cs`, `LoadTicketsAsync` |
| 3 | Filtering and paging composed in the query | [`docs/PagingInTheDatabase.md`](docs/PagingInTheDatabase.md) · `TicketQueryService.RunSearchAsync` |
| 4 | Lookup ComboBoxes loaded before the first search | [`docs/LookupComboBoxes.md`](docs/LookupComboBoxes.md) · `GetStatusesAsync` / `GetCustomersAsync`, `LoadLookupsAsync` |
| 5 | Search, Next and Previous with a total count and a loading guard | [`docs/SearchPagingAndTheLoadingGuard.md`](docs/SearchPagingAndTheLoadingGuard.md) · `LoadTicketsAsync`, `UpdatePagingButtons` |

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the Module 2 solution; migration applied, seed present | `SupportDeskDevelopmentDatabase.cs` (MigrateAsync + seed at start) |
| 2 · `TicketListItem`, `TicketSearchCriteria`, `PagedResult<T>` | `SupportDesk.Services/TicketBrowsing.cs` |
| 3 · `SearchTicketsAsync` | `TicketQueryService.SearchTicketsAsync` → `RunSearchAsync` |
| 4 · `ticketBindingSource` + `ticketsDataGridView`, `AutoGenerateColumns = false`, `DataPropertyName` columns | `TicketBrowserPage.Designer.cs` |
| 5 · Lookups in `Load` with `DisplayMember`/`ValueMember` and an "All" entry | `TicketBrowserPage.LoadLookupsAsync` |
| 6 · `LoadTicketsAsync` with the loading guard | `TicketBrowserPage.LoadTicketsAsync`, `ReadCriteria` |
| 7 · Search / Next / Previous, count and page size in the status label | `searchButton_Click`, `nextPageButton_Click`, `prevPageButton_Click`, `UpdatePagingButtons` |
| 8 · EF Core logging; exactly two statements per search | `appsettings.Development.json`; `TicketSearchTests` |
| 9 · Loading, empty result and failure visible, search again afterwards | `LoadTicketsAsync` (`catch` → `AlertBox`, `finally` restores the buttons) |

## Self-check answers

- **Which lifetime did you choose for the `DbContext` inside `SearchTicketsAsync`, and what would break if the page kept one context alive?**
  One context per search, created and disposed inside the method. A page-lived context would run two
  operations at once when a click arrives during an await (EF Core refuses that), keep every materialised
  entity in its change tracker for the life of the tab, serve stale values from its identity map, and stay
  broken after a failed connection instead of being replaced on the next click.
- **After a search, what does the server hold for this session, and what stays only in the database?**
  The page and its controls, the `BindingSource` with fifty `TicketListItem` records, the two lookup lists,
  the selected row, `_pageIndex` and `_totalCount`: a few tens of kilobytes. Everything else (other tickets,
  descriptions, `RowVersion`, related entities) stays in the database; no `DbContext`, no tracked entity and
  no query is held. A second operator costs a second copy of that, bounded by the page size, not the table size.
- **If the customer lookup grew to fifty thousand rows, what would change?**
  `SearchTicketsAsync` and `TicketSearchCriteria` stay as they are: they take a `CustomerId`. `GetCustomersAsync`
  becomes a searching lookup (`StartsWith`, `Take(20)`), and the page swaps the drop-down list for an
  autocomplete or a small lookup dialog that keeps the chosen key.
