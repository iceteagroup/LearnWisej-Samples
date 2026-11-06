using System;
using System.Collections.Generic;
using FieldOps.Model;

namespace FieldOps.Services
{
    /// <summary>
    /// The stand-in for the FieldOps database: one in-memory list shared by every session of this
    /// server process, seeded with the orders the Module 1 walkthrough shows. It is deliberately
    /// process-wide (two browser windows must see each other's changes) and every access is
    /// locked. Swap it for a real repository and only this class changes.
    /// </summary>
    public sealed class WorkOrderStore
    {
        private readonly object _gate = new object();
        private readonly List<WorkOrder> _orders;

        public static WorkOrderStore Shared { get; } = new WorkOrderStore(Seed());

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
            var now = DateTime.UtcNow;
            WorkOrder O(string n, string c, string a, string f, WorkOrderPriority p, WorkOrderStatus s, string t) =>
                new WorkOrder { Number = n, Customer = c, SiteAddress = a, Fault = f, Priority = p, Status = s, AssignedTechnician = t, Version = 1, LastChangedUtc = now };

            return new List<WorkOrder>
            {
                O("WO-4181", "Harborview Cold Store", "14 Quay Road, Harborview", "Chiller 3 not holding temp", WorkOrderPriority.High, WorkOrderStatus.OnSite, "dana"),
                O("WO-4184", "Meridian Print Works", "2 Meridian Way, Eastfield", "Quarterly compressor service", WorkOrderPriority.Normal, WorkOrderStatus.Accepted, "dana"),
                O("WO-4190", "Lakeside Brewery", "Unit 9, Lakeside Trading Estate", "Glycol pump vibration", WorkOrderPriority.High, WorkOrderStatus.Assigned, "dana"),
                O("WO-4193", "Castle Park Leisure", "Castle Park, Leisure Centre Plant Room", "Replace door seal, unit 2", WorkOrderPriority.Low, WorkOrderStatus.Assigned, "dana"),
                // Someone else's work: must never appear on Dana's list.
                O("WO-4187", "Northgate Substation", "Northgate Industrial Park", "Transformer fan fault", WorkOrderPriority.Normal, WorkOrderStatus.Assigned, "marek"),
                O("WO-4176", "Kingsway Depot", "Kingsway, Depot Gate B", "Compressor service", WorkOrderPriority.Normal, WorkOrderStatus.Completed, "dana"),
            };
        }
    }
}
