using System;
using System.Collections.Generic;

namespace OrderDesk.Domain
{
    public enum OrderStatus
    {
        Open,
        InProgress,
        Shipped,
        Invoiced,
        Hold
    }

    /// <summary>One line of an order. Plain data.</summary>
    public sealed class OrderLine
    {
        public string Sku { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal LineTotal => Quantity * UnitPrice;
    }

    /// <summary>
    /// An order as LegacyOrderDesk has always modelled it. The class carries no reference to
    /// System.Windows.Forms or Wisej.Web, which is why it ports unchanged.
    /// </summary>
    public sealed class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string PoNumber { get; set; }
        public string Owner { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<OrderLine> Lines { get; set; } = new List<OrderLine>();

        /// <summary>Set by OrderService.Save: the total the business rule computed.</summary>
        public decimal Total { get; set; }

        public string CustomerName => Customer?.Name ?? "";

        public Order Clone()
        {
            var copy = (Order)MemberwiseClone();
            copy.Lines = new List<OrderLine>();
            foreach (var line in Lines)
                copy.Lines.Add(new OrderLine { Sku = line.Sku, Description = line.Description, Quantity = line.Quantity, UnitPrice = line.UnitPrice });
            return copy;
        }
    }
}
