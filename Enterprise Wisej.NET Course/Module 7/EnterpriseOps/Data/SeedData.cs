using System;
using System.Collections.Generic;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>Deterministic seed so every run shows the same queue (WO-100232 is the video's work order).</summary>
    internal static class SeedData
    {
        private static readonly string[] Tenants = { "contoso", "fabrikam", "northwind" };

        private static readonly (string Title, string Customer, string Site)[] Jobs =
        {
            ("Loading dock pump inspection",      "Harbor Logistics",   "Pier 4"),
            ("Chiller compressor replacement",    "Northgate Mall",     "Roof plant"),
            ("Fire panel annual test",            "Civic Center",       "Basement B1"),
            ("Generator load bank test",          "Riverside Hospital", "Energy centre"),
            ("Elevator door sensor fault",        "Skyline Towers",     "Lift 3"),
            ("HVAC filter change · quarterly",    "Westfield Offices",  "Floor 7"),
            ("Boiler pressure relief valve",      "Grand Hotel",        "Plant room"),
            ("Cold room door seal",               "FreshMart",          "Store 12"),
            ("Access control reader swap",        "Harbor Logistics",   "Gate 2"),
            ("Sprinkler flow switch",             "Civic Center",       "Level 3"),
            ("Solar inverter fault code F21",     "Greenfield Farm",    "Array 2"),
            ("UPS battery string replacement",    "DataHub",            "Hall A"),
        };

        public static List<WorkOrder> WorkOrders()
        {
            var rows = new List<WorkOrder>();
            var baseDate = new DateTime(2026, 9, 1, 8, 0, 0, DateTimeKind.Utc);
            int id = 100230;

            for (int i = 0; i < 48; i++)
            {
                var job = Jobs[i % Jobs.Length];
                var status = (WorkOrderStatus)(i % 4);           // New, Assigned, InProgress, OnHold — never pre-escalated
                if (i % 11 == 10) status = WorkOrderStatus.Completed;

                rows.Add(new WorkOrder
                {
                    Id = id + i,
                    TenantId = Tenants[i % 3 == 0 ? 0 : (i % 3)],   // ~half contoso
                    Title = job.Title,
                    Customer = job.Customer,
                    Site = job.Site,
                    Status = status,
                    Priority = (Priority)((i * 7) % 4),
                    AssignedTo = i % 2 == 0 ? "ben.tech" : "d.silva",
                    CreatedUtc = baseDate.AddHours(i * 5),
                    DueUtc = baseDate.AddDays(3 + i % 9),
                    Version = 1 + i % 3,
                });
            }

            // The video's work order: WO-100232, Critical, blocked for six days.
            var video = rows.Find(w => w.Id == 100232);
            video.TenantId = "contoso";
            video.Title = "Loading dock pump inspection";
            video.Customer = "Harbor Logistics";
            video.Site = "Pier 4";
            video.Status = WorkOrderStatus.OnHold;
            video.Priority = Priority.Critical;
            video.Version = 3;

            return rows;
        }
    }
}
