using System.ComponentModel.DataAnnotations;

namespace SupportDesk.Services;

/// <summary>
/// One validation failure. <see cref="FieldName"/> matches a <see cref="TicketEditModel"/> property name
/// (<c>Title</c>, <c>CustomerId</c>, <c>CategoryId</c>, <c>DueDate</c>, …) so <c>TicketEditorForm.ShowValidation</c>
/// can route it to <c>errorProvider.SetError</c> on the matching control; <see langword="null"/> means the
/// rule belongs to no single control and only ever reaches the summary label.
/// </summary>
public sealed record ValidationMessage(string? FieldName, string Message);

/// <summary>
/// Runs <see cref="TicketEditModel"/> through its <c>DataAnnotations</c> and the one cross-field rule the
/// attributes cannot express, and returns a flat list of <see cref="ValidationMessage"/> the UI can map to
/// controls. This is the <b>domain validation</b> layer the lesson names — the middle of the three: UI
/// feedback is a courtesy, the database is the final authority, and this service is what makes the middle
/// layer a real, testable thing instead of a scattering of <c>if</c> statements inside a click handler.
/// </summary>
/// <remarks>
/// <para>
/// <b>UI-free and testable alone.</b> This class has no reference to <c>Wisej.Web</c>, no <c>ErrorProvider</c>,
/// no form — it takes a <see cref="TicketEditModel"/> and returns data.
/// <c>TicketValidatorTests</c> (in the Tests project) exercises every negative case against it directly,
/// with no page, no session and no database.
/// </para>
/// <para>
/// <b>Why <see cref="Validator.TryValidateObject(object, ValidationContext, ICollection{ValidationResult}?, bool)"/>
/// and not EF Core.</b> EF Core understands <c>[Required]</c>/<c>[MaxLength]</c> as mapping instructions on an
/// <i>entity</i> — they decide column nullability and width, and are never run as a validation pass before
/// <c>SaveChangesAsync</c>. <see cref="TicketEditModel"/> is not an entity, and nothing calls
/// <see cref="Validator"/> for you: <see cref="Validate"/> is the one place in this solution that does,
/// with <c>validateAllProperties: true</c> so every annotated property is checked, not just the first one
/// that fails.
/// </para>
/// <para>
/// <b>The cross-field rule.</b> "A Closed ticket cannot have a future due date" cannot be expressed as an
/// attribute on a single property — it reads two properties at once — so it is appended by hand after the
/// annotation pass, reported against <see cref="TicketEditModel.DueDate"/> (see the Student review question
/// in the Module 5 README for the case where it should report no field at all instead).
/// </para>
/// </remarks>
public sealed class TicketValidator
{
    /// <summary>
    /// Runs every <c>DataAnnotations</c> attribute on <paramref name="model"/> plus the closed/future-due-date
    /// rule, and returns every failure found — never just the first. An empty list means the model may be
    /// saved.
    /// </summary>
    public IReadOnlyList<ValidationMessage> Validate(TicketEditModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);

        var messages = results
            .SelectMany(
                r => r.MemberNames.DefaultIfEmpty(null),
                (r, member) => new ValidationMessage(member, r.ErrorMessage ?? "Invalid value."))
            .ToList();

        // The cross-field rule DataAnnotations cannot express: it reads Status AND DueDate together.
        if (model.Status == TicketStatuses.Closed && model.DueDate?.Date > DateTime.Today)
            messages.Add(new ValidationMessage(nameof(TicketEditModel.DueDate), "Closed tickets cannot have a future due date."));

        return messages;
    }
}
