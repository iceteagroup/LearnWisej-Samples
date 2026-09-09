namespace OrderDesk.Domain
{
    /// <summary>
    /// A customer account. Plain data — no UI, no desktop assumptions, moves to the web unchanged.
    /// </summary>
    public sealed class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        /// <summary>Contract discount applied by OrderService.DiscountFor (0.0 – 1.0).</summary>
        public decimal DiscountRate { get; set; }

        public override string ToString() => Name;
    }
}
