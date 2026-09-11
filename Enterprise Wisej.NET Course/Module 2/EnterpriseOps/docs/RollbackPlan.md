# Rollback plan

Deliverable 4 of the lab. Answer to the review question **"which migration step can be rolled back
independently?"** — if the answer is *none*, it is a rewrite, not a migration.

Source of truth in the sample: `Data/MigrationInventoryStore.Steps()` (the fallback points) and
`Services/MigrationWorkflow.RollbackAsync` (what restoring one actually does).

## Seven steps, seven fallback points

| Step | Check that proves it | Fallback point | Restoring it means | Independent? |
|---|---|---|---|---|
| 1 Compile | `dotnet build` both targets, 0 errors | `git tag pre-fx` | check out the tag; the 4.8 solution builds again | yes |
| 2 Run | session starts, MainPage loads | `config backup` | restore `Global.asax` + `Web.config`; step 1 stays done | yes |
| 3 Compare behavior | flows 1–5 (security + behavior) | `package pin 3.5` | pin Wisej-3 3.5 in the project file; the framework move stays done | yes |
| 4 Check themes | full harness, 10 flows incl. the visual diff | `theme folder copy` | restore the pre-step copy of `Themes/Blue-2019`; steps 1–3 stay done | yes |
| 5 Verify sessions | flows 9–10 (session + background task) | `feature flag off` | switch the two migrated JS widgets off; the screens keep working without them | yes |
| 6 Review deployment | staging deploy answers `/healthz` | `previous package` | redeploy the previous package to staging; nothing in the code changes | yes |
| 7 Measure performance | work queue query under the 3.5 budget | `previous package` | as above; the budget is a gate, not a code change | yes |

Seven of seven. That is the property that makes this a migration.

## The two rules the workflow enforces in code

**1. Never run past a failed step.**

```csharp
var failed = FailedStep;
if (failed != null)
    return Fail(ctx, $"Step {failed.Number} ({failed.Name}) failed — roll back to its fallback point "
                   + $"\"{failed.FallbackPoint}\" before re-running. Never fix forward on a broken build.", failed);
```

Steps `n+1 … 7` are never started. The server log says so explicitly:
`Job:  step 4/7 Check themes → FAILED — path halted at fallback point "theme folder copy" (steps 5–7 not started)`.

**2. Never fix forward on a broken build.**

`MigrationWorkflow.MapThemeMixin` refuses while step 4 is in the `Failed` state:

```csharp
if (failed != null && failed.Number == 4)
    return CommandResult.Fail(ctx, "Step 4 is in the Failed state — roll back to \"theme folder copy\" first. "
                                 + "Fix on the fallback point, never on the broken build.");
```

The order is therefore forced: **fail → roll back → fix → re-run**, which is exactly the sequence the walkthrough
shows. Clicking *Map theme mixin* straight after the failure produces a visible, explained refusal instead of a
hidden half-migrated state.

## Blast radius of one rollback

Rolling back step *n* touches step *n* only:

- `step.State` goes `Failed` → `RolledBack`;
- the dossier rows whose `ProvedByStep == n` go back to `↶ rolled back`; rows proven by earlier steps keep
  `✓ proven`;
- the artefact of that step is restored (for step 4: `ThemeService.RestoreFolderCopy()` puts the 3.x
  `Themes/Blue-2019` folder copy back and discards the migrated, unmapped theme);
- steps `n+1 … 7` were never started, so there is nothing to undo.

## Who may roll back

Rolling back and mapping the theme change the build **for everyone**, so both need a Manager or an Admin
(`Security/PermissionService`, `Permission.RollbackMigrationStep` / `Permission.MapThemeMixin`). A Technician gets
a typed, explained denial — the check is on the server, never in the button's `Enabled` state:

```
Security: MapThemeMixin for ben.tech (Technician) → DENIED (requires Manager or Admin)
```

## Rollback of the whole migration

Abandoning the attempt means going back to the state right after the package upgrade: seven steps pending, flows
not run, dossier rows open, theme unmapped. In a real project that is `git checkout pre-fx` plus a redeploy of the
previous package; nothing downstream of step 1 has shipped.

## Evidence — what the running app shows

1. **Run migration path** → steps 1–3 `✓`, step 4 `✕`, steps 5–7 still `·` (pending, never started).
2. **Map theme mixin** now → amber banner: *Step 4 is in the Failed state — roll back to "theme folder copy" first.*
3. **Roll back failed step** → step 4 becomes `↶ rolled back to "theme folder copy" — fix, then re-run`; the server
   log records `Data: ThemeStore → Themes/Blue-2019 restored from the pre-step copy; migrated (unmapped) theme discarded`.
4. **Map theme mixin** → `Theme mixin mapped — 3/3 tokens match the 3.5 baseline.`
5. **Run migration path** again → it resumes at step 4 and finishes 7/7.
