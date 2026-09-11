# ErrorProvider: cleared before every run, set for Title, Customer, Category and DueDate

**Deliverable:** `ErrorProvider` cleared before each run and set for Title, Customer, Category and DueDate.

## The component

`errorProvider` is a `Wisej.Web.ErrorProvider` component created against the form's `components` container
(`new Wisej.Web.ErrorProvider(this.components)`, in `TicketEditorForm.Designer.cs` — unchanged since Module 4,
which declared it but never used it). `SetError(control, message)` shows an icon and tooltip next to a
control; `SetError(control, "")` clears one; `Clear()` clears every icon on the form at once.

## The mapping: `ShowValidation`

`TicketEditorForm.ShowValidation(IReadOnlyList<ValidationMessage> messages)` is the one place `errorProvider`
is ever touched:

```csharp
private void ShowValidation(IReadOnlyList<ValidationMessage> messages)
{
    this.errorProvider.Clear();

    foreach (var group in messages.GroupBy(m => m.FieldName))
    {
        var text = string.Join(" ", group.Select(m => m.Message));
        switch (group.Key)
        {
            case nameof(TicketEditModel.Title):       this.errorProvider.SetError(this.txtTitle, text); break;
            case nameof(TicketEditModel.Description):  this.errorProvider.SetError(this.txtDescription, text); break;
            case nameof(TicketEditModel.Status):       this.errorProvider.SetError(this.cboStatus, text); break;
            case nameof(TicketEditModel.Priority):     this.errorProvider.SetError(this.cboPriority, text); break;
            case nameof(TicketEditModel.CustomerId):   this.errorProvider.SetError(this.cboCustomer, text); break;
            case nameof(TicketEditModel.CategoryId):   this.errorProvider.SetError(this.cboCategory, text); break;
            case nameof(TicketEditModel.DueDate):      this.errorProvider.SetError(this.dtpDueDate, text); break;
            // a null FieldName (the cross-field rule) reaches only the summary label
        }
    }
    // … validationSummaryLabel — see CrossFieldRuleAndSaveGating.md
}
```

`Clear()` runs first, every single call — so a run that fixes one field and still fails another never leaves a
stale icon behind on the field the operator already fixed. Every lab-named control the task asks for is wired:
`txtTitle`, `cboCustomer`, `cboCategory`, `dtpDueDate` — plus `txtDescription`, `cboStatus` and `cboPriority`,
which are also annotated on the model and would otherwise have a message with nowhere to go.

## When it runs

Two callers, both going through `RunValidation(paint: true)` (never `ShowValidation` directly):

1. **`SaveAsync`**, right after `errorProvider.Clear()` → `editBindingSource.EndEdit()` → the model is read.
   This is the "first failed Save" moment the lab guide asks about.
2. **Live field-change events** (`Field_Changed`, `DueDate_Changed` — see
   [`CrossFieldRuleAndSaveGating.md`](CrossFieldRuleAndSaveGating.md)), wired in the Designer to `Validated` on
   `txtTitle`/`txtDescription`, `SelectedValueChanged` on `cboCustomer`/`cboCategory`/`cboStatus`, and
   `ValueChanged`/`Validated` on `dtpDueDate`.

Both paths call `errorProvider.Clear()` (once inside `ShowValidation`, once again explicitly at the top of
`SaveAsync`) before painting anything new — "cleared before each run" is not just true on the first run, it is
true on every run, including the second time the operator opens the same dialog in the same session (the lab
guide's "Review & run" step asks the reviewer to open the editor twice in a row and confirm the previous run's
icons are gone).

## Evidence

- `dotnet build` — 0 warnings, 0 errors: `ShowValidation` compiles against every control name the lab guide
  and the course cookbook agree on (`errorProvider`, `txtTitle`, `cboCustomer`, `cboCategory`, `dtpDueDate`).
- `SupportDesk.Tests/TicketValidatorTests.cs` proves the *messages* carry the right `FieldName` for every
  control `ShowValidation` maps (`Title`, `CustomerId`, `CategoryId`, `DueDate`, `Description`) — the mapping
  itself, and the icon actually appearing next to the right control, is UI behaviour only a running dialog can
  show; see the README's "Verified / unverified".
