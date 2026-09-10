namespace EnterpriseOps.Domain
{
    /// <summary>
    /// The projection the grid binds to — never the entity. It carries what the screen shows and nothing else:
    /// no navigation properties, no customer record, no internal version number beyond what the UI needs.
    /// </summary>
    public sealed class WorkQueueRow
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public string TenantId { get; set; }

        /// <summary>"—" until an approval is recorded. The audit log is the authoritative version of this column.</summary>
        public string ApprovedBy { get; set; }
    }
}
