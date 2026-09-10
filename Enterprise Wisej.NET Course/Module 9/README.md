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

| Card | What it shows |
| --- | --- |
| **Command palette host** | the `Wisej.Web.Widget` that loads `/Interop/palette.client.js` (Package) and `command-palette-host.js` (InitScript). Press **Ctrl+K** anywhere on the page. |
| **Browser capabilities** | what feature detection found, and the **fallback the server chose** for anything missing. Detection only — it never grants a permission. |
| **Server · live activity trace** | one line per layer: `Client:` (what the browser said) → `Interop:` (contract validation) → `Security:` (the permission decision) → `Service:` → `Data:`. |
| **Security · audit log** | append-only, one row per crossing: outcome · user · tenant · correlation · command · entity. |

Switch the **user** combo in the header to change the session role (`ana.ops` Manager, `ben.tech`
Technician, `cara.admin` Admin) and the **tenant** combo to change which ids resolve.

## What to click

| Button | Path | What you should see |
| --- | --- | --- |
| **Set target** | — | pushes `Options.entityId` to the client (`update(options, old)`) and reports the target's real status from the repository |
| **Open palette (server → client)** | success | `Call("paletteOpen")` — the palette opens with no keystroke; trace `Client: palette opened (via server)` |
| **Run approve** (as `ana.ops`, target `WO-1040`) | **success** | `Interop → Security ALLOWED → Service: Approve → InProgress`; green status, toast, audit `ALLOWED` |
| **Run approve** (as `ben.tech`) | **failure — the walkthrough's path** | `Security: … WorkOrder.Approve → DENIED`; red banner *…the command never ran*; audit `DENIED`; **no `Service:` line** |
| **Run escalate** (as `ben.tech`) | success | the one state change a Technician holds — same wire, different permission |
| **Run open work queue** | success | a command with no entity; sending one anyway would be `MALFORMED_PAYLOAD` |
| **Unknown command** | failure | `catalogue lookup 'workorder.delete' → not found · no service called` → `UNKNOWN_COMMAND` |
| **Malformed payload** | failure | oversized entity id + a correlation id that is not 8 hex chars → rejected **before** any service ran |
| **Closed work order** | failure | `WO-1041` is seeded `Completed`: permitted, resolvable, still refused → `INVALID_STATE` |
| **Other tenant's WO** | failure | a well-formed id from another tenant → `INVALID_TARGET`, the same answer as "does not exist" |
| **⚠ Trust the client** | **anti-pattern** | confirm, then the browser claims role `Admin` and status `Completed`; the status changes with no rule, no version bump; audit `TAMPERED` |
| **Revert tampered** | **recovery** | restores the audit log's before-snapshot; audit `REVERTED` |
| **Forged capabilities** | failure | the report gains `canApprove=1;role=Admin`; both are dropped as unknown keys and traced |
| **Re-detect browser** | progress | `Call("paletteCollect")` — feature detection runs again and the panel refreshes |
| **Clear trace** | — | clears the trace; the audit log is append-only and is **not** cleared |

**The lifecycle demo happens on load, before you click anything**: the page calls
`commandPaletteHost.ShowResult(...)` while the client widget does not exist yet. The trace says
`… DEFERRED — the host widget does not exist yet`, then `Client: paletteReady …` and
`Interop: flushing 1 deferred server → client call(s)`. The bottom-right label shows
`host: ready · 1 deferred`.

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
                                             ResultCodes · CommandDescriptor · InteropContract ·
                                             IClientCommandService
      CommandPaletteHost.cs                  the Widget: Package + InitScript + WiredEvents,
                                             [WebMethod] GetCommandCatalog (RegisterWebMethods),
                                             lifecycle-guarded server → client calls
      palette.client.js                      the palette library (Package, served statically)
      command-palette-host.js                the client adapter (InitScript, embedded resource)
    UI/
      CommandCenterShell.cs / .Designer.cs   the page + the two [WebMethod]s
      BrowserCapabilityPanel.cs / .Designer.cs
    Services/
      ActivityTrace.cs                       Client / Interop / Security / Service / Data / Audit
      SessionContext.cs                      SessionContext · CommandContext · CommandResult ·
                                             ResultCodes · ServiceRegistry
      ClientCommandService.cs                the owner of the boundary — five gates + the anti-pattern
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
| **Deliverable — server callback methods** | [`docs/ServerCallbackMethods.md`](EnterpriseOps/docs/ServerCallbackMethods.md); in code: `CommandCenterShell.RunClientCommand`, `RunTrustedClientCommand`, `CommandPaletteHost.GetCommandCatalog`, and `CommandPaletteHost.Send` for the server → client direction |
| **Deliverable — browser capability panel** | [`docs/BrowserCapabilityPanel.md`](EnterpriseOps/docs/BrowserCapabilityPanel.md); in code: `palette.client.js → detectCapabilities()`, `Services/BrowserCapabilityService.cs`, `UI/BrowserCapabilityPanel.cs` |
| **Deliverable — security review notes** | [`docs/SecurityReviewNotes.md`](EnterpriseOps/docs/SecurityReviewNotes.md) |
| Show every path: success, validation and error, without leaking internals | the bottom bar; `ClientCommandResult` carries a named code and a user-readable message, never an exception |
| Review & run: production-readiness note | `docs/SecurityReviewNotes.md` §8 (findings and residual risk) and §9 (checklist for the next interop method) |

