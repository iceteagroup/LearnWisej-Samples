using System.Collections.Generic;

namespace WisejPerfLab.Data.Entities
{
    /// <summary>
    /// A node of the customer hierarchy: region → account → site. The tree screen walks
    /// <see cref="ParentId"/>, so the same table is both the root list and every branch.
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }

        /// <summary>Null for the regions at the top of the tree.</summary>
        public int? ParentId { get; set; }

        public Customer Parent { get; set; }

        public ICollection<Customer> Children { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
