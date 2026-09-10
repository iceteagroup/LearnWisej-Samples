# Deliverable 2 · TextBox, ComboBox, DateTimePicker and CheckBox properties bound to the edit model

All bindings are added once, in `TicketEditorForm.Designer.cs`'s `InitializeComponent`, against
`editBindingSource` — before it has a `DataSource`. This mirrors `TicketBrowserPage.Designer.cs`, where the
grid's columns are declared with a `DataPropertyName` before any row exists.

| Control | Property bound | Model property | `DataSourceUpdateMode` | Notes |
|---|---|---|---|---|
| `txtTitle` | `Text` | `Title` | `OnValidation` | Text commits when focus leaves — the value the validation placeholder checks. |
| `txtDescription` | `Text` | `Description` | `OnValidation` | Multiline; same reasoning as `txtTitle`. |
| `cboCustomer` | `SelectedValue` | `CustomerId` (`int?`) | `OnPropertyChanged` | Nothing selected already means `null` — no sentinel needed; `Customer` is required to *save*, not to have something selected while editing. |
| `cboAgent` | `SelectedValue` | `AgentId` (`int?`) | `OnPropertyChanged` | The one ComboBox with a real "— unassigned —" row — see below. |
| `cboCategory` | `SelectedValue` | `CategoryId` (`int?`) | `OnPropertyChanged` | Same reasoning as `cboCustomer`. |
| `cboStatus` | `SelectedValue` | `Status` (`string`) | `OnPropertyChanged` | `DataSource` = `TicketStatuses.All`, a `List<string>` — the value *is* the display text, no `ValueMember`. |
| `cboPriority` | `SelectedValue` | `Priority` (`string`) | `OnPropertyChanged` | Same shape as `cboStatus`, `TicketPriorities.All`. |
| `chkIsUrgent` | `Checked` | `IsUrgent` (`bool`) | `OnPropertyChanged` | |
| `dtpDueDate` | *(none — see below)* | `DueDate` (`DateTime?`) | *(manual)* | |

Lookups are loaded and each ComboBox's `DisplayMember`/`DataSource` are set in `LoadEditorAsync`
**before** `editBindingSource.DataSource = model` — the same rule `TicketBrowserPage.LoadLookupsAsync`
follows for its filter ComboBoxes: a bound `SelectedValue` with nothing to select resolves to nothing.

## The agent sentinel: Format/Parse, not a plain binding

`cboAgent`'s list carries a real "— unassigned —" row (`LookupItem(TicketCommandService.UnassignedAgentId, "— unassigned —")`, `UnassignedAgentId = 0` — no real agent ever has `Id == 0`) next to the actual
agents, because the lab asks the operator to be able to *choose* "nobody", not merely leave a field blank.
A plain `SelectedValue` binding would push the sentinel `0` into `AgentId`, which is wrong — `AgentId`
must become `null`. `Wisej.Web.Binding` exposes `Format`/`Parse` events (confirmed in the Wisej.NET XML
docs: `Wisej.Web.Binding.Format`, `Wisej.Web.Binding.Parse`, both `Wisej.Web.ConvertEventHandler` over
`Wisej.Web.ConvertEventArgs`) for exactly this conversion:

```csharp
Wisej.Web.Binding agentBinding = this.cboAgent.DataBindings.Add(
    "SelectedValue", this.editBindingSource, nameof(TicketEditModel.AgentId), true, Wisej.Web.DataSourceUpdateMode.OnPropertyChanged);
agentBinding.Format += new Wisej.Web.ConvertEventHandler(this.AgentBinding_Format);
agentBinding.Parse += new Wisej.Web.ConvertEventHandler(this.AgentBinding_Parse);
```

```csharp
private void AgentBinding_Format(object sender, ConvertEventArgs e)
    => e.Value = (e.Value as int?) ?? TicketCommandService.UnassignedAgentId;   // model → control

private void AgentBinding_Parse(object sender, ConvertEventArgs e)
{
    if (e.Value is int selected && selected == TicketCommandService.UnassignedAgentId)
        e.Value = null;                                                        // control → model
}
```

## The DateTimePicker: copied by hand, not bound through DataBindings.Add

`dtpDueDate` is the one control **not** wired with `DataBindings.Add`. The course cookbook flags binding a
nullable `DateTime?` model property to `DateTimePicker.Value` (which is not nullable) as unverified, and
names the safer alternative: copy `Checked`/`Value` by hand. `TicketEditorForm` does exactly that:

```csharp
// LoadEditorAsync, after the model is loaded and assigned:
this.dtpDueDate.Checked = model.DueDate.HasValue;
this.dtpDueDate.Value = model.DueDate ?? DateTime.Today;

// SaveAsync, right after EndEdit — so the picker's state is captured no matter which
// control the operator touched last:
model.DueDate = this.dtpDueDate.Checked ? this.dtpDueDate.Value.Date : (DateTime?)null;
```

`dtpDueDate.ShowCheckBox = true` — unticked means "no due date", the same semantics
`TicketBrowserPage`'s two filter pickers already use (also unverified there, in Module 3).

## Evidence

- `dotnet build SupportDesk.slnx -nologo -v q` — 0 warnings, 0 errors: every `DataBindings.Add` call,
  the `Format`/`Parse` wiring and the manual `dtpDueDate` copy all compile against the real
  `Wisej.Web.Binding`/`ConvertEventArgs`/`ConvertEventHandler` types (confirmed against the Wisej-4 4.1.0
  XML documentation, not guessed).
- `SupportDesk.Tests/TicketCommandServiceTests.cs` → `Load_then_edit_then_save_then_reload_round_trips_exactly_what_was_typed`
  sets every field a control would push into the model — including `AgentId = null` (the "— unassigned —"
  case) and a `DueDate` — saves and reloads, and asserts every value round-trips. This proves the *model
  and service* side of the binding contract; it does not exercise the Wisej controls themselves.

**Not verified here — for the browser reviewer.** Whether `cboAgent`'s Format/Parse events fire the way
the XML docs describe when the operator actually picks "— unassigned —" in a running ComboBox; whether
`dtpDueDate.ShowCheckBox` really means "unticked = no date" in Wisej.NET's rendered control (flagged
unverified in the course cookbook since Module 1); whether `OnValidation` truly waits for focus to leave
`txtTitle`/`txtDescription` before pushing into the model, which is what makes `EndEdit` necessary at all
(see `docs/LoadAndSaveFlows.md`).
