# SupportDesk · Data Binding with EF Core · Module 5

Local lab build for **Module 5 · Validation, ErrorProvider and User Feedback**. The Module 4 editor learns to
say no before the database does: DataAnnotations on `TicketEditModel`, `TicketValidator` (annotations plus
the cross-field rule *Closed tickets cannot have a future due date*), `errorProvider` icons, a
`validationSummaryLabel`, Save disabled while the model is invalid, and a friendly message when the database
still rejects a save (a duplicate ticket number).

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 5/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5405
```

Tests: `dotnet test SupportDesk.Tests` (the negative cases are in `TicketValidatorTests.cs` and `DuplicateNumberTests.cs`).

## What to try

The editor now has Title, Customer, Category, Status, Priority, Due date, Agent, Description, *Escalate as
urgent*, the `validationSummaryLabel`, Delete, Cancel and Save.

- **Add Ticket**: Save starts disabled with *Fill in Title, Customer and Category.*; fill them and Save enables.
- **Empty title / no customer**: an `errorProvider` icon on the field and the message in the summary; Save stays disabled.
- **Edit a ticket, set Status to Closed with a future due date**: the icon sits on the due date and the rule
  shows in the summary. Fix it and the messages clear.
- **Open the editor twice in a row**: the previous run's icons are gone.
- **Duplicate ticket number**: a unique-index violation becomes one plain sentence in an `AlertBox`
  (`FriendlyDatabaseErrors.TicketSaveRejected`), the lookups reload, and the full exception goes to the
  server console. It is reproduced by `DuplicateNumberTests` (`SaveAsync(model, forceDuplicateNumber: true)`)
  or by two sessions saving a new ticket at the same moment.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | DataAnnotations and a `TicketValidator` returning field-level and summary errors | [`docs/ValidationLayers.md`](docs/ValidationLayers.md) · `TicketEditModel.cs`, `TicketValidator.cs` |
| 2 | `ErrorProvider` cleared before each run and set for Title, Customer, Category and DueDate | [`docs/ErrorProviderFeedback.md`](docs/ErrorProviderFeedback.md) · `TicketEditorForm.ShowValidation` |
| 3 | Summary label and Save disabled while invalid | [`docs/CrossFieldRuleAndSaveGating.md`](docs/CrossFieldRuleAndSaveGating.md) · `RunValidation`, `validationSummaryLabel` |
| 4 | `DbUpdateException` handled with a friendly message | [`docs/DuplicateNumberHandling.md`](docs/DuplicateNumberHandling.md) · `TicketEditorForm.SaveAsync`, `FriendlyDatabaseErrors` |
| 5 | At least five negative test cases | [`docs/NegativeTests.md`](docs/NegativeTests.md) · `TicketValidatorTests.cs`, `DuplicateNumberTests.cs` |

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 2 · DataAnnotations on `TicketEditModel`, foreign keys as `int?` | `SupportDesk.Services/TicketEditModel.cs` |
| 3 · `ValidationMessage` and `TicketValidator.Validate`, registered in DI, injected into the form | `TicketValidator.cs`, `Startup.cs` |
| 4 · `errorProvider.Clear()`, `EndEdit()`, validate, `ShowValidation`, return before any context | `TicketEditorForm.SaveAsync`, `ShowValidation` |
| 5 · Save disabled while invalid or saving, restored in `finally` | `RunValidation`, `Field_Changed`, `DueDate_Changed` |
| 6 · `catch (DbUpdateException)` below the concurrency catch; log, friendly sentence, reload lookups | `TicketEditorForm.SaveAsync`, `ReloadLookupsAsync` |
| 7 · Five negative tests asserting `FieldName` | `SupportDesk.Tests/TicketValidatorTests.cs` |

The lab guide calls the editor controls `titleTextBox`, `customerComboBox`, `categoryComboBox`,
`dueDateTimePicker` and `saveButton`; this sample keeps the Module 4 names (`txtTitle`, `cboCustomer`,
`cboCategory`, `dtpDueDate`, `btnSave`).

## Self-check answers

- **If the DataAnnotations moved from `TicketEditModel` to the `Ticket` entity, which negative tests would still pass?**
  None of the four annotation tests: `Validator.TryValidateObject` only reads attributes on the object it is
  given, the edit model. The closed/future-due-date test is hand-written in `TicketValidator` and unaffected.
  The bad ticket would then be caught only by the database, as a `DbUpdateException`.
- **The closed-ticket rule reports `DueDate`. What changes if it reported no field?**
  With `DueDate` the icon points at the date picker and the summary repeats it. With no field it shows only
  in the summary. No field is the better choice when either side of a two-field rule is an equally valid fix.
- **Two agents create a ticket in the same second and the second save throws `DbUpdateException`. What does each side see?**
  The agent sees one sentence in an `AlertBox`, the dialog stays open with the typed values, and the lookups
  reload. The server console holds the full exception with `UNIQUE constraint failed: Tickets.Number`. Only
  the friendly sentence ever reaches a user.
