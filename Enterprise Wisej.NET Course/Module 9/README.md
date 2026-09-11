# Module 9 — JavaScript object model, browser APIs & secure interop contracts

**Enterprise Wisej.NET: Architecture to Cloud · EnterpriseOps Command Center · port 5209**

A runnable Wisej.NET 4 lab: an advanced keyboard **command palette** (Ctrl+K, written in JavaScript
and served from the project folder) and a **browser capability panel**. JavaScript collects browser
features and triggers server-side commands through a typed interop contract — and the server
enforces permissions before the command runs.

> Every boundary is a small API. The palette suggests; the server decides.

## Run it

```bash
cd EnterpriseOps
dotnet run -f net10.0 --urls http://localhost:5209
```

Then open <http://localhost:5209>. In-memory data only — no database, no network, no cloud account.

## What you are looking at

The `CommandCenterShell` page, as in the walkthrough:

| Control | What it shows |
| --- | --- |
| header `lblScreenName` | *EnterpriseOps — Command Center* |
| `commandPaletteHost` | the `Wisej.Web.Widget` that loads `/Interop/palette.client.js` (Package) and `command-palette-host.js` (InitScript). Its resting card shows the hotkey and the last command sent. Press **Ctrl+K** anywhere on the page. |
| `capabilityPanel` | *Browser capabilities · detection only*: one ✓/✕ row per capability, with the **fallback the server chose** for anything missing. It never grants a permission. |
| `lblBanner` | the failure banner (hidden until a command is refused) |
| `lblStatus` | the status bar at the bottom |

The session is signed in as `ben.tech` (Technician, tenant `contoso`) — there is no login screen.
The palette targets `WO-1040`, the first work order of the queue.

## What to try

| In the palette (Ctrl+K) | Path | What you should see |
| --- | --- | --- |
| *approve* → Enter | **failure — the walkthrough's path** | banner *You can't approve work order.* · `PERMISSION_DENIED · checked server-side in ClientCommandService · the command never ran`; status bar `CommandResult.Fail — PERMISSION_DENIED · audited · correlation …`; the palette stays open with the code in its footer |
| *escalate* → Enter | success | the one state change a Technician holds: toast *Escalated (v2).*, the palette closes, status bar `OK · …` |
| *escalate* again | failure | `INVALID_STATE` — *it cannot be escalated again* |
| *open work queue* → Enter | success | a command with no entity |

Rows the role does not hold are greyed with *needs a higher role* — a display hint only; running one
still goes to the server, which answers `PERMISSION_DENIED`. Every crossing is logged server-side
(`Client:` → `Interop:` → `Security:` → `Service:` → `Data:` → `Audit:`) through
`System.Diagnostics.Trace`.

## File tree

```
Module 9/
  README.md
  EnterpriseOps.slnx
  EnterpriseOps/
    EnterpriseOps.csproj            (net10.0-windows;net10.0 · embeds the InitScript)
    Program.cs Startup.cs Default.html Default.json Web.config
    Properties/launchSettings.json  (http://localhost:5209)
    Interop/
      JavaScriptInteropContractPatterns.cs   ClientCommandRequest · ClientCommandResult ·
                                             CommandDescriptor · InteropContract · IClientCommandService
      CommandPaletteHost.cs                  the Widget: Package + InitScript + WiredEvents,
                                             [WebMethod] GetCommandCatalog (RegisterWebMethods),
                                             lifecycle-guarded server → client calls
      palette.client.js                      the palette library (Package, served statically)
      command-palette-host.js                the client adapter (InitScript, embedded resource)
    UI/
      CommandCenterShell.cs / .Designer.cs   the page + the RunClientCommand [WebMethod]
      BrowserCapabilityPanel.cs / .Designer.cs
    Services/
      ActivityTrace.cs                       server-side log (System.Diagnostics.Trace)
      SessionContext.cs                      SessionContext · CommandContext · CommandResult ·
                                             ResultCodes · ServiceRegistry
      ClientCommandService.cs                the owner of the boundary — five gates
      WorkOrderService.cs                    the business rules and the WorkQueueRow projection
      BrowserCapabilityService.cs            accepts, sanitises and drops; picks the fallbacks
    Security/
      AppUser.cs Permission.cs PermissionService.cs AuditLog.cs
    Domain/WorkOrder.cs
    Data/InMemoryWorkOrderRepository.cs      ~60 rows across three tenants, deterministic seed
    docs/
      InteropContract.md          interop-boundary.svg
      CommandPaletteScript.md
      ServerCallbackMethods.md
      BrowserCapabilityPanel.md
      SecurityReviewNotes.md
```

