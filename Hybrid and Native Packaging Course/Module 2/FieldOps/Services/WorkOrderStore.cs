using System;
using System.Collections.Generic;
using FieldOps.Model;

namespace FieldOps.Services
{
    /// <summary>
    /// The stand-in for the FieldOps database: one in-memory list shared by every session of this
    /// server process, seeded with the orders the Module 2 walkthrough shows. It is deliberately
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
            var today = DateTime.Today;
            var now = DateTime.UtcNow;
            WorkOrder O(string n, string c, string asset, string a, string f, WorkOrderPriority p, WorkOrderStatus s, DateTime due, string t) =>
                new WorkOrder { Number = n, Customer = c, Asset = asset, SiteAddress = a, Fault = f, Priority = p, Status = s, DueAt = due, AssignedTechnician = t, Version = 1, LastChangedUtc = now };

            // The five orders of the Module 2 walkthrough, due today and tomorrow.
            return new List<WorkOrder>
            {
                O("WO-4821", "Riverside Plant", "Pump 3", "Riverside Road, Pump House 3", "Pump 3 seal leaking", WorkOrderPriority.High, WorkOrderStatus.OnSite, today.AddHours(11.5), "dana"),
                O("WO-4824", "Kilmore Depot", "Chiller 1", "Kilmore Depot, Bay 4", "Chiller 1 short-cycling", WorkOrderPriority.High, WorkOrderStatus.Assigned, today.AddHours(13), "dana"),
                O("WO-4830", "Aldergate Mill", "Line 2 drive", "Aldergate Mill, Line 2", "Line 2 drive overheating", WorkOrderPriority.Normal, WorkOrderStatus.Assigned, today.AddHours(15.25), "dana"),
                O("WO-4833", "Harbour Lift Station", "Panel B", "Harbour Lift Station, Control Room", "Panel B alarm fault", WorkOrderPriority.Normal, WorkOrderStatus.Assigned, today.AddHours(16 + 40 / 60.0), "dana"),
                O("WO-4836", "Westbrook Store", "Cold room", "Westbrook Retail Park, Unit 3", "Cold room door heater", WorkOrderPriority.Low, WorkOrderStatus.Assigned, today.AddDays(1).AddHours(9), "dana"),
                // Someone else's work: must never appear on Dana's list.
                O("WO-4827", "Northgate Substation", "Transformer T2", "Northgate Industrial Park", "Transformer fan fault", WorkOrderPriority.Normal, WorkOrderStatus.Assigned, today.AddHours(14), "marek"),
            };
        }    }
}
