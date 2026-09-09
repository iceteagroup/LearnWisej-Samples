# Validation rules — close to the UI, not trapped in it

The lab deliverable for lesson s22: where each kind of validation lives in this project, the readable Save
handler, the Save/Cancel pattern checklist, and the control names that keep the event code readable.

## 1. Validation placement

Validation belongs close enough to the UI that users get instant feedback, but the important business rules
must stay testable outside a button click. The dialog checks simple field problems; the service owns the rules
about the data itself.

| Validation type | Example from the lesson | Good place | Where it lives in this project |
|---|---|---|---|
| Simple UI validation | Title textbox is empty; Status is not selected | a dialog method such as `ValidateForm()` | `TicketDialog.ValidateForm()` — Title required, Title ≤ 80 characters (`TicketDialog.MaxTitleLength`), Status required, Customer required; the message is written into `lblValidation` |
| Business rule | a closed ticket cannot be edited by a support agent | `TicketService` or a validation service | `TicketService.UpdateTicket` throws `InvalidOperationException` when the Id does not exist; `AddTicket` assigns the next Id itself (the dialog never chooses Ids) and fills a missing `CreatedDate`. This module has no role model, so the "closed ticket" rule is not implemented — it would go in `TicketService`, not in the dialog |
| Persistence rule | Ticket Id already exists in storage | the repository / service layer | `TicketService.SaveTicket` decides add-vs-update by looking the Id up in the store; `UpdateTicket` replaces by `FindIndex(t => t.Id == ticket.Id)` |

What is deliberately **not** in the dialog: Id generation, the "does this ticket exist" check, the ordering of the
list. What is deliberately **not** in the service: anything about textboxes, combo boxes or messages.

### The rules `ValidateForm()` checks, and the messages it shows

| Rule | Message (one bullet each, all problems listed at once) | Focus goes to |
|---|---|---|
| Title required | `Title is required.` | `txtTitle` |
| Title ≤ 80 characters | `Title is too long (n characters, maximum 80).` | `txtTitle` |
| Status required | `Status is required — choose Open, In Progress or Closed.` | `cboStatus` |
| Customer required | `Customer is required.` | `txtCustomer` |

`lblValidation` starts hidden (`Visible = false`); a failed check shows it with
*Please fix the following before saving:* followed by the bullets; a passing check clears and hides it again.
The message says exactly what to fix — never just "invalid input".

## 2. The readable Save handler

```csharp
private void btnSave_Click(object sender, EventArgs e)
{
    if (!ValidateForm())
        return;

    TicketResult = BuildTicketFromFields();
    this.DialogResult = DialogResult.OK;
    this.Close();
}
```

Four lines, three helpers, no storage code:

- `ValidateForm()` — returns early when anything is wrong; nothing is saved and the dialog stays open.
- `BuildTicketFromFields()` — turns the controls into a `Ticket` (keeps `Id` / `CreatedDate` when editing).
- `DialogResult = OK; Close()` — the dialog's only job is to return a clear result.

The lesson's version calls `ticketService.SaveTicket(ticket)` inside the handler. This project moves that call
one level up, into the page's `if (result == DialogResult.OK)` block, so the dialog can be reused by any screen
and the grid refresh sits right next to the save it follows. Both shapes satisfy "only valid input reaches the
service"; `TicketService.SaveTicket` exists for the lesson's one-call form.

## 3. The Save / Cancel pattern — checklist

| Rule (lesson s22) | Done here by |
|---|---|
| Save validates first | `if (!ValidateForm()) return;` is the first statement of `btnSave_Click` |
| … then calls the service | `ticketService.AddTicket(dialog.TicketResult)` / `UpdateTicket(...)` in `TicketsWindow` after `DialogResult.OK` |
| … then closes the dialog only after success | `DialogResult.OK; Close()` run only after validation passed; a failed check returns before them |
| Cancel closes without saving | `btnCancel_Click`: `DialogResult = Cancel; Close();` — `TicketResult` stays null; `CancelButton = btnCancel` so Escape does the same |
| Error messages tell the user exactly what to fix | `lblValidation` bullets above |
| The parent refreshes the grid only when the dialog returns OK | `RefreshTicketGrid()` is inside the `OK` branch; the `else` branch only logs "cancelled — nothing saved" |
| Clear button names | `btnSave`, `btnCancel` in the dialog; `btnNew`, `btnEdit`, `btnDelete`, `btnRefresh` on the page |

## 4. Control naming

Every control that has behaviour is named after what it holds, so the handlers read as sentences
(`txtTitle.Text`, `cboStatus.SelectedItem`, `lblValidation.Visible = true`):

| Dialog (`TicketDialog`) | Page (`TicketsWindow`) |
|---|---|
| `txtTitle`, `txtCustomer`, `txtAssignedTo`, `txtDescription` (multiline) | `dgvTickets` bound through `ticketsBindingSource` |
| `cboStatus` (Open / In Progress / Closed), `cboPriority` (Low / Medium / High) — `DropDownList`, no free text | `btnNew`, `btnEdit`, `btnDelete`, `btnRefresh` |
| `lblValidation` — red, hidden until `ValidateForm()` fails | `lblPageTitle`, `lblStatus`, `lblSelection` |
| `btnSave` (`AcceptButton`), `btnCancel` (`CancelButton`) | `lstEventLog`, `lblWorkflow` |

Nothing is called `textBox1` or `button1`.

## Evidence — what the running app shows

| Path | What you do | What you see |
|---|---|---|
| Missing Title | New Ticket → Customer "Northwind" → Save | dialog stays open; `lblValidation`: *• Title is required.*; cursor in `txtTitle`; log: `step 3 Validate → ValidateForm() = false → "Title is required." → return (dialog stays open, nothing saved)`; status red |
| Missing Customer and Title | New Ticket → Save with everything blank | one message with two bullets: *• Title is required.  • Customer is required.* — Status passes because the dialog defaults it to Open |
| Title too long | paste 90 characters into Title → Save | *• Title is too long (90 characters, maximum 80).* (the textbox itself allows 120 so the rule can be seen firing) |
| Recovery | fix the fields → Save | `ValidateForm() = true`, `DialogResult.OK`, `AddTicket`, refresh, toast "Ticket created." |
| Cancel | any state → Cancel / Escape | dialog closes; log: `dialog returned DialogResult.Cancel → … nothing saved, grid not refreshed`; grid unchanged |
