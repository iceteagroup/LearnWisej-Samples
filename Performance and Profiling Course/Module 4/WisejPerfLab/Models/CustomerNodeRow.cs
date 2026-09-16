namespace WisejPerfLab.Models
{
    /// <summary>One node of the customer tree, with the ticket count the label shows.</summary>
    public sealed class CustomerNodeRow
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string Name { get; set; }

        public int TicketCount { get; set; }

        /// <summary>"Northwind Traders - North (142)" — what the node says on screen.</summary>
        public string Label => Name + "  (" + TicketCount + ")";
    }
}
