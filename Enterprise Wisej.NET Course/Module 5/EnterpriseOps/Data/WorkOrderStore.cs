using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The fake database: 6,000 work orders across three tenants, seeded deterministically, held in memory
    /// for the life of the process. It plays the role of the DbContext + the WorkOrders table, which is why
    /// it is the one process-wide singleton in the sample (shared data, not user state — user and tenant
    /// state live in <c>SessionContext</c>).
    ///
    /// Every read goes through <see cref="Snapshot"/> and every write through a method that takes the lock,
    /// so a batch "commit" is one atomic step per row — the per-row transaction the lab talks about.
    /// </summary>
    public sealed class WorkOrderStore
    {
        // Lazy, not a plain static initializer: static fields are initialized in declaration order, and Seed()
        // reads the Templates / Sites / Customers arrays declared further down. Creating the instance eagerly
        // here would run the constructor while those arrays are still null.
        private static readonly Lazy<WorkOrderStore> Singleton =
            new Lazy<WorkOrderStore>(() => new WorkOrderStore(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static WorkOrderStore Instance => Singleton.Value;

        private readonly object _gate = new object();
        private readonly Dictionary<int, WorkOrder> _orders = new Dictionary<int, WorkOrder>();
        private readonly List<Technician> _technicians = new List<Technician>();
        private readonly List<Tenant> _tenants = new List<Tenant>();

        public const int SeedCount = 6000;

        private WorkOrderStore()
        {
            Seed();
        }

        public IReadOnlyList<Tenant> Tenants => _tenants;
        public IReadOnlyList<Technician> Technicians => _technicians;

        public Technician FindTechnician(string userName) =>
            _technicians.FirstOrDefault(t => string.Equals(t.UserName, userName, StringComparison.OrdinalIgnoreCase));

        /// <summary>A copy of the current rows — the "table" a query runs against.</summary>
        public IReadOnlyList<WorkOrder> Snapshot()
        {
            lock (_gate)
                return _orders.Values.ToList();
        }

        public int Count
        {
            get { lock (_gate) return _orders.Count; }
        }

        public WorkOrder Find(int id)
        {
            lock (_gate)
                return _orders.TryGetValue(id, out var order) ? order : null;
        }

        /// <summary>
        /// The write the batch uses: checks the version, applies the assignment, bumps the version.
        /// One order, one lock, one commit — the row-level transaction the lab asks for.
        /// </summary>
        public bool TryReassign(int id, int expectedVersion, string technician, out WorkOrder updated)
        {
            lock (_gate)
            {
                updated = null;
                if (!_orders.TryGetValue(id, out var order) || order.Version != expectedVersion)
                    return false;

                order.AssignedTo = technician;
                if (order.Status == WorkOrderStatus.New)
                    order.Status = WorkOrderStatus.Assigned;
                order.Version++;
                updated = order;
                return true;
            }
        }

        #region Seed data

        private static readonly string[] Customers =
        {
            "Contoso Retail", "Fabrikam Logistics", "Northwind Foods", "Adventure Works", "Tailspin Airlines",
            "Litware Pharma", "Wingtip Hotels", "Proseware Data", "Alpine Ski House", "Coho Winery",
        };

        private static readonly string[] Sites =
        {
            "Building A", "Building B", "Building C", "Warehouse 1", "Warehouse 2", "Plant North", "Plant South",
            "Loading dock", "Data hall", "Lobby", "Roof", "Parking deck",
        };

        // Title template, required certification (null = none), typical priority weight.
        private static readonly (string Title, string Certification)[] Templates =
        {
            ("Replace HVAC filter — {site}", "hvac"),
            ("Loading dock pump inspection", null),
            ("Elevator quarterly certification", "elevator"),
            ("Calibrate pressure sensors", null),
            ("Replace lobby lighting", "electrical"),
            ("Stale cache on node B", null),
            ("Boiler annual inspection", "boiler"),
            ("Fire alarm panel test — {site}", "fire-safety"),
            ("Chiller compressor noise", "hvac"),
            ("Sprinkler head replacement", "fire-safety"),
            ("Generator load test", "electrical"),
            ("Badge reader offline — {site}", null),
            ("Roof drain cleaning", null),
            ("Cooling tower water treatment", "hvac"),
            ("Freight elevator door sensor", "elevator"),
            ("Emergency lighting battery swap", "electrical"),
            ("Steam trap survey", "boiler"),
            ("Server room humidity alarm", "hvac"),
            ("Dock leveler hydraulic leak", null),
            ("Parking gate arm stuck", null),
        };

        private void Seed()
        {
            _tenants.Add(new Tenant { Id = "contoso", Name = "Contoso" });
            _tenants.Add(new Tenant { Id = "fabrikam", Name = "Fabrikam" });
            _tenants.Add(new Tenant { Id = "northwind", Name = "Northwind" });

            AddTechnician("r.alvarez", "Rosa Alvarez", "hvac", "elevator", "boiler");
            AddTechnician("t.nguyen", "Tam Nguyen", "hvac", "boiler", "electrical");
            AddTechnician("s.patel", "Sana Patel", "hvac", "elevator", "fire-safety", "electrical");
            AddTechnician("j.kim", "Jae Kim", "hvac");
            AddTechnician("ben.tech", "Ben Tech", "hvac", "electrical");

            var rng = new Random(5);                      // deterministic: the same 6,000 rows on every run
            var today = DateTime.UtcNow.Date;
            var technicians = _technicians.Select(t => t.UserName).ToArray();
            var statuses = (WorkOrderStatus[])Enum.GetValues(typeof(WorkOrderStatus));

            for (int i = 0; i < SeedCount; i++)
            {
                int id = 100001 + i;
                var template = Templates[rng.Next(Templates.Length)];
                string site = Sites[rng.Next(Sites.Length)];

                // Tenant split ≈ 50 / 30 / 20 so every tenant has enough pages.
                int t = rng.Next(100);
                string tenantId = t < 50 ? "contoso" : t < 80 ? "fabrikam" : "northwind";

                // Status mix: most open, a fifth closed.
                int s = rng.Next(100);
                WorkOrderStatus status =
                    s < 22 ? WorkOrderStatus.New :
                    s < 45 ? WorkOrderStatus.Assigned :
                    s < 65 ? WorkOrderStatus.InProgress :
                    s < 72 ? WorkOrderStatus.OnHold :
                    s < 78 ? WorkOrderStatus.Escalated :
                    s < 94 ? WorkOrderStatus.Completed : WorkOrderStatus.Cancelled;

                int p = rng.Next(100);
                Priority priority = p < 30 ? Priority.Low : p < 65 ? Priority.Normal : p < 88 ? Priority.High : Priority.Critical;

                int ageDays = rng.Next(0, 120);
                var created = today.AddDays(-ageDays).AddHours(rng.Next(6, 19));
                var due = created.AddDays(rng.Next(1, 30));

                _orders[id] = new WorkOrder
                {
                    Id = id,
                    TenantId = tenantId,
                    Title = template.Title.Replace("{site}", site),
                    Customer = Customers[rng.Next(Customers.Length)],
                    Site = site,
                    Status = status,
                    Priority = priority,
                    AssignedTo = status == WorkOrderStatus.New ? "" : technicians[rng.Next(technicians.Length)],
                    CreatedUtc = created,
                    DueUtc = due,
                    Version = 1 + rng.Next(0, 4),
                    RequiredCertification = template.Certification,
                    OpenApprovalId = status == WorkOrderStatus.Escalated && rng.Next(4) == 0 ? "APR-" + (1000 + rng.Next(900)) : null,
                };
            }

            // The six rows the walkthrough video shows on its first page, so "pump" finds the same order.
            // WO-100236 has an approval in flight (APR-1042), so reassigning WO-100234..236 to s.patel gives the
            // video's "2 succeeded, 1 failed — locked by an open approval".
            Anchor(100231, "Replace HVAC filter — Building A", Priority.High, "r.alvarez", "hvac", today.AddDays(2));
            Anchor(100232, "Loading dock pump inspection", Priority.High, "t.nguyen", null, today.AddDays(2));
            Anchor(100233, "Elevator quarterly certification", Priority.Normal, "s.patel", "elevator", today.AddDays(3));
            Anchor(100234, "Calibrate pressure sensors", Priority.Normal, "t.nguyen", null, today.AddDays(4));
            Anchor(100235, "Replace lobby lighting", Priority.Low, "j.kim", "electrical", today.AddDays(5));
            Anchor(100236, "Stale cache on node B", Priority.Low, "j.kim", null, today.AddDays(6), approvalId: "APR-1042");
        }

        private void Anchor(int id, string title, Priority priority, string assignedTo, string certification, DateTime due,
                            string approvalId = null)
        {
            var order = _orders[id];
            order.TenantId = "contoso";
            order.Title = title;
            order.Customer = "Contoso Retail";
            order.Site = "Building A";
            order.Status = WorkOrderStatus.Assigned;
            order.Priority = priority;
            order.AssignedTo = assignedTo;
            order.RequiredCertification = certification;
            order.OpenApprovalId = approvalId;
            order.DueUtc = due;
            order.Version = 2;
        }

        private void AddTechnician(string userName, string displayName, params string[] certifications)
        {
            var tech = new Technician { UserName = userName, DisplayName = displayName };
            foreach (var c in certifications)
                tech.Certifications.Add(c);
            _technicians.Add(tech);
        }

        #endregion

        public override string ToString() =>
            string.Format(CultureInfo.InvariantCulture, "WorkOrderStore({0} rows, {1} tenants)", Count, _tenants.Count);
    }
}
