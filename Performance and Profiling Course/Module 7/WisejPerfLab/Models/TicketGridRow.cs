namespace WisejPerfLab.Models
{
    /// <summary>
    /// A ticket as the grid shows it, and nothing else: the six displayed columns, already formatted.
    /// No navigation property, no <c>Description</c>, no <c>Notes</c>.
    /// </summary>
    /// <remarks>
    /// The rule this type enforces is the Module 5 rule: <b>no entity reaches the UI layer</b>. An entity
    /// carries every column the domain has, including two long text columns no list screen shows, and a
    /// navigation property that invites a query per row. A row type carries what one row of one grid
    /// displays — which is also, not by coincidence, what the update payload has to carry to the browser.
    /// </remarks>
    public sealed class TicketGridRow
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public string Customer { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }

        /// <summary>Precomputed: the grid never formats anything.</summary>
        public string AgeText { get; set; }

        public string UpdatedText { get; set; }
    }
}
