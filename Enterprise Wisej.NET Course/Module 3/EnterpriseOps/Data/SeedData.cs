using System;
using System.Collections.Generic;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// Deterministic seed for the shared store: 24 contoso work orders (2001–2024 — 2002 is the walkthrough's
    /// "Repair loading dock pump"), 16 for fabrikam (3001–3016) and 12 for northwind (4001–4012). The other
    /// tenants exist so tenant isolation has something to isolate: every contoso query must return 24 rows,
    /// and 3001 must be unreachable from a contoso session.
    /// </summary>
    public static class SeedData
    {
        public static IEnumerable<WorkOrder> WorkOrders()
        {
            DateTime now = DateTime.UtcNow;
            var rows = new List<WorkOrder>();

            // ── contoso — the tenant the walkthrough works in ────────────────────────────────────────
            string[,] contoso =
            {
                { "Loading dock pump — pressure alarm", "Contoso Logistics", "Depot-2" },
                { "Repair loading dock pump",           "Contoso Logistics", "Depot-2" },   // 2002 — the video's record
                { "Payment gateway timeout",            "Contoso Retail",    "DC-East" },
                { "Search index lag",                   "Contoso Retail",    "DC-East" },
                { "Export queue backlog",               "Contoso Retail",    "DC-West" },
                { "Stale cache on node B",              "Contoso Retail",    "DC-West" },
                { "Chiller alarm — floor 3",            "Contoso HQ",        "HQ-North" },
                { "Badge reader offline",               "Contoso HQ",        "HQ-North" },
                { "Backup job skipped",                 "Contoso Retail",    "DC-East" },
                { "Printer fleet firmware",             "Contoso HQ",        "HQ-South" },
                { "VPN concentrator errors",            "Contoso Retail",    "DC-West" },
                { "Generator test overdue",             "Contoso HQ",        "HQ-North" },
                { "Wi-Fi dead zone — warehouse",        "Contoso Logistics", "Depot-2" },
                { "Forklift charger fault",             "Contoso Logistics", "Depot-1" },
                { "Cold store door seal",               "Contoso Logistics", "Depot-1" },
                { "Conveyor belt tracking",             "Contoso Logistics", "Depot-3" },
                { "Sprinkler inspection due",           "Contoso HQ",        "HQ-South" },
                { "Loading ramp hydraulics",            "Contoso Logistics", "Depot-2" },
                { "Yard camera offline",                "Contoso Logistics", "Depot-3" },
                { "Pallet wrapper jam",                 "Contoso Logistics", "Depot-1" },
                { "Air compressor service",             "Contoso Retail",    "DC-West" },
                { "Dock leveller calibration",          "Contoso Logistics", "Depot-2" },
                { "UPS battery replacement",            "Contoso Retail",    "DC-East" },
                { "Emergency lighting test",            "Contoso HQ",        "HQ-North" },
            };
            for (int i = 0; i < contoso.GetLength(0); i++)
                rows.Add(Make(2001 + i, "contoso", contoso[i, 0], contoso[i, 1], contoso[i, 2],
                    (WorkOrderStatus)(i % 5), (Priority)((i + 1) % 4), i % 3 == 0 ? "ben.tech" : (i % 3 == 1 ? "ana.ops" : null),
                    now.AddHours(-i - 1), now.AddHours(i + 2)));

            // 2002 is the record both sessions open in the failure path: in progress, assigned, version 7.
            WorkOrder pump = rows.Find(o => o.Id == 2002);
            pump.Status = WorkOrderStatus.InProgress;
            pump.Priority = Priority.High;
            pump.AssignedTo = "ben.tech";
            pump.Version = 7;                       // the walkthrough's "expected v7"
            pump.ModifiedBy = "ana.ops";
            pump.ModifiedUtc = now.AddMinutes(-25);

            // ── fabrikam / northwind — other customers' work, unreachable from a contoso session ─────
            string[] fabrikam = { "Substation telemetry gap", "Meter batch failed", "Pump station leak", "Outage map stale",
                "Crew tablet sync", "Transformer inspection", "SCADA alert flood", "Line crew reassignment",
                "Feeder trip analysis", "Smart meter firmware", "Pole inspection backlog", "Relay test overdue",
                "Cable fault locate", "Switchgear cleaning", "Load profile export", "Storm crew standby" };
            for (int i = 0; i < fabrikam.Length; i++)
                rows.Add(Make(3001 + i, "fabrikam", fabrikam[i], "Fabrikam Utilities", $"Grid-{i % 3 + 1}",
                    (WorkOrderStatus)(i % 5), (Priority)(i % 4), i % 2 == 0 ? "ben.tech" : null,
                    now.AddHours(-i), now.AddHours(i + 3)));

            string[] northwind = { "HVAC filter change", "Elevator certificate", "Roof leak — block C", "Lighting retrofit",
                "Fire panel fault", "Parking gate stuck", "Water heater replace", "Boiler flue inspection",
                "Window seal survey", "Car park line marking", "Legionella flush", "Green roof drainage" };
            for (int i = 0; i < northwind.Length; i++)
                rows.Add(Make(4001 + i, "northwind", northwind[i], "Northwind Facilities", $"Campus-{i % 2 + 1}",
                    (WorkOrderStatus)((i + 2) % 6), (Priority)((i + 1) % 4), "cara.admin",
                    now.AddDays(-i), now.AddDays(i + 1)));

            return rows;
        }

        private static WorkOrder Make(int id, string tenantId, string title, string customer, string site,
            WorkOrderStatus status, Priority priority, string assignedTo, DateTime createdUtc, DateTime? dueUtc)
        {
            return new WorkOrder
            {
                Id = id,
                TenantId = tenantId,
                Title = title,
                Customer = customer,
                Site = site,
                Status = status,
                Priority = priority,
                AssignedTo = assignedTo,
                CreatedUtc = createdUtc,
                DueUtc = dueUtc,
                Version = 1,
                ModifiedBy = "seed",
                ModifiedUtc = createdUtc,
            };
        }
    }
}
