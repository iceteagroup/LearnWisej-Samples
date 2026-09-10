// HybridOfflinePatterns.cs — the contracts the walkthrough video writes on screen.
//
// This file is the whole "offline vocabulary" of the module: a state for every offline action, a durable
// command that carries that state, the device abstraction screens call instead of platform APIs, and the
// queue contract. Everything else in Hybrid/ is an implementation of one of these.
//
// The four members of OfflineCommand and the three interfaces below are reproduced exactly as the lesson
// resource shows them; the extra init-only properties at the bottom of the record are lab additions the
// screen needs to render the queue (they travel with the record through `with` expressions).

#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseOps.Hybrid
{
    /// <summary>
    /// The explicit state every offline action carries. There is no "probably saved": a command is
    /// LocalOnly (written on the device, not yet queued), PendingSync (queued, waiting for a connection),
    /// Synced (accepted by the server), Conflict (the server row moved while we were offline — a person
    /// decides) or Rejected (the server refused it: permission revoked, or the technician kept the server
    /// version). Nothing is ever silently dropped.
    /// </summary>
    public enum SyncState { LocalOnly, PendingSync, Synced, Conflict, Rejected }

    /// <summary>
    /// One durable local change. It is a value, not an entity: the payload is serialized at the moment the
    /// technician acted, together with the device clock reading, so replaying it later is deterministic and
    /// auditable. On a real device this row lives in the local SQLite file (see <see cref="LocalStore"/>).
    /// </summary>
    public sealed record OfflineCommand(
        Guid LocalId,
        string CommandName,
        string EntityId,
        string PayloadJson,
        DateTimeOffset CreatedAt,
        SyncState State)
    {
        // ── lab additions (init-only, so `with { State = … }` keeps them) ──────────────────────────────

        /// <summary>One line for the queue card: "Completed 11:04 — seal replaced".</summary>
        public string? Summary { get; init; }

        /// <summary>Why the command is in its current state — the server's message after a replay attempt.</summary>
        public string? SyncMessage { get; init; }

        /// <summary>The row version the device held when the technician acted: the optimistic-concurrency check.</summary>
        public int BaseVersion { get; init; }

        /// <summary>Set when the command reached the server, whatever the outcome.</summary>
        public DateTimeOffset? SyncedAt { get; init; }

        /// <summary>The correlation id of the server call that replayed it — the link into the audit log.</summary>
        public string? CorrelationId { get; init; }

        public bool IsOpen => State == SyncState.LocalOnly || State == SyncState.PendingSync;
    }

    /// <summary>
    /// Everything the screens are allowed to know about the device. A screen asks "am I online?" or
    /// "scan something for me" — never "is this Android?". One implementation per application shape
    /// (browser, Hybrid shell, kiosk, test double) and the screens never change.
    /// </summary>
    public interface IDeviceServices
    {
        Task<bool> IsOnlineAsync();

        Task<string?> ScanDocumentAsync();

        Task VibrateAsync();
    }

    /// <summary>
    /// The synchronization boundary's contract. Three verbs are enough: put a change in, read what is still
    /// waiting, record what the server decided. The queue never talks to the database and never decides an
    /// outcome — it only remembers.
    /// </summary>
    public interface IOfflineCommandQueue
    {
        Task EnqueueAsync(OfflineCommand command);

        Task<IReadOnlyList<OfflineCommand>> GetPendingAsync();

        Task MarkAsync(Guid localId, SyncState state, string? message = null);
    }
}
