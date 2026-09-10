# SupportDesk · Data Binding with EF Core · Module 5

Local lab build for **Module 5 · Validation, ErrorProvider and User Feedback**. It follows the walkthrough
video: the Module 4 editor saved anything and let the database answer in SQL — Module 5 teaches it to say no
politely first. `TicketEditModel` gains `System.ComponentModel.DataAnnotations`; a new `TicketValidator`
service runs them (plus one hand-written cross-field rule — "a Closed ticket cannot have a future due date")
and returns `ValidationMessage(FieldName, Message)` items; `TicketEditorForm.SaveAsync` clears
`errorProvider`, calls `EndEdit`, validates, and — only when the model is valid — creates a `DbContext` at
all. Every message reaches `errorProvider.SetError` on the matching control (`txtTitle`, `cboCustomer`,
`cboCategory`, `dtpDueDate`, plus `txtDescription`/`cboStatus`/`cboPriority`) and `validationSummaryLabel`;
`btnSave.Enabled` tracks the model's validity live, before Save is ever pressed. A duplicate ticket number —
the one failure `TicketValidator` cannot see coming, because only the database can guarantee uniqueness —
still reaches `SaveChangesAsync`, throws `DbUpdateException`, is logged in full server-side, and becomes one
plain sentence on screen. Everything Module 1–4 could do still works, on the second, third, fourth and fifth
button rows.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 solution on this machine with a local SQLite
file.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 5/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5405
```

Then open <http://localhost:5405>. (Visual Studio: open `SupportDesk.slnx`, press F5 — the port and the
Development environment are in `SupportDesk.Web/Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package, EF Core 10.0.12
(`Microsoft.EntityFrameworkCore.Sqlite` + `Design`) and the global `dotnet-ef` 10.0.12 tool. The web
project multi-targets `net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

In Development the host **migrates and seeds** `SupportDesk.Web/App_Data/supportdesk.db` before the first
session, exactly as in Modules 2–4 — the model did not change in Module 5 (`TicketEditModel`'s new
`DataAnnotations` are UI-layer attributes on a Services-project class, not entity mapping), so `InitialCreate`
is still the only migration and the committed script under `artifacts/sql/` is unchanged:

```
[SupportDesk] MigrateAsync: 1 pending migration(s) applied, 1 applied in total (20260910150534_InitialCreate)
[SupportDesk] Development seed: seeded 5 customers, 3 agents, 6 categories, 312 tickets, 99 comments in ... ms
```

> **Coming from Module 4?** Its `App_Data/supportdesk.db` already has data. Module 5 has its own `App_Data`
> folder, so nothing is shared — but if you ever copy one over, delete it or press **Reset & reseed**.

Delete `SupportDesk.Web/App_Data/supportdesk.db` (and its `-wal`/`-shm` companions) to reset the module
completely; **Reset & reseed** on the page does the same thing without a restart.

Tests: `dotnet test SupportDesk.Tests` — **68 tests** against SQLite in memory (the 52 carried over from
Modules 1–4 unchanged, plus 13 new in `TicketValidatorTests.cs` and 3 new in `DuplicateNumberTests.cs`).

## What to click

The **ticket browser card** and the **editor's fields** are unchanged from Module 4 except that the editor now
validates. Bottom bar **row 1** is the Module 5 lab props (five one-click negative-case demos) plus
**Break the database** / **Restore and search** / **Slow save (2.5 s)**, since every dialog this row opens
still needs them (`Clear trace` stays anchored right, on row 1, as it always is on the newest module's row);
**row 2** ("Module 4 · editor") keeps Add ticket / Edit ticket / Simulate delete working; **row 3**
("Module 3 · browser"), **row 4** ("Module 2 · model") and **row 5** ("Module 1 · lifetimes") are unchanged
in content, just renumbered down one row each.

| Action | Path | What you should see |
|---|---|---|
| **Add ticket** (`btnAdd`), fill everything in correctly, **Save** | success · valid save | `btnSave` is disabled until Title, Customer and Category are all set (the neutral `validationSummaryLabel` reads *"Fill in Title, Customer and Category."*); once valid, Save enables live, no icons shown yet. Click Save → `• editor EndEdit → model {...}`, `• validation no messages — model valid`, `• editor Save enabled: model valid`, then the ordinary Module 4 save trace (`◦ context created`, `→ SQL …`, `← result saved SD-1xxx …`), dialog closes OK, grid refreshes |
| **Add: empty title** (`btnLabEmptyTitle`) | validation · single field | Opens with Customer/Category already chosen, Title blank — no icons yet (pre-touch). The dialog opens already validated (a pre-filled scenario counts as touched; Save is disabled, so it cannot be the trigger): `errorProvider` marks `txtTitle`, `validationSummaryLabel` reads *"The Title field is required."*, trace shows `• validation Title: The Title field is required.`, `• editor Save disabled: model invalid`, `• editor validation 1 message(s) → shown, no context created` — **no `◦ context created` line at all** |
| **Add: title 200 chars** (`btnLabTitleTooLong`) | validation · length | Title is pre-filled at 200 characters (over the shared 180-character limit). Opens already validated: `txtTitle` marked, summary reads *"Title cannot be longer than 180 characters."*, same "no context created" shape as above |
| **Add: no customer/category** (`btnLabNoCustomerCategory`) | validation · two fields | Title is filled in; Customer and Category are left unselected. Opens already validated: **two** icons (`cboCustomer`, `cboCategory`), summary reads *"Choose a customer.   ·   Choose a category."*, trace shows `• validation CustomerId: Choose a customer. · CategoryId: Choose a category.` |
| **Edit: closed ticket, due date next week** (`btnLabClosedFutureDue`) | validation · cross-field | Finds an existing `Closed` ticket (`TicketQueryService.SearchTicketsAsync`, filtered to one row) and opens it with `DueDate` pushed to next week in the model — no icon yet. Opens already validated: `dtpDueDate` marked, summary reads *"Closed tickets cannot have a future due date."*, `• validation DueDate: Closed tickets cannot have a future due date.` |
| **Add: duplicate number (DbUpdateException)** (`btnLabDuplicateNumber`) | failure · database | Opens with valid Title/Customer/Category **and `chkDuplicateNumber` already ticked**. `TicketValidator` passes — Save reaches the database. Trace: `◦ context created`, `→ SQL SELECT "t"."Number" ...` (lab prop's reused-number lookup), `→ SQL failed SqliteException: SQLite Error 19: 'UNIQUE constraint failed: Tickets.Number'. — INSERT INTO "Tickets" …`, `• editor caught DbUpdateException → friendly message shown, full exception logged server-side`, `• editor lookups reloaded — a foreign key may have gone stale while the dialog was open`; the banner and a top-right `AlertBox` both read the one friendly sentence; **the dialog stays open** — untick the lab checkbox and Save again to see it succeed |
| Type a live fix (e.g. type a Title after **Add: empty title**) | live validation | The moment `txtTitle` loses focus (its `Validated` event), the validator re-runs: the icon and the summary line for Title disappear, `btnSave` enables if nothing else is wrong, and the trace shows a fresh `• validation …`/`• editor Save enabled|disabled: …` pair — the same live re-check any `ComboBox`/`DateTimePicker` change triggers |
| **Break the database**, then any Save | failure · outage | Unchanged from Module 4: `DatabaseUnavailableException` → friendly banner, dialog stays open |
| **Restore and search**, then Save again | recovery | Unchanged from Module 4 |
| **Slow save (2.5 s)**, then any Save | progress · guard | Unchanged from Module 4 — armed for the *next* dialog, including the lab demo buttons |
| Edit ticket / Delete / Cancel / Simulate delete / Search / paging / Module 1–3 buttons | unchanged | See the Module 1–4 READMEs — every path still works |

The right-hand card is unchanged: **Server ⇄ Database · EF Core lifetime & SQL trace**. Every validation run
— live or on Save — reaches this list the same way a save or a search always has, through the
`Action<char, string, string>` callback `TicketBrowserPage` gives the dialog when it is opened.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | DataAnnotations on `TicketEditModel` and a `TicketValidator` returning field-level and summary errors | [`docs/ValidationLayers.md`](docs/ValidationLayers.md) · `SupportDesk.Services/TicketEditModel.cs`, `TicketValidator.cs` |
| 2 | `ErrorProvider` cleared before each run and set for Title, Customer, Category and DueDate | [`docs/ErrorProviderFeedback.md`](docs/ErrorProviderFeedback.md) · `TicketEditorForm.ShowValidation` |
| 3 | Summary label for cross-field rules and Save disabled while invalid | [`docs/CrossFieldRuleAndSaveGating.md`](docs/CrossFieldRuleAndSaveGating.md) · `TicketEditorForm.RunValidation`, `validationSummaryLabel` |
| 4 | `DbUpdateException` handled with a friendly message for a duplicate ticket number | [`docs/DuplicateNumberHandling.md`](docs/DuplicateNumberHandling.md) · `TicketEditorForm.FailDbUpdateAsync`, `TicketCommandService.SaveAsync(…, forceDuplicateNumber)`, `FriendlyDatabaseErrors` |
| 5 | At least five negative test cases | [`docs/NegativeTests.md`](docs/NegativeTests.md) · `SupportDesk.Tests/TicketValidatorTests.cs`, `DuplicateNumberTests.cs` |

The Module 1–4 deliverables and their notes are unchanged and still in `docs/`:
[`ModelAndDeleteBehaviours.md`](docs/ModelAndDeleteBehaviours.md), [`RowVersionAndIndexes.md`](docs/RowVersionAndIndexes.md),
[`MigrationWorkflow.md`](docs/MigrationWorkflow.md), [`SeedData.md`](docs/SeedData.md),
[`SchemaVsUiValidation.md`](docs/SchemaVsUiValidation.md), [`TicketSearchService.md`](docs/TicketSearchService.md),
[`TicketBrowserBinding.md`](docs/TicketBrowserBinding.md), [`PagingInTheDatabase.md`](docs/PagingInTheDatabase.md),
[`LookupComboBoxes.md`](docs/LookupComboBoxes.md), [`SearchPagingAndTheLoadingGuard.md`](docs/SearchPagingAndTheLoadingGuard.md),
[`EditorFormAndBindingSource.md`](docs/EditorFormAndBindingSource.md), [`ControlDataBindings.md`](docs/ControlDataBindings.md),
[`LoadAndSaveFlows.md`](docs/LoadAndSaveFlows.md), [`DeleteConfirmationAndReload.md`](docs/DeleteConfirmationAndReload.md),
[`DialogResultAndGridRefresh.md`](docs/DialogResultAndGridRefresh.md).

## Where things live

```
Module 5/
├─ SupportDesk.slnx                 the four projects
├─ artifacts/sql/
│  └─ supportdesk_migrations.sql    unchanged since Module 2 — the model did not change in Module 5
├─ SupportDesk.Web/                 the Wisej.NET application (net10.0-windows;net10.0)
│  ├─ Startup.cs                    + AddTransient<TicketValidator>()
│  ├─ TicketBrowserPage.cs          + btnLabEmptyTitle/TitleTooLong/NoCustomerCategory/ClosedFutureDue/
│  │                                  DuplicateNumber_Click, OpenEditorAsync(…, EditorLabScenario)
│  ├─ TicketBrowserPage.Designer.cs + panelActions row 1 = Module 5 (5 demo buttons + Break/Restore/
│  │                                  SlowSave/Clear); panelActions2..5 = the former Module 4/3/2/1 rows,
│  │                                  unchanged inside, plus a new panelActions5 for Module 1's row;
│  │                                  panelTrace/panelModel widened to fit
│  ├─ TicketEditorForm.cs           + EditorLabScenario enum, ApplyLabScenario, Field_Changed,
│  │                                  DueDate_Changed, RunValidation, ShowValidation, FailDbUpdateAsync,
│  │                                  ReloadLookupsAsync; SaveAsync now validates before any DbContext
│  └─ TicketEditorForm.Designer.cs  + validationSummaryLabel, chkDuplicateNumber; live-validation event
│                                     wiring on txtTitle/txtDescription/cboCustomer/cboCategory/cboStatus/
│                                     dtpDueDate; form grown to 620×600
├─ SupportDesk.Data/                unchanged since Module 2 — no entity or mapping changes in Module 5
├─ SupportDesk.Services/
│  ├─ TicketEditModel.cs            + DataAnnotations (Title/Description/Status/Priority/CustomerId/CategoryId)
│  ├─ TicketValidator.cs            NEW — ValidationMessage, TicketValidator.Validate
│  ├─ FriendlyDatabaseErrors.cs     NEW — the plain sentences the operator sees for a rejected save/delete
│  ├─ TicketCommandService.cs       SaveAsync + forceDuplicateNumber lab prop
│  └─ (TicketQueryService.cs / DevelopmentSeeder.cs / ModelDemoService.cs / ... — unchanged)
├─ SupportDesk.Tests/               xunit, SQLite in memory — 68 tests
│  ├─ TicketValidatorTests.cs       NEW — the Module 5 deliverable: 13 tests against the validator alone
│  ├─ DuplicateNumberTests.cs       NEW — the DbUpdateException/SqliteException database test + the
│  │                                  friendly-message assertion
│  └─ (everything from Modules 1–4 — unchanged, still green)
└─ docs/                            the five Module 5 deliverables + the fifteen Module 1–4 notes
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the Module 4 solution, confirm the editor still saves any input straight through `SaveChangesAsync` | `SupportDesk.slnx`; unchanged from Module 4 until the edits below |
| 2 · `DataAnnotations` on `TicketEditModel`: `[Required, StringLength(180)]` Title, `[StringLength(4000)]` Description, `[Required]` Status/Priority/CustomerId/CategoryId, foreign keys stay `int?` | `SupportDesk.Services/TicketEditModel.cs` — see [`docs/ValidationLayers.md`](docs/ValidationLayers.md) |
| 3 · `ValidationMessage(FieldName, Message)` record and `TicketValidator` service: `Validator.TryValidateObject(..., validateAllProperties: true)`, flatten by member name, append the cross-field rule; register Transient, inject into `TicketEditorForm` | `SupportDesk.Services/TicketValidator.cs`; `SupportDesk.Web/Startup.cs` — see [`docs/ValidationLayers.md`](docs/ValidationLayers.md) |
| 4 · `SaveAsync`: `errorProvider.Clear()` → `EndEdit()` → validate → `ShowValidation` maps messages to `SetError` on Title/CustomerId/CategoryId/DueDate, joins the summary label, returns before any `DbContext` | `TicketEditorForm.SaveAsync`, `ShowValidation`, `RunValidation` — see [`docs/ErrorProviderFeedback.md`](docs/ErrorProviderFeedback.md) and [`docs/CrossFieldRuleAndSaveGating.md`](docs/CrossFieldRuleAndSaveGating.md) |
| 5 · Live validation on `editBindingSource`/control events; `btnSave.Enabled` tracks validity, false while saving, restored in `finally` | `TicketEditorForm.Field_Changed`, `DueDate_Changed`, `RunValidation` — see [`docs/CrossFieldRuleAndSaveGating.md`](docs/CrossFieldRuleAndSaveGating.md) |
| 6 · `catch (DbUpdateException)` below the concurrency catch: log full exception server-side, one friendly sentence via `AlertBox`, reload lookups on a possibly-stale foreign key | `TicketEditorForm.FailDbUpdateAsync`, `ReloadLookupsAsync` — see [`docs/DuplicateNumberHandling.md`](docs/DuplicateNumberHandling.md) |
| 7 · Reproduce the duplicate-number path: `chkDuplicateNumber` lab checkbox, `TicketCommandService.SaveAsync(..., forceDuplicateNumber)` | `TicketEditorForm.Designer.cs` (`chkDuplicateNumber`), `TicketCommandService.SaveAsync` — see [`docs/DuplicateNumberHandling.md`](docs/DuplicateNumberHandling.md) |
| 8 · At least five negative tests against `TicketValidator` alone; assert `FieldName` | `SupportDesk.Tests/TicketValidatorTests.cs` — see [`docs/NegativeTests.md`](docs/NegativeTests.md) |
| 9 · Show every path: valid save, empty title/missing customer with icons, closed+future-due-date in the summary with Save disabled, duplicate number with the friendly database message | the bottom bar row 1 buttons — see **What to click** above |
| 10 · Review & run: open the editor twice in a row, confirm the previous run's icons are cleared; confirm no `DbContext` is created when validation fails; write the layer note | **Self-check answers** below; `docs/NegativeTests.md`; the "no `◦ context created` line at all" row in **What to click** |

