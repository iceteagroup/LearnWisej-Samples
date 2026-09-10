using System;
using System.Collections.Generic;

namespace OrderDesk.Domain
{
    /// <summary>
    /// The application service the desktop event handlers already called. It has no UI dependency,
    /// so a Wisej.NET page calls exactly the same methods the WinForms form called.
    /// </summary>
    public sealed class OrderService
    {
        private readonly OrderStore _store;

        public OrderService() : this(OrderStore.Shared()) { }
        public OrderService(OrderStore store) { _store = store ?? throw new ArgumentNullException(nameof(store)); }

        public OrderStore Store => _store;

        public List<Order> GetAll() => _store.GetAll();
        public Order Find(int id) => _store.Find(id);
        public List<Order> Search(OrderQuery query) => _store.Search(query);
        public int Count(OrderQuery query) => _store.CountMatching(query);
        public IReadOnlyList<Customer> Customers => _store.Customers;
        public IReadOnlyList<string> Owners => _store.Owners;

        public decimal CalculateOrderTotal(Order order) => OrderCalculator.CalculateOrderTotal(order);

        /// <summary>Saves after the business rules pass; throws when they do not (the caller decides how to show it).</summary>
        public void Save(Order order)
        {
            var result = new OrderValidator().Validate(order);
            if (result.HasErrors)
                throw new ValidationException(result);
            _store.Save(order);
        }

        public Dictionary<OrderStatus, int> CountByStatus() => _store.CountByStatus();
    }
}
