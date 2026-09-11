# Deliverable 2 · TextBox, ComboBox, DateTimePicker and CheckBox properties bound to the edit model

Module 4's editor has five input controls: `txtTitle`, `cboCustomer`, `cboCategory`, `dtpDueDate` and
`chkIsUrgent` (*Escalate as urgent*). Two kinds of binding, added at two different moments, all against
`editBindingSource`:

- the simple ones (`txtTitle`, `chkIsUrgent`) are added once, in `TicketEditorForm.Designer.cs`'s
  `InitializeComponent`, before `editBindingSource` has a `DataSource`. This mirrors
  `TicketBrowserPage.Designer.cs`, where the grid's columns are declared with a `DataPropertyName` before
  any row exists;
- the ComboBox `SelectedValue` bindings are added in `TicketEditorForm.BindLookupControls`, once the lookup
  lists **and** the model both exist. Added earlier they neither show the model value nor write the
  selection back.

| Control | Property bound | Model property | `DataSourceUpdateMode` | Notes |
|---|---|---|---|---|
| `txtTitle` | `Text` | `Title` | `OnValidation` | Text commits when focus leaves; `EndEdit` in Save covers the case where it has not. |
| `chkIsUrgent` | `Checked` | `IsUrgent` (`bool`) | `OnPropertyChanged` | |
| `cboCustomer` | `SelectedValue` | `CustomerId` (`int?`) | `OnPropertyChanged` | Nothing selected already means `null` — no sentinel needed; `Customer` is required to *save*, not to have something selected while editing. |
| `cboCategory` | `SelectedValue` | `CategoryId` (`int?`) | `OnPropertyChanged` | Same reasoning as `cboCustomer`. |
| `dtpDueDate` | *(none — see below)* | `DueDate` (`DateTime?`) | *(manual)* | |

```csharp
// BindLookupControls, called from LoadEditorAsync right after editBindingSource.DataSource = model
this.cboCustomer.DataBindings.Add("SelectedValue", this.editBindingSource, nameof(TicketEditModel.CustomerId), true, DataSourceUpdateMode.OnPropertyChanged);
this.cboCategory.DataBindings.Add("SelectedValue", this.editBindingSource, nameof(TicketEditModel.CategoryId), true, DataSourceUpdateMode.OnPropertyChanged);
```

Lookups are loaded and each ComboBox's `DisplayMember`/`ValueMember`/`DataSource` are set in
`LoadEditorAsync` **before** `editBindingSource.DataSource = model` — the same rule
`TicketBrowserPage.LoadLookupsAsync` follows for its filter ComboBoxes: a bound `SelectedValue` with
nothing to select resolves to nothing.

`TicketEditModel` also carries `Description`, `Status`, `Priority` and `AgentId`. Module 4's screen has no
control for them yet (`txtDescription`, `cboStatus`, `cboPriority` and `cboAgent` arrive in Module 5):
`LoadEditModelAsync` fills them from the database and `SaveAsync` writes them back unchanged. Module 5
also brings the agent ComboBox's "— unassigned —" row, mapped to a null `AgentId` through the binding's
`Format`/`Parse` events.

## The DateTimePicker: copied by hand, not bound through DataBindings.Add

`dtpDueDate` is the one control **not** wired with `DataBindings.Add`. The model's `DueDate` is a
nullable `DateTime?` and `DateTimePicker.Value` is not nullable; the course cookbook flags that binding
as unverified and names the safer alternative: copy `Checked`/`Value` by hand. `TicketEditorForm` does
exactly that:

```csharp
// LoadEditorAsync, after the model is loaded and assigned:
this.dtpDueDate.Checked = model.DueDate.HasValue;
this.dtpDueDate.Value = model.DueDate ?? DateTime.Today;

// SaveAsync, right after EndEdit — so the picker's state is captured no matter which
// control the operator touched last:
model.DueDate = this.dtpDueDate.Checked ? this.dtpDueDate.Value.Date : (DateTime?)null;
```

`dtpDueDate.ShowCheckBox = true` — unticked means "no due date".

## Evidence

- The solution builds: every `DataBindings.Add` call and the manual `dtpDueDate` copy compile against the
  real Wisej.NET types.
- `SupportDesk.Tests/TicketCommandServiceTests.cs` → `Load_then_edit_then_save_then_reload_round_trips_exactly_what_was_typed`
  sets every field of the edit model — including `AgentId = null` and a `DueDate` — saves and reloads, and
  asserts every value round-trips. This proves the *model and service* side of the binding contract; it
  does not exercise the Wisej controls themselves.

**Not verified here — for the browser reviewer.** Whether `dtpDueDate.ShowCheckBox` really means
"unticked = no date" in Wisej.NET's rendered control (flagged unverified in the course cookbook since
Module 1); whether `OnValidation` truly waits for focus to leave `txtTitle` before pushing into the model,
which is what makes `EndEdit` necessary at all (see `docs/LoadAndSaveFlows.md`).
