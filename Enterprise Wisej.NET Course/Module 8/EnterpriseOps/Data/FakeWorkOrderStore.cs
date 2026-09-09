using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// In-memory stand-in for the work-order repository: 150 work orders across three tenants with a
    /// status history each, seeded deterministically so the chart and the timeline look the same on
    /// every run. One instance per session (created by the page); no statics, no database, no network.
    /// </summary>
    public class FakeWorkOrderStore
    {
        private readonly List<Tenant> _tenants = new List<Tenant>();
        private readonly List<WorkOrder> _workOrders = new List<WorkOrder>();
        private readonly List<WorkOrderHistoryEntry> _history = new List<WorkOrderHistoryEntry>();

        private static readonly string[] Titles =
        {
            "Replace compressor", "Dock door sensor fault", "Quarterly boiler inspection", "Conveyor belt misalignment",
            "Chiller refrigerant top-up", "Forklift charger outage", "Cold-room door seal", "Emergency lighting test",
            "Sprinkler valve leak", "HVAC filter change", "Loading bay camera offline", "Pallet wrapper jam",
        };
        private static readonly string[] Customers = { "Northfield Energy", "Harbor Logistics", "Bluepeak Foods", "Crestline Clinics" };
        private static readonly string[] Sites = { "Dock 4", "Plant B", "Warehouse 2", "Clinic North", "Depot East" };
        private static readonly string[] Technicians = { "t.nguyen", "r.okafor", "ben.tech", "s.ibarra", "m.weber" };

        public FakeWorkOrderStore()
        {
            _tenants.Add(new Tenant { Id = "fabrikam", Name = "Fabrikam Field Services" });
            _tenants.Add(new Tenant { Id = "contoso", Name = "Contoso Facilities" });
            _tenants.Add(new Tenant { Id = "northwind", Name = "Northwind Utilities" });

            var random = new Random(8);   // Module 8: the same data every run

            // fabrikam: 100 work orders, 2001..2100 — 38 open · 22 on hold · 12 escalated · 28 done
            // (the percentages the walkthrough video shows).
            var fabrikamStatuses = new List<WorkOrderStatus>();
            fabrikamStatuses.AddRange(Enumerable.Repeat(WorkOrderStatus.New, 10));
            fabrikamStatuses.AddRange(Enumerable.Repeat(WorkOrderStatus.Assigned, 14));
            fabrikamStatuses.AddRange(Enumerable.Repeat(WorkOrderStatus.InProgress, 14));
            fabrikamStatuses.AddRange(Enumerable.Repeat(WorkOrderStatus.OnHold, 22));
            fabrikamStatuses.AddRange(Enumerable.Repeat(WorkOrderStatus.Escalated, 12));
            fabrikamStatuses.AddRange(Enumerable.Repeat(WorkOrderStatus.Completed, 22));
            fabrikamStatuses.AddRange(Enumerable.Repeat(WorkOrderStatus.Cancelled, 6));
            Shuffle(fabrikamStatuses, random);

            // Work order 2002 is the one the walkthrough opens: it must be Escalated.
            int firstEscalated = fabrikamStatuses.IndexOf(WorkOrderStatus.Escalated);
            fabrikamStatuses[firstEscalated] = fabrikamStatuses[1];
            fabrikamStatuses[1] = WorkOrderStatus.Escalated;

            Seed("fabrikam", 2001, fabrikamStatuses, random);
            Seed("contoso", 2101, RandomStatuses(30, random), random);
            Seed("northwind", 2131, RandomStatuses(20, random), random);
        }

        public IReadOnlyList<Tenant> Tenants => _tenants;

        public WorkOrder Find(int id) => _workOrders.FirstOrDefault(w => w.Id == id);

        public IEnumerable<WorkOrder> ForTenant(string tenantId)
            => _workOrders.Where(w => w.TenantId == tenantId);

        public IEnumerable<WorkOrderHistoryEntry> HistoryOf(int workOrderId)
            => _history.Where(h => h.WorkOrderId == workOrderId).OrderBy(h => h.AtUtc);

        #region Seeding

        private void Seed(string tenantId, int firstId, IList<WorkOrderStatus> statuses, Random random)
        {
            var baseUtc = new DateTime(2026, 6, 1, 8, 0, 0, DateTimeKind.Utc);
            for (int i = 0; i < statuses.Count; i++)
            {
                int id = firstId + i;
                var created = baseUtc.AddHours(i * 5 + random.Next(0, 3));
                var workOrder = new WorkOrder
                {
                    Id = id,
                    TenantId = tenantId,
                    Title = Titles[(i * 7 + id) % Titles.Length],
                    Customer = Customers[(i * 3 + id) % Customers.Length],
                    Site = Sites[(i + id) % Sites.Length],
                    Status = statuses[i],
                    Priority = (Priority)(random.Next(0, 4)),
                    AssignedTo = statuses[i] == WorkOrderStatus.New ? "" : Technicians[(i + id) % Technicians.Length],
                    CreatedUtc = created,
                    DueUtc = created.AddDays(2 + random.Next(0, 5)),
                    Version = 1,
                };
                _workOrders.Add(workOrder);

                if (id == 2002)
                    SeedVideoHistory(workOrder);
                else
                    SeedHistory(workOrder, random);
            }
        }

        /// <summary>The exact four entries the walkthrough video shows for work order 2002.</summary>
        private void SeedVideoHistory(WorkOrder workOrder)
        {
            workOrder.Title = "Dock door sensor fault";
            workOrder.Customer = "Harbor Logistics";
            workOrder.Site = "Dock 4";
            workOrder.AssignedTo = "t.nguyen";
            workOrder.Priority = Priority.High;
            workOrder.CreatedUtc = new DateTime(2026, 6, 10, 9, 12, 0, DateTimeKind.Utc);
            workOrder.DueUtc = new DateTime(2026, 6, 14, 17, 0, 0, DateTimeKind.Utc);
            workOrder.Version = 4;

            Add(workOrder.Id, new DateTime(2026, 6, 10, 9, 12, 0, DateTimeKind.Utc), WorkOrderStatus.New, "Imported from fabrikam_q2.csv", "import.job");
            Add(workOrder.Id, new DateTime(2026, 6, 11, 14, 5, 0, DateTimeKind.Utc), WorkOrderStatus.Assigned, "t.nguyen — dock crew", "ana.ops");
            Add(workOrder.Id, new DateTime(2026, 6, 12, 8, 30, 0, DateTimeKind.Utc), WorkOrderStatus.OnHold, "vendor part backordered", "t.nguyen");
            Add(workOrder.Id, new DateTime(2026, 6, 12, 10, 41, 0, DateTimeKind.Utc), WorkOrderStatus.Escalated, "approver m.weber · due Jun 14", "ana.ops");
        }

        /// <summary>A plausible path from Created to the work order's current status.</summary>
        private void SeedHistory(WorkOrder workOrder, Random random)
        {
            var at = workOrder.CreatedUtc;
            Add(workOrder.Id, at, WorkOrderStatus.New, "Created from the customer portal", "portal");
            if (workOrder.Status == WorkOrderStatus.New) return;

            at = at.AddHours(3 + random.Next(0, 20));
            Add(workOrder.Id, at, WorkOrderStatus.Assigned, $"{workOrder.AssignedTo} — {workOrder.Site}", "ana.ops");
            if (workOrder.Status == WorkOrderStatus.Assigned) return;

            switch (workOrder.Status)
            {
                case WorkOrderStatus.InProgress:
                    Add(workOrder.Id, at.AddHours(2 + random.Next(0, 8)), WorkOrderStatus.InProgress, "technician on site", workOrder.AssignedTo);
                    break;
                case WorkOrderStatus.OnHold:
                    Add(workOrder.Id, at.AddHours(2 + random.Next(0, 8)), WorkOrderStatus.InProgress, "technician on site", workOrder.AssignedTo);
                    Add(workOrder.Id, at.AddHours(12 + random.Next(0, 8)), WorkOrderStatus.OnHold, "waiting for customer access", workOrder.AssignedTo);
                    break;
                case WorkOrderStatus.Escalated:
                    Add(workOrder.Id, at.AddHours(4 + random.Next(0, 8)), WorkOrderStatus.OnHold, "vendor part backordered", workOrder.AssignedTo);
                    Add(workOrder.Id, at.AddHours(20 + random.Next(0, 8)), WorkOrderStatus.Escalated, "SLA at risk · approver m.weber", "ana.ops");
                    break;
                case WorkOrderStatus.Completed:
                    Add(workOrder.Id, at.AddHours(2 + random.Next(0, 8)), WorkOrderStatus.InProgress, "technician on site", workOrder.AssignedTo);
                    Add(workOrder.Id, at.AddHours(9 + random.Next(0, 30)), WorkOrderStatus.Completed, "signed off by the customer", workOrder.AssignedTo);
                    break;
                case WorkOrderStatus.Cancelled:
                    Add(workOrder.Id, at.AddHours(5 + random.Next(0, 30)), WorkOrderStatus.Cancelled, "duplicate of an existing order", "ana.ops");
                    break;
            }
        }

        private void Add(int workOrderId, DateTime atUtc, WorkOrderStatus status, string note, string actor)
        {
            _history.Add(new WorkOrderHistoryEntry
            {
                Id = _history.Count + 1,
                WorkOrderId = workOrderId,
                AtUtc = atUtc,
                Status = status,
                Note = note,
                Actor = actor,
            });
        }

        private static List<WorkOrderStatus> RandomStatuses(int count, Random random)
        {
            var values = (WorkOrderStatus[])Enum.GetValues(typeof(WorkOrderStatus));
            var list = new List<WorkOrderStatus>(count);
            for (int i = 0; i < count; i++)
                list.Add(values[random.Next(values.Length)]);
            return list;
        }

        private static void Shuffle<T>(IList<T> list, Random random)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        #endregion
    }
}
