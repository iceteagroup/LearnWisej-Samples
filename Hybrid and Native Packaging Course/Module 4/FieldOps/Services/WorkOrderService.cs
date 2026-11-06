using System;
using System.Collections.Generic;
using System.Linq;
using FieldOps.Model;

namespace FieldOps.Services
{
    /// <summary>A status change the rules do not allow. The message is written for the technician.</summary>
    public sealed class StatusTransitionException : Exception
    {
        public StatusTransitionException(string message) : base(message) { }
    }

    /// <summary>The order changed since this screen loaded it (another window, another device).</summary>
    public sealed class WorkOrderConflictException : Exception
    {
        public WorkOrderConflictException(string message) : base(message) { }
    }

    /// <summary>
    /// The only way a screen reads or changes a work order. The transition rule lives here and
    /// nowhere else, so no screen - browser tab, PWA window or Hybrid client - can move an order
    /// backwards or skip a step.
    /// </summary>
    public sealed class WorkOrderService
    {
        private readonly WorkOrderStore _store;

        public WorkOrderService(WorkOrderStore store)
        {
            _store = store;
        }

        /// <summary>The service every page of this server uses.</summary>
        public static WorkOrderService Current => new WorkOrderService(WorkOrderStore.Shared);

        public IReadOnlyList<Technician> Technicians => _store.Technicians;

        public Technician FindTechnician(string id) =>
            _store.Technicians.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase));

        /// <summary>One technician's open orders (everything not yet Completed), as copies.</summary>
        public IReadOnlyList<WorkOrder> GetOpenOrders(string technicianId)
        {
            return _store.Read(all => all
                .Where(o => o.AssignedTechnician == technicianId && o.Status != WorkOrderStatus.Completed)
                .OrderBy(o => o.DueAt)
                .Select(o => o.Clone())
                .ToList());
        }

        /// <summary>
        /// Today's orders for one technician, finished ones included (the desktop list shows the
        /// whole day), newest number first.
        /// </summary>
        public IReadOnlyList<WorkOrder> GetTodaysOrders(string technicianId)
        {
            return _store.Read(all => all
                .Where(o => o.AssignedTechnician == technicianId)
                .OrderByDescending(o => o.Number, StringComparer.Ordinal)
                .Select(o => o.Clone())
                .ToList());
        }

        /// <summary>Dispatch: a new order for a technician. Raises <see cref="OrderFeed.NewOrder"/>.</summary>
        public WorkOrder Assign(string technicianId, string customer, string address, string task, WorkOrderPriority priority)
        {
            if (FindTechnician(technicianId) == null)
                throw new KeyNotFoundException($"No technician '{technicianId}'.");
            var order = _store.Write(all =>
            {
                var o = new WorkOrder
                {
                    Number = _store.NextNumberUnlocked(all), Customer = customer, SiteAddress = address, Fault = task,
                    Priority = priority, Status = WorkOrderStatus.Assigned, AssignedTechnician = technicianId,
                    DueAt = DateTime.Now.AddHours(2), Version = 1, LastChangedUtc = DateTime.UtcNow,
                };
                all.Add(o);
                return o.Clone();
            });
            OrderFeed.Publish(order);
            return order;
        }

        public WorkOrder Get(string number)
        {
            return _store.Read(all => all.FirstOrDefault(o => o.Number == number)?.Clone())
                ?? throw new KeyNotFoundException($"Work order {number} does not exist.");
        }

        /// <summary>
        /// Applies a status change. <paramref name="expectedVersion"/> is the version the screen
        /// showed; if the order moved on since, the change is refused instead of overwriting it.
        /// </summary>
        public WorkOrder UpdateStatus(string number, WorkOrderStatus newStatus, int expectedVersion, string technicianId)
        {
            return _store.Write(all =>
            {
                var order = all.FirstOrDefault(o => o.Number == number)
                    ?? throw new KeyNotFoundException($"Work order {number} does not exist.");

                if (order.AssignedTechnician != technicianId)
                    throw new StatusTransitionException($"{number} is not assigned to you, so its status cannot be changed here.");

                if (order.Version != expectedVersion)
                    throw new WorkOrderConflictException(
                        $"{number} changed after this screen loaded it (version {expectedVersion}, now {order.Version}).");

                ValidateTransition(order, newStatus);

                order.Status = newStatus;
                order.Version++;
                order.LastChangedUtc = DateTime.UtcNow;
                return order.Clone();
            });
        }

        /// <summary>
        /// Records a meter reading taken on site. The order must be On site; the reading must be a
        /// non-negative number. Same version check as a status change.
        /// </summary>
        public WorkOrder SubmitReading(string number, string readingText, string notes, int expectedVersion, string technicianId)
        {
            if (!decimal.TryParse((readingText ?? "").Trim(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var reading) || reading < 0)
                throw new StatusTransitionException($"\"{readingText}\" is not a meter reading. Enter a number such as 412.8.");

            return _store.Write(all =>
            {
                var order = all.FirstOrDefault(o => o.Number == number)
                    ?? throw new KeyNotFoundException($"Work order {number} does not exist.");
                if (order.AssignedTechnician != technicianId)
                    throw new StatusTransitionException($"{number} is not assigned to you.");
                if (order.Version != expectedVersion)
                    throw new WorkOrderConflictException($"{number} changed after this screen loaded it (version {expectedVersion}, now {order.Version}).");
                if (order.Status != WorkOrderStatus.OnSite)
                    throw new StatusTransitionException($"{number} is {WorkOrder.StatusText(order.Status)}: a reading is taken On site.");

                order.MeterReading = reading;
                order.Notes = notes;
                order.Version++;
                order.LastChangedUtc = DateTime.UtcNow;
                return order.Clone();
            });
        }

        /// <summary>Accepted -> On site -> Completed, one step at a time and only forwards.</summary>
        public static void ValidateTransition(WorkOrder order, WorkOrderStatus next)
        {
            var current = order.Status;
            if (next == current)
                throw new StatusTransitionException(
                    $"{order.Number} is already {WorkOrder.StatusText(current)}.");
            if (next < current)
                throw new StatusTransitionException(
                    $"{order.Number} is {WorkOrder.StatusText(current)}. An order only moves forward, so it cannot go back to {WorkOrder.StatusText(next)}.");
            if (next > current + 1)
                throw new StatusTransitionException(
                    $"{order.Number} must be {WorkOrder.StatusText(current + 1)} before it can be {WorkOrder.StatusText(next)}.");
        }
    }
}
