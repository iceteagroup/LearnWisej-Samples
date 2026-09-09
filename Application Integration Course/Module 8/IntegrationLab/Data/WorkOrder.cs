using System;

namespace IntegrationLab.Data
{
    /// <summary>Lifecycle of a work order.</summary>
    public enum WorkOrderStatus
    {
        Open,
        InProgress,
        Done
    }

    /// <summary>
    /// One maintenance work order. This is the in-memory "database row": the
    /// endpoints never hand this type to the browser directly, they map it to
    /// <see cref="WorkOrderRow"/> (the DTO) so the wire shape is explicit.
    /// </summary>
    public sealed class WorkOrder
    {
        public WorkOrder(int number, string asset, WorkOrderStatus status, string priority, string assignee, DateTime dueDate, double hours)
        {
            this.Number = number;
            this.Asset = asset;
            this.Status = status;
            this.Priority = priority;
            this.Assignee = assignee;
            this.DueDate = dueDate;
            this.Hours = hours;
        }

        /// <summary>Numeric part of the identifier (1042 for "WO-1042").</summary>
        public int Number { get; }

        /// <summary>Display identifier, e.g. "WO-1042".</summary>
        public string Id => "WO-" + this.Number;

        public string Asset { get; }
        public WorkOrderStatus Status { get; }

        /// <summary>Low, Normal, High or Urgent.</summary>
        public string Priority { get; }

        public string Assignee { get; }
        public DateTime DueDate { get; }

        /// <summary>Estimated effort in hours.</summary>
        public double Hours { get; }

        /// <summary>The DTO that crosses the wire (both endpoints return the same shape).</summary>
        public WorkOrderRow ToRow() => new WorkOrderRow
        {
            Id = this.Id,
            Asset = this.Asset,
            Status = this.Status.ToString(),
            Priority = this.Priority,
            Assignee = this.Assignee,
            DueDate = this.DueDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            Hours = this.Hours
        };
    }
}
