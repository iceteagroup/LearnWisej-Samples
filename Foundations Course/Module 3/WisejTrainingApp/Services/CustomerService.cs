using System.Collections.Generic;
using System.Linq;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>In-memory customer list, one instance per user session (MainPage owns it).</summary>
    public class CustomerService
    {
        private readonly List<Customer> customers;
        private int nextId;

        public CustomerService()
        {
            customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Northwind",            Contact = "Nancy Davolio",  City = "Seattle" },
                new Customer { Id = 2, Name = "Contoso",              Contact = "Paul Contreras", City = "Redmond" },
                new Customer { Id = 3, Name = "Fabrikam",             Contact = "Yvonne McKay",   City = "Denver" },
                new Customer { Id = 4, Name = "Adventure Works",      Contact = "Ken Sanchez",    City = "Bothell" },
                new Customer { Id = 5, Name = "Tailspin",             Contact = "Lena Barbosa",   City = "Lisbon" },
                new Customer { Id = 6, Name = "Wide World Importers", Contact = "Sven Ottlieb",   City = "Hamburg" },
            };
            nextId = 7;
        }

        public List<Customer> GetCustomers()
        {
            return customers.OrderBy(c => c.Id).ToList();
        }

        public Customer GetCustomer(int id)
        {
            return customers.FirstOrDefault(c => c.Id == id);
        }

        public void AddCustomer(Customer c)
        {
            c.Id = nextId++;
            customers.Add(c);
        }

        public int Count()
        {
            return customers.Count;
        }
    }
}
