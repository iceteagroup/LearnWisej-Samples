using System;
using System.Collections.Generic;
using ValidationClinic.Models;

namespace ValidationClinic.Services
{
    public sealed record ValidationMessage(string Field, string Message);

    public sealed class ContactValidator
    {
        // The clock is an argument so birthdays can be tested without changing the machine date.
        public List<ValidationMessage> Validate(ContactEditModel model, DateTime? today = null)
        {
            var errors = new List<ValidationMessage>();
            var date = (today ?? DateTime.Today).Date;
            if (string.IsNullOrWhiteSpace(model.Name)) errors.Add(new("Name", "Enter the customer's name."));
            if (string.IsNullOrWhiteSpace(model.Email) || !model.Email.Contains('@')) errors.Add(new("Email", "Enter an email address containing @."));
            if (!model.BirthDate.HasValue) errors.Add(new("BirthDate", "Enter a birth date."));
            else if (model.BirthDate.Value.Date > date) errors.Add(new("BirthDate", "Birth date cannot be in the future."));
            else if (!IsOldEnough(model.BirthDate.Value, date, 18)) errors.Add(new("BirthDate", "The contact must be at least 18 years old."));
            var dates = ValidateDates(model.StartDate, model.EndDate);
            if (dates.Length > 0)
            {
                errors.Add(new("StartDate", dates));
                errors.Add(new("EndDate", dates));
            }
            if (model.Status == "Closed" && !model.ClosedDate.HasValue)
                errors.Add(new("ClosedDate", "Closed contacts require a closed date."));
            return errors;
        }

        public static bool IsOldEnough(DateTime birthDate, DateTime today, int minimumAge) =>
            birthDate.Date <= today.Date && birthDate.Date.AddYears(minimumAge) <= today.Date;

        public static string ValidateDates(DateTime? start, DateTime? end) =>
            !start.HasValue || !end.HasValue ? "Both dates are required." :
            start.Value.Date > end.Value.Date ? "Start date must be on or before end date." : "";
    }
}
