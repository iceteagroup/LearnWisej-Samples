using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using SupportDesk.Data;

namespace SupportDesk.Services;

/// <summary>
/// The Add/Edit Ticket screen's edit model — a UI-only class, <b>never</b> the tracked EF Core
/// <c>Ticket</c> entity. <see cref="TicketEditorForm"/> binds every editor control to a property here
/// through <c>editBindingSource</c>; <c>TicketCommandService</c> maps the approved values onto a tracked
/// entity only inside <c>SaveAsync</c>, in its own fresh <c>DbContext</c>.
/// </summary>
/// <remarks>
/// <para>
/// <c>Number</c>, <c>CreatedAt</c> and <c>UpdatedAt</c> are deliberately left out — the screen must not be
/// able to edit a field it never meant to expose. <c>Number</c> is assigned once, by
/// <c>TicketCommandService.SaveAsync</c>, the first time a ticket is saved; <c>lblNumber</c> in the editor
/// shows it, but nothing binds to it and nothing can type into it. <c>UpdatedAt</c> is stamped by
/// <c>SupportDeskContext.SaveChanges(Async)</c> on every insert and update.
/// </para>
/// <para>
/// <b>RowVersion is not here either — not yet.</b> Module 7 adds it, together with the conflict dialog
/// that reads a <c>DbUpdateConcurrencyException</c> and asks the operator to keep or discard their edit.
/// Until then, <c>TicketCommandService.SaveAsync</c> reloads the ticket fresh, immediately before mapping
/// and saving, so it always writes against whatever row is in the database right now — the last Save
/// always wins. The <c>RowVersion</c> column itself is still a real concurrency token on the entity (see
/// <c>SupportDeskContext.StampTickets</c>), so a second, truly concurrent write in the same instant can
/// still throw <c>DbUpdateConcurrencyException</c> — Module 4 catches that exception with a friendly
/// message, it just does not carry the token forward from when the editor opened to detect the conflict on
/// purpose.
/// </para>
/// <para>
/// <see cref="INotifyPropertyChanged"/> is implemented so a value changed in code (for example, copying
/// the DateTimePicker's <c>Checked</c>/<c>Value</c> by hand — see <c>TicketEditorForm</c>) can still
/// notify a bound control if one is ever added later. The controls that bind directly through
/// <c>DataBindings.Add</c> in Module 4 do not strictly need it — Wisej.NET's <c>BindingSource</c> reads the
/// model once when it becomes the current item and pushes control edits back through
/// <c>DataSourceUpdateMode</c> — but it costs nothing and keeps the model honest as a real bindable type.
/// </para>
/// <para>
/// <b>Module 5: DataAnnotations describe the UI rules, nothing runs them by itself.</b>
/// <see cref="Title"/> is <c>[Required, StringLength(SupportDeskContext.TitleMaxLength)]</c> — the same 180
/// characters <c>SupportDeskContext</c> enforces with a CHECK constraint, named once and shared, not
/// retyped. <see cref="Description"/> is <c>[StringLength(4000)]</c>. <see cref="Status"/>,
/// <see cref="Priority"/>, <see cref="CustomerId"/> and <see cref="CategoryId"/> are <c>[Required]</c> —
/// the two foreign keys stay <c>int?</c> on purpose (see their remarks below) so an unselected
/// <c>ComboBox</c> is a null value <c>[Required]</c> catches, not a silent zero a non-nullable <c>int</c>
/// would let through unnoticed. None of this runs on its own: EF Core does not validate annotations before
/// <c>SaveChangesAsync</c>, and Wisej.NET's <c>BindingSource</c> does not either — <c>TicketValidator.Validate</c>
/// is the one place <c>Validator.TryValidateObject</c> is actually called, from <c>TicketEditorForm.SaveAsync</c>.
/// </para>
/// </remarks>
public sealed class TicketEditModel : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private string? _description;
    private string _status = TicketStatuses.Open;
    private string _priority = TicketPriorities.Normal;
    private int? _customerId;
    private int? _agentId;
    private int? _categoryId;
    private DateTime? _dueDate;
    private bool _isUrgent;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>0 for a ticket that has never been saved — the signal <c>SaveAsync</c> uses to Add instead of load.</summary>
    public int Id { get; set; }

    [Required(ErrorMessage = "The Title field is required.")]
    [StringLength(SupportDeskContext.TitleMaxLength, ErrorMessage = "Title cannot be longer than {1} characters.")]
    public string Title { get => _title; set => Set(ref _title, value); }

    [StringLength(4000, ErrorMessage = "Description cannot be longer than {1} characters.")]
    public string? Description { get => _description; set => Set(ref _description, value); }

    [Required(ErrorMessage = "Status is required.")]
    public string Status { get => _status; set => Set(ref _status, value); }

    [Required(ErrorMessage = "Priority is required.")]
    public string Priority { get => _priority; set => Set(ref _priority, value); }

    /// <summary>Bound to <c>cboCustomer.SelectedValue</c>. Required to save, but nullable here so a fresh "Add ticket" can start with nothing chosen — an unselected ComboBox yields null, which <c>[Required]</c> catches; a non-nullable <c>int</c> would silently pass as zero instead.</summary>
    [Required(ErrorMessage = "Choose a customer.")]
    public int? CustomerId { get => _customerId; set => Set(ref _customerId, value); }

    /// <summary>Bound to <c>cboAgent.SelectedValue</c>. Null means unassigned — a ticket may exist before anyone owns it. Not required: an unassigned ticket is a valid ticket.</summary>
    public int? AgentId { get => _agentId; set => Set(ref _agentId, value); }

    /// <summary>Bound to <c>cboCategory.SelectedValue</c>. Required to save, same reasoning as <see cref="CustomerId"/>.</summary>
    [Required(ErrorMessage = "Choose a category.")]
    public int? CategoryId { get => _categoryId; set => Set(ref _categoryId, value); }

    /// <summary>
    /// Copied by hand between <c>dtpDueDate.Checked</c>/<c>Value</c> and this property in
    /// <c>LoadEditorAsync</c>/<c>SaveAsync</c> rather than bound through <c>DataBindings.Add</c> — see the
    /// remarks on <see cref="TicketEditorForm"/> for why.
    /// </summary>
    public DateTime? DueDate { get => _dueDate; set => Set(ref _dueDate, value); }

    public bool IsUrgent { get => _isUrgent; set => Set(ref _isUrgent, value); }

    /// <summary>A blank model for the "Add ticket" case: the two defaults every new ticket starts with, nothing else set.</summary>
    public static TicketEditModel NewTicket() => new() { Status = TicketStatuses.Open, Priority = TicketPriorities.Normal };

    private void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
