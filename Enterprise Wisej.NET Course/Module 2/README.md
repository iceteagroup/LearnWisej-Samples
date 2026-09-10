# Module 2 — Wisej.NET 4 Project Modernization & Migration Strategy

Runnable lab sample for **Advanced Module 2** of *Enterprise Wisej.NET: Architecture to Cloud*
(site slug `azure-docker`). Video: **"Plan a Wisej.NET 4 migration dossier"**.

The lab: build a **migration dossier** for the Intermediate *TicketOps Console* and extend it into the Advanced
**EnterpriseOps** baseline — current-state inventory, target-state inventory, compatibility/risk matrix, a screen
regression plan for ten key flows, and a rollback strategy. Then prove it by running the path, hitting the failure
the video shows, rolling back and re-running.

## Run

```bash
cd "Module 2/EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5202
```

Then open <http://localhost:5202>. In-memory only: no database, no network, no cloud account, nothing deployed.
The Windows target (`net10.0-windows`) is what the Wisej.NET Designer opens; both frameworks build.

## The two screens

| Screen | What it is |
|---|---|
| `UI/MigrationDossierPage` (MainPage) | the dossier: **Build dossier**, **Run migration path**, **Cancel**, six tabs (Dossier · Inventory · Compatibility / risk · Incremental path · Regression plan (10 flows) · Decision memo), the banner, the dark footer with the harness verdict, and the live activity trace |
| `UI/WorkOrdersPage` | the migrated TicketOps screen from the video — `Open` / `In progress` / `Done`, **+ New work order**, `dgvWorkOrders`, the dark footer. It **paints itself from `ThemeService.Current`**, so the visual diff is on the screen rather than in a document |

## What to click

Do it in this order the first time — it is the walkthrough.

