# Deliverable 3 — Payload contract table

Module 7 · Events & Handler Contracts. An event payload is a contract: the client adapter
promises a name and a small, camel-cased object; the server treats every field as untrusted,
validates it, and looks up anything sensitive itself. The application depends on this table,
never on the vendor's event argument shape.

## The contract

| Event (client → server) | Source widget | Fields (JSON type) | Fires when | Becomes (.NET) | Server validation |
|---|---|---|---|---|---|
| `thresholdCrossed` | gauge (`gauge-init.js`) | `value: number` — the reading at the crossing<br>`level: "warn" \| "high"` — which line was crossed | the reading crosses `WarnAt` or `Threshold` **upward**; rising edge, once per crossing; never while it stays above; never on the way down | `GaugeWidget.ThresholdCrossed` → `GaugeThresholdEventArgs { Value, Level }` (raised from `OnWidgetEvent`) | `value` finite and within `Minimum..Maximum`; `level` ∈ {warn, high}; `value` ≥ the server's line for that level; `Value` in the DTO is the **server** value |
| `valueChanged` | knob (`knob-init.js`) | `value: number` — the new value<br>`source: "user" \| "server"` — who caused it | the dial value changes: a user drag/wheel (`user`), or the echo of a server `Options.value` change (`server`) | page-level `knob_WidgetEvent` switch → `OnValueChanged(KnobValueEventArgs { Value, Source })` | `value` finite, within `Minimum..Maximum`, on the `Step` grid; `source` ∈ {user, server}; committed with `AcceptClientValue` (no re-render, no echo) |
| `pointClicked` | chart (`chart-init.js`) | `index: number` (integer) — position of the point<br>`label: string` — its category<br>`value: number` — its series value | the user clicks a point circle (vendor `pointclick`); hover, zoom, render, layout and legend clicks are **not** forwarded | `ChartWidget.PointClicked` → `ChartPointEventArgs { Index, Label, Value }` (raised from `OnWebEvent`) | `index` integer within `0..Labels.Length-1`; `label` string; `value` finite; **index is a lookup key** — `Label`/`Value` are read from server data, client copies only compared |
| `error` (all three) | any adapter | `phase: "init" \| "update"`<br>`message: string` | the adapter caught a vendor exception | logged as `✖ vendor failure`, no .NET event | strings only, displayed, never executed |

All names are `WiredEvents` entries on the server (`GaugeWidget`: `thresholdCrossed, error`;
`KnobWidget`: `valueChanged, error`; `ChartWidget`: `pointClicked, error`). Nothing else may arrive;
an unknown name on the knob's page handler is logged as `rejected: not in the contract`.

## What the payloads deliberately leave out

| Vendor gives | Contract keeps | Dropped because |
|---|---|---|
| `VendorGauge.rangechange { range, previous, value }`, `thresholdexceeded { value, threshold }` | `{ value, level }` | `previous`/`threshold` are server state already; the edge is decided by the adapter, not the vendor |
| `knobchange` `CustomEvent` (`detail.value`, DOM target, bubbling) | `{ value, source }` | a DOM event is not serializable, and the server does not care which element fired it |
| `pointclick { seriesIndex, index, label, value, x, y, domEvent }` | `{ index, label, value }` | pixel coordinates, the series index and the DOM event are vendor/browser details; a different chart library would not have the same |

## Versioning

- Names are stable strings the integration chose (`thresholdCrossed`, not the vendor's `thresholdexceeded`).
- Adding a field is backward compatible (the server ignores what it does not read); renaming or removing one is a new contract and a new `WiredEvents` name (`pointClicked2`) while both are handled.
- Payloads are built from **a few values the wrapper extracts**, so a vendor upgrade that changes its event argument shape touches one function in one adapter (`_getEventData` / the `pointclick` handler) and nothing in the application.

## The three review questions

1. **Does the payload contain only primitives and small objects?** Yes — numbers and short strings; no vendor objects, DOM nodes or whole data rows. The DTOs in `Contracts/` mirror this: `double`, `int`, `string`, nothing else.
2. **Could the same payload be produced by a different vendor library?** Yes — any gauge with a change callback, any knob with a value event, any chart with a click callback can fill these three shapes; the edge logic (`above`/`warned`) and the source detection (`_applying`) live in the adapter, not in the vendor.
3. **Is every field validated on the server before it is used?** Yes — `PayloadReader.TryDouble/TryInt/TryString` guard type and finiteness, the widgets check ranges against **server** limits, and a lookup key (`index`) resolves to server data. A row key in a payload is a lookup key, never proof that the user may act on that row: the server decides.

## Evidence

The "Server WidgetEvent log" prints every payload exactly as received (`e.Data = {…}`) followed by
the DTO that was built from it, or the `✖ rejected …` reason. "Bad payload" (`{ index: -1 }`) is the
failure path; "Noise counter" shows how many vendor events never became payloads at all.
