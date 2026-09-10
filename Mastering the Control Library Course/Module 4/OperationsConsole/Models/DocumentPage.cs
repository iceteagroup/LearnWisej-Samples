using System.Collections.Generic;

namespace OperationsConsole.Models
{
    /// <summary>
    /// The <b>server-side page</b> that sits between <c>DocumentService</c> and the virtual-mode ListView.
    /// <para>
    /// This is the object the reading calls "a server-side page or cache that the service filled when the
    /// category changed": <c>RetrieveVirtualItem</c> reads <see cref="Items"/> by index and never asks the
    /// service (let alone a database) for a single row. <see cref="Total"/> is what
    /// <c>documentList.VirtualListSize</c> is set to.
    /// </para>
    /// <para>
    /// In this sample the page holds every row of the category, so <c>Items.Count == Total</c>. In a real
    /// application the page would be a window (<c>Skip</c>/<c>Take</c>) plus an index-keyed cache that
    /// <c>CacheVirtualItems</c> tops up; the shape of this class would not change.
    /// </para>
    /// </summary>
    public class DocumentPage
    {
        /// <summary>The category the page belongs to — checked before the page is used, so a late reply cannot fill the wrong list.</summary>
        public int CategoryId { get; set; }

        /// <summary>"Contoso ▸ Contracts".</summary>
        public string CategoryPath { get; set; }

        /// <summary>The rows this page holds, in list order.</summary>
        public IList<DocumentSummary> Items { get; set; } = new List<DocumentSummary>();

        /// <summary>How many rows the category has in total — this is <c>VirtualListSize</c>.</summary>
        public int Total { get; set; }

        /// <summary>True when the category holds no documents at all (the "No documents in this category" state).</summary>
        public bool IsEmpty => this.Total == 0;
    }
}
