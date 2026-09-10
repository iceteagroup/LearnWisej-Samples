# The lookup ComboBoxes

Deliverable 4 of the Module 3 lab: `statusComboBox` and `customerComboBox` show **names** and hand the
query **keys**, and both are filled before the first search runs.

## Two lookups, two very different sources

| | `statusComboBox` | `customerComboBox` |
|---|---|---|
| service | `TicketQueryService.GetStatusesAsync()` | `TicketQueryService.GetCustomersAsync()` |
| source | the fixed `TicketStatuses.All` list | `SELECT "Id", "Name" FROM "Customers" ORDER BY "Name"` |
| statements | **0** | **1** |
| `DisplayMember` | `Name` | `Name` |
| `ValueMember` | `Key` (a `string`) | `Id` (an `int`) |
| "All" row | `("", "All statuses")` | `(0, "All customers")` |

The status list is deliberately **not** a `SELECT DISTINCT "Status" FROM "Tickets"`. A distinct query costs
a statement, and worse, it only ever offers the statuses that happen to exist right now: the moment the
last resolved ticket is closed, "Resolved" disappears from the filter and the operator cannot ask for it
again. One fixed list — the same one the seeder writes and, from Module 5, the same one the validator
checks — cannot drift away from the data:

```csharp
public static class TicketStatuses
{
    public const string Open = "Open";
    public const string InProgress = "In Progress";
    public const string Waiting = "Waiting";
    public const string Resolved = "Resolved";
    public const string Closed = "Closed";

    public static readonly IReadOnlyList<string> All = new[] { Open, InProgress, Waiting, Resolved, Closed };
}
```

Customers are the opposite: real rows that change, so they are read — but projected to `LookupItem(Id,
Name)` and marked `AsNoTracking`. A ComboBox needs two values per row; loading `Customer` entities to fill
one would carry `Email` and a `Tickets` collection into session memory for nothing.

## Loaded before the first search

`TicketBrowserPage_Load` runs, in this order:

```csharp
await LoadLookupsAsync();      // 1. the lookups
await RefreshModelCardAsync(); // 2. the Module 2 card
await LoadTicketsAsync(...);   // 3. the first search
```

The order is not cosmetic. `ReadCriteria()` reads `statusComboBox.SelectedValue` and
`customerComboBox.SelectedValue`; a `SelectedValue` on a ComboBox with no items — or with no item matching
the value — resolves to **nothing**, silently. A search that runs before the lookups exist filters by
nothing and looks like it worked, and the same trap catches an editor that assigns its edit model to a
BindingSource before the lookup is populated (Module 4).

`LoadLookupsAsync` also seeds the two date pickers with sensible values (today and today + 14 days) that
only take effect once the operator ticks their check boxes.

## Names out, keys in

```csharp
var statusRows = new List<LookupText> { new LookupText("", "All statuses") };
statusRows.AddRange(statuses.Select(status => new LookupText(status, status)));
this.statusComboBox.DisplayMember = nameof(LookupText.Name);
this.statusComboBox.ValueMember   = nameof(LookupText.Key);
this.statusComboBox.DataSource    = statusRows;
this.statusComboBox.SelectedIndex = 0;

var customerRows = new List<LookupItem> { new LookupItem(0, "All customers") };
customerRows.AddRange(customers);
this.customerComboBox.DisplayMember = nameof(LookupItem.Name);
this.customerComboBox.ValueMember   = nameof(LookupItem.Id);
this.customerComboBox.DataSource    = customerRows;
this.customerComboBox.SelectedIndex = 0;
```

and, when the criteria are built:

```csharp
var status     = this.statusComboBox.SelectedValue is string s && s != "" ? s : null;
var customerId = this.customerComboBox.SelectedValue is int id && id != 0 ? id : (int?)null;
```

`SelectedValue` is typed `object`, and a boxed `int` does **not** unbox through `as int?` — `is int id` is
what works. The "All" rows are ordinary lookup rows whose key is the sentinel each type can spare (an empty
string, and `0`, which no identity column ever produces), mapped back to `null` in one place. Null is what
makes the filter *not exist*: see [PagingInTheDatabase.md](PagingInTheDatabase.md).

Nothing anywhere compares display text. `customerComboBox` filters by `CustomerId`, so renaming
"Northgate Retail Group" changes one label and no query.

## When the lookup outgrows a ComboBox

`DropDownStyle = DropDownList` with five customers is fine. At a few hundred rows the drop-down stops being
usable; at fifty thousand it is a wire and memory problem too, once per session. The answer is a UI change
— a searchable picker, an autocomplete that queries on keystroke with its own `Take(20)`, or a lookup
dialog with its own paged grid. `SearchTicketsAsync` does not change at all: it already takes an
`int? CustomerId`, and where that integer came from was never its business.

## Evidence

Captured in a console check against SQLite in memory.

**`GetStatusesAsync`** — no statement is sent, and the list is exactly:

```
Open, In Progress, Waiting, Resolved, Closed
```

**`GetCustomersAsync`** — one statement:

```sql
SELECT "c"."Id", "c"."Name" FROM "Customers" AS "c" ORDER BY "c"."Name"
```

five rows, the first being `Brightwater Clinics`.

**Trace, page Load** (the shape the page writes):

```
• session      page created for session a1b2c3d4 · services through [Inject] → resolved from Microsoft DI
• lookup       status lookup: the fixed TicketStatuses.All list (5 values) — no statement is sent
◦ context      #1 created (SupportDeskContext from the factory)
→ SQL          SELECT "c"."Id", "c"."Name" FROM "Customers" AS "c" ORDER BY "c"."Name"   (0.4 ms)
• lookup       customer lookup: 5 rows projected to LookupItem (Id + Name), nothing tracked
◦ context      #1 disposed (0 tracked entities released)
◦ lookups      statusComboBox 6 rows, customerComboBox 6 rows — filled BEFORE the first search · 1 statement(s), 1 context created, 1 disposed
```

Six rows each: five values plus the "All" row.

**Tests** (`SupportDesk.Tests/TicketSearchTests.cs`):

- `GetStatusesAsync_returns_the_five_statuses_the_seeder_uses_and_sends_nothing` — the list matches
  `Open, In Progress, Waiting, Resolved, Closed`, `scope.Commands == 0`, and every status actually present
  in the seeded data appears in the lookup.
- `GetCustomersAsync_returns_keys_and_names_in_one_statement` — five rows, one command, one context created
  and disposed, ordered by name, every `Id > 0`, and the SQL text is asserted verbatim.
- `Customer_filter_uses_the_key_not_the_display_text` — filtering by the key returns exactly the tickets a
  separate `CountAsync` finds for that customer.

**Not verified here:** that Wisej.NET's `ComboBox.SelectedValue` returns the boxed `ValueMember` value for
a `DropDownList` bound to a `List<T>` of records — the code follows the documented behaviour, but only the
browser proves it. See the README's *Verified / unverified*.
