# Command palette script

Two files, two jobs. Both live in `Interop/` next to the contract they implement.

| File | How it ships | What it is |
| --- | --- | --- |
| [`Interop/palette.client.js`](../Interop/palette.client.js) | **Widget Package**, served as a static file from the project folder at `/Interop/palette.client.js` | The palette "library": hotkey capture, filtering, selection, feature detection. Registers `window.EnterpriseOpsPalette`. |
| [`Interop/command-palette-host.js`](../Interop/command-palette-host.js) | **InitScript**, an embedded resource (`EnterpriseOps.Interop.command-palette-host.js`, see the csproj) handed to the widget by `CommandPaletteHost` | The adapter between the qooxdoo widget wrapper and the library: options, events, the two remote calls, the lifecycle. |

Splitting them is not decoration. The library is servable and readable in the browser's dev tools,
which is how a reviewer confirms the claim in the security notes: **there is no business rule in the
client script at all**. The adapter is the only place that knows the server exists.

---

## What the script is allowed to do

* capture `Ctrl+K`, filter the command list while the user types, move the selection, highlight a match
* detect what the browser supports and report it as data
* hand a **named command id** to the server

## What the script must never do

* decide whether a command may run — the `allowed` flag it receives is a display hint
* change a work order's state — no status, no assignee, no version crosses from the browser
* invent a payload field — three fields, and the server drops anything else

The lesson's review test: *what would break if the browser lied?* Every answer here is "the screen
looks wrong", never "the wrong thing ran". A user who edits the client list to unlock a greyed-out
row still hits gate 3 and gets `PERMISSION_DENIED` — that is the failure path the walkthrough shows.

---

## Lifecycle — "what happens before the target widget exists?"

**Nothing.** Concretely:

| Moment | What the script does | Where |
| --- | --- | --- |
| page load | nothing at all — the file is loaded, no handler is attached | — |
| `init(options)` | builds the resting card in `this.container`, creates the palette (which appends **one** overlay element to `document.body`), attaches `document.addEventListener("keydown", …, true)` | `command-palette-host.js` → `this.init` |
| one tick later | fires `paletteReady` and sends the first capability report | `setTimeout(…, 0)` in `init` |
| `_addListener(name, handler)` | stores the framework's handler per wired event; calling it is what crosses the wire | `this._addListener` |
| `update(options, old)` | re-syncs `hotkey` and `entityId` only | `this.update` |
| `dispose()` | removes the keydown handler, destroys the palette (which removes its overlay from `document.body`), removes the resting card, **then** delegates to the framework's own dispose | `this.dispose` wrapping `frameworkDispose` |

`paletteReady` is fired a tick late on purpose: a `fireWidgetEvent` raised synchronously while the
framework is still initialising (or during `update()`) is dropped. The reliable routes are the
handler stored by `_addListener` or a deferred `fireWidgetEvent` — `this._fire` uses the first and
falls back to the second.

### Every attach has a matching detach

The palette's modal lives on `document.body`, not inside the widget's own DOM subtree, because a
modal clipped by an absolutely-positioned qooxdoo container is not a modal. That makes the detach
mandatory: `Palette.destroy()` removes its three DOM listeners *and* the overlay node. Without it a
long session would accumulate dead overlays holding references to disposed widgets, and the errors
would surface on some unrelated screen.

### The server side of the same rule

`CommandPaletteHost.Send(...)` refuses to call a widget that does not exist yet: it checks
`PaletteReady && IsLoaded`, queues the call, and flushes the queue when
`paletteReady` arrives. Nothing is silently lost and nothing throws.

---

## The two remote calls

```js
// 1. list — the widget's own [WebMethod], registered by RegisterWebMethods(config)
this.GetCommandCatalogAsync(query)     // → [{ Id, Title, Shortcut, RequiresEntity, Allowed }]

// 2. run — the ONE remote method the palette may call, on the top-level page
App.MainPage.RunClientCommandAsync(commandName, entityId, correlationId)
```

Notes that cost real debugging time:

* **Never pass `null`.** The Wisej client wrapper calls `getId` on every argument, so an absent
  entity id is `""`. The adapter coerces every argument with `String(...)` for that reason.
* **Return values keep their .NET casing** (unlike `Options`, which is camel-cased on the way out).
  `_loadCatalog` and `_send` read `r.Id ?? r.id` and `result.Code ?? result.code` so either shape works.
* **A `null` result means the server threw.** The Promise resolves — it does not reject — so the
  adapter treats `null` as `SERVER_ERROR` and never as success.
* **Never redefine a framework method** on the wrapper (`getWidth`, `resize`, `getValue`, `destroy`…).
  Every function this adapter adds is prefixed: `paletteOpen`, `paletteClose`, `paletteCollect`, `paletteShowResult`.

## Events out (`WiredEvents`)

| Event | Payload | Why it is worth a round trip |
| --- | --- | --- |
| `paletteReady` | `{hotkey, contractVersion}` | the lifecycle signal the server waits for |
| `capabilities` | `{report}` | the capability report |
| `error` | `{phase, message}` | the script failed; the server state is unchanged |

A keystroke does **not** raise an event, and neither does opening or closing the palette. A palette that reported every keypress would flood the
session with round trips and tell the server nothing it can act on; the catalogue call while typing
is a query, and only the selection is a business fact.

---

## Evidence — what the running app shows

| Path | How to reproduce | What proves it |
| --- | --- | --- |
| Attach after creation | load the page | the capability panel fills a moment after load: the first report is sent from the `setTimeout` in `init`, i.e. after the widget exists |
| Real browser round trip | press Ctrl+K, type *escalate*, Enter | the resting card shows `→ workorder.escalate WO-1040 corr …` then `← OK · Escalated (v2).`; server log `Client: App.MainPage.RunClientCommand("workorder.escalate", …)` |
| The display hint is only a hint | press Ctrl+K (the session is `ben.tech`, a Technician) | "Approve work order…" is greyed with *needs a higher role* — run it anyway and the server answers `PERMISSION_DENIED` |
| Detach | close the browser tab / dispose the page | the `document` keydown handler and the `.eop-overlay` node are removed in `dispose()` before the framework's own dispose runs |
