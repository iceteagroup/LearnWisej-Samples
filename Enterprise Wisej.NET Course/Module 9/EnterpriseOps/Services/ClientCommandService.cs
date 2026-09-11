using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;
using EnterpriseOps.Interop;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The owner of the interop boundary (the "Owner" row of docs/InteropContract.md).
    ///
    /// Everything the palette can trigger arrives here as a validated <see cref="ClientCommandRequest"/>
    /// and passes five gates before a single row is touched:
    ///
    ///   1. catalogue    — is this a command at all?            → UNKNOWN_COMMAND
    ///   2. arity        — does it need an entity, or not?      → MALFORMED_PAYLOAD
    ///   3. permission   — may THIS session run it?             → PERMISSION_DENIED
    ///   4. target       — does the id resolve IN THIS TENANT?  → INVALID_TARGET
    ///   5. state        — is the entity in a legal state?      → INVALID_STATE
    ///
    /// The order is deliberate: permission is checked BEFORE the target is resolved, so a user who
    /// may not approve cannot use the difference between INVALID_TARGET and INVALID_STATE to probe
    /// which work orders exist in another tenant. Every outcome, allowed or denied, is audited.
    /// </summary>
    public sealed class ClientCommandService : IClientCommandService
    {
        private readonly SessionContext _session;
        private readonly IPermissionService _permissions;
        private readonly WorkOrderService _workOrders;
        private readonly AuditLog _audit;
        private readonly ActivityTrace _trace;

        public ClientCommandService(
            SessionContext session,
            IPermissionService permissions,
            WorkOrderService workOrders,
            AuditLog audit,
            ActivityTrace trace)
        {
            _session = session;
            _permissions = permissions;
            _workOrders = workOrders;
            _audit = audit;
            _trace = trace;
        }

        /// <summary>The last result, so the page can show it without re-running the command.</summary>
        public ClientCommandResult LastResult { get; private set; }

        #region The contract path

        public ClientCommandResult ExecuteFromClient(ClientCommandRequest request)
        {
            // The context is built from the SESSION. The only thing taken from the payload is the
            // correlation id, and only so the browser log and the server log line up.
            CommandContext context = _session.NewCommand(request.CorrelationId);
            _trace.Interop($"ExecuteFromClient cmd={request.CommandName} entity={Display(request.EntityId)} corr={context.CorrelationId} · identity from session ({context.UserName}/{context.Role}/{context.TenantId})");

            // ---- gate 1: the command must exist in the published catalogue -----------------
            CommandDescriptor descriptor = InteropContract.Find(request.CommandName);
            if (descriptor == null)
            {
                _trace.Interop($"catalogue lookup '{request.CommandName}' → not found · no service called");
                return Deny(context, request, ResultCodes.UnknownCommand,
                    "That command does not exist.", AuditOutcome.Rejected,
                    $"command '{request.CommandName}' is not in the catalogue");
            }

            // ---- gate 2: arity — an entity command needs an entity, and vice versa ---------
            bool hasEntity = !string.IsNullOrEmpty(request.EntityId);
            if (descriptor.RequiresEntity && !hasEntity)
            {
                return Deny(context, request, ResultCodes.MalformedPayload,
                    $"{descriptor.Title} needs a work order.", AuditOutcome.Rejected,
                    "entityId is required for this command");
            }
            if (!descriptor.RequiresEntity && hasEntity)
            {
                return Deny(context, request, ResultCodes.MalformedPayload,
                    $"{descriptor.Title} does not take a work order.", AuditOutcome.Rejected,
                    "entityId was supplied for a command that takes none");
            }

            // ---- gate 3: permission, from the session role, BEFORE anything is looked up ---
            PermissionDecision decision = _permissions.Check(_session, descriptor.Permission);
            if (!decision.Allowed)
            {
                return Deny(context, request, ResultCodes.PermissionDenied,
                    $"You can't {descriptor.Title.TrimEnd('…').ToLowerInvariant()}.", AuditOutcome.Denied,
                    decision.Reason);
            }

            // ---- gate 4: the target must resolve inside the SESSION tenant -----------------
            WorkOrder order = null;
            if (descriptor.RequiresEntity)
            {
                if (!InteropContract.TryReadEntityKey(request.EntityId, out int key))
                    return Deny(context, request, ResultCodes.InvalidTarget, "That work order id is not readable.",
                        AuditOutcome.Rejected, "entityId could not be read as a key");

                order = _workOrders.Resolve(context.TenantId, key);
                if (order == null)
                {
                    // Deliberately the same answer for "does not exist" and "belongs to another
                    // tenant": the browser must not be able to enumerate other tenants' data.
                    return Deny(context, request, ResultCodes.InvalidTarget,
                        "That work order is not in your queue.", AuditOutcome.Rejected,
                        $"{request.EntityId} does not resolve in tenant {context.TenantId}");
                }
            }

            // ---- gate 5: the business rule decides -----------------------------------------
            CommandResult result = Run(descriptor, context, order);

            if (!result.Succeeded)
                return Deny(context, request, result.Code, result.Message, AuditOutcome.Rejected, result.Message);

            _audit.Record(context, descriptor.Id, request.EntityId, AuditOutcome.Allowed, result.Message);
            LastResult = ClientCommandResult.Ok(descriptor.Id, context.CorrelationId, result.Message);
            _trace.Interop($"result {LastResult.Code} · contract v{InteropContract.Version} → browser");
            return LastResult;
        }

        private CommandResult Run(CommandDescriptor descriptor, CommandContext context, WorkOrder order)
        {
            switch (descriptor.Id)
            {
                case "workorder.approve": return _workOrders.Approve(context, order);
                case "workorder.reassign": return _workOrders.Reassign(context, order);
                case "workorder.escalate": return _workOrders.Escalate(context, order);
                case "workqueue.open": return _workOrders.OpenWorkQueue(context);
                case "import.new": return _workOrders.StartImport(context);
                case "diagnostics.open": return _workOrders.OpenDiagnostics(context);
                default:
                    // Unreachable: gate 1 already proved the id is in the catalogue. Kept so that
                    // adding a catalogue entry without a handler fails loudly instead of silently.
                    _trace.Service($"no handler for catalogue entry '{descriptor.Id}'");
                    return CommandResult.Fail(context.CorrelationId, ResultCodes.ServerError, "That command is not wired up yet.");
            }
        }

        /// <summary>
        /// The catalogue the palette is allowed to render. Filtering here is a UX courtesy — a user
        /// who edits the client list still hits gate 3, which is why the failure path in the video
        /// is a denial and not a broken screen.
        /// </summary>
        public IReadOnlyList<CommandDescriptor> GetVisibleCatalog(string query)
        {
            string needle = (query ?? "").Trim().ToLowerInvariant();
            if (needle.Length > 60) needle = needle.Substring(0, 60);

            List<CommandDescriptor> visible = InteropContract.Catalog
                .Where(c => needle.Length == 0
                            || c.Title.ToLowerInvariant().Contains(needle)
                            || c.Id.Contains(needle))
                .ToList();

            _trace.Interop($"GetCommandCatalog(query=\"{needle}\") → {visible.Count} of {InteropContract.Catalog.Count} commands (display filter only)");
            return visible;
        }

        /// <summary>Which catalogue entries this session may actually run — used by the panel's badges.</summary>
        public bool MayRun(CommandDescriptor descriptor)
            => PermissionService.PermissionsOf(_session.Role).Contains(descriptor.Permission);

        #endregion

        private ClientCommandResult Deny(CommandContext context, ClientCommandRequest request, string code, string message,
                                         AuditOutcome outcome, string detail)
        {
            _audit.Record(context, request.CommandName, request.EntityId, outcome, $"{code}: {detail}");
            LastResult = ClientCommandResult.Fail(request.CommandName, context.CorrelationId, code, message);
            _trace.Interop($"result {code} · the command never ran · message to browser: \"{message}\"");
            return LastResult;
        }

        private static string Display(string entityId) => string.IsNullOrEmpty(entityId) ? "—" : entityId;
    }
}
