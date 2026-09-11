# Deliverable 1 · TicketEditModel and TicketEditorForm with a BindingSource and an ErrorProvider

## TicketEditModel

`SupportDesk.Services/TicketEditModel.cs` is a plain, UI-only class — never the tracked EF Core `Ticket`
entity. It carries exactly the fields the screen may edit:

```csharp
public sealed class TicketEditModel : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public int? CustomerId { get; set; }
    public int? AgentId { get; set; }
    public int? CategoryId { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsUrgent { get; set; }
}
```

`Number`, `CreatedAt` and `UpdatedAt` are deliberately left out — the screen must not be able to edit a
field it never meant to expose. The number is shown only in the dialog title (*Edit Ticket — SD-1042*),
never in an editable control.

Module 4's screen binds five of these fields (`Title`, `CustomerId`, `CategoryId`, `DueDate`, `IsUrgent`);
`Description`, `Status`, `Priority` and `AgentId` get their controls in Module 5. Until then they travel
through the model untouched: `LoadEditModelAsync` fills them and `SaveAsync` writes them back as they were.

**RowVersion is not in the model — not yet.** Module 7 adds it, together with the conflict dialog that
reads a `DbUpdateConcurrencyException` and lets the operator keep or discard their edit. Until then,
`TicketCommandService.SaveAsync` reloads the ticket fresh, immediately before mapping and saving — the last
Save always wins, on purpose (see `docs/LoadAndSaveFlows.md`). This is a deliberate scope cut for Module 4,
noted in `TicketEditModel`'s XML doc comment and in the lab guide's lesson text (which mentions RowVersion
as part of a *later* concurrency check the reading discusses but this module does not implement).

`INotifyPropertyChanged` is implemented (a private `Set<T>` helper raises `PropertyChanged`) so a value
changed in code — the DateTimePicker copy described in `docs/ControlDataBindings.md` — could still notify a
bound control. The controls that bind directly through `DataBindings.Add` do not strictly require it for
Module 4's one-way-per-click flow, but it costs nothing and keeps the model a real bindable type.

## TicketEditorForm

`SupportDesk.Web/TicketEditorForm.cs` (+ `.Designer.cs`) is a modal `Form`:

```csharp
public partial class TicketEditorForm : Form
{
    [Inject]
    private TicketCommandService Commands { get; set; }

    /// <param name="ticketId">Null for "Add Ticket"; an existing ticket's key for "Edit Ticket".</param>
    public TicketEditorForm(int? ticketId)
    {
        _ticketId = ticketId;
        InitializeComponent();
    }
}
```

`[Inject]` resolves `TicketCommandService` in the constructor exactly like a `Page` does — the cookbook's
verified rule ("injection happens for top-level containers (Form/Page) in their constructor") applies to a
`Form` shown with `new TicketEditorForm(...)` the same as it does to `TicketBrowserPage`.

The controls: `txtTitle`, `cboCustomer`, `cboCategory`, `dtpDueDate`, `chkIsUrgent` (*Escalate as urgent*)
and the buttons `btnDelete`, `btnCancel`, `btnSave`. `editBindingSource` (`Wisej.Web.BindingSource`) and
`errorProvider` (`Wisej.Web.ErrorProvider`) are both created as designer components in
`InitializeComponent`:

```csharp
this.editBindingSource = new Wisej.Web.BindingSource(this.components);
this.errorProvider = new Wisej.Web.ErrorProvider(this.components);
```

`editBindingSource.DataSource` is **not** set in `InitializeComponent` — it starts empty. It becomes a
`TicketEditModel` only inside `LoadEditorAsync`, after the lookup ComboBoxes already have their
`DataSource` (see `docs/ControlDataBindings.md`). The form itself is fixed-size, centred on its parent and
not in the taskbar:

```csharp
this.FormBorderStyle = Wisej.Web.FormBorderStyle.Fixed;
this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
this.ShowInTaskbar = false;
this.AcceptButton = this.btnSave;
this.CancelButton = this.btnCancel;
```

`btnDelete.Visible` is toggled in `LoadEditorAsync`: hidden for a new ticket (nothing to delete yet),
visible once an existing ticket's `Id` is known.

## Evidence

- The solution builds with `TicketEditorForm` and `TicketEditModel` compiled into `SupportDesk.Web` and
  `SupportDesk.Services`.
- `SupportDesk.Tests/TicketCommandServiceTests.cs` exercises `TicketEditModel` end to end through
  `TicketCommandService` (the form itself is not unit-testable without a browser): `LoadEditModelAsync_maps_every_editable_field_and_leaves_Number_out_of_the_model`
  asserts every field the model is supposed to carry, and that `Number` travels alongside the model
  (`TicketEditData.Number`) rather than inside it.
- Reading `TicketEditorForm.Designer.cs` shows `editBindingSource` and `errorProvider` constructed with
  `this.components` — the same pattern the cookbook names as the shorthand for a component that needs no
  visual placement.

**Not verified here — for the browser reviewer.** Whether the dialog actually renders centred over
`TicketBrowserPage`, at a fixed, non-resizable size, with `btnDelete` hidden for a new ticket and visible
for an existing one.
