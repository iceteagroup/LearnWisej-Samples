using System;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// What a screen edits: the fields the editor shows plus the <see cref="ConcurrencyToken"/> the record
    /// carried when it was loaded. The entity never leaves the data layer, and the token is opaque to the UI —
    /// the editor displays it and hands it back untouched, it never invents one.
    ///
    /// This is <b>tab-scoped state</b>: it belongs to the browser window that opened the work order. Two tabs
    /// hold two edit models with two tokens, which is exactly why the session context must not hold "the
    /// current work order".
    /// </summary>
    public sealed class WorkOrderEditModel
    {
        public int Id { get; init; }
        public string TenantId { get; init; }
        public string Title { get; set; }
        public WorkOrderStatus Status { get; set; }
        public string Customer { get; init; }
        public string Site { get; init; }
        public string AssignedTo { get; init; }
        public Priority Priority { get; init; }
        public string ModifiedBy { get; init; }
        public DateTime ModifiedUtc { get; init; }

        /// <summary>The version this copy is based on. Sent back with the save; compared by the store.</summary>
        public ConcurrencyToken Token { get; init; }

        public static WorkOrderEditModel From(WorkOrder order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            return new WorkOrderEditModel
            {
                Id = order.Id,
                TenantId = order.TenantId,
                Title = order.Title,
                Status = order.Status,
                Customer = order.Customer,
                Site = order.Site,
                AssignedTo = order.AssignedTo,
                Priority = order.Priority,
                ModifiedBy = order.ModifiedBy,
                ModifiedUtc = order.ModifiedUtc,
                Token = ConcurrencyToken.From(order),
            };
        }

        /// <summary>A copy of this model carrying the user's unsaved edits — what the conflict dialog calls "your edit".</summary>
        public WorkOrderEditModel WithEdits(string title, WorkOrderStatus status)
        {
            return new WorkOrderEditModel
            {
                Id = Id,
                TenantId = TenantId,
                Title = title,
                Status = status,
                Customer = Customer,
                Site = Site,
                AssignedTo = AssignedTo,
                Priority = Priority,
                ModifiedBy = ModifiedBy,
                ModifiedUtc = ModifiedUtc,
                Token = Token,
            };
        }
    }
}
