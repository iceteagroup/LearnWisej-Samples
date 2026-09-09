using System;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// A row of the filtered work-order grid. A projection: the entity's Version, CreatedUtc and internal
    /// fields stay on the server.
    /// </summary>
    public class WorkQueueRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Customer { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public string Due { get; set; }
    }

    /// <summary>
    /// What the history service returns for one transition. The page maps it into the StatusTimeline's
    /// own <c>TimelineItem</c>; the timeline never sees the entity (and never sees the actor).
    /// </summary>
    public class HistoryEntryView
    {
        public DateTime AtUtc { get; set; }
        public WorkOrderStatus Status { get; set; }
        public string StatusLabel { get; set; }
        public string Note { get; set; }
    }

    /// <summary>The history of one work order: its header line and the ordered transitions.</summary>
    public class WorkOrderHistoryView
    {
        public int WorkOrderId { get; set; }
        public string Title { get; set; }
        public WorkOrderStatus Status { get; set; }
        public HistoryEntryView[] Entries { get; set; }
    }

    /// <summary>How many work orders of the tenant sit in one reporting group (one chart segment).</summary>
    public class StatusCountView
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public int Count { get; set; }
    }
}
