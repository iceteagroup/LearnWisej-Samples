using System;
using System.Collections.Generic;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// Deterministic seed: 12 open incidents for contoso (INC-1042 … INC-1053 — the first four are the
    /// walkthrough's rows), a few closed ones from earlier days, and the other two tenants' work so tenant
    /// filtering has something to filter out. Due times are relative to "now" so "SLA at risk" stays at 3.
    /// </summary>
    public static class SeedData
    {
        public static IEnumerable<WorkOrder> WorkOrders()
        {
            DateTime now = DateTime.UtcNow;
            var rows = new List<WorkOrder>();

            // ── contoso: the 12 open incidents the dashboard starts with ─────────────────────────────
            rows.Add(Make(1042, "contoso", "Payment gateway timeout", "Contoso Retail", "DC-East", WorkOrderStatus.New, Priority.High, "ben.tech", now.AddHours(-3), now.AddHours(1)));      // SLA at risk
            rows.Add(Make(1043, "contoso", "Search index lag", "Contoso Retail", "DC-East", WorkOrderStatus.Assigned, Priority.Normal, "ben.tech", now.AddHours(-2), now.AddHours(9)));
            rows.Add(Make(1044, "contoso", "Export queue backlog", "Contoso Retail", "DC-West", WorkOrderStatus.New, Priority.Normal, null, now.AddHours(-2), now.AddHours(12)));
            rows.Add(Make(1045, "contoso", "Stale cache on node B", "Contoso Retail", "DC-West", WorkOrderStatus.OnHold, Priority.Low, "ben.tech", now.AddHours(-6), now.AddDays(2)));
            rows.Add(Make(1046, "contoso", "Chiller alarm — floor 3", "Contoso HQ", "HQ-North", WorkOrderStatus.InProgress, Priority.Critical, "ben.tech", now.AddHours(-1), now.AddHours(2)));  // SLA at risk
            rows.Add(Make(1047, "contoso", "Badge reader offline", "Contoso HQ", "HQ-North", WorkOrderStatus.New, Priority.High, null, now.AddMinutes(-40), now.AddHours(3)));            // SLA at risk
            rows.Add(Make(1048, "contoso", "Backup job skipped", "Contoso Retail", "DC-East", WorkOrderStatus.Assigned, Priority.Normal, "ben.tech", now.AddHours(-5), now.AddHours(20)));
            rows.Add(Make(1049, "contoso", "Printer fleet firmware", "Contoso HQ", "HQ-South", WorkOrderStatus.New, Priority.Low, null, now.AddDays(-1), now.AddDays(5)));
            rows.Add(Make(1050, "contoso", "VPN concentrator errors", "Contoso Retail", "DC-West", WorkOrderStatus.Escalated, Priority.High, "ana.ops", now.AddHours(-8), now.AddHours(6)));
            rows.Add(Make(1051, "contoso", "Generator test overdue", "Contoso HQ", "HQ-North", WorkOrderStatus.OnHold, Priority.Normal, null, now.AddDays(-2), now.AddDays(3)));
            rows.Add(Make(1052, "contoso", "Loading dock sensor", "Contoso Logistics", "Depot-2", WorkOrderStatus.New, Priority.Normal, null, now.AddHours(-4), now.AddDays(1)));
            rows.Add(Make(1053, "contoso", "Wi-Fi dead zone — warehouse", "Contoso Logistics", "Depot-2", WorkOrderStatus.Assigned, Priority.Low, "ben.tech", now.AddDays(-1), now.AddDays(4)));

            // ── contoso: closed earlier — not "today", so not on the dashboard ───────────────────────
            for (int id = 1030; id < 1042; id++)
            {
                bool cancelled = id % 5 == 0;
                rows.Add(Make(id, "contoso", $"Routine maintenance #{id}", "Contoso Retail", "DC-East",
                    cancelled ? WorkOrderStatus.Cancelled : WorkOrderStatus.Completed, Priority.Low, "ben.tech",
                    now.AddDays(-(1042 - id) - 1), null, updatedUtc: now.AddDays(-(1042 - id))));
            }

            // ── fabrikam / northwind: other tenants' work, filtered out by every contoso query ───────
            string[] fabrikamTitles = { "Substation telemetry gap", "Meter batch failed", "Pump station leak", "Outage map stale", "Crew tablet sync", "Transformer inspection", "SCADA alert flood", "Line crew reassignment" };
            for (int i = 0; i < fabrikamTitles.Length; i++)
                rows.Add(Make(2001 + i, "fabrikam", fabrikamTitles[i], "Fabrikam Utilities", $"Grid-{i % 3 + 1}",
                    (WorkOrderStatus)(i % 5), (Priority)(i % 4), i % 2 == 0 ? "ben.tech" : null, now.AddHours(-i), now.AddHours(i + 2)));

            string[] northwindTitles = { "HVAC filter change", "Elevator certificate", "Roof leak — block C", "Lighting retrofit", "Fire panel fault", "Parking gate stuck", "Water heater replace" };
            for (int i = 0; i < northwindTitles.Length; i++)
                rows.Add(Make(3001 + i, "northwind", northwindTitles[i], "Northwind Facilities", $"Campus-{i % 2 + 1}",
                    (WorkOrderStatus)((i + 2) % 6), (Priority)((i + 1) % 4), "ben.tech", now.AddDays(-i), now.AddDays(i + 1)));

            return rows;
        }

        private static WorkOrder Make(int id, string tenantId, string title, string customer, string site,
            WorkOrderStatus status, Priority priority, string assignedTo, DateTime createdUtc, DateTime? dueUtc,
            DateTime? updatedUtc = null)
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
                UpdatedUtc = updatedUtc ?? createdUtc,
                Version = 1,
            };
        }
    }
}
