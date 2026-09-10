using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderDesk.Domain
{
    /// <summary>Order lifecycle states used by LegacyOrderDesk (unchanged by the migration).</summary>
    public enum OrderStatus
    {
        Open,
        InProgress,
        Shipped,
        Invoiced,
        Hold
    }

    public sealed class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        /// <summary>Gold customers get a discount over the discount threshold.</summary>
        public string Tier { get; set; } = "Standard";
        /// <summary>Sales-tax rate applied to the subtotal (0 = tax exempt).</summary>
        public decimal TaxRate { get; set; }
        public decimal CreditLimit { get; set; }
        /// <summary>Sensitive: never ship this to the browser as part of a grid row.</summary>
        public string TaxId { get; set; }

        public override string ToString() => Name;
    }

    public sealed class OrderLine
    {
        public string Sku { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal LineTotal => Quantity * UnitPrice;
    }

    /// <summary>
    /// The order aggregate. Pure data + the business rules that were already in the desktop app.
    /// Nothing in this file depends on System.Windows.Forms or Wisej.Web — that is what makes it
    /// a "direct reuse" item in the migration assessment.
    /// </summary>
    public sealed class Order
    {
        public int Id { get; set; }
        public string PoNumber { get; set; }
        public Customer Customer { get; set; }
        public string Owner { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime Date { get; set; }
        public List<OrderLine> Lines { get; set; } = new List<OrderLine>();
        public string Notes { get; set; }

        public string CustomerName => Customer?.Name ?? "";
        public int LineCount => Lines.Count;

        /// <summary>The total as the desktop app always computed it (see OrderCalculator).</summary>
        public decimal Total => OrderCalculator.CalculateOrderTotal(this);

        public Order Clone()
        {
            var copy = (Order)MemberwiseClone();
            copy.Lines = Lines.Select(l => new OrderLine { Sku = l.Sku, Description = l.Description, Quantity = l.Quantity, UnitPrice = l.UnitPrice }).ToList();
            return copy;
        }

        public override string ToString() => $"Order {Id} · {CustomerName} · {Total:C}";
    }

    /// <summary>
    /// The order math from LegacyOrderDesk.OrderService — "business logic — moves almost unchanged".
    /// </summary>
    public static class OrderCalculator
    {
        public const decimal GoldDiscountThreshold = 15000m;
        public const decimal GoldDiscountRate = 0.05m;

        public static decimal CalculateOrderTotal(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            var subtotal = order.Lines.Sum(l => l.Quantity * l.UnitPrice);
            var discount = DiscountFor(order.Customer, subtotal);
            return Math.Round(subtotal - discount + Tax(order.Customer, subtotal), 2);
        }

        public static decimal DiscountFor(Customer customer, decimal subtotal)
        {
            if (customer == null) return 0m;
            return customer.Tier == "Gold" && subtotal >= GoldDiscountThreshold
                ? Math.Round(subtotal * GoldDiscountRate, 2)
                : 0m;
        }

        public static decimal Tax(Customer customer, decimal subtotal)
        {
            var rate = customer?.TaxRate ?? 0m;
            return Math.Round(subtotal * rate, 2);
        }
    }
}
