using System;
using Wisej.Web;
using ValidationClinic.Services;

namespace ValidationClinic.Validation
{
    public sealed class MinimumAgeValidationRule : ValidationRule
    {
        public int MinimumAge { get; set; } = 18;
        public MinimumAgeValidationRule() : base("MinimumAge", "The contact must be at least 18 years old.") { }
        public override bool OnValidating(Control control) =>
            control is not DateTimePicker picker ||
            ContactValidator.IsOldEnough(picker.Value, DateTime.Today, MinimumAge);
        // No OnValidated/OnControlCreated override is needed: this rule has no formatting or client setup.
    }
}
