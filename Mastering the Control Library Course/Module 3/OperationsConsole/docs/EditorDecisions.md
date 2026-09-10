# EditorDecisions.md — Operations Console, Module 2

The lab deliverable for **Module 2 · Editors, Buttons, Validation, and Feedback**: which editor each value got and
why, how the six-step validation flow is wired, which control carries which kind of feedback, and what the running
app shows on every path. It is the second page of the team's control-selection guide (after
[`ControlSelection.md`](ControlSelection.md), which Module 1 wrote and this module does not touch).

Rule applied everywhere, same as Module 1: **choose the simplest native control that expresses the user's task** —
and, for an input screen, *the editor whose type already excludes impossible input*.

## One rule, six layers

A single business rule ("a credit limit is a number between 0 and 250,000") can be enforced at editor properties,
validation extenders, event handlers, data binding, service validation and the database. The UI's job is to prevent
the obvious mistakes and explain the rest — it is never the only guard. This module implements the first four
layers and names the two it does not have:

| Layer | Where in this sample |
|---|---|
| editor properties | `Editors/CustomerEditor.Designer.cs` — `MaxLength`, `CharacterCasing`, `Minimum` / `Maximum` / `Increment`, `MinDate` / `MaxDate`, `DropDownStyle = DropDownList` |
| typed values instead of parsing | `CustomerEditor.ReadFromEditors()` — `dtpStartDate.Value`, `numCreditLimit.Value`, the ComboBox key; nothing calls `DateTime.Parse` or `decimal.Parse` |
| field validation | each editor's `Validating` event → one named validator (`ValidateRequiredName`, `ValidateEmail`, `ValidateCreditLimit`, `ValidateStartDate`) |
| form-level validation | `CustomerEditor.ValidateContent()` — runs every validator, marks every offending control, focuses the first |
| service-level validation | `Services/CustomerService.ValidateForSave()` — the email must not already belong to another record; only the store can know that |
| database constraints | not in this sample (in-memory store); this is the layer that must exist in production even when the UI is perfect |

## The editor per field

| Field | Control | Name | Properties that make bad input impossible | Why not a plain `TextBox` |
|---|---|---|---|---|
| Customer name | `TextBox` | `txtName` | `MaxLength = 80`, `Watermark` as an example (never instead of the label) | It *is* ordinary text — this one is a TextBox on purpose. `Multiline` would be wrong: the name is not a paragraph. |
| Email | `TextBox` | `txtEmail` | `MaxLength = 120`, `CharacterCasing = Lower` (normalised while typing) | An email has no predictable shape to mask; `MaskedTextBox` would fight the user. The shape rule stays a validator. |
| Status | `ComboBox` | `cboStatus` | `DropDownStyle = DropDownList`, `DataSource` = `OptionItem` list, `DisplayMember = "Text"`, `ValueMember = "Key"` | Free text would let "active ", "Aktiv" and "ACTIVE" into the store. `DropDownList` means the user cannot type at all. |
| Customer type | `ComboBox` | `cboCustomerType` | same, keys `DIR` / `RSL` / `OEM` / `GOV` | Same reason; a `RadioButton` group would also work for four fixed values but costs four times the vertical space. |
| Start date | `DateTimePicker` | `dtpStartDate` | `Format = Short`, `MinDate = 2000-01-01`, `MaxDate = today + 1 year` | "03/04/2026" is 3 April or 4 March depending on the reader. The picker returns a `DateTime`; nothing parses text. |
| Credit limit | `NumericUpDown` | `numCreditLimit` | `Minimum = 0`, `Maximum = 250000`, `Increment = 1000`, `DecimalPlaces = 0` | "1.500" is 1.5 or 1500 depending on the culture. The spin box returns a `decimal` inside a range the control owns. |

**Display text vs stored key.** `OptionItem` has `Key` (what is persisted: `"ACT"`) and `Text` (what is read:
`"Active"`). `CustomerModel.StatusKey` only ever holds the key, and `CustomerEditor.SelectedKey(combo)` is the one
place that reads a selection — so "Active" on screen can never become the value in the record.

**Where a mask or a typed text box would have been right instead:** a VAT number or a contract reference
(`MaskedTextBox`, the shape is predictable) and a field whose whole purpose is parse-and-format round tripping
(`TypedTextBox`). Neither of the six values here is one of those.

## Which errors disappeared, and which still need a message

