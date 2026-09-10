# OrderDesk.Web · From WinForms to the Web · Module 7

Local lab build for **Module 7 · Modernize, Secure, and Deploy the Capstone**. It follows the lesson and the
walkthrough video: the working port from Modules 1–6 is **modernized** (theme + mixin, tool buttons, watermark,
toast) without touching the business logic, made **responsive** through `ClientProfiles.json` and three explicit
layouts, **reviewed as a web app** (server-side auth per action, download guard, `AllowHtml` encoded / sanitized /
raw, session lifecycle, audit log), given an **operations dashboard**, a **health endpoint** and the eight-item
**readiness checklist** as code, and packaged with **IIS / Docker deployment assets**. The **capstone report**
(reused · adapted · modernized · risks · next slice) closes the course.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine. The deployment files under
`deploy/` are written to be run, not executed by the sample.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 7/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5607
```

Then open <http://localhost:5607>. (Visual Studio: open `OrderDesk.slnx`, F5. `GET http://localhost:5607/health`
answers JSON without a browser session.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

The left column is a `TabControl` (**Orders · Security · Dashboard · Readiness & deploy**); the right column is the
migration log (live trace), the status line, the banner, **Open second session ↗** and **Clear**.

| Action | Path | What you should see |
|---|---|---|
| Page load | success | Trace: `• server startup Default.json → OrderDesk.Program.Main → Application.MainPage = new MainPage()`, `session new browser session xxxxxxxx · 1 session(s) in this process`, `theme Bootstrap-4 · button radius = 14 (Themes/orderdesk.mixin.theme merged at startup)`, `DownloadGuard.Store exports/orders.csv · 5 orders · under …\App_Data`, `OrderService.GetOrders 5 orders from the shared repository`, `→ .NET→JS ResponsiveLayout.Apply Desktop ← Application.ActiveProfile (Desktop) · desktop: grid + detail side by side · toolbar icon + full text · grid 380×118`, `deploy assets 8/8 files present under …`, `timerAudit 1 s · …`. The grid shows 1042 Northwind Traders 4,820.00 Open first; status `● capstone running · signed in: nobody · sign in on the Security tab` |
| **Orders · toolbar New Order** | success (modernized, logic unchanged) | Trace `← JS→.NET toolBar.ButtonClick toolNew "New Order"`, `• server OrderService.Save order 1043 · CalculateOrderTotal = 1,280.00 (business logic reused, unchanged since Module 1)`, `→ .NET→JS Ui.Toast …`; a toast top-right; the Dashboard KPIs move (Open orders 3, Revenue today 1,280.00) |
| **Orders · Print Invoice** | recovery (Module 1/6) | `⚠ boundary Print Invoice PrintDocument → local printer ⇒ server PDF (n bytes) → PdfViewer`; the modal preview opens; closing it traces `InvoicePreviewForm closed and disposed` |
| **Simulate tablet / Simulate phone / Simulate desktop** | success (deliverable: responsive) | `← JS→.NET simulate Tablet — the same Apply a real Tablet profile triggers` + `→ .NET→JS ResponsiveLayout.Apply Tablet · tablet: detail below the grid · toolbar icon + short text · grid 602×66`; phone: `… phone: detail hidden · toolbar hidden · actions in one ComboBox · grid 602×84` and the *Actions…* dropdown replaces the toolbar (pick *New Order* there: `comboActions (phone) "New Order" …`); desktop restores the side-by-side layout. Resizing the browser across 600/1024 px produces `← JS→.NET ResponsiveProfileChanged Desktop → Tablet · …` followed by the same Apply line |
| **Material-3 / FluentDark-5 / Bootstrap-4** | success (deliverable: theme) | `• server Application.LoadTheme Bootstrap-4 → Material-3 · button radius <before> → <after>`, `→ .NET→JS theme every session in this process is restyled — zero lines of form code changed`; the whole page restyles (a second tab too); audit row `theme` |
| **Apply orderdesk mixin** | success (deliverable: mixin) | `• server Application.LoadTheme(mixins) Material-3 + [orderdesk] · button radius <before> → 14 (Themes/orderdesk.mixin.theme says 14)`; buttons get 14 px corners; `labelTheme` shows the radius |
| **Security · Sign in kelly** | success | `• server AuthService.SignIn ✓ kelly · Acme · Manager → Application.Session.UserContext (this session only)`; green banner *✓ Signed in as kelly · Manager of Acme. Permissions: read, export, download, delete …*; `labelAuth` = `AuthService.Current = kelly · Acme · Manager …`; audit row ✓ `sign-in` |
| **Security · Download** (`exports/orders.csv`, as kelly) | success | `• server DownloadGuard.Download ✓ permission files.download · resolved under …\App_Data · exists`, `→ .NET→JS Application.Download orders.csv — the resolved server path, never the browser's string`; the browser saves the file; audit ✓ `download exports/orders.csv (n bytes)` |
| **Security · Try ..\Web.config** (as kelly) | **failure** (traversal rejected) | Text box becomes `..\Web.config`; `• server DownloadGuard.Download ✕ Path traversal rejected: '..\Web.config'.`; red banner *✕ Rejected by the download guard: Path traversal rejected … refused before File.Exists is even asked. Audited as ✕.*; status `● path traversal rejected — audited`; audit row ✕ `download REJECTED '..\Web.config' — …` |
| **Security · Sign in sam** → **Orders · Export** | **failure** (denied on the server) | `• server AuthService.Demand ✕ 'orders.export' denied: role Clerk lacks 'orders.export'.`; red banner *✕ Export denied on the server: 'orders.export' denied: role Clerk lacks 'orders.export'. The button was clickable — …*; status `● export denied — audited`; audit rows ✕ `authorize orders.export DENIED — role Clerk lacks 'orders.export'` |
| **Security · Download** (as sam) | failure | `✕ 'files.download' denied: role Clerk lacks 'files.download'.` — denied before the path is resolved |
| **Security · kelly, wrong password** | **failure** (denied, audited) | `• server AuthService.SignIn ✕ denied for 'kelly' — audited, nothing stored in the session`; red banner *✕ Sign-in denied for kelly: the salted SHA-256 hash did not match …*; audit row ✕ `sign-in denied for 'kelly'` |
| **Security · Render notes** | success + recovery (AllowHtml policy) | `labelEncoded` shows `<b>Rush</b> order <img src=x onerror="alert(1)"> <i>ship Friday</i>` literally (AllowHtml = false); `labelSanitized` shows **Rush** order *ship Friday*; trace `• server HtmlSanitizer.Sanitize kept 4 tag(s) (b, i, br) · removed 1: <img src=x onerror="alert(1)">`; green banner |
| **Security · Render raw (unsafe)** | **failure** (the injection) | `⚠ boundary AllowHtml desktop Label.Text was inert text ⇒ web Label with AllowHtml = true is innerHTML: <img src=x onerror="alert(1)"> becomes a DOM element`; red banner *✕ The note is now part of the page DOM … if an alert(1) just popped up, that was user-entered text executing in your session …*; the browser most likely shows `alert(1)`; audit row ✕ `allowhtml.raw` |
| **Open second session ↗** → in the new tab **Security · Sign in sam** → back in the first tab | success (multi-user, the progress path) | Within a second the first tab's audit grid gains `sign-in sam` from the other session id, the trace says `• server AuditLog (other session) 1 new entry from session xxxxxx: sign-in sam` and `Application.SessionCount 2 session(s) in this process`; the Dashboard line reads `● 2 sessions · N audit entries`; the first tab is still kelly (`labelAuth`) |
| **Dashboard** tab | success | KPIs (Open orders 2 · Revenue today 0.00 · Invoiced 1 · On-time % 80.0 before any New Order), the *Orders by status* bar chart (Open 2 · Shipped 1 · Invoiced 1 · Hold 1), the recent-activity grid from the audit log, `● 1 session · N audit entries` |
| **Readiness & deploy · Health check** | success | `labelHealth` lists ✓ status / version / uptime / storage root (writable) / log folder / client profiles / theme mixin / sessions / theme / profile / web socket; one trace line per item `• server health · <name> ✓ …`; green banner *✓ 11 health items, all ✓ …* |
| **Readiness & deploy · GET /health** | success (progress path) | A new tab with the JSON (`{"status":{"ok":true,"value":"healthy"},…,"ok":true}`); trace `→ .NET→JS Application.Navigate /health, _blank …`, then from the background task `• server HttpClient GET http://localhost:5607/health → n bytes` and `/health JSON {…}`; green banner. If the app runs on another port: `✕ … → HttpRequestException …` and an orange banner explaining the probe-URL mismatch |
| **Readiness & deploy · Run readiness checklist** | success (deliverable) | Eight rows in the grid; trace `• server readiness 1/8 ✓ Starts through the Wisej.NET startup files … — Default.json startup → …` … `readiness 8/8 ✓ Deployment configuration … — 5/5 deployment files present · /health endpoint in Startup.cs · logs in …`; banner *✓ Readiness checklist: 8/8 items with evidence …* |
| **Readiness & deploy · Static-state audit** | success | One trace line per static field (`✓ readonly …`, `✕ writable AuditLog._version`), then `• server StaticStateAudit 0 open item(s) · N static field(s) scanned · readonly shared state in: …`; green banner *✓ 0 open items …* |
| **Clear** | – | Empties the trace |

