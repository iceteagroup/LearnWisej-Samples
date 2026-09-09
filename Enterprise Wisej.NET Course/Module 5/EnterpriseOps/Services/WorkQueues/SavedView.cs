using System;

namespace EnterpriseOps.Services.WorkQueues
{
    /// <summary>
    /// A saved view is a named, owned <see cref="WorkQueueQuery"/> — a stored query definition, not a cached
    /// result. Applying it runs the query again, so the rows are always current.
    /// </summary>
    public sealed class SavedView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TenantId { get; set; }
        public string Owner { get; set; }
        public WorkQueueQuery Definition { get; set; }
        public DateTime CreatedUtc { get; set; }

        public override string ToString() => Name;
    }
}