| Cannot happen any more (the control prevents it) | Still needs an `ErrorProvider` message |
|---|---|
| A name longer than 80 characters (`MaxLength`) | An **empty** name — `MaxLength` cannot express "required" |
| An email longer than 120 characters, or in mixed case | A **malformed** email — one `@`, a dot in the domain |
| A status or type that is not one of the four keys (`DropDownList`) | — |
| A date before 2000 or more than a year ahead (`MinDate` / `MaxDate`) | A start date **in the future for an Active customer** (`MaxDate` deliberately allows +1 year, so this rule is reachable) |
| A credit limit below 0, above 250,000, or with decimals | A limit **below 1,000 for an Active customer** or **above 5,000 for a Prospect** — the range depends on another field, which no single control can express |

That table is the honest answer to "why do I still need validators if the editors are typed": the editors remove the
*shape* errors, the validators carry the *business* rules.

## Validation flow (the six steps, in code order)

1. **Editor properties** — `Editors/CustomerEditor.Designer.cs` + the two range properties in the constructor
   (`MinDate` / `MaxDate` depend on today, so they cannot be designer constants).
2. **Typed values** — `ReadFromEditors()` builds a `CustomerModel` from `.Value` / `.Text` / the selected key.
3. **Field validation** — `txtName_Validating`, `txtEmail_Validating`, `numCreditLimit_Validating`,
   `dtpStartDate_Validating`, each one line: call the named validator. A validator sets **or clears** exactly one
   message on exactly one control (`errorProvider.SetError(ctl, msg)` / `SetError(ctl, "")`). **No validator ever sets
   `e.Cancel = true`** — leaving a field that is not finished yet is normal data entry, not an error to trap the user in.
   `cboStatus.SelectedIndexChanged` re-runs the two rules that read the status, so switching Active → Prospect clears
   a mark that is no longer true.
4. **Form then service** — `btnSave_Click` → `SaveAsync()` → `ValidateContent()` (form) → `CustomerService.ValidateForSave()`
   (service) → `CustomerService.SaveAsync()`.
5. **On failure** — the user stays on the screen, every offending control keeps its own message,
   `ValidationResult.FocusFirstInvalid()` puts the caret in the first one, and one `AlertBox` says how many fields
   need attention. Nothing partial is written: the service is not called at all.
6. **On success** — a `Toast` (no OK button), the status area turns green, the record id and the save stamp appear in
   the editor's state row, and `ConsoleLog.Record(id)` fills the shell's diagnostic panel.

`ValidateContent` is deliberately **not** called `Validate`: `ContainerControl` already has validation members, and a
method named `Validate()` on a UserControl reads as "the framework's one" to the next developer.

## Feedback channels — one role each

| Channel | Control | Used for | Never used for |
|---|---|---|---|
| Guidance | `ToolTip` on every editor (`toolTip.SetToolTip(txtEmail, "…")`) | What the field expects, in one sentence | Errors — a tooltip disappears when the pointer leaves |
| Guidance (commands) | `HelpTip` on `btnSave` / `btnReset` | What the command will do to the user's data | Field-level messages |
| Field error | one `ErrorProvider` | The control that is wrong + what to change | Workflow status ("saved", "loading") |
| Blocking decision | `MessageBox.ShowAsync` — **exactly one** in this module: Reset while the form is dirty | A question only the user can answer | "Saved successfully" — that would stop the workflow for nothing |
| Non-blocking success | `Toast` (TopRight, 3 s, `icon-check`) | Save committed, validation passed | Errors the user must act on |
| Non-blocking problem | `AlertBox` (TopRight, 4–5 s, Warning / Error) | Validation failed, the service refused, the service threw | Anything with a stack trace in it |
| Persistent state | shell `statusLabel` via `ConsoleLog.Status` | The current state of the screen, in colour | Transient chatter |
| Diagnostics | shell Event log via `ConsoleLog.Add` | Every action, every service call, the exception type and message | Anything the user must read to continue |

Exception text never reaches the user: `SaveAsync`'s `catch` writes `ex.GetType().Name` and `ex.Message` to the Event
log and shows one friendly sentence.

## The command model

- **Primary command** — `btnSave`. Placed last (rightmost) in the row, it is the one that changes the world.
- **Secondary commands** — `btnValidate` (check without saving) and `btnReset` (undo to the last saved values).
  `btnReset.CausesValidation = false`, so pressing it from inside a half-typed field does not mark that field on the
  way out. From Module 3 on, secondary commands of this kind belong in the `ToolBar`.
- **State controls, not commands** — `chkSimulateFailure` on the page: a CheckBox sets state, it never fires an action.
- **Busy state** — `SetBusy(true)` disables all three commands and sets `btnSave.ShowLoader = true` inside a
  `try` / `finally`, and a `_busy` flag rejects a call that arrives anyway. Disabled buttons and busy feedback are
  reliability, not decoration: the "Save twice" button on the page proves it.

