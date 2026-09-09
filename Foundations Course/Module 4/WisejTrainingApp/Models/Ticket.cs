using System;

namespace WisejTrainingApp.Models
{
    /// <summary>
    /// The shape of one support ticket — the Module 4 model (lesson s16 §2).
    /// Plain properties only: no UI, no data access, nothing that knows about a grid or a textbox.
    /// The same class is used by Modules 5, 7 and 10 so the samples line up.
    /// </summary>
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Customer { get; set; }

        /// <summary>Open | In Progress | Closed.</summary>
        public string Status { get; set; }

        /// <summary>Low | Medium | High.</summary>
        public string Priority { get; set; }

        public string AssignedTo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
    }
}
