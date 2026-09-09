using System;

namespace WisejTrainingApp.Models
{
    /// <summary>
    /// The support ticket the course works with from Module 4 onwards.
    /// Plain properties only — the grid binds to them, the dialog reads and writes them.
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
    }
}
