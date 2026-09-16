namespace WisejPerfLab.Models
{
    /// <summary>The ticket search criteria. Part of the scenario: a measurement without them is not repeatable.</summary>
    public sealed class TicketFilter
    {
        public string Status { get; set; } = "Open";

        /// <summary>How many rows the screen asks for. The dataset stays at 50,000 either way.</summary>
        public int PageSize { get; set; } = 5000;
    }
}