Lab code check (`labs.js` m5): a `TicketEditModel` with the named `DataAnnotations`, a `TicketValidator`
service with `Validate`, `errorProvider.Clear()`/`SetError` in `SaveAsync`, a validation summary label,
`Save` gated on validity, a `catch (DbUpdateException)` below the concurrency catch, and at least five
negative tests — all present and exercised by `TicketValidatorTests.cs`/`DuplicateNumberTests.cs`. Two
naming deviations from the lab guide's literal control names are carried over from the course cookbook and
Module 4 (see **Deviations** below).

## Self-check answers (the lab's student review questions)

- **If you removed every DataAnnotation from `TicketEditModel` but kept them on the `Ticket` entity, which
  of your five negative tests would still fail before `SaveChangesAsync`, and why?**

  None of them. Every negative test in `TicketValidatorTests.cs` calls `TicketValidator.Validate(model)`
  directly, and that method's only source of rules is `Validator.TryValidateObject(model, context, results,
  validateAllProperties: true)` — it inspects the attributes on **`TicketEditModel`**, the object actually
  passed in, never on `Ticket`. Moving `[Required]`/`[StringLength]` onto the entity would still shape the
  generated column (nullability, `HasMaxLength`) through EF Core's convention-based model discovery, but that
  is a fact the *migration* would pick up, not a fact `Validator.TryValidateObject` would ever see — .NET's
  `DataAnnotations` validator only ever reads attributes on the type of the object it is handed, and it has
  no idea `TicketEditModel.Title` and `Ticket.Title` are "the same field" in the lesson's sense; they are two
  unrelated properties on two unrelated classes. So every one of `Empty_title_is_reported_against_Title`,
  `Title_over_180_characters_is_reported_against_Title`, `Missing_customer_is_reported_against_CustomerId`,
  `Missing_category_is_reported_against_CategoryId` and
  `Closed_ticket_with_a_future_due_date_is_reported_against_DueDate` would start failing — `Validate` would
  return an empty list for every one of them, because there would be nothing left on `TicketEditModel` for
  `Validator.TryValidateObject` to check (the closed/future-due-date test would fail regardless, since that
  rule is hand-written in `TicketValidator.Validate` itself and was never an attribute to begin with — but
  the other four would go from "reported" to "silently accepted"). The ticket would still be caught
  eventually — `CK_Tickets_Title_Length` and the `NOT NULL` columns on `CustomerId`/`CategoryId` would refuse
  it at `SaveChangesAsync` — but as a raw `DbUpdateException` a support agent cannot read, exactly the
  production problem Module 5's lesson opens with, not as a friendly `ErrorProvider` icon before the database
  is ever touched.

- **The closed-ticket rule reports `DueDate` as its field. What would change for the user if it reported no
  field at all, and when would that be the better choice?**

  Reporting `DueDate` (`new ValidationMessage(nameof(TicketEditModel.DueDate), "Closed tickets cannot have a
  future due date."`) routes the message through `ShowValidation`'s `switch` to
  `errorProvider.SetError(this.dtpDueDate, text)` — the operator sees a red icon sitting specifically on the
  date picker, next to the message repeated in `validationSummaryLabel`. That is a real, useful nudge *because
  a Closed ticket's due date should usually just be cleared* — pushing the fix toward one control is the
  right call when the rule genuinely has a natural "wrong" field. If the message instead carried a `null`
  `FieldName` (`ShowValidation`'s `switch` has no case for `null`, so the loop simply skips painting any
  icon — `errorProvider.Clear()` still runs, but nothing new is set), the rule would surface **only** in
  `validationSummaryLabel`, with no icon anywhere. That would change the user experience: nothing on the form
  visually points at *which* control to change, so the operator has to read the sentence and decide for
  themselves. That is the better choice exactly when the rule is genuinely about a **relationship between two
  peer fields where either side is an equally valid fix** — this very rule, looked at the other way, could
  just as reasonably be fixed by changing `Status` back to something other than `Closed` as by clearing
  `DueDate`. Blaming `DueDate` specifically is a deliberate, defensible bias (closing a ticket is usually the
  more final, intentional action; a leftover due date is usually the accident), but a rule with no such
  asymmetry — two lookups that must not both be set, say — is exactly the case the lab guide's question is
  pointing at: reporting no field at all avoids implying a "correct" side to a genuinely two-sided problem.

- **Two agents create a ticket in the same second and the second save throws `DbUpdateException`. What does
  the second agent see, what does the server log, and which of the two would you show to a customer?**

  The **second agent** sees exactly one sentence, twice: once inline in the dialog's own `labelBanner`
  (`ShowBanner(friendly)`) and once as a top-right toast
  (`AlertBox.Show(friendly, MessageBoxIcon.Error, alignment: TopRight, autoCloseDelay: 4000)`), where
  `friendly` is `FriendlyDatabaseErrors.TicketSaveRejected` — *"The ticket could not be saved because the
  database rejected the change. Please verify required fields and lookup values."* Nothing about SQL, the
  unique index's name, or the fact that it was specifically a `Number` collision. The dialog stays open with
  their typed values intact, and `ReloadLookupsAsync()` refreshes the three lookup ComboBoxes in case a
  foreign key (not just the number) was the real cause. The **server log** gets everything:
  `Console.Error.WriteLine($"[SupportDesk] server log: DbUpdateException saving ticket — {ex}")` writes the
  *entire* exception — type, message, stack trace and, through `ex.InnerException`, the raw
  `Microsoft.Data.Sqlite.SqliteException` with `SQLite Error 19: 'UNIQUE constraint failed:
  Tickets.Number'.` — and the trace card shows the same failed `INSERT` statement (compacted, but including
  the constraint name) via `QueryTraceInterceptor.CommandFailed`, because that reporting happens inside the
  EF Core pipeline before the exception is ever caught by application code. **Which of the two would reach a
  customer:** only the first, the friendly sentence — and only if a customer-facing screen exists at all;
  this course's Support Desk console is an internal agent tool, so today "the customer" never sees either one.
  The second, the full exception with the constraint name, must never leave the server log: a constraint name
  and a SQL fragment are implementation detail an attacker could use to fingerprint the schema, and neither
  one is actionable for anybody who cannot fix the code.

## Verified / unverified

Built and tested on this machine (Wisej-4 4.1.0, .NET 10, EF Core 10.0.12, SQLite):

- `dotnet build SupportDesk.slnx -nologo -v q` — succeeds with **0 warnings, 0 errors**.
- `dotnet test SupportDesk.Tests -nologo -v q` — **68 passed**, 0 failed (52 carried over from Modules 1–4
  unchanged, 13 new in `TicketValidatorTests.cs`, 3 new in `DuplicateNumberTests.cs`).
- The validator's exact messages were captured from `TicketValidator.Validate` running against SQLite in
  memory (a console probe and the tests, the same code path `TicketEditorForm` calls), not written from
  memory:
  ```
  Title: The Title field is required.
  Description: Description cannot be longer than 4000 characters.
  CustomerId: Choose a customer.
  CategoryId: Choose a category.
  DueDate: Closed tickets cannot have a future due date.
  ```
  and, separately, `Title cannot be longer than 180 characters.` for the over-length case.
- The duplicate-number failure was captured the same way, from `TicketCommandService.SaveAsync(model,
  TimeSpan.Zero, forceDuplicateNumber: true)` against SQLite in memory:
  `DbUpdateException` with `InnerException` typed `Microsoft.Data.Sqlite.SqliteException`, message
  `SQLite Error 19: 'UNIQUE constraint failed: Tickets.Number'.` — the friendly sentence shown to the
  operator (`FriendlyDatabaseErrors.TicketSaveRejected`) was checked, in its own test, to contain none of
  `SQLite`, `SQL`, `constraint`, `UNIQUE`, `Data Source` or `Tickets.Number`.
- `Validator.TryValidateObject(..., validateAllProperties: true)` really does report every broken rule at
  once, not just the first: `Every_broken_rule_is_reported_at_once_not_just_the_first` breaks three rules
  simultaneously and asserts exactly three messages come back.
- The cross-field rule's boundary is exact, not approximate: `Closed` + today's date does **not** report
  (`Closed_ticket_with_todays_due_date_is_not_reported_the_rule_is_strictly_future`), `Closed` + a past date
  does not report, and `Open` (or any other status) + a future date does not report — the rule is proven to
  fire in exactly the one combination the lesson names.
- `TicketCommandService.SaveAsync`'s ordinary path (`forceDuplicateNumber: false`) still produces a fresh,
  non-colliding number when the lab prop is off — the new parameter changes nothing about Module 4's
  behaviour by default.

Facts carried over from Modules 1–4 and still relied on here: `Application.Services.AddService<IServiceProvider>(app.Services)`
makes `[Inject]` resolve through Microsoft DI for a `Page` and a `Form` constructed with `new`;
`TicketEditorForm`'s `[Inject] TicketValidator Validator` depends on this exactly as `[Inject]
TicketCommandService Commands` always has; application services must be Transient or Singleton for the same
root-provider reason; `Application.Update(this)` in `finally` pushes the final state after an `await`;
Wisej.NET delivers a second click while an awaited handler is pending, which is what makes `_saving` and
`btnSave.Enabled` load-bearing together, not just cosmetic.

**Deviations from the lab guide's exact wording, and why:**

1. **Control names follow the course cookbook and Module 4, not the lab guide's alternate names.** The lab
   guide's task text names `titleTextBox`, `customerComboBox`, `categoryComboBox` and `dueDateTimePicker`;
   this module (like Module 4 before it) uses `txtTitle`, `cboCustomer`, `cboCategory` and `dtpDueDate` — the
   names the course cookbook fixes explicitly for this course ("`txtTitle`/`titleTextBox` as the module's lab
   names them") and the names already chosen in Module 4's `TicketEditorForm.Designer.cs`, which this module
   is a copy of. Renaming them now would break every `DataBindings.Add` call and every doc cross-reference
   Module 4 already wrote, for a purely cosmetic difference.
2. **The `[Required]`/`[StringLength]` `ErrorMessage`s are custom, fixed strings, not the DataAnnotations
   framework defaults.** The lesson's own sample code lets `ErrorMessage` default (`Validator.TryValidateObject`
   would then generate "The Title field is required." from the property name automatically for `[Required]`,
   but a less predictable "The field Description must be a string with a maximum length of 4000." shape for
   `[StringLength]`). This module sets `ErrorMessage` explicitly on every attribute instead, so the exact text
   shown to the operator and asserted in tests is chosen and stable rather than inferred from .NET's resource
   strings, which can vary by attribute and are easy to get wrong from memory. The `[Required]` message for
   `Title` was deliberately written to match the framework default anyway (`"The Title field is required."`)
   since that is the literal text the lab's own walkthrough quotes.
3. **`chkDuplicateNumber` is a genuinely new control**, not named by the lab guide or `labs.js` at all — it is
   this module's answer to "add at least five negative test cases" and "a `DbUpdateException` handler … for a
   duplicate ticket number" needing a *reachable* way to reproduce the failure from the running page, per this
   module's build instructions, rather than only from a test. It is visible only when adding a new ticket
   (`_ticketId is null`), since `forceDuplicateNumber` is only ever read when `model.Id == 0`.
4. **`TicketEditModel` still has no `RowVersion` property**, unchanged from Module 4's documented deviation —
   Module 7 adds it, with the conflict dialog it needs to be useful.

Every `DataBindings.Add` call continues to use `nameof(TicketEditModel.X)`, unchanged since Module 4.

**Not verified here — for the browser reviewer.** The application was not started (by instruction), so
everything below is Wisej.NET behaviour that only a running page and a running dialog can confirm — this
list is the main content of what remains open, per the course cookbook's honesty rule:

1. `errorProvider.SetError` actually renders a visible icon and tooltip next to `txtTitle`/`cboCustomer`/
   `cboCategory`/`dtpDueDate` (and `txtDescription`/`cboStatus`/`cboPriority`), and `Clear()` actually removes
   every icon at the start of the next run — including the "open the editor twice in a row" case the lab
   guide's Review & run step names.
2. `validationSummaryLabel`'s neutral pre-touch text, its color change to the red failure palette once
   painted, and its `Visible` toggling all render as described — this is straightforward WinForms-style logic
   in code, but only the browser shows the actual paint.
3. `btnSave.Enabled` visibly greys the button out and back in, live, as the operator types into `txtTitle` or
   changes a `ComboBox`/`DateTimePicker` — the underlying `RunValidation` logic is exercised by
   `TicketValidatorTests.cs`, but the *event wiring* (`Validated`, `SelectedValueChanged`, `ValueChanged` on
   the real Wisej.NET controls, in the real browser) is not.
4. `chkDuplicateNumber` is visible for a new ticket and hidden for an existing one, and ticking it truly
   changes what `SaveAsync` sends — proven server-side by `DuplicateNumberTests.cs`, not proven end-to-end
   through a real click in the dialog.
5. `AlertBox.Show(..., alignment: TopRight, autoCloseDelay: 4000)` appears top-right for the duplicate-number
   failure, the same as every other failure path in this course.
6. The **"Edit: closed ticket, due date next week"** button finds a real `Closed` ticket from the seed and
   the dialog opens on it correctly — the search itself (`TicketQueryService.SearchTicketsAsync` filtered to
   `Status = Closed`) is the same code Module 3's browser card already exercises, but this specific call path
   was not run against a live seeded database in this session.
7. The reviewer-visible flow end to end: click each of the five Module 5 demo buttons in turn, see the
   correct icon(s)/summary/disabled-Save for each on the first Save click; fix the field live and watch the
   icon disappear without clicking Save again; tick and untick `chkDuplicateNumber` on a fresh Add and watch
   the duplicate-number failure appear and then the retry succeed.
8. `dtpDueDate.ShowCheckBox = true` behaves as "unticked = no date" (flagged unverified in the cookbook since
   Module 1, still unverified here) — Module 5 additionally relies on `DueDate_Changed` firing on both a tick
   and an untick, which is new behaviour this module adds on top of that same unverified fact.

**Browser verification: see the note at the end.**

## Browser results (reviewer, 2026-09-10)

Run on this machine at <http://localhost:5405> in the Browser pane; trace read back from the page. The four Module 4
binding fixes were ported into this module before the run (`BindLookupControls`, `NamedValue` rows for Status/Priority,
the `Load` try/catch, `DataSource = typeof(TicketEditModel)`), and one Module 5 change was made:

- **Found and fixed:** the lab demo buttons opened the dialog with Save **disabled** (correct — the model is invalid), so
  "click Save to see the icons" could never happen: a disabled Wisej button delivers no click. The four validation
  scenarios (`EmptyTitle`, `TitleTooLong`, `NoCustomerCategory`, `ClosedFutureDueDate`) now run the validator once as the
  dialog opens — the pre-filled fields count as touched — and the trace says so:
  `• editor lab scenario NoCustomerCategory: fields pre-filled, validator runs once as if you had touched them`.
  A hand-typed ticket still gets its first icons from the live re-check when a field changes.

Paths verified:

- **Add: empty title** → dialog with Customer/Category chosen, Title blank, Save disabled, the neutral summary
  *Fill in Title, Customer and Category.*; the Title icon appears once the scenario validation runs / a field changes.
- **Add: no customer/category** → two red `errorProvider` icons (next to `cboCustomer` and `cboCategory`), the red summary
  *Choose a customer. · Choose a category.*, Save disabled; trace `• validation CustomerId: Choose a customer. · CategoryId: Choose a category.`
  and `• editor Save disabled: model invalid`. **Cancel** → `• editor Cancel: DialogResult.Cancel — no context created, nothing written`.
- **Edit: closed ticket, due date next week** → *Edit ticket SD-1005* opens on a Closed ticket with the due date ticked and set a
  week ahead, one icon next to `dtpDueDate`, summary *Closed tickets cannot have a future due date.*, Save disabled;
  trace `• validation DueDate: Closed tickets cannot have a future due date.`.
- **Add: duplicate number (DbUpdateException)** → valid fields and the lab checkbox ticked, Save enabled; **Save** →
  `• validation no messages — model valid`, `◦ context #12 created`, `→ SQL SELECT "t"."Number" FROM "Tickets" AS "t" ORDER BY "t"."Id" LIMIT 1`,
  `• editor SaveAsync: lab prop — reusing existing number SD-1001 to reproduce the UNIQUE constraint on Tickets.Number`,
  `→ SQL failed SqliteException: SQLite Error 19: 'UNIQUE constraint failed: Tickets.Number'. — INSERT INTO "Tickets" (…)`,
  `• editor caught DbUpdateException → friendly message shown, full exception logged server-side`, then a second context reloading the three
  lookups (`• editor lookups reloaded — a foreign key may have gone stale while the dialog was open`). The dialog stays open with the red banner
  *The ticket could not be saved because the database rejected the change. Please verify required fields and lookup values.* and the same
  sentence as a top-right AlertBox — no SQL, no constraint name anywhere on screen.
- The Module 1–4 rows behave as in their own modules (grid, paging, editor add/edit/delete, Simulate, Cancel).

