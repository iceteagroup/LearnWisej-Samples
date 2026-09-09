using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// In-memory work orders — one instance per session (created by the page, never static).
    /// Ids 2002–2005 are the four rows the walkthrough video shows on the migrated WorkOrdersPage;
    /// the rest make the grids, filters and counts look like a real tenant.
    /// </summary>
    public class FakeWorkOrderStore
    {
        private readonly List<WorkOrder> _rows = new List<WorkOrder>();

        public IReadOnlyList<Tenant> Tenants { get; } = new[]
        {
            new Tenant { Id = "contoso",   Name = "Contoso Facilities" },
            new Tenant { Id = "fabrikam",  Name = "Fabrikam Industrial" },
            new Tenant { Id = "northwind", Name = "Northwind Logistics" },
        };

        public FakeWorkOrderStore()
        {
            Seed();
        }

        public int Count => _rows.Count;

        public IEnumerable<WorkOrder> All() => _rows;

        public WorkOrder Find(int id) => _rows.FirstOrDefault(w => w.Id == id);

        public WorkOrder Add(WorkOrder workOrder)
        {
            workOrder.Id = _rows.Max(w => w.Id) + 1;
            workOrder.Version = 1;
            workOrder.CreatedUtc = DateTime.UtcNow;
            _rows.Add(workOrder);
            return workOrder;
        }

        private void Seed()
        {
            var start = new DateTime(2026, 8, 24, 8, 0, 0, DateTimeKind.Utc);
            int id = 2001;

            // The video's four rows first (contoso).
            AddSeed(id++, "contoso", "Replace air handler belt", "Contoso HQ", "Building A", WorkOrderStatus.Completed, Priority.Normal, "ben.tech", start.AddDays(-9), start.AddDays(-4));
            AddSeed(id++, "contoso", "Repair loading dock pump", "Contoso HQ", "Dock 2", WorkOrderStatus.New, Priority.High, "", start.AddDays(-1), start.AddDays(1));
            AddSeed(id++, "contoso", "Quarterly elevator inspection", "Contoso HQ", "Tower 1", WorkOrderStatus.InProgress, Priority.Normal, "ben.tech", start.AddDays(-3), start.AddDays(4));
            AddSeed(id++, "contoso", "Replace lobby lighting", "Contoso HQ", "Lobby", WorkOrderStatus.Assigned, Priority.Low, "ben.tech", start.AddDays(-2), start.AddDays(7));
            AddSeed(id++, "contoso", "Calibrate pressure sensors", "Contoso Plant", "Line 3", WorkOrderStatus.InProgress, Priority.High, "ben.tech", start.AddDays(-2), start.AddDays(2));

            // The rest: a plausible spread across three tenants and every status.
            string[] titles =
            {
                "Inspect rooftop HVAC unit", "Replace fire extinguisher", "Service backup generator", "Fix leaking faucet",
                "Reset badge reader", "Repaint stairwell", "Check emergency lighting", "Clean condenser coils",
                "Replace ceiling tiles", "Test sprinkler valve", "Patch parking lot", "Replace door closer",
                "Align conveyor rollers", "Replace forklift battery", "Seal warehouse roof", "Inspect crane hoist",
                "Replace dock bumpers", "Calibrate scale", "Upgrade PLC firmware", "Repair chiller pump",
                "Replace worn cable", "Service compressor", "Inspect fall protection", "Replace HVAC filter",
                "Fix loading ramp", "Test backup lighting", "Replace exit sign", "Grease bearings",
                "Inspect racking", "Replace thermostat", "Check gas detectors", "Service boiler",
                "Replace pallet wrapper film", "Repair rollup door", "Inspect roof drains", "Tighten guardrails",
                "Replace water heater", "Test UPS", "Clean gutters", "Replace floor mats"
            };
            string[] tenants = { "contoso", "fabrikam", "northwind" };
            string[] customers = { "Contoso Plant", "Fabrikam Works", "Northwind Depot" };
            string[] sites = { "Line 1", "Line 2", "Bay 4", "Yard", "Warehouse B", "Office 2F" };
            var statuses = (WorkOrderStatus[])Enum.GetValues(typeof(WorkOrderStatus));
            var priorities = (Priority[])Enum.GetValues(typeof(Priority));

            for (int i = 0; i < titles.Length; i++)
            {
                int t = i % 3;
                var status = statuses[(i * 5 + 2) % statuses.Length];
                var priority = priorities[(i * 3 + 1) % priorities.Length];
                bool unassigned = status == WorkOrderStatus.New;
                AddSeed(id++, tenants[t], titles[i], customers[t], sites[i % sites.Length], status, priority,
                    unassigned ? "" : (i % 4 == 0 ? "ana.ops" : "ben.tech"),
                    start.AddDays(-(i % 12)), status == WorkOrderStatus.Completed ? (DateTime?)null : start.AddDays(i % 9));
            }
        }

        private void AddSeed(int id, string tenant, string title, string customer, string site, WorkOrderStatus status,
            Priority priority, string assignedTo, DateTime created, DateTime? due)
        {
            _rows.Add(new WorkOrder
            {
                Id = id, TenantId = tenant, Title = title, Customer = customer, Site = site, Status = status,
                Priority = priority, AssignedTo = assignedTo, CreatedUtc = created, DueUtc = due, Version = 1
            });
        }
    }
}
