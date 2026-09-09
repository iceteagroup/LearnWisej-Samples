using System;
using System.Collections.Generic;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// ~48 work orders across the three tenants. The first three contoso rows assigned to ben.tech are the
    /// ones the walkthrough video shows (WO-1041, WO-1038, WO-1037); twelve are assigned to ben.tech in total,
    /// which is what the field cache holds ("cache: 12 work orders").
    /// </summary>
    public static class SeedData
    {
        public static readonly Tenant[] Tenants =
        {
            new Tenant { Id = "contoso",   Name = "Contoso Utilities" },
            new Tenant { Id = "fabrikam",  Name = "Fabrikam Industrial" },
            new Tenant { Id = "northwind", Name = "Northwind Water" },
        };

        public static List<WorkOrder> WorkOrders()
        {
            var today = DateTime.UtcNow.Date;
            var rows = new List<WorkOrder>();
            int id = 1030;

            WorkOrder Add(string tenant, string title, string customer, string site, string assignedTo,
                          WorkOrderStatus status, Priority priority, int dueInDays, int version = 2)
            {
                var wo = new WorkOrder
                {
                    Id = id++,
                    TenantId = tenant,
                    Title = title,
                    Customer = customer,
                    Site = site,
                    AssignedTo = assignedTo,
                    Status = status,
                    Priority = priority,
                    CreatedUtc = today.AddDays(-3).AddHours(8),
                    DueUtc = today.AddDays(dueInDays).AddHours(17),
                    Version = version,
                    Notes = "",
                    LastChangedBy = "ana.ops",
                    LastChangedUtc = today.AddDays(-1).AddHours(9),
                };
                rows.Add(wo);
                return wo;
            }

            // ben.tech — contoso field assignments (the cache). Ids 1030..1041; the video's three come first
            // in the cache because they are due first.
            Add("contoso", "Meter room 4 — recalibrate flow meter",       "Contoso Utilities", "Plant North",  "ben.tech", WorkOrderStatus.Assigned,   Priority.Normal,   3);
            Add("contoso", "Boiler 3 — replace pressure sensor",           "Contoso Utilities", "Plant North",  "ben.tech", WorkOrderStatus.Assigned,   Priority.High,     2);
            Add("contoso", "Cooling tower — clean intake screen",          "Contoso Utilities", "Plant East",   "ben.tech", WorkOrderStatus.Assigned,   Priority.Low,      5);
            Add("contoso", "Compressor bay — inspect belts",               "Contoso Utilities", "Plant East",   "ben.tech", WorkOrderStatus.InProgress, Priority.Normal,   2);
            Add("contoso", "Pump station 2 — replace check valve",         "Contoso Utilities", "Riverside",    "ben.tech", WorkOrderStatus.Assigned,   Priority.Normal,   4);
            Add("contoso", "Tank farm — sample corrosion inhibitor",       "Contoso Utilities", "Riverside",    "ben.tech", WorkOrderStatus.Assigned,   Priority.Low,      6);
            Add("contoso", "Control room — swap UPS batteries",            "Contoso Utilities", "Plant North",  "ben.tech", WorkOrderStatus.Assigned,   Priority.High,     3);
            Add("contoso", "Valve yard — replace relief valve",            "Contoso Utilities", "Valve yard",   "ben.tech", WorkOrderStatus.Assigned,   Priority.High,     0); // WO-1037
            Add("contoso", "Substation B — inspect breaker",               "Contoso Utilities", "Substation B", "ben.tech", WorkOrderStatus.Assigned,   Priority.Critical, 0); // WO-1038
            Add("contoso", "Basement plant — lubricate conveyor",          "Contoso Utilities", "Plant East",   "ben.tech", WorkOrderStatus.Assigned,   Priority.Normal,   1);
            Add("contoso", "Gate house — replace card reader",             "Contoso Utilities", "Plant North",  "ben.tech", WorkOrderStatus.Assigned,   Priority.Normal,   1);
            Add("contoso", "Pump station 7 — replace seal",                "Contoso Utilities", "Pump station 7", "ben.tech", WorkOrderStatus.Assigned, Priority.Critical, 0); // WO-1041

            // Other contoso technicians and states (never cached on ben.tech's device).
            string[] others = { "dan.tech", "eva.tech", "finn.tech" };
            string[] sites = { "Plant North", "Plant East", "Riverside", "Harbour" };
            string[] titles =
            {
                "Replace actuator", "Inspect lightning arrestor", "Torque flange bolts", "Replace filter cartridge",
                "Calibrate level transmitter", "Repair heat tracing", "Test emergency stop", "Replace gasket set",
                "Clean strainer", "Inspect cathodic protection", "Replace flow switch", "Service air dryer",
            };
            var statuses = new[]
            {
                WorkOrderStatus.New, WorkOrderStatus.Assigned, WorkOrderStatus.InProgress, WorkOrderStatus.OnHold,
                WorkOrderStatus.Escalated, WorkOrderStatus.Completed,
            };

            for (int i = 0; i < 12; i++)
                Add("contoso", $"{sites[i % sites.Length]} — {titles[i]}", "Contoso Utilities", sites[i % sites.Length],
                    others[i % others.Length], statuses[i % statuses.Length], (Priority)(i % 4), i % 7);

            for (int i = 0; i < 12; i++)
                Add("fabrikam", $"Line {i % 3 + 1} — {titles[(i + 4) % titles.Length]}", "Fabrikam Industrial", $"Line {i % 3 + 1}",
                    others[(i + 1) % others.Length], statuses[(i + 2) % statuses.Length], (Priority)((i + 1) % 4), i % 6);

            for (int i = 0; i < 12; i++)
                Add("northwind", $"Reservoir {i % 4 + 1} — {titles[(i + 8) % titles.Length]}", "Northwind Water", $"Reservoir {i % 4 + 1}",
                    others[(i + 2) % others.Length], statuses[(i + 4) % statuses.Length], (Priority)((i + 2) % 4), i % 5);

            return rows;
        }
    }
}
