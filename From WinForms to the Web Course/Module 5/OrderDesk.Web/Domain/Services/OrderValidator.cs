using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderDesk.Domain
{
    /// <summary>
    /// What a validation run found. <see cref="Errors"/> is keyed by field name so a form can put
    /// each message next to its control (ErrorProvider); <see cref="General"/> holds the messages
    /// that belong to the whole order. Plain data — no UI type anywhere.
    /// </summary>
    public sealed class ValidationResult
    {
        public Dictionary<string, string> Errors { get; } = new Dictionary<string, string>(StringComparer.Ordinal);

        public List<string> General { get; } = new List<string>();

        public bool HasErrors => Errors.Count > 0 || General.Count > 0;

        /// <summary>One line for the trace: "Customer: Customer is required. · PoNumber: PO number is required."</summary>
        public string Describe()
        {
            if (!HasErrors) return "valid";
            var parts = Errors.Select(e => $"{e.Key}: {e.Value}").Concat(General.Select(g => "order: " + g));
            return string.Join(" · ", parts);
        }
    }

    /// <summary>
    /// Module 5: the order rules taken OUT of EditOrderDialog.saveButton_Click. On the desktop the
    /// rule ("Select a customer.") lived inside the form and could only ever run there; here the
    /// same rules serve the dialog, a batch import or a web API, and they always run on the server
    /// where the data is — a browser can be bypassed, this class cannot.
    /// </summary>
    public static class OrderValidator
    {
        /// <summary>Field names used as keys in <see cref="ValidationResult.Errors"/>.</summary>
        public const string CustomerField = "Customer";
        public const string PoNumberField = "PoNumber";
        public const string QuantityField = "Quantity";
        public const string UnitPriceField = "UnitPrice";
        public const string TotalField = "Total";

        public const int PoNumberMaxLength = 20;

        public static ValidationResult Validate(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            var result = new ValidationResult();

            // 1. customer required
            if (order.Customer == null || order.CustomerId == 0)
                result.Errors[CustomerField] = "Customer is required.";

            // 2. PO number required, 20 characters at most
            var po = (order.PoNumber ?? "").Trim();
            if (po.Length == 0)
                result.Errors[PoNumberField] = "PO number is required.";
            else if (po.Length > PoNumberMaxLength)
                result.Errors[PoNumberField] = $"PO number is {PoNumberMaxLength} characters at most.";

            // 3. at least one line — a message about the order, not about one field
            if (order.Lines.Count == 0)
            {
                result.General.Add("Add at least one order line.");
                return result;
            }

            // 4. every line: quantity > 0, unit price ≥ 0
            if (order.Lines.Any(l => l.Quantity <= 0))
                result.Errors[QuantityField] = "Quantity must be at least 1.";
            if (order.Lines.Any(l => l.UnitPrice < 0))
                result.Errors[UnitPriceField] = "Unit price can't be negative.";

            // 5. the order total can't be negative (a credit note is a different document)
            if (order.Lines.Sum(l => l.LineTotal) < 0)
                result.Errors[TotalField] = "Total can't be negative.";

            // Owner is optional: an order can sit in the queue before somebody picks it up.
            return result;
        }
    }
}
