namespace EnterpriseOps.Domain
{
    /// <summary>
    /// The status rules, as pure domain logic: no DbContext, no Form, nothing to mock.
    /// The command service asks these questions inside its transaction, after loading the aggregate.
    /// </summary>
    public static class WorkOrderTransitions
    {
        /// <summary>
        /// Approve moves InProgress → Completed. Everything else is rejected with the message the user
        /// sees (already a user message: the service maps it to WO_STATE_INVALID, nothing is thrown).
        /// </summary>
        public static bool CanApprove(WorkOrderStatus status, out string reason)
        {
            switch (status)
            {
                case WorkOrderStatus.InProgress:
                    reason = null;
                    return true;
                case WorkOrderStatus.OnHold:
                    reason = "This work order is on hold — resolve the hold before approving.";
                    return false;
                case WorkOrderStatus.Completed:
                    reason = "This work order is already approved.";
                    return false;
                case WorkOrderStatus.Cancelled:
                    reason = "This work order was cancelled and cannot be approved.";
                    return false;
                case WorkOrderStatus.Escalated:
                    reason = "This work order is escalated — close the escalation before approving.";
                    return false;
                default:
                    reason = "This work order has not been started — assign and start it before approving.";
                    return false;
            }
        }

        /// <summary>Update may change the status only along these edges; Approve owns the Completed edge.</summary>
        public static bool CanChangeStatus(WorkOrderStatus from, WorkOrderStatus to, out string reason)
        {
            reason = null;
            if (from == to)
                return true;

            if (to == WorkOrderStatus.Completed)
            {
                reason = "Use Approve to complete a work order.";
                return false;
            }

            if (from == WorkOrderStatus.Completed || from == WorkOrderStatus.Cancelled)
            {
                reason = $"A {from} work order cannot be reopened.";
                return false;
            }

            return true;
        }
    }
}
