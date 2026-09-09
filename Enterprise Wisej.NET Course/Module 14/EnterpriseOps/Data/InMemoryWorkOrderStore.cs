using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>Thrown by the store when a write carries a version the row has moved past (Module 4/5 pattern).</summary>
    public sealed class ConcurrencyException : Exception
    {
        public int ExpectedVersion { get; }
        public int CurrentVersion { get; }

        public ConcurrencyException(int workOrderId, int expected, int current)
            : base($"work order #{workOrderId} is stale: expected version {expected}, current version {current}")
        {
            ExpectedVersion = expected;
            CurrentVersion = current;
        }
    }

    /// <summary>
    /// The database stand-in. In production this is the EF Core DbContext of Module 4; the capstone keeps
    /// a per-session in-memory store so the sample runs with one `dotnet run` and nothing static holds
    /// tenant data. Seeded deterministically (Random(14)) so the trace lines are reproducible.
    /// </summary>
    public sealed class InMemoryWorkOrderStore
    {
        private readonly List<WorkOrder> _rows;
        private readonly object _gate = new object();

        public static readonly Tenant[] Tenants =
        {
            new Tenant { Id = "contoso", Name = "Contoso Facilities" },
            new Tenant { Id = "fabrikam", Name = "Fabrikam Energy" },
            new Tenant { Id = "northwind", Name = "Northwind Logistics" },
        };

        public InMemoryWorkOrderStore()
        {
            _rows = Seed(120);
        }

        public int Count => _rows.Count;

        /// <summary>Tenant-scoped read. The tenant filter is applied here, not by the caller — a query can never forget it.</summary>
        public IEnumerable<WorkOrder> Query(string tenantId)
        {
            lock (_gate)
                return _rows.Where(w => w.TenantId == tenantId).Select(w => w.Clone()).ToList();
        }

        public WorkOrder Find(int id)
        {
            lock (_gate)
                return _rows.FirstOrDefault(w => w.Id == id)?.Clone();
        }

        /// <summary>Optimistic write: succeeds only when the caller saw the current version.</summary>
        public WorkOrder Update(WorkOrder changed, int expectedVersion)
        {
            lock (_gate)
            {
                var row = _rows.FirstOrDefault(w => w.Id == changed.Id);
                if (row == null)
                    throw new KeyNotFoundException($"work order #{changed.Id} not found");
                if (row.Version != expectedVersion)
                    throw new ConcurrencyException(changed.Id, expectedVersion, row.Version);

                row.Status = changed.Status;
                row.AssignedTo = changed.AssignedTo;
                row.CompletedUtc = changed.CompletedUtc;
                row.Version++;
                return row.Clone();
            }
        }

        /// <summary>Used by the "stale version" failure path: someone else saved the row first.</summary>
        public void BumpVersionBehindTheScenes(int id)
        {
            lock (_gate)
            {
                var row = _rows.FirstOrDefault(w => w.Id == id);
                if (row != null) row.Version++;
            }
        }

        private static List<WorkOrder> Seed(int count)
        {
            var random = new Random(14);
            string[] titles =
            {
                "Replace HVAC compressor", "Annual boiler inspection", "Emergency generator test", "Fire panel fault",
                "Chiller pump vibration", "Lighting retrofit — floor 3", "Elevator door sensor", "Roof leak above loading bay",
                "UPS battery replacement", "Cooling tower descaling", "Access control reader offline", "Water heater relief valve",
                "Solar inverter alarm", "Fuel tank level sensor", "Conveyor motor overheating", "Cold-room door seal",
            };
            string[] customers = { "Harbor Medical", "Ridgeway Schools", "Atlas Foods", "Meridian Bank", "Summit Retail", "Pioneer Labs" };
            string[] sites = { "Plant A", "Plant B", "HQ", "Depot 3", "Warehouse North", "Campus East" };
            string[] techs = { "ben.tech", "dan.field", "eva.field", "raj.field", null };
            var statuses = (WorkOrderStatus[])Enum.GetValues(typeof(WorkOrderStatus));
            var priorities = (Priority[])Enum.GetValues(typeof(Priority));
            var now = DateTime.UtcNow;

            var rows = new List<WorkOrder>(count);
            for (int i = 1; i <= count; i++)
            {
                var status = statuses[random.Next(statuses.Length)];
                var created = now.AddDays(-random.Next(1, 40)).AddHours(-random.Next(0, 24));
                rows.Add(new WorkOrder
                {
                    Id = 1000 + i,
                    TenantId = Tenants[i % Tenants.Length].Id,
                    Title = titles[random.Next(titles.Length)],
                    Customer = customers[random.Next(customers.Length)],
                    Site = sites[random.Next(sites.Length)],
                    Status = status,
                    Priority = priorities[random.Next(priorities.Length)],
                    AssignedTo = status == WorkOrderStatus.New ? null : techs[random.Next(techs.Length)],
                    CreatedUtc = created,
                    DueUtc = random.Next(10) == 0 ? (DateTime?)null : now.Date.AddDays(random.Next(-5, 14)).AddHours(17),
                    CompletedUtc = status == WorkOrderStatus.Completed ? created.AddDays(random.Next(1, 12)) : (DateTime?)null,
                    Version = 1 + random.Next(0, 5),
                });
            }
            return rows;
        }
    }
}
