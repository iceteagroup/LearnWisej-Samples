namespace OrderDesk.Domain
{
    /// <summary>
    /// Server-side filter/sort/page model. On the desktop the grid got every row and filtered in
    /// memory; on the web the query travels to the service and only the requested slice comes back.
    /// </summary>
    public sealed class OrderQuery
    {
        public OrderStatus? Status { get; set; }
        public string Search { get; set; }
        public string SortBy { get; set; } = "Id";
        public bool Descending { get; set; } = true;
        public int Skip { get; set; }
        public int Take { get; set; } = int.MaxValue;

        public OrderQuery Clone() => (OrderQuery)MemberwiseClone();

        public override string ToString()
        {
            var take = Take == int.MaxValue ? "all" : Take.ToString();
            var status = Status.HasValue ? Status.ToString() : "any";
            return "status=" + status + " search=\"" + Search + "\" sort=" + SortBy + (Descending ? " desc" : "") + " skip=" + Skip + " take=" + take;
        }
    }
}
