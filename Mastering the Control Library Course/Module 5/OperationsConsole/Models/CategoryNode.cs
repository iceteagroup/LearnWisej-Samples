namespace OperationsConsole.Models
{
    /// <summary>
    /// One category (a customer folder or one of its document folders) as the explorer needs it:
    /// a <b>stable ID</b>, a display title, and whether it can be expanded.
    /// <para>
    /// The title is deliberately NOT unique — "Contracts" exists under Contoso, Fabrikam and Northwind.
    /// The tree stores <see cref="Id"/> in <c>TreeNode.Tag</c> (and <c>"cat-" + Id</c> in <c>TreeNode.Name</c>);
    /// no handler in this module ever keys on <c>Node.Text</c>.
    /// </para>
    /// </summary>
    public class CategoryNode
    {
        /// <summary>Stable business ID — what lands in <c>TreeNode.Tag</c>.</summary>
        public int Id { get; set; }

        /// <summary>What the user reads. Not unique, never used as a key.</summary>
        public string Title { get; set; }

        /// <summary>"Contoso ▸ Contracts" — shown in the detail control so the user sees which branch a document came from.</summary>
        public string Path { get; set; }

        /// <summary>True when the category has sub-categories, so the tree gives it a placeholder child (the expand glyph).</summary>
        public bool HasChildren { get; set; }

        /// <summary>How many documents live directly under this category (0 for the folder levels).</summary>
        public int DocumentCount { get; set; }

        /// <summary>The <c>TreeNode.Name</c> this category gets: "cat-7".</summary>
        public string NodeName => "cat-" + Id;
    }
}
