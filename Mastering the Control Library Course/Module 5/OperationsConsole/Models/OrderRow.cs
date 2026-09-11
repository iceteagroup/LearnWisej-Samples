using System;

namespace OperationsConsole.Models
{
    /// <summary>
    /// The Orders grid view model (Module 5 · DataGridView Mastery).
    /// <para>
    /// It is a <b>view model</b>, not an entity: it carries exactly the five values the Orders screen shows plus the
    /// stable order number used as the record ID, and nothing about persistence. That is what keeps the explicit
    /// columns honest — a column binds to <see cref="Number"/>, <see cref="Customer"/>, <see cref="DueDate"/>,
    /// <see cref="Total"/> or <see cref="Status"/> and to nothing that a repository might rename tomorrow.
    /// </para>
    /// <para>
    /// <see cref="Status"/> holds the <b>plain</b> status text. The badge markup the grid shows is produced in
    /// <c>DataGridViewPage.ordersGrid_CellFormatting</c> and never stored here: display formatting is not model data.
    /// </para>
    /// </summary>
    public class OrderRow
    {
        /// <summary>Stable ID of the order ("SO-100417"). Separate from the display text — the command column and
        /// <c>ShellStatus.Record(...)</c> use this, never the cell text of another column.</summary>
        public string Number { get; set; }

        /// <summary>Customer name as typed by a human — it may contain &amp;, &lt; or &gt;, which is why any HTML the
        /// screen builds around it has to be encoded.</summary>
        public string Customer { get; set; }

        /// <summary>The date the order is due. The only editable value on this screen.</summary>
        public DateTime DueDate { get; set; }

        /// <summary>Order total in the tenant currency; formatted with "C2" by the column's cell style, not here.</summary>
        public decimal Total { get; set; }

        /// <summary>Plain status text ("Open", "Confirmed", "Packed", "Shipped", "On hold", "Cancelled").</summary>
        public string Status { get; set; }

        /// <inheritdoc/>
        public override string ToString() => Number + " · " + Customer;
    }
}
