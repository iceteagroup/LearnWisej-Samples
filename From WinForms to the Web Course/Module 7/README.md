# OrderDesk.Web · From WinForms to the Web · Module 7

Local lab build for **Module 7 · Modernize, Secure, and Deploy the Capstone**. The working port from Modules 1–6 is
**modernized** (theme + mixin, tool buttons, watermark, toast) without touching the business logic, made
**responsive** through `ClientProfiles.json` and three explicit layouts, **secured as a web app** (server-side sign-in
and a permission check on every action, a download guard, `AllowHtml` only after a sanitizer, an audit log), given an
**operations dashboard** and a **health endpoint**, and packaged with **IIS / Docker deployment assets**. The
**capstone report** (reused · adapted · modernized · risks · next slice) closes the course.

The deployment files under `deploy/` are written to be run, not executed by the sample.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 7/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5607
```

Then open <http://localhost:5607>. (Visual Studio: open `OrderDesk.slnx`, F5.) `GET http://localhost:5607/health`
answers JSON without a browser session.

## What to try

| Action | What you should see |
|---|---|
| Page load | "OrderDesk" in Bootstrap-4 with the mixin's rounded buttons; the Orders tab shows 1042 Northwind Traders 4,820.00 Open first; the header says *Not signed in* |
| Toolbar **New Order** | order 1043 (Litware, 1,280.00 — the same total as Module 1) at the top and a toast; the Dashboard KPIs move |
| Toolbar **Print Invoice** | the invoice PDF opens in a modal `PdfViewer` |
| Resize the browser below 1025 px, then below 601 px | tablet: short toolbar text and the detail below the grid; phone: the toolbar becomes an *Actions…* dropdown and the detail is hidden |
| **Sign in kelly** → **Export** | `orders.csv` downloads through the download guard (kelly is a Manager) |
| **Sign in sam** → **Export** | a warning toast: `'orders.export' denied: role Clerk lacks 'orders.export'` — the server checks every action |
| Type `<b>Rush</b> order <img src=x onerror="alert(1)"> <i>ship Friday</i>` into **Order note** | the preview shows **Rush** order *ship Friday*; the `<img>` is removed, nothing runs |
| **Second session ↗** → sign in as sam there → back in the first tab, **Dashboard** | the recent-activity feed shows sam's sign-in within a second; `● live · 2 sessions` |
| **Dashboard** | KPIs (Open orders, Revenue today, Invoiced, On-time %), the *Orders by status* chart and the audit-logged activity feed |

## Where things live

```
Module 7/
└─ OrderDesk.Web/                     the Wisej.NET 4 app (net10.0-windows;net10.0)
   ├─ Program.cs / Startup.cs         session entry point / Kestrel host + the /health endpoint (MapGet, no Wisej session)
   ├─ Default.html / Default.json / Web.config   theme Bootstrap-4 · OrderDesk.StorageRoot, ThemeMixin, Version
   ├─ ClientProfiles.json             Phone ≤600 · Tablet 601–1024 · Desktop ≥1025 (copied next to the assembly, never served)
   ├─ Themes/orderdesk.mixin.theme    button + toolbar-button radius 14, orderdesk-accent colour
   ├─ Domain/                         unchanged since Module 1 — the reused business logic
   ├─ Services/ResponsiveLayout.cs    profile → Desktop / Tablet / Phone layout, in code
   ├─ Services/ThemeService.cs, AppConfig.cs
   ├─ Security/AuthService.cs         SignIn (salted hashes) · Authorize / Demand per action · roles Manager / Clerk
   ├─ Security/DownloadGuard.cs       permission → resolve under the storage root → exists → Application.Download
   ├─ Security/HtmlSanitizer.cs       Encode · Sanitize (whitelist b, i, br)
   ├─ Security/AuditLog.cs            process-wide append-only audit log with a Version counter
   ├─ Security/UserSessionContext.cs  the per-session user (Module 4)
   ├─ Diagnostics/HealthCheck.cs      the /health report
   ├─ Diagnostics/AppLog.cs           App_Data/logs/orderdesk-yyyyMMdd.log + System.Diagnostics.Trace
   ├─ Reporting/                      InvoicePdfWriter (server PDF), CsvExport (in-memory export)
   ├─ Views/                          Ui, InvoicePreviewForm
   ├─ MainPage.cs / .Designer.cs      sign-in header, Orders tab, Dashboard tab
   ├─ deploy/                         IIS web.config, Dockerfile, docker-compose.yml, production settings notes
   └─ docs/                           the lab deliverables + migration-log.md
```

