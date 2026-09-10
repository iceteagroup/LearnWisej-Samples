using SupportDesk.Services;

namespace SupportDesk.Tests;

/// <summary>
/// The Module 5 deliverable, tested against <see cref="TicketValidator"/> alone — no form, no session, no
/// database. Six negative cases (one more than the lab's minimum of five) plus a positive case, so the
/// validator's shape (flattened <c>ValidationResult</c> items, the hand-appended cross-field rule) is
/// proved independently of how <c>TicketEditorForm</c> happens to use it.
/// </summary>
/// <remarks>
/// Which of the three validation layers each negative case exercises (the Module 5 lab's first "Student
/// review question"): every test here exercises the <b>domain layer</b> — <see cref="TicketValidator"/>
/// running <c>DataAnnotations</c> plus the hand-written cross-field rule. None of them touch the database
/// layer (that is <see cref="TicketCommandServiceTests"/>'s duplicate-number test) or the UI layer
/// (<c>ErrorProvider</c>/<c>validationSummaryLabel</c>, which only a running <c>TicketEditorForm</c> can
/// show — see the Module 5 README's "Verified / unverified").
/// </remarks>
public sealed class TicketValidatorTests
{
    private readonly TicketValidator _validator = new();

    /// <summary>A model that passes every rule — every negative test below starts from a copy of this and breaks exactly one thing.</summary>
    private static TicketEditModel ValidModel() => new()
    {
        Title = "Printer on floor 3 will not accept the network driver",
        Description = "Escalated from the walk-up desk.",
        Status = TicketStatuses.Open,
        Priority = TicketPriorities.Normal,
        CustomerId = 1,
        CategoryId = 2,
        DueDate = null
    };

    [Fact]
    public void Valid_model_produces_no_messages()
    {
        var messages = _validator.Validate(ValidModel());

        Assert.Empty(messages);
    }

    [Fact]
    public void Empty_title_is_reported_against_Title()
    {
        var model = ValidModel();
        model.Title = "";

        var messages = _validator.Validate(model);

        Assert.Contains(messages, m => m.FieldName == nameof(TicketEditModel.Title));
    }

    [Fact]
    public void Whitespace_only_title_is_reported_against_Title()
    {
        var model = ValidModel();
        model.Title = "   ";

        var messages = _validator.Validate(model);

        // [Required] on a string treats whitespace as empty (AllowEmptyStrings defaults to false).
        Assert.Contains(messages, m => m.FieldName == nameof(TicketEditModel.Title));
    }

    [Fact]
    public void Title_over_180_characters_is_reported_against_Title()
    {
        var model = ValidModel();
        model.Title = new string('A', 181);

        var messages = _validator.Validate(model);

        Assert.Contains(messages, m => m.FieldName == nameof(TicketEditModel.Title));
    }

    [Fact]
    public void Title_at_exactly_180_characters_is_not_reported()
    {
        var model = ValidModel();
        model.Title = new string('A', 180);          // SupportDeskContext.TitleMaxLength — the boundary itself must pass

        var messages = _validator.Validate(model);

        Assert.DoesNotContain(messages, m => m.FieldName == nameof(TicketEditModel.Title));
    }

    [Fact]
    public void Missing_customer_is_reported_against_CustomerId()
    {
        var model = ValidModel();
        model.CustomerId = null;

        var messages = _validator.Validate(model);

        Assert.Contains(messages, m => m.FieldName == nameof(TicketEditModel.CustomerId));
    }

    [Fact]
    public void Missing_category_is_reported_against_CategoryId()
    {
        var model = ValidModel();
        model.CategoryId = null;

        var messages = _validator.Validate(model);

        Assert.Contains(messages, m => m.FieldName == nameof(TicketEditModel.CategoryId));
    }

    [Fact]
    public void Closed_ticket_with_a_future_due_date_is_reported_against_DueDate()
    {
        var model = ValidModel();
        model.Status = TicketStatuses.Closed;
        model.DueDate = DateTime.Today.AddDays(7);

        var messages = _validator.Validate(model);

        var message = Assert.Single(messages, m => m.FieldName == nameof(TicketEditModel.DueDate));
        Assert.Contains("future due date", message.Message);
    }

    [Fact]
    public void Closed_ticket_with_todays_due_date_is_not_reported_the_rule_is_strictly_future()
    {
        var model = ValidModel();
        model.Status = TicketStatuses.Closed;
        model.DueDate = DateTime.Today;

        var messages = _validator.Validate(model);

        Assert.DoesNotContain(messages, m => m.FieldName == nameof(TicketEditModel.DueDate));
    }

    [Fact]
    public void Closed_ticket_with_a_past_due_date_is_not_reported()
    {
        var model = ValidModel();
        model.Status = TicketStatuses.Closed;
        model.DueDate = DateTime.Today.AddDays(-3);

        var messages = _validator.Validate(model);

        Assert.DoesNotContain(messages, m => m.FieldName == nameof(TicketEditModel.DueDate));
    }

    [Fact]
    public void An_open_ticket_with_a_future_due_date_is_not_reported_the_rule_only_applies_to_Closed()
    {
        var model = ValidModel();
        model.Status = TicketStatuses.Open;
        model.DueDate = DateTime.Today.AddDays(7);

        var messages = _validator.Validate(model);

        Assert.Empty(messages);
    }

    [Fact]
    public void Every_broken_rule_is_reported_at_once_not_just_the_first()
    {
        var model = ValidModel();
        model.Title = "";
        model.CustomerId = null;
        model.CategoryId = null;

        var messages = _validator.Validate(model);

        Assert.Contains(messages, m => m.FieldName == nameof(TicketEditModel.Title));
        Assert.Contains(messages, m => m.FieldName == nameof(TicketEditModel.CustomerId));
        Assert.Contains(messages, m => m.FieldName == nameof(TicketEditModel.CategoryId));
        Assert.Equal(3, messages.Count);
    }

    [Fact]
    public void Description_over_4000_characters_is_reported_against_Description()
    {
        var model = ValidModel();
        model.Description = new string('B', 4001);

        var messages = _validator.Validate(model);

        Assert.Contains(messages, m => m.FieldName == nameof(TicketEditModel.Description));
    }
}
