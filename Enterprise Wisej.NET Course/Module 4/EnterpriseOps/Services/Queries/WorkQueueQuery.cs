using EnterpriseOps.Domain;

namespace EnterpriseOps.Services.Queries
{
    /// <summary>What the grid asks for: tenant, free text, optional status, page.</summary>
    public sealed class WorkQueueQuery
    {
        public string TenantId { get; set; }
        public string Search { get; set; }
        public WorkOrderStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