## Lab steps → where in the code

| Lab step / deliverable | Where |
| --- | --- |
| Open the project and run it once | `EnterpriseOps.slnx` · `dotnet run -f net10.0 --urls http://localhost:5209` |
| Lab goal: palette + capability panel, JS triggers server commands, permissions enforced server-side | `Interop/` + `Services/ClientCommandService.cs` + `UI/CommandCenterShell.cs` |
| **Deliverable — interop contract document** | [`docs/InteropContract.md`](EnterpriseOps/docs/InteropContract.md) + [`docs/interop-boundary.svg`](EnterpriseOps/docs/interop-boundary.svg); in code: `Interop/JavaScriptInteropContractPatterns.cs` |
| **Deliverable — command palette script** | [`docs/CommandPaletteScript.md`](EnterpriseOps/docs/CommandPaletteScript.md); in code: `Interop/palette.client.js` + `Interop/command-palette-host.js` |
| **Deliverable — server callback methods** | [`docs/ServerCallbackMethods.md`](EnterpriseOps/docs/ServerCallbackMethods.md); in code: `CommandCenterShell.RunClientCommand`, `CommandPaletteHost.GetCommandCatalog`, and `CommandPaletteHost.Send` for the server → client direction |
| **Deliverable — browser capability panel** | [`docs/BrowserCapabilityPanel.md`](EnterpriseOps/docs/BrowserCapabilityPanel.md); in code: `palette.client.js → detectCapabilities()`, `Services/BrowserCapabilityService.cs`, `UI/BrowserCapabilityPanel.cs` |
| **Deliverable — security review notes** | [`docs/SecurityReviewNotes.md`](EnterpriseOps/docs/SecurityReviewNotes.md) |
| Show every path without leaking internals | `ClientCommandResult` carries a named code and a user-readable message, never an exception; `RunClientCommand` catches everything and answers `SERVER_ERROR` |
| Review & run: production-readiness note | `docs/SecurityReviewNotes.md` §8 (findings and residual risk) and §9 (checklist for the next interop method) |

## Student review questions, answered against the sample

**What happens before the target widget exists?**
Nothing listens and nothing is lost. The script attaches its `document` keydown handler inside
`init()` — after the host widget is created — never on page load, and removes it in `dispose()`
together with the overlay node it appended to `document.body`. On the server,
`CommandPaletteHost.Send` checks `PaletteReady && IsLoaded`, queues the call, and flushes the queue
when the client raises `paletteReady`.

**Which values from JavaScript are trusted?**
None. `commandName` is matched against a published catalogue, `entityId` is shape-checked and then
resolved *inside the session tenant*, and `correlationId` is a log key that grants nothing. The
`Allowed` flag the palette renders is a hint the server itself sent, re-checked at gate 3. The
capability report cannot invent a key. Identity, role and tenant come from `SessionContext` on every
call.

**Which business rules remain server-side?**
Existence (the catalogue), arity, authorisation (`PermissionService`), tenancy (every repository
query is tenant-scoped), state transitions and the version bump (`WorkOrderService`), and the choice
of fallback when a capability is missing (`BrowserCapabilityService`). The browser owns keystrokes,
filtering, selection and feature detection — nothing that survives a refresh.

## Instructor acceptance criteria, answered

