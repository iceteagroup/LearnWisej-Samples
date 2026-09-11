using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;

namespace EnterpriseOps.Hybrid
{
    /// <summary>
    /// The device's copy of one work order. Deliberately NOT <see cref="WorkOrder"/>: the device holds a
    /// projection of the fields the field workflow needs, plus the version it was cached at, so the sync
    /// boundary can detect that the server moved on. Properties (not fields) because the grid binds to them.
    /// </summary>
    public class CachedWorkOrder
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Customer { get; set; }
        public string Site { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }

        /// <summary>The server version this copy was taken from — travels into every offline command.</summary>
        public int Version { get; set; }

        public string Due { get; set; }

        /// <summary>What the device knows locally that the server does not yet: "Completed · pending sync".</summary>
        public string LocalState { get; set; }

        public DateTime CachedUtc { get; set; }

        public static CachedWorkOrder From(WorkOrder row) => new CachedWorkOrder
        {
            Id = row.Id,
            Code = row.Code,
            Title = row.Title,
            Customer = row.Customer,
            Site = row.Site,
            Status = row.Status.ToString(),
            Priority = row.Priority.ToString(),
            Version = row.Version,
            Due = row.DueUtc.HasValue ? row.DueUtc.Value.ToLocalTime().ToString("ddd HH:mm") : "—",
            LocalState = "",
            CachedUtc = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// The device's local database — in the sample an in-memory stand-in for the SQLite file a real Hybrid
    /// build would open (the production file is <c>LocalStore.Sqlite.cs</c>; the interface it exposes is
    /// this one, so nothing above it changes). It holds three things and nothing else:
    ///
    ///   1. the technician's own open work orders (the cache scope — never the whole table),
    ///   2. the offline command queue,
    ///   3. the permission snapshot issued at the last connection.
    ///
    /// Two production behaviours it reproduces deliberately: the store reports that it is encrypted at rest
    /// (a real build would open the SQLite file with a device-bound key), and <see cref="Wipe"/> throws
    /// everything away on logout or after a lock-out period, so a stolen device holds nothing replayable
    /// (the prototype has no logout, so nothing calls it yet).
    /// </summary>
    public class LocalStore
    {
        private readonly IActivityTrace _trace;
        private readonly object _gate = new object();
        private readonly List<CachedWorkOrder> _cache = new List<CachedWorkOrder>();
        private readonly List<OfflineCommand> _commands = new List<OfflineCommand>();

        public LocalStore(IActivityTrace trace)
        {
            _trace = trace;
        }

        /// <summary>Simulated: the SQLite file is opened with a device-bound key. See docs/HybridOfflineArchitectureNote.md.</summary>
        public bool EncryptedAtRest => true;

        /// <summary>When the cache was last downloaded — shown on the screen so stale data is visible.</summary>
        public DateTime? CachedAtUtc { get; private set; }

        /// <summary>The permission set the device is carrying. Replaced on every reconnect, never merged.</summary>
        public CachedPermissionSet Permissions { get; private set; }

        public IReadOnlyList<CachedWorkOrder> Cache
        {
            get { lock (_gate) return _cache.ToArray(); }
        }

        public IReadOnlyList<OfflineCommand> Commands
        {
            get { lock (_gate) return _commands.ToArray(); }
        }

        public int PendingCount
        {
            get { lock (_gate) return _commands.Count(c => c.IsOpen); }
        }

        /// <summary>Replaces the whole cache — the only way rows enter the device.</summary>
        public void ReplaceCache(IEnumerable<CachedWorkOrder> rows)
        {
            lock (_gate)
            {
                _cache.Clear();
                _cache.AddRange(rows);
                CachedAtUtc = DateTime.UtcNow;
            }
            _trace.Log(TraceLayer.Device, $"local store: cached {_cache.Count} work orders (encrypted at rest: {EncryptedAtRest}) ≈ {ApproximateSizeBytes()} bytes");
        }

        public void StorePermissions(CachedPermissionSet permissions)
        {
            lock (_gate) Permissions = permissions;
            _trace.Log(TraceLayer.Device, $"local store: permission snapshot replaced → {permissions}");
        }

        public CachedWorkOrder Find(int id)
        {
            lock (_gate) return _cache.FirstOrDefault(w => w.Id == id);
        }

        public void Append(OfflineCommand command)
        {
            lock (_gate) _commands.Add(command);
        }

        /// <summary>Replaces a queued command in place (state transitions go through here).</summary>
        public OfflineCommand Replace(Guid localId, Func<OfflineCommand, OfflineCommand> change)
        {
            lock (_gate)
            {
                int i = _commands.FindIndex(c => c.LocalId == localId);
                if (i < 0) return null;
                return _commands[i] = change(_commands[i]);
            }
        }

        public OfflineCommand FindCommand(Guid localId)
        {
            lock (_gate) return _commands.FirstOrDefault(c => c.LocalId == localId);
        }

        /// <summary>
        /// Logout / lock-out: nothing of the tenant's data survives on the device. A device whose local
        /// state is doubted is wiped and re-downloaded, never repaired.
        /// </summary>
        public void Wipe()
        {
            int rows, commands;
            lock (_gate)
            {
                rows = _cache.Count;
                commands = _commands.Count;
                _cache.Clear();
                _commands.Clear();
                Permissions = null;
                CachedAtUtc = null;
            }
            _trace.Log(TraceLayer.Device, $"local store WIPED: {rows} cached work orders and {commands} commands destroyed, permission snapshot discarded");
        }

        /// <summary>A rough "how much of the tenant's data is on this device" number for the server log.</summary>
        public int ApproximateSizeBytes()
        {
            lock (_gate)
                return _cache.Sum(w => 120 + (w.Title?.Length ?? 0) + (w.Site?.Length ?? 0))
                     + _commands.Sum(c => 160 + (c.PayloadJson?.Length ?? 0));
        }
    }
}
