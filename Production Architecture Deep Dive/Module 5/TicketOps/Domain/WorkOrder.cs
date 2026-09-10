using System;
using System.Globalization;

namespace TicketOps.Domain
{
    public enum WorkOrderPriority
    {
        Low,
        Medium,
        High
    }

    /// <summary>The Work Order lifecycle. The legal moves between these live in <see cref="WorkOrderTransitions"/>.</summary>
    public enum WorkOrderStatus
    {
        New,
        Assigned,
        InProgress,
        OnHold,
        Completed,
        Closed,
        Cancelled
    }

    /// <summary>
    /// The work order record. Plain C# with no UI dependency: it would compile in a class library that
    /// never references Wisej.NET. <see cref="RowVersion"/> only changes when a transaction commits, so a
    /// reader can prove that a failed save changed nothing.
    /// </summary>
    public sealed class WorkOrder
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AssigneeId { get; set; }
        public WorkOrderPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public decimal EstimatedCost { get; set; }
        public double EstimatedHours { get; set; }
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.New;
        public int RowVersion { get; set; } = 1;

        public override string ToString()
            => string.Format(CultureInfo.InvariantCulture,
                "#{0} \"{1}\" · {2} · {3} · due {4:yyyy-MM-dd} · ${5:N2} · {6}h · v{7}",
                Id, Title, AssigneeId, Status, DueDate, EstimatedCost, EstimatedHours, RowVersion);
    }
}
