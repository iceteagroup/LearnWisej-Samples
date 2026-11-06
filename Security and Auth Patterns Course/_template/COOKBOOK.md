# AccessOps cookbook - Wisej-4 4.1.7 / .NET 10

Everything here was established by building and running the AccessOps labs against the public
`Wisej-4` **4.1.7** package from nuget.org, and reading the result back. Each entry says how.

## Structure

One solution per module under `Module N/`, the web application in `Module N/AccessOps`
(namespace `AccessOps`), ports 6401-6407. From Module 3 the solution also carries
`AccessOps.Tests` (xUnit, `dotnet test`), from Module 4 `AccessOps.TestIdentityProvider` (a local
OpenID Connect test double on 6414-6417). Single target `net10.0`.

Runtime data (SQLite database, documents, quarantine, logs) lives in `Module N/AccessOpsData/`,
beside the project folder and therefore outside the web root (`WebRootPath = "./"` is the project
folder). The course `.gitignore` keeps it out of git.

## Package versions

- `Wisej-4` **4.1.7**, the released package. It has **no** `Wisej.Web.Passkey`,
  `Wisej.Web.OAuthClient` or `IPasskeyCredentialStore` - those arrive with Wisej.NET 4.2. Checked
  by reflection over `lib/net8.0/Wisej.Framework.dll` (file version 4.1.7.0): `GetType(...)`
  returns null for all three.
- `Microsoft.Data.Sqlite` **10.0.14**. `10.0.0` pulls `SQLitePCLRaw.lib.e_sqlite3` 2.1.11, which
  restore flags as `NU1903` (high, GHSA-2m69-gcr7-jv3q); 10.0.14 pulls 2.1.13 and restores clean.

## Members verified in 4.1.7 (reflection)

`Application`: `RefreshSessionId()`, `SetSessionTimeout(int)`, `Exit()`, `Reload()`,
`Navigate(url, target)`, `RunInContext(IWisejComponent, Action)`, `Update(IWisejComponent, Action)`,
`Download(Stream, string, Action<string>)`, `Download(string, string, Action<string>)`,
`Call(string, params object[])`, `Eval(string)`, `StartTask(Action)`; properties `Session`
(dynamic), `SessionId`, `User`, `IsAuthenticated`, `UserHostAddress`, `UserAgent`, `QueryString`,
`IsSecure`, `IsWebSocket`, `Url`, `StartupUrl`, `Cookies`, `Services`, `Configuration`, `Browser`;
events `SessionTimeout` (HandledEventHandler), `ApplicationStart`, `ApplicationExit`,
`ApplicationRefresh`, `BeginRequest`, `ThreadException`.

`Application.Cookies.Add(name, value, domain, expires, secure, httpOnly, sameSite)` exists; the
last parameter is the nested enum `Wisej.Base.Cookie.SameSiteMode`.

`Application.Services` is `Wisej.Services.ServiceProvider`; `ServiceLifetime` is
`Shared, Session, Thread, Transient`.

`MessageBox.Show(text, caption, buttons, icon, defaultButton, modal, allowHtml = true, ...)` -
**allowHtml defaults to true**. Pass `allowHtml: false` whenever the text contains user input.

`Wisej.Core.Configuration` (what `Application.Configuration` returns) exposes `SessionTimeout`,
`SessionStorage`, `Secure`, `ValidateClient`, `ValidateResources`, `MaxSessions`,
`MaxSessionsPerClient`, `Debug`, `EnableWebSocket`, `Settings`, ... - so a screen can report the
real values instead of repeating Default.json from memory.

Upload: `UploadedEventArgs.Files` is `Wisej.Core.HttpFileCollection`; each `HttpPostedFile` has
`FileName`, `ContentType`, `ContentLength`, `InputStream`, `LastModified`, `SaveAs(path)`.

## What the browser holds for a DataGridView (Module 1)

Read back in the browser pane with
`qx.core.ObjectRegistry` -> the `wisej.web.datagrid.DataModel` -> `getRowData(3).data`.

