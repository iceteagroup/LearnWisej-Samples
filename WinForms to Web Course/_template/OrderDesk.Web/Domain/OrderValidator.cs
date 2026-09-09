using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderDesk.Domain
{
    /// <summary>Field-level validation outcome: one message per offending field.</summary>
    public sealed class ValidationResult
    {
        private readonly Dictionary<string, string> _errors = new Dictionary<string, string>();

        public bool HasErrors => _errors.Count > 0;
        public IReadOnlyDictionary<string, string> Errors => _errors;

        public void Add(string field, string message)
        {
            if (!_errors.ContainsKey(field)) _errors[field] = message;
        }

        public override string ToString() => HasErrors ? string.Join("; ", _errors.Select(e => e.Key + ": " + e.Value)) : "valid";
    }

    public sealed class ValidationException : Exception
    {
        public ValidationResult Result { get; }
        public ValidationException(ValidationResult result) : base("Validation failed — " + result) { Result = result; }
    }

    /// <summary>
    /// The reusable rule — no UI dependency. The same class serves the edit dialog, a batch import
    /// and a web API; the caller renders field-level messages (ErrorProvider), not a MessageBox.
    /// </summary>
    public sealed class OrderValidator
    {
        public ValidationResult Validate(Order o)
        {
            var r = new ValidationResult();
            if (o == null) { r.Add("Order", "No order."); return r; }
            if (o.Customer == null || string.IsNullOrWhiteSpace(o.Customer.Name))
                r.Add(nameof(o.Customer), "Customer is required.");
            if (string.IsNullOrWhiteSpace(o.Owner))
                r.Add(nameof(o.Owner), "Assign an owner before saving.");
            if (o.Lines == null || o.Lines.Count == 0)
                r.Add(nameof(o.Lines), "Add at least one order line.");
            else if (o.Lines.Any(l => l.Quantity <= 0))
                r.Add(nameof(o.Lines), "Every line needs a quantity of 1 or more.");
            if (o.Total < 0)
                r.Add(nameof(o.Total), "Total can't be negative.");
            if (o.Customer != null && o.Total > o.Customer.CreditLimit)
                r.Add(nameof(o.Total), "Total " + o.Total.ToString("C") + " exceeds the credit limit " + o.Customer.CreditLimit.ToString("C") + ".");
            return r;
        }
    }
}
