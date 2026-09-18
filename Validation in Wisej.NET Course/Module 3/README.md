# ValidationClinic · Validation in Wisej.NET · Module 3

Local lab build for **Module 3 · Built-in Rules and the Validation Extender**. The Validation extender replaces the repeated field handlers with reusable Required, Email, Integer, Telephone, Currency and Regex rule arrays.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

Requirements: .NET 10 SDK and NuGet access to `Wisej-4` 4.1.0. Use a Wisej development license or trial
as required by your installation. No database setup or external service is needed.

```bash
cd "Validation in Wisej.NET Course/Module 3/ValidationClinic"
dotnet run -f net10.0 --urls http://localhost:5903
```

Open <http://localhost:5903>. In Visual Studio, open `ValidationClinic.slnx` and press F5.
The project targets both `net10.0-windows` and `net10.0`, so the CLI needs `-f` when running.

## What to try

| Action | Expected result |
|---|---|
| Blank email; Save | Required is first, so the message is Email is required. |
| Type maria.example.com into email; Tab | The Email rule reports the format problem; a valid address clears it. |
| Use abc for age or X for customer code | Integer and Regex rules reject the values before a write. |
| Use 2125550123 as phone and 100.50 as credit limit | Telephone uses an explicit ten-digit mask; the currency rule accepts a formatted number for the current culture. |
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
| Design note and reproduction steps | [docs/RuleOrder.md](ValidationClinic/docs/RuleOrder.md) |

## Self-check answers

- Required belongs first so an empty email is explained as missing. Some optional format rules accept empty values, so rule behavior must be verified rather than assuming every format rule rejects blanks.

- The extender owns the event hooks and repetition. The application still chooses required fields, rule order, masks, wording and display targets.

- A two-field date rule must run on Save and after either date changes; a rule on just one field can miss edits to the other.

## Design notes

[Module note](ValidationClinic/docs/RuleOrder.md) records the API choices and failure behavior.

The sample uses `AutoValidate.EnableAllowFocusChange`: errors remain visible while the user reaches another
field to fix a related value. Cancel never causes validation. Save explicitly validates every relevant layer.
The in-memory repository is deliberately a storage stand-in; callers own validation, as in the lesson.
It checks name uniqueness and swaps the saved snapshot only after all repository checks pass.


## Verification

Both target frameworks are compiled. The capstone has domain tests and browser checks for its validation
and persistence paths. See the [course verification record](../README.md#verification) for the tested scope.
