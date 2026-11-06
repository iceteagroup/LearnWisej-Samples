using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FieldOps.Model;
using Wisej.Hybrid;

namespace FieldOps.Services
{
    /// <summary>
    /// The sync design the decision record made ours, kept as small as it can honestly be:
    ///   * down: while FieldOps runs Remote inside the Hybrid client, today's orders are written
    ///     ONTO THE DEVICE (Device.FileSystem, the app's data directory) after every load;
    ///   * offline: the Local app (FieldOps.Offline) reads that snapshot and records every status
    ///     change it makes as a pending change, also on the device;
    ///   * up: at the next Remote sign-in the pending changes are replayed through
    ///     WorkOrderService.UpdateStatus with the version they were based on. The server wins a
    ///     conflict, and the technician is told.
    /// Every path is built from Device.FileSystem.AppDataDirectory, so the same code works for
    /// the Remote session (the bytes travel over the bridge) and the Local app (on the device).
    /// </summary>
    public static class DeviceSync
    {
        public sealed class Snapshot
        {
            public string TechnicianId { get; set; }
            public DateTime SyncedAt { get; set; }
            public List<WorkOrder> Orders { get; set; }
        }

        public sealed class PendingChange
        {
            public string Number { get; set; }
            public WorkOrderStatus Status { get; set; }
            public int BaseVersion { get; set; }
            public DateTime ChangedAt { get; set; }
        }

        private static string Folder => Join(Device.FileSystem.AppDataDirectory, "FieldOps");
        private static string SnapshotPath => Join(Folder, "orders.json");
        private static string PendingPath => Join(Folder, "pending.json");

        /// <summary>
        /// Device paths are built with the DEVICE's separator, not the server's: a Linux server
        /// talking to a Windows laptop must not produce "C:\Users\...\Data/FieldOps".
        /// </summary>
        private static string Join(string folder, string name)
        {
            char sep = folder.Contains('\\') ? '\\' : '/';
            return folder.TrimEnd('\\', '/') + sep + name;
        }

        public static void SaveSnapshot(Technician tech, IEnumerable<WorkOrder> orders)
        {
            var snapshot = new Snapshot { TechnicianId = tech.Id, SyncedAt = DateTime.Now, Orders = orders.ToList() };
            Device.FileSystem.CreateDirectory(Folder);
            Device.FileSystem.WriteText(SnapshotPath, JsonSerializer.Serialize(snapshot));
        }

        public static Snapshot LoadSnapshot()
        {
            if (!Device.FileSystem.Exists(SnapshotPath)) return null;
            return JsonSerializer.Deserialize<Snapshot>(Device.FileSystem.ReadText(SnapshotPath));
        }

        public static List<PendingChange> LoadPending()
        {
            if (!Device.FileSystem.Exists(PendingPath)) return new List<PendingChange>();
            return JsonSerializer.Deserialize<List<PendingChange>>(Device.FileSystem.ReadText(PendingPath)) ?? new List<PendingChange>();
        }

        public static void RecordPending(WorkOrder before, WorkOrderStatus status)
        {
            var pending = LoadPending();
            pending.Add(new PendingChange { Number = before.Number, Status = status, BaseVersion = before.Version, ChangedAt = DateTime.Now });
            Device.FileSystem.CreateDirectory(Folder);
            Device.FileSystem.WriteText(PendingPath, JsonSerializer.Serialize(pending));
        }

        /// <summary>
        /// Replays offline changes. Returns a sentence for the screen, or null when there was
        /// nothing to replay. Pending changes are removed once the server has decided each one.
        /// </summary>
        public static string ReplayPending(WorkOrderService service, Technician tech)
        {
            var pending = LoadPending();
            if (pending.Count == 0) return null;

            int applied = 0, refused = 0;
            // Each change was based on the version the offline app had; a chain of changes to the
            // same order (Accepted, then On site) advances that version by one per step.
            var versions = new Dictionary<string, int>();
            foreach (var change in pending)
            {
                int baseVersion = versions.TryGetValue(change.Number, out var v) ? v : change.BaseVersion;
                try
                {
                    var saved = service.UpdateStatus(change.Number, change.Status, baseVersion, tech.Id);
                    versions[change.Number] = saved.Version;
                    applied++;
                    OpsLog.Info($"sync: {change.Number} -> {WorkOrder.StatusText(change.Status)} (made offline {change.ChangedAt:HH:mm}) applied");
                }
                catch (Exception ex) when (ex is WorkOrderConflictException || ex is StatusTransitionException)
                {
                    refused++;
                    OpsLog.Error($"sync: {change.Number} -> {change.Status} (made offline {change.ChangedAt:HH:mm}) refused, server wins", ex);
                }
            }
            Device.FileSystem.DeleteFile(PendingPath);
            return refused == 0
                ? $"Synced {applied} change{(applied == 1 ? "" : "s")} made offline."
                : $"Synced {applied} offline change{(applied == 1 ? "" : "s")}; {refused} conflicted with newer changes on the server and were not applied.";
        }
    }
}
