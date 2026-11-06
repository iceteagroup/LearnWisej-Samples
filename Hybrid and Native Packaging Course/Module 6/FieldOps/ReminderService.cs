using System;
using System.Collections.Generic;
using FieldOps.Model;
using FieldOps.Services;
using Wisej.Hybrid;
using Wisej.Web;

namespace FieldOps
{
    /// <summary>
    /// "Next job" thirty minutes before the appointment, in the phone's tray - a LOCAL notification:
    /// no push server, and it fires even if FieldOps is closed by then. Scheduled when the
    /// appointment is assigned (the order feed) or first seen in this session. In a browser there is
    /// no tray: the job list, kept current by server push, carries the reminder.
    /// </summary>
    public static class ReminderService
    {
        public static readonly TimeSpan Lead = TimeSpan.FromMinutes(30);

        private static HashSet<string> Scheduled =>
            Application.Session["Reminders"] as HashSet<string> ?? (HashSet<string>)(Application.Session["Reminders"] = new HashSet<string>());

        /// <summary>Returns a sentence for the screen, or null when nothing was scheduled.</summary>
        public static string ScheduleFor(WorkOrder order)
        {
            if (!DeviceCapabilities.Current.LocalReminders) return null;
            if (order.Status == WorkOrderStatus.Completed || order.DueAt == default) return null;
            if (!Scheduled.Add(order.Number)) return null;

            var at = order.DueAt - Lead;
            if (at <= DateTime.Now) at = DateTime.Now.AddSeconds(5);   // already inside the window: remind now

            Device.LocalNotification.Schedule("Next job", $"{order.Number} · {order.Customer} at {order.DueAt:HH:mm}", at);
            Application.Session["ReminderFor"] = order.Number;
            OpsLog.Info($"reminder: Device.LocalNotification.Schedule \"Next job\" {order.Number} at {at:HH:mm} (session {Application.SessionId[..8]})");
            UsageTelemetry.Record("reminder.schedule", "ok");
            return $"Reminder set for {at:HH:mm}";
        }

        /// <summary>Call once per page: a tap on the reminder opens that work order.</summary>
        public static void HandleResponses(Action<string> openOrder)
        {
            if (!DeviceCapabilities.Current.LocalReminders) return;
            Device.LocalNotification.Responded += (s, e) =>
            {
                var number = Application.Session["ReminderFor"] as string;
                OpsLog.Info($"reminder: Responded -> {number ?? "(none)"}");
                if (number != null) openOrder(number);
            };
        }
    }
}