- Bound to `EmployeeListItem` (Id, DisplayName, Department, Role, ManagerName) with four columns
  (`AutoGenerateColumns = false`): the client row is
  `{"1":"Kwame Osei","2":"Finance","3":"Manager","4":"A. Lindqvist"}`. **Properties without a
  column are not sent** - the Id stayed on the server.
- The same grid with one extra column bound to `Id` and `Visible = false`: the client row is
  `{..., "4":"A. Lindqvist","5":"4187"}`. **A hidden column is serialized to the browser.**
  `Visible = false` changes what is drawn, not what is sent. The only way to keep a salary band
  on the server is to never give it a column (or a property of a bound row).

## Static files and .json

With `WebRootPath = "./"` the project folder is the web root, so `Default.json` and
`appsettings.json` sit in it. With
`app.UseWhen(cx => !cx.Request.Path.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase), a => a.UseFileServer())`
a `fetch` from the running page returned: `/Default.json` 404, `/appsettings.json` 404,
`/Data/seed-employees.json` 404, `/Default.html` 200, `/AccessOps.csproj` 404 (unknown content type,
the static file middleware refuses it anyway).

## SQLite

`Microsoft.Data.Sqlite` turns `PRAGMA foreign_keys` **on** for every connection. A seed that
inserts an employee before its manager fails with `SQLite Error 19: FOREIGN KEY constraint
failed`; `PRAGMA defer_foreign_keys = ON` inside the seeding transaction checks the keys at commit.

## Browser pane

The pane in this environment is small; screenshots come back at roughly 324x208 for a 1400x900
viewport. Read state with `get_page_text`, `find` and `javascript_tool`, and use screenshots for
layout only. Grid cells are not in the accessibility tree; click rows by coordinate.

## Session ID renewal: `Application.RefreshSessionId()` (Module 2)

Logged `Application.SessionId` and `Application.IsWebSocket` around the call:

- **From a WebSocket event (a button click):** the server switched to the new ID
  (`0f72… → aa50…`), but the browser never adopted it. Its next request created a brand-new
  session (`e7ea…`, `Program.Main` ran again) and the signed-in state was gone.
- **From a page load (HTTP, inside `Application.ApplicationRefresh`):** `b942… → 2e9f…`, the
  browser's `sessionStorage["Wisej.SessionId"]` became `2e9f…`, and a further reload kept the
  session (ApplicationRefresh fired again with the same page).
- **From `Program.Main`** (also a page load) the call works the same way.

So AccessOps verifies the password on the click, parks the verified context under a pending key
(it is not the session identity yet), calls `Application.Reload()`, and on the page load calls
`RefreshSessionId()` first and only then stores the `SessionContext`
(`Infrastructure/WisejSession.cs`). The order the lesson asks for - new ID, then identity - holds.

## Ending a session: `Application.Exit()` (Module 2)

- The 4.1.7 client handles the exit action by dropping its session ID and calling
  `Wisej.onExit`, which is `location.reload()` when the configuration's `autoReload` is true and
  **`window.close()`** otherwise. With the default, a script-opened tab simply closed. AccessOps
  sets `"autoReload": true` in Default.json so the tab reloads into a new session on the login page.
- Script sent with `Application.Eval` in the same request as `Application.Exit()` never ran - the
  exit won. To tell the next page why the session ended, AccessOps calls a page function with
  `Application.Call("accessOpsEnded", callback, "idle")` and calls `Exit()` in the callback; the
  function keeps the word in sessionStorage and Default.html turns it into `?ended=idle`.
- `Application.SessionTimeout` with `"sessionTimeout": 60` fired 61 s after the last click; with
  `e.Handled = true` no prolong dialog appeared and the handler's sign-out ran (audit row
  `SignOutIdle`, reload to the login page with the idle sentence).

## Cookies written from Wisej.NET (Module 2)

`Application.Cookies.Add(name, value, null, expires, true, true, Wisej.Base.Cookie.SameSiteMode.Strict)`
written during `ApplicationRefresh` (a page load) on `http://localhost`: `document.cookie` is empty
(HttpOnly), and a new session in the same browser (sessionStorage cleared, page reloaded) read the
value back through `Application.Cookies[name]` in `Program.Main` and signed in through the
remembered-device path. Chrome accepted the `Secure` flag on `http://localhost`.

