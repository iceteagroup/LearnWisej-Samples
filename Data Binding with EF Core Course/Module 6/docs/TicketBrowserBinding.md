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
    this.colNumber, this.colTitle, this.colCustomerName, this.colAgentName, this.colCategoryName,
    this.colStatus, this.colPriority, this.colDueDate, this.colUpdatedAt});
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

| Column | `DataPropertyName` | Notes |
|---|---|---|
| `colNumber` | `Number` | |
| `colTitle` | `Title` | widest fill weight |
| `colCustomerName` | `CustomerName` | joined by the database |
| `colAgentName` | `AgentName` | `DefaultCellStyle.NullValue = "— unassigned —"` |
| `colCategoryName` | `CategoryName` | joined by the database |
| `colStatus` | `Status` | |
| `colPriority` | `Priority` | |
| `colDueDate` | `DueDate` | `DefaultCellStyle.Format = "yyyy-MM-dd"`, empty when null |
| `colUpdatedAt` | `UpdatedAt` | `Format = "yyyy-MM-dd HH:mm"` — the sort key |

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

## The anti-pattern, run for real

`SupportDesk.Services/BoundIQueryableAntiPattern.cs` is the wrong version, kept out of the real services
and named after what it is. `buttonBindQuery` runs it:

```csharp
var db = await _dbFactory.CreateDbContextAsync(token);
try
{
    boundQuery = db.Tickets.OrderByDescending(t => t.UpdatedAt).Select(t => new TicketListItem(…));
    // "ticketBindingSource.DataSource = boundQuery;" — the assignment sends nothing at all
}
finally
{
    await db.DisposeAsync();       // the handler returns; `await using` disposes the unit of work
}

var rows = boundQuery.ToList();    // the grid enumerates its data source — and this is where it dies
```

The assignment looks like it worked: an `IQueryable` is a recipe, so no `COUNT`, no `SELECT` and no rows.
The failure arrives later, when the grid enumerates to paint its first row — after the handler has
returned and the context is gone. EF Core answers with `ObjectDisposedException`, thrown from inside the
UI layer, in a stack that contains none of the data code.

Even if the context somehow survived, the grid would re-run the whole query on every scroll, sort and
repaint, with no `Skip`/`Take` and every row tracked. The fix is one call: `await …ToListAsync()` before
the assignment.

## Evidence

**The exception, captured in a console check against SQLite in memory:**

```
ObjectDisposedException: Cannot access a disposed context instance. A common cause of this error is
disposing a context instance that was resolved from dependency injection and then later trying to use
the same context instance elsewhere in your application. …
```

**Trace, `Bind IQueryable (anti-pattern)`** (the shape the page writes):

```
• anti-pattern ticketBindingSource.DataSource = db.Tickets.Select(...) — the query instead of the list; the context is then disposed and the grid enumerates
◦ context      #9 created (SupportDeskContext from the factory)
• service      ticketBindingSource.DataSource = <IQueryable> — the assignment sends nothing: no COUNT, no SELECT, no rows. The handler looks like it worked
◦ context      #9 disposed (0 tracked entities released)
• service      the handler has returned and the context is disposed — now the grid enumerates its data source to paint the first rows (this is where the real query would run)
• caught       ObjectDisposedException: Cannot access a disposed context instance.
• anti-pattern in a real page this exception is thrown while the grid paints its first row — inside the UI layer, with no data code in the stack. The grid shows nothing and the session looks broken
• anti-pattern the fix is one call: await …ToListAsync() before the assignment, so the BindingSource holds rows and not a recipe
```

Note the trace has **no `→ SQL` line at all**: the query never reached the database.

**Trace, a normal search** — the BindingSource row count is reported in the four-lifetimes table under the
grid: `1 page, the grid, the trace list and a BindingSource holding 50 TicketListItem row(s)`.

**Tests** (`SupportDesk.Tests/TicketSearchTests.cs`):

- `Binding_the_query_instead_of_the_list_fails_when_the_grid_enumerates` — the anti-pattern throws
  `ObjectDisposedException` (or `InvalidOperationException`) and the message names a disposed context.
- `The_search_tracks_nothing_and_never_returns_an_entity` — what the BindingSource is handed is a list of
  `TicketListItem`, and nothing is tracked.

**Not verified here:** the grid's own rendering — column widths under `Fill`, the `— unassigned —` null
text and the `yyyy-MM-dd` due date — is Wisej.NET behaviour that only the browser can confirm. See the
README's *Verified / unverified*.