The right-hand card is the **migration log · live trace**: every user action (`← JS→.NET`), every business-logic call
(`• server`), everything pushed to the browser (`→ .NET→JS`) and every desktop boundary hit and replaced (`⚠ boundary`).

## Where things live

```
Module 7/
├─ OrderDesk.slnx                     OrderDesk.Web only (the WinForms original lives in Modules 1 and 2)
└─ OrderDesk.Web/                     the Wisej.NET 4 app (net10.0-windows;net10.0)
   ├─ Program.cs / Startup.cs         session entry point / Kestrel host + the /health endpoint (MapGet, no Wisej session)
   ├─ Properties/launchSettings.json  port 5607 for F5 (dotnet run needs --urls)
   ├─ Default.html / Default.json / Web.config   theme Bootstrap-4 · OrderDesk.StorageRoot, ThemeMixin, Version
   ├─ ClientProfiles.json             Phone ≤600 · Tablet 601–1024 · Desktop ≥1025 (copied next to the assembly, never served)
   ├─ Themes/orderdesk.mixin.theme    button + toolbar-button radius 14, orderdesk-accent colour
   ├─ Domain/                         ✓ unchanged since Module 1 — the reused business logic
   ├─ Services/ThemeService.cs        Apply(theme) · ApplyMixin(name) · ButtonRadius() read-back
   ├─ Services/ResponsiveLayout.cs    profile → Desktop / Tablet / Phone layout, in code
   ├─ Services/AppConfig.cs           Web.config reader (StorageRoot, ThemeMixin, Version)
   ├─ Security/AuthService.cs         SignIn (salted hashes) · Authorize / Demand per action · roles Manager / Clerk
   ├─ Security/DownloadGuard.cs       permission → resolve under the storage root → exists → Application.Download
   ├─ Security/HtmlSanitizer.cs       Encode · Sanitize (whitelist b, i, br)
   ├─ Security/AuditLog.cs            process-wide append-only audit log with a Version counter
   ├─ Security/UserSessionContext.cs  the per-session user (Module 4)
   ├─ Diagnostics/HealthCheck.cs      Report / Text / Json — the /health endpoint and the console button
   ├─ Diagnostics/ReadinessChecklist.cs   the lesson's 8 items with live Evidence
   ├─ Diagnostics/StaticStateAudit.cs     reflection over every static field
   ├─ Diagnostics/AppLog.cs           App_Data/logs/orderdesk-yyyyMMdd.log + System.Diagnostics.Trace
   ├─ Reporting/                      InvoicePdfWriter (server PDF), CsvExport (in-memory export)
   ├─ Views/                          TracePanel, Ui, InvoicePreviewForm
   ├─ MainPage.cs / .Designer.cs      the lab console: tabs Orders · Security · Dashboard · Readiness & deploy
   ├─ deploy/iis/web.config           ASP.NET Core Module handler + merged appSettings
   ├─ deploy/docker/Dockerfile        multi-stage sdk:10.0 → aspnet:10.0, EXPOSE 8080, volume, HEALTHCHECK
   ├─ deploy/docker/docker-compose.yml   one service, 5607:8080, App_Data volume, healthcheck
   ├─ deploy/appsettings.Production.notes.md   which setting moves where
   └─ docs/                           the lab deliverables + migration-log.md
```

