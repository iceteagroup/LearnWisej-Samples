using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OperationsConsole.Models;

namespace OperationsConsole.Services
{
    /// <summary>
    /// The in-memory customer store of the Operations Console. It holds the option lists, applies the
    /// service-level validation the editor cannot do on its own (uniqueness across records), takes a moment
    /// to answer so the busy state is visible, and fails on demand through <see cref="SimulateFailure"/>.
    /// </summary>
    public sealed class CustomerService
    {
        private readonly Dictionary<string, CustomerModel> _saved = new Dictionary<string, CustomerModel>(StringComparer.OrdinalIgnoreCase);
        private int _nextId;

        /// <summary>When true, <see cref="SaveAsync"/> throws instead of persisting.</summary>
        public bool SimulateFailure { get; set; }

        /// <summary>How long the simulated round trip takes.</summary>
        public int LatencyMilliseconds { get; set; } = 1200;

        /// <summary>Status list for <c>cboStatus</c>: stored "ACT", shown "Active".</summary>
        public static IList<OptionItem> StatusOptions()
        {
            return new List<OptionItem>
            {
                new OptionItem("PRO", "Prospect"),
                new OptionItem("ACT", "Active"),
                new OptionItem("HLD", "On hold"),
                new OptionItem("CLO", "Closed")
            };
        }

        /// <summary>Customer type list for <c>cboCustomerType</c>: stored "RSL", shown "Reseller".</summary>
        public static IList<OptionItem> CustomerTypeOptions()
        {
            return new List<OptionItem>
            {
                new OptionItem("DIR", "Direct"),
                new OptionItem("RSL", "Reseller"),
                new OptionItem("OEM", "OEM partner"),
                new OptionItem("GOV", "Government")
            };
        }

        /// <summary>An empty new record: the state the editor starts in.</summary>
        public CustomerModel CreateBlank()
        {
            return new CustomerModel
            {
                Name = "",
                Email = "",
                StatusKey = "PRO",
                CustomerTypeKey = "DIR",
                StartDate = DateTime.Today,
                CreditLimit = 0
            };
        }

        /// <summary>
        /// Rules that need the whole store, not one screen: only the service knows whether another customer
        /// already uses the email address. An empty list means "go ahead and save".
        /// </summary>
        public IList<string> ValidateForSave(CustomerModel customer)
        {
            var messages = new List<string>();

            if (customer == null)
            {
                messages.Add("There is nothing to save.");
                return messages;
            }

            foreach (var existing in _saved.Values)
            {
                if (!string.Equals(existing.Id, customer.Id, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(existing.Email, customer.Email, StringComparison.OrdinalIgnoreCase))
                {
                    messages.Add("The email address " + customer.Email + " already belongs to " + existing.Id + " (" + existing.Name + ").");
                }
            }

            return messages;
        }

        /// <summary>
        /// Persists the customer and returns the stored record (with its id and save stamp). Nothing is written
        /// when the call fails.
        /// </summary>
        public async Task<CustomerModel> SaveAsync(CustomerModel customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            await Task.Delay(LatencyMilliseconds);

            if (SimulateFailure)
                throw new InvalidOperationException("The customer service is not reachable.");

            var stored = customer.Clone();
            if (stored.IsNew)
            {
                _nextId++;
                stored.Id = "CUS-" + _nextId.ToString("0000");
            }

            stored.SavedAt = DateTime.Now;
            _saved[stored.Id] = stored;

            return stored.Clone();
        }

        /// <summary>The record behind an id, or null.</summary>
        public CustomerModel Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return _saved.TryGetValue(id, out var found) ? found.Clone() : null;
        }
    }
}
