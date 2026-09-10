# Security review notes — the interop boundary

Reviewed: `Interop/`, `Services/ClientCommandService.cs`, `Security/`, and the two `[WebMethod]`s on
`UI/CommandCenterShell.cs`. Contract version 1.0.

---

## 1. Which values from JavaScript are trusted?

**None.** In full:

| Value the browser can send | Trusted? | What the server does |
| --- | --- | --- |
| `commandName` | no | matched against a published catalogue; anything else is `UNKNOWN_COMMAND` |
| `entityId` | no | shape-checked, then resolved **inside the session tenant**; a foreign or absent id is `INVALID_TARGET` |
| `correlationId` | no | shape-checked, then used **only** as a log key; it grants nothing |
| the `Allowed` flag on a catalogue row | no | it is a display hint the server itself sent; the permission is re-checked at gate 3 |
| the capability report | no | unknown keys dropped, values sanitised and bounded; never consulted for a decision |
| claimed role / claimed status (anti-pattern endpoint) | **believed — that is the bug being demonstrated** | see §5 |

Identity, role and tenant are read from `SessionContext` on **every** call. `SessionContext` is
per-session, created in the page constructor, and its setters are only reachable from the header
combo boxes — never from a payload.

## 2. Which business rules remain server-side?

All of them:

* **Existence** — the command catalogue (`InteropContract.Catalog`).
* **Arity** — whether a command takes an entity.
* **Authorisation** — `PermissionService`, from the session role only.
* **Tenancy** — every repository query takes a tenant id; `IWorkOrderRepository` has no
  "find by id" without one, so a forged id from another tenant cannot resolve to anything.
* **State transitions** — `WorkOrderService.Approve/Reassign/Escalate`, including the version bump.
* **Fallback selection** — `BrowserCapabilityService` decides what a missing capability means.

The client script holds keystrokes, filtering, selection and feature detection. Nothing else.

## 3. Gate order

```
shape → catalogue → arity → permission → target → state
```

Permission is checked **before** the target is resolved. If it were the other way round, a user who
may not approve could still distinguish `INVALID_TARGET` from `INVALID_STATE` and use the endpoint to
enumerate which work orders exist in a tenant they cannot see. For the same reason "does not exist"
and "belongs to another tenant" return the **same** code and the same message.

## 4. Information leaked to the browser

| Kind | Leaked? | Note |
| --- | --- | --- |
| exception message / stack trace | no | `catch` answers `SERVER_ERROR` with a fixed sentence |
| internal database keys | no | ids cross as `WO-1040`; `InteropContract.ToEntityId` / `TryReadEntityKey` translate |
| the reason a permission was denied | partially, on purpose | the user is told *what* they cannot do, not which role would be needed; the full reason (`WorkOrder.Approve requires Manager or Admin; ben.tech is Technician`) stays in the server trace and the audit row |
| whether an id exists in another tenant | no | same answer either way |
| the payload, echoed back | no | `Message` never quotes what was sent; the trace prints a bounded, control-character-stripped copy |

## 5. The anti-pattern, and why it is in the sample

`RunTrustedClientCommand` believes a `claimedRole` and a `claimedStatus` from the payload. Pressing
**⚠ Trust the client** shows the whole cost in one screen:

* `PermissionService.CheckWithClaimedRole` runs a check that *looks* like security and is not — the
  role it is checking came from the browser. (It is a separate method so the correct one cannot be
  called with client data by mistake.)
* `WorkOrderService` never runs, so there is no state rule, no version bump, no `Save`.
* The audit row reads `TAMPERED` and carries a **before-snapshot**, which is the only reason the
  damage is recoverable at all.

Recovery: **Revert tampered** restores the snapshot and appends a `REVERTED` row. In production the
snapshot would be the difference between "we restored it" and "we do not know what it was".

Review heuristic from the lesson: *what would break if the browser lied?* On the contract path the
answer is "a wrong-looking palette". On this path the answer is "a wrong approval" — which is exactly
the signal that the logic is in the wrong place.

