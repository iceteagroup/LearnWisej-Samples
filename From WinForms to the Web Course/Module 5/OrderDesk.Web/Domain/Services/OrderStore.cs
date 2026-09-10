using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace OrderDesk.Domain
{
    /// <summary>
    /// The production-like data set of Module 5: 200,000 orders, seeded once per process (lazily,
    /// thread-safe) so every browser session queries the same store — exactly like a database.
    /// <see cref="InMemoryOrderRepository.Shared"/> (the five walkthrough orders) is left alone.
    /// </summary>
    public static class OrderStore
    {
        /// <summary>The walkthrough's number: "SELECT * · 200k rows".</summary>
        public const int LargeCount = 200_000;

        private static readonly Lazy<LargeOrderRepository> _large =
            new Lazy<LargeOrderRepository>(Seed, LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>The big store. The first session that touches it pays the seed time once.</summary>
        public static LargeOrderRepository Large => _large.Value;

        /// <summary>True once a session has seeded the store.</summary>
        public static bool IsSeeded => _large.IsValueCreated;

        /// <summary>How long the one-time seed took (for the trace).</summary>
        public static TimeSpan SeedElapsed { get; private set; }

        private static LargeOrderRepository Seed()
        {
            var watch = Stopwatch.StartNew();
            var repository = new LargeOrderRepository(Generate(LargeCount));
            watch.Stop();
            SeedElapsed = watch.Elapsed;
            return repository;
        }

        // ── the generator ─────────────────────────────────────────────────────────────

        private static readonly string[] Owners = { "dana", "priya", "sam", "kelly", "" };

        private static readonly string[] PoPrefixes = { "NW", "CT", "FB", "AW", "GX", "TT", "WW", "LW" };

        private static readonly OrderLine[] Catalog =
        {
            new OrderLine { Sku = "WJ-DEV-SEAT",  Description = "Developer seat",        UnitPrice = 190.00m },
            new OrderLine { Sku = "WJ-CTRL-STD",  Description = "Control library seat",  UnitPrice = 350.00m },
            new OrderLine { Sku = "WJ-ENT-SEAT",  Description = "Enterprise seat",       UnitPrice = 495.00m },
            new OrderLine { Sku = "WJ-SRV-CORE",  Description = "Server core license",   UnitPrice = 990.50m },
            new OrderLine { Sku = "WJ-SUP-GOLD",  Description = "Gold support, 1 year",  UnitPrice = 330.00m },
            new OrderLine { Sku = "WJ-SUP-PLAT",  Description = "Platinum support",      UnitPrice = 500.00m },
            new OrderLine { Sku = "WJ-TRN-DAY",   Description = "On-site training day",  UnitPrice = 150.00m },
            new OrderLine { Sku = "WJ-THEME-PK",  Description = "Theme pack",            UnitPrice = 1030.00m },
        };

        /// <summary>
        /// The five walkthrough orders first (ids 1042 … 1038, the newest by date), then generated
        /// orders with ids 1043 upward and dates spread over the eight years before them, so
        /// "newest first" (Date desc) still opens on 1042 Northwind Traders. One line per order keeps
        /// 200,000 orders around 60 MB of managed heap. Deterministic: the same seed every run.
        /// </summary>
        private static IEnumerable<Order> Generate(int count)
        {
            var walkthrough = SampleData.Orders().ToList();
            foreach (var order in walkthrough)
                yield return order;

            var random = new Random(20260910);
            var math = new OrderService(new InMemoryOrderRepository(Array.Empty<Order>()));
            var customers = SampleData.Customers;
            var newest = new DateTime(2026, 9, 3);
            int generated = count - walkthrough.Count;
            double perDay = generated / 3167.0;              // ≈ 63 orders a day since January 2018

            for (int i = 0; i < generated; i++)
            {
                int customerIndex = random.Next(customers.Length);
                var customer = customers[customerIndex];
                var template = Catalog[random.Next(Catalog.Length)];
                var order = new Order
                {
                    Id = 1043 + i,
                    CustomerId = customer.Id,
                    Customer = customer,
                    PoNumber = PoPrefixes[customerIndex] + "-" + random.Next(1000, 99999).ToString(),
                    Owner = Owners[random.Next(Owners.Length)],
                    Status = StatusFor(random.Next(100)),
                    CreatedOn = newest.AddDays(-(int)((generated - 1 - i) / perDay))
                };
                order.Lines.Add(new OrderLine
                {
                    Sku = template.Sku, Description = template.Description, UnitPrice = template.UnitPrice, Quantity = 1 + random.Next(25)
                });
                order.Total = math.CalculateOrderTotal(order);
                yield return order;
            }
        }

        /// <summary>Open 40 % · InProgress 15 % · Shipped 20 % · Invoiced 20 % · Hold 5 %.</summary>
        private static OrderStatus StatusFor(int roll)
        {
            if (roll < 40) return OrderStatus.Open;
            if (roll < 55) return OrderStatus.InProgress;
            if (roll < 75) return OrderStatus.Shipped;
            if (roll < 95) return OrderStatus.Invoiced;
            return OrderStatus.Hold;
        }
    }

    /// <summary>
    /// An <see cref="IOrderRepository"/> sized for the Module 5 test. It keeps the same contract as
    /// <see cref="InMemoryOrderRepository"/> (GetAll clones the whole table — that IS the desktop
    /// "SELECT *") and adds what the web grid needs: <see cref="Query"/> and <see cref="Summarize"/>
    /// filter, sort and page under the lock and clone only the rows that leave the store.
    /// </summary>
    public sealed class LargeOrderRepository : IOrderRepository
    {
        private readonly object _gate = new object();
        private readonly Dictionary<int, Order> _orders = new Dictionary<int, Order>();
        private int _maxId;
        private int _version;

        // The last ordered result set (references, not clones) keyed by OrderQuery.Key: block 2..n
        // of the same filter + sort only Skip/Take into it. Invalidated by any write.
        private string _memoKey;
        private int _memoVersion = -1;
        private List<Order> _memoRows;

        public LargeOrderRepository(IEnumerable<Order> seed)
        {
            if (seed == null) throw new ArgumentNullException(nameof(seed));
            foreach (var order in seed)
            {
                _orders[order.Id] = order;
                if (order.Id > _maxId) _maxId = order.Id;
            }
        }

        public int Count
        {
            get { lock (_gate) return _orders.Count; }
        }

        // ── IOrderRepository — the contract OrderService already uses ─────────────────

        /// <summary>Every order, cloned. 200,000 clones per call: the cost the naive port pays.</summary>
        public IEnumerable<Order> GetAll()
        {
            lock (_gate) return _orders.Values.Select(o => o.Clone()).ToList();
        }

        public Order Find(int id)
        {
            lock (_gate) return _orders.TryGetValue(id, out var order) ? order.Clone() : null;
        }

        public void Save(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            lock (_gate)
            {
                _orders[order.Id] = order.Clone();
                if (order.Id > _maxId) _maxId = order.Id;
                _version++;
            }
        }

        public void Delete(int id)
        {
            lock (_gate)
            {
                if (_orders.Remove(id)) _version++;
            }
        }

        public int NextId()
        {
            lock (_gate) return _orders.Count == 0 ? 1000 : _maxId + 1;
        }

        // ── Module 5: filter, sort and page on the server ─────────────────────────────

        /// <summary>One page of the filtered, ordered table. Only <c>query.Take</c> rows are cloned.</summary>
        public PagedResult<Order> Query(OrderQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            var watch = Stopwatch.StartNew();
            lock (_gate)
            {
                var rows = Ordered(query, out bool fromCache);
                var page = rows.Skip(query.Skip).Take(query.Take).Select(o => o.Clone()).ToList();
                watch.Stop();
                return new PagedResult<Order>(page, rows.Count, watch.Elapsed, fromCache);
            }
        }

        /// <summary>How many rows the filter matches — without cloning any of them.</summary>
        public int CountMatching(OrderQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            lock (_gate) return Ordered(query, out _).Count;
        }

        /// <summary>The Σ row: count and sum of the filtered table, computed where the data is.</summary>
        public OrderSummary Summarize(OrderQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            var watch = Stopwatch.StartNew();
            lock (_gate)
            {
                var rows = Ordered(query, out _);
                decimal total = 0m;
                foreach (var order in rows) total += order.Total;
                watch.Stop();
                return new OrderSummary(rows.Count, total, watch.Elapsed);
            }
        }

        /// <summary>Filter + sort, memoized per filter/sort key until the next write. Call under the lock.</summary>
        private List<Order> Ordered(OrderQuery query, out bool fromCache)
        {
            string key = query.Key;
            if (_memoRows != null && _memoVersion == _version && _memoKey == key)
            {
                fromCache = true;
                return _memoRows;
            }

            IEnumerable<Order> rows = _orders.Values;
            if (query.Status.HasValue)
                rows = rows.Where(o => o.Status == query.Status.Value);
            if (query.CustomerId.HasValue)
                rows = rows.Where(o => o.CustomerId == query.CustomerId.Value);
            if (!string.IsNullOrWhiteSpace(query.Text))
            {
                // The same three fields OrderService.Search has always looked at.
                var text = query.Text.Trim();
                rows = rows.Where(o =>
                    o.CustomerName.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    o.Id.ToString().Contains(text) ||
                    (o.PoNumber ?? "").IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            _memoRows = Sort(rows, query.SortBy, query.Descending).ToList();
            _memoKey = key;
            _memoVersion = _version;
            fromCache = false;
            return _memoRows;
        }

        /// <summary>Stable order: the requested column, then Id descending so paging never shows a row twice.</summary>
        private static IEnumerable<Order> Sort(IEnumerable<Order> rows, OrderSort sortBy, bool descending)
        {
            switch (sortBy)
            {
                case OrderSort.Customer:
                    return descending
                        ? rows.OrderByDescending(o => o.CustomerName, StringComparer.OrdinalIgnoreCase).ThenByDescending(o => o.Id)
                        : rows.OrderBy(o => o.CustomerName, StringComparer.OrdinalIgnoreCase).ThenByDescending(o => o.Id);
                case OrderSort.Total:
                    return descending ? rows.OrderByDescending(o => o.Total).ThenByDescending(o => o.Id) : rows.OrderBy(o => o.Total).ThenByDescending(o => o.Id);
                case OrderSort.Status:
                    return descending ? rows.OrderByDescending(o => o.Status).ThenByDescending(o => o.Id) : rows.OrderBy(o => o.Status).ThenByDescending(o => o.Id);
                case OrderSort.Date:
                    return descending ? rows.OrderByDescending(o => o.CreatedOn).ThenByDescending(o => o.Id) : rows.OrderBy(o => o.CreatedOn).ThenByDescending(o => o.Id);
                default:
                    return descending ? rows.OrderByDescending(o => o.Id) : rows.OrderBy(o => o.Id);
            }
        }
    }

    /// <summary>
    /// The business-facing entry point for the grid: the query the browser needs, nothing more.
    /// Sits next to <see cref="OrderService"/> (which keeps its Module 1 API untouched) and reads
    /// from <see cref="OrderStore.Large"/> unless a repository is injected.
    /// </summary>
    public sealed class OrderQueryService
    {
        private readonly LargeOrderRepository _repository;

        public OrderQueryService() : this(OrderStore.Large) { }

        public OrderQueryService(LargeOrderRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>The rows the viewport asked for (Skip/Take) plus the total the filter matches.</summary>
        public PagedResult<Order> Page(OrderQuery query) => _repository.Query(query);

        /// <summary>The total the filter matches — what VirtualMode needs for RowCount.</summary>
        public int Count(OrderQuery query) => _repository.CountMatching(query);

        /// <summary>Count + sum for the Σ row.</summary>
        public OrderSummary Summarize(OrderQuery query) => _repository.Summarize(query);
    }
}