### The handler shape the lab asks for

```csharp
private async void btnRevert_Click(object sender, EventArgs e)
{
    try
    {
        DialogResult confirm = await MessageBox.ShowAsync(
            "Restore the last tampered work order from the audit log's before-snapshot?",
            "Revert", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        CommandResult result = _commands.RevertLastTamperedChange();   // the service decides
        RenderAudit();
        SetStatus(result.Message, result.Succeeded ? StatusKind.Ok : StatusKind.Warn);
    }
    catch (Exception ex)
    {
        HandleUnexpected(ex, "The revert could not be completed.");     // traced, never leaked
    }
}
```

Every other handler is the same shape without the dialog: trace the intent, call one service or push
one widget call, render the result. No handler contains a rule.

## Student review questions, answered against the sample

**What happens before the target widget exists?**
Nothing listens and nothing is lost. The script attaches its `document` keydown handler inside
`init()` — after the host widget is created — never on page load, and removes it in `dispose()`
together with the overlay node it appended to `document.body`. On the server,
`CommandPaletteHost.Send` checks `PaletteReady && IsLoaded`, queues the call, raises `Deferred`, and
flushes the queue when the client raises `paletteReady`. Load the page and read the first three
trace lines: the deferral, the ready, the flush.

**Which values from JavaScript are trusted?**
None. `commandName` is matched against a published catalogue, `entityId` is shape-checked and then
resolved *inside the session tenant*, and `correlationId` is a log key that grants nothing. The
`Allowed` flag the palette renders is a hint the server itself sent, re-checked at gate 3. The
capability report cannot invent a key. Identity, role and tenant come from `SessionContext` on every
call — which is exactly what `RunTrustedClientCommand` violates on purpose, and what the audit log's
`TAMPERED` row measures.

**Which business rules remain server-side?**
Existence (the catalogue), arity, authorisation (`PermissionService`), tenancy (every repository
query is tenant-scoped), state transitions and the version bump (`WorkOrderService`), and the choice
of fallback when a capability is missing (`BrowserCapabilityService`). The browser owns keystrokes,
filtering, selection and feature detection — nothing that survives a refresh.

## Instructor acceptance criteria, answered

| Criterion | Where it is satisfied |
| --- | --- |
| Follows the course architecture baseline | folder-per-layer (`UI`, `Interop`, `Services`, `Security`, `Domain`, `Data`, `docs`), namespaces matching the folders, typed commands and results, per-session services created in the page constructor, no static user or tenant state |
| UI event handlers remain thin and explainable | every handler in `CommandCenterShell.cs` traces its intent and calls one service or pushes one widget call; the longest is 12 lines and contains a confirmation dialog |
| Service-level logic reviewable without opening the designer | `ClientCommandService.ExecuteFromClient` reads as five numbered gates; `WorkOrderService` holds the transitions; `PermissionService` holds the matrix |
| At least one failure path demonstrated | five: `UNKNOWN_COMMAND`, `MALFORMED_PAYLOAD`, `INVALID_STATE`, `INVALID_TARGET`, `PERMISSION_DENIED` — plus the `TAMPERED` anti-pattern and its `REVERTED` recovery |
| Can explain state ownership, security implications and production behaviour | `docs/SecurityReviewNotes.md` (§1–§9, with findings and residual risk) and `docs/InteropContract.md` (§5 gate order, §7 versioning) |

## Relationship to the walkthrough video