## Deliverables

1. **Modernization notes** (theme, mixin, toolkit — what changed and what did not) — [`OrderDesk.Web/docs/ModernizationNotes.md`](OrderDesk.Web/docs/ModernizationNotes.md); code: [`Services/ThemeService.cs`](OrderDesk.Web/Services/ThemeService.cs), [`Themes/orderdesk.mixin.theme`](OrderDesk.Web/Themes/orderdesk.mixin.theme)
2. **Client profiles and responsive layouts** — [`OrderDesk.Web/docs/ResponsiveProfiles.md`](OrderDesk.Web/docs/ResponsiveProfiles.md); code: [`ClientProfiles.json`](OrderDesk.Web/ClientProfiles.json), [`Services/ResponsiveLayout.cs`](OrderDesk.Web/Services/ResponsiveLayout.cs)
3. **Security review** (six items with evidence, the AllowHtml injection) — [`OrderDesk.Web/docs/SecurityReview.md`](OrderDesk.Web/docs/SecurityReview.md); code: [`Security/`](OrderDesk.Web/Security)
4. **Deployment checklist + assets** — [`OrderDesk.Web/docs/DeploymentChecklist.md`](OrderDesk.Web/docs/DeploymentChecklist.md), [`deploy/`](OrderDesk.Web/deploy)
5. **Final readiness checklist** — [`OrderDesk.Web/docs/ReadinessChecklist.md`](OrderDesk.Web/docs/ReadinessChecklist.md); code: [`Diagnostics/ReadinessChecklist.cs`](OrderDesk.Web/Diagnostics/ReadinessChecklist.cs), [`Diagnostics/StaticStateAudit.cs`](OrderDesk.Web/Diagnostics/StaticStateAudit.cs), [`Diagnostics/HealthCheck.cs`](OrderDesk.Web/Diagnostics/HealthCheck.cs)
6. **Capstone migration report** (reused · adapted · modernized · risks · next slice) — [`OrderDesk.Web/docs/CapstoneReport.md`](OrderDesk.Web/docs/CapstoneReport.md)
7. **Migration log** — [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md), carried forward from Modules 1–6 and closed with the Module 7 section

