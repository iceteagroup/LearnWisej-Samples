namespace OperationsConsole.Models
{
    /// <summary>
    /// One entry of the Layouts section: a named region of the console and the container decision behind it.
    /// The cards are what the <c>FlowLayoutPanel</c> of <c>Sections/LayoutsPage</c> wraps, and what the
    /// StatusBar counts as "records" for that section.
    /// </summary>
    public sealed class LayoutCard
    {
        public LayoutCard(string id, string region, string container, string rule)
        {
            Id = id;
            Region = region;
            Container = container;
            Rule = rule;
        }

        /// <summary>Stable ID shown in the StatusBar ("LAY-003").</summary>
        public string Id { get; }

        /// <summary>The region of the shell this card describes ("Navigation").</summary>
        public string Region { get; }

        /// <summary>The container that was chosen for it ("SplitContainer.Panel1").</summary>
        public string Container { get; }

        /// <summary>The layout decision rule that drove the choice ("the user should control the space").</summary>
        public string Rule { get; }

        public override string ToString() => Id + " · " + Region;
    }
}
