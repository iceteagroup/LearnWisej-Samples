# Log redaction review · AccessOps Module 6

**Method.** One real approval and one real download were run in the browser pane on 2026-11-06
(Jamie Mercer approving REQ-4471, Sofia Reyes downloading document 4471), plus the refusal that came
between them. Every line those commands wrote to `AccessOpsData/logs/accessops-YYYYMMDD.jsonl` was
read field by field and marked **necessary**, **redacted** or **removed**. The lines below are copied
from that file (JSON re-wrapped for reading; the `ConnectionId`/`RequestId` values shortened).

## The policy, enforced at the sink

[RedactingLogger.cs](../AccessOps/Infrastructure/RedactingLogger.cs) is the only logger provider
(`builder.Logging.ClearProviders()` in Startup). Before a line reaches the console or the file it:

- replaces the value of any structured property or `key=value` / `key: value` pair whose name is
  `password`, `pwd`, `secret`, `client_secret`, `token`, `access_token`, `refresh_token`,
  `id_token`, `authorization`, `cookie`, `set-cookie`, `signingkey`, `sig`, `code_verifier` or
  `connectionstring` with `[redacted]`;
- masks JWT-shaped strings (`eyJ….….…`), `Bearer …` values and e-mail addresses;
- drops message-template keys (`{OriginalFormat}`) that scopes from libraries carry;
- keeps the scope properties - `CorrelationId` and `UserId` from the command, `TraceId`,
  `ConnectionId`, `RequestPath` from ASP.NET Core.

The test `The_log_sink_redacts_secrets_and_keeps_the_correlation_scope` in
[AuditTests.cs](../AccessOps.Tests/AuditTests.cs) logs a known password marker, a JWT, a bearer
header, an e-mail address and a connection-string password through the real provider and asserts
none of them is in the file - and that all four lines carry the correlation ID.

A search of the whole run's log file found **0** unredacted `sig=` values (the share-link signature),
**0** JWT fragments and **0** e-mail addresses.

## The approval (correlation `1a7c-6c72`)

```json
{"ts":"20:56:12.617","level":"Information","category":"AccessOps.Commands",
 "SpanId":"49572154b5243c6f","TraceId":"c50d4dc98ac0561e28f5ea199032be5a","ParentId":"0000000000000000",
 "ConnectionId":"0HNPS9111CCB0","RequestId":"0HNPS9111CCB0:00000001","RequestPath":"/ws.wx",
 "CorrelationId":"1a7c-6c72","UserId":"u-2201","message":"Command started · Approval.Approve REQ-4471"}
{"…":"…","CorrelationId":"1a7c-6c72","UserId":"u-2201","category":"AccessOps.Services.ApprovalService","message":"Request 4471 moved Pending → Approved"}
{"…":"…","CorrelationId":"1a7c-6c72","UserId":"u-2201","category":"AccessOps.Services.ApprovalService","message":"Audit entry 2 written for Approval.Approve"}
{"…":"…","CorrelationId":"1a7c-6c72","UserId":"u-2201","category":"AccessOps.Commands","message":"Command completed · Approval.Approve REQ-4471 · 58 ms"}
```

## The refusal in between (correlation `7581-eb92`)

```json
{"ts":"20:56:28.768","level":"Warning","category":"AccessOps.Services.Authorize","RequestPath":"/ws.wx",
 "CorrelationId":"7581-eb92","UserId":"u-2201",
 "message":"DENIED ApproveRequest for employee 2201 ((null), Finance) on REQ-4104: request is outside the approver's reports (or does not exist)"}
```

## The download (correlation `d32f-9c01`)

```json
{"ts":"2026-11-06T21:01:36.300Z","level":"Information","category":"AccessOps.Commands","RequestPath":"/ws.wx",
 "CorrelationId":"d32f-9c01","UserId":"u-3001","message":"Command started · Document.Download 4471"}
{"…":"…","CorrelationId":"d32f-9c01","UserId":"u-3001","category":"AccessOps.Services.DocumentService","message":"Download 4471 streamed"}
{"…":"…","CorrelationId":"d32f-9c01","UserId":"u-3001","category":"AccessOps.Commands","message":"Command completed · Document.Download 4471 · 18 ms"}
```

## Field by field

| Field | Verdict | Reason |
|---|---|---|
| `ts` | necessary - **changed** | The first run wrote `20:56:12.617` (time only); a line copied to an index loses its day. Now ISO 8601 UTC, `2026-11-06T21:01:36.300Z`. |
| `level`, `category` | necessary | Filtering and ownership. |
| `CorrelationId` | necessary | The key that joins every line and the audit row of one command; quoted to the user as the reference. |
| `UserId` | necessary | `u-<id>`, a stable reference - not the name or e-mail. |
| `TraceId`, `SpanId`, `ParentId` | necessary (HTTP) | Join an HTTP request (`/signin-oidc`, `/share/{id}`) to proxy and platform logs. Over the Wisej.NET WebSocket (`/ws.wx`) they are per connection, so the correlation ID - not the trace ID - is what separates two clicks. |
| `ConnectionId`, `RequestId`, `RequestPath` | necessary | Which channel the command came through. |
| message: request / document ids | necessary | References (`REQ-4471`, `4471`), not titles: a file name can itself be sensitive. |
| message: `DENIED … ((null), Finance)` | **removed** | The guard printed the user *name*; for a work-account user that is null, for others it is personal data the `UserId` already identifies. The line is now `DENIED ApproveRequest for employee 2201 (Finance) on REQ-4104: …`. |
| `{OriginalFormat}` from `HttpClient` scopes | **removed** | A template string, no information. |
| Share-link `sig`, OIDC tokens, cookies, `Authorization` | **redacted** at the sink | Never seen in this run's lines (the code logs none of them); the policy is there for the line someone adds next year. |
| Client address in `Share link opened … from ::1` | necessary | Security signal for an anonymous endpoint; the logs' retention (30 days) is shorter than the audit trail's. |
| `ID token validated … for subject 3e77-41a0-jamie-mercer` | necessary | The provider's pseudonymous subject - the key that links the external account. Not the e-mail claim. |
| `Document query: SELECT … [department=Finance]` | necessary | The parameterised SQL and its parameter values (a department, never free text from a user). |

## Lines outside a command

Start-up lines and the dashboard's first data load run before any user command exists, so they
carry ASP.NET Core's `TraceId`/`RequestId` but no `CorrelationId`. The sign-in itself now opens a
scope (`WisejExternalSignIn.TryCompleteOnPageLoad`), so its log line and its `Session.SignIn` audit
row share one correlation ID.
