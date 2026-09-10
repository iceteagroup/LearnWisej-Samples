using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderDesk.Domain
{
    /// <summary>
    /// The business logic of LegacyOrderDesk. This file is copied verbatim from the WinForms
    /// project into OrderDesk.Web — it is the "business logic — moves almost unchanged" half of
    /// the Module 1 diff. It has no UI dependency and no desktop assumption.
    /// </summary>
    public sealed class OrderService
    {
        // Tax is a jurisdiction setting in the real system; LegacyOrderDesk prices are tax-inclusive
        // for its reseller accounts, so the default rate is 0. The rule stays where it always was.
        public decimal TaxRate { get; set; } = 0m;

        private readonly IOrderRepository _repository;

        public OrderService() : this(InMemoryOrderRepository.Shared) { }

        public OrderService(IOrderRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // ── the order math — unchanged by the migration ─────────────────────────────

        public decimal CalculateOrderTotal(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            var subtotal = order.Lines.Sum(l => l.Quantity * l.UnitPrice);
            var discount = DiscountFor(order.Customer, subtotal);
            return Math.Round(subtotal - discount + Tax(subtotal), 2);
        }

        public decimal DiscountFor(Customer customer, decimal subtotal)
        {
            if (customer == null || customer.DiscountRate <= 0) return 0m;
            return Math.Round(subtotal * customer.DiscountRate, 2);
        }

        public decimal Tax(decimal subtotal) => Math.Round(subtotal * TaxRate, 2);

        // ── data access — reused as-is (the repository is in-memory for the labs) ───

        public IList<Order> GetOrders() => _repository.GetAll().OrderByDescending(o => o.Id).ToList();

        public Order Find(int id) => _repository.Find(id);

        public IList<Order> Search(OrderFilter filter)
        {
            IEnumerable<Order> query = _repository.GetAll();
            if (filter != null)
            {
                if (filter.Status.HasValue)
                    query = query.Where(o => o.Status == filter.Status.Value);
                if (!string.IsNullOrWhiteSpace(filter.Text))
                {
                    var text = filter.Text.Trim();
                    query = query.Where(o =>
                        o.CustomerName.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        o.Id.ToString().Contains(text) ||
                        (o.PoNumber ?? "").IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
                }
            }
            return query.OrderByDescending(o => o.Id).ToList();
        }

        /// <summary>Applies the business rule and persists. Returns the saved order.</summary>
        public Order Save(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (order.Customer == null) throw new InvalidOperationException("An order needs a customer.");
            order.Total = CalculateOrderTotal(order);
            if (order.Id == 0) order.Id = _repository.NextId();
            _repository.Save(order);
            return order;
        }

        public void Delete(int id) => _repository.Delete(id);
    }

    public sealed class OrderFilter
    {
        public string Text { get; set; }
        public OrderStatus? Status { get; set; }
    }

    public interface IOrderRepository
    {
        IEnumerable<Order> GetAll();
        Order Find(int id);
        void Save(Order order);
        void Delete(int id);
        int NextId();
    }

    /// <summary>
    /// The lab data store. One instance per process, guarded by a lock: on the desktop that lock
    /// never mattered (one process, one user); on the server every session shares it.
    /// </summary>
    public sealed class InMemoryOrderRepository : IOrderRepository
    {
        public static readonly InMemoryOrderRepository Shared = new InMemoryOrderRepository(SampleData.Orders());

        private readonly object _gate = new object();
        private readonly Dictionary<int, Order> _orders;

        public InMemoryOrderRepository(IEnumerable<Order> seed)
        {
            _orders = seed.ToDictionary(o => o.Id, o => o.Clone());
        }

        public IEnumerable<Order> GetAll()
        {
            lock (_gate) return _orders.Values.Select(o => o.Clone()).ToList();
        }

        public Order Find(int id)
        {
            lock (_gate) return _orders.TryGetValue(id, out var o) ? o.Clone() : null;
        }

        public void Save(Order order)
        {
            lock (_gate) _orders[order.Id] = order.Clone();
        }

        public void Delete(int id)
        {
            lock (_gate) _orders.Remove(id);
        }

        public int NextId()
        {
            lock (_gate) return _orders.Count == 0 ? 1000 : _orders.Keys.Max() + 1;
        }
    }
}
