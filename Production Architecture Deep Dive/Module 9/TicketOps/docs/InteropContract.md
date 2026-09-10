# Interop contract — the JavaScript ⇄ C# surface of the TicketOps Console

*Module 9 deliverable · every interop point named, typed and kept small*

"Treat the boundary between JavaScript and C# exactly like a public method: few parameters, clear names,
and never trust what comes across it." This file is that method list. If it is not here, the browser and
the server do not talk about it.

## Where the pieces live

| Piece | File | Technique (lesson §4 cheat-sheet) |
|---|---|---|
| The only client script | `Platform/ticketops.interop.js` | **Embedded resource** — bundled by Wisej.NET (`[assembly: WisejResources]`, `<EmbeddedResource Include="Platform\*.js" />`) |
| Attaching it to the search box | `Views/WorkOrdersView.Designer.cs` → `javaScript.SetJavaScript(searchBox, "ticketOps.attachSearchShortcuts(this);")` | **JavaScript extender** — runs when the widget is created, `this` = the widget |
| Server callback for the shortcut | `Controls/GlobalSearchBox.cs` → `[WebMethod] ReportShortcut(string)` (`OnWebRender` + `RegisterWebMethods`) | **Remote method / WebMethod** — JS calling C# by name |
| Browser API calls | `Infrastructure/BrowserApi.cs` → `CopyToClipboardAsync(string)` | **`Application.EvalAsync`** — one-shot, expression only, awaited |
| The decision | `Services/TicketLinkService.cs`, `Domain/WorkOrder.CanShareLink` | plain C# — no interop at all |
| The record | `Services/AuditLogService.cs` | plain C# — written after confirmation |

## The three calls

### C1 · `ticketOps.attachSearchShortcuts(widget) : boolean` — server → client, once per widget

| | |
|---|---|
| Called by | the `JavaScript` extender script attached to `searchBox`, when the widget is created (and again on refresh) |
| Argument | `widget` — the `GlobalSearchBox` client widget (`this` in the extender script) |
| Effect | registers one capture-phase `keydown` listener on `window`: **Ctrl/Cmd+K** → `preventDefault`, `widget.focus()`, select all, then S1; **Esc** → close the `?` overlay, or clear a focused non-empty box (`widget.setValue("")`, so the server sees `TextChanged`); **?** (outside inputs) → toggle the client-only shortcut overlay |
| Returns | `true` when attached, `false` (with a console warning) when `this` was not a widget |
| Idempotent | yes — removes the listener it registered before |
| Never | reads or holds application data; builds URLs; decides anything |

### S1 · `GlobalSearchBox.ReportShortcut(string shortcut) : bool` — client → server ([WebMethod])

| | |
|---|---|
| Client call | `widget.ReportShortcutAsync("ctrl+k")` → Promise\<bool\> (falls back to `widget.ReportShortcut("ctrl+k", callback)`) |
| Registered by | `OnWebRender(dynamic config) { base.OnWebRender((object)config); RegisterWebMethods(config); }` on the control — a child control, so it opts in explicitly |
| Validation | trim, lower-case, length ≤ 16, must be in `{ "ctrl+k" }`; otherwise `false` and a `[CLIENT] ⚠ refused … (n chars)` line — the payload is never logged |
| Effect | `[CLIENT] Ctrl+K → focus search`, raises `ShortcutPressed` → the screen logs `[UI] … acknowledged` and sets the status **● search focused (Ctrl+K)** |
| Returns | `true` accepted, `false` refused |
| Authorizes | nothing — a focus change has no business meaning. A shortcut that *acts* must go through the same service method as the button (see C2) |

### C2 · `ticketOps.copyToClipboard(text, options) : Promise<{ ok, reason?, detail? }>` — server → client → server

