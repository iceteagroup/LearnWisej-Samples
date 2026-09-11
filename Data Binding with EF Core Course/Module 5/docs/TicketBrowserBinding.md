# The grid, the BindingSource and the list

Deliverable 2 of the Module 3 lab: `ticketsDataGridView` bound to `TicketListItem` rows **through**
`ticketBindingSource` — and never to a `DbSet` or an `IQueryable`.

## The three objects and their lifetimes

```
List<TicketListItem>   ← replaced on every search        (the rows, ~50 records)
        │
ticketBindingSource    ← lives for the session           (the bridge: current item, notifications)
        │
ticketsDataGridView    ← lives for the session           (columns designed once, never rebuilt)
```

The grid is designed once and never touched again. Only the BindingSource's `DataSource` changes, and only
with a **materialised list**. That is the whole binding story of the module.

## What the designer sets

`SupportDesk.Web/TicketBrowserPage.Designer.cs`:

```csharp
this.ticketBindingSource = new Wisej.Web.BindingSource(this.components);
…
this.ticketsDataGridView.AutoGenerateColumns = false;
this.ticketsDataGridView.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
this.ticketsDataGridView.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
    this.colNumber, this.colTitle, this.colCustomerName, this.colAgentName,
    this.colStatus, this.colDueDate, this.colUpdatedAt});
this.ticketsDataGridView.MultiSelect = false;
this.ticketsDataGridView.ReadOnly = true;
this.ticketsDataGridView.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
// The DataSource is assigned AFTER the columns exist.
this.ticketsDataGridView.DataSource = this.ticketBindingSource;
```

`AutoGenerateColumns = false` is the first line for a reason: with it left on, the grid would invent a
column per property of whatever list arrives, in whatever order the record declares them, with `Id` on the
left and no formatting anywhere. Off, the columns are the ones below and the record can gain a property
without changing the screen.

Each column names one property:

| Column | Header | `DataPropertyName` | Notes |
|---|---|---|---|
| `colNumber` | Number | `Number` | |
| `colTitle` | Title | `Title` | widest fill weight |
| `colCustomerName` | Customer | `CustomerName` | joined by the database |
| `colAgentName` | Agent | `AgentName` | `DefaultCellStyle.NullValue = "—"` for an unassigned ticket |
| `colStatus` | Status | `Status` | |
| `colDueDate` | Due | `DueDate` | `DefaultCellStyle.Format = "yyyy-MM-dd"`, empty when null |
| `colUpdatedAt` | Updated | `UpdatedAt` | `Format = "yyyy-MM-dd HH:mm"` — the sort key |

`TicketListItem` also carries `CategoryName` and `Priority`; with `AutoGenerateColumns = false` the grid
simply has no column for them.

The order matters: **columns first, `DataSource` second**. Wisej.NET matches a column to a bound property
when the source arrives, and binding before the columns exist throws *Cannot bind to the property or column
… on the DataSource*. Assigning the BindingSource once in `InitializeComponent` (which runs in the page
constructor) also means the grid is bound before the page is ever shown, so the first search only has to
change the list.

`ReadOnly` + `FullRowSelect` + `MultiSelect = false` describe what the browser is: a list you pick a row
from. Editing arrives in Module 4, in a modal form with its own tracked context.

## What the handler assigns

`TicketBrowserPage.LoadTicketsAsync`, inside the awaited operation:

```csharp
var result = await TicketQueries.SearchTicketsAsync(criteria);

_totalCount = result.TotalCount;

this.ticketBindingSource.DataSource = result.Items.ToList();
this.ticketBindingSource.ResetBindings(false);
```

Three things are true about that assignment and all three matter:

1. **It is a list.** `result.Items` is already materialised — `ToList()` here only turns the
   `IReadOnlyList<T>` into the concrete `List<T>` the BindingSource expects. The context that produced the
   rows was disposed inside the service, before this line ran, and the list does not care.
2. **The grid is not re-bound.** `ticketsDataGridView.DataSource` still points at the same BindingSource it
   was given in the constructor. Columns, widths and formats survive every search.
3. **`ResetBindings(false)`** tells the bound controls the list was replaced (`false` = the *schema* did
   not change, only the rows), so the grid repaints without rebuilding its columns.

## The anti-pattern: binding the query instead of the list

The wrong version is `ticketBindingSource.DataSource = db.Tickets.Select(t => new TicketListItem(…))`,
assigned inside a handler whose `await using` then disposes the context. The sample does not ship it; the
reasoning is what matters.

The assignment looks like it worked: an `IQueryable` is a recipe, so no `COUNT`, no `SELECT` and no rows.
The failure arrives later, when the grid enumerates to paint its first row — after the handler has
returned and the context is gone. EF Core answers with `ObjectDisposedException` (*Cannot access a
disposed context instance.*), thrown from inside the UI layer, in a stack that contains none of the data
code.

Even if the context somehow survived, the grid would re-run the whole query on every scroll, sort and
repaint, with no `Skip`/`Take` and every row tracked. The fix is one call: `await …ToListAsync()` before
the assignment, which is exactly what `SearchTicketsAsync` does inside the service.

## Evidence

**In the browser:** a search fills the grid with 50 rows and `statusLabel` reads *Showing 50 of 312
tickets · page 1 of 7 · page size 50*; paging replaces the rows and keeps the columns.

**Tests** (`SupportDesk.Tests/TicketSearchTests.cs`):

- `The_search_tracks_nothing_and_never_returns_an_entity` — what the BindingSource is handed is a list of
  `TicketListItem`, and nothing is tracked.
- `Empty_criteria_return_one_page_and_the_total_of_every_ticket` — the list is materialised: 50 rows of
  312, 7 pages.

**Not verified here:** the grid's own rendering — column widths under `Fill`, the `—` null text for an
unassigned agent and the `yyyy-MM-dd` due date — is Wisej.NET behaviour that only the browser can confirm.
See the README's *Verified / unverified*.
