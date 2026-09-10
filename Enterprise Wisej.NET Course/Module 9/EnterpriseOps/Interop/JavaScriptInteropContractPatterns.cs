using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EnterpriseOps.Security;

namespace EnterpriseOps.Interop
{
    // ==========================================================================================
    // JavaScriptInteropContractPatterns.cs — the file the walkthrough types on screen.
    //
    // ONE boundary, ONE contract. Everything JavaScript may send the server is described here:
    // the request shape, the result shape, the named failure codes, the validation rules and the
    // catalogue of commands that exist at all. The palette script cannot call "arbitrary server
    // code": it can call one remote method, with these three fields, and nothing else crosses.
    //
    // The rule the module is about: the payload is a SUGGESTION. Authority (who you are, which
    // tenant you are in, what you may do) is read on the server, from the session, every time.
    // ==========================================================================================

    /// <summary>
    /// The wire request. Three named fields, each with a type, a rule and a default —
    /// small enough to validate in a few lines and to version when a field is added.
    /// </summary>
    /// <remarks>
    /// The walkthrough declares <c>string? EntityId</c>; this project builds with
    /// &lt;Nullable&gt;disable&lt;/Nullable&gt;, so the optional field is a plain string that may be
    /// empty. Empty — never <c>null</c>: a null argument to a Wisej WebMethod is rejected by the
    /// client wrapper before the call leaves the browser.
    /// </remarks>
    public sealed record ClientCommandRequest(
        string CommandName,
        string EntityId,
        string CorrelationId);

    /// <summary>
    /// The wire result. A success flag, a NAMED code from <see cref="Services.ResultCodes"/>, a
    /// message the user may read, and the correlation id that ties the browser line, the trace
    /// line and the audit row together. No exception text, no stack trace, no internal ids.
    /// </summary>
    public sealed class ClientCommandResult
    {
        public bool Succeeded { get; set; }
        public string Code { get; set; } = Services.ResultCodes.Ok;
        public string CommandName { get; set; } = "";
        public string Message { get; set; } = "";
        public string CorrelationId { get; set; } = "";

        /// <summary>Contract version — the browser logs it, so a mismatch is visible in support tickets.</summary>
        public string ContractVersion { get; set; } = InteropContract.Version;

        public static ClientCommandResult Ok(string commandName, string correlationId, string message)
            => new ClientCommandResult
            {
                Succeeded = true,
                Code = Services.ResultCodes.Ok,
                CommandName = commandName ?? "",
                CorrelationId = correlationId ?? "",
                Message = message ?? "",
            };

        public static ClientCommandResult Fail(string commandName, string correlationId, string code, string message)
            => new ClientCommandResult
            {
                Succeeded = false,
                Code = code,
                CommandName = commandName ?? "",
                CorrelationId = correlationId ?? "",
                Message = message ?? "",
            };
    }

