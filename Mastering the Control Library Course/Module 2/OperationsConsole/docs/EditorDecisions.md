# EditorDecisions.md — Operations Console, Module 2

The lab deliverable for **Module 2 · Editors, Buttons, Validation, and Feedback**: which editor each value got and
why, how the validation flow is wired, which control carries which kind of feedback, and what the running app shows
on every path. It is the second page of the team's control-selection guide (after [`ControlSelection.md`](ControlSelection.md)).

Rule applied everywhere: **choose the simplest native control that expresses the user's task** — and, for an input
screen, *the editor whose type already excludes impossible input*.

## One rule, six layers

| Layer | Where in this sample |
|---|---|
| editor properties | `Editors/CustomerEditor.Designer.cs` — `MaxLength`, `CharacterCasing`, `Minimum` / `Maximum` / `Increment`, `MinDate` / `MaxDate`, `DropDownStyle = DropDownList` |
| typed values instead of parsing | `CustomerEditor.ReadFromEditors()` — `dtpStartDate.Value`, `numCreditLimit.Value`, the ComboBox key |
| field validation | each editor's `Validating` event → one named validator (`ValidateRequiredName`, `ValidateEmail`, `ValidateCreditLimit`, `ValidateStartDate`) |
| form-level validation | `CustomerEditor.ValidateContent()` — runs every validator, marks every offending control, focuses the first |
| service-level validation | `Services/CustomerService.ValidateForSave()` — the email must not already belong to another record |
| database constraints | not in this sample (in-memory store); the layer that must exist in production even when the UI is perfect |

## The editor per field

| Field | Control | Name | Properties that make bad input impossible | Why not a plain `TextBox` |
|---|---|---|---|---|
| Customer name | `TextBox` | `txtName` | `MaxLength = 80`, `Watermark` as an example (never instead of the label) | It *is* ordinary text — a TextBox on purpose. |
| Email | `TextBox` | `txtEmail` | `MaxLength = 120`, `CharacterCasing = Lower` | An email has no predictable shape to mask; the shape rule stays a validator. |
| Status | `ComboBox` | `cboStatus` | `DropDownStyle = DropDownList`, `DataSource` = `OptionItem` list, `DisplayMember = "Text"`, `ValueMember = "Key"` | Free text would let "active ", "Aktiv" and "ACTIVE" into the store. |
| Customer type | `ComboBox` | `cboCustomerType` | same, keys `DIR` / `RSL` / `OEM` / `GOV` | Same reason. |
| Start date | `DateTimePicker` | `dtpStartDate` | `Format = Short`, `MinDate = 2000-01-01`, `MaxDate = today + 1 year` | "03/04/2026" is 3 April or 4 March depending on the reader. |
| Credit limit | `NumericUpDown` | `numCreditLimit` | `Minimum = 0`, `Maximum = 250000`, `Increment = 1000`, `DecimalPlaces = 0` | "1.500" is 1.5 or 1500 depending on the culture. |

**Display text vs stored key.** `OptionItem` has `Key` (persisted: `"ACT"`) and `Text` (shown: `"Active"`).
`CustomerModel.StatusKey` only ever holds the key, and `CustomerEditor.SelectedKey(combo)` is the one place that
reads a selection.

## Which errors disappeared, and which still need a message

| Cannot happen any more (the control prevents it) | Still needs an `ErrorProvider` message |
|---|---|
| A name longer than 80 characters (`MaxLength`) | An **empty** name — `MaxLength` cannot express "required" |
| An email longer than 120 characters, or in mixed case | A **malformed** email — one `@`, a dot in the domain |
| A status or type that is not one of the four keys (`DropDownList`) | — |
| A date before 2000 or more than a year ahead (`MinDate` / `MaxDate`) | A start date **in the future for an Active customer** |
| A credit limit below 0, above 250,000, or with decimals | A limit **below 1,000 for an Active customer** or **above 5,000 for a Prospect** |

## Validation flow

