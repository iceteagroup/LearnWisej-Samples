# OperationsConsole · Mastering the Control Library · Module 2

Local lab build for **Module 2 · Editors, Buttons, Validation, and Feedback**. It follows the lesson guide, the
lab / exam guide and the walkthrough video: the **Editors** section is no longer a placeholder but hosts a
`CustomerEditor` UserControl — six value-matched editors with a `Label` each, one `ErrorProvider` driven by four
named validators, `ToolTip` / `HelpTip` for guidance, Save / Reset / Validate with a real busy state, and
`Toast` / `AlertBox` feedback that never blocks the workflow. One `MessageBox` survives, for the only genuine
decision on the screen (discarding unsaved changes).

The course is cumulative: the shell (`MainPage` with its navigation / command / status / content areas, the Event
log card and the `Shell/` contract) and the other five section placeholders come from **Module 1** and are
unchanged here. This module only replaces the body of `Sections/EditorsPage` and adds `Editors/`, three models
and `Services/CustomerService.cs`.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 2/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5702
```

Then open <http://localhost:5702> and click **Editors** in the navigation. (Visual Studio: open
`OperationsConsole.slnx` in this folder, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Click **Editors** | ready state | the editor on a blank record: `Record: — (new customer, not saved yet)`, `Last saved: —`, `Idle — the three commands are enabled.`; log `EditorsPage → CustomerEditor hosted (Dock = Fill) · 6 editors, 1 ErrorProvider, 3 commands` |
| Hover a field / hover **Save** | guidance | the `ToolTip` says what the field expects ("One address for order confirmations, e.g. name@company.com…"), the `HelpTip` on Save says what the command does. No red marks: guidance and errors are separate channels |
| **Load sample** → **Validate** | success | Toast top-right "Validation passed: all six fields are valid.", green status, log `CustomerEditor.ValidateContent() → all six fields are valid` |
| **Load sample** → **Save** | success + progress | all three commands grey out and the loader spins on Save for ~1.2 s (`busy state on — btnSave.ShowLoader = true …`), then Toast "Customer ACME Manufacturing saved as CUS-0001.", green status, `Record: CUS-0001 · status ACT · type RSL`, `Last saved: HH:mm:ss · 1 record(s) in memory`, shell diagnostics `Record: CUS-0001`; log `CustomerService.SaveAsync(...) → started` … `→ committed CUS-0001` |
| Clear the name, press Tab | failure · required field | red mark beside `txtName`: "Enter the customer name — it appears on every order and invoice." — you are **not** trapped in the field |
| Type `orders.acme`, press Tab | failure · email shape | "Enter an email address such as name@company.com (one @ and a dot in the domain)."; fix it and tab out again — the mark clears |
| Status **Active**, credit limit **500**, **Save** | failure · business range | "An active customer needs a credit limit of at least 1,000 — raise the limit or set the status to Prospect."; switch the status to **Prospect** and the mark clears itself, because the rule no longer applies |
| Status **Active**, start date **next month**, **Save** | failure · date | "An active customer cannot start in the future — pick today or earlier, or set the status to Prospect." (the picker itself already refuses anything before 2000 or beyond +1 year) |
| **Load invalid sample** | four failures at once | four red marks (name, email, credit limit, start date), caret in `txtName`, amber status "4 fields need attention", four `✗ control — message` lines in the Event log, an amber AlertBox — the service is never called |
| Tick **Simulate service failure**, **Load sample**, **Save** | service failure | 1.2 s busy state, then a red AlertBox "The customer could not be saved. Nothing was changed — please try again in a moment.", red status; the Event log (only) gets `✗ CustomerService.SaveAsync threw InvalidOperationException` + the message. `Record:` stays `—`: nothing partial was written |
| Untick it, **Save** again | recovery | log `CustomerService.SimulateFailure = false — Save works again (recovery)`, the record is committed, Toast appears |
| **Load sample**, **Save twice** | duplicate submit | log `btnSave ignored — a save is already running (busy state)` and `first call returned True, second call returned False — one record, not two (1 in memory)` |
| Change a field, **Reset** | blocking decision | the only `MessageBox` of the module: "Discard the changes and go back to the last saved values?" — **No** keeps your typing (amber status), **Yes** restores the last saved values and clears every error mark |
| **Reset** without changing anything | no confirmation | log `btnReset → nothing was changed, no confirmation needed`, values restored silently |
| Shell **Refresh** with a saved record | shell command | `EditorsPage.RefreshSection() → reloaded CUS-0001 from CustomerService`, the editor is back to the stored values |

## Where things live

```
OperationsConsole/
├─ Editors/                              ← new in Module 2
│  ├─ CustomerEditor.cs                  the reusable editor: validators, SaveAsync, Reset, busy state, feedback
│  └─ CustomerEditor.Designer.cs         6 labelled editors + ErrorProvider / ToolTip / HelpTip + 3 commands
├─ Sections/EditorsPage.cs / .Designer.cs   ← rebuilt: hosts customerEditor (Dock = Fill) + the command row
├─ Models/
│  ├─ CustomerModel.cs                   ← new: the typed value (Clone, HasSameValues for IsDirty)
│  ├─ OptionItem.cs                      ← new: Key (stored) + Text (displayed) for the two ComboBoxes
│  ├─ ValidationResult.cs                ← new: IsValid, (ControlName, Message) list, FirstInvalid, Summary
│  ├─ SectionKey.cs / SectionInfo.cs     (Module 1)
├─ Services/
│  ├─ CustomerService.cs                 ← new: in-memory store, CUS-000N ids, 1.2 s latency, SimulateFailure,
│  │                                       service-level validation, option lists
│  └─ SectionCatalog.cs                  (Module 1)
├─ Shell/  IConsoleShell.cs, ConsoleLog.cs, ISection.cs      (Module 1 — unchanged, and never changes)
├─ MainPage.cs / .Designer.cs            (Module 1 — the shell; Module 3 rebuilds the frame)
├─ Sections/ Layouts, ListsTrees, DataGridView, Dashboard, Widgets   (Module 1 placeholders, untouched)
├─ docs/
│  ├─ EditorDecisions.md                 ← new deliverable: editor per field, the six layers, feedback table, Evidence
│  └─ ControlSelection.md                (Module 1 — untouched)
└─ ClientProfiles.json, Program.cs, Startup.cs, Default.*, Web.config   (Module 1)
```

## Lab steps → where in the code

| Lab step (`m2.json`) | Where |
|---|---|
| 1 · Open the Module 1 solution, confirm the empty Editors placeholder | this folder is the Module 1 solution with the Editors page filled in; every other file is byte-identical |
| 2 · Lab goal: a CustomerEditor with six labelled fields, ErrorProvider messages, Save / Reset / Validate with busy behaviour, Toast or AlertBox | `Editors/CustomerEditor.cs` + `.Designer.cs`, hosted by `Sections/EditorsPage` |
| 3 · `CustomerEditor` UserControl in an `Editors` folder, a `Label` beside every field, names `txtName`, `txtEmail`, `cboStatus`, `dtpStartDate`, `numCreditLimit`, `cboCustomerType` | `Editors/CustomerEditor.Designer.cs` — `lblName`/`txtName`, `lblEmail`/`txtEmail`, `lblStatus`/`cboStatus`, `lblCustomerType`/`cboCustomerType`, `lblStartDate`/`dtpStartDate`, `lblCreditLimit`/`numCreditLimit` |
| 4 · The editor that matches each value; `Minimum`, `Maximum`, `MaxLength` and fixed items; display text separate from the stored key | `.Designer.cs` (`MaxLength 80` / `120`, `CharacterCasing.Lower`, `DropDownStyle = DropDownList`, `Minimum 0` / `Maximum 250000` / `Increment 1000` / `DecimalPlaces 0`, `Format = Short`) + `CustomerEditor` ctor (`MinDate` 2000-01-01, `MaxDate` today + 1 year) + `Models/OptionItem.cs` (`Key` ≠ `Text`) and `CustomerEditor.SelectedKey()` |
| 5 · A `ToolTip` (or `HelpTip`) on each editor, so guidance and errors stay on separate channels | end of `InitializeComponent()` in `.Designer.cs` — `toolTip.SetToolTip(...)` × 6, `helpTip.SetHelpTip(...)` on `btnSave` / `btnReset` |
| 6 · One `ErrorProvider`, named validators, called from each `Validating` and all from `ValidateContent` returning a result object that focuses the first invalid control | `CustomerEditor.ValidateRequiredName` / `ValidateEmail` / `ValidateCreditLimit` / `ValidateStartDate`, the four `*_Validating` handlers, `ValidateContent()`, `Models/ValidationResult.cs` (`FocusFirstInvalid()`) |
| 7 · `btnSave`, `btnReset`, `btnValidate`; Save = ValidateContent → service call → state refresh; all three disabled with busy feedback while it runs | `CustomerEditor.SaveAsync()` (validate → `ValidateForSave` → `SaveAsync` → `RefreshState` → notify) and `SetBusy()` (`Enabled = false` × 3 + `btnSave.ShowLoader`, in `try` / `finally`) |
| 8 · Toast or AlertBox when validation or save completes, shell status updated, MessageBox only for a genuine confirmation | `ShowToast()` (success), `ReportValidationFailure()` and the `catch` (AlertBox), `ConsoleLog.Status(...)` on every outcome, `btnReset_Click` → `MessageBox.ShowAsync` when `IsDirty` |
| 9 · Show every path: valid save, required field, malformed email, credit limit out of range, invalid date, service failure — all with actionable feedback and no partial state | the command row in `Sections/EditorsPage.Designer.cs` (`btnLoadSample`, `btnLoadInvalidSample`, `chkSimulateFailure`, `btnSaveTwice`) + the "What to try" table above; nothing is written when validation or the service fails (`CustomerService.SaveAsync` throws **before** touching the store) |
| 10 · Host the control on the Editors page, run it, write the note explaining the decisions | `Sections/EditorsPage` (`pnlEditorHost`, `customerEditor.Dock = Fill`) and `docs/EditorDecisions.md` |

## Self-check answers (lab / exam guide)

- **Which validation errors can no longer happen at all because of the editor you chose, and which still need an
  `ErrorProvider` message?**
  Gone because of the control: a name over 80 characters or an email over 120 (`MaxLength`); a mixed-case email
  (`CharacterCasing.Lower`); a status or type outside the four keys (`ComboBox` + `DropDownStyle = DropDownList`, so
  the user cannot type at all); an unparseable or out-of-era date (`DateTimePicker` returns a `DateTime`, and
  `MinDate` 2000-01-01 / `MaxDate` today + 1 year bound it); a negative, fractional or over-250,000 credit limit
  (`NumericUpDown` returns a `decimal` inside `Minimum` / `Maximum` with `DecimalPlaces = 0`). Still validators,
  because no single control can express them: **required** name (empty is a legal string), the **shape** of an email,
  and the two rules that read *another* field — a credit limit of at least 1,000 for an Active customer and at most
  5,000 for a Prospect, and a start date that may not be in the future for an Active customer. The editors remove
  the shape errors; the validators carry the business rules.

- **If the user clicks Save twice in one second, what does your screen do, and where in the code is that
  guaranteed?**
  The second click does nothing and nothing is saved twice. Two things guarantee it, both in
  `Editors/CustomerEditor.cs`. Visibly, `SetBusy(true)` at the start of `SaveAsync()` sets
  `btnSave.Enabled = btnReset.Enabled = btnValidate.Enabled = false` and `btnSave.ShowLoader = true`, and the matching
  `SetBusy(false)` sits in the `finally` block, so the buttons come back whether the call succeeded, was rejected or
  threw. Behind that, the `_busy` field is checked on the first line of `SaveAsync()`: a call that arrives anyway —
  from code, from a click already in flight, or from the page's **Save twice** button — logs
  `btnSave ignored — a save is already running (busy state)` and returns `false` without reaching the service. The
  page's `btnSaveTwice_Click` demonstrates exactly that and prints both return values to the Event log.

- **What does the shell page know about how the email field is validated, and what would have to change if the rule
  moved to a service?**
  Nothing. `Sections/EditorsPage` only uses the editor's public surface — `LoadCustomer`, `ValidateContent`,
  `SaveAsync`, `Reset`, `IsDirty`, `Customer`, `LastValidation` and the `Saved` / `ValidationFailed` events. It never
  names `txtEmail`, never touches the `ErrorProvider`, and reads failures as a `ValidationResult` of
  `(ControlName, Message)` pairs. If the email rule moved to a service, the only file that changes is
  `CustomerEditor.ValidateEmail()`: it would call `Service.CheckEmail(...)` (or `ValidateForSave`, where the
  uniqueness rule already lives) instead of `LooksLikeEmail`, and still do the one thing its contract promises — set
  or clear one message on `txtEmail`. `EditorsPage`, `MainPage` and `ConsoleLog` compile untouched. The one caveat:
  a service call is asynchronous, so a rule that must run on `Validating` either needs a cached answer or has to move
  to the Save path, where `await` is already allowed.

## Known simplifications / unverified

- **`CustomerService` is in memory and per session.** Ids restart at `CUS-0001` for every browser session; there is
  no database, so the sixth enforcement layer of the reading (database constraints) is named in
  `docs/EditorDecisions.md` but not implemented.
- **The service-level rule is uniqueness of the email address.** It is reachable (save one customer, then load the
  sample again and save a second new record with the same email) but it is not one of the paths the lab lists, so it
  has no button of its own.
- **Busy feedback is `btnSave.ShowLoader`, not a marquee `ProgressBar`.** `Wisej.Web.ProgressBar` in 4.1.0 has no
  `Style` / `ProgressBarStyle.Marquee` property (checked in `Wisej.Framework.xml`), and the save has no measurable
  progress, so the loader on the primary command plus `lblBusy` carries the state.
- **Used from the XML docs only, not yet executed in the browser** (please check these while reviewing):
  `ErrorProvider.SetError` / `GetError` / `Clear` and `ContainerControl` + `BlinkStyle = ErrorBlinkStyle.NeverBlink`;
  `ToolTip.SetToolTip` and `HelpTip.SetHelpTip` as designer extenders; `Control.ShowLoader` on a `Button`;
  `TextBox.CharacterCasing` / `Watermark`; `ComboBox.DataSource` + `DisplayMember` / `ValueMember` / `SelectedValue`
  with a plain `List<OptionItem>`; `DateTimePicker.MinDate` / `MaxDate` / `Format = Short`; `NumericUpDown.Increment`
  / `DecimalPlaces`; `Control.Validating` as `System.ComponentModel.CancelEventHandler`; `btnReset.CausesValidation
  = false`; `MessageBox.ShowAsync(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question)` inside an
  `async void` handler.
- **`LoadCustomer`, not `Load`.** `Wisej.Web.UserControl` already has a `Load` **event**, so the public method that
  fills the editors is named `LoadCustomer`; the lab's wording ("`Load(CustomerModel)`") would have shadowed it.
- **Reset restores the last *saved* values**, i.e. the baseline set by the last `LoadCustomer` or the last successful
  save — not the values from before the last "Load sample" click.
