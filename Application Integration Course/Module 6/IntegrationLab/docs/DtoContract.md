# DTO contract · what crosses the wire and how it is named

Deliverable 3 of the Module 6 lab: the DTO used for the client-state return, the serialization
rules it relies on, what must never cross the wire, and how the contract is versioned.

Written **before** the calls that carry them, as the lesson asks: once the shapes are fixed, both
`SimpleGauge.cs` and `gauge-init.js` have something concrete to be tested against.

## The two DTOs

### `Contracts/GaugeStateDto.cs` — returned by `await CallAsync("getSelectedState")`

| C# property | Type | JavaScript name on the wire | Meaning |
|---|---|---|---|
| `Value` | `double` | `value` | what the vendor gauge shows right now (mid-sweep while animating) |
| `IsAboveThreshold` | `bool` | `isAboveThreshold` | shown value ≥ the threshold the server configured |
| `Width` | `int` | `width` | rendered width, CSS px |
| `Height` | `int` | `height` | rendered height, CSS px |
| `IsAnimating` | `bool` | `isAnimating` | sweep or settle animation running — **client-only state** the server never owns |

Wire example: `{"value":72,"isAboveThreshold":false,"width":480,"height":244,"isAnimating":false}` — 74 bytes, one trace line.

### `Contracts/RenderedSize.cs` — returned by `await CallAsync("getRenderedSize")`

| C# property | Type | Wire name |
|---|---|---|
| `Width` | `int` | `width` |
| `Height` | `int` | `height` |

Both classes are `sealed`, hold primitives only, have no behavior and no references to anything
else. Both are produced on the client by a function that builds a fresh plain object — never
`this.widget`, `this.host` or a DOM node.

## Camel-casing: PascalCase in, camelCase out

Every argument passed to the client and every object placed in `Options` goes through the
Wisej.NET JSON serializer, which converts property names to camel case
(`WisejSerializerOptions.CamelCase`, the default):

| C# (server) | JavaScript (client) |
|---|---|
| `SelectedValue = 72` | `selectedValue: 72` |
| `IsAnimating = true` | `isAnimating: true` |
| `MaxValue = 120` | `maxValue: 120` |
| `IsAboveThreshold` | `isAboveThreshold` |
| `Width` / `Height` | `width` / `height` |

The reverse direction has its own rule: a JavaScript object returned from `CallAsync` arrives as a
`dynamic` value that keeps its **JavaScript names**. Read it by those names or map it once into a
typed DTO:

```csharp
dynamic result = await CallAsync("getSelectedState");
result.width    // 480
result.Width    // null — the member does not exist; no exception, no warning, just wrong data
```

Depending on the dynamic implementation a missing member may throw a binder exception instead of
yielding `null`. Either way the fix is the same: map once, by JavaScript names, into the DTO
(`GetSelectedStateAsync` does exactly that), and never touch the dynamic object again.

Other values to check in every contract: **dates** (send ISO strings or ticks, not `DateTime`
objects you have not tested), **numbers beyond 2^53** (`long` ids lose precision in JavaScript —
send them as strings), **enums** (decide once: number or name).

## What must never cross the wire

A persistence-style entity — a work order with a `Lines` collection and a `Customer` navigation
property — serializes everything it references when it is placed in `Options` or passed to `Call`:
internal remarks, the approver, the customer's e-mail, tax id and credit limit. The gauge needs none
of it. An entity with a navigation property serializes the navigation target too, so the safe habit
is: **never pass a domain object across the wire, only an object built for the crossing.** Anything
security-sensitive, anything the widget does not render, anything that changes when the database
schema changes stays on the server.

Checklist for any object placed in `Options`, passed to `Call`, or returned from a client function:

- [ ] primitives (and arrays/objects of primitives) only
- [ ] every property is read by the other side
- [ ] no navigation properties, no entities, no `IQueryable`, no `Control` references
- [ ] no secrets, tokens, connection strings, internal remarks
- [ ] small enough to read in one trace line

## Versioning rules

Small DTOs are stable because they change rarely. When they do:

| Change | Compatible? | Rule |
|---|---|---|
| add a property | yes | the client ignores what it does not know; the server mapper ignores what it does not read |
| rename a property | **no** | breaking — treat as a new contract version; ship the old name alongside for one release or bump the wrapper and the server together |
| remove a property | **no** | breaking — same as rename |
| change a type (`int` → `string`) | **no** | breaking — validation on the receiving side must change too |
| change semantics (`value` is now Celsius) | **no** | breaking even though the JSON looks identical — document it, version it |

A DTO with five properties is versioned in minutes. A DTO that mirrors a domain entity would change
every time the entity does, and every change would be a chance to break the client silently.

## Evidence

- **Get selected state** → the command trace shows `→ await CallAsync("getSelectedState")` and then
  `← result {"value":72,"isAboveThreshold":false,"width":480,"height":244,"isAnimating":false}`: the
  camelCase wire shape of `GaugeStateDto`. Click it right after **Reset animation** and `isAnimating`
  is `true`: client-only state the server can only ask for.
- **Read rendered size** → `← result {"width":480,"height":244}`, mapped into `RenderedSize`.
