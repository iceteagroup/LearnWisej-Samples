namespace WisejTrainingApp.Models
{
    /// <summary>
    /// A helpdesk customer: the contact person and the company a ticket's <see cref="Ticket.Customer"/> names.
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }

        /// <summary>Contact person.</summary>
        public string Name { get; set; }

        /// <summary>The company name tickets refer to (Ticket.Customer).</summary>
        public string Company { get; set; }

        public string Email { get; set; }
    }
}
