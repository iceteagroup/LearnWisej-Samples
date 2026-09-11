using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Security;
using EnterpriseOps.Services;

namespace EnterpriseOps.Hybrid
{
    /// <summary>One conflicting command, with both versions, ready for the screen to show side by side.</summary>
    public class SyncConflict
    {
        public Guid LocalId;
        public int WorkOrderId;
        public string Code;

        /// <summary>The device clock reading when the technician completed the work.</summary>
        public DateTimeOffset LocalTime;

        public string LocalNotes;
        public int LocalBaseVersion;

        /// <summary>The row as the server has it now.</summary>
        public WorkOrderSnapshot Server;

        public string LocalHeader => $"Your local change · {LocalTime.ToLocalTime():HH:mm} offline";
        public string LocalBody => $"Completed — “{LocalNotes}”";
        public string ServerHeader => $"Server version · {Server.LastChangedUtc.ToLocalTime():HH:mm} by {Server.LastChangedBy}";
        public string ServerBody => $"{Server.Status} — “{Server.Notes}”";
    }

    /// <summary>A single step of the replay, pushed to the screen from the background thread.</summary>
    public class SyncProgress
    {
        public int Index;
        public int Total;
        public string Message;
        public OfflineCommand Command;
        public SyncConflict Conflict;
    }

    /// <summary>The outcome of one reconnect.</summary>
    public class SyncReport
    {
        public int Total;
        public int Synced;
        public int Rejected;
        public int Remaining;
        public SyncConflict Conflict;
        public string Message;

        public bool StoppedOnConflict => Conflict != null;
    }

    /// <summary>
    /// The synchronization boundary — the only place where device state becomes server state.
    ///
    /// Two rules make offline work safe, and both live here:
    ///
    ///   1. Permissions are refreshed BEFORE the first command replays. The device's cached permission set
    ///      is yesterday's answer; a revoked technician's queued commands must be rejected, not applied.
    ///   2. Every command is replayed through <see cref="WorkOrderService"/> — the same service the online
    ///      screen calls — so the permission check, the optimistic-concurrency check and the audit entry are
    ///      literally the same code. The device never reaches the repository.
    ///
    /// A conflict stops the replay and hands the decision to a person: the commands behind it stay
    /// PendingSync, so ordering is preserved and nothing is applied out of sequence.
    ///
    /// <see cref="Replay"/> is synchronous on purpose: the page calls it inside
    /// <c>Application.StartTask</c> and reports each step through the <c>onProgress</c> callback, which
    /// pushes UI changes with <c>Application.Update</c>.
    /// </summary>
    public class SyncWorkflow
    {
        private readonly LocalOfflineCommandQueue _queue;
        private readonly LocalStore _store;
        private readonly WorkOrderService _service;
        private readonly PermissionService _permissions;
        private readonly SessionContext _session;
        private readonly IActivityTrace _trace;

        public SyncWorkflow(LocalOfflineCommandQueue queue, LocalStore store, WorkOrderService service,
                            PermissionService permissions, SessionContext session, IActivityTrace trace)
        {
            _queue = queue;
            _store = store;
            _service = service;
            _permissions = permissions;
            _session = session;
            _trace = trace;
        }

        /// <summary>Pause between commands, so the progress push stays well above the 250 ms floor and
        /// each state change is visible.</summary>
        public int StepDelayMs { get; set; } = 700;

        /// <summary>Step 1 of every reconnect, also used when the device is re-provisioned after a wipe.</summary>
        public CachedPermissionSet RefreshPermissions()
        {
            var fresh = _permissions.Issue(_session.User);
            _store.StorePermissions(fresh);
            return fresh;
        }

