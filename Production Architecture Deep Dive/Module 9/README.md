# TicketOps · Production Architecture Deep Dive · Module 9

Local lab build for **Module 9 · JavaScript Integration, Widget Interop & Client-Side Enhancements**.
It follows the walkthrough video *Add safe JavaScript interop*: the TicketOps Console gets two small,
safe enhancements — a **Ctrl+K** shortcut that focuses the global search from anywhere, and a
**server-confirmed clipboard copy** of the selected work order's link — plus a security note for every
interop point. JavaScript as a layer, not a rewrite: the browser focuses, copies and shows; the server
validates, decides and records.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 9\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5109
```

Then open <http://localhost:5109>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.) `localhost` counts as a secure context, so `navigator.clipboard` is
available; on a plain-http host name every copy would take the fallback path shown below.

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with no warnings for both targets (`net10.0-windows`, `net10.0`).

## What to click in the Work Orders window

The left card is the screen; the right card is the **Activity trace**. Lines tagged `[CLIENT]` are the
browser's part of a round trip, so you can see exactly what crossed the boundary and what the server did
before and after.

| Button / key | Path | What you should see |
|---|---|---|
| **Ctrl+K** (from anywhere, even with the trace list focused) | client-side shortcut, server told | the search box gets the focus instantly (no round trip for the focus itself); then `[CLIENT] GlobalSearchBox.ReportShortcut — Ctrl+K → focus search (focused in the browser; the server is told, not asked)` and `[UI] WorkOrdersView.searchBox_ShortcutPressed — ctrl+k acknowledged — … no service call, nothing persisted`; status **● search focused (Ctrl+K)** |
| type `pump` (Enter/Tab) | normal Wisej event | `[UI] searchBox_TextChanged → IWorkOrderService.SearchAsync(4 chars)` · `[SVC] "pump" → GetAllAsync() then filter` · `[SVC] 1 of 6 match`; the grid shows #2002; **1 match for "pump"**. Type `<b>x</b>` to see the text escaped, never rendered |
| **Esc** (search box focused, non-empty) | client-side, server hears a normal event | the script clears the box; the server sees `TextChanged` and reloads all 6 rows |
| **?** (focus outside a text field) | client-only polish | a small shortcut list appears bottom-right; **no trace line at all** — the server never hears about it; `?` or Esc closes it |
| select **#2002**, **⧉ Copy link** | success (server-confirmed) | `[UI] → ITicketLinkService.BuildLinkAsync(#2002)` · `[SVC] re-check #2002 for L. Romero (the id is client input) → FindAsync` · `[DATA] #2002 found` · `[SVC] built https://ticketops.local/t/2002?sig=… — signed server-side` · `[CLIENT] BrowserApi.CopyToClipboardAsync → ticketOps.copyToClipboard(41 chars) — Application.EvalAsync, awaiting the browser` · `[CLIENT] navigator.clipboard.writeText — resolved → ok` · `[SVC] TicketLinkService.ConfirmCopiedAsync — the browser confirmed → Record` · `[SVC] AuditLogService.Record — ticket.link.copied — #2002 by L. Romero` · `[UI] OK · Link for #2002 copied.`; the **CLIPBOARD** box shows the link, the **AUDIT LOG (SERVER)** gains a line, status **● Link for #2002 copied.**, toast *Link copied to clipboard.* — and Ctrl+V pastes the link |
| **⧉ Copy link** with nothing selected | validation | orange banner **Select a work order to copy its link.**; nothing crosses to the browser |
| **▶ Verify 6 links** | progress | a `Timer` builds one link per tick through the same `BuildLinkAsync`; the progress bar and **● verifying n/6** advance; the CLIPBOARD box shows each link; #2006 is refused (`[DOMAIN] ⚠`); ends **● 6 links verified — 5 shareable, 1 refused by the domain rule**. No `[CLIENT]` line: a Timer tick is not a user gesture, so the batch never touches the clipboard |
| **Copy link for #9999** | failure 1 (forged client id) | `[SVC] re-check #9999 …` · `[DATA] #9999 not found` · `[SVC] ⚠ rejected: #9999 does not exist — nothing crosses to the browser`; orange banner **That work order does not exist or is not visible to you.**; no `[CLIENT]` line |
| **Copy confidential #2006** | failure 2 (domain rule) | `[DOMAIN] ⚠ WorkOrder.CanShareLink — #2006 rejected: Confidential work orders cannot be shared by link.`; the script never ran |
| **Simulate clipboard denied** | client-side error path | the switch turns ON and copies the selected (or #2002) link: `[CLIENT] → ticketOps.copyToClipboard(41 chars, simulateDenied)` · `[CLIENT] ⚠ navigator.clipboard.writeText — rejected: NotAllowedError — Write permission denied (simulated).` · `[UI] ⚠ not confirmed by the browser (NotAllowedError) → no audit entry`; orange banner **Copy failed — select the link text below and copy it manually (Ctrl+C).**; the link is selected in the CLIPBOARD box; the audit log does **not** grow |
| **Clipboard denied: ON — restore** (same button) | recovery | the switch turns OFF and the same copy succeeds: `[CLIENT] resolved → ok`, audit line, **● Link for #2002 copied.** |
| **Simulate data outage** | error path | `[DATA] ✖ outage: SELECT * FROM WorkOrders failed — timeout connecting to sql01:1433 …` stays in the trace; the user sees only the red banner **The action could not be completed. Check the log for details.** and a toast. **Copy link** now fails the same way while re-checking the id (`[DATA] ✖`, no `[CLIENT]` line) |
| **Recover the data store** (same button) | recovery | the repository answers again, the grid reloads, status **● ready** |
| **Clear trace** | — | empties the right-hand card |

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Embedded or JavaScriptSource script | `TicketOps/Platform/ticketops.interop.js` — embedded (`<EmbeddedResource Include="Platform\*.js" />` + `[assembly: WisejResources]` in `Properties/AssemblyInfo.cs`), attached to the search box by the `Wisej.Web.JavaScript` extender in `Views/WorkOrdersView.Designer.cs` (`SetJavaScript(searchBox, "ticketOps.attachSearchShortcuts(this);")`) |
| 2 | C# method invoking client behavior | `Infrastructure/BrowserApi.CopyToClipboardAsync` — `Application.EvalAsync("ticketOps.copyToClipboard(<json literal>, {…})")`, awaited; called from `Views/WorkOrdersView.CopyLinkServerConfirmedAsync` |
| 3 | Server callback handler | `Controls/GlobalSearchBox.ReportShortcut` — `[WebMethod]` registered on the control (`OnWebRender` → `RegisterWebMethods`), whitelist-validated, raises `ShortcutPressed`; the clipboard answer comes back through the awaited `EvalAsync` and is read by `ClipboardOutcome.From` |
| 4 | Security note for each interop point | [`docs/InteropSecurityNotes.md`](TicketOps/docs/InteropSecurityNotes.md) |
| — | Interop contract (the named surface) | [`docs/InteropContract.md`](TicketOps/docs/InteropContract.md) |
| — | Production-readiness note (lab step 8) | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |
| — | Every path visible without leaking internals | `WorkOrdersView.ShowResult` / `ReportFailure`, `Resources/Strings.cs`, the trace panel (which now HTML-encodes what it renders) |

Names follow the video and the guides: `TicketLinkService`, `AuditLogService`, `ticketops.interop.js`,
`copyTicketLink`'s shape as `copyToClipboard`, `L. Romero`, work orders 2001–2005. The video's `MainPage.cs`
is `Views/WorkOrdersView` here (a `Form`, as in every module of this course), and its `Scripts/` folder is
`Platform/`, the folder Wisej.NET bundles automatically. The video's `Application.Eval` is `Application.EvalAsync`
here so the server can **await the browser's answer** before it writes the audit entry.

## Where things live

```
TicketOps/
├─ Views/
│  ├─ WorkOrdersView.cs           the screen: thin handlers; CopyLinkServerConfirmedAsync is the round trip
│  └─ WorkOrdersView.Designer.cs  layout + the JavaScript extender attaching the script to searchBox
├─ Controls/
│  ├─ GlobalSearchBox.cs          TextBox + [WebMethod] ReportShortcut (the server end of Ctrl+K) + ShortcutPressed
│  └─ StatusBanner                reusable "● state" + banner UserControl (display only)
├─ Platform/
│  └─ ticketops.interop.js        THE client script: attachSearchShortcuts, copyToClipboard, the ? overlay
├─ Services/
│  ├─ ITicketLinkService / TicketLinkService   re-check the id, apply the rule, sign the URL; confirm → audit
│  ├─ IAuditLogService / AuditLogService       the server-side record
│  └─ IWorkOrderService / WorkOrderService     bounded search over the work orders
├─ Domain/
│  ├─ WorkOrder.cs                record + CanShareLink(user) — the rule the script never sees
│  ├─ SessionUser.cs, TicketLink.cs, AuditEntry.cs, OperationResult.cs
├─ Data/
│  ├─ IWorkOrderRepository.cs     persistence contract
│  └─ InMemoryWorkOrderRepository.cs  fake store seeded with the video's rows; SimulateOutage throws like a driver
├─ Infrastructure/
│  ├─ BrowserApi.cs               the ONE place that calls a browser API (EvalAsync + escaping + "it said no")
│  ├─ SessionContext.cs           who is signed in (per session, never static)
│  ├─ ILog.cs / ActivityLog.cs    cross-cutting logging
│  └─ AppComposition.cs           who gets what: one object graph per session, demo signing key stays here
├─ Resources/Strings.cs           safe user-facing messages (LinkCopied, CopyFailed, …)
├─ Diagnostics/ActivityTracePanel the live trace card (HTML-encodes each line)
├─ Properties/AssemblyInfo.cs     [assembly: WisejResources] — bundles Platform/*.js into the client
├─ docs/                          the deliverables
├─ Program.cs                     Wisej.NET session entry point → AppComposition
└─ Startup.cs                     Kestrel host (app.UseWisej())
```

## Self-check answers (lesson guide)

- **Does the script run after the widget exists?**
  Yes. It is attached by the `JavaScript` extender (`SetJavaScript(searchBox, …)`), whose code runs when the
  `GlobalSearchBox` widget is created and again when it is refreshed — `this` is the widget, so there is
  nothing to look up and nothing to miss. `Default.html` carries no script. `attachSearchShortcuts` is
  idempotent, so a refresh does not stack listeners.
- **What data crosses the JS/server boundary, and is it re-validated on the server?**
  Up: the shortcut name (`ReportShortcut` whitelists it and bounds its length), the clipboard answer
  (`ClipboardOutcome.From` treats anything but `{ ok: true }` as not confirmed), the search text
  (`SearchAsync` trims and bounds it). Down: the finished link only, JSON-serialized into the `Eval`
  expression so it can never become script. Ids and numbers are the only other values that reach a script.
- **What server-side check still runs after the shortcut fires?**
  For Ctrl+K, none is needed — focusing a box authorizes nothing, which is why the server is *told*.
  For the copy, everything: `BuildLinkAsync` re-loads the work order by id, `WorkOrder.CanShareLink`
  decides, the URL is signed with a key the browser never sees, and the audit entry is written only after
  the browser confirmed. A shortcut that triggered the copy would call the same
  `CopyLinkServerConfirmedAsync` and get the same checks.
- **Could this feature have been built server-side without JavaScript at all — and if so, should it be?**
  Ctrl+K: yes — `Form.Accelerators` with `Keys.Control | Keys.K` and the `Accelerator` event let the
  server call `searchBox.Focus()` with no script, at the cost of one round trip per press. If the
  shortcut were the only JavaScript left, that smaller surface would win; the lab keeps the script to show
  the client-handling + callback pattern. The clipboard: no — only the browser can write to it, so the
  server → browser → server shape is the minimum, and the value of the module is that the *decision* and
  the *record* never left the server.
