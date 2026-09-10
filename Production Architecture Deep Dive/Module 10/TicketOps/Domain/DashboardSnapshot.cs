using System.Collections.Generic;
using System.Linq;

namespace TicketOps.Domain
{
    /// <summary>
    /// What the dashboard shows: the work orders and one count per status (the four KPI cards).
    /// Raw values only — the screen formats and localizes them.
    /// </summary>
    public sealed class DashboardSnapshot
    {
        public IReadOnlyList<WorkOrder> Orders { get; }

        public DashboardSnapshot(IReadOnlyList<WorkOrder> orders)
        {
            Orders = orders ?? new List<WorkOrder>();
        }

        public int CountOf(WorkOrderStatus status) => Orders.Count(o => o.Status == status);

        public int OpenCount => Orders.Count(o => o.Status != WorkOrderStatus.Done);
    }
}
