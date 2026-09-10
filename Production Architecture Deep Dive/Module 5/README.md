# TicketOps · Production Architecture Deep Dive · Module 5

Local lab build for **Module 5 · Validation, Error UX & Safe Save Pipelines**. It follows the walkthrough
video *Add validation & a safe save pipeline*: the Work Order editor gets layered validation rules as plain C#
(`WorkOrderValidator`, `WorkOrderRules`, an explicit status-transition table), a reusable `SaveWorkOrderCommand`
that carries the intent through the pipeline, field-level errors painted by an `ErrorProvider` plus a summary
panel that lists every problem, and a safe save pipeline in `WorkOrderService.SaveAsync` —
**validate → rules & authorize → persist atomically → confirm** — where nothing is written unless every step
passes and an unexpected failure reaches the user as one safe sentence. The documented test cases run inside the app.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 5\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5105
```

Then open <http://localhost:5105>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with no warnings for both targets (`net10.0-windows`, `net10.0`).

## What to click in the Work Orders window

The left card is the editor (grid, fields, **Acting as** role, the error summary panel); the right card is the
**Activity trace · UI → Service → Domain → Data**. Every save is logged as it crosses a boundary
(`[UI]` → `[SVC]` → `[DOMAIN]` → `[DATA]` → `[UI]`, plus `[SESSION]` when the role changes and `[CLIENT]` for the
crafted request), so you can see which step said no and that no `[DATA] tx#…` line exists for a rejected save.
The grid's **Ver** column is the `RowVersion`: it only moves when a transaction commits.

