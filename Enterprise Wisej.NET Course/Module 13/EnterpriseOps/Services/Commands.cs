using System;
using System.Collections.Generic;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// "Complete this work order" — the typed command the server service accepts. Online, the screen sends it
    /// directly; offline, its payload is serialized into an <see cref="Hybrid.OfflineCommand"/> and this same
    /// object is rebuilt on replay.
    /// </summary>
    public class CompleteWorkOrderCommand
    {
        public int WorkOrderId { get; set; }

        /// <summary>The version the technician saw when they completed the work — the concurrency check.</summary>
        public int ExpectedVersion { get; set; }

        public string Notes { get; set; }

        /// <summary>When the technician completed it, on the device clock. Audited separately from sync time.</summary>
        public DateTimeOffset CompletedAt { get; set; }

        /// <summary>Set by the conflict resolution "Apply my completion" — requires workorder.override on the server.</summary>
        public bool OverrideServerChange { get; set; }
    }

    /// <summary>A read-only view of the server row, returned inside a conflict so the screen can show "their version".</summary>
    public class WorkOrderSnapshot
    {
        public int Id;
        public WorkOrderStatus Status;
        public int Version;
        public string Notes;
        public string LastChangedBy;
        public DateTime LastChangedUtc;

        public static WorkOrderSnapshot From(WorkOrder row) => new WorkOrderSnapshot
        {
            Id = row.Id,
            Status = row.Status,
            Version = row.Version,
            Notes = row.Notes,
            LastChangedBy = row.LastChangedBy,
            LastChangedUtc = row.LastChangedUtc,
        };

        public override string ToString()
            => $"{Status} v{Version} by {LastChangedBy} {LastChangedUtc.ToLocalTime():HH:mm}" + (string.IsNullOrEmpty(Notes) ? "" : $" — \"{Notes}\"");
    }

    /// <summary>What every service call returns to the UI — never a raw entity, never an exception for a business outcome.</summary>
    public class CommandResult
    {
        public bool Succeeded { get; private set; }
        public bool IsConflict { get; private set; }
        public bool IsPermissionDenied { get; private set; }
        public List<string> Errors { get; } = new List<string>();
        public string CorrelationId { get; private set; }
        public string Message { get; private set; }
        public int NewVersion { get; private set; }

        /// <summary>Filled on a conflict: the row as the server has it now.</summary>
        public WorkOrderSnapshot ServerVersion { get; private set; }

        public static CommandResult Ok(string correlationId, int newVersion, string message)
            => new CommandResult { Succeeded = true, CorrelationId = correlationId, NewVersion = newVersion, Message = message };

        public static CommandResult Failed(string correlationId, string error)
        {
            var r = new CommandResult { Succeeded = false, CorrelationId = correlationId, Message = error };
            r.Errors.Add(error);
            return r;
        }

        public static CommandResult Denied(string correlationId, string error)
        {
            var r = Failed(correlationId, error);
            r.IsPermissionDenied = true;
            return r;
        }

        public static CommandResult Conflict(string correlationId, WorkOrderSnapshot serverVersion, string error)
        {
            var r = Failed(correlationId, error);
            r.IsConflict = true;
            r.ServerVersion = serverVersion;
            return r;
        }
    }
}