## Client address

`Application.UserHostAddress` reported `::1` for some requests and `127.0.0.1` for others from the
same browser pane. A per-address throttle keyed on the raw string counts them separately; behind a
real proxy the address comes from `app.UseForwardedHeaders()` for the proxies you trust.

## External sign-in on 4.1.7: ASP.NET Core OpenID Connect + a Wisej.NET hand-off (Module 4)

Verified end to end in the browser pane against the module's test identity provider:

- `Microsoft.AspNetCore.Authentication.OpenIdConnect` **10.0.14** works beside `app.UseWisej()`.
  `app.UseAuthentication()` and the `MapGet` endpoints are registered before `UseWisej()`; the
  handler's `/signin-oidc` and `/signout-callback-oidc` are served by the authentication middleware.
- The handler's default `ResponseMode` is **`form_post`** (seen in the authorize URL). The test
  provider answers with a query redirect; set `ResponseMode = OpenIdConnectResponseMode.Query` so the
  request says what the provider does.
- On plain `http://localhost` set `CorrelationCookie.SameSite` and `NonceCookie.SameSite` to `Lax`:
  the handler's default `None` without `Secure` is rejected by the browser. The callback is a
  top-level GET, so Lax cookies are sent.
- In `OnTokenValidated` the identity is parked server-side and the browser gets a random handle in an
  HttpOnly cookie; `ctx.HandleResponse()` skips the cookie sign-in entirely (`SaveTokens = false`,
  tokens kept from `ctx.TokenEndpointResponse`).
- **`Application.Cookies[name]` reads an HttpOnly cookie written by ASP.NET Core** on the next page
  load, both in `Program.Main` (new session) and in `ApplicationRefresh` (the tab's session survived
  the round trip through the provider - the session ID in sessionStorage is untouched by same-tab
  navigation).
- A forged callback (`/signin-oidc?code=forged&state=forged`) ends in `OnRemoteFailure` with
  `Unable to unprotect the message.State.`
- `Results.SignOut(props, cookie, OpenIdConnect)` from a GET endpoint sends the browser to the
  provider's `end_session_endpoint` with `post_logout_redirect_uri = …/signout-callback-oidc`, then
  to `props.RedirectUri`.
- A refresh-token grant with `IHttpClientFactory` against `options.ConfigurationManager`'s
  `TokenEndpoint` works from a Wisej.NET async handler (`await`, then `Application.Update(this)`).

## Uploads, downloads and the share-link endpoint (Module 5)

- The 4.1.7 `Upload` control renders an `<input type="file">` whose `accept` is the
  `AllowedFileTypes` value (`.pdf,.docx,.xlsx`). It can be driven from a test without a file
  picker: build a `File`, put it in a `DataTransfer`, assign `input.files` and dispatch `change`;
  the server's `Uploaded` event fires with `e.Files[0].FileName`, `.ContentLength`, `.InputStream`.
- The browser encodes `"` in a multipart file name as `%22`: an upload named
  `<img src=x onerror="…">.pdf` arrived as `<img src=x onerror=%22…%22>.pdf`. Shown in a `Label`, a
  grid cell and through `Application.Call` → `textContent`, it stayed text (no element created).
- `Application.Download(new MemoryStream(bytes), name)` from a click handler streams the file to the
  current session; there is no URL to share.
- A minimal-API share endpoint with `(int id, long? exp, string sig, ...)` binds a missing `exp` or
  `sig` as null (so the endpoint can answer 404 like every other failure, instead of a binding 400).
  `Results.File(bytes, contentType, name)` sends
  `Content-Disposition: attachment; filename="…"; filename*=UTF-8''…`.
- `builder.WebHost.ConfigureKestrel(k => k.Limits.MaxRequestBodySize = …)` is the host ceiling that
  matches `Upload.MaxFileSize` (an `int`, bytes).

