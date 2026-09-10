# Migration dossier — TicketOps Console → EnterpriseOps baseline

Deliverable: the dossier the walkthrough opens in Visual Studio. One row per area that changes, with **what it
risks**, **how it is proven** and **how to get back**. A row is not closed by a green build — it closes when its
named regression proof runs and passes.

Source of truth in the sample: `Data/MigrationInventoryStore.DossierRows()`.
Risk is **never typed by hand** — `Services/MigrationAssessmentService.AssessRisk` computes it.

## The rows

| Area | Current | Target | Risk | Regression proof | Rollback (fallback point) | Proved by step |
|---|---|---|---|---|---|---|
| Target framework | .NET Framework 4.8 | .NET 10 (`net10.0-windows;net10.0`) | M | full build + smoke run | `git tag pre-fx` | 1 Compile |
| Wisej.NET version | 3.5 | 4.1 | **H** | 10 key screen flows | `package pin 3.5` | 3 Compare behavior |
| Startup / configuration | `Global.asax` | `Program.cs` + `Startup.cs` | M | session + auth flows | `config backup` | 2 Run |
| Themes / resources | custom Blue-2019 | mapped theme mixin | **H** | visual diff per screen | `theme folder copy` | 4 Check themes |
| Custom widgets | 2 JS interop widgets | re-tested on 4 | M | widget event log | `feature flag off` | 5 Verify sessions |
| Authentication | forms auth | same behavior | **H** | login + role checks | `standalone gate` | 5 Verify sessions |
| Deployment target | IIS | IIS + health checks | L | staging deploy | `previous package` | 6 Review deployment |

## The risk rule (one place, so a reviewer can argue with it)

```csharp
// Services/MigrationAssessmentService.cs
public RiskLevel AssessRisk(DossierRow row)
{
    if (row.BreakingChange && row.UserVisible) return RiskLevel.High;   // behaviour changes AND a user can see it
    if (row.BreakingChange) return RiskLevel.Medium;                    // breaking, but invisible
    return RiskLevel.Low;
}
```

`BreakingChange` and `UserVisible` are recorded per row in the inventory store; `Risk` is derived. That is why the
matrix cannot drift: change the facts, the risk follows.

## Row states

| State | Meaning |
|---|---|
| `open` | planned; no proof yet |
| `✓ proven` | the named regression proof ran and passed — the row closes |
| `✕ failed` | the proof ran and failed — the fallback point is the next move |
| `↶ rolled back` | restored to the fallback point; waiting for the fix and a re-run |

The workflow moves the rows: `MigrationAssessmentService.MarkRows(step, state)` is called by
`MigrationWorkflow.RunAsync` / `RollbackAsync` — the screen never sets a state itself.

## Not complete until

The dossier is not complete until the team demonstrates **behavior, theme, session, background-task, security and
deployment** regression flows. See `RegressionPlan.md` — ten flows, five categories.

## Evidence — what the running app shows

1. **Build dossier** → the *Dossier* tab fills with the seven rows above, risk column coloured H red / M amber /
   L green, every row `open`. The trace prints one `Service:` line per row with the two inputs and the computed
   risk, e.g.
   `Service:  Themes / resources: custom Blue-2019 → mapped theme mixin — breaking=yes user-visible=yes → risk H · proof "visual diff per screen" · fallback "theme folder copy"`.
2. **Run migration path** → rows for steps 1, 2 and 3 turn `✓ proven`; the *Themes / resources* row turns
   `✕ failed` when step 4's visual diff fails.
3. **Roll back failed step** → that row turns `↶ rolled back`; the rows already proven are untouched.
4. **Map theme mixin** then **Run migration path** again → all seven rows read `✓ proven`.
