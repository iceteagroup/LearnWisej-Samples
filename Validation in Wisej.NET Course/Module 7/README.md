# ValidationClinic · Validation in Wisej.NET · Module 7

Local lab build for **Module 7 · Production Validation UX and Capstone**. The capstone combines binding, field rules, annotations, business rules and the contacts grid in one ordered Save pipeline. `SummaryErrorProvider` keeps extender, icon and summary feedback together.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

Requirements: .NET 10 SDK and NuGet access to `Wisej-4` 4.1.0. Use a Wisej development license or trial
as required by your installation. No database setup or external service is needed.

```bash
cd "Validation in Wisej.NET Course/Module 7/ValidationClinic"
dotnet run -f net10.0 --urls http://localhost:5907
```

Open <http://localhost:5907>. In Visual Studio, open `ValidationClinic.slnx` and press F5.
The project targets both `net10.0-windows` and `net10.0`, so the CLI needs `-f` when running.

## What to try

| Action | Expected result |
|---|---|
| Open intake; Save with empty Name and Email | The field stage stops the pipeline; the icons and custom summary show the errors. |
| Load valid values; set Age to 130; Save | Field shape passes, then DataAnnotations blocks the model. No grid check or write follows. |
| Set start after end; Save | The business layer marks both dates and stops the pipeline. |
| Cancel; mark a grid row Closed without its date; reopen intake, Load valid values, Save | The grid stage blocks the entire save; the intake summary includes the row message. |
| Correct the grid row and intake; Save | One atomic repository replacement persists the contact and edited grid rows; the trace ends with write. |
| Create a new contact named Ada Lovelace; Save | The duplicate-name exception becomes a Name field error. No grid or contact changes are persisted. |
| Load valid values, choose Fail next write, then Save | A friendly system message appears; the write count does not change. Save again to recover with the same input. |
| Discard grid edits | Restores the last saved snapshot. Working-copy edits are not persisted automatically. |

The repository starts with Ada Lovelace and Grace Hopper. Every browser session owns its own repository.
Its data disappears when the session ends or the app restarts. The live trace shows which validation stage ran;
the write count provides visible evidence that rejected input never called a successful write.

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Main page, contacts working copy, live trace | [MainPage.cs](ValidationClinic/MainPage.cs) |
| Named controls and event-driven intake | [CustomerIntakeForm.Designer.cs](ValidationClinic/CustomerIntakeForm.Designer.cs) |
| Validation and the Save guard | [CustomerIntakeForm.cs](ValidationClinic/CustomerIntakeForm.cs) |
| Copy-on-save repository and failure switch | [Services/ContactRepository.cs](ValidationClinic/Services/ContactRepository.cs) |
| Reusable ordered rule arrays | [Validation/ClinicRules.cs](ValidationClinic/Validation/ClinicRules.cs) |
| UI-independent business rules | [Services/ContactValidator.cs](ValidationClinic/Services/ContactValidator.cs) |
| Custom minimum-age rule | [Validation/MinimumAgeValidationRule.cs](ValidationClinic/Validation/MinimumAgeValidationRule.cs) |
| Annotated model | [Models/ContactEditModel.cs](ValidationClinic/Models/ContactEditModel.cs) |
| All-property annotation validation | [Services/ModelValidation.cs](ValidationClinic/Services/ModelValidation.cs) |
| Automatic bound errors and provider-source replacement | [DataBoundErrorsForm.cs](ValidationClinic/DataBoundErrorsForm.cs) |
| Pending cell, row, conversion and Save checks | [MainPage.GridValidation.cs](ValidationClinic/MainPage.GridValidation.cs) |
| Custom summary with mirrored icons | [Validation/SummaryErrorProvider.cs](ValidationClinic/Validation/SummaryErrorProvider.cs) |
| Design note and reproduction steps | [docs/ValidationDesign.md](ValidationClinic/docs/ValidationDesign.md) |

## Self-check answers

- The pending edit must be committed before ValidateChildren and model checks. The trace records binding, fields, annotations, business, grid and write in that order.

- SummaryErrorProvider stores one entry per control and replaces or removes it. Distinct message text avoids duplicate cross-field lines.

- DuplicateNameException is a correctable field problem. An unexpected exception stays in the server log and leaves the user a friendly retry message with their entries intact.

## Design notes

[Capstone test matrix](ValidationClinic/docs/TestMatrix.md) and [five-minute demo](ValidationClinic/docs/DemoScript.md).

The sample uses `AutoValidate.EnableAllowFocusChange`: errors remain visible while the user reaches another
field to fix a related value. Cancel never causes validation. Save explicitly validates every relevant layer.
The in-memory repository is deliberately a storage stand-in; callers own validation, as in the lesson.
It checks name uniqueness and swaps the saved snapshot only after all repository checks pass.


## Automated checks

From the module folder, run `dotnet test ValidationClinic.Tests/ValidationClinic.Tests.csproj`.
The tests compile the actual model and service sources without a Wisej session: required values,
email shape, name and age boundaries, birthday boundaries, paired date errors, Closed rows,
snapshot isolation, duplicate names, atomic failed writes and retry. Browser interaction is checked separately.

## Verification

Both target frameworks are compiled. The capstone has domain tests and browser checks for its validation
and persistence paths. See the [course verification record](../README.md#verification) for the tested scope.
