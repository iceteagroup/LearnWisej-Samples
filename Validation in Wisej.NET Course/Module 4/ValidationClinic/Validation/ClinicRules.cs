using Wisej.Web;

namespace ValidationClinic.Validation
{
    public static class ClinicRules
    {
        // Return fresh instances: an extender may associate a rule with its control.
        public static ValidationRule[] RequiredName() => new ValidationRule[]
        { new RequiredValidationRule { Trim = true, InvalidMessage = "Name is required." } };
        public static ValidationRule[] RequiredEmail() => new ValidationRule[]
        {
            new RequiredValidationRule { Trim = true, InvalidMessage = "Email is required." },
            new EmailValidationRule { InvalidMessage = "Enter a valid email address." }
        };
        public static ValidationRule[] RequiredAge() => new ValidationRule[]
        {
            new RequiredValidationRule { Trim = true, InvalidMessage = "Age is required." },
            new IntegerValidationRule { InvalidMessage = "Age must be a whole number." }
        };
        public static ValidationRule[] RequiredPhone() => new ValidationRule[]
        {
            new RequiredValidationRule { Trim = true, InvalidMessage = "Phone is required." },
            new TelephoneValidationRule { Mask = "0000000000", InvalidMessage = "Enter a 10-digit phone number." }
        };
        public static ValidationRule[] CreditLimit() => new ValidationRule[]
        { new CurrencyValidationRule { InvalidMessage = "Enter a valid credit amount, such as 100.50." } };
        public static ValidationRule[] CustomerCode() => new ValidationRule[]
        { new RegexValidationRule { ValidateExpression = @"^CUS-\d{4}$", InvalidMessage = "Use CUS followed by a dash and four digits." } };
    }
}