        /// <summary>
        /// Replays the queue against the server, oldest command first. Runs on a background thread.
        /// </summary>
        public SyncReport Replay(Action<SyncProgress> onProgress)
        {
            var report = new SyncReport();

            // ── 1. permissions, before anything replays ───────────────────────────────────────────────
            var before = _store.Permissions;
            var fresh = RefreshPermissions();
            _trace.Log(TraceLayer.Job, $"reconnect 1/2 — permission snapshot refreshed BEFORE replay: {fresh}");
            if (before != null && before.Permissions.Count != fresh.Permissions.Count)
                _trace.Log(TraceLayer.Security, $"cached permissions were stale ({before.Permissions.Count} → {fresh.Permissions.Count} grants)");

            var pending = _queue.GetPendingAsync().GetAwaiter().GetResult();
            report.Total = pending.Count;
            report.Remaining = pending.Count;

            onProgress(new SyncProgress
            {
                Index = 0,
                Total = report.Total,
                Message = report.Total == 0
                    ? "Connection restored — permissions refreshed · queue empty"
                    : $"Connection restored — permissions refreshed · replaying {report.Total} command(s)",
            });

            if (report.Total == 0)
            {
                report.Message = "Queue empty — nothing to sync.";
                _trace.Log(TraceLayer.Job, "reconnect 2/2 — nothing queued");
                return report;
            }

            // ── 2. replay, in the order the technician acted ──────────────────────────────────────────
            for (int i = 0; i < pending.Count; i++)
            {
                var command = pending[i];
                if (StepDelayMs > 0)
                    Thread.Sleep(StepDelayMs);

                var payload = _queue.ReadCompletion(command);
                var ctx = _session.NewCommand();
                _queue.LinkToAudit(command.LocalId, ctx.CorrelationId);

                _trace.Log(TraceLayer.Job,
                    $"replay {i + 1}/{pending.Count}: {command.EntityId} → WorkOrderService.Complete (corr {ctx.CorrelationId})");

                CommandResult result = _service.Complete(payload, ctx, command.CreatedAt);

                if (result.Succeeded)
                {
                    _queue.MarkAsync(command.LocalId, SyncState.Synced, result.Message).GetAwaiter().GetResult();
                    ApplyToCache(payload.WorkOrderId, "Completed", result.NewVersion, "");
                    report.Synced++;
                    report.Remaining--;
                    onProgress(new SyncProgress
                    {
                        Index = i + 1,
                        Total = pending.Count,
                        Command = _queue.Find(command.LocalId),
                        Message = $"{command.EntityId} synced (v{result.NewVersion})",
                    });
                    continue;
                }

                if (result.IsConflict)
                {
                    var conflict = new SyncConflict
                    {
                        LocalId = command.LocalId,
                        WorkOrderId = payload.WorkOrderId,
                        Code = command.EntityId,
                        LocalTime = command.CreatedAt,
                        LocalNotes = payload.Notes,
                        LocalBaseVersion = payload.ExpectedVersion,
                        Server = result.ServerVersion,
                    };

                    _queue.MarkAsync(command.LocalId, SyncState.Conflict, result.Message).GetAwaiter().GetResult();
                    ApplyToCache(payload.WorkOrderId, result.ServerVersion.Status.ToString(), result.ServerVersion.Version, "conflict — waiting for you");

                    report.Conflict = conflict;
                    report.Message = $"Sync conflict on {command.EntityId} — the server changed it while you were offline.";
                    _trace.Log(TraceLayer.Job,
                        $"replay stopped at {command.EntityId}: {pending.Count - i - 1} command(s) stay PendingSync");

                    onProgress(new SyncProgress
                    {
                        Index = i + 1,
                        Total = pending.Count,
                        Command = _queue.Find(command.LocalId),
                        Conflict = conflict,
                        Message = report.Message,
                    });
                    return report;
                }

                // Permission denied (revoked while offline) or a plain failure: recorded, never applied.
                _queue.MarkAsync(command.LocalId, SyncState.Rejected, result.Message).GetAwaiter().GetResult();
                ApplyToCache(payload.WorkOrderId, null, 0, "rejected by the server");
                report.Rejected++;
                report.Remaining--;
                _trace.Log(TraceLayer.Job, $"{command.EntityId} rejected — {result.Message}");

                onProgress(new SyncProgress
                {
                    Index = i + 1,
                    Total = pending.Count,
                    Command = _queue.Find(command.LocalId),
                    Message = $"{command.EntityId} rejected — {result.Message}",
                });
            }

            report.Message = $"Queue drained · {report.Synced} synced · {report.Rejected} rejected · audit logged";
            _trace.Log(TraceLayer.Job, $"reconnect 2/2 — {report.Message}");
            return report;
        }

        /// <summary>
        /// Conflict resolution A — "Keep server, attach my notes". The server row is left exactly as the
        /// dispatcher left it; the technician's field notes are appended so no information is lost, and the
        /// command ends as Rejected with a reason. Any technician may do this: it changes no status.
        /// </summary>
        public async Task<CommandResult> KeepServerAsync(SyncConflict conflict)
        {
            var ctx = _session.NewCommand();
            _trace.Log(TraceLayer.Service, $"resolution: keep server for {conflict.Code} — status stays {conflict.Server.Status}, notes attached (corr {ctx.CorrelationId})");

            var result = await _service.AttachNotesAsync(conflict.WorkOrderId, conflict.LocalNotes, ctx, conflict.LocalTime);
            await _queue.MarkAsync(conflict.LocalId, SyncState.Rejected, "kept server — notes preserved");
            ApplyToCache(conflict.WorkOrderId, conflict.Server.Status.ToString(), result.NewVersion, "resolved — notes preserved");
            return result;
        }

        /// <summary>
        /// Conflict resolution B — "Apply my completion". A product decision, not a technical one: writing a
        /// completion over the dispatcher's change needs <c>workorder.override</c>, which a Technician does
        /// not have. The attempt is made against the server anyway so the refusal is audited and the
        /// technician sees the real rule instead of a greyed-out button.
        /// </summary>
        public async Task<CommandResult> ApplyMineAsync(SyncConflict conflict)
        {
            var command = _queue.Find(conflict.LocalId);
            var payload = _queue.ReadCompletion(command);
            payload.OverrideServerChange = true;
            payload.ExpectedVersion = conflict.Server.Version;   // the technician has now seen the server version

            var ctx = _session.NewCommand();
            _trace.Log(TraceLayer.Service, $"resolution: apply local completion over the server change for {conflict.Code} — requires {Permissions.OverrideConflict} (corr {ctx.CorrelationId})");

            var result = await _service.CompleteAsync(payload, ctx, command.CreatedAt);
            if (result.Succeeded)
            {
                await _queue.MarkAsync(conflict.LocalId, SyncState.Synced, "override applied");
                ApplyToCache(conflict.WorkOrderId, "Completed", result.NewVersion, "");
            }
            else
            {
                await _queue.MarkAsync(conflict.LocalId, SyncState.Conflict, result.Message);
            }
            return result;
        }

        /// <summary>Keeps the device's copy honest after the server answered. The cache is a projection, never a source.</summary>
        private void ApplyToCache(int workOrderId, string status, int version, string localState)
        {
            var cached = _store.Find(workOrderId);
            if (cached == null) return;

            if (status != null) cached.Status = status;
            if (version > 0) cached.Version = version;
            cached.LocalState = localState;
        }
    }
}
