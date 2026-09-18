# ValidationClinic · Validation in Wisej.NET · Module 5

Local lab build for **Module 5 · Data-Bound Validation and Model Errors**. The form binds to an annotated model, commits pending edits before validation and maps model errors to fields. A separate dialog demonstrates automatic `IDataErrorInfo` feedback and switching the provider source.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

Requirements: .NET 10 SDK and NuGet access to `Wisej-4` 4.1.0. Use a Wisej development license or trial
as required by your installation. No database setup or external service is needed.

```bash
cd "Validation in Wisej.NET Course/Module 5/ValidationClinic"
dotnet run -f net10.0 --urls http://localhost:5905
```

Open <http://localhost:5905>. In Visual Studio, open `ValidationClinic.slnx` and press F5.
The project targets both `net10.0-windows` and `net10.0`, so the CLI needs `-f` when running.

## What to try

| Action | Expected result |
|---|---|
| Load valid values; set Age to 130; Save | The Integer field rule passes, but Range rejects the bound model and the trace stops at the model stage. |
| Enter a name of 81 characters; Save | StringLength produces a Name field error; 80 is allowed. |
| Edit email and immediately press Enter | EndEdit and explicit binding WriteValue run before the model checks; the typed value is checked. |
| Cancel; open Data-bound errors; blank Name; Check model | The bound ErrorProvider reads IDataErrorInfo. This dialog never calls SetError or saves. |
| Switch source in Data-bound errors | BindToDataAndErrors receives the new BindingSource and an empty member in one call. |
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
| Design note and reproduction steps | [docs/BindingAndModelErrors.md](ValidationClinic/docs/BindingAndModelErrors.md) |

## Self-check answers

- The missing step is committing pending edits before model validation. EndEdit completes the binding edit; this implementation also calls WriteValue and uses OnPropertyChanged to cover Enter-to-save.

- Without validateAllProperties: true, StringLength, EmailAddress and Range are skipped. Required alone does not enforce them.

- Annotations and ContactValidator can be invoked by an import service. Attributes do not execute themselves; a non-UI caller must explicitly run both validators.

## Design notes

[Module note](ValidationClinic/docs/BindingAndModelErrors.md) records the API choices and failure behavior.

The sample uses `AutoValidate.EnableAllowFocusChange`: errors remain visible while the user reaches another
field to fix a related value. Cancel never causes validation. Save explicitly validates every relevant layer.
The in-memory repository is deliberately a storage stand-in; callers own validation, as in the lesson.
It checks name uniqueness and swaps the saved snapshot only after all repository checks pass.


## Verification

Both target frameworks are compiled. The capstone has domain tests and browser checks for its validation
and persistence paths. See the [course verification record](../README.md#verification) for the tested scope.
