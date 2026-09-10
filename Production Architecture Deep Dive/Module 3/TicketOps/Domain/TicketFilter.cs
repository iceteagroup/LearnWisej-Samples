namespace TicketOps.Domain
{
    /// <summary>
    /// What the workspace asks the service to search for: free text from the SearchBar plus at most
    /// one filter chip from the FlowLayoutPanel ("Open", "High", "Mine", "Today", "Unassigned", "Escalated").
    /// The chip names are the vocabulary of the screen; what they mean is decided in the service.
    /// </summary>
    public sealed class TicketFilter
    {
        public const string ChipOpen = "Open";
        public const string ChipHigh = "High";
        public const string ChipMine = "Mine";
        public const string ChipToday = "Today";
        public const string ChipUnassigned = "Unassigned";
        public const string ChipEscalated = "Escalated";

        public static readonly string[] Chips = { ChipOpen, ChipHigh, ChipMine, ChipToday, ChipUnassigned, ChipEscalated };

        public string Text { get; set; } = "";
        public string Chip { get; set; }

        public bool IsEmpty => string.IsNullOrWhiteSpace(Text) && string.IsNullOrEmpty(Chip);

        public override string ToString()
            => $"{{text:\"{Text}\", chip:{(string.IsNullOrEmpty(Chip) ? "none" : Chip)}}}";
    }
}
