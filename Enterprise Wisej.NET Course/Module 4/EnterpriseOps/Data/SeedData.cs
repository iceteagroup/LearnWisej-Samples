using System;
using System.Collections.Generic;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// Deterministic seed: 60 work orders (ids 2001–2060) across contoso / fabrikam / northwind, with
    /// versions, assignees and a mix of statuses so the grid and every failure path look real.
    /// Work order 2002 is the walkthrough's: "Repair loading dock pump", fabrikam, on hold, v8.
    /// </summary>
    public static class SeedData
    {
        private static readonly string[] Titles =
        {
            "Replace HVAC compressor", "Inspect fire suppression line", "Calibrate conveyor scale",
            "Repair loading dock pump", "Service backup generator", "Replace warehouse lighting",
            "Fix cold-room door seal", "Upgrade badge reader firmware", "Clean chiller condenser",
            "Rewire packing station 3", "Test emergency lighting", "Replace forklift battery charger",
            "Patch roof leak, bay 2", "Recommission boiler 1", "Align dock leveller", "Inspect sprinkler heads",
            "Replace UPS batteries", "Service air compressor", "Repair pallet wrapper", "Recalibrate CO sensors",
        };

        private static readonly (string Id, string Name, string[] Customers, string[] Sites)[] Tenants =
        {
            ("contoso",   "Contoso Facilities",  new[] { "Contoso Retail", "Contoso Pharma", "Contoso Foods" },   new[] { "Plant A", "Plant B", "DC North" }),
            ("fabrikam",  "Fabrikam Logistics",  new[] { "Fabrikam Logistics", "Fabrikam Cold Chain" },          new[] { "Dock 4", "Hub West", "Yard 2" }),
            ("northwind", "Northwind Traders",   new[] { "Northwind Traders", "Northwind Fresh" },               new[] { "Depot 1", "Depot 7" }),
        };

        private static readonly string[] Technicians = { "ben.tech", "dmitri.field", "eva.field", "" };

        public static int Seed(EnterpriseOpsDbContext context)
        {
            foreach (var t in Tenants)
                context.Tenants.Add(new Tenant { Id = t.Id, Name = t.Name });

            var random = new Random(4);           // fixed seed: the same data every session
            var statuses = (WorkOrderStatus[])Enum.GetValues(typeof(WorkOrderStatus));
            var priorities = (Priority[])Enum.GetValues(typeof(Priority));
            var baseDate = new DateTime(2026, 9, 1, 8, 0, 0, DateTimeKind.Utc);
            var rows = new List<WorkOrder>();

            for (int i = 0; i < 60; i++)
            {
                int id = 2001 + i;
                var tenant = Tenants[i % 3];
                var status = statuses[random.Next(statuses.Length)];
                rows.Add(new WorkOrder
                {
                    Id = id,
                    TenantId = tenant.Id,
                    Number = $"WO-{id}",
                    Title = Titles[i % Titles.Length],
                    Customer = tenant.Customers[random.Next(tenant.Customers.Length)],
                    Site = tenant.Sites[random.Next(tenant.Sites.Length)],
                    Status = status,
                    Priority = priorities[random.Next(priorities.Length)],
                    AssignedTo = status == WorkOrderStatus.New ? "" : Technicians[random.Next(Technicians.Length)],
                    CreatedUtc = baseDate.AddHours(-random.Next(24, 24 * 30)),
                    DueUtc = random.Next(4) == 0 ? (DateTime?)null : baseDate.AddDays(random.Next(-3, 21)),
                    Version = 1 + random.Next(0, 12),
                    ApprovedBy = status == WorkOrderStatus.Completed ? "ana.ops" : null,
                    ApprovedUtc = status == WorkOrderStatus.Completed ? baseDate.AddHours(-random.Next(1, 200)) : (DateTime?)null,
                });
            }

            // The walkthrough's work order, exactly as the video shows it.
            var pump = rows[1];
            pump.TenantId = "fabrikam";
            pump.Number = "WO-2002";
            pump.Title = "Repair loading dock pump";
            pump.Customer = "Fabrikam Logistics";
            pump.Site = "Dock 4";
            pump.Status = WorkOrderStatus.OnHold;
            pump.Priority = Priority.High;
            pump.AssignedTo = "ben.tech";
            pump.Version = 8;
            pump.ApprovedBy = null;
            pump.ApprovedUtc = null;

            // Make sure fabrikam has a few approvable rows for the happy path.
            int inProgress = 0;
            foreach (var row in rows)
            {
                if (row.TenantId == "fabrikam" && row.Id != 2002 && inProgress < 4 && row.Status != WorkOrderStatus.Completed)
                {
                    row.Status = WorkOrderStatus.InProgress;
                    row.AssignedTo = "ben.tech";
                    inProgress++;
                }
            }

            context.WorkOrders.AddRange(rows);
            context.SaveChanges();
            return rows.Count;
        }
    }
}
