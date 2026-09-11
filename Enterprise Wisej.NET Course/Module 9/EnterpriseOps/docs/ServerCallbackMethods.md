# Server callback methods

Traffic crosses the boundary in both directions, and both directions are the same trust boundary.
This document lists every method on each side, where it is registered, and what it guarantees.

---

## 1. Client → server: `[WebMethod]`

Wisej discovers `[Wisej.Core.WebMethod]` automatically on **top-level containers** (a `Page`, `Form`
or `Desktop`) and on static `Program` methods. A **child control** registers its own by overriding
`OnWebRender` and calling `RegisterWebMethods(config)`; the render then carries `config.webMethods`
and the client gets the functions on the widget wrapper.

Each registration produces **two** client functions: `Name(args…, callback)` and
`NameAsync(args…) → Promise`.

| Method | Registered on | Client call | Returns |
| --- | --- | --- | --- |
| `RunClientCommand(commandName, entityId, correlationId)` | `CommandCenterShell` — top-level Page, automatic | `App.MainPage.RunClientCommandAsync(…)` | `ClientCommandResult` |
| `GetCommandCatalog(query)` | `CommandPaletteHost` — child control, `RegisterWebMethods` in `OnWebRender` | `this.GetCommandCatalogAsync(query)` on the wrapper | `[{Id,Title,Shortcut,RequiresEntity,Allowed}]` |

Both styles are in the sample on purpose: the *execution* endpoint belongs to the page (there is
exactly one, and it is easy to find in a review), while the *listing* endpoint belongs to the widget
that needs it, so a second palette on another screen would carry its own.

### Rules every WebMethod in this project follows

1. **Validate the shape first.** `RunClientCommand` calls `InteropContract.TryParse` before anything
   else. A malformed payload costs a regex, not a service call, and never reaches a repository.
2. **Never trust a field.** Identity, role and tenant come from `SessionContext`. The correlation id
   is echoed for log correlation only; it grants nothing.
3. **Delegate the decision.** The method parses and calls `IClientCommandService`. Business rules are
   reviewable in `Services/`, without opening the designer.
4. **Never leak.** `catch (Exception)` answers `SERVER_ERROR` with a user-readable message. The
   exception type goes to the server log; the message, the stack and the internal ids do not go to the browser.
5. **Never `null` arguments.** The client wrapper calls `getId` on each argument, so the script sends
   `""`. Default parameter values are not supported by the marshaller either.
6. **Attributable.** Every call writes user, tenant, correlation id, command and outcome to `AuditLog`
   — allowed, denied and rejected alike.

### Argument and return marshalling

* Arguments are marshaled into the typed parameters; a server `ArgumentException` surfaces as the
  Wisej exception popup and the awaiting Promise resolves to `null`. This project therefore **returns
  a named failure instead of throwing** — a contract failure is data, not an exception.
* Return values are **not** camel-cased (`Succeeded`, `Code`, `Message`), unlike `Options`, which is.

---

## 2. Server → client: `Control.Call`

Every server → client call on `CommandPaletteHost` goes through one private method, `Send(call)`:

```csharp
if (!PaletteReady || !this.IsLoaded) { _pending.Add(call); return; }
try { call(); } catch (ObjectDisposedException) { }
```

| Server method | Client function | Purpose |
| --- | --- | --- |
| `OpenPalette()` / `ClosePalette()` | `paletteOpen` / `paletteClose` | open the palette without a keyboard |
| `CollectCapabilities()` | `paletteCollect` | re-run feature detection |
| `ShowResult(code, message)` | `paletteShowResult` | write the server's answer into the palette footer |
| `TargetEntityId` (property) | `update(options, old)` | push the target through `Options`, not through a call |

`CommandCenterShell` sets `TargetEntityId` on load; the `Call`-based methods are the widget's public
API for screens that need them.

**Assume the widget may be missing, disposed, or not yet initialized**, and make every callback safe
to run twice: `paletteOpen` returns immediately if the palette is already open, `paletteShowResult`
overwrites one element, and `paletteCollect` recomputes a report from scratch. None of them appends.

---

## 3. Attributability

Both directions carry the same four facts, so a support engineer can reconstruct a crossing from
either end:

| Fact | Client side | Server side |
| --- | --- | --- |
| user | — (the browser is not told who it is) | `SessionContext.UserName` |
| tenant | — | `SessionContext.TenantId` |
| correlation id | generated per command, shown in the resting card | echoed into `CommandContext` and every audit row |
| command | the catalogue id | the catalogue id |

---

## Evidence — what the running app shows

"Server log" is the `System.Diagnostics.Trace` output of `ActivityTrace`.

| Path | How to reproduce | What proves it |
| --- | --- | --- |
| Automatic page registration | Ctrl+K → any command → Enter | server log `Client: App.MainPage.RunClientCommand(…)` |
| `RegisterWebMethods` on a child | press Ctrl+K and type | server log `Interop: GetCommandCatalog(query="app") → 1 of 6 commands (display filter only)`; the palette list narrows |
| Deferred callback | call `commandPaletteHost.ShowResult(…)` from the page's `Load` | the call is queued until `paletteReady`, then the palette footer and the resting card show it |
| Failure isolation | dev tools: `App.MainPage.RunClientCommandAsync("workorder.approve", "WO-1040-INJECT", "not-a-corr")` | the Promise still resolves with `MALFORMED_PAYLOAD`, and no exception popup appears |
| Both directions audited | any command | one `Audit:` line per crossing in the server log: outcome · user · tenant · correlation · command · entity |
