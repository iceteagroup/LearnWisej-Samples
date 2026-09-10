using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OperationsConsole.Models;
using OperationsConsole.Shell;

namespace OperationsConsole.Services
{
    /// <summary>
    /// The in-memory customer store of the Operations Console (Module 2). It plays the part the real
    /// application would give to a repository or a web API: it holds the option lists, it applies the
    /// <b>service-level</b> validation the editor cannot do on its own (uniqueness across records),
    /// it takes time (<c>await Task.Delay(1200)</c>) so the busy state is visible, and it fails on
    /// demand through <see cref="SimulateFailure"/>.
    ///
    /// Every call is written to the Event log through <see cref="ConsoleLog"/>, so the learner can see
    /// which layer rejected a value: editor property, field validator, form validation, service validation.
    /// </summary>
    public sealed class CustomerService
    {
        // saved records by id — the "database" of this sample
        private readonly Dictionary<string, CustomerModel> _saved = new Dictionary<string, CustomerModel>(StringComparer.OrdinalIgnoreCase);
        private int _nextId;

        /// <summary>Failure path: the next <see cref="SaveAsync"/> throws instead of persisting.</summary>
        public bool SimulateFailure { get; set; }

        /// <summary>How long the simulated round trip takes — long enough to see the disabled commands.</summary>
        public int LatencyMilliseconds { get; set; } = 1200;

        // ------------------------------------------------------------------------------------------------
        // Option lists — the stored key is deliberately different from the display text
        // ------------------------------------------------------------------------------------------------

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

        /// <summary>Text of a stored key, for log lines and the docs ("ACT" → "Active").</summary>
        public static string TextOf(IList<OptionItem> options, string key)
        {
            foreach (var option in options)
            {
                if (string.Equals(option.Key, key, StringComparison.OrdinalIgnoreCase))
                    return option.Text;
            }
            return key;
        }

        // ------------------------------------------------------------------------------------------------
        // Sample records the command row loads
        // ------------------------------------------------------------------------------------------------

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

        /// <summary>A record that passes every rule — the success path.</summary>
        public CustomerModel CreateSample()
        {
            return new CustomerModel
            {
                Name = "ACME Manufacturing",
                Email = "orders@acme-manufacturing.com",
                StatusKey = "ACT",
                CustomerTypeKey = "RSL",
                StartDate = DateTime.Today.AddMonths(-8),
                CreditLimit = 25000
            };
        }

        /// <summary>
        /// A record that breaks four rules at once, so the learner sees four ErrorProvider marks together:
        /// no name, a malformed email, a credit limit below the minimum an Active customer needs,
        /// and a start date in the future while the status is Active.
        /// </summary>
        public CustomerModel CreateInvalidSample()
        {
            return new CustomerModel
            {
                Name = "",
                Email = "orders.acme-manufacturing",
                StatusKey = "ACT",
                CustomerTypeKey = "OEM",
                StartDate = DateTime.Today.AddMonths(3),
                CreditLimit = 500
            };
        }

        // ------------------------------------------------------------------------------------------------
        // Service-level validation — step 4 of the flow, the layer the editor cannot replace
        // ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Rules that need the whole store, not one screen: the editor can check the shape of an email
        /// address, only the service knows whether another customer already uses it. Returns the messages
        /// to show; an empty list means "go ahead and save".
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

            ConsoleLog.Add("CustomerService.ValidateForSave(" + Describe(customer) + ") → "
                + (messages.Count == 0 ? "ok" : messages.Count + " service rule(s) rejected"));

            return messages;
        }

        // ------------------------------------------------------------------------------------------------
        // Persistence
        // ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Persists the customer and returns the stored record (with its id and save stamp). Takes
        /// <see cref="LatencyMilliseconds"/> so the busy state is visible, and throws when
        /// <see cref="SimulateFailure"/> is on — nothing is written in that case, so no partial state is saved.
        /// </summary>
        public async Task<CustomerModel> SaveAsync(CustomerModel customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            ConsoleLog.Add("CustomerService.SaveAsync(" + Describe(customer) + ") → started, " + LatencyMilliseconds + " ms round trip");

            await Task.Delay(LatencyMilliseconds);

            if (SimulateFailure)
            {
                // the store is untouched: the record keeps the id (or stays new) it had before the call
                ConsoleLog.Add("CustomerService.SaveAsync → the simulated back end refused the call, nothing was written");
                throw new InvalidOperationException("The customer service is not reachable (simulated failure).");
            }

            var stored = customer.Clone();
            if (stored.IsNew)
            {
                _nextId++;
                stored.Id = "CUS-" + _nextId.ToString("0000");
                ConsoleLog.Add("CustomerService → new record, id " + stored.Id + " assigned");
            }

            stored.SavedAt = DateTime.Now;
            _saved[stored.Id] = stored;

            ConsoleLog.Add("CustomerService.SaveAsync → committed " + stored.Id + " (" + _saved.Count + " record(s) in memory)");
            return stored.Clone();
        }

        /// <summary>The record behind an id, or null — used by the section's Refresh.</summary>
        public CustomerModel Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return _saved.TryGetValue(id, out var found) ? found.Clone() : null;
        }

        /// <summary>How many records the in-memory store holds.</summary>
        public int Count => _saved.Count;

        private static string Describe(CustomerModel customer)
        {
            if (customer == null)
                return "null";

            return (customer.IsNew ? "new" : customer.Id)
                + " · \"" + customer.Name + "\" · " + customer.StatusKey
                + " · " + customer.CustomerTypeKey
                + " · " + customer.StartDate.ToString("yyyy-MM-dd")
                + " · limit " + customer.CreditLimit.ToString("0");
        }
    }
}
