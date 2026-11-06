using System;
using System.Collections.Generic;
using FieldOps.Model;

namespace FieldOps.Services
{
    /// <summary>
    /// The stand-in for the FieldOps database: one in-memory list shared by every session of this
    /// server process, seeded with the orders the Module 3 walkthrough shows. It is deliberately
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

            // The four orders of the Module 3 walkthrough. WO-4830 (Elmtree Pumping Station) is
            // not here: it arrives later, through the order feed (POST /dispatch/assign).
            return new List<WorkOrder>
            {
                O("WO-4821", "Harbour Pumping Station", "Harbour Road, Pump House 2", "Pump seal replacement", WorkOrderPriority.High, WorkOrderStatus.Assigned, 14, "dana"),
                O("WO-4818", "Meadow Lane Substation", "Meadow Lane, Gate 3", "Quarterly inspection", WorkOrderPriority.Normal, WorkOrderStatus.OnSite, 11, "dana"),
                O("WO-4815", "Kingsway Depot", "Kingsway, Depot Gate B", "Compressor service", WorkOrderPriority.Normal, WorkOrderStatus.Completed, 9, "dana"),
                O("WO-4809", "Riverside Plant", "Riverside Road, Pump House 3", "Valve calibration", WorkOrderPriority.Low, WorkOrderStatus.Completed, 8, "dana"),
                O("WO-4812", "Northgate Substation", "Northgate Industrial Park", "Transformer fan fault", WorkOrderPriority.Normal, WorkOrderStatus.Accepted, 13, "marek"),
                O("WO-4807", "Castle Park Leisure", "Castle Park, Plant Room", "Door seal, unit 2", WorkOrderPriority.Low, WorkOrderStatus.Assigned, 15, "marek"),
            };
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
