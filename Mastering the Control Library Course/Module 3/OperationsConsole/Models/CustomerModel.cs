using System;

namespace OperationsConsole.Models
{
    /// <summary>
    /// The value the <c>CustomerEditor</c> reads and writes. Every property is already a typed value:
    /// the editors (DateTimePicker, NumericUpDown, ComboBox) produce a <see cref="DateTime"/>, a
    /// <see cref="decimal"/> and a stored key, so nothing on the Save path parses text.
    /// </summary>
    public sealed class CustomerModel
    {
        /// <summary>"CUS-0001" once the service has saved it; null or empty while the record is new.</summary>
        public string Id { get; set; }

        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        /// <summary>The stored key of <c>cboStatus</c> ("ACT", "PRO", "HLD", "CLO") — never the display text.</summary>
        public string StatusKey { get; set; } = "PRO";

        /// <summary>The stored key of <c>cboCustomerType</c> ("DIR", "RSL", "OEM", "GOV").</summary>
        public string CustomerTypeKey { get; set; } = "DIR";

        public DateTime StartDate { get; set; } = DateTime.Today;

        public decimal CreditLimit { get; set; }

        /// <summary>Stamped by <c>CustomerService.SaveAsync</c>.</summary>
        public DateTime? SavedAt { get; set; }

        /// <summary>True until the service has given the record an id.</summary>
        public bool IsNew => string.IsNullOrEmpty(Id);

        /// <summary>
        /// The editor keeps a clone as the "last saved" baseline, so <c>IsDirty</c> and <c>Reset()</c>
        /// compare against a snapshot the user cannot change by typing.
        /// </summary>
        public CustomerModel Clone()
        {
            return new CustomerModel
            {
                Id = Id,
                Name = Name,
                Email = Email,
                StatusKey = StatusKey,
                CustomerTypeKey = CustomerTypeKey,
                StartDate = StartDate,
                CreditLimit = CreditLimit,
                SavedAt = SavedAt
            };
        }

        /// <summary>Value comparison of the six edited fields (the id and the stamp are service state, not user input).</summary>
        public bool HasSameValues(CustomerModel other)
        {
            if (other == null)
                return false;

            return string.Equals(Name, other.Name, StringComparison.Ordinal)
                && string.Equals(Email, other.Email, StringComparison.Ordinal)
                && string.Equals(StatusKey, other.StatusKey, StringComparison.Ordinal)
                && string.Equals(CustomerTypeKey, other.CustomerTypeKey, StringComparison.Ordinal)
                && StartDate.Date == other.StartDate.Date
                && CreditLimit == other.CreditLimit;
        }
    }
}
