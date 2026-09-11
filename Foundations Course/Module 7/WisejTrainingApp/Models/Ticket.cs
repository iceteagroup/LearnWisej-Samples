using System;

namespace WisejTrainingApp.Models
{
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
