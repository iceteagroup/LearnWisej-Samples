using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EnterpriseOps.Services;

namespace EnterpriseOps.Hybrid
{
    /// <summary>
    /// The device-side queue: <see cref="IOfflineCommandQueue"/> backed by the local store.
    ///
    /// It is deliberately dumb. It appends, it reads what is still open, it records the outcome the sync
    /// boundary reports. It never decides whether a command is allowed, never touches a work order, never
    /// re-orders or coalesces (two completions of the same order replay in the order the technician made
    /// them, and the second one becomes a conflict — that is information, not noise).
    ///
    /// Durability is the store's job; on a device the store is the SQLite file, so a command survives the
    /// application being killed mid-shift.
    /// </summary>
    public class LocalOfflineCommandQueue : IOfflineCommandQueue
    {
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions { WriteIndented = false };

        private readonly LocalStore _store;
        private readonly IActivityTrace _trace;

        public LocalOfflineCommandQueue(LocalStore store, IActivityTrace trace)
        {
            _store = store;
            _trace = trace;
        }

        public int PendingCount => _store.PendingCount;

        /// <summary>Everything the device holds, newest last — what the queue card renders.</summary>
        public IReadOnlyList<OfflineCommand> All() => _store.Commands;

        public OfflineCommand Find(Guid localId) => _store.FindCommand(localId);

        /// <summary>Builds the durable command for a completion the technician made offline.</summary>
        public OfflineCommand CreateCompletion(CompleteWorkOrderCommand payload, string code, string summary)
            => new OfflineCommand(
                    LocalId: Guid.NewGuid(),
                    CommandName: "CompleteWorkOrder",
                    EntityId: code,
                    PayloadJson: JsonSerializer.Serialize(payload, Json),
                    CreatedAt: payload.CompletedAt,
                    State: SyncState.PendingSync)
            {
                Summary = summary,
                BaseVersion = payload.ExpectedVersion,
            };

        /// <summary>Rebuilds the typed command the server service accepts. The payload is the contract.</summary>
        public CompleteWorkOrderCommand ReadCompletion(OfflineCommand command)
            => JsonSerializer.Deserialize<CompleteWorkOrderCommand>(command.PayloadJson, Json);

        // ── IOfflineCommandQueue ──────────────────────────────────────────────────────────────────────

        public Task EnqueueAsync(OfflineCommand command)
        {
            _store.Append(command);
            _trace.Log(TraceLayer.Device,
                $"queue.Enqueue {command.CommandName} {command.EntityId} (base v{command.BaseVersion}) → {command.State} · {_store.PendingCount} pending · payload {command.PayloadJson.Length} bytes");
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<OfflineCommand>> GetPendingAsync()
        {
            IReadOnlyList<OfflineCommand> pending = _store.Commands
                .Where(c => c.IsOpen)
                .OrderBy(c => c.CreatedAt)
                .ToArray();

            _trace.Log(TraceLayer.Device, $"queue.GetPending → {pending.Count} command(s), oldest first");
            return Task.FromResult(pending);
        }

        public Task MarkAsync(Guid localId, SyncState state, string message = null)
        {
            var updated = _store.Replace(localId, c => c with
            {
                State = state,
                SyncMessage = message,
                SyncedAt = DateTimeOffset.Now,
            });

            if (updated == null)
                _trace.Log(TraceLayer.Device, $"queue.Mark {localId:N} → command not found (already wiped?)");
            else
                _trace.Log(TraceLayer.Device, $"queue.Mark {updated.EntityId} → {state}{(string.IsNullOrEmpty(message) ? "" : " · " + message)}");

            return Task.CompletedTask;
        }

        /// <summary>Sets the correlation id of the server call that replayed the command (the audit-log link).</summary>
        public void LinkToAudit(Guid localId, string correlationId)
            => _store.Replace(localId, c => c with { CorrelationId = correlationId });
    }
}
