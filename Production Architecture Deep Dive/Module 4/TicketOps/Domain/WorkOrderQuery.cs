using System;

namespace TicketOps.Domain
{
    /// <summary>
    /// What the toolbar collects: a free-text search and an optional status. UI → data, before any
    /// decision is made. The matching rule lives here (<see cref="Matches"/>) so the service applies it
    /// to the loaded rows, to an imported batch, or in a unit test — never inline in a TextChanged handler.
    /// </summary>
    public sealed class WorkOrderQuery
    {
        public static readonly WorkOrderQuery Everything = new WorkOrderQuery(null, null);

        public WorkOrderQuery(string text, WorkOrderStatus? status)
        {
            Text = (text ?? "").Trim();
            Status = status;
        }

        public string Text { get; }
        public WorkOrderStatus? Status { get; }

        public bool IsEmpty => Text.Length == 0 && Status == null;

        public bool Matches(WorkOrder order)
        {
            if (order == null)
                return false;

            if (Status.HasValue && order.Status != Status.Value)
                return false;

            if (Text.Length == 0)
                return true;

            return Contains(order.Title, Text) || Contains(order.AssignedTo, Text);
        }

        private static bool Contains(string haystack, string needle)
            => !string.IsNullOrEmpty(haystack) && haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;

        public override string ToString()
            => $"{{text:\"{Text}\", status:{(Status.HasValue ? Status.Value.ToString() : "All")}}}";
    }
}