| # | Button (screen) | Path | What you should see |
|---|---|---|---|
| 1 | **Build dossier** (Dossier) | success | 7 dossier areas with the risk computed (H×3 M×3 L×1), 14 inventory items, 9 compatibility entries (3 blocked, 3 mitigated). Footer: `Dossier: 7 areas · risk H×3 M×3 L×1 · 3 blocked combinations mitigated` |
| 2 | **Open WorkOrdersPage →** | the broken build, visible | Grey app bar, square tabs, monochrome priority column. Red banner: `Visual diff — 3 failure(s): accent color lost … · corner radius 0 … · priority colors dropped …`. Footer: `… theme NOT mapped: 3 visual difference(s)`. **It compiles, it runs, it is wrong** |
| 3 | `In progress` tab, **+ New work order**, **Fail: new work order, blank title**, select a row → **Approve selected**, **Fail: approve with stale version** | behaviour flows 2–5, by hand | server-side filter · a created work order · a rejected blank title with the store unchanged · an approval · `stale version v1 — the work order is at v2; reload and retry` |
| 4 | **← Back to the migration dossier** | navigation (flow 9) | the trace list is **not empty**: the buffer came across because the `ServiceRegistry` and the `SessionContext` did |
| 5 | **▶ Run migration path** (Dossier) | progress + **the failure path** | steps 1–3 turn `✓`, the flows tab fills in live, then step 4 *Check themes* turns `✕`. Footer: `Regression harness: FAIL — 7/10 flows, 3 theme flows broken (…)`. Steps 5–7 are **never started** |
| 6 | **Map theme mixin** | refused on purpose | `Step 4 is in the Failed state — roll back to "theme folder copy" first. Fix on the fallback point, never on the broken build.` |
| 7 | **↶ Roll back failed step** | recovery 1 | step 4 → `↶ rolled back to "theme folder copy" — fix, then re-run`; steps 1–3 keep `✓`; the dossier row *Themes / resources* reopens |
| 8 | **Fail: map theme as ben.tech** | permission failure | `Security: MapThemeMixin for ben.tech (Technician) → DENIED (requires Manager or Admin)` — decided on the server, not by a disabled button. Click again to come back as `ana.ops` |
| 9 | **Map theme mixin** | recovery 2 | `Theme mixin mapped — 3/3 tokens match the 3.5 baseline.` The trace shows the resource mapping `Themes/Blue-2019/*.json (3.x) → Themes/Blue-2019.mixin.json` |
| 10 | **▶ Run migration path** again | success | resumes at step 4, `Regression harness: PASS — 10/10 flows`, then 5, 6, 7 → `Migration path complete — 7/7 steps passed, 7/7 dossier rows proven` |
| 11 | **Open WorkOrdersPage →** | the fix, visible | blue app bar `#1565D8`, rounded tabs, High red / Normal amber / Low green. Footer: `N open work orders — Wisej.NET 4, Blue-2019 mixin (Wisej.NET 4)`. `Critical` rows stay grey **on purpose**: the 3.5 theme only defined three priority colours and `Critical` is new in the Advanced baseline — inventory item #9 |
| 12 | **Write decision memo** | deliverable 5 | the *Decision memo* tab, generated from the evidence this run produced — steps, harness verdict, theme diff |
| 13 | **Cancel** (during a run) | cancellation | the run stops at the next check; passed steps keep their state |
| 14 | **Reset** | replay | 7 steps pending, flows not run, dossier rows open, theme unmapped — the state right after the package upgrade |
| 15 | **Clear trace** | — | empties the trace buffer and the list |

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Open the project, run it locally without missing references | `EnterpriseOps.slnx`, `EnterpriseOps.csproj` (`net10.0-windows;net10.0`, Wisej-4 4.1.0), `Properties/launchSettings.json` (port 5202) |
| Lab goal — a dossier for TicketOps extended into the Advanced baseline | `UI/MigrationDossierPage.cs` + `Services/MigrationAssessmentService.cs` |
| **Deliverable: migration inventory table** | `docs/MigrationInventory.md` · `Data/MigrationInventoryStore.Inventory()` · `Domain/InventoryItem.cs` · *Inventory* tab |
| **Deliverable: compatibility / risk matrix** | `docs/RiskMatrix.md` · `Data/MigrationInventoryStore.CompatibilityMatrix()` · `Domain/CompatibilityEntry.cs` · *Compatibility / risk* tab |
| **Deliverable: regression test plan for ten key flows** | `docs/RegressionPlan.md` · `Data/MigrationInventoryStore.Flows()` · `Services/RegressionHarnessService.cs` · *Regression plan* tab |
| **Deliverable: rollback plan** | `docs/RollbackPlan.md` · `Data/MigrationInventoryStore.Steps()` (fallback points) · `Services/MigrationWorkflow.RollbackAsync` · *Incremental path* tab |
| **Deliverable: modernization decision memo** | `docs/ModernizationDecisionMemo.md` · `Services/MigrationAssessmentService.BuildDecisionMemo` · *Decision memo* tab |
| The dossier itself (the file the video opens) | `docs/MigrationDossier.md` · `Data/MigrationInventoryStore.DossierRows()` · `Domain/DossierRow.cs` |
| The incremental path as a diagram | `docs/MigrationPath.svg` |
| Create the service / command / model boundary first | `Services/` (`MigrationAssessmentService`, `MigrationWorkflow`, `RegressionHarnessService`, `ThemeService`, `WorkOrderService`), `Services/CommandContext.cs`, `Services/CommandResult.cs`, `Services/ServiceRegistry.cs` |
| Build the screen using the project standards | `UI/MigrationDossierPage.cs` + `.Designer.cs`, `UI/WorkOrdersPage.cs` + `.Designer.cs` |
| **Add a failure-path demonstration, not only the happy path** | step 4 fails the visual diff (`MigrationWorkflow.ExecuteStepAsync` case 4 → `RegressionHarnessService` flows 6–8); permission denial (`Security/PermissionService`); blank title and stale version on `WorkOrdersPage`; "run before building the dossier" and "map before rolling back" refusals |
| Show every path without leaking internals | `ShowWorkflowResult` / `ShowCommandResult` / `ReportFailure` — typed results become banners and a status colour; an unexpected exception becomes a generic message plus a correlation id, details stay in the trace |
| Record what changed / production-readiness note | `docs/ModernizationDecisionMemo.md` (and the generated memo in the app) |

