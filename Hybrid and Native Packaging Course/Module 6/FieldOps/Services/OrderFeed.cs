using System;
using FieldOps.Model;

namespace FieldOps.Services
{
    public sealed class OrderEventArgs : EventArgs
    {
        public OrderEventArgs(WorkOrder order) { Order = order; }
        public WorkOrder Order { get; }
    }

    /// <summary>
    /// New orders from dispatch, process-wide. It is raised on the dispatcher's thread, NOT in a
    /// technician's session: a page that subscribes must hop into its own session with
    /// <c>Application.RunInContext(this, ...)</c> before touching controls or <c>Device</c>,
    /// and push the change with <c>Application.Update(this)</c> (Wisej.NET server push).
    /// </summary>
    public static class OrderFeed
    {
        public static event EventHandler<OrderEventArgs> NewOrder;

        public static void Publish(WorkOrder order)
        {
            OpsLog.Info($"dispatch: {order.Number} {order.Customer} assigned to {order.AssignedTechnician}");
            NewOrder?.Invoke(null, new OrderEventArgs(order));
        }
    }
}
