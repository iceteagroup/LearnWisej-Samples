namespace WisejPerfLab.Models
{
    /// <summary>
    /// A ticket as the grid shows it: only the displayed columns, with the display strings already
    /// built. Projected <b>once</b> per search and kept — not rebuilt on every redraw.
    /// </summary>
    /// <remarks>
    /// Module 4 introduced this type to stop the per-redraw allocation: the page used to hold the
    /// entities and build a fresh row object, with five fresh strings, every time the grid was redrawn.
    /// It is still built from full entities that the service loaded — narrowing the <i>query</i> is
    /// Module 5, and removing the per-row customer lookup is Module 6. One problem at a time, each with
    /// its own measurement.
    /// </remarks>
    public sealed class TicketRow
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
