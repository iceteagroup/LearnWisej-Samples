# Editable grid operation contract (Module 9, deliverable 2)

**Widget:** `IntegrationLab.Widgets.WorkOrderGrid` (`Widgets/WorkOrderGrid.cs`) hosting the stand-in
`VendorGrid` library (`wwwroot/vendor-grid.js`) with **Kendo-style DataSource** semantics.
**Adapter:** `wwwroot/grid-init.js`. **Server:** the widget's `WebRequest` handler →
`GridDataController.Handle(action, query, body)` → `WorkOrderStore`.

The contract is designed first and the vendor is mapped onto it. It has three parts: the
request/result types, the four operations, and what deliberately stays outside.

## 1. Types (`Contracts/`)

```csharp
public sealed class GridOperationRequest
{
    public const int MaxTake = 100;
    public int Skip { get; set; }                       // remote paging
    public int Take { get; set; } = 20;                 // 1..MaxTake
    public List<SortDescriptor>   Sort   { get; set; }  // remote sorting:   { Field, Desc }
    public List<FilterDescriptor> Filter { get; set; }  // remote filtering: { Field, Op, Value }
}
public sealed class GridOperationResult<T> { IReadOnlyList<T> Rows; int Total; int Skip; int Take; }

public sealed class RowUpdateRequest { string RowKey; Dictionary<string, object> Changes; }
public sealed class RowInsertRequest { Dictionary<string, object> Values; }
public sealed class RowDeleteRequest { string RowKey; }
```

Row shape on the wire (camelCase, `Data/WorkOrder.cs`):
`{ "id":"WO-1001", "asset":"Pump 3", "status":"Open", "priority":"High", "assignee":"Ana", "hours":4.5, "site":"Plant A" }`.
`id` is the key (`schema.model.id` for Kendo, `CustomStore.key` for DevExtreme); the server
assigns it and the client never edits it.

Server-side rules (`WorkOrderStore`), enforced on every call regardless of vendor:

| Rule | Violation |
|---|---|
| `skip >= 0`, `1 <= take <= 100` | 400 |
| sort/filter fields ∈ `id, asset, status, priority, assignee, hours, site` | 400 |
| filter ops ∈ `eq, neq, contains, gt, gte, lt, lte` (numeric ops only on `hours`) | 400 |
| editable fields ∈ `asset, status, priority, assignee, hours, site` (never `id`) | 400 |
| `status`, `priority`, `site` ∈ their allowed lists; `hours` numeric 0..1000; `asset` non-empty ≤ 60 chars | 400 |
| unknown `rowKey` on update/delete | 404 |
| unknown `action` | 400 (checked before anything is parsed) |

## 2. Operations: one contract, mapped three ways

| Operation | Kendo DataSource (what `VendorGrid` does) | DevExtreme CustomStore (equivalent) | Server (`GridDataController.Handle`) |
|---|---|---|---|
| **load** | `transport.read: { url, data: { action: "load" } }` → `GET url&action=load&skip=0&take=20&sort=status asc&filter=[…]` | `load: o => $.getJSON(url, { action:"load", skip:o.skip, take:o.take, sort: JSON.stringify(o.sort) })` | `ParseLoadArguments(query)` → `GridOperationRequest` → `store.Load` → `{ rows, total, skip, take }` |
| **insert** | `transport.create: { url: url + "&action=create" }` GET with `payload={ values }` | `insert: values => $.ajax({ url: url + "&action=create", type:"POST", data: JSON.stringify({ values }) })` | `RowInsertRequest` → `store.Insert` → `{ row }` (server assigns `id`) |
| **update** | `transport.update: { url: url + "&action=update" }` GET with `payload={ rowKey, changes }` | `update: (key, vals) => $.ajax({ …"&action=update", data: JSON.stringify({ rowKey:key, changes:vals }) })` | `RowUpdateRequest` → `store.Update` → `{ row }` or 404 |
| **delete** | `transport.destroy: { url: url + "&action=destroy" }` GET with `payload={ rowKey }` | `remove: key => $.ajax({ …"&action=destroy", data: JSON.stringify({ rowKey:key }) })` | `RowDeleteRequest` → `store.Delete` → `{ rowKey, deleted:true }` or 404 |

