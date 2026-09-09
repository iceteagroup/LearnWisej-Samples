using System;
using System.Collections.Generic;
using System.Linq;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// The customers behind the ticket list. Same pattern as TicketService, one size smaller: an in-memory
    /// list per session, a validated Add, and read methods that return copies.
    /// </summary>
    public class CustomerService
    {
        private readonly List<Customer> customers;

        public CustomerService()
        {
            // One contact per company the seeded tickets name.
            customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Nancy Davolio",   Company = "Northwind",            Email = "nancy@northwind.example" },
                new Customer { Id = 2, Name = "Andrew Fuller",   Company = "Contoso",              Email = "andrew@contoso.example" },
                new Customer { Id = 3, Name = "Janet Leverling", Company = "Fabrikam",             Email = "janet@fabrikam.example" },
                new Customer { Id = 4, Name = "Margaret Peacock", Company = "Adventure Works",     Email = "margaret@adventure-works.example" },
                new Customer { Id = 5, Name = "Steven Buchanan", Company = "Tailspin",             Email = "steven@tailspin.example" },
                new Customer { Id = 6, Name = "Michael Suyama",  Company = "Wide World Importers", Email = "michael@wwi.example" },
            };
        }

        public List<Customer> GetCustomers()
        {
            return customers.OrderBy(c => c.Id).ToList();
        }

        /// <summary>Company names for the ticket dialog's Customer field.</summary>
        public List<string> GetCompanyNames()
        {
            return customers.Select(c => c.Company).Distinct().OrderBy(c => c).ToList();
        }

        /// <summary>The rules for a new customer, returned as messages so the screen can show them.</summary>
        public ValidationResult ValidateCustomer(Customer c)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(c.Name))
                result.Add("Contact name is required.");
            if (string.IsNullOrWhiteSpace(c.Company))
                result.Add("Company is required.");
            else if (customers.Any(x => string.Equals(x.Company, c.Company.Trim(), StringComparison.OrdinalIgnoreCase)))
                result.Add($"Company \"{c.Company.Trim()}\" already exists.");
            if (string.IsNullOrWhiteSpace(c.Email) || !c.Email.Contains("@"))
                result.Add("Email must contain an @.");

            return result;
        }

        public void AddCustomer(Customer c)
        {
            if (c == null)
                throw new ArgumentNullException(nameof(c));

            var result = ValidateCustomer(c);
            if (!result.IsValid)
                throw new InvalidOperationException("Customer rejected: " + string.Join(" ", result.Errors));

            c.Id = customers.Count == 0 ? 1 : customers.Max(x => x.Id) + 1;
            c.Name = c.Name.Trim();
            c.Company = c.Company.Trim();
            c.Email = c.Email.Trim();
            customers.Add(c);
        }
    }
}
