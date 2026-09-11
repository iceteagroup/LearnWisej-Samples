# OperationsConsole · Mastering the Control Library · Module 2

Lab build for **Module 2 · Editors, Buttons, Validation, and Feedback**: the **Editors** section hosts a
`CustomerEditor` UserControl — six value-matched editors with a `Label` each, one `ErrorProvider` driven by four
named validators, a `ToolTip` per editor, Save / Reset / Validate with a busy state, and `Toast` / `AlertBox`
feedback. One `MessageBox` remains, for the only real decision (discarding unsaved changes).

The shell and the other five placeholders come from Module 1. This module fills `Sections/EditorsPage` and adds
`Editors/`, three models and `Services/CustomerService.cs`.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 2/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5702
```

Then open <http://localhost:5702> and click **Editors**.

## What to try

| Action | What you should see |
|---|---|
| Hover a field | the `ToolTip` says what the field expects |
| Fill in a valid customer (status **Active**, credit limit ≥ 1,000), **Validate** | Toast "Validation passed: all six fields are valid.", green status |
| **Save** | the three commands grey out, the loader spins on Save and "Saving…" shows for ~1.2 s, then Toast "Customer … saved as CUS-0001.", diagnostics `Record: CUS-0001` |
| Clear the name, press Tab | red mark beside `txtName`; you are not trapped in the field |
| Type `orders.acme` in Email, press Tab | red mark with the email-shape message; fix it and it clears |
| Status **Active**, credit limit **500**, **Save** | credit-limit message; switch to **Prospect** and the mark clears |
| Status **Active**, start date next month, **Save** | start-date message |
| Tick **Simulate service failure**, **Save** a valid customer | busy state, then a red AlertBox and red status; nothing is saved. Untick and save again to recover |
| Change a field, **Reset** | `MessageBox` "Discard the changes …?" — **No** keeps your typing, **Yes** restores the last saved values |

## Where things live

```
OperationsConsole/
├─ Editors/CustomerEditor.cs / .Designer.cs   the editor: validators, SaveAsync, Reset, busy state, feedback
├─ Sections/EditorsPage.cs / .Designer.cs     hosts customerEditor + the Simulate service failure switch
├─ Models/CustomerModel.cs, OptionItem.cs, ValidationResult.cs
├─ Services/CustomerService.cs                in-memory store, CUS-000N ids, latency, SimulateFailure, ValidateForSave
└─ docs/EditorDecisions.md                    deliverable: editor per field, validation layers, feedback channels
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| `CustomerEditor` in `Editors`, a `Label` beside every field, names `txtName`, `txtEmail`, `cboStatus`, `dtpStartDate`, `numCreditLimit`, `cboCustomerType` | `Editors/CustomerEditor.Designer.cs` |
| Matching editors, `Minimum` / `Maximum` / `MaxLength` / fixed items, display text separate from the stored key | designer properties + constructor (`MinDate` / `MaxDate`) + `Models/OptionItem.cs` |
| A `ToolTip` on each editor | `toolTip.SetToolTip(...)` × 6 at the end of `InitializeComponent()` |
| One `ErrorProvider`, named validators from `Validating`, `ValidateContent` returning a result that focuses the first invalid control | `ValidateRequiredName` / `ValidateEmail` / `ValidateCreditLimit` / `ValidateStartDate`, `ValidateContent()`, `Models/ValidationResult.cs` |
| `btnSave`, `btnReset`, `btnValidate`; Save = validate → service → state refresh, all three disabled with busy feedback | `CustomerEditor.SaveAsync()` and `SetBusy()` (`try` / `finally`) |
| Toast or AlertBox on completion, shell status updated, MessageBox only for a real confirmation | `ShowToast()`, `ReportValidationFailure()`, the `catch`, `btnReset_Click` |
| Show every path incl. a service failure | the editor itself + `chkSimulateFailure` on `EditorsPage` |
| The note explaining the decisions | `docs/EditorDecisions.md` |

## Self-check answers

- **Which validation errors can no longer happen because of the editor you chose, and which still need an `ErrorProvider` message?**
  Gone: over-long name or email (`MaxLength`), mixed-case email (`CharacterCasing.Lower`), a status or type outside
  the four keys (`DropDownList`), an unparseable or out-of-range date (`DateTimePicker` + `MinDate` / `MaxDate`), a
  negative, fractional or over-250,000 credit limit (`NumericUpDown`). Still validators: the **required** name, the
  **shape** of the email, and the rules that read another field (credit limit and start date against the status).
- **If the user clicks Save twice in one second, what does your screen do, and where is that guaranteed?**
  The second click does nothing. `SetBusy(true)` disables all three commands and turns on `btnSave.ShowLoader`, and
  `SetBusy(false)` sits in the `finally`. Behind that, `_busy` is checked on the first line of `SaveAsync()`, so a
  call that arrives anyway returns `false` without reaching the service.
- **What does the shell page know about how the email field is validated?**
  Nothing. `EditorsPage` only uses the editor's public surface and never names `txtEmail` or the `ErrorProvider`.
  If the rule moved to a service, only `CustomerEditor.ValidateEmail()` would change.

## Known simplifications

- `CustomerService` is in memory and per session; ids restart at `CUS-0001` for every session.
- Busy feedback is `btnSave.ShowLoader` plus a "Saving…" label: `ProgressBar` in Wisej.NET 4.1 has no marquee style.
