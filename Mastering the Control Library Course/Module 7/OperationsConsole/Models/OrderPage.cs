using System.Collections.Generic;

namespace OperationsConsole.Models
{
    /// <summary>
    /// One page of orders as the service returns it: the rows, where they start in the filtered result and how big the
    /// whole result is. This is the unit the large-data path moves around — <c>CellValueNeeded</c> asks
    /// <c>OrderCache</c> for a single cell, the cache answers from a page like this one and only fetches a new page
    /// when the requested row is outside every page it holds.
    /// </summary>
    public class OrderPage
    {
        public OrderPage(int firstIndex, IList<OrderRow> rows, int totalCount)
        {
            FirstIndex = firstIndex;
            Rows = rows ?? new List<OrderRow>();
            TotalCount = totalCount;
        }

        /// <summary>Index of <see cref="Rows"/>[0] inside the whole filtered result.</summary>
        public int FirstIndex { get; }

        /// <summary>The rows of this page (never null; may be empty at the end of the result).</summary>
        public IList<OrderRow> Rows { get; }

        /// <summary>How many rows the filtered result has in total — what the grid's <c>RowCount</c> is set from.</summary>
        public int TotalCount { get; }

        /// <summary>Index of the row after the last one in this page.</summary>
        public int NextIndex => FirstIndex + Rows.Count;

        /// <summary>True when <paramref name="index"/> is served by this page without going back to the service.</summary>
        public bool Contains(int index) => index >= FirstIndex && index < NextIndex;

        /// <summary>The row at an absolute index, or null when this page does not hold it.</summary>
        public OrderRow this[int index] => Contains(index) ? Rows[index - FirstIndex] : null;

        /// <inheritdoc/>
        public override string ToString() => "rows " + FirstIndex + "–" + (NextIndex - 1) + " of " + TotalCount;
    }
}
