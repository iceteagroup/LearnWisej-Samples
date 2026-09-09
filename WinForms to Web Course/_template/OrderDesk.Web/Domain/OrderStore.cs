using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderDesk.Domain
{
    /// <summary>
    /// The in-memory "database". Deterministic sample data so every module shows the same orders
    /// the walkthrough videos show (1042 Northwind Traders $4,820.00 Open, 1041 Contoso Ltd
    /// $1,290.50 Shipped, 1040 Fabrikam Inc $760.00 Open, 1039 Adventure Works $12,400.00 Invoiced,
    /// 1038 Globex Corp $3,090.00 Open) followed by generated history.
    ///
    /// This is shared reference data guarded by a lock: one of the statics that MAY stay static in
    /// a multi-user server (Module 4). Per-user state never lives here.
    /// </summary>
    public sealed class OrderStore
    {
        private static readonly object Gate = new object();
        private static OrderStore _shared;

        /// <summary>The process-wide store. Created on first use with at least <paramref name="count"/> orders.</summary>
        public static OrderStore Shared(int count = 5000)
        {
            lock (Gate)
            {
                if (_shared == null || _shared.Count < count)
                    _shared = Create(count);
                return _shared;
            }
        }

        public static OrderStore Create(int count, int seed = 42)
        {
            var store = new OrderStore();
            store.Seed(count, seed);
            return store;
        }

        private readonly List<Order> _orders = new List<Order>();
        private readonly Dictionary<int, Order> _byId = new Dictionary<int, Order>();
        private readonly object _gate = new object();

        public IReadOnlyList<Customer> Customers { get; private set; }
        public IReadOnlyList<string> Owners { get; } = new[] { "Dana", "Priya", "Sam", "Kelly", "Alex", "Jordan" };

        public int Count { get { lock (_gate) return _orders.Count; } }

        public Order Find(int id)
        {
            lock (_gate) return _byId.TryGetValue(id, out var o) ? o : null;
        }

        /// <summary>The whole table — what the desktop grid used to bind to.</summary>
        public List<Order> GetAll()
        {
            lock (_gate) return _orders.OrderByDescending(o => o.Id).ToList();
        }

        public int CountMatching(OrderQuery query)
        {
            lock (_gate) return Filter(query ?? new OrderQuery()).Count();
        }

        /// <summary>Filter, sort and page on the server; only the slice leaves this method.</summary>
        public List<Order> Search(OrderQuery query)
        {
            if (query == null) query = new OrderQuery();
            lock (_gate)
            {
                IEnumerable<Order> rows = Filter(query);
                rows = Sort(rows, query);
                if (query.Skip > 0) rows = rows.Skip(query.Skip);
                if (query.Take != int.MaxValue) rows = rows.Take(query.Take);
                return rows.ToList();
            }
        }

        public void Save(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            lock (_gate)
            {
                if (_byId.TryGetValue(order.Id, out var existing) && !ReferenceEquals(existing, order))
                {
                    existing.PoNumber = order.PoNumber;
                    existing.Customer = order.Customer;
                    existing.Owner = order.Owner;
                    existing.Status = order.Status;
                    existing.Notes = order.Notes;
                    existing.Lines = order.Lines.Select(l => new OrderLine { Sku = l.Sku, Description = l.Description, Quantity = l.Quantity, UnitPrice = l.UnitPrice }).ToList();
                }
                else if (!_byId.ContainsKey(order.Id))
                {
                    if (order.Id == 0) order.Id = _orders.Count == 0 ? 1 : _orders.Max(o => o.Id) + 1;
                    _orders.Add(order);
                    _byId[order.Id] = order;
                }
            }
        }

        public Dictionary<OrderStatus, int> CountByStatus()
        {
            lock (_gate) return _orders.GroupBy(o => o.Status).ToDictionary(g => g.Key, g => g.Count());
        }

        private IEnumerable<Order> Filter(OrderQuery q)
        {
            IEnumerable<Order> rows = _orders;
            if (q.Status.HasValue) rows = rows.Where(o => o.Status == q.Status.Value);
            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var s = q.Search.Trim();
                rows = rows.Where(o =>
                    o.Id.ToString().Contains(s) ||
                    (o.CustomerName != null && o.CustomerName.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (o.Owner != null && o.Owner.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (o.PoNumber != null && o.PoNumber.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0));
            }
            return rows;
        }

        private static IEnumerable<Order> Sort(IEnumerable<Order> rows, OrderQuery q)
        {
            switch (q.SortBy ?? "Id")
            {
                case "Customer": return q.Descending ? rows.OrderByDescending(o => o.CustomerName) : rows.OrderBy(o => o.CustomerName);
                case "Total": return q.Descending ? rows.OrderByDescending(o => o.Total) : rows.OrderBy(o => o.Total);
                case "Status": return q.Descending ? rows.OrderByDescending(o => o.Status) : rows.OrderBy(o => o.Status);
                case "Owner": return q.Descending ? rows.OrderByDescending(o => o.Owner) : rows.OrderBy(o => o.Owner);
                case "Date": return q.Descending ? rows.OrderByDescending(o => o.Date) : rows.OrderBy(o => o.Date);
                default: return q.Descending ? rows.OrderByDescending(o => o.Id) : rows.OrderBy(o => o.Id);
            }
        }

        private void Seed(int count, int seed)
        {
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Northwind Traders", Country = "US", Tier = "Gold", TaxRate = 0m, CreditLimit = 50000m, TaxId = "US-77-1042001" },
                new Customer { Id = 2, Name = "Contoso Ltd", Country = "UK", TaxRate = 0m, CreditLimit = 20000m, TaxId = "GB-882-1041" },
                new Customer { Id = 3, Name = "Fabrikam Inc", Country = "US", TaxRate = 0m, CreditLimit = 15000m, TaxId = "US-31-7760" },
                new Customer { Id = 4, Name = "Adventure Works", Country = "CA", Tier = "Gold", TaxRate = 0m, CreditLimit = 80000m, TaxId = "CA-12400-AW" },
                new Customer { Id = 5, Name = "Globex Corp", Country = "US", TaxRate = 0m, CreditLimit = 30000m, TaxId = "US-30-9090" },
                new Customer { Id = 6, Name = "Tailspin Toys", Country = "DE", TaxRate = 0.19m, CreditLimit = 12000m, TaxId = "DE-811-TT" },
                new Customer { Id = 7, Name = "Wide World Importers", Country = "US", TaxRate = 0.07m, CreditLimit = 60000m, TaxId = "US-45-WWI" },
                new Customer { Id = 8, Name = "Alpine Ski House", Country = "AT", TaxRate = 0.20m, CreditLimit = 9000m, TaxId = "AT-U-ASH" },
                new Customer { Id = 9, Name = "Litware Inc", Country = "US", Tier = "Gold", TaxRate = 0.06m, CreditLimit = 45000m, TaxId = "US-90-LIT" },
                new Customer { Id = 10, Name = "Proseware", Country = "IE", TaxRate = 0.23m, CreditLimit = 18000m, TaxId = "IE-PRO-23" },
                new Customer { Id = 11, Name = "Coho Winery", Country = "US", TaxRate = 0.0825m, CreditLimit = 7000m, TaxId = "US-CO-HO" },
                new Customer { Id = 12, Name = "Margie's Travel", Country = "US", TaxRate = 0m, CreditLimit = 5000m, TaxId = "US-MT-01" },
            };
            Customers = customers;

            // The five orders every video shows, with lines that produce the exact totals.
            AddSeed(1042, customers[0], "NW-88231", "Dana", OrderStatus.Open, new DateTime(2026, 9, 8), 10, 482.00m, "Desk lamp, brass");
            AddSeed(1041, customers[1], "CT-55019", "Priya", OrderStatus.Shipped, new DateTime(2026, 9, 7), 5, 258.10m, "Monitor arm");
            AddSeed(1040, customers[2], "FB-20017", null, OrderStatus.Open, new DateTime(2026, 9, 7), 4, 190.00m, "Cable kit");
            AddSeed(1039, customers[3], "AW-77120", "Sam", OrderStatus.Invoiced, new DateTime(2026, 9, 6), 8, 1550.00m, "Standing desk");
            AddSeed(1038, customers[4], "GX-11003", "Kelly", OrderStatus.Open, new DateTime(2026, 9, 5), 6, 515.00m, "Office chair");

            // Generated history below 1038.
            var rnd = new Random(seed);
            var skus = new[]
            {
                new { Sku = "LAMP-01", Desc = "Desk lamp", Price = 48.00m },
                new { Sku = "ARM-02", Desc = "Monitor arm", Price = 129.90m },
                new { Sku = "CBL-03", Desc = "Cable kit", Price = 19.50m },
                new { Sku = "DSK-04", Desc = "Standing desk", Price = 775.00m },
                new { Sku = "CHR-05", Desc = "Office chair", Price = 257.50m },
                new { Sku = "KBD-06", Desc = "Keyboard", Price = 89.00m },
                new { Sku = "HUB-07", Desc = "USB-C hub", Price = 64.00m },
                new { Sku = "PAD-08", Desc = "Desk pad", Price = 32.00m },
            };
            var statuses = new[] { OrderStatus.Open, OrderStatus.Open, OrderStatus.InProgress, OrderStatus.Shipped, OrderStatus.Shipped, OrderStatus.Invoiced, OrderStatus.Invoiced, OrderStatus.Invoiced, OrderStatus.Hold };
            var date = new DateTime(2026, 9, 5);
            int generated = Math.Max(0, count - 5);
            for (int id = 1037; id > 1037 - generated; id--)
            {
                var c = customers[rnd.Next(customers.Count)];
                var o = new Order
                {
                    Id = id,
                    Customer = c,
                    PoNumber = c.Name.Substring(0, 2).ToUpperInvariant() + "-" + rnd.Next(10000, 99999),
                    Owner = rnd.Next(10) < 8 ? Owners[rnd.Next(Owners.Count)] : null,
                    Status = statuses[rnd.Next(statuses.Length)],
                    Date = date,
                };
                int lines = 1 + rnd.Next(3);
                for (int i = 0; i < lines; i++)
                {
                    var s = skus[rnd.Next(skus.Length)];
                    o.Lines.Add(new OrderLine { Sku = s.Sku, Description = s.Desc, Quantity = 1 + rnd.Next(12), UnitPrice = s.Price });
                }
                if (rnd.Next(7) == 0) date = date.AddDays(-1);
                _orders.Add(o);
                _byId[id] = o;
            }
        }

        private void AddSeed(int id, Customer c, string po, string owner, OrderStatus status, DateTime date, int qty, decimal price, string desc)
        {
            var o = new Order { Id = id, Customer = c, PoNumber = po, Owner = owner, Status = status, Date = date };
            o.Lines.Add(new OrderLine { Sku = "SEED-" + id, Description = desc, Quantity = qty, UnitPrice = price });
            _orders.Add(o);
            _byId[id] = o;
        }
    }
}