The video types `JavaScriptInteropContractPatterns.cs` and shows `CommandPaletteHost.cs`,
`palette.client.js` and `InteropContract.md` under `Interop/`, with `ClientCommandService.cs` in the
services layer — the sample keeps all of those names, the `CommandCenterShell` screen, the
`commandPaletteHost` control, the `ClientCommandRequest(CommandName, EntityId, CorrelationId)` record
and the `RunClientCommand` remote method, and reproduces the failure scene (Ctrl+K → *approve* →
Enter → `PERMISSION_DENIED`, "checked server-side in `ClientCommandService`, the command never ran").

Two deliberate differences:

* The video's sketch declares `CommandPaletteHost : UserControl` with constructor injection. The
  runnable sample makes it a `Wisej.Web.Widget` (the verified way to ship an InitScript) with a
  parameterless constructor plus `Attach(IClientCommandService)`, because the designer needs one.
* The video's sketch shows `string? EntityId`. This project builds with `<Nullable>disable</Nullable>`,
  so the optional field is an empty string — and it must be, because a `null` argument is rejected by
  the Wisej client wrapper before the call leaves the browser.

The video's Solution Explorer also shows `InteropContract.md` sitting inside `Interop/`. The course
cookbook puts every lab deliverable under `docs/`, so it lives at
`EnterpriseOps/docs/InteropContract.md` here — same document, one folder over.

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
The app was **not** run — the reviewer runs it.

## Verified / unverified

Used from the course cookbook and **verified** in earlier samples on the same framework build:

* `[Wisej.Core.WebMethod]` on instance methods of a top-level `Page` → `App.MainPage.Name(args…, cb)`
  and `App.MainPage.NameAsync(args…)`; a child control registering its own with
  `OnWebRender(dynamic config) { base.OnWebRender((object)config); RegisterWebMethods((object)config); }`
  → `this.NameAsync(args…)` on the wrapper. Return values keep .NET casing; `null` arguments are rejected.
* `Wisej.Web.Widget`: `Packages` (`Name`, `Source`, served from the project folder), `InitScript` via
  `GetResourceString(...)` on an embedded resource, `WiredEvents`, `Options` (camel-cased, first-level
  change → `update(options, old)`), `Control.Call("fn", args)`, `OnWidgetEvent` with `e.Type` /
  `dynamic e.Data`, the `_addListener` / `_removeListener` / `_getEventData` pattern, and the rule that
  a synchronous `fireWidgetEvent` during `update()` is dropped.
* Never redefining a framework method (`getWidth`, `resize`, `destroy`, `getValue` …) on the wrapper;
  wrapping rather than replacing `dispose`.
* `MessageBox.ShowAsync(...)` in an `async void` handler; `AlertBox.Show(..., ContentAlignment.TopRight, autoCloseDelay: 4000)`.
* Fonts `"default"` / `"monospace"`, `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`, `Label.TextAlign`
  with `System.Drawing.ContentAlignment`.
* `Application.MainPage = new UI.CommandCenterShell();` from `Program.Main(NameValueCollection)`.

**Unverified** (compiles; the reviewer should confirm at runtime):

| Item | Where it is used | Why it should hold |
| --- | --- | --- |
| `Widget.IsLoaded` as the "client widget exists" gate | `CommandPaletteHost.Send` | cookbook: *"`IsLoaded` tells whether the client widget has initialized"*. The gate also requires the widget's own `paletteReady`, so a wrong `IsLoaded` would only defer, never mis-send. |
| A Package `Source` outside `wwwroot` (`"Interop/palette.client.js"`) | `CommandPaletteHost` ctor | the cookbook's static-file rule is *the file server serves the project folder*; the Integration course used `wwwroot/…`, this sample uses `Interop/…`. If it 404s, move the file to `wwwroot/` and change the one `Source` string. |
| A widget's overlay element appended to `document.body` escaping the qooxdoo container | `palette.client.js` → `Palette` ctor | plain DOM inside a `position:fixed` element; it is removed in `destroy()`. |
| `Application.Session.Context = …` and `Application.SessionId` | `ServiceRegistry.CreateSessionContext` | cookbook lists both; both calls are wrapped in `try/catch` so a design-time or session-less context cannot break the page. |
| `Application.Browser` / `Application.ClientTimeZone` | **not used** | the cookbook marks them unverified; this sample gets the same facts from browser-side detection instead, which is also the point of the capability panel. |
| `ListBox.BeginUpdate()` / `EndUpdate()` | `RenderAudit`, `BrowserCapabilityPanel.Render` | compiles against `Wisej.Web.ListBox`; worst case it is a no-op. |