## The failure path, in one paragraph

The package upgrade succeeds. `dotnet build` reports 0 errors. The session starts, the screens open, the grid
binds. And the 4.x theme engine has quietly ignored `Themes/Blue-2019/*.json`, because that is a 3.x format, so
every screen renders with engine defaults: accent `#6B7C8F` instead of `#1565D8`, corner radius `0` instead of
`7`, and no priority colours at all. Nothing throws. The only thing that notices is the harness, whose flows 6–8
diff the theme token map against the 3.5 baseline — which is why the plan protects **behavior, not compilation**.
The workflow then refuses to start step 5, refuses to let you fix forward, and names the fallback point
(`theme folder copy`) that gets you back.

## Student review questions, answered against this sample

**What exact behavior must remain unchanged?** Seven things, each written as an assertion the harness runs, not as
a sentence: the tab filter is decided on the server (`page.Total == CountInStore`, flow 3); a blank title is
rejected and nothing is written (flow 4); a stale `Version` is refused and the current one accepted (flow 5);
`ana.ops` may approve and `ben.tech` may not, checked server-side (flow 1); accent `#1565D8`, radius `7 px`,
priority colours High red / Normal amber / Low green (flows 6–8); one `SessionContext` per session with **zero**
static fields (flow 9); background job progress reaches the UI (flow 10). Everything else — project layout, target
framework, startup file, package set, hosting model — is explicitly allowed to change.

**Which migration step can be rolled back independently?** All seven; that property is what makes this a
migration rather than a rewrite. Each step names its own fallback point (`git tag pre-fx`, `config backup`,
`package pin 3.5`, `theme folder copy`, `feature flag off`, `previous package` ×2) and the workflow never starts
step *n+1* until step *n* passed, so a rollback of step *n* has nothing downstream to undo. Roll back step 4 in the
app and watch steps 1–3 keep their `✓`. See `docs/RollbackPlan.md`.

**Which screens are most likely to hide timing or state assumptions?** `WorkOrdersPage`, because a tab filter that
was secretly client-side on 3.5 looks identical until the data grows — flow 3 compares the page total with the
store count so it cannot pass quietly. `ImportCenterPage`'s background import, because a `System.Timers.Timer` in
`Global.asax` writes from a thread that no longer owns the session — on Wisej.NET 4 that is
`Application.StartTask` + `Application.Update`, and flow 10 counts the ticks that actually arrive. And any screen
reached by navigation, because user identity kept in a `static` "works" for exactly one user — flow 9 fails a build
with even one static field on `SessionContext`.

## Instructor acceptance criteria, answered against this sample

