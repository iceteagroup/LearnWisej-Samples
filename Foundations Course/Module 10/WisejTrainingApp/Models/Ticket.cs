using System;

namespace WisejTrainingApp.Models
{
    /// <summary>
    /// The support ticket the course works with from Module 4 onwards — the same shape in every module,
    /// so the samples line up. Plain properties only: the grid binds to them, the dialog reads and writes them,
    /// the validator checks them. No behaviour lives here.
    /// </summary>
    public class Ticket
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Customer { get; set; }

        /// <summary>Open | In Progress | Closed</summary>
        public string Status { get; set; }

        /// <summary>Low | Medium | High</summary>
        public string Priority { get; set; }

        public string AssignedTo { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Description { get; set; }

        /// <summary>A copy for the Edit dialog, so Cancel never changes the ticket the grid is showing.</summary>
        public Ticket Clone()
        {
            return new Ticket
            {
                Id = Id,
                Title = Title,
                Customer = Customer,
                Status = Status,
                Priority = Priority,
                AssignedTo = AssignedTo,
                CreatedDate = CreatedDate,
                Description = Description,
            };
        }
    }
}