## Public surface of `CustomerEditor`

```csharp
public CustomerService Service { get; set; }        // the back end the host owns
public CustomerModel Customer { get; }              // the typed value the editors hold
public bool IsDirty { get; }
public ValidationResult LastValidation { get; }
public void LoadCustomer(CustomerModel customer);   // named LoadCustomer: UserControl.Load is an event
public ValidationResult ValidateContent();
public Task<bool> SaveAsync();
public void Reset();
public event EventHandler Saved;
public event EventHandler ValidationFailed;
```

Every child control is `private`. `Sections/EditorsPage` never touches `txtEmail`, `cboStatus` or the
`ErrorProvider` — which is exactly what makes "move the email rule into a service" a change inside one file.

## Evidence (what the running app shows)

Run `dotnet run -f net10.0 --urls http://localhost:5702`, click **Editors** in the navigation.

- **Ready** — the editor opens on a blank record: `Record: — (new customer, not saved yet)`, `Last saved: —`,
  `Idle — the three commands are enabled.`, log line `EditorsPage → CustomerEditor hosted (Dock = Fill) · 6 editors, 1 ErrorProvider, 3 commands`.
- **Guidance** — hovering `txtEmail` shows "One address for order confirmations, e.g. name@company.com. Stored in
  lower case."; hovering **Save** shows the HelpTip. No red marks appear from hovering: guidance and errors are
  separate channels.
- **Valid save (success + progress)** — **Load sample** → ACME Manufacturing, Active, Reseller, 25,000, start date
  eight months ago; **Save** → all three buttons grey out, the loader spins on Save for ~1.2 s
  (`busy state on — btnSave.ShowLoader = true …`), then a Toast top-right "Customer ACME Manufacturing saved as
  CUS-0001.", green status, `Record: CUS-0001 · status ACT · type RSL`, `Last saved: HH:mm:ss · 1 record(s) in memory`,
  and the shell diagnostics show `Record: CUS-0001`.
- **Required field** — clear the name, tab out: a red mark appears beside `txtName` with "Enter the customer name — it
  appears on every order and invoice."; **Save** refuses, focuses `txtName`, shows an amber AlertBox "1 field needs
  attention…", and the service is never called.
- **Malformed email** — type `orders.acme` and tab out: "Enter an email address such as name@company.com (one @ and a
  dot in the domain)." Fix it → the mark disappears on the next `Validating`.
- **Credit limit outside the business range** — status Active with 500 → "An active customer needs a credit limit of at
  least 1,000 — raise the limit or set the status to Prospect."; switch the status to Prospect and the mark clears
  itself (the `SelectedIndexChanged` re-run), because the rule no longer applies.
- **Invalid date** — status Active with a start date next month → "An active customer cannot start in the future — pick
  today or earlier, or set the status to Prospect." The picker still refuses anything before 2000 or beyond +1 year, so
  that class of error never reaches the validator.
- **Four marks at once** — **Load invalid sample** loads the record and runs `ValidateContent()`: four red marks
  (name, email, credit limit, start date), the caret in `txtName`, amber status "4 fields need attention", and four
  `✗ control — message` lines in the Event log.
- **Service failure** — tick **Simulate service failure**, load the valid sample, **Save**: the busy state runs for
  1.2 s, then a red AlertBox "The customer could not be saved. Nothing was changed — please try again in a moment.",
  red status, and the Event log carries `✗ CustomerService.SaveAsync threw InvalidOperationException` +
  `The customer service is not reachable (simulated failure).` The record id stays `—`: nothing partial was written.
- **Recovery** — untick the checkbox ("CustomerService.SimulateFailure = false — Save works again (recovery)") and
  **Save** again: the record is committed and the Toast appears.
- **Duplicate submit** — **Save twice** calls `SaveAsync()` twice 200 ms apart: the log shows
  `btnSave ignored — a save is already running (busy state)` and
  `first call returned True, second call returned False — one record, not two (1 in memory)`.
- **Reset with a dirty form** — change the name and press **Reset**: a `MessageBox` asks "Discard the changes and go
  back to the last saved values?" — the only blocking dialog in the module. **No** restores the values and leaves an
  amber status; **Yes** puts the last saved values back and clears every error mark.
- **Refresh (shell command)** — with a saved record, the shell's Refresh reloads it from `CustomerService`
  (`EditorsPage.RefreshSection() → reloaded CUS-0001 from CustomerService`); with nothing saved it returns the editor
  to a blank record.
