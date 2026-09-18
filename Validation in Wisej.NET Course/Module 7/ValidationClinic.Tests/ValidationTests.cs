using ValidationClinic.Models;
using ValidationClinic.Services;

public class ValidationTests
{
    private static ContactEditModel Valid() => new() { Name = "Maria Chen", Email = "maria@example.com", Age = 30 };

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(120, true)]
    [InlineData(121, false)]
    public void Annotation_age_boundaries(int age, bool accepted)
    {
        var model = Valid();
        model.Age = age;
        Assert.Equal(accepted, ModelValidation.ValidateModel(model).Count == 0);
    }

    [Theory]
    [InlineData("", "maria@example.com", false)]
    [InlineData("   ", "maria@example.com", false)]
    [InlineData("Maria", "", false)]
    [InlineData("Maria", "maria.example.com", false)]
    [InlineData("Maria", "maria@example.com", true)]
    public void Required_and_email_annotations(string name, string email, bool accepted)
    {
        var model = Valid(); model.Name = name; model.Email = email;
        Assert.Equal(accepted, ModelValidation.ValidateModel(model).Count == 0);
    }

    [Theory]
    [InlineData(80, true)]
    [InlineData(81, false)]
    public void Name_length_boundary(int length, bool accepted)
    {
        var model = Valid(); model.Name = new string('x', length);
        Assert.Equal(accepted, ModelValidation.ValidateModel(model).Count == 0);
    }

    [Fact]
    public void Service_rejects_email_without_a_browser()
    {
        var model = Valid(); model.Email = "no-at-sign";
        Assert.Contains(new ContactValidator().Validate(model), x => x.Field == "Email");
    }

    [Fact]
    public void Eighteenth_birthday_and_day_before_are_distinct()
    {
        var birth = new DateTime(2008, 9, 18);
        Assert.False(ContactValidator.IsOldEnough(birth, new DateTime(2026, 9, 17), 18));
        Assert.True(ContactValidator.IsOldEnough(birth, new DateTime(2026, 9, 18), 18));
        Assert.True(ContactValidator.IsOldEnough(new DateTime(2008, 2, 29), new DateTime(2026, 2, 28), 18));
    }

    [Fact]
    public void Future_and_missing_birth_dates_are_rejected()
    {
        var model = Valid();
        model.BirthDate = DateTime.Today.AddDays(1);
        Assert.Contains(new ContactValidator().Validate(model), x => x.Field == "BirthDate");
        model.BirthDate = null;
        Assert.Contains(new ContactValidator().Validate(model), x => x.Field == "BirthDate");
    }

    [Fact]
    public void Cross_field_rule_marks_both_dates_and_accepts_equal_dates()
    {
        var model = Valid(); model.EndDate = model.StartDate.Value.AddDays(-1);
        var errors = new ContactValidator().Validate(model);
        Assert.Contains(errors, x => x.Field == "StartDate");
        Assert.Contains(errors, x => x.Field == "EndDate");
        Assert.Equal("", ContactValidator.ValidateDates(model.StartDate, model.StartDate));
        Assert.NotEmpty(ContactValidator.ValidateDates(null, model.EndDate));
    }

    [Fact]
    public void Closed_contact_requires_a_date()
    {
        var model = Valid(); model.Status = "Closed";
        Assert.Contains(new ContactValidator().Validate(model), x => x.Field == "ClosedDate");
        model.ClosedDate = DateTime.Today;
        Assert.DoesNotContain(new ContactValidator().Validate(model), x => x.Field == "ClosedDate");
    }

    [Fact]
    public void Editing_a_snapshot_does_not_persist()
    {
        var repository = new ContactRepository();
        repository.Snapshot()[0].Name = "Unsaved";
        Assert.Equal("Ada Lovelace", repository.Snapshot()[0].Name);
        Assert.Equal(0, repository.WriteCount);
    }

    [Fact]
    public void Duplicate_name_is_case_insensitive_and_atomic()
    {
        var repository = new ContactRepository(); var model = Valid(); model.Name = " ada lovelace ";
        var grid = repository.Snapshot(); grid[1].Email = "edited@example.com";
        Assert.Throws<DuplicateNameException>(() => repository.SaveAll(grid, model));
        Assert.Equal("grace@example.com", repository.Snapshot()[1].Email);
        Assert.Equal(0, repository.WriteCount);
    }

    [Fact]
    public void Failed_write_preserves_data_and_retry_succeeds_once()
    {
        var repository = new ContactRepository { FailNextSave = true }; var model = Valid();
        Assert.Throws<InvalidOperationException>(() => repository.Save(model));
        Assert.Equal(2, repository.Snapshot().Count);
        Assert.Equal(0, model.Id);
        repository.Save(model);
        Assert.Equal(3, repository.Snapshot().Count);
        Assert.Equal(1, repository.WriteCount);
        model.Email = "updated@example.com";
        repository.Save(model);
        Assert.Equal(3, repository.Snapshot().Count);
        Assert.Equal("updated@example.com", repository.Snapshot().Single(x => x.Id == model.Id).Email);
    }

    [Fact]
    public void Data_error_model_clears_only_the_corrected_property()
    {
        var model = new ContactErrorModel();
        Assert.NotEmpty(model["Name"]); Assert.NotEmpty(model["Email"]);
        model.Name = "Maria";
        Assert.Empty(model["Name"]); Assert.NotEmpty(model["Email"]);
        Assert.Empty(model["Unknown"]);
    }
}
