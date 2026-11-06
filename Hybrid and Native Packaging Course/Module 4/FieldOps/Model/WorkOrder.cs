using System;

namespace FieldOps.Model
{
    /// <summary>
    /// Where a work order is in its life. The order of the values IS the transition rule:
    /// an order only ever moves to the next value, never backwards and never skipping one.
    /// </summary>
    public enum WorkOrderStatus
    {
        Assigned = 0,
        Accepted = 1,
        OnSite = 2,
        Completed = 3,
    }

    public enum WorkOrderPriority
    {
        Low,
        Normal,
        High,
    }

    /// <summary>
    /// One field-service job. Plain data: no UI type, no Wisej type, nothing that cares which
    /// client opened the screen - the same object serves the browser and every Hybrid client.
    /// </summary>
    public sealed class WorkOrder
    {
        public string Number { get; init; }
        public string Customer { get; init; }
        public string SiteAddress { get; init; }
        public string Fault { get; init; }
        public string Asset { get; init; }
        public DateTime DueAt { get; init; }
        /// <summary>The last submitted meter reading (Module 4), null until one is submitted.</summary>
        public decimal? MeterReading { get; set; }
        public string Notes { get; set; }
        public WorkOrderPriority Priority { get; init; }
        public WorkOrderStatus Status { get; set; }
        public string AssignedTechnician { get; init; }

        /// <summary>Bumped by every saved change; a screen holding an older value is stale.</summary>
        public int Version { get; set; }

        public DateTime LastChangedUtc { get; set; }

        /// <summary>"Customer · asset", the Site / asset column.</summary>
        public string SiteAndAsset => string.IsNullOrEmpty(Asset) ? Customer : Customer + " · " + Asset;

        /// <summary>"11:30" today, "Tomorrow", or a date.</summary>
        public string DueText =>
            DueAt.Date == DateTime.Today ? DueAt.ToString("HH:mm") :
            DueAt.Date == DateTime.Today.AddDays(1) ? "Tomorrow" : DueAt.ToString("d MMM");

        public WorkOrder Clone() => (WorkOrder)MemberwiseClone();

        public static string StatusText(WorkOrderStatus status) =>
            status == WorkOrderStatus.OnSite ? "On site" : status.ToString();
    }

    /// <summary>A technician who can sign in to FieldOps.</summary>
    public sealed class Technician
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public string Van { get; init; }
    }
}
