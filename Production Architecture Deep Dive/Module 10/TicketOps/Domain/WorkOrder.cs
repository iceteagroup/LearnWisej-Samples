using System;

namespace TicketOps.Domain
{
    /// <summary>
    /// The four statuses every screen of the console shows. The StatusChip renders them; the theme colours them;
    /// the resources name them (Status.Open, Status.InProgress, …). Nothing here knows about any of that.
    /// </summary>
    public enum WorkOrderStatus
    {
        Open,
        InProgress,
        Blocked,
        Done
    }

    /// <summary>
    /// A maintenance work order and its own rule. Plain C# with no UI, no Wisej.NET and no culture:
    /// the dates and amounts are raw values, and the screen formats them for the operator's culture.
    /// User-facing reasons are returned as resource KEYS (Rule.*), so the domain stays language-neutral
    /// and the service resolves them for the session's culture.
    /// </summary>
    public sealed class WorkOrder
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;
        public DateTime CreatedOn { get; set; }
        public DateTime DueOn { get; set; }
        public decimal LaborCost { get; set; }
        public double Hours { get; set; }

        /// <summary>Open → In progress → Blocked → Done; a finished order cannot be advanced.</summary>
        public bool CanAdvance(out string reasonKey)
        {
            if (Status == WorkOrderStatus.Done)
            {
                reasonKey = "Rule.DoneCannotAdvance";
                return false;
            }

            reasonKey = null;
            return true;
        }

        public WorkOrderStatus NextStatus()
        {
            switch (Status)
            {
                case WorkOrderStatus.Open: return WorkOrderStatus.InProgress;
                case WorkOrderStatus.InProgress: return WorkOrderStatus.Blocked;
                case WorkOrderStatus.Blocked: return WorkOrderStatus.Done;
                default: return WorkOrderStatus.Done;
            }
        }

        public void Advance()
        {
            if (!CanAdvance(out string reasonKey))
                throw new InvalidOperationException(reasonKey);

            Status = NextStatus();
        }
    }
}
