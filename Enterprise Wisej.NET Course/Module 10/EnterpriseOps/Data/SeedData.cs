using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// In-memory fake data. No database, no network, no cloud account — the whole module runs from these lists.
    ///
    /// The rows are deterministic (a fixed <see cref="Random"/> seed) so two learners see the same screen and the
    /// README can name row counts. Most rows belong to <c>fabrikam</c>, the tenant the walkthrough signs into;
    /// the <c>contoso</c> rows exist so the cross-tenant attempt has something real to reach for.
    /// </summary>
    public static class SeedData
    {
        private static readonly string[] Titles =
        {
            "Compressor 3 — overheating alarm", "Line 2 conveyor — belt slip", "Chiller A — pressure drift",
            "Packaging robot — calibration due", "Boiler 1 — annual inspection", "Dust extractor — filter change",
            "Pump station — vibration warning", "Weighbridge — load cell fault", "Gate motor — intermittent stop",
            "Cold store — door seal", "Palletiser — safety curtain fault", "Air line — pressure loss overnight",
            "Silo 4 — level sensor drift", "Sorter — barcode misreads", "Generator — fuel polishing due",
            "Roof unit — condensate overflow", "Forklift charger — trip", "CIP skid — valve stuck open",
            "Labeller — ribbon feed error", "Filler head 6 — drip",
        };

        private static readonly string[] Customers =
        {
            "Northgate Foods", "Harbour Logistics", "Pinewood Dairy", "Belmont Steel",
            "Riverside Brewing", "Kestrel Plastics", "Acorn Packaging",
        };

        private static readonly string[] Sites =
        {
            "Plant 1 · Leeds", "Plant 2 · Rotterdam", "DC North · Hull", "Plant 4 · Antwerp", "Depot · Bristol",
        };

        private static readonly string[] Technicians = { "l.romero", "svc.import", "m.weber", "unassigned" };

        /// <summary>40 work orders: 31 on fabrikam, 9 on contoso (the tenant this session must never reach).</summary>
        public static List<WorkOrder> WorkOrders()
        {
            var random = new Random(1010);
            var rows = new List<WorkOrder>();

            for (int i = 0; i < 40; i++)
            {
                bool otherTenant = i % 9 == 8;
                rows.Add(new WorkOrder
                {
                    Id = 1000 + i,
                    TenantId = otherTenant ? Tenants.Contoso.Id : Tenants.Fabrikam.Id,
                    Title = Titles[i % Titles.Length],
                    Customer = Customers[random.Next(Customers.Length)],
                    Site = Sites[random.Next(Sites.Length)],
                    Status = (WorkOrderStatus)random.Next(0, 6),
                    Priority = (Priority)random.Next(0, 4),
                    AssignedTo = Technicians[random.Next(Technicians.Length)],
                    CreatedUtc = DateTime.UtcNow.AddDays(-random.Next(1, 40)).AddMinutes(-random.Next(0, 900)),
                    DueUtc = DateTime.UtcNow.AddDays(random.Next(-3, 14)),
                    Version = 1,
                });
            }

            // Two rows the lab points at by name: one waiting for an approval, one on the other tenant.
            rows[2].Status = WorkOrderStatus.Escalated;
            rows[2].Priority = Priority.Critical;
            rows[2].Title = "Compressor 3 — overheating alarm";
            rows[8].Title = "Contoso · Chiller A — pressure drift";

            return rows;
        }

        /// <summary>
        /// Notes attached to work orders. Every one of them arrived from outside the application, and the third
        /// carries an injection payload — a customer portal will happily accept one.
        ///
        /// The payload is a defacement plus an inline event handler. It is harmless on purpose (it recolours a
        /// line and renames the browser tab) so the lab can be run safely; a real one would read the session and
        /// post it somewhere. The point is identical: the characters below become **markup** the moment a control
        /// with AllowHtml = true is given them.
        /// </summary>
        public static List<WorkOrderNote> Notes()
        {
            return new List<WorkOrderNote>
            {
                new WorkOrderNote
                {
                    Id = 1, WorkOrderId = 1002, TenantId = Tenants.Fabrikam.Id,
                    Source = "field app (technician)", Author = "l.romero",
                    CreatedUtc = DateTime.UtcNow.AddHours(-5),
                    Text = "Second alarm this week. Head pressure climbs after 20 minutes of run time.",
                },
                new WorkOrderNote
                {
                    Id = 2, WorkOrderId = 1002, TenantId = Tenants.Fabrikam.Id,
                    Source = "e-mail gateway", Author = "site.manager@northgate.example",
                    CreatedUtc = DateTime.UtcNow.AddHours(-3),
                    Text = "Production says the line stops at 14:00. Please attend before then — <b>urgent</b>.",
                },
                new WorkOrderNote
                {
                    Id = 3, WorkOrderId = 1002, TenantId = Tenants.Fabrikam.Id,
                    Source = "customer portal (public form)", Author = "\"maintenance\" <no-reply@unknown.example>",
                    CreatedUtc = DateTime.UtcNow.AddHours(-1),
                    Text = "Unit is down. <span style=\"color:#c0392b;font-weight:800\">CALL 1-555-0100 NOW — DO NOT DISPATCH</span>"
                         + "<img src=\"x\" onerror=\"document.title='pwned by note #3'\">"
                         + "<a href=\"javascript:void(0)\">confirm here</a>",
                },
            };
        }

        /// <summary>
        /// The audit history the screen opens with — the same rows the walkthrough video shows, plus enough
        /// earlier activity to look like a real trail. Written straight into the shared log, once per process.
        /// </summary>
        public static void SeedAuditHistory(AuditLog log)
        {
            DateTime today = DateTime.Now.Date;   // the times below are wall-clock, converted to UTC on the way in

            void Entry(string time, string user, string action, AuditResult result, string target, string detail)
            {
                log.Seed(new AuditEntry
                {
                    AtUtc = DateTime.SpecifyKind(today.Add(TimeSpan.Parse(time)), DateTimeKind.Local).ToUniversalTime(),
                    UserId = user,
                    TenantId = Tenants.Fabrikam.Id,
                    Action = action,
                    Result = result,
                    Target = target,
                    Detail = detail,
                    CorrelationId = Guid.NewGuid().ToString("N").Substring(0, 8),
                });
            }

            Entry("09:12:44", "svc.import", "SignIn", AuditResult.Ok, "", "claims mapped → ServiceAccount");
            Entry("09:14:02", "svc.import", "EditWorkOrders", AuditResult.Ok, "WO-1031", "granted through roles ServiceAccount");
            Entry("09:31:19", "m.weber", "SignIn", AuditResult.Ok, "", "claims mapped → Manager");
            Entry("09:33:50", "m.weber", "ViewAuditLog", AuditResult.Ok, "", "granted through roles Manager");
            Entry("09:41:26", "l.romero", "SignIn", AuditResult.Ok, "", "claims mapped → Technician");
            Entry("09:44:03", "l.romero", "EditWorkOrders", AuditResult.Ok, "WO-1007", "granted through roles Technician");
            Entry("09:47:55", "l.romero", "ApproveWorkOrders", AuditResult.Denied, "WO-1007", "missing permission for roles Technician");

            // The five rows on screen in the walkthrough, in the walkthrough's order.
            Entry("09:51:07", "svc.import", "EditWorkOrders", AuditResult.Ok, "WO-1018", "granted through roles ServiceAccount");
            Entry("09:55:12", "m.weber", "ExportData", AuditResult.Ok, "export #1 · 18 rows", "granted through roles Manager");
            Entry("09:58:30", "j.kim", "ExportData", AuditResult.Denied, "export #2 · 240 rows", "missing permission for roles Auditor");
            Entry("10:01:48", "m.weber", "ApproveWorkOrders", AuditResult.Ok, "WO-1024", "granted through roles Manager");
            Entry("10:02:11", "l.romero", "ApproveWorkOrders", AuditResult.Denied, "WO-1024", "missing permission for roles Technician");
        }

        /// <summary>The audit history must be seeded once per process, not once per session.</summary>
        public static void EnsureAuditHistory(AuditLog log)
        {
            lock (SeedGate)
            {
                if (_auditSeeded) return;
                SeedAuditHistory(log);
                _auditSeeded = true;
            }
        }

        private static readonly object SeedGate = new object();
        private static bool _auditSeeded;

        /// <summary>Row counts the README quotes, computed rather than typed.</summary>
        public static string Describe()
        {
            var rows = WorkOrders();
            return $"{rows.Count} work orders · {rows.Count(r => r.TenantId == Tenants.Fabrikam.Id)} fabrikam · "
                 + $"{rows.Count(r => r.TenantId == Tenants.Contoso.Id)} contoso";
        }
    }
}
