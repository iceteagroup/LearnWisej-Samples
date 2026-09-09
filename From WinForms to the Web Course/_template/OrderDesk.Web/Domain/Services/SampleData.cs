using System;
using System.Collections.Generic;

namespace OrderDesk.Domain
{
    /// <summary>
    /// The orders LegacyOrderDesk ships with (the same five the course videos show).
    /// </summary>
    public static class SampleData
    {
        public static readonly Customer[] Customers =
        {
            new Customer { Id = 1, Name = "Northwind Traders", City = "Seattle",     Country = "US" },
            new Customer { Id = 2, Name = "Contoso Ltd",       City = "London",      Country = "UK" },
            new Customer { Id = 3, Name = "Fabrikam Inc",      City = "Toronto",     Country = "CA" },
            new Customer { Id = 4, Name = "Adventure Works",   City = "Bothell",     Country = "US" },
            new Customer { Id = 5, Name = "Globex Corp",       City = "Springfield", Country = "US" },
            new Customer { Id = 6, Name = "Tailspin Toys",     City = "Denver",      Country = "US", DiscountRate = 0.05m },
            new Customer { Id = 7, Name = "Wide World Importers", City = "Hamburg",  Country = "DE" },
            new Customer { Id = 8, Name = "Litware Inc",       City = "Austin",      Country = "US" },
        };

        public static IEnumerable<Order> Orders()
        {
            var svc = new OrderService(new InMemoryOrderRepository(Array.Empty<Order>()));
            var list = new List<Order>
            {
                Make(1042, 1, "NW-88231", "dana",  OrderStatus.Open,     new DateTime(2026, 9, 8),
                    L("WJ-CTRL-STD", "Control library seat", 10, 350.00m),
                    L("WJ-SUP-GOLD", "Gold support, 1 year", 4, 330.00m)),                       // 4,820.00
                Make(1041, 2, "CT-10077", "priya", OrderStatus.Shipped,  new DateTime(2026, 9, 7),
                    L("WJ-SRV-CORE", "Server core license", 1, 990.50m),
                    L("WJ-TRN-DAY",  "On-site training day", 2, 150.00m)),                       // 1,290.50
                Make(1040, 3, "FB-2210",  "",      OrderStatus.Open,     new DateTime(2026, 9, 7),
                    L("WJ-DEV-SEAT", "Developer seat", 4, 190.00m)),                             // 760.00
                Make(1039, 4, "AW-5501",  "sam",   OrderStatus.Invoiced, new DateTime(2026, 9, 5),
                    L("WJ-ENT-SEAT", "Enterprise seat", 20, 495.00m),
                    L("WJ-SUP-PLAT", "Platinum support", 5, 500.00m)),                           // 12,400.00
                Make(1038, 5, "GX-0042",  "kelly", OrderStatus.Hold,     new DateTime(2026, 9, 4),
                    L("WJ-THEME-PK", "Theme pack", 3, 1030.00m)),                                // 3,090.00
            };
            foreach (var order in list)
                order.Total = svc.CalculateOrderTotal(order);
            return list;
        }

        private static Order Make(int id, int customerId, string po, string owner, OrderStatus status, DateTime created, params OrderLine[] lines)
        {
            var customer = Array.Find(Customers, c => c.Id == customerId);
            var order = new Order { Id = id, CustomerId = customerId, Customer = customer, PoNumber = po, Owner = owner, Status = status, CreatedOn = created };
            order.Lines.AddRange(lines);
            return order;
        }

        private static OrderLine L(string sku, string description, int qty, decimal price) =>
            new OrderLine { Sku = sku, Description = description, Quantity = qty, UnitPrice = price };
    }
}
