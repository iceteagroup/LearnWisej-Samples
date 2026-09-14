namespace WisejPerfLab.Models
{
    /// <summary>One node of the customer tree, with the ticket count the label shows.</summary>
    public sealed class CustomerNodeRow
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string Name { get; set; }

        public int TicketCount { get; set; }

        /// <summary>
        /// How many children this node has. A lazy tree needs it before it loads them: it decides
        /// whether the node gets an expander at all, and it is what the placeholder says.
        /// </summary>
        public int ChildCount { get; set; }

        /// <summary>
        /// What the node says on screen. A branch carries the <b>count placeholder</b> — how many nodes are
        /// inside it — so a collapsed branch is still informative without loading anything; a leaf carries
        /// its ticket count. Both numbers come from the two grouped queries that loaded the level.
        /// </summary>
        public string Label => ChildCount > 0
            ? Name + "  (" + ChildCount.ToString("N0") + " below)"
            : Name + "  (" + TicketCount.ToString("N0") + " tickets)";
    }
}
