using System;
using System.Collections.Generic;
using FieldOps.Model;

namespace FieldOps.Services
{
    /// <summary>
    /// The stand-in for the FieldOps database: one in-memory list shared by every session of this
    /// server process, seeded with the orders the Module 4 and 5 walkthroughs show. It is deliberately
    /// process-wide (two browser windows must see each other's changes) and every access is
    /// locked. Swap it for a real repository and only this class changes.
    /// </summary>
    public sealed class WorkOrderStore
    {
        private readonly object _gate = new object();
        private readonly List<WorkOrder> _orders;

        /// <summary>Replaced by the offline app with the snapshot saved on the device.</summary>
        public static WorkOrderStore Shared { get; set; } = new WorkOrderStore(Seed());

        public WorkOrderStore(List<WorkOrder> orders)
        {
            _orders = orders;
        }

        public IReadOnlyList<Technician> Technicians { get; } = new[]
        {
            new Technician { Id = "dana", Name = "Dana Whitfield", Van = "Van 12" },
            new Technician { Id = "marek", Name = "Marek Sobczak", Van = "Van 7" },
        };

        public T Read<T>(Func<List<WorkOrder>, T> query)
        {
            lock (_gate) return query(_orders);
        }

        public T Write<T>(Func<List<WorkOrder>, T> change)
        {
            lock (_gate) return change(_orders);
        }

        private static List<WorkOrder> Seed()
        {
            var today = DateTime.Today;
            var now = DateTime.UtcNow;
            WorkOrder O(string n, string c, string a, string task, WorkOrderPriority p, WorkOrderStatus s, double dueHour, string t) =>
                new WorkOrder { Number = n, Customer = c, SiteAddress = a, Fault = task, Priority = p, Status = s, DueAt = today.AddHours(dueHour), AssignedTechnician = t, Version = 1, LastChangedUtc = now };

            // WO-4187 keeps the Module 5 device features; the Today list of the Module 6 walkthrough follows.
            return new List<WorkOrder>
            {
                new WorkOrder
                {
                    Number = "WO-4187", Customer = "Meridian Foods", SiteAddress = "Unit 7, Halstow Industrial Park",
                    Fault = "Chiller 3 — pressure fault", Priority = WorkOrderPriority.High, Status = WorkOrderStatus.OnSite,
                    DueAt = DateTime.Now.AddHours(2).AddMinutes(10), AssignedTechnician = "dana", Version = 1, LastChangedUtc = now,
                    SiteLatitude = 51.4012, SiteLongitude = 0.6131,
                },
                // The capstone's "Today" list (Module 6 walkthrough).
                O("WO-4121", "Wick Depot", "Depot · Wick", "Meter swap", WorkOrderPriority.Low, WorkOrderStatus.Completed, 8, "dana"),
                O("WO-4119", "Thurso Treatment Works", "Site 12 · Thurso", "Compressor inspection", WorkOrderPriority.Normal, WorkOrderStatus.OnSite, 11, "dana"),
                O("WO-4118", "Kirkwall Harbour", "Bay 3 · Kirkwall", "Pump seal replacement", WorkOrderPriority.High, WorkOrderStatus.Assigned, 14, "dana"),
                O("WO-4812", "Kingsway Depot", "Kingsway, Depot Gate B", "Valve calibration", WorkOrderPriority.Normal, WorkOrderStatus.Accepted, 13, "marek"),            };
        }

        /// <summary>The next number the dispatcher hands out. Call inside <see cref="Write"/>.</summary>
        public string NextNumberUnlocked(List<WorkOrder> all)
        {
            int max = 0;
            foreach (var o in all)
                if (int.TryParse(o.Number.AsSpan(3), out var n) && n > max) max = n;
            return "WO-" + Math.Max(max + 1, 4830);
        }    }
}