## 6. Denial of service and payload bounds

| Bound | Value | Where |
| --- | --- | --- |
| `commandName` | 40 chars | `InteropContract.MaxCommandNameLength` |
| `entityId` | 12 chars | `MaxEntityIdLength` |
| `correlationId` | 8 chars, hex only | `MaxCorrelationIdLength` |
| catalogue query | 60 chars | `ClientCommandService.GetVisibleCatalog` |
| capability report | 800 chars, each value ≤ 40 | `BrowserCapabilityService.Accept` |
| trace / audit lines | bounded and control-character stripped | `CommandCenterShell.Safe`, `BrowserCapabilityService.Sanitize` |

Shape validation runs before any lookup, so an oversized payload costs a regex, not a query. The
palette does not raise an event per keystroke; only a selection crosses the wire.

## 7. Lifecycle as a security property

A handler attached before its target exists, or left attached after the widget is disposed, is not
only a bug — it is a component still reacting to input on behalf of a session that may be gone. The
script attaches in `init()` and detaches in `dispose()` (listener **and** overlay node), and the
server queues callbacks until `paletteReady`. See `docs/CommandPaletteScript.md`.

## 8. Findings and residual risk

| # | Finding | Severity | Status |
| --- | --- | --- | --- |
| 1 | `RunTrustedClientCommand` accepts a role and a status from the payload | **critical** | intentional teaching artefact — must not ship; delete the method, `ExecuteTrustingClient` and `CheckWithClaimedRole` together |
| 2 | The catalogue row carries `Allowed` to the browser | low | accepted: it is a UX hint, re-checked at gate 3; it discloses only what the user's own role already implies |
| 3 | The denial message tells the user which action was refused | low | accepted: a user must know what they cannot do; the *reason* stays server-side |
| 4 | The palette's overlay lives on `document.body` | low | accepted: required for a modal; removed in `dispose()` |
| 5 | Fake data and a per-session in-memory store | n/a | lab scope: no database, no network, no cloud account |
| 6 | No rate limiting on `RunClientCommand` | medium | out of scope for the lab; in production the endpoint would sit behind the same per-session throttle as the rest of the application |

## 9. Review checklist for the next interop method

- [ ] Does the method have a name, a purpose, an input schema, an output schema, a validation rule and a named failure behaviour?
- [ ] Is every field bounded and type-checked before any service is called?
- [ ] Does identity come from the session, and only from the session?
- [ ] Is the permission checked before the target is resolved?
- [ ] Do "not found" and "not yours" answer identically?
- [ ] Does the failure path return a named code instead of throwing?
- [ ] Is every outcome — allowed, denied and rejected — audited with user, tenant, correlation id and command?
- [ ] Does every `addEventListener` / appended node have a matching removal in `dispose()`?
- [ ] Is the callback safe to run twice, and safe when the widget is missing or disposed?
- [ ] Would the answer to "what would break if the browser lied?" be cosmetic?

---

## Evidence — what the running app shows

| Claim | How to reproduce | What proves it |
| --- | --- | --- |
| The role comes from the session | switch the user combo, then **Run approve** | trace `Security: session identity is now ben.tech (Technician) — the payload has no say in this`, then `… role from session … DENIED` |
| The command never ran | same | no `Service:` line follows the denial; the work order's status and version are unchanged |
| Denials are audited | same | audit row `DENIED ben.tech contoso <corr> workorder.approve WO-1040` |
| Tenancy holds | **Other tenant's WO** | `Data: … Find(tenant=contoso, id=1060) → not found` → `INVALID_TARGET` |
| The anti-pattern is real | **⚠ Trust the client** | audit `TAMPERED`, banner *the browser just changed a work order's state*, `Version still 1` in the trace |
| The damage is recoverable | **Revert tampered** | audit `REVERTED`, status back to its previous value |
| Capabilities grant nothing | **Forged capabilities**, then **Run approve** as `ben.tech` | keys ignored, and the denial is unchanged |
