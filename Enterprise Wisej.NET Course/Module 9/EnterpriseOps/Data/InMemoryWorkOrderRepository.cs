using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The repository boundary the services talk to. Every query is tenant-scoped on purpose:
    /// there is no "find by id" without a tenant, so a forged id from another tenant cannot
    /// resolve to anything.
    /// </summary>
    public interface IWorkOrderRepository
    {
        IReadOnlyList<WorkOrder> Query(string tenantId);
        WorkOrder Find(string tenantId, int id);
        void Save(WorkOrder workOrder);
    }

    /// <summary>
    /// In-memory fake store, seeded per session (~60 rows across the three tenants).
    /// No database, no network — Module 9 is about the browser boundary, not persistence.
    /// </summary>
    public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
    {
        private readonly List<WorkOrder> _rows = new List<WorkOrder>();

        private static readonly string[] Titles =
        {
            "Replace compressor bearing", "Annual boiler inspection", "HVAC filter change", "Elevator brake test",
            "Chiller refrigerant top-up", "Generator load test", "Fire panel battery swap", "Cooling tower descale",
            "Pump seal replacement", "Roof vent repair", "Lighting retrofit", "Water heater relief valve",
            "Conveyor belt alignment", "Backflow preventer test", "Door closer adjustment", "Sprinkler head replacement",
            "Air handler belt change", "Transformer thermal scan", "Sump pump float check", "BMS sensor calibration",
        };

        private static readonly string[] Customers = { "Adatum", "Litware", "Proseware", "Tailspin", "Wingtip", "Woodgrove" };
        private static readonly string[] Sites = { "Plant A", "Plant B", "HQ tower", "Depot 3", "Warehouse N", "Campus East" };
        private static readonly string[] Technicians = { "ben.tech", "dana.tech", "eli.tech", "faye.tech" };

        public InMemoryWorkOrderRepository()
        {
            var random = new Random(9);          // deterministic: the same queue on every run
            var statuses = (WorkOrderStatus[])Enum.GetValues(typeof(WorkOrderStatus));
            var priorities = (Priority[])Enum.GetValues(typeof(Priority));
            int id = 1040;

            foreach (Tenant tenant in Tenant.All)
            {
                for (int i = 0; i < 20; i++)
                {
                    WorkOrderStatus status = statuses[random.Next(statuses.Length)];
                    _rows.Add(new WorkOrder
                    {
                        Id = id++,
                        TenantId = tenant.Id,
                        Title = Titles[(i + tenant.Id.Length) % Titles.Length],
                        Customer = Customers[random.Next(Customers.Length)],
                        Site = Sites[random.Next(Sites.Length)],
                        Status = status,
                        Priority = priorities[random.Next(priorities.Length)],
                        AssignedTo = status == WorkOrderStatus.New ? "" : Technicians[random.Next(Technicians.Length)],
                        CreatedUtc = DateTime.UtcNow.AddDays(-random.Next(1, 40)),
                        DueUtc = random.Next(4) == 0 ? (DateTime?)null : DateTime.UtcNow.AddDays(random.Next(-3, 14)),
                        Version = 1,
                    });
                }
            }

            // Make the first rows of the default tenant predictable for the walkthrough:
            // 1040 is New (approve succeeds), 1041 is Completed (approve → INVALID_STATE).
            _rows[0].Status = WorkOrderStatus.New; _rows[0].AssignedTo = "";
            _rows[1].Status = WorkOrderStatus.Completed;
        }

        public IReadOnlyList<WorkOrder> Query(string tenantId)
            => _rows.Where(r => r.TenantId == tenantId).OrderBy(r => r.Id).ToList();

        public WorkOrder Find(string tenantId, int id)
            => _rows.FirstOrDefault(r => r.TenantId == tenantId && r.Id == id);

        public void Save(WorkOrder workOrder)
        {
            int index = _rows.FindIndex(r => r.Id == workOrder.Id && r.TenantId == workOrder.TenantId);
            if (index < 0) throw new InvalidOperationException($"Work order {workOrder.Id} is not in the store.");
            workOrder.Version++;
            _rows[index] = workOrder;
        }
    }
}
