using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderDesk.Domain
{
    /// <summary>Customer lookup — read-mostly reference data, reused as-is by the web app.</summary>
    public sealed class CustomerService
    {
        public IList<Customer> GetCustomers() => SampleData.Customers.Select(Clone).OrderBy(c => c.Name).ToList();

        public Customer Find(int id) => SampleData.Customers.Where(c => c.Id == id).Select(Clone).FirstOrDefault();

        public Customer FindByName(string name) =>
            SampleData.Customers.Where(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)).Select(Clone).FirstOrDefault();

        private static Customer Clone(Customer c) => new Customer { Id = c.Id, Name = c.Name, City = c.City, Country = c.Country, DiscountRate = c.DiscountRate };
    }
}