1. **Editor properties** — the designer, plus `MinDate` / `MaxDate` in the constructor (they depend on today).
2. **Typed values** — `ReadFromEditors()` builds a `CustomerModel` from `.Value` / `.Text` / the selected key.
3. **Field validation** — each `*_Validating` handler calls its named validator, which sets **or clears** one message
   on one control. No validator sets `e.Cancel = true`: the user may leave a field that is not finished yet.
   `cboStatus.SelectedIndexChanged` re-runs the two rules that read the status.
4. **Form then service** — `btnSave_Click` → `SaveAsync()` → `ValidateContent()` → `CustomerService.ValidateForSave()`
   → `CustomerService.SaveAsync()`.
5. **On failure** — the user stays on the screen, every offending control keeps its own message, the caret goes to
   the first one, and one `AlertBox` says how many fields need attention. Nothing is written.
6. **On success** — a `Toast` (no OK button), the status area turns green and the shell's diagnostics show the record id.

## Feedback channels — one role each

| Channel | Control | Used for | Never used for |
|---|---|---|---|
| Guidance | `ToolTip` on every editor | What the field expects, in one sentence | Errors |
| Field error | one `ErrorProvider` | The control that is wrong + what to change | Workflow status |
| Blocking decision | `MessageBox.ShowAsync` — only for Reset while the form is dirty | A question only the user can answer | "Saved successfully" |
| Non-blocking success | `Toast` (TopRight, 3 s, `icon-check`) | Save committed, validation passed | Errors the user must act on |
| Non-blocking problem | `AlertBox` (TopRight, 4–5 s) | Validation failed, the service refused, the service threw | Exception text |
| Persistent state | shell `statusLabel` via `ShellStatus.Show` | The current state of the screen, in colour | Transient chatter |

## The command model

- **Save, Reset, Validate** sit in one row on the editor, Save first as the primary command.
- `btnReset.CausesValidation = false`, so pressing it from a half-typed field does not mark that field on the way out.
- **Busy state** — `SetBusy(true)` disables all three commands, sets `btnSave.ShowLoader = true` and shows
  "Saving…" inside a `try` / `finally`; a `_busy` flag rejects a second call that arrives anyway.
- **State control, not a command** — `chkSimulateFailure` on the page sets the service state; it never fires an action.

## Public surface of `CustomerEditor`

```csharp
public CustomerService Service { get; set; }        // the back end the host owns
public CustomerModel Customer { get; }              // the typed value the editors hold
public bool IsDirty { get; }
public void LoadCustomer(CustomerModel customer);   // named LoadCustomer: UserControl.Load is an event
public ValidationResult ValidateContent();
public Task<bool> SaveAsync();
public void Reset();
```

Every child control is `private`; `Sections/EditorsPage` never touches `txtEmail`, `cboStatus` or the `ErrorProvider`.

## Evidence (what the running app shows)

- **Valid save** — fill in a valid customer and press **Save**: all three buttons grey out and the loader spins on
  Save for ~1.2 s, then a Toast "Customer … saved as CUS-0001.", green status, and the diagnostics show `Record: CUS-0001`.
- **Required field** — clear the name and tab out: a red mark with "Enter the customer name — it appears on every
  order and invoice."; **Save** refuses, focuses `txtName` and shows an amber AlertBox.
- **Malformed email** — type `orders.acme` and tab out: "Enter an email address such as name@company.com …". Fix it
  and the mark disappears.
- **Credit limit outside the business range** — status Active with 500 → "An active customer needs a credit limit of
  at least 1,000 …"; switch the status to Prospect and the mark clears itself.
- **Invalid date** — status Active with a start date next month → "An active customer cannot start in the future …".
- **Service failure** — tick **Simulate service failure** and **Save** a valid customer: after the busy state, a red
  AlertBox "The customer could not be saved. Nothing was changed …" and a red status; nothing is written. Untick it
  and save again to recover.
- **Reset with a dirty form** — change the name and press **Reset**: the only `MessageBox` of the module asks first.
