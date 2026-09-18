# ValidationClinic · Validation in Wisej.NET · Module 4

Local lab build for **Module 4 · Custom Validation Methods, Services and Rules**. A UI-independent `ContactValidator`, a `MinimumAgeValidationRule` and paired start/end date feedback separate field shape from business meaning.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

Requirements: .NET 10 SDK and NuGet access to `Wisej-4` 4.1.0. Use a Wisej development license or trial
as required by your installation. No database setup or external service is needed.

```bash
cd "Validation in Wisej.NET Course/Module 4/ValidationClinic"
dotnet run -f net10.0 --urls http://localhost:5904
```

Open <http://localhost:5904>. In Visual Studio, open `ValidationClinic.slnx` and press F5.
The project targets both `net10.0-windows` and `net10.0`, so the CLI needs `-f` when running.

## What to try

| Action | Expected result |
|---|---|
| Load valid values; move end date before start date | Both date pickers show the same message; focus can move to either picker to correct it. |
| Set birth date to tomorrow or less than 18 years ago | The custom age rule rejects it. The shared service repeats the age policy when invoked without a UI. |
| Use a birth date exactly 18 years ago | The boundary is accepted. |
| Correct the date order; Save | Both date messages clear; field and business stages pass before the write. |
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
| Design note and reproduction steps | [docs/CustomRules.md](ValidationClinic/docs/CustomRules.md) |

## Self-check answers

- The custom rule gives feedback when the picker is validated. The service protects callers without controls. This sample shares the age calculation between both.

- Clearing all errors after showing date errors removes those icons. The save sequence maps model errors first, then applies the date feedback.

- Call ContactValidator.Validate with a plain ContactEditModel. The capstone test project includes that test without creating a Wisej session.

## Design notes

[Module note](ValidationClinic/docs/CustomRules.md) records the API choices and failure behavior.

The sample uses `AutoValidate.EnableAllowFocusChange`: errors remain visible while the user reaches another
field to fix a related value. Cancel never causes validation. Save explicitly validates every relevant layer.
The in-memory repository is deliberately a storage stand-in; callers own validation, as in the lesson.
It checks name uniqueness and swaps the saved snapshot only after all repository checks pass.


## Verification

Both target frameworks are compiled. The capstone has domain tests and browser checks for its validation
and persistence paths. See the [course verification record](../README.md#verification) for the tested scope.
