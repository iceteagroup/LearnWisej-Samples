# Validation in Wisej.NET · lab samples

Seven runnable Wisej.NET 4 applications following the course specifications, lesson guides, lab steps and
walkthroughs. Each module is a cumulative snapshot of **ValidationClinic**, with a contacts grid, a customer
intake form, a per-session in-memory repository and a live server trace. The final module is the capstone.

| Module | Folder | What it builds | Run |
|---|---|---|---|
| 1 · Validation Fundamentals | `Module 1` | The customer intake form introduces `Validating`, `Validated`, `e.Cancel`, `AutoValidate`, a working Cancel button and the `ValidateChildren()` save guard. | `http://localhost:5901` |
| 2 · Programmatic Validation and ErrorProvider Feedback | `Module 2` | The intake form adds pure `FieldValidation` results, one `ErrorProvider`, per-field clearing, a visible summary and an `InvalidMessage` comparison on the customer code. | `http://localhost:5902` |
| 3 · Built-in Rules and the Validation Extender | `Module 3` | The Validation extender replaces the repeated field handlers with reusable Required, Email, Integer, Telephone, Currency and Regex rule arrays. | `http://localhost:5903` |
| 4 · Custom Validation Methods, Services and Rules | `Module 4` | A UI-independent `ContactValidator`, a `MinimumAgeValidationRule` and paired start/end date feedback separate field shape from business meaning. | `http://localhost:5904` |
| 5 · Data-Bound Validation and Model Errors | `Module 5` | The form binds to an annotated model, commits pending edits before validation and maps model errors to fields. A separate dialog demonstrates automatic `IDataErrorInfo` feedback and switching the provider source. | `http://localhost:5905` |
| 6 · DataGridView Cell Validation and Row Errors | `Module 6` | The contacts grid rejects uncommitted bad email/age values, explains conversion failures, marks Closed rows without a date and checks every row before saving the working copy. | `http://localhost:5906` |
| 7 · Production Validation UX and Capstone | `Module 7` | The capstone combines binding, field rules, annotations, business rules and the contacts grid in one ordered Save pipeline. `SummaryErrorProvider` keeps extender, icon and summary feedback together. | `http://localhost:5907` |

## Run a module

Requirements: .NET 10 SDK, NuGet access to `Wisej-4` 4.1.0 and a Wisej development license or trial.
Open a module's `ValidationClinic.slnx` in Visual Studio and press F5, or run:

```bash
cd "Validation in Wisej.NET Course/Module 7/ValidationClinic"
dotnet run -f net10.0 --urls http://localhost:5907
```

Each project targets `net10.0-windows` and `net10.0`. Ports 5901-5907 let the snapshots run side by side.
The capstone tests run from `Module 7` with `dotnet test ValidationClinic.Tests/ValidationClinic.Tests.csproj`.
Nothing requires a database, a third-party service or a deployment.

## Conventions

- Designer-style `.Designer.cs` files hold the named controls; short event handlers and helpers live in code-behind.
- Open customer intake to work through the form rules. Load valid/invalid values makes each path reproducible.
- The trace records validation stages; saved-contact and write counts distinguish editing from persistence.
- Cancel bypasses validation. EnableAllowFocusChange lets the user reach related fields; Save still stops.
- Module 5 adds a separate IDataErrorInfo comparison dialog. Module 6 adds Save grid; Module 7 saves the
  intake and grid together only after all stages pass. Cancel intake before correcting a grid row.
- Grid rows and form models are working copies. A failed validation or repository write never mutates the saved snapshot.
- The annotated nullable Age model accepts null by itself; the form's Required rule and the grid's integer rule
  supply the required-field policy. Non-UI callers must choose that policy and run validators explicitly.

## `_template`

The working Module 1 baseline, with port 5900, ready to extend. `COOKBOOK.md` records framework behavior,
event ordering and the binding/grid issues encountered during verification.

## Verification

All seven module solutions and the template compile for both target frameworks. The capstone's 20 automated
tests exercise domain boundaries and atomic repository behavior without a browser or Wisej session.
Browser verification covers the capstone Save pipeline, rejected input, valid save, correction and recovery;
the module READMEs and capstone test matrix provide the remaining manual exercises. No load, deployment,
screen-reader or cross-browser certification is claimed.
