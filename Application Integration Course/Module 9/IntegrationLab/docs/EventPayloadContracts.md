# Event and operation payload contracts (Module 9, deliverable 4)

Editable grids need more than `load`. Two kinds of payload cross the wire from the browser:
**events** (the user did something; raised through `fireWidgetEvent` and handled in
`OnWidgetEvent`) and **operations** (the vendor persists something; sent over HTTP to the postback
handler). Both are small camelCase objects, both are validated on the server, and neither carries
behavior — only data.

## 1. The four payloads

| Name | Payload (camelCase) | Kind | Fires when | Where it lands (.NET) | Validation on the server |
|---|---|---|---|---|---|
| `cellClick` | `{ rowKey, field, value }` | event | the user clicks a cell (vendor `cellclick { row, field, value }`) | `WorkOrderGrid.OnWidgetEvent` → `CellClick` event → `CellClickEventArgs { RowKey, Field, Value }` | `rowKey` matches `WO-…` and `field` is a known field; otherwise the event is dropped |
| `rowUpdate` | `{ rowKey, changes }` | event **and** operation | the user commits an inline edit (Enter/blur). The vendor first emits `rowupdate { row, changes }` (intent), then GETs `&action=update&payload={ rowKey, changes }` (persistence) | event: `OnWidgetEvent("rowUpdated")` → `RowUpdated` → `RowUpdatedEventArgs { RowKey, Changes }`; operation: `HandleWebRequest` → `Handle("update")` → `WorkOrderStore.Update` | event: key shape + every key of `changes` is editable; operation: full value validation, 404 on unknown key |
| `rowInsert` | `{ values }` | operation | the user saves the "Add row" form (vendor emits `rowinsert`, then GETs `&action=create&payload={ values }`) | `HandleWebRequest` → `Handle("create")` → `WorkOrderStore.Insert` → `{ row }` with the server-assigned `id` | every key insertable, values in range; `id` is never accepted |
| `rowDelete` | `{ rowKey }` | operation | the user clicks a row's ✕ (vendor emits `rowdelete`, then GETs `&action=destroy&payload={ rowKey }`) | `HandleWebRequest` → `Handle("destroy")` → `WorkOrderStore.Delete` | non-empty key, 404 on unknown key |

One more contract event completes the grid's `WiredEvents = { cellClick, rowUpdated, error }`:

| Name | Payload | Fires when | Lands in |
|---|---|---|---|
| `error` | `{ phase, status, message }` | the adapter caught a vendor exception (`phase` = init/update/read/create/update/destroy…, `status` = HTTP status or 0) | `WidgetError` → `DataWidgetErrorEventArgs`; the page shows an error toast |

And the pivot's (`WiredEvents = { cellClick, dataLoaded, error }`):

| Name | Payload | Lands in |
|---|---|---|
| `cellClick` | `{ rowKey, columnKey, value }` | `WorkOrderPivot.CellClick` → `PivotCellClickEventArgs` |
| `dataLoaded` | `{ rows, columns, cells }` (counts) | `WorkOrderPivot.DataLoaded` |
| `error` | `{ phase, status, message }` | `WorkOrderPivot.WidgetError` |

## 2. Why `rowUpdate` is both an event and an operation

The event tells the application *that the user decided something* (audit, a toast, enabling a
Save-all button) and fires regardless of whether persistence succeeds. The operation is *the
persistence itself* and is answered with the row the server actually stored — which may differ
from the change the user typed (normalized casing, rounded hours) or be refused (400/404). The
`RowUpdatedEventArgs` doc comment says so explicitly: read the persisted values from the store,
not from the event.

`rowInsert` and `rowDelete` are operations only: the postback handler's operation line
(`create … → 200 → WO-1151`, `destroy … → 200`) already tells the application everything the
vendor's `rowinsert`/`rowdelete` events would, so they are not forwarded as widget events. Adding
them later is one line in `_vendorEvents` and one `case` in `OnWidgetEvent`.

## 3. Translation (vendor → contract) is one function

`grid-init.js`:

```js
this._vendorEvents = { cellClick: "cellclick", rowUpdated: "rowupdate" };
this._getEventData = function (type, e) {
    switch (type) {
        case "cellClick":  return { rowKey: e.row && e.row.id, field: e.field, value: e.value };
        case "rowUpdated": return { rowKey: e.row && e.row.id, changes: e.changes };
        case "error":      return { phase: e.phase, status: e.status, message: e.message };
    }
    return null;
};
```

The vendor's whole `row` object never leaves the browser; only the key does. The vendor's noisy
`hover { row, field }` and `scroll { top }` events are not wired at all — the contract lists what
is meaningful, and everything else stays local.

Events are registered through the framework's `_addListener(name, handler)` pattern, so a vendor
event raised while a server-side `Options` change is being applied (e.g. a `cellclick` during
`update()`) is deferred by the framework instead of being dropped.

## 4. Server-side handling

`WorkOrderGrid.OnWidgetEvent` (`Widgets/WorkOrderGrid.cs`) reads each field by its JavaScript
name from `e.Data`, normalizes it (`JsonCodec.NormalizeValue` / `ToDictionary`), lists the raw
payload (`← JS→.NET rowUpdated {rowKey:"WO-1043",changes:{status:"Closed"}}`), validates it, and
only then raises the .NET event. Unknown event types go to `base.OnWidgetEvent(e)` — never
swallowed.

## 5. Evidence (what the running app shows)

- Click any cell: `← JS→.NET cellClick {rowKey:"WO-1007",field:"status",value:"Open"}` in the
  Remote operations list.
- Double-click a status cell, pick "Closed", Enter: `rowUpdated {…}`, a toast
  `WO-1007 changed: {status:"Closed"}`, then the `update … → 200 WO-1007 saved` operation line.
- Type `abc` into an Hours cell: the event still fires (`changes:{hours:"abc"}`), the operation
  comes back `→ 400 hours must be a number (received abc).`, the `error` event follows (error
  toast) and the cell reverts to the last server value.
