# Interop security notes — one note per interop point

*Module 9 deliverable · TicketOps Console · "Security note for each interop point"*

The TicketOps Console has exactly three places where JavaScript and C# meet, plus the packaging that
carries the script. Each one is treated as a small public API: named, documented, validated on the
server, and never trusted. The rule behind every note is the same — **the browser requests or reports;
the server validates, decides and records.**

| # | Interop point | Direction | What crosses the boundary | Where it is handled |
|---|---|---|---|---|
| 1 | Ctrl+K focuses the global search | browser → server (report) | the shortcut name `"ctrl+k"` | `Platform/ticketops.interop.js` → `Controls/GlobalSearchBox.ReportShortcut` ([WebMethod]) |
| 2 | Copy ticket link | server → browser → server | the finished, signed URL (down); `{ ok, reason, detail }` (up) | `Views/WorkOrdersView.buttonCopyLink_Click` → `Infrastructure/BrowserApi.CopyToClipboardAsync` → `Services/TicketLinkService` |
| 3 | Search text / Esc clears the box | browser → server (normal Wisej event) | the query string | `TextChanged` → `Services/WorkOrderService.SearchAsync` |
| — | The script itself | shipped with the assembly | nothing at runtime | `Platform/ticketops.interop.js`, `[assembly: WisejResources]`, the `JavaScript` extender |

---

## 1 · Ctrl+K → focus search (client-side key handling, server told)

**What crosses.** After the script has moved the focus, it calls `widget.ReportShortcutAsync("ctrl+k")`
— a `[WebMethod]` registered on the `GlobalSearchBox` control. One string goes up; a `bool` comes back.

**Why the server never trusts it.** The argument is client input: a user can open the console and call
`ReportShortcutAsync("<script>…")` or a 10 KB string. `ReportShortcut` therefore trims, lower-cases,
bounds the length (16) and checks the name against a constant whitelist (`{ "ctrl+k" }`). Anything else is
refused without being echoed back.

**What the server does with it.** Nothing that matters. A focus change has no business meaning, so the
callback only raises `ShortcutPressed`, which the screen turns into a status text. This is the one interop
point where "tell, don't ask" is correct — and the moment a shortcut *triggers* an action (save, close,
copy) the browser only requests it and the server runs the same service checks it runs for a click (see
point 2).

**Timing.** The listener is attached by the Wisej.NET `JavaScript` extender, whose script runs when the
search widget is created — so it can never target a widget that does not exist yet. The script is
idempotent (a re-render removes the previous listener first). Nothing is placed in `Default.html`.

**No secrets.** The script carries no ids, keys or user names; it receives the widget as `this`.

---

## 2 · Copy ticket link (server builds, browser copies, server confirms)

**What crosses — down.** The server builds the canonical link in `TicketLinkService.BuildLinkAsync`
*after* re-loading the work order by the id the screen supplied (an unknown id → refused, nothing crosses)
and *after* the domain rule `WorkOrder.CanShareLink` said yes (a confidential work order → refused, nothing
crosses). Only then does `BrowserApi.CopyToClipboardAsync` hand the finished string to the browser. The
client never assembles a URL and never sees the signing key: `?sig=8c41f0` is an HMAC computed in the
service with a key that exists only in `AppComposition` → `TicketLinkService`.

**Escaping.** The link becomes part of an `Application.EvalAsync` expression —
`ticketOps.copyToClipboard(<literal>)`. Anything interpolated into `Eval` is live script, so the literal
is produced by `System.Text.Json.JsonSerializer.Serialize(text)`: quotes, backslashes, line breaks and
`< > &` are escaped. Even if a ticket title were ever part of the text, it could not close the string and
inject code. Ids and numbers are the only other values that ever reach a script.

**What crosses — up.** The script's Promise always resolves to `{ ok: true }` or
`{ ok: false, reason, detail }`; it never rejects and it times out after 4 s, so the server-side `await`
always gets an answer. `ClipboardOutcome.From` reads that object defensively: `null`, a missing `ok`, an
unexpected shape — all count as **not confirmed**. The server never assumes success.

**Why the server never trusts it.** It does not have to trust the text (it owns the text); it only trusts
the *yes/no* for one consequence: the audit entry. `ITicketLinkService.ConfirmCopiedAsync` →
`AuditLogService.Record("ticket.link.copied", …)` runs only on `ok: true`. If the browser lied and said
"copied", the worst case is one audit line claiming a copy that did not happen — which is why the audit
entry names the action and the user, not any claim the browser made. The `reason`/`detail` strings are
browser-reported text: they go to the server log and never into the banner — the user sees
`Strings.CopyFailed`.

**"It can say no" is a normal path.** `navigator.clipboard.writeText` rejects without a recent user
gesture, on an insecure origin, or when permission is denied. The screen shows the link in a read-only
box and tells the user to select and copy it; nothing is thrown, nothing is audited.

**It needs a user action.** `Copy link` is a click, so the browser sees a user gesture. Code that runs
from a timer or a background task must never call the clipboard — the browser would refuse.

---

## 3 · Search text and Esc (a normal Wisej.NET event carrying client input)

**What crosses.** The text in the `GlobalSearchBox`, via the ordinary `TextChanged` event. When the
script clears the box on Esc, it does so with `widget.setValue("")`, so the server hears about it through
the same event — the script does not call the server itself.

**Why the server never trusts it.** `WorkOrderService.SearchAsync` trims and bounds the query (80
characters) before filtering, and matches with an invariant, case-insensitive comparison — no dynamic
SQL, no `eval`. The same method would serve a WebMethod or a REST endpoint unchanged.

**Escaping.** The query is displayed in `labelCount` (a `Label` with `AllowHtml = false`, so it is
escaped). Typing `<b>pump</b>` shows the tags as text.

---

## The script itself — packaging and what it may never contain

- **One named file**, `Platform/ticketops.interop.js`, embedded in the assembly and bundled into the client
  by Wisej.NET (`[assembly: WisejResources]`). It is versioned and deployed with the app; nobody can forget
  to copy it and nobody can edit it in the web root.
- **Attached through the `JavaScript` extender** (`SetJavaScript(searchBox, "ticketOps.attachSearchShortcuts(this);")`)
  — the behavior travels with the control and runs after the widget exists.
- **No secrets, no rules, no state.** The file contains no keys, no URLs to build, no permission
  checks and no application state; the only data it keeps is the list of shortcut labels for the `?`
  list. Everything that *means* something — who may share which link, what the link is, whether the copy
  counts — lives in `Services/` and `Domain/`.
- **No `innerHTML`.** The shortcut list is built with `createElement`/`textContent`.
- **Reviewable in one sitting.** Three functions, one `sourceURL`. If a fourth interop point is ever
  needed, it is added here and gets its own row in this file and in [`InteropContract.md`](InteropContract.md).

## Common mistakes checked

| Mistake (lesson guide) | Present? |
|---|---|
| A business rule in JavaScript | no — `WorkOrder.CanShareLink` is C#; the script has no `if` about a work order |
| Trusting a callback argument | no — `ReportShortcut` whitelists; `BuildLinkAsync` re-loads the id; `ClipboardOutcome.From` defaults to "not confirmed" |
| Interpolating raw user text into `Eval` | no — `JsonSerializer.Serialize` produces the literal; ids are the only other values |
| Free-floating script in `Default.html` | no — the extender attaches it to the control; `Default.html` is the template's |
| Injecting user text as HTML | no — `Label.AllowHtml` stays false; the shortcut list uses `textContent` |
| Holding application state in JS | no — the widget reference and the shortcut list element are the only client state |
