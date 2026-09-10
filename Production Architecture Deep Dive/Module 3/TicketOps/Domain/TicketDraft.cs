namespace TicketOps.Domain
{
    /// <summary>
    /// What the detail form collects for a save: UI → data, before any decision is made.
    /// Id is null for a new ticket. The service validates it; the form never does.
    /// </summary>
    public sealed class TicketDraft
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public TicketPriority Priority { get; set; }
        public string Assignee { get; set; }
        public double HoursLogged { get; set; }
        public string Notes { get; set; }

        public override string ToString()
            => $"{{id:{(Id.HasValue ? Id.Value.ToString() : "new")}, title:\"{Title}\", priority:{Priority}, assignee:\"{Assignee}\", hours:{HoursLogged}}}";
    }
}
