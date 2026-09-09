using EnterpriseOps.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// In-memory fake store (no database, no network). Module 12 uses it for exactly one thing: the
    /// "known query returns rows" smoke test and the health probe's database check — a release is not
    /// alive until the data path answers. The seed is read-only reference data, so a static list is
    /// fine here (Module 3's rule is about *user/tenant state* in statics, which this is not).
    /// </summary>
    public class WorkOrderRepository
    {
        private static readonly List<WorkOrder> Seed = CreateSeed();

        public static readonly Tenant[] Tenants =
        {
            new Tenant { Id = "contoso",   Name = "Contoso Field Services" },
            new Tenant { Id = "fabrikam",  Name = "Fabrikam Utilities" },
            new Tenant { Id = "northwind", Name = "Northwind Facilities" },
        };

        /// <summary>The "known query" the smoke test and the database health check run.</summary>
        public int CountOpen(string tenantId)
            => Seed.Count(w => w.TenantId == tenantId && w.Status != WorkOrderStatus.Completed && w.Status != WorkOrderStatus.Cancelled);

        public int CountAll() => Seed.Count;

        private static List<WorkOrder> CreateSeed()
        {
            // 60 deterministic rows across three tenants so the count is stable between runs.
            string[] titles = { "Boiler inspection", "HVAC filter swap", "Meter replacement", "Leak repair", "Generator test", "Lighting retrofit" };
            string[] customers = { "Harbor Mall", "Riverside Clinic", "North Depot", "Bay Tower", "Elm Street School" };
            string[] techs = { "ben.tech", "dana.tech", "eli.tech" };
            var rows = new List<WorkOrder>();
            var start = new DateTime(2026, 8, 1, 8, 0, 0, DateTimeKind.Utc);
            for (int i = 0; i < 60; i++)
            {
                rows.Add(new WorkOrder
                {
                    Id = 1000 + i,
                    TenantId = Tenants[i % 3].Id,
                    Title = titles[i % titles.Length],
                    Customer = customers[i % customers.Length],
                    Site = $"Site {(i % 7) + 1}",
                    Status = (WorkOrderStatus)(i % 7),
                    Priority = (Priority)(i % 4),
                    AssignedTo = techs[i % techs.Length],
                    CreatedUtc = start.AddHours(i * 5),
                    DueUtc = start.AddDays(3 + (i % 9)),
                    Version = 1 + (i % 3),
                });
            }
            return rows;
        }
    }
}
