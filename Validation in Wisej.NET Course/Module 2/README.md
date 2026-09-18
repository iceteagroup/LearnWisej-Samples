# ValidationClinic · Validation in Wisej.NET · Module 2

Local lab build for **Module 2 · Programmatic Validation and ErrorProvider Feedback**. The intake form adds pure `FieldValidation` results, one `ErrorProvider`, per-field clearing, a visible summary and an `InvalidMessage` comparison on the customer code.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

Requirements: .NET 10 SDK and NuGet access to `Wisej-4` 4.1.0. Use a Wisej development license or trial
as required by your installation. No database setup or external service is needed.

```bash
cd "Validation in Wisej.NET Course/Module 2/ValidationClinic"
dotnet run -f net10.0 --urls http://localhost:5902
```

Open <http://localhost:5902>. In Visual Studio, open `ValidationClinic.slnx` and press F5.
The project targets both `net10.0-windows` and `net10.0`, so the CLI needs `-f` when running.

## What to try

| Action | Expected result |
|---|---|
| Load invalid values, then Save | Name, email and phone problems appear in the summary and beside their fields. |
| Correct email and Tab | Only the email message disappears; the remaining problems stay visible. |
| Leave customer code as X and leave its field | The editor shows InvalidMessage, with no provider icon for that field; the summary includes it explicitly. |
| Load valid values; Save | All field messages clear and the summary hides. |
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
| Design note and reproduction steps | [docs/FeedbackStyles.md](ValidationClinic/docs/FeedbackStyles.md) |

## Self-check answers

- Clear removes every provider message, including an unrelated invalid email. SetError(txtName, "") removes only the corrected name.

- InvalidMessage is outside the provider collection. RefreshSummary reads the customer code explicitly; shared providers avoid needing that special case.

- Uniqueness is a save-time business/repository concern. A duplicate name is mapped to its field; a failed repository is a separate system message.

## Design notes

[Module note](ValidationClinic/docs/FeedbackStyles.md) records the API choices and failure behavior.

The sample uses `AutoValidate.EnableAllowFocusChange`: errors remain visible while the user reaches another
field to fix a related value. Cancel never causes validation. Save explicitly validates every relevant layer.
The in-memory repository is deliberately a storage stand-in; callers own validation, as in the lesson.
It checks name uniqueness and swaps the saved snapshot only after all repository checks pass.


## Verification

Both target frameworks are compiled. The capstone has domain tests and browser checks for its validation
and persistence paths. See the [course verification record](../README.md#verification) for the tested scope.
