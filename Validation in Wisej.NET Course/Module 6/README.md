# ValidationClinic · Validation in Wisej.NET · Module 6

Local lab build for **Module 6 · DataGridView Cell Validation and Row Errors**. The contacts grid rejects uncommitted bad email/age values, explains conversion failures, marks Closed rows without a date and checks every row before saving the working copy.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

Requirements: .NET 10 SDK and NuGet access to `Wisej-4` 4.1.0. Use a Wisej development license or trial
as required by your installation. No database setup or external service is needed.

```bash
cd "Validation in Wisej.NET Course/Module 6/ValidationClinic"
dotnet run -f net10.0 --urls http://localhost:5906
```

Open <http://localhost:5906>. In Visual Studio, open `ValidationClinic.slnx` and press F5.
The project targets both `net10.0-windows` and `net10.0`, so the CLI needs `-f` when running.

## What to try

| Action | Expected result |
|---|---|
| Double-click a grid email, type missing-at-sign, then Tab | CellValidating reads FormattedValue, refuses the edit and puts a message on the cell and in the summary. |
| Type abc, -1 or 121 in Age | The cell rejects each value; 0 and 120 pass. Escape cancels an invalid edit. |
| Set Status to Closed with no closed date | The row header and date cell both show the rule, and the visible summary explains it. |
| Type not-a-date in Closed date | DataError suppresses the conversion exception and logs technical details on the server. |
| Correct the date; Save grid | Every non-placeholder row is rechecked, including annotations and shared business rules, then saved together. |
| Start a new row but leave required fields blank; Save grid | The partially entered contact is validated; only the untouched placeholder is skipped. |
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
| Design note and reproduction steps | [docs/GridValidation.md](ValidationClinic/docs/GridValidation.md) |

## Self-check answers

- Replacing a previously valid grid email with an @-less value demonstrates why reading cell.Value checks stale data. FormattedValue contains the edit still in progress.

- The age handler uses TryParse and rejects non-numbers. DataError remains necessary for other conversion and commit failures, such as not-a-date in ClosedDate.

- The Closed rule appears on the row and date cell, with a visible summary. Correcting or changing status removes its messages without erasing unrelated conversion errors.

## Design notes

[Module note](ValidationClinic/docs/GridValidation.md) records the API choices and failure behavior.

The sample uses `AutoValidate.EnableAllowFocusChange`: errors remain visible while the user reaches another
field to fix a related value. Cancel never causes validation. Save explicitly validates every relevant layer.
The in-memory repository is deliberately a storage stand-in; callers own validation, as in the lesson.
It checks name uniqueness and swaps the saved snapshot only after all repository checks pass.


## Verification

Both target frameworks are compiled. The capstone has domain tests and browser checks for its validation
and persistence paths. See the [course verification record](../README.md#verification) for the tested scope.
