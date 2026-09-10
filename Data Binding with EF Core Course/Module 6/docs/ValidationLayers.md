# Validation layers: DataAnnotations on the edit model, TicketValidator for the domain rule

**Deliverable:** DataAnnotations on `TicketEditModel` and a `TicketValidator` returning field-level and summary errors.

## What changed since Module 4

Module 4's `SaveAsync` had one inline check: `if (string.IsNullOrWhiteSpace(model.Title)) …`. Module 5 replaces it
with two things that stay apart on purpose:

1. **`TicketEditModel`** (`SupportDesk.Services/TicketEditModel.cs`) is decorated with
   `System.ComponentModel.DataAnnotations`:
   - `Title`: `[Required]`, `[StringLength(SupportDeskContext.TitleMaxLength)]` — the same `180` the database's
     `CK_Tickets_Title_Length` CHECK constraint uses (`SupportDesk.Data/SupportDeskContext.cs`), named once and
     shared, never retyped as a second `180` that could drift out of sync.
   - `Description`: `[StringLength(4000)]`.
   - `Status`, `Priority`, `CustomerId`, `CategoryId`: `[Required]`. The two foreign keys stay `int?` — an
     unselected `ComboBox` yields `null`, and `null` is exactly what `[Required]` rejects; a non-nullable `int`
     would silently pass as `0`, which is not a real category or customer.
   - `AgentId` carries no annotation: an unassigned ticket is a valid ticket, by design since Module 4.

2. **`TicketValidator`** (`SupportDesk.Services/TicketValidator.cs`) is the service that actually *runs* those
   annotations. Nothing does that automatically — EF Core maps `[Required]`/`[MaxLength]` on an *entity* into
   column nullability and width; it never runs a `DataAnnotations` validation pass before `SaveChangesAsync`.
   `Validate(TicketEditModel model)`:
   ```csharp
   var results = new List<ValidationResult>();
   var context = new ValidationContext(model);
   Validator.TryValidateObject(model, context, results, validateAllProperties: true);

   var messages = results
       .SelectMany(r => r.MemberNames.DefaultIfEmpty(null),
                   (r, member) => new ValidationMessage(member, r.ErrorMessage ?? "Invalid value."))
       .ToList();

   if (model.Status == TicketStatuses.Closed && model.DueDate?.Date > DateTime.Today)
       messages.Add(new ValidationMessage(nameof(TicketEditModel.DueDate), "Closed tickets cannot have a future due date."));

   return messages;
   ```
   `validateAllProperties: true` means every annotated property is checked, not just the first failure —
   `TicketEditorForm` can show three icons at once, not one at a time. The cross-field rule ("a Closed ticket
   cannot have a future due date") cannot be expressed as an attribute on a single property, so it is appended
   by hand, reported against `DueDate`.

`ValidationMessage(string? FieldName, string Message)` is the shared shape: `FieldName` matches a
`TicketEditModel` property name so `TicketEditorForm.ShowValidation` can route it to a control, or is `null`
when a rule belongs to no single field.

## Registration

`TicketValidator` is registered `Transient` in `SupportDesk.Web/Startup.cs`, next to `TicketCommandService` —
the same root-provider reasoning the cookbook has followed since Module 1: it is stateless (no fields at all)
and Wisej.NET resolves `[Inject]` through the root Microsoft DI provider, so a `Scoped` registration would
throw at construction.

## Why this is UI-free

`TicketValidator` has no `using Wisej.Web`, no `ErrorProvider`, no form. It takes a `TicketEditModel` and
returns data. That is what lets `TicketValidatorTests.cs` (13 tests) run against it directly — no page, no
session, no database — and is also why the cross-field rule and every `[Required]`/`[StringLength]` check
run identically whether `TicketEditorForm` calls them or a future import screen does.

## Evidence

- `dotnet build` — 0 warnings, 0 errors, confirming the attributes compile and `TitleMaxLength` is shared.
- `SupportDesk.Tests/TicketValidatorTests.cs` — 13 tests, all passing: a valid model produces no messages;
  each individual rule (empty title, over-length title, the 180-character boundary itself, missing customer,
  missing category, the closed/future-due-date cross-field rule, today's date, a past date, an open ticket
  with a future date, an over-length description) is exercised in isolation, plus one test proving three
  broken rules are reported *together*, not just the first.
- Console probe of `TicketValidator.Validate` against a model that breaks five rules at once, run on this
  machine:
  ```
  Title: The Title field is required.
  Description: Description cannot be longer than 4000 characters.
  CustomerId: Choose a customer.
  CategoryId: Choose a category.
  DueDate: Closed tickets cannot have a future due date.
  ```
- `TicketEditorForm.SaveAsync` — see [`SummaryAndSaveGating.md`](SummaryAndSaveGating.md) — calls
  `Validator.Validate(model)` immediately after `EndEdit` and returns **before any `DbContext` is created**
  when the result is non-empty; see [`NegativeTests.md`](NegativeTests.md) for the "no context created" trace
  line.
