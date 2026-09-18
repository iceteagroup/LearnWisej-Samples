# ValidationClinic · Validation in Wisej.NET · Module 1

Local lab build for **Module 1 · Validation Fundamentals**. The customer intake form introduces `Validating`, `Validated`, `e.Cancel`, `AutoValidate`, a working Cancel button and the `ValidateChildren()` save guard.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

Requirements: .NET 10 SDK and NuGet access to `Wisej-4` 4.1.0. Use a Wisej development license or trial
as required by your installation. No database setup or external service is needed.

```bash
cd "Validation in Wisej.NET Course/Module 1/ValidationClinic"
dotnet run -f net10.0 --urls http://localhost:5901
```

Open <http://localhost:5901>. In Visual Studio, open `ValidationClinic.slnx` and press F5.
The project targets both `net10.0-windows` and `net10.0`, so the CLI needs `-f` when running.

## What to try

| Action | Expected result |
|---|---|
| Open customer intake; enter a name but leave email untouched; Save | The untouched email is checked, an error appears, and the repository write count stays at zero. |
| Type an @-less email and leave the field | An error is shown; focus may move because this sample chooses EnableAllowFocusChange. Save still refuses. |
| Type a padded name and leave it | Validated trims only accepted text; it never writes to the repository. |
| Cancel while name is invalid | The dialog closes and the repository remains unchanged. |
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
| Design note and reproduction steps | [docs/FocusBehavior.md](ValidationClinic/docs/FocusBehavior.md) |

## Self-check answers

- An icon alone does not reject input. Without e.Cancel, ValidateChildren can return true and Save can continue.

- Leaving an editor runs its field handler; Save runs ValidateChildren for fields the user never visited, such as an untouched email.

- EnableAllowFocusChange lets the user reach another field; EnablePreventFocusChange holds focus. Both reject an invalid Save. Cancel uses CausesValidation = false in either mode.

## Design notes

[Module note](ValidationClinic/docs/FocusBehavior.md) records the API choices and failure behavior.

The sample uses `AutoValidate.EnableAllowFocusChange`: errors remain visible while the user reaches another
field to fix a related value. Cancel never causes validation. Save explicitly validates every relevant layer.
The in-memory repository is deliberately a storage stand-in; callers own validation, as in the lesson.
It checks name uniqueness and swaps the saved snapshot only after all repository checks pass.


## Verification

Both target frameworks are compiled. The capstone has domain tests and browser checks for its validation
and persistence paths. See the [course verification record](../README.md#verification) for the tested scope.