## Deliverables

1. **Modernization notes** (theme, mixin, toolkit): [`OrderDesk.Web/docs/ModernizationNotes.md`](OrderDesk.Web/docs/ModernizationNotes.md); [`Themes/orderdesk.mixin.theme`](OrderDesk.Web/Themes/orderdesk.mixin.theme)
2. **Client profiles and responsive layouts**: [`OrderDesk.Web/docs/ResponsiveProfiles.md`](OrderDesk.Web/docs/ResponsiveProfiles.md); [`ClientProfiles.json`](OrderDesk.Web/ClientProfiles.json), [`Services/ResponsiveLayout.cs`](OrderDesk.Web/Services/ResponsiveLayout.cs)
3. **Security review** (six items, the AllowHtml injection): [`OrderDesk.Web/docs/SecurityReview.md`](OrderDesk.Web/docs/SecurityReview.md); [`Security/`](OrderDesk.Web/Security)
4. **Deployment checklist + assets**: [`OrderDesk.Web/docs/DeploymentChecklist.md`](OrderDesk.Web/docs/DeploymentChecklist.md), [`deploy/`](OrderDesk.Web/deploy)
5. **Final readiness checklist**: [`OrderDesk.Web/docs/ReadinessChecklist.md`](OrderDesk.Web/docs/ReadinessChecklist.md)
6. **Capstone migration report**: [`OrderDesk.Web/docs/CapstoneReport.md`](OrderDesk.Web/docs/CapstoneReport.md)
7. **Migration log**: [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md)

## Self-check answers (lab guide + video)

- **Which business logic was reused as-is?** `Domain/` in full — `OrderService` (totals, discount, tax, search, save),
  `CustomerService`, `InvoiceDocument`, the models. **New Order** still computes 1,280.00 and the search box calls the
  same `OrderService.Search`. Modernization touched the shell (toolbar, watermark, toast, theme, layouts), never the rule.
- **Which desktop boundary was replaced with a web-safe pattern?** In this module, the *trust* boundary: the desktop
  trusted its own process (static user, `File.Open` on the user's disk, inert label text). The web replacements are
  `AuthService.Demand(permission)` on every server action, `DownloadGuard` (storage root only, `..` refused),
  `HtmlSanitizer` behind the one `AllowHtml = true` label, and the audit log.
- **How is per-user state kept out of static fields?** `UserSessionContext` in `Application.Session`, read by
  `AuthService.Current` on every call. The only statics are readonly shared state behind locks — the repository, the
  audit list, the account table — shared *by design*, like a database.
- **What was tested before calling the migrated feature complete?** The three layouts in a resized browser, the security
  paths (kelly exports, sam is denied, the note preview strips the `<img onerror>`), `/health`, and two concurrent
  sessions sharing one audit log. Not tested and therefore not claimed: a phone device, an executed IIS/Docker
  deployment, a real load balancer.
- **Why never change visual design and business behaviour in the same step?** A bug hides where you cannot see it: if
  the total were wrong after a theme change you could not tell which change caused it. Parity first (Modules 1–6),
  then the theme, mixin and layouts — with the 1,280.00 New Order as the regression check.

## Runtime facts

- Every `Themes/*.mixin.theme` file is merged over the `Default.json` theme at startup. `Application.LoadTheme(name)`
  swaps the theme object shared by every session in the process.
- `Application.ResponsiveProfileChanged` is a static event: subscribe in `Load`, unsubscribe in `Dispose`, and check
  `IsDisposed` in the handler. `ClientProfiles.json` is read next to the assembly, so the csproj copies it to the output.
- `Label.AllowHtml = true` sets the element's innerHTML: a user-entered `<img onerror>` executes in the browser.
  Encoding is the default; the sanitizer is the only path allowed to turn it off on user data.
- `Application.Download(path, name)` is called with the **resolved** server path, never the browser's string.
- `/health` is a plain ASP.NET Core endpoint (`MapGet` in `Startup.cs`) — no Wisej session is created for it.
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
