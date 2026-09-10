# Deliverable 1 — Diagnostics page

**Code:** `UI/DiagnosticsPage.cs` · `UI/DiagnosticsPage.Designer.cs` · `UI/PerfBudgetPanel.cs` ·
`Diagnostics/DiagnosticsService.cs` · `Diagnostics/DiagnosticsPerformancePatterns.cs` (`DiagnosticSnapshot`) ·
`Security/DiagnosticsAccessPolicy.cs` · `Data/DeploymentConfig.cs`

## What the screen answers

A support engineer opening this page gets, in one glance, the five answers the runbook demands:

| Question | Where on the screen | Source |
|---|---|---|
| What version is running? | **VERSION** card — `2.4.1+sha.9e21b0` | `DiagnosticsService.Version` reads `AssemblyInformationalVersion` from `Properties/AssemblyInfo.cs` — no hand-typed string |
| Which environment / node? | **ENVIRONMENT** and **NODE** cards | `DeploymentConfig.Environment` (`ASPNETCORE_ENVIRONMENT`), `DeploymentConfig.NodeName` (`ENTERPRISEOPS_NODE`, default `app-node-B`) |
| Which tenant / user? | header bar — `tenant fabrikam`, the user combo | `SessionContext.Tenant` / `SessionContext.User`, per session |
| Which services are healthy? | **Session & health** card — the `HEALTHY` / `DEGRADED` / `UNHEALTHY` chip, its tooltip lists every check | `Diagnostics/HealthCheck.cs` |
| What errors occurred, and which id ties them to the report? | **Structured log** card + `correlation …` in the header | `Diagnostics/StructuredLog.cs`, `CommandContext.CorrelationId` |

Plus what the lesson adds beyond a bare health page: the active **theme**, the **feature flags**, the live
**session count / managed heap / working set / uptime**, and the **performance budget table**.

## Safe by type, not by discipline

`DiagnosticSnapshot` is a `record` with exactly six fields:

```csharp
public sealed record DiagnosticSnapshot(
    string Version,
    string Environment,
    string NodeName,
    string ActiveTheme,
    IReadOnlyDictionary<string, string> FeatureFlags,
    IReadOnlyList<string> SafeRecentEvents);
```

There is **no field a connection string, an API key, a stack trace or a row of customer data could travel in**.
That is the point: the page cannot leak them, because the type it binds to has nowhere to put them.

`DeploymentConfig` deliberately *does* hold the dangerous values (`ConnectionString`, `ApiKey`,
`StorageAccountName`), so that `DiagnosticsService.CaptureSnapshot()` has something real to keep out. It
records what it did in `RedactionNotes`, which the page shows as the amber **`secrets redacted (5)`** chip
(hover for the list) and writes in full to the trace:

```
Diagnostics: redacted 5 — ConnectionString — excluded: no snapshot field can carry it ·
StorageAccountName — masked → st•••••prod · ApiKey — excluded ·
exception messages — never in SafeRecentEvents (server log only) · session id — shortened to …a91f3c
```

Three redaction techniques appear, and they are not interchangeable:

- **Excluded by type** — the secret has no field to travel in (connection string, API key).
- **Masked** — the value is recognisable but not usable: `stenterpriseopsprod` → `st•••••prod`
  (`DiagnosticsService.Mask`). Use it when support needs to confirm *which* account, not use it.
- **Shortened** — the session id is shown as its last six characters, enough to match a log line, not
  enough to impersonate a session.

The structured log card applies the same rule at display time: an **error** entry is rendered as its
`SafeSummary` plus `← error detail is server-log only`. The exception type and message exist in the
`LogEntry`, are written to the server-side log, and never reach the browser
(`DiagnosticsPage.ForDisplay`).

**The projector test:** every value on this screen could be shown to a customer on a projector. Nothing
would have to be covered.

## Role-protected, and the attempt is audited

Opening the page is a privileged action, decided on the server:

```csharp
AccessDecision decision = _accessPolicy.CanViewDiagnostics(_session.User);
AuditEntry entry = _auditTrail.Record(_session.User.UserName, "OpenDiagnostics",
                                      decision.Allowed ? "allowed" : "denied", ctx.CorrelationId);
```

`DiagnosticsAccessPolicy.OperatorRoles` is `{ Manager, Admin }`. `ben.tech` (Technician) is denied:
the role chip turns red, the snapshot cards fall back to `—`, and the budgets / log / health cards are
hidden — **the values are never rendered**, rather than rendered and covered. The denial is written to the
audit trail *and* to the structured log with the same correlation id, so "who looked at diagnostics, and
when" is answerable.

## Evidence — what the running app shows

1. **On load** the header reads `EnterpriseOps — Diagnostics · tenant fabrikam · user ana.ops ·
   correlation <8 hex>`; the chips read `Role: Manager ✓` and `secrets redacted (5)`; the four cards fill
   in with the version, environment, node and theme; the status line reads
   `● Diagnostics — live · app-node-B · 2.4.1+sha.9e21b0 · tenant fabrikam` in green.
2. **Hover `secrets redacted (5)`** → the five redaction notes, including the masked storage account.
3. **Switch the user combo to `ben.tech`** → red banner *"ben.tech may not open diagnostics: Technician is
   not an operator role (allowed: Manager, Admin). The attempt is audited (correlation …)"*, the cards go
   to `—`, the lower cards disappear, and the trace shows
   `Security: DiagnosticsAccessPolicy → DENIED …` followed by `Security: audited · … OpenDiagnostics → denied · correlation …`.
4. **Switch back to `ana.ops`** → the snapshot is re-captured (new correlation id) and everything returns.
5. **Click `Health check`** → the chip and its tooltip list all six checks (work-order store, live refresh
   job, deployment config, performance budgets, session memory, structured log) with their detail, and one
   `HealthCheck` entry appears in the structured log.
