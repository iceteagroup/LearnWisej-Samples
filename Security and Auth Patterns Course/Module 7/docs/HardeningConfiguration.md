# Hardening configuration · AccessOps Module 7

Everything the host owns is in one file, [Infrastructure/Hardening.cs](../AccessOps/Infrastructure/Hardening.cs),
registered in [Startup.cs](../AccessOps/Startup.cs) **first in the pipeline**, before
`MapAccessOpsAccountEndpoints()`, `MapShareLinks()` and `app.UseWisej()` - so the Wisej.NET pages,
the start page, the OIDC callback, the share link and even a `429` carry the same headers.

```
UseForwardedHeaders  →  UseHsts + UseHttpsRedirection (RequireHttps)  →  security headers
→  UseCookiePolicy  →  UseRateLimiter  →  account endpoints, /share/{id}, /csp-report  →  UseWisej  →  file server (no .json)
```

Settings live in `Hardening` in [appsettings.json](../AccessOps/appsettings.json) with **production
values**; [appsettings.Development.json](../AccessOps/appsettings.Development.json) switches off only
`RequireHttps` for the `http://localhost` lab, and is excluded from publish.

## HTTPS, HSTS and `secure`

| Control | Production | Lab (Development) | Verified |
|---|---|---|---|
| HTTP → HTTPS | `UseHttpsRedirection`, 308, port `Hardening:HttpsPort` | off | test `Plain_http_is_redirected_to_https`: `http://accessops.contoso.example/Default.html?x=1` → 308 `https://…/Default.html?x=1` |
| HSTS | `max-age=31536000`, no `includeSubDomains` until every subdomain serves TLS, no preload | off (and ASP.NET Core never sends it for `localhost`) | test `Https_responses_carry_hsts_and_the_security_headers` |
| Wisej client `secure` | the **published** `Default.json` has `"secure": true` (csproj target `SecureWisejClientOnPublish`) | `"secure": false` | `dotnet publish` output: `AccessOps: published Default.json has "secure": true`. With `"secure": true` on the http lab, the 4.1.7 client loaded over http and then navigated to `https://localhost:6407/` (`ERR_SSL_PROTOCOL_ERROR`, no certificate) - the key is honoured |
| Behind a proxy | `UseForwardedHeaders` trusting only `Hosting:KnownProxies` (loopback by default) | same | - |

No certificate was created, installed or trusted for this lab; the HTTPS-only behaviour is checked
in memory with `Microsoft.AspNetCore.TestHost` (`AccessOps.Tests/HardeningTests.cs`).

## Security headers

Read back from the running app with `fetch('/Default.html', {cache: 'no-store'})`:

```
content-security-policy: default-src 'self'; script-src 'unsafe-inline' 'unsafe-eval' 'self'; style-src 'unsafe-inline' 'self';
                         img-src 'self' data:; font-src 'self' data:; connect-src 'self'; object-src 'none'; base-uri 'self';
                         form-action 'self'; frame-ancestors 'none'; report-uri /csp-report
x-content-type-options:  nosniff
referrer-policy:         strict-origin-when-cross-origin
x-frame-options:         DENY
permissions-policy:      camera=(), microphone=(), geolocation=(), payment=()
```

No `Server:` header (`AddServerHeader = false`). The share link (`200 application/pdf`,
`Content-Disposition: attachment`) and its `429` carry the same set.

**Why the CSP keeps `'unsafe-inline' 'unsafe-eval'`.** The Wisej.NET client evaluates script and
writes inline styles, and `Default.html` has two small inline functions (the sign-out hand-off and
the toast). `script-src 'self'` alone stops the client. `'unsafe-eval'` stays even in the stricter
nonce form; a nonce-based start page is the next step and is not in this sample.

**Report-only first.** `Hardening:CspMode` was `ReportOnly` for the first pass: sign-in page,
work-account round trip, dashboard, documents (upload, download, share-link dialog), approvals
(approve), employees, admin console, sign-out. Violations are posted to `/csp-report` and logged as
`CSP violation reported: …`; **none** was reported. `CspMode` was then switched to `Enforce` and the
sign-in page, the provider round trip, dashboard, documents (with the oversized-upload refusal),
employees, admin console and sign-out were walked again - all loaded and worked, no
reports, no console errors from the app.