| Criterion | Where it is satisfied |
| --- | --- |
| Follows the course architecture baseline | folder-per-layer (`UI`, `Interop`, `Services`, `Security`, `Domain`, `Data`, `docs`), namespaces matching the folders, typed commands and results, per-session services created in the page constructor, no static user or tenant state |
| UI event handlers remain thin and explainable | `CommandCenterShell` parses, delegates to one service and renders; no handler contains a rule |
| Service-level logic reviewable without opening the designer | `ClientCommandService.ExecuteFromClient` reads as five numbered gates; `WorkOrderService` holds the transitions; `PermissionService` holds the matrix |
| At least one failure path demonstrated | the walkthrough's `PERMISSION_DENIED` (Ctrl+K → *approve*), plus `INVALID_STATE` from a second *escalate*; `UNKNOWN_COMMAND`, `MALFORMED_PAYLOAD` and `INVALID_TARGET` are enforced for hand-edited payloads |
| Can explain state ownership, security implications and production behaviour | `docs/SecurityReviewNotes.md` (§1–§9) and `docs/InteropContract.md` (§5 gate order, §7 versioning) |

## Relationship to the walkthrough video

The video types `JavaScriptInteropContractPatterns.cs` and shows `CommandPaletteHost.cs`,
`palette.client.js` and `InteropContract.md` under `Interop/`, with `ClientCommandService.cs` in the
services layer — the sample keeps all of those names, the `CommandCenterShell` screen (title bar,
palette host, capability panel, status bar), the `ClientCommandRequest(CommandName, EntityId,
CorrelationId)` record and the `RunClientCommand` remote method, and reproduces the failure scene
(Ctrl+K → *approve* → Enter → `PERMISSION_DENIED`, "checked server-side in `ClientCommandService`,
the command never ran").

Deliberate differences:

* The video's sketch declares `CommandPaletteHost : UserControl` with constructor injection. The
  runnable sample makes it a `Wisej.Web.Widget` (the verified way to ship an InitScript) with a
  parameterless constructor plus `Attach(IClientCommandService)`, because the designer needs one.
  The designer control is named `commandPaletteHost` (the video's designer shows `commandPaletteHost1`).
* The video's sketch shows `string? EntityId`. This project builds with `<Nullable>disable</Nullable>`,
  so the optional field is an empty string — and it must be, because a `null` argument is rejected by
  the Wisej client wrapper before the call leaves the browser.
* `InteropContract.md` lives in `docs/` (the course cookbook puts every lab deliverable there), not in
  `Interop/`.

## Build

```
$ cd "Module 9/EnterpriseOps"
$ dotnet build -nologo -v q

Build succeeded.
    0 Warning(s)
    0 Error(s)
```

Both target frameworks (`net10.0-windows` and `net10.0`) build clean. The embedded InitScript is
present in both outputs under the logical name `EnterpriseOps.Interop.command-palette-host.js`.

## Verified / unverified

Used from the course cookbook and **verified** in earlier samples on the same framework build:
`[WebMethod]` on a top-level `Page` (`App.MainPage.NameAsync(args…)`) and on a child control via
`RegisterWebMethods` in `OnWebRender`; `Wisej.Web.Widget` `Packages` / `InitScript` / `WiredEvents` /
`Options` / `Call` / `OnWidgetEvent` and the `_addListener` pattern; `AlertBox.Show(..., TopRight)`.

**Unverified** (compiles; confirm at runtime):

| Item | Where it is used | Why it should hold |
| --- | --- | --- |
| `Widget.IsLoaded` as the "client widget exists" gate | `CommandPaletteHost.Send` | the gate also requires the widget's own `paletteReady`, so a wrong `IsLoaded` would only defer, never mis-send |
| A Package `Source` outside `wwwroot` (`"Interop/palette.client.js"`) | `CommandPaletteHost` ctor | the static file server serves the project folder; if it 404s, move the file to `wwwroot/` and change the one `Source` string |
| A widget's overlay element appended to `document.body` | `palette.client.js` → `Palette` ctor | plain DOM inside a `position:fixed` element; removed in `destroy()` |
| `Application.Session.Context = …` and `Application.SessionId` | `ServiceRegistry.CreateSessionContext` | both calls are wrapped in `try/catch` |
| `ListBox.BeginUpdate()` / `EndUpdate()` | `BrowserCapabilityPanel.Render` | compiles against `Wisej.Web.ListBox`; worst case it is a no-op |