## Audit, correlation, secrets (Module 6)

- `GenerateAssemblyInfo=false` (the Wisej.NET project template default) also switches off the
  generated `UserSecretsId` attribute: `dotnet user-secrets set` wrote the values, but
  `WebApplication.CreateBuilder` never loaded them (`Oidc:ClientSecret is not configured`). With
  `GenerateAssemblyInfo=true` (no `AssemblyInfo.cs` in the project) the secrets load in Development.
- `ILogger.BeginScope(dictionary)` opened at the top of a Wisej.NET click handler flows into every
  log line written during the click, including services and `HttpClient`. A custom
  `ILoggerProvider` that implements `ISupportExternalScope` receives the scopes; over the WebSocket
  ASP.NET Core's own scope is per connection (`RequestPath: /ws.wx`, one `TraceId` for many clicks),
  so the per-click correlation ID has to be your own.
- Scope dictionaries from `HttpClient` logging carry a `{OriginalFormat}` key - drop keys starting
  with `{` in the sink.
- `ShowDialog()` without `await` keeps the calling handler (and its `using` scope) open until the
  dialog closes; end the command before showing a dialog.
- A grid re-bound with `DataSource = …` loses its current row; keep the selected id and set
  `CurrentCell` again, or the next toolbar action lands on row 0.
- SQLite has no grants: an INSERT-only audit table is two `BEFORE UPDATE/DELETE … RAISE(ABORT, …)`
  triggers; `DROP TRIGGER` by the file owner removes them, which is why the hash chain matters.

## Host hardening (Module 7)

- The 4.1.7 client honours `"secure": true` in Default.json: loaded over `http://localhost`, it
  navigated itself to `https://localhost:6407/`. Wisej.NET has one Default.json, so the lab keeps
  `false` and a publish target (`AfterTargets="Publish"`, `ReadAllText` + `Replace` +
  `WriteLinesToFile`) writes `true` into the published copy.
- `Application.Configuration.Secure`, `.ValidateClient`, `.SessionStorage` (enum value `Session`)
  read the live values; `sessionStorage` holds `Wisej.SessionId`, `Wisej.TabId`,
  `Wisej.StartupUrl`; `document.cookie` stays empty.
- CSP the 4.1.7 client ran under, enforced, with no violation reports across sign-in, the OIDC round
  trip, grids, dialogs, uploads and sign-out:
  `default-src 'self'; script-src 'unsafe-inline' 'unsafe-eval' 'self'; style-src 'unsafe-inline' 'self'; img-src 'self' data:; font-src 'self' data:; connect-src 'self'; object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'none'`
  (`connect-src 'self'` covers the same-origin WebSocket in Chromium).
- After switching from report-only to enforce, a revalidated (304) `fetch('/Default.html')` showed
  both headers from the browser cache; `cache: 'no-store'` showed only the new one.
- The OIDC hand-off cookie works as `SameSite=Strict`: it is set on the cross-site `/signin-oidc`
  response and read by the start page's own same-origin `app.wx` request. The handler's correlation
  and nonce cookies must stay `Lax` (they ride the provider's cross-site callback).
- `Upload.MaxFileSize`: a larger file is never sent; the control raises `Error` with
  `UploadErrorEventArgs.ErrorType == UploadErrorType.FileTooLarge`, `FileNames`, `FileSizes`
  (events: `Uploading` (cancellable), `Uploaded`, `Error`, `Progress`). Without an `Error` handler the
  user sees nothing.
- A raw POST over the Kestrel `MaxRequestBodySize` to `/app.wx` is dropped (`Request body too
  large`); the browser reports `Failed to fetch`.
- Endpoint rate limits (`AddRateLimiter` + `RequireRateLimiting`) work beside `UseWisej()`; a click
  cannot be limited that way - the login token bucket lives in the login service.
- `Microsoft.AspNetCore.TestHost` 10.0.14 runs the same middleware in xUnit with `https://` and a
  real host name, so redirect + HSTS are testable without a certificate (`UseHsts` never sends the
  header for `localhost`).