Browser caching note: after the switch, a `fetch('/Default.html')` that the browser revalidated
(304) still showed the old `Content-Security-Policy-Report-Only` header next to the new one; with
`cache: 'no-store'` only the enforced header came back.

## `.json` and other files

`app.UseWhen(cx => !cx.Request.Path.Value.EndsWith(".json", …), a => a.UseFileServer())` -
`/Default.json`, `/appsettings.json` and `/appsettings.Development.json` answered **404** from the
running app. The web root has no third-party JavaScript files (the Wisej.NET client is served from
the assembly as `resource.wx/…`).

## Cookies and the session

| What | Where it lives / flags | Verified |
|---|---|---|
| Wisej.NET session ID | `sessionStorage` (`Wisej.SessionId`, one per tab), not a cookie - so there is no HttpOnly/SameSite to set. Protected by TLS + `secure`, `validateClient: true`, `RefreshSessionId()` after sign-in (Module 2) and no script injection | `Object.keys(sessionStorage)` → `Wisej.StartupUrl, Wisej.SessionId, Wisej.TabId`; `document.cookie` → empty |
| `AccessOps.Handoff` (one-time OIDC hand-off, 2 min) | HttpOnly, `SameSite=Strict`, Secure on HTTPS; written by ASP.NET Core, expired from Wisej.NET with `Application.Cookies.Add(…, Application.IsSecure, true, SameSiteMode.Strict)` | the work-account round trip still completes with Strict: the start page's own same-origin requests carry it |
| OIDC correlation / nonce cookies | HttpOnly, `SameSite=Lax` (the provider's callback is a cross-site top-level GET), Secure under the cookie policy once HTTPS-only | sign-in works |
| Any cookie ASP.NET Core writes | `CookiePolicyOptions`: `HttpOnly = Always`, `Secure = Always` when `RequireHttps`, minimum `SameSite = Lax` | test `Cookies_written_through_aspnetcore_get_secure_httponly_and_samesite` |
| `sessionTimeout` | 1200 s in `Default.json`; `Application.SessionTimeout` handled with `e.Handled = true` and a sign-out (Module 2) | Module 2 |
| `validateClient` | `true`, stated explicitly in `Default.json` | footer reads `Application.Configuration.ValidateClient` → `true` |

`maxSessionsPerClient` is not set: one proxy address can be a whole office, so it would be a
secondary control at best.

## Request limits

| Limit | Value | Verified |
|---|---|---|
| Request body (host) | `Upload.MaxFileSize` + 64 KB envelope = 21 037 056 bytes (`ConfigureKestrel` in Startup) | a 21 MB `POST /app.wx` from the page: the host stopped reading and dropped the request (browser: `Failed to fetch`), logged `Request body too large. The max request body size is 21037056 bytes.` |
| `Upload.MaxFileSize` | 20 MB | a 21 MB file dropped on the Upload control: never sent; the `Error` event (`FileTooLarge`) now shows *oversized-report.pdf was refused: it is larger than the 20 MB limit. Nothing was uploaded.* and writes a `Refused:Size(client)` audit row |
| Headers | 32 KB total, 8 KB request line, 15 s to receive them | configuration |
| `/csp-report` | 8 KB, `413` above | code |

## Rate limits

| Where | How | Verified |
|---|---|---|
| Share link `/share/{id}` | ASP.NET Core rate limiter, token bucket per client address (10, +5/min), `429`, every refusal audited (`Document.ShareOpen`, `RateLimited`) under its own correlation ID | browser: 13 requests → ten `404` (bad signature) then `429 429 429`, audit rows 21-23 `RateLimited`; tests `The_share_link_answers_429…`, `The_429_carries_the_security_headers_too` |
| Password sign-in | `LoginRateLimiter` in `LoginService`: bucket per *user name + client address* (5, +1 per 12 s), taken **before** the lockout check and the hash; same sentence on screen as a wrong password; audited `RateLimited` | browser: seven wrong passwords for the service account → five `BadCredential`, then `RateLimited`, screen unchanged; test `Twenty_passwords_in_ten_seconds…` (even the right password is refused while the bucket is empty) |
| `/csp-report` | fixed window, 60/min per address | code |

The login buckets, the lockout counters and the hand-off/ticket stores are in process memory. With
more than one server they belong in a store all servers share.
