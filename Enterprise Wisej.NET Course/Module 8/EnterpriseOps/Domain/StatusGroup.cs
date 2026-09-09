using System;
using System.Collections.Generic;

namespace EnterpriseOps.Domain
{
    /// <summary>
    /// The four reporting groups the work-order history chart shows. This is a domain rule (which
    /// statuses count as "open"), so it lives here and not in the chart wrapper: the widget only ever
    /// receives a key, a label and a number.
    /// </summary>
    public static class StatusGroup
    {
        public const string Open = "open";
        public const string OnHold = "onhold";
        public const string Escalated = "escalated";
        public const string Done = "done";

        /// <summary>Display order of the groups (left to right in the chart).</summary>
        public static readonly IReadOnlyList<string> All = new[] { Open, OnHold, Escalated, Done };

        public static string KeyFor(WorkOrderStatus status)
        {
            switch (status)
            {
                case WorkOrderStatus.New:
                case WorkOrderStatus.Assigned:
                case WorkOrderStatus.InProgress:
                    return Open;
                case WorkOrderStatus.OnHold:
                    return OnHold;
                case WorkOrderStatus.Escalated:
                    return Escalated;
                case WorkOrderStatus.Completed:
                case WorkOrderStatus.Cancelled:
                    return Done;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown work order status.");
            }
        }

        public static string LabelFor(string groupKey)
        {
            switch (groupKey)
            {
                case Open: return "Open";
                case OnHold: return "On hold";
                case Escalated: return "Escalated";
                case Done: return "Done";
                default: return groupKey;
            }
        }

        public static bool IsKnown(string groupKey)
            => groupKey == Open || groupKey == OnHold || groupKey == Escalated || groupKey == Done;

        /// <summary>The human label of a status as the timeline shows it ("Created", "On hold" …).</summary>
        public static string StatusLabel(WorkOrderStatus status)
        {
            switch (status)
            {
                case WorkOrderStatus.New: return "Created";
                case WorkOrderStatus.Assigned: return "Assigned";
                case WorkOrderStatus.InProgress: return "In progress";
                case WorkOrderStatus.OnHold: return "On hold";
                case WorkOrderStatus.Escalated: return "Escalated";
                case WorkOrderStatus.Completed: return "Completed";
                case WorkOrderStatus.Cancelled: return "Cancelled";
                default: return status.ToString();
            }
        }
    }
}
