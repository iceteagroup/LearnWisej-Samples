using System;

namespace EnterpriseOps.Services.Queries
{
    /// <summary>
    /// The read model the grid binds to — a projection shaped for the screen, not the entity.
    /// Read-only by construction: nothing here can be saved.
    /// </summary>
    public sealed class WorkQueueRow
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string Customer { get; set; }
        public string Site { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public DateTime? DueUtc { get; set; }
        public int Version { get; set; }
    }
}
