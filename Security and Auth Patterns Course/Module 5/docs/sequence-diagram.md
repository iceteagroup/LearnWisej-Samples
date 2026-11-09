# AccessOps · partner federation, sanitized sequence (Module 5)

Drawn from the observed run (`docs/run-trace.txt`). Values that are secrets or single-use
protocol material are shown as placeholders only: `‹state›`, `‹nonce›`, `‹verifier›`, `‹code›`,
`‹id_token›`, `‹handle›`, `‹client secret›`. None of them appears in any AccessOps or provider log line.

```mermaid
sequenceDiagram
    autonumber
    participant B as Browser tab (Wisej.NET session in sessionStorage)
    participant W as AccessOps · Wisej.NET session
    participant H as AccessOps · ASP.NET Core handlers (before UseWisej)
    participant P as Contoso Identity (test provider)

    B->>W: click "Sign in with your work account" (WebSocket)
    W-->>B: IExternalSignIn.Begin() → Application.Navigate("account/signin")
    B->>H: GET /account/signin
    H-->>B: 302 to /connect/authorize?response_type=code&scope=openid profile email&code_challenge=S256(‹verifier›)&state=‹state›&nonce=‹nonce›<br/>Set-Cookie correlation + nonce (HttpOnly, SameSite=Lax)
    B->>P: GET /connect/authorize (redirect_uri must match exactly)
    P-->>B: Contoso Identity sign-in page
    B->>P: e-mail + password (typed at the provider only), then the second factor: Approve
    P-->>B: 302 to /signin-oidc?code=‹code›&state=‹state›&iss=…
    B->>H: GET /signin-oidc (correlation + nonce cookies ride along: Lax, top-level GET)
    H->>H: state unprotects and matches the correlation cookie
    H->>P: POST /connect/token: ‹code› + ‹verifier› + client_id + ‹client secret› (back channel)
    P-->>H: ‹id_token› (RS256), access token
    H->>H: validate ‹id_token›: signature (JWKS), iss, aud, exp, nonce
    H->>H: OnTokenValidated: ExternalIdentity(iss, sub) parked in ExternalHandoffStore
    H-->>B: Set-Cookie AccessOps.Handoff=‹handle› (HttpOnly, Secure, SameSite=Strict, 2 min) · 302 /Default.html · HandleResponse()
    B->>W: page load (same tab: ApplicationRefresh; a new session: Program.Main)
    W->>W: TryCompleteOnPageLoad: Application.Cookies["AccessOps.Handoff"], expire it, redeem ‹handle› once
    W->>W: ExternalIdentityMapper: (iss, sub) → external_logins → account, approved tenant, role (AccessOps data)
    W->>W: GrantStore.Issue → exactly one grant
    W->>W: RefreshSessionId() → ApplicationPrincipal.Establish (Application.User)
    W-->>B: ConsolePage "AccessOps · Records" + Application.Call("AccessOpsAuth.issueCookie", ‹cookie handle›)
    B->>H: POST /auth/cookie (antiforgery) → host ticket AccessOps.Auth (Secure, HttpOnly, SameSite=Strict)

    Note over B,P: Refusals (F06 changed state, F07 replayed callback, provider denial) end at step 10-12 in OnRemoteFailure / OnAccessDenied → 302 /Default.html?signin=failed. F08 ends at step 19 (unapproved tenant). No grant in any of them.

    B->>W: click Sign out
    W->>W: SignOutService: revoke the account's grants · Application.User = null
    W-->>B: Application.CallAsync("AccessOpsAuth.signOut", "signout-external")
    B->>H: POST /auth/signout (antiforgery) → host cookie expired
    W-->>B: Application.Exit() → tab reloads (autoReload)
    B->>H: POST /account/signout (form, antiforgery) → Results.SignOut(Cookies, OpenIdConnect)
    H-->>B: 302 to /connect/endsession?post_logout_redirect_uri=…/signout-callback-oidc
    B->>P: GET /connect/endsession → provider session ended
    P-->>B: 302 /signout-callback-oidc → /Default.html?ended=signout
```