`ParseLoadArguments` accepts both sort shapes — Kendo's string (`"status asc,priority desc"`)
and DevExtreme's JSON array (`[{"selector":"status","desc":false}]`) — so an InitScript for
either vendor stays a few lines of translation. The `url` is the widget's postback URL
(`this.getPostbackUrl()` on the client, `GetPostbackURL()` on the server); the pivot in this lab
shows the other entry point (a `[WebMethod]`), and both end in the same `Handle`.

Responses always carry `Content-Type: application/json`. Errors are `{ "status": 400|404, "message": "…" }`
with the same HTTP status — never a stack trace.

## 3. What stays outside the data contract

- **Layout state** (column widths, visible columns, sort the user last chose) is a separate
  per-user object and is *not* part of `GridOperationRequest`. In this lab the typed `Columns`
  list and `Sort` property are application defaults; persisting a user's layout would be its own
  `GridLayoutState { Columns[], Sort }` stored per user and applied through `Options`, never sent
  with a load.
- **Export** is either the vendor's client-side feature (what is on screen) or a server-side
  pipeline that reuses `store.Load` with `take = MaxTake` in a loop; it is not a fifth action.
- **Vendor knowledge** (transport shape, schema, event names) lives only in `grid-init.js`.

## 4. Remote operations: the moment it becomes a real integration

`VendorGrid` never sorts or pages in memory: every page change, header click, edit, insert and
delete is a server call. That is what the Remote operations list makes visible — one
`← JS→.NET load … → 200` line per interaction. Consequences the contract already accounts for:

- every call must be **fast** (the store is indexed by nothing here; a real one needs an index
  for each sortable field) and **bounded** (`MaxTake`);
- every call must be **safe**: whitelists, not the vendor's field names, decide what a filter may
  contain;
- the browser may have a stale view after a 4xx; the vendor re-renders its last server state and
  the user reloads. The server never trusts the row the client thinks it has.

## 5. Configuration: typed properties (the promoted version)

`WorkOrderGrid` exposes `PageSize` (1..100, throws otherwise), `Editable`, `Sort` (validated
against the sortable fields) and `Columns` (`List<GridColumn>` — field, title, width, editable,
type, values). Each setter writes exactly one first-level `Options` field, so Wisej renders the
change and `update(options, old)` re-syncs the vendor. The protected `OnConfigureOptions(dynamic)`
is the escape hatch for the rare vendor option without a typed property.

## 6. Evidence (what the running app shows)

- Page load: `← JS→.NET load {skip:0,take:20} → 200 (20/150)`; the grid shows page 1/8.
- The pager's ›: `load {skip:20,take:20} → 200 (20/150)`.
- A header click: `load {skip:0,take:20,sort:"status asc"} → 200 (20/150)`.
- Double-click a cell, change it, Enter: `← JS→.NET rowUpdated {rowKey:"WO-1043",changes:{status:"Closed"}}`,
  a toast, then `← JS→.NET update {"rowKey":"WO-1043","changes":{…}} → 200 WO-1043 saved`.
- **+ Add row**: `create {"values":{…}} → 200 → WO-1151` followed by a reload.
- A row's ✕: `destroy {"rowKey":"WO-1010"} → 200 WO-1010 removed (149 left)`.
- `abc` in an Hours cell: `update {…} → 400 hours must be a number (received abc).`, then
  `← JS→.NET error {phase:"update",status:400,…}` and an error toast; the cell reverts.
- The 404 (unknown key) and the `take` bound (`take` > 100 → 400) are enforced by the store and
  come back the same way when a request breaks them.
