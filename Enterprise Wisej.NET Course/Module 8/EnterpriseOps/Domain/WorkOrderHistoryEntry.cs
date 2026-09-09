using System;

namespace EnterpriseOps.Domain
{
    /// <summary>
    /// One status transition of a work order — the raw material of the StatusTimeline. The entity keeps
    /// the actor and the internal note; only the fields a timeline row needs leave the service.
    /// </summary>
    public class WorkOrderHistoryEntry
    {
        public int Id;
        public int WorkOrderId;
        public DateTime AtUtc;
        public WorkOrderStatus Status;
        public string Note;            // what happened, in the operator's words
        public string Actor;           // who did it (never shown in the timeline; audit only)
    }
}
