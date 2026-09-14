using System;

namespace WisejPerfLab.Data.Entities
{
    /// <summary>
    /// The support ticket entity as the application first models it: every column the domain has,
    /// including the two long text columns no list screen ever shows. Module 5 and Module 6 are about
    /// what it costs to hand this whole object to the UI layer.
    /// </summary>
    public class Ticket
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }

        public string Subject { get; set; }

        /// <summary>Long text. Never shown in a list, always fetched by a `SELECT *` style query.</summary>
        public string Description { get; set; }

        /// <summary>Long text. Same story as <see cref="Description"/>.</summary>
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public int MinutesToFirstResponse { get; set; }
    }
}
