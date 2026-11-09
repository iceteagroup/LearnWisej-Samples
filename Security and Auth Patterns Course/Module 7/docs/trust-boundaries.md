# Trust boundaries · AccessOps release (Module 7)

Two entry points, one policy. Every arrow that crosses a dashed box is a trust boundary; the label
says what crosses it. Owners are roles, not people.

```mermaid
flowchart LR
  subgraph Browsers["Browsers (untrusted)"]
    SB["Staff browser<br/>domain-joined PC"]
    PB["Partner browser<br/>public internet"]
  end

  subgraph Ingress["Ingress · owner: Platform"]
    IIS1["IIS app 'Lab Intranet'<br/>Anonymous off · Windows auth on<br/>(lab: Kestrel + Negotiate :6408)"]
    LB["TLS proxy / balancer<br/>sticky sessions · WebSocket<br/>ingress switch (partner on/off)"]
    IIS2["IIS app 'Partner'<br/>Anonymous on · Windows auth off<br/>(lab: Kestrel :6407)"]
  end

  subgraph Directory["Directory · owner: Directory team"]
    AD["Active Directory<br/>accounts · groups · SPNs"]
  end

  subgraph IdP["Identity provider · owner: Partner IdP admin"]
    OIDC["Contoso Identity<br/>(lab: AccessOps.TestIdentityProvider :6417)"]
  end

  subgraph App["AccessOps nodes · owner: App team"]
    W["Wisej.NET session<br/>Application.User = app principal<br/>(account, tenant, grant claims)"]
    RP["RecordPolicy.Read<br/>grant · tenant · record<br/>AuditService.Deny"]
    SR["ScopedDocumentReader<br/>RunImpersonated(request token)"]
  end

  subgraph Store["Shared store · owner: Data owner (grants), Security (credentials)"]
    G[("grants + revocations")]
    H[("sign-in hand-off handles")]
    C[("passkey challenges + credentials")]
    K[("Data Protection key ring")]
    A[("audit (append-only, redacted)")]
  end

  subgraph Backend["Record backend · owner: Data owner"]
    R[("records repository")]
    F[("approved file root<br/>NTFS ACLs")]
    X[("\\\\files.contoso.test<br/>remote share (F04)")]
  end

  SB -- "Negotiate (Kerberos/NTLM) on the canonical host" --> IIS1
  IIS1 -- "ticket validation" --> AD
  IIS1 -- "host principal: WindowsIdentity" --> W
  PB -- "HTTPS only" --> LB --> IIS2
  IIS2 -- "anonymous transport" --> W
  PB -- "authorization code + PKCE" --> OIDC
  OIDC -- "callback /signin-oidc (state, nonce, code)" --> IIS2
  W -- "every protected read" --> RP
  RP -- "current grant, every call" --> G
  RP -- "only after an allow" --> R
  W -- "scoped read, after the policy" --> SR --> F
  SR -. "second hop: refused (no delegation target)" .-> X
  IIS2 -- "redeem once" --> H
  W -- "challenge, assertion, CAS commit" --> C
  IIS2 -- "protect / unprotect cookies + state" --> K
  RP --> A
```

## Identities: origin, trust, expiry, revocation

| Identity | Originates at | Trusted because | Expires | Revoked by (owner) |
|---|---|---|---|---|
| Host principal (`WindowsIdentity` as `Application.User` on the page load) | IIS + the directory (lab: Kestrel Negotiate, local account) | Kerberos ticket or NTLM exchange validated by the host before any Wisej.NET code runs (401 → 200) | the HTTP connection / ticket lifetime; never stored by AccessOps | Directory team: disable the account, remove it from the Reader group |
| App principal + grant (`account`, `tenant`, `grant` claims) | AccessOps, after a verified sign-in on either route | a grant row in the shared store, read on **every** call by `RecordPolicy` | grant: absolute 8 h (`GrantStore.DefaultLifetime`); principal: with the Wisej.NET session | AccessOps admin / Security: `GrantStore.Revoke`; the user: Sign out (all grants of the account) |
| Effective Windows token | the request's Windows token, intranet route only | `WindowsIdentity.RunImpersonated(identity.AccessToken, …)` for one approved file | at the end of the scope, also on an exception (observed: case 4) | the scope itself; the directory for the account |
| Partner external account (issuer + subject) | Contoso Identity | validated ID token: signature, `iss`, `aud`, `exp`, `nonce`; mapped through `external_logins` to an approved tenant | the provider transaction (hand-off handle 2 min) | Partner IdP admin **plus** an AccessOps grant revoke - disabling the user at the provider does not end a grant already issued |
| Passkey credential | the partner's authenticator; enrolled after a verified sign-in | signature over the challenge, exact origin, RP ID hash, owner (user handle), counter, CAS commit | until deleted | the owner (My passkeys → Delete) or Security (credential store) |
| Wisej.NET session | the node that served the first page load | session ID rotated on the sign-in page load (`RefreshSessionId`), client fingerprint | `sessionTimeout` 1200 s; lost with the node | `Application.Exit()` (sign-out) or timeout; a lost node loses its sessions |
| Host auth cookie `AccessOps.Auth` / antiforgery / OIDC correlation + nonce | issued over HTTPS responses by the partner node | protected by the shared Data Protection key ring + application name `AccessOps` | cookie: 8 h, non-sliding; correlation: the provider transaction | grant revoke (`OnValidatePrincipal` rechecks it), key-ring rotation (Platform) |

**Rule of the diagram:** none of the identities above is a permission. The grant row is, and only
`RecordPolicy` reads it - per call, from the shared store - before the repository or a scope runs.