| | |
|---|---|
| Called by | `BrowserApi.CopyToClipboardAsync(text)` through `Application.EvalAsync("ticketOps.copyToClipboard(<literal>, <options>)")` — an **expression** (never `return …`); the returned Promise is awaited by Wisej.NET |
| `text` | the finished link built and signed by `TicketLinkService.BuildLinkAsync` — passed as a JSON-serialized literal (`JsonSerializer.Serialize`), never string-concatenated raw |
| `options` | `{}` or `{ simulateDenied: true }` (lab switch: reject with `NotAllowedError`) |
| Effect | `navigator.clipboard.writeText(text)`; unsupported API → `NotSupportedError`; no answer in 4 s → `Timeout` |
| Resolves | `{ ok: true }` or `{ ok: false, reason: "<DOMException.name>", detail: "<message>" }` — **never rejects** |
| Server reading | `ClipboardOutcome.From(answer)` — `null`/unknown shape/missing `ok` ⇒ not confirmed; `reason`/`detail` go to the log only |
| Consequence on the server | `ok: true` ⇒ `ITicketLinkService.ConfirmCopiedAsync` ⇒ `AuditLogService.Record("ticket.link.copied", id, user)` ⇒ **● Link for #2002 copied.** + toast; anything else ⇒ `Strings.CopyFailed`, the link stays selectable in `textLink`, **no audit entry** |

## The round trip, end to end

```
 browser                                          server (session)
 ───────                                          ────────────────
 Ctrl+K (keydown)                                 
   ├─ preventDefault, widget.focus()  [C1]         
   └─ ReportShortcutAsync("ctrl+k") ─────────────▶ GlobalSearchBox.ReportShortcut  [S1]
                                                    validate (whitelist) → [CLIENT] line
                                                    ShortcutPressed → WorkOrdersView → [UI] line, status
   ◀──────────────────────────────────── true ──── 

 click "Copy link"  ─────────────────────────────▶ buttonCopyLink_Click
                                                    ITicketLinkService.BuildLinkAsync(id)
                                                      IWorkOrderRepository.FindAsync  [DATA]
                                                      WorkOrder.CanShareLink          [DOMAIN]
                                                      HMAC sign → TicketLink          [SVC]
   ◀──── EvalAsync: ticketOps.copyToClipboard(link) ─ BrowserApi.CopyToClipboardAsync  [CLIENT →]
 navigator.clipboard.writeText(link)  [C2]
   ├─ resolves ──────── { ok: true } ─────────────▶ ClipboardOutcome.Confirmed        [CLIENT ✓]
   │                                                ITicketLinkService.ConfirmCopiedAsync
   │                                                  AuditLogService.Record            [SVC]
   │                                                "Link for #2002 copied." + toast     [UI]
   └─ rejects ── { ok:false, reason:"NotAllowedError" } ▶ ClipboardOutcome.Refused    [CLIENT ⚠]
                                                    no audit entry; Strings.CopyFailed; textLink selected
```

## What is deliberately *not* interop

| Need | Why no JavaScript |
|---|---|
| Filtering the grid as the user searches | a normal `TextChanged` → `IWorkOrderService.SearchAsync`; the server owns the data |
| Deciding whether a link may be shared | `WorkOrder.CanShareLink` — a business rule stays a server check |
| Building/signing the link | `TicketLinkService` — the key never leaves the server |
| Verifying six links in a batch | the same service, paced by a `Timer`; **no clipboard call**, because a Timer tick is not a user gesture |
| The `?` shortcut overlay | pure presentation — the server never hears about it, which is why it produces no trace line |

## Changing the contract

Adding an interop point means: a named function in `ticketops.interop.js` (or a `[WebMethod]` next to
the control it serves), a row in this file, a note in [`InteropSecurityNotes.md`](InteropSecurityNotes.md),
and the server-side validation written **before** the client call. Prefer named functions with typed
arguments over string-built script; keep `Eval` expressions to one call with JSON-serialized arguments.

## Evidence

The trace panel shows the contract executing: every `[CLIENT]` line is one of C1/S1/C2, and each is
preceded by the `[UI]`/`[SVC]` line that decided to make the call and followed by the `[SVC]`/`[UI]` line
that acted on the answer. The `?` overlay proves the negative: it opens and closes with no trace line.
