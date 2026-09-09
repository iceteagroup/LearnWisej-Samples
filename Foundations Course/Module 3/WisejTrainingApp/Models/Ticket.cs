using System;

namespace WisejTrainingApp.Models
{
    /// <summary>
    /// The support ticket every Foundations module shares (Modules 3, 4, 5, 7 and 10 use the same shape).
    /// Status: Open | In Progress | Closed.  Priority: Low | Medium | High.
    /// </summary>
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Customer { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
    }
}
