using OperationsConsole.Models;

namespace OperationsConsole.Orders
{
    /// <summary>
    /// The one place that knows which grid column shows which property of an <see cref="OrderRow"/>.
    /// <para>
    /// The bound path never needs this: <c>DataPropertyName</c> does the mapping. The virtual path does — the grid
    /// asks <c>CellValueNeeded</c> for "row 120 400, column 3" and something has to turn that column index into a
    /// value. Keeping the mapping here (instead of a <c>switch</c> inside the cell handler) means the cache, the
    /// command column and the designer all agree on the column order, and adding a column is a change in two places
    /// that sit next to each other rather than a bug that only shows when someone scrolls.
    /// </para>
    /// </summary>
    public static class OrderColumns
    {
        /// <summary>colNumber — the stable order ID.</summary>
        public const int Number = 0;

        /// <summary>colCustomer — plain text, never HTML.</summary>
        public const int Customer = 1;

        /// <summary>colDueDate — the only editable column (custom editor).</summary>
        public const int DueDate = 2;

        /// <summary>colTotal — right aligned, "C2".</summary>
        public const int Total = 3;

        /// <summary>colStatus — AllowHtml; the badge is built in CellFormatting.</summary>
        public const int Status = 4;

        /// <summary>colOpen — the command column ("Open").</summary>
        public const int Open = 5;

        /// <summary>How many columns the Orders grid has.</summary>
        public const int Count = 6;

        /// <summary>
        /// The raw value for one cell of one row. Raw on purpose: the status is the plain text, exactly like the bound
        /// path, so <c>CellFormatting</c> can build the same badge whichever path fed the cell.
        /// </summary>
        public static object ValueOf(OrderRow row, int columnIndex)
        {
            if (row == null)
                return null;

            switch (columnIndex)
            {
                case Number: return row.Number;
                case Customer: return row.Customer;
                case DueDate: return row.DueDate;
                case Total: return row.Total;
                case Status: return row.Status;
                case Open: return "Open";       // the button column uses its column text, this keeps the cell non-null
                default: return null;
            }
        }
    }
}
