namespace WisejPerfLab.Models
{
    /// <summary>
    /// What the ticket grid shows. In this module the page builds one of these for every entity it
    /// received, every time it redraws — which is exactly the allocation pattern Module 4 measures.
    /// </summary>
    public sealed class TicketDisplayRow
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public string Customer { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }

        public string AgeText { get; set; }

        public string UpdatedText { get; set; }
    }
}
