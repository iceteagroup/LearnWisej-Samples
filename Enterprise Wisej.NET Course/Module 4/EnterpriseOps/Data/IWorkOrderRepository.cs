using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// Loads and saves the aggregate for commands — always by tenant + id, so a command can never touch
    /// another tenant's row by mistake. Screens never see this interface; the command service does.
    /// </summary>
    public interface IWorkOrderRepository
    {
        Task<WorkOrder> LoadAsync(string tenantId, int workOrderId, CancellationToken cancellationToken);
        void Add(WorkOrder workOrder);
        void AddAudit(AuditEntry entry);

        /// <summary>Tells EF Core which version the caller saw, so the UPDATE carries WHERE Version = expected.</summary>
        void ExpectVersion(WorkOrder workOrder, int expectedVersion);
    }
}