## Self-check answers (lab guide + storyboard)

- **Which business logic was reused as-is?** `Domain/` in full — `OrderService` (totals, discount, tax, search, save),
  `CustomerService`, `InvoiceDocument`, the models. Module 7 changed none of it: **New Order** still computes 1,280.00,
  the search box calls the same `OrderService.Search`. Modernization touched the shell (toolbar, watermark, toast,
  theme, layouts), never the rule.
- **Which desktop boundary was replaced with a web-safe pattern?** In this module, the *trust* boundary: the desktop
  trusted its own process (static user, `File.Open` on the user's disk, inert label text). The web replacements are
  `AuthService.Demand(permission)` on every server action, `DownloadGuard` (storage root only, `..` refused),
  `HtmlSanitizer` behind the one `AllowHtml = true` label, and the audit log — plus the earlier ones still in place
  (server PDF, `Application.Download`, storage root).
- **How is per-user state kept out of static fields?** `UserSessionContext` in `Application.Session`, read by
  `AuthService.Current` on every call; the **Static-state audit** proves it by reflection (0 open items). The only
  statics are readonly shared state behind locks — the repository, the audit list, the account table — which is
  shared *by design*, like a database, and `AuditLog._version`, a counter mutated inside the lock.
- **What was tested before calling the migrated feature complete?** The readiness checklist (8/8 with evidence), the
  health check in-session and over HTTP, the three layouts (simulate buttons + a resized browser), the security paths
  (kelly allowed, sam denied, wrong password denied, traversal rejected, raw HTML injecting), and two concurrent
  sessions sharing one audit log. Not tested and therefore not claimed: a phone device, an executed IIS/Docker
  deployment, a real load balancer.
- **Storyboard · "It runs in a browser — can you explain what was reused vs rebuilt?"** Yes: `docs/CapstoneReport.md`
  has the three tables (reused · adapted · modernized) with the module that did each, and every console button traces
  which side of the line it is on (`• server` reused logic vs `⚠ boundary` replaced assumption).
- **Storyboard · "Does it survive two concurrent users with logging and rollback?"** Two tabs, kelly and sam: separate
  session contexts, one audit log seen by both within a second, sam's export denied while kelly's download succeeds.
  Rollback: the build output and the storage root are separate (`docs/DeploymentChecklist.md`), so redeploying the
  previous build leaves uploads, exports and logs intact.
- **Storyboard · "Why never change visual design and business behaviour in the same step?"** A bug hides where you
  cannot see it: if the total were wrong after a theme change you could not tell which change caused it. Parity
  first (Modules 1–6), then the theme, mixin and layouts — with the 1,280.00 New Order as the regression check.

## Runtime facts

- `Application.LoadTheme(name)` swaps the theme object shared by every session in the process — the second tab
  restyles too. `LoadTheme(name, mixins)` re-merges the named `Themes/*.mixin.theme` files; the merged value is read
  back with `Application.Theme.GetStyle<object>("button", "radius", "default")`.
- `Application.ResponsiveProfileChanged` is a static event: subscribe in `Load`, unsubscribe in `Dispose`, and check
  `IsDisposed` in the handler. `Application.ActiveProfile.Name` comes from `ClientProfiles.json`; the framework
  reads the file next to the assembly, so the csproj copies it to the output for both target frameworks.
- `Label.AllowHtml = true` sets the element's innerHTML: a user-entered `<img onerror>` executes in the browser.
  Encoding is the default; the sanitizer is the only path allowed to turn it off on user data.
- `Application.Download(path, name)` is called with the **resolved** server path, never the browser's string.
- `/health` is a plain ASP.NET Core endpoint (`MapGet` in `Startup.cs`) — no Wisej session is created for it, so it
  can only report process-level facts; the in-session **Health check** adds sessions, theme, profile, web socket.
- The audit grid polls with a 1 s `Wisej.Web.Timer` and redraws only when `AuditLog.Version` or
  `Application.SessionCount` changed; the trace line `AuditLog (other session)` appears only for entries recorded by
  a different session id.
- `HttpClient` from inside a Wisej session works in `Application.StartTask` + `Application.Update(this, …)`; the probe
  URL is hard-wired to `http://localhost:5607/health` on purpose (a deployment's probe is too).
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