    /// <summary>
    /// One catalogue entry: what the palette may offer, and the single permission the server
    /// checks before it runs. The shortcut is browser-side sugar; the permission is the contract.
    /// </summary>
    public sealed class CommandDescriptor
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Shortcut { get; set; }
        public Permission Permission { get; set; }
        public bool RequiresEntity { get; set; }
        public string Summary { get; set; }
    }

    /// <summary>
    /// The published contract: version, field rules, the command catalogue, and the payload
    /// validator. Everything here is data — docs/InteropContract.md is the same table in prose,
    /// and the two are meant to be reviewed together.
    /// </summary>
    public static class InteropContract
    {
        public const string Version = "1.0";

        /// <summary>Names look like <c>workorder.approve</c>: lower-case segments, at least two.</summary>
        public static readonly Regex CommandNamePattern = new Regex(@"^[a-z][a-z0-9]{1,15}(\.[a-z][a-z0-9]{1,15}){1,2}$", RegexOptions.Compiled);

        /// <summary>Entity ids cross the wire as <c>WO-1040</c>, never as a raw database key.</summary>
        public static readonly Regex EntityIdPattern = new Regex(@"^WO-[0-9]{4}$", RegexOptions.Compiled);

        /// <summary>Correlation ids are 8 lower-case hex characters (see SessionContext.NewCorrelationId).</summary>
        public static readonly Regex CorrelationIdPattern = new Regex(@"^[0-9a-f]{8}$", RegexOptions.Compiled);

        public const int MaxCommandNameLength = 40;
        public const int MaxEntityIdLength = 12;
        public const int MaxCorrelationIdLength = 8;

        /// <summary>
        /// The whole catalogue. A command that is not in this list does not exist: the server
        /// answers UNKNOWN_COMMAND without touching a service, whatever the browser claims.
        /// </summary>
        public static readonly IReadOnlyList<CommandDescriptor> Catalog = new List<CommandDescriptor>
        {
            new CommandDescriptor { Id = "workorder.approve",  Title = "Approve work order…",  Shortcut = "Ctrl+Shift+A", Permission = Permission.WorkOrderApprove,  RequiresEntity = true,  Summary = "Move a New or Assigned work order to InProgress." },
            new CommandDescriptor { Id = "workorder.reassign", Title = "Reassign selected…",   Shortcut = "Ctrl+Shift+R", Permission = Permission.WorkOrderReassign, RequiresEntity = true,  Summary = "Hand the work order to another technician." },
            new CommandDescriptor { Id = "workorder.escalate", Title = "Escalate selected…",   Shortcut = "Ctrl+Shift+E", Permission = Permission.WorkOrderEscalate, RequiresEntity = true,  Summary = "Raise the work order to the escalation queue." },
            new CommandDescriptor { Id = "workqueue.open",     Title = "Open work queue",      Shortcut = "Ctrl+1",       Permission = Permission.WorkQueueOpen,     RequiresEntity = false, Summary = "Open the tenant work queue." },
            new CommandDescriptor { Id = "import.new",         Title = "New import…",          Shortcut = "Ctrl+I",       Permission = Permission.ImportCreate,      RequiresEntity = false, Summary = "Start a work-order import batch." },
            new CommandDescriptor { Id = "diagnostics.open",   Title = "Open diagnostics",     Shortcut = "Ctrl+D",       Permission = Permission.DiagnosticsView,   RequiresEntity = false, Summary = "Open the diagnostics screen." },
        };

        public static CommandDescriptor Find(string commandId)
            => Catalog.FirstOrDefault(c => string.Equals(c.Id, commandId, StringComparison.Ordinal));

        /// <summary>
        /// Rule 1 of the contract: validate the SHAPE before anything else runs. No service, no
        /// repository, no permission lookup happens until the three fields are known-good — an
        /// oversized or malformed payload must cost the server a regex, not a database round trip.
        /// Returns false and a named code; never throws, never echoes the raw value back.
        /// </summary>
        public static bool TryParse(string commandName, string entityId, string correlationId,
                                    out ClientCommandRequest request, out string code, out string error)
        {
            request = null;
            code = Services.ResultCodes.MalformedPayload;

            commandName = (commandName ?? "").Trim();
            entityId = (entityId ?? "").Trim();
            correlationId = (correlationId ?? "").Trim().ToLowerInvariant();

            if (commandName.Length == 0)
            {
                error = "commandName is required.";
                return false;
            }
            if (commandName.Length > MaxCommandNameLength)
            {
                error = $"commandName is longer than {MaxCommandNameLength} characters ({commandName.Length}).";
                return false;
            }
            if (!CommandNamePattern.IsMatch(commandName))
            {
                error = "commandName must look like 'area.verb' (lower-case letters, digits and dots).";
                return false;
            }
            if (entityId.Length > MaxEntityIdLength)
            {
                error = $"entityId is longer than {MaxEntityIdLength} characters ({entityId.Length}).";
                return false;
            }
            if (entityId.Length > 0 && !EntityIdPattern.IsMatch(entityId))
            {
                error = "entityId must look like 'WO-1040'.";
                return false;
            }
            if (correlationId.Length == 0)
            {
                error = "correlationId is required.";
                return false;
            }
            if (correlationId.Length > MaxCorrelationIdLength || !CorrelationIdPattern.IsMatch(correlationId))
            {
                error = "correlationId must be 8 hexadecimal characters.";
                return false;
            }

            request = new ClientCommandRequest(commandName, entityId, correlationId);
            code = Services.ResultCodes.Ok;
            error = null;
            return true;
        }

        /// <summary>Turns the wire id <c>WO-1040</c> into the internal key. Shape is already validated.</summary>
        public static bool TryReadEntityKey(string entityId, out int key)
            => int.TryParse((entityId ?? "").Length > 3 ? entityId.Substring(3) : "", out key);

        /// <summary>The wire form of an internal key — the browser never sees a raw database id.</summary>
        public static string ToEntityId(int key) => "WO-" + key.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// The owner of the boundary. The WebMethod does no business thinking of its own: it parses,
    /// then hands a validated request to this service, which decides. Named in the contract
    /// document under "Owner", so a reviewer knows exactly which class to read.
    /// </summary>
    public interface IClientCommandService
    {
        /// <summary>Runs a client-suggested command after re-checking tenant, target, state and permission.</summary>
        ClientCommandResult ExecuteFromClient(ClientCommandRequest request);

        /// <summary>The commands the current session may see. Still re-checked at execution time.</summary>
        IReadOnlyList<CommandDescriptor> GetVisibleCatalog(string query);
    }
}
