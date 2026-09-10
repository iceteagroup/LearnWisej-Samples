using OperationsConsole.Models;

namespace OperationsConsole.Orders
{
    /// <summary>
    /// What <c>OrderService.UpdateDueDate</c> answers: accepted or rejected, plus a message written for the user.
    /// <para>
    /// A rejected edit is <b>not</b> an exception — it is a normal business answer, and the grid handles it by putting
    /// the old value back and telling the user why. Exceptions are reserved for "the service did not answer at all".
    /// This is also why the validation lives in the service and not in <c>CellFormatting</c> or in the cell handler:
    /// the same rule has to hold whether the date comes from the in-cell editor, from the command row, or from an
    /// import nobody has written yet.
    /// </para>
    /// </summary>
    public class OrderUpdateResult
    {
        private OrderUpdateResult(bool accepted, string message, OrderRow order)
        {
            Accepted = accepted;
            Message = message;
            Order = order;
        }

        /// <summary>True when the service stored the new value.</summary>
        public bool Accepted { get; }

        /// <summary>A sentence the user can read — no exception text, no internals.</summary>
        public string Message { get; }

        /// <summary>The order as it stands now (the stored value, whether or not the edit was accepted).</summary>
        public OrderRow Order { get; }

        public static OrderUpdateResult Ok(OrderRow order, string message) => new OrderUpdateResult(true, message, order);

        public static OrderUpdateResult Rejected(OrderRow order, string message) => new OrderUpdateResult(false, message, order);
    }
}