| Criterion | How this sample meets it |
|---|---|
| Follows the course architecture baseline | folder-per-layer with matching namespaces (`UI`, `Domain`, `Services`, `Data`, `Security`), root namespace `EnterpriseOps`, one web project, ADR-001's layout from Module 1 |
| UI event handlers remain thin and explainable | every handler is a few lines: `try { var result = await _service.…Async(CurrentContext); Show…(result); } catch { … }`. The longest is `btnRunPath_Click`, and its body is one service call plus two progress callbacks. No decision is taken in `UI/` |
| Service-level logic reviewable without opening the designer | the risk rule is `MigrationAssessmentService.AssessRisk` (3 lines); the step semantics are `MigrationWorkflow.ExecuteStepAsync`; the ten assertions are `RegressionHarnessService.ExecuteAsync`; the permissions are `PermissionService.Check`. None of them reference a control |
| At least one failure path is demonstrated | five: the step-4 visual diff (the video's), the Technician permission denial, the blank title, the stale version, and the two ordering refusals (run before building the dossier; map before rolling back) |
| The student can explain state ownership, security implications and production behaviour | **State**: one `SessionContext` and one `ServiceRegistry` per session, created in `Program.Main`, stored in `Application.Session`, handed to both pages — instance fields only, and flow 9 asserts `SessionContext` has 0 static fields. **Security**: `PermissionService` decides on the server; a denial is a typed result with a reason, not a hidden button. **Production**: every layer's decision is in the trace with a correlation id per command; unexpected exceptions become a generic message plus that id |

## Trace

Every action, service decision, failure and recovery is one line in the right-hand card, tagged with the layer:

```
UI →      MigrationDossierPage_Load → session contoso/ana.ops (Manager), trace buffer 1 line(s) replayed
Service:  MigrationAssessmentService.AssessAsync for contoso/ana.ops corr=8b41c07d — inventory before action
Data:     MigrationInventoryStore → 7 dossier areas, 14 inventory items, 9 compatibility entries
Service:  Themes / resources: custom Blue-2019 → mapped theme mixin — breaking=yes user-visible=yes → risk H · proof "visual diff per screen" · fallback "theme folder copy"
Job:      step 4/7 Check themes — running · check: full harness, 10 flows incl. visual diff · fallback point: "theme folder copy"
Service:  flow  6 Theme: accent color on header + tabs → FAIL — accent color lost (#6B7C8F ≠ baseline #1565D8)
Job:      step 4/7 Check themes → FAILED — path halted at fallback point "theme folder copy" (steps 5–7 not started)
Security: MapThemeMixin for ben.tech (Technician) → DENIED (requires Manager or Admin)
Data:     ThemeStore → Themes/Blue-2019 restored from the pre-step copy; migrated (unmapped) theme discarded
```

The buffer lives in the per-session `ActivityTrace`, not in the page, which is why it survives
`MigrationDossierPage ⇄ WorkOrdersPage` — the navigation flow 9 walks.

## File tree

```
Module 2/
  README.md                                  ← this file
  EnterpriseOps.slnx  .gitignore
  EnterpriseOps/
    Program.cs                               session entry point: one SessionContext + one ServiceRegistry
    Startup.cs  Default.html  Default.json  Web.config
    Properties/launchSettings.json           port 5202
    UI/
      MigrationDossierPage.cs / .Designer.cs the dossier screen (MainPage)
      WorkOrdersPage.cs / .Designer.cs       the migrated TicketOps screen, painted from the theme map
    Domain/
      DossierRow.cs                          + RiskLevel, DossierRowState
      InventoryItem.cs                       + InventoryCategory (10 categories)
      CompatibilityEntry.cs                  one cell of the compatibility matrix
      MigrationStep.cs                       + StepState — a step, its check, its fallback point
      RegressionFlow.cs                      + FlowCategory, FlowOutcome
      ThemeMap.cs                            the three tokens + the visual diff
      WorkOrder.cs                           + WorkOrderStatus, Priority, Tenant
    Services/
      MigrationAssessmentService.cs          risk rule · matrix check · decision memo
      MigrationWorkflow.cs                   the seven steps, the refusals, the rollback
      RegressionHarnessService.cs            the ten flows
      ThemeService.cs                        unmapped → folder copy → mapped mixin
      WorkOrderService.cs                    the behaviour that must not change
      ActivityTrace.cs  CommandContext.cs  CommandResult.cs  ServiceRegistry.cs
    Data/
      MigrationInventoryStore.cs             dossier rows · inventory · matrix · steps · flows
      FakeWorkOrderStore.cs                  45 in-memory work orders, ids 2002–2005 as in the video
      ThemeStore.cs                          the three theme states
    Security/
      SessionContext.cs                      per session, zero static fields (flow 9 asserts it)
      PermissionService.cs                   server-side rules
    docs/
      MigrationDossier.md                    the dossier table (the file the video opens)
      MigrationInventory.md                  deliverable 1
      RiskMatrix.md                          deliverable 2
      RegressionPlan.md                      deliverable 3
      RollbackPlan.md                        deliverable 4
      ModernizationDecisionMemo.md           deliverable 5
      MigrationPath.svg                      the seven steps and their fallback points
```

The video's Solution Explorer shows these under `docs/migration/`; this sample keeps `docs/` flat, as the course
cookbook specifies.

## Verified / unverified

Used from the EnterpriseOps cookbook and **verified** in earlier course samples on the same framework build:
`Page` as the screen type; the 1348×680 layout with white cards and the `lstTrace` monospace trace card;
`DataGridView` with `AutoGenerateColumns = false`, explicit `DataGridViewTextBoxColumn`s, `AutoSizeColumnsMode.Fill`,
`FillWeight`, `SelectionMode = FullRowSelect`, `DataSource = new BindingSource { DataSource = list }`;
`async void` handlers with `await Task.Delay(…)` and `Application.Update(this)` after the awaits, with a
`CancellationTokenSource` in an instance field; `AlertBox.Show(text, icon, alignment: ContentAlignment.TopRight,
autoCloseDelay: 4000)`; `Application.Session` as the per-session bag; fonts `"default"` / `"monospace"`;
`Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`.

Relied on and **not yet exercised at runtime** — please check these when you run the sample:

| Item | Where | Why it should work |
|---|---|---|
| `Wisej.Web.TabControl` + `TabPage` with a `Dock = Fill` grid per page, `SelectedIndex` set from code, `TabPages.IndexOf(page)` | `MigrationDossierPage.Designer.cs`, `btnMemo_Click` | the cookbook names `TabControl` as the wizard/step container; both members exist in `Wisej.Framework.dll` |
| `DataGridViewCell.Style` read-modify-write to colour **one cell** (`cell.Style.ForeColor`) rather than the whole row | `MigrationDossierPage.PaintCell`, `WorkOrdersPage.PaintPriorityColumn` | Module 1 verified `DataGridViewRow.DefaultCellStyle`; per-cell `Style` is the same type. The code tolerates a null `Style` |
| `DataGridView.CurrentRow` + `Cells[DataGridViewColumn]` as the selection read | `WorkOrdersPage.SelectedWorkOrderId` | both members exist in `Wisej.Framework.dll`; the code handles `null` (no selection) with a banner |
| `Control.CssStyle = "border-radius: 7px !important;"` to make the theme's corner radius visible on the tab buttons | `WorkOrdersPage.ApplyTheme` | `Wisej.Web.Control.CssStyle` exists. If a theme's own button styling wins, the radius part of the visual diff will be less obvious — the accent colour and the priority colours still show it, and flows 6–8 compare the token map regardless |
| `Wisej.Web.TextBox` with `Multiline = true` + `ScrollBars = ScrollBars.Both` for the memo | `MigrationDossierPage.Designer.cs` | `Multiline` is on `TextBoxBase`; `TextBox.ScrollBars` and `Wisej.Web.ScrollBars.Both` both exist |
| Navigation by assigning `Application.MainPage = new <OtherPage>(services)` mid-session, with `ActivityTrace.Detach()` before and `Attach()` in the new page's `Load` | `btnWorkOrders_Click`, `btnBackToDossier_Click` | the standard Wisej page-switch; the detach/attach pair is what keeps the trace buffer from being written to a disposed `ListBox` |

Nothing in this sample needs a network, a database, a cloud account or a licence key.

## Build

```
$ cd "Module 2/EnterpriseOps" && dotnet build -nologo -v q

Build succeeded.
    0 Warning(s)
    0 Error(s)
```

Both target frameworks (`net10.0-windows` and `net10.0`) build. `CS7022` (the `Program.Main` entry point is
ignored in favour of top-level statements in `Startup.cs`) is expected and already in `NoWarn`.