| Button | Path | What you should see |
|---|---|---|
| **Save** (#2002 is selected on load; change the title) | success | `[UI] buttonSave_Click — command built {…}`, `[UI] pre-check passed → IWorkOrderService.SaveAsync`, `[SVC] step 1 validate …`, `[DATA] FindAsync #2002 found — Assigned · v1`, `[SVC] step 2 rules …`, `[DOMAIN] WorkOrderRules.Check — all rules pass`, `[SVC] step 3 persist`, `[DATA] tx#1 open`, `tx#1 stage UPDATE WorkOrders #2002`, `tx#1 stage INSERT AuditLog`, `tx#1 committed — 1 row(s), 1 audit entry · #2002 now v2`, `[SVC] step 4 confirm`, `[UI] OK · Work order #2002 saved.`; status **● Work order #2002 saved.**; grid shows `v2` |
| **Save** with a bad value typed in (e.g. clear the title, set cost 25000) | UX pre-check | `⚠ [UI] pre-check: 2 errors (Title, EstimatedCost) — nothing sent, nothing saved` — no `[SVC]` line at all; red glyph beside each field (hover for the message); summary panel "2 problems need attention"; status **● 2 problems found — nothing was saved** |
| **Acting as** → Supervisor / Technician | — | `[SESSION] SessionContext — role → Supervisor (server-side; the command never carries it)` |
| **New / ↻ Refresh** | — | clears the editor / reloads through `GetWorkOrdersAsync` |
| **▶ Run 15 test cases** | progress | one `[DOMAIN] WorkOrderValidator.Validate — TC-nn "…" · expected … · got … → PASS` (or `WorkOrderRules.Check`) per tick; progress bar and **● running TC-07 · 7/7 passed** advance; ends **● 15/15 test cases passed** |
| **Save with empty title** | failure 1 (validation) | `[UI] pre-check skipped (like an import …) → SaveAsync`, `⚠ [SVC] rejected at validate: 1 error (Title) — nothing persisted`; glyph on Title; panel "• Title — Title is required." |
| **Save 1,200 hours** | failure 2 (range) | `⚠ [SVC] rejected at validate: 1 error (EstimatedHours)`; glyph on Estimated hours; "Hours must be between 0 and 999." |
| **Edit closed #2006** | failure 3 (business rule) | `[SVC] step 1 validate … ` passes, `[DATA] FindAsync #2006 found — Closed · v3`, `⚠ [DOMAIN] WorkOrderRules.Check — #2006 rejected: Closed work orders cannot be edited. Ask a Supervisor to reopen it.`, `⚠ [SVC] rejected at rules`; no glyph (every field is valid), the panel carries the summary error |
| **Bypass: crafted command** | failure 4 (role — the server is the gate) | `⚠ [CLIENT] crafted request — a disabled button is not authorization …`, validator passes (cost 9,500 is in range), `⚠ [DOMAIN] … rejected: Only a Supervisor may set a cost above $2,500.`; switch **Acting as** to Supervisor and click again: it saves (`#2002 now v…`) |
| **Simulate write outage** | error path | the editor is saved as-is: validation and rules pass, `[DATA] tx#n stage …`, then `✖ [DATA] outage: COMMIT tx#n … failed — timeout connecting to sql01:1433 …`, `⚠ [DATA] tx#n rolled back — 0 rows changed`, `✖ [SVC] persist failed after validation passed — … nothing written`, `✖ [UI] caught DataOutageException — user sees the safe message, edits stay on screen`, `[UI] re-read #2002: v2 … — unchanged, nothing partial`; the user sees only the red banner + toast **The work order could not be saved. Your changes are still here …**, status **● Save failed — your changes are still here**; the editor still holds the edits; **Ver** did not move |
| **Recover the data store** (same button) | recovery | `[UI] outage OFF → retrying the same save (recovery)` → the full success path, `tx#n+1 committed`, `#2002 now v3`, status **● Work order #2002 saved.** |
| **Clear trace** | — | empties the right-hand card |

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Validation rules / model validation | `Validation/WorkOrderValidator.cs` (pure: required, format, range, cross-field, transition), `Domain/WorkOrderRules.cs` (closed freeze, stored-status transition, role threshold), `Domain/WorkOrderTransitions.cs` (the table) — see [`docs/ValidationRules.md`](TicketOps/docs/ValidationRules.md) |
| 2 | Save command object | `Validation/SaveWorkOrderCommand.cs` — immutable, built once by `WorkOrderEditor.BuildSaveCommandFromEditor`, carries no role |
| 3 | Error summary panel (+ field-level errors) | `Views/WorkOrderEditor.cs` `ShowValidation` — `ErrorProvider.SetError` per field + `panelSummary` listing every error; `Validation/ValidationResult.cs` (`ValidationError`, field vs summary channels) — see [`docs/ErrorUxGuidelines.md`](TicketOps/docs/ErrorUxGuidelines.md) |
| 4 | Safe save pipeline method | `Services/WorkOrderService.SaveAsync` over `Data/IWorkOrderRepository.cs` + `IWorkOrderTransaction` (`Data/InMemoryWorkOrderRepository.cs`) — see [`docs/SavePipeline.md`](TicketOps/docs/SavePipeline.md) and [`docs/SavePipeline.svg`](TicketOps/docs/SavePipeline.svg) |
| 5 | Validation test cases | [`docs/TestCases.md`](TicketOps/docs/TestCases.md) + `Validation/ValidationTestCases.cs` (`ValidationTestRunner`), executed by **▶ Run 15 test cases** |
| 6 | Every path visible without leaking internals | `ShowSaveResult` / `ReportFailure` / `VerifyNothingPartialAsync`, `Resources/Strings.cs`, the trace panel |
| 7 | Production-readiness note | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |

## Where things live

```
TicketOps/
├─ Views/
│  ├─ WorkOrderEditor.cs            the screen: buttonSave_Click (collect → pre-check → service → show), ShowValidation, ReportFailure
│  └─ WorkOrderEditor.Designer.cs   GENERATED-style layout incl. the ErrorProvider and the summary panel — no logic here
├─ Controls/StatusBanner            reusable "● state" + banner UserControl (display only)
├─ Validation/
│  ├─ SaveWorkOrderCommand.cs       the command: what the editor collected, immutable, no role
│  ├─ ValidationResult.cs           ValidationError + the two channels (field / summary); errors are collected, not thrown
│  ├─ WorkOrderValidator.cs         the pure validator (command in, result out) — runs in the editor, the service and the tests
│  └─ ValidationTestCases.cs        the documented cases + ValidationTestRunner (no Wisej.NET type)
├─ Services/
│  ├─ IWorkOrderService.cs          the contract the screen calls
│  └─ WorkOrderService.cs           THE PIPELINE: validate → rules & authorize → persist (one tx) → confirm
├─ Domain/
│  ├─ WorkOrder.cs                  record + RowVersion (moves only on commit)
│  ├─ WorkOrderTransitions.cs       the status machine as a table (Legal / Privileged / Illegal)
│  ├─ WorkOrderRules.cs             rules that need the stored record or the role; UserRole
│  └─ SaveResult.cs                 Saved / Invalid + the collected errors
├─ Data/
│  ├─ IWorkOrderRepository.cs       persistence + IWorkOrderTransaction (Upsert, Audit, CommitAsync, Dispose = rollback)
│  └─ InMemoryWorkOrderRepository.cs fake store, seeded; SimulateWriteOutage fails the COMMIT like a read-only primary
├─ Infrastructure/
│  ├─ ILog.cs / ActivityLog.cs      cross-cutting logging (details stay here)
│  ├─ SessionContext.cs             who is acting (role) — server-side, per session
│  └─ AppComposition.cs             who gets what: one object graph per session, constructor injection, no statics
├─ Resources/Strings.cs             safe user-facing messages (SaveFailed …)
├─ Diagnostics/ActivityTracePanel   the live trace card
├─ docs/                            ValidationRules, SavePipeline (+ .svg), ErrorUxGuidelines, TestCases, ProductionReadinessNote
├─ Program.cs                       Wisej.NET session entry point → AppComposition
└─ Startup.cs                       Kestrel host (app.UseWisej())
```

## Self-check answers (lesson guide)

- **Can a malicious user bypass the disabled Save button?**
  Yes — and it does not matter. **Bypass: crafted command** builds a `SaveWorkOrderCommand` with no editor at all and hands
  it to `IWorkOrderService.SaveAsync`; the server re-runs `WorkOrderValidator` and `WorkOrderRules` and rejects the
  Technician's $9,500. The editor's pre-check, the spin-box `Maximum` and the button state are hints; step 1 and 2 of
  `SaveAsync` are the guard.
- **Where is the status-transition rule enforced — client, server, or both?**
  Both, with different inputs. The editor's pre-check runs `WorkOrderTransitions.Classify(FromStatus, ToStatus)` with the
  status it remembered; the service runs it again with the **stored** status it just read (`FindAsync`). TC-15 shows the
  difference: the editor thinks Assigned → InProgress is fine, the store says the order is already Completed.
- **Can the same validation run during an import?**
  Yes. `WorkOrderValidator.Validate` and `WorkOrderRules.Check` take a command, a record and a role — no control, no
  repository. **▶ Run 15 test cases** is exactly that: fifteen commands validated with no form involved, and the bottom-bar
  buttons call `SaveAsync` without the pre-check "the way an import would".
- **Does a failure halfway through the save leave the data untouched?**
  Yes. The row and its audit entry are staged in one `IWorkOrderTransaction`; the write outage fails the `CommitAsync`,
  `Dispose` rolls back (`tx#n rolled back — 0 rows changed`), the exception reaches the handler, and
  `VerifyNothingPartialAsync` re-reads #2002 and logs `v… — unchanged, nothing partial`. **Ver** in the grid never moved.